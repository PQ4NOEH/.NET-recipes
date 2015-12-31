/* 
 * Context.js
 * Copyright Jacob Kelley
 * MIT License
 */

var context = context || (function () {
    "use strict";

    var options = {
        fadeSpeed: 100,
        filter: function ($obj) {
            // Modify $obj, Do not return
        },
        above: "auto",
        preventDoubleContext: true,
        compress: false,
        positionX: 0,
        positionY: 0
    };

    function initialize(opts) {
        options = $.extend({}, options, opts);

        function closePopups(e) {
            var dropdowns = $(".dropdown-context.dropdown-context-opened");
            
            if (dropdowns.length !== 0) {
                e.preventDefault();
                dropdowns
                    .removeClass("dropdown-context-opened")
                    .fadeOut(options.fadeSpeed, function () {
                        $(".dropdown-context").css({ display: "" }).find(".drop-left").removeClass("drop-left");
                    });
            }
        }
        $(document).on("click", "html", closePopups);
        $(window).on("keyup", function(e) {
            if (e.which === 27) {
                closePopups();
            }
        });
        if (options.preventDoubleContext) {
            $(document).on("contextmenu", ".dropdown-context", function (e) {
                e.preventDefault();
            });
        }
        $(document).on("mouseenter", ".dropdown-submenu", function () {
            var $sub = $(this).find(".dropdown-context-sub:first"),
                subWidth = $sub.width(),
                subLeft = $sub.offset().left,
                collision = (subWidth + subLeft) > window.innerWidth;
            if (collision) {
                $sub.addClass("drop-left");
            }
        });

    }

    function updateOptions(opts) {
        options = $.extend({}, options, opts);
    }

    var getId = (function (id) {
        return function () {
            return ++id;
        };
    })(0);

    function buildMenu(data, id, subMenu) {
        var subClass = (subMenu) ? " dropdown-context-sub" : "",
            compressed = options.compress ? " compressed-context" : "",
            $menu = $("<ul class=\"dropdown-menu dropdown-context" + subClass + compressed + "\" id=\"dropdown-" + id + "\"></ul>");
        var i = 0, linkTarget = "";
        for (i; i < data.length; i++) {
            if (typeof data[i].divider !== "undefined") {
                $menu.append("<li class=\"divider\"></li>");
            } else if (typeof data[i].header !== "undefined") {
                $menu.append("<li class=\"nav-header\">" + data[i].header + "</li>");
            } else {
                if (typeof data[i].href == "undefined") {
                    data[i].href = "#";
                }
                if (typeof data[i].target !== "undefined") {
                    linkTarget = " target=\"" + data[i].target + "\"";
                }
                var $sub;
                if (typeof data[i].subMenu !== "undefined") {
                    $sub = ("<li class=\"dropdown-submenu\"><a tabindex=\"-1\" href=\"" + data[i].href + "\">" + data[i].text + "</a></li>");
                } else {
                    $sub = $("<li><a tabindex=\"-1\" href=\"" + data[i].href + "\"" + linkTarget + ">" + data[i].text + "</a></li>");
                }
                if (typeof data[i].action !== "undefined") {
                    var actionId = "event-" + getId(),
                        eventAction = data[i].action;
                    $sub.find("a").attr("id", actionId);
                    $("#" + actionId).addClass("context-event");
                    (function(aid, eaction) {
                        $(document).on("click", "#" + aid, function (e) { eaction(e, $menu[0].context.element) });
                    })(actionId, eventAction);
                }
                $menu.append($sub);
                if (typeof data[i].subMenu !== "undefined") {
                    var subMenuData = buildMenu(data[i].subMenu, id, true);
                    $menu.find("li:last").append(subMenuData);
                }
            }
            if (typeof options.filter === "function") {
                options.filter($menu.find("li:last"));
            }
        }
        return $menu;
    }

    function addContext(selector, data) {
        var id = getId(),
            $menu = buildMenu(data, id);

        $("body").append($menu);

        $(document).on("contextmenu", selector, function (e) {
            e.preventDefault();
            e.stopPropagation();

            $(".dropdown-context:not(.dropdown-context-sub)").hide();

            var position = this.getBoundingClientRect(),
                $dd = $("#dropdown-" + id);

            $dd[0].context = { element: this };
            $dd.addClass("dropdown-context-opened");

            if (typeof options.above == "boolean" && options.above) {
                $dd.addClass("dropdown-context-up").css({
                    top: e.pageY - 20 - $dd.height() + options.positionY,
                    left: e.pageX - 13 + options.positionX
                }).fadeIn(options.fadeSpeed);
            } else if (typeof options.above === "string" && options.above === "auto") {
                $dd.removeClass("dropdown-context-up");
                var autoH = $dd.height() + 12;
                if ((e.pageY + autoH) > $("html").height()) {
                    $dd.addClass("dropdown-context-up").css({
                        top: e.pageY - 20 - autoH,
                        left: e.pageX - 3 + options.positionX
                    }).fadeIn(options.fadeSpeed);
                } else {
                    $dd.css({
                        top: position.top + this.offsetHeight + options.positionY, //e.pageY + 10,
                        left: position.left - 20 + options.positionX  //e.pageX - 13
                    }).fadeIn(options.fadeSpeed);
                }
            } else {
                $dd.css({
                    top: position.top + this.offsetHeight + options.positionY, //e.pageY + 10,
                    left: position.left - 20 + options.positionX //e.pageX - 13
                }).fadeIn(options.fadeSpeed);
            }
        });
    }

    function destroyContext(selector) {
        $(document).off("contextmenu", selector).off("click", ".context-event");
    }

    return {
        init: initialize,
        settings: updateOptions,
        attach: addContext,
        destroy: destroyContext
    };
})();