window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var api = Object.create({
        loadStax: "/WordStax/Stax",
        loadInbox: "/WordStax/Inbox",
        loadGraphs: "/WordStax/IndexGraphs",
        searchWord: "/WordStax/Search",
        acceptWord: "/WordStax/Accept",
        deleteWord: "/WordStax/Delete"
    });

    // ReSharper disable InconsistentNaming
    var MIN_WORD_LENGTH = 2,
        REGEXP_MATCH = new XRegExp("[^\\p{L}' -]", "i");
    // ReSharper restore InconsistentNaming

    var errors = Object.create({
        none: 0x00,
        unknown: 0xf0,
        dataExists: 0xf1,
        dataNotExists: 0xf2,
        inboxOverflow: 0xf3
    });

    var inboxData = [], staxData = null;

    function insertInboxData(data, update, scroll) {
        if (data === undefined || data === null || data.translations === undefined || data.translations === null) {
            return null;
        }

        if (typeof update !== "boolean") {
            update = true;
        }

        if (typeof scroll !== "boolean") {
            scroll = true;
        }

        var li = $(document.createElement("li"))
            .data({
                id: data.id,
                word: data.translations.word,
                origin: data.origin,
                insertDate: data.insertDate,
                selected: 0
            });

        $(document.createElement("span"))
            .addClass("wn-dll-ins fa fa-arrow-down")
            .attr("title", i18n.t("stax.vocabulary.inbox.click-to-accept"))
            .appendTo(li);

        $(document.createElement("span"))
            .addClass("wn-dll-md wn-dll-mdt-" + data.origin)
            .attr("title", i18n.t("actions.dblclick-delete"))
            .text(data.translations.word)
            .appendTo(li);

        $(document.createElement("span"))
            .addClass("wn-dll-pl fa fa-volume-up")
            .attr("title", i18n.t("actions.play"))
            .appendTo(li);

        if (data.translations.suggestedTranslation !== null) {
            $(document.createElement("span"))
                .addClass("wn-dll-sd wn-dll-ms wn-dll-msc")
                .text(data.translations.suggestedTranslation)
                .data("type", 1)
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
                .data("type", translations.type)
                .appendTo(div);

            for (var j = 0, k = translations.translations.length; j < k; j++) {
                var translation = translations.translations[j];

                var spanWrapper = $(document.createElement("span"))
                    .addClass("wn-dll-sdw")
                    .appendTo(div);

                $(document.createElement("span"))
                    .addClass("wn-dll-sd wn-dll-dd wn-dll-ddc")
                    .text(translation)
                    .data("type", translations.type)
                    .appendTo(spanWrapper);

                if ((j + 1) !== k) {
                    $(document.createTextNode(","))
                        .appendTo(spanWrapper);

                    $(document.createTextNode(" "))
                        .appendTo(div);
                }
            }
        }

        var dataInStax = data.dataInStax || staxData[data.translations.word];

        if (dataInStax !== undefined && dataInStax !== null) {
            li.find(".wn-dll-sd").each(function() {
                if (dataInStax.indexOf(this.innerText) !== -1) {
                    this.classList.add("done");
                }
            });
        }

        var objectData = Object.create({
            word: data.translations.word,
            origin: data.origin,
            insertDate: data.insertDate,
            element: li
        });

        inboxData.push(objectData);
        $(li).data("position", inboxData.length - 1);

        if (update) {
            window.inbox.showInbox(inboxData, scroll);
        }

        return objectData;
    }

    function searchData(form, input, callback) {
        if (form.hasClass("disabled")) {
            return;
        }

        var word = input.val().trim(),
            normalizedWord = word.toLowerCase();

        if (normalizedWord.length === 0) {
            return;
        }

        form.addClass("disabled");
        input.prop("disabled", true);

        if (word.length < MIN_WORD_LENGTH || REGEXP_MATCH.test(word)) {
            createNoty("warning", "center", i18n.t("wiselab.invalidsearchedword"), 1500, ["click"], {
                callback:
                {
                    afterClose: function() {
                        form.removeClass("disabled");
                        input.prop("disabled", false);
                        input.focus();
                        if (typeof callback === "function") {
                            callback();
                        }
                    }
                }
            });
        } else {
            var exists = false;
            for (var i = 0, l = inboxData.length; i < l; i++) {
                if (inboxData[i] !== undefined && inboxData[i].word.toLowerCase() === normalizedWord) {
                    exists = true;
                    break;
                }
            }

            if (exists) {
                createNoty("warning", "center", i18n.t("stax.vocabulary.inbox.search-exists"), 1500, ["click"], {
                    callback:
                    {
                        afterClose: function() {
                            form.removeClass("disabled");
                            input.prop("disabled", false);
                            input.focus();
                            if (typeof callback === "function") {
                                callback();
                            }
                        }
                    }
                });
            } else {
                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.searchWord,
                    data: {
                        word: word,
                        offsetDate: window.user.offsetDate
                    },

                    success: function(result) {
                        switch (result.error) {
                        case errors.none:
                            var data = {
                                id: result.id,
                                origin: 1,
                                translations: result.translations,
                                insertDate: new Date()
                            };
                            var objectData = insertInboxData(data, false, false);
                            window.inbox.prependToInbox(objectData, true);

                            form.removeClass("disabled");
                            input.prop("disabled", false);
                            input.val("");
                            input.focus();
                            if (typeof callback === "function") {
                                callback();
                            }
                            break;

                        case errors.dataExists:
                            createNoty("warning", "center", i18n.t("stax.vocabulary.inbox.search-exists"), 1500, ["click"], {
                                callback:
                                {
                                    afterClose: function() {
                                        form.removeClass("disabled");
                                        input.prop("disabled", false);
                                        input.focus();
                                        if (typeof callback === "function") {
                                            callback();
                                        }
                                    }
                                }
                            });
                            break;

                        case errors.dataNotExists:
                            createNoty("warning", "center", i18n.t("stax.vocabulary.inbox.search-not-exists"), 1500, ["click"], {
                                callback:
                                {
                                    afterClose: function() {
                                        form.removeClass("disabled");
                                        input.prop("disabled", false);
                                        input.focus();
                                        if (typeof callback === "function") {
                                            callback();
                                        }
                                    }
                                }
                            });
                            break;

                        case errors.inboxOverflow:
                            createNoty("warning", "center", i18n.t("stax.vocabulary.inbox.overflow"), 1500, ["click"], {
                                callback:
                                {
                                    afterClose: function() {
                                        form.removeClass("disabled");
                                        input.prop("disabled", false);
                                        input.focus();
                                        if (typeof callback === "function") {
                                            callback();
                                        }
                                    }
                                }
                            });
                            break;
                        }

                    },

                    error: function() {
                        createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                            callback:
                            {
                                afterClose: function() {
                                    form.removeClass("disabled");
                                    input.prop("disabled", false);
                                    input.focus();
                                    if (typeof callback === "function") {
                                        callback();
                                    }
                                }
                            }
                        });
                    }
                });
            }
        }
    }

    function acceptData(wrapper, element) {
        if (typeof wrapper.busy === "boolean" && wrapper.busy) {
            setTimeout(function() {
                acceptData(wrapper, element);
            }, 100);
            return;
        }

        wrapper.busy = true;

        var parent = $(element).parent("li");
        var translations = parent.find(".wn-dll-sd.selected.clicked").map(function() {
            return {
                data: this.innerText,
                type: $(this).data("type")
            };
        });

        var uniqueTranslations = [], types = [];

        for (var i = 0, l = translations.length; i < l; i++) {
            var normalizedTranslation = translations[i].data.trim().toLowerCase();

            var exists = false;
            for (var j = 0, k = uniqueTranslations.length; j < k; j++) {
                if (uniqueTranslations[j].toLowerCase() === normalizedTranslation) {
                    exists = true;
                    break;
                }
            }

            if (!exists) {
                uniqueTranslations.push(translations[i].data.trim());
                types.push(translations[i].type);
            }
        }

        if (uniqueTranslations.length === 0) {
            createNoty("warning", "center", i18n.t("stax.vocabulary.inbox.select-translations"));
            wrapper.busy = false;
            return;
        } else if (uniqueTranslations.length > window.inbox.getMaxSelection()) {
            wrapper.busy = false;
            return;
        }

        var word = parent.data("word");

        $.ajax({
            async: true,
            type: "POST",
            url: api.acceptWord,
            data: {
                id: parent.data("id"),
                word: word,
                translations: uniqueTranslations,
                types: types,
                offsetDate: window.user.offsetDate
            },

            success: function(result) {
                if (result === 0) {
                    inboxData[parent.data("position")] = undefined;
                    parent.remove();

                    var normalizedWord = word.trim().toLowerCase(),
                        normalizedTranslations = uniqueTranslations.map(function(value) {
                            return value.trim().toLowerCase();
                        });

                    if (staxData[normalizedWord] === undefined || staxData[normalizedWord] === null) {
                        staxData[normalizedWord] = normalizedTranslations;
                    } else {
                        for (var i = 0, l = normalizedTranslations.length; i < l; i++) {
                            if (staxData[normalizedWord].indexOf(normalizedTranslations[i]) === -1) {
                                staxData[normalizedWord].push(normalizedTranslations[i]);
                            }
                        }
                    }

                    //$("#stk-eqs-wsc-1").tipsy("hide");
                    window.stax.loadStax(api.loadStax, true, function() {
                        //$("#stk-eqs-wsc-1").trigger("mouseenter");
                        wrapper.busy = false;
                    }, undefined, true);
                } else {
                    createNoty("warning", "center", i18n.t("error-processing"));
                    wrapper.busy = false;
                }
            },

            error: function() {
                createNoty("error", "center", i18n.t("internal-error"));
                wrapper.busy = false;
            }
        });
    }

    function deleteData(wrapper, element) {
        if (typeof wrapper.busy === "boolean" && wrapper.busy) {
            setTimeout(function() {
                deleteData(wrapper, element);
            }, 100);
            return;
        }

        wrapper.busy = true;

        var parent = $(element).parent("li");

        $.ajax({
            async: true,
            type: "POST",
            url: api.deleteWord,
            data: {
                id: parent.data("id"),
                word: parent.data("word")
            },

            success: function() {
                inboxData[parent.data("position")] = undefined;
                parent.remove();
                wrapper.busy = false;
            },

            error: function() {
                createNoty("error", "center", i18n.t("internal-error"));
                wrapper.busy = false;
            }
        });
    }

    function playData(wrapper, element) {
        window.sm.play($(element).parent("li").data("word"), 1, {
            autoplay: true,
            destroyable: false
        });
    }

    window.stax.loadStax(api.loadStax, true);

    window.inbox.setMaxSelection(3);
    window.inbox.loadInbox(
        api.loadInbox,
        function(result) {
            staxData = result.dataInStax;

            for (var i = result.inbox.length - 1; i >= 0; i--) {
                insertInboxData(result.inbox[i], false, false);
            }

            window.inbox.showInbox(inboxData);
        },
        {
            searchFunction: searchData,
            acceptPath: ".wn-dll-ins",
            acceptFunction: acceptData,
            deletePath: ".wn-dll-md",
            deleteFunction: deleteData,
            playPath: ".wn-dll-pl",
            playFunction: playData
        });

    window.lists.getUserLists(window.lists.types.vocabulary);

    window.stax.loadGraphs(api.loadGraphs);
});
