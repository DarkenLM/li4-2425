import { makeId } from "../util/randUtil.js";
import MCSlot from "./MCSlot.js";

class MCItem {
    static ITEM_ID = "item-id";
    static SLOT_ID = "slot-id";

    /** @type {HTMLDivElement} */
    elem;
    /** @type {string?} */
    slotId;

    constructor(elem, rebind = true) {
        this.elem = elem;
        this.slotId = null;

        if (rebind) {
            this.elem.addEventListener("dragstart", this._onDragStart.bind(this));
            this.elem.setAttribute("draggable", "true");
            this.elem.id = `mc-item-${makeId(16)}`;
        }
    }

    getName() {
        return this.elem.getAttribute("data-name") ?? "Unknown";
    }

    getDescription() {
        return this.elem.getAttribute("data-description") ?? "No description";
    }

    /**
     * @param {MCSlot} slot 
     */
    bindSlot(slot) {
        this.slotId = slot.getId();
        console.log("Item bound:", this.slotId);

        return this;
    }

    /**
     * Event handler for the dragstart event.
     * @param {DragEvent} event 
     */
    _onDragStart(event) {
        event.dataTransfer.setData(MCItem.ITEM_ID, this.elem.id);
        event.dataTransfer.setData(MCItem.SLOT_ID, this.slotId);
        event.dataTransfer.effectAllowed = "move";

        // console.log("drag start", event, [this.elem, event.dataTransfer.getData(MCItem.ITEM_ID)]);
    }
}

export default MCItem;
