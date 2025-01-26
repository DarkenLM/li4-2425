const UNITS = ["px", "%", "em", "rem", "vw", "vh", "vmin", "vmax", "cm", "mm", "in", "pt", "pc", "ch", "ex"];

const WHITESPACE = (c) => /\s/.test(c);

class InputStream {
    /** @type {string} */
    input;

    constructor(input) {
        this.input = input;
        this.pos = 0;
    }

    next() {
        return this.input.charAt(this.pos++);
    }

    peek() {
        return this.input.charAt(this.pos);
    }

    peekN(n) {
        return this.input.slice(this.pos, this.pos + n);
    }

    eof() {
        return this.peek() === "";
    }

    error(msg) {
        throw new Error(`${msg} (at ${this.pos})`);
    }
}

class CSSUtil {
    // getTrueValue(element, property) {
    //     if (!element || !property) {
    //         throw new Error("Both element and property must be provided.");
    //     }
    
    //     // Retrieve the computed style for the property
    //     const style = getComputedStyle(element);
    //     let value = style.getPropertyValue(property).trim();
    
    //     if (value.startsWith("calc(")) {
    //         // Remove "calc(" and ")" to get the inner expression
    //         const calcExpression = value.slice(5, -1);
    
    //         // Evaluate the expression
    //         // Replace variable references and helper functions as needed
    //         const resolvedValue = calcExpression
    //             .replace(/(\d+)\s*px/g, (_, num) => parseFloat(num)) // Convert "50px" to 50
    //             .replace(/round\(([^)]+)\)/g, (_, inner) => Math.round(eval(inner))) // Handle round function
    //             .replace(/(\d+)%/g, (_, percent) => {
    //                 // Handle percentages relative to the parent element
    //                 const parentValue = parseFloat(getComputedStyle(element.parentElement).getPropertyValue(property) || 0);
    //                 return (parseFloat(percent) / 100) * parentValue;
    //             });
    
    //         // Use `eval` safely to compute the final value
    //         try {
    //             value = eval(resolvedValue);
    //         } catch (e) {
    //             console.error("Failed to evaluate calc expression:", calcExpression, e);
    //             return null;
    //         }
    //     }
    
    //     return value;
    // }    

    getTrueValue(element, property) {
        const propValue = getComputedStyle(element).getPropertyValue(property).trim();
        return this._format(this.parseValue(propValue));
    }

    parseValue(value) {
        const input = new InputStream(value);
        return this._parseExpr(input);
    }

    /**
     * 
     * @param {InputStream} input 
     */
    _parseExpr(input) {
        const res = [];
        let prevPos = input.pos;

        while (!input.eof()) {
            let c = input.peek();

            if (/[a-z]/.test(c)) {
                res.push(this._parseFunc(input));
            } else {
                res.push(this._parseUnit(input));
            }

            this._trim(input);
            if (prevPos === input.pos) break;
            prevPos = input.pos;
        }

        return res;
    }

    _parseUnit(input) {
        const value = this._readWhile(input, c => /[\.\-\d]/.test(c));
        const _unit = this._readWhile(input, c => /[a-z]/.test(c));
        let unit;

        if (_unit === "") unit = "none";
        else if (!UNITS.includes(_unit)) {
            input.error("Invalid unit");
        }

        unit = _unit;

        return { type: "UNIT", value: parseFloat(value), unit: unit };
    }

    _parseFunc(input) {
        const funcName = this._readWhile(input, c => c !== "(");
        input.next();

        console.log("FUNC NAME:", funcName);

        return { type: "FUNC", name: funcName, args };
    }

    /**
     * 
     * @param {*} ast 
     */
    _format(ast) {
        return ast.map(node => {
            if (node.type === "UNIT") {
                return `${node.value}${node.unit}`;
            } else if (node.type === "FUNC") {
                return `${node.name}(${node.args.join(", ")})`;
            }
        }).join(" ");
    }

    /**
     * 
     * @param {InptuStream} input 
     * @param {Predicate} predicate 
     * @returns 
     */
    _readWhile(input, predicate) {
        let result = "";
        while (!input.eof() && predicate(input.peek())) {
            result += input.next();
        }

        return result;
    }

    _trim(input) {
        while (!input.eof() && WHITESPACE(input.peek())) {
            input.next();
        }
    }
}

export default CSSUtil;
export const cssutil = new CSSUtil();

/**
 * @callback Predicate
 * @param {string} c
 * @returns {boolean}
 */
