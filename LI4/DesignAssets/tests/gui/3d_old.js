// Import Three.js
import * as THREE from "three";
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';

import oakFenceModule from "./mcstruct/blocks/oak_fence.json" with { type: "json" };

const BLOCK_GRID_SIZE = 16;

// Load textures
const textureLoader = new THREE.TextureLoader();
/** @type {Record<string, typeof THREE.Texture>} */
const textures = {};
Object.entries(oakFenceModule.textures).forEach(([k,v]) => {
    textures[k] = textureLoader.load(v);
    textures[k].magFilter = THREE.NearestFilter;
    textures[k].colorSpace = THREE.SRGBColorSpace;
    // textures[k].rotation = THREE.MathUtils.degToRad(180);
});

console.log("TEXTURES:", textures);


function createBlockGeometry(schema) {
    const group = new THREE.Group(); // Group to hold all parts

    schema.elements.forEach(element => {
        const from = element.from; // [x, y, z]
        const to = element.to;     // [x, y, z]

        const width = Math.abs(to[0] - from[0]);
        const height = Math.abs(to[1] - from[1]);
        const depth = Math.abs(to[2] - from[2]);

        console.log("WIDTH:", width, "HEIGHT:", height, "DEPTH:", depth);

        // Create a box for this element
        const geometry = new THREE.BoxGeometry(width, height, depth);

        // Adjust the position of the geometry
        const offsetX = (from[0] + to[0]) / 2 - (BLOCK_GRID_SIZE / 2); // Center within block
        const offsetY = (from[1] + to[1]) / 2 - (BLOCK_GRID_SIZE / 2); // Center within block
        const offsetZ = (from[2] + to[2]) / 2 - (BLOCK_GRID_SIZE / 2); // Center within block
        geometry.translate(offsetX, offsetY, offsetZ);

        // Map textures to faces
        const materials = [];
        const faceOrder = ['right', 'left', 'top', 'bottom', 'front', 'back']; // Three.js order
        const schemaFaceOrder = ['east', 'west', 'up', 'down', 'south', 'north'];

        schemaFaceOrder.forEach((face, index) => {
            const faceData = element.faces[face];
            if (faceData) {
                const texture = textures[faceData.texture];
                const uv = faceData.uv.map(v => v / 16); // Convert UVs to [0, 1] range
                texture.wrapS = THREE.RepeatWrapping;
                texture.wrapT = THREE.RepeatWrapping;
                texture.repeat.set(1, 1);

                console.log(face.toUpperCase(), "==========================");
                console.log("UV:", uv);

                // Update UV mapping for this face
                const material = new THREE.MeshBasicMaterial({ map: texture });
                material.blending = THREE.NoBlending;
                materials[index] = material;

                // UV mapping
                const faceUvs = geometry.attributes.uv.array;
                const uvIndex = index * 8; // Each face has 4 vertices, each vertex has (u, v)

                console.log("FACEUVS:", faceUvs);
                console.log("UV INDEX:", uvIndex);

                if (face === "south") {
                    faceUvs[uvIndex + 0] = 0; // Bottom-left
                    faceUvs[uvIndex + 1] = 0.375;
                    faceUvs[uvIndex + 2] = 0.625; // Bottom-right
                    faceUvs[uvIndex + 3] = 0.375;
                    faceUvs[uvIndex + 4] = 0; // Top-right
                    faceUvs[uvIndex + 5] = 0;
                    faceUvs[uvIndex + 6] = 0.625; // Top-left
                    faceUvs[uvIndex + 7] = 0;
                }

                // BR[0] = 1 + TL[0] = 1 -> Cut in half vertically
                // BL[1] = 1 + BR[1] = 1 -> Cut in half horizontally
                // BL[0] = 1 + TL[1] = 1 -> Stretch upper triangle horizontally, skew lower triangle
                // BL[0] = 1 + TR[0] = 1 -> Stretches the fuck out of the face, first column, possibly in the middle.
                //    BL[0] = 0.5 + TR[0] = 0.5 -> Cut in half horizontally and vertically, snap to BR.
                // BL[0] = 1 + TR[1] = 1 -> Stretch upper triangle horizontally, skew lower triangle



                // faceUvs[uvIndex + 0] = uv[0]; // Bottom-left
                // faceUvs[uvIndex + 1] = uv[1];
                // faceUvs[uvIndex + 2] = uv[0]; // Bottom-right
                // faceUvs[uvIndex + 3] = uv[1];
                // faceUvs[uvIndex + 4] = uv[0]; // Top-right
                // faceUvs[uvIndex + 5] = uv[1];
                // faceUvs[uvIndex + 6] = uv[0]; // Top-left
                // faceUvs[uvIndex + 7] = uv[1];
            }
        });

        // Create a mesh for this part
        const mesh = new THREE.Mesh(geometry, materials);
        group.add(mesh);
    });

    return group;
}

// Initialize scene, camera, and renderer
// const scene = new THREE.Scene();
// const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
// const renderer = new THREE.WebGLRenderer();
// renderer.setSize(window.innerWidth, window.innerHeight);
// document.body.appendChild(renderer.domElement);

const scene = new THREE.Scene();

// Renderer setup
const renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.setSize(window.innerWidth, window.innerHeight);
// renderer.shadowMap.enabled = true; // Enable shadow rendering
// renderer.shadowMap.type = THREE.PCFSoftShadowMap; // Soft shadows
renderer.outputColorSpace = THREE.SRGBColorSpace;
document.body.appendChild(renderer.domElement);

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

camera.position.set(5, 5, 10);
camera.lookAt(0, 0, 0);
controls.update();

// Create a basic light
const light = new THREE.AmbientLight(0xffffff, 1); // Soft white light
scene.add(light);

// Parse schema and add blocks
const blockGeometry = createBlockGeometry(oakFenceModule);
scene.add(blockGeometry);

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

// Animation loop
function animate() {
    requestAnimationFrame(animate);
    controls.update();
    renderer.render(scene, camera);
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


// const schema = {
//     "stages": [
//         {
//             "blocks": [
//                 { "block": "minecraft:oak_fence", "x": 0, "y": 0, "z": 0 },
//                 { "block": "minecraft:stone", "x": 1, "y": 0, "z": 0 },
//                 { "block": "minecraft:grass_block", "x": 0, "y": 1, "z": 0 }
//             ]
//         }
//     ]
// };

// // Initialize scene, camera, and renderer
// const scene = new THREE.Scene();
// const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
// const renderer = new THREE.WebGLRenderer();
// renderer.setSize(window.innerWidth, window.innerHeight);
// document.body.appendChild(renderer.domElement);

// // Setup camera position
// camera.position.set(5, 5, 10);
// camera.lookAt(0, 0, 0);

// // Create a basic light
// const light = new THREE.AmbientLight(0xffffff); // Soft white light
// scene.add(light);

// // Block textures/materials (Example textures)
// const blockMaterials = {
//     "minecraft:oak_fence": new THREE.MeshBasicMaterial({ color: 0x8B4513 }),
//     "minecraft:stone": new THREE.MeshBasicMaterial({ color: 0x808080 }),
//     "minecraft:grass_block": new THREE.MeshBasicMaterial({ color: 0x00FF00 })
// };

// // Block geometry
// const blockGeometry = new THREE.BoxGeometry(1, 1, 1);

// // Parse schema and add blocks
// schema.stages.forEach(stage => {
//     stage.blocks.forEach(({ block, x, y, z }) => {
//         const material = blockMaterials[block] || new THREE.MeshBasicMaterial({ color: 0xff0000 });
//         const blockMesh = new THREE.Mesh(blockGeometry, material);
//         blockMesh.position.set(x, y, z);
//         scene.add(blockMesh);
//     });
// });

// // Animation loop
// function animate() {
//     requestAnimationFrame(animate);
//     renderer.render(scene, camera);
// }
// animate();