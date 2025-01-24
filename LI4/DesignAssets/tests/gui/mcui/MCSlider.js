import { cssutil } from "../../util/cssUtil.js";

class MCSlider {
    /** @type {HTMLDivElement} */
    elem;
    /** @type {HTMLInputElement} */
    inputElem;
    /** @type {HTMLDivElement?} */
    progressElem;
    /** @type {boolean} */
    showProgressCount;

    constructor(elem) {
        this.elem = elem;
        this.inputElem = elem.querySelector("input");
        this.progressElem = elem.querySelector(".mc-slider-progress");
        this.showProgressCount = this.progressElem ? true : false;

        if (this.progressElem) this._updateProgressElem();

        this.inputElem.addEventListener("input", this._onInput.bind(this));
    }

    setChevrons(chevrons) {
        this.elem.style.setProperty("--chevrons", chevrons);
        this.inputElem.max = chevrons;

        const percentage = (this.inputElem.value / chevrons);
        this.elem.style.setProperty('--progress', `${percentage}`);

        if (this.inputElem.value > chevrons) {
            this.inputElem.value = chevrons;
        }

        if (this.progressElem) this._updateProgressElem();

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

    /**
     * Event handler for the input event.
     * @param {InputEvent} event 
     */
    _onInput(event) {
        const value = this.inputElem.value;
        const max = this.inputElem.max;

        // console.log("input", event, [value, max]);

        // Calculate progress percentage
        // const percentage = (value / max) * 100;
        const percentage = (value / max);

        // Update the filled portion width
        // this.elem.style.setProperty('--progress', `${percentage}%`);
        // this.elem.style.setProperty('--fill-width', `${percentage}%`);
        this.elem.style.setProperty('--progress', `${percentage}`);


        if (this.progressElem) this._updateProgressElem();
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
}

export default MCSlider;
