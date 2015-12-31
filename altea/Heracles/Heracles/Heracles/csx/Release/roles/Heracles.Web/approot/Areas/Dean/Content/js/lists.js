window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        container: document.getElementById("dn-lists"),
        spinner: $(document.getElementById("dn-lists-spn")),
        types: document.getElementById("dn-lst-slc-t"),
        areas: document.getElementById("dn-lst-slc-a"),
        searchCategories: document.getElementById("dn-lst-slc-chs"),
        categories: document.getElementById("dn-lst-slc-ccc"),
        searchLists: document.getElementById("dn-lst-slc-lhs"),
        lists: document.getElementById("dn-lst-slc-lcc")
    });

    var api = Object.create({
        data: "/Dean/GetLists"
    });

    var dataLoaded = false,
        loadingData = false,
        errorLoading = false;

    var typeSelected = false,
        areaSelected = false;

    var categories, lists;
    var assignments;

    function addArea(area) {
        var button = document.createElement("button");
        button.className = "dn-lst-slc-a input mini input-gr";
        button.dataset.id = area.id;
        button.innerText = area.name;
        dom.areas.appendChild(button);
    }

    function createListData(data, category, arrow) {
        var div = document.createElement("div");
        div.className = "dn-lst-slc-ld";
        div.dataset.id = data.id;

        if (category) {
            div.className += " dn-lst-slc-ld-tc";

            if (arrow) {
                div.className += " dn-lst-slc-ld-cld";
            }
        } else {
            div.className += " dn-lst-slc-ld-tl";
        }

        var outer = document.createElement("div");
        outer.className = "dn-lst-slc-ldo";
        div.appendChild(outer);

        if (category && arrow) {
            var arr = document.createElement("span");
            arr.className = "dn-lst-slc-lda";
            outer.appendChild(arr);
        }

        var title = document.createElement("span");
        title.className = "dn-lst-slc-ldt";
        title.innerText = data.name;
        outer.appendChild(title);

        var count = document.createElement("span");
        count.className = "dn-lst-slc-ldc";

        if (category) {
            count.innerText = data.listCount[typeSelected];

            if (arrow) {
                var inner = document.createElement("div");
                inner.className = "dn-lst-slc-ldi";
                div.appendChild(inner);
            }
        } else {
            count.innerText = data.count;
        }

        outer.appendChild(count);

        return div;
    }

    function showAreaData() {
        for (var i = 0, l = categories.length; i < l; i++) {
            if (categories[i].id === areaSelected) {
                var categoriesData = categories[i].children;
                while (categoriesData.length === 1 && categoriesData[0].isArea) {
                    categoriesData = categoriesData[0].children;
                }

                for (var j = 0, k = categoriesData.length; j < k; j++) {
                    if (categoriesData[j].listCount[typeSelected] !== undefined
                        && categoriesData[j].listCount[typeSelected] !== 0) {
                        var category = createListData(categoriesData[j], true, false);
                        dom.categories.appendChild(category);
                    }
                }

                break;
            }
        }
    }

    function showListsData(category, element) {
        for (var i = 0, l = lists.length; i < l; i++) {
            if (lists[i].type === typeSelected && lists[i].categories.indexOf(category.id) !== -1) {
                var list = createListData(lists[i], false, false);
                element.appendChild(list);
            }
        }
    }

    function setActiveType(type) {
        while (dom.categories.firstChild) {
            dom.categories.removeChild(dom.categories.firstChild);
        }

        while (dom.lists.firstChild) {
            dom.lists.removeChild(dom.lists.firstChild);
        }

        dom.searchCategories.value = null;
        dom.searchLists.value = null;

        dom.types.getElementsByClassName("dn-lst-slc-t").forEach(function(n) {
            if (n.dataset.id === type) {
                n.classList.remove("input-gr");
                n.classList.add("input-bl", "input-nh", "input-nc");
            } else {
                n.classList.remove("input-bl", "input-nh", "input-nc");
                n.classList.add("input-gr");
            }
        });

        typeSelected = parseInt(type);

        var areas = dom.areas.getElementsByClassName("dn-lst-slc-a");

        var changeArea = false;

        for (var i = 0, l = categories.length; i < l; i++) {
            for (var j = 0, k = areas.length; j < k; j++) {
                if (parseInt(areas[j].dataset.id) === categories[i].id) {
                    if (categories[i].listCount[typeSelected] === undefined
                        || categories[i].listCount[typeSelected] === 0) {
                        if (areaSelected === categories[i].id) {
                            changeArea = true;
                        }
                        areas[j].disabled = true;
                    } else {
                        areas[j].disabled = false;
                    }

                    break;
                }
            }
        }

        if (changeArea) {
            areaSelected = false;

            dom.areas.getElementsByClassName("dn-lst-slc-a").forEach(function(n) {
                n.classList.remove("input-bl", "input-nh", "input-nc");
                n.classList.add("input-gr");
            });
        }

        if (areaSelected !== false) {
            showAreaData();
        }
    }

    function setActiveArea(area) {
        while (dom.categories.firstChild) {
            dom.categories.removeChild(dom.categories.firstChild);
        }

        while (dom.lists.firstChild) {
            dom.lists.removeChild(dom.lists.firstChild);
        }

        dom.searchCategories.value = null;
        dom.searchLists.value = null;

        dom.areas.getElementsByClassName("dn-lst-slc-a").forEach(function(n) {
            if (n.dataset.id === area) {
                n.classList.remove("input-gr");
                n.classList.add("input-bl", "input-nh", "input-nc");
            } else {
                n.classList.remove("input-bl", "input-nh", "input-nc");
                n.classList.add("input-gr");
            }
        });

        areaSelected = parseInt(area);

        if (typeSelected !== false) {
            showAreaData();
        }
    }

    function setActiveCategory(category) {
        var id = parseInt(category.dataset.id);

        while (dom.lists.firstChild) {
            dom.lists.removeChild(dom.lists.firstChild);
        }

        dom.categories.getElementsByClassName("dn-lst-slc-ld").forEach(function(n) {
            n.classList.remove("dn-lst-slc-ld-slc");
        });

        category.classList.add("dn-lst-slc-ld-slc");

        function recursiveSubcategories(subcategories, element) {
            if (subcategories === undefined || subcategories === null) {
                return;
            }

            for (var i = 0, l = subcategories.length; i < l; i++) {
                if (subcategories[i].listCount[typeSelected] !== undefined) {
                    var subcategory = createListData(subcategories[i], true, true);
                    element.appendChild(subcategory);
                    var inner = subcategory.getElementsByClassName("dn-lst-slc-ldi")[0];
                    recursiveSubcategories(subcategories[i].children, inner);
                    showListsData(subcategories[i], inner);
                }
            }
        }

        for (var i = 0, l = categories.length; i < l; i++) {
            if (categories[i].id === areaSelected) {
                var categoriesData = categories[i].children;
                while (categoriesData.length === 1 && categoriesData[0].isArea) {
                    categoriesData = categoriesData[0].children;
                }

                for (var j = 0, k = categoriesData.length; j < k; j++) {
                    if (categoriesData[j].id === id) {
                        recursiveSubcategories(categoriesData[j].children, dom.lists);
                        showListsData(categoriesData[j], dom.lists);
                        break;
                    }
                }

                break;
            }
        }
    }

    function loadListsData() {
        loadingData = true;

        $.ajax({
            async: true,
            type: "POST",
            url: api.data,
            data: { type: 0 },

            success: function(result) {
                categories = result.categories;
                lists = result.lists;

                for (var i = 0, l = categories.length; i < l; i++) {
                    addArea(categories[i]);
                }

                dataLoaded = true;
                loadingData = false;
                dom.spinner.removeClass("app-spinner-active");

                setActiveType(dom.types.getElementsByClassName("dn-lst-slc-t").filter(function(n) {
                    return !n.disabled;
                })[0].dataset.id);

                setActiveArea(dom.areas.getElementsByClassName("dn-lst-slc-a").filter(function(n) {
                    return !n.disabled;
                })[0].dataset.id);

                dataLoaded = true;
                loadingData = false;
            },
            error: function() {
                errorLoading = true;

                dom.container.style.display = "table";
                window.setInternalErrorMessage($(dom.container));

                dom.spinner.removeClass("app-spinner-active");
            }
        });
    }

    $(dom.types).on("click", ".dn-lst-slc-t", function() {
        if (!this.classList.contains("input-bl") && !this.disabled) {
            setActiveType(this.dataset.id);
        }
    });

    $(dom.areas).on("click", ".dn-lst-slc-a", function() {
        if (!this.classList.contains("input-bl") && !this.disabled) {
            setActiveArea(this.dataset.id);
        }
    });

    $(dom.searchCategories).on("input", function() {
        var text = dom.searchCategories.value.trim().toLowerCase();
        var categories = dom.categories.getElementsByClassName("dn-lst-slc-ld");

        if (text === "") {
            categories.forEach(function(n) {
                n.style.display = null;
            });
        } else {
            categories.forEach(function(n) {
                var title = n.getElementsByClassName("dn-lst-slc-ldt")[0].innerText.toLowerCase();
                if (title.indexOf(text) === -1 && !n.classList.contains("dn-lst-slc-ld-slc")) {
                    n.style.display = "none";
                } else {
                    n.style.display = null;
                }
            });
        }
    });

    $(dom.categories).on("click", ".dn-lst-slc-ld", function() {
        setActiveCategory(this);
    });

    $(dom.searchLists).on("input", function() {
        var text = dom.searchLists.value.trim().toLowerCase();
        var lists = dom.lists.getElementsByClassName("dn-lst-slc-ld");

        if (text === "") {
            lists.forEach(function(n) {
                n.style.display = null;
            });
        } else {
            lists.forEach(function(n) {
                var title = n.getElementsByClassName("dn-lst-slc-ldt")[0].innerText.toLowerCase();
                if (title.indexOf(text) === -1) {
                    n.style.display = "none";
                } else {
                    n.style.display = null;
                }
            });
        }
    });

    $(dom.lists).on("click", ".dn-lst-slc-ld", function(e) {
        if ((e.target.classList.contains("dn-lst-slc-ldo") || e.target.parentNode.classList.contains("dn-lst-slc-ldo"))
            && this.classList.contains("dn-lst-slc-ld-tc")) {
            this.classList.toggle("dn-lst-slc-ld-opn");
            this.classList.toggle("dn-lst-slc-ld-cld");
        } else {
        }

        e.preventDefault();
        e.stopPropagation();
    });

    window.dean.loadListsData = function() {
        if (!dataLoaded && !loadingData) {
            $(".dn-lst-slc-cc").mCustomScrollbar({
                axis: "y",
                scrollbarPosition: "inside",
                alwaysShowScrollbar: 2,
                scrollButtons: { enable: true },
                theme: "dark-3"
            });

            loadListsData();
        }
    }

    window.dean.loadMemberLists = function(data) {
        if (errorLoading) {
            return;
        }

        assignments = data;
    }
});
