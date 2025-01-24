import * as THREE from "three";
import { OrbitControls } from "three/addons/controls/OrbitControls.js";
import { GUI } from "three/addons/libs/lil-gui.module.min.js";
import Stats from "three/addons/libs/stats.module.js";
import MCStruct from "./mcstruct/MCStruct.js";
import MCRender from "./three/mcRender.js";

import smallTent1 from "./mcstruct/constructions/small_tent_1.json" with { type: "json" };
import craftingTableModule from "./mcstruct/blocks/crafting_table.json" with { type: "json" };

import { flipUVBox, offsetUVBox, convertUVs, rotateUVs } from "./three/uvUtil.js";
import { make3DGrid, makeGrid } from "./three/gridUtil.js";

//#region ============== Scene Setup ==============
// Initialize scene, camera, and renderer
const scene = new THREE.Scene();

// Renderer setup
const renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.setSize(window.innerWidth, window.innerHeight);
renderer.outputColorSpace = THREE.SRGBColorSpace;
document.body.appendChild(renderer.domElement);

const stats = new Stats();
document.body.appendChild(stats.dom);

// Orthographic camera for isometric perspective
const aspect = window.innerWidth / window.innerHeight;
const cameraSize = 20;
// const camera = new THREE.OrthographicCamera(
//     /* left   */ -cameraSize * aspect,
//     /* right  */ cameraSize * aspect,
//     /* top    */ cameraSize,
//     /* bottom */ -cameraSize,
//     /* near   */ 0.01,
//     /* far    */ 1000
// );
const camera = new THREE.PerspectiveCamera(75, aspect, 0.1, 1000);
const controls = new OrbitControls( camera, renderer.domElement );

camera.position.set(5, 5, 10);
// camera.position.set(-10, 5, -5);
camera.lookAt(0, 0, 0);
controls.update();

// Create a basic light
const light = new THREE.AmbientLight(0xffffff, 1); // Soft white light
scene.add(light);
//#endregion ============== Scene Setup ==============

//#region ============== Scene Objects ==============
const struct = new MCStruct(smallTent1);
console.log("STRUCTURE:", struct);

console.log("URI:", new URL("./mcstruct/blocks/", document.location.href));

const mcrender = new MCRender({
    modelLibraryPath: new URL("./mcstruct/blocks/", document.location.href),
});
console.log("MCRENDER:", mcrender);

await mcrender.loadStructure(struct);
// const ctMM = mcrender.createModelMesh(craftingTableModule);
console.log("MCRENDER 2:", mcrender);

mcrender._debugTextures(scene);
mcrender.prepare(scene);

console.log("MCRENDER 3:", mcrender);

const GRID_BLOCKS = 7;
const GRID_BLOCK_SIZE = 16;
const GRID_HALF = ((GRID_BLOCKS - 1) / 2) * GRID_BLOCK_SIZE;

// const H_GRID = makeGrid(GRID_BLOCKS, GRID_BLOCK_SIZE, new THREE.Vector3(0, 0, 0));
// H_GRID.position.x += GRID_HALF;
// H_GRID.position.z += GRID_HALF;
// scene.add(H_GRID);

// // const V_GRID = makeGrid(GRID_BLOCKS, GRID_BLOCK_SIZE, new THREE.Euler(Math.PI / 2, 0, 0), new THREE.Vector3(0, GRID_HALF, -GRID_HALF));
// const V_GRID = makeGrid(GRID_BLOCKS, GRID_BLOCK_SIZE, new THREE.Euler(Math.PI / 2, 0, 0), new THREE.Vector3(0, GRID_HALF, -GRID_HALF));
// V_GRID.position.x += GRID_HALF;
// V_GRID.position.z += GRID_HALF;
// scene.add(V_GRID);

// // const V2_GRID = makeGrid(GRID_BLOCKS, GRID_BLOCK_SIZE, new THREE.Euler(0, 0, Math.PI / 2), new THREE.Vector3(-GRID_HALF - GRID_BLOCK_SIZE, GRID_HALF, 0));
// const V2_GRID = makeGrid(GRID_BLOCKS, GRID_BLOCK_SIZE, new THREE.Euler(0, 0, Math.PI / 2), new THREE.Vector3(-GRID_HALF - GRID_BLOCK_SIZE, GRID_HALF, 0));
// V2_GRID.position.x += GRID_HALF;
// V2_GRID.position.z += GRID_HALF;
// scene.add(V2_GRID);

const grid = make3DGrid(GRID_BLOCKS, GRID_BLOCK_SIZE, new THREE.Vector3(0, 0, 0));
grid.position.x += GRID_HALF;
grid.position.z += GRID_HALF;
scene.add(grid);

//#endregion ============== Scene Objects ==============

//#region ============== GUI Setup ==============
let _GUI_selfupdate = false;
let GUI_autotick = false;
const gui = new GUI();
const param = {
    "Current Tick": 0,
    "Autotick": false,
};
gui.add(param, "Current Tick", 0, mcrender._totalTicks).listen().onChange(function (v) {
    if (_GUI_selfupdate) {
        _GUI_selfupdate = false;
        return;
    }
    mcrender.__currentTick = v;
    mcrender.setTick(v);
});
gui.add(param, "Autotick").onChange(function (v) {
    GUI_autotick = v;
});
//#endregion ============== GUI Setup ==============


//#region ============== Scene Render ==============
animate();

//#region ============== Scene Render ==============
function animate() {
    requestAnimationFrame(animate);
    controls.update();

    const time = Date.now() * 0.001;

    // Render calls here
    // updateBlocks(time, ctBG, ctM, count, -20);
    if (GUI_autotick) {
        mcrender.__currentTick++;
        mcrender._currentTick = mcrender._bounceValueBetween(mcrender.__currentTick, 0, mcrender._totalTicks);
        console.log("TICK:", mcrender.__currentTick, mcrender._currentTick);

        _GUI_selfupdate = true;
        param["Current Tick"] = mcrender._currentTick;
    }

    mcrender.update(time);

    renderer.render(scene, camera);
    stats.update();
}

// Resize handler for responsiveness
window.addEventListener("resize", () => {
    const aspect = window.innerWidth / window.innerHeight;
    camera.left = -cameraSize * aspect;
    camera.right = cameraSize * aspect;
    camera.updateProjectionMatrix();
    renderer.setSize(window.innerWidth, window.innerHeight);
});
//#endregion ============== Scene Render ==============