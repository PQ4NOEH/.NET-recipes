window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        container: document.getElementById("dn-desksbooks"),
        spinner: $(document.getElementById("dn-desksbooks-spn")),
        content: document.getElementById("dsk-bks-c"),

        assigned: document.getElementById("dsk-asd-a"),
        finished: document.getElementById("dsk-asd-f"),
        certified: document.getElementById("dsk-asd-c")
    });

    var api = Object.create({
        data: "/Dean/GetDesksBooks",

        userAssign: "/Dean/DesksExams/Assign",
        userUnblock: "/Dean/DesksExams/Unblock",
        groupAssign: "/Dean/DesksExams/GroupAssign",
        groupUsersAssign: "/Dean/DesksExams/GroupUsersAssign",

        userAssignTest: "/Dean/DesksExams/AssignTest",
        userUnblockTest: "/Dean/DesksExams/UnblockTest",
        groupAssignTest: "/Dean/DesksExams/GroupAssignTest",
        groupUsersAssignTest: "/Dean/DesksExams/GroupUsersAssignTest"
    });

    var MEMBERS_ENUM = {
        0: "Student",
        1: "Teacher"
    };

    var QUESTIONTYPES_ENUM = {
        User: 1,
        Teacher: 2
    };

    var data = [],
        dataLoaded = [],
        loadingData = [],
        errorLoading = false;

    var memberLoaded = -1;

    var books = {};
    var book = null;

    var assignments;

    function manageAssignments(btns) {

    }

    function drawDesksBooksData(result) {
        if (result === undefined) {
            book = books[window.dean.member];
        } else {
            books[window.dean.member] = new window.desks.books();
            book = books[window.dean.member];
            var domElements = book.draw(
                window.model["books" + MEMBERS_ENUM[window.dean.member] + "Columns"],
                result,
                dom.content,
                book.visuals.Dean);

            var btns = [];

            manageAssignments(btns);

            memberLoaded = window.dean.member;
        }
    }

    function loadDesksBooksData() {
        var member = window.dean.member;

        loadingData[member] = true;

        $.ajax({
            async: true,
            type: "POST",
            url: api.data,
            data: {
                type: QUESTIONTYPES_ENUM[MEMBERS_ENUM[member]]
            },

            success: function(result) {
                if (window.dean.type === 0 && window.dean.member === member) {
                    drawDesksBooksData(result);
                }

                dom.spinner.removeClass("app-spinner-active");

                data[member] = result;
                dataLoaded[member] = true;
                loadingData[member] = false;
            },

            error: function() {
                errorLoading = true;

                dom.container.style.display = "table";
                window.setInternalErrorMessage($(dom.container));

                dom.spinner.removeClass("app-spinner-active");
            }
        });
    }

    window.desks.books.levels = window.model.levels;

    window.dean.loadDesksBooksData = function() {
        if (errorLoading) {
            return;
        }

        if (window.dean.member !== memberLoaded) {
            if (!dataLoaded[window.dean.member]) {
                dom.spinner.addClass("app-spinner-active");
                loadDesksBooksData();
            } else if (dataLoaded[window.dean.member]) {
                drawDesksBooksData();
            }
        }
    }

    window.dean.loadMemberDesksBooks = function(result) {
        if (window.dean.member !== memberLoaded) {
            if (book !== null) {
                book.clear();
                book = null;
            }
        }

        assignments = result;

        (function x() {
            if (errorLoading) {
                return;
            }

            if (book === null) {
                setTimeout(x, 100);
            } else {
                book.assignments(result);
            }
        })();
    }

    window.dean.showDesksBooksAssignments = function(assigned, finished, certified) {

    }
});
