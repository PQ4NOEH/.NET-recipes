window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var cacheTypes = ["Role", "Instance", "Altea"],
        clearingCache = false;

    function clearCache() {
        if (clearingCache) {
            return;
        }

        clearingCache = true;
        window.mpd.block();

        var cacheCheckboxes = [],
            cachesToClear = Object.create({}),
            anyCacheToClear = false;

        cacheTypes.forEach(function(element) {
            var cacheName = "altea-cache-" + element.toLowerCase();
            var checkbox = document.getElementById(cacheName);
            cacheCheckboxes.push(checkbox);
            cachesToClear[element] = checkbox.checked;
            if (checkbox.checked) {
                anyCacheToClear = true;
            }
        });

        if (!anyCacheToClear) {
            clearingCache = false;
            window.mpd.unblock();
            return;
        }

        var clearButton = this;
        clearButton.disabled = true;

        cacheCheckboxes.forEach(function(element) {
            element.disabled = true;
        });

        $.ajax({
            async: true,
            type: "POST",
            url: "/Cache/Clear",
            data: { cacheTypes: cachesToClear },

            success: function() {
                window.location.reload();
            },

            error: function() {
                createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                    callback: {
                        afterClose: function() {
                            clearingCache = false;
                            clearButton.disabled = false;
                            cacheCheckboxes.forEach(function(element) {
                                element.disabled = false;
                            });
                            window.mpd.unblock();
                        }
                    }
                });
            }
        });
    }

    function showClearCacheDialog() {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var mpdContent = window.mpd.create(
            "clear-cache",
            "450px",
            "Clear cache",
            { closebtn: true }
        );

        var content = document.createElement("div");
        content.className = "clearfix";
        content.style.textAlign = "center";
        content.style.fontSize = "1.6em";
        content.style.lineHeight = "1em";
        content.style.marginBottom = "0.5em";
        mpdContent.appendChild(content);

        cacheTypes.forEach(function(element) {
            var div = document.createElement("div");
            div.style.width = "33.33%";
            div.style.float = "left";
            content.appendChild(div);

            var cacheName = "altea-cache-" + element.toLowerCase();
            var checkbox = document.createElement("input");
            checkbox.id = cacheName;
            checkbox.className = "input mini";
            checkbox.type = "checkbox";
            checkbox.style.verticalAlign = "middle";
            div.appendChild(checkbox);

            var checkLabel = document.createElement("label");
            checkLabel.className = "option";
            checkLabel.htmlFor = cacheName;
            checkLabel.style.verticalAlign = "middle";
            div.appendChild(checkLabel);

            var label = document.createElement("label");
            label.htmlFor = cacheName;
            label.innerText = element;
            label.style.paddingLeft = "5px";
            label.style.verticalAlign = "middle";
            div.appendChild(label);
        });

        var clear = document.createElement("button");
        clear.id = "altea-cache-clear";
        clear.className = "input input-or";
        clear.innerText = "Clear cache";
        mpdContent.appendChild(clear);

        $(clear).on("click", clearCache);

        window.mpd.show();
    }

    $(document.getElementById("nav_clear_cache")).on("click", function(e) {
        e.preventDefault();
        e.stopPropagation();
        showClearCacheDialog();
    });
});
