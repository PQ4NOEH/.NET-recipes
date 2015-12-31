window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        manager: $("#stk-lst-ctn"),
        managerSpinner: $("#stk-lst-spn"),
        managerContents: $("#stk-lst-dc"),
        managerNoLists: document.getElementById("stk-lst-nlt")
    });

    var api = Object.create({
        getUserLists: "/Lists/Lists"
    });

    window.lists = Object.create({
        types: Object.create({
            vocabulary: 1,
            termDefis: 3
        }),

        getUserLists: function(type) {
            dom.managerSpinner.addClass("app-spinner-active");

            $.ajax({
                async: true,
                type: "POST",
                url: api.getUserLists,
                data: { id: type },
                dataType: "json",

                success: function(result) {
                    if (result.lists.length === 0) {
                        dom.managerNoLists.classList.add("stk-lst-nlt-act");
                    } else {
                        for (var i = 0, l = result.lists.length; i < l; i++) {
                            drawList(result[i]);
                        }

                        dom.managerContents.mCustomScrollbar({
                            axis: "y",
                            scrollbarPosition: "inside",
                            alwaysShowScrollbar: 0,
                            scrollButtons: { enable: true },
                            theme: "dark-3"
                        });
                    }

                    dom.managerSpinner.removeClass("app-spinner-active");
                },

                error: function() {
                    window.setInternalErrorMessage(dom.manager);
                }
            });
        }
    });

    function drawList(data) {
        console.log(data);
    }
});
