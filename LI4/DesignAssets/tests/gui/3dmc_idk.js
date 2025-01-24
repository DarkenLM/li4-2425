// Import Three.js
import * as THREE from "three";
import { OrbitControls } from "three/addons/controls/OrbitControls.js";
import Stats from "three/addons/libs/stats.module.js";
import RotationMatrix from "./three/RotationMatrix.js";

import oakFenceModule from "./mcstruct/blocks/oak_fence.json" with { type: "json" };
import lecternModule from "./mcstruct/blocks/lectern.json" with { type: "json" };
import oakPlanksModule from "./mcstruct/blocks/oak_planks.json" with { type: "json" };
import whiteWoolModule from "./mcstruct/blocks/white_wool.json" with { type: "json" };
import craftingTableModule from "./mcstruct/blocks/crafting_table.json" with { type: "json" };
import cobblestoneModule from "./mcstruct/blocks/cobblestone.json" with { type: "json" };
import oakLogModule from "./mcstruct/blocks/oak_log.json" with { type: "json" };
import oakStairsModule from "./mcstruct/blocks/oak_stairs.json" with { type: "json" };
import glassPaneModule from "./mcstruct/blocks/glass_pane.json" with { type: "json" };
import oakDoorModule from "./mcstruct/blocks/oak_door.json" with { type: "json" };
import yellowBedModule from "./mcstruct/blocks/yellow_bed.json" with { type: "json" };
import bookshelfModule from "./mcstruct/blocks/bookshelf.json" with { type: "json" };
import grassBlockModule from "./mcstruct/blocks/grass_block.json" with { type: "json" };

import { flipUVBox, offsetUVBox, rotateUVs } from "./three/uvUtil.js";

const BLOCK_GRID_SIZE = 16;

const NORMALS = [
    0,  0,  1,
    0,  0,  1,
    0,  0,  1,
    0,  0,  1,
    1,  0,  0,
    1,  0,  0,
    1,  0,  0,
    1,  0,  0,
    0,  0, -1,
    0,  0, -1,
    0,  0, -1,
    0,  0, -1,
    1,  0,  0,
    1,  0,  0,
    1,  0,  0,
    1,  0,  0,
    0,  1,  0,
    0,  1,  0,
    0,  1,  0,
    0,  1,  0,
    0, -1,  0,
    0, -1,  0,
    0, -1,  0,
    0, -1,  0
];

function* chunks(array, n){
    for(let i = 0; i < array.length; i += n) yield array.slice(i, i + n);
}

// Load textures
const textureLoader = new THREE.TextureLoader();
/** @type {Record<string, typeof THREE.Texture>} */
const textures = {};
// Object.entries(oakFenceModule.textures).forEach(([k,v]) => {
//     textures[k] = textureLoader.load(v);
//     textures[k].magFilter = THREE.NearestFilter;
//     textures[k].colorSpace = THREE.SRGBColorSpace;
// });

console.log("TEXTURES:", textures);

// function offsetUVBox(uvBox, offsetU, offsetV) {
//     return uvBox.map((v, i) => (i % 2 === 0) ? v + offsetU : v + offsetV);
// }

// /**
//  * 
//  * @param {number[]} uvBox 
//  * @param {number} uAxis 
//  * @param {number} vAxis 
//  * @returns 
//  */
// function flipUVBox(uvBox, uAxis, vAxis) {
//     return uvBox.map((v, i) => {
//         if (i % 2 === 0) {
//             if (uAxis) return uAxis - v;
//             return v;
//         } else {
//             if (vAxis) return vAxis - v;
//             return v;
//         }
//     });
// }

// function rotateUVs(uvArray, angle, center = [0.5, 0.5]) {
//     const cosTheta = Math.cos(angle);
//     const sinTheta = Math.sin(angle);
//     const [cx, cy] = center;

//     console.log("ROTATING UV:", uvArray, "BY:", angle, "AROUND:", center);

//     for (let i = 0; i < uvArray.length; i += 2) {
//         let u = uvArray[i] - cx;
//         let v = uvArray[i + 1] - cy;

//         // Apply 2D rotation
//         const uRotated = u * cosTheta - v * sinTheta;
//         const vRotated = u * sinTheta + v * cosTheta;

//         // Translate back and store
//         uvArray[i] = uRotated + cx;
//         uvArray[i + 1] = vRotated + cy;
//     }

//     console.log("NEW UV:", uvArray);
// }

function convertUVs(uvArray, uvBox, width, height) {
    const [u1, v1, u2, v2] = uvBox;

    return [
        u1 / width, v2 / height, // Bottom-left
        u2 / width, v2 / height, // Bottom-right
        u2 / width, v1 / height, // Top-right
        u1 / width, v1 / height, // Top-left
    ]
}

function createBlockGeometry(schema) {
    // const group = new THREE.Group(); // Group to hold all meshes
    const materials = []; // Array to hold materials
    const textureIndexMap = {}; // Map textures to indices for reuse

    Object.entries(schema.textures).forEach(([k,v]) => {
        if (!(v in textures)) {
            textures[v] = textureLoader.load(v);
            textures[v].magFilter = THREE.NearestFilter;
            textures[v].colorSpace = THREE.SRGBColorSpace;
        }
    });

    // Create a single BufferGeometry for the block
    const geometry = new THREE.BufferGeometry();
    const positions = [];
    const normals = [];
    const uvs = [];
    const indices = [];
    let indexOffset = 0;

    console.log("SCHEMA:", schema, "=======================================");

    schema.elements.forEach((element, i) => {
        const { from, to, faces, rotation } = element;

        const width =  to[0] - from[0];
        const height = to[1] - from[1];
        const depth =  to[2] - from[2];

        console.log("ELEMENT:", element);
        console.log("FROM:", from, "TO:", to, "WIDTH:", width, "HEIGHT:", height, "DEPTH:", depth);

        // Create a rotation matrix if rotation is defined
        let rotationMatrix = new THREE.Matrix4();
        let rotated = false;
        if (rotation) {
            console.log("ROTATION:", rotation, "FOR:", element);
            
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
                console.log("NO ROTATION");
            }
        }

        // Iterate through faces and create geometry for each
        Object.entries(faces).forEach(([direction, { uv, texture, rotation: faceRotation }], ii) => {
            const textureSize = 16;
            const oUV = flipUVBox(offsetUVBox(uv, 0, 0), 0, textureSize);

            let plane;
            let faceNormal;

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
                    // x + from[0] - BLOCK_GRID_SIZE / 2,
                    // y + from[1] - BLOCK_GRID_SIZE / 2,
                    // z + from[2] - BLOCK_GRID_SIZE / 2
                    x + from[0],
                    y + from[1],
                    z + from[2]
                );

                if (rotation && rotated) {
                    const p = rotationMatrix.timesXYZ(vertex.x, vertex.y, vertex.z);
                    vertex.set(p[0], p[1], p[2]);
                }

                vertex.sub(new THREE.Vector3(
                    BLOCK_GRID_SIZE / 2,
                    BLOCK_GRID_SIZE / 2,
                    BLOCK_GRID_SIZE / 2
                ));
                
                positions.push(vertex.x, vertex.y, vertex.z);
                normals.push(...faceNormal);
            });

            // addFaceUV(uvs, oUV, textureSize, textureSize);
            const faceUVs = convertUVs(uvs, oUV, textureSize, textureSize);
            if (faceRotation) {
                rotateUVs(faceUVs, THREE.MathUtils.degToRad(faceRotation));
            }
            uvs.push(...faceUVs);

            // Add face indices
            indices.push(
                startIdx, startIdx + 1, startIdx + 2,
                startIdx, startIdx + 2, startIdx + 3
            );
            indexOffset += 4;

            // Handle material
            if (!(texture in textureIndexMap)) {
                const materialIndex = materials.length;
                textureIndexMap[texture] = materialIndex;
                materials.push(
                    new THREE.MeshBasicMaterial({
                        map: textures[schema.textures[texture]],
                        side: THREE.DoubleSide
                    })
                );
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

    console.log("MATERIALS:", materials);
    console.log("")

    // Create a single mesh with multiple materials
    // const mesh = new THREE.InstancedMesh(geometry, materials);
    // mesh.instanceMatrix.setUsage(THREE.DynamicDrawUsage);
    // group.add(mesh);

    // return group;
    return { geometry, materials };
}







// Initialize scene, camera, and renderer
const scene = new THREE.Scene();

// Renderer setup
const renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.setSize(window.innerWidth, window.innerHeight);
// renderer.shadowMap.enabled = true; // Enable shadow rendering
// renderer.shadowMap.type = THREE.PCFSoftShadowMap; // Soft shadows
renderer.outputColorSpace = THREE.SRGBColorSpace;
document.body.appendChild(renderer.domElement);

const stats = new Stats();
document.body.appendChild(stats.dom);

// Orthographic camera for isometric perspective
const aspect = window.innerWidth / window.innerHeight;
const cameraSize = 20;
const camera = new THREE.OrthographicCamera(
    -cameraSize * aspect,
    cameraSize * aspect,
    cameraSize,
    -cameraSize,
    0.1,
    1000
);
const controls = new OrbitControls( camera, renderer.domElement );

// camera.position.set(5, 5, 10);
camera.position.set(-10, 5, -5);
camera.lookAt(0, 0, 0);
controls.update();

// Create a basic light
const light = new THREE.AmbientLight(0xffffff, 1); // Soft white light
scene.add(light);

// Parse schema and add blocks
function addBlocks(module, count) {
    const _blockGeometry = createBlockGeometry(module);
    const blockGeometry = new THREE.InstancedMesh(_blockGeometry.geometry, _blockGeometry.materials, count);
    blockGeometry.instanceMatrix.setUsage(THREE.DynamicDrawUsage);

    // scene.add(blockGeometry);

    const mock = new THREE.Object3D();

    for (let i = 0; i < count; i++) {
        const x = i * 10;
        const y = 1;
        const z = 1;

        mock.position.set(x, y, z);
        mock.updateMatrix();

        blockGeometry.setMatrixAt(i, mock.matrix);
    }

    scene.add(blockGeometry);

    return { blockGeometry, mock };
}

const count = 100;
const { blockGeometry: ctBG, mock: ctM } = addBlocks(craftingTableModule, count);


const texs = Object.entries(textures);
for (let i = 0; i < texs.length; i++) {
    const [key, value] = texs[i];
    const plane = new THREE.Mesh(
        new THREE.PlaneGeometry(10, 10).translate(10 * i, 0, 5),
        new THREE.MeshBasicMaterial({ map: value })
    );
    plane.position.set(0, -20, 0);
    scene.add(plane);
}

function updateBlocks(time, blockGeometry, mock, count, zValue, each) {
    for (let i = 0; i < count; i++) {
        const x = i * 20;
        const y = Math.sin(time + i + (zValue / 5)) * 10;
        const z = zValue ?? 1;

        mock.position.set(x, y, z);
        if (each) each(mock, i);

        mock.updateMatrix();
        blockGeometry.setMatrixAt(i, mock.matrix);
    }

    blockGeometry.instanceMatrix.needsUpdate = true;
	blockGeometry.computeBoundingSphere();
}

// Animation loop
function animate() {
    requestAnimationFrame(animate);
    controls.update();

    const time = Date.now() * 0.001;

    updateBlocks(time, ctBG, ctM, count, -20);

    renderer.render(scene, camera);
    stats.update();
}

animate();

// Resize handler for responsiveness
window.addEventListener('resize', () => {
    const aspect = window.innerWidth / window.innerHeight;
    camera.left = -cameraSize * aspect;
    camera.right = cameraSize * aspect;
    camera.updateProjectionMatrix();
    renderer.setSize(window.innerWidth, window.innerHeight);
});
