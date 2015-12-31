window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        container: document.getElementById("dn-wisenet"),
        spinner: $(document.getElementById("dn-wisenet-spn"))
    });

    var api = Object.create({
        data: "/Dean/GetWiseNet"
    });

    var dataLoaded = false,
        loadingData = false,
        errorLoading = false;

    var typeSelected = false,
        areaSelected = false;

    var searchEngines, magazines;

    function loadWiseNetData() {
        loadingData = true;

        $.ajax({
            async: true,
            type: "POST",
            url: api.data,

            success: function(result) {
                searchEngines = result.searchEngines;
                magazines = result.magazines;

                dom.spinner.removeClass("app-spinner-active");

                dataLoaded = true;
                loadingData = false;
            },

            error: function() {
                errorLoading = true;

                dom.container.style.display = "table";
                window.setInternalErrorMessage($(dom.container));

                dom.spinner.removeClass("app-spinner-active");
            }
        });
    }

    window.dean.loadWiseNetData = function() {
        if (!dataLoaded && !loadingData) {
            loadWiseNetData();
        }
    }

    window.dean.loadMemberNewsStand = function() {
        if (errorLoading) {
            return;
        }
    }
});
