window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    if (window.desks === undefined) {
        window.desks = Object.create(null);
    }

    var extra = function() {
        var _this = this;

        var dom = Object.create({
            areas: null,
            assignCount: document.getElementsByClassName("dsk-mct-asd-a"),
            finishCount: document.getElementsByClassName("dsk-mct-asd-f"),
            certifyCount: document.getElementsByClassName("dsk-mct-asd-c")
        });

        var domElements = null;

        var areas = {}, extraAreas = {};
        var buttons = {};

        function resizeRows() {
            if (dom.subjects !== null) {
                var heights = [];

                for (var i = 0, l = dom.areas.length; i < l; i++) {
                    var scrollBox = dom.areas[i].getElementsByClassName("mCustomScrollBox")[0];
                    scrollBox.style.height = null;
                    heights.push([scrollBox]);
                }

                for (var i = 0, l = dom.areas.length; i < l; i++) {
                    heights[i].push(dom.areas[i].clientHeight + "px");
                }

                for (var i = 0, l = heights.length; i < l; i++) {
                    heights[i][0].style.height = heights[i][1];
                }
            }
        }

        function findLevel(id, levels) {
            function recursiveFindLevel(group) {
                for (var i = 0, l = group.length; i < l; i++) {
                    if (group[i].id === id) {
                        return group[i];
                    } else if (group[i].children !== null) {
                        var level = recursiveFindLevel(group[i].children);
                        if (level !== null) {
                            return level;
                        }
                    }
                }

                return null;
            }

            return recursiveFindLevel(levels);
        }

        //#region Draw
        function filterColumns(columns, areas) {
            for (var i = 0, l = columns.length; i < l; i++) {
                for (var j = 0, k = columns[i].rows.length; j < k; j++) {
                    for (var m = 0, n = columns[i].rows[j].length; m < n; m++) {
                        if (areas.indexOf(columns[i].rows[j][m].area) === -1) {
                            columns[i].rows[j][m] = undefined;
                        }
                    }

                    columns[i].rows[j].clean(undefined);
                    if (columns[i].rows[j].length === 0) {
                        columns[i].rows[j] = undefined;
                    }
                }

                columns[i].rows.clean(undefined);
                if (columns[i].rows.length === 0) {
                    columns[i] = undefined;
                }
            }

            columns.clean(undefined);
        }

        function filterAreas(areas, areaTypes) {
            for (var i = 0, l = areas.length; i < l; i++) {
                for (var j = 0, k = areas[i].types.length; j < k; j++) {
                    if (areaTypes[areas[i].id].indexOf(areas[i].types[j].id) === -1) {
                        areas[i].types[j] = undefined;
                    }
                }

                areas[i].types.clean(undefined);
            }
        }

        var drawArea = {
            1: function(area, containers, visual) {
                if (visual === _this.visuals.Dean) {
                    var search = document.createElement("input");
                    search.className = "dsk-ext-srch";
                    search.placeholder = i18n.t("Search") + "...";
                    containers[-1].appendChild(search);

                    $(search).on("input", function() {
                        var text = this.value.trim();
                        var buttons = [];

                        for (var i in containers) {
                            if (i !== "-1" && Object.prototype.hasOwnProperty.call(containers, i)) {
                                buttons.pushRange(containers[i].getElementsByClassName("dsk-btn-w"));
                            }
                        }

                        if (text === "") {
                            for (var i = 0, l = buttons.length; i < l; i++) {
                                buttons[i].style.display = null;
                            }
                        } else {
                            for (var i = 0, l = buttons.length; i < l; i++) {
                                var title = buttons[i].getElementsByClassName("dsk-btn")[0].dataset.title;
                                if (title === null || title.match(text) === null) {
                                    buttons[i].style.display = "none";
                                } else {
                                    buttons[i].style.display = null;
                                }
                            }
                        }

                        resizeRows();
                    });
                }
            },

            2: function(area, containers, visual) {
                if (visual === _this.visuals.Dean) {
                    var search = document.createElement("input");
                    search.className = "dsk-ext-srch";
                    search.placeholder = i18n.t("Search") + "...";
                    containers[-1].appendChild(search);

                    $(search).on("input", function() {
                        var text = this.value.trim();
                        var buttons = [];

                        for (var i in containers) {
                            if (i !== "-1" && Object.prototype.hasOwnProperty.call(containers, i)) {
                                buttons.pushRange(containers[i].getElementsByClassName("dsk-btn-w"));
                            }
                        }

                        if (text === "") {
                            for (var i = 0, l = buttons.length; i < l; i++) {
                                buttons[i].style.display = null;
                            }
                        } else {
                            for (var i = 0, l = buttons.length; i < l; i++) {
                                var title = buttons[i].getElementsByClassName("dsk-btn")[0].dataset.title;
                                if (title === null || title.match(text) === null) {
                                    buttons[i].style.display = "none";
                                } else {
                                    buttons[i].style.display = null;
                                }
                            }
                        }

                        resizeRows();
                    });
                }
            },

            3: function(area, containers, visual) {
                var container = containers[0];

                for (var i = 0, l = area.extraData.length; i < l; i++) {
                    var test = document.createElement("div");
                    test.className = "dsk-ext-pts-tst";
                    container.appendChild(test);

                    var num = document.createElement("div");
                    num.className = "dsk-ext-pts-tst-nm";
                    if (visual === _this.visuals.Dean) {
                        num.className += " dsk-ext-pts-tst-dnm";
                        num.innerText = findLevel(area.extraData[i], window.model.levels).name;
                    } else {
                        num.innerText = i + 1;
                    }
                    test.appendChild(num);

                    var units = document.createElement("div");
                    units.className = "dsk-ext-pts-tst-un";
                    units.dataset.id = area.extraData[i];
                    test.appendChild(units);

                    extraAreas[3][area.extraData[i]] = [units, 0];
                }
            }
        }

        function drawRow(area, data, container, visual) {
            areas[data.area] = {};
            extraAreas[data.area] = {};

            var row = document.createElement("div");
            row.className = "dsk-row dsk-exs-row dsk-row-" + data.rows;
            row.dataset.area = data.area;
            container.appendChild(row);

            var title = document.createElement("div");
            title.className = "dsk-hdr dsk-ext-are";
            row.appendChild(title);
            areas[data.area][-1] = title;

            var tt = document.createElement("div");
            tt.className = "dsk-hdr-tt";
            tt.innerText = i18n.t("desks.extra.areas." + data.area + ".0");
            title.appendChild(tt);

            var contents = document.createElement("div");
            contents.className = "dsk-sbs";
            row.appendChild(contents);

            var contentsWrapper = document.createElement("div");
            contentsWrapper.className = "dsk-sbs-c";
            contents.appendChild(contentsWrapper);

            if (area.types.length === 0) {
                var contentType = document.createElement("div");
                contentType.className = "dsk-ext-cty dsk-ext-cty-act";
                contentType.className += " dsk-ext-cty-" + data.area + " dsk-ext-cty-" + data.area + "-0";
                contentType.dataset.id = 0;
                contentsWrapper.appendChild(contentType);

                areas[data.area][0] = contentType;

            } else {
                var typesWrapper = document.createElement("div");
                typesWrapper.className = "dsk-ext-twr";
                title.appendChild(typesWrapper);

                for (var i = 0, j = area.types.length; i < j; i++) {
                    var type = document.createElement("span");
                    type.className = "dsk-ext-tbt input " + (i === 0 ? "input-or input-nh" : "input-gr");
                    type.dataset.id = area.types[i].id;
                    type.innerText = i18n.t("desks.extra.areas." + data.area + "." + area.types[i].id);
                    typesWrapper.appendChild(type);

                    var contentType = document.createElement("div");
                    contentType.className = "dsk-ext-cty" + (i === 0 ? " dsk-ext-cty-act" : "");
                    contentType.className += " dsk-ext-cty-" + data.area + " dsk-ext-cty-" + data.area + "-" + area.types[i].id;
                    contentType.dataset.id = area.types[i].id;
                    contentsWrapper.appendChild(contentType);

                    areas[data.area][area.types[i].id] = contentType;
                }
            }

            if (Object.prototype.hasOwnProperty.call(drawArea, data.area)) {
                drawArea[data.area](area, areas[data.area], visual);
            }
        }

        function drawColumn(areas, data, container, total, visual) {
            var clm = document.createElement("div");
            clm.className = "dsk-clm";
            clm.style.flexShrink = total - data.columns;
            container.appendChild(clm);

            for (var i = 0, l = data.rows.length; i < l; i++) {
                var cc = document.createElement("div");
                cc.className = "dsk-clm-c";
                clm.appendChild(cc);

                for (var j = 0, k = data.rows[i].length; j < k; j++) {
                    var area;
                    for (var m = 0, n = areas.length; m < n; m++) {
                        if (areas[m].id === data.rows[i][j].area) {
                            area = areas[m];
                            break;
                        }
                    }

                    drawRow(area, data.rows[i][j], cc, visual);
                }
            }

            return clm;
        }

        var drawPartArea = {
            1: (function(num) {
                return function(d, a, v) {
                    var buttonWrapper = document.createElement("span");
                    buttonWrapper.className = "dsk-btn-w dsk-ext-btn-1";

                    var buttonContent = document.createElement("span");
                    buttonContent.className = "dsk-btn-cw";
                    buttonWrapper.appendChild(buttonContent);

                    var button = document.createElement("span");
                    button.className = "dsk-btn-c";
                    button.dataset.id = d.id;
                    buttonContent.appendChild(button);

                    var buttonData = document.createElement("span");
                    buttonData.className = "dsk-btn";
                    buttonData.innerText = ++num;
                    buttonData.title = d.mainData.title;
                    buttonData.dataset.title = d.mainData.title;
                    button.appendChild(buttonData);

                    buttons[d.id] = {};

                    for (var i in a) {
                        if (Object.prototype.hasOwnProperty.call(a, i) && parseInt(i) >= 0) {
                            var t = null;
                            for (var j = 0, k = d.types.length; j < k; j++) {
                                if (d.types[j].type === parseInt(i)) {
                                    t = d.types[j];
                                    break;
                                }
                            }

                            var btn = buttonWrapper.cloneNode(true);
                            var bc = btn.getElementsByClassName("dsk-btn-c")[0];
                            var b = bc.getElementsByClassName("dsk-btn")[0];
                            if (t === null) {
                                bc.className += " dsk-btn-in";
                                b.title = null;
                                b.dataset.title = null;
                            } else {
                                bc.dataset.type = t.type;
                                buttons[d.id][t.type] = bc;

                                $(b).tipsy({
                                    gravity: "s",
                                    html: true,
                                    title: function() {
                                        var p = document.createElement("p");

                                        var data = document.createElement("p");
                                        data.style.fontSize = "2em";
                                        data.style.lineHeight = "1.2em";
                                        data.style.margin = "0";
                                        data.innerText = this.dataset.title;
                                        p.appendChild(data);

                                        return p.innerHTML;
                                    }
                                });
                            }

                            a[i].appendChild(btn);
                        }
                    }
                }
            })(0),

            2: (function(num) {
                return function(d, a, v) {
                    var buttonWrapper = document.createElement("span");
                    buttonWrapper.className = "dsk-btn-w dsk-ext-btn-2";

                    var buttonContent = document.createElement("span");
                    buttonContent.className = "dsk-btn-cw";
                    buttonWrapper.appendChild(buttonContent);

                    var button = document.createElement("span");
                    button.className = "dsk-btn-c";
                    button.dataset.id = d.id;
                    buttonContent.appendChild(button);

                    var buttonData = document.createElement("span");
                    buttonData.className = "dsk-btn";
                    buttonData.innerText = ++num;
                    buttonData.title = d.mainData.title;
                    buttonData.dataset.title = d.mainData.title;
                    button.appendChild(buttonData);

                    buttons[d.id] = {};

                    for (var i in a) {
                        if (Object.prototype.hasOwnProperty.call(a, i) && parseInt(i) >= 0) {
                            var t = null;
                            for (var j = 0, k = d.types.length; j < k; j++) {
                                if (d.types[j].type === parseInt(i)) {
                                    t = d.types[j];
                                    break;
                                }
                            }

                            var btn = buttonWrapper.cloneNode(true);
                            var bc = btn.getElementsByClassName("dsk-btn-c")[0];
                            var b = bc.getElementsByClassName("dsk-btn")[0];

                            if (t === null) {
                                bc.className += " dsk-btn-in";
                                b.title = null;
                                b.dataset.title = null;
                            } else {
                                bc.dataset.type = t.type;
                                buttons[d.id][t.type] = bc;

                                $(b).tipsy({
                                    gravity: "s",
                                    html: true,
                                    title: function() {
                                        var p = document.createElement("p");

                                        var data = document.createElement("p");
                                        data.style.fontSize = "2em";
                                        data.style.lineHeight = "1.2em";
                                        data.style.margin = "0";
                                        data.innerText = this.dataset.title;
                                        p.appendChild(data);

                                        return p.innerHTML;
                                    }
                                });
                            }

                            a[i].appendChild(btn);
                        }
                    }
                }
            })(0),

            3: function(d, a, v) {
                var buttonWrapper = document.createElement("span");
                buttonWrapper.className = "dsk-btn-w dsk-ext-btn-3";

                var buttonContent = document.createElement("span");
                buttonContent.className = "dsk-btn-cw";
                buttonWrapper.appendChild(buttonContent);

                var button = document.createElement("span");
                button.className = "dsk-btn-c";
                button.dataset.id = d.id;
                buttonContent.appendChild(button);

                var buttonData = document.createElement("span");
                buttonData.className = "dsk-btn";
                button.appendChild(buttonData);

                for (var i = 0, l = d.mainData.length; i < l; i++) {
                    var btn = buttonWrapper.cloneNode(true);
                    btn.getElementsByClassName("dsk-btn")[0].innerText = ++extraAreas[3][d.mainData[i]][1];
                    extraAreas[3][d.mainData[i]][0].appendChild(btn);
                }

                buttons[d.id] = { 0: button };
            },

            4: function(d, a, v) {
                var buttonWrapper = document.createElement("span");
                buttonWrapper.className = "dsk-btn-w dsk-ext-btn-4";

                var buttonContent = document.createElement("span");
                buttonContent.className = "dsk-btn-cw";
                buttonWrapper.appendChild(buttonContent);

                var button = document.createElement("span");
                button.className = "dsk-btn-c";
                button.dataset.id = d.id;
                buttonContent.appendChild(button);

                var buttonData = document.createElement("span");
                buttonData.className = "dsk-btn";
                buttonData.innerText = d.mainData.title;
                buttonData.title = d.mainData.title;
                button.appendChild(buttonData);

                a[0].appendChild(buttonWrapper);
                buttons[d.id] = { 0: button };
            }
        }

        function drawPart(data, area, visual) {
            drawPartArea[data.area](data, area, visual);
        }

        this.draw = function(columns, data, container, visual) {
            var ars = [], areaTypes = [];

            for (var i = 0, l = data.parts.length; i < l; i++) {
                if (ars.indexOf(data.parts[i].area) === -1) {
                    ars.push(data.parts[i].area);
                }

                if (areaTypes[data.parts[i].area] === undefined) {
                    areaTypes[data.parts[i].area] = [0];
                }

                if (data.parts[i].types !== null) {
                    for (var j = 0, k = data.parts[i].types.length; j < k; j++) {
                        if (areaTypes[data.parts[i].area].indexOf(data.parts[i].types[j].type) === -1) {
                            areaTypes[data.parts[i].area].push(data.parts[i].types[j].type);
                        }
                    }
                }
            }

            var c = JSON.parse(JSON.stringify(columns)); // Clone object
            filterColumns(c, ars);

            var a = JSON.parse(JSON.stringify(data.areas)); // Clone object
            filterAreas(a, areaTypes);

            var totalColumns = 0;

            for (var i = 0, l = c.length; i < l; i++) {
                totalColumns += c[i].columns;
            }

            domElements = [];

            for (var i = 0, l = c.length; i < l; i++) {
                domElements.push(drawColumn(a, c[i], container, totalColumns, visual));
            }

            dom.areas = container.getElementsByClassName("dsk-sbs");

            for (var i = 0, l = data.parts.length; i < l; i++) {
                drawPart(data.parts[i], areas[data.parts[i].area], visual);
            }

            $(dom.areas).mCustomScrollbar({
                axis: "y",
                scrollbarPosition: "inside",
                alwaysShowScrollbar: 0,
                scrollButtons: { enable: true },
                theme: "dark-3"
            });

            $(".dsk-ext-tbt").on("click", function() {
                if (!this.classList.contains("input-or")) {
                    var header = this.ancestor(".dsk-hdr", true);
                    var activeBtn = header.getElementsByClassName("input-or")[0];
                    activeBtn.classList.remove("input-or", "input-nh");
                    activeBtn.classList.add("input-gr");

                    this.classList.remove("input-gr");
                    this.classList.add("input-or", "input-nh");

                    var title = header.getElementsByClassName("dsk-hdr-tt")[0];
                    title.innerText = this.innerText;

                    var id = this.dataset.id;
                    var row = this.ancestor(".dsk-row", true);
                    var containers = row.getElementsByClassName("dsk-ext-cty");

                    var status = 0;
                    for (var i = 0, l = containers.length; i < l; i++) {
                        if (containers[i].classList.contains("dsk-ext-cty-act")) {
                            containers[i].classList.remove("dsk-ext-cty-act");
                            status++;
                        } else if (containers[i].dataset.id === id) {
                            containers[i].classList.add("dsk-ext-cty-act");
                            status++;
                        }

                        if (status === 2) {
                            break;
                        }
                    }

                    resizeRows();
                }
            });

            resizeRows();

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
        //#endregion

        //#region Assignments
        this.assignments = function(assignments) {
            var assigned = 0, finished = 0, certified = 0;

            for (var i = 0, l = assignments.length; i < l; i++) {
                if (assignments[i].certified) {
                    buttons[assignments[i].id][assignments[i].type].classList.add("dsk-btn-cr");
                } else if (assignments[i].finished) {
                    buttons[assignments[i].id][assignments[i].type].classList.add("dsk-btn-fn");
                } else if (assignments[i].assigned) {
                    buttons[assignments[i].id][assignments[i].type].classList.add("dsk-btn-as");

                    if (assignments[i].blocked) {
                        buttons[assignments[i].id][assignments[i].type].classList.add("dsk-btn-bl");
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

        window.addEventListener("resize", resizeRows, true);
    }

    extra.prototype.visuals = Object.create({
        User: 1,
        Teacher: 2,
        Viewer: 3,
        Dean: 4
    });

    window.desks.extra = extra;
});
