import MCButton from "../mcui/MCButton.js";
import MCDialog from "../mcui/MCDialog.js";

document.addEventListener('DOMContentLoaded', function() {
    //#region ======= Nuke Dialog =======
    const nukeDialogElem = document.getElementById("nuke_dialog");
    const nukeDialog = new MCDialog(nukeDialogElem);

    const nukeDialogQueueIdElem = document.getElementById("nuke_dialog_queue_id");
    const nukeDialogQueueElems = document.getElementById("nuke_dialog_queue_elems");

    const nukeConfirmBtn = new MCButton(document.getElementById("nuke_it_btn")).onClick(function(e) {
        console.log("ALLAHU AKBAR");
        nukeDialog.close();
    });

    const nukeCancelBtn = new MCButton(document.getElementById("nuke_cancel_btn")).onClick(function(e) {
        console.log("Cancel nuke");
        nukeDialog.close();
    });
    //#endregion ======= Nuke Dialog =======

    const _subBtns = document.getElementsByName("subtract_btn");
    const _nukeBtns = document.getElementsByName("nuke_btn");

    const subBtns = /** @type {MCButton[]} */ [];
    const nukeBtns = /** @type {MCButton[]} */ [];

    _subBtns.forEach((btn) => {
        const queueId = parseInt(btn.parentElement?.parentElement?.parentElement?.parentElement?.getAttribute("data-queue-id") ?? "-1");

        subBtns.push(new MCButton(btn).onClick(function(e) {
            console.log(`Subtracting from queue ${queueId}`);
        }));
    });

    _nukeBtns.forEach((btn) => {
        const queueId = parseInt(btn.parentElement?.parentElement?.parentElement?.parentElement?.getAttribute("data-queue-id") ?? "-1");
        const queueElems = parseInt(btn.parentElement?.parentElement?.parentElement?.parentElement?.getAttribute("data-queue-elems") ?? "-1");

        nukeBtns.push(new MCButton(btn).onClick(function(e) {
            console.log(`Nuking queue ${queueId}`);

            nukeDialogQueueIdElem.textContent = queueId.toString();
            nukeDialogQueueElems.textContent = queueElems.toString();

            nukeDialog.open();
        }));
    });
});