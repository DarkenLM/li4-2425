/**
 * Offsets all coordinates of the UV box by the given offsets on it's U and V axis.
 * 
 * @param {number[]} uvBox The UV box to offset.
 * @param {number} offsetU The offset to apply to the U axis.
 * @param {number} offsetV The offset to apply to the V axis.
 * @returns 
 */
function offsetUVBox(uvBox, offsetU, offsetV) {
    return uvBox.map((v, i) => (i % 2 === 0) ? v + offsetU : v + offsetV);
}

/**
 * Flips the UV box around the given axis, if the value passed for each is non-zero.
 * For non-zero values, the coordinates for each axis become the axis value minus the original value.
 * @param {number[]} uvBox 
 * @param {number} uAxis 
 * @param {number} vAxis 
 * @returns 
 */
function flipUVBox(uvBox, uAxis, vAxis) {
    return uvBox.map((v, i) => {
        if (i % 2 === 0) {
            if (uAxis) return uAxis - v;
            return v;
        } else {
            if (vAxis) return vAxis - v;
            return v;
        }
    });
}

/**
 * Rotates the UV coordinates around a given center by a given angle.
 * 
 * @param {number[]} uvArray The UV coordinates to rotate.
 * @param {number} angle The angle to rotate by, in radians.
 * @param {[number, number]} center The center to rotate around, in UV coordinates. Defaults to `[0.5, 0.5]`.
 */
function rotateUVs(uvArray, angle, center = [0.5, 0.5]) {
    const cosTheta = Math.cos(angle);
    const sinTheta = Math.sin(angle);
    const [cx, cy] = center;

    for (let i = 0; i < uvArray.length; i += 2) {
        let u = uvArray[i] - cx;
        let v = uvArray[i + 1] - cy;

        // Apply 2D rotation
        const uRotated = u * cosTheta - v * sinTheta;
        const vRotated = u * sinTheta + v * cosTheta;

        // Translate back and store
        uvArray[i] = uRotated + cx;
        uvArray[i + 1] = vRotated + cy;
    }
}

/**
 * Converts a UV box in the format `[startX, startY, endX, endY]` to an array of UV coordinates.
 * 
 * @param {[number, number, number, number]} uvBox The UV box to convert to UV coordinates.
 * @param {number} width The width of the texture to map to.
 * @param {number} height The height of the texture to map to.
 * @returns 
 */
function convertUVs(uvBox, width, height) {
    const [u1, v1, u2, v2] = uvBox;

    return [
        u1 / width, v2 / height, // Bottom-left
        u2 / width, v2 / height, // Bottom-right
        u2 / width, v1 / height, // Top-right
        u1 / width, v1 / height, // Top-left
    ]
}

export {
    offsetUVBox,
    flipUVBox,
    rotateUVs,
    convertUVs
}