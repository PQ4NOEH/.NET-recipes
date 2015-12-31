// ReSharper disable MissingHasOwnPropertyInForeach
// Because ReSharper doesn't know how to call hasOwnProperty from prototype
// https://developer.mozilla.org/en-US/docs/Web/API/window.btoa
function base64_encode(str, escapedChars) {
    var base64 = window.btoa(unescape(encodeURIComponent(str)));

    if (typeof escapedChars === "object") {
        for (var character in escapedChars) {
            if (Object.prototype.hasOwnProperty.call(escapedChars, character)) {
                var c = character;
                if (c === "\\" || c === "+" || c === "*" || c === "?" || c === "|" || c === "#"
                    || c === "{" || c === "}" || c === "[" || c === "]" || c === "(" || c === ")"
                    || c === "^" || c === "$" || c === ".") {
                    c = "\\" + c;
                }
                base64 = base64.replace(new RegExp(c, "g"), escapedChars[c]);
            }
        }
    }

    return base64;
}

function base64_decode(str, escapedChars) {
    if (typeof escapedChars === "object") {
        for (var character in escapedChars) {
            if (Object.prototype.hasOwnProperty.call(escapedChars, character)) {
                var c = escapedChars[character];
                if (c === "\\" || c === "+" || c === "*" || c === "?" || c === "|" || c === "#"
                    || c === "{" || c === "}" || c === "[" || c === "]" || c === "(" || c === ")"
                    || c === "^" || c === "$" || c === ".") {
                    c = "\\" + c;
                }
                str = str.replace(new RegExp(c, "g"), character);
            }
        }
    }

    return decodeURIComponent(escape(window.atob(str)));
}
// ReSharper restore MissingHasOwnPropertyInForeach
