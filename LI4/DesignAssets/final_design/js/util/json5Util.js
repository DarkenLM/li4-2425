import JSON5 from "json5";

async function loadJSON5Module(url) {
    const response = await fetch(url);
    const text = await response.text();
    return JSON5.parse(text);
}

export { loadJSON5Module };
