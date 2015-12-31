window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        title: document.getElementById("wr-te-mtd"),
        titleInput: document.getElementById("wr-te-mti")
    });

    var title = window.model.id === undefined ? "" : window.model.title;
    dom.title.innerText = (title === "" ? i18n.t("wisereader.texteditor.untitled") : title);
    dom.titleInput.value = title;

    var editor = CKEDITOR.replace("wr-te", {
        language: 'en',
        height: "100%",
        on:
        {
            instanceReady: function() {
                saveButton(false);
            },

            save: function() {
                flags.saved = true;
                saveButton(false);
                save(false);
            },

            change: function() {
                if (editor.checkDirty() && !flags.changed) {
                    flags.saved = false;
                    flags.changed = true;

                    if (window.wiseboard.title.length !== 0) {
                        saveButton(true);
                    } else {
                        saveButton(false);
                    }
                }
            }
        }
    });
});
