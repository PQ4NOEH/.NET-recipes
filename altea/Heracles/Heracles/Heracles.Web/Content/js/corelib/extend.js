(function () {
    "use strict";

    Object.extend = function (source, destination, replace) {
        for (var property in source) {
            if (Object.prototype.hasOwnProperty.call(source, property)) {
                if (replace || !Object.prototype.hasOwnProperty.call(destination, property)) {
                    destination[property] = source[property];
                }
            }
        }
    };
})();