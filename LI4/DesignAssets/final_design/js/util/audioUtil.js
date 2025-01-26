/**
 * @typedef {Object} AudioPoolEntry
 * @property {HTMLAudioElement} audio
 * @property {boolean} available
 */

/**
 * @class AudioPool
 * @description Provides a pool of the same audio source that can be played in parallel without the need for the 
 * rapid creation and destruction of single-use audio elements.
 */
class AudioPool {
    /**
     * @type {AudioPoolEntry[]}
     */
    pool = [];
    _src = "N/A";
    debug = false;

    /**
     * 
     * @param {(Audio | string)?} audio 
     * @param {number?} size 
     */
    constructor(audio, size) {
        this.pool = [];

        /** @type {string} */
        this._src = (audio instanceof Audio) ? audio.src : audio;

        if (audio && size) {
            for (let i = 0; i < size; i++) {
                this.addAudio(new Audio(this._src));
            }
        }
    }

    /**
     * Adds an audio source to the pool.
     * @param {Audio} audio 
     */
    addAudio(audio) {
        this.pool.push({
            audio,
            available: true
        });
    }

    /**
     * Plays the audio source from the pool.
     */
    play() {
        const audio = this._get();

        if (audio) {
            if (this.debug) console.log("Playing audio:", this._src);

            audio.audio.addEventListener("ended", () => {
                audio.available = true;
            }, { once: true });
            audio.audio.play();
        } else {
            if (this.debug) console.log("Could not play audio:", this._src, " - empty pool.");
        }
    }

    /**
     * Returns an available audio source from the pool, or null if all entries are unavailable.
     * @returns {AudioPoolEntry}
     */
    _get() {
        const entry = this.pool.find(entry => entry.available);
        if (entry) {
            entry.available = false;
            return entry;
        }

        return null;
    }
}

export default AudioPool;
