(function () {
    "use strict";

    Date.parseUTC = function(string) {
        return new Date(string + " UTC");
    }
})();