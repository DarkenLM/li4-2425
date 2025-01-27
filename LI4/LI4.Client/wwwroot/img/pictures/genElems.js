import { readdir, writeFile } from "fs/promises";
import path from "path";

const template = `
<div class="mc-slot stock-slot">
    <div class="mc-item">
        <div class="icon">
            <img src="../img/pictures/{{NAME}}.png" alt="{{NAME_PROPER}}">
        </div>
        <div class="stock-item-name">{{NAME_PROPER}}</div>
        <div class="mc-item-count">{{RAND(0, 100)}}</div>
    </div>
</div>
`;

let formatted = "";
const files = await readdir(".", { withFileTypes: true });
for (const file of files) {
    if (!file.isFile() || !file.name.endsWith(".png")) continue;
    const name = file.name.slice(0, -4);
    const content = template
        .replace(/{{NAME}}/g, name)
        .replace(/{{NAME_PROPER}}/g, (name[0].toUpperCase() + name.slice(1)).replace(/_/g, " "))
        .replace(/{{RAND\((\d+), (\d+)\)}}/g, (_, min, max) => Math.floor(Math.random() * (max - min + 1) + min));

    formatted += content;
    formatted += "\n";
}

await writeFile(`genElems-${Date.now()}.html`, formatted, { encoding: "utf8" });