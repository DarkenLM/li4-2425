import AudioPool from "../util/audioUtil.js";

const clickSound = new AudioPool("../sound/click.mp3", 5);

class MCCheckbox {
    /** @type {HTMLDivElement} */
    elem;
    /** @type {HTMLInputElement} */
    _checkboxElem;
    /** @type {HTMLLabelElement} */
    _labelElem;
    /** @type {ClickEvent[]} */
    _clickCallbacks;
    /** @type {Map<string, unknown>} */
    _state;

    constructor(elem) {
        if (!elem) throw new Error("Element is null or undefined");

        this.elem = elem;
        this._checkboxElem = elem.querySelector("input[type='checkbox']");
        this._labelElem = elem.querySelector("label");
        this._clickCallbacks = [];
        this._state = new Map();

        this._labelElem.addEventListener("click", this._onClick.bind(this));
    }

    enable() {
        this._checkboxElem.disabled = false;
        return this;
    }

    disable() {
        this._checkboxElem.disabled = true;
        return this;
    }

    setText(text) {
        this.elem.textContent = text;

        return this
    }

    isChecked() {
        return this._checkboxElem.checked;
    }

    setChecked(checked) {
        this._checkboxElem.checked = checked;
        return this;
    }

    //#region ======= State Management =======
    hasState(key) {
        return this._state.has(key);
    }

    getState(key) {
        return this._state.get(key);
    }

    setState(key, value) {
        this._state.set(key, value);
        return this;
    }
    //#endregion ======= State Management =======

    //#region ======= Click Event =======
    /**
     * Adds a click event listener to the button.
     * @param {ClickEvent} callback 
     */
    onClick(callback) {
        this._clickCallbacks.push(callback.bind(this));
        return this;
    }

    /**
     * Handles the click event.
     * @param {MouseEvent} e
     */
    _onClick(e) {
        clickSound.play();
        this._clickCallbacks.forEach(cb => cb(this.elem));
    }
    //#endregion ======= Click Event =======
}

export default MCCheckbox;

/**
 * @callback ClickEvent
 * @this {MCCheckbox}
 * @param {HTMLDivElement} elem
 * @returns {void}
 */