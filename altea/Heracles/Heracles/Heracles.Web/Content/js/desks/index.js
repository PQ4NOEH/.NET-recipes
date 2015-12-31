window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    if (window.desks === undefined) {
        window.desks = Object.create(null);
    }

    var ENUM_TYPES = Object.create({
        1: "ChooseFromTheBox",
        3: "MultipleChoice",
        5: "GapFilling",
        6: "WordFormation",
        7: "Rephrasing"
    });

    var index = function() {
        var dom = Object.create({
            subjects: null,
            assignCount: document.getElementsByClassName("dsk-mct-asd-a"),
            finishCount: document.getElementsByClassName("dsk-mct-asd-f"),
            certifyCount: document.getElementsByClassName("dsk-mct-asd-c")
        });

        var domElements = null;

        var types = [];
        var areas = undefined;
        var subjects = {};

        function resizeRows() {
            if (dom.subjects !== null) {
                var heights = [];

                for (var i = 0, l = dom.subjects.length; i < l; i++) {
                    var scrollBox = dom.subjects[i].getElementsByClassName("mCustomScrollBox")[0];
                    scrollBox.style.height = null;
                    heights.push([scrollBox]);
                };

                for (var i = 0, l = dom.subjects.length; i < l; i++) {
                    heights[i].push(dom.subjects[i].clientHeight + "px");
                }

                for (var i = 0, l = heights.length; i < l; i++) {
                    heights[i][0].style.height = heights[i][1];
                }
            }
        }

        //#region Draw
        function filterColumns(columns, data) {
            var areas = [];

            for (var i = 0, l = data.subjects.length; i < l; i++) {
                for (var j = 0, k = data.subjects[i].areas.length; j < k; j++) {
                    if (areas.indexOf(data.subjects[i].areas[j].area) === -1) {
                        if (data.subjects[i].areas[j].boards.length !== 0) {
                            areas.push(data.subjects[i].areas[j].area);
                        }
                    }
                }
            }

            for (var i = 0, l = columns.length; i < l; i++) {
                if (areas.indexOf(columns[i].id) === -1) {
                    columns[i] = undefined;
                }
            }

            columns.clean(undefined);
        }

        function drawRow(row, column) {
            var r = document.createElement("div");
            r.className = "dsk-row dsk-exs-row dsk-row-" + row.rowSize;
            r.dataset.id = row.id;
            column.appendChild(r);

            var title = document.createElement("div");
            title.className = "dsk-hdr dsk-are";
            title.innerText = row.name;
            r.appendChild(title);

            var subjects = document.createElement("div");
            subjects.className = "dsk-sbs";
            r.appendChild(subjects);

            var subjectsContent = document.createElement("div");
            subjectsContent.className = "dsk-sbs-c";
            subjects.appendChild(subjectsContent);

            areas[row.id] = subjectsContent;
        }

        function drawColumn(column, container) {
            var c = document.createElement("div");
            c.className = "dsk-clm";
            container.appendChild(c);

            var cc = document.createElement("div");
            cc.className = "dsk-clm-c";
            c.appendChild(cc);

            for (var i = 0, l = column.length; i < l; i++) {
                drawRow(column[i], cc);
            }

            return c;
        }

        function drawColumns(columns, container) {
            var cs = [];
            var cols = [], colsData = [];

            for (var i = 0, l = columns.length; i < l; i++) {
                var indexOf = cols.indexOf(columns[i].column);
                if (indexOf === -1) {
                    cols.push(columns[i].column);
                    indexOf = colsData.length;
                    colsData.push([]);
                }

                colsData[indexOf].push(columns[i]);
            }

            for (var i = 0, l = cols.length; i < l; i++) {
                var c = drawColumn(colsData[i], container);
                cs.push(c);
            }

            dom.subjects = container.getElementsByClassName("dsk-sbs");

            return cs;
        }

        function drawSubjectButtons(subject, boards) {
            var wrapper = document.createElement("div");
            wrapper.className = "dsk-sbj-exs";
            subject.appendChild(wrapper);

            var buttons = {};

            for (var i = 0, l = types.length; i < l; i++) {
                var numBoards = boards[ENUM_TYPES[types[i].id]];
                if (numBoards === undefined) {
                    numBoards = 0;
                }

                var buttonWrapper = document.createElement("span");
                buttonWrapper.className = "dsk-btn-w";
                wrapper.appendChild(buttonWrapper);

                var buttonContent = document.createElement("span");
                buttonContent.className = "dsk-btn-cw";
                buttonWrapper.appendChild(buttonContent);

                var button = document.createElement("span");
                button.className = "dsk-btn-c";
                if (numBoards === 0) {
                    button.className += " dsk-btn-in";
                }
                button.dataset.id = types[i].id;
                button.dataset.boards = numBoards;
                buttonContent.appendChild(button);

                var buttonData = document.createElement("span");
                buttonData.className = "dsk-btn";
                buttonData.innerText = types[i].character;
                buttonData.title = types[i].name;
                button.appendChild(buttonData);

                buttons[types[i].id] = button;
            }

            return buttons;
        }

        function drawTheoryButton(subject, id) {
            var button = document.createElement("div");
            button.className = "dsk-thr";
            button.dataset.id = id;
            subject.appendChild(button);

            var theory = document.createElement("span");
            theory.className = "dsk-thr-b fa fa-graduation-cap";
            button.appendChild(theory);
        }

        //function addSubjectArrow(subject, area) { 
        //}

        function drawAreaSubject(subject, parent, area, level, container) {
            var boards = 0;

            for (var board in area.boards) {
                if (Object.prototype.hasOwnProperty.call(area.boards, board)) {
                    boards += area.boards[board];
                }
            }

            var sw = document.createElement("div");
            sw.className = "dsk-sbj-w dsk-sbj-lvl-" + level;
            container.appendChild(sw);

            var s = document.createElement("div");
            s.className = "dsk-sbj";
            s.dataset.id = subject.id;
            sw.appendChild(s);

            if (parent !== undefined && parent !== null) {
                s.dataset.parent = parent;
                //addSubjectArrow(parent, area.area);
            }

            var title = document.createElement("div");
            title.className = "dsk-subhdr dsk-sbj-tle";
            title.innerText = subject.name;
            title.title = subject.name;
            s.appendChild(title);

            if (area.theory !== null) {
                drawTheoryButton(s, area.theory);
            }

            var buttons = drawSubjectButtons(s, area.boards);

            if (subjects[subject.id] === undefined) {
                subjects[subject.id] = {};
            }

            subjects[subject.id][area.area] = [s, buttons];
        }

        function drawSubject(subject, parent, level) {
            if (level === undefined) {
                level = 0;
            }

            for (var i = 0, l = subject.areas.length; i < l; i++) {
                var container;
                if (level === 0) {
                    container = areas[subject.areas[i].area];
                } else {
                    var children = areas[subject.areas[i].area].getElementsByClassName("dsk-sbj-cw");
                    var c = undefined;
                    children.forEach(function (e) {
                        if (e.dataset.id == parent) {
                            c = e;
                            return false;
                        }

                        return true;
                    });
                    
                    if (c === undefined) {
                        c = document.createElement("div");
                        c.className = "dsk-sbj-cw";
                        c.dataset.id = parent;

                        var s = undefined;
                        areas[subject.areas[i].area].getElementsByClassName("dsk-sbj").forEach(function(e) {
                            if (e.dataset.id == parent) {
                                s = e;
                                return false;
                            }

                            return true;
                        });

                        if (s === undefined) {
                            areas[subject.areas[i].area].appendChild(c);
                        } else {
                            s.parentNode.insertBefore(c, s.nextSibling);
                        }
                    }

                    container = c;
                }

                drawAreaSubject(subject, parent, subject.areas[i], level, container);
            }

            if (subject.children !== null) {

                for (var i = 0, l = subject.children.length; i < l; i++) {
                    drawSubject(subject.children[i], subject.id, level + 1);
                }
            }
        }

        function drawSubjects(subjects, container) {
            for (var i = 0, l = subjects.length; i < l; i++) {
                drawSubject(subjects[i], container);
            }
        }

        this.draw = function(columns, data, container) {
            data.types.sort(function(a, b) {
                return a.position - b.position;
            });

            for (var i = 0, l = data.types.length; i < l; i++) {
                var t = Object.create({
                    id: data.types[i].exerciseType,
                    name: undefined,
                    character: undefined
                });

                t.name = i18n.t("desks.index.types." + t.id);
                t.character = t.name.substr(0, 1);

                types.push(t);
            }

            if (container === null) {
                for (var c in columns) {
                    if (Object.prototype.hasOwnProperty.call(columns, c)) {
                        while (columns[c].firstChild) {
                            columns[c].removeChild(columns[c].firstChild);
                        }

                        // TODO ADD TO COLS
                    }
                }
                areas = columns;
            } else {
                var finalColumns = JSON.parse(JSON.stringify(columns));
                filterColumns(finalColumns, data);

                areas = {};
                domElements = drawColumns(finalColumns, container);
            }

            drawSubjects(data.subjects);

            if (container !== null) {
                $(dom.subjects).mCustomScrollbar({
                    axis: "y",
                    scrollbarPosition: "inside",
                    alwaysShowScrollbar: 0,
                    scrollButtons: { enable: true },
                    theme: "dark-3"
                });

                resizeRows();
            }

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
        this.assignments = function(assignments, show) {
            var assigned = 0, finished = 0, certified = 0;

            for (var i in subjects) {
                if (Object.prototype.hasOwnProperty.call(subjects, i)) {
                    for (var j in subjects[i]) {
                        if (Object.prototype.hasOwnProperty.call(subjects[i], j)) {
                            for (var m in subjects[i][j][1]) {
                                if (Object.prototype.hasOwnProperty.call(subjects[i][j][1], m)) {
                                    subjects[i][j][1][m].classList.remove("dsk-btn-as", "dsk-btn-ras", "dsk-btn-bl", "dsk-btn-fn", "dsk-btn-cr");
                                }
                            }
                        }
                    }
                }
            }

            for (var i = 0, l = assignments.length; i < l; i++) {
                var button = subjects[assignments[i].subject][assignments[i].area][1][assignments[i].type];

                if (button.classList.contains("dsk-btn-in")) {
                    continue;
                }

                if (assignments[i].assigned > 0) {
                    button.classList.add("dsk-btn-as");

                    if (assignments[i].blocked > 0) {
                        button.classList.add("dsk-btn-bl");
                    }
                } else if (assignments[i].certified > 0) {
                    if (assignments[i].finished > 0) {
                        button.classList.add("dsk-btn-fc");
                    } else {
                        button.classList.add("dsk-btn-cr");
                    }
                } else if (assignments[i].finished > 0) {
                    button.classList.add("dsk-btn-fn");
                }

                assigned += assignments[i].assigned;
                finished += assignments[i].finished;
                certified += assignments[i].certified;
            }

            if (show) {
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
        }
        //#endregion

        window.addEventListener("resize", resizeRows, true);
    }

    window.desks.index = index;
});
