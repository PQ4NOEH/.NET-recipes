jQuery.fn.highlight = function (pattern, color, className) {
    (typeof className !== "string") && (className == "");

    var regex = new RegExp("\\b" + pattern + "\\b", "ig");
    var patternLength = pattern.length;

    function innerHighlight(node) {
        if (node.nodeType === 3 && node.data.length > 0) {
            var pos = node.data.search(regex);

            if (pos >= 0) {
                var spanNode = document.createElement("altea-wiselab-span");
                spanNode.className = ("altea-wiselab-span-word " + className).trim();
                spanNode.setAttribute("style", "background-color: " + color + ";");

                var middleBit = node.splitText(pos);
                middleBit.splitText(patternLength);

                var middleClone = middleBit.cloneNode(true);
                spanNode.appendChild(middleClone);
                middleBit.parentNode.replaceChild(spanNode, middleBit);
                return 1;
            }
        } else if (node.nodeType === 1 && node.childNodes && !node.tagName.match(/(script|style)/i)) {
            for (var j = 0, k = node.childNodes.length; j < k; j++)
                j += innerHighlight(node.childNodes[j], pattern, color);
        }
        return 0;
    }

    for (var i = 0, l = this.length; i < l; i++) {
        i += innerHighlight(this[i]);
    }
};

jQuery.fn.removeHighlight = function (text, className) {
    var elements = this.find(".wordimeterScoutPadHighlightedWord");

    if (typeof className === "string")
        className.trim();
    else
        className = "";

    if (className.length !== 0)
        elements = elements.filter(function (k, v) {
            return v.classList.contains(className);
            // return $(v).hasClass(className);
        });

    return elements.each(function () {
        if (this.innerText.toLowerCase() === text.toLowerCase()) {
        // if ($(this).text().toLowerCase() === text.toLowerCase()) {
            var parentNode = this.parentNode;
            var childNodes = this.childNodes;
            if (childNodes.length === 1) {
                parentNode.replaceChild(this.firstChild, this);
                this.normalize();
            } else {
                //var self = this;

                for (var i = 0, l = childNodes.length; i < l; i++) {
                    if (childNodes[i].nodeType !== 3 || childNodes[i].data.length !== 0) {
                        parentNode.replaceChild(childNodes[i], this);
                        this.normalize();
                        break;
                    }
                }

                // $.each(childNodes, function (k, v) {
                //     if (v.nodeType !== 3 || v.data.length !== 0) {
                //         parentNode.replaceChild(v, self);
                //         self.normalize();
                //         return false;
                //     }
                //     return true;
                // });
            }
        }
    });
}
