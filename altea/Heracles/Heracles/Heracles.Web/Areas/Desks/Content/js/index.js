window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        spinner: $(document.getElementById("dsk-spn")),
        content: document.getElementById("dsk-idx-c")
    });

    var api = Object.create({
        list: "/Index/List",
        assignments: "/Index/Assignments",
        exercise: "/Index/Part/{level}/{type}/{area}/{subject}",
        theory: "/Index/Theory/{id}"
    });

    var index = new window.desks.index();

    var goingToData = false,
        loadingAssignments = false,
        firstAssignmentLoad = true,
        errorAssignmentsLoad = false;

    function loadAssignments() {
        if (goingToData) {
            return;
        }

        if (loadingAssignments) {
            setTimeout(loadAssignments, 1000);
            return;
        }

        loadingAssignments = true;

        if (errorAssignmentsLoad) {
            return;
        }

        $.ajax({
            async: true,
            type: "POST",
            url: api.assignments,
            data: {
                level: window.model.level.id
            },

            success: function(result) {
                index.assignments(result, true);

                if (firstAssignmentLoad) {
                    firstAssignmentLoad = false;
                    dom.spinner.removeClass("app-spinner-active");
                }

                setTimeout(loadAssignments, 6000);
                loadingAssignments = false;
            },

            error: function() {
                if (firstAssignmentLoad) {
                    window.setInternalErrorMessage($("#ct-c"));
                }

                errorAssignmentsLoad = true;
                loadingAssignments = false;
            }
        });
    }

    $.ajax({
        async: true,
        type: "POST",
        url: api.list,
        data: {
            level: window.model.level.id
        },

        success: function(result) {
            if (result === null) {
                window.setInternalErrorMessage($("#ct-c"));
            } else {
                index.draw(window.model.areas, result, dom.content);
                $(dom.content.getElementsByClassName("dsk-btn-c")).on("click", function() {
                    if (this.classList.contains("dsk-btn-as") && !goingToData) {
                        goingToData = true;

                        var ex = api.exercise;
                        ex = ex.replace("{level}", window.model.level.id);
                        ex = ex.replace("{type}", this.dataset.id);
                        ex = ex.replace("{area}", this.ancestor(".dsk-exs-row", true).dataset.id);
                        ex = ex.replace("{subject}", this.ancestor(".dsk-sbj", true).dataset.id);
                        window.location.href = ex;
                    }
                });

                $(dom.content.getElementsByClassName("dsk-thr")).on("click", function() {
                    if (!goingToData) {
                        goingToData = true;

                        window.location.href = api.theory.replace("{id}", this.dataset.id);
                    }
                });

                loadAssignments();
            }
        },

        error: function() {
            window.setInternalErrorMessage($("#ct-c"));
        }
    });
});
