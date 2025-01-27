import * as THREE from "three";
import { OrbitControls } from "three/addons/controls/OrbitControls.js";
import Stats from "three/addons/libs/stats.module.js";
import MCRender from "./MCRender.js";
import { make3DGrid, makeGrid } from "../three/gridUtil.js";

const cameraSize = 20;

class MC3D {
    /**
     * 
     * @param {MC3DOptions} options 
     */
    constructor({ 
        size = { width: window.innerWidth, height: window.innerHeight }, 
        modelLibraryPath = new URL("./mcstruct/blocks/", document.location.href),
        textureLibraryPath = new URL("./img/textures/", document.location.href),
    }) {
        this._ticking = false;

        //#region ======= Scene Setup =======
        this.scene = new THREE.Scene();

        this.renderer = new THREE.WebGLRenderer({ antialias: true });
        this.renderer.setSize(size.width, size.height);
        this.renderer.outputColorSpace = THREE.SRGBColorSpace;

        this.camera = new THREE.PerspectiveCamera(75, size.width / size.height, 0.1, 1000);
        // const aspect = size.width / size.height;
        // const cameraSize = 20;
        // this.camera = new THREE.OrthographicCamera(
        //     -cameraSize * aspect,
        //     cameraSize * aspect,
        //     cameraSize,
        //     -cameraSize,
        //     0.1,
        //     1000
        // );
        this.camera.position.set(5, 5, 10);
        this.camera.lookAt(0, 0, 0);

        this.controls = new OrbitControls(this.camera, this.renderer.domElement);

        const light = new THREE.AmbientLight(0xffffff, 1);
        this.scene.add(light);
        //#endregion ======= Scene Setup =======

        this.grid = undefined;
        this.stats = undefined;

        this.mcrender = new MCRender({
            modelLibraryPath: modelLibraryPath,
            textureLibraryPath: textureLibraryPath,
        });
    }

    //#region ======= Scene Methods =======
    attach(element) {
        element.appendChild(this.renderer.domElement);
    }

    addStats() {
        this.stats = new Stats();
    }

    addGrid(cells, cellSize) {
        const GRID_HALF = ((cells - 1) / 2) * cellSize;
        this.grid = make3DGrid(cells, cellSize, new THREE.Vector3(0, 0, 0));
        this.grid.position.x += GRID_HALF;
        this.grid.position.z += GRID_HALF;

        this.scene.add(this.grid);
    }

    resize(width, height) {
        // const aspect = width / height;

        // this.camera.left = -cameraSize * aspect;
        // this.camera.right = cameraSize * aspect;
        // this.camera.updateProjectionMatrix();
        // this.renderer.setSize(width, height);
    }

    clearScene() {
        this.stop();
        this.mcrender.clear(this.scene, { clearMaterials: true });
    }

    dispose() {
        this.stop();
        this.mcrender.dispose();
    }
    //#endregion ======= Scene Methods =======

    //#region ======= Camera Methods =======
    setCameraPosition(x, y, z) {
        this.camera.position.set(x, y, z);
        return this;
    }

    setCameraLookAt(x, y, z) {
        this.camera.lookAt(x, y, z);
        return this;
    }
    //#endregion ======= Camera Methods =======

    //#region ======= MCRender Methods =======
    async loadStructure(struct) {
        this.clearScene();
        await this.mcrender.loadStructure(struct);
    }
    //#endregion ======= MCRender Methods =======

    //#region ======= Lifecycle Methods =======
    prepare() {
        this.mcrender.prepare(this.scene);
    }

    render() {
        this.renderer.render(this.scene, this.camera);
        if (this.stats) this.stats.update();
    }

    update(time) {
        this.controls.update();
        
        this.mcrender.update(time);
    }

    /**
     * 
     * @param {Hook} preHook 
     * @param {Hook} postHook 
     */
    start(preHook, postHook) {
        const _preHook  = /** @type {Hook} */ preHook?.bind(this);
        const _postHook = /** @type {Hook} */ postHook?.bind(this);

        const animate = (time) => {
            if (preHook) _preHook(time);

            this.update(time);
            this.render();

            if (postHook) _postHook(time);
            
            if (this._ticking) requestAnimationFrame(animate);
        }

        this._ticking = true;
        requestAnimationFrame(animate);
    }

    stop() {
        this._ticking = false;
    }
    //#endregion ======= Lifecycle Methods =======
}

export default MC3D;

/**
 * @typedef {Object} MC3DOptions
 * @property {Size} size
 * @property {URL} modelLibraryPath
 */

/**
 * @typedef {Object} Size
 * @property {number} width
 * @property {number} height
 */

/**
 * @callback Hook
 * @this {MC3D}
 * @param {number} time
 */	
