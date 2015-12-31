window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var emptyGuid = "00000000-0000-0000-0000-000000000000";

    var dom = Object.create({
        wisereaderSpinner: $(document.getElementById("wr-spn")),
        wisereaderFilesSpinner: $(document.getElementById("wr-fls-spn")),
        createButton: $(document.getElementById("wr-fld-btn-nw")),
        uploadButton: $(document.getElementById("wr-fld-btn-up")),
        mpdId: "wbup",
        mpdCreateId: "wbct",
        mpdEditId: "wbed",
        mpdDeleteId: "wbdl",
        mpdContent: null,
        folders: document.getElementById("wr-fld-ctn-dc"),
        files: document.getElementById("wr-fls-d")
    });

    var api = Object.create({
        getFolders: "/WiseReader/Folders/Get",
        getFiles: "/WiseReader/Folders/Files",
        createFolder: "/WiseReader/Folders/Create",
        editFolder: "/WiseReader/Folders/Edit",
        deleteFolder: "/WiseReader/Folders/Delete",
        textEditor: "/WiseReader/TextEditor/",
        uploadFile: "/WiseReader/Upload"
    });

    var fileTypes = Object.create({
        1: "wr-txt",
        2: "wr-pdf",
        5: "wr-doc",
        6: "wr-doc"
    });

    context.init({
        fadeSpeed: 100,
        filter: function($obj) {},
        above: false,
        preventDoubleContext: true,
        compress: false
    });

    var today = new Date();
    today.setHours(0);
    today.setMinutes(0);
    today.setSeconds(0);
    today.setMilliseconds(0);

    var thisWeek = new Date(today.getTime());
    thisWeek.setDate(thisWeek.getDate() - thisWeek.getDay());
    thisWeek.setDate(thisWeek.getDate() - (today.getDay() === 0 ? 6 : -1));

    var thisMonth = new Date(today.getTime());
    thisMonth.setDate(1);

    var rootFolderId = null,
        actualFolder = null,
        actualFolderFiles = [],
        processingFolderFiles = [],
        processingPendingFiles = [],
        folderIds = null,
        rootFolder = null,
        flatFolders = null;

    var allowReloads = true, reloadTimeout = null;

    function drawFolder(folder, level, element) {
        var li = document.createElement("li");
        li.className = "wr-fld-ctn-fld wr-fld-ctn-fld-cld";
        element.appendChild(li);

        var titleDiv = document.createElement("div");
        titleDiv.className = "wr-fld-ctn-tdv";
        li.appendChild(titleDiv);

        if (level > 0) {
            var arrow = document.createElement("span");
            arrow.className = "wr-fld-ctn-arr";
            titleDiv.appendChild(arrow);
        }

        var name = document.createElement("span");
        name.className = "wr-fld-ctn-tit";
        name.innerText = folder.name;
        name.dataset.id = folder.id.replace(/-/g, "");
        titleDiv.appendChild(name);

        if (level === 0) {
            name.className += " wr-fld-ctn-mfld";
        } else {
            name.className += " wr-fld-ctn-sfld";
        }

        var childrenDiv = document.createElement("div");
        childrenDiv.className = "wr-fld-ctn-cdv";
        li.appendChild(childrenDiv);

        if (folder.children === null || folder.children.length === 0) {
            li.className += " wr-fld-ctn-fld-nch";
            return null;
        } else {
            return childrenDiv;
        }
    }

    function recursiveDrawFolders(folders, parentFolder, level, parent) {
        var ul = document.createElement("ul");
        ul.className = "wr-fld-ctn-lst";
        parent.appendChild(ul);

        folders.sort(function(a, b) {
            if (a.position < b.position) {
                return -1;
            } else {
                return 1;
            }
        });

        for (var i = 0, l = folders.length; i < l; i++) {
            folders[i].parent = parentFolder;
            flatFolders.push(folders[i]);

            var childrenDiv = drawFolder(folders[i], level, ul);

            if (childrenDiv !== null) {
                recursiveDrawFolders(folders[i].children, folders[i], level + 1, childrenDiv);
            }
        }
    }

    function drawFolders() {
        flatFolders = [];
        recursiveDrawFolders([rootFolder], null, 0, dom.folders);

        var mainLi = dom.folders.getElementsByClassName("wr-fld-ctn-fld")[0];
        mainLi.classList.remove("wr-fld-ctn-fld-cld");
        mainLi.classList.add("wr-fld-ctn-fld-opn");
    }

    function drawFileArea(headerName) {
        var area = document.createElement("div");
        area.className = "wr-fls-da";
        var areaHeader = document.createElement("div");
        areaHeader.className = "wr-fls-dah";
        areaHeader.innerText = headerName;
        area.appendChild(areaHeader);
        var areaContent = document.createElement("div");
        areaContent.className = "wr-fls-dac";
        area.appendChild(areaContent);

        return area;
    }

    function drawFileAreas() {
        var today = drawFileArea(i18n.t("Modified") + " " + i18n.t("today"));
        dom.files.appendChild(today);

        var thisWeek = drawFileArea(i18n.t("Modified") + " " + i18n.t("thisWeek"));
        dom.files.appendChild(thisWeek);

        var thisMonth = drawFileArea(i18n.t("Modified") + " " + i18n.t("thisMonth"));
        dom.files.appendChild(thisMonth);

        var older = drawFileArea(i18n.t("Older"));
        dom.files.appendChild(older);

        return [
            [today, today.getElementsByClassName("wr-fls-dac")[0], false, false],
            [thisWeek, thisWeek.getElementsByClassName("wr-fls-dac")[0], false, true],
            [thisMonth, thisMonth.getElementsByClassName("wr-fls-dac")[0], false, true],
            [older, older.getElementsByClassName("wr-fls-dac")[0], false, true]
        ];
    }

    function drawFile(area, fileData) {
        var fileWrapper = document.createElement("div");
        fileWrapper.className = "wr-fls-daf";
        fileWrapper.dataset.id = fileData.id;
        area[1].appendChild(fileWrapper);

        var file = document.createElement("div");
        file.className = "wr-fls-dafc";
        fileWrapper.appendChild(file);

        var type = document.createElement("span");
        type.className = "wr-fls-daf-t " + fileTypes[fileData.type];
        file.appendChild(type);

        if (!fileData.processed || fileData.invalid) {
            var status = document.createElement("span");
            status.className = "wr-fls-daf-s";

            if (!fileData.processed) {
                var spinner = document.createElement("span");
                spinner.className = "app-spinner app-spinner-active";
                status.appendChild(spinner);
                fileWrapper.className += " wr-fls-dafp";
                file.className += " wr-fls-dafp";
            } else if (fileData.invalid) {
                var error = document.createElement("span");
                error.className = "app-error";
                status.appendChild(error);
                fileWrapper.className += " wr-fls-dafe";
                file.className += " wr-fls-dafe";
            }

            file.appendChild(status);
        }

        var name = document.createElement("span");
        name.className = "wr-fls-daf-n";
        name.innerText = fileData.name;
        file.appendChild(name);

        var date = document.createElement("span");
        date.className = "wr-fls-daf-d";
        date.innerText = moment(fileData.lastModifiedDate).format(area[3] ? "ll" : "LT");
        file.appendChild(date);

        var options = document.createElement("span");
        options.className = "wr-fls-daf-o";
        file.appendChild(options);

        return fileWrapper;
    }


    function getFileData(folder, callback, fallback) {
        $.ajax({
            async: true,
            type: "POST",
            url: api.getFiles,
            data: {
                folder: folder
            },
            dataType: "json",
            success: function(result) {
                if (callback != undefined && callback != null) {
                    callback(result);
                }
            },
            error: function() {
                if (fallback != undefined && fallback != null) {
                    fallback();
                }
            }
        });
    }

    function getFiles() {
        clearTimeout(reloadTimeout);

        dom.wisereaderFilesSpinner.addClass("app-spinner-active");
        while (dom.files.firstChild) {
            dom.files.removeChild(dom.files.firstChild);
        }

        var folder = actualFolder;
        getFileData(
            actualFolder,
            function(result) {
                actualFolderFiles.length = 0;
                processingFolderFiles.length = 0;

                if (result.length === 0) {
                    var messageDiv = document.createElement("div");
                    messageDiv.id = "wr-fls-d-nfm";
                    messageDiv.innerText = i18n.t(window.HTMLModElement.hasTextEditor ? "wisereader.nofiles" : "wisereader.nofilesnoeditor");
                    dom.files.appendChild(messageDiv);
                } else {
                    var areas = drawFileAreas();
                    var waitingProcessFile = false;

                    for (var i = 0, l = result.length; i < l; i++) {
                        actualFolderFiles.name = result[i].name.toLowerCase().trim();

                        var area;
                        var lastModifiedDate = new Date(result[i].lastModifiedDate);
                        if (lastModifiedDate >= today) {
                            area = areas[0];
                        } else if (lastModifiedDate >= thisWeek) {
                            area = areas[1];
                        } else if (lastModifiedDate >= thisMonth) {
                            area = areas[2];
                        } else {
                            area = areas[3];
                        }

                        var file = drawFile(area, result[i]);

                        if (!waitingProcessFile && !result[i].processed) {
                            processingFolderFiles.push(file);
                            waitingProcessFile = true;
                        }
                    }

                    var finalAreas = [];
                    for (var i = 0, l = areas.length; i < l; i++) {
                        if (areas[i][1].getElementsByClassName("wr-fls-daf").length === 0) {
                            dom.files.removeChild(areas[i][0]);
                        } else {
                            finalAreas.push(areas[i]);
                        }
                    }

                    if (finalAreas.length === 1 && finalAreas[0][2] === true) {
                        finalAreas[0][0].removeChild(finalAreas[0][0].getElementsByClassName("wr-fls-dah")[0]);
                    }

                    if (!waitingProcessFile) {
                        // TODO REFRESH
                        /*var reloadFunction = function() {
                            getFileData(folder, function() {
                                if (actualFolder === folder) {
                                    setTimeout(reloadFunction, 6000);
                                }
                            }, undefined);
                        }

                        reloadTimeout = setTimeout(reloadFunction, 6000);*/
                    }
                }

                dom.wisereaderSpinner.removeClass("app-spinner-active");
                dom.wisereaderFilesSpinner.removeClass("app-spinner-active");
            }, function() {
                window.setInternalErrorMessage($(dom.files));
                dom.wisereaderSpinner.removeClass("app-spinner-active");
                dom.wisereaderFilesSpinner.removeClass("app-spinner-active");
            });
    }

    function setFolder(folder) {
        if (folderIds.indexOf(folder) === -1) {
            actualFolder = rootFolderId;
        } else {
            actualFolder = folder;
        }

        var folderDom;
        $(".wr-fld-ctn-tit").each(function(k, v) {
            if (v.dataset.id === actualFolder) {
                folderDom = v;
                return false;
            }

            return true;
        });

        folderDom.classList.add("wr-fld-ctn-tit-act");

        var parentLi = $(folderDom).parents(".wr-fld-ctn-fld").eq(0);
        while ((parentLi = parentLi.parents(".wr-fld-ctn-fld").eq(0)).length !== 0) {
            parentLi.addClass("wr-fld-ctn-fld-opn").removeClass("wr-fld-ctn-fld-cld");
        }

        getFiles();
    }

    function toggleArrow(li, forceOpen) {
        if (forceOpen === undefined || typeof forceOpen !== "boolean") {
            forceOpen = false;
        }

        if (!li.classList.contains("wr-fld-ctn-fld-nch")) {
            var div = li.getElementsByClassName("wr-fld-ctn-cdv")[0];
            var ul = div.getElementsByClassName("wr-fld-ctn-lst")[0];
            div.style.height = ul.height;

            if (forceOpen) {
                li.classList.remove("wr-fld-ctn-fld-cld");
                li.classList.add("wr-fld-ctn-fld-opn");
            } else {
                li.classList.toggle("wr-fld-ctn-fld-cld");
                li.classList.toggle("wr-fld-ctn-fld-opn");
            }
        }
    }

    $(dom.folders).on("click", ".wr-fld-ctn-arr", function() {
        var li = $(this).parents(".wr-fld-ctn-fld")[0];
        toggleArrow(li);
    });

    $(dom.folders).on("click", ".wr-fld-ctn-tit", function() {
        if (!this.classList.contains("wr-fld-ctn-tit-act")) {
            dom.folders.getElementsByClassName("wr-fld-ctn-tit-act")[0].classList.remove("wr-fld-ctn-tit-act");

            if (this.dataset.id === rootFolderId) {
                window.history.pushState({ id: this.dataset.id }, null, "#");
            } else {
                window.history.pushState({ id: this.dataset.id }, null, "#" + this.dataset.id);
            }

            setFolder(this.dataset.id);
        }

        var li = $(this).parents(".wr-fld-ctn-fld")[0];
        toggleArrow(li, true);
    });

    $(dom.files).on("click", ".wr-fls-daf", function(e) {
        if (this.classList.contains("wr-fls-dafp")) {
            return;
        }

        if (e.target.className === "wr-fls-daf-o") {
            // OPEN MENU
        } else if (!this.classList.contains("wr-fls-dafe")) {
            allowReloads = false;
            window.location.href = "/WiseReader/View/" + this.dataset.id.replace(/-/g, "");
        }
    });


    function createFolderPopup(e, element) {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var mpdContent = window.mpd.create(
            dom.mpdCreateId,
            "400px",
            i18n.t("wisereader.createsubfolder"),
            { closebtn: true }
        );

        var folder;
        for (var i = 0, l = flatFolders.length; i < l; i++) {
            if (element.dataset.id === flatFolders[i].id.replace(/-/g, "")) {
                folder = flatFolders[i];
                break;
            }
        }

        var input = document.createElement("input");
        input.id = "wr-fld-cfp";
        input.className = "input";
        input.maxLength = 64;
        input.placeholder = i18n.t("wisereader.foldername"),
            mpdContent.appendChild(input);

        var buttonsDiv = document.createElement("div");
        buttonsDiv.id = "wr-fld-btns";
        mpdContent.appendChild(buttonsDiv);

        var saveButton = document.createElement("button");
        saveButton.id = "wr-fld-del";
        saveButton.className = "wr-fld-btn input input-bl";
        saveButton.innerText = i18n.t("actions.create");
        buttonsDiv.appendChild(saveButton);

        var cancelButton = document.createElement("button");
        cancelButton.id = "wr-fld-cls";
        cancelButton.className = "wr-fld-btn input input-gr";
        cancelButton.innerText = i18n.t("close");
        buttonsDiv.appendChild(cancelButton);

        $(input).on("keyup", function(e) {
            if (e.which === 13) {
                $(saveButton).trigger("click");
            }
        });

        $(saveButton).on("click", function() {
            window.mpd.block();
            input.disabled = true;
            saveButton.disabled = true;
            cancelButton.disabled = true;

            var name = input.value.trim();

            if (name.length > 64) {
                createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                    callback: {
                        afterClose: function() {
                            input.disabled = false;
                            saveButton.disabled = false;
                            cancelButton.disabled = false;
                            window.mpd.unblock();
                        }
                    }
                });
            } else {
                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.createFolder,
                    data: { parent: folder.id, name: name },
                    success: function(result) {
                        if (result !== emptyGuid) {
                            var li = element.ancestor("li", true);
                            var childrenUl;

                            var position;
                            if (folder.children === null) {
                                folder.children = [];
                                position = 1;

                                childrenUl = document.createElement("ul");
                                childrenUl.className = "wr-fld-ctn-lst";
                                li.getElementsByClassName("wr-fld-ctn-cdv")[0].appendChild(childrenUl);

                            } else {
                                position = folder.children.length + 1;
                                childrenUl = li.getElementsByClassName("wr-fld-ctn-lst")[0];
                            }

                            var childFolder = {
                                id: result,
                                name: name,
                                position: position,
                                children: null,
                                parent: folder
                            };

                            folder.children.push(childFolder);
                            folderIds.push(result);
                            flatFolders.push(childFolder);

                            li.classList.remove("wr-fld-ctn-fld-cld");
                            li.classList.add("wr-fld-ctn-fld-opn");
                            li.classList.remove("wr-fld-ctn-fld-nch");
                            drawFolder(childFolder, 1, childrenUl);

                            window.mpd.unblock();
                            window.mpd.hide();
                        } else {
                            createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                                callback: {
                                    afterClose: function() {
                                        input.disabled = false;
                                        saveButton.disabled = false;
                                        cancelButton.disabled = false;
                                        window.mpd.unblock();
                                    }
                                }
                            });
                        }
                    },

                    error: function() {
                        createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    input.disabled = false;
                                    saveButton.disabled = false;
                                    cancelButton.disabled = false;
                                    window.mpd.unblock();
                                }
                            }
                        });
                    }
                });
            }
        });

        $(cancelButton).on("click", function() {
            $(mpdContent).parents(".mb-box").eq(0).find(".mb-close").eq(0).trigger("click");
        });

        window.mpd.show();

        $(input).focus();
    }

    function editFolderPopup(e, element) {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var mpdContent = window.mpd.create(
            dom.mpdEditId,
            "400px",
            i18n.t("wisereader.editfolder"),
            { closebtn: true }
        );

        var folder;
        for (var i = 0, l = flatFolders.length; i < l; i++) {
            if (element.dataset.id === flatFolders[i].id.replace(/-/g, "")) {
                folder = flatFolders[i];
                break;
            }
        }

        var input = document.createElement("input");
        input.id = "wr-fld-efp";
        input.className = "input";
        input.maxLength = 64;
        input.placeholder = i18n.t("wisereader.foldername");
        input.value = folder.name;
        mpdContent.appendChild(input);

        var buttonsDiv = document.createElement("div");
        buttonsDiv.id = "wr-fld-btns";
        mpdContent.appendChild(buttonsDiv);

        var saveButton = document.createElement("button");
        saveButton.id = "wr-fld-del";
        saveButton.className = "wr-fld-btn input input-bl";
        saveButton.innerText = i18n.t("actions.save");
        saveButton.disabled = true;
        buttonsDiv.appendChild(saveButton);

        var cancelButton = document.createElement("button");
        cancelButton.id = "wr-fld-cls";
        cancelButton.className = "wr-fld-btn input input-gr";
        cancelButton.innerText = i18n.t("close");
        buttonsDiv.appendChild(cancelButton);

        $(input).on("input", function() {
            var disabled = input.value.trim() === "" || input.value.trim() === folder.name;
            if (!disabled) {
                for (var i = 0, l = folder.parent.children.length; i < l; i++) {
                    if (folder.parent.children[i].name.trim().toLowerCase() === input.value.trim().toLowerCase()) {
                        disabled = true;
                        break;
                    }
                }
            }

            saveButton.disabled = disabled;
        });

        $(input).on("keyup", function(e) {
            if (e.which === 13) {
                $(saveButton).trigger("click");
            }
        });

        $(saveButton).on("click", function() {
            window.mpd.block();
            input.disabled = true;
            saveButton.disabled = true;
            cancelButton.disabled = true;

            var name = input.value.trim();

            if (name.length > 64) {
                createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                    callback: {
                        afterClose: function() {
                            input.disabled = false;
                            saveButton.disabled = false;
                            cancelButton.disabled = false;
                            window.mpd.unblock();
                        }
                    }
                });
            } else {
                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.editFolder,
                    data: { folder: folder.id, name: name },
                    success: function(result) {
                        if (result === 0) {
                            element.innerText = name;
                            folder.name = name;

                            window.mpd.unblock();
                            window.mpd.hide();
                        } else {
                            createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                                callback: {
                                    afterClose: function() {
                                        input.disabled = false;
                                        saveButton.disabled = false;
                                        cancelButton.disabled = false;
                                        window.mpd.unblock();
                                    }
                                }
                            });
                        }
                    },

                    error: function() {
                        createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    input.disabled = false;
                                    saveButton.disabled = false;
                                    cancelButton.disabled = false;
                                    window.mpd.unblock();
                                }
                            }
                        });
                    }
                });
            }
        });

        $(cancelButton).on("click", function() {
            $(mpdContent).parents(".mb-box").eq(0).find(".mb-close").eq(0).trigger("click");
        });

        window.mpd.show();

        $(input).focus();
    }

    function deleteFolderPopup(e, element) {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var mpdContent = window.mpd.create(
            dom.mpdDeleteId,
            "400px",
            i18n.t("wisereader.deletefolder"),
            { closebtn: true }
        );

        var folder;
        for (var i = 0, l = flatFolders.length; i < l; i++) {
            if (element.dataset.id === flatFolders[i].id.replace(/-/g, "")) {
                folder = flatFolders[i];
                break;
            }
        }

        var p = document.createElement("p");
        p.className = "wr-fld-ppp";
        p.innerText = i18n.t("wisereader.confirmdelete");
        mpdContent.appendChild(p);

        var buttonsDiv = document.createElement("div");
        buttonsDiv.id = "wr-fld-btns";
        mpdContent.appendChild(buttonsDiv);

        var deleteButton = document.createElement("button");
        deleteButton.id = "wr-fld-del";
        deleteButton.className = "wr-fld-btn input input-re";
        deleteButton.innerText = i18n.t("actions.delete");
        buttonsDiv.appendChild(deleteButton);

        var cancelButton = document.createElement("button");
        cancelButton.id = "wr-fld-cls";
        cancelButton.className = "wr-fld-btn input input-gr";
        cancelButton.innerText = i18n.t("close");
        buttonsDiv.appendChild(cancelButton);

        $(deleteButton).on("click", function() {
            window.mpd.block();
            deleteButton.disabled = true;
            cancelButton.disabled = true;

            if (name.length > 64) {
                createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                    callback: {
                        afterClose: function() {
                            deleteButton.disabled = false;
                            cancelButton.disabled = false;
                            window.mpd.unblock();
                        }
                    }
                });
            } else {
                var id = folder.id;

                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.deleteFolder,
                    data: { folder: id },
                    success: function(result) {
                        if (result === 0) {
                            var parent = element.ancestor(".wr-fld-ctn-fld", true);
                            if (parent.getElementsByClassName("wr-fld-ctn-fld").length !== 0) {
                                $(document.getElementsByClassName("wr-fld-ctn-tit")[0]).trigger("click");
                            }

                            var fullParent = parent.ancestor("li", true);
                            parent.parentNode.removeChild(parent);

                            for (var i = 0, l = folder.parent.children.length; i < l; i++) {
                                if (folder.parent.children[i].id === folder.id) {
                                    folder.parent.children.splice(i, 1);
                                    break;
                                }
                            }

                            if (folder.parent.children === null || folder.parent.children.length === 0) {
                                fullParent.classList.remove("wr-fld-ctn-fld-opn");
                                fullParent.classList.add("wr-fld-ctn-fld-cld");
                                fullParent.classList.add("wr-fld-ctn-fld-nch");
                            }

                            for (var i = 0, l = folderIds.length; i < l; i++) {
                                if (folderIds[i] === id) {
                                    folderIds.splice(i, 1);
                                    break;
                                }
                            }

                            window.mpd.unblock();
                            window.mpd.hide();
                        } else {
                            createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                                callback: {
                                    afterClose: function() {
                                        deleteButton.disabled = false;
                                        cancelButton.disabled = false;
                                        window.mpd.unblock();
                                    }
                                }
                            });
                        }
                    },

                    error: function() {
                        createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    deleteButton.disabled = false;
                                    cancelButton.disabled = false;
                                    window.mpd.unblock();
                                }
                            }
                        });
                    }
                });
            }
        });

        $(cancelButton).on("click", function() {
            $(mpdContent).parents(".mb-box").eq(0).find(".mb-close").eq(0).trigger("click");
        });

        window.mpd.show();
    }


    $.ajax({
        async: true,
        type: "POST",
        url: api.getFolders,
        dataType: "json",

        success: function(result) {
            folderIds = [];

            for (var i = 0, l = result.folderIds.length; i < l; i++) {
                folderIds.push(result.folderIds[i].replace(/-/g, ""));
            }

            rootFolder = result.rootFolder;
            rootFolderId = rootFolder.id.replace(/-/g, "");

            drawFolders();

            context.attach(".wr-fld-ctn-mfld", [
                {
                    text: i18n.t("wisereader.createsubfolder"),
                    action: createFolderPopup
                }
            ]);

            context.attach(".wr-fld-ctn-sfld", [
                {
                    text: i18n.t("wisereader.createsubfolder"),
                    action: createFolderPopup
                },
                {
                    divider: true
                },
                {
                    text: i18n.t("wisereader.editfolder"),
                    action: editFolderPopup
                },
                {
                    text: i18n.t("wisereader.deletefolder"),
                    action: deleteFolderPopup
                }
            ]);

            var folder;
            if (window.location.hash.length <= 1) {
                folder = rootFolderId;
            } else {
                folder = window.location.hash.substring(1);

                if (folderIds.indexOf(folder) === -1) {
                    folder = rootFolderId;
                }
            }

            setFolder(folder);
        },
        error: function() {
            window.setInternalErrorMessage($("#ct-c"));
        }
    });

    function formatFileSize(bytes) {
        if (typeof bytes !== "number") {
            return "";
        }

        if (bytes >= 1073741824) {
            return (bytes / 1073741824).toFixed(2) + " GB";
        }

        if (bytes >= 1048576) {
            return (bytes / 1048576).toFixed(2) + " MB";
        }

        return (bytes / 1024).toFixed(2) + " KB";
    }

    function createUploadPopup() {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        dom.mpdContent = window.mpd.create(
            dom.mpdId,
            "600px",
            i18n.t("wisereader.upload.title"),
            { closebtn: true }
        );

        processingFolderFiles.length = 0;

        var messageDiv = document.createElement("div");
        messageDiv.id = "wr-fld-upl-msg";
        messageDiv.innerText = i18n.t("wisereader.upload.message");
        dom.mpdContent.appendChild(messageDiv);

        var uploadDiv = document.createElement("div");
        uploadDiv.id = "wr-fld-upl-flsw";
        dom.mpdContent.appendChild(uploadDiv);

        var uploadList = document.createElement("ul");
        uploadList.id = "wr-fld-upl-fls";
        uploadDiv.appendChild(uploadList);

        var $uploadDiv = $(uploadDiv);
        $uploadDiv.mCustomScrollbar({
            axis: "y",
            scrollbarPosition: "inside",
            alwaysShowScrollbar: 0,
            scrollButtons: { enable: true },
            theme: "dark-3"
        });

        var buttonsDiv = document.createElement("div");
        buttonsDiv.id = "wr-fld-upl-btns";
        dom.mpdContent.appendChild(buttonsDiv);

        var selectFileButton = document.createElement("button");
        selectFileButton.id = "wr-fld-upl-slc";
        selectFileButton.className = "input input-bl";
        buttonsDiv.appendChild(selectFileButton);

        var selectFileTextButton = document.createElement("span");
        selectFileTextButton.innerText = i18n.t("wisereader.upload.select");
        selectFileButton.appendChild(selectFileTextButton);

        var inputFile = document.createElement("input");
        inputFile.id = "wr-fld-upl-slc-inp";
        inputFile.type = "file";
        inputFile.multiple = true;
        selectFileButton.appendChild(inputFile);

        var cancelFileButton = document.createElement("button");
        cancelFileButton.id = "wr-fld-upl-cnl";
        cancelFileButton.className = "input input-gr";
        cancelFileButton.innerText = i18n.t("close");
        buttonsDiv.appendChild(cancelFileButton);

        $(cancelFileButton).on("click", function() {
            $(dom.mpdContent).parents(".mb-box").eq(0).find(".mb-close").eq(0).trigger("click");
        });

        var uploadingFiles = 0;

        $(inputFile).fileupload({
            type: "POST",
            url: api.uploadFile + "/" + actualFolder,
            dataType: "json",
            autoUpload: false,

            add: function(e, data) {
                selectFileTextButton.innerText = i18n.t("wisereader.upload.addmore");

                uploadDiv.className = "hf";
                var fileElement = document.createElement("li");
                fileElement.innerHTML =
                    "<div class=\"wr-fld-upl-flds wr-fld-upl-wrk\">" +
                    "<div class=\"wr-fld-upl-flcn\"><input class=\"wr-fld-upl-flck\" type=\"text\" value=\"0\"></div>" +
                    "<div class=\"wr-fld-upl-fldt\">" +
                    "<div class=\"wr-fld-upl-fldtf\">" +
                    "<span class=\"wr-fld-upl-fn\"></span>" +
                    "</div>" +
                    "<div class=\"wr-fld-upl-fldts\">" +
                    "<span class=\"wr-fld-upl-fs\"></span>" +
                    "<span class=\"wr-fld-upl-fsp\"></span>" +
                    "</div>" +
                    "</div>" +
                    "<div class=\"wr-fld-upl-flcn\">" +
                    "</div>" +
                    "</div>";

                var knobElement = $(fileElement.getElementsByClassName("wr-fld-upl-flck")[0]);

                knobElement.knob({
                    width: 58,
                    height: 58,
                    angleArc: 270,
                    angleOffset: -135,
                    bgColor: window.theme.colors.grey25,
                    fgColor: window.theme.colors.blue
                });

                fileElement.getElementsByClassName("wr-fld-upl-fn")[0].innerText = data.files[0].name;
                fileElement.getElementsByClassName("wr-fld-upl-fs")[0].innerText = formatFileSize(data.files[0].size);

                uploadList.appendChild(fileElement);
                window.mpd.resize();

                $uploadDiv.mCustomScrollbar("scrollTo", "bottom");

                data.context = $(fileElement);

                if (!/(\.|\/)(txt|pdf)$/i.test(data.files[0].name)) {
                    knobElement
                        .trigger("configure", { bgColor: window.theme.colors.red, fgColor: window.theme.colors.red })
                        .css("color", window.theme.colors.red);
                    var fsp = fileElement.getElementsByClassName("wr-fld-upl-fsp")[0];
                    fsp.className += " wr-fld-upl-ift";
                    fsp.innerText = i18n.t("wisereader.upload.invalidfiletype");
                } else if (data.files[0].size > 5242880) {
                    knobElement
                        .trigger("configure", { bgColor: window.theme.colors.red, fgColor: window.theme.colors.red })
                        .css("color", window.theme.colors.red);
                    var fsp = fileElement.getElementsByClassName("wr-fld-upl-fsp")[0];
                    fsp.className += " wr-fld-upl-ift";
                    fsp.innerText = i18n.t("wisereader.upload.filetoobig");
                } else {
                    var nameWithoutExtension = data.files[0].name.substring(0, data.files[0].name.lastIndexOf(".")).trim();
                    var normalizedName = nameWithoutExtension.toLowerCase();

                    if (nameWithoutExtension.length > 260) {
                        knobElement
                            .trigger("configure", { bgColor: window.theme.colors.red, fgColor: window.theme.colors.red })
                            .css("color", window.theme.colors.red);
                        var fsp = fileElement.getElementsByClassName("wr-fld-upl-fsp")[0];
                        fsp.className += " wr-fld-upl-ift";
                        fsp.innerText = i18n.t("wisereader.upload.filenametoolong");
                    } else if (actualFolderFiles.indexOf(normalizedName) !== -1 || processingFolderFiles.indexOf(normalizedName) !== -1) {
                        knobElement
                            .trigger("configure", { bgColor: window.theme.colors.red, fgColor: window.theme.colors.red })
                            .css("color", window.theme.colors.red);
                        var fsp = fileElement.getElementsByClassName("wr-fld-upl-fsp")[0];
                        fsp.className += " wr-fld-upl-ift";
                        fsp.innerText = i18n.t("wisereader.upload.duplicatename");
                    } else {
                        window.mpd.block();
                        cancelFileButton.innerText = i18n.t("wisereader.upload.uploading");
                        cancelFileButton.disabled = true;

                        uploadingFiles++;
                        processingFolderFiles.push(normalizedName);

                        data.submit();
                    }
                }
            },

            progress: function(e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                data.context.find(".wr-fld-upl-flck").val(progress).change();

                var fsp = data.context[0].getElementsByClassName("wr-fld-upl-fsp")[0];
                fsp.innerText = "";

                if (progress === 100) {
                    var flds = data.context[0].getElementsByClassName("wr-fld-upl-flds")[0];
                    flds.classList.remove("wr-fld-upl-wrk");

                } else {
                    var hoursRemaining = 0,
                        minutesRemaining = 0,
                        secondsRemaining = (data.total - data.loaded) * 8 / data.bitrate;

                    if (secondsRemaining > 60) {
                        minutesRemaining = parseInt(secondsRemaining / 60);
                        secondsRemaining = parseInt(secondsRemaining % 60);

                        if (minutesRemaining > 60) {
                            hoursRemaining = parseInt(minutesRemaining / 60);
                            minutesRemaining = parseInt(minutesRemaining % 60);
                        } else {
                            minutesRemaining = parseInt(minutesRemaining);
                        }
                    } else {
                        secondsRemaining = parseInt(secondsRemaining);
                    }

                    var text = "";
                    if (hoursRemaining > 0) {
                        text = hoursRemaining + " " + i18n.t("hours") + " ";
                    }

                    if (minutesRemaining > 0) {
                        text += minutesRemaining + " " + i18n.t("minutes") + " ";
                    }

                    if (secondsRemaining > 0) {
                        text += secondsRemaining + " " + i18n.t("seconds") + " ";
                    }

                    if (text.length === 0) {
                        text += "0 " + i18n.t("seconds") + " ";
                    }

                    fsp.innerText = text + i18n.t("remaining");
                }
            },

            done: function(e, data) {
                data.context.find(".wr-fld-upl-flck")
                    .trigger("configure", { bgColor: window.theme.colors.green, fgColor: window.theme.colors.green })
                    .css("color", window.theme.colors.green);

                var flds = data.context[0].getElementsByClassName("wr-fld-upl-flds")[0];
                flds.classList.remove("wr-fld-upl-wrk");
                flds.classList.add("wr-fld-upl-fns");

                uploadingFiles--;

                var nameWithoutExtension = data.files[0].name.substring(0, data.files[0].name.lastIndexOf(".")).trim();
                var normalizedName = nameWithoutExtension.toLowerCase();
                var indexOf = processingFolderFiles.indexOf(normalizedName);

                if (indexOf !== -1) {
                    processingFolderFiles.splice(indexOf, 1);
                }

                var fsp = data.context[0].getElementsByClassName("wr-fld-upl-fsp")[0];
                fsp.innerText = "";

                // TODO ADD FILE TO FOLDER LIST!!!!

                if (uploadingFiles === 0) {
                    window.mpd.unblock();
                    cancelFileButton.innerText = i18n.t("finished");
                    cancelFileButton.disabled = false;
                    $(cancelFileButton).on("click", function() {
                        getFiles();
                    });
                }
            },

            fail: function(e, data) {
                data.context.find(".wr-fld-upl-flck")
                    .trigger("configure", { bgColor: window.theme.colors.red, fgColor: window.theme.colors.red })
                    .css("color", window.theme.colors.red);

                var flds = data.context[0].getElementsByClassName("wr-fld-upl-flds")[0];
                flds.classList.remove("wr-fld-upl-wrk");
                flds.classList.add("wr-fld-upl-err");

                var fsp = data.context[0].getElementsByClassName("wr-fld-upl-fsp")[0];
                fsp.className += " wr-fld-upl-ift";
                fsp.innerText = i18n.t("wisereader.upload.unknownerror");

                uploadingFiles--;

                var nameWithoutExtension = data.files[0].name.substring(0, data.files[0].name.lastIndexOf(".")).trim();
                var normalizedName = nameWithoutExtension.toLowerCase();
                var indexOf = processingFolderFiles.indexOf(normalizedName);

                if (indexOf !== -1) {
                    processingFolderFiles.splice(indexOf, 1);
                }

                if (uploadingFiles === 0) {
                    window.mpd.unblock();
                    cancelFileButton.innerText = i18n.t("finished");
                    cancelFileButton.disabled = false;
                }
            }
        });
    }

    if (dom.createButton.length !== 0) {
        dom.createButton.on("click", function() {
            window.location.href = api.textEditor + "#" + actualFolder;
        });
    }

    dom.uploadButton.on("click", function() {
        createUploadPopup();
        window.mpd.show();
    });
});
