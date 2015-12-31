(function (window, document, $, undefined) {
    "use strict";

    /* Modal Popup Dialog */
    function mpd() {
        var _this = this;

        var isCreated = false,
            isFirstShown = false,
            isShown = false,
            hasClosebtn = false,
            canHide = false,
            mpdId = null;

        var body = $(window.document.body);
        var mb, wp, $wpw, bx, $bx, xb, h1, tlw, $h1, cb;
        var buttons = [];
        var scback, hcback;

        this.created = function () {
            return isCreated ? mpdId : false;
        };

        this.shown = function() {
            if (isCreated !== false) {
                return isShown;
            } else {
                return false;
            }
        };

        var update = function () {
            if (isCreated !== false) {
                var x = function () {
                    cb.style.height = "";
                    cb.style.maxHeight = "";

                    var bodyHeight = body.height();
                    var paddings = parseInt($wpw.css("padding-top")) + parseInt($wpw.css("padding-bottom"));
                    var maxHeight = (bodyHeight - paddings) * 0.975;
                    var height = $bx.height();
                    var finalHeight = Math.min(height, maxHeight);

                    bx.style.maxHeight = maxHeight + "px";
                    cb.style.height = (finalHeight - $(h1).height()) + "px";

                    if (!isFirstShown) {
                        $(".mb-content").mCustomScrollbar({
                            axis: "y",
                            scrollbarPosition: "inside",
                            alwaysShowScrollbar: 0,
                            scrollButtons: { enable: true },
                            theme: "dark-3"
                        });

                        isFirstShown = true;
                    }
                };

                x();
            }
        };

        this.resize = function () {
            update();
        };

        function addButtons(btns) {
            if (btns instanceof Array) {
                var btw = document.createElement("div");
                btw.className = "mb-buttons";
                tlw.appendChild(btw);

                for (var i = 0, l = btns.length; i < l; i++) {
                    var button = document.createElement("button");
                    button.className = "mb-button input input-gr";
                    button.innerText = btns[i].text;
                    button.i = i;
                    btw.appendChild(button);
                    buttons.push([button, btns[i].action]);
                }
            }
        }

        this.create = function (id, width, title, parameters) {
            if (isCreated) {
                return false;
            }

            if (typeof id !== "string") {
                id = "";
            }

            if (typeof width !== "string") {
                width = "600px";
            }

            if (typeof title !== "string") {
                title = "";
            } else {
                title = title.trim();
            }

            if (parameters === undefined || parameters === null) {
                parameters = Object.create({});
            } else {
                parameters = Object.create(parameters);
            }

            if (typeof parameters.closebtn !== "boolean") {
                parameters.closebtn = true;
            }

            isCreated = true;
            isFirstShown = false;
            isShown = false;

            hasClosebtn = parameters.closebtn;
            canHide = (hasClosebtn ? hasClosebtn : true);
            mpdId = id;

            var bg = document.createElement("div");
            bg.className = "mb mb-bg";
            body.append(bg);

            if (typeof parameters.css === "object") {
                $(bg).css(parameters.css);
            }

            var wpw = document.createElement("div");
            $wpw = $(wpw);
            wpw.className = "mb mb-wpw";
            body.append(wpw);

            wp = document.createElement("div");
            wp.className = "mb mb-wrap mb-hidden";
            wpw.appendChild(wp);

            mb = $([ bg, wpw, wp ]);

            var bxw = document.createElement("div");
            bxw.className = "mb-bxw";
            wp.appendChild(bxw);

            bx = document.createElement("div");
            $bx = $(bx);
            bx.className = "mb-box";
            bx.style.width = width;
            bxw.appendChild(bx);

            tlw = document.createElement("div");
            tlw.className = "mb-title";
            bx.appendChild(tlw);

            h1 = document.createElement("h1");
            $h1 = $(h1);
            h1.innerText = title;
            h1.title = title;
            tlw.appendChild(h1);

            buttons = [];
            addButtons(parameters.buttons);

            xb = document.createElement("button");
            xb.className = "mb-close";
            xb.innerText = "×";
            tlw.appendChild(xb);

            if (!parameters.closebtn) {
                xb.className += " mb-inactive";
            }

            var cbw = document.createElement("div");
            cbw.className = "mb-contentw";
            bx.appendChild(cbw);

            cb = document.createElement("div");
            cb.className = "mb-content";
            cbw.appendChild(cb);

            if (typeof parameters.overflow === "boolean" && parameters.overflow) {
                cb.className += " mb-overflow";
            }

            $(tlw).on("click", ".mb-button", function () {
                if (!this.classList.contains("input-bl")) {
                    for (var i = 0, l = buttons.length; i < l; i++) {
                        buttons[i][0].classList.remove("input-bl", "input-nh");
                        buttons[i][0].classList.add("input-gr");
                    }

                    this.classList.add("input-bl", "input-nh");
                    this.classList.remove("input-gr");

                    buttons[this.i][1](cb);
                }
            });

            $(xb).on("click", this.hide);

            scback = parameters.showCallback;
            hcback = parameters.hideCallback;

            return cb;
        }

        this.show = function () {
            if (!isCreated) {
                return false;
            }

            mb.one("webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend", function () {
                mb.off("webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend");

                isShown = true;
                if (typeof scback === "function") {
                    scback();
                }
            });

            mb.show(1, function () {
                update();
                mb.addClass("mb-visible");
                document.body.classList.add("mpd");
            });

            return true;
        };

        this.buttons = function() {
            return buttons.map(function(button) {
                return button[0];
            });
        };

        this.addButtons = function(btns) {
            addButtons(btns);
        }

        this.removeButtons = function() {
            for (var i = 0, l = buttons.length; i < l; i++) {
                buttons[i][0].parentNode.removeChild(buttons[i][0]);
                buttons[i] = undefined;
            }

            buttons.length = 0;
        };

        this.click = function(i) {
            $(tlw).find(".mb-button").eq(i).trigger("click");
        }

        var _hide = function (callback) {
            if (!isCreated || !canHide) {
                return false;
            }

            mb.one("webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend", function () {
                mb.off("webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend");

                mb.hide();
                isShown = false;
                if (typeof callback === "function") {
                    callback();
                }

                if (typeof hcback === "function") {
                    hcback();
                }
            });

            mb.removeClass("mb-visible");
            document.body.classList.remove("mpd");

            return true;
        };

        this.hide = function () {
            _hide();
        };

        this.block = function () {
            canHide = false;
            xb.classList.add("inactive");
        };

        this.unblock = function () {
            canHide = true;
            xb.classList.remove("inactive");
        };

        this.setTitle = function (title) {
            if (!isCreated || typeof title !== "string") {
                return false;
            }

            h1.text(title);
            return true;
        };

        this.destroy = function (hide) {
            if (!isCreated) {
                return false;
            }

            if (typeof hide !== "boolean") {
                hide = false;
            };

            if (hide) {
                _hide(function() {
                    mb.remove();
                });
            } else {
                mb.remove();
            }

            isCreated = false;
            return true;
        }

        $(window).on("resize", update);

        var hide = false;
        $(document).on("mousedown", function (e) {
            var target = $(e.target);
            hide = (target.hasClass("mb-bxw") || (e.target.ancestor(".mb").length === 0 && !target.hasClass("ui-timepicker-wrapper") && e.target.ancestor(".ui-timepicker-wrapper").length === 0))
                && (isCreated !== false && isShown && hasClosebtn && canHide);
        });

        $(document).on("mouseup", function () {
            if (hide) {
                _this.hide();
            }

            hide = false;
        });

        $(document).on("keyup", function(e) {
            if (e.which === 27 && isCreated !== false && isShown && hasClosebtn && canHide) {
                _this.hide();
            }

            hide = false;
        });
    }

    window.mpd = new mpd();
})(window, document, jQuery);
