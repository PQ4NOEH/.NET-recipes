window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    if (window.desks === undefined) {
        window.desks = Object.create(null);
    }

    var exams = function() {
        var dom = Object.create({
            subjects: null,
            assignCount: document.getElementsByClassName("dsk-mct-asd-a"),
            finishCount: document.getElementsByClassName("dsk-mct-asd-f"),
            certifyCount: document.getElementsByClassName("dsk-mct-asd-c")
        });

        var domElements = null;

        var groups = {},
            buttons = {},
            testButtons = [];

        //#region Draw
        function drawPaper(group, paper, container) {
            var p = document.createElement("div");
            p.className = "dsk-exm-ppr";
            p.dataset.id = paper.id;
            container.appendChild(p);

            var hdr = document.createElement("div");
            hdr.className = "dsk-exm-ppr-h dsk-subhdr";
            hdr.innerText = paper.title;
            p.appendChild(hdr);

            if (paper.info !== null) {
                var info = document.createElement("div");
                info.className = "dsk-exm-ppr-hi";
                info.datset.id = paper.info;
                hdr.appendChild(info);
            }

            var tests = document.createElement("div");
            tests.className = "dsk-exm-ppr-tsts";
            p.appendChild(tests);

            groups[group][2][paper.id] = tests;
        }

        function drawPaperGroup(group, papers, container, hasHeader) {
            var pp = document.createElement("div");
            pp.className = "dsk-exm-pprg";
            container.appendChild(pp);

            if (papers[0].headerId !== null) {
                var hdr = document.createElement("div");
                hdr.className = "dsk-exm-pprs-h dsk-hdr";
                pp.appendChild(hdr);

                if (papers[0].headerTitle !== null) {
                    var title = document.createElement("span");
                    title.className = "dsk-exm-pprs-ht";
                    title.innerText = papers[0].headerTitle;
                    hdr.appendChild(title);
                }

                if (papers[0].headerSubtitle !== null) {
                    var subtitle = document.createElement("span");
                    subtitle.className = "dsk-exm-pprs-hst";
                    subtitle.innerText = papers[0].headerSubtitle;
                    hdr.appendChild(subtitle);
                }
            } else if (hasHeader) {
                var hdr = document.createElement("div");
                hdr.className = "dsk-exm-pprs-h dsk-exm-pprs-nh dsk-hdr";
                pp.appendChild(hdr);

                var title = document.createElement("span");
                title.className = "dsk-exm-pprs-ht";
                title.innerText = "&nbsp;";
                hdr.appendChild(title);
            }

            var content = document.createElement("div");
            content.className = "dsk-exm-pprc";
            pp.appendChild(content);

            for (var i = 0, l = papers.length; i < l; i++) {
                drawPaper(group, papers[i], content);
            }
        }

        function drawPapers(group, papers, container) {
            var p = document.createElement("div");
            p.className = "dsk-exm-pprs";
            container.appendChild(p);

            var hasHeader = false;
            var i = 0, l = papers.length;
            while (i < l) {
                if (papers[i].headerId !== null) {
                    hasHeader = true;
                    break;
                }

                i++;
            }

            var groupedPapers = undefined;
            var id = -1;
            i = 0;
            while (i < l) {
                if (papers[i].headerId !== id) {
                    if (groupedPapers !== undefined) {
                        drawPaperGroup(group, groupedPapers, p, hasHeader);
                    }

                    id = papers[i].headerId;
                    groupedPapers = [];
                }

                // ReSharper disable once QualifiedExpressionMaybeNull
                groupedPapers.push(papers[i]);
                i++;
            }

            drawPaperGroup(group, groupedPapers, p, hasHeader);
        }

        function drawGroup(data, position, container) {
            var group = document.createElement("div");
            group.className = "dsk-exm-grp";
            group.dataset.id = data.id;
            container.appendChild(group);

            var posWrapper = document.createElement("div");
            posWrapper.className = "dsk-exm-posw";
            group.appendChild(posWrapper);

            var pos = document.createElement("div");
            pos.className = "dsk-exm-pos";
            posWrapper.appendChild(pos);

            var posNumber = document.createElement("span");
            posNumber.className = "dsk-exm-posc";
            posNumber.innerText = position;
            pos.appendChild(posNumber);

            var tests = document.createElement("div");
            tests.className = "dsk-exm-tst-c";
            group.appendChild(tests);

            var testsContainer = document.createElement("div");
            testsContainer.className = "dsk-exm-tst-cc";
            tests.appendChild(testsContainer);

            var papers = {};

            for (var i = 0, l = data.papers.length; i < l; i++) {
                papers[data.papers[i].id] = data.papers[i];
            }

            groups[data.id] = [group, testsContainer, {}, pos, papers];
            drawPapers(data.id, data.papers, group);

            return group;
        }

        function drawTestCount(container, num) {
            for (var i = 0; i < num; i++) {
                var n = document.createElement("div");
                n.className = "dsk-exm-tstn";
                n.innerText = i + 1;
                container.appendChild(n);
            }
        }

        function drawExamPart(id, paper, container) {
            var examContainer = document.createElement("div");
            examContainer.className = "dsk-btn-ww";
            container.appendChild(examContainer);

            /* TODO: DYNAMIC ROUNDS */
            for (var i = 0, l = 3; i < l; i++) {
                var buttonWrapper = document.createElement("span");
                buttonWrapper.className = "dsk-btn-w";
                examContainer.appendChild(buttonWrapper);

                var buttonContent = document.createElement("span");
                buttonContent.className = "dsk-btn-cw";
                buttonWrapper.appendChild(buttonContent);

                var button = document.createElement("span");
                button.className = "dsk-btn-c dsk-exm-tst-btn";
                button.dataset.id = id;
                button.dataset.round = i + 1;
                buttonContent.appendChild(button);

                var buttonData = document.createElement("span");
                buttonData.className = "dsk-btn";
                buttonData.innerText = i18n.t("desks.exams.r") + (i + 1);
                buttonData.title = i18n.t("desks.exams.round") + " " + (i + 1);
                button.appendChild(buttonData);

                testButtons.push([id, paper, i + 1, button]);
            }
        }

        function drawEmptyTestParts(container, numParts) {
            var position = 0;

            for (var i = 0, l = numParts; i < l; i++) {
                var buttonWrapper = document.createElement("span");
                buttonWrapper.className = "dsk-btn-w";
                container.appendChild(buttonWrapper);

                var buttonContent = document.createElement("span");
                buttonContent.className = "dsk-btn-cw";
                buttonWrapper.appendChild(buttonContent);

                var button = document.createElement("span");
                button.className = "dsk-btn-c dsk-btn-in";
                buttonContent.appendChild(button);

                var buttonData = document.createElement("span");
                buttonData.className = "dsk-btn";
                buttonData.innerText = ++position;
                button.appendChild(buttonData);
            }
        }

        function drawTestParts(groupId, paper, test, container, numParts, teacherMode) {
            var position = 0;

            for (var i = 0, l = test.parts.length; i < l; i++) {
                if (test.parts[i].paper === paper) {
                    var buttonWrapper = document.createElement("span");
                    buttonWrapper.className = "dsk-btn-w";
                    container.appendChild(buttonWrapper);

                    var buttonContent = document.createElement("span");
                    buttonContent.className = "dsk-btn-cw";
                    buttonWrapper.appendChild(buttonContent);

                    var button = document.createElement("span");
                    button.className = "dsk-btn-c dsk-exm-prt-btn";
                    button.dataset.id = test.parts[i].id;
                    buttonContent.appendChild(button);

                    if (!teacherMode && (test.parts[i].hasVocabulary === true || (test.parts[i].hasVocabulary === null && groups[groupId][4][paper].hasVocabulary === true))) {
                        var vocabularyButton = document.createElement("span");
                        vocabularyButton.className = "dsk-btn-c dsk-btn-l";
                        button.appendChild(vocabularyButton);


                        var vButtonData = document.createElement("span");
                        vButtonData.className = "dsk-btn";
                        vButtonData.innerText = i18n.t("desks.exams.v");
                        vButtonData.title = i18n.t("desks.exams.vocabulary");
                        vocabularyButton.appendChild(vButtonData);
                    }

                    var buttonData = document.createElement("span");
                    buttonData.className = "dsk-btn";
                    buttonData.innerText = ++position;
                    button.appendChild(buttonData);

                    if (buttons[test.parts[i].id] === undefined) {
                        buttons[test.parts[i].id] = [];
                    }

                    buttons[test.parts[i].id].push(button);
                }
            }

            if (position === 0 && numParts !== 0) {
                drawEmptyTestParts(container, numParts);
            }
        }

        function drawGroupTests(groupId, tests, groupData, teacherMode) {
            for (var i = 0, l = tests.length; i < l; i++) {
                for (var paper in groupData) {
                    if (Object.prototype.hasOwnProperty.call(groupData, paper)) {
                        var numParts = 0;

                        for (var j = 0, k = tests.length; j < k; j++) {
                            var testNumParts = 0;

                            for (var m = 0, n = tests[j].parts.length; m < n; m++) {
                                if (tests[j].parts[m].paper === parseInt(paper)) {
                                    testNumParts++;
                                }
                            }

                            numParts = Math.max(numParts, testNumParts);
                        }

                        var paperTest = document.createElement("div");
                        paperTest.className = "dsk-exm-ppr-tst";
                        paperTest.dataset.id = tests[i].id;
                        groupData[paper].appendChild(paperTest);

                        if (!teacherMode && groups[groupId][4][paper].examMode) {
                            drawExamPart(tests[i].id, parseInt(paper), paperTest);
                        }

                        drawTestParts(groupId, parseInt(paper), tests[i], paperTest, numParts, teacherMode);
                    }
                }
            }
        }

        function editPapers(group, numTests) {
            for (var i in group[2]) {
                if (Object.prototype.hasOwnProperty.call(group[2], i)) {
                    if (group[2][i].getElementsByClassName("dsk-btn-c").length === 0) {
                        var paper = group[2][i].ancestor(".dsk-exm-ppr", true);
                        paper.parentNode.removeChild(paper);
                    }
                }
            }
        }

        function drawTests(data, teacherMode) {
            var tests = {};

            for (var i = 0, l = data.length; i < l; i++) {
                if (tests[data[i].groupId] === undefined) {
                    tests[data[i].groupId] = [];
                }

                tests[data[i].groupId].push(data[i]);
            }

            for (var group in tests) {
                if (Object.prototype.hasOwnProperty.call(tests, group)) {
                    tests[group].sort(function(a, b) {
                        return a.position - b.position;
                    });
                }

                drawTestCount(groups[group][1], tests[group].length);
                drawGroupTests(group, tests[group], groups[group][2], teacherMode);
            }
        }

        this.draw = function(data, container, teacherMode) {
            if (typeof teacherMode !== "boolean") {
                teacherMode = false;
            }

            domElements = [];

            for (var i = 0, l = data.groups.length; i < l; i++) {
                domElements.push(drawGroup(data.groups[i], i + 1, container));
            }

            var numTests = data.tests;
            drawTests(data.tests, teacherMode);

            for (var group in groups) {
                if (Object.prototype.hasOwnProperty.call(groups, group)) {
                    editPapers(groups[group], numTests);
                    groups[group][3].style.height = groups[group][1].clientHeight + "px";
                }
            }

            $(container).mCustomScrollbar({
                axis: "y",
                scrollbarPosition: "inside",
                alwaysShowScrollbar: 0,
                scrollButtons: { enable: true },
                theme: "dark-3"
            });

            return domElements;
        }

        this.clear = function() {
            for (var i = 0, l = domElements.length; i < l; i++) {
                domElements[i].parentNode.removeChild(domElements[i]);
            }
        }

        this.fill = function(container) {
            for (var i = 0, l = domElements.length; i < l; i++) {
                container.appendChild(domElements[i]);
            }
        }
        // #endregion

        //#region Assignments
        this.assignments = function(assignments) {
            var assigned = 0, finished = 0, certified = 0;

            for (var i = 0, l = assignments.length; i < l; i++) {
                if (assignments[i].part !== undefined) {
                    for (var j = 0, k = buttons[assignments[i].part].length; j < k; j++) {
                        var button = buttons[assignments[i].part][j];

                        if (assignments[i].vocabulary) {
                            button = button.getElementsByClassName("dsk-btn-l")[0];
                        }

                        if (assignments[i].certified) {
                            button.classList.add("dsk-btn-cr");
                        } else if (assignments[i].finished) {
                            button.classList.add("dsk-btn-fn");
                        } else if (assignments[i].assigned) {
                            button.classList.add("dsk-btn-as");

                            if (assignments[i].blocked) {
                                button.classList.add("dsk-btn-bl");
                            }
                        }
                    }
                } else {
                    for (var j = 0, k = testButtons.length; j < k; j++) {
                        if (testButtons[j][0] === assignments[i].test && testButtons[j][1] === assignments[i].paper && testButtons[j][2] === assignments[i].round) {
                            if (assignments[i].certified) {
                                testButtons[j][3].classList.add("dsk-btn-cr");
                            } else if (assignments[i].finished) {
                                testButtons[j][3].classList.add("dsk-btn-fn");
                            } else if (assignments[i].assigned) {
                                testButtons[j][3].classList.add("dsk-btn-as");

                                if (assignments[i].blocked) {
                                    testButtons[j][3].classList.add("dsk-btn-bl");
                                }
                            }
                        }
                    }
                }

                if (assignments[i].certified) {
                    certified++;
                } else if (assignments[i].finished) {
                    finished++;
                } else if (assignments[i].assigned) {
                    assigned++;

                    if (assignments[i].blocked) {

                    }
                }
            }

            for (var i = 0, l = dom.assignCount.length; i < l; i++) {
                dom.assignCount[i].innerText = assigned;
            }

            for (var i = 0, l = dom.finishCount.length; i < l; i++) {
                dom.finishCount[i].innerText = finished;
            }

            for (var i = 0, l = dom.certifyCount.length; i < l; i++) {
                dom.certifyCount[i].innerText = certified;
            }
        }
        //#endregion
    }

    window.desks.exams = exams;
});
