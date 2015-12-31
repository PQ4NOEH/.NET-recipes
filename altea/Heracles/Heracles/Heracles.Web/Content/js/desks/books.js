window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    if (window.desks === undefined) {
        window.desks = Object.create(null);
    }

    var books = function() {
        var _this = this;

        var dom = Object.create({
            types: null,
            assignCount: document.getElementsByClassName("dsk-mct-asd-a"),
            finishCount: document.getElementsByClassName("dsk-mct-asd-f"),
            certifyCount: document.getElementsByClassName("dsk-mct-asd-c")
        });

        var api = Object.create({
            cover: "/Books/Cover/{publication}/{collection}"
        });

        var domElements = null;

        var types = {};
        var buttons = {};

        function resizeRows(c) {
            if (dom.types !== null) {
                var heights = [];

                if (c === undefined) {
                    for (var i = 0, l = dom.types.length; i < l; i++) {
                        var scrollBox = dom.types[i].getElementsByClassName("mCustomScrollBox")[0];
                        scrollBox.style.height = null;
                        heights.push([dom.types[i], scrollBox]);
                    };

                    for (var i = 0, l = heights.length; i < l; i++) {
                        heights[i].push(heights[i][0].clientHeight + "px");
                    }
                } else {
                    var scrollBox = c.getElementsByClassName("mCustomScrollBox")[0];
                    scrollBox.style.height = null;
                    heights.push([c, scrollBox]);
                    heights[0].push(heights[0][0].clientHeight + "px");
                }

                for (var i = 0, l = heights.length; i < l; i++) {
                    heights[i][1].style.height = heights[i][2];

                    var width = heights[i][0].clientWidth;

                    var change = null;
                    if (width <= 415) {
                        if (!heights[i][0].classList.contains("dsk-bks-w2")) {
                            change = "dsk-bks-w2";
                        }
                    } else if (width <= 550) {
                        if (!heights[i][0].classList.contains("dsk-bks-w3")) {
                            change = "dsk-bks-w3";
                        }
                    } else if (width <= 700) {
                        if (!heights[i][0].classList.contains("dsk-bks-w4")) {
                            change = "dsk-bks-w4";
                        }
                    } else if (width <= 825) {
                        if (!heights[i][0].classList.contains("dsk-bks-w5")) {
                            change = "dsk-bks-w5";
                        }
                    } else if (width <= 1200) {
                        if (!heights[i][0].classList.contains("dsk-bks-w6")) {
                            change = "dsk-bks-w6";
                        }
                    } else {
                        if (!heights[i][0].classList.contains("dsk-bks-w7")) {
                            change = "dsk-bks-w7";
                        }
                    }

                    if (change !== null) {
                        heights[i][0].classList.remove("dsk-bks-w2", "dsk-bks-w3", "dsk-bks-w4", "dsk-bks-w5", "dsk-bks-w6", "dsk-bks-w7");
                        heights[i][0].classList.add(change);
                    }
                }
            }
        }

        //#region Draw
        function filterColumns(columns, types) {
            for (var i = 0, l = columns.length; i < l; i++) {
                for (var j = 0, k = columns[i].rows.length; j < k; j++) {
                    for (var m = 0, n = columns[i].rows[j].length; m < n; m++) {
                        if (types.indexOf(columns[i].rows[j][m].type) === -1) {
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

        var drawType = {
            1: function(containers, visual) {
                if (visual === _this.visuals.Dean) {
                    var search = document.createElement("input");
                    search.className = "dsk-bks-srch";
                    search.placeholder = i18n.t("Search") + "...";
                    containers[-1].insertBefore(search, containers[-1].firstChild);

                    $(search).on("input", function() {
                        var text = this.value.trim();
                        var buttons = [];
                        //TODO
                        for (var i in containers) {
                            if (i !== "-1" && Object.prototype.hasOwnProperty.call(containers, i)) {
                                //buttons.pushRange(containers[i].getElementsByClassName("dsk-btn-w"));
                            }
                        }

                        //if (text === "") {
                        //    for (var i = 0, l = buttons.length; i < l; i++) {
                        //        buttons[i].style.display = null;
                        //    }
                        //} else {
                        //    for (var i = 0, l = buttons.length; i < l; i++) {
                        //        var title = buttons[i].getElementsByClassName("dsk-btn")[0].dataset.title;
                        //        if (title === null || title.match(text) === null) {
                        //            buttons[i].style.display = "none";
                        //        } else {
                        //            buttons[i].style.display = null;
                        //        }
                        //    }
                        //}

                        resizeRows();
                    });
                }
            },

            2: function(containers, visual) {
                if (visual === _this.visuals.Dean) {
                    var search = document.createElement("input");
                    search.className = "dsk-bks-srch";
                    search.placeholder = i18n.t("Search") + "...";
                    containers[-1].insertBefore(search, containers[-1].firstChild);

                    $(search).on("input", function() {
                        var text = this.value.trim();
                        var buttons = [];
                        //TODO
                        for (var i in containers) {
                            if (i !== "-1" && Object.prototype.hasOwnProperty.call(containers, i)) {
                                //buttons.pushRange(containers[i].getElementsByClassName("dsk-btn-w"));
                            }
                        }

                        //if (text === "") {
                        //    for (var i = 0, l = buttons.length; i < l; i++) {
                        //        buttons[i].style.display = null;
                        //    }
                        //} else {
                        //    for (var i = 0, l = buttons.length; i < l; i++) {
                        //        var title = buttons[i].getElementsByClassName("dsk-btn")[0].dataset.title;
                        //        if (title === null || title.match(text) === null) {
                        //            buttons[i].style.display = "none";
                        //        } else {
                        //            buttons[i].style.display = null;
                        //        }
                        //    }
                        //}

                        resizeRows();
                    });
                }
            }
        }

        function drawRow(type, data, exercises, container, visual) {
            types[data.type] = {};

            var row = document.createElement("div");
            row.className = "dsk-row dsk-bks-row dsk-row-" + data.rows;
            row.dataset.area = data.type;
            container.appendChild(row);

            var title = document.createElement("div");
            title.className = "dsk-hdr dsk-bks-are";
            row.appendChild(title);
            types[data.type][-1] = title;

            var tt = document.createElement("div");
            tt.className = "dsk-hdr-tt dsk-hdr-ltt";
            tt.innerText = i18n.t("desks.books.types." + data.type);
            title.appendChild(tt);

            var contents = document.createElement("div");
            contents.className = "dsk-sbs dsk-bks-sbs";
            row.appendChild(contents);

            var contentsWrapper = document.createElement("div");
            contentsWrapper.className = "dsk-sbs-c dsk-bks-sbs-c";
            contents.appendChild(contentsWrapper);

            var typesWrapper = document.createElement("div");
            typesWrapper.className = "dsk-bks-twr";
            title.appendChild(typesWrapper);

            if (exercises[0].indexOf(type.id) !== -1) {
                var articlesType = document.createElement("span");
                articlesType.className = "dsk-bks-tbt input input-gr";
                articlesType.dataset.id = 0;
                articlesType.innerText = i18n.t("desks.books.data.articles");
                typesWrapper.appendChild(articlesType);

                var articlesContent = document.createElement("div");
                articlesContent.className = "dsk-bks-cty";
                articlesContent.dataset.id = 0;
                contentsWrapper.appendChild(articlesContent);
                types[data.type][0] = articlesContent;
            }

            if (exercises[1].indexOf(type.id) !== -1) {
                var booksType = document.createElement("span");
                booksType.className = "dsk-bks-tbt input input-gr";
                booksType.dataset.id = 1;
                booksType.innerText = i18n.t("desks.books.data.books");
                typesWrapper.appendChild(booksType);

                var booksContent = document.createElement("div");
                booksContent.className = "dsk-bks-cty";
                booksContent.dataset.id = 1;
                contentsWrapper.appendChild(booksContent);
                types[data.type][1] = booksContent;
            }

            if (exercises[2].indexOf(type.id) !== -1) {
                var collectionsType = document.createElement("span");
                collectionsType.className = "dsk-bks-tbt input input-gr";
                collectionsType.dataset.id = 2;
                collectionsType.innerText = i18n.t("desks.books.data.collections");
                typesWrapper.appendChild(collectionsType);

                var collectionsContent = document.createElement("div");
                collectionsContent.className = "dsk-bks-cty";
                collectionsContent.dataset.id = 2;
                contentsWrapper.appendChild(collectionsContent);
                types[data.type][2] = collectionsContent;
            }

            if (Object.prototype.hasOwnProperty.call(drawType, data.type)) {
                drawType[data.type](types[data.type], visual);
            }
        }

        function drawColumn(types, data, exercises, container, total, visual) {
            var clm = document.createElement("div");
            clm.className = "dsk-clm";
            clm.style.flexShrink = total - data.columns;
            container.appendChild(clm);

            for (var i = 0, l = data.rows.length; i < l; i++) {
                var cc = document.createElement("div");
                cc.className = "dsk-clm-c";
                clm.appendChild(cc);

                for (var j = 0, k = data.rows[i].length; j < k; j++) {
                    var type;
                    for (var m = 0, n = types.length; m < n; m++) {
                        if (types[m].id === data.rows[i][j].type) {
                            type = types[m];
                            break;
                        }
                    }

                    drawRow(type, data.rows[i][j], exercises, cc, visual);
                }
            }

            return clm;
        }

        function drawArticle(data, type, container) {
            //console.log(data, type, container);
        }

        function drawPublication(data, type, container, collection, visual) {
            var b = document.createElement("div");
            b.className = "dsk-bks-bok";
            b.dataset.id = data.id;
            container.appendChild(b);

            var bc = document.createElement("div");
            bc.className = "dsk-bks-bok-c";
            b.appendChild(bc);

            var cover = document.createElement("div");
            cover.className = "dsk-bks-cvr";
            bc.appendChild(cover);

            var cv = api.cover;
            cv = cv.replace("{publication}", data.id);
            cv = cv.replace("{collection}", +collection);

            var coverImg = document.createElement("img");
            coverImg.src = cv;
            cover.appendChild(coverImg);

            var d = document.createElement("div");
            d.className = "dsk-bks-btm";
            bc.appendChild(d);

            var numChapters = 0;
            for (var i in data.publications) {
                if (Object.prototype.hasOwnProperty.call(data.publications, i)) {
                    numChapters++;
                }
            }

            var chapters = document.createElement("div");
            chapters.className = "dsk-bks-btm-btn dsk-bks-btm-ttl";
            chapters.dataset.id = data.id;
            chapters.innerText = numChapters;
            d.appendChild(chapters);

            var assigned = document.createElement("div");
            assigned.className = "dsk-bks-btm-btn dsk-bks-btm-asg";
            assigned.dataset.id = data.id;
            assigned.innerText = 0;
            d.appendChild(assigned);

            var finished = document.createElement("div");
            finished.className = "dsk-bks-btm-btn dsk-bks-btm-fns";
            finished.dataset.id = data.id;
            finished.innerText = 0;
            d.appendChild(finished);

            var certified = document.createElement("div");
            certified.className = "dsk-bks-btm-btn dsk-bks-btm-crt";
            certified.dataset.id = data.id;
            certified.innerText = 0;
            d.appendChild(certified);

            var bd = document.createElement("div");
            bd.className = "dsk-bks-dta";
            bc.appendChild(bd);

            var bdc = document.createElement("div");
            bdc.className = "dsk-bks-dta-cte";
            bd.appendChild(bdc);

            var title = document.createElement("div");
            title.className = "dsk-bks-dta-ttl";
            title.innerText = data.title === undefined ? data.name : data.title;
            bdc.appendChild(title);

            if (data.authors !== undefined && data.authors.length !== 0) {
                var authors = document.createElement("div");
                authors.className = "dsk-bks-dta-aut";
                authors.innerText = "";
                bdc.appendChild(authors);

                for (var i = 0, l = data.authors.length; i < l; i++) {
                    authors.innerText += data.authors[i];
                    if (i + 1 < l) {
                        authors.innerText += ", ";
                    }
                }
            }

            if (visual === _this.visuals.Dean && data.levels !== undefined && data.levels.length !== 0) {
                var lvls = document.createElement("div");
                lvls.className = "dsk-bks-dta-lvs";
                lvls.innerText = "";
                bdc.appendChild(lvls);

                for (var i = 0, l = data.levels.length; i < l; i++) {
                    lvls.innerText += books.findLevel(data.levels[i]).adminDisplayName;
                    if (i + 1 < l) {
                        lvls.innerText += ", ";
                    }
                }
            }
        }

        function drawBook(data, type, container, visual) {
            if (container === null) {
                container = types[type][1];
            }

            drawPublication(data, type, container, false, visual);
        }

        function drawCollection(data, type, visual) {
            drawPublication(data, type, types[type][2], true, visual);
        }

        this.draw = function(columns, data, container, visual) {
            var areas = ["articles", "books", "collections"];
            var exerciseTypes = [[], [], []];
            for (var i = 0, l = areas.length; i < l; i++) {
                for (var j = 0, k = data[areas[i]].length; j < k; j++) {
                    for (var m = 0, n = data[areas[i]][j].exerciseTypes.length; m < n; m++) {
                        if (exerciseTypes[i].indexOf(data[areas[i]][j].exerciseTypes[m]) === -1) {
                            exerciseTypes[i].push(data[areas[i]][j].exerciseTypes[m]);
                        }
                    }
                }
            }

            var tps = [];
            for (var i = 0, l = exerciseTypes.length; i < l; i++) {
                for (var j = 0, k = exerciseTypes[i].length; j < k; j++) {
                    if (tps.indexOf(exerciseTypes[i][j]) === -1) {
                        tps.push(exerciseTypes[i][j]);
                    }
                }
            }

            var c = JSON.parse(JSON.stringify(columns)); // Clone object
            filterColumns(c, tps);

            var totalColumns = 0;
            for (var i = 0, l = c.length; i < l; i++) {
                totalColumns += c[i].columns;
            }

            domElements = [];

            for (var i = 0, l = c.length; i < l; i++) {
                domElements.push(drawColumn(data.types, c[i], exerciseTypes, container, totalColumns, visual));
            }

            for (var i = 0, l = areas.length; i < l; i++) {
                for (var j = 0, k = data[areas[i]].length; j < k; j++) {
                    for (var m = 0, n = data[areas[i]][j].exerciseTypes.length; m < n; m++) {
                        if (areas[i] === "articles") {
                            drawArticle(data[areas[i]][j], data[areas[i]][j].exerciseTypes[m], null, visual);
                        } else if (areas[i] === "books") {
                            drawBook(data[areas[i]][j], data[areas[i]][j].exerciseTypes[m], null, visual);
                        } else if (areas[i] === "collections") {
                            drawCollection(data[areas[i]][j], data[areas[i]][j].exerciseTypes[m], visual);
                        }
                    }
                }
            }

            dom.types = container.getElementsByClassName("dsk-sbs");

            $(dom.types).mCustomScrollbar({
                axis: "y",
                scrollbarPosition: "inside",
                alwaysShowScrollbar: 2,
                scrollButtons: { enable: true },
                theme: "dark-3"
            });

            $(".dsk-bks-tbt").on("click", function() {
                if (!this.classList.contains("input-or")) {
                    var header = this.ancestor(".dsk-hdr", true);
                    var activeBtn = header.getElementsByClassName("input-or")[0];
                    if (activeBtn !== undefined) {
                        activeBtn.classList.remove("input-or", "input-nh");
                        activeBtn.classList.add("input-gr");
                    }

                    this.classList.remove("input-gr");
                    this.classList.add("input-or", "input-nh");

                    var id = this.dataset.id;
                    var row = this.ancestor(".dsk-row", true);
                    var containers = row.getElementsByClassName("dsk-bks-cty");

                    var status = 0;
                    for (var i = 0, l = containers.length; i < l; i++) {
                        if (containers[i].classList.contains("dsk-bks-cty-act")) {
                            containers[i].classList.remove("dsk-bks-cty-act");
                            status++;
                        } else if (containers[i].dataset.id === id) {
                            containers[i].classList.add("dsk-bks-cty-act");
                            status++;
                        }

                        if (status === 2) {
                            break;
                        }
                    }

                    resizeRows(row.getElementsByClassName("dsk-sbs")[0]);
                }
            });

            for (var i in types) {
                if (Object.prototype.hasOwnProperty.call(types, i)) {
                    $(types[i][-1].getElementsByClassName("dsk-bks-tbt")[0]).trigger("click");
                }
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
        //#endregion

        //#region Assignments
        this.assignments = function(assignments) {
        }
        //#endregion

        window.addEventListener("resize", function() {
            resizeRows();
        }, true);
    }

    books.prototype.visuals = Object.create({
        User: 1,
        Teacher: 2,
        Viewer: 3,
        Dean: 4
    });

    books.levels = null;

    books.findLevel = function(id) {
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

        return recursiveFindLevel(window.model.levels);
    }

    window.desks.books = books;
});
