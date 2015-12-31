window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    function WiseTank() {
        var api = Object.create({
            getData: "/WiseTank/GetTimelines",
            addArticle: "/WiseTank/Articles/Create"
        });

        var dom = Object.create({
            title: document.getElementById("wt-ft-nv-it"),
            container: document.getElementById("wt-ft-ct"),
            box: document.getElementById("wt-ft-ct-bx"),
            arrow: document.getElementById("wt-ft-ct-bx-arr"),
            contents: document.getElementById("wt-ft-ct-bx-ctn"),
            spinner: document.getElementById("wt-spn"),
            areas: document.getElementById("wt-aw"),
            timelineTitle: document.getElementById("wt-twtt"),
            timelineBack: document.getElementById("wt-twtb"),
            timelineContents: document.getElementById("wt-tww")
        });

        var scrollbarParameters = Object.create({
            axis: "y",
            scrollbarPosition: "inside",
            alwaysShowScrollbar: 0,
            scrollButtons: { enable: true },
            theme: "dark-3"
        });

        $(dom.timelineContents).mCustomScrollbar(scrollbarParameters);

        var areas = [],
            sortedAreas = null,
            timelines = [];

        var wisetankButton = null,
            wisetankButtonDisabledClass = null,
            wisetankButtonActiveClass = null;

        var loadedData = false;

        function openCloseBox(open, animate) {
            if (typeof animate !== "boolean") {
                animate = true;
            }

            var animated = typeof dom.container.animated === "boolean" && dom.container.animated;

            if (dom.container.classList.contains("wt-ft-ct-vs") && !animated) {
                if (animate) {
                    dom.container.animated = true;
                }

                var state = dom.container.classList.contains("wt-ft-ct-ac");
                var boxStyles = window.getComputedStyle(dom.box);

                var height = (
                    dom.contents.offsetHeight
                        + parseInt(boxStyles.getPropertyValue("padding-top"))
                        + parseInt(boxStyles.getPropertyValue("padding-bottom"))
                ) + "px";

                if (open === true || ((open === undefined || open === null || open === "auto") && !state)) {
                    if (animate) {
                        $(dom.box).transition({ "height": height }, 400)
                            .promise()
                            .always(function() {
                                dom.container.animated = false;
                            });
                    } else {
                        dom.box.style.height = height;
                    }

                    dom.container.classList.add("wt-ft-ct-ac");
                } else {
                    if (animate) {
                        $(dom.box).transition({ "height": "0px" }, 400)
                            .promise()
                            .always(function() {
                                dom.container.animated = false;
                            });
                    } else {
                        dom.box.style.height = "0px";
                    }

                    dom.container.classList.remove("wt-ft-ct-ac");
                }
            }
        }

        function showHideBox(show, animate, open) {
            if (typeof animate !== "boolean") {
                animate = true;
            }

            var state = dom.container.classList.contains("wt-ft-ct-vs");

            if (show === true || ((show === undefined || show === null || show === "auto") && !state)) {
                if (animate) {
                    dom.container.style.opacity = 0;
                }

                dom.container.classList.add("wt-ft-ct-vs");
                openCloseBox(open, animate);

                if (animate) {
                    $(dom.container).animate({ opacity: 1 }, 400);
                }
            } else {
                if (animate) {
                    dom.container.style.opacity = 1;

                    if (open) {
                        openCloseBox(dom.container, false, true);
                    }

                    $(dom.container).animate({ opacity: 0 }, 400)
                        .promise()
                        .always(function() {
                            dom.container.classList.remove("wt-ft-ct-vs");
                        });
                } else {
                    dom.container.classList.remove("wt-ft-ct-vs");
                }
            }
        }

        // Arrow and title click: open or close boxes
        $(dom.arrow).on("click", function() {
            openCloseBox("auto", true);
        });

        $(dom.title).on("click", function() {
            openCloseBox("auto", true);
        });

        function drawAreas() {
            while (dom.areas.firstChild) {
                dom.areas.removeChild(dom.areas.firstChild);
            }

            var numAreas = 0;
            for (var area in areas) {
                if (areas.hasOwnProperty(area)) {
                    var element = document.createElement("span");
                    element.className = "wt-aa";
                    element.innerText = area;
                    element.dataset.id = areas[area];

                    dom.areas.appendChild(element);

                    numAreas++;
                }
            }

            var width = (100 / numAreas) + "%";
            for (var i = 0, l = dom.areas.childNodes.length; i < l; i++) {
                dom.areas.childNodes[i].style.width = width;
            }
        }

        function drawTimelines(area) {
            console.log(area);
        }

        function selectArea(element) {
            if (element.classList.contains("wt-aa-sl")) return false;

            var selected = dom.areas.getElementsByClassName("wt-aa-sl")[0];
            if (selected !== undefined && selected != null) {
                selected.classList.remove("wt-aa-sl");
            }
            element.classList.add("wt-aa-sl");

            drawTimelines(element.dataset.id);

            return true;
        }

        $(dom.areas).on("click", ".wt-aa", function() {
            selectArea(this);
        });


        function loadData(callback) {
            if (wisetankButton !== undefined && wisetankButton !== null) {
                wisetankButton.classList.add(wisetankButtonDisabledClass);
            }

            $.ajax({
                async: true,
                type: "POST",
                url: api.getData,
                data: {
                    offsetDate: window.user.offsetDate
                },

                success: function(result) {
                    areas = result.areas;
                    sortedAreas = Object.keys(areas).sort(function(a, b) { return areas[a] - areas[b] });
                    timelines = result.timelines;
                    callback();

                    if (wisetankButton !== undefined && wisetankButton !== null) {
                        wisetankButton.classList.remove(wisetankButtonDisabledClass);
                    }
                },
                error: function() {
                    createNoty("warning", "center", i18n.t("internal-error"), 1500, ["click"], {
                        callback: {
                            afterClose: function() {
                                if (wisetankButton !== undefined && wisetankButton !== null) {
                                    wisetankButton.classList.remove(wisetankButtonDisabledClass);
                                }
                            }
                        }
                    });
                }
            });
        }

        function addArticle(origin, source, favicon, title, reference, description, image, timeline, hasTitle) {
            if (window.mpd.created()) {
                window.mpd.destroy();
            }

            var mpdContent = window.mpd.create(
                "wisetank-createarticle",
                "500px",
                i18n.t("wisetank.createarticle"),
                { closebtn: true }
            );

            var titleInput;
            if (hasTitle) {
                titleInput = document.createElement("input");
                titleInput.id = "wt-add-tt";
                titleInput.type = "text";
                titleInput.className = "input";
                if (title !== undefined && title !== null && title.length > 128) {
                    title = title.substring(0, 125) + "...";
                }

                titleInput.value = title;
                titleInput.placeholder = i18n.t("Title");
                titleInput.maxLength = 128;
                mpdContent.appendChild(titleInput);
            }

            var hasTimeline = timeline !== undefined && timeline !== null && timeline !== false;
            var areaSelect, timelineSelect;

            if (!hasTimeline) {
                areaSelect = document.createElement("select");
                areaSelect.id = "wt-add-tar";
                areaSelect.className = "input";
                areaSelect.placeholder = i18n.t("wisetank.selectArea");
                var defaultAreaTOption = document.createElement("option");
                defaultAreaTOption.selected = true;
                defaultAreaTOption.disabled = true;
                defaultAreaTOption.value = "";
                defaultAreaTOption.innerText = i18n.t("wisetank.selectArea");
                areaSelect.appendChild(defaultAreaTOption);

                for (var area in sortedAreas) {
                    if (Object.prototype.hasOwnProperty.call(sortedAreas, area)) {
                        var option = document.createElement("option");
                        option.value = areas[sortedAreas[area]];
                        option.innerText = i18n.t("wisetank.areas." + sortedAreas[area].toLowerCase());
                        areaSelect.appendChild(option);
                    }
                }

                mpdContent.appendChild(areaSelect);

                timelineSelect = document.createElement("select");
                timelineSelect.id = "wt-add-ttr";
                timelineSelect.className = "input";
                timelineSelect.placeholder = i18n.t("wisetank.selectTimeline");
                var defaultTimelineTOption = document.createElement("option");
                defaultTimelineTOption.selected = true;
                defaultTimelineTOption.disabled = true;
                defaultTimelineTOption.value = "";
                defaultTimelineTOption.innerText = i18n.t("wisetank.selectTimeline");
                timelineSelect.appendChild(defaultTimelineTOption);
                mpdContent.appendChild(timelineSelect);
            }

            var leadInput = document.createElement("textarea");
            leadInput.id = "wt-add-ld";
            leadInput.className = "input";
            leadInput.placeholder = i18n.t("Lead");
            leadInput.maxLength = 330;
            mpdContent.appendChild(leadInput);

            var buttons = document.createElement("div");
            buttons.id = "wt-add-bt";
            mpdContent.appendChild(buttons);

            var add = document.createElement("button");
            add.id = "wt-add-ct";
            add.className = "input input-bl";
            add.innerText = i18n.t("actions.add");
            add.disabled = true;
            buttons.appendChild(add);

            var close = document.createElement("button");
            close.id = "wt-add-cl";
            close.className = "input input-gr";
            close.innerText = i18n.t("Close");
            buttons.appendChild(close);

            var selectedTimeline = null;
            if (hasTimeline) {
                selectedTimeline = timeline;
            }

            function checkDisabled() {
                var disabled = !(
                    (!hasTitle || (titleInput.value.length > 0
                            && titleInput.value.length < 128
                            && titleInput.value.trim().length > 0))
                        && leadInput.value.length > 0
                        && leadInput.value.length < 330
                        && leadInput.value.trim().length > 0
                        && selectedTimeline !== null
                        && selectedTimeline !== ""
                );

                add.disabled = disabled;
                return disabled;
            }

            $(titleInput).on("input", checkDisabled);
            $(leadInput).on("input", checkDisabled);

            if (!hasTimeline) {
                $(areaSelect).selectize({
                    onChange: function(value) {
                        timelineSelect.selectize.clearOptions();
                        timelineSelect.selectize.clear();

                        if (value === "") {
                            timelineSelect.selectize.disable();
                        } else {
                            var selectedArea = parseInt(value);

                            for (var i = 0, l = timelines.length; i < l; i++) {
                                if (timelines[i].area === selectedArea) {
                                    timelineSelect.selectize.addOption({
                                        value: timelines[i].id,
                                        text: timelines[i].name,
                                        level: 0
                                    });

                                    if (timelines[i].children !== null) {
                                        for (var j = 0, k = timelines[i].children.length; j < k; j++) {
                                            timelineSelect.selectize.addOption({
                                                value: timelines[i].children[j].id,
                                                text: timelines[i].children[j].name,
                                                level: 1
                                            });
                                        }
                                    }
                                }
                            }

                            timelineSelect.selectize.enable();
                        }

                        timelineSelect.selectize.refreshOptions(false);
                    }
                });

                $(timelineSelect).selectize({
                    render: {
                        option: function(data) {
                            return "<div data-value=\"" + data.value + "\" class=\"option option-" + data.level + "\">" + data.text + "</div>";
                        }
                    },
                    onChange: function(value) {
                        selectedTimeline = value;
                        checkDisabled();
                    }
                });

                timelineSelect.selectize.disable();
            }

            $(add).on("click", function() {
                if (checkDisabled()) {

                } else {
                    window.mpd.block();
                    if (hasTitle) {
                        titleInput.disabled = true;
                    }

                    if (!hasTimeline) {
                        areaSelect.selectize.disable();
                        timelineSelect.selectize.disable();
                    }

                    leadInput.disabled = true;
                    add.disabled = true;
                    close.disabled = true;

                    $.ajax({
                        async: true,
                        type: "POST",
                        url: api.addArticle,
                        data: {
                            timeline: selectedTimeline,
                            origin: origin,
                            reference: reference,
                            source: source,
                            favicon: favicon,
                            title: (hasTitle ? titleInput.value.trim() : null),
                            lead: leadInput.value.trim(),
                            description: description,
                            image: image,
                            offsetDate: window.user.offsetDate
                        },
                        success: function(result) {
                            if (result === 0) {
                                window.mpd.unblock();
                                window.mpd.hide();
                                wisetankButton.classList.add(wisetankButtonActiveClass);
                                createNoty("information", "center", i18n.t("wisetank.articlecreated"), 1500, ["click"]);
                            } else {
                                createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                                    callback: {
                                        afterClose: function() {
                                            window.mpd.unblock();
                                            if (hasTitle) {
                                                titleInput.disabled = false;
                                            }

                                            if (!hasTimeline) {
                                                areaSelect.selectize.enable();
                                                timelineSelect.selectize.enable();
                                            }

                                            leadInput.disabled = false;
                                            add.disabled = false;
                                            close.disabled = false;
                                        }
                                    }
                                });
                            }
                        },
                        error: function() {
                            createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                                callback: {
                                    afterClose: function() {
                                        window.mpd.unblock();
                                        if (hasTitle) {
                                            titleInput.disabled = false;
                                        }

                                        if (!hasTimeline) {
                                            areaSelect.selectize.enable();
                                            timelineSelect.selectize.enable();
                                        }

                                        leadInput.disabled = false;
                                        add.disabled = false;
                                        close.disabled = false;
                                    }
                                }
                            });

                        }
                    });
                }
            });

            $(close).on("click", function() {
                $(mpdContent).parents(".mb-box").eq(0).find(".mb-close").eq(0).trigger("click");
            });

            window.mpd.show();
        }

        //this.init = function () {
        //    if (mainLoaded || mainLoading) return false;

        //    mainLoading = true;

        //    $.ajax({
        //        async: true,
        //        type: "POST",
        //        url: api.getData,

        //        success: function (result) {
        //            mainLoaded = true;
        //            mainLoading = false;

        //            areas = result.areas;
        //            timelines = result.timelines;

        //            drawAreas();
        //            selectArea(dom.areas.getElementsByClassName("wt-aa")[0]);
        //            dom.spinner.classList.remove("app-spinner-active");
        //            // ENABLE BUTTONS
        //            showHideBox(true, false, false);
        //            mainLoading = false;
        //        },
        //        error: function () {
        //            //TODO: MENSAJE DE ERROR
        //            mainLoaded = false;
        //            mainLoading = false;
        //        }
        //    });

        //    return true;
        //}

        this.setButton = function(button, disabledClass, activeClass) {
            wisetankButton = button;
            wisetankButtonDisabledClass = disabledClass;
            wisetankButtonActiveClass = activeClass;
        };

        this.addArticle = function(origin, source, favicon, title, reference, description, image, timeline, hasTitle) {
            if (typeof hasTitle !== "boolean") {
                hasTitle = true;
            }

            if (loadedData) {
                addArticle(origin, source, title, reference, description, image, timeline, hasTitle);
            } else {
                loadData(
                    function() {
                        addArticle(origin, source, favicon, title, reference, description, image, timeline, hasTitle);
                    });
            }
        };
    }

    window.wisetank = new WiseTank();
});
