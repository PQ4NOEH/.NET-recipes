window.ALTEA_PROMISES.push(function (window, document, $, undefined) {
    "use strict";

    // TODO: improve URI compression with https://github.com/antirez/smaz
    
    // ReSharper disable InconsistentNaming
    var WISENET_STORAGE_GUID = "6d608bfe6114401d835008be9e61dc2aa4e4f01c52a94225865a93f125a98579",
        WISENET_INJECTED_ATTRIBUTE = "wdata-ae903d5fc3ead62fea882dc1b180753d",
        WISENET_ESCAPE_CHARS = { "=": "-", "/": "_" },
        WISENET_HOME_URL = "wisenet://home",
        WISENET_SEARCH_URL = window.model.defaultSearchUrl,
        WISENET_QUERY_KEY = "uri",
        WISELAB_ORIGIN = 4,
        WISETANK_ORIGIN = 1;
    // ReSharper restore InconsistentNaming

    var dom = Object.create({
        icons: document.getElementsByClassName("wn-icn"),
        back: document.getElementById("wn-prv"),
        forward: document.getElementById("wn-nxt"),
        home: document.getElementById("wn-hme"),
        refresh: document.getElementById("wn-rfs"),

        wisetank: document.getElementById("wn-wtk"),
        wisepod: document.getElementById("wn-wsp"),

        godBar: document.getElementById("wn-gbr"),
        reportButton: document.getElementById("wn-reb"),
        iframe: document.getElementById("wn-ct-c"),
        iframeContent: null
    });

    var api = Object.create({
        mainUri: "/WiseNet#",
        proxyUri: "/WiseNet/Load/0?uri=",
        getArticle: "/WiseNet/Articles/Get",
        createArticle: "/WiseNet/Articles/Create",
        reportError: "/WiseNet/Error"
    });

    (function() {
        // Compute GodBar padding
        var computedStyle = window.getComputedStyle(dom.reportButton);
        dom.godBar.style.paddingRight = (parseInt(computedStyle.width) + (2 * parseInt(computedStyle.right)) - 1) + "px";
    })();

    var actualUri = null,
        isWiseNetUri = false,
        isWiseNetHome = false,
        loading = false,
        triggeredLoad = false,
        reference = -1;

    window.wiselab.setup({
        container: dom.iframe,
        frame: true,
        live: true,
        confirmFinish: true,
        canSearch: true,
        hasDoneButtons: true,
        wisepod: dom.wisepod,
        wisepodClass: "wn-dsb"
    });

    //#region Storage
    var actualPosition, maxPosition, uriList;

    function initStorage(uri) {
        if (typeof (window.Storage) === "undefined") {
            actualPosition = -1;
            maxPosition = -1;
            uriList = [];
        } else {
            if (window.sessionStorage[WISENET_STORAGE_GUID] === undefined) {
                window.sessionStorage[WISENET_STORAGE_GUID] = JSON.stringify({
                    actualPosition: -1,
                    maxPosition: -1,
                    uriList: []
                });
            } else {
                var storage = JSON.parse(window.sessionStorage[WISENET_STORAGE_GUID]);
                var uris = storage.uriList;
                var uriPosition = -1;

                for (var i = 0, l = uris.length; i < l; i++) {
                    if (uri === uris[i]) {
                        uriPosition = i;
                        break;
                    }
                }

                if (uriPosition === -1) {
                    window.sessionStorage[WISENET_STORAGE_GUID] = JSON.stringify({
                        actualPosition: -1,
                        maxPosition: -1,
                        uriList: []
                    });
                } else {
                    storage.actualPosition = uriPosition;
                    window.sessionStorage[WISENET_STORAGE_GUID] = JSON.stringify(storage);
                }
            }
        }
    }

    function getActualPosition() {
        if (typeof (window.Storage) === "undefined") {
            return actualPosition;
        } else {
            return JSON.parse(window.sessionStorage[WISENET_STORAGE_GUID]).actualPosition;
        }
    }

    function setActualPosition(value) {
        if (typeof (window.Storage) === "undefined") {
            actualPosition = value;
        } else {
            var storage = JSON.parse(sessionStorage[WISENET_STORAGE_GUID]);
            storage.actualPosition = value;
            sessionStorage[WISENET_STORAGE_GUID] = JSON.stringify(storage);
        }
    }

    function getMaxPosition() {
        if (typeof (window.Storage) === "undefined") {
            return maxPosition;
        } else {
            return JSON.parse(sessionStorage[WISENET_STORAGE_GUID]).maxPosition;
        }
    }

    function setMaxPosition(value) {
        if (typeof (window.Storage) === "undefined") {
            maxPosition = value;
        } else {
            var storage = JSON.parse(sessionStorage[WISENET_STORAGE_GUID]);
            storage.maxPosition = value;
            sessionStorage[WISENET_STORAGE_GUID] = JSON.stringify(storage);
        }
    }

    function getUriList(position) {
        if (typeof (window.Storage) === "undefined") {
            return uriList[position];
        } else {
            return JSON.parse(sessionStorage[WISENET_STORAGE_GUID]).uriList[position];
        }
    }

    function setUriList(position, value) {
        if (typeof (window.Storage) === "undefined") {
            uriList[position] = value;
        } else {
            var storage = JSON.parse(sessionStorage[WISENET_STORAGE_GUID]);
            storage.uriList[position] = value;
            sessionStorage[WISENET_STORAGE_GUID] = JSON.stringify(storage);
        }
    }

//#endregion Storage

    //#region URI
    function validateUri(uri) {
        uri = new URI(uri);
        if (uri.scheme == null) {
            uri = new URI("http://" + uri.toString());
        }

        if (uri.scheme === "wisenet" || ((uri.scheme === "http" || uri.scheme === "https") &&
            /^([a-z0-9\-\.]+\.)?[a-z0-9\-\.]+\.[a-z0-9]+$/i.test(uri.authority))) {
            return uri.toString();
        } else {
            return false;
        }
    }

    function removeHash(uri) {
        var newUri = new URI(uri);

        if (validateUri(newUri.toString())) {
            newUri =
                newUri.scheme + "://" + newUri.authority +
                (newUri.authority[newUri.authority.length] !== "/" && (newUri.path === null || newUri.path[0]) !== "/" ? "/" : "") +
                (newUri.path !== null ? encodeURIComponent(newUri.path) : "") +
                (newUri.query !== null ? "?" + newUri.query : "");

            return newUri;
        } else {
            return uri;
        }
    }

    // ReSharper disable InconsistentNaming
    function base64_encode_hash(uri) {
        var newUri = new URI(uri),
            uriHash = null;

        if (validateUri(newUri.toString())) {
            if (newUri.fragment !== null) {
                uriHash = newUri.fragment;
            }

            newUri =
                newUri.scheme + "://" + newUri.authority +
                (newUri.authority[newUri.authority.length] !== "/" && (newUri.path === null || newUri.path[0]) !== "/" ? "/" : "") +
                (newUri.path !== null ? newUri.path : "") +
                (newUri.query !== null ? "?" + newUri.query : "");
            return base64_encode(newUri, WISENET_ESCAPE_CHARS) + (uriHash !== null ? "#" + uriHash : "");
        } else {
            return base64_encode(uri, WISENET_ESCAPE_CHARS);
        }
    }

    function base64_decode_hash(uri) {
        var uriData = uri.split("#");

        return base64_decode(uriData[0], WISENET_ESCAPE_CHARS) + (uriData.length === 2 ? "#" + uriData[1] : "");
    }

    function getUriFromQuery(query) {
        var queryParams = query.split("&");
        for (var i = 0, l = queryParams.length; i < l; i++) {
            var keyValue = queryParams[i].split("=");
            if (keyValue[0] === WISENET_QUERY_KEY && keyValue.length === 2) {
                return base64_decode_hash(keyValue[1]);
            }
        }

        return null;
    }

// ReSharper restore InconsistentNaming
    //#endregion

    function setLoading(status) {
        if (typeof status !== "boolean" || status) {
            loading = true;
            window.wiselab.stop(false);
            dom.iframe.style.cursor = "progress";

            for (var i = 0, l = dom.icons.length; i < l; i++) {
                dom.icons[i].classList.add("wn-dsb");
                dom.icons[i].classList.remove("wn-slc");
            }

            dom.reportButton.disabled = true;
        } else {
            loading = false;
            dom.iframe.style.cursor = "";

            var position = getActualPosition();
            if (position === 0) {
                dom.back.classList.add("wn-dsb");
            } else {
                dom.back.classList.remove("wn-dsb");
            }

            if (position === getMaxPosition()) {
                dom.forward.classList.add("wn-dsb");
            } else {
                dom.forward.classList.remove("wn-dsb");
            }

            dom.reportButton.disabled = isWiseNetUri;

            if (isWiseNetHome) {
                dom.home.classList.add("wn-dsb");
                dom.refresh.classList.add("wn-dsb");
            } else {
                dom.home.classList.remove("wn-dsb");
                dom.refresh.classList.remove("wn-dsb");
            }

            if (window.model.hasWisetank && dom.wisetank !== null) {
                if (isWiseNetUri) {
                    dom.wisetank.classList.add("wn-dsb");
                } else {
                    dom.wisetank.classList.remove("wn-dsb");
                }
            }
        }
    }

    function loadUri(uri) {
        var navUri = validateUri(uri);
        if (navUri === false) {
            // ReSharper disable once InconsistentNaming
            navUri = new URI(WISENET_SEARCH_URL.replace("{0}", encodeURIComponent(uri)));
            navUri = navUri.toString();
        }

        setLoading(true);
        dom.iframe.src = api.proxyUri + base64_encode_hash(navUri);
    }

    function changeUri(uri) {
        actualUri = uri;

        reference = null;
        setUriList(getActualPosition(), actualUri);

        var finalUri = new URI(actualUri);
        if (finalUri.scheme.trim().toLowerCase() === "wisenet") {
            var testUri = actualUri;
            if (testUri[testUri.length - 1] === "/") {
                testUri = testUri.substring(0, testUri.length - 1);
            }

            switch (testUri) {
            case WISENET_HOME_URL:
                dom.godBar.value = "";
                window.history.replaceState(null, null, api.mainUri);
                isWiseNetHome = true;
                break;

            default:
                isWiseNetHome = false;
                break;
            }

            isWiseNetUri = true;
            return;
        } else {
            dom.godBar.value = actualUri;
            window.history.replaceState(null, null, api.mainUri + base64_encode(actualUri, WISENET_ESCAPE_CHARS));

            finalUri.fragment = null;
            var finalUriString = finalUri.toString();

            isWiseNetUri = false;
            isWiseNetHome = false;

            $.ajax({
                async: true,
                type: "POST",
                url: api.getArticle,
                data: { uri: finalUriString },

                success: function(result) {
                    var startCallback = null;

                    reference = result;

                    if (reference === -1) {
                        startCallback = function(f, g) {
                            $.ajax({
                                async: true,
                                type: "POST",
                                url: api.createArticle,
                                data: {
                                    uri: finalUriString,
                                    offsetDate: window.user.offsetDate
                                },

                                success: function(res) {
                                    reference = res;
                                    f(reference);
                                    g();
                                }
                            });
                        };
                    }

                    window.wiselab.get(WISELAB_ORIGIN, result, function() {
                        (function x() {
                            if (loading)
                                setTimeout(x, 100);
                            else {
                                window.wiselab.start();
                            }
                        })();
                    }, startCallback, null);
                },

                error: function() {
                    createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"]);
                }
            });
        }
    }

    function injectAttribute(node) {
        var attribute = document.createAttribute(WISENET_INJECTED_ATTRIBUTE);
        attribute.value = "true";
        node.attributes.setNamedItem(attribute);

        var attributeJs = document.createAttribute(WISENET_INJECTED_ATTRIBUTE + "js");
        attributeJs.value = "true";
        node.attributes.setNamedItem(attributeJs);
    }

    function loaded(uri) {
        dom.iframeContent = dom.iframe.contentDocument;

        var finalUri = new URI(uri);
        if (finalUri.scheme.trim().toLowerCase() !== "wisenet") {
            var $iframeContent = $(dom.iframeContent);
            $iframeContent.find("a[href]").each(function() {
                var attribute = this.attributes.getNamedItem(WISENET_INJECTED_ATTRIBUTE);
                if (attribute === undefined || attribute === null || attribute.value !== "true") {
                    var originalUri = this.href,
                        newUri = new URI(originalUri).resolve(new URI(actualUri)).toString();

                    this.href = api.proxyUri + base64_encode_hash(newUri);
                    injectAttribute(this);
                }
            });

            $iframeContent.find("form").each(function() {
                var attribute = this.attributes.getNamedItem(WISENET_INJECTED_ATTRIBUTE);
                if (attribute === undefined || attribute === null || attribute.value !== "true") {
                    var originalUri = this.action,
                        newUri;

                    if (typeof originalUri === "undefined" || $.trim(originalUri) === "") {
                        newUri = actualUri;
                    } else {
                        newUri = new URI(originalUri).resolve(new URI(actualUri)).toString();
                    }

                    this.action = api.proxyUri + base64_encode_hash(newUri);
                    injectAttribute(this);
                }
            });

            $iframeContent.find("a[href]").on("click", function() {
                if (this.href.length !== 0 && this.href[0] !== "#" && this.href.indexOf("javascript:") !== 0) {
                    var hrefUri = new URI(this.href);
                    if (hrefUri.fragment !== null) {
                        if (getUriFromQuery(hrefUri.query) !== actualUri) {
                            setLoading(true);
                        }
                    } else {
                        setLoading(true);
                    }
                }
            });

            $iframeContent.find("form").on("submit", function() {
                if (this.action.length === 0 || (this.action[0] !== "#" && this.action.indexOf("javascript:") !== 0)) {
                    setLoading(true);
                }
            });
        }

        if (!triggeredLoad) {
            setActualPosition(getActualPosition() + 1);
            setMaxPosition(getActualPosition());
        }

        triggeredLoad = false;

        changeUri(uri);
        setLoading(false);
    }

    /* First URL load */
    var hash = window.location.hash.substr(1);
    if (hash.length === 0) {
        initStorage();
        loadUri(WISENET_HOME_URL);
    } else {
        var url = base64_decode(hash, WISENET_ESCAPE_CHARS);
        initStorage(url);

        if (getActualPosition() !== -1) {
            triggeredLoad = true;
        }

        loadUri(url);
    }

    /* GodBar URL load */
    $(dom.godBar).on("keyup", function(e) {
        if (e.which === 13) { // RETURN
            var navUri = dom.godBar.value;

            if (navUri.trim().length !== 0) {
                loadUri(navUri);
            }
        } else if (e.which === 27) { // ESC
            dom.godBar.value = getUriList(getActualPosition());
        }
    });

    $(dom.iframe).on("load", function() {
        loaded(getUriFromQuery(new URI(dom.iframe.contentWindow.location.href).query));
        //var queryParams = new URI(dom.iframe.contentWindow.location.href).query.split("&");
        //for (var i = 0, l = queryParams.length; i < l; i++) {
        //    var keyValue = queryParams[i].split("=");
        //    if (keyValue[0] === WISENET_QUERY_KEY && keyValue.length === 2) {
        //        loaded(base64_decode_hash(keyValue[1]));
        //        break;
        //    }
        //}
    });

    /* Button events */
    $(dom.back).on("click", function() {
        if (this.classList.contains("wn-dsb") || getActualPosition() <= 0) {
            return;
        }

        setLoading(true);
        setActualPosition(getActualPosition() - 1);
        triggeredLoad = true;
        dom.iframe.src = api.proxyUri + base64_encode_hash(getUriList(getActualPosition()));
    });

    $(dom.forward).on("click", function() {
        if (this.classList.contains("wn-dsb") || getActualPosition() >= getMaxPosition()) {
            return;
        }

        setLoading(true);
        setActualPosition(getActualPosition() + 1);
        triggeredLoad = true;
        dom.iframe.src = api.proxyUri + base64_encode_hash(getUriList(getActualPosition()));
    });

    $(dom.home).on("click", function() {
        if (this.classList.contains("wn-dsb")) {
            return;
        }

        loadUri(WISENET_HOME_URL);
    });

    $(dom.refresh).on("click", function() {
        if (this.classList.contains("wn-dsb")) {
            return;
        }

        setLoading(true);
        triggeredLoad = true;
        dom.iframe.src = api.proxyUri + base64_encode_hash(getUriList(getActualPosition()));
    });

    if (window.model.hasWisetank && dom.wisetank !== null) {
        window.wisetank.setButton(dom.wisetank, "wn-dsb", "wn-slc");

        function getFavicon() {
            var favicon = null;

            var faviconNode = dom.iframeContent.querySelector("link[rel=\"icon\"]");
            if (faviconNode !== undefined && faviconNode !== null) {
                favicon = faviconNode.getAttribute("href");
            }

            if (favicon === undefined || favicon === null) {
                faviconNode = dom.iframeContent.querySelector("link[rel=\"shortcut icon\"]");
                if (faviconNode !== undefined && faviconNode !== null) {
                    favicon = faviconNode.getAttribute("href");
                }
            }

            if (favicon === undefined || favicon === null) {
                return null;
            } else if (faviconNode === undefined || faviconNode === null) {
                return null;
            } else {
                var attribute = faviconNode.attributes.getNamedItem(WISENET_INJECTED_ATTRIBUTE);
                if (attribute === undefined || attribute === null || attribute.value !== "true") {
                    return favicon;
                } else {
                    var uriSplit = favicon.split("?");
                    if (uriSplit.length < 2) {
                        return null;
                    } else {
                        var keyValue = uriSplit[1].split("=");
                        if (keyValue[0] === "uri" && keyValue.length === 2) {
                            favicon = base64_decode_hash(keyValue[1]);
                            return favicon;
                        } else {
                            return null;
                        }
                    }
                }
            }
        }

        function getSource() {
            var source = null;

            var sourceNode = dom.iframeContent.querySelector("meta[property=\"og:site_name\"]");
            if (sourceNode !== undefined && sourceNode !== null) {
                source = sourceNode.getAttribute("content");
            }

            if (source === undefined || source === null) {
                sourceNode = dom.iframeContent.querySelector("meta[name=\"dc.publisher\"]");
                if (sourceNode !== undefined && sourceNode !== null) {
                    source = sourceNode.getAttribute("content");
                }
            }

            if (source === undefined || source === null) {
                source = new URI(actualUri).authority;
            }

            if (source === undefined || source === null) {
                return null;
            }

            return source.trim();
        }

        function getTitle() {
            var title = null;

            var titleNode = dom.iframeContent.querySelector("meta[property=\"og:title\"]");
            if (titleNode !== undefined && titleNode !== null) {
                title = titleNode.getAttribute("content");
            }

            if (title === undefined || title === null) {
                titleNode = dom.iframeContent.querySelector("meta[property=\"fb_title\"]");
                if (titleNode !== undefined && titleNode !== null) {
                    title = titleNode.getAttribute("content");
                }
            }

            if (title === undefined || title === null) {
                titleNode = dom.iframeContent.querySelector("meta[property=\"twitter:title\"]");
                if (titleNode !== undefined && titleNode !== null) {
                    title = titleNode.getAttribute("content");
                }
            }

            if (title === undefined || title === null) {
                titleNode = dom.iframeContent.querySelector("meta[itemprop=\"name\"]");
                if (titleNode !== undefined && titleNode !== null) {
                    title = titleNode.getAttribute("content");
                }
            }

            if (title === undefined || title === null) {
                title = dom.iframeContent.title;
            }

            if (title === undefined || title === null) {
                return null;
            }

            return title.trim();
        }

        function getDescription() {
            var description = null;

            var descriptionNode = dom.iframeContent.querySelector("meta[name=\"description\"]");
            if (descriptionNode !== undefined && descriptionNode !== null) {
                description = descriptionNode.getAttribute("content");
            }

            if (description === undefined || description === null) {
                descriptionNode = dom.iframeContent.querySelector("meta[property=\"og:description\"]");
                if (descriptionNode !== undefined && descriptionNode !== null) {
                    description = descriptionNode.getAttribute("content");
                }
            }

            if (description === undefined || description === null) {
                descriptionNode = dom.iframeContent.querySelector("meta[property=\"twitter:description\"]");
                if (descriptionNode !== undefined && descriptionNode !== null) {
                    description = descriptionNode.getAttribute("content");
                }
            }

            if (description === undefined || description === null) {
                descriptionNode = dom.iframeContent.querySelector("meta[itemprop=\"description\"]");
                if (descriptionNode !== undefined && descriptionNode !== null) {
                    description = descriptionNode.getAttribute("content");
                }
            }

            if (description === undefined || description === null) {
                return null;
            }

            return description.trim();
        }

        function getImage() {
            var image = null;

            var imageNode = dom.iframeContent.querySelector("meta[property=\"og:image\"]");
            if (imageNode !== undefined && imageNode !== null) {
                image = imageNode.getAttribute("content");
            }

            if (image === undefined || image === null) {
                imageNode = dom.iframeContent.querySelector("meta[property=\"twitter:description\"]");
                if (imageNode !== undefined && imageNode !== null) {
                    image = imageNode.getAttribute("content");
                }
            }

            if (image === undefined || image === null) {
                return null;
            }

            return image.trim();
        }

        $(dom.wisetank).on("click", function() {
            if (this.classList.contains("wn-dsb")) {
                return;
            }

            window.wisetank.addArticle(
                WISETANK_ORIGIN,
                getSource(),
                getFavicon(),
                getTitle(),
                actualUri,
                getDescription(),
                getImage()
            );
        });
    }

    /* Report event */
    $(dom.reportButton).on("click", function() {
        if (this.disabled || loading || !isWiseNetUri) {
            return false;
        }

        this.disabled = true;

        $.ajax({
            async: true,
            type: "POST",
            url: api.reportError,
            data: {
                uri: removeHash(actualUri)
            }
        });

        createNoty("information", "center", i18n.t("wisenet.reported"), 5000, ["click"]);

        return true;
    });

    /* Public class */
    window.wisenet = new (function() {
        this.load = function(uri) {
            loadUri(uri);
        };
    });
});
