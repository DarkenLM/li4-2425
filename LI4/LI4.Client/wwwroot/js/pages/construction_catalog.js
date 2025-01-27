import MCInput from "../mcui/MCInput.js";

const PROXY_EVENTS = {};
function proxyAddEventListener(elem, event, handler) {
    if (!PROXY_EVENTS[event]) PROXY_EVENTS[event] = { elem: elem, listeners: []};
    PROXY_EVENTS[event].listeners.push(handler);

    elem.addEventListener(event, handler);
}

function disposeEventListeners() {
    for (const event in PROXY_EVENTS) {
        const handlers = PROXY_EVENTS[event].listeners;
        handlers.forEach((handler) => {
            PROXY_EVENTS[event].elem.removeEventListener(event, handler);
        });
    }
}

// document.addEventListener("DOMContentLoaded", function() {
window.__BRIDGE_CONSTRUCTION_CATALOG = function() {
    console.log("======= Construction Catalog Script =======");

    const addBtn = document.getElementById("add_btn");
    const subtractBtn = document.getElementById("subtract_btn");
    const countInput = document.getElementById("count_input");
    const nukeBtn = document.getElementById("nuke_btn");
    const confirmBtn = document.getElementById("confirm_btn");

    const materialList = document.getElementById("material_list");

    countInput.value = 1;
    _updateConfirmBtn(updateCosts(materialList, countInput.value), confirmBtn);

    proxyAddEventListener(addBtn, "click", function() {
        let value = Math.min(99, parseInt(countInput.value, 10) + 1);
        if (isNaN(value)) value = 1;

        countInput.value = value;
        _updateConfirmBtn(updateCosts(materialList, value), confirmBtn);

        // confirmBtn.disabled = false;
    });

    proxyAddEventListener(subtractBtn, "click", function() {
        let value = Math.max(1, parseInt(countInput.value, 10) - 1);
        if (isNaN(value)) value = 1;

        countInput.value = value;
        _updateConfirmBtn(updateCosts(materialList, value), confirmBtn);
    });

    proxyAddEventListener(nukeBtn, "click", function() {
        countInput.value = 1;
        _updateConfirmBtn(updateCosts(materialList, 1), confirmBtn);

        // const itemElems = materialList.querySelectorAll("[data-item]");
        // itemElems.forEach((itemElem) => {
        //     itemElem.classList.remove("unavailable");
        // });

        confirmBtn.disabled = false;
    });

    const countInputElem = new MCInput(countInput);
    countInputElem.onInput(function() {
        if (countInput.value === "") confirmBtn.disabled = false;

        let value = Math.max(1, Math.min(99, parseInt(countInput.value, 10)));

        if (!isNaN(value)) {
            confirmBtn.disabled = false;
            countInput.value = value;
            _updateConfirmBtn(updateCosts(materialList, value), confirmBtn);
        } else {
            confirmBtn.disabled = true;
        }
    });
};

window.__BRIDGE_CONSTRUCTION_CATALOG_DISPOSE = function() {
    disposeEventListeners();
};

function updateCosts(materialList, count) {
    const itemElems = materialList.querySelectorAll("[data-item]");
    let allAvailable = true;

    itemElems.forEach((itemElem) => {
        const availableElem = itemElem.querySelector("[name=available]");
        const totalElem = itemElem.querySelector("[name=total]");

        const available = parseInt(availableElem.textContent, 10);
        const itemCost = itemElem.getAttribute("data-cost");
        const totalCost = itemCost * count;

        totalElem.textContent = totalCost;

        if (available < totalCost) {
            itemElem.classList.add("unavailable");
            allAvailable = false;
        } else {
            itemElem.classList.remove("unavailable");
        }
    });

    return allAvailable;
}

function _updateConfirmBtn(allAvailable, confirmBtn) {
    console.log("All available:", !allAvailable, confirmBtn);
    confirmBtn.disabled = !allAvailable;
}

console.log("Construction catalog loaded.");