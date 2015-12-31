window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var api = Object.create({
        stack: "/WordStax/Stack/"
    });

    var dom = Object.create({
        equalizers: $(document.getElementById("stk-eqs-ctn")),
        equalizersSpinner: document.getElementById("stk-eqs-spn"),
        equalizersContents: $(document.getElementById("stk-eqs-dc")),

        stats: $(document.getElementById("stk-sts-ctn")),
        statsSpinner: document.getElementById("stk-sts-spn"),

        leftEqualizer: document.getElementById("stk-eq-lft"),
        rightEqualizer: document.getElementById("stk-eq-rgt")
    });

    var stax = null;
    var finished = 0;
    var settings = null;

    window.stax = Object.create({
        loadStax: function(url, setWidth, callback, fallback, update, checkAchievement) {
            if (typeof setWidth !== "boolean") {
                setWidth = true;
            }

            if (!update) {
                dom.equalizersSpinner.classList.add("app-spinner-active");
            }

            if (typeof checkAchievement !== "boolean") {
                checkAchievement = false;
            }

            $.ajax({
                async: true,
                type: "POST",
                url: url,
                dataType: "json",

                success: function(result) {
                    var finishedModified = finished !== result.finished;

                    stax = result.stax;
                    finished = result.finished;
                    settings = result.settings;
                    if (update) {
                        dom.equalizersContents.empty();
                    }

                    for (var i = 0, l = stax.length; i < l; i++) {
                        drawStack(stax[i], settings, setWidth);
                    }

                    drawFinishedStack(finished);

                    if (!update) {
                        dom.equalizersSpinner.classList.remove("app-spinner-active");
                    }

                    if (typeof callback === "function") {
                        callback();
                    }

                    if (checkAchievement && finishedModified) {
                        var level;

                        if (finished >= 5000) {
                            level = 6;
                        } else if (finished >= 1000) {
                            level = 5;
                        } else if (finished >= 500) {
                            level = 4;
                        } else if (finished >= 250) {
                            level = 3;
                        } else if (finished >= 100) {
                            level = 2;
                        } else if (finished >= 50) {
                            level = 1;
                        } else {
                            level = 0;
                        }

                        if (level !== 0) {
                            window.achievements.unlock("vocabulary_finished", level);
                        }
                    }
                },

                error: function() {
                    if (typeof fallback === "function") {
                        fallback();
                    }
                    window.setInternalErrorMessage(dom.equalizers);
                }
            });
        },

        loadGraphs: function(url) {
            $.ajax({
                async: true,
                type: "POST",
                url: url,
                dataType: "json",
                data: { offsetDate: window.user.offsetDate },

                success: function(result) {
                    var graphs = createGraphs(result);
                    var maxYValue = 50;

                    for (var g in graphs) {
                        for (var d in graphs[g].datum) {
                            if (Object.prototype.hasOwnProperty.call(graphs[g].datum, d)) {
                                for (var i in graphs[g].datum[d]) {
                                    if (Object.prototype.hasOwnProperty.call(graphs[g].datum[d], i)) {
                                        maxYValue = Math.max(maxYValue, graphs[g].datum[d][i].y);
                                    }
                                }
                            }
                        }
                    }

                    d3.rebind("clipVoronoi");

                    for (var g in graphs) {
                        addGraph(graphs[g], maxYValue);
                    }

                    $(window).on("resize", function() {
                        for (var g in graphs) {
                            var width = graphs[g].wrap.width(),
                                height = graphs[g].wrap.height();

                            graphs[g].graph.width(width).height(height);

                            d3.select(graphs[g].chart)
                                .attr("width", width)
                                .attr("height", height)
                                .call(graphs[g].graph);
                        }
                    });

                    dom.statsSpinner.classList.remove("app-spinner-active");
                },

                error: function() {
                    window.setInternalErrorMessage(dom.stats);
                }
            });
        },

        loadFormulaStax: function(data) {
            if (stax === undefined || stax === null) {
                return false;
            }

            while (dom.leftEqualizer.firstChild) {
                dom.leftEqualizer.removeChild(dom.leftEqualizer.firstChild);
            }

            drawStackFrames(dom.leftEqualizer, stax[data.stackNum - 1], settings, true);

            if (data.stackNum < data.maxStack) {
                while (dom.rightEqualizer.firstChild) {
                    dom.rightEqualizer.removeChild(dom.rightEqualizer.firstChild);
                }

                drawStackFrames(dom.rightEqualizer, stax[data.stackNum], settings, true);
            }

            return true;
        },

        getStaxUnderflow: function() {
            return settings.underflow;
        },

        getStaxOverflow: function() {
            return settings.overflow;
        },

        getStackNumData: function(stack) {
            return stax[stack - 1].data;
        }
    });

    function createDatum(data) {
        var arr = [];

        for (var i = 0; i < 7; i++) {
            arr.push({ x: i, y: data[i].count });
        }

        return arr;
    }

    function createGraphs(data) {
        var graphs = Object.create(null);

        for (var i = 0, l = data.length; i < l; i++) {
            graphs[data[i].name] = {
                graph: null,
                wrap: $(document.getElementById("stk-sts-" + data[i].name)),
                chart: "#stk-sts-c-" + data[i].name,
                datum: {
                    thisWeek: createDatum(data[i].thisWeek),
                    lastWeek: createDatum(data[i].lastWeek),
                    averageWeek: createDatum(data[i].averageWeek)
                }
            };
        }

        return graphs;
    }

    function addGraph(g, maxYValue) {
        var width = g.wrap.width(),
            height = g.wrap.height();

        nv.addGraph({
            generate: function() {
                var chart = nv.models.lineChart()
                    .width(width)
                    .height(height)
                    .margin({ top: 0, right: 15, bottom: 17, left: 30 })
                    .interpolate("monotone")
                    .tooltipContent(function(key, x, y) {
                        return "<p>" + y + "</p>";
                    })
                    .options({
                        showLegend: true
                    });

                chart.xAxis.tickValues([0, 1, 2, 3, 4, 5, 6]);
                chart.xAxis.tickFormat(function(d) {
                    var day = ((d + (window.user.weekStart - 1) + 7) % 7);
                    return i18n.t("shortweekdays." + day);
                });
                chart.yAxis.tickPadding(10);

                chart.clipVoronoi(false);

                return chart;
            },

            callback: function(graph) {
                g.graph = graph;

                g.graph
                    .width(width)
                    .height(height)
                    .forceY([0, maxYValue]);

                d3.select(g.chart)
                    .attr("width", width)
                    .attr("height", height)
                    .datum([
                        { "values": g.datum.averageWeek, key: i18n.t("stax.graphs.average"), color: window.theme.colors.saturateddarkgreen },
                        { "values": g.datum.lastWeek, key: i18n.t("stax.graphs.last"), color: window.theme.colors.saturatedorange },
                        { "values": g.datum.thisWeek, key: i18n.t("stax.graphs.this"), color: window.theme.colors.saturatedblue }
                    ])
                    .call(g.graph);
            }
        });
    }

    function drawStackFrames(container, stackData, stackSettings, full) {
        var steps = (full ? 1 : stackSettings.steps);
        var height = 100 / ((stackSettings.overflow / steps) + 1);

        var underflowFrames = stackSettings.overflow - stackSettings.underflow;
        var dangerFrames = stackSettings.underflow / 2;

        for (var i = 0, l = (stackSettings.overflow + steps); i < l; i += steps) {
            var framew = document.createElement("div");
            framew.className = "stk-eqs-wss-frw";
            framew.style.height = height + "%";

            var framec = document.createElement("div");
            framec.className = "stk-eqs-wss-fr";

            if (i === 0) {
                framec.className += " stk-eqs-wss-fr-of";
            } else if (i <= dangerFrames) {
                framec.className += " stk-eqs-wss-fr-df";
            } else if (i >= underflowFrames + steps) {
                framec.className += " stk-eqs-wss-fr-uf";
            }

            if (i === 0 && stackData.data < stackSettings.overflow) {
                framec.className += " stk-eqs-wss-fr-in";
            } else if ((l - i) > stackData.data) {
                if (steps !== 1 || (l - i - steps) >= stackData.data) {
                    framec.className += " stk-eqs-wss-fr-in";
                }
            }

            framew.appendChild(framec);
            container.appendChild(framew);
        }
    }

    function drawStack(stackData, stackSettings, setWidth) {
        $(".stk-eqs-wsc").each(function() {
            var $this = $(this);
            $this.tipsy("hide");
            $this.tipsy("disable");
            $this.data("tipsy", null);
        });

        var stacka = document.createElement("a");
        stacka.className = "stk-lnk";
        stacka.href = api.stack + stackData.number;
        stacka.dataset.numData = stackData.data;

        var stackw = document.createElement("div");
        stackw.id = "stk-eqs-ws-" + stackData.number;
        stackw.className = "stk-eqs-ws stk-eq-wst";
        if (setWidth) {
            stackw.style.width = (100 / stackSettings.numStax) + "%";
        }

        var stackwc = document.createElement("div");
        stackwc.id = "stk-eqs-wsc-" + stackData.number;
        stackwc.className = "stk-eqs-wsc";
        stackw.appendChild(stackwc);

        var stack = document.createElement("div");
        stack.id = "stk-eqs-wss-" + stackData.number;
        stack.className = "stk-eqs-wss";

        var stackc = document.createElement("div");
        stackc.className = "stk-eqs-wss-c";

        drawStackFrames(stackc, stackData, stackSettings, false);

        stack.appendChild(stackc);
        stackwc.appendChild(stack);

        var numberw = document.createElement("div");
        numberw.className = "stk-eqs-wss-nm";

        if (stackData.data < stackSettings.underflow) {
            numberw.className += " stk-eq-wss-nmi";
        }

        var number = document.createElement("div");
        number.className = "stk-eqs-wss-nmn";
        number.innerText = stackData.number;
        numberw.appendChild(number);

        stackwc.appendChild(numberw);

        var stats = document.createElement("div");
        stats.className = "stk-eqs-wss-sts";

        var total = stackData.success + stackData.errors;

        if (total === 0) {
            stats.className += " stk-eq-wss-sts-nosts";
        } else {
            var success = (100 * (stackData.success / total));
            var errors = 100 - success;

            var successStats = document.createElement("div");
            successStats.className = "stk-eqs-wss-sts-bar stk-eqs-wss-sts-suc";
            successStats.style.width = success + "%";
            stats.appendChild(successStats);

            var errorsStats = document.createElement("div");
            errorsStats.className = "stk-eqs-wss-sts-bar stk-eqs-wss-sts-err";
            errorsStats.style.width = errors + "%";
            stats.appendChild(errorsStats);
        }

        stackw.appendChild(stats);
        stacka.appendChild(stackw);
        dom.equalizersContents.append(stacka);

        $(stackwc)
            .tipsy({
                gravity: "s",
                fade: true,
                html: true,
                delayIn: 100,
                delayOut: 0,
                title: function() {
                    var p = document.createElement("p");

                    var data = document.createElement("p");
                    data.style.fontSize = "2em";
                    data.style.lineHeight = "1.2em";
                    data.style.margin = "0";
                    data.style.whiteSpace = "nowrap";
                    data.innerText = i18n.t("Stack") + " " + stackData.number + " (" + stackData.data + "/" + stackSettings.overflow + ")";
                    p.appendChild(data);

                    if (total > 0) {
                        var pSuccess = document.createElement("p");
                        pSuccess.style.fontSize = "1.618em";
                        pSuccess.style.lineHeight = "1.2em";
                        pSuccess.style.margin = "0";
                        pSuccess.style.whiteSpace = "nowrap";
                        pSuccess.innerText = i18n.t("Success") + ": ";
                        p.appendChild(pSuccess);

                        var spanSuccess = document.createElement("span");
                        spanSuccess.style.color = window.theme.colors.green;
                        spanSuccess.innerText = (+success.toFixed(2)) + "%";
                        pSuccess.appendChild(spanSuccess);

                        var pError = document.createElement("p");
                        pError.style.fontSize = "1.618em";
                        pError.style.lineHeight = "1.2em";
                        pError.style.margin = "0";
                        pError.style.whiteSpace = "nowrap";
                        pError.innerText = i18n.t("Error") + ": ";
                        p.appendChild(pError);

                        var spanError = document.createElement("span");
                        spanError.style.color = window.theme.colors.red;
                        spanError.innerText = (+errors.toFixed(2)) + "%";
                        pError.appendChild(spanError);

                        var pMean = document.createElement("p");
                        pMean.style.fontSize = "1.618em";
                        pMean.style.lineHeight = "1.2em";
                        pMean.style.margin = "0";
                        pMean.style.whiteSpace = "nowrap";
                        pMean.innerText = i18n.t("Time") + "/" + i18n.t("exercise") + ": ";
                        p.appendChild(pMean);

                        var spanMean = document.createElement("span");
                        spanMean.innerText = "≈" + moment.duration(stackData.mean).asSeconds() + "s";
                        pMean.appendChild(spanMean);
                    }

                    return p.innerHTML;
                }
            });
    }

    function drawFinishedStack(finished) {
        var stackw = document.createElement("div");
        stackw.id = "stk-eqs-ws-f";
        stackw.className = "stk-eqs-ws";

        var numberw = document.createElement("div");
        numberw.className = "stk-eqs-wss-nmf";

        var number = document.createElement("div");
        number.className = "stk-eqs-wss-nmfn";
        number.innerText = finished;
        numberw.appendChild(number);

        stackw.appendChild(numberw);
        dom.equalizersContents.append(stackw);
    }

    $(document).on("click", ".stk-lnk", function(e) {
        var numData = parseInt(this.dataset.numData);

        if (numData < window.stax.getStaxUnderflow()) {
            e.preventDefault();
            window.shake(this, undefined, 2, 150);
        }
    });
});
