import { makeId } from "../../util/randUtil.js";
import MCItem from "./MCItem.js";
import MCTooltip from "./MCTooltip.js";

class MCSlot {
    static EVENT_UPDATE = "event_update";

    /** @type {string} */
    id;
    /** @type {HTMLDivElement} */
    elem;
    /** @type {MCItem?} */
    item;

    constructor(elem) {
        this.id = makeId(16);

        this.elem = elem;
        this.elem.id = `mc-slot-${this.id}`;

        this.elem.addEventListener("dragover", this._onDragOver.bind(this));
        this.elem.addEventListener("drop", this._onDrop.bind(this));
        this.elem.addEventListener("dragenter", this._onDragEnter.bind(this));
        this.elem.addEventListener("dragleave", this._onDragLeave.bind(this));
        this.elem.addEventListener("mouseenter", this._onMouseEnter.bind(this));
        this.elem.addEventListener("mouseleave", this._onMouseLeave.bind(this));
        this.elem.addEventListener("mousedown", this._onMouseDown.bind(this));
        this.elem.addEventListener("mouseup", this._onMouseUp.bind(this));
        this.elem.addEventListener("mousemove", this._onMouseMove.bind(this));

        this.elem.addEventListener(MCSlot.EVENT_UPDATE, this.update.bind(this));

        const item = this.elem.querySelector(".mc-item");
        if (item) {
            this.item = new MCItem(item).bindSlot(this);
        } else {
            this.item = null;
        }
    }

    getId() {
        return this.id;
    }

    isDisabled() {
        return this.elem.classList.contains("disabled");
    }

    update() {
        if (this.item) {
            // Check if the slot still holds the item
            const _item = this.elem.querySelector(".mc-item");
            if (!_item) {
                this.item = null;
            }
        } else {
            // Check if the slot has a new item
            const _item = this.elem.querySelector(".mc-item");
            if (_item) {
                this.item = new MCItem(_item).bindSlot(this);
            }
        }
    }

    /**
     * Event handler for the dragover event.
     * @param {DragEvent} event 
     */
    _onDragOver(event) {
        event.preventDefault();
        // console.log("drag over", event, [event.dataTransfer.getData(MCItem.ITEM_ID)]);

        if (event.dataTransfer.getData(MCItem.ITEM_ID)) {
            event.dataTransfer.dropEffect = "move";
        }
    }

    /**
     * Event handler for the drop event.
     * @param {DragEvent} event 
     */
    _onDrop(event) {
        event.preventDefault();
        // console.log("drop", event);

        const data = event.dataTransfer.getData(MCItem.ITEM_ID);
        // console.log("drop data", [data]);

        if (!data) return;
        if (this.isDisabled()) return;

        const item = document.getElementById(data);
        this.elem.appendChild(item);
        // this.item = new MCItem(item, false);
        this.update();

        const oldSlot = document.getElementById(`mc-slot-${event.dataTransfer.getData(MCItem.SLOT_ID)}`);
        if (oldSlot) {
            oldSlot.dispatchEvent(new Event(MCSlot.EVENT_UPDATE));
        }
    }

    /**
     * Event handler for the dragenter event.
     * @param {DragEvent} event 
     */
    _onDragEnter(event) {
        // event.dataTransfer.setData("text", event.target.id);
        // console.log("drag enter", event);
    }

    /**
     * Event handler for the dragleave event.
     * @param {DragEvent} event 
     */
    _onDragLeave(event) {
        // console.log("drag leave", event);
    }

    _onMouseEnter(event) {
        if (!this.item) return;
        // MCTooltip.getGlobalTooltip().followMouse().setTitle(`${this.item.slotId}`).showTooltip();
        MCTooltip .getGlobalTooltip()
            .setTitle(`${this.item.getName()}`)
            .setDescription(`${this.item.getDescription()}`)
            .followMouse()
            .showTooltip();
    }

    _onMouseLeave(event) {
        this._possibleDrag = false;
        MCTooltip.getGlobalTooltip().hideTooltip();
    }

    _onMouseDown(event) {
        this._possibleDrag = true;
        MCTooltip.getGlobalTooltip().hideTooltip();
    }

    _onMouseUp(event) {
        if (!this.item) return;
        // MCTooltip.getGlobalTooltip().setTitle(`${this.item.slotId}`).showTooltip();
        MCTooltip.getGlobalTooltip()
            .setTitle(`${this.item.getName()}`)
            .setDescription(`${this.item.getDescription()}`)
            .showTooltip();
    }

    _onMouseMove(event) {
        if (this._possibleDrag) return;
        if (!this.item) return;
        // MCTooltip.getGlobalTooltip().setTitle(`${this.item.slotId}`).showTooltip();
        MCTooltip.getGlobalTooltip()
            .setTitle(`${this.item.getName()}`)
            .setDescription(`${this.item.getDescription()}`)
            .showTooltip();
    }
}

export default MCSlot;
