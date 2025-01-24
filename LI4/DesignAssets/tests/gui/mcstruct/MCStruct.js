class MCStruct {
    /** @type {Structure} */
    structure;

    /**
     * Creates an instance of MCStruct from a structure object.
     * @param {Structure} structure
     * @memberof MCStruct
     */
    constructor(structure) {
        this.structure = structure;
    }

    getName() {
        return this.structure.name;
    }

    /**
     * Returns a set of all the block types used in the structure.
     * 
     * @param {_GetBlockListOptions} options The options to use when getting the block list.
     * @returns {Set<{block: string, count?: number}>} A set of block types used in the structure.
     */
    getBlockList(options) {
        const blockCount = [];
        if (options?.blockCount) {
            for (const stage of this.structure.stages) {
                for (const block of stage.blocks) {
                    if (block.block in blockCount) {
                        blockCount[block.block]++;
                    } else {
                        blockCount[block.block] = 1;
                    }
                }
            }
        }

        const _blockList = new Set(this.structure.stages.reduce((acc, stage) => {
            return acc.concat(stage.blocks.map((block) => block.block));
        }, []));
        const blockList = new Set();
        _blockList.forEach((block) => {
            const blockData = { block: block };
            if (options?.blockCount) blockData.count = blockCount[block];

            blockList.add(blockData);
        });

        return blockList;
    }

    getStageCount() {
        return this.structure.stages.length;
    }

    getStages() {
        return this.structure.stages;
    }

    getStage(stageIndex) {
        return this.structure.stages[stageIndex];
    }

    /**
     * Creates an instance of MCStruct from a file.
     *
     * @static
     * @param {string} file The URI of the structure file to read.
     * @memberof MCStruct
     */
    static async fromFile(file) {
        const fileData = await fetch(file);
        const fileContent = await fileData.json();

        return new MCStruct(fileContent);
    }
}

export default MCStruct;

/**
 * @typedef {Object} Structure
 * @property {string} name
 * @property {StructureStage[]} stages
 */

/**
 * @typedef {Object} StructureStage
 * @property {StructureStageBlock[]} blocks
 */

/**
 * @typedef {Object} StructureStageBlock
 * @property {string} block
 * @property {string} x
 * @property {string} y
 * @property {string} z
 * @property {StructureStageBlockRotation} rotation
 */

/**
 * @typedef {Object} StructureStageBlockRotation
 * @property {string | undefined} x
 * @property {string | undefined} y
 * @property {string | undefined} z
 */

/**
 * @typedef {Object} _GetBlockListOptions
 * @property {boolean} [blockCount=true]
 */
