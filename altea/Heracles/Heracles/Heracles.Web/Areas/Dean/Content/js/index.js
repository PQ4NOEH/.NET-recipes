window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        deanSpinner: $(document.getElementById("dn-spn")),
        headerInfo: document.getElementById("pgs-tr"),

        viewSwitch: document.getElementById("dn-vw-sw"),
        viewModeOff: document.getElementById("dn-vw-off"),
        viewModeOn: document.getElementById("dn-vw-on"),

        remoteSwitch: document.getElementById("dn-rm-sw"),
        remoteModeOff: document.getElementById("dn-rm-off"),
        remoteModeOn: document.getElementById("dn-rm-on"),

        usersTypeSwitch: document.getElementById("dn-prosl"),
        usersAcademicSelector: document.getElementById("dn-ulcs-ac"),
        usersProfessionalSelector: document.getElementById("dn-ulcs-pr"),
        usersActiveSwitch: document.getElementById("dn-uacsl"),
        usersSearcher: document.getElementById("dn-usrc"),
        usersPage: document.getElementById("dn-users").getElementsByClassName("pg-fc")[0],
        usersAcademicPage: document.getElementById("dn-acu"),
        usersProfessionalPage: document.getElementById("dn-pru"),

        groupsSelector: document.getElementById("dn-glcs"),
        groupsActiveSwitch: document.getElementById("dn-gacsl"),
        groupsPrimarySwitch: document.getElementById("dn-gapcsl"),
        groupsSearcher: document.getElementById("dn-gsrc"),

        groupCreateButton: document.getElementById("dn-gnew"),
        groupCallsButton: document.getElementById("dn-gxcl"),

        groupsPage: document.getElementById("dn-groups").getElementsByClassName("pg-fc")[0],

        memberLevel: document.getElementById("dn-mct-lv"),
        memberName: document.getElementById("dn-mct-hn"),
        memberUserName: document.getElementById("dn-mct-hun")
    });

    var MEMBERS_ENUM = {
        User: 0,
        Group: 1
    };

    var TYPES_ENUM = {
        Academy: 0,
        Professional: 1
    };

    var api = Object.create({
        mainUri: "/Dean/{id}/{type}/{pro}/{level}/{sublevel}",
        getMembers: "/Dean/GetMembers",
        getUserData: "/Dean/GetUserData",
        getUserProData: "/Dean/GetUserProData",

        getGroupData: "/Dean/GetGroupData"
    });

    context.init({
        fadeSpeed: 100,
        filter: function($obj) {},
        above: false,
        preventDoubleContext: true,
        compress: false,
        positionY: 1,
        positionX: 25
    });

    var autoOpen = false;

    var pagination;
    var pages = [];

    var levels = { academic: {}, professional: {} };

    var academicUserContainers;
    var academicUsers = { all: {}, data: {}, active: [], inactive: [] };

    var professionalUserContainers;
    var professionalUsers = { all: {}, data: {}, active: [], inactive: [] };

    var groupContainers;
    var groups = { all: {}, data: {}, active: [], inactive: [] };

    //#region Draw
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

    function findAcademicLevel(id) {
        if (levels.academic[id] === undefined) {
            var level = findLevel(id, window.model.levels);
            levels.academic[id] = level;
            return level;
        } else {
            return levels.academic[id];
        }
    }

    function findProLevel(id, subid) {
        if (levels.professional["L" + id + "S" + subid] === undefined) {
            var mainLevel = findLevel(id, window.model.proLevels);
            if (mainLevel !== null && mainLevel.subLevels !== null) {
                for (var i = 0, l = mainLevel.subLevels.length; i < l; i++) {
                    if (mainLevel.subLevels[i].id === subid) {
                        var level = mainLevel.subLevels[i];
                        levels.professional["L" + id + "S" + subid] = level;
                        return level;
                    }
                }
            }
        } else {
            return levels.professional["L" + id + "S" + subid];
        }

        return null;
    }

    function addColumnsPage(page, id) {
        var div = document.createElement("div");
        div.className = "dn-mcp";
        div.dataset.id = id;
        if (id === 0) {
            div.className += " dn-amcp";
        }
        page.appendChild(div);
        return div;
    }

    function addColumn(page) {
        var div = document.createElement("div");
        div.className = "dn-mc";
        page.appendChild(div);
        return div;
    }

    function addRow(column, title, id, subId, priority) {
        var div = document.createElement("div");
        div.className = "dn-mr";
        div.dataset.id = id;
        div.dataset.subId = subId;
        div.style.flexGrow = priority;
        div.style.flexShrink = priority;
        column.appendChild(div);

        var content = document.createElement("div");
        content.className = "dn-mrc";
        div.appendChild(content);

        var titleDiv = document.createElement("div");
        titleDiv.className = "dn-mrct";
        content.appendChild(titleDiv);

        var search = document.createElement("input");
        search.className = "dn-mrcti";
        search.placeholder = i18n.t("Search") + "...";
        titleDiv.appendChild(search);

        var titleContent = document.createElement("span");
        titleContent.className = "dn-mrctc";
        titleContent.innerText = title;
        titleDiv.appendChild(titleContent);

        var members = document.createElement("div");
        members.className = "dn-mrcm";
        content.appendChild(members);

        var noMembersDiv = document.createElement("div");
        noMembersDiv.className = "dn-mrcmc-nm";
        noMembersDiv.innerText = i18n.t("no-members");
        members.appendChild(noMembersDiv);

        var membersContent = document.createElement("div");
        membersContent.className = "dn-mrcmc";
        membersContent.dataset.id = id;
        membersContent.dataset.subId = subId;
        members.appendChild(membersContent);

        return div;
    }

    function fillLevels(columns, levels, noLevel, selector, page) {
        for (var i = 0, l = columns.length; i < l; i++) {
            var totalLevelsColumn = columns[i].levels.length;

            for (var j = 0, k = totalLevelsColumn; j < k; j++) {
                var totalLevels = columns[i].levels[j].length;

                for (var m = 0, n = totalLevels; m < n; m++) {
                    if (levels.indexOf(columns[i].levels[j][m].id) === -1) {
                        columns[i].levels[j][m] = undefined;

                        totalLevels--;
                    }
                }

                columns[i].levels[j] = columns[i].levels[j].filter(function(x) {
                    return x !== undefined;
                });

                if (totalLevels === 0) {
                    columns[i].levels[j] = undefined;
                    totalLevelsColumn--;
                }
            }

            columns[i].levels = columns[i].levels.filter(function(x) {
                return x !== undefined;
            });

            if (totalLevelsColumn === 0) {
                columns[i] = undefined;
            }
        }

        columns = columns.filter(function(x) {
            return x !== undefined;
        });

        var columnPages = [];

        if (columns.length === 0 && !noLevel) {
            var noData = document.createElement("div");
            noData.className = "dn-nda";
            noData.innerText = i18n.t("dean.nodata");
            page.appendChild(noData);
            return;
        } else if (columns.length > 1) {
            selector.classList.add("dn-alcs");

            for (var i = 0, l = columns.length; i < l; i++) {
                var option = document.createElement("option");
                option.value = i;
                option.innerText = columns[i].name;
                selector.appendChild(option);
            }

            $(selector).on("change", function() {
                for (var i = 0, l = columnPages.length; i < l; i++) {
                    columnPages[i].classList.remove("dn-amcp");
                }

                var options = selector.getElementsByTagName("option");
                for (var i = 0, l = options.length; i < l; i++) {
                    if (options[i].selected) {
                        columnPages[options[i].value].classList.add("dn-amcp");
                        break;
                    }
                }
            });
        } else {
            selector.classList.remove("dn-alcs");
        }

        if (columns.length === 0) {
            var columnsPage = addColumnsPage(page, 0);
            var column = addColumn(columnsPage);
            addRow(column, i18n.t("dean.nolevel"), 0, 0, 1);
            columnPages[0] = columnsPage;
        } else {
            for (var i = 0, l = columns.length; i < l; i++) {
                var columnsPage = addColumnsPage(page, i);
                columnPages[i] = columnsPage;
                if (noLevel) {
                    var column = addColumn(columnsPage);
                    addRow(column, i18n.t("dean.nolevel"), 0, 0, 1);
                }

                for (var j = 0, k = columns[i].levels.length; j < k; j++) {
                    var column = addColumn(columnsPage);

                    for (var m = 0, n = columns[i].levels[j].length; m < n; m++) {
                        var lev;

                        if (columns[i].levels[j][m].subId === undefined) {
                            lev = findAcademicLevel(columns[i].levels[j][m].id);
                        } else {
                            lev = findProLevel(columns[i].levels[j][m].id, columns[i].levels[j][m].subId);
                        }

                        addRow(
                            column,
                            lev.adminDisplayName,
                            columns[i].levels[j][m].id,
                            columns[i].levels[j][m].subId,
                            columns[i].levels[j][m].priority);
                    }
                }
            }
        }
    }

    function createProfessionalColumns(levels, subLevels) {
        function findInnerProLevels(ll) {
            function recursiveFindInnerProLevels(group) {
                if (!group.isCategory && group.selectable && group.subLevels !== null) {
                    return [group];
                } else if (group.children !== null) {
                    var children = [];

                    for (var i = 0, l = group.children.length; i < l; i++) {
                        children.pushRange(recursiveFindInnerProLevels(group.children[i]));
                    }

                    return children;
                } else {
                    return [];
                }
            }

            return recursiveFindInnerProLevels(ll);
        }

        var columns = [];

        for (var i = 0, l = window.model.proLevels.length; i < l; i++) {
            var lev = window.model.proLevels[i];

            if (lev.isCategory || !lev.selectable || lev.children !== null) {
                var ls = findInnerProLevels(lev);
                var c = null;
                var lNames = [];

                for (var j = 0, k = ls.length; j < k; j++) {
                    if (levels.indexOf(ls[j].id) !== -1) {
                        if (c === null) {
                            c = {
                                name: lev.adminDisplayName,
                                levels: []
                            }
                        }

                        var subL = [];
                        lNames.push(ls[j].id);
                        for (var m = 0, n = ls[j].subLevels.length; m < n; m++) {
                            if (subLevels[ls[j].id].indexOf(ls[j].subLevels[m].id) !== -1) {
                                subL.push({ id: ls[j].id, subId: ls[j].subLevels[m].id, priority: 1 });
                            }
                        }

                        c.levels.push(subL);
                    }
                }

                if (c !== null) {
                    if (c.levels.length === 1) {
                        var tmpLevels = c.levels[0];

                        c.name = lNames[0];
                        c.levels = [];

                        for (var j = 0, k = tmpLevels.length; j < k; j++) {
                            c.levels.push([tmpLevels[j]]);
                        }
                    }

                    columns.push(c);
                }
            } else {
                if (levels.indexOf(lev.id) !== -1) {
                    var c = {
                        name: lev.adminDisplayName,
                        levels: []
                    };

                    for (var j = 0, k = lev.subLevels.length; j < k; j++) {
                        if (subLevels[lev.id].indexOf(lev.subLevels[j].id) !== -1) {
                            c.levels.push([{ id: lev.id, subId: lev.subLevels[j].id, priority: 1 }]);
                        }
                    }

                    columns.push(c);
                }
            }
        }

        return columns;
    }

    function selectCategoryLevel(id) {
        var category = null;

        window.model.levels.forEach(function(element) {
            if (element.isCategory && element.id === id) {
                category = element;
                return false;
            }

            return true;
        });

        if (category === null) {
            return;
        }
    }

    function createMember(member, className, members, container, level, isPrimary) {
        if (container === undefined || container === null) {
            return null;
        }

        var id = member.id.replace(/-/g, "");

        var div = document.createElement("div");
        div.className = "dn-mbr " + className;
        div.dataset.id = id;
        if (typeof level === "object") {
            if (level.Item1 !== undefined && level.Item2 !== undefined) {
                div.dataset.level = level.Item1;
                div.dataset.subLevel = level.Item2;
            } else {
                div.dataset.level = level.level;
            }
        } else {
            div.dataset.level = level;
        }

        if (member.userName !== undefined) {
            var name = document.createElement("span");
            name.className = "dn-mbrn";
            name.innerText = member.name;
            div.appendChild(name);

            var username = document.createElement("span");
            username.className = "dn-mbrun";
            username.innerText = "@" + member.userName;
            div.appendChild(username);
        } else if (member.groupName !== undefined) {
            var name = document.createElement("span");
            name.className = "dn-mbrn";
            name.innerText = member.groupName;
            div.appendChild(name);

            if (!member.active) {
                div.classList.add("dn-mbr-ginc", "dn-mbr-ahid");
            } else if (!level.active) {
                div.classList.add("dn-mbr-ginc", "dn-mbr-ahid");
            } else {
                var now = new Date().getMilliseconds();
                if ((level.startDate !== null && Date.parseUTC(level.startDate) > now) || (level.endDate !== null && Date.parseUTC(level.endDate) < now)) {
                    div.classList.add("dn-mbr-ginc", "dn-mbr-ahid");
                }
            }
        }

        if (level === 0) {
            container[0].appendChild(div);
            members.active.push(div);

            for (var i = 1, l = container.length; i < l; i++) {
                var clonedDiv = div.cloneNode(true);
                container[i].appendChild(clonedDiv);

                academicUsers.active.push(clonedDiv);
            }
        } else {
            if (isPrimary) {
                members.active.push(div);
            } else {
                members.inactive.push(div);
                div.classList.add("dn-mbr-in", "dn-mbr-hid");
            }

            container.appendChild(div);
        }

        return div;
    }

    function fillMembers(members, containers) {
        for (var i = 0, l = members.length; i < l; i++) {
            var member = members[i];

            var id = member.id.replace(/-/g, "");

            if (member.levels.length === 0) {
                var div = createMember(member, "dn-ac-usr", academicUsers, containers[0], 0, true);
                if (div !== null) {
                    if (academicUsers.all[id] === undefined) {
                        academicUsers.all[id] = {};
                    }

                    academicUsers.all[id][0] = div;
                }

            } else {
                for (var j = 0, k = member.levels.length; j < k; j++) {
                    if (member.userName !== undefined) {
                        var div = createMember(
                            member,
                            "dn-ac-usr",
                            academicUsers,
                            containers[member.levels[j]],
                            member.levels[j],
                            member.levels[j] === member.primaryLevel);

                        if (div !== null) {
                            if (academicUsers.all[id] === undefined) {
                                academicUsers.all[id] = {};
                            }

                            academicUsers.all[id][member.levels[j]] = div;
                        }
                    } else if (member.groupName !== undefined) {
                        var div = createMember(
                            member,
                            "dn-ac-grp",
                            groups,
                            containers[member.levels[j].level],
                            member.levels[j],
                            member.levels[j].level === member.primaryLevel);

                        if (div !== null) {
                            if (groups.all[id] === undefined) {
                                groups.all[id] = {};
                            }

                            groups.all[id][member.levels[j].level] = div;
                        }
                    }
                }
            }
        }
    }

    function fillProMembers(members, containers) {
        for (var i = 0, l = members.length; i < l; i++) {
            var member = members[i];

            if (member.proLevels.length !== 0) {
                for (var j = 0, k = member.proLevels.length; j < k; j++) {
                    var div = createMember(
                        member,
                        "dn-pr-usr",
                        professionalUsers,
                        containers[member.proLevels[j].Item1][member.proLevels[j].Item2],
                        member.proLevels[j],
                        member.proLevels[j].Item1 === member.primaryProLevel.Item1
                        && member.proLevels[j].Item2 === member.primaryProLevel.Item2);

                    if (div !== null) {
                        var id = member.id.replace(/-/g, "");
                        if (professionalUsers.all[id] === undefined) {
                            professionalUsers.all[id] = {};
                        }

                        professionalUsers.all[id]["L" + member.proLevels[j].Item1 + "S" + member.proLevels[j].Item2] = div;
                    }
                }
            }
        }
    }

//#endregion

    function setScrollMembers(containers) {
        for (var container in containers) {
            if (Object.prototype.hasOwnProperty.call(containers, container)) {
                if (Array.isArray(containers[container])) {
                    for (var i = 0, l = containers[container].length; i < l; i++) {
                        $(containers[container][i]).mCustomScrollbar({
                            axis: "y",
                            scrollbarPosition: "inside",
                            alwaysShowScrollbar: 2,
                            scrollButtons: { enable: true },
                            theme: "dark-3"
                        });
                    }
                } else {
                    $(containers[container]).mCustomScrollbar({
                        axis: "y",
                        scrollbarPosition: "inside",
                        alwaysShowScrollbar: 2,
                        scrollButtons: { enable: true },
                        theme: "dark-3"
                    });
                }
            }
        }
    }

    function checkContainers(containers) {
        function checkContainer(container) {
            var members = container.getElementsByClassName("dn-mbr");
            var noMembersElement = container.parentNode.getElementsByClassName("dn-mrcmc-nm")[0];
            if (members.length === 0) {
                noMembersElement.style.display = "block";
            } else {
                var noHiddenMembers = false;

                for (var i = 0, l = members.length; i < l; i++) {
                    if (!members[i].classList.contains("dn-mbr-hid")
                        && !members[i].classList.contains("dn-mbr-ahid")
                        && !members[i].classList.contains("dn-mbr-lsr")
                        && !members[i].classList.contains("dn-mbr-glsr")) {
                        noHiddenMembers = true;
                        break;
                    }
                }

                if (noHiddenMembers) {
                    noMembersElement.style.display = null;
                } else {
                    noMembersElement.style.display = "block";
                }
            }
        }

        for (var c in containers) {
            if (Object.prototype.hasOwnProperty.call(containers, c)) {
                if (Array.isArray(containers[c])) {
                    for (var j = 0, k = containers[c].length; j < k; j++) {
                        if (containers[c][j] !== undefined) {
                            checkContainer(containers[c][j]);
                        }
                    }
                } else {
                    checkContainer(containers[c]);
                }
            }
        }
    }

    function createAcademicUsersPage() {
        // Easy way to clone an object
        var columns = JSON.stringify(window.model.columns);

        var searchedLevels = [], dataWithoutLevel = false;
        for (var i = 0, l = window.model.users.length; i < l; i++) {
            academicUsers.data[window.model.users[i].id.replace(/-/g, "")] = window.model.users[i];

            if (window.model.users[i].primaryLevel === 0) {
                dataWithoutLevel = true;
            } else {
                for (var j = 0, k = window.model.users[i].levels.length; j < k; j++) {
                    if (searchedLevels.indexOf(window.model.users[i].levels[j]) === -1) {
                        searchedLevels.push(window.model.users[i].levels[j]);
                        var level = findAcademicLevel(window.model.users[i].levels[j]);
                        level.hasUsers = true;
                    }
                }
            }
        }

        fillLevels(
            JSON.parse(columns),
            searchedLevels,
            dataWithoutLevel,
            dom.usersAcademicSelector,
            dom.usersAcademicPage);

        var containers = {},
            elements = dom.usersAcademicPage.getElementsByClassName("dn-mrcmc");

        for (var i = 0, l = elements.length; i < l; i++) {
            var id = parseInt(elements[i].dataset.id);

            if (id === 0) {
                if (containers[0] === undefined) {
                    containers[0] = [];
                }

                containers[0].push(elements[i]);
            } else {
                containers[id] = elements[i];
            }
        }

        fillMembers(window.model.users, containers);
        setScrollMembers(containers);

        academicUserContainers = containers;
        checkContainers(academicUserContainers);
    }

    function createProfessionalUsersPage() {
        var searchedLevels = [], searchedSubLevels = {};

        for (var i = 0, l = window.model.users.length; i < l; i++) {
            professionalUsers.data[window.model.users[i].id.replace(/-/g, "")] = window.model.users[i];

            for (var j = 0, k = window.model.users[i].proLevels.length; j < k; j++) {
                var item1 = window.model.users[i].proLevels[j].Item1;
                var item2 = window.model.users[i].proLevels[j].Item2;

                if (searchedLevels.indexOf(item1) === -1) {
                    searchedLevels.push(item1);
                    searchedSubLevels[item1] = [];
                }

                if (searchedSubLevels[item1].indexOf(item2) === -1) {
                    searchedSubLevels[item1].push(item2);
                    var level = findProLevel(item1, item2);
                    level.hasUsers = true;
                }
            }
        }

        var columns = createProfessionalColumns(searchedLevels, searchedSubLevels);

        fillLevels(
            columns,
            searchedLevels,
            false,
            dom.usersProfessionalSelector,
            dom.usersProfessionalPage);

        var containers = {},
            elements = dom.usersProfessionalPage.getElementsByClassName("dn-mrcmc");

        for (var i = 0, l = elements.length; i < l; i++) {
            var id = parseInt(elements[i].dataset.id);

            if (containers[id] === undefined) {
                containers[id] = [];
            }
            containers[id][parseInt(elements[i].dataset.subId)] = elements[i];
        }

        fillProMembers(window.model.users, containers);
        setScrollMembers(containers);

        professionalUserContainers = containers;
        checkContainers(professionalUserContainers);
    }

    function createUsersPage() {
        createAcademicUsersPage();

        if (window.model.hasProDean) {
            createProfessionalUsersPage();
        }
    }

    function createGroupsPage() {
        // Easy way to clone an object
        var columns = JSON.stringify(window.model.columns);

        var searchedLevels = [], dataWithoutLevel = false;
        for (var i = 0, l = window.model.groups.length; i < l; i++) {
            groups.data[window.model.groups[i].id.replace(/-/g, "")] = window.model.groups[i];

            for (var j = 0, k = window.model.groups[i].levels.length; j < k; j++) {
                if (searchedLevels.indexOf(window.model.groups[i].levels[j].level) === -1) {
                    searchedLevels.push(window.model.groups[i].levels[j].level);
                    var level = findAcademicLevel(window.model.groups[i].levels[j].level);
                    level.hasGroups = true;
                }
            }
        }

        fillLevels(
            JSON.parse(columns),
            searchedLevels,
            false,
            dom.groupsSelector,
            dom.groupsPage);

        var containers = {},
            elements = dom.groupsPage.getElementsByClassName("dn-mrcmc");

        for (var i = 0, l = elements.length; i < l; i++) {
            var id = parseInt(elements[i].dataset.id);

            if (id === 0) {
                if (containers[0] === undefined) {
                    containers[0] = [];
                }

                containers[0].push(elements[i]);
            } else {
                containers[id] = elements[i];
            }
        }

        fillMembers(window.model.groups, containers);
        setScrollMembers(containers);

        groupContainers = containers;
        checkContainers(groupContainers);
    }

    function createPagination() {
        var pageNames = [],
            disabledPages = [],
            titlePages = [],
            showCallbacks = [],
            afterShowCallbacks = [];

        if (window.model.hasUserDean) {
            pages.push("users");
            pageNames.push(i18n.t("Users"));
            disabledPages.push(false);
            titlePages.push(false);
            showCallbacks.push(function() {
                dom.headerInfo.classList.remove("pgs-tr-act");
            });
            afterShowCallbacks.push(undefined);
        }

        if (window.model.hasGroupDean) {
            pages.push("groups");
            pageNames.push(i18n.t("Groups"));
            disabledPages.push(false);
            titlePages.push(false);
            showCallbacks.push(function() {
                dom.headerInfo.classList.remove("pgs-tr-act");
            });
            afterShowCallbacks.push(undefined);

            pages.push("groupplanner");
            pageNames.push(i18n.t("GroupPlanner"));
            disabledPages.push(true);
            titlePages.push(false);
            showCallbacks.push(function() {
                dom.headerInfo.classList.remove("pgs-tr-act");
            });
            afterShowCallbacks.push(undefined);
        }

        pages.push("desksindex");
        pageNames.push(i18n.t("DesksIndex"));
        disabledPages.push(true);
        titlePages.push(true);
        showCallbacks.push(window.dean.loadDesksIndexData);
        afterShowCallbacks.push(function() {
            window.dean.showDesksIndexAssignments();
            dom.headerInfo.classList.add("pgs-tr-act");
        });

        pages.push("desksexams");
        pageNames.push(i18n.t("DesksExams"));
        disabledPages.push(true);
        titlePages.push(true);
        showCallbacks.push(window.dean.loadDesksExamsData);
        afterShowCallbacks.push(function() {
            window.dean.showDesksExamsAssignments();
            dom.headerInfo.classList.add("pgs-tr-act");
        });

        if (window.model.hasGroupDean) {
            pages.push("desksextra");
            pageNames.push(i18n.t("DesksExtra"));
            disabledPages.push(true);
            titlePages.push(true);
            showCallbacks.push(window.dean.loadDesksExtraData);
            afterShowCallbacks.push(function() {
                window.dean.showDesksExtraAssignments();
                dom.headerInfo.classList.add("pgs-tr-act");
            });
        }

        if (window.model.hasUserDean) {
            pages.push("desksbooks");
            pageNames.push(i18n.t("DesksBooks"));
            disabledPages.push(true);
            titlePages.push(true);
            showCallbacks.push(window.dean.loadDesksBooksData);
            afterShowCallbacks.push(function() {
                window.dean.showDesksBooksAssignments();
                dom.headerInfo.classList.add("pgs-tr-act");
            });

            if (window.model.hasProDean) {
                pages.push("prodesks");
                pageNames.push(i18n.t("ProDesks"));
                disabledPages.push(true);
                titlePages.push(true);
                showCallbacks.push(window.dean.loadProDesksData);
                afterShowCallbacks.push(function() {
                    window.dean.showProDesksAssignments();
                    dom.headerInfo.classList.add("pgs-tr-act");
                });
            }

            if (window.model.hasWiseNet) {
                pages.push("wisenet");
                pageNames.push(i18n.t("WiseNet"));
                disabledPages.push(true);
                titlePages.push(true);
                showCallbacks.push(function() {
                    dom.headerInfo.classList.remove("pgs-tr-act");
                    window.dean.loadWiseNetData();
                });
                afterShowCallbacks.push(undefined);
            }

            if (window.model.hasLists) {
                pages.push("lists");
                pageNames.push(i18n.t("Lists"));
                disabledPages.push(true);
                titlePages.push(true);
                showCallbacks.push(function() {
                    dom.headerInfo.classList.remove("pgs-tr-act");
                    window.dean.loadListsData();
                });
                afterShowCallbacks.push(undefined);
            }
        }

        // ReSharper disable once ConstructorCallNotUsed
        pagination = new window.Pagination({
            pages: ".pg",
            titlePage: "#pgs-t",
            pagesContainer: "#pgs-c",
            pagesWrapper: "#pgs-cw",
            paginationContainer: "#pgs-ft",
            firstPage: 0,
            pageNames: pageNames,
            disabledPages: disabledPages,
            titlePages: disabledPages,
            showCallbacks: showCallbacks,
            afterShowCallbacks: afterShowCallbacks
        });
    }

    if (window.model.hasUserDean || window.model.hasGroupDean) {
        $.ajax({
            async: true,
            type: "POST",
            url: api.getMembers,

            success: function(result) {
                if (window.model.hasUserDean) {
                    window.model.users = result.users;
                    createUsersPage();
                }

                if (window.model.hasGroupDean) {
                    window.model.groups = result.groups;
                    createGroupsPage();
                }

                createPagination();

                selectCategoryLevel(
                    window.model.levels.filter(function(value) {
                        return value.isCategory;
                    })[0]);

                if (window.model.sticky.id === null) {
                    dom.deanSpinner.removeClass("app-spinner-active");
                    pagination.autoGoTo();
                } else {
                    var div = null;

                    if (window.model.sticky.type === 1) { // Load user
                        if (window.model.sticky.pro) {
                            div = groupUsers.all[window.model.sticky.id.replace(/-/g, "")];
                            if (div !== null) {
                                div = div["L" + window.model.sticky.level + "S" + window.model.sticky.sublevel];
                            }
                        } else {
                            div = academicUsers.all[window.model.sticky.id.replace(/-/g, "")];
                            if (div !== null) {
                                div = div[window.model.sticky.level];
                            }
                        }
                    } else if (window.model.sticky.type === 2) { // Load group
                        div = groups.all[window.model.sticky.id.replace(/-/g, "")];
                        if (div !== null) {
                            div = div[window.model.sticky.level];
                        }
                    }

                    if (div !== null) {
                        autoOpen = true;
                        $(div).trigger("click");
                    }
                }

                //#region Contexts
                var academicUserContext = [];
                var academicGroupContext = [];
                if (window.model.hasAdmin) {
                    academicUserContext.push(
                    {
                        text: i18n.t("admin.edituserlevels"),
                        action: function(event, element) {
                            window.admin.editUserLevels(
                                element.dataset.id,
                                window.model.levels,
                                Object
                                .keys(academicUsers.all[element.dataset.id])
                                .map(function(a) {
                                    return parseInt(a);
                                }),
                                academicUsers.data[element.dataset.id].primaryLevel);
                        }
                    });
                }

                if (window.model.canCreateGroups) {
                    academicGroupContext.push(
                    {
                        text: i18n.t("admin.editgrouplevels"),
                        action: function(event, element) {
                            window.admin.editGroupLevels(
                                element.dataset.id,
                                window.model.levels,
                                Object
                                .keys(groups.all[element.dataset.id])
                                .map(function(a) {
                                    return parseInt(a);
                                }),
                                groups.data[element.dataset.id].primaryLevel);
                        }
                    });
                }

                if (academicUserContext.length !== 0) {
                    context.attach(".dn-ac-usr", academicUserContext);
                }

                if (academicGroupContext.length !== 0) {
                    context.attach(".dn-ac-grp", academicGroupContext);
                }
                //#endregion
            },

            error: function() {
                window.setInternalErrorMessage($("#ct-c"));
            }
        });
    }

    function setSearchMember(members, search, searchClass) {
        if (search === "") {
            for (var i = 0, l = members.length; i < l; i++) {
                members[i].classList.remove(searchClass);
            }
        } else {
            var regex = new RegExp(search, "i");

            for (var i = 0, l = members.length; i < l; i++) {
                var name = members[i].getElementsByClassName("dn-mbrn")[0];
                if (name.innerText.match(regex) !== null) {
                    members[i].classList.remove(searchClass);
                } else {
                    var id = members[i].getElementsByClassName("dn-mbrun")[0];
                    if (id !== undefined) {
                        if (id.innerText.match(regex) !== null) {
                            members[i].classList.remove(searchClass);
                        } else {
                            members[i].classList.add(searchClass);
                        }
                    } else {
                        members[i].classList.add(searchClass);
                    }
                }
            }
        }
    }

    if (window.model.hasUserDean) {
        function getAcademicUser(id, level) {
            pagination.pause();

            if (!autoOpen) {
                dom.deanSpinner.addClass("app-spinner-semiactive");

                var uri = api.mainUri;
                uri = uri.replace("{id}", id);
                uri = uri.replace("{type}", 1);
                uri = uri.replace("{pro}", 0);
                uri = uri.replace("{level}", level);
                uri = uri.replace("{sublevel}", 0);
                window.history.replaceState(null, null, uri);
            }

            $.ajax({
                async: true,
                type: "POST",
                url: api.getUserData,
                data: {
                    id: id,
                    level: level
                },

                success: function(result) {
                    window.dean.uid = id;
                    window.dean.member = MEMBERS_ENUM.User;
                    window.dean.type = TYPES_ENUM.Academy;
                    window.dean.level = level;
                    window.dean.sublevel = null;

                    var l = findAcademicLevel(level);
                    if (l !== null) {
                        dom.memberLevel.style.display = null;
                        dom.memberLevel.innerText = l.adminDisplayName;
                    } else {
                        dom.memberLevel.style.display = "none";
                    }

                    dom.memberName.innerText = professionalUsers.data[id].name;
                    dom.memberUserName.innerText = "@" + professionalUsers.data[id].userName;

                    pagination.disablePage(pages.indexOf("groupplanner"));
                    pagination.disablePage(pages.indexOf("prodesks"));

                    if (window.dean.level !== 0) {
                        pagination.enablePage(pages.indexOf("desksindex"));
                        window.dean.loadMemberDesksIndex(result.desksIndexAssignments);
                        pagination.enablePage(pages.indexOf("desksexams"));
                        window.dean.loadMemberDesksExams(result.desksExamsAssignments);
                    } else {
                        pagination.disablePage(pages.indexOf("desksindex"));
                        pagination.disablePage(pages.indexOf("desksexams"));
                    }

                    pagination.enablePage(pages.indexOf("desksbooks"));
                    window.dean.loadMemberDesksBooks(result.desksExamsAssignments);

                    if (window.model.hasWiseNet
                        && (result.searchEngines !== undefined || result.magazines !== undefined)) {
                        pagination.enablePage(pages.indexOf("wisenet"));
                        window.dean.loadMemberNewsStand(result.wisenet);
                    } else {
                        pagination.disablePage(pages.indexOf("wisenet"));
                    }

                    if (window.model.hasLists && result.lists !== undefined) {
                        pagination.enablePage(pages.indexOf("lists"));
                        window.dean.loadMemberLists(result.lists);
                    } else {
                        pagination.disablePage(pages.indexOf("lists"));
                    }

                    pagination.resume();

                    if (autoOpen) {
                        autoOpen = false;
                        window.model.sticky = null;
                        dom.deanSpinner.removeClass("app-spinner-active");
                        pagination.autoGoTo();
                    } else {
                        dom.deanSpinner.removeClass("app-spinner-semiactive");
                        pagination.goToFirst([pages.indexOf("users"), pages.indexOf("groups")]);
                    }
                },

                error: function() {
                    window.setInternalErrorMessage($("#ct-c"));
                }
            });
        }

        function getProfessionalUser(id, level, sublevel) {
            pagination.pause();

            if (!autoOpen) {
                dom.deanSpinner.addClass("app-spinner-semiactive");

                var uri = api.mainUri;
                uri = uri.replace("{id}", id);
                uri = uri.replace("{type}", 1);
                uri = uri.replace("{pro}", 1);
                uri = uri.replace("{level}", level);
                uri = uri.replace("{sublevel}", sublevel);
                window.history.replaceState(null, null, uri);
            }

            $.ajax({
                async: true,
                type: "POST",
                url: api.getUserProData,
                data: {
                    id: id,
                    level: level,
                    sublevel: sublevel
                },

                success: function(result) {
                    window.dean.uid = id;
                    window.dean.member = MEMBERS_ENUM.User;
                    window.dean.type = TYPES_ENUM.Professional;
                    window.dean.level = level;
                    window.dean.sublevel = sublevel;

                    dom.memberLevel.innerText = findProLevel(level).adminDisplayName;
                    dom.memberLevel.style.display = null;
                    dom.memberName.innerText = professionalUsers.data[id].name;
                    dom.memberUserName.innerText = "@" + professionalUsers.data[id].userName;

                    pagination.disablePage(pages.indexOf("groupplanner"));
                    pagination.disablePage(pages.indexOf("desksindex"));
                    pagination.disablePage(pages.indexOf("desksexams"));
                    pagination.disablePage(pages.indexOf("desksbooks"));

                    pagination.enablePage(pages.indexOf("prodesks"));

                    if (window.model.hasWiseNet
                        && (result.searchEngines !== undefined || result.magazines !== undefined)) {
                        pagination.enablePage(pages.indexOf("wisenet"));
                        window.dean.loadMemberNewsStand(result.wisenet);
                    } else {
                        pagination.disablePage(pages.indexOf("wisenet"));
                    }

                    if (window.model.hasLists && result.lists !== undefined) {
                        pagination.enablePage(pages.indexOf("lists"));
                        window.dean.loadMemberLists(result.lists);
                    } else {
                        pagination.disablePage(pages.indexOf("lists"));
                    }

                    pagination.resume();

                    if (autoOpen) {
                        autoOpen = false;
                        window.model.sticky = null;
                        dom.deanSpinner.removeClass("app-spinner-active");
                        pagination.autoGoTo();
                    } else {
                        dom.deanSpinner.removeClass("app-spinner-semiactive");
                        pagination.goToFirst([pages.indexOf("users"), pages.indexOf("groups")]);
                    }
                },

                error: function() {
                    window.setInternalErrorMessage($("#ct-c"));
                }
            });
        }

        $(dom.usersPage).on("click", ".dn-mbr", function() {
            var id = this.dataset.id;
            var level = parseInt(this.dataset.level);
            var sublevel = this.dataset.subLevel;

            if (sublevel === undefined) {
                getAcademicUser(id, level);
            } else {
                sublevel = parseInt(sublevel);
                getProfessionalUser(id, level, sublevel);
            }
        });

        if (window.model.hasProDean) {
            $(dom.usersTypeSwitch).on("click", ".dn-lcsv", function() {
                if (this.classList.contains("input-bl")) {
                    return;
                }

                var active = dom.usersTypeSwitch.getElementsByClassName("input-bl")[0];
                active.classList.remove("input-bl", "input-nh");
                active.classList.add("input-gr");
                this.classList.remove("input-gr");
                this.classList.add("input-bl", "input-nh");

                if (this.id === "dn-prosl-ac") {
                    dom.usersAcademicPage.classList.add("dn-mbrp-a");
                    dom.usersProfessionalPage.classList.remove("dn-mbrp-a");
                    dom.usersAcademicSelector.style.display = null;
                    dom.usersProfessionalSelector.style.display = "none";
                } else if (this.id === "dn-prosl-pr") {
                    dom.usersProfessionalPage.classList.add("dn-mbrp-a");
                    dom.usersAcademicPage.classList.remove("dn-mbrp-a");
                    dom.usersProfessionalSelector.style.display = null;
                    dom.usersAcademicSelector.style.display = "none";
                }
            });
        }

        $(dom.usersActiveSwitch).on("click", ".dn-lcsv", function() {
            if (this.classList.contains("input-bl")) {
                return;
            }

            var active = dom.usersActiveSwitch.getElementsByClassName("input-bl")[0];
            active.classList.remove("input-bl", "input-nh");
            active.classList.add("input-gr");
            this.classList.remove("input-gr");
            this.classList.add("input-bl", "input-nh");

            if (this.id === "dn-uacsl-ac") {
                for (var i = 0, l = academicUsers.inactive.length; i < l; i++) {
                    academicUsers.inactive[i].classList.add("dn-mbr-hid");
                }

                for (var i = 0, l = professionalUsers.inactive.length; i < l; i++) {
                    professionalUsers.inactive[i].classList.add("dn-mbr-hid");
                }
            } else if (this.id === "dn-uacsl-al") {
                for (var i = 0, l = academicUsers.inactive.length; i < l; i++) {
                    academicUsers.inactive[i].classList.remove("dn-mbr-hid");
                }

                for (var i = 0, l = professionalUsers.inactive.length; i < l; i++) {
                    professionalUsers.inactive[i].classList.remove("dn-mbr-hid");
                }
            }

            checkContainers(academicUserContainers);
            checkContainers(professionalUserContainers);
        });

        var usersSearcherTimeout = null;
        $(dom.usersSearcher).on("input", function () {
            clearTimeout(usersSearcherTimeout);

            (function(that) {
                usersSearcherTimeout = setTimeout(function() {
                    var users = dom.usersPage.getElementsByClassName("dn-mbr");
                    var value = that.value.trim().toLowerCase();
                    setSearchMember(users, value, "dn-mbr-glsr");
                    checkContainers(dom.usersPage.getElementsByClassName("dn-mrcm"));
                }, 100);
            })(this);
        });

        var usersPageTimeout = null;
        $(dom.usersPage).on("input", ".dn-mrcti", function () {
            clearTimeout(usersPageTimeout);

            (function(that) {
                usersPageTimeout = setTimeout(function() {
                    var ancestor = that.ancestor(".dn-mrc", true);
                    var users = ancestor.getElementsByClassName("dn-mbr");
                    var value = that.value.trim().toLowerCase();
                    setSearchMember(users, value, "dn-mbr-lsr");
                    checkContainers([ancestor]);
                }, 100);
            })(this);
        });
    }

    if (window.model.hasGroupDean) {
        function getGroup(id, level) {
            pagination.pause();

            if (!autoOpen) {
                dom.deanSpinner.addClass("app-spinner-semiactive");

                var uri = api.mainUri;
                uri = uri.replace("{id}", id);
                uri = uri.replace("{type}", 2);
                uri = uri.replace("{pro}", 0);
                uri = uri.replace("{level}", level);
                uri = uri.replace("{sublevel}", 0);
                window.history.replaceState(null, null, uri);
            }

            $.ajax({
                async: true,
                type: "POST",
                url: api.getGroupData,
                data: {
                    id: id,
                    level: level
                },

                success: function(result) {
                    window.dean.uid = id;
                    window.dean.member = MEMBERS_ENUM.Group;
                    window.dean.type = TYPES_ENUM.Academy;
                    window.dean.level = level;
                    window.dean.sublevel = null;

                    dom.memberLevel.innerText = findAcademicLevel(level).adminDisplayName;
                    dom.memberLevel.style.display = null;
                    dom.memberName.innerText = groups.data[id].groupName;
                    dom.memberUserName.innerText = null;

                    pagination.disablePage(pages.indexOf("prodesks"));
                    pagination.disablePage(pages.indexOf("wisenet"));
                    pagination.disablePage(pages.indexOf("lists"));

                    pagination.enablePage(pages.indexOf("groupplanner"));

                    pagination.enablePage(pages.indexOf("desksindex"));
                    window.dean.loadMemberDesksIndex(result.desksIndexAssignments);
                    pagination.enablePage(pages.indexOf("desksexams"));
                    window.dean.loadMemberDesksExams(result.desksExamsAssignments, true);
                    pagination.enablePage(pages.indexOf("desksextra"));
                    window.dean.loadMemberDesksExtra(result.desksExtraAssignments);

                    pagination.resume();

                    if (autoOpen) {
                        autoOpen = false;
                        window.model.sticky = null;
                        dom.deanSpinner.removeClass("app-spinner-active");
                        pagination.autoGoTo();
                    } else {
                        dom.deanSpinner.removeClass("app-spinner-semiactive");

                        var excluded = [pages.indexOf("users"), pages.indexOf("groups")];
                        if (result.planning.finished) {
                            excluded.push(pages.indexOf("groupplanner"));
                        }
                        pagination.goToFirst(excluded);
                    }
                },

                error: function() {
                    window.setInternalErrorMessage($("#ct-c"));
                }
            });
        }

        $(dom.groupsPage).on("click", ".dn-mbr", function() {
            var id = this.dataset.id;
            var level = parseInt(this.dataset.level);

            getGroup(id, level);
        });

        $(dom.groupsActiveSwitch).on("click", ".dn-lcsv", function() {
            if (this.classList.contains("input-bl")) {
                return;
            }

            var active = dom.groupsActiveSwitch.getElementsByClassName("input-bl")[0];
            active.classList.remove("input-bl", "input-nh");
            active.classList.add("input-gr");
            this.classList.remove("input-gr");
            this.classList.add("input-bl", "input-nh");

            if (this.id === "dn-gacsl-ac") {
                for (var group in groups.data) {
                    if (!groups.data.active) {
                        for (var level in groups.all[group]) {
                            groups.all[group][level].classList.add("dn-mbr-ahid");
                        }
                    }
                }
            } else if (this.id === "dn-gacsl-al") {
                for (var group in groups.data) {
                    if (!groups.data.active) {
                        for (var level in groups.all[group]) {
                            groups.all[group][level].classList.remove("dn-mbr-ahid");
                        }
                    }
                }
            }

            checkContainers(groupContainers);
        });

        $(dom.groupsPrimarySwitch).on("click", ".dn-lcsv", function() {
            if (this.classList.contains("input-bl")) {
                return;
            }

            var active = dom.groupsPrimarySwitch.getElementsByClassName("input-bl")[0];
            active.classList.remove("input-bl", "input-nh");
            active.classList.add("input-gr");
            this.classList.remove("input-gr");
            this.classList.add("input-bl", "input-nh");

            if (this.id === "dn-gapcsl-ac") {
                for (var i = 0, l = groups.inactive.length; i < l; i++) {
                    groups.inactive[i].classList.add("dn-mbr-hid");
                }
            } else if (this.id === "dn-gapcsl-al") {
                for (var i = 0, l = groups.inactive.length; i < l; i++) {
                    groups.inactive[i].classList.remove("dn-mbr-hid");
                }
            }

            checkContainers(groupContainers);
        });

        var groupsSearcherTimeout = null;
        $(dom.groupsSearcher).on("input", function () {
            clearTimeout(groupsSearcherTimeout);

            (function(that) {
                groupsSearcherTimeout = setTimeout(function() {
                    var groups = dom.groupsPage.getElementsByClassName("dn-mbr");
                    var value = that.value.trim().toLowerCase();
                    setSearchMember(groups, value, "dn-mbr-glsr");
                    checkContainers(dom.groupsPage.getElementsByClassName("dn-mrcm"));
                }, 100);
            })(this);
        });

        var groupsPageTimeout = null;
        $(dom.groupsPage).on("input", ".dn-mrcti", function () {
            clearTimeout(groupsPageTimeout);

            (function(that) {
                groupsPageTimeout = setTimeout(function() {
                    var ancestor = that.ancestor(".dn-mrc", true);
                    var groups = ancestor.getElementsByClassName("dn-mbr");
                    var value = that.value.trim().toLowerCase();
                    setSearchMember(groups, value, "dn-mbr-lsr");
                    checkContainers([ancestor]);
                }, 100);
            })(this);
        });

        $(dom.groupCreateButton).on("click", function() {
            window.admin.addGroup();
        });

        function createExamCallsPopup() {
            if (window.mpd.created()) {
                window.mpd.destroy();
            }

            dom.mpdContent = window.mpd.create(
                "dn-edt-exc",
                "400px",
                i18n.t("dean.editexamcalls"),
                { closebtn: true }
            );
        }

        $(dom.groupCallsButton).on("click", function() {
            createExamCallsPopup();
            window.mpd.show();
        });
    }

    $(dom.viewSwitch).on("click", ".input", function() {
        if (this.classList.contains("input-bl")) {
            return;
        }

        /*var active = dom.viewSwitch.getElementsByClassName("input-bl")[0];
        active.classList.remove("input-bl", "input-nh");
        active.classList.add("input-gr");
        this.classList.remove("input-gr");
        this.classList.add("input-bl", "input-nh");

        if (this === dom.viewModeOff) {
            window.dean.viewMode = false;
        } else if (this === dom.viewModeOn) {
            window.dean.viewModel = true;
        }*/
    });

    $(dom.remoteSwitch).on("click", ".input", function() {
        if (this.classList.contains("input-bl")) {
            return;
        }

        var active = dom.remoteSwitch.getElementsByClassName("input-bl")[0];
        active.classList.remove("input-bl", "input-nh");
        active.classList.add("input-gr");
        this.classList.remove("input-gr");
        this.classList.add("input-bl", "input-nh");

        if (this === dom.remoteModeOff) {
            window.dean.remoteMode = false;
        } else if (this === dom.remoteModeOn) {
            window.dean.remoteMode = true;
        }
    });

    window.dean = {
        member: null,
        type: null,
        level: null,
        sublevel: null,
        viewMode: false,
        remoteMode: false
    };
});
