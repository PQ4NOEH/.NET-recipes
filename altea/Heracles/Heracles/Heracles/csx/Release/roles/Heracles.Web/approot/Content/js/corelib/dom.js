(function () {
    "use strict";

    Node.prototype.ancestor = function(match, first) {
        var node = this;
        var ancestors = [];

        if ((match = match.split(".")).length === 1) {
            match.push(null);
        } else if (!match[0]) {
            match[0] = null;
        }

        if (typeof first !== "boolean") {
            first = false;
        }

        if (typeof match[0] === "string") {
            match[0] = match[0].toLowerCase();
        }

        var classRegExp = new RegExp("( |^)(" + match[1] + ")( |$)");

        while ((node = node.parentNode) !== null) {
            if (
                (!match[0] || match[0] === node.nodeName.toLowerCase())
                    && (!match[1] || classRegExp.test(node.className))
            ) {
                ancestors.push(node);
                if (first) {
                    break;
                }
            }
        }

        return (first ? ancestors[0] : ancestors);
    };



    function domToArray() {
        var collection = this,
            length = collection.length,
            result = new Array(length);

        for (var i = 0; i < length; i++) {
            result[i] = collection[i];
        }

        return result;
    }

    HTMLCollection.prototype.toArray = domToArray;
    NodeList.prototype.toArray = domToArray;

    function domForEach() {
        var that = this.toArray();
        Array.prototype.forEach.apply(that, arguments);
    }

    Node.prototype.forEach = HTMLCollection.prototype.forEach = domForEach;

    ["map", "filter", "reduce", "reduceRight", "every", "some"].forEach(
    function (p) {
        NodeList.prototype[p] = HTMLCollection.prototype[p] = Array.prototype[p];
    });
})();