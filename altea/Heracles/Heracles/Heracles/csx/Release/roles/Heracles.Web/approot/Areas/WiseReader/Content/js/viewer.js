window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var load = false;

    // ReSharper disable InconsistentNaming
    var WISELAB_ORIGIN = 5;
    // ReSharper restore InconsistentNaming

    window.wisereader = {
        loaded: function() {
            load = true;

            var iframe = document.getElementById("wr-fli");
            var iframeDocument = iframe.contentDocument;
            var wiselabContainer = $(iframeDocument.getElementById("viewer"));


            window.wiselab.setup({
                container: wiselabContainer,
                frame: false,
                frameContents: iframe,
                live: true,
                confirmFinish: true,
                canSearch: true,
                hasDoneButton: true,
                stepCallback: function() {},
                finishcallback: function() {}
            }, function() {
                window.wiselab.get(WISELAB_ORIGIN, window.model.referenceId, function() {
                    window.wiselab.start();
                }, null, null);
            });
        }
    };
});
