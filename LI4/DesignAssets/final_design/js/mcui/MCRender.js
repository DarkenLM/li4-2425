/** @import { Structure } from "./MCStruct.js" */
import * as THREE from "three";
import RotationMatrix from "../three/RotationMatrix.js";
import { flipUVBox, offsetUVBox, convertUVs, rotateUVs } from "../three/uvUtil.js";
import MCStruct, * as mcs from "./MCStruct.js";
import BezierEasing from "../three/BezierEasing.js";
import { fileExists } from "../util/fetchUtil.js";

const BLOCK_GRID_SIZE = 16;
const TEXTURE_SIZE = 16;

/**
 * The order by which THREE.js defines the vertices of a BoxGeometry.
 * 
 * @readonly
 */
const schemaFaceOrder = /** @type {const} */ (["west", "east", "up", "down", "north", "south"]);

let lastTick = 0;
class MCRender {
    /** 
     * The URI to the model library to be used for loading models.
     * @type {URL} 
     */
    modelLibraryPath;

    /** 
     * The URI to the texture library to be used for loading textures.
     * @type {URL} 
     */
    textureLibraryPath;

    // /** @type {Record<string, typeof THREE.Texture>} */
    /** @type {Record<string, { texture: typeof THREE.Texture, transparent: boolean }>} */
    textures;

    /** @type {Record<string, _Model>} */
    models;

    /** @type {_StructureData | undefined} */
    _structureData;

    constructor({
        modelLibraryPath = new URL(""),
        textureLibraryPath = new URL(""),
    }) {
        this.modelLibraryPath = modelLibraryPath;
        this.textureLibraryPath = textureLibraryPath;
        this.textures = {};
        this.models = {};
        this._structureData = undefined;

        this._textureLoader = new THREE.TextureLoader();
        this._meshes = {};
        this._mocks = {};
        this._structureGroup = undefined;

        this.__currentTick = 0;
        this._currentTick = 0;
        this._totalTicks = 0;
        this._ticksPerSecond = 100;
        // this._ticksPerBlock = this._ticksPerSecond;
        this._ticksPerBlock = 40;
        this._stageGapTicks = 2 * this._ticksPerSecond;
        this._stageTicks = []
        this._blockDistance = 1000;
        this._blockStep = this._blockDistance / this._ticksPerBlock;

        // this._easeFunction = this._easeInOutCubic;
        // this._easeFunction = BezierEasing(0.750, 0.250, 0.250, 0.750);
        // this._easeFunction = BezierEasing(0.750, 0.250, 0.062, 1.077); // Bouncy
        this._easeFunction = BezierEasing(0.750, 0.250, 0.098, 1);
    }

    //#region ============== Tick Operations ==============
    setTick(tick) {
        this.__currentTick = (tick % this._totalTicks);
        this._currentTick = tick;
    }

    tick(n = 1) {
        this.__currentTick += n;
        this._currentTick = this._bounceValueBetween(this.__currentTick, 0, this._totalTicks);
    }
    //#endregion ============== Tick Operations ==============


    //#region ============== Model Operations ==============
    /**
     * Gets a model from the model library or loads it if it doesn't exist.
     * 
     * @param {string} name The name of the model to get.
     * @returns {Promise<_Model>} The model object.
     * @throws {Error} If the model does not exist in the library.
     */
    async getOrLoadModel(name) {
        if (name in this.models) {
            return this.models[name];
        } else {
            await this.loadModelFromLibrary(name);
            return this.models[name];
        }
    }

    /**
     * Loads a model from the model library.
     * 
     * @param {string} name The name of the model to load.
     * @throws {Error} If the model does not exist in the library.
     */
    async loadModelFromLibrary(name) {
        const url = new URL(`${name}.json`, this.modelLibraryPath);
        const fileData = await fetch(url);
        const fileContent = await fileData.json();

        await this.loadModel(fileContent, name);
    }

    /**
     * Loads a model from a given {@link Model} object.
     * 
     * @param {Model} model 
     * @param {string} name
     * @returns 
     */
    async loadModel(model, name) {
        console.log("LOADING MODEL:", name, model);
        const mesh = await this.createModelMesh(model);
        this.models[name] = { schema: model, mesh };
    }

    /**
     * Creates a new {@link ModelMesh|Mesh} from a given {@link Model}.
     * 
     * @param {Model} model The model to create the geometry from.
     * @returns {ModelMesh} The mesh created from the model.
     */
    async createModelMesh(model) {
        /** @type {THREE.Material[]}*/
        const materials = [];
        /** @type {Record<string, string>} */
        const textureIndexMap = {};
        
        // For each texture, load it if it hasn't been loaded yet.
        // Object.entries(model.textures).forEach(([k,v]) => {
        //     if (!(v in this.textures)) {
        //         this.textures[v] = this._textureLoader.load(v);
        //         this.textures[v].magFilter = THREE.NearestFilter;
        //         this.textures[v].colorSpace = THREE.SRGBColorSpace;
                
        //         const metaFile = v.replace(".png", ".json");
        //         const metaURL = new URL(metaFile, this.modelLibraryPath);
        //         fetch(metaURL)
        //             .then(res => res.json())
        //             .then(meta => {
        //                 this.textures[v].wrapS = meta.wrap;
        //                 this.textures[v].wrapT = meta.wrap;
        //             });
        //     }
        // });
        for (const [k,v] of Object.entries(model.textures)) {
            if (!(v in this.textures)) {
                // this.textures[v] = this._textureLoader.load(v);
                // this.textures[v].magFilter = THREE.NearestFilter;
                // this.textures[v].colorSpace = THREE.SRGBColorSpace;

                console.log("LOADING TEXTURE:", k, v);

                this.textures[v] = {
                    texture: undefined,
                    transparent: false,
                };
                // this.textures[v].texture = this._textureLoader.load(v);
                this.textures[v].texture = this._textureLoader.load(new URL(v, this.textureLibraryPath));
                this.textures[v].texture.magFilter = THREE.NearestFilter;
                this.textures[v].texture.colorSpace = THREE.SRGBColorSpace;
                
                try {
                    const metaFile = v.split("/").at(-1).replace(".png", ".json");
                    const metaURL = new URL(metaFile, this.textureLibraryPath);

                    console.log("ATTEMPTING TO LOAD META FROM:", this.textureLibraryPath);

                    if (!(await fileExists(metaURL))) continue;
                    console.log("METAFILE EXISTS:", metaURL);

                    const metaFileData = await fetch(metaURL);
                    const meta = await metaFileData.json();

                    if (meta.transparent) {
                        this.textures[v].transparent = true;
                    }
                } catch (_e) {
                    // Ignore
                }
            }
        }
    
        // Create a single BufferGeometry for the block
        const geometry = new THREE.BufferGeometry();
        const positions = [];
        const normals = [];
        const uvs = [];
        const indices = [];
        let indexOffset = 0;
    
        // console.log("SCHEMA:", schema, "=======================================");
    
        // Iterate through elements and create geometry for each
        model.elements.forEach((element) => {
            const { from, to, faces, rotation } = element;
    
            const width =  to[0] - from[0];
            const height = to[1] - from[1];
            const depth =  to[2] - from[2];
    
            // console.log("ELEMENT:", element);
            // console.log("FROM:", from, "TO:", to, "WIDTH:", width, "HEIGHT:", height, "DEPTH:", depth);
    
            // Create a rotation matrix if rotation for the element is defined
            let rotationMatrix = new THREE.Matrix4();
            let rotated = false;
            if (rotation) {
                // console.log("ROTATION:", rotation, "FOR:", element);
                
                if (rotation.angle !== 0) {
                    const angle = THREE.MathUtils.degToRad(rotation.angle);
                    const rM = new RotationMatrix(
                        rotation.origin[0], 
                        rotation.origin[1], 
                        rotation.origin[2], 
                        rotation.axis === "x" ? 1 : 0,
                        rotation.axis === "y" ? 1 : 0,
                        rotation.axis === "z" ? 1 : 0,
                        angle
                    );
    
                    rotationMatrix = rM;
                    rotated = true;
                } else {
                    // console.log("NO ROTATION");
                }
            }
    
            // Iterate through faces and create geometry for each
            Object.entries(faces).forEach(([direction, { uv, texture, rotation: faceRotation }]) => {
                // const textureSize = 16;
                const oUV = flipUVBox(offsetUVBox(uv, 0, 0), 0, TEXTURE_SIZE);
    
                let plane;
                let faceNormal;
    
                // Define position and normals for the face
                switch (direction) {
                    case "north": // +Z
                        plane = [
                            [0, 0, depth], [width, 0, depth], [width, height, depth], [0, height, depth]
                        ];
                        faceNormal = [0, 0, 1];
                        break;
                    case "south": // -Z
                        plane = [
                            [width, 0, 0], [0, 0, 0], [0, height, 0], [width, height, 0]
                        ];
                        faceNormal = [0, 0, -1];
                        break;
                    case "east": // +X
                        plane = [
                            [width, 0, depth], [width, 0, 0], [width, height, 0], [width, height, depth]
                        ];
                        faceNormal = [1, 0, 0];
                        break;
                    case "west": // -X
                        plane = [
                            [0, 0, 0], [0, 0, depth], [0, height, depth], [0, height, 0]
                        ];
                        faceNormal = [-1, 0, 0];
                        break;
                    case "up": // +Y
                        plane = [
                            [0, height, depth], [width, height, depth], [width, height, 0], [0, height, 0]
                        ];
                        faceNormal = [0, 1, 0];
                        break;
                    case "down": // -Y
                        plane = [
                            [0, 0, 0], [width, 0, 0], [width, 0, depth], [0, 0, depth]
                        ];
                        faceNormal = [0, -1, 0];
                        break;
                }
    
                // Add vertices, normals, and UVs
                const startIdx = indexOffset;
                plane.forEach(([x, y, z]) => {
                    const vertex = new THREE.Vector3(
                        x + from[0],
                        y + from[1],
                        z + from[2]
                    );
                    
                    // Rotate the vertex if the element rotation is defined
                    if (rotation && rotated) {
                        const p = rotationMatrix.timesXYZ(vertex.x, vertex.y, vertex.z);
                        vertex.set(p[0], p[1], p[2]);
                    }
                    
                    // Normalize the vertex position relative to the center of the block
                    vertex.sub(new THREE.Vector3(
                        BLOCK_GRID_SIZE / 2,
                        BLOCK_GRID_SIZE / 2,
                        BLOCK_GRID_SIZE / 2
                    ));
                    
                    positions.push(vertex.x, vertex.y, vertex.z);
                    normals.push(...faceNormal);
                });
    
                // Add UVs
                const faceUVs = convertUVs(oUV, TEXTURE_SIZE, TEXTURE_SIZE);
                if (faceRotation) {
                    // Rotate the UVs if the face rotation is defined
                    rotateUVs(faceUVs, THREE.MathUtils.degToRad(faceRotation));
                }
                uvs.push(...faceUVs);
    
                // Add face indices
                indices.push(
                    startIdx, startIdx + 1, startIdx + 2,
                    startIdx, startIdx + 2, startIdx + 3
                );
                indexOffset += 4;
    
                // Handle materials
                if (!(texture in textureIndexMap)) {
                    const materialIndex = materials.length;
                    textureIndexMap[texture] = materialIndex;

                    console.log("TEXTURE:", texture, model.textures[texture], this.textures[model.textures[texture]]);

                    const tex = this.textures[model.textures[texture]]?.texture;
                    let mat;
                    if (tex) {
                        mat = new THREE.MeshBasicMaterial({
                            map: tex,
                            side: THREE.FrontSide
                        });
                    } else {
                        mat = new THREE.MeshBasicMaterial({
                            color: 0xffffff,
                            side: THREE.FrontSide
                        });
                    }

                    // const mat = new THREE.MeshBasicMaterial({
                    //     // map: this.textures[model.textures[texture]].texture,
                    //     map: tex,
                    //     side: THREE.DoubleSide
                    // });
                    if (this.textures[model.textures[texture]]?.transparent) {
                        mat.transparent = true;
                    }

                    // const mat = new THREE.MeshBasicMaterial({
                    //     map: this.textures[model.textures[texture]]?.texture,
                    //     side: THREE.DoubleSide
                    // });

                    materials.push(mat);
                }
                const materialIndex = textureIndexMap[texture];
    
                // Add group for the face
                geometry.addGroup(indices.length - 6, 6, materialIndex);
            });
        });
    
        // Build the BufferGeometry
        geometry.setAttribute("position", new THREE.Float32BufferAttribute(positions, 3));
        geometry.setAttribute("normal", new THREE.Float32BufferAttribute(normals, 3));
        geometry.setAttribute("uv", new THREE.Float32BufferAttribute(uvs, 2));
        geometry.setIndex(indices);
    
        // console.log("MATERIALS:", materials);
    
        return { geometry, materials };
    }
    //#endregion ============== Model Operations =============

    //#region ============== Scene Operations ==============
    /**
     * Loads a structure into the scene.
     * 
     * @param {MCStruct} struct 
     * @throws {Error} If the structure could not be loaded. The state of the scene may be inconsistent at this point.
     */
    async loadStructure(struct) {
        const blockList = struct.getBlockList({ blockCount: true });
        console.log("BLOCK LIST:", blockList);

        // Load all models
        for (const block of blockList) {
            await this.getOrLoadModel(block.block);
        }

        this._structureData = { structure: struct, blockList };
    }

    prepare(scene) {
        if (!this._structureData) {
            throw new Error("No structure has been loaded.");
        }

        // Calculate ticks
        const stages = this._structureData.structure.getStageCount();
        for (let i = 0; i < stages; i++) {
            const stage = this._structureData.structure.getStage(i);
            const stageTicks = stage.blocks.length * this._ticksPerBlock;
            this._stageTicks.push(stageTicks);
            this._totalTicks += stageTicks;
        }

        // Clear and initialize the meshes
        this._meshes = {};
        for (const block of this._structureData.blockList) {
            const model = this.models[block.block];
            if (model) {
                const blockGeometry = new THREE.InstancedMesh(model.mesh.geometry, model.mesh.materials, block.count);
                blockGeometry.instanceMatrix.setUsage(THREE.DynamicDrawUsage);
                this._meshes[block.block] = blockGeometry;

                console.log("BLOCKMESH GENERATED:", block.block, block.count);
            }
        }

        // Create a group for the structure
        this._structureGroup = new THREE.Group();
        this._structureGroup.name = `MCRender:${this._structureData.structure.getName()}`;

        console.log("STRUCTURE:", this._structureData.structure);
        console.log("STAGE 0:", this._structureData.structure.getStage(2));

        const renderMeshes = [];

        // for (const stage of this._structureData.structure.getStages()) {
        for (let i = 0; i < this._structureData.structure.getStageCount(); i++) {
            const stage = this._structureData.structure.getStage(i);
            const stageTicks = this._getAccumulatedStageTick(i);

            // for (const _block in stage.blocks) {
            for (let _block = 0; _block < stage.blocks.length; _block++) {
                const block = stage.blocks[_block];
                const blockMesh = this._meshes[block.block];

                if (!blockMesh) continue;

                let blockMock;
                if (block.block in this._mocks) {
                    blockMock = this._mocks[block.block];
                } else {
                    blockMock = { mock: new THREE.Object3D(), index: 0 };
                    this._mocks[block.block] = blockMock;
                }

                console.log("BLOCK #" + _block + ":", block);
                // console.log("BLOCKMESH:", blockMesh);
                
                blockMock.mock.position.set(block.x * BLOCK_GRID_SIZE, block.y * BLOCK_GRID_SIZE + this._blockDistance, block.z * BLOCK_GRID_SIZE);
                this._handleBlockRotation(blockMock.mock, block);
                this._handleBlockProgressScale(blockMock.mock, 0.0);

                blockMock.mock.visible = false;

                blockMock.mock.updateMatrix();
                blockMesh.setMatrixAt(blockMock.index, blockMock.mock.matrix);
                blockMock.index++;

                
                if (!renderMeshes.includes(blockMesh)) renderMeshes.push(blockMesh);
            }
        }

        // Add the meshes to the group
        for (const mesh of renderMeshes) {
            console.log("MESH:", mesh);
            this._structureGroup.add(mesh);
        }

        // Add the group to the scene
        scene.add(this._structureGroup);
    }
    
    update(time) {
        if (!this._structureData) {
            throw new Error("No structure has been loaded.");
        }

        console.log("CURRENT TICK:", this._currentTick, "/", this._totalTicks);
        if (this._currentTick === lastTick) return;
        lastTick = this._currentTick;



        const alteredMeshes = [];

        // Reset the index for each block mock
        for (const mock in this._mocks) {
            this._mocks[mock].index = 0;
        }

        for (let stageIndex = 0; stageIndex < this._structureData.structure.getStageCount(); stageIndex++) {
            const stage = this._structureData.structure.getStage(stageIndex);
            const stageStartTick = this._getAccumulatedStageTick(stageIndex);
    
            // Calculate stage-specific tick
            const stageTick = this._currentTick - stageStartTick;
            console.log("STAGE:", stageIndex, "TICK:", stageTick, "STAGE START:", stageStartTick);
    
            // Skip the stage if it hasn't started yet
            if (this._currentTick < stageStartTick) {
                console.log("SKIPPING STAGE:", stageIndex);
                for (let blockIndex = 0; blockIndex < stage.blocks.length; blockIndex++) {
                    const block = stage.blocks[blockIndex];
                    const blockMesh = this._meshes[block.block];
                    const blockMock = this._mocks[block.block];

                    blockMock.mock.position.set(block.x * BLOCK_GRID_SIZE, block.y * BLOCK_GRID_SIZE + this._blockDistance, block.z * BLOCK_GRID_SIZE);
                    this._handleBlockRotation(blockMock.mock, block);
                    this._handleBlockProgressScale(blockMock.mock, 0);

                    blockMock.mock.visible = false;

                    blockMock.mock.updateMatrix();
                    blockMesh.setMatrixAt(blockMock.index, blockMock.mock.matrix);
                    blockMock.index++;
    
                    if (!alteredMeshes.includes(blockMesh)) alteredMeshes.push(blockMesh);
                }
                continue;
            }

            console.log("STAGE:", stageIndex, "TICK:", stageTick, "STAGE TICKS:", this._stageTicks[stageIndex]);
    
            // Skip the stage if it's in the gap period
            if (stageTick >= this._stageTicks[stageIndex]) {
                console.log("GAP STAGE:", stageIndex);
                for (let blockIndex = 0; blockIndex < stage.blocks.length; blockIndex++) {
                    const block = stage.blocks[blockIndex];
                    const blockMesh = this._meshes[block.block];
                    const blockMock = this._mocks[block.block];
                    
                    blockMock.mock.position.set(block.x * BLOCK_GRID_SIZE, block.y * BLOCK_GRID_SIZE, block.z * BLOCK_GRID_SIZE);
                    this._handleBlockRotation(blockMock.mock, block);
                    this._handleBlockProgressScale(blockMock.mock, 1.0);

                    blockMock.mock.visible = false;

                    blockMock.mock.updateMatrix();
                    blockMesh.setMatrixAt(blockMock.index, blockMock.mock.matrix);
                    blockMock.index++;
    
                    if (!alteredMeshes.includes(blockMesh)) alteredMeshes.push(blockMesh);
                }
                continue;
            }

            // console.log("COMPUTING STAGE:", stageIndex);
    
            for (let blockIndex = 0; blockIndex < stage.blocks.length; blockIndex++) {
                const block = stage.blocks[blockIndex];
                const blockMesh = this._meshes[block.block];
                const blockMock = this._mocks[block.block];
    
                const blockStartTick = blockIndex * this._ticksPerBlock;
                const blockEndTick = blockStartTick + this._ticksPerBlock;
    
                // Check if the block should be falling
                if (stageTick < blockStartTick) {
                    this._handleBlockProgressScale(blockMock.mock, 0.0);
                    continue;
                }
                else if (stageTick >= blockEndTick) {
                    blockMock.mock.position.set(block.x * BLOCK_GRID_SIZE, block.y * BLOCK_GRID_SIZE, block.z * BLOCK_GRID_SIZE);
                    this._handleBlockRotation(blockMock.mock, block);
                    this._handleBlockProgressScale(blockMock.mock, 1.0);

                    blockMock.mock.visible = true;

                    blockMock.mock.updateMatrix();
                    blockMesh.setMatrixAt(blockMock.index, blockMock.mock.matrix);
                    blockMock.index++;
                    continue;
                }
    
                // Compute animation progress (0.0 to 1.0)
                const linearProgress = (stageTick - blockStartTick) / this._ticksPerBlock;
                const easedProgress = this._easeFunction(linearProgress);

    
                // Calculate block's falling position
                const x = block.x * BLOCK_GRID_SIZE;
                const y = block.y * BLOCK_GRID_SIZE + (this._blockDistance - easedProgress * this._blockDistance);
                const z = block.z * BLOCK_GRID_SIZE;
    
                blockMock.mock.position.set(x, y, z);
                this._handleBlockRotation(blockMock.mock, block);
                this._handleBlockProgressScale(blockMock.mock, 1.0);

                blockMock.mock.updateMatrix();
                blockMesh.setMatrixAt(blockMock.index, blockMock.mock.matrix);
                blockMock.index++;
    
                if (!alteredMeshes.includes(blockMesh)) alteredMeshes.push(blockMesh);
            }
        }

        for (const mesh of alteredMeshes) {
            mesh.instanceMatrix.needsUpdate = true;
            mesh.computeBoundingSphere();
        }
    }

    /**
     * 
     * @param {THREE.Scene} scene 
     */
    clear(scene, { clearMaterials = false } = {}) {
        if (this._structureGroup) {
            scene.remove(this._structureGroup);
            this._structureGroup = undefined;

            // Clear meshes
            for (const mesh of Object.values(this._meshes)) {
                if (mesh.geometry) mesh.geometry.dispose();
                if (clearMaterials) {
                    if (mesh.material instanceof Array) {
                        mesh.material.forEach(material => material.dispose());
                    } else {
                        mesh.material.dispose();
                    }
                }

                mesh.dispose();
            }
            this.meshes = {};

            // Clear mocks
            this._mocks = {};
        }

        this.__currentTick = 0;
        this._currentTick = 0;
        this._totalTicks = 0;
        this._stageTicks = [];
    }

    dispose() {
        for (const tex of Object.values(this.textures)) {
            tex.texture.dispose();
        }
        this.textures = {};

        for (const model of Object.values(this.models)) {
            model.mesh.geometry.dispose();
            
            if (model.mesh.materials instanceof Array) {
                model.mesh.materials.forEach(material => material.dispose());
            } else {
                model.mesh.materials.dispose();
            }
        }
        this.models = {};
    }

    _debugTextures(scene, x = 0, y = -20, z = 0) {
        const texs = Object.entries(this.textures);
        for (let i = 0; i < texs.length; i++) {
            const [key, value] = texs[i];
            const plane = new THREE.Mesh(
                new THREE.PlaneGeometry(10, 10).translate(x + 10 * i, 0, 5),
                new THREE.MeshBasicMaterial({ map: value.texture, transparent: value.transparent })
            );
            // plane.position.set(0, -20, 0);
            plane.position.set(x, y, z);
            scene.add(plane);
        }
    }

    /**
     * Gets the number of ticks until the given stage index.
     * 
     * @param {number} stageIndex 
     * @returns {number}
     */
    _getAccumulatedStageTick(stageIndex) {
        return this._stageTicks.slice(0, stageIndex).reduce((acc, val) => acc + val, 0);
    }

    _bounceValueBetween(value, lowerBound, upperBound) {
        const range_size = upperBound - lowerBound;
        // return lowerBound + Math.abs((value - lowerBound) % (2 * range_size) - range_size);
        return upperBound - (lowerBound + Math.abs((value % (2 * range_size)) - range_size));
    }

    _handleBlockProgressScale(mock, progress) {
        mock.scale.set(progress, progress, progress);
    }

    _handleBlockRotation(mock, block) {
        mock.rotation.set(0, 0, 0);

        if (block.rotation) {
            if (block.rotation.x) mock.rotation.x = THREE.MathUtils.degToRad(parseFloat(block.rotation.x));
            if (block.rotation.y) mock.rotation.y = THREE.MathUtils.degToRad(parseFloat(block.rotation.y));
            if (block.rotation.z) mock.rotation.z = THREE.MathUtils.degToRad(parseFloat(block.rotation.z));
        }
    }

    _easeInOutCubic(t) {
        return t < 0.5 ? 4 * t * t * t : 1 - Math.pow(-2 * t + 2, 3) / 2;
    }

    _easeInOutQuint(t) {
        return t < 0.5 ? 16 * t * t * t * t * t : 1 - Math.pow(-2 * t + 2, 5) / 2;
    }
    //#endregion ============== Scene Operations ==============
}

export default MCRender;

/**
 * @typedef {Object} Model
 * @property {Record<string, string>} textures
 * @property {Element[]} elements
 */

/**
 * @typedef {Object} ModelMesh
 * @property {THREE.BufferGeometry} geometry
 * @property {THREE.Material[]} materials
 */

/**
 * @typedef {Object} Element
 * @property {[number, number, number, number]} from
 * @property {[number, number, number, number]} to
 * @property {Record<FaceDirection, Face>} faces
 * @property {Rotation} rotation
 */

/**
 * @typedef {"west" | "east" | "up" | "down" | "north" | "south"} FaceDirection
 */

/**
 * @typedef {Object} Face
 * @property {[number, number, number, number]} uv
 * @property {string} texture
 * @property {number?} rotation
 */



/**
 * @typedef {Object} _Model
 * @property {Model} schema
 * @property {ModelMesh} mesh
 */

/**
 * @typedef {Object} _StructureData
 * @property {MCStruct} structure
 * @property {Set<{block: string, count: number}>} blockList
 */
