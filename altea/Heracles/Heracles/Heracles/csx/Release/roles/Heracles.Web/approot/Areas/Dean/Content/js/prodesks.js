window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        container: document.getElementById("dn-prodesks"),
        spinner: $(document.getElementById("dn-prodesks-spn")),
        content: document.getElementById("pec-c")
    });

    var api = Object.create({
        data: "/Dean/GetProDesks"
    });

    var MEMBERS_ENUM = {
        0: "User",
        1: "Teacher"
    };

    var QUESTIONTYPES_ENUM = {
        User: 3,
        Teacher: 4
    };

    var data = [],
        dataLoaded = [],
        loadingData = [],
        errorLoading = false;

    var memberLoaded = -1,
        levelLoaded = -1,
        subLevelLoaded = -1;

    data[0] = []; // Users
    dataLoaded[0] = [];
    loadingData[0] = [];

    function drawProDesksData(result) {
        while (dom.content.firstChild) {
            dom.content.removeChild(dom.content.firstChild);
        }

        if (result === undefined) {
            var d = data[window.dean.member][window.dean.level][window.dean.sublevel];
            for (var i = 0, l = d.length; i < l; i++) {
                dom.content.appendChild(d[i]);
            }
        } else {
            if (!data[window.dean.member][window.dean.level]) {
                data[window.dean.member][window.dean.level] = [];
            }

            data[window.dean.member][window.dean.level][window.dean.sublevel] =
                window.prodesks.draw(
                    window.model.proColumns,
                    result,
                    dom.content);
        }
    }

    function loadProDesksData() {
        var member = window.dean.member;
        var level = window.dean.level;
        var sublevel = window.dean.sublevel;

        loadingData[member][level] = true;

        $.ajax({
            async: true,
            type: "POST",
            url: api.data,
            data: {
                level: level,
                sublevel: sublevel,
                type: QUESTIONTYPES_ENUM[MEMBERS_ENUM[member]]
            },

            success: function(result) {
                if (window.dean.type === 1
                    && (window.dean.member === member && window.dean.level === level && window.dean.sublevel === sublevel)) {
                    drawProDesksData(result);
                }

                dom.spinner.removeClass("app-spinner-active");

                if (!dataLoaded[member][level]) {
                    dataLoaded[member][level] = [];
                }

                dataLoaded[member][level][sublevel] = true;

                if (!loadingData[member][level]) {
                    loadingData[member][level] = [];
                }

                loadingData[member][level][sublevel] = false;
            },

            error: function() {
                errorLoading = true;
                dom.container.style.display = "table";
                window.setInternalErrorMessage($(dom.container));

                dom.spinner.removeClass("app-spinner-active");
            }
        });
    }

    window.dean.loadProDesksData = function() {
        if (errorLoading) {
            return;
        }

        if (window.dean.member !== memberLoaded || window.dean.level !== levelLoaded) {
            if (
                (!dataLoaded[window.dean.member][window.dean.level] ||
                        !dataLoaded[window.dean.member][window.dean.level][window.dean.sublevel])
                    && (!loadingData[window.dean.member][window.dean.level] ||
                        !loadingData[window.dean.member][window.dean.level][window.dean.sublevel])) {
                dom.spinner.addClass("app-spinner-active");
                loadProDesksData();
            } else if (dataLoaded[window.dean.member][window.dean.level]
                && dataLoaded[window.dean.member][window.dean.level][window.dean.sublevel]) {
                drawProDesksData();
            }
        }
    }

    window.dean.loadMemberProDesks = function() {
        if (errorLoading) {
            return;
        }
    }

    window.dean.showProDesksAssignments = function(assigned, finished, certified) {

    }
});
