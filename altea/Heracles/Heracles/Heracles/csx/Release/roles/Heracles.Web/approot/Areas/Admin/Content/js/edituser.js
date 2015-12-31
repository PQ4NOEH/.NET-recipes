window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    if (window.admin === undefined) {
        window.admin = {};
    }

    var api = Object.create({
        setUserLevels: "/Admin/Users/SetLevels"
    });

    var dom = Object.create({
        mpdEditUserLevelsId: "adm-edt-usr-lvl",
        mpdContent: null,
        timetableWrapper: null
    });

    var member = null;
    var originalSelectedLevels = null, originalPrimaryLevel = null;
    var selectedLevels = null, primaryLevel = null;

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

    function displayLevel(level, container, userLevel, selected) {
        var div = document.createElement("div");
        div.className = "adm-edt-lvl-lv";
        container.appendChild(div);

        var left = document.createElement("div");
        left.className = "adm-edt-lvl-lf";
        div.appendChild(left);

        var check = document.createElement("input");
        check.id = (userLevel ? "adm-edt-lvl-ulv-" : "adm-edt-lvl-lv-") + level.id;
        check.className = "adm-edt-lvl-lvc";
        check.type = "checkbox";
        check.checked = selected;
        check.dataset.id = level.id;
        left.appendChild(check);

        var label = document.createElement("label");
        label.className = "option";
        label.htmlFor = (userLevel ? "adm-edt-lvl-ulv-" : "adm-edt-lvl-lv-") + level.id;
        left.appendChild(label);

        var mid = document.createElement("div");
        mid.className = "adm-edt-lvl-md" + (userLevel ? " adm-edt-lvl-has-rg" : "");
        //mid.htmlFor = (userLevel ? "adm-edt-lvl-ulv-" : "adm-edt-lvl-lv-") + level.id;
        mid.innerText = level.adminDisplayName;
        div.appendChild(mid);

        if (userLevel) {
            var right = document.createElement("div");
            right.className = "adm-edt-lvl-rg";
            div.appendChild(right);

            var radio = document.createElement("input");
            radio.id = "adm-edt-lvl-ulv-prm-" + level.id;
            radio.className = "adm-edt-lvl-ulv-prm";
            radio.name = "adm-edt-lvl-ulv-prm";
            radio.type = "radio";
            radio.checked = level.id === primaryLevel;
            radio.dataset.id = level.id;
            right.appendChild(radio);

            var radioLabel = document.createElement("label");
            radioLabel.className = "option";
            radioLabel.htmlFor = "adm-edt-lvl-ulv-prm-" + level.id;
            right.appendChild(radioLabel);
        }

        return check;
    }

    function displaySelectedCategory(levels, container) {
        for (var i = 0, l = levels.length; i < l; i++) {
            if (!levels[i].isCategory && levels[i].selectable) {
                displayLevel(levels[i], container, false, selectedLevels.indexOf(levels[i].id) !== -1);
            }

            if (levels[i].children !== null) {
                displaySelectedCategory(levels[i].children, container);
            }
        }
    }

    function displayAllLevels(levels, container) {
        var select = document.createElement("select");
        var $select = $(select);
        select.id = "adm-edt-lvl-slc";
        select.className = "input";
        container.appendChild(select);

        for (var i = 0, l = levels.length; i < l; i++) {
            var option = document.createElement("option");
            option.value = i;
            option.innerText = levels[i].adminDisplayName;
            select.appendChild(option);
        }

        var levelsContainer = document.createElement("div");
        levelsContainer.className = "adm-edt-lvl-lvs";
        container.appendChild(levelsContainer);

        $select.on("change", function() {
            var selectedOption = this.getElementsByTagName("option");
            for (var i = 0, l = selectedOption.length; i < l; i++) {
                if (selectedOption[i].selected) {
                    selectedOption = selectedOption[i];
                    break;
                }
            }

            var opt = parseInt(selectedOption.value);
            while (levelsContainer.firstChild) {
                levelsContainer.removeChild(levelsContainer.firstChild);
            }

            displaySelectedCategory([levels[opt]], levelsContainer);
            window.mpd.resize();
        });

        return $select;
    }

    function createEditUserLevelsPopup(levels) {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        dom.mpdContent = window.mpd.create(
            dom.mpdEditUserLevelsId,
            "800px",
            i18n.t("admin.edituserlevels"),
            { closebtn: true }
        );

        var allLevels = document.createElement("div");
        allLevels.id = "adm-edt-usr-lvl-alv";
        dom.mpdContent.appendChild(allLevels);

        var allLevelsTitle = document.createElement("div");
        allLevelsTitle.id = "adm-edt-usr-lvl-alv-ttl";
        allLevelsTitle.innerText = i18n.t("Levels");
        allLevels.appendChild(allLevelsTitle);

        var $select = displayAllLevels(levels, allLevels);

        var userLevels = document.createElement("div");
        userLevels.id = "adm-edt-usr-lvl-ulv";
        dom.mpdContent.appendChild(userLevels);

        var userLevelsTitle = document.createElement("div");
        userLevelsTitle.id = "adm-edt-usr-lvl-ulv-ttl";
        userLevelsTitle.innerText = i18n.t("admin.userlevels");
        userLevels.appendChild(userLevelsTitle);

        var userLevelsData = document.createElement("div");
        userLevelsData.className = "adm-edt-lvl-lvs";
        userLevels.appendChild(userLevelsData);

        for (var i = 0, l = selectedLevels.length; i < l; i++) {
            var level = findLevel(parseInt(selectedLevels[i]), levels);
            if (level !== null) {
                displayLevel(level, userLevelsData, true, true);
            }
        }

        var save = document.createElement("button");
        save.id = "adm-edt-usr-lvl-cte";
        save.className = "input input-bl";
        save.innerText = i18n.t("Save");
        save.disabled = selectedLevels.length === 0;
        dom.mpdContent.appendChild(save);

        $(allLevels).on("change", ".adm-edt-lvl-lvc", function() {
            if (this.disabled) {
                return;
            }

            var lvl = parseInt(this.dataset.id);
            var l = document.getElementById("adm-edt-lvl-ulv-" + lvl);

            if (this.checked) {
                if (l !== null) l.checked = true;
                else {
                    if (selectedLevels.length === 0) primaryLevel = lvl;
                    displayLevel(findLevel(parseInt(lvl), levels), userLevelsData, true, true);
                    selectedLevels.push(lvl);
                }
            } else {
                l.checked = false;
                selectedLevels.splice(selectedLevels.indexOf(lvl), 1);
                var parent = l.ancestor(".adm-edt-lvl-lv", true);
                parent.parentNode.removeChild(parent);

                if (primaryLevel === lvl) {
                    var other = userLevelsData.getElementsByClassName("adm-edt-lvl-lv");
                    if (other.length !== 0) {
                        other[0].getElementsByClassName("adm-edt-lvl-ulv-prm")[0].checked = true;
                        primaryLevel = parseInt(other[0].getElementsByClassName("adm-edt-lvl-lvc")[0].dataset.id);
                    } else {
                        primaryLevel = null;
                    }
                }
            }

            save.disabled = selectedLevels.length === 0;
            window.mpd.resize();
        });

        $(userLevelsData).on("change", ".adm-edt-lvl-lvc", function() {
            if (this.disabled) {
                return;
            }

            var lvl = parseInt(this.dataset.id);
            var l = document.getElementById("adm-edt-lvl-lv-" + lvl);
            if (this.checked) {
                if (l !== null) l.checked = true;
                if (selectedLevels.indexOf(lvl) === -1) {
                    selectedLevels.push(lvl);
                }
            } else {
                if (l !== null) l.checked = false;
                selectedLevels.splice(selectedLevels.indexOf(lvl), 1);
                var parent = this.ancestor(".adm-edt-lvl-lv", true);
                parent.parentNode.removeChild(parent);

                if (primaryLevel === lvl) {
                    var other = userLevelsData.getElementsByClassName("adm-edt-lvl-lv");
                    if (other.length !== 0) {
                        other[0].getElementsByClassName("adm-edt-lvl-ulv-prm")[0].checked = true;
                        primaryLevel = parseInt(other[0].getElementsByClassName("adm-edt-lvl-lvc")[0].dataset.id);
                    } else {
                        primaryLevel = null;
                    }
                }
            }

            save.disabled = selectedLevels.length === 0;
            window.mpd.resize();
        });

        $(userLevelsData).on("change", ".adm-edt-lvl-ulv-prm", function() {
            if (this.disabled) {
                return;
            }

            primaryLevel = parseInt(this.dataset.id);
        });

        $(save).on("click", function() {
            if (selectedLevels.length === 0 || primaryLevel === null || selectedLevels.indexOf(primaryLevel) === -1) {
                return;
            }

            selectedLevels.sort();
            if (originalPrimaryLevel === primaryLevel && originalSelectedLevels.equals(selectedLevels)) {
                window.mpd.hide();
                return;
            }

            window.mpd.block();

            var inputs = dom.mpdContent.getElementsByTagName("input");
            for (var i = 0, l = inputs.length; i < l; i++) {
                inputs[i].disabled = true;
            }

            save.disabled = true;

            $.ajax({
                async: true,
                type: "POST",
                url: api.setUserLevels,
                data: {
                    userId: member,
                    levels: selectedLevels,
                    primaryLevel: primaryLevel
                },

                success: function(result) {
                    if (result) {
                        window.location.reload();
                    } else {
                        createNoty("warning", "center", i18n.t("error-processing"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    var inputs = dom.mpdContent.getElementsByTagName("input");
                                    for (var i = 0, l = inputs.length; i < l; i++) {
                                        inputs[i].disabled = false;
                                    }
                                    save.disabled = false;
                                    window.mpd.unblock();
                                }
                            }
                        });
                    }
                },

                error: function() {
                    createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                        callback: {
                            afterClose: function() {
                                var inputs = dom.mpdContent.getElementsByTagName("input");
                                for (var i = 0, l = inputs.length; i < l; i++) {
                                    inputs[i].disabled = false;
                                }
                                save.disabled = false;
                                window.mpd.unblock();
                            }
                        }
                    });
                }
            });
        });

        return $select;
    }

    window.admin.editUserLevels = function(user, levels, selected, primary) {
        member = user;

        if (selected.indexOf(0) !== -1) {
            originalSelectedLevels = [];
            selectedLevels = [];
        } else {
            originalSelectedLevels = JSON.parse(JSON.stringify(selected));
            originalSelectedLevels.sort();
            selectedLevels = selected;
        }

        if (primary === 0) {
            originalPrimaryLevel = null;
            primaryLevel = null;
        } else {
            originalPrimaryLevel = primary;
            primaryLevel = primary;
        }
        var $select = createEditUserLevelsPopup(levels);
        $select.trigger("change");

        window.mpd.show();
    }
});
