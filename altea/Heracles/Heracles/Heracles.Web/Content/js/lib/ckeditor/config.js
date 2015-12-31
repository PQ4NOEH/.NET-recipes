/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    config.toolbar = [
        { name: "document", items: ["Save", "SavedVersions"] },
        { name: "clipboard", items: ["Undo", "Redo"] },
        { name: "basycstyles", items: ["Bold", "Italic", "Underline", "-", "RemoveFormat"] },
        { name: "paragraph", items: ["NumberedList", "BulletedList", "-", "Outdent", "Indent", "-", "JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock"] },
        { name: "styles", items: ["Format", "Font", "FontSize"] },
        { name: "colors", items: ["TextColor"] }
    ];

    config.resize_enabled = false;
    config.removePlugins = "elementspath,resize";
};
