import * as THREE from "three";
import { OrbitControls } from "three/addons/controls/OrbitControls.js";
import { GUI } from "three/addons/libs/lil-gui.module.min.js";
import MCStruct from "./js/mcui/MCStruct.js";
import MC3D from "./js/mcui/MC3D.js";

import { loadJSON5Module } from "./js/util/json5Util.js";

import smallTent1 from "./mcstruct/constructions/small_tent_1.json" with { type: "json" };
// import plainsVillageHouse5 from "./mcstruct/constructions/plains_village_house_5.json" with { type: "json" };
const plainsVillageHouse5 = await loadJSON5Module("./mcstruct/constructions/plains_village_house_5.json");
const plainsLibrary2 = await loadJSON5Module("./mcstruct/constructions/plains_library_2.json");
// const singleton = await loadJSON5Module("./mcstruct/constructions/singleton.json");

const _structs = [smallTent1, plainsVillageHouse5, plainsLibrary2];
const structs = _structs.map((s) => new MCStruct(s));
let structIndex = 0;

const struct = new MCStruct(smallTent1);
// const struct = new MCStruct(plainsVillageHouse5);
// const struct = new MCStruct(plainsLibrary2);
console.log("STRUCTURE:", struct);

const pVHStruct = new MCStruct(plainsVillageHouse5);

// Initialize MC3D
const mc3d = new MC3D({});
window.addEventListener("resize", () => {
    mc3d.resize(window.innerWidth, window.innerHeight);
});
mc3d.addGrid(12, 16);
// mc3d.setCameraPosition(5 * 16, 5 * 16, 10 * 16);

await mc3d.loadStructure(struct);
mc3d.prepare();

// //#region ------- Minecraft Orthographic Camera -------
// const aspect = window.innerWidth / window.innerHeight;
// const cameraSize = 20;
// mc3d.camera = new THREE.OrthographicCamera(
//     -cameraSize * aspect,
//     cameraSize * aspect,
//     cameraSize,
//     -cameraSize,
//     0.1,
//     1000
// );
// mc3d.camera.position.set(16, 13.75, 16);
// mc3d.camera.lookAt(0, 0, 0);
// mc3d.controls = new OrbitControls(mc3d.camera, mc3d.renderer.domElement);
// //#endregion ------- Minecraft Orthographic Camera -------

mc3d.mcrender._debugTextures(mc3d.scene);

//#region ============== GUI Setup ==============
let _GUI_selfupdate = false;
let GUI_autotick = false;
const gui = new GUI();
const param = {
    "Current Tick": 0,
    "Autotick": false,
};
const curTick = gui.add(param, "Current Tick", 0, mc3d.mcrender._totalTicks).listen().onChange(function (v) {
    if (_GUI_selfupdate) {
        _GUI_selfupdate = false;
        return;
    }
    mc3d.mcrender.__currentTick = v;
    mc3d.mcrender.setTick(v);
});
gui.add(param, "Autotick").onChange(function (v) {
    GUI_autotick = v;
});
//#endregion ============== GUI Setup ==============

function animate(time) {
    if (GUI_autotick) {
        this.mcrender.__currentTick++;
        this.mcrender._currentTick = this.mcrender._bounceValueBetween(this.mcrender.__currentTick, 0, this.mcrender._totalTicks);
        console.log("TICK:", this.mcrender.__currentTick, this.mcrender._currentTick);

        _GUI_selfupdate = true;
        param["Current Tick"] = this.mcrender._currentTick;
    }
}

// Attach to document and start rendering
mc3d.attach(document.body);
// mc3d.start(function (time) {
//     if (GUI_autotick) {
//         this.mcrender.__currentTick++;
//         this.mcrender._currentTick = this.mcrender._bounceValueBetween(this.mcrender.__currentTick, 0, this.mcrender._totalTicks);
//         console.log("TICK:", this.mcrender.__currentTick, this.mcrender._currentTick);

//         _GUI_selfupdate = true;
//         param["Current Tick"] = this.mcrender._currentTick;
//     }
// });
mc3d.start(animate);

document.addEventListener("keydown", async function (e) {
    if (e.key === " ") {
        GUI_autotick = !GUI_autotick;
        param["Autotick"] = GUI_autotick;
    } else if (e.key === "ArrowRight") {
        mc3d.mcrender.__currentTick += 5;
        mc3d.mcrender._currentTick = mc3d.mcrender._bounceValueBetween(mc3d.mcrender.__currentTick, 0, mc3d.mcrender._totalTicks);

        _GUI_selfupdate = true;
        param["Current Tick"] = mc3d.mcrender._currentTick;
    } else if (e.key === "ArrowLeft") {
        mc3d.mcrender.__currentTick -= 5;
        mc3d.mcrender._currentTick = mc3d.mcrender._bounceValueBetween(mc3d.mcrender.__currentTick, 0, mc3d.mcrender._totalTicks);

        _GUI_selfupdate = true;
        param["Current Tick"] = mc3d.mcrender._currentTick;
    } else if (e.key === "r") {
        mc3d.clearScene();
        await mc3d.loadStructure(pVHStruct);
        mc3d.prepare();

        _GUI_selfupdate = true;
        curTick
            .setValue(0)
            .max(mc3d.mcrender._totalTicks)
            .reset();

        mc3d.start(animate)
    } else {
        const numKey = parseInt(e.key);
        console.log("LOADING:", numKey);
        if (numKey >= 1 && numKey <= structs.length) {
            const struct = structs[numKey - 1];

            mc3d.clearScene();
            await mc3d.loadStructure(struct);
            mc3d.prepare();

            _GUI_selfupdate = true;
            curTick
                .setValue(0)
                .max(mc3d.mcrender._totalTicks)
                .reset();

            mc3d.start(animate);
        }
    }
});