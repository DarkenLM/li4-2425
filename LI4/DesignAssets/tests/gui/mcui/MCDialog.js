class MCDialog {
    /** @type {HTMLDialogElement} */
    dialog;
    /** @type {HTMLDivElement} */
    content;
    /** @type {HTMLButtonElement} */
    closeBtn;

    constructor(dialog) {
        this.dialog = dialog;
        this.content = this.dialog.querySelector(".mc-container");
        this.closeBtn = this.content.querySelector(".title > .close");

        console.log("Dialog created:", this.dialog, this.content, this.closeBtn);
        console.log(this.content)

        this.dialog.addEventListener("click", (e) => {
            if (e.target === e.currentTarget) {
                this.close();
            }
        });

        this.closeBtn.addEventListener("click", (e) => {
            this.close();
        });
    }

    open() {
        this.content.classList.remove("collapse-and-hide-box");
        this.content.classList.add("show-and-expand-box");
        this.dialog.showModal();
    }

    close() {
        this.content.classList.remove("show-and-expand-box");
        this.content.classList.add("collapse-and-hide-box");
        this.content.addEventListener("animationend", (function() { this.dialog.close() }).bind(this), { once: true });
    }
}

export default MCDialog;
