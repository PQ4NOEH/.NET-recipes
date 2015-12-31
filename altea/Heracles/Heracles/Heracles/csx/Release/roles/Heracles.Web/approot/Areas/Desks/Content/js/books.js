window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        spinner: $(document.getElementById("dsk-spn")),
        content: document.getElementById("dsk-bks-c")
    });

    var api = Object.create({
        list: "/Books/List",
        assignments: "/Books/Assignments",
        exercise: "/Books/{type}/{subtype}/{publication}"
    });

    var books = new window.desks.books();

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
                exams.assignments(result);

                if (firstAssignmentLoad) {
                    firstAssignmentLoad = false;
                    dom.spinner.removeClass("app-spinner-active");
                }

                setTimeout(loadAssignments, 6000);
                loadingAssignments = false;
            },

            error: function() {
                if (firstAssignmentLoad) {
                    dom.spinner.removeClass("app-spinner-active");
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

        success: function(result) {
            if (result === null) {
                window.setInternalErrorMessage($("#ct-c"));
            } else {
                books.draw(result, dom.content);
                $(dom.content.getElementsByClassName("dsk-btn-c")).on("click", function() {
                    if (this.classList.contains("dsk-btn-as") && !goingToData) {
                        goingToData = true;

                        var ex = api.exercise;
                        ex = ex.replace("{type}", this.dataset.type);
                        ex = ex.replace("{subtype}", this.dataset.subtype);
                        ex = ex.replace("{publication}", this.dataset.id);
                        window.location.href = ex;
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
