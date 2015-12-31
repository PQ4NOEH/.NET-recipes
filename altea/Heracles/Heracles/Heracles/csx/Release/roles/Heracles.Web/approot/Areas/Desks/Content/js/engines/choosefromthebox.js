window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var type = window.desks.engine.types.INDEX;
    var data;

    function loadCallback(result) {
        data = result;
        window.desks.engine.start();
    }

    function startCallback(data) {

    }

    window.desks.engine.load(type, window.model.id, loadCallback, startCallback);
});
