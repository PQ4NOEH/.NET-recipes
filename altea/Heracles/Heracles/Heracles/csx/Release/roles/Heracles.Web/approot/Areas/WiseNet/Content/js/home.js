window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    var dom = Object.create({
        searchEnginesWrapper: document.getElementById("wnh-se"),
        godBar: document.getElementById("wnh-gb"),
        magazinesWrapper: document.getElementById("wnh-dww"),
        magazines: document.getElementById("wnh-dwc")
    });

    function addSearchEngine(searchEngine) {
        var engine = document.createElement("div");
        engine.className = "wnh-se";
        dom.searchEnginesWrapper.appendChild(engine);

        if (!searchEngine.visible) {
            engine.className += " wnh-seh";
        }

        $(engine).data({
            name: searchEngine.name,
            description: searchEngine.description
        });

        var wrapper = document.createElement("div");
        wrapper.className = "wnh-sew";
        engine.appendChild(wrapper);

        var iconDiv = document.createElement("div");
        iconDiv.className = "wnh-seu";
        wrapper.appendChild(iconDiv);

        var iconLink = document.createElement("a");
        iconLink.className = "wnh-l wnh-sel";
        iconLink.href = searchEngine.url;
        iconDiv.appendChild(iconLink);

        var iconWrapper = document.createElement("div");
        iconWrapper.className = "wnh-selw";
        iconLink.appendChild(iconWrapper);

        var iconContent = document.createElement("div");
        iconContent.className = "wnh-selwc";
        iconWrapper.appendChild(iconContent);

        var icon = document.createElement("img");
        icon.className = "wnh-seli";
        icon.src = searchEngine.image;
        icon.alt = searchEngine.name;
        iconContent.appendChild(icon);

        if (searchEngine.sections.length !== 0) {
            wrapper.className += " wnh-sewh";

            var sections = document.createElement("div");
            sections.className = "wnh-sedw";
            wrapper.appendChild(sections);

            for (var i = 0, l = searchEngine.sections.length; i < l; i++) {
                var section = document.createElement("div");
                section.className = "wnh-sed";
                sections.appendChild(section);

                var link = document.createElement("a");
                link.className = "wnh-l wnh-sedl";
                link.href = searchEngine.sections[i].url;
                link.innerText = searchEngine.sections[i].name;
                section.appendChild(link);
            }
        }
    }

    function addMagazines(magazine) {
        var carousel = document.createElement("div");
        carousel.className = "wnh-crs";
        dom.magazines.appendChild(carousel);

        var title = document.createElement("div");
        title.className = "wnh-crs-tle";
        title.innerText = magazine.name;
        carousel.appendChild(title);

        var contents = document.createElement("div");
        contents.className = "wnh-crs-cts";
        carousel.appendChild(contents);

        var left = document.createElement("div");
        left.className = "wnh-crs-scr wnh-crs-scr-lf";
        contents.appendChild(left);
        var leftSpan = document.createElement("span");
        leftSpan.innerText = "〈";
        left.appendChild(leftSpan);

        var right = document.createElement("div");
        right.className = "wnh-crs-scr wnh-crs-scr-rt";
        contents.appendChild(right);
        var rightSpan = document.createElement("span");
        rightSpan.innerText = "〉";
        right.appendChild(rightSpan);

        var magazines = document.createElement("div");
        magazines.className = "wnh-crs-mgz";
        contents.appendChild(magazines);
    }

    for (var i = 0, l = window.model.searchEngines.length; i < l; i++) {
        addSearchEngine(window.model.searchEngines[i]);
    }

    /* GodBar URL load */
    $(dom.godBar).on("keyup", function(e) {
        if (e.which === 13) { // RETURN
            var navUri = dom.godBar.value;

            if (navUri.trim().length !== 0) {
                window.top.window.wisenet.load(navUri);
            }
        } else if (e.which === 27) { // ESC
            dom.godBar.value = "";
        }
    });

    if (window.model.hasMagazines) {
        $(dom.magazinesWrapper).mCustomScrollbar({
            axis: "y",
            scrollbarPosition: "inside",
            alwaysShowScrollbar: 2,
            scrollButtons: { enable: true },
            theme: "dark-3"
        });

        for (var i = 0, l = window.model.magazines.length; i < l; i++) {
            addMagazines(window.model.magazines[i]);
        }
    }

    $(".wnh-l").on("click", function(e) {
        e.preventDefault();
        window.top.window.wisenet.load(this.href);
    });
});
