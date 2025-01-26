import { cssutil } from "../util/cssUtil.js";

const SILENT_EVENT = Symbol("silent_event");

class MCSlider {
    /** @type {HTMLDivElement} */
    elem;
    /** @type {HTMLInputElement} */
    inputElem;
    /** @type {HTMLDivElement?} */
    progressElem;
    /** @type {boolean} */
    showProgressCount;
    /** @type {MCInputEvent[]} */
    _inputCallbacks;

    constructor(elem) {
        this.elem = elem;
        this.inputElem = elem.querySelector("input");
        this.progressElem = elem.querySelector(".mc-slider-progress");
        this.showProgressCount = this.progressElem ? true : false;
        this._inputCallbacks = [];

        this._snapToChevrons = false;
        this._chevrons = [];

        if (this.progressElem) this._updateProgressElem();

        this.inputElem.addEventListener("input", this._onInput.bind(this));
    }

    setSnapToChevrons(snap) {
        this._snapToChevrons = snap;
        console.log("Snap to chevrons", snap, this._chevrons);
        return this;
    }

    setChevrons(chevrons, detached = false) {
        // this.elem.style.setProperty("--chevrons", chevrons);
        // this.inputElem.max = chevrons;

        // const percentage = (this.inputElem.value / chevrons);
        // this.elem.style.setProperty('--progress', `${percentage}`);

        // if (this.inputElem.value > chevrons) {
        //     this.inputElem.value = chevrons;
        // }

        // if (this.progressElem) this._updateProgressElem();

        // return this;

        let chevronNumber = Array.isArray(chevrons) ? chevrons.length : chevrons;
        let chevronArr = Array.isArray(chevrons) ? chevrons : [...Array(chevrons + 1).keys().map(i => Math.round(i * this.inputElem.max / chevrons))];


        this.elem.style.setProperty("--chevrons", chevronNumber);
        this._chevrons = chevronArr;
        
        if (!detached) this.setSteps(chevronNumber);

        return this;
    }

    setSteps(steps) {
        this.inputElem.max = steps;

        const percentage = (this.inputElem.value / steps);
        this.elem.style.setProperty('--progress', `${percentage}`);

        if (this.inputElem.value > steps) {
            this.inputElem.value = steps;
        }

        if (this.progressElem) this._updateProgressElem();
        return this;
    }

    getProgress() {
        return this.inputElem.value;
    }

    setProgress(progress, propagate = true, silent = true) {
        this.inputElem.value = progress;
        if (propagate) this._onInput(silent ? SILENT_EVENT : undefined);
        return this;
    }

    showProgress() {
        this._updateProgressElem();
        this.progressElem.style.display = "block";
        this.showProgressCount = true;
        return this;
    }

    hideProgress() {
        this.progressElem.style.display = "none";
        this.showProgressCount = false
        return this;
    }

    enable() {
        this.inputElem.disabled = false;
    }

    disable() {
        this.inputElem.disabled = true;
    }

    /**
     * Adds an input event listener to the slider.
     * @param {MCInputEvent} callback 
     */
    onInput(callback) {
        this._inputCallbacks.push(callback.bind(this));
        return this;
    }

    /**
     * Event handler for the input event.
     * @param {InputEvent} event 
     */
    _onInput(event) {
        let value = this.inputElem.value;
        const max = this.inputElem.max;

        console.log("input", event, [value, max]);
        if (this._snapToChevrons) {
            // value = this._snapToClosestChevron(max, 5, value);
            value = this._snapToClosestChevron(value);
            this.inputElem.value = value;
        }

        // console.log("input", event, [value, max]);

        // Calculate progress percentage
        // const percentage = (value / max) * 100;
        const percentage = (value / max);

        // Update the filled portion width
        // this.elem.style.setProperty('--progress', `${percentage}%`);
        // this.elem.style.setProperty('--fill-width', `${percentage}%`);
        this.elem.style.setProperty('--progress', `${percentage}`);


        if (this.progressElem) this._updateProgressElem();
        if (event !== SILENT_EVENT) this._inputCallbacks.forEach(cb => cb(this.elem));
    }

    _updateProgressElem() {
        // Offset the progress text using JS because I'm too lazy to adjust the CSS to use flexbox and still keep the slider above it.
        // (z-indexing is a fucking pain in the ass).
        const value = this.inputElem.value;
        const progressElemStyle = getComputedStyle(this.progressElem);
        const progressElemWidth = cssutil.parseValue(getComputedStyle(this.elem).width)[0];
        const fontSize = cssutil.parseValue(progressElemStyle.getPropertyValue("font-size"))[0];
        const correctedOffset = Math.ceil((progressElemWidth.value - (fontSize.value * value.length)) / 2);
    
        this.progressElem.innerText = value;
        this.progressElem.style.left = `${correctedOffset}px`;
    }

    // _snapToClosestChevron(max_steps, stages, x) {
    //     const step_size = max_steps / (stages - 1); 
    //     const closest_stage = Math.round(x / step_size) * step_size;

    //     return Math.min(Math.max(closest_stage, 0), max_steps);
    // }

    _snapToClosestChevron(x) {
        return this._chevrons.reduce((closest, current) =>
            Math.abs(current - x) < Math.abs(closest - x) ? current : closest
        );
    }
}

export default MCSlider;
/**
 * @callback MCInputEvent
 * @this {MCSlider}
 * @param {HTMLDivElement} elem
 * @returns {void}
 */
