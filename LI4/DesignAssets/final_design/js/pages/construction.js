/**
 * Base file for rendering 3D constructions.
 */

import MCSlider from "../mcui/MCSlider.js";
import MCCheckbox from "../mcui/MCCheckbox.js";
import MCStruct from "../mcui/MCStruct.js";
import MC3D from "../mcui/MC3D.js";
import { loadJSON5Module } from "../util/json5Util.js";

const NO_PROCESSING = "noprocessing";
const NO_RENDER = "norender";
const RENDER = "render";

let stopped = false;
let autoTick = false;
let repeat = true;
let snapToStages = false;
/** @type {MC3D | undefined} */
let mc3d = undefined;
/** @type {MCSlider | undefined} */
let controlSlider; 
/** @type {MCCheckbox[]} */
const controlCheckboxes = [];

document.addEventListener("DOMContentLoaded", async function() {
    if (!WebGLRenderingContext) {
        console.error("WebGL not supported");
        return;
    }

    const renderElem = document.getElementById("render3d");
    const constructionElem = document.getElementById("construction");
    const masterElem = constructionElem.parentElement.parentElement;

    if (!renderElem) {
        console.error("No render3d element found");
        return;
    }
    if (!constructionElem) {
        console.error("No construction element found");
        return;
    }

    const renderMode = constructionElem.getAttribute("data-render");
    if (!constructionElem.hasAttribute("data-render") || renderMode === NO_PROCESSING) {
        console.log("Rendering disabled");
        return;
    }

    const constructionName = constructionElem.getAttribute("data-construction");
    if (!constructionName) {
        console.error("No construction name found");
        return;
    }

    try {
        const constructionPath = new URL(`../mcstruct/constructions/${constructionName}.json`, document.location.href);
        const modelData = await loadJSON5Module(constructionPath);
        const struct = new MCStruct(modelData);

        //#region ------- Setup MC3D -------
        mc3d = new MC3D({
            size: { width: renderElem.offsetWidth, height: renderElem.offsetHeight },
            modelLibraryPath: new URL("../mcstruct/blocks/", document.location.href),
            textureLibraryPath: new URL("../", document.location.href),
        });
        window.addEventListener("resize", () => {
            mc3d.resize(renderElem.offsetWidth, renderElem.offsetHeight);
        });
        //#endregion ------- Setup MC3D -------

        //#region ------- Debug -------
        // mc3d.addGrid(12, 16);
        mc3d.mcrender._debugTextures(mc3d.scene);
        //#endregion ------- Debug -------

        //#region ------- Load Construction -------
        await mc3d.loadStructure(struct);
        mc3d.prepare();
        //#endregion ------- Load Construction -------

        //#region ------- GUI Setup -------
        const controls = masterElem.querySelector(".construction-controls");

        if (controls) {
            // Setup slider
            const sliderContainer = controls.querySelector(".slider");
            const slider = sliderContainer.querySelector(".mc-slider");

            if (sliderContainer && slider) {
                const sliderWidth = sliderContainer.offsetWidth;
                slider.style.setProperty("--width", `${sliderWidth}px`);

                const mcSlider = new MCSlider(slider);
                // mcSlider.setChevrons(mc3d.mcrender._stageTicks.length, true);
                mcSlider.setChevrons([
                    0, 
                    ...mc3d.mcrender._stageTicks.map((t, i) => t + mc3d.mcrender._ticksPerBlock * i).slice(0, -1), 
                    mc3d.mcrender._totalTicks
                ], true);
                mcSlider.onInput(function(e) {
                    console.log("Slider input", this.getProgress());
                    mc3d.setTick(this.getProgress());
                });
                mcSlider.enable();
                slider.style.visibility = "visible";

                controlSlider = mcSlider;
            } else {
                console.warn("No slider found. Defaulting to auto-tick");
                autoTick = true;
            }

            // Setup checkboxes
            const checkboxes = controls.querySelectorAll(".mc-checkbox");
            checkboxes.forEach((elem, i) => {
                const checkbox = new MCCheckbox(elem);
                checkbox.onClick(function(e) {
                    console.log("Checkbox clicked", !this.isChecked());
                    console.log("Control:", e.getAttribute("data-control"));

                    const control = e.getAttribute("data-control");
                    if (control === "autotick") {
                        autoTick = !this.isChecked();

                        if (autoTick) {
                            controlSlider.setSnapToChevrons(false);
                            controlSlider.disable();
                        } else {
                            controlSlider.setSnapToChevrons(snapToStages);
                            controlSlider.enable();
                        }
                    } else if (control === "repeat") {
                        repeat = !this.isChecked();

                        if (stopped) {
                            mc3d.setTick(0);
                            stopped = false;
                            mc3d.start(animate);
                        }
                    } else if (control === "snap-stage") {
                        snapToStages = !this.isChecked();
                        controlSlider.setSnapToChevrons(snapToStages);
                    }
                });

                const control = elem.getAttribute("data-control");
                if (control === "autotick") checkbox.setChecked(autoTick);
                else if (control === "repeat") checkbox.setChecked(repeat);
                else if (control === "snap-stage") checkbox.setChecked(controlSlider._snapToChevrons);

                checkbox.enable();
                controlCheckboxes.push(checkbox);
            });
        } else {
            console.warn("No construction controls found. Defaulting to auto-tick");
            autoTick = true;
        }
        //#endregion ------- GUI Setup -------

        //#region ------- Start -------
        mc3d.addPlot(modelData.meta.size.x ?? 16, modelData.meta.size.z ?? 16, 16);

        console.log("Model Meta:", modelData.meta);

        const cameraAngles = [
            ((modelData.meta.size.x / 2)) * 16 - 8,
            ((modelData.meta.size.y / 2)) * 16 - 8,
            ((modelData.meta.size.z / 2)) * 16 - 8
        ];
        const cameraPosition = [
            -((modelData.meta.size.x / 16) + 0) * 16 - 8,
            ((modelData.meta.size.y / 1) + 2) * 16 - 8,
            -((modelData.meta.size.z / 2) + 0) * 16 - 8
        ];

        mc3d.setCameraLookAt(...cameraAngles);
        mc3d.setCameraPosition(...cameraPosition);

        console.log("Camera Angles:", cameraAngles.map(v => v / 16), cameraAngles);
        console.log("Camera Positions:", cameraPosition.map(v => v / 16), cameraPosition);

        if (renderMode === NO_RENDER) {
            console.log("Rendering disabled");
            mc3d.dispose();
            return;
        }

        mc3d.attach(renderElem);
        mc3d.start(animate);
        //#endregion ------- Start -------
    } catch (e) {
        console.error("Unable to initialize 3d render:", e);
    }
});

function animate(time) {
    if (autoTick && !stopped) {
        if (mc3d.mcrender._currentTick >= mc3d.mcrender._totalTicks) {
            if (repeat) {
                // mc3d.setTick(0);
                mc3d.tick();
            } else {
                // mc3d.stop();
                stopped = true;
                return;
            }
        } else {
            mc3d.tick();
        }

        // mc3d.tick();
        controlSlider.setProgress(mc3d.mcrender._currentTick, true, true);
        console.log("TICK:", mc3d.mcrender.__currentTick, mc3d.mcrender._currentTick);
    }
}