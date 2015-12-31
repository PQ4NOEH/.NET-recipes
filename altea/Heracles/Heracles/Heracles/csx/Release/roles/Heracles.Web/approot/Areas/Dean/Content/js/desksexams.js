window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        container: document.getElementById("dn-desksexams"),
        spinner: $(document.getElementById("dn-desksexams-spn")),
        content: document.getElementById("dsk-exm-cc"),

        assigned: document.getElementById("dsk-asd-a"),
        finished: document.getElementById("dsk-asd-f"),
        certified: document.getElementById("dsk-asd-c")
    });

    var api = Object.create({
        data: "/Dean/GetDesksExams",

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

    var exams = {};
    var exam = null;

    var assignments;

    function checkAssignmentStatus(button, vocabulary) {
        var group = parseInt(button.ancestor(".dsk-exm-grp", true).dataset.id);
        var paper = parseInt(button.ancestor(".dsk-exm-ppr", true).dataset.id);
        var part = parseInt(button.dataset.id);

        for (var i = 0, l = assignments.length; i < l; i++) {
            if (assignments[i].part === part && assignments[i].vocabulary === vocabulary) {
                return {
                    group: group,
                    paper: paper,
                    part: part,
                    vocabulary: vocabulary,
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
            group: group,
            paper: paper,
            part: part,
            vocabulary: vocabulary,
            assigned: false,
            remoteAssignment: false,
            blocked: false,
            finished: false,
            certified: false
        });

        return {
            group: group,
            paper: paper,
            part: part,
            vocabulary: vocabulary,
            assignmentI: assignmentI,
            assigned: false,
            blocked: false,
            finished: false,
            certified: false
        }
    }

    function checkTestAssignmentStatus(button) {
        var group = parseInt(button.ancestor(".dsk-exm-grp", true).dataset.id);
        var paper = parseInt(button.ancestor(".dsk-exm-ppr", true).dataset.id);
        var test = parseInt(button.dataset.id);
        var round = parseInt(button.dataset.round);

        for (var i = 0, l = assignments.length; i < l; i++) {
            if (assignments[i].group === group && assignments[i].paper === paper && assignments[i].test === test) {
                return {
                    group: group,
                    paper: paper,
                    test: test,
                    round: round,
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
            group: group,
            paper: paper,
            test: test,
            round: round,
            assigned: false,
            remoteAssignment: false,
            blocked: false,
            finished: false,
            certified: false
        });

        return {
            group: group,
            paper: paper,
            test: test,
            round: round,
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
                        vocabulary: s.vocabulary,
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
                        window.dean.showDesksExamsAssignments();
                        b.active = false;
                    }
                });
            })(status, button);
        },

        unblock: function(button, status) {
        },

        assignTest: function(button, status, assign) {
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
                        ? api.userAssignTest
                        : window.dean.groupUsersMode
                        ? api.groupUsersAssignTest
                        : api.groupAssignTest,
                    data: {
                        user: window.dean.uid,
                        level: window.dean.level,
                        group: s.group,
                        paper: s.paper,
                        test: s.test,
                        round: s.round,
                        remote: window.dean.remoteMode,
                        status: true,
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
                        window.dean.showDesksExamsAssignments();
                        b.active = false;
                    }
                });
            })(status, button);
        },

        unblockTest: function(button, status) {
        }
    };

    function manageAssignments(btns) {
        $(btns).on("click", function(e) {
            if (this.active === true) {
                return;
            }

            if (this.classList.contains("dsk-btn-in")) {
                return;
            }

            if (this.classList.contains("dsk-exm-prt-btn")) {
                var button, vocabulary;
                if (e.target.classList.contains("dsk-btn-l")) {
                    button = e.target;
                    vocabulary = true;
                } else {
                    var ancestor = e.target.ancestor(".dsk-btn-l", true);
                    if (ancestor !== undefined) {
                        button = ancestor;
                        vocabulary = true;
                    } else {
                        button = this;
                        vocabulary = false;
                    }
                }

                var status = checkAssignmentStatus(this, vocabulary);
                var onlyExam;
                for (var i = 0, l = data[window.dean.member][window.dean.level].groups.length; i < l; i++) {
                    if (data[window.dean.member][window.dean.level].groups[i].id === status.group) {
                        for (var j = 0, k = data[window.dean.member][window.dean.level].groups[i].papers.length; j < k; j++) {
                            if (data[window.dean.member][window.dean.level].groups[i].papers[j].id === status.paper) {
                                onlyExam = data[window.dean.member][window.dean.level].groups[i].papers[j].onlyExamMode;
                                break;
                            }
                        }
                        break;
                    }
                }

                if (window.model.dean === 0 && onlyExam) {
                    return;
                }

                if (status.blocked) {
                    assignmentFunctions.unblock(button, status);
                    window.dean.showDesksExamsAssignments();
                } else if (!status.assigned) {
                    assignmentFunctions.assign(button, status, true);
                    window.dean.showDesksExamsAssignments();
                } else {
                    assignmentFunctions.assign(button, status, false);
                    window.dean.showDesksExamsAssignments();
                }
            } else if (this.classList.contains("dsk-exm-tst-btn")) {
                // TEST
                var status = checkTestAssignmentStatus(this);
                var hasExam = true;
                for (var i = 0, l = data[window.dean.member][window.dean.level].groups.length; i < l; i++) {
                    if (data[window.dean.member][window.dean.level].groups[i].id === status.group) {
                        for (var j = 0, k = data[window.dean.member][window.dean.level].groups[i].papers.length; j < k; j++) {
                            if (data[window.dean.member][window.dean.level].groups[i].papers[j].id === status.paper) {
                                hasExam = data[window.dean.member][window.dean.level].groups[i].papers[j].examMode;
                                break;
                            }
                        }
                        break;
                    }
                }

                if (window.model.dean === 1 || !hasExam) {
                    return;
                }

                if (status.blocked) {
                    assignmentFunctions.unblockTest(this, status);
                    window.dean.showDesksExamsAssignments();
                } else if (!status.assigned) {
                    assignmentFunctions.assignTest(this, status, true);
                    window.dean.showDesksExamsAssignments();
                } else {
                    assignmentFunctions.assignTest(this, status, false);
                    window.dean.showDesksExamsAssignments();
                }
            } else if (this.classList.contains("dsk-exm-mts-btn")) {
                // MIXED
                // TODO
            }
        });
    }

    function drawDesksExamsData(result) {
        if (result === undefined) {
            exam = exams[window.dean.member][window.dean.level];
            exam.fill(dom.content);
        } else {
            if (exams[window.dean.member] === undefined) {
                exams[window.dean.member] = {};
            }

            exams[window.dean.member][window.dean.level] = new window.desks.exams();
            exam = exams[window.dean.member][window.dean.level];

            var domElements = exam.draw(
                result,
                dom.content,
                window.dean.member === 1);

            var btns = [];
            for (var i = 0, l = domElements.length; i < l; i++) {
                btns.pushRange(domElements[i].getElementsByClassName("dsk-btn-c"));
            }

            manageAssignments(btns);

            memberLoaded = window.dean.member;
            levelLoaded = window.dean.level;
        }
    }

    function loadDesksExamsData() {
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
                    drawDesksExamsData(result);
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

    window.dean.loadDesksExamsData = function() {
        if (errorLoading) {
            return;
        }

        if (window.dean.member !== memberLoaded || window.dean.level !== levelLoaded) {
            if (!dataLoaded[window.dean.member][window.dean.level]
                && !loadingData[window.dean.member][window.dean.level]) {
                dom.spinner.addClass("app-spinner-active");
                loadDesksExamsData();
            } else if (dataLoaded[window.dean.member][window.dean.level]) {
                drawDesksExamsData();
            }
        }
    }

    window.dean.loadMemberDesksExams = function(result) {
        if (window.dean.member !== memberLoaded || window.dean.level !== levelLoaded) {
            if (exam !== null) {
                exam.clear();
                exam = null;
            }
        }

        assignments = result;

        (function x() {
            if (errorLoading) {
                return;
            }

            if (exam === null) {
                setTimeout(x, 100);
            } else {
                exam.assignments(result);
            }
        })();
    }

    window.dean.showDesksExamsAssignments = function() {
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
