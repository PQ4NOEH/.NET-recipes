window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        container: document.getElementById("dn-desksindex"),
        spinner: $(document.getElementById("dn-desksindex-spn")),
        content: document.getElementById("dsk-idx-c"),

        assigned: document.getElementById("dsk-asd-a"),
        finished: document.getElementById("dsk-asd-f"),
        certified: document.getElementById("dsk-asd-c")
    });

    var api = Object.create({
        data: "/Dean/GetDesksIndex",

        userAssign: "/Dean/DesksIndex/Assign",
        userUnblock: "/Dean/DesksIndex/Unblock",
        groupAssign: "/Dean/DesksIndex/GroupAssign",
        groupUsersAssign: "/Dean/DesksIndex/GroupUsersAssign"
    });

    var MEMBERS_ENUM = {
        0: "User",
        1: "Teacher"
    };

    var QUESTIONTYPES_ENUM = {
        User: 1,
        Teacher: 2
    };

    var TYPES_ENUM = Object.create({
        1: "ChooseFromTheBox",
        3: "MultipleChoice",
        5: "GapFilling",
        6: "WordFormation",
        7: "Rephrasing"
    });

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

    var indexes = {};
    var index = null;

    var assignments;

    function checkAssignmentStatus(button) {
        var area = parseInt(button.ancestor(".dsk-row", true).dataset.id);
        var subject = parseInt(button.ancestor(".dsk-sbj", true).dataset.id);
        var type = parseInt(button.dataset.id);

        var assigned = 0, blocked = 0, finished = 0, certified = 0, total;

        var assignmentI = -1;
        for (var i = 0, l = assignments.length; i < l; i++) {
            if (assignments[i].area === area && assignments[i].subject === subject && assignments[i].type === type) {
                assigned = assignments[i].assigned;
                blocked = assignments[i].blocked;
                finished = assignments[i].finished;
                certified = assignments[i].certified;
                assignmentI = i;
                break;
            }
        }

        var subjectI = -1, areaI = -1;
        for (var i = 0, l = data[window.dean.member][window.dean.level].subjects.length; i < l; i++) {
            if (data[window.dean.member][window.dean.level].subjects[i].id === subject) {
                var areas = data[window.dean.member][window.dean.level].subjects[i].areas;
                for (var j = 0, k = areas.length; j < k; j++) {
                    if (areas[j].area === area) {
                        total = areas[j].boards[TYPES_ENUM[type]];
                        areaI = j;
                        break;
                    }
                }
                subjectI = i;
                break;
            }
        }

        if (total === undefined) {
            total = -1;
        } else if (assignmentI === -1) {
            assignmentI = assignments.length;
            assignments.push({
                area: area,
                subject: subject,
                type: type,
                assigned: 0,
                remoteAssignment: 0,
                blocked: 0,
                finished: 0,
                certified: 0
            });
        }

        return {
            area: area,
            subject: subject,
            type: type,
            assignmentI: assignmentI,
            subjectI: subjectI,
            areaI: areaI,
            assigned: assigned,
            blocked: blocked,
            finished: finished,
            certified: certified,
            total: total
        }
    }

    var assignmentFunctions = {
        assign: function(button, status, num) {
            button.active = true;

            var as = false, bl = false, fn = false, cr = false;
            if (button.classList.contains("dsk-btn-as")) {
                as = true;
            } else {
                button.classList.add("dsk-btn-as");
            }
            if (button.classList.contains("dsk-btn-bl")) {
                button.classList.remove("dsk-btn-bl");
                bl = true;
            }
            if (button.classList.contains("dsk-btn-fn")) {
                button.classList.remove("dsk-btn-fn");
                fn = true;
            }
            if (button.classList.contains("dsk-btn-cr")) {
                button.classList.remove("dsk-btn-cr");
                cr = true;
            }

            var sum = assignments[status.assignmentI].assigned + num + status.finished + status.certified;
            if (sum >= status.total) {
                num = status.total - status.finished - status.certified - status.assigned;
            }

            assignments[status.assignmentI].assigned += num;

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
                        area: s.area,
                        subject: s.subject,
                        type: s.type,
                        num: num,
                        remote: window.dean.remoteMode,
                        status: true,
                        offsetDate: window.user.offsetDate
                    },

                    success: function() {
                        b.active = false;
                    },

                    error: function() {
                        assignments[status.assignmentI] -= num;

                        if (!as) b.classList.remove("dsk-btn-as");
                        if (bl) b.classList.add("dsk-btn-bl");
                        if (fn) b.classList.add("dsk-btn-fn");
                        if (cr) b.classList.add("dsk-btn-cr");

                        shake(b.ancestor(".dsk-btn-w", true), undefined, 2, 250);
                        window.dean.showDesksIndexAssignments();
                        b.active = false;
                    }
                });
            })(status, button);
        },

        unassign: function(button, status, num) {
            button.active = true;

            if (num === -1) {
                num = assignments[status.assignmentI].assigned;
            }

            var as = false, bl = 0;
            if (button.classList.contains("dsk-btn-as")) {
                if (assignments[status.assignmentI].assigned === num) {
                    button.classList.remove("dsk-btn-as");
                    as = true;
                }
            }
            if (button.classList.contains("dsk-btn-bl")) {
                bl = status.blocked;
                if (bl <= num) {
                    button.classList.remove("dsk-btn-bl");
                }
            }

            assignments[status.assignmentI].assigned -= num;
            if (bl <= num) {
                assignments[status.assignmentI].blocked -= bl;
            } else {
                assignments[status.assignmentI].blocked -= num;
            }

            if (assignments[status.assignmentI].assigned === 0) {
                if (assignments[status.assignmentI].finished > 0) {
                    button.classList.add("dsk-btn-fn");
                } else if (assignments[status.assignmentI].certified > 0) {
                    button.classList.add("dsk-btn-cr");
                }
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
                        area: s.area,
                        subject: s.subject,
                        type: s.type,
                        num: num,
                        remote: window.dean.remoteMode,
                        status: false,
                        offsetDate: window.user.offsetDate
                    },

                    success: function() {
                        b.active = false;
                    },

                    error: function() {
                        assignments[status.assignmentI].assigned += num;
                        assignments[status.assignmentI].blocked = bl;

                        if (as) {
                            b.classList.add("dsk-btn-as");
                        }
                        if (bl > 0) {
                            b.classList.add("dsk-btn-bl");
                        }

                        b.classList.remove("dsk-btn-fn", "dsk-btn-cr");

                        shake(b.ancestor(".dsk-btn-w", true), undefined, 2, 250);
                        window.dean.showDesksIndexAssignments();
                        b.active = false;
                    }
                });
            })(status, button);
        },

        unblock: function(button, status) {
            if (window.dean.member !== 0) {
                return;
            }

            button.active = true;

            var bl = status.blocked;
            button.classList.remove("dsk-btn-bl");
            button.classList.add("dsk-btn-as");

            assignments[status.assignmentI].blocked = 0;

            (function(s, b) {
                $.ajax({
                    async: true,
                    type: "POST",
                    url: window.dean.userUnblock,
                    data: {
                        user: window.dean.member === 0 ? window.dean.uid : undefined,
                        group: window.dean.member === 1 ? window.dean.uid : undefined,
                        level: window.dean.level,
                        area: s.area,
                        subject: s.subject,
                        type: s.type,
                        offsetDate: window.user.offsetDate
                    },

                    success: function() {
                        b.active = false;
                    },

                    error: function() {
                        assignments[status.assignmentI].blocked = bl;

                        if (bl > 0) {
                            b.classList.add("dsk-btn-bl");
                            b.classList.remove("dsk-btn-as");
                        }

                        shake(b.ancestor(".dsk-btn-w", true), undefined, 2, 250);
                        window.dean.showDesksIndexAssignments();
                        b.active = false;
                    }
                });
            })(status, button);
        }
    };

    function manageAssignments(btns) {
        $(btns).on("click", function() {
            if (this.active === true) {
                return;
            }

            var status = checkAssignmentStatus(this);

            if (status.total === -1) {
                return;
            }

            if (status.blocked > 0) {
                assignmentFunctions.unblock(this, status);
                window.dean.showDesksIndexAssignments();
            } else if (status.assigned === 0) {
                assignmentFunctions.assign(this, status, 1);
                window.dean.showDesksIndexAssignments();
            } else {
                assignmentFunctions.unassign(this, status, -1);
                window.dean.showDesksIndexAssignments();
            }
        });
    }

    function drawDesksIndexData(result) {
        if (result === undefined) {
            index = indexes[window.dean.member][window.dean.level];
            index.fill(dom.content);
        } else {
            if (indexes[window.dean.member] === undefined) {
                indexes[window.dean.member] = {};
            }

            indexes[window.dean.member][window.dean.level] = new window.desks.index();
            index = indexes[window.dean.member][window.dean.level];

            var domElements = index.draw(
                window.model.indexAreas,
                result,
                dom.content);

            var btns = [];
            for (var i = 0, l = domElements.length; i < l; i++) {
                btns.pushRange(domElements[i].getElementsByClassName("dsk-btn-c"));
            }

            manageAssignments(btns);

            memberLoaded = window.dean.member;
            levelLoaded = window.dean.level;
        }
    }

    function loadDesksIndexData() {
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
                if (window.dean.type === 0
                    && (window.dean.member === member && window.dean.level === level)) {
                    drawDesksIndexData(result);
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

    window.dean.loadDesksIndexData = function() {
        if (errorLoading) {
            return;
        }

        if (window.dean.member !== memberLoaded || window.dean.level !== levelLoaded) {
            if (!dataLoaded[window.dean.member][window.dean.level]
                && !loadingData[window.dean.member][window.dean.level]) {
                dom.spinner.addClass("app-spinner-active");
                loadDesksIndexData();
            } else if (dataLoaded[window.dean.member][window.dean.level]) {
                drawDesksIndexData();
            }
        }
    }

    window.dean.loadMemberDesksIndex = function(result) {
        if (window.dean.member !== memberLoaded || window.dean.level !== levelLoaded) {
            if (index !== null) {
                index.clear();
                index = null;
            }
        }

        assignments = result;

        (function x() {
            if (errorLoading) {
                return;
            }

            if (index === null) {
                setTimeout(x, 100);
            } else {
                index.assignments(result, false);
            }
        })();
    }

    window.dean.showDesksIndexAssignments = function() {
        var a = 0, f = 0, c = 0;

        for (var i = 0, l = assignments.length; i < l; i++) {
            a += assignments[i].assigned;
            f += assignments[i].finished;
            c += assignments[i].certified;
        }

        dom.assigned.innerText = a;
        dom.finished.innerText = f;
        dom.certified.innerText = c;
    }
});