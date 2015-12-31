window.ALTEA_PROMISES.push(function (window, document, $, undefined) {
    "use strict";

    // TODO: Dialog shows input. Dev types key, sets scope and term and cache key is loaded and showed in JSON format.

    var jsonPrinter = {
        replacer: function (match, pIndent, pKey, pVal, pEnd) {
            var key = "<span class=\"json-key\">";
            var val = "<span class=\"json-value\">";
            var str = "<span class=\"json-string\">";
            var r = pIndent || "";
            if (pKey)
                r = r + key + pKey.replace(/[": ]/g, "") + "</span>: ";
            if (pVal)
                r = r + (pVal[0] === "\"" ? str : val) + pVal + "</span>";
            return r + (pEnd || "");
        },
        prettyPrint: function (obj) {
            var jsonLine = /^( *)("[\w]+": )?("[^"]*"|[\w.+-]*)?([,[{])?$/mg;
            return JSON.stringify(obj, null, 3)
                .replace(/&/g, "&amp;").replace(/\\"/g, "&quot;")
                .replace(/</g, "&lt;").replace(/>/g, "&gt;")
                .replace(jsonLine, jsonPrinter.replacer);
        }
    };

    function showCacheDialog() {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var mpdContent = window.mpd.create(
            "cache",
            "850px",
            "Cache",
            { closebtn: true }
        );

        var content = document.createElement("div");
        mpdContent.appendChild(content);

        var key = document.createElement("input");
        key.className = "input";
        content.appendChild(key);

        var scope = document.createElement("select");
        scope.className = "input";
        content.appendChild(scope);

        var term = document.createElement("select");
        term.className = "input";
        content.appendChild(term);

        var global = document.createElement("input");
        global.className = "input";
        content.appendChild(global);

        var view = document.createElement("button");
        view.id = "altea-cache-clear";
        view.className = "input input-or";
        view.innerText = "View";
        content.appendChild(view);

        $(view).on("click", function() {
            
        });

        window.mpd.show();
    }

    $(document.getElementById("nav_cache")).on("click", function (e) {
        e.preventDefault();
        e.stopPropagation();
        showCacheDialog();
    });
});
