window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        modules: document.getElementsByClassName("hh-mdl-s")
    });

    var api = Object.create({
        get: "/Stats"
    });

    var statusClasses = Object.create({
        0: "",
        1: " hh-mdl-stc-o",
        2: " hh-mdl-stc-w",
        3: " hh-mdl-stc-d"
    });

    function addStat(wrapper, module, stat) {
        var container = document.createElement("div");
        container.className = "hh-mdl-std";
        wrapper.appendChild(container);

        var name = document.createElement("div");
        name.className = "hh-mdl-stn";
        name.innerText = i18n.t("stats." + module + "." + stat.name);
        container.appendChild(name);

        var data = document.createElement("div");
        data.className = "hh-mdl-stc" + statusClasses[stat.status];
        data.innerText = stat.value;
        container.appendChild(data);
    }

    $.ajax({
        async: true,
        type: "POST",
        url: api.get,
        data: { modules: window.model.modules },
        success: function(result) {
            var module;

            for (var i = 0, l = result.length; i < l; i++) {
                module = null;

                for (var j = 0, k = dom.modules.length; j < k; j++) {
                    if (dom.modules[j].classList.contains("hh-mdl-" + result[i].name)) {
                        module = dom.modules[j];
                        break;
                    }
                }

                if (module === undefined || module === null) {
                    continue;
                }

                var wrapper = module.getElementsByClassName("hh-mdl-st")[0];

                for (j = 0, k = result[i].stats.length; j < k; j++) {
                    addStat(wrapper, result[i].name, result[i].stats[j]);
                }

                module.classList.remove("hh-mdl-sw");
                module.getElementsByClassName("app-spinner")[0].classList.remove("app-spinner-active");
            }

            for (i = 0, l = dom.modules.length; i < l; i++) {
                module = dom.modules[i];

                if (module.classList.contains("hh-mdl-sw")) {
                    module.classList.remove("hh-mdl-sw");
                    window.setInternalErrorMessage($(module.getElementsByClassName("hh-mdl-st-c")[0]), i18n.t("error"));
                }
            }
        },
        error: function() {
            for (var i = 0, l = dom.modules.length; i < l; i++) {
                window.setInternalErrorMessage($(dom.modules[i].getElementsByClassName("hh-mdl-st-c")[0]), i18n.t("error"));
            }
        }
    });
});
