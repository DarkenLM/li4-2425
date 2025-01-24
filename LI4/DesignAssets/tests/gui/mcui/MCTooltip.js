function tooltipElementFactory() {
    const tooltip = document.createElement("div");
    tooltip.className = "mc-tooltip";
    tooltip.style.display = "none";

    const tooltipTitle = document.createElement("div");
    tooltipTitle.className = "mc-tooltip-title";
    tooltip.appendChild(tooltipTitle);

    const tooltipDescription = document.createElement("div");
    tooltipDescription.className = "mc-tooltip-description";
    tooltip.appendChild(tooltipDescription);

    return { tooltip, tooltipTitle, tooltipDescription };
}

class MCTooltip {
    static OFFSET = 5;

    /** @type {HTMLDivElement} */
    tooltip;
    /** @type {HTMLDivElement} */
    tooltipTitle;
    /** @type {HTMLDivElement} */
    tooltipDescription;
    /** @type {number} */
    hideTimeout;

    constructor() {
        // this.tooltip = document.createElement("div");
        // this.tooltip.className = "mc-tooltip";
        // this.tooltip.style.display = "none";
        // document.body.appendChild(this.tooltip);

        const tt = tooltipElementFactory();
        this.tooltip = tt.tooltip;
        this.tooltipTitle = tt.tooltipTitle;
        this.tooltipDescription = tt.tooltipDescription;
        document.body.appendChild(this.tooltip);

        this.hideTimeout = undefined;
        this.cancelHide = false;

        // Bound event handler, so it can be removed later
        this.__onMouseMove = this._onMouseMove.bind(this);
        this._followingMouse = false;
    }

    //#region ============== Mouse Movement ==============
    followMouse() {
        if (this._followingMouse) return this;

        document.addEventListener("mousemove", this.__onMouseMove);
        this._followingMouse = true;
        return this;
    }

    stopFollowMouse() {
        if (!this._followingMouse) return this;

        document.removeEventListener("mousemove", this.__onMouseMove);
        this._followingMouse = false;
        return this;
    }

    _onMouseMove(e) {
        this.tooltip.style.left = e.pageX + MCTooltip.OFFSET + "px";
        this.tooltip.style.top = e.pageY + MCTooltip.OFFSET + "px";
    }
    //#endregion ============== Mouse Movement ==============

    reset() {
        // this.tooltip.style.display = "none";
        // this.tooltip.style.left = 0 + "px";
        // this.tooltip.style.top = 0 + "px";

        return this;
    }

    setTitle(title) {
        this.tooltipTitle.innerHTML = title;
        return this;
    }

    setDescription(text) {
        this.tooltipDescription.innerHTML = text;
        return this;
    }

    setTooltip(text, x, y) {
        this.tooltip.innerHTML = text;
        this.tooltip.style.left = x + MCTooltip.OFFSET + "px";
        this.tooltip.style.top = y + MCTooltip.OFFSET + "px";

        this.showTooltip();

        return this;
    }

    showTooltip() {
        if (this.hideTimeout) {
            clearTimeout(this.hideTimeout);
            this.hideTimeout = undefined;
        }
        this.tooltip.style.display = "block";

        return this;
    }

    hideTooltip() {
        // this.tooltip.style.display = "none";
        this.hideTimeout = setTimeout(() => {
            if (this.cancelHide) {
                this.cancelHide = false;
                return;
            }

            this.tooltip.style.display = "none";
        }, 50);

        return this;
    }

    /** @type {MCTooltip} */
    static globalTooltip = undefined;

    /**
     * @static
     * @return {MCTooltip} 
     * @memberof MCTooltip
     */
    static getGlobalTooltip() {
        if (!MCTooltip.globalTooltip) {
            MCTooltip.globalTooltip = new MCTooltip();
        }

        return MCTooltip.globalTooltip;
    }
}

export default MCTooltip;