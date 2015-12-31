(function (window, document, $) {
    "use strict";

    var pagination = function (options) {
        // ReSharper disable once InconsistentNaming
        var _this = this;

        if (options === undefined
                || options === null
                || typeof options.pages !== "string"
                || typeof options.pagesContainer !== "string"
                || typeof options.pagesWrapper !== "string"
                || typeof options.paginationContainer !== "string") {
            return false;
        }

        // ReSharper disable InconsistentNaming
        var TRANSITION_EVENT = "webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend";
        // ReSharper restore InconsistentNaming

        var defaultOptions = {
            firstPage: 0,
            pageNames: [],
            disabledPages: [],
            pageScrolls: [],
            showCallbacks: [],
            afterShowCallbacks: [],
            hideCallbacks: [],
            queryPage: true,
            hidden: false,
            scrollParameters: {
                axis: "y",
                scrollbarPosition: "inside",
                alwaysShowScrollbar: 2,
                scrollButtons: { enable: true },
                theme: "dark-3",
                live: false
            },
            pageScrollParameters: []
        };

        Object.extend(defaultOptions, options);

        options.titlePage = $(options.titlePage);
        options.pages = $(options.pages);
        options.pagesContainer = $(options.pagesContainer);
        options.pagesWrapper = $(options.pagesWrapper);
        options.paginationContainer = $(options.paginationContainer);

        var numPages = options.pageNames.length,
            pagePercentage = (Math.floor(1000000000000 / numPages) / 10000000000).toFixed(10);

        var actualPage = 0;

        var scrollsCreated = [];
        var paused = false;

        options.pagesWrapper.css("width", (numPages * 100) + "%");
        options.pages.each(function () {
            this.style.width = pagePercentage + "%";
        });

        options.paginationContainer.empty();
        if (options.hidden) {
            options.paginationContainer.css("visibility", "hidden");
        }

        for (var i = 0, l = options.pages.length; i < l; i++) {
            var span = document.createElement("span");
            span.className = "pg-p";

            if (options.disabledPages[i]) {
                options.pages[i].classList.add("pg-in");
                span.className += " inactive";
            }
            span.innerText = options.pageNames[i] || (i + 1);
            span.dataset.pgNumpage = i;

            options.paginationContainer.append(span);
        }



        var createScrolls = function (page) {
            if (options.pageScrolls[page] !== undefined && !scrollsCreated[page]) {
                var scrollParameters = options.scrollParameters;

                if (options.pageScrollParameters[page] !== undefined) {
                    Object.extend(options.pageScrollParameters[page], scrollParameters, true);
                }

                pageScrolls[page].mCustomScrollbar(scrollParameters);
                scrollsCreated[page] = true;
            }
        }

        var deleteScrolls = function (page) {
            var del = function (p) {
                if (options.pageScrolls[p] !== undefined && scrollsCreated[p]) {
                    pageScrolls[p].mCustomScrollbar("destroy");
                    scrollsCreated[p] = false;
                }
            }

            if (typeof page === "number") {
                del(page);
            } else {
                for (var i = 0, l = options.pageScrolls.length; i < l; i++) {
                    del(i);
                }
            }
        }

        this.setPageScrolls = function (page, scrolls, append) {
            if (typeof append !== "boolean") {
                append = false;
            }

            deleteScrolls(page);

            if (append) {
                if (options.pageScrolls[page] === undefined) {
                    pageScrolls[page] = $();
                }

                pageScrolls[page] = pageScrolls[page].add($(scrolls));
            } else {
                pageScrolls[page] = $(scrolls);
            }
        }

        this.enablePage = function (pageNum, className) {
            var page = options.paginationContainer.find(".pg-p[data-pg-numpage=\"" + pageNum + "\"]");
            page.removeClass("inactive");
            if (options.pages[pageNum] !== undefined) {
                options.pages[pageNum].classList.remove("pg-in");
            }
            if (typeof className === "string") {
                page.removeClass(className);
            }
        }

        this.enableAllPages = function (excludedPages, className) {
            if (typeof excludedPages === "string") {
                options.paginationContainer
                    .find(".pg-g")
                    .filter(function() {
                        return this.classList.contains(excludedPages);
                    })
                    .removeClass("inactive")
                    .removeClass(excludedPages);
            } else {
                excludedPages = excludedPages || [];

                for (var i = 0; i < numPages; i++) {
                    if (excludedPages.indexOf(excludedPages) !== -1) {
                        var pages = options.paginationContainer.find(".pg-g[data-pg-numpage=\"" + i + "\"]");
                        pages.removeClass("inactive");
                        if (options.pages[i] !== undefined) {
                            options.pages[i].classList.remove("pg-in");
                        }
                        if (typeof className === "string") {
                            pages.removeClass(className);
                        }
                    }
                }
            }
        }

        this.disablePage = function (pageNum, className) {
            var page = options.paginationContainer.find(".pg-p[data-pg-numpage=\"" + pageNum + "\"]");
            page.addClass("inactive");
            if (typeof className === "string") {
                page.addClass(className);
            }
            if (options.pages[pageNum] !== undefined) {
                options.pages[pageNum].classList.add("pg-in");
            }
            scrollsCreated[page] = false;
        }

        this.disableAllPages = function (excludedPages, className) {
            excludedPages = excludedPages || [];

            for (var i = 0; i < numPages; i++) {
                if (excludedPages.indexOf(excludedPages) !== -1) {
                    var pages = options.paginationContainer.find(".pg-g[data-pg-numpage=\"" + i + "\"]");
                    pages.addClass("inactive");
                    if (options.pages[i] !== undefined) {
                        options.pages[i].classList.add("pg-in");
                    }
                    if (typeof className === "string") {
                        pages.addClass(className);
                    }
                }
            }
        }

        this.isEnabled = function (pageNum) {
            return options.paginationContainer.find(".pg-g[data-pg-numpage=\"" + pageNum + "\"]").hasClass("inactive");
        }

        this.goToPage = function (page, animate, replaceState, setState) {
            if (paused) {
                return;
            }

            if (typeof animate !== "boolean") {
                animate = true;
            }

            if (typeof replaceState !== "boolean") {
                replaceState = true;
            }

            if (typeof setState !== "boolean") {
                setState = true;
            }

            var activePage = options.paginationContainer
                .find(".pg-p.pg-ap")
                .data("pg-numpage");

            function transitionLogic() {
                actualPage = page;

                if (setState && options.queryPage) {
                    var func = replaceState
                        ? window.history.replaceState.bind(window.history)
                        : window.history.pushState.bind(window.history);

                    func(page, null, window.location.pathname + "#" + page);
                }

                if (!scrollsCreated[page]) {
                    createScrolls(page);
                }

                if (typeof options.afterShowCallbacks[page] === "function" && options.afterShowCallbacks[page](activePage)) {
                    options.afterShowCallbacks[page](activePage);
                }

                options.paginationContainer
                    .find(".pg-p")
                    .removeClass("pg-ap")
                    .eq(page).addClass("pg-ap");
            }

            if (typeof options.hideCallbacks[activePage] === "function") {
                options.hideCallbacks[activePage](page);
            }

            if (typeof options.showCallbacks[page] === "function") {
                options.showCallbacks[page](page);
            }


            if (animate) {
                options.pagesContainer.one(TRANSITION_EVENT, function (e) {
                    if (options.pagesWrapper.is(e.target)) {
                        options.pagesContainer.off(TRANSITION_EVENT);

                        options.pagesWrapper.css({
                            "left": "-" + (page * 100) + "%",
                            "transition": "",
                            "transform": ""
                        });

                        transitionLogic();
                    }
                });

                options.pagesWrapper.css({
                    "left": "",
                    "transform": "translateX(-" + (pagePercentage * actualPage) + "%)"
                });

                setTimeout(function() {
                    options.pagesWrapper.css({
                        "left": "",
                        "transition": "transform 0.25s linear",
                        "transform": "translateX(-" + (pagePercentage * page) + "%)"
                    });
                }, 0);
            } else {
                options.pagesWrapper.css({
                    "left": "-" + (page * 100) + "%",
                    "transition": "",
                    "transform": ""
                });

                transitionLogic();
            }
        }

        this.autoGoTo = function (excludedPages) {
            excludedPages = excludedPages || [];

            var page = options.paginationContainer.find(".pg-p:not(.inactive)").filter(function() {
                return excludedPages.indexOf($(this).data("pg-numpage")) === -1;
            }).first().data("pg-numpage");

            if (window.location.hash !== undefined && window.location.hash !== "") {
                var searchPage = parseInt(window.location.hash.substr(1));

                if (!isNaN(searchPage) && !options.paginationContainer.find(".pg-p[data-pg-numpage=\"" + searchPage + "\"]").hasClass("inactive")) {
                    page = searchPage;
                }
            }

            if (options.firstPage !== page) {
                this.goToPage(page, false, true, true);
            }
        }

        this.goToFirst = function (excludedPages, animate) {
            excludedPages = excludedPages || [];

            if (typeof animate !== "boolean") {
                animate = true;
            }

            var firstPage = options.paginationContainer.find(".pg-p:not(.inactive)").filter(function() {
                return excludedPages.indexOf($(this).data("pg-numpage")) === -1;
            }).map(function() {
                return $(this).data("pg-numpage");
            }).sort(function(a, b) {
                if (a === options.firstPage || a < b) {
                    return -1;
                } else {
                    return 1;
                }
            })[0];

            this.goToPage(firstPage, animate, true, true);
        }

        this.actualPage = function () {
            return options.paginationContainer.find(".pg-g.pg-ap").data("pg-numpage");
        }

        this.show = function () {
            return options.paginationContainer.css("visibility", "visible");
        }

        this.hide = function () {
            return options.paginationContainer.css("visibility", "hidden");
        }

        this.pause = function() {
            paused = true;
            options.titlePage.addClass("pg-psd");
            options.pagesContainer.addClass("pg-psd");
            options.paginationContainer.addClass("pg-psd");
        }

        this.resume = function() {
            paused = false;
            options.titlePage.removeClass("pg-psd");
            options.pagesContainer.removeClass("pg-psd");
            options.paginationContainer.removeClass("pg-psd");
        }

        this.destroy = function () {
            options.paginationContainer.empty();
        }

        this.goToPage(options.firstPage, false, false, false);

        options.paginationContainer.on("click", ".pg-p", function() {
            if (!this.classList.contains("pg-ap") && !this.classList.contains("inactive")) {
                _this.goToPage($(this).data("pg-numpage"), true, true, true);
            }
        });

        $(window).on("popstate", function (event) {
            if (event.state !== null) {
                _this.goToPage(event.state, true, false);
            }
        });

        return true;
    };

    window.Pagination = pagination;
})(window, document, jQuery);
