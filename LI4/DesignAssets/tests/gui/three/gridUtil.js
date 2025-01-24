import * as THREE from "three";
import { Line2 } from 'three/addons/lines/Line2.js';
import { LineMaterial } from 'three/addons/lines/LineMaterial.js';
import { LineGeometry } from 'three/addons/lines/LineGeometry.js';

const AXIS_COLORS = {
    X: 0xff0000,
    Y: 0x00ff00,
    Z: 0x0000ff,
}

/**
 * Create an axis with a specified length and color.
 * 
 * @param {THREE.Vector3} axis 
 * @param {number} length 
 * @param {number} color 
 * @returns {Line2}
 */
function makeAxis(axis, length, color) {
    const geometry = new LineGeometry()
		.setPositions([0, 0, 0, ...(new THREE.Vector3().copy(axis).multiplyScalar(length))])

    const material = new LineMaterial({ 
        color: color, 
        linewidth: 3,
    });

    const line = new Line2(geometry, material);
    line.computeLineDistances();

    return line;
}

/**
 * Create a grid with the specified number of blocks and block size.
 * 
 * @param {number} blocks 
 * @param {number} blockSize 
 * @param {THREE.Vector3?} position 
 * @param {THREE.Vector3?} rotation 
 * @returns 
 */
function makeGrid(blocks, blockSize, rotation, position) {
    const GRID_SIZE = blocks * blockSize;
    const GRID_DIVISIONS = blocks * blockSize;

    const gridHelper = new THREE.GridHelper( GRID_SIZE, GRID_DIVISIONS );
    gridHelper.position.y = -8;

    const gridGroup = new THREE.Group();
    gridGroup.add(gridHelper);

    const gridEdgePos = -(GRID_SIZE / 2);

    // Add helper lines for the grid
    const xLineMaterial = new THREE.LineBasicMaterial({ color: 0xffff00 });
    const zLineMaterial = new THREE.LineBasicMaterial({ color: 0x00ffff });
    for (let i = 1; i < blocks; i++) {
        const xLineGeometry = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(0, 0, 0), new THREE.Vector3(0, 0, GRID_SIZE)]);
        const xLine = new THREE.Line(xLineGeometry, xLineMaterial);
        
        const zLineGeometry = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(0, 0, 0), new THREE.Vector3(GRID_SIZE, 0, 0)]);
        const zLine = new THREE.Line(zLineGeometry, zLineMaterial);

        xLine.position.x = gridEdgePos + i * blockSize;
        xLine.position.y = -(blockSize / 2);
        xLine.position.z = gridEdgePos;

        zLine.position.x = gridEdgePos;
        zLine.position.y = -(blockSize / 2);
        zLine.position.z = gridEdgePos + i * blockSize;

        gridGroup.add(xLine);
        gridGroup.add(zLine);
    }

    if (position) gridGroup.position.copy(position);
    if (rotation) gridGroup.rotation.copy(rotation);

    return gridGroup;
}

function make3DGrid(blocks, blockSize, position) {
    const GRID_HALF = ((blocks - 1) / 2) * blockSize;
    const GRID_HALF_AXIS = ((blocks - 0) / 2) * blockSize;

    const gridGroup = new THREE.Group();

    //#region ======= Add Grids =======
    // XZ Grid
    const XZ_GRID = makeGrid(blocks, blockSize);
    gridGroup.add(XZ_GRID);
    
    // YZ Grid
    const YZ_GRID = makeGrid(blocks, blockSize, new THREE.Euler(Math.PI / 2, 0, 0), new THREE.Vector3(0, GRID_HALF, -GRID_HALF));
    gridGroup.add(YZ_GRID);
    
    // XY Grid
    const XY_GRID = makeGrid(blocks, blockSize, new THREE.Euler(0, 0, Math.PI / 2), new THREE.Vector3(-GRID_HALF - blockSize, GRID_HALF, 0));
    gridGroup.add(XY_GRID);
    //#endregion ======= Add Grids =======

    //#region ======= Add Axis =======
    // X Axis
    const X_AXIS = makeAxis(new THREE.Vector3(1, 0, 0), blocks * blockSize, AXIS_COLORS.X);
    X_AXIS.position.set(-GRID_HALF_AXIS, -(blockSize / 2), -GRID_HALF_AXIS);
    gridGroup.add(X_AXIS);

    // Y Axis
    const Y_AXIS = makeAxis(new THREE.Vector3(0, 1, 0), blocks * blockSize, AXIS_COLORS.Y);
    Y_AXIS.position.set(-GRID_HALF_AXIS, -(blockSize / 2), -GRID_HALF_AXIS);
    gridGroup.add(Y_AXIS);

    // Z Axis
    const Z_AXIS = makeAxis(new THREE.Vector3(0, 0, 1), blocks * blockSize, AXIS_COLORS.Z);
    Z_AXIS.position.set(-GRID_HALF_AXIS, -(blockSize / 2), -GRID_HALF_AXIS);
    gridGroup.add(Z_AXIS);
    //#region ======= Add Axis =======


    if (position) gridGroup.position.copy(position);

    return gridGroup;
}

export { makeGrid, make3DGrid };
