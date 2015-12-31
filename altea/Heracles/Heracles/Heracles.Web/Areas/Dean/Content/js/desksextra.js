window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        container: document.getElementById("dn-desksextra"),
        spinner: $(document.getElementById("dn-desksextra-spn")),
        content: document.getElementById("dsk-ext-c"),

        assigned: document.getElementById("dsk-asd-a"),
        finished: document.getElementById("dsk-asd-f"),
        certified: document.getElementById("dsk-asd-c")
    });

    var api = Object.create({
        data: "/Dean/GetDesksExtra",

        userAssign: "/Dean/DesksExtra/Assign",
        userUnblock: "/Dean/DesksExtra/Unblock",
        groupAssign: "/Dean/DesksExtra/GroupAssign",
        groupUsersAssign: "/Dean/DesksExtra/GroupUsersAssign"
    });

    var MEMBERS_ENUM = {
        0: "User",
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

    var memberLoaded = -1,
        levelLoaded = -1;

    data[0] = []; // Users
    dataLoaded[0] = [];
    loadingData[0] = [];

    data[1] = []; // Groups
    dataLoaded[1] = [];
    loadingData[1] = [];

    var extras = {};
    var extra = null;

    var assignments;

    function checkAssignmentStatus(button) {
        var part = parseInt(button.dataset.id);
        var type = button.dataset.type;

        if (type === undefined) {
            type = 0;
        } else {
            type = parseInt(type);
        }

        for (var i = 0, l = assignments.length; i < l; i++) {
            if (assignments[i].id === part && assignments[i].type === type) {
                return {
                    part: part,
                    type: type,
                    assignmentI: i,
                    assigned: assignments[i].assigned,
                    blocked: assignments[i].blocked,
                    finished: assignments[i].finished,
                    certified: assignments[i].certified
                }
            }
        }

        var assignmentI = assignments.length;
        assignments.push({
            id: part,
            type: type,
            assigned: false,
            remoteAssignment: false,
            blocked: false,
            finished: false,
            certified: false
        });

        return {
            part: part,
            type: type,
            assignmentI: assignmentI,
            assigned: false,
            blocked: false,
            finished: false,
            certified: false
        }
    }

    var assignmentFunctions = {
        assign: function(button, status, assign) {

            button.active = true;

            if (assign) {
                button.classList.add("dsk-btn-as");
                assignments[status.assignmentI].assigned = true;
            } else {
                button.classList.remove("dsk-btn-as");
                assignments[status.assignmentI].assigned = false;
            }

            (function(s, b) {
                $.ajax({
                    async: true,
                    type: "POST",
                    url: window.dean.member === 0
                        ? api.userAssign
                        : window.dean.groupUsersMode
                        ? api.groupUsersAssign
                        : api.groupAssign,
                    data: {
                        user: window.dean.member === 0 ? window.dean.uid : undefined,
                        group: window.dean.member === 1 ? window.dean.uid : undefined,
                        level: window.dean.level,
                        part: s.part,
                        type: s.type,
                        remote: window.dean.remoteMode,
                        status: assign,
                        offsetDate: window.user.offsetDate
                    },

                    success: function() {
                        b.active = false;
                    },

                    error: function() {
                        if (assign) {
                            b.classList.remove("dsk-btn-as");
                            assignments[status.assignmentI].assigned = false;
                        } else {
                            b.classList.add("dsk-btn-as");
                            assignments[status.assignmentI].assigned = true;
                        }

                        shake(b.ancestor(".dsk-btn-w", true), undefined, 2, 250);
                        window.dean.showDesksExtraAssignments();
                        b.active = false;
                    }
                });
            })(status, button);
        },

        unblock: function(button, status) {
        }
    };

    function manageAssignments(btns) {
        $(btns).on("click", function() {
            if (this.active === true) {
                return;
            }

            if (this.classList.contains("dsk-btn-in")) {
                return;
            }

            var status = checkAssignmentStatus(this);
            var typeFound = false;
            for (var i = 0, l = data[window.dean.member][window.dean.level].parts.length; i < l; i++) {
                if (data[window.dean.member][window.dean.level].parts[i].id === status.part) {
                    if (status.type === 0 && data[window.dean.member][window.dean.level].parts[i].types === null) {
                        typeFound = true;
                    } else {
                        for (var j = 0, k = data[window.dean.member][window.dean.level].parts[i].types.length; j < k; j++) {
                            if (status.type === data[window.dean.member][window.dean.level].parts[i].types[j].type) {
                                typeFound = true;
                                break;
                            }
                        }
                    }
                    break;
                }
            }

            if (!typeFound) {
                return;
            }

            if (status.blocked) {
                assignmentFunctions.unblock(this, status);
                window.dean.showDesksExtraAssignments();
            } else if (!status.assigned) {
                assignmentFunctions.assign(this, status, true);
                window.dean.showDesksExtraAssignments();
            } else {
                assignmentFunctions.assign(this, status, false);
                window.dean.showDesksExtraAssignments();
            }
        });
    }

    function drawDesksExtraData(result) {
        if (result === undefined) {
            extra = extras[window.dean.member][window.dean.level];
            extra.fill(dom.content);
        } else {
            if (extras[window.dean.member] === undefined) {
                extras[window.dean.member] = {};
            }

            extras[window.dean.member][window.dean.level] = new window.desks.extra();
            extra = extras[window.dean.member][window.dean.level];

            var domElements =
                extra.draw(
                    window.model["extra" + MEMBERS_ENUM[window.dean.member] + "Columns"],
                    result,
                    dom.content,
                    extra.visuals.Dean);

            var btns = [];
            for (var i = 0, l = domElements.length; i < l; i++) {
                btns.pushRange(domElements[i].getElementsByClassName("dsk-btn-c"));
            }

            manageAssignments(btns);

            memberLoaded = window.dean.member;
            levelLoaded = window.dean.level;
        }
    }

    function loadDesksExtraData() {
        var member = window.dean.member;
        var level = window.dean.level;

        loadingData[member][level] = true;

        $.ajax({
            async: true,
            type: "POST",
            url: api.data,
            data: {
                level: level,
                type: QUESTIONTYPES_ENUM[MEMBERS_ENUM[member]]
            },

            success: function(result) {
                if (window.dean.type === 0 && (window.dean.member === member && window.dean.level === level)) {
                    drawDesksExtraData(result);
                }

                dom.spinner.removeClass("app-spinner-active");

                data[member][level] = result;
                dataLoaded[member][level] = true;
                loadingData[member][level] = false;
            },

            error: function() {
                errorLoading = true;

                dom.container.style.display = "table";
                window.setInternalErrorMessage($(dom.container));

                dom.spinner.removeClass("app-spinner-active");
            }
        });
    }

    window.dean.loadDesksExtraData = function() {
        if (errorLoading) {
            return;
        }

        if (window.dean.member !== memberLoaded || window.dean.level !== levelLoaded) {
            if (!dataLoaded[window.dean.member][window.dean.level]
                && !loadingData[window.dean.member][window.dean.level]) {
                dom.spinner.addClass("app-spinner-active");
                loadDesksExtraData();
            } else if (dataLoaded[window.dean.member][window.dean.level]) {
                drawDesksExtraData();
            }
        }
    }

    window.dean.loadMemberDesksExtra = function(result) {
        if (window.dean.member !== memberLoaded || window.dean.level !== levelLoaded) {
            if (extra !== null) {
                extra.clear();
                extra = null;
            }
        }

        assignments = result;

        (function x() {
            if (errorLoading) {
                return;
            }

            if (extra === null) {
                setTimeout(x, 100);
            } else {
                extra.assignments(result);
            }
        })();
    }

    window.dean.showDesksExtraAssignments = function() {
        var a = 0, f = 0, c = 0;

        for (var i = 0, l = assignments.length; i < l; i++) {
            if (assignments[i].assigned) a++;
            if (assignments[i].finished) f++;
            if (assignments[i].certified) c++;
        }

        dom.assigned.innerText = a;
        dom.finished.innerText = f;
        dom.certified.innerText = c;
    }
});
