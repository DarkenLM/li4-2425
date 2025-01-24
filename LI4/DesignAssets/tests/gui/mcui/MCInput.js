class MCInput {
    /** @type {HTMLInputElement} */
    elem;

    constructor(elem) {
        this.elem = elem;

        // this.elem.addEventListener("input", this._onInput.bind(this));
    }

    onInput(callback) {
        this.elem.addEventListener("input", callback.bind(this));
        return this;
    }

    // /**
    //  * Event handler for the input event.
    //  * @param {InputEvent} event 
    //  */
    // _onInput(event) {
    //     const value = this.elem.value;
    //     console.log("input", event, [value]);
    // }
}

export default MCInput;

/**
 * @callback ClickEvent
 * @this {MCInput}
 * @param {HTMLInputElement} elem
 * @returns {void}
 */
