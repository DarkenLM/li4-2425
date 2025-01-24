import AudioPool from "../util/audioUtil.js";
import CSSUtil from "../util/cssUtil.js";
import MCButton from "./mcui/MCButton.js";
import MCDialog from "./mcui/MCDialog.js";
import MCInput from "./mcui/MCInput.js";
import MCSlider from "./mcui/MCSlider.js";
import MCSlot from "./mcui/MCSlot.js";
import MCTooltip from "./mcui/MCTooltip.js";
window.CSSUtil = new CSSUtil();

// const clickSound = new AudioPool("./sound/click.mp3", 5);

document.addEventListener("DOMContentLoaded", function() {
    const container = document.querySelector(".mc-container");
    // bouncyBox();
    // open(container);

    let open = true;
    function toggle() {
        if (open) {
            container.classList.remove("show-and-expand-box");
            container.classList.add("collapse-and-hide-box");
        } else {
            container.classList.remove("collapse-and-hide-box");
            container.classList.add("show-and-expand-box");
        }

        open = !open;
    }

    // container.addEventListener("click", function() {
    //     toggle();
    // });

    // setInterval(() => {
    //     toggle();
    // }, 2000);






    // /** @type {HTMLDialogElement} */
    // const innerContainer = document.getElementById("innerContainer");
    // const icContent = innerContainer.querySelector(".mc-container");
    // innerContainer.addEventListener("click", function(e) {
    //     if (e.target === e.currentTarget) {
    //         const container = icContent;
    //         container.addEventListener("animationend", function() {
    //             e.target.close()
    //         }, { once: true });
    //         container.classList.remove("show-and-expand-box");
    //         container.classList.add("collapse-and-hide-box");
    //     }
    // });
    const innerContainer = document.getElementById("innerContainer");
    const dialog = new MCDialog(innerContainer);

    const TEXTS = [
        "Hello",
        "Blocky",
        "World"
    ];

    const buttons = [];
    const button = document.querySelectorAll(".mc-button");
    button.forEach((b, i) => {
        if (i === 0) buttons.push(new MCButton(b).onClick(function (e) {
            if (!this.hasState("text")) {
                this.setState("text", 1);
            }

            const text = TEXTS[this.getState("text")];
            console.log("IND:", this.getState("text"), "TEXT:", text);

            this.setText(text);
            this.setState("text", (this.getState("text") + 1) % TEXTS.length);
        }));
        else if (i === 1) {
            buttons.push(new MCButton(b).onClick(function (e) {
                // icContent.classList.remove("collapse-and-hide-box");
                // icContent.classList.add("show-and-expand-box");
                // innerContainer.showModal();
                dialog.open();
            }));
        } else buttons.push(new MCButton(b));

        // bouncyBox(b, 200, 25 * i);
    });

    const slots = [];
    const slot = document.querySelectorAll(".mc-slot");
    slot.forEach((s, i) => {
        slots.push(new MCSlot(s));
    });

    // document.querySelectorAll(".mc-textarea").forEach((t, i) => {
    //     bouncyBox(t, 200, 50 * i);
    // });

    // document.querySelectorAll(".mc-slot").forEach((t, i) => {
    //     bouncyBox(t, 50, 33 * i);
    // });

    document.querySelectorAll(".mc-progress-bar").forEach((t, i) => {
        const bar = t.querySelector(".mc-progress-bar-fill");

        let bounce = i % 2 === 0;
        let progress = i % 2 === 0 ? 0 : 100;
        setInterval(() => {
            if (bounce) {
                if (progress <= 0) {
                    bounce = false;
                    progress = 0;
                } else progress -= 1;
            } else {
                if (progress >= 100) {
                    bounce = true;
                    progress = 100;
                } else progress += 1;
            }

            bar.style.setProperty("--progress", `${progress}%`);
        }, 20)
    });

    /** @type {MCSlider[]} */
    const sliders = [];
    document.querySelectorAll(".mc-slider").forEach((t, i) => {
        sliders.push(new MCSlider(t));
    });

    new MCInput(document.getElementById("sliderChevrons"))
        .onInput(function (e) {
            sliders[0].setChevrons(parseInt(this.elem.value));
        });

    // MCTooltip.getGlobalTooltip().followMouse().showTooltip("Hello World", 0, 0);

    document.addEventListener("keydown", function (e) {
        if (e.key === "a") {
            MCTooltip.getGlobalTooltip().stopFollowMouse();
        }
    });
});

function bouncyBox(element, f, offset) {
    element ??= document.querySelector(".mc-container");

    f ??= 800;
    let t = offset ?? 0;
    
    const update = () => {
        t += 1;
        // container.style.transform = `translateX(${f * Math.sin(t / 100)}px)`;
        const w = Math.abs(f * Math.sin(t / 100));
        const h = Math.abs(f * Math.cos((t / 100) + (f / 2)));

        // console.log("w", w, "h", h);

        element.style.width = `${w}px`;
        element.style.height = `${h}px`;
        requestAnimationFrame(update);
    }

    update();
}

/**
 *
 *
 * @param {HTMLDivElement} container
 * @param {number} [time=1000]
 */
function open(container, time = 1000) {
    let t = 0;

    const targetW = parseInt(container.getAttribute("data-width") ?? "100", 10);
    const targetH = parseInt(container.getAttribute("data-height") ?? "100", 10);
    const stepW = (targetW - 0) / time;
    const stepH = (targetH - 0) / time;

    const update = () => {
        t += 1;
        const w = Math.min(targetW, stepW * t);
        const h = Math.min(targetH, stepH * t);

        container.style.width = `${w}px`;
        container.style.height = `${h}px`;

        if (t < time) {
            requestAnimationFrame(update);
        }
    }

    update();

}