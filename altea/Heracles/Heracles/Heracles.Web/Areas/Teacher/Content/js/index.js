window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        container: document.getElementById("ct"),
        spinner: document.getElementById("tchr-spn"),
        levelsWrapper: document.getElementById("tchr-tp-fic"),
        levels: document.getElementById("tchr-tp-lvl"),
        groupsWrapper: document.getElementById("tchr-tp-sic"),
        groupsContainer: document.getElementById("tchr-tp-grp-bxc"),
        groups: document.getElementById("tchr-tp-grp"),
        groupDays: document.getElementById("tchr-tp-grp-days"),
        groupTeachers: document.getElementById("tchr-tp-grp-tchr"),
        searchGroup: document.getElementById("tchr-tp-grp-src"),
        noGroups: document.getElementById("tchr-tp-grp-ngr"),

        indexContent: document.getElementById("dsk-idx-c"),
        examsContent: document.getElementById("dsk-exm-c"),
        extraContent: document.getElementById("dsk-ext-c")
    });

    var api = Object.create({
        mainUri: "/Teacher",
        getLevel: "/Teacher/GetLevel",
        getGroup: "/Teacher/GetGroup"
    });

    var pagination = null;

    function clearScreen() {
        function clearPage(page) {
            while (page.firstChild) {
                page.removeChild(page.firstChild);
            }
        }

        clearPage(dom.indexContent);
        clearPage(dom.examsContent);
        clearPage(dom.extraContent);
    }

    var autoOpen = 0;
    var levelsOpened = false, groupsOpened = false;
    var levelSelected = null, groupSelected = null;

    var groupFiltering = {
        level: 0,
        day: -1,
        teacher: -1,
        search: null
    };

    function getGroupLevel(group, level) {
        var groupLevel = null;

        for (var i = 0, l = window.model.groups.length; i < l; i++) {
            if (window.model.groups[i].id === group) {
                for (var j = 0, k = window.model.groups[i].levels.length; j < k; j++) {
                    if (window.model.groups[i].levels[j].level === level) {
                        groupLevel = window.model.groups[i].levels[j];
                    }
                }
                break;
            }
        }

        return groupLevel;
    }

    function filterGroups() {
        var groups = dom.groupsWrapper.getElementsByClassName("tchr-tp-bxd");
        var visibleLevels = 0;

        for (var i = 0, l = groups.length; i < l; i++) {
            if (groups[i].classList.contains("tchr-tp-bxd-sel")) {
                groups[i].classList.remove("tchr-tp-bxd-hd");
                visibleLevels++;
            } else {
                if (parseInt(groups[i].dataset.level) !== groupFiltering.level) {
                    groups[i].classList.add("tchr-tp-bxd-hd");
                } else {
                    var dayOk = false, teacherOk = false, searchOk = false;

                    var groupLevel = null;

                    if (groupFiltering.day === -1) {
                        dayOk = true;
                    } else {
                        groupLevel = getGroupLevel(groups[i].dataset.id, levelSelected);
                        if (groupLevel === null) {
                            dayOk = false;
                        } else {
                            for (var j = 0, k = groupLevel.timetable.length; j < k; j++) {
                                if (groupLevel.timetable[j].weekday === groupFiltering.day) {
                                    dayOk = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (dayOk) {
                        if (groupFiltering.teacher === -1) {
                            teacherOk = true;
                        } else {
                            if (groupLevel === null) {
                                groupLevel = getGroupLevel(parseInt(groups[i].dataset.id), levelSelected);
                            }

                            if (groupLevel === null) {
                                teacherOk = false;
                            } else {
                                for (var j = 0, k = groupLevel.timetable.length; j < k; j++) {
                                    for (var m = 0, n = groupLevel.timetable[j].teachers.length; m < n; m++) {
                                        if (groupLevel.timetable[j].teachers[m] === groupFiltering.teacher) {
                                            teacherOk = true;
                                            break;
                                        }
                                    }

                                    if (teacherOk) {
                                        break;
                                    }
                                }
                            }
                        }

                        if (teacherOk) {
                            if (groupFiltering.search === null) {
                                searchOk = true;
                            } else {
                                searchOk = groups[i].innerText.match(groupFiltering.search) !== null;
                            }
                        }
                    }

                    if (dayOk && teacherOk && searchOk) {
                        groups[i].classList.remove("tchr-tp-bxd-hd");
                        visibleLevels++;
                    } else {
                        groups[i].classList.add("tchr-tp-bxd-hd");
                    }
                }
            }
        }

        if (visibleLevels === 0) {
            dom.noGroups.classList.add("tchr-tp-grp-ngr-act");
            dom.groupsContainer.classList.add("tchr-tp-bxc-hd");
        } else {
            dom.noGroups.classList.remove("tchr-tp-grp-ngr-act");
            dom.groupsContainer.classList.remove("tchr-tp-bxc-hd");
        }
    }

    var levelData = {};
    var lastLevelLoaded = 0;

    function loadLevelData(level, callback) {
        if (levelData[level] === undefined) {
            levelData[level] = false;

            $.ajax({
                async: true,
                type: "POST",
                url: api.getLevel,
                data: {
                    level: level
                },

                success: function(result) {
                    levelData[level] = {
                        data: result,
                        index: new window.desks.index(),
                        exams: new window.desks.exams(),
                        extra: new window.desks.extra(),
                        content: undefined
                    };

                    callback(true, false);
                },

                error: function() {
                    window.setInternalErrorMessage($("#ct-c"));
                }
            });
        } else if (levelData[level] === false) {
            setTimeout(function() {
                loadLevelData(level, callback);
            }, 100);
        } else {
            callback(true, false);
        }
    }

    function drawLevel(level) {
        clearScreen();

        if (levelData[level].content === undefined) {
            var elements;
            levelData[level].content = [];

            if (levelData[level].data.index !== null) {
                levelData[level].index.draw(window.model.areas, levelData[level].data.index, dom.indexContent);
                pagination.enablePage(0);

                elements = dom.indexContent.childNodes;
                levelData[level].content[0] = [];
                for (var i = 0, l = elements.length; i < l; i ++) {
                    levelData[level].content[0].push(elements[i]);
                }
            } else {
                pagination.disablePage(0);
                levelData[level].content[0] = [];
            }

            if (levelData[level].data.exams !== null) {
                levelData[level].exams.draw(levelData[level].data.exams, dom.examsContent, true);
                pagination.enablePage(1);

                elements = dom.examsContent.childNodes;
                levelData[level].content[1] = [];
                for (var i = 0, l = elements.length; i < l; i++) {
                    levelData[level].content[1].push(elements[i]);
                }
            } else {
                pagination.disablePage(1);
                levelData[level].content[1] = [];
            }

            if (levelData[level].data.extra !== null) {
                levelData[level].extra.draw(window.model.extra, levelData[level].data.extra, dom.extraContent);
                pagination.enablePage(2);

                elements = dom.extraContent.childNodes;
                levelData[level].content[2] = [];
                for (var i = 0, l = elements.length; i < l; i++) {
                    levelData[level].content[2].push(elements[i]);
                }
            } else {
                pagination.disablePage(2);
                levelData[level].content[2] = [];
            }
        } else {
            var domElems = [dom.indexContent, dom.examsContent, dom.extraContent];
            for (var i = 0, l = levelData[level].content.length; i < l; i++) {
                for (var j = 0, k = levelData[level].content[i].length; j < k; j++) {
                    domElems[i].appendChild(levelData[level].content[i][j]);
                }
            }
        }
    }

    function drawAssignments(assignments) {

    }

    function loadGroup(auto) {
        dom.container.classList.remove("tchr-ct-hd");
        dom.spinner.classList.add("app-spinner-active");

        var level = levelSelected;
        var group = groupSelected;

        var res = null;

        var x = (function(levelDataLoaded, groupDataLoaded) {
            return function(setLevelData, setGroupData) {
                if (setLevelData) {
                    levelDataLoaded = true;
                }

                if (setGroupData) {
                    groupDataLoaded = true;
                }

                if (levelDataLoaded && groupDataLoaded) {
                    if (level === levelSelected && group === groupSelected) {
                        if (lastLevelLoaded !== level) {
                            lastLevelLoaded = level;

                            drawLevel(level);
                        }

                        drawAssignments(res);

                        dom.spinner.classList.remove("app-spinner-active");
                        if (auto) {
                            pagination.autoGoTo();
                        } else {
                            pagination.goToFirst(undefined, false);
                        }
                    }
                }
            }
        })(levelData[level] !== undefined, false);

        loadLevelData(level, x);

        $.ajax({
            async: true,
            type: "POST",
            url: api.getGroup,
            data: {
                level: level,
                group: group
            },

            success: function(result) {
                if (pagination === null) {
                    pagination = new window.Pagination({
                        pages: ".pg",
                        titlePage: "#pgs-t",
                        pagesContainer: "#pgs-c",
                        pagesWrapper: "#pgs-cw",
                        paginationContainer: "#pgs-ft",
                        firstPage: 0,
                        pageNames: [i18n.t("DesksIndex"), i18n.t("DesksExams"), i18n.t("DesksExtra")],
                        disabledPages: [true, true, true],
                        titlePages: [false, false, false],
                        showCallbacks: [undefined, undefined, undefined]
                    });
                }

                res = result;
                x(false, true);
            },

            error: function() {
                window.setInternalErrorMessage($("#ct-c"));
            }
        });
    }

    $(dom.levelsWrapper).on("click", function(e) {
        if (e.target.classList.contains("tchr-tp-bxd")) {
            var level = parseInt(e.target.dataset.id);
            if (levelSelected !== level) {
                var selected = dom.levelsWrapper.getElementsByClassName("tchr-tp-bxd-sel");
                for (var i = 0, l = selected.length; i < l; i++) {
                    selected[i].classList.remove("tchr-tp-bxd-sel");
                }

                e.target.classList.add("tchr-tp-bxd-sel");

                levelSelected = level;
                dom.levels.innerText = e.target.innerText;

                var gSel = dom.groupsWrapper.getElementsByClassName("tchr-tp-bxd-sel");
                for (var i = 0, l = gSel.length; i < l; i++) {
                    gSel[i].classList.remove("tchr-tp-bxd-sel");
                }

                if (groupSelected !== null) {
                    dom.groups.innerText = i18n.t("Groups");
                }

                groupSelected = null;
                dom.groupsWrapper.classList.remove("tchr-tp-dsp-dsb");
                dom.container.classList.add("tchr-ct-hd");

                groupFiltering.level = level;
                filterGroups();

                if (autoOpen > 0) {
                    autoOpen--;

                    if (window.model.group === null) {
                        dom.groupsWrapper.classList.add("tchr-tp-oic");
                        groupsOpened = true;
                    }
                } else {
                    clearScreen();
                    dom.groupsWrapper.classList.add("tchr-tp-oic");
                    groupsOpened = true;

                    window.history.replaceState(null, null, api.mainUri + "/" + levelSelected);
                }
            }

            dom.levelsWrapper.classList.remove("tchr-tp-oic");
            levelsOpened = false;
        } else if (e.target === dom.levels) {
            if (dom.levelsWrapper.classList.contains("tchr-tp-oic")) {
                dom.levelsWrapper.classList.remove("tchr-tp-oic");
                levelsOpened = false;
            } else {
                dom.levelsWrapper.classList.add("tchr-tp-oic");
                levelsOpened = true;

                if (groupsOpened) {
                    dom.groupsWrapper.classList.remove("tchr-tp-oic");
                    groupsOpened = false;
                }
            }
        }

        e.preventDefault();
        e.stopPropagation();
    });

    $(dom.groupsWrapper).on("click", function(e) {
        if (levelSelected !== null) {
            if (e.target.classList.contains("tchr-tp-bxd")) {
                var group = e.target.dataset.id;
                if (groupSelected !== group) {
                    var selected = dom.groupsWrapper.getElementsByClassName("tchr-tp-bxd-sel");
                    for (var i = 0, l = selected.length; i < l; i++) {
                        selected[i].classList.remove("tchr-tp-bxd-sel");
                    }

                    e.target.classList.add("tchr-tp-bxd-sel");

                    groupSelected = group;
                    dom.groups.innerText = e.target.innerText;

                    if (autoOpen > 0) {
                        autoOpen = 0;
                        loadGroup(true);
                    } else {
                        dom.groupsWrapper.classList.remove("tchr-tp-oic");
                        groupsOpened = false;

                        window.history.replaceState(null, null, api.mainUri + "/" + levelSelected + "/" + groupSelected.replace(/-/g, ""));
                        loadGroup(false);
                    }

                }

            } else if (e.target === dom.groups) {
                if (dom.groupsWrapper.classList.contains("tchr-tp-oic")) {
                    dom.groupsWrapper.classList.remove("tchr-tp-oic");
                    groupsOpened = false;
                } else {
                    dom.groupsWrapper.classList.add("tchr-tp-oic");
                    groupsOpened = true;

                    if (levelsOpened) {
                        dom.levelsWrapper.classList.remove("tchr-tp-oic");
                        levelsOpened = false;
                    }
                }
            }
        }

        e.preventDefault();
        e.stopPropagation();
    });

    if (window.model.level !== null) {
        var levels = dom.levelsWrapper.getElementsByClassName("tchr-tp-bxd");
        var l = null, g = null;

        for (var i = 0, l = levels.length; i < l; i++) {
            if (parseInt(levels[i].dataset.id) === window.model.level) {
                autoOpen++;
                l = levels[i];
                break;
            }
        }

        if (window.model.group !== null) {
            var groups = dom.groupsWrapper.getElementsByClassName("tchr-tp-bxd");
            for (var i = 0, l = groups.length; i < l; i++) {
                if (groups[i].dataset.id === window.model.group) {
                    autoOpen++;
                    g = groups[i];
                    break;
                }
            }
        }

        if (l !== null) {
            $(levels[i]).trigger("click");
        }

        if (g !== null) {
            $(groups[i]).trigger("click");
        }
    }

    $(window).on("click", function(e) {
        if (levelsOpened && e.target !== dom.levelsWrapper && e.target.ancestor(".tchr-tp-lvl", true) === undefined) {
            dom.levelsWrapper.classList.remove("tchr-tp-oic");
            levelsOpened = false;

            e.preventDefault();
            e.stopPropagation();
        } else if (groupsOpened && e.target !== dom.groupWrapper && e.target.ancestor(".tchr-tp-grp", true) === undefined) {
            dom.groupsWrapper.classList.remove("tchr-tp-oic");
            groupsOpened = false;

            e.preventDefault();
            e.stopPropagation();
        }
    });

    $(dom.groupDays).on("change", function() {
        var options = dom.groupDays.getElementsByTagName("option");
        for (var i = 0, l = options.length; i < l; i++) {
            if (options[i].selected) {
                var value = options[i].value;
                groupFiltering.day = value === "-1" ? -1 : parseInt(value);
                break;
            }
        }
        filterGroups();
    });

    $(dom.groupTeachers).on("change", function() {
        var options = dom.groupTeachers.getElementsByTagName("option");
        for (var i = 0, l = options.length; i < l; i++) {
            if (options[i].selected) {
                var value = options[i].value;
                groupFiltering.teacher = value === "-1" ? -1 : value;
                break;
            }
        }
        filterGroups();
    });

    $(dom.searchGroup).on("input", function() {
        var search = dom.searchGroup.value.trim();

        if (search.length === "") {
            groupFiltering.search = null;
        } else {
            groupFiltering.search = new RegExp(search, "gi");
        }

        filterGroups();
    });

    $(".tchr-tp-bxw").mCustomScrollbar({
        axis: "y",
        scrollbarPosition: "inside",
        alwaysShowScrollbar: 0,
        scrollButtons: { enable: true },
        theme: "dark-3"
    });
});
