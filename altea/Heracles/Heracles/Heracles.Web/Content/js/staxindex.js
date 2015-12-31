window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        inbox: $(document.getElementById("stk-inb-ctn")),
        inboxSpinner: $(document.getElementById("stk-inb-spn")),
        inboxSearchForm: $(document.getElementById("stk-inb-sf")),
        inboxSearchInput: $(document.getElementById("stk-inb-st")),
        inboxSearchSubmitWrapper: $(document.getElementById("stk-inb-sbw")),
        inboxSearchSubmit: $(document.getElementById("stk-inb-sb")),
        inboxDataWrapper: $(document.getElementById("stk-inb-d")),
        inboxDataNoContents: $(document.getElementById("stk-inb-nd")),
        inboxDataContentsWrapper: $(document.getElementById("stk-inb-dw")),
        inboxDataContents: $(document.getElementById("stk-inb-dc"))
    });

    var maxSelection = 0;

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

    window.inbox = Object.create({
        setMaxSelection: function(max) {
            maxSelection = max;
        },

        getMaxSelection: function() {
            return maxSelection;
        },

        loadInbox: function(url, callback, functions) {
            if (functions === undefined || functions === null) {
                functions = Object.create(null);
            }

            dom.inboxSpinner.addClass("app-spinner-active");
            dom.inboxSearchForm.addClass("disabled");
            dom.inboxSearchInput.prop("disabled", true);
            dom.inboxSearchSubmit.prop("disabled", true);

            $.ajax({
                async: true,
                type: "POST",
                url: url,
                dataType: "json",

                success: function(result) {
                    if (typeof callback === "function") {
                        callback(result);
                    }

                    dom.inboxDataContentsWrapper.mCustomScrollbar({
                        axis: "y",
                        scrollbarPosition: "inside",
                        alwaysShowScrollbar: 2,
                        scrollButtons: { enable: true },
                        theme: "dark-3"
                    });

                    dom.inboxSearchForm.removeClass("disabled");
                    dom.inboxSearchInput.prop("disabled", false);
                    dom.inboxSearchSubmit.prop("disabled", false);
                    dom.inboxSpinner.removeClass("app-spinner-active");

                    dom.inboxDataContents.find(".wn-dll-ds").each(function() {
                        setScrollHeight(this);
                    });

                    if (typeof functions.searchFunction === "function") {
                        var searching = false;

                        dom.inboxSearchForm.on("submit", function(e) {
                            e.preventDefault();
                            e.stopPropagation();

                            if (!searching) {
                                searching = true;
                                functions.searchFunction(dom.inboxSearchForm, dom.inboxSearchInput, function() {
                                    searching = false;
                                });
                            }
                        });

                        dom.inboxSearchSubmitWrapper.on("click", function(e) {
                            e.preventDefault();
                            e.stopPropagation();
                            dom.inboxSearchForm.trigger("submit");
                        });
                    }

                    if (typeof functions.acceptFunction === "function") {
                        dom.inboxDataContents.on("click", functions.acceptPath, function(e) {
                            e.preventDefault();
                            functions.acceptFunction(dom.inbox[0], this);
                        });
                    }

                    if (typeof functions.deleteFunction === "function") {
                        dom.inboxDataContents.on("dblclick", functions.deletePath, function(e) {
                            e.preventDefault(this);
                            functions.deleteFunction(dom.inbox[0], this);
                        });
                    }

                    if (typeof functions.playFunction === "function") {
                        dom.inboxDataContents.on("click", functions.playPath, function(e) {
                            e.preventDefault(this);
                            functions.playFunction(dom.inbox[0], this);
                        });
                    }
                },

                error: function() {
                    window.setInternalErrorMessage(dom.inbox);
                }
            });
        },

        showInbox: function(data, scroll) {
            if (data.length === 0) {
                dom.inboxDataNoContents.addClass("stk-inb-nd-act");
            } else {
                dom.inboxDataNoContents.removeClass("stk-inb-nd-act");

                for (var i = 0, l = data.length; i < l; i++) {
                    data[i].element.detach();
                    dom.inboxDataContents.append(data[i].element);

                    data[i].element.find(".wn-dll-ds").each(function() {
                        setScrollHeight(this);
                    });
                }
            }
        },

        prependToInbox: function(data, scroll) {
            dom.inboxDataNoContents.removeClass("stk-inb-nd-act");
            dom.inboxDataContents.prepend(data.element);

            data.element.find(".wn-dll-ds").each(function() {
                setScrollHeight(this);
            });

            if (typeof scroll === "boolean" && scroll) {
                dom.inboxDataContentsWrapper.mCustomScrollbar("scrollTo", "top");
            }
        }
    });

    function toggleSelectData(element) {
        if (element.hasClass("done")) {
            return;
        }

        var select = !element.hasClass("selected");
        var li = element.parents("li");
        var word = element.text();

        var elements = li.find(".wn-dll-sd").filter(function() {
            return (this.innerText === word);
        });

        if (select) {
            if (li.data("selected") >= maxSelection) {
                shake(element[0], undefined, 1, 150);
                return;
            }

            elements.each(function() {
                if (!this.hasWidth) {
                    this.style.width = window.getComputedStyle(this).width;
                    this.hasWidth = true;
                }
                this.classList.add("selected");
            });

            element.addClass("clicked");

            li.data("selected", li.data("selected") + 1);
        } else {
            elements.each(function() {
                this.classList.remove("selected", "clicked");
            });

            li.data("selected", li.data("selected") - 1);
        }
    }

    dom.inboxDataContents.on("click", ".wn-dll-sd", function() {
        toggleSelectData($(this));
    });

    dom.inboxDataContents.on("click", ".wn-dll-ds", function(e) {
        if (this.classList.contains("scrollHeight") && e.target === this && e.offsetX < 16) {
            if (this.classList.contains("noOverflow")) {
                this.classList.remove("noOverflow");
                $(this).transition({ height: "1.25em" }, 200);
            } else {
                this.classList.add("noOverflow");
                $(this).transition({ height: this.scrollHeight }, 200);
            }
        }
    });

    $(window).on("resize", function() {
        dom.inboxDataContents.find(".wn-dll-ds").each(function() {
            setScrollHeight(this);
        });
    });
});
