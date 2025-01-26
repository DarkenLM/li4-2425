import AudioPool from "../util/audioUtil.js";

const clickSound = new AudioPool("../sound/click.mp3", 5);

class MCButton {
    /** @type {HTMLDivElement} */
    elem;
    /** @type {HTMLParagraphElement | undefined} */
    _textElem;
    /** @type {ClickEvent[]} */
    _clickCallbacks;
    /** @type {Map<string, unknown>} */
    _state;

    constructor(elem) {
        if (!elem) throw new Error("Element is null or undefined");

        this.elem = elem;
        this._textElem = elem.querySelector("p");
        this._clickCallbacks = [];
        this._state = new Map();

        this.elem.addEventListener("click", this._onClick.bind(this));
    }

    setText(text) {
        if (!this._textElem) this._addText(text);
        else this._textElem.textContent = text;

        return this
    }

    _addText(text) {
        const pElem = document.createElement("p");
        pElem.textContent = text;
        this.elem.appendChild(pElem);

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
        e.preventDefault();
        e.stopImmediatePropagation();
        clickSound.play();
        this._clickCallbacks.forEach(cb => cb(this.elem));
    }
    //#endregion ======= Click Event =======
}

export default MCButton;

/**
 * @callback ClickEvent
 * @this {MCButton}
 * @param {HTMLDivElement} elem
 * @returns {void}
 */