window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    // ReSharper disable once InconsistentNaming
    function WiseLab() {
        var api = Object.create({
            getFinishedWords: "/WiseLab/GetFinishedWords",
            getArticle: "/WiseLab/GetArticle",
            getOriginParameters: "/WiseLab/GetOriginParameters",
            searchWord: "/WiseLab/SearchWord",
            addHuntData: "/WiseLab/AddHuntData",
            removeHuntData: "/WiseLab/RemoveHuntData",
            saveLead: "/WiseLab/SaveLead",
            finishStatus: "/WiseLab/FinishStatus",
            logException: "/WiseLab/LogException"
        });

        var status = Object.create({
            none: 0x00,
            scout: 0x01,
            key: 0x02,
            expression: 0x04,
            wisdom: 0x08,
            finished: 0xff
        });

        var errors = Object.create({
            none: 0x00,
            unknown: 0xf0,
            dataExists: 0xf1,
            dataNotExists: 0xf2,
            inboxOverflow: 0xf3,
            dataInboxWIP: 0xf4,
            cantFinish: 0xf5,
            invalidStatus: 0xf6
        });

        var colors = Object.create({
            finishedWords: window.theme.colors.lightyellow,
            scoutedWords: window.theme.colors.yellow,
            keyWords: window.theme.colors.green,
            expressions: window.theme.colors.orange
        });

        var dom = Object.create({
            footer: document.getElementById("wl-ft"),
            containers: document.getElementsByClassName("wl-ft-ct"),
            arrows: document.getElementsByClassName("wl-ft-ct-bx-arr"),
            titles: document.getElementsByClassName("wl-ft-nv-it"),

            scoutpad: Object.create({
                container: document.getElementById("wl-ft-ct-sp"),
                maximize: document.getElementById("wl-sp-mxz"),
                ulWrapper: document.getElementById("wl-sp-liw"),
                $ulWrapper: $(document.getElementById("wl-sp-liw")),
                ul: document.getElementById("wl-sp-li"),
                search: document.getElementById("wl-sp-sr"),
                searchInput: document.getElementById("wl-sp-sr-st"),
                searchSubmit: document.getElementById("wl-sp-sr-sb"),
                doneWrapper: document.getElementById("wr-sp-dn"),
                done: document.getElementById("wr-sp-dn-bt")
            }),

            wisdom: Object.create({
                container: document.getElementById("wl-ft-ct-wh"),
                doneWrapper: document.getElementById("wr-wh-dn"),
                done: document.getElementById("wr-wh-dn-bt"),

                actual: document.getElementById("wl-wh-whc-a"),
                max: document.getElementById("wl-wh-whc-m"),

                keyhunter: Object.create({
                    ulWrapper: document.getElementById("wl-wh-khw"),
                    $ulWrapper: $(document.getElementById("wl-wh-khw")),
                    ul: document.getElementById("wl-wh-kh")
                }),

                expression: Object.create({
                    ulWrapper: document.getElementById("wl-wh-ehw"),
                    $ulWrapper: $(document.getElementById("wl-wh-ehw")),
                    ul: document.getElementById("wl-wh-eh")
                }),

                wisdomhunter: Object.create({
                    textWrapper: document.getElementById("wl-wh-whw"),
                    $textWrapper: $(document.getElementById("wl-wh-whw")),
                    text: $(document.getElementById("wl-wh-wh"))
                })
            })
        });

        if (dom.footer === null) {
            return;
        }

        var scrollbarVisibleParameters = Object.create({
                axis: "y",
                scrollbarPosition: "inside",
                alwaysShowScrollbar: 2,
                scrollButtons: { enable: true },
                theme: "dark-3"
            }),
            scrollbarParameters = Object.create({
                axis: "y",
                scrollbarPosition: "inside",
                alwaysShowScrollbar: 0,
                scrollButtons: { enable: true },
                theme: "dark-3"
            });

        $(dom.scoutpad.ulWrapper).mCustomScrollbar(scrollbarVisibleParameters);
        $(dom.wisdom.expression.ulWrapper).mCustomScrollbar(scrollbarParameters);


// ReSharper disable InconsistentNaming
        var MIN_WORD_LENGTH = 2,
            REGEXP_MATCH = new XRegExp("[^\\p{L}' -]", "i"),
            MIN_KEYWORDS = 20,
            MAX_KEYWORDS = 200,
            MIN_LEAD = 33,
            MAX_LEAD = 330;
        // ReSharper restore InconsistentNaming

        // ReSharper disable InconsistentNaming
        var WISELAB_SEARCHED_TYPE = 0;
        // ReSharper restore InconsistentNaming

        var ready = false,
            mainLoaded = false,
            mainLoading = false,
            ajaxBusy = false,
            loaded = false,
            started = false,
            paused = false;

        var finishedWords = null;

        var origin = 0,
            reference = 0,
            parameters = undefined,
            article = undefined,
            articlehunt = Object.create({
                actualScoutedWords: 0,
                scouted: null,
                keywords: null,
                actualKeywords: 0,
                maxKeywords: 0,
                expressions: null,
                actualExpressions: 0,
                maxExpressions: 0,
                lead: "",
                actualLead: 0,
                maxLead: 0
            });

        var getCallback = undefined,
            startCallback = undefined,
            createCallback = undefined;

        var originParameters = [];

        var setupParams = Object.create({
            live: false,
            confirmFinish: true,
            canSearch: true,
            stepCallback: undefined,
            finishCallback: undefined,
            wisepod: undefined,
            wisepodClass: undefined,
            podContainer: undefined
        });

        var containers = Object.create({
            container: null,
            originalContainer: null,
            frame: false,
            contents: null,
            body: null,
            window: null,
            data: null
        });


        function setScrollHeight(element) {
            element.style.height = "";
            if (element.scrollHeight > Math.ceil(parseFloat(window.getComputedStyle(element).lineHeight))) {
                element.classList.add("scrollHeight");
                if (element.classList.contains("noOverflow")) {
                    element.style.height = element.scrollHeight + "px";
                }
            } else {
                element.classList.remove("scrollHeight", "noOverflow");
            }
        }

        function playData(element) {
            window.sm.play($(element).parent("li").data("word"), 1, {
                autoplay: true,
                destroyable: false
            });
        }

        $(dom.scoutpad.ul).on("click", ".wn-dll-pl", function(e) {
            e.preventDefault(this);
            playData(this);
        });


        function openCloseBox(container, open, animate, callback) {
            if (typeof animate !== "boolean") {
                animate = true;
            }

            var animated = typeof container.animated === "boolean" && container.animated;

            if (container.classList.contains("wl-ft-ct-vs") && !animated) {
                if (animate) {
                    container.animated = true;
                }

                var state = container.classList.contains("wl-ft-ct-ac");

                var box = container.getElementsByClassName("wl-ft-ct-bx")[0];
                var boxStyles = window.getComputedStyle(box);
                var contents = box.getElementsByClassName("wl-ft-ct-bx-ctn")[0];

                var height = (
                    contents.offsetHeight
                        + parseInt(boxStyles.getPropertyValue("padding-top"))
                        + parseInt(boxStyles.getPropertyValue("padding-bottom"))
                ) + "px";

                if (open === true || ((open === undefined || open === null || open === "auto") && !state)) {
                    if (animate) {
                        $(box).transition({ "height": height }, 400)
                            .promise()
                            .always(function() {
                                container.animated = false;

                                if (typeof callback === "function") {
                                    callback();
                                }
                            });
                    } else {
                        box.style.height = height;

                        if (typeof callback === "function") {
                            callback();
                        }
                    }

                    container.classList.add("wl-ft-ct-ac");
                } else {
                    if (animate) {
                        $(box).transition({ "height": "0px" }, 400)
                            .promise()
                            .always(function() {
                                container.animated = false;

                                if (typeof callback === "function") {
                                    callback();
                                }
                            });
                    } else {
                        box.style.height = "0px";

                        if (typeof callback === "function") {
                            callback();
                        }
                    }

                    container.classList.remove("wl-ft-ct-ac");
                }
            }
        }

        function showHideBox(container, show, animate, open, callback) {
            if (typeof animate !== "boolean") {
                animate = true;
            }

            var state = container.classList.contains("wl-ft-ct-vs");

            if (show === true || ((show === undefined || show === null || show === "auto") && !state)) {
                if (animate) {
                    container.style.opacity = 0;
                }

                container.classList.add("wl-ft-ct-vs");

                if (container === dom.scoutpad.container) {
                    $(dom.scoutpad.ul).find(".wn-dll-ds").each(function() {
                        setScrollHeight(this);
                    });
                }

                if (typeof callback === "function") {
                    callback();
                }

                openCloseBox(container, open, animate);

                if (animate) {
                    $(container).animate({ opacity: 1 }, 400);
                }
            } else {
                if (animate) {
                    container.style.opacity = 1;

                    if (typeof callback === "function") {
                        callback();
                    }

                    if (open) {
                        openCloseBox(container, false, true);
                    }

                    $(container).animate({ opacity: 0 }, 400)
                        .promise()
                        .always(function() {
                            container.classList.remove("wl-ft-ct-vs");
                        });
                } else {
                    container.classList.remove("wl-ft-ct-vs");
                }
            }
        }

        function logSentenceError(origin, reference, exception, selection) {
            $.ajax({
                async: true,
                type: "POST",
                url: api.logSentenceError,
                contentType: "application/json;charset=utf8",
                data: JSON.stringify({
                    origin: origin,
                    reference: reference,
                    stack: e.stack,
                    message: e.message,
                    word: selection.selection,
                    context: selection.context,
                    parent: selection.parent
                })
            });
        }


        function checkHideScoutWords() {
            if (!dom.scoutpad.$ulWrapper.hasClass("maximized")) {
                dom.scoutpad.$ulWrapper.find("li").each(function(k) {
                    if (k === 0) {
                        this.style.display = "";
                    } else {
                        this.style.display = "none";
                    }
                });
            } else {
                dom.scoutpad.$ulWrapper.find("li").each(function() {
                    this.style.display = "";
                });
            }
        }


// Arrow and title click: open or close boxes
        $(dom.arrows).on("click", function() {
            var container = this.ancestor(".wl-ft-ct", true);
            openCloseBox(container, "auto", true);
        });

        $(dom.titles).on("click", function() {
            var container = this.ancestor(".wl-ft-ct", true);
            openCloseBox(container, "auto", true);
        });

        // Maximize ScoutPad list
        $(dom.scoutpad.maximize).on("click", function() {
            var self = $(this);

            if (!self.hasClass("clicked")) {
                self.addClass("clicked").toggleClass("down");
                dom.scoutpad.$ulWrapper.toggleClass("maximized");
                checkHideScoutWords();

                openCloseBox(dom.scoutpad.container, true, true);
                self.removeClass("clicked");
            }
        });

        $(dom.scoutpad.ul).on("dblclick", ".wn-dll-md", function() {
            deleteScoutedWord(this);
        });

        $(dom.wisdom.keyhunter.ul).on("dblclick", ".wn-dll-md", function() {
            deleteKeyword(this);
        });

        dom.wisdom.wisdomhunter.text.on("input", function() {
            if (article.status === status.wisdom) {
                var content = dom.wisdom.wisdomhunter.text.val();
                if (content.length > articlehunt.maxLead) {
                    content = content.substring(0, articlehunt.maxLead);
                    dom.wisdom.wisdomhunter.text.val(content);
                }

                articlehunt.actualLead = content.length;

                dom.wisdom.actual.innerText = articlehunt.actualLead;
                dom.wisdom.max.innerText = articlehunt.maxLead;

                autoSaveLead();
            }
        });

        var scoutpadTransition = false;
        $(dom.scoutpad.ul).on("click", ".wn-dll-ds", function(e) {
            if (!scoutpadTransition && this.classList.contains("scrollHeight") && e.target === this && e.offsetX < 16) {
                scoutpadTransition = true;

                if (this.classList.contains("noOverflow")) {
                    this.classList.remove("noOverflow");
                    $(this).transition({ height: "1.25em" }, 200, function() {
                        openCloseBox(dom.scoutpad.container, true, true, function() {
                            scoutpadTransition = false;
                        });
                    });
                } else {
                    this.classList.add("noOverflow");
                    $(this).transition({ height: this.scrollHeight }, 200, function() {
                        openCloseBox(dom.scoutpad.container, true, true, function() {
                            scoutpadTransition = false;
                        });
                    });
                }
            }
        });

        $(window).on("resize", function() {
            $(dom.scoutpad.ul).find(".wn-dll-ds").each(function() {
                setScrollHeight(this);
            });

            openCloseBox(dom.scoutpad.container, true, false);
            openCloseBox(dom.wisdom.container, true, false);
        });


        function clearData() {
            while (dom.scoutpad.ul.firstChild) {
                dom.scoutpad.ul.removeChild(dom.scoutpad.ul.firstChild);
            }

            dom.scoutpad.searchInput.value = "";

            while (dom.wisdom.keyhunter.ul.firstChild) {
                dom.wisdom.keyhunter.ul.removeChild(dom.wisdom.keyhunter.ul.firstChild);
            }

            while (dom.wisdom.expression.ul.firstChild) {
                dom.wisdom.expression.ul.removeChild(dom.wisdom.expression.ul.firstChild);
            }

            dom.wisdom.wisdomhunter.text.val("");
        }

        function refreshContainer() {
            if (containers.frame) {
                containers.contents = containers.container.contents();
                containers.body = containers.contents.find("body");

                containers.window = containers.container[0].contentWindow;
                containers.data = containers.body;
            } else {
                containers.contents = null;
                containers.body = null;

                containers.window = containers.frameContents !== false ? containers.frameContents.contentWindow : window;
                containers.data = containers.container;
            }
        }

        function createArticleIfNotExists(after) {
            var createFunction = function(f) {
                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.getArticle,
                    data: { origin: origin, reference: reference },

                    success: function(result) {
                        loaded = true;

                        parameters = Object.create(result.parameters);
                        article = Object.create(result.article);
                        if (article.status === status.none)
                            article.status = status.scout;

                        articlehunt.scouted = (result.article.scouted === null ? [] : result.article.scouted);
                        articlehunt.actualScoutedWords = articlehunt.scouted.length;
                        articlehunt.keywords = (result.article.keywords === null ? [] : result.article.keywords);
                        articlehunt.actualKeywords = articlehunt.keywords.length;
                        articlehunt.expressions = (result.article.expressions === null ? [] : result.article.expressions);
                        articlehunt.actualExpressions = articlehunt.expressions.length;
                        articlehunt.lead = (result.article.lead === undefined || result.article.lead === null ? "" : result.article.lead);
                        articlehunt.actualLead = articlehunt.lead.length;

                        clearData();

                        if (typeof createCallback === "function") {
                            createCallback(origin, reference);
                        }

                        if (typeof getCallback === "function") {
                            getCallback();
                        }

                        f();
                    }
                });
            }

            if (reference === -1 && typeof startCallback === "function") {
                startCallback(function(ref) {
                    reference = ref;
                }, function() {
                    createFunction(after);
                });
            } else {
                after();
            }
        }

        var setStatus = [];

        function highlight(data, color, className) {
            if (typeof data === "string") {
                containers.data.highlight(data, color, className);
            } else if (typeof data === "object" && data !== null) {
                for (var i = 0, l = data.length; i < l; i++) {
                    if (typeof data[i] === "string") {
                        containers.data.highlight(data[i], color, className);
                    } else if (typeof data[i] === "object") {
                        containers.data.highlight(data[i].data, color, className);
                    }
                }
            }
        };

        function finishStatus(doneButton, confirmFinish, callback) {
            if (!loaded || !started || paused) {
                return false;
            }

            if (typeof confirmFinish !== "boolean") {
                confirmFinish = setupParams.confirmFinish;
            }

            // TODO CHECK IF NODE IS NOT NULL
            doneButton.classList.add("inactive");
            doneButton.disabled = true;

            createArticleIfNotExists(function() {
                var minReached = true,
                    minMessage = null,
                    confirmMessage = null,
                    errorMessage = null;

                switch (article.status) {
                case status.scout:
                    if (parameters.minScoutWords > 0 && articlehunt.actualScoutedWords < parameters.minScoutWords) {
                        minReached = false;
                    }

                    minMessage = i18n.t("wiselab.minimumscout");
                    confirmMessage = i18n.t("wiselab.confirmscoutfinish");
                    errorMessage = i18n.t("wiselab.errorscoutfinish");
                    break;

                case status.key:
                    if (parameters.minKeywords > 0 && articlehunt.actualKeywords < parameters.minKeywords) {
                        minReached = false;
                    }

                    minMessage = i18n.t("wiselab.minimumkeywords");
                    confirmMessage = i18n.t("wiselab.confirmkeyfinish");
                    errorMessage = i18n.t("wiselab.errorkeyfinish");
                    break;

                case status.expression:
                    if (parameters.minScoutWords > 0 && articlehunt.actualScoutedWords < parameters.minScoutWords) {
                        minReached = false;
                    }

                    minMessage = i18n.t("wiselab.minimumexpressions");
                    confirmMessage = i18n.t("wiselab.confirmexpessionfinish");
                    errorMessage = i18n.t("wiselab.errorexpressionfinish");
                    break;

                case status.wisdom:
                    if (articlehunt.actualLead.trim() <= 0) {
                        minReached = false;
                    }

                    minMessage = i18n.t("wiselab.minimumwisdom");
                    confirmMessage = i18n.t("wiselab.confirmwisdomfinish");
                    errorMessage = i18n.t("wiselab.errorwisdomfinish");
                    break;
                }

                var wrapper = undefined;

                function x() {
                    if (ajaxBusy) {
                        setTimeout(x, 100);
                        return;
                    }

                    ajaxBusy = true;

                    $.ajax({
                        async: true,
                        type: "POST",
                        url: api.finishStatus,
                        data: {
                            origin: origin,
                            reference: reference,
                            offsetDate: window.user.offsetDate
                        },

                        success: function(result) {
                            if (result.error === errors.unknown) {
                                createNoty("error", "center", errorMessage);

                                // TODO CHECK IF NODE IS NOT NULL
                                doneButton.classList.remove("inactive");
                            } else if (result.error === errors.cantFinish) {
                                createNoty("error", "center", minMessage);

                                // TODO CHECK IF NODE IS NOT NULL
                                doneButton.classList.remove("inactive");
                            } else {
                                article.status = result.status;

                                if (typeof setupParams.stepCallback === "function") {
                                    setupParams.stepCallback();
                                }

                                if (typeof setupParams.finishCallback === "function") {
                                    setupParams.finishCallback();
                                }

                                refreshStatus();

                                if (typeof callback === "function") {
                                    callback();
                                }
                            }

                            if (wrapper !== undefined) {
                                document.body.removeChild(wrapper);
                            }
                            ajaxBusy = false;
                        },

                        error: function() {
                            createNoty("error", "center", errorMessage);

                            // TODO CHECK IF NODE IS NOT NULL
                            doneButton.classList.remove("inactive");

                            if (wrapper !== undefined) {
                                document.body.removeChild(wrapper);
                                doneButton.disabled = false;
                            }
                            ajaxBusy = false;
                        }
                    });
                }

                if (!minReached) {
                    createNoty("warning", "center", minMessage);

                    // TODO CHECK IF HAS BUTTON!!!
                    doneButton.classList.remove("inactive");
                } else if (confirmFinish) {
                    var buttons = [
                        {
                            text: i18n.t("cancel"),
                            addClass: "input input-bl",
                            onClick: function($noty) {
                                $noty.$buttons.find("button").off("click");
                                $noty.close();

                                doneButton.classList.remove("inactive");
                                doneButton.disabled = false;

                                if (wrapper !== undefined) {
                                    document.body.removeChild(wrapper);
                                }
                            }
                        },
                        {
                            text: i18n.t("ok"),
                            addClass: "input input-or",
                            onClick: function($noty) {
                                $noty.$buttons.find("button").off("click");
                                $noty.close();
                                x();
                            }
                        }
                    ];

                    wrapper = document.createElement("div");
                    wrapper.id = "wn-dn-wr";
                    document.body.appendChild(wrapper);
                    createConfirmNoty("center", confirmMessage, buttons);
                } else {
                    x();
                }
            });

            return true;
        }


        function checkScoutedWord(word) {
            if (!loaded || !started || paused) {
                return false;
            }

            var normalizedWord = word.trim().toLowerCase();
            var exists = false;

            for (var i = 0, l = articlehunt.scouted.length; i < l; i++) {
                if (articlehunt.scouted[i] === normalizedWord) {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        function insertScoutedWord(data) {
            if (data === undefined || data === null || data.translations === undefined || data.translations === null) {
                return null;
            }

            var li = $(document.createElement("li"))
                .data({
                    word: data.translations.word
                });

            $(document.createElement("span"))
                .addClass("wn-dll-md wn-dll-mdtw-" + (data.searched ? WISELAB_SEARCHED_TYPE : origin))
                .attr("title", i18n.t("actions.dblclick-delete"))
                .text(data.translations.word)
                .appendTo(li);

            $(document.createElement("span"))
                .addClass("wn-dll-pl fa fa-volume-up")
                .attr("title", i18n.t("actions.play"))
                .appendTo(li);

            if (data.translations.suggestedTranslation !== null) {
                $(document.createElement("span"))
                    .addClass("wn-dll-ms")
                    .text(data.translations.suggestedTranslation)
                    .appendTo(li);
            }

            var translationsDiv = $(document.createElement("div"))
                .addClass("wn-dll-da")
                .appendTo(li);

            for (var i = 0, l = data.translations.otherTranslations.length; i < l; i++) {
                var translations = data.translations.otherTranslations[i];

                if (translations.translations.length === 0) {
                    continue;
                }

                var div = $(document.createElement("div"))
                    .addClass("wn-dll-ds")
                    .appendTo(translationsDiv);

                $(document.createElement("span"))
                    .addClass("wn-dll-dst wn-dll-dst-" + translations.type)
                    .text(i18n.t("wordtypes." + translations.type))
                    .appendTo(div);

                for (var j = 0, k = translations.translations.length; j < k; j++) {
                    var translation = translations.translations[j];

                    var spanWrapper = $(document.createElement("span"))
                        .addClass("wn-dll-sdw")
                        .appendTo(div);

                    $(document.createElement("span"))
                        .addClass("wn-dll-dd")
                        .text(translation)
                        .appendTo(spanWrapper);

                    if ((j + 1) !== k) {
                        $(document.createTextNode(","))
                            .appendTo(spanWrapper);

                        $(document.createTextNode(" "))
                            .appendTo(div);
                    }
                }
            }

            $(dom.scoutpad.ul).prepend(li);

            li.find(".wn-dll-ds").each(function() {
                setScrollHeight(this);
            });

            articlehunt.actualScoutedWords++;
            return true;
        }

        function addScoutedWord(word, sentence, searched, callback) {
            if (!loaded || !started || paused) {
                return false;
            }

            if (typeof searched !== "boolean") {
                searched = false;
            }

            createArticleIfNotExists(function() {
                if (article.status === status.scout) {
                    (function x() {
                        if (ajaxBusy) {
                            setTimeout(x, 100);
                            return;
                        }

                        ajaxBusy = true;

                        var apiParameters = Object.create({
                            origin: origin,
                            reference: reference,
                            data: word,
                            offsetDate: window.user.offsetDate
                        });

                        if (!searched) {
                            apiParameters.sentence = sentence;
                            apiParameters.translate = true;
                        }

                        $.ajax({
                            async: true,
                            type: "POST",
                            url: (searched ? api.searchWord : api.addHuntData),
                            data: apiParameters,

                            success: function(result) {
                                switch (result.error) {
                                case errors.none:
                                    highlight(word, colors.scoutedWords, "altea-wiselab-span-word-1");
                                    clearSelection(containers.window);
                                    result.searched = searched;
                                    insertScoutedWord(result);
                                    checkHideScoutWords();
                                    openCloseBox(dom.scoutpad.container, true, true);
                                    break;

                                case errors.dataNotExists:
                                    createNoty("warning", "center", i18n.t("wiselab.notranslations"));
                                    break;

                                case errors.unknown:
                                    createNoty("warning", "center", i18n.t("error-processing"));
                                }

                                if (typeof callback === "function") {
                                    callback();
                                }

                                ajaxBusy = false;
                            },

                            error: function() {
                                createNoty("error", "center", i18n.t("internal-error"));

                                if (typeof callback === "function") {
                                    callback();
                                }

                                ajaxBusy = false;

                            }
                        });
                    })();
                } else {
                    createNoty("error", "center", i18n.t("internal-error"));
                }
            });

            return true;
        }

        function searchScoutWord() {
            addScoutedWord(dom.scoutpad.searchInput.value, null, true);
            dom.scoutpad.searchInput.value = null;
        }

        function deleteScoutedWord(word) {
            if (!loaded || !started || paused) {
                return false;
            }

            if (article.status === status.scout) {
                var normalizedWord = word.innerText.trim().toLowerCase();
                var position = -1;

                for (var i = 0, l = articlehunt.scouted.length; i < l; i++) {
                    if (articlehunt.scouted[i].data === normalizedWord) {
                        position = i;
                        break;
                    }
                }

                if (position !== -1) {
                    (function x() {
                        if (ajaxBusy) {
                            setTimeout(x, 100);
                            return;
                        }

                        ajaxBusy = true;

                        var apiParameters = Object.create({
                            origin: origin,
                            reference: reference,
                            data: word
                        });

                        $.ajax({
                            async: true,
                            type: "POST",
                            url: api.removeHuntData,
                            data: apiParameters,

                            success: function(result) {
                                switch (result.error) {
                                case errors.none:
                                    articlehunt.scouted.splice(position, 1);
                                    articlehunt.actualScoutedWords--;

                                    var li = word.parent;
                                    li.parent.removeChild(li);
                                    break;

                                case errors.unknown:
                                    createNoty("warning", "center", i18n.t("error-processing"));
                                    break;
                                }

                                ajaxBusy = false;
                            },

                            error: function() {
                                createNoty("error", "center", i18n.t("internal-error"));
                                ajaxBusy = false;
                            }
                        });
                    })();
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        setStatus[status.scout] = function() {
            dom.wisdom.keyhunter.$ulWrapper.hide();
            dom.wisdom.expression.$ulWrapper.hide();
            dom.wisdom.wisdomhunter.$textWrapper.hide();

            if (setupParams.hasDoneButtons) {
                dom.scoutpad.done.classList.remove("inactive");
                dom.scoutpad.done.disabled = false;
                dom.scoutpad.doneWrapper.style.display = "";
            } else {
                dom.scoutpad.done.classList.add("inactive");
                dom.scoutpad.done.disabled = true;
                dom.scoutpad.doneWrapper.style.display = "none";
            }

            if (setupParams.canSearch) {
                dom.scoutpad.search.style.display = "";
                dom.scoutpad.searchInput.disabled = false;
                dom.scoutpad.searchSubmit.disabled = false;

                $(dom.scoutpad.searchInput).on("keyup", function(e) {
                    if (e.which === 13) {
                        searchScoutWord();
                    }
                });

                $(dom.scoutpad.searchSubmit).on("click", searchScoutWord);
            } else {
                dom.scoutpad.search.style.display = "none";
                dom.scoutpad.searchInput.disabled = true;
                dom.scoutpad.searchSubmit.disabled = true;
            }

            dom.scoutpad.container.classList.add("wl-ft-ct-as");
            showHideBox(dom.scoutpad.container, true, true, true);
            dom.scoutpad.ul.waiting = false;

            containers.data.on("dblclick", function() {
                if (!loaded || !started || paused) {
                    return false;
                }

                var selection = containers.window.getSelection();
                var selectedData = getSelectionData(selection);

                (function x() {
                    if (dom.scoutpad.ul.waiting) {
                        setTimeout(x, 100);
                        return;
                    }

                    dom.scoutpad.ul.waiting = true;

                    var word = selectedData.selection;
                    var sentence;

                    try {
                        sentence = getPhrase(word, selectedData.context, selectedData.parent);
                    } catch (e) {
                        sentence = null;
                        logSentenceError(origin, reference, e, selectedData);
                    }


                    if (word.length < MIN_WORD_LENGTH || REGEXP_MATCH.test(word)) {
                        createNoty("warning", "center", i18n.t("wiselab.invalidword"));
                        dom.scoutpad.ul.waiting = false;
                    } else if (checkScoutedWord(word)) {
                        createNoty("warning", "center", i18n.t("wiselab.alreadycapturedword"));
                        dom.scoutpad.ul.waiting = false;
                    } else {
                        addScoutedWord(word, sentence, false, function() {
                            dom.scoutpad.ul.waiting = false;
                        });
                    }
                })();

                return true;
            });
        }

        function checkKeyword(word) {
            if (!loaded || !started || paused) {
                return false;
            }

            var normalizedWord = word.trim().toLowerCase();
            var exists = false;

            for (var i = 0, l = articlehunt.keywords.length; i < l; i++) {
                if (articlehunt.keywords[i] === normalizedWord) {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        function insertKeyword(word) {
            if (word === undefined || word === null) {
                return null;
            }

            var li = $(document.createElement("li"));

            $(document.createElement("span"))
                .addClass("wn-dll-md")
                .attr("title", i18n.t("actions.dblclick-delete"))
                .text(word)
                .appendTo(li);

            $(dom.wisdom.keyhunter.ul).append(li);

            articlehunt.actualKeywords++;
            return true;
        }

        function addKeyword(word, callback) {
            if (!loaded || !started || paused) {
                return false;
            }

            if (article.status === status.key) {
                (function x() {
                    if (ajaxBusy) {
                        setTimeout(x, 100);
                        return;
                    }

                    ajaxBusy = true;

                    $.ajax({
                        async: true,
                        type: "POST",
                        url: api.addHuntData,
                        data: Object.create({
                            origin: origin,
                            reference: reference,
                            data: word,
                            sentence: null,
                            translate: false,
                            offsetDate: window.user.offsetDate
                        }),

                        success: function(result) {
                            switch (result.error) {
                            case errors.none:
                                highlight(word, colors.keyWords, "altea-wiselab-span-word-2");
                                clearSelection(containers.window);
                                insertKeyword(word);
                                openCloseBox(dom.wisdom.container, true, true);
                                break;

                            case errors.unknown:
                                createNoty("warning", "center", i18n.t("error-processing"));

                            }

                            if (typeof callback === "function") {
                                callback();
                            }

                            ajaxBusy = false;
                        },

                        error: function() {
                            createNoty("error", "center", i18n.t("internal-error"));

                            if (typeof callback === "function") {
                                callback();
                            }

                            ajaxBusy = false;

                        }
                    });
                })();
            } else {
                createNoty("error", "center", i18n.t("internal-error"));
            }

            return true;
        }

        function deleteKeyword(word) {
            if (!loaded || !started || paused) {
                return false;
            }

            if (article.staus === status.keyhunter) {
                var normalizedWord = word.innerText.trim().toLowerCase();
                var position = -1;

                for (var i = 0, l = articlehunt.keywords.length; i < l; i++) {
                    if (articlehunt.keywords[i] === normalizedWord) {
                        position = i;
                        break;
                    }
                }

                if (position !== -1) {
                    (function x() {
                        if (ajaxBusy) {
                            setTimeout(x, 100);
                            return;
                        }

                        ajaxBusy = true;

                        var apiParameters = Object.create({
                            origin: origin,
                            reference: reference,
                            data: word
                        });

                        $.ajax({
                            async: true,
                            type: "POST",
                            url: api.removeHuntData,
                            data: apiParameters,

                            success: function(result) {
                                switch (result.error) {
                                case errors.none:
                                    articlehunt.keywords.splice(position, 1);
                                    articlehunt.actualKeywords--;
                                    dom.wisdom.actualKeywords.text(articlehunt.actualKeywords);

                                    var li = word.parent;
                                    li.parent.removeChild(li);
                                    break;

                                case errors.unknown:
                                    createNoty("warning", "center", i18n.t("error-processing"));
                                    break;
                                }

                                ajaxBusy = false;
                            },

                            error: function() {
                                createNoty("error", "center", i18n.t("internal-error"));
                                ajaxBusy = false;
                            }
                        });
                    })();
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        setStatus[status.key] = function() {
            dom.wisdom.container.dataset.status = 2;
            dom.wisdom.keyhunter.$ulWrapper.show();
            $(dom.wisdom.keyhunter.ulWrapper).mCustomScrollbar(scrollbarParameters);
            dom.wisdom.expression.$ulWrapper.hide();
            dom.wisdom.wisdomhunter.$textWrapper.hide();

            dom.scoutpad.done.classList.add("inactive");
            dom.scoutpad.done.disabled = true;

            dom.scoutpad.doneWrapper.style.display = "none";
            dom.scoutpad.search.style.display = "none";

            showHideBox(dom.scoutpad.container, true, false, false);

            dom.wisdom.actual.innerText = articlehunt.actualKeywords;
            dom.wisdom.max.innerText = articlehunt.maxKeywords;

            if (setupParams.hasDoneButtons) {
                dom.wisdom.done.classList.remove("inactive");
                dom.wisdom.done.disabled = false;
                dom.wisdom.doneWrapper.style.display = "";
            } else {
                dom.wisdom.done.classList.add("inactive");
                dom.wisdom.done.disabled = true;
                dom.wisdom.doneWrapper.style.display = "none";
            }

            dom.wisdom.container.classList.add("wl-ft-ct-as");
            showHideBox(dom.wisdom.container, true, true, true);

            containers.data.on("dblclick", function() {
                if (!loaded || !started || paused) {
                    return false;
                }

                var selection = containers.window.getSelection();
                var word = getSelectionData(selection, true);

                (function x() {
                    if (dom.wisdom.keyhunter.ul.waiting) {
                        setTimeout(x, 100);
                        return;
                    }

                    dom.wisdom.keyhunter.ul.waiting = true;

                    if (word.length < MIN_WORD_LENGTH || REGEXP_MATCH.test(word)) {
                        createNoty("warning", "center", i18n.t("wiselab.invalidword"));
                        dom.wisdom.keyhunter.ul.waiting = false;
                    } else if (checkKeyword(word)) {
                        createNoty("warning", "center", i18n.t("wiselab.alreadycapturedword"));
                        dom.wisdom.keyhunter.ul.waiting = false;
                    } else if (articlehunt.actualKeywords >= articlehunt.maxKeywords) {
                        createNoty("warning", "center", i18n.t("wiselab.maxkeywords"));
                    } else {
                        addKeyword(word, function() {
                            dom.wisdom.actual.innerText = articlehunt.actualKeywords;
                            dom.wisdom.keyhunter.ul.waiting = false;
                        });
                    }
                })();

                return true;
            });
        }

        function checkExpression() {

        }

        function insertExpression() {

        }

        function deleteExpression() {

        }

        setStatus[status.expression] = function() {
            // TODO!
        }

        var leadTimeout = null, lastSavedLead = null;

        function autoSaveLead() {
            if (!loaded || !started || paused) {
                return false;
            }

            var lead = dom.wisdom.wisdomhunter.text.val();
            if (lead !== lastSavedLead) {
                clearTimeout(leadTimeout);

                leadTimeout = setTimeout(function x() {
                    var newLead = dom.wisdom.wisdomhunter.text.val();

                    if (newLead !== lastSavedLead) {
                        $.ajax({
                            async: true,
                            type: "POST",
                            url: api.saveLead,
                            data: {
                                origin: origin,
                                reference: reference,
                                lead: newLead,
                                autoSave: true
                            },

                            success: function() {
                                lastSavedLead = newLead;
                            }
                        });
                    }
                }, 500);
            }
        }

        setStatus[status.wisdom] = function() {
            dom.wisdom.container.dataset.status = 8;
            dom.wisdom.keyhunter.$ulWrapper.show();
            $(dom.wisdom.keyhunter.ulWrapper).mCustomScrollbar(scrollbarVisibleParameters);
            dom.wisdom.expression.$ulWrapper.hide();
            dom.wisdom.wisdomhunter.$textWrapper.show();

            dom.scoutpad.done.classList.add("inactive");
            dom.scoutpad.done.disabled = true;

            dom.scoutpad.doneWrapper.style.display = "none";
            dom.scoutpad.search.display = "none";

            showHideBox(dom.scoutpad.container, true, false, false);

            dom.wisdom.actual.innerText = articlehunt.actualLead;
            dom.wisdom.max.innerText = articlehunt.maxLead;

            if (setupParams.hasDoneButtons) {
                dom.wisdom.done.classList.remove("inactive");
                dom.wisdom.done.disabled = false;
                dom.wisdom.doneWrapper.style.display = "";
            } else {
                dom.wisdom.done.classList.add("inactive");
                dom.wisdom.done.disabled = true;
                dom.wisdom.doneWrapper.style.display = "none";
            }

            dom.wisdom.container.classList.add("wl-ft-ct-as");
            showHideBox(dom.wisdom.container, true, true, true, function() {
                var styles = window.getComputedStyle(dom.wisdom.keyhunter.ulWrapper);
                dom.wisdom.wisdomhunter.textWrapper.style.marginLeft = (parseFloat(styles.width) + 7 + 5) + "px";
                dom.wisdom.wisdomhunter.textWrapper.style.height = styles.height;
            });
        }

        setStatus[status.finished] = function() {
            DOM.scoutpad.box.find(".scoutSectionInline").hide();
            DOM.scoutpad.done.addClass("inactive").hide();

            showBoxes(true, false, parameters.hasPlanner);
            if (typeof setupParams.wisepod === "object") {
                setupParams.wisepod.removeClass(setupParams.wisepodClass);
                setupParams.wisepod.on("click", function() {
                    if (!$(this).hasClass(setupParams.wisepodClass)) {
                        window.wisepod.get(origin, reference, {
                            onLoad: function(pod, content) {
                                pod.show();
                            }
                        });
                    }
                });
            }
        }

        function refreshStatus() {
            if (!loaded || !started || paused) {
                return false;
            }

            containers.data.off("dblclick");
            containers.data.off("mouseup");
            //dom.wisdom.wisdomhunter.text.off("focus");
            //dom.wisdom.wisdomhunter.text.off("blur");

            if (typeof setupParams.wisepod === "object") {
                setupParams.wisepod.classList.add(setupParams.wisepodClass);
            }

            for (var i = 0, l = dom.containers.length; i < l; i++) {
                dom.containers[i].classList.remove("wl-ft-ct-as");
            }

            setStatus[article.status]();
            return true;
        }


// Click on finish buttons
        $(dom.scoutpad.done).on("click", function() {
            if (!loaded || !started || paused || !setupParams.hasDoneButtons) {
                return false;
            }

            if (article.status === status.scout && !dom.scoutpad.done.classList.contains("inactive")) {
                finishStatus(dom.scoutpad.done, setupParams.confirmFinish);
            }
            return true;
        });

        $(dom.wisdom.done).on("click", function() {
            if (!loaded || !started || paused || !setupParams.hasDoneButtons) {
                return false;
            }

            if (article.status > status.scout && article.status < status.finished && !dom.wisdom.done.classList.contains("inactive")) {
                finishStatus(dom.wisdom.done, setupParams.confirmFinish);
            }
            return true;
        });


/* [BEGIN] PUBLIC METHODS */

        this.setup = function(params, callback) {
            if (typeof params !== "object" || params === null) {
                return false;
            }

            if (!mainLoaded && !mainLoading) {
                mainLoading = true;

                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.getFinishedWords,

                    success: function(result) {
                        mainLoaded = true;
                        mainLoading = false;
                        finishedWords = result;
                        ready = true;

                        if (typeof callback === "function") {
                            callback();
                        }
                    },
                    error: function() {
                        //TODO: MENSAJE DE ERROR
                        mainLoaded = false;
                        mainLoading = false;
                    }
                });
            } else {
                ready = true;

                if (typeof callback === "function") {
                    callback();
                }
            }

            var container;

            if (params.container instanceof Node) {
                container = $(params.container);
            } else if (params.container instanceof jQuery) {
                container = params.container;
            } else {
                return false;
            }

            containers.container = container;
            containers.originalContainer = container;
            containers.frame = typeof params.frame === "boolean" ? params.frame : false;
            containers.frameContents = params.frameContents instanceof Node ? params.frameContents : false;

            setupParams.live = typeof params.live === "boolean" ? params.live : false;
            setupParams.confirmFinish = typeof params.confirmFinish === "boolean" ? params.confirmFinish : true;
            setupParams.canSearch = typeof params.canSearch === "boolean" ? params.canSearch : true;
            setupParams.hasDoneButtons = typeof params.hasDoneButtons === "boolean" ? params.hasDoneButtons : true;

            setupParams.stepCallback = typeof params.stepCallback === "function" ? params.stepCallback : undefined;
            setupParams.finishCallback = typeof params.finishCallback === "function" ? params.finishCallback : undefined;

            setupParams.wisepod = params.wisepod;
            setupParams.wisepodClass = typeof params.wisepodClass === "object" ? params.wisepodClass : "wisedisabled";
            setupParams.podContainer = params.podContainer;

            return true;
        };

        this.get = function(o, r, c, sc, cc) {
            if (!ready) {
                return false;
            }

            if (typeof o !== "number" || typeof r !== "number" ||
                    o !== (o | 0) || r !== (r | 0) ||
                    o !== +o || r !== +r ||
                    o === 0 || r === 0
            ) {
                return false;
            }

            (function x() {
                if (!mainLoaded || ajaxBusy) {
                    setTimeout(x, 100);
                    return;
                }

                ajaxBusy = true;
                loaded = false;
                started = false;
                getCallback = c;

                if (r === -1) {
                    startCallback = sc;
                    createCallback = cc;

                    var loadOriginParameters = function(result) {
                        loaded = true;
                        origin = o;
                        reference = r;

                        parameters = Object.create(result);
                        article = Object.create({ status: status.scout });

                        articlehunt.scouted = [];
                        articlehunt.actualScoutedWords = 0;
                        articlehunt.keywords = [];
                        articlehunt.actualKeywords = 0;
                        articlehunt.expressions = [];
                        articlehunt.actualExpressions = 0;
                        articlehunt.lead = "";
                        articlehunt.actualLead = 0;

                        clearData();

                        if (typeof getCallback === "function") {
                            getCallback();
                        }

                        lastSavedLead = articlehunt.lead;

                        ajaxBusy = false;
                    };

                    if (originParameters[o] === undefined || originParameters[o] === null) {
                        $.ajax({
                            async: true,
                            type: "POST",
                            url: api.getOriginParameters,
                            data: { origin: o },

                            success: function(result) {
                                originParameters[o] = result;
                                loadOriginParameters(result);
                            },

                            error: function() {
                                ajaxBusy = false;
                            }
                        });
                    } else {
                        loadOriginParameters(originParameters[o]);
                    }
                } else {
                    startCallback = null;
                    createCallback = null;

                    $.ajax({
                        async: true,
                        type: "POST",
                        url: api.getArticle,
                        data: { origin: o, reference: r },

                        success: function(result) {
                            loaded = true;
                            origin = o;
                            reference = r;

                            parameters = Object.create(result.parameters);
                            article = Object.create(result.article);
                            if (article.status === status.none) {
                                article.status = status.scout;
                            }

                            articlehunt.searched = [];
                            articlehunt.scouted = (result.article.scouted === null ? [] : result.article.scouted);
                            articlehunt.actualScoutedWords = articlehunt.scouted.length;
                            articlehunt.keywords = (result.article.keywords === null ? [] : result.article.keywords);
                            articlehunt.actualKeywords = articlehunt.keywords.length;
                            articlehunt.expressions = (result.article.expressions === null ? [] : result.article.expressions);
                            articlehunt.actualExpressions = articlehunt.expressions.length;
                            articlehunt.lead = (result.article.lead === undefined || result.article.lead === null ? "" : result.article.lead);
                            articlehunt.actualLead = articlehunt.lead.length;

                            clearData();

                            if (typeof getCallback === "function") {
                                getCallback();
                            }

                            if (article.forcePod) {
                                // TODO WISEPOD SHOW!
                            }

                            lastSavedLead = articlehunt.lead;

                            ajaxBusy = false;
                        },

                        error: function() {
                            ajaxBusy = false;
                        }
                    });
                }
            })();
            return true;
        };

        this.start = function() {
            if (!loaded || started || paused) {
                return false;
            }

            started = true;
            paused = false;

            if (setupParams.live) {
                containers.container = $(containers.originalContainer);
            }

            refreshContainer();

            var dataCounter = containers.data.wordcounter();
            articlehunt.maxKeywords = Math.min(
            (typeof parameters.minKeywords !== "number" || parameters.minKeywords === -1
                ? Math.max(parseInt(dataCounter[0] * 0.01), MIN_KEYWORDS)
                : parameters.minKeywords),
            (typeof parameters.maxKeywords !== "number" || parameters.maxKeywords === -1
                ? MAX_KEYWORDS
                : parameters.maxKeywords));

            articlehunt.maxExpressions = 0;

            articlehunt.maxLead = Math.min(
            (typeof parameters.minLead !== "number" || parameters.minLead === -1
                ? Math.max(parseInt(dataCounter[0] * 0.07), MIN_LEAD)
                : parameters.minLead),
            (typeof parameters.maxLead !== "number" || parameters.maxLead === -1
                ? MAX_LEAD
                : parameters.maxLead));

            highlight(finishedWords, colors.finishedWords, "altea-wiselab-span-word-0");

            if (reference !== -1) {
                highlight(articlehunt.scouted, colors.scoutedWords, "altea-wiselab-span-word-1");
                highlight(articlehunt.keywords, colors.keyWords, "altea-wiselab-span-word-2");
                highlight(articlehunt.expressions, colors.expressions, "altea-wiselab-span-word-3");
                dom.wisdom.wisdomhunter.text.val(articlehunt.lead);

                var i, l;
                for (i = 0, l = articlehunt.scouted.length; i < l; i++) {
                    insertScoutedWord(articlehunt.scouted[i]);
                }

                $(dom.scoutpad.ulWrapper).find("li").each(function(k) {
                    if (k === 0) {
                        this.style.display = "";

                        $(this.length).find(".wn-dll-ds").each(function() {
                            setScrollHeight(this);
                        });
                    } else {
                        this.style.display = "none";
                    }
                });

                for (i = 0, l = articlehunt.keywords.length; i < l; i++) {
                    insertKeyword(articlehunt.keywords[i].data);
                }

                for (i = 0, l = articlehunt.keywords.length; i < l; i++) {
                    insertExpression(articlehunt.expressions[i]);
                }
            }

            refreshStatus();

            return true;
        };

        this.stop = function(animate) {
            if (!loaded || !started || paused) {
                return false;
            }

            started = false;

            containers.data.off("dblclick");

            var boxes = document.getElementsByClassName("wl-ft-ct");
            for (var i = 0, l = boxes.length; i < l; i++) {
                showHideBox(boxes[i], false, animate, true);
            }

            loaded = false;

            return true;
        };

        this.pause = function() {
            if (!loaded || !started) {
                return false;
            }

            paused = true;
            var boxes = document.getElementsByClassName("wl-ft-ct");
            for (var i = 0, l = boxes.length; i < l; i++) {
                boxes[i].classList.add("wl-ft-ct-ps");
            }

            return true;
        };

        this.resume = function() {
            if (!loaded || !started) {
                return false;
            }

            paused = false;
            var boxes = document.getElementsByClassName("wl-ft-ct");
            for (var i = 0, l = boxes.length; i < l; i++) {
                boxes[i].classList.remove("wl-ft-ct-ps");
            }

            return true;
        };
    }

    window.wiselab = new WiseLab();
});
