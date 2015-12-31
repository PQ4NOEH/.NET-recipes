(function () {
    "use strict";

    Number.prototype.padZeros = function(size) {
        size = size || 2;

        var str = this + "";

        while (str.length < size) {
            str = "0" + str;
        }

        return str;
    }
})();