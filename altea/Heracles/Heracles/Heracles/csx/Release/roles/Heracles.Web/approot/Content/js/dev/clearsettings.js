window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var settingsTypes = [
        { id: 0, name: "AppId" },
        { id: 1, name: "AppName" },
        { id: 2, name: "AppSettings" },
        { id: 3, name: "InstanceSettings" }
    ];

    var clearingSettings = false;

    function clearSettings() {
        if (clearingSettings) {
            return;
        }

        clearingSettings = true;
        window.mpd.block();

        var settingsCheckboxes = [],
            settingsToClear = [],
            anySettingsToClear = false;

        settingsTypes.forEach(function(element) {
            var settingsName = "altea-setting-type-" + element.id;
            var checkbox = document.getElementById(settingsName);
            settingsCheckboxes.push(checkbox);

            if (checkbox.checked) {
                settingsToClear.push(element.id);
                anySettingsToClear = true;
            }
        });

        if (!anySettingsToClear) {
            clearingSettings = false;
            window.mpd.unblock();
            return;
        }

        var clearButton = this;
        clearButton.disabled = true;

        settingsCheckboxes.forEach(function(element) {
            element.disabled = true;
        });

        $.ajax({
            async: true,
            type: "POST",
            url: "/Settings/Clear",
            data: { settings: settingsToClear },

            success: function() {
                window.location.reload();
            },

            error: function() {
                createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                    callback: {
                        afterClose: function() {
                            clearingSettings = false;
                            clearButton.disabled = false;
                            settingsCheckboxes.forEach(function(element) {
                                element.disabled = false;
                            });
                            window.mpd.unblock();
                        }
                    }
                });
            }
        });
    }

    function showClearSettingsDialog() {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var mpdContent = window.mpd.create(
            "clear-settings",
            "450px",
            "Clear settings",
            { closebtn: true }
        );

        var content = document.createElement("div");
        content.className = "clearfix";
        content.style.textAlign = "center";
        content.style.fontSize = "1.6em";
        content.style.lineHeight = "1em";
        content.style.marginBottom = "0.5em";
        mpdContent.appendChild(content);

        settingsTypes.forEach(function(element) {
            var div = document.createElement("div");
            div.style.width = "50%";
            div.style.float = "left";
            div.style.lineHeight = "1.5em";
            content.appendChild(div);

            var checkbox = document.createElement("input");
            checkbox.id = "altea-setting-type-" + element.id;
            checkbox.className = "input mini";
            checkbox.type = "checkbox";
            checkbox.dataset.id = element.id;
            checkbox.style.verticalAlign = "middle";
            div.appendChild(checkbox);

            var checkLabel = document.createElement("label");
            checkLabel.className = "option";
            checkLabel.htmlFor = "altea-setting-type-" + element.id;
            checkLabel.style.verticalAlign = "middle";
            div.appendChild(checkLabel);

            var label = document.createElement("label");
            label.htmlFor = "altea-setting-type-" + element.id;
            label.innerText = element.name;
            label.style.paddingLeft = "5px";
            label.style.verticalAlign = "middle";
            div.appendChild(label);
        });

        var clear = document.createElement("button");
        clear.id = "altea-settings-clear";
        clear.className = "input input-or";
        clear.innerText = "Clear settings";
        mpdContent.appendChild(clear);

        $(clear).on("click", clearSettings);

        window.mpd.show();
    }

    $(document.getElementById("nav_clear_settings")).on("click", function(e) {
        e.preventDefault();
        e.stopPropagation();
        showClearSettingsDialog();
    });
});
