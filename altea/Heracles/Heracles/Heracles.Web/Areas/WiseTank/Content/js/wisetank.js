window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    // ReSharper disable once InconsistentNaming
    var WISETANK_ORIGIN = 3;

    var api = Object.create({
        getData: "/WiseTank/GetData",
        getStreamData: "/WiseTank/Streams/GetData",
        getGroupBoxData: "/WiseTank/Timelines/GetData",
        createStream: "/WiseTank/Streams/Create",
        deleteStream: "/WiseTank/Streams/Delete",
        renameStream: "/WiseTank/Streams/Rename",
        editStream: "/WiseTank/Streams/Rename",
        setAutoRefresh: "/WiseTank/Streams/SetRefreshRate",
        createBox: "/WiseTank/Streams/CreateBox",
        editStreamBoxWidth: "/WiseTank/Streams/EditBoxWidth",
        vote: "/WiseTank/Articles/Vote",
        karma: "/WiseTank/Articles/Karma",
        createGroup: "/WiseTank/Timelines/Create",
        createColumn: "/WiseTank/Timelines/AddColumn",
        editGroup: "/WiseTank/Timelines/Edit",
        editGroupBoxWidth: "/WiseTank/Timelines/EditGroupBoxWidth",
        getTimelineUsers: "/WiseTank/Timelines/GetUsers",
        addTimelineUser: "/WiseTank/Timelines/AddUser",
        editTimelineUser: "/WiseTank/Timelines/EditUser",
        articleImage: "/WiseTank/Articles/Image/",
        boxTypes: 6
    });

    var dom = Object.create({
        streamsIcon: document.getElementById("wt-str-lb-str"),
        streamsMainContainer: document.getElementById("wt-str"),
        spinner: document.getElementById("wt-str-spn"),
        streams: document.getElementById("wt-str-l"),
        newStream: document.getElementById("wt-str-ln"),
        newBox: document.getElementById("wt-str-sh-add"),
        renameStream: document.getElementById("wt-str-sh-etn"),
        refresh: document.getElementById("wt-str-sh-rfs"),
        autoRefresh: document.getElementById("wt-str-sh-atr"),
        autoRefreshHelper: document.getElementById("wt-str-sh-atr-tm"),
        autoRefreshSelector: document.getElementById("wt-str-sh-atr-lst"),
        boxSliderWrapper: document.getElementById("wt-str-sh-bwsw"),
        boxSlider: document.getElementById("wt-str-sh-bws"),
        boxesContainer: document.getElementById("wt-str-c"),
        noTabs: document.getElementById("wt-str-no-tab"),
        noBoxes: document.getElementById("wt-str-no-bxs"),
        boxes: document.getElementById("wt-str-bxs"),
        boxesWrapper: document.getElementById("wt-str-bxs-w"),

        groupsIcon: document.getElementById("wt-str-lb-grp"),
        groupsMainContainer: document.getElementById("wt-grp"),
        groupAreasContainer: document.getElementById("wt-grp-la"),
        groupAreasName: document.getElementById("wt-grp-ldan"),
        groupAreasArrow: document.getElementById("wt-grp-ldaarr"),
        groupAreas: document.getElementById("wt-grp-laars"),
        groupTimelinesContainer: document.getElementById("wt-grp-ls"),
        groupTimelinesAdd: document.getElementById("wt-grp-ln"),
        newGroup: document.getElementById("wt-grp-ln"),
        groupAddColumn: document.getElementById("wt-grp-sh-add"),
        editGroup: document.getElementById("wt-grp-sh-egr"),
        groupUsers: document.getElementById("wt-grp-sh-usr"),
        groupRefresh: document.getElementById("wt-grp-sh-rfs"),
        groupAutoRefresh: document.getElementById("wt-grp-sh-atr"),
        groupAutoRefreshHelper: document.getElementById("wt-grp-sh-atr-tm"),
        groupAutoRefreshSelector: document.getElementById("wt-grp-sh-atr-lst"),
        groupBoxSliderWrapper: document.getElementById("wt-grp-sh-bwsw"),
        groupBoxSlider: document.getElementById("wt-grp-sh-bws"),
        groupBoxes: document.getElementById("wt-grp-bxs"),
        groupBoxesWrapper: document.getElementById("wt-grp-bxs-w"),
        selectArea: document.getElementById("wt-grp-sl-are"),
        noTimelines: document.getElementById("wt-grp-no-tml"),
        selectTimeline: document.getElementById("wt-grp-sl-tml"),

        articlesBoxes: document.getElementsByClassName("wt-bxs"),
        articlesBoxesWrapper: document.getElementsByClassName("wt-bxs-w")
    });

    var allPermissions = [
        "listUsers", "inviteUsers", "readArticles", "writeArticles",
        "unmoderatedArticles", "voteArticles", "assignArticles", "approveArticles",
        "deleteArticles", "readComments", "writeComments", "writePrivateComments",
        "unmoderatedComments", "approveComments", "deleteComments", "createSubtimelines",
        "overrideOwnArticles"
    ];

    var areas = [],
        sortedAreas = null,
        actualArea = null,
        timelines = [],
        streams = [],
        actualStream = null,
        actualGroup = null,
        selectingStream = false,
        selectingGroup = false,
        refreshingStream = false,
        refreshingGroup = false,
        refreshInterval = -1,
        refreshGroupInterval = -1,
        refreshPending = 0,
        refreshGroupPending = 0,
        refreshTimeout = null,
        refreshGroupTimeout = null;

    var roles = [], ownerRoleId = 0;

    $(".wt-str-lb-ic")
        .each(function(k, v) {
            v.title = "<p class=\"wt-str-lb-ict\">" + v.title + "</p>";
        })
        .tipsy({
            gravity: "w",
            html: true
        });

    function resizeBoxes(element) {
        if (element === undefined || element === null) {
            element = dom.articlesBoxes;
        }

        function res(el) {
            var numBoxes = el.dataset.numBoxes;
            var width = el.offsetWidth * Math.max(1, numBoxes / el.dataset.width);

            el.getElementsByClassName("wt-bxs-w")[0].style.width = width + "px";
            $(el).mCustomScrollbar("update");
        }

        if (element instanceof HTMLCollection) {
            for (var i = 0, l = element.length; i < l; i++) {
                res(element[i]);
            }
        } else {
            res(element);
        }
    }

    function normalizeBoxes(element) {
        var boxes = element.getElementsByClassName("wt-str-bx"),
            numBoxes = boxes.length;

        var boxWidth = 100 / Math.max(numBoxes, element.dataset.width);
        for (var i = 0, l = numBoxes; i < l; i++) {
            boxes[i].style.width = boxWidth + "%";
        }

        resizeBoxes(element);
    }

    function disableStreamOptions() {
        dom.refresh.classList.add("wt-sh-inc");
        dom.autoRefresh.classList.add("wt-sh-inc");
        dom.newBox.classList.add("wt-sh-inc");
        dom.renameStream.classList.add("wt-sh-inc");
        dom.boxSliderWrapper.classList.add("wt-sh-inc");
    }

    function enableStreamOptions() {
        dom.refresh.classList.remove("wt-sh-inc");
        dom.autoRefresh.classList.remove("wt-sh-inc");
        dom.newBox.classList.remove("wt-sh-inc");
        dom.renameStream.classList.remove("wt-sh-inc");
        dom.boxSliderWrapper.classList.remove("wt-sh-inc");
    }

    function disableGroupOptions() {
        dom.groupRefresh.classList.add("wt-sh-inc");
        dom.groupAutoRefresh.classList.add("wt-sh-inc");
        dom.groupAddColumn.classList.add("wt-sh-inc");
        dom.editGroup.classList.add("wt-sh-inc");
        dom.groupUsers.classList.add("wt-sh-inc");
        dom.groupBoxSliderWrapper.classList.add("wt-sh-inc");
    }

    function enableGroupOptions() {
        dom.groupRefresh.classList.remove("wt-sh-inc");
        dom.groupAutoRefresh.classList.remove("wt-sh-inc");
        dom.groupAddColumn.classList.remove("wt-sh-inc");
        dom.editGroup.classList.remove("wt-sh-inc");
        dom.groupUsers.classList.remove("wt-sh-inc");
        dom.groupBoxSliderWrapper.classList.remove("wt-sh-inc");
    }

    function drawBox(boxData, isGroup) {
        var queryValue;

        if (isGroup) {
            queryValue = boxData.name;
        } else {
            switch (boxData.type) {
            case 1: // Timeline
                queryValue = boxData.query;
                break;

            case 2: // Area
                queryValue = i18n.t("wisetank.areas." + boxData.query);
                break;

            case 3: // User
                queryValue = "@" + boxData.query;
                break;
            }
        }

        function draw(bx) {
            var box = document.createElement("div");
            var position = bx.position;
            box.className = "wt-str-bx";
            box.dataset.id = bx.id;
            box.firstRefresh = true;
            box.refreshErrors = 0;
            box.lastMessageReached = false;
            box.dataset.position = position;

            var wrapper = document.createElement("div");
            wrapper.className = "wt-str-bx-w";

            var boxHeader = document.createElement("div");
            boxHeader.className = "wt-str-bx-hd";

            var boxName = document.createElement("span");
            boxName.className = "wt-str-bx-hd-nm";
            if (isGroup) {
                boxName.className += " wt-str-bx-hd-nme";
            }
            boxName.innerText = queryValue;
            boxName.title = queryValue;
            boxHeader.appendChild(boxName);

            if (!isGroup) {
                var boxType = document.createElement("span");
                boxType.className = "wt-str-bx-hd-tp";
                boxType.innerText = i18n.t("wisetank.boxTypes." + boxData.type);
                boxHeader.appendChild(boxType);
            }

            var boxMenu = document.createElement("div");
            boxMenu.className = "wt-str-bx-hd-bxm";
            boxHeader.appendChild(boxMenu);

            if (isGroup && (bx.writeOwnArticles || bx.permissions.overrideOwnArticles)) {
                var boxWrite = document.createElement("div");
                boxWrite.className = "fa fa-paper-plane wt-grp-bx-hd-wrt";
                boxMenu.appendChild(boxWrite);
            }

            var boxRefresh = document.createElement("div");
            boxRefresh.className = "fa fa-refresh wt-str-bx-hd-rfs";
            boxMenu.appendChild(boxRefresh);

            var boxArrow = document.createElement("div");
            boxArrow.className = "fa fa-cog wt-str-bx-hd-arr";
            boxMenu.appendChild(boxArrow);

            wrapper.appendChild(boxHeader);

            var contentsWrapper = document.createElement("div");
            contentsWrapper.className = "wt-str-bx-ct";

            var contents = document.createElement("div");
            contents.className = "wt-str-bx-ctw";
            contentsWrapper.appendChild(contents);

            var noContents = document.createElement("div");
            noContents.className = "wt-str-bx-no-ct";
            noContents.innerText = i18n.t("wisetank.nochypps");
            contents.appendChild(noContents);

            var contentsDataWrapper = document.createElement("div");
            contentsDataWrapper.className = "wt-str-bx-ctc";
            contents.appendChild(contentsDataWrapper);

            var contentsData = document.createElement("div");
            contentsData.className = "wt-str-bx-ctc-tws";
            contentsDataWrapper.appendChild(contentsData);

            wrapper.appendChild(contentsWrapper);

            box.appendChild(wrapper);

            if (isGroup) {
                dom.groupBoxesWrapper.appendChild(box);
            } else {
                dom.boxesWrapper.appendChild(box);
            }
        }

        draw(boxData);

        if (isGroup) {
            if (boxData.children !== null) {
                for (var i = 0, l = boxData.children.length; i < l; i++) {
                    drawBox(boxData.children[i], isGroup);
                }
            }
        }
    }

    function getLastMessage(box, messageId) {
        var messages = box.getElementsById("wt-str-tw");
        var message = null;

        for (var i = 0, l = messages.length; i < l; i++) {
            if (messages[i].dataset.id === messageId) {
                message = messages[i];
                break;
            }
        }

        return message;
    }

    function drawMessage(box, message, direction, lastMessage) {
        var messageContainer = document.createElement("div");
        messageContainer.className = "wt-str-tw";
        messageContainer.dataset.id = message.id;

        var avatar = document.createElement("div");
        avatar.className = "wt-str-tw-av";
        if (message.avatar === undefined || message.avatar === null) {
            avatar.className += " wt-str-tw-nav fa fa-user";
        }
        messageContainer.appendChild(avatar);

        var nameContainer = document.createElement("div");
        nameContainer.className = "wt-str-tw-nm";
        messageContainer.appendChild(nameContainer);
        if (message.userFullName !== null) {
            var authorName = document.createElement("span");
            authorName.className = "wt-str-tw-atn";
            authorName.innerText = message.userFullName;
            nameContainer.appendChild(authorName);
        }

        var author = document.createElement("span");
        author.className = "wt-str-tw-at";
        author.innerText = "@" + message.user;
        nameContainer.appendChild(author);

        var date = document.createElement("span");
        date.className = "wt-str-tw-dt";
        date.innerText = moment(message.addDate).format("L LT");
        messageContainer.appendChild(date);

        var karmaCount = message.karma.toFixed(1);
        var karmaContainer = document.createElement("div");
        karmaContainer.className = "wt-str-tw-krmw";
        var karmaImage = document.createElement("span");
        karmaImage.className = "wt-str-tw-krmi wt-str-tw-krmi-" + ((Math.round(karmaCount * 2) / 2).toFixed(1).replace(".", ""));
        for (var i = 0; i < 5; i++) {
            var karmaStar = document.createElement("i");
            karmaStar.className = "fa fa-star wt-str-tw-krms wt-str-tw-krms-" + (i + 1);
            karmaStar.innerText = ""; // star icon in fontawesome
            karmaImage.appendChild(karmaStar);
        }
        karmaContainer.appendChild(karmaImage);
        var karmaValue = document.createElement("span");
        karmaValue.className = "wt-str-tw-krmv";
        karmaValue.innerText = karmaCount;
        karmaContainer.appendChild(karmaValue);
        messageContainer.appendChild(karmaContainer);
        if (message.userKarmaed !== null) {
            karmaImage.className += " wt-str-tw-krmi-u" + message.userKarmaed;
        }
        if (!message.canVote) {
            karmaContainer.className += " wt-str-tw-krmw-nv";
            karmaImage.className += " wt-str-tw-krmi-nv";
        }

        var votesContainer = document.createElement("div");
        votesContainer.className = "wt-str-tw-vtsw";
        var upvote = document.createElement("i");
        upvote.className = "fa fa-thumbs-up wt-str-tw-vti wt-str-tw-vtu";
        votesContainer.appendChild(upvote);
        var downvote = document.createElement("i");
        downvote.className = "fa fa-thumbs-down wt-str-tw-vti wt-str-tw-vtd";
        votesContainer.appendChild(downvote);
        var numVotes = message.upVotes - message.downVotes;
        var votes = document.createElement("span");
        votes.className = "wt-str-tw-vt";
        votes.innerText = (numVotes > 0 ? "+" : "") + numVotes;
        if (numVotes > 0) {
            votes.className += " wt-str-tw-vt-up";
        } else if (numVotes < 0) {
            votes.className += " wt-str-tw-vt-dw";
        }
        if (message.userUpvoted !== null) {
            if (message.userUpvoted) {
                upvote.className += " wt-str-tw-vts";
            } else {
                downvote.className += " wt-str-tw-vts";
            }
        }
        if (!message.canVote) {
            votesContainer.className += " wt-str-tw-vtsw-nv";
        }
        votesContainer.appendChild(votes);
        messageContainer.appendChild(votesContainer);

        var lead = document.createElement("span");
        lead.className = "wt-str-tw-ld";
        if (message.upVotes > message.downVotes && message.karma >= 4) {
            lead.className += " wt-str-tw-ld-pri";
        }
        lead.innerText = message.lead;
        messageContainer.appendChild(lead);

        switch (message.origin) {
        case WISETANK_ORIGIN:
            var plane = document.createElement("span");
            plane.className = "wt-str-tw-wtp fa fa-users";
            lead.insertBefore(plane, lead.firstChild);
            messageContainer.className += " wt-str-tw-wt";
            break;

        default:
            var referenceWrapper = document.createElement("div");
            referenceWrapper.className = "wt-str-tw-rfw";

            if (message.description !== null || message.hasImage) {
                referenceWrapper.className += " wt-str-tw-rfwe";
            }

            messageContainer.appendChild(referenceWrapper);
            var reference = document.createElement("div");
            reference.className = "wt-str-tw-rf";
            var referenceTitleWrapper = document.createElement("div");
            referenceTitleWrapper.className = "wt-str-tw-rftw";
            reference.appendChild(referenceTitleWrapper);
            var origin = document.createElement("span");
            origin.className = "wt-str-tw-or";
            origin.innerText = i18n.t("wisetank.origins." + message.origin);
            referenceTitleWrapper.appendChild(origin);

            if (message.favicon !== null) {
                var favicon = document.createElement("img");
                favicon.className = "wt-str-tw-rff";
                favicon.src = "data:image/png;base64," + message.favicon;
                referenceTitleWrapper.appendChild(favicon);
            }

            if (message.source !== null) {
                var source = document.createElement("span");
                source.className = "wt-str-tw-rfs";
                source.innerText = message.source;
                referenceTitleWrapper.appendChild(source);
            }

            var name = document.createElement("a");
            name.href = message.reference;
            name.className = "wt-str-tw-rfl";
            name.innerText = message.name;
            name.dataset.origin = message.origin;
            reference.appendChild(name);
            referenceWrapper.appendChild(reference);

            if (message.hasImage) {
                var image = document.createElement("img");
                image.className = "wt-str-tw-rfi";
                image.src = api.articleImage + message.id;
                reference.appendChild(image);
            }

            if (message.description !== null) {
                var description = document.createElement("div");
                description.className = "wt-str-tw-rfd";
                description.innerText = message.description;
                reference.appendChild(description);
            }
            break;
        }

        var lMessage;
        switch (direction) {
        case 1:
            if (lastMessage === null) {
                box.appendChild(messageContainer);
            } else {
                lMessage = getLastMessage(box, lastMessage[0]);
                if (lMessage === null) {
                    box.appendChild(messageContainer);
                } else {
                    box.insertBefore(messageContainer, lMessage);
                }
            }
            break;

        case 0:
            lMessage = getLastMessage(box, lastMessage[1]);
            if (lMessage === null) {
                box.appendChild(messageContainer);
            } else {
                box.insertBefore(messageContainer, lMessage);
            }
            break;

        case -1:
            box.appendChild(messageContainer);
            break;
        }
    }

    function getBoxesForRefresh() {
        var boxIds = [];

        var boxes = dom.boxes.getElementsByClassName("wt-str-bx"),
            l = parseInt(dom.boxes.dataset.numBoxes);

        if (l === 0) {
            refreshingStream = false;
            return [];
        }
        refreshPending = l;

        for (var i = 0; i < l; i++) {
            boxIds.push(boxes[i]);
        }

        return boxIds;
    }

    function refreshSetTimeout() {
        if (refreshTimeout !== null) {
            clearTimeout(refreshTimeout);
        }

        if (refreshInterval !== -1) {
            setTimeout(refreshStreamTimeout, refreshInterval * 60 * 1000);
        }
    }

    function refreshStreamBox(streamId, box, position, direction, lastMessage) {
        $.ajax({
            async: true,
            type: "POST",
            url: api.getStreamData,
            data: {
                stream: streamId,
                box: box.dataset.id,
                lastMessage: (box.firstRefresh ? [] : lastMessage), //TODO
                direction: (typeof direction === "number" ? direction : 1)
            },

            success: function(result) {
                if (selectingStream || actualStream !== streamId) {
                    refreshingStream = false;
                    return;
                }

                var messages = result.articles.length;

                if (box.firstRefresh) {
                    var noMessages = box.getElementsByClassName("wt-str-bx-no-ct")[0];
                    if (messages === 0) {
                        noMessages.classList.toggle("wt-str-bx-no-ct-act", true);
                    } else {
                        box.firstRefresh = false;
                        noMessages.classList.toggle("wt-str-bx-no-ct-act", false);
                    }
                }

                for (var i = 0; i < messages; i++) {
                    drawMessage(box.getElementsByClassName("wt-str-bx-ctc-tws")[0], result.articles[i], direction, (box.firstRefresh ? null : null)); //TODO 
                }

                if (messages < result.articleCount) {
                    // TODO AÑADIR HUECO DE FALTAN MESSAGES
                }

                // TODO IF FIRST REFRESH, WRITE ALL
                // TODO ELSE -> MAINTAIN SCROLL POSITION -> ADD HEADER WITH NEW -> CLICK HEADER == GO TO FIRST UNREAD

                refreshPending--;

                if (refreshPending === 0) {
                    refreshingStream = false;
                }

                box.refreshErrors = 0;
            },

            error: function() {
                if (!selectingStream && actualStream !== streamId) {
                    refreshPending--;

                    if (refreshPending === 0) {
                        refreshingStream = false;
                    }

                    box.refreshErrors++;
                    // TODO WRITE MESSAGE "HAY ALGUN PROBLEMA ACTUALIZANDO MENSAJES, TRY AGAIN?"
                }
            }
        });
    }

    function refreshStreamTimeout() {
        if (selectingStream) {
            return;
        }

        var streamId = actualStream;
        refreshingStream = true;
        var boxIds = getBoxesForRefresh();

        if (boxIds !== undefined && boxIds !== null) {
            for (var i = 0, l = boxIds.length; i < l; i++) {
                if (selectingStream || actualStream !== streamId) {
                    break;
                }

                refreshStreamBox(streamId, boxIds[i], i, 1, (boxIds[i].firstRefresh ? null : boxIds[i].getElementsByClassName("wt-str-tw")[0].dataset.id));
            }
        }

        refreshSetTimeout();
    }

    function refreshStream() {
        if (selectingStream) {
            return;
        }

        var streamId = actualStream;
        refreshingStream = true;
        var boxIds = getBoxesForRefresh();

        if (boxIds !== undefined && boxIds !== null) {
            for (var i = 0, l = boxIds.length; i < l; i++) {
                if (selectingStream || actualStream !== streamId) {
                    break;
                }

                refreshStreamBox(streamId, boxIds[i], i, 1, null);
            }
        }

        refreshSetTimeout();

        return;
    }

    function drawStream(stream, position, append) {
        var streamHeader = document.createElement("li");
        streamHeader.className = "wt-str-ld wt-str-lds";
        streamHeader.dataset.position = position;

        var streamContent = document.createElement("span");
        streamContent.className = "wt-str-ldc";
        streamContent.innerText = stream.name;
        streamHeader.appendChild(streamContent);

        var streamShadowContent = document.createElement("span");
        streamShadowContent.className = "wt-str-ldsc";
        streamShadowContent.innerText = stream.name;
        streamHeader.appendChild(streamShadowContent);

        var streamClose = document.createElement("span");
        streamClose.className = "wt-str-ldcc";
        streamClose.innerText = "×";
        streamHeader.appendChild(streamClose);

        if (append) {
            dom.streams.insertBefore(streamHeader, dom.newStream);
        } else {
            dom.streams.insertBefore(streamHeader, dom.streams.firstChild);
        }

        return streamHeader;
    }

    function drawStreams() {
        for (var i = streams.length - 1, l = 0; i >= l; i--) {
            drawStream(streams[i], i, false);

        }
    }

    function selectStream(element) {
        selectingStream = true;

        (function x() {
            if (refreshingStream) {
                setTimeout(x, 100);
                return;
            }

            actualStream = null;

            if (refreshTimeout !== null) {
                clearTimeout(refreshTimeout);
            }

            var streamsLi = dom.streams.getElementsByClassName("wt-str-lds");
            for (var i = 0, l = streams.length; i < l; i++) {
                streamsLi[i].classList.remove("wt-str-lda");
            }

            while (dom.boxesWrapper.firstChild) {
                dom.boxesWrapper.removeChild(dom.boxesWrapper.firstChild);
            }


            if (element === undefined || element === null) {
                dom.noTabs.className = "wt-str-no-tab-act";
                dom.noBoxes.className = "";
                disableStreamOptions();
                return;
            } else {
                dom.noTabs.className = "";
                element.classList.add("wt-str-lda");
            }

            var stream = streams[element.dataset.position];

            if (stream.boxes.length === 0) {
                dom.noTabs.className = "";
                dom.noBoxes.className = "wt-str-no-bxs-act";
            } else {
                dom.noBoxes.className = "";

                for (var i = 0, l = stream.boxes.length; i < l; i++) {
                    drawBox(stream.boxes[i]);
                }
            }

            actualStream = stream.id;
            dom.boxes.dataset.id = stream.id;
            dom.boxes.dataset.width = stream.boxesWidth;
            dom.boxes.dataset.originalWidth = stream.boxesWidth;
            dom.boxes.dataset.numBoxes = stream.boxes.length;
            dom.boxSlider.className = "wt-str-sh-bws-" + stream.boxesWidth;
            dom.boxSlider.style.left = "";
            normalizeBoxes(dom.boxes);

            if (stream.refreshRate === 0) {
                refreshInterval = -1;
                dom.autoRefreshHelper.innerText = i18n.t("Disabled");
            } else {
                refreshInterval = stream.refreshRate;
                dom.autoRefreshHelper.innerText = stream.refreshRate + " " + i18n.t("Minutes");
            }

            selectingStream = false;
            refreshStream();
            enableStreamOptions();
        })();
    }

    function getGroupBoxesForRefresh() {
        var boxIds = [];

        var boxes = dom.groupBoxes.getElementsByClassName("wt-str-bx"),
            l = parseInt(dom.groupBoxes.dataset.numBoxes);

        if (l === 0) {
            refreshingGroup = false;
            return [];
        }

        refreshGroupPending = l;

        for (var i = 0; i < l; i++) {
            boxIds.push(boxes[i]);
        }

        return boxIds;
    }

    function refreshGroupSetTimeout() {
        if (refreshGroupTimeout !== null) {
            clearTimeout(refreshingGroupTimeout);
        }

        if (refreshGroupInterval !== -1) {
            setTimeout(refreshingGroupTimeout, refreshGroupInterval * 60 * 1000);
        }
    }

    function refreshGroupBox(groupId, box, position, direction, lastMessage) {
        $.ajax({
            async: true,
            type: "POST",
            url: api.getGroupBoxData,
            data: {
                timeline: box.dataset.id,
                lastMessage: (box.firstRefresh ? [] : lastMessage), //TODO
                direction: (typeof direction === "number" ? direction : 1)
            },

            success: function(result) {
                if (selectingGroup || actualGroup !== groupId) {
                    refreshingGroup = false;
                    return;
                }

                var messages = result.articles.length;

                if (box.firstRefresh) {
                    var noMessages = box.getElementsByClassName("wt-str-bx-no-ct")[0];
                    if (messages === 0) {
                        noMessages.classList.toggle("wt-str-bx-no-ct-act", true);
                    } else {
                        box.firstRefresh = false;
                        noMessages.classList.toggle("wt-str-bx-no-ct-act", false);
                    }
                }

                for (var i = 0; i < messages; i++) {
                    drawMessage(box.getElementsByClassName("wt-str-bx-ctc-tws")[0], result.articles[i], direction, (box.firstRefresh ? null : null)); //TODO 
                }

                if (messages < result.articleCount) {
                    // TODO AÑADIR HUECO DE FALTAN MESSAGES
                }

                // TODO IF FIRST REFRESH, WRITE ALL
                // TODO ELSE -> MAINTAIN SCROLL POSITION -> ADD HEADER WITH NEW -> CLICK HEADER == GO TO FIRST UNREAD

                refreshGroupPending--;

                if (refreshGroupPending === 0) {
                    refreshingGroup = false;
                }

                box.refreshErrors = 0;
            },

            error: function() {
                if (!selectingGroup && actualGroup === groupId) {
                    refreshGroupPending--;

                    if (refreshGroupPending === 0) {
                        refreshingGroup = false;
                    }

                    box.refreshErrors++;
                    // TODO WRITE MESSAGE "HAY ALGUN PROBLEMA ACTUALIZANDO MENSAJES, TRY AGAIN?"
                }
            }
        });
    }

    function refreshingGroupTimeout() {
        if (selectingGroup) {
            return;
        }

        var groupId = actualGroup;
        refreshingGroup = true;
        var boxIds = getGroupBoxesForRefresh();

        for (var i = 0, l = boxIds.length; i < l; i++) {
            if (selectingGroup || actualGroup !== groupId) {
                break;
            }

            refreshGroupBox(groupId, boxIds[i], i, 1, (boxIds[i].firstRefresh ? null : boxIds[i].getElementsByClassName("wt-str-tw")[0].dataset.id));
        }

        refreshGroupSetTimeout();
    }

    function refreshGroup() {
        if (selectingGroup) {
            return;
        }

        var groupId = actualGroup;
        refreshingGroup = true;
        var boxIds = getGroupBoxesForRefresh();

        if (boxIds !== undefined && boxIds !== null) {
            for (var i = 0, l = boxIds.length; i < l; i++) {
                if (selectingGroup || actualGroup !== groupId) {
                    break;
                }

                refreshGroupBox(groupId, boxIds[i], i, 1, null);
            }
        }

        refreshGroupSetTimeout();

        return;
    }

    function selectGroup(element) {
        selectingGroup = true;

        (function x() {
            if (refreshingGroup) {
                setTimeout(x, 100);
                return;
            }

            actualGroup = null;

            if (refreshGroupTimeout !== null) {
                clearTimeout(refreshGroupTimeout);
            }

            var groupsLi = dom.groupTimelinesContainer.getElementsByClassName("wt-grp-lds");
            for (var i = 0, l = groupsLi.length; i < l; i++) {
                groupsLi[i].classList.remove("wt-grp-lda");
            }

            while (dom.groupBoxesWrapper.firstChild) {
                dom.groupBoxesWrapper.removeChild(dom.groupBoxesWrapper.firstChild);
            }

            element.classList.add("wt-grp-lda");

            dom.selectTimeline.classList.remove("wt-grp-sl-tml-act");
            var timeline = timelines[element.dataset.position];
            drawBox(timeline, true);


            if (timeline.permissions.createSubtimelines) {
                dom.groupAddColumn.classList.remove("wt-grp-sh-add-dsb");
            } else {
                dom.groupAddColumn.classList.add("wt-grp-sh-add-dsb");
            }

            if (window.user.name === timeline.createdBy && timeline.permissionTypes !== undefined && timeline.permissionTypes !== null) {
                dom.editGroup.classList.remove("wt-grp-sh-egr-dsb");
            } else {
                dom.editGroup.classList.add("wt-grp-sh-egr-dsb");
            }

            actualGroup = timeline.id;
            dom.groupBoxes.dataset.id = timeline.id;
            dom.groupBoxes.dataset.width = timeline.boxesWidth;
            dom.groupBoxes.dataset.originalWidth = timeline.boxesWidth;
            dom.groupBoxes.dataset.numBoxes = 1 + (timeline.children === null ? 0 : timeline.children.length);
            dom.groupBoxSlider.className = "wt-str-sh-bws-" + timeline.boxesWidth;
            dom.groupBoxSlider.style.left = "";
            normalizeBoxes(dom.groupBoxes);

            if (timeline.refreshRate === 0) {
                refreshGroupInterval = -1;
                dom.groupAutoRefreshHelper.innerText = i18n.t("Disabled");
            } else {
                refreshGroupInterval = timeline.refreshRate;
                dom.groupAutoRefreshHelper.innerText = timeline.refreshRate + " " + i18n.t("Minutes");
            }

            selectingGroup = false;
            refreshGroup();
            enableGroupOptions();

            return;

            actualStream = stream.id;
            dom.boxes.dataset.id = stream.id;
            dom.boxes.dataset.width = stream.boxesWidth;
            dom.boxes.dataset.originalWidth = stream.boxesWidth;
            dom.boxes.dataset.numBoxes = stream.boxes.length;
            dom.boxSlider.className = "wt-str-sh-bws-" + stream.boxesWidth;
            dom.boxSlider.style.left = "";
            normalizeBoxes(dom.boxes);

            if (stream.refreshRate === 0) {
                refreshInterval = -1;
                dom.autoRefreshHelper.innerText = i18n.t("Disabled");
            } else {
                refreshInterval = stream.refreshRate;
                dom.autoRefreshHelper.innerText = stream.refreshRate + " " + i18n.t("Minutes");
            }

            selectingStream = false;
            refreshStream();
        })();
    }

    function selectGroupsArea(area, areaName) {
        disableGroupOptions();
        while (dom.groupBoxesWrapper.firstChild) {
            dom.groupBoxesWrapper.removeChild(dom.groupBoxesWrapper.firstChild);
        }

        var headerTimelines = dom.groupTimelinesContainer.getElementsByClassName("wt-grp-lds");

        while (headerTimelines.length !== 0) {
            dom.groupTimelinesContainer.removeChild(headerTimelines[0]);
        }

        var numTimelines = 0;

        var defaultTimeline = null;

        for (var i = 0, l = timelines.length; i < l; i++) {
            if (timelines[i].area === area) {
                var timelineHeader = document.createElement("li");
                timelineHeader.className = "wt-grp-ld wt-grp-lds";
                timelineHeader.dataset.id = timelines[i].id;
                timelineHeader.dataset.position = i;

                var timelineContent = document.createElement("span");
                timelineContent.className = "wt-grp-ldc";
                timelineContent.innerText = (timelines[i].areaDefault ? areaName : timelines[i].name);
                if (!timelines[i].areaDefault && timelines[i].description !== undefined && timelines[i].description !== null) {
                    timelineContent.title = timelines[i].description;
                }
                timelineHeader.appendChild(timelineContent);

                var timelineShadowContent = document.createElement("span");
                timelineShadowContent.className = "wt-grp-ldsc";
                timelineShadowContent.innerText = timelines[i].name;
                timelineHeader.appendChild(timelineShadowContent);

                if (timelines[i].areaDefault) {
                    timelineHeader.dataset.default = true;
                    defaultTimeline = timelineHeader;
                } else {
                    var timelineClose = document.createElement("span");
                    timelineClose.className = "wt-grp-ldcc";
                    timelineClose.innerText = "×";
                    timelineHeader.appendChild(timelineClose);
                }

                dom.groupTimelinesContainer.insertBefore(timelineHeader, dom.newGroup);
                numTimelines++;
            }
        }

        actualGroup = null;

        if (numTimelines === 0) {
            dom.selectTimeline.classList.remove("wt-grp-sl-tml-act");
            dom.noTimelines.classList.add("wt-grp-no-tml-act");
            dom.groupBoxesWrapper.style.width = "0px";
            $("#wt-grp-bxs").mCustomScrollbar("update");
        } else {
            dom.noTimelines.classList.remove("wt-grp-no-tml-act");
            if (defaultTimeline == null) {
                dom.selectTimeline.classList.add("wt-grp-sl-tml-act");
                dom.groupBoxesWrapper.style.width = "0px";
                $("#wt-grp-bxs").mCustomScrollbar("update");
            } else {
                selectGroup(defaultTimeline);
                dom.selectTimeline.classList.remove("wt-grp-sl-tml-act");
            }
        }

        // TODO CLEAR SCREEN && SELECT FIRST AREA
    }

    /*** MPDS ***/
    function createNewStream() {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var mpdContent = window.mpd.create(
            "new-stream",
            "400px",
            i18n.t("wisetank.newstream"),
            { closebtn: true }
        );

        var name = document.createElement("input");
        name.id = "wt-str-nt-nm";
        name.className = "input";
        name.type = "text";
        name.placeholder = i18n.t("Name");
        mpdContent.appendChild(name);

        var buttons = document.createElement("div");
        buttons.id = "wt-str-pup-bt";
        mpdContent.appendChild(buttons);

        var create = document.createElement("button");
        create.id = "wt-str-pup-ct";
        create.className = "input input-bl";
        create.innerText = i18n.t("wisetank.createstream");
        create.disabled = true;
        buttons.appendChild(create);

        var close = document.createElement("button");
        close.id = "wt-str-pup-cl";
        close.className = "input input-gr";
        close.innerText = i18n.t("Close");
        buttons.appendChild(close);

        $(name).on("input", function() {
            create.disabled = this.value.trim() === "";
        });

        $(name).on("keyup", function(e) {
            if (e.keyCode === 13) {
                $(create).trigger("click");
            }
        });

        $(create).on("click", function() {
            if (create.disabled) {
                return;
            }

            window.mpd.block();
            name.disabled = true;
            create.disabled = true;
            close.disabled = true;

            $.ajax({
                async: true,
                type: "POST",
                url: api.createStream,
                data: { name: name.value },

                success: function(result) {
                    if (result.status === 0) {
                        var stream = {
                            id: result.id,
                            name: name.value,
                            position: streams.length,
                            boxesWidth: 3,
                            boxes: [],
                            refreshRate: 5
                        };

                        streams.push(stream);
                        var streamElement = drawStream(stream, streams.length - 1, true);
                        selectStream(streamElement);
                        window.mpd.unblock();
                        window.mpd.hide();
                    } else {
                        createNoty("warning", "center", i18n.t("error-processing"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    window.mpd.unblock();
                                    name.disabled = false;
                                    create.disabled = false;
                                    close.disabled = false;
                                }
                            }
                        });
                    }
                },

                error: function() {
                    createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                        callback: {
                            afterClose: function() {
                                window.mpd.unblock();
                                name.disabled = false;
                                create.disabled = false;
                                close.disabled = false;
                            }
                        }
                    });
                }
            });
        });

        $(close).on("click", function() {
            $(mpdContent).parents(".mb-box").eq(0).find(".mb-close").eq(0).trigger("click");
        });
    }

    function deleteStream(streamId) {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var mpdContent = window.mpd.create(
            "delete-stream",
            "300px",
            i18n.t("wisetank.deletestream"),
            { closebtn: true }
        );

        var message = document.createElement("div");
        message.id = "wt-str-pup-msg";
        message.innerText = i18n.t("wisetank.confirmDeleteStream");
        mpdContent.appendChild(message);

        var buttons = document.createElement("div");
        buttons.id = "wt-str-pup-bt";
        mpdContent.appendChild(buttons);

        var deleteBtn = document.createElement("button");
        deleteBtn.id = "wt-str-pup-dt";
        deleteBtn.className = "input input-bl";
        deleteBtn.innerText = i18n.t("wisetank.deletestream");
        buttons.appendChild(deleteBtn);

        var close = document.createElement("button");
        close.id = "wt-str-pup-cl";
        close.className = "input input-gr";
        close.innerText = i18n.t("Close");
        buttons.appendChild(close);

        $(deleteBtn).on("click", function() {
            window.mpd.block();
            deleteBtn.disabled = true;
            close.disabled = true;

            $.ajax({
                async: true,
                type: "POST",
                url: api.deleteStream,
                data: { stream: streamId },

                success: function(result) {
                    if (result === 0) {
                        var position = -1;
                        for (var i = 0, l = streams.length; i < l; i++) {
                            if (streams[i].id === streamId) {
                                position = i;
                                break;
                            }
                        }

                        if (position > -1) {
                            for (var i = position + 1, l = streams.length; i < l; i++) {
                                streams[i].position--;
                            }

                            var deletedStream = null,
                                activeStreams = dom.streams.getElementsByClassName("wt-str-lds");

                            for (var i = 0, l = activeStreams.length; i < l; i++) {
                                var activeStream = activeStreams[i];
                                if (streams[i].id === streamId) {
                                    deletedStream = activeStream;
                                } else if (i > position) {
                                    activeStreams[i].dataset.position--;
                                }
                            }

                            if (deletedStream !== null) {
                                streams.splice(position, 1);

                                if (deletedStream.classList.contains("wt-str-lda")) {
                                    selectStream(activeStreams[(position === 0 ? 1 : 0)]);
                                }

                                dom.streams.removeChild(deletedStream);
                            }

                            window.mpd.unblock();
                            window.mpd.hide();
                        } else {
                            createNoty("warning", "center", i18n.t("internal-error"), 1500, ["click"], {
                                callback: {
                                    afterClose: function() {
                                        window.mpd.unblock();
                                        name.disabled = false;
                                        deleteBtn.disabled = false;
                                        close.disabled = false;
                                    }
                                }
                            });
                        }
                    } else {
                        createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    window.mpd.unblock();
                                    name.disabled = false;
                                    deleteBtn.disabled = false;
                                    close.disabled = false;
                                }
                            }
                        });
                    }
                },

                error: function() {
                    createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                        callback: {
                            afterClose: function() {
                                window.mpd.unblock();
                                name.disabled = false;
                                deleteBtn.disabled = false;
                                close.disabled = false;
                            }
                        }
                    });
                }
            });
        });

        $(close).on("click", function() {
            $(mpdContent).parents(".mb-box").eq(0).find(".mb-close").eq(0).trigger("click");
        });
    }

    function renameStream(streamId) {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var mpdContent = window.mpd.create(
            "new-stream",
            "400px",
            i18n.t("wisetank.editstream"),
            { closebtn: true }
        );

        var position = -1;

        for (var i = 0, l = streams.length; i < l; i++) {
            if (streams[i].id === streamId) {
                position = i;
                break;
            }
        }

        var name = document.createElement("input");
        name.id = "wt-str-nt-nm";
        name.className = "input";
        name.type = "text";
        name.placeholder = i18n.t("Name");
        name.value = streams[position].name;
        mpdContent.appendChild(name);

        var buttons = document.createElement("div");
        buttons.id = "wt-str-pup-bt";
        mpdContent.appendChild(buttons);

        var save = document.createElement("button");
        save.id = "wt-str-pup-ct";
        save.className = "input input-bl";
        save.innerText = i18n.t("Save");
        buttons.appendChild(save);

        var close = document.createElement("button");
        close.id = "wt-str-pup-cl";
        close.className = "input input-gr";
        close.innerText = i18n.t("Close");
        buttons.appendChild(close);

        $(name).on("input", function() {
            save.disabled = this.value.trim() === "";
        });

        $(name).on("keyup", function(e) {
            if (e.keyCode === 13) {
                $(save).trigger("click");
            }
        });

        $(save).on("click", function() {
            if (save.disabled) {
                return;
            }

            window.mpd.block();
            name.disabled = true;
            save.disabled = true;
            close.disabled = true;

            if (name.value === streams[position].name) {
                window.mpd.unblock();
                window.mpd.hide();
                return;
            }

            $.ajax({
                async: true,
                type: "POST",
                url: api.renameStream,
                data: { stream: streamId, name: name.value },

                success: function(result) {
                    if (result === 0) {
                        streams[position].name = name.value;
                        var element = dom.streams.getElementsByClassName("wt-str-lda")[0];
                        element.getElementsByClassName("wt-str-ldc")[0].innerText = name.value;
                        element.getElementsByClassName("wt-str-ldsc")[0].innerText = name.value;

                        window.mpd.unblock();
                        window.mpd.hide();
                    } else {
                        createNoty("warning", "center", i18n.t("internal-error"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    window.mpd.unblock();
                                    name.disabled = false;
                                    save.disabled = false;
                                    close.disabled = false;
                                }
                            }
                        });
                    }
                },

                error: function() {
                    createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                        callback: {
                            afterClose: function() {
                                window.mpd.unblock();
                                name.disabled = false;
                                save.disabled = false;
                                close.disabled = false;
                            }
                        }
                    });
                }
            });
        });

        $(close).on("click", function() {
            $(mpdContent).parents(".mb-box").eq(0).find(".mb-close").eq(0).trigger("click");
        });
    }

    function createNewBox() {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var mpdContent = window.mpd.create(
            "delete-stream",
            "400px",
            i18n.t("wisetank.newbox"),
            { closebtn: true, overflow: true }
        );

        var select = document.createElement("select");
        select.id = "wt-str-ns-tp";
        select.className = "input";
        select.placeholder = i18n.t("wisetank.selectBoxType");
        var defaultOption = document.createElement("option");
        defaultOption.selected = true;
        defaultOption.disabled = true;
        defaultOption.value = "";
        defaultOption.innerText = i18n.t("wisetank.selectBoxType");
        select.appendChild(defaultOption);
        //for (var i = 1; i <= api.boxTypes; i++) {
        [1, 2, 3].forEach(function(i) {
            var option = document.createElement("option");
            option.value = i;
            option.innerText = i18n.t("wisetank.boxTypes." + i);
            select.appendChild(option);
        });
        //}
        mpdContent.appendChild(select);
        var optionContents = document.createElement("div");
        optionContents.id = "wt-str-ns-td";
        mpdContent.appendChild(optionContents);

        var buttons = document.createElement("div");
        buttons.id = "wt-str-pup-bt";
        mpdContent.appendChild(buttons);

        var create = document.createElement("button");
        create.id = "wt-str-pup-ct";
        create.className = "input input-bl";
        create.innerText = i18n.t("wisetank.createbox");
        create.disabled = true;
        buttons.appendChild(create);

        var close = document.createElement("button");
        close.id = "wt-str-pup-cl";
        close.className = "input input-gr";
        close.innerText = i18n.t("Close");
        buttons.appendChild(close);

        var typeSelected, query = null, selectors = [];
        var typeSelect = $(select).selectize({
            onChange: function(value) {
                create.disabled = true;
                selectors.length = 0;

                while (optionContents.firstChild) {
                    optionContents.removeChild(optionContents.firstChild);
                }

                switch (value) {
                case "":
                    break;

                case "1": // Timeline
                    typeSelected = 1;
                    var areaSelect = document.createElement("select");
                    areaSelect.id = "wt-str-ns-tar";
                    areaSelect.className = "input";
                    areaSelect.placeholder = i18n.t("wisetank.selectArea");
                    var defaultAreaTOption = document.createElement("option");
                    defaultAreaTOption.selected = true;
                    defaultAreaTOption.disabled = true;
                    defaultAreaTOption.value = "";
                    defaultAreaTOption.innerText = i18n.t("wisetank.selectArea");
                    areaSelect.appendChild(defaultAreaTOption);

                    for (var area in sortedAreas) {
                        if (Object.prototype.hasOwnProperty.call(sortedAreas, area)) {
                            var option = document.createElement("option");
                            option.value = areas[sortedAreas[area]];
                            option.innerText = i18n.t("wisetank.areas." + sortedAreas[area].toLowerCase());
                            areaSelect.appendChild(option);
                        }
                    }
                    optionContents.appendChild(areaSelect);

                    var timelineSelect = document.createElement("select");
                    timelineSelect.id = "wt-str-ns-ttr";
                    timelineSelect.className = "input";
                    timelineSelect.placeholder = i18n.t("wisetank.selectTimeline");
                    var defaultTimelineTOption = document.createElement("option");
                    defaultTimelineTOption.selected = true;
                    defaultTimelineTOption.disabled = true;
                    defaultTimelineTOption.value = "";
                    defaultTimelineTOption.innerText = i18n.t("wisetank.selectTimeline");
                    timelineSelect.appendChild(defaultTimelineTOption);
                    optionContents.appendChild(timelineSelect);

                    selectors.push(areaSelect);
                    selectors.push(timelineSelect);

                    $(areaSelect).selectize({
                        onChange: function(value) {
                            timelineSelect.selectize.clearOptions();
                            timelineSelect.selectize.clear();

                            if (value === "") {
                                timelineSelect.selectize.disable();
                            } else {
                                var selectedArea = parseInt(value);

                                for (var i = 0, l = timelines.length; i < l; i++) {
                                    if (timelines[i].area === selectedArea) {
                                        timelineSelect.selectize.addOption({
                                            value: timelines[i].id,
                                            text: timelines[i].name,
                                            level: 0
                                        });

                                        if (timelines[i].children !== null) {
                                            for (var j = 0, k = timelines[i].children.length; j < k; j++) {
                                                timelineSelect.selectize.addOption({
                                                    value: timelines[i].children[j].id,
                                                    text: timelines[i].children[j].name,
                                                    level: 1
                                                });
                                            }
                                        }
                                    }
                                }

                                timelineSelect.selectize.enable();
                            }

                            timelineSelect.selectize.refreshOptions(false);
                        }
                    });

                    $(timelineSelect).selectize({
                        render: {
                            option: function(data) {
                                return "<div data-value=\"" + data.value + "\" class=\"option option-" + data.level + "\">" + data.text + "</div>";
                            }
                        },
                        onChange: function(value) {
                            create.disabled = (value === "");
                            query = value;
                        }
                    });

                    timelineSelect.selectize.disable();
                    break;

                case "2": // Area
                    typeSelected = 2;
                    var areaSelect = document.createElement("select");
                    areaSelect.id = "wt-str-ns-tar";
                    areaSelect.className = "input";
                    areaSelect.placeholder = i18n.t("wisetank.selectArea");
                    var defaultAreaOption = document.createElement("option");
                    defaultAreaOption.selected = true;
                    defaultAreaOption.disabled = true;
                    defaultAreaOption.value = "";
                    defaultAreaOption.innerText = i18n.t("wisetank.selectArea");
                    areaSelect.appendChild(defaultAreaOption);

                    for (var area in sortedAreas) {
                        if (Object.prototype.hasOwnProperty.call(sortedAreas, area)) {
                            var option = document.createElement("option");
                            option.value = areas[sortedAreas[area]];
                            option.innerText = i18n.t("wisetank.areas." + sortedAreas[area].toLowerCase());
                            areaSelect.appendChild(option);
                        }
                    }
                    optionContents.appendChild(areaSelect);

                    selectors.push(areaSelect);

                    $(areaSelect).selectize({
                        onChange: function(value) {
                            create.disabled = (value === "");
                            query = value;
                        }
                    });
                    break;

                case "3": // User
                    typeSelected = 3;
                    var userWrapper = document.createElement("div");
                    userWrapper.id = "wt-str-ns-tusw";
                    var userInput = document.createElement("input");
                    userInput.id = "wt-str-ns-tus";
                    userInput.type = "text";
                    userInput.className = "input";
                    userInput.placeholder = i18n.t("username");
                    userWrapper.appendChild(userInput);
                    var atDiv = document.createElement("div");
                    atDiv.id = "wt-str-ns-tusat";
                    atDiv.innerText = "@";
                    userWrapper.appendChild(atDiv);
                    optionContents.appendChild(userWrapper);

                    selectors.push(userInput);

                    $(userInput).on("input", function() {
                        create.disabled = (this.value === "");
                        query = this.value;
                    });
                    break;
                }

                window.mpd.resize();
            }
        });

        $(name).on("input", function() {
            create.disabled = this.value.trim() === "";
        });

        $(create).on("click", function() {
            if (this.disabled) {
                return;
            }

            window.mpd.block();
            typeSelect[0].selectize.disable();
            selectors.forEach(function(element) {
                if (element.selectize === undefined || element.selectize === null) {
                    element.disabled = true;
                } else {
                    element.selectize.disable();
                }
            });
            create.disabled = true;
            close.disabled = true;

            $.ajax({
                async: true,
                type: "POST",
                url: api.createBox,
                data: { stream: actualStream, type: typeSelected, query: query },

                success: function(result) {
                    if (result.status === 0) {
                        for (var i = 0, l = streams.length; i < l; i++) {
                            if (streams[i].id === actualStream) {
                                var maxPosition = 0;

                                for (var j = 0, k = streams[i].boxes.length; j < k; j++) {
                                    maxPosition = Math.max(maxPosition, streams[i].boxes[j].position);
                                }

                                // TODO: POSITION, QUERY NAME, DRAW BOX, RESIZE BOX, REFRESH BOX
                                var boxData = {
                                    id: result.id,
                                    name: null,
                                    type: typeSelected,
                                    position: ++maxPosition,
                                    query: "TODO"
                                };

                                //streams[i].boxes.push(boxData);
                                //drawBox(boxData);
                            }
                        }

                        window.mpd.unblock();
                        window.mpd.hide();
                    } else {
                        createNoty("warning", "center", i18n.t("internal-error"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    window.mpd.unblock();
                                    typeSelect[0].selectize.enable();
                                    selectors.forEach(function(element) {
                                        if (element.selectize === undefined || element.selectize === null) {
                                            element.disabled = false;
                                        } else {
                                            element.selectize.enable();
                                        }
                                    });
                                    create.disabled = false;
                                    close.disabled = false;
                                }
                            }
                        });
                    }
                },

                error: function() {
                    createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                        callback: {
                            afterClose: function() {
                                window.mpd.unblock();
                                typeSelect[0].selectize.enable();
                                selectors.forEach(function(element) {
                                    if (element.selectize === undefined || element.selectize === null) {
                                        element.disabled = true;
                                    } else {
                                        element.selectize.enable();
                                    }
                                });
                                create.disabled = false;
                                close.disabled = false;
                            }
                        }
                    });
                }
            });
        });

        $(close).on("click", function() {
            $(mpdContent).parents(".mb-box").eq(0).find(".mb-close").eq(0).trigger("click");
        });
    }

    function drawPermissionSelectors(element, disabled) {
        if (typeof disabled !== "boolean") {
            disabled = false;
        }

        for (var i = 0, l = allPermissions.length; i < l; i++) {
            var permission = document.createElement("div");
            permission.className = "wt-grp-pup-prm";
            element.appendChild(permission);

            var check = document.createElement("input");
            check.id = allPermissions[i];
            check.className = "mini";
            check.type = "checkbox";
            check.disabled = disabled;
            permission.appendChild(check);

            var labelCheck = document.createElement("label");
            labelCheck.className = "option";
            labelCheck.htmlFor = allPermissions[i];
            permission.appendChild(labelCheck);

            var labelText = document.createElement("label");
            labelText.innerText = i18n.t("wisetank.permissions." + allPermissions[i]);
            labelText.htmlFor = allPermissions[i];
            permission.appendChild(labelText);
        }
    }

    function addNewGroup() {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var content = document.createElement("div");
        content.id = "wt-grp-pup-agc";

        var buttons = document.createElement("div");
        buttons.id = "wt-str-pup-bt";

        var create = document.createElement("button");
        create.id = "wt-str-pup-ct";
        create.className = "input input-bl";
        create.disabled = true;
        buttons.appendChild(create);

        var close = document.createElement("button");
        close.id = "wt-str-pup-cl";
        close.className = "input input-gr";
        close.innerText = i18n.t("Close");
        buttons.appendChild(close);

        function showNewGroup(content, create) {
            create.innerText = i18n.t("Create");
            create.disabled = true;

            var name = document.createElement("input");
            name.id = "wt-grp-pup-nw-nme";
            name.className = "input";
            name.type = "text";
            name.placeholder = i18n.t("Name");
            content.appendChild(name);

            var description = document.createElement("input");
            description.id = "wt-grp-pup-nw-dsc";
            description.className = "input";
            description.type = "text";
            description.placeholder = i18n.t("Description") + " (" + i18n.t("optional") + ")";
            content.appendChild(description);

            var accessDiv = document.createElement("div");
            accessDiv.id = "wt-grp-pup-nw-atd";
            content.appendChild(accessDiv);

            var accessSelected = 1;

            var publicAccess = document.createElement("button");
            publicAccess.className = "input input-bl input-nh";
            publicAccess.innerText = i18n.t("Public");
            publicAccess.dataset.id = 1;
            accessDiv.appendChild(publicAccess);

            var protectedAccess = document.createElement("button");
            protectedAccess.className = "input input-gr";
            protectedAccess.innerText = i18n.t("InviteRequired");
            protectedAccess.dataset.id = 2;
            accessDiv.appendChild(protectedAccess);

            var privateAccess = document.createElement("button");
            privateAccess.className = "input input-gr";
            privateAccess.innerText = i18n.t("Private");
            privateAccess.dataset.id = 3;
            accessDiv.appendChild(privateAccess);

            $(accessDiv).on("click", ".input", function() {
                accessSelected = this.dataset.id;

                publicAccess.className = "input input-gr";
                protectedAccess.className = "input input-gr";
                privateAccess.className = "input input-gr";

                this.className = "input input-bl input-nh";
            });

            var moderationDiv = document.createElement("div");
            moderationDiv.id = "wt-grp-pup-nw-mdd";
            content.appendChild(moderationDiv);

            var articleModerationDiv = document.createElement("div");
            articleModerationDiv.id = "wt-grp-pup-nw-amd";
            articleModerationDiv.className = "wt-grp-pup-nw-md";
            moderationDiv.appendChild(articleModerationDiv);

            var articleCheck = document.createElement("input");
            articleCheck.className = "mini";
            articleCheck.type = "checkbox";
            articleCheck.id = "wt-grp-pup-nw-am";
            articleModerationDiv.appendChild(articleCheck);

            var articlelabelCheck = document.createElement("label");
            articlelabelCheck.className = "option";
            articlelabelCheck.htmlFor = "wt-grp-pup-nw-am";
            articleModerationDiv.appendChild(articlelabelCheck);

            var articleLabelText = document.createElement("label");
            articleLabelText.className = "wt-grp-pup-nw-mlt";
            articleLabelText.innerText = i18n.t("wisetank.moderation.articles");
            articleLabelText.htmlFor = "wt-grp-pup-nw-am";
            articleModerationDiv.appendChild(articleLabelText);

            var commentModerationDiv = document.createElement("div");
            commentModerationDiv.id = "wt-grp-pup-nw-cmd";
            commentModerationDiv.className = "wt-grp-pup-nw-md";
            moderationDiv.appendChild(commentModerationDiv);

            var moderationArticleCheck = document.createElement("input");
            moderationArticleCheck.className = "mini";
            moderationArticleCheck.type = "checkbox";
            moderationArticleCheck.id = "wt-grp-pup-nw-cm";
            commentModerationDiv.appendChild(moderationArticleCheck);

            var moderationlabelCheck = document.createElement("label");
            moderationlabelCheck.className = "option";
            moderationlabelCheck.htmlFor = "wt-grp-pup-nw-cm";
            commentModerationDiv.appendChild(moderationlabelCheck);

            var moderationLabelText = document.createElement("label");
            moderationLabelText.className = "wt-grp-pup-nw-mlt";
            moderationLabelText.innerText = i18n.t("wisetank.moderation.comments");
            moderationLabelText.htmlFor = "wt-grp-pup-nw-cm";
            commentModerationDiv.appendChild(moderationLabelText);

            var writeArticlesDiv = document.createElement("div");
            writeArticlesDiv.id = "wt-grp-pup-nw-wod";
            writeArticlesDiv.className = "wt-grp-pup-nw-md";
            moderationDiv.appendChild(writeArticlesDiv);

            var writeArticlesCheck = document.createElement("input");
            writeArticlesCheck.className = "mini";
            writeArticlesCheck.type = "checkbox";
            writeArticlesCheck.id = "wt-grp-pup-nw-wo";
            writeArticlesDiv.appendChild(writeArticlesCheck);

            var writeLabelCheck = document.createElement("label");
            writeLabelCheck.className = "option";
            writeLabelCheck.htmlFor = "wt-grp-pup-nw-wo";
            writeArticlesDiv.appendChild(writeLabelCheck);

            var writeLabelText = document.createElement("label");
            writeLabelText.className = "wt-grp-pup-nw-mlt";
            writeLabelText.innerText = i18n.t("wisetank.moderation.write");
            writeLabelText.htmlFor = "wt-grp-pup-nw-wo";
            writeArticlesDiv.appendChild(writeLabelText);

            var rolesDiv = document.createElement("div");
            rolesDiv.id = "wt-grp-pup-nw-rld";
            content.appendChild(rolesDiv);

            var rolesTitle = document.createElement("div");
            rolesTitle.id = "wt-grp-pup-nw-rlt";
            rolesTitle.innerText = i18n.t("Roles");
            rolesDiv.appendChild(rolesTitle);

            var rolesSelector = document.createElement("div");
            rolesSelector.id = "wt-grp-pup-nw-rls";
            rolesDiv.appendChild(rolesSelector);

            var permissionTypes = [];

            var roleCount = 0,
                roleButtons = [];
            for (var i = 0, l = roles.length; i < l; i++) {
                if (roles[i].selectable) {
                    var role = document.createElement("button");
                    role.dataset.id = i;
                    role.className = "wt-grp-pup-nw-rle input input-gr mini";
                    role.innerText = i18n.t("wisetank.roles." + roles[i].name);
                    rolesSelector.appendChild(role);
                    permissionTypes[i] = -1;
                    roleCount++;
                    roleButtons.push(role);
                }
            }

            var roleWidth = (100 / roleCount) + "%";
            for (var i = 0, l = roleButtons.length; i < l; i++) {
                roleButtons[i].style.width = roleWidth;
            }

            var permissionSelector = document.createElement("select");
            permissionSelector.id = "wt-grp-pup-nw-rlp";
            permissionSelector.className = "input";
            var permissionPlaceholder = document.createAttribute("placeholder");
            permissionPlaceholder.value = i18n.t("wisetank.selectPermissionType");
            permissionSelector.attributes.setNamedItem(permissionPlaceholder);
            rolesDiv.appendChild(permissionSelector);

            var permissionsContent = document.createElement("div");
            permissionsContent.id = "wt-grp-pup-nw-prc";
            rolesDiv.appendChild(permissionsContent);
            drawPermissionSelectors(permissionsContent, true);

            function checkCreateEnabled() {
                var hasPermissions = true;

                for (var permission in permissionTypes) {
                    if (Object.prototype.hasOwnProperty.call(permissionTypes, permission) && permissionTypes[permission] === null) {
                        hasPermissions = false;
                        break;
                    }
                }

                create.disabled = !(
                    (name.value !== null && name.value.trim() !== "")
                        && hasPermissions
                );
            }

            $(permissionSelector).selectize({
                onChange: function(value) {
                    var permissions;

                    if (value !== "" && value !== "-1") {
                        var permissionValue = permissionSelector.selectize.options[value];
                        permissions = roles[permissionValue.role].permissions[permissionValue.value].permissions;

                        for (var permission in permissions) {
                            if (Object.prototype.hasOwnProperty.call(permissions, permission)) {
                                var element = document.getElementById(permission);
                                if (element !== undefined && element !== null) {
                                    element.checked = permissions[permission];
                                }
                            }
                        }

                        permissionTypes[permissionValue.role] = roles[permissionValue.role].permissions[permissionValue.value].id;
                    } else {
                        permissions = permissionsContent.getElementsByClassName("wt-grp-pup-prm");
                        for (var i = 0, l = permissions.length; i < l; i++) {
                            permissions[i].getElementsByTagName("input")[0].checked = false;
                        }
                    }

                    checkCreateEnabled();
                }
            });
            permissionSelector.selectize.disable();

            function setPermissions(id) {
                permissionSelector.selectize.clearOptions();
                permissionSelector.selectize.clear();

                permissionSelector.selectize.addOption({
                    value: -1,
                    text: i18n.t("wisetank.permissionTypes.disabled"),
                    role: id
                });

                for (var i = 0, l = roles[id].permissions.length; i < l; i++) {
                    permissionSelector.selectize.addOption({
                        value: i,
                        text: i18n.t("wisetank.permissionTypes." + roles[id].permissions[i].name),
                        role: id
                    });
                }

                permissionSelector.selectize.enable();
                permissionSelector.selectize.refreshOptions(false);

                permissionSelector.selectize.setValue(permissionTypes[id]);
            }

            $(rolesSelector).on("click", ".wt-grp-pup-nw-rle", function() {
                if (!this.classList.contains("input-bl")) {
                    var roles = rolesSelector.getElementsByClassName("wt-grp-pup-nw-rle");
                    for (var i = 0, l = roles.length; i < l; i++) {
                        roles[i].classList.remove("input-bl", "input-nh");
                        roles[i].classList.add("input-gr");
                    }

                    this.classList.remove("input-gr");
                    this.classList.add("input-bl", "input-nh");
                    setPermissions(parseInt(this.dataset.id));
                }
            });

            $(rolesSelector.getElementsByClassName("wt-grp-pup-nw-rle")[0]).trigger("click");

            $(name).on("input", function() {
                checkCreateEnabled();
            });

            return function() {
                if (create.disabled) {
                    return;
                }

                window.mpd.block();
                name.disabled = true;
                description.disabled = true;
                publicAccess.disabled = true;
                protectedAccess.disabled = true;
                privateAccess.disabled = true;
                articleCheck.disabled = true;
                moderationArticleCheck.disabled = true;
                writeArticlesCheck.disabled = true;
                $(rolesSelector).find(".wt-grp-pup-nw-rle").prop("disabled", true);
                permissionSelector.selectize.disable();
                create.disabled = true;
                close.disabled = true;

                var buttons = window.mpd.buttons();
                for (var i = 0, l = buttons.length; i < l; i++) {
                    buttons[i].disabled = true;
                }

                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.createGroup,
                    data: {
                        name: name.value,
                        description: description.value,
                        accessType: accessDiv.getElementsByClassName("input-bl")[0].dataset.id,
                        moderatedArticles: articleCheck.checked,
                        moderatedComments: moderationArticleCheck.checked,
                        writeOwnArticles: writeArticlesCheck.checked,
                        permissionTypes: permissionTypes.reduce(function(arr, value, index) {
                            if (i !== undefined) {
                                arr.push({
                                    role: roles[index].id,
                                    permissionType: value
                                });
                            }
                            return arr;
                        }, []),
                        area: actualArea,
                        offsetDate: window.user.offsetDate
                    },

                    success: function(result) {
                        if (result.status === 0) {
                            window.mpd.unblock();
                            window.mpd.hide();
                        } else {
                            createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                                callback: {
                                    afterClose: function() {
                                        window.mpd.unblock();
                                        name.disabled = false;
                                        description.disabled = false;
                                        publicAccess.disabled = false;
                                        protectedAccess.disabled = false;
                                        privateAccess.disabled = false;
                                        articleCheck.disabled = false;
                                        moderationArticleCheck.disabled = false;
                                        writeArticlesCheck.disabled = false;
                                        $(rolesSelector).find(".wt-grp-pup-nw-rle").prop("disabled", false);
                                        permissionSelector.selectize.enable();
                                        create.disabled = false;
                                        close.disabled = false;

                                        var buttons = window.mpd.buttons();
                                        for (var i = 0, l = buttons.length; i < l; i++) {
                                            buttons[i].disabled = false;
                                        }
                                    }
                                }
                            });
                        }
                    },

                    error: function() {
                        createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    window.mpd.unblock();
                                    name.disabled = false;
                                    description.disabled = false;
                                    publicAccess.disabled = false;
                                    protectedAccess.disabled = false;
                                    privateAccess.disabled = false;
                                    articleCheck.disabled = false;
                                    moderationArticleCheck.disabled = false;
                                    writeArticlesCheck.disabled = false;
                                    $(rolesSelector).find(".wt-grp-pup-nw-rle").prop("disabled", false);
                                    permissionSelector.selectize.enable();
                                    create.disabled = false;
                                    close.disabled = false;

                                    var buttons = window.mpd.buttons();
                                    for (var i = 0, l = buttons.length; i < l; i++) {
                                        buttons[i].disabled = false;
                                    }
                                }
                            }
                        });
                    }
                });
            };
        }

        function showSearch() {
            create.innerText = i18n.t("Add");
            create.disabled = true;

            var search = document.createElement("input");
            search.id = "wt-grp-pup-nw-sri";
            search.className = "input";
            search.placeholder = i18n.t("wisetank.selectGroupName");
            content.appendChild(search);
            $(search).selectize({
                load: function() {
                }
            });
        }

        var createFunction = null;

        function emptyContent() {
            while (content.firstChild) {
                content.removeChild(content.firstChild);
            }
        }

        var mpdContent = window.mpd.create(
            "new-group",
            "700px",
            i18n.t("wisetank.addgroup"),
            {
                closebtn: true,
                buttons: [
                    {
                        text: i18n.t("New"),
                        action: function() {
                            emptyContent();

                            if (createFunction !== null) {
                                $(create).off("click");
                            }
                            createFunction = showNewGroup(content, create);
                            $(create).on("click", createFunction);

                            window.mpd.resize();
                        }
                    } //,
                    // TODO
                    //{
                    //    text: i18n.t("Search"),
                    //    action: function () {
                    //        emptyContent();

                    //        if (createFunction !== null) {
                    //            $(create).off("click");
                    //        }
                    //        createFunction = showSearch(content, create);
                    //        $(create).on("click", createFunction);

                    //        window.mpd.resize();
                    //    }
                    //}
                ]
            }
        );

        mpdContent.appendChild(content);
        mpdContent.appendChild(buttons);

        $(close).on("click", function() {
            $(mpdContent).parents(".mb-box").eq(0).find(".mb-close").eq(0).trigger("click");
        });
    }

    function groupAddColumn() {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var mpdContent = window.mpd.create(
            "group-add-column",
            "700px",
            i18n.t("wisetank.addcolumn"),
            { closebtn: true }
        );

        var content = document.createElement("div");
        content.id = "wt-grp-pup-agc";
        mpdContent.appendChild(content);

        var name = document.createElement("input");
        name.id = "wt-grp-pup-nw-nme";
        name.className = "input";
        name.placeholder = i18n.t("Name"),
            content.appendChild(name);

        var ttlevelContainer = document.createElement("div");
        ttlevelContainer.id = "wt-grp-pup-nw-ttl-w";
        content.appendChild(ttlevelContainer);

        var ttlevelText = document.createElement("span");
        ttlevelText.id = "wt-grp-pup-nw-ttl-t";
        ttlevelText.innerText = i18n.t("wisetank.ttlevel") + ": ";
        ttlevelContainer.appendChild(ttlevelText);

        var ttlevelValue = document.createElement("span");
        ttlevelValue.id = "wt-grp-pup-nw-ttl-v";
        ttlevelValue.innerText = 0;
        ttlevelText.appendChild(ttlevelValue);

        var ttlevelWrapper = document.createElement("div");
        ttlevelWrapper.id = "wt-grp-pup-nw-ttl-sw";
        ttlevelContainer.appendChild(ttlevelWrapper);

        var ttlevel = document.createElement("div");
        ttlevel.id = "wt-grp-pup-nw-ttl";
        ttlevelWrapper.appendChild(ttlevel);

        $(ttlevel).noUiSlider({
            start: [0],
            step: 1,
            orientation: "horizontal",
            connect: "lower",
            range: {
                'min': 0,
                'max': 9
            },
            format: wNumb({
                decimals: 0
            })
        });

        $(ttlevel).Link("lower").to($(ttlevelValue));

        var accessDiv = document.createElement("div");
        accessDiv.id = "wt-grp-pup-nw-actd";
        content.appendChild(accessDiv);

        var accessSelected = 1;

        var publicAccess = document.createElement("button");
        publicAccess.className = "input input-bl input-nh";
        publicAccess.innerText = i18n.t("Public");
        publicAccess.dataset.id = 1;
        accessDiv.appendChild(publicAccess);

        var privateAccess = document.createElement("button");
        privateAccess.className = "input input-gr";
        privateAccess.innerText = i18n.t("Private");
        privateAccess.dataset.id = 3;
        accessDiv.appendChild(privateAccess);

        $(accessDiv).on("click", ".input", function() {
            accessSelected = this.dataset.id;

            publicAccess.className = "input input-gr";
            privateAccess.className = "input input-gr";

            this.className = "input input-bl input-nh";
        });

        var moderationDiv = document.createElement("div");
        moderationDiv.id = "wt-grp-pup-nw-mdd";
        content.appendChild(moderationDiv);

        var articleModerationDiv = document.createElement("div");
        articleModerationDiv.id = "wt-grp-pup-nw-amd";
        articleModerationDiv.className = "wt-grp-pup-nw-md";
        moderationDiv.appendChild(articleModerationDiv);

        var articleCheck = document.createElement("input");
        articleCheck.className = "mini";
        articleCheck.type = "checkbox";
        articleCheck.id = "wt-grp-pup-nw-am";
        articleModerationDiv.appendChild(articleCheck);

        var articlelabelCheck = document.createElement("label");
        articlelabelCheck.className = "option";
        articlelabelCheck.htmlFor = "wt-grp-pup-nw-am";
        articleModerationDiv.appendChild(articlelabelCheck);

        var articleLabelText = document.createElement("label");
        articleLabelText.className = "wt-grp-pup-nw-mlt";
        articleLabelText.innerText = i18n.t("wisetank.moderation.articles");
        articleLabelText.htmlFor = "wt-grp-pup-nw-am";
        articleModerationDiv.appendChild(articleLabelText);

        var commentModerationDiv = document.createElement("div");
        commentModerationDiv.id = "wt-grp-pup-nw-cmd";
        commentModerationDiv.className = "wt-grp-pup-nw-md";
        moderationDiv.appendChild(commentModerationDiv);

        var moderationArticleCheck = document.createElement("input");
        moderationArticleCheck.className = "mini";
        moderationArticleCheck.type = "checkbox";
        moderationArticleCheck.id = "wt-grp-pup-nw-cm";
        commentModerationDiv.appendChild(moderationArticleCheck);

        var moderationlabelCheck = document.createElement("label");
        moderationlabelCheck.className = "option";
        moderationlabelCheck.htmlFor = "wt-grp-pup-nw-cm";
        commentModerationDiv.appendChild(moderationlabelCheck);

        var moderationLabelText = document.createElement("label");
        moderationLabelText.className = "wt-grp-pup-nw-mlt";
        moderationLabelText.innerText = i18n.t("wisetank.moderation.comments");
        moderationLabelText.htmlFor = "wt-grp-pup-nw-cm";
        commentModerationDiv.appendChild(moderationLabelText);

        var writeArticlesDiv = document.createElement("div");
        writeArticlesDiv.id = "wt-grp-pup-nw-wod";
        writeArticlesDiv.className = "wt-grp-pup-nw-md";
        moderationDiv.appendChild(writeArticlesDiv);

        var writeArticlesCheck = document.createElement("input");
        writeArticlesCheck.className = "mini";
        writeArticlesCheck.type = "checkbox";
        writeArticlesCheck.id = "wt-grp-pup-nw-wo";
        writeArticlesDiv.appendChild(writeArticlesCheck);

        var writeLabelCheck = document.createElement("label");
        writeLabelCheck.className = "option";
        writeLabelCheck.htmlFor = "wt-grp-pup-nw-wo";
        writeArticlesDiv.appendChild(writeLabelCheck);

        var writeLabelText = document.createElement("label");
        writeLabelText.className = "wt-grp-pup-nw-mlt";
        writeLabelText.innerText = i18n.t("wisetank.moderation.write");
        writeLabelText.htmlFor = "wt-grp-pup-nw-wo";
        writeArticlesDiv.appendChild(writeLabelText);

        var buttons = document.createElement("div");
        buttons.id = "wt-str-pup-bt";
        mpdContent.appendChild(buttons);

        var create = document.createElement("button");
        create.id = "wt-str-pup-ct";
        create.className = "input input-bl";
        create.innerText = i18n.t("Create");
        create.disabled = true;
        buttons.appendChild(create);

        var close = document.createElement("button");
        close.id = "wt-str-pup-cl";
        close.className = "input input-gr";
        close.innerText = i18n.t("Close");
        buttons.appendChild(close);

        $(name).on("input", function() {
            create.disabled = (name.value === null || name.value.trim() === "");
        });

        $(create).on("click", function() {
            if (create.disabled) {
                return;
            }

            window.mpd.block();
            name.disabled = true;
            publicAccess.disabled = true;
            privateAccess.disabled = true;
            articleCheck.disabled = true;
            moderationArticleCheck.disabled = true;
            writeArticlesCheck.disabled = true;
            create.disabled = true;
            close.disabled = true;

            var columnName = name.value,
                columnAccess = accessDiv.getElementsByClassName("input-bl")[0].dataset.id,
                columnModerateArticles = articleCheck.checked,
                columnModerateComments = moderationArticleCheck.checked,
                columnWriteArticles = writeArticlesCheck.checked;

            $.ajax({
                async: true,
                type: "POST",
                url: api.createColumn,
                data: {
                    timeline: actualGroup,
                    name: columnName,
                    ttlevel: $(ttlevel).val(),
                    accessType: columnAccess,
                    moderatedArticles: columnModerateArticles,
                    moderatedComments: columnModerateComments,
                    writeOwnArticles: columnWriteArticles,
                    offsetDate: window.user.offsetDate
                },

                success: function(result) {
                    if (result.status === 0) {
                        /*var stream = {
                            id: result.id,
                            name: name.value,
                            position: streams.length,
                            boxesWidth: 3,
                            boxes: [],
                            refreshRate: 5
                        };

                        streams.push(stream);
                        var streamElement = drawStream(stream, streams.length - 1, true);
                        selectStream(streamElement);*/
                        window.mpd.unblock();
                        window.mpd.hide();
                    } else {
                        createNoty("warning", "center", i18n.t("internal-error"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    window.mpd.unblock();
                                    name.disabled = false;
                                    publicAccess.disabled = false;
                                    privateAccess.disabled = false;
                                    articleCheck.disabled = false;
                                    moderationArticleCheck.disabled = false;
                                    writeArticlesCheck.disabled = false;
                                    create.disabled = false;
                                    close.disabled = false;
                                }
                            }
                        });
                    }
                },

                error: function() {
                    createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                        callback: {
                            afterClose: function() {
                                window.mpd.unblock();
                                name.disabled = false;
                                publicAccess.disabled = false;
                                privateAccess.disabled = false;
                                articleCheck.disabled = false;
                                moderationArticleCheck.disabled = false;
                                writeArticlesCheck.disabled = false;
                                create.disabled = false;
                                close.disabled = false;
                            }
                        }
                    });
                }
            });
        });

        $(close).on("click", function() {
            $(mpdContent).parents(".mb-box").eq(0).find(".mb-close").eq(0).trigger("click");
        });
    }

    function editGroup() {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var mpdContent = window.mpd.create(
            "edit-group",
            "700px",
            i18n.t("wisetank.editgroup"),
            { closebtn: true, overflow: true }
        );

        var content = document.createElement("div");
        content.id = "wt-grp-pup-agc";
        mpdContent.appendChild(content);

        var buttons = document.createElement("div");
        buttons.id = "wt-str-pup-bt";
        mpdContent.appendChild(buttons);

        var save = document.createElement("button");
        save.id = "wt-str-pup-ct";
        save.className = "input input-bl";
        save.innerText = i18n.t("Save");
        save.disabled = true;
        buttons.appendChild(save);

        var close = document.createElement("button");
        close.id = "wt-str-pup-cl";
        close.className = "input input-gr";
        close.innerText = i18n.t("Close");
        buttons.appendChild(close);

        function showEditGroup(content, save, timeline) {
            var name = document.createElement("input");
            name.id = "wt-grp-pup-nw-nme";
            name.className = "input";
            name.type = "text";
            name.placeholder = i18n.t("Name");
            name.value = timeline.name;
            content.appendChild(name);

            var description = document.createElement("input");
            description.id = "wt-grp-pup-nw-dsc";
            description.className = "input";
            description.type = "text";
            description.placeholder = i18n.t("Description") + " (" + i18n.t("optional") + ")";
            description.value = timeline.description;
            content.appendChild(description);

            var accessDiv = document.createElement("div");
            accessDiv.id = "wt-grp-pup-nw-atd";
            content.appendChild(accessDiv);

            var accessSelected = timeline.accessType;

            var publicAccess = document.createElement("button");
            publicAccess.className = "input " + (accessSelected === 1 ? "input-bl input-nh" : "input-gr");
            publicAccess.innerText = i18n.t("Public");
            publicAccess.dataset.id = 1;
            accessDiv.appendChild(publicAccess);

            var protectedAccess = document.createElement("button");
            protectedAccess.className = "input " + (accessSelected === 2 ? "input-bl input-nh" : "input-gr");
            protectedAccess.innerText = i18n.t("InviteRequired");
            protectedAccess.dataset.id = 2;
            accessDiv.appendChild(protectedAccess);

            var privateAccess = document.createElement("button");
            privateAccess.className = "input " + (accessSelected === 3 ? "input-bl input-nh" : "input-gr");
            privateAccess.innerText = i18n.t("Private");
            privateAccess.dataset.id = 3;
            accessDiv.appendChild(privateAccess);

            $(accessDiv).on("click", ".input", function() {
                accessSelected = this.dataset.id;

                publicAccess.className = "input input-gr";
                protectedAccess.className = "input input-gr";
                privateAccess.className = "input input-gr";

                this.className = "input input-bl input-nh";
            });

            var moderationDiv = document.createElement("div");
            moderationDiv.id = "wt-grp-pup-nw-mdd";
            content.appendChild(moderationDiv);

            var articleModerationDiv = document.createElement("div");
            articleModerationDiv.id = "wt-grp-pup-nw-amd";
            articleModerationDiv.className = "wt-grp-pup-nw-md";
            moderationDiv.appendChild(articleModerationDiv);

            var articleCheck = document.createElement("input");
            articleCheck.className = "mini";
            articleCheck.type = "checkbox";
            articleCheck.id = "wt-grp-pup-nw-am";
            articleCheck.checked = timeline.moderatedArticles;
            articleModerationDiv.appendChild(articleCheck);

            var articlelabelCheck = document.createElement("label");
            articlelabelCheck.className = "option";
            articlelabelCheck.htmlFor = "wt-grp-pup-nw-am";
            articleModerationDiv.appendChild(articlelabelCheck);

            var articleLabelText = document.createElement("label");
            articleLabelText.className = "wt-grp-pup-nw-mlt";
            articleLabelText.innerText = i18n.t("wisetank.moderation.articles");
            articleLabelText.htmlFor = "wt-grp-pup-nw-am";
            articleModerationDiv.appendChild(articleLabelText);

            var commentModerationDiv = document.createElement("div");
            commentModerationDiv.id = "wt-grp-pup-nw-cmd";
            commentModerationDiv.className = "wt-grp-pup-nw-md";
            moderationDiv.appendChild(commentModerationDiv);

            var moderationArticleCheck = document.createElement("input");
            moderationArticleCheck.className = "mini";
            moderationArticleCheck.type = "checkbox";
            moderationArticleCheck.id = "wt-grp-pup-nw-cm";
            moderationArticleCheck.checked = timeline.moderatedComments;
            commentModerationDiv.appendChild(moderationArticleCheck);

            var moderationlabelCheck = document.createElement("label");
            moderationlabelCheck.className = "option";
            moderationlabelCheck.htmlFor = "wt-grp-pup-nw-cm";
            commentModerationDiv.appendChild(moderationlabelCheck);

            var moderationLabelText = document.createElement("label");
            moderationLabelText.className = "wt-grp-pup-nw-mlt";
            moderationLabelText.innerText = i18n.t("wisetank.moderation.comments");
            moderationLabelText.htmlFor = "wt-grp-pup-nw-cm";
            commentModerationDiv.appendChild(moderationLabelText);

            var writeArticlesDiv = document.createElement("div");
            writeArticlesDiv.id = "wt-grp-pup-nw-wod";
            writeArticlesDiv.className = "wt-grp-pup-nw-md";
            moderationDiv.appendChild(writeArticlesDiv);

            var writeArticlesCheck = document.createElement("input");
            writeArticlesCheck.className = "mini";
            writeArticlesCheck.type = "checkbox";
            writeArticlesCheck.id = "wt-grp-pup-nw-wo";
            writeArticlesCheck.checked = timeline.writeOwnArticles;
            writeArticlesDiv.appendChild(writeArticlesCheck);

            var writeLabelCheck = document.createElement("label");
            writeLabelCheck.className = "option";
            writeLabelCheck.htmlFor = "wt-grp-pup-nw-wo";
            writeArticlesDiv.appendChild(writeLabelCheck);

            var writeLabelText = document.createElement("label");
            writeLabelText.className = "wt-grp-pup-nw-mlt";
            writeLabelText.innerText = i18n.t("wisetank.moderation.write");
            writeLabelText.htmlFor = "wt-grp-pup-nw-wo";
            writeArticlesDiv.appendChild(writeLabelText);

            var rolesDiv = document.createElement("div");
            rolesDiv.id = "wt-grp-pup-nw-rld";
            content.appendChild(rolesDiv);

            var rolesTitle = document.createElement("div");
            rolesTitle.id = "wt-grp-pup-nw-rlt";
            rolesTitle.innerText = i18n.t("Roles");
            rolesDiv.appendChild(rolesTitle);

            var rolesSelector = document.createElement("div");
            rolesSelector.id = "wt-grp-pup-nw-rls";
            rolesDiv.appendChild(rolesSelector);

            var permissionTypes = [];

            var roleCount = 0,
                roleButtons = [];
            for (var i = 0, l = roles.length; i < l; i++) {
                if (roles[i].selectable) {
                    var role = document.createElement("button");
                    role.dataset.id = i;
                    role.className = "wt-grp-pup-nw-rle input input-gr mini";
                    role.innerText = i18n.t("wisetank.roles." + roles[i].name);
                    rolesSelector.appendChild(role);
                    if (timeline.permissionTypes[roles[i].id] === undefined || timeline.permissionTypes[roles[i].id] === null) {
                        permissionTypes[i] = -1;
                    } else {
                        permissionTypes[i] = timeline.permissionTypes[roles[i].id];
                    }
                    roleCount++;
                    roleButtons.push(role);
                }
            }

            var roleWidth = (100 / roleCount) + "%";
            for (var i = 0, l = roleButtons.length; i < l; i++) {
                roleButtons[i].style.width = roleWidth;
            }

            var permissionSelector = document.createElement("select");
            permissionSelector.id = "wt-grp-pup-nw-rlp";
            permissionSelector.className = "input";
            var permissionPlaceholder = document.createAttribute("placeholder");
            permissionPlaceholder.value = i18n.t("wisetank.selectPermissionType");
            permissionSelector.attributes.setNamedItem(permissionPlaceholder);
            rolesDiv.appendChild(permissionSelector);

            var permissionsContent = document.createElement("div");
            permissionsContent.id = "wt-grp-pup-nw-prc";
            rolesDiv.appendChild(permissionsContent);
            drawPermissionSelectors(permissionsContent, true);

            function checkSaveEnabled() {
                var hasPermissions = true;

                for (var permission in permissionTypes) {
                    if (Object.prototype.hasOwnProperty.call(permissionTypes, permission) && permissionTypes[permission] === null) {
                        hasPermissions = false;
                        break;
                    }
                }

                save.disabled = !(
                    (name.value !== null && name.value.trim() !== "")
                        && hasPermissions
                );
            }

            $(permissionSelector).selectize({
                onChange: function(value) {
                    var permissions;

                    if (value !== "" && value !== "-1") {
                        var permissionValue = permissionSelector.selectize.options[value];
                        permissions = roles[permissionValue.role].permissions[permissionValue.value].permissions;

                        for (var permission in permissions) {
                            if (Object.prototype.hasOwnProperty.call(permissions, permission)) {
                                var element = document.getElementById(permission);
                                if (element !== undefined && element !== null) {
                                    element.checked = permissions[permission];
                                }
                            }
                        }

                        permissionTypes[permissionValue.role] = roles[permissionValue.role].permissions[permissionValue.value].id;
                    } else {
                        permissions = permissionsContent.getElementsByClassName("wt-grp-pup-prm");
                        for (var i = 0, l = permissions.length; i < l; i++) {
                            permissions[i].getElementsByTagName("input")[0].checked = false;
                        }
                    }

                    checkSaveEnabled();
                }
            });
            permissionSelector.selectize.disable();

            function setPermissions(id) {
                permissionSelector.selectize.clearOptions();
                permissionSelector.selectize.clear();

                permissionSelector.selectize.addOption({
                    value: -1,
                    text: i18n.t("wisetank.permissionTypes.disabled"),
                    role: id
                });

                for (var i = 0, l = roles[id].permissions.length; i < l; i++) {
                    permissionSelector.selectize.addOption({
                        value: i,
                        text: i18n.t("wisetank.permissionTypes." + roles[id].permissions[i].name),
                        role: id
                    });
                }

                permissionSelector.selectize.enable();
                permissionSelector.selectize.refreshOptions(false);

                var index = -1;

                for (var i = 0, l = roles[id].permissions.length; i < l; i++) {
                    if (roles[id].permissions[i].id === permissionTypes[id]) {
                        index = i;
                        break;
                    }
                }

                permissionSelector.selectize.setValue(index);
            }

            $(rolesSelector).on("click", ".wt-grp-pup-nw-rle", function() {
                if (!this.classList.contains("input-bl")) {
                    var roles = rolesSelector.getElementsByClassName("wt-grp-pup-nw-rle");
                    for (var i = 0, l = roles.length; i < l; i++) {
                        roles[i].classList.remove("input-bl", "input-nh");
                        roles[i].classList.add("input-gr");
                    }

                    this.classList.remove("input-gr");
                    this.classList.add("input-bl", "input-nh");
                    setPermissions(parseInt(this.dataset.id));
                }
            });

            $(rolesSelector.getElementsByClassName("wt-grp-pup-nw-rle")[0]).trigger("click");

            $(name).on("input", function() {
                checkSaveEnabled();
            });

            $(save).on("click", function() {
                if (save.disabled) {
                    return;
                }

                window.mpd.block();
                name.disabled = true;
                description.disabled = true;
                publicAccess.disabled = true;
                protectedAccess.disabled = true;
                privateAccess.disabled = true;
                articleCheck.disabled = true;
                moderationArticleCheck.disabled = true;
                writeArticlesCheck.disabled = true;
                $(rolesSelector).find(".wt-grp-pup-nw-rle").prop("disabled", true);
                permissionSelector.selectize.disable();
                save.disabled = true;
                close.disabled = true;

                var mpdButtons = window.mpd.buttons();
                for (var i = 0, l = mpdButtons.length; i < l; i++) {
                    mpdButtons[i].disabled = true;
                }

                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.editGroup,
                    data: {
                        timeline: timeline.id,
                        name: name.value,
                        description: description.value,
                        accessType: accessDiv.getElementsByClassName("input-bl")[0].dataset.id,
                        moderatedArticles: articleCheck.checked,
                        moderatedComments: moderationArticleCheck.checked,
                        writeOwnArticles: writeArticlesCheck.checked,
                        permissionTypes: permissionTypes.reduce(function(arr, value, index) {
                            if (i !== undefined) {
                                arr.push({
                                    role: roles[index].id,
                                    permissionType: value
                                });
                            }
                            return arr;
                        }, [])
                    },

                    success: function(result) {
                        if (result === 0) {
                            window.mpd.unblock();
                            window.mpd.hide();
                        } else {
                            createNoty("error", "center", i18n.t("error-processing"), 1500, ["click"], {
                                callback: {
                                    afterClose: function() {
                                        window.mpd.unblock();
                                        name.disabled = false;
                                        description.disabled = false;
                                        publicAccess.disabled = false;
                                        protectedAccess.disabled = false;
                                        privateAccess.disabled = false;
                                        articleCheck.disabled = false;
                                        moderationArticleCheck.disabled = false;
                                        writeArticlesCheck.disabled = false;
                                        $(rolesSelector).find(".wt-grp-pup-nw-rle").prop("disabled", false);
                                        permissionSelector.selectize.enable();
                                        save.disabled = false;
                                        close.disabled = false;

                                        var mpdButtons = window.mpd.buttons();
                                        for (var i = 0, l = mpdButtons.length; i < l; i++) {
                                            mpdButtons[i].disabled = false;
                                        }
                                    }
                                }
                            });
                        }
                    },

                    error: function() {
                        createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    window.mpd.unblock();
                                    name.disabled = false;
                                    description.disabled = false;
                                    publicAccess.disabled = false;
                                    protectedAccess.disabled = false;
                                    privateAccess.disabled = false;
                                    articleCheck.disabled = false;
                                    moderationArticleCheck.disabled = false;
                                    writeArticlesCheck.disabled = false;
                                    $(rolesSelector).find(".wt-grp-pup-nw-rle").prop("disabled", false);
                                    permissionSelector.selectize.enable();
                                    save.disabled = false;
                                    close.disabled = false;

                                    var mpdButtons = window.mpd.buttons();
                                    for (var i = 0, l = mpdButtons.length; i < l; i++) {
                                        mpdButtons[i].disabled = false;
                                    }
                                }
                            }
                        });
                    }
                });
            });
        }

        $(close).on("click", function() {
            $(mpdContent).parents(".mb-box").eq(0).find(".mb-close").eq(0).trigger("click");
        });

        for (var i = 0, l = timelines.length; i < l; i++) {
            if (timelines[i].id === actualGroup) {
                showEditGroup(content, save, timelines[i]);
                break;
            }
        }
    }

    function groupUsers() {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        var timeline;

        for (var i = 0, l = timelines.length; i < l; i++) {
            if (timelines[i].id === actualGroup) {
                timeline = timelines[i];
                break;
            }
        }

        var actualStatus = null;


        var mpdContent = window.mpd.create(
            "group-users",
            "850px",
            i18n.t("Users"),
            {
                closebtn: true
            }
        );

        var spinner = document.createElement("div");
        spinner.className = "app-spinner app-spinner-hide";
        mpdContent.appendChild(spinner);

        var wrapper = document.createElement("div");
        wrapper.id = "wt-grp-pup-flw";
        mpdContent.appendChild(wrapper);

        var timelineSelector = document.createElement("select");
        timelineSelector.id = "wt-grp-pup-tls";
        timelineSelector.className = "input";
        wrapper.appendChild(timelineSelector);

        var content = document.createElement("div");
        content.id = "wt-grp-pup-agc";
        content.className = "wt-grp-pup-agc-gus";
        wrapper.appendChild(content);


        function showRoles(element, maxTtl) {
            var thinktankWrapper = document.createElement("div");
            thinktankWrapper.id = "wt-grp-pup-used-ttlv";
            element.appendChild(thinktankWrapper);

            var ttlTitle = document.createElement("span");
            ttlTitle.className = "wt-grp-pup-used-ttl";
            ttlTitle.innerText = i18n.t("wisetank.ttlevel");
            thinktankWrapper.appendChild(ttlTitle);

            var ttlNumber = document.createElement("span");
            ttlNumber.id = "wt-grp-pup-used-ttlv-v";
            thinktankWrapper.appendChild(ttlNumber);

            var ttLevel = document.createElement("div");
            ttLevel.id = "wt-grp-pup-used-ttlv-sl";
            thinktankWrapper.appendChild(ttLevel);

            var $ttLevel = $(ttLevel);
            $ttLevel.noUiSlider({
                start: [0],
                step: 1,
                orientation: "horizontal",
                connect: "lower",
                range: {
                    'min': 0,
                    'max': maxTtl
                },
                format: wNumb({
                    decimals: 0
                })
            });

            $ttLevel.Link("lower").to($(ttlNumber));

            var rolesDiv = document.createElement("div");
            rolesDiv.id = "wt-grp-pup-used-rle";
            element.appendChild(rolesDiv);

            var rolesTitle = document.createElement("span");
            rolesTitle.className = "wt-grp-pup-used-ttl";
            rolesTitle.innerText = i18n.t("Role");
            rolesDiv.appendChild(rolesTitle);

            var rolesSelector = document.createElement("select");
            rolesSelector.id = "wt-grp-pup-used-rles";
            rolesSelector.className = "input";
            rolesDiv.appendChild(rolesSelector);

            var permissionDiv = document.createElement("div");
            permissionDiv.id = "wt-grp-pup-used-prm";
            permissionDiv.className = "clearfix";
            element.appendChild(permissionDiv);

            var permissionTitle = document.createElement("span");
            permissionTitle.className = "wt-grp-pup-used-ttl";
            permissionTitle.innerText = i18n.t("Permissions");
            permissionDiv.appendChild(permissionTitle);

            var permissionSelector = document.createElement("select");
            permissionSelector.className = "input";
            permissionDiv.appendChild(permissionSelector);

            var permissionList = document.createElement("div");
            permissionList.id = "wt-grp-pup-used-prl";
            permissionDiv.appendChild(permissionList);

            drawPermissionSelectors(permissionList, true);

            return [rolesSelector, permissionSelector, thinktankWrapper, $ttLevel];
        }

        function showView(tl, element, rolePosition, permissionPosition, thinktankLevel) {
            actualStatus = "view";

            var userContainer = document.createElement("div");
            userContainer.id = "wt-grpp-pup-usrc";
            element.appendChild(userContainer);

            for (var i = 0, l = timeline.users[tl.id].length; i < l; i++) {
                var userDiv = document.createElement("div");
                userDiv.className = "wt-grp-pup-usr";
                userDiv.dataset.id = i;
                userDiv.dataset.role = timeline.users[tl.id][i].role;
                userContainer.appendChild(userDiv);

                var userNameData = document.createElement("div");
                userNameData.className = "wt-grp-pup-usr-und";
                userDiv.appendChild(userNameData);

                var userNameContainer = document.createElement("div");
                userNameContainer.className = "wt-grp-pup-usr-unc";
                userNameData.appendChild(userNameContainer);

                var userNameContainerW = document.createElement("div");
                userNameContainerW.className = "wt-grp-pup-usr-nmc";
                userNameContainer.appendChild(userNameContainerW);

                var userFullName = document.createElement("div");
                userFullName.className = "wt-grp-pup-usr-fnm";
                userFullName.innerText = timeline.users[tl.id][i].fullName;
                userNameContainerW.appendChild(userFullName);

                var userName = document.createElement("span");
                userName.className = "wt-grp-pup-usr-nme";
                userName.innerText = "@" + timeline.users[tl.id][i].userName;
                userNameContainerW.appendChild(userName);

                var userRole = document.createElement("div");
                userRole.className = "wt-grp-pup-usr-rle";
                for (var j = 0, k = roles.length; j < k; j++) {
                    if (roles[j].id === timeline.users[tl.id][i].role) {

                        if (timeline.hasPermissions[tl.id]) {
                            var userTtl = timeline.users[tl.id][i].thinktankLevel;

                            if (!roles[j].selectable
                                || roles[j].position > rolePosition
                                || (roles[j].position === rolePosition && (thinktankLevel === null || (userTtl !== null && thinktankLevel <= userTtl)))) {
                                userDiv.classList.add("wt-grp-pup-usr-nslc");
                            } else {
                                if (timeline.users[tl.id][i].customPermission) {
                                    userDiv.className += " wt-grp-pup-usr-slc";
                                } else {
                                    userDiv.className += " wt-grp-pup-usr-slc";
                                }
                            }
                        }

                        userRole.innerText = i18n.t("wisetank.roles." + roles[j].name);
                        break;
                    }
                }

                userDiv.appendChild(userRole);
            }
        }

        function showEdit(tl, miniContent) {
            actualStatus = "edit";

            var userList = document.createElement("div");
            userList.id = "wt-grp-pup-uslt";
            if (timeline.hasPermissions[tl.id]) {
                userList.className = "wt-grp-pup-uslt-hf";
            }
            miniContent.appendChild(userList);

            var currentRolePosition = 0,
                currentPermissionPosition = 0;

            var selectedRole = null,
                selectedPermissions = null;

            for (var i = 0, l = roles.length; i < l; i++) {
                if (roles[i].id === tl.role) {
                    currentRolePosition = roles[i].position;
                    for (var j = 0, k = roles[i].permissions.length; j < k; j++) {
                        if (roles[i].permissions[j].id === tl.permissionType) {
                            currentPermissionPosition = roles[i].permissions[j].position;
                            break;
                        }
                    }
                    break;
                }
            }

            showView(tl, userList, currentRolePosition, currentPermissionPosition, timeline.userThinktank);

            var userDataList = document.createElement("div");
            userDataList.id = "wt-grp-pup-used";
            miniContent.appendChild(userDataList);
            var rp = showRoles(userDataList, Math.max(timeline.userThinktank - 1, 0));
            rp[3].attr("disabled", "disabled");

            var saveButton = document.createElement("button");
            saveButton.id = "wt-grp-pup-uslt-sv";
            saveButton.className = "input input-bl";
            saveButton.innerText = i18n.t("Save");
            saveButton.disabled = true;
            userDataList.appendChild(saveButton);


            $(rp[0]).selectize({
                onChange: function(value) {
                    rp[1].selectize.clearOptions();
                    rp[1].selectize.clear();

                    if (value === null) {
                        rp[1].selectize.disable();
                        rp[3].attr("disabled", "disabled");
                    } else {
                        rp[1].selectize.enable();

                        rp[1].selectize.addOption({
                            value: -2,
                            text: i18n.t("Default")
                        });

                        //rp[1].selectize.addOption({
                        //    value: -1,
                        //    text: i18n.t("Custom")
                        //});

                        for (var i = 0, l = roles[value].permissions.length; i < l; i++) {
                            if (roles[value].permissions[i].position <= currentPermissionPosition) {
                                rp[1].selectize.addOption({
                                    value: roles[value].permissions[i].id,
                                    text: i18n.t("wisetank.permissionTypes." + roles[value].permissions[i].name)
                                });
                            }
                        }


                        rp[1].selectize.refreshOptions(false);
                        rp[1].selectize.enable();
                        rp[1].selectize.setValue(-2, false);

                        //checkSaveButton();

                        selectedRole = roles[value].id;
                        selectedPermissions = -2;
                    }
                }
            });

            rp[0].selectize.disable();

            for (var i = 0, l = roles.length; i < l; i++) {
                if (roles[i].selectable
                    && (roles[i].position < currentRolePosition || roles[i].position === currentRolePosition)
                    && timeline.permissionTypes[roles[i].id] !== undefined) {
                    rp[0].selectize.addOption({
                        value: i,
                        text: i18n.t("wisetank.roles." + roles[i].name)
                    });
                }
            }

            rp[0].selectize.refreshOptions(false);

            $(rp[1]).selectize({
                onChange: function(value) {
                    var role = roles[rp[0].selectize.getValue()];

                    switch (parseInt(value)) {
                    case -2: // DEFAULT
                        for (var permission in allPermissions) {
                            if (Object.prototype.hasOwnProperty.call(allPermissions, permission)) {
                                var element = document.getElementById(allPermissions[permission]);
                                if (element !== undefined && element !== null) {
                                    element.disabled = true;
                                    var wrapper = element.ancestor(".wt-grp-pup-prm", true);
                                    wrapper.classList.remove("wt-grp-pup-prmd", "wt-grp-pup-prma");
                                }
                            }
                        }

                        var permissionId = timeline.defaultPermissions[role.id],
                            rolePermissions = null;

                        for (var i = 0, l = role.permissions.length; i < l; i++) {
                            if (role.permissions[i].id === permissionId) {
                                rolePermissions = role.permissions[i].permissions;
                                break;
                            }
                        }

                        for (var permission in rolePermissions) {
                            if (Object.prototype.hasOwnProperty.call(rolePermissions, permission)) {
                                var element = document.getElementById(permission);
                                if (element !== undefined && element !== null) {
                                    element.checked = rolePermissions[permission];
                                }
                            }
                        }
                        break;

                    case -1: // CUSTOM
                        for (var permission in allPermissions) {
                            if (Object.prototype.hasOwnProperty.call(allPermissions, permission)) {
                                var element = document.getElementById(allPermissions[permission]);
                                if (element !== undefined && element !== null) {
                                    var wrapper = element.ancestor(".wt-grp-pup-prm", true);

                                    if (tl.permissions[allPermissions[permission]]) {
                                        wrapper.classList.remove("wt-grp-pup-prmd");
                                        wrapper.classList.add("wt-grp-pup-prma");
                                        element.disabled = false;
                                        element.checked = true;
                                    } else {
                                        wrapper.classList.add("wt-grp-pup-prmd");
                                        wrapper.classList.remove("wt-grp-pup-prma");
                                        element.disabled = true;
                                        element.checked = false;
                                    }
                                }
                            }
                        }

                        break;

                    default: // OTHER
                        for (var permission in allPermissions) {
                            if (Object.prototype.hasOwnProperty.call(allPermissions, permission)) {
                                var element = document.getElementById(allPermissions[permission]);
                                if (element !== undefined && element !== null) {
                                    element.disabled = true;
                                    var wrapper = element.ancestor(".wt-grp-pup-prm", true);
                                    wrapper.classList.remove("wt-grp-pup-prmd", "wt-grp-pup-prma");
                                }
                            }
                        }

                        var permissionId = parseInt(value),
                            rolePermissions = null;
                        for (var i = 0, l = role.permissions.length; i < l; i++) {
                            if (role.permissions[i].id === permissionId) {
                                rolePermissions = role.permissions[i].permissions;
                                break;
                            }
                        }

                        for (var permission in rolePermissions) {
                            if (Object.prototype.hasOwnProperty.call(rolePermissions, permission)) {
                                var element = document.getElementById(permission);
                                if (element !== undefined && element !== null) {
                                    element.checked = rolePermissions[permission];
                                }
                            }
                        }
                        break;
                    }

                    selectedPermissions = value;
                }
            });

            rp[1].selectize.disable();

            var selectedUser;

            $(userList).on("click", ".wt-grp-pup-usr", function() {
                if (!this.classList.contains("wt-grp-pup-usr-slc")
                    || this.classList.contains("wt-grp-pup-usr-slct")
                    || this.classList.contains("wt-grp-pup-usr-nslc")) {
                    return;
                }

                selectedUser = this.dataset.id;

                var users = userList.getElementsByClassName("wt-grp-pup-usr");
                for (var i = 0, l = users.length; i < l; i++) {
                    users[i].classList.remove("wt-grp-pup-usr-slct");
                }

                this.classList.add("wt-grp-pup-usr-slct");
                rp[0].selectize.enable();
                rp[1].selectize.enable();

                for (var i = 0, l = roles.length; i < l; i++) {
                    if (roles[i].id === timeline.users[tl.id][this.dataset.id].role) {
                        rp[0].selectize.setValue(i);
                    }
                }

                if (timeline.users[tl.id][this.dataset.id].defaultPermission) {
                    rp[1].selectize.setValue(-2, false);
                } else if (timeline.users[tl.id][this.dataset.id].customPermission) {
                    rp[1].selectize.setValue(-1, false);
                } else {
                    rp[1].selectize.setValue(timeline.users[tl.id][this.dataset.id].permissionType, false);
                }

                rp[3].attr("disabled", false);
                rp[3].val(timeline.users[timeline.id][this.dataset.id].thinktankLevel);
                saveButton.disabled = false;
            });

            $(saveButton).on("click", function() {
                window.mpd.block();
                var buttons = window.mpd.buttons();
                for (var i = 0, l = buttons.length; i < l; i++) {
                    buttons[i].disabled = true;
                }
                timelineSelector.selectize.disable();
                rp[3].attr("disabled", "disabled");
                rp[0].selectize.disable();
                rp[1].selectize.disable();
                saveButton.disabled = true;

                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.editTimelineUser,
                    data: {
                        timeline: tl.id,
                        user: selectedUser,
                        thinktankLevel: rp[3].val(),
                        role: selectedRole,
                        permissions: selectedPermissions
                    },

                    success: function(result) {
                        var showNoty, notyTitle = null;

                        switch (result) {
                        case 0:
                            showNoty = false;
                            break;

                        default:
                            showNoty = true;
                            notyTitle = i18n.t("error-processing");
                            break;
                        }

                        if (showNoty) {
                            createNoty("warning", "center", notyTitle, 1500, ["click"], {
                                callback: {
                                    afterClose: function() {
                                        window.mpd.unblock();
                                        for (var i = 0, l = buttons.length; i < l; i++) {
                                            buttons[i].disabled = false;
                                        }
                                        timelineSelector.selectize.enable();
                                        rp[3].attr("disabled", false);
                                        rp[0].selectize.enable();
                                        rp[1].selectize.enable();
                                        saveButton.disabled = false;
                                    }
                                }
                            });
                        } else {
                            timeline.users = undefined;
                            window.mpd.unblock();
                            window.mpd.hide();
                        }
                    },

                    error: function() {
                        createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    window.mpd.unblock();
                                    for (var i = 0, l = buttons.length; i < l; i++) {
                                        buttons[i].disabled = false;
                                    }
                                    timelineSelector.selectize.enable();
                                    rp[3].attr("disabled", false);
                                    rp[0].selectize.enable();
                                    rp[1].selectize.enable();
                                    saveButton.disabled = false;
                                }
                            }
                        });
                    }
                });
            });
            window.mpd.resize();
        }

        function showAdd(tl, miniContent) {
            actualStatus = "add";

            var userWrapper = document.createElement("div");
            userWrapper.id = "wt-str-ns-tusw";
            var userInput = document.createElement("input");
            userInput.id = "wt-str-ns-tus";
            userInput.type = "text";
            userInput.className = "input";
            userInput.placeholder = i18n.t("username");
            userWrapper.appendChild(userInput);
            var atDiv = document.createElement("div");
            atDiv.id = "wt-str-ns-tusat";
            atDiv.innerText = "@";
            userWrapper.appendChild(atDiv);
            miniContent.appendChild(userWrapper);

            var rp = showRoles(miniContent, Math.max(timeline.userThinktank - 1, 0));

            var saveButton = document.createElement("button");
            saveButton.id = "wt-grp-pup-uslt-sv";
            saveButton.className = "input input-bl";
            saveButton.innerText = i18n.t("Save");
            saveButton.disabled = true;
            miniContent.appendChild(saveButton);

            var selectedRole = null,
                selectedPermissions = null;

            function checkSaveButton() {
                var enabled =
                    userInput.value.length !== 0
                        && selectedRole !== null
                        && selectedPermissions !== null;

                saveButton.disabled = !enabled;
                return enabled;
            }

            var currentRolePosition = 0,
                currentPermissionPosition = 0;

            for (var i = 0, l = roles.length; i < l; i++) {
                if (roles[i].id === tl.role) {
                    currentRolePosition = roles[i].position;
                    for (var j = 0, k = roles[i].permissions.length; j < k; j++) {
                        if (roles[i].permissions[j].id === tl.permissionType) {
                            currentPermissionPosition = roles[i].permissions[j].position;
                            break;
                        }
                    }
                    break;
                }
            }

            $(userInput).on("input", function() {
                checkSaveButton();
            });

            $(rp[0]).selectize({
                onChange: function(value) {
                    rp[1].selectize.clearOptions();
                    rp[1].selectize.clear();

                    rp[1].selectize.addOption({
                        value: -2,
                        text: i18n.t("Default")
                    });

                    //rp[1].selectize.addOption({
                    //    value: -1,
                    //    text: i18n.t("Custom")
                    //});

                    for (var i = 0, l = roles[value].permissions.length; i < l; i++) {
                        if (roles[value].permissions[i].position <= currentPermissionPosition) {
                            rp[1].selectize.addOption({
                                value: roles[value].permissions[i].id,
                                text: i18n.t("wisetank.permissionTypes." + roles[value].permissions[i].name)
                            });
                        }
                    }

                    selectedRole = roles[value].id;

                    rp[1].selectize.refreshOptions(false);
                    rp[1].selectize.enable();
                    rp[1].selectize.setValue(-2, false);

                    checkSaveButton();
                }
            });

            for (var i = 0, l = roles.length; i < l; i++) {
                if (roles[i].selectable
                    && (roles[i].position < currentRolePosition || roles[i].position === currentRolePosition)
                    && timeline.permissionTypes[roles[i].id] !== undefined) {
                    rp[0].selectize.addOption({
                        value: i,
                        text: i18n.t("wisetank.roles." + roles[i].name)
                    });
                }
            }

            rp[0].selectize.refreshOptions(false);

            $(rp[1]).selectize({
                onChange: function(value) {
                    var role = roles[rp[0].selectize.getValue()];

                    var permission, permissionId, rolePermissions = null, element;
                    switch (parseInt(value)) {
                    case -2: // DEFAULT
                        for (permission in allPermissions) {
                            if (Object.prototype.hasOwnProperty.call(allPermissions, permission)) {
                                element = document.getElementById(allPermissions[permission]);
                                if (element !== undefined && element !== null) {
                                    element.disabled = true;
                                    var wrapper = element.ancestor(".wt-grp-pup-prm", true);
                                    wrapper.classList.remove("wt-grp-pup-prmd", "wt-grp-pup-prma");
                                }
                            }
                        }

                        permissionId = timeline.defaultPermissions[role.id];

                        for (var i = 0, l = role.permissions.length; i < l; i++) {
                            if (role.permissions[i].id === permissionId) {
                                rolePermissions = role.permissions[i].permissions;
                                break;
                            }
                        }

                        for (permission in rolePermissions) {
                            if (Object.prototype.hasOwnProperty.call(rolePermissions, permission)) {
                                element = document.getElementById(permission);
                                if (element !== undefined && element !== null) {
                                    element.checked = rolePermissions[permission];
                                }
                            }
                        }

                        break;

                    //case -1: // CUSTOM
                    //    for (var permission in allPermissions) {
                    //        if (Object.prototype.hasOwnProperty.call(allPermissions, permission)) {
                    //            var element = document.getElementById(allPermissions[permission]);
                    //            if (element !== undefined && element !== null) {
                    //                var wrapper = element.ancestor(".wt-grp-pup-prm", true);

                    //                if (tl.permissions[allPermissions[permission]]) {
                    //                    wrapper.classList.remove("wt-grp-pup-prmd");
                    //                    wrapper.classList.add("wt-grp-pup-prma");
                    //                    element.disabled = false;
                    //                    element.checked = true;
                    //                } else {
                    //                    wrapper.classList.add("wt-grp-pup-prmd");
                    //                    wrapper.classList.remove("wt-grp-pup-prma");
                    //                    element.disabled = true;
                    //                    element.checked = false;
                    //                }
                    //            }
                    //        }
                    //    }
                    //    break;

                    default: // OTHER
                        for (permission in allPermissions) {
                            if (Object.prototype.hasOwnProperty.call(allPermissions, permission)) {
                                element = document.getElementById(allPermissions[permission]);
                                if (element !== undefined && element !== null) {
                                    element.disabled = true;
                                    wrapper = element.ancestor(".wt-grp-pup-prm", true);
                                    wrapper.classList.remove("wt-grp-pup-prmd", "wt-grp-pup-prma");
                                }
                            }
                        }

                        permissionId = parseInt(value);

                        for (var i = 0, l = role.permissions.length; i < l; i++) {
                            if (role.permissions[i].id === permissionId) {
                                rolePermissions = role.permissions[i].permissions;
                                break;
                            }
                        }

                        for (permission in rolePermissions) {
                            if (Object.prototype.hasOwnProperty.call(rolePermissions, permission)) {
                                element = document.getElementById(permission);
                                if (element !== undefined && element !== null) {
                                    element.checked = rolePermissions[permission];
                                }
                            }
                        }

                        break;
                    }

                    selectedPermissions = value;
                    checkSaveButton();
                }
            });

            rp[1].selectize.disable();

            rp[3].noUiSlider({
                start: [0],
                range: {
                    'min': 0,
                    'max': 9
                }
            }, true);

            $(saveButton).on("click", function() {
                if (!checkSaveButton()) {
                    return;
                }

                window.mpd.block();
                var buttons = window.mpd.buttons();
                for (var i = 0, l = buttons.length; i < l; i++) {
                    buttons[i].disabled = true;
                }
                timelineSelector.selectize.disable();
                userInput.disabled = true;
                rp[3].attr("disabled", "disabled");
                rp[0].selectize.disable();
                rp[1].selectize.disable();
                saveButton.disabled = true;

                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.addTimelineUser,
                    data: {
                        timeline: tl.id,
                        username: userInput.value,
                        thinktankLevel: rp[3].val(),
                        role: selectedRole,
                        permissions: selectedPermissions
                    },

                    success: function(result) {
                        var showNoty, notyTitle = null;

                        switch (result) {
                        case 0:
                            showNoty = false;
                            break;

                        case 242:
                            showNoty = true;
                            notyTitle = i18n.t("usernotfound");
                            break;

                        default:
                            showNoty = true;
                            notyTitle = i18n.t("error-processing");
                            break;
                        }

                        if (showNoty) {
                            createNoty("warning", "center", notyTitle, 1500, ["click"], {
                                callback: {
                                    afterClose: function() {
                                        window.mpd.unblock();
                                        for (var i = 0, l = buttons.length; i < l; i++) {
                                            buttons[i].disabled = false;
                                        }
                                        timelineSelector.selectize.enable();
                                        userInput.disabled = false;
                                        rp[3].attr("disabled", false);
                                        rp[0].selectize.enable();
                                        rp[1].selectize.enable();
                                        saveButton.disabled = false;
                                    }
                                }
                            });
                        } else {
                            timeline.users = undefined;
                            window.mpd.unblock();
                            window.mpd.hide();
                        }
                    },

                    error: function() {
                        createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    window.mpd.unblock();
                                    for (var i = 0, l = buttons.length; i < l; i++) {
                                        buttons[i].disabled = false;
                                    }
                                    timelineSelector.selectize.enable();
                                    userInput.disabled = false;
                                    rp[3].attr("disabled", false);
                                    rp[0].selectize.enable();
                                    rp[1].selectize.enable();
                                    saveButton.disabled = false;
                                }
                            }
                        });
                    }
                });
            });
        }

        function emptyContent(element) {
            while (element.firstChild) {
                element.removeChild(element.firstChild);
            }
        }

        var canAddUsers = (function checkCanAddUser() {
            var canAdd = false;
            for (var i in timeline.permissionTypes) {
                if (Object.prototype.hasOwnProperty.call(timeline.permissionTypes, i)) {
                    for (var j = 0, k = roles.length; j < k; j++) {
                        if (roles[j].id === parseInt(i) && roles[j].selectable) {
                            canAdd = true;
                            break;
                        }
                    }
                }

                if (canAdd) {
                    break;
                }
            }

            return canAdd;
        })();


        function loadTimelineData(tl) {
            window.mpd.removeButtons();
            emptyContent(content);

            if (timeline.hasPermissions[tl.id]) {
                if (canAddUsers) {
                    window.mpd.addButtons([
                        {
                            text: i18n.t("EditUsers"),
                            action: function() {
                                emptyContent(content);
                                showEdit(tl, content);
                                window.mpd.resize();
                            }
                        },
                        {
                            text: i18n.t("AddUser"),
                            action: function() {
                                emptyContent(content);
                                showAdd(tl, content);
                                window.mpd.resize();
                            }
                        }
                    ]);

                    switch (actualStatus) {
                    case "edit":
                        window.mpd.click(0);
                        break;

                    case "add":
                        window.mpd.click(1);
                        break;

                    default:
                        window.mpd.click(0);
                        break;
                    }
                } else {
                    showEdit(tl, content);
                    window.mpd.resize();
                }

            } else {
                showView(tl, content);
                window.mpd.resize();
            }
        }

        $(timelineSelector).selectize({
            onChange: function(value) {
                loadTimelineData(parseInt(value) === -1 ? timeline : timeline.children[value]);
            }
        });

        timelineSelector.selectize.addOption({
            value: -1,
            text: timeline.name
        });

        if (timeline.children !== null) {
            for (var i = 0, l = timeline.children.length; i < l; i++) {
                timelineSelector.selectize.addOption({
                    value: i,
                    text: timeline.children[i].name
                });
            }
        }

        timelineSelector.selectize.refreshOptions(false);

        if (timeline.users === undefined) {
            spinner.className += " app-spinner-active";

            window.mpd.block();

            $.ajax({
                async: true,
                type: "POST",
                url: api.getTimelineUsers,
                data: { timeline: actualGroup },

                success: function(result) {
                    timeline.users = result.userData;
                    timeline.hasPermissions = result.hasPermissions;
                    timeline.defaultPermissions = result.permissions;

                    spinner.classList.remove("app-spinner-active");
                    window.mpd.unblock();
                    timelineSelector.selectize.setValue(-1, false);
                },

                error: function() {
                    window.setInternalErrorMessage($(mpdContent));
                    window.mpd.resize();
                    window.mpd.unblock();
                }
            });
        } else {
            timelineSelector.selectize.setValue(-1, false);
        }
    }


/*** EVENTS ***/
    $(dom.streamsIcon).on("click", function() {
        if (!this.classList.contains("wt-str-lb-ic-act")) {
            dom.streamsIcon.classList.add("wt-str-lb-ic-act");
            dom.streamsMainContainer.classList.add("wt-bct-act");
            dom.groupsIcon.classList.remove("wt-str-lb-ic-act");
            dom.groupsMainContainer.classList.remove("wt-bct-act");
        }
    });

    $(dom.groupsIcon).on("click", function() {
        if (!this.classList.contains("wt-str-lb-ic-act")) {
            dom.groupsIcon.classList.add("wt-str-lb-ic-act");
            dom.groupsMainContainer.classList.add("wt-bct-act");
            dom.streamsIcon.classList.remove("wt-str-lb-ic-act");
            dom.streamsMainContainer.classList.remove("wt-bct-act");
        }
    });

    $(dom.streams).on("click", ".wt-str-lds", function(e) {
        if (e.target.className === "wt-str-ldcc") {
            deleteStream(streams[this.dataset.position].id);
            window.mpd.show();
        } else if (!this.classList.contains("wt-str-lda")) {
            selectStream(this);
        }
    });

    $(dom.newStream).on("click", function() {
        createNewStream();
        window.mpd.show();
        $("#wt-str-nt-nm").focus();
    });

    $(dom.renameStream).on("click", function() {
        if (actualStream !== null) {
            renameStream(actualStream);
            window.mpd.show();
            $("#wt-str-nt-nm").focus();
        }
    });

    $(dom.refresh).on("click", function() {
        if (actualStream !== null) {
            // TODO
        }
    });

    function closeAutoRefresh(event) {
        if (event.target !== null &&
                event.target.id !== "wt-str-sh-atr" &&
                (
                    event.target.parentElement === null ||
                        event.target.parentElement.id !== "wt-str-sh-atr" &&
                        event.target.parentElement.id !== "wt-str-sh-atr-lst"
                )
        ) {
            event.preventDefault();
            event.stopPropagation();
            dom.autoRefresh.classList.remove("wt-str-sh-act");
            window.removeEventListener("click", closeAutoRefresh, true);
        }
    }

    $(dom.autoRefresh).on("click", function() {
        if (this.classList.contains("wt-str-sh-act")) {
            this.classList.remove("wt-str-sh-act");
            window.removeEventListener("click", closeAutoRefresh, true);
        } else {
            this.classList.add("wt-str-sh-act");
            window.addEventListener("click", closeAutoRefresh, true);
        }
    });

    $(dom.autoRefreshSelector).on("click", "span", function() {
        var value = parseInt(this.dataset.value);

        var position = -1;
        for (var i = 0, l = streams.length; i < l; i++) {
            if (streams[i].id === actualStream) {
                position = i;
                break;
            }
        }

        if (position !== -1 && streams[position].refreshRate !== value) {
            refreshInterval = -1;
            clearTimeout(refreshTimeout);

            streams[position].refreshRate = value;

            $.ajax({
                async: true,
                type: "POST",
                url: api.setAutoRefresh,
                data: {
                    stream: actualStream,
                    refreshRate: value
                }
            });

            if (value === 0) {
                dom.autoRefreshHelper.innerText = i18n.t("Disabled");
                refreshInterval = -1;

            } else {
                dom.autoRefreshHelper.innerText = value + " " + i18n.t("Minutes");
                refreshInterval = value;
                // TODO SET TIMEOUT PARA REFRESCO
            }
        }
    });

    $(dom.newBox).on("click", function() {
        createNewBox();
        window.mpd.show();
    });

    /*$(dom.streams).sortable({
        items: ".wt-str-lds",
        containment: "parent",
        delay: 150,
        distance: 15,

        helper: function (e, ui) {
            var style = window.getComputedStyle(ui[0], null);
            ui.outerWidth(style.getPropertyValue("width"));
            ui.outerHeight(style.getPropertyValue("height"));
            return ui;
        },

        start: function (e, ui) {
            ui.placeholder.outerWidth(ui.helper[0].style.width);
            ui.placeholder.outerHeight(ui.helper[0].style.height);
        }
    });*/

    $(dom.boxSlider).draggable({
        axis: "x",
        containment: "parent",
        grid: [17, 0],

        drag: function(event, ui) {
            var boxesWidth = (ui.position.left / 17) + 1;

            if (parseInt(dom.boxes.dataset.width) !== boxesWidth) {
                dom.boxes.dataset.width = boxesWidth;
                dom.boxSlider.classList.remove("wt-str-sh-bws-" + dom.boxes.dataset.width);
                dom.boxSlider.classList.add("wt-str-sh-bws-" + boxesWidth);
                normalizeBoxes(dom.boxes);
            }
        },

        stop: function(event, ui) {
            var boxesWidth = (ui.position.left / 17) + 1;

            if (parseInt(dom.boxes.dataset.originalWidth) !== boxesWidth) {
                for (var i = 0, l = streams.length; i < l; i++) {
                    if (streams[i].id === dom.boxes.dataset.id) {
                        streams[i].boxesWidth = boxesWidth;
                        break;
                    }
                }

                dom.boxes.dataset.width = boxesWidth;
                dom.boxes.dataset.originalWidth = boxesWidth;
                dom.boxSlider.className = "wt-str-sh-bws-" + boxesWidth;
                normalizeBoxes(dom.boxes);

                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.editStreamBoxWidth,
                    data: { stream: dom.boxes.dataset.id, width: boxesWidth }
                });
            }
        }
    });


    $("#wt-str-bxs").mCustomScrollbar({
        axis: "x",
        scrollbarPosition: "inside",
        alwaysShowScrollbar: 2,
        scrollButtons: { enable: true },
        theme: "dark-3",
        mouseWheel: { enable: false }
    });

    $(".wt-str-bx-ctc").mCustomScrollbar({
        axis: "y",
        scrollbarPosition: "inside",
        alwaysShowScrollbar: 2,
        scrollButtons: { enable: true },
        theme: "dark-3",
        live: true
    });

    $("#wt-grp-pup-uslt").mCustomScrollbar({
        axis: "y",
        scrollbarPosition: "inside",
        alwaysShowScrollbar: 2,
        scrollButtons: { enable: true },
        theme: "dark-3",
        live: true
    });


    $(dom.groupBoxSlider).draggable({
        axis: "x",
        containment: "parent",
        grid: [17, 0],

        drag: function(event, ui) {
            var boxesWidth = (ui.position.left / 17) + 1;

            if (parseInt(dom.groupBoxes.dataset.width) !== boxesWidth) {
                dom.groupBoxes.dataset.width = boxesWidth;
                dom.groupBoxSlider.classList.remove("wt-str-sh-bws-" + dom.groupBoxes.dataset.width);
                dom.groupBoxSlider.classList.add("wt-str-sh-bws-" + boxesWidth);
                normalizeBoxes(dom.groupBoxes);
            }
        },

        stop: function(event, ui) {
            var boxesWidth = (ui.position.left / 17) + 1;


            if (parseInt(dom.groupBoxes.dataset.originalWidth) !== boxesWidth) {
                for (var i = 0, l = timelines.length; i < l; i++) {
                    if (timelines[i].id === dom.groupBoxes.dataset.id) {
                        timelines[i].boxesWidth = boxesWidth;
                        break;
                    }
                }

                dom.groupBoxes.dataset.width = boxesWidth;
                dom.groupBoxSlider.dataset.originalWidth = boxesWidth;
                dom.groupBoxSlider.className = "wt-str-sh-bws-" + boxesWidth;
                normalizeBoxes(dom.groupBoxes);

                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.editGroupBoxWidth,
                    data: { timeline: dom.groupBoxes.dataset.id, width: boxesWidth }
                });
            }
        }
    });

    $("#wt-grp-bxs").mCustomScrollbar({
        axis: "x",
        scrollbarPosition: "inside",
        alwaysShowScrollbar: 2,
        scrollButtons: { enable: true },
        theme: "dark-3",
        mouseWheel: { enable: false }
    });

    $(".wt-grp-bx-ctc").mCustomScrollbar({
        axis: "y",
        scrollbarPosition: "inside",
        alwaysShowScrollbar: 2,
        scrollButtons: { enable: true },
        theme: "dark-3",
        live: true
    });

    $(dom.articlesBoxesWrapper).on("click", ".wt-str-bx-hd-rfs", function() {
        if (this.classList.contains("wt-sh-inc")) {
            return;
        }

        var wrapper = this.ancestor(".wt-str-bx", true);

        refreshStreamBox(
            actualStream,
            wrapper,
            wrapper.dataset.position,
            1,
            (wrapper.firstRefresh
                ? null
                : wrapper.getElementsByClassName("wt-str-tw")[0].dataset.id));
    });

    $(dom.articlesBoxesWrapper).on("click", ".wt-str-tw-vti", function() {
        var element = this,
            wrapper = element.parentNode;

        if (!wrapper.classList.contains("wt-str-tw-vtsw-nv")) {
            (function x(el) {
                var articleElement = el.ancestor(".wt-str-tw", true);

                if (typeof articleElement.voting !== "boolean") {
                    articleElement.voting = false;
                }

                if (articleElement.voting) {
                    setTimeout(function() { x(el); }, 100);
                    return;
                }

                articleElement.voting = true;

                var articleId = articleElement.dataset.id,
                    timelineElement = articleElement.ancestor(".wt-str-bx", true),
                    timelineId = timelineElement.dataset.id;

                var articles = dom.boxesWrapper.getElementsByClassName("wt-str-tw");
                var thisArticles = [];

                for (var i = 0, l = articles.length; i < l; i++) {
                    if (articles[i].dataset.id === articleId) {
                        articles[i].voting = true;
                        thisArticles.push(articles[i]);
                    }
                }

                var vote = null;

                if (el.classList.contains("wt-str-tw-vts")) {
                    vote = null;
                } else if (el.classList.contains("wt-str-tw-vtu")) {
                    vote = true;
                } else if (el.classList.contains("wt-str-tw-vtd")) {
                    vote = false;
                }

                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.vote,
                    data: { timeline: timelineId, article: articleId, upvote: vote },

                    success: function(result) {
                        if (result === undefined || result === null) {
                            createNoty("warning", "center", i18n.t("error-processing"), 1500, ["click"], {
                                callback: {
                                    afterClose: function() {
                                        for (var i = 0, l = thisArticles.length; i < l; i ++) {
                                            thisArticles[i].voting = false;
                                        }
                                    }
                                }
                            });
                        } else {
                            for (var i = 0, l = thisArticles.length; i < l; i++) {
                                var article = thisArticles[i];
                                article.voting = false;

                                if (vote == null) {
                                    article.getElementsByClassName("wt-str-tw-vtu")[0].classList.remove("wt-str-tw-vts");
                                    article.getElementsByClassName("wt-str-tw-vtd")[0].classList.remove("wt-str-tw-vts");
                                } else if (vote) {
                                    article.getElementsByClassName("wt-str-tw-vtu")[0].classList.add("wt-str-tw-vts");
                                    article.getElementsByClassName("wt-str-tw-vtd")[0].classList.remove("wt-str-tw-vts");
                                } else {
                                    article.getElementsByClassName("wt-str-tw-vtu")[0].classList.remove("wt-str-tw-vts");
                                    article.getElementsByClassName("wt-str-tw-vtd")[0].classList.add("wt-str-tw-vts");
                                }

                                var voteText = article.getElementsByClassName("wt-str-tw-vt")[0];
                                voteText.innerText = (result > 0 ? "+" : "") + result;
                                voteText.classList.remove("wt-str-tw-vt-up");
                                voteText.classList.remove("wt-str-tw-vt-dw");
                                if (result > 0) {
                                    voteText.className += " wt-str-tw-vt-up";
                                } else if (result < 0) {
                                    voteText.className += " wt-str-tw-vt-dw";
                                }
                            }
                        }
                    },

                    error: function() {
                        createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    for (var i = 0, l = thisArticles.length; i < l; i++) {
                                        thisArticles[i].voting = false;
                                    }
                                }
                            }
                        });
                    }
                });
            })(element);
        }
    });

    $(dom.articlesBoxesWrapper).on("mouseover", ".wt-str-tw-krmi", function(e) {
        if (!this.classList.contains("wt-str-tw-krmi-nv")) {
            this.classList.remove("wt-str-tw-krmuh", "wt-str-tw-krmuh-1", "wt-str-tw-krmuh-2", "wt-str-tw-krmuh-3", "wt-str-tw-krmuh-4", "wt-str-tw-krmuh-5");

            if (e.target.classList.contains("wt-str-tw-krms")) {
                if (e.target.classList.contains("wt-str-tw-krms-1")) {
                    this.classList.add("wt-str-tw-krmuh", "wt-str-tw-krmuh-1");
                } else if (e.target.classList.contains("wt-str-tw-krms-2")) {
                    this.classList.add("wt-str-tw-krmuh", "wt-str-tw-krmuh-2");

                } else if (e.target.classList.contains("wt-str-tw-krms-3")) {
                    this.classList.add("wt-str-tw-krmuh", "wt-str-tw-krmuh-3");

                } else if (e.target.classList.contains("wt-str-tw-krms-4")) {
                    this.classList.add("wt-str-tw-krmuh", "wt-str-tw-krmuh-4");

                } else if (e.target.classList.contains("wt-str-tw-krms-5")) {
                    this.classList.add("wt-str-tw-krmuh", "wt-str-tw-krmuh-5");

                }
            }
        }
    });

    $(dom.articlesBoxesWrapper).on("mouseout", ".wt-str-tw-krmi", function() {
        this.classList.remove("wt-str-tw-krmuh", "wt-str-tw-krmuh-1", "wt-str-tw-krmuh-2", "wt-str-tw-krmuh-3", "wt-str-tw-krmuh-4", "wt-str-tw-krmuh-5");
    });

    $(dom.articlesBoxesWrapper).on("click", ".wt-str-tw-krmi", function() {
        if (!this.classList.contains("wt-str-tw-krmi-nv") && this.classList.contains("wt-str-tw-krmuh")) {
            var karma = 0;

            if (this.classList.contains("wt-str-tw-krmuh-1")) {
                karma = 1;
            } else if (this.classList.contains("wt-str-tw-krmuh-2")) {
                karma = 2;
            } else if (this.classList.contains("wt-str-tw-krmuh-3")) {
                karma = 3;
            } else if (this.classList.contains("wt-str-tw-krmuh-4")) {
                karma = 4;
            } else if (this.classList.contains("wt-str-tw-krmuh-5")) {
                karma = 5;
            }

            if (this.classList.contains("wt-str-tw-krmi-u" + karma)) {
                karma = null;
            }

            if (karma !== 0) {
                (function x(el) {
                    var articleElement = el.ancestor(".wt-str-tw", true);

                    if (typeof articleElement.voting !== "boolean") {
                        articleElement.voting = false;
                    }

                    if (articleElement.voting) {
                        setTimeout(function() { x(el); }, 100);
                        return;
                    }

                    articleElement.voting = true;

                    var articleId = articleElement.dataset.id,
                        timelineElement = articleElement.ancestor(".wt-str-bx", true),
                        timelineId = timelineElement.dataset.id;

                    var articles = dom.boxesWrapper.getElementsByClassName("wt-str-tw");
                    var thisArticles = [];

                    for (var i = 0, l = articles.length; i < l; i++) {
                        if (articles[i].dataset.id === articleId) {
                            articles[i].voting = true;
                            thisArticles.push(articles[i]);
                        }
                    }

                    $.ajax({
                        async: true,
                        type: "POST",
                        url: api.karma,
                        data: { timeline: timelineId, article: articleId, karma: karma },

                        success: function(result) {
                            if (result === undefined || result === null) {
                                createNoty("warning", "center", i18n.t("error-processing"), 1500, ["click"], {
                                    callback: {
                                        afterClose: function() {
                                            for (var i = 0, l = thisArticles.length; i < l; i++) {
                                                thisArticles[i].voting = false;
                                            }
                                        }
                                    }
                                });
                            } else {
                                var karmaCount = result.toFixed(1);
                                for (var i = 0, l = thisArticles.length; i < l; i++) {
                                    var article = thisArticles[i];
                                    article.voting = false;

                                    var karmaImage = article.getElementsByClassName("wt-str-tw-krmi")[0];
                                    karmaImage.classList.remove(
                                        "wt-str-tw-krmi-05", "wt-str-tw-krmi-10", "wt-str-tw-krmi-15", "wt-str-tw-krmi-20", "wt-str-tw-krmi-25",
                                        "wt-str-tw-krmi-30", "wt-str-tw-krmi-35", "wt-str-tw-krmi-40", "wt-str-tw-krmi-45", "wt-str-tw-krmi-50",
                                        "wt-str-tw-krmi-u1", "wt-str-tw-krmi-u2", "wt-str-tw-krmi-u3", "wt-str-tw-krmi-u4", "wt-str-tw-krmi-u5");
                                    karmaImage.classList.add("wt-str-tw-krmi-" + ((Math.round(karmaCount * 2) / 2).toFixed(1).replace(".", "")));

                                    if (karma !== null) {
                                        karmaImage.classList.add("wt-str-tw-krmi-u" + karma);
                                    }

                                    var karmaValue = article.getElementsByClassName("wt-str-tw-krmv")[0];
                                    karmaValue.innerText = karmaCount;
                                }
                            }
                        },

                        error: function() {
                            createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                                callback: {
                                    afterClose: function() {
                                        for (var i = 0, l = thisArticles.length; i < l; i++) {
                                            thisArticles[i].voting = false;
                                        }
                                    }
                                }
                            });
                        }
                    });
                })(this);
            }
        }
    });


    function closeGroupAreasSelector(event) {
        if (event === null || event.target !== null &&
                event.target.id !== "wt-grp-la" &&
                (
                    event.target.parentElement === null ||
                        event.target.parentElement.id !== "wt-grp-la" &&
                        event.target.parentElement.id !== "wt-grp-laart" &&
                        event.target.parentElement.id !== "wt-grp-laars"
                )
        ) {
            if (event !== null) {
                event.preventDefault();
                event.stopPropagation();
            }

            dom.groupAreasContainer.classList.remove("wt-grp-la-act");
            window.removeEventListener("click", closeGroupAreasSelector, true);
        }
    }

    $(dom.groupAreasContainer).on("click", function() {
        if (this.classList.contains("wt-grp-la-act")) {
            this.classList.remove("wt-grp-la-act");
            window.removeEventListener("click", closeGroupAreasSelector, true);
        } else {
            this.classList.add("wt-grp-la-act");
            window.addEventListener("click", closeGroupAreasSelector, true);
        }
    });

    $(dom.groupAreasContainer).on("click", ".wt-grp-laars-a", function(e) {
        e.stopPropagation();
        closeGroupAreasSelector(null);

        if (!this.classList.contains("wt-grp-laars-a-act")) {
            var areaSelectors = dom.groupAreasContainer.getElementsByClassName("wt-grp-laars-a");
            for (var i = 0, l = areaSelectors.length; i < l; i++) {
                areaSelectors[i].classList.remove("wt-grp-laars-a-act");
            }

            this.classList.add("wt-grp-laars-a-act");
            dom.groupAreasName.innerText = this.innerText;
            dom.newGroup.classList.add("wt-grp-ln-act");

            actualArea = this.dataset.id;
            selectGroupsArea(parseInt(actualArea), this.innerText);
            dom.selectArea.classList.remove("wt-grp-sl-are-act");
        }
    });

    $(dom.groupTimelinesContainer).on("click", ".wt-grp-lds", function(e) {
        if (e.target.className === "wt-grp-ldcc") {
            // TODO deleteTimeline();
            window.mpd.show();
        } else if (!this.classList.contains("wt-grp-lda")) {
            selectGroup(this);
        }
    });

    function closeGroupAutoRefresh(event) {
        if (event.target !== null &&
                event.target.id !== "wt-grp-sh-atr" &&
                (
                    event.target.parentElement === null ||
                        event.target.parentElement.id !== "wt-grp-sh-atr" &&
                        event.target.parentElement.id !== "wt-grp-sh-atr-lst"
                )
        ) {
            event.preventDefault();
            event.stopPropagation();
            dom.groupAutoRefresh.classList.remove("wt-grp-sh-act");
            window.removeEventListener("click", closeGroupAutoRefresh, true);
        }
    }

    $(dom.groupAutoRefresh).on("click", function() {
        if (this.classList.contains("wt-sh-inc")) {
            return;
        }

        if (this.classList.contains("wt-grp-sh-act")) {
            this.classList.remove("wt-grp-sh-act");
            window.removeEventListener("click", closeGroupAutoRefresh, true);
        } else {
            this.classList.add("wt-grp-sh-act");
            window.addEventListener("click", closeGroupAutoRefresh, true);
        }
    });

    $(dom.groupAutoRefreshSelector).on("click", "span", function() {
        if (this.classList.contains("wt-sh-inc")) {
            return;
        }

        var value = parseInt(this.dataset.value);
        // TODO!
        /*var position = -1;
        for (var i = 0, l = streams.length; i < l; i++) {
            if (streams[i].id === actualStream) {
                position = i;
                break;
            }
        }

        if (position !== -1 && streams[position].refreshRate !== value) {
            refreshInterval = -1;
            clearTimeout(refreshTimeout);

            streams[position].refreshRate = value;

            $.ajax({
                async: true,
                type: "POST",
                url: api.setAutoRefresh,
                data: {
                    stream: actualStream,
                    refreshRate: value
                }
            });

            if (value === 0) {
                dom.autoRefreshHelper.innerText = i18n.t("Disabled");
                refreshInterval = -1;

            } else {
                dom.autoRefreshHelper.innerText = value + " " + i18n.t("Minutes");
                refreshInterval = value;
                // TODO SET TIMEOUT PARA REFRESCO
            }
        }*/
    });

    $(dom.newGroup).on("click", function() {
        if (this.classList.contains("wt-sh-inc")) {
            return;
        }

        addNewGroup();
        window.mpd.show();
        window.mpd.click(0);
    });

    $(dom.groupAddColumn).on("click", function() {
        if (this.classList.contains("wt-sh-inc")) {
            return;
        }

        groupAddColumn();
        window.mpd.show();
    });

    $(dom.editGroup).on("click", function() {
        if (this.classList.contains("wt-sh-inc")) {
            return;
        }

        editGroup();
        window.mpd.show();
    });

    $(dom.groupUsers).on("click", function() {
        if (this.classList.contains("wt-sh-inc")) {
            return;
        }

        groupUsers();
        window.mpd.show();
    });

    $(dom.groupBoxesWrapper).on("click", ".wt-grp-bx-hd-wrt", function() {
        var timelineId = this.ancestor(".wt-str-bx", true).dataset.id;
        window.wisetank.addArticle(
            WISETANK_ORIGIN,
            null,
            null,
            null,
            null,
            null,
            null,
            timelineId,
            false);
    });


    $(window).on("resize", function() {
        resizeBoxes();
    });

    $.ajax({
        async: true,
        type: "POST",
        url: api.getData,
        data: {
            offsetdate: window.user.offsetDate
        },

        success: function(result) {
            areas = result.areas;
            sortedAreas = Object.keys(areas).sort(function(a, b) { return areas[a] - areas[b] });
            timelines = result.timelines;
            streams = result.streams;

            roles = result.roles;
            for (var i = 0, l = roles.length; i < l; i++) {
                if (roles[i].name === "owner") {
                    ownerRoleId = roles[i].id;
                    break;
                }
            }

            drawStreams();

            var firstStream = dom.streams.getElementsByClassName("wt-str-lds")[0];
            dom.spinner.classList.remove("app-spinner-active");
            selectStream(firstStream);

            for (var i = 0, l = sortedAreas.length; i < l; i++) {
                var area = document.createElement("span");
                area.className = "wt-grp-laars-a";
                area.dataset.id = areas[sortedAreas[i]];
                area.innerText = i18n.t("wisetank.areas." + sortedAreas[i].toLowerCase());
                dom.groupAreas.appendChild(area);
            }
        },

        error: function() {
            window.setInternalErrorMessage($("#ct-c"));
        }
    });
});
