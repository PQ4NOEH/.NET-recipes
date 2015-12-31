window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var api = Object.create({
        loadStax: "/WordStax/Stax",
        loadInbox: "/WordStax/Inbox",
        newExercise: "/WordStax/New/",
        saveExercise: "/WordStax/Save/"
    });

    var dom = Object.create({
        countdown: document.getElementById("stk-hdr-ctn-c"),
        formulaWrapper: $(document.getElementById("stk-frm-w")),
        formulaSpinner: document.getElementById("stk-frm-spn"),
        formulaData: $(document.getElementById("stk-dta-ctn"))
    });

    var errors = Object.create({
        NOERROR: 0,
        UNDERFLOW: 1,
        OVERFLOW: 2
    });

    var transitionEnd = true;

    function errorMessage(errorType) {
        var message = null;
        switch (errorType) {
        case errors.UNDERFLOW:
            message = i18n.t("stax.vocabulary.underflow");
            break;

        case errors.OVERFLOW:
            message = i18n.t("stax.vocabulary.overflow");
            break;
        }

        window.setInternalErrorMessage(dom.formulaWrapper, message);
        dom.formulaSpinner.classList.remove("app-spinner-active");
    }

    var timeLimitTimeout = null, timeLimitEllapsed = null;

    function checkFlow(callback, fallback, excluded) {
        if (window.stax.getStackNumData(window.model.stackNum) < window.stax.getStaxUnderflow()) {
            fallback(errors.UNDERFLOW);
        } else if (window.model.stackNum < window.model.maxStack && window.stax.getStackNumData(window.model.stackNum + 1) > window.stax.getStaxOverflow()) {
            fallback(errors.OVERFLOW);
        } else {
            callback(excluded, true);
        }
    }

    function saveExercise(parameters, callback) {
        $.ajax({
            type: "POST",
            url: api.saveExercise + window.model.stackNum,
            dataType: "json",
            data: parameters,
            complete: callback
        });
    }

    var checkExercise = (function(stackNum) {
        var data = [];
        var formula = [], finished = [];

        formula[1] =
            formula[2] = function(exercise, element) {
                if ($(element).data("answer") === exercise.answer) {
                    element.classList.add("stk-vl");
                    return true;
                } else {
                    element.classList.add("stk-ivl");
                    return false;
                }
            };

        finished[1] = function(exercise, status, time) {
            var answers = document.getElementsByClassName("stk-awr");
            for (var i = 0, l = answers.length; i < l; i++) {
                answers[i].classList.remove("stk-hvr");
            }

            var parameters = {
                ids: [exercise.id],
                contents: [[exercise.answer]],
                status: status,
                time: (new Date().getTime() - time) / 1000,
                offsetDate: window.user.offsetDate
            };

            return parameters;
        };

        finished[2] = function(exercise, status, time) {
            var answers = document.getElementsByClassName("stk-awr");
            for (var i = 0, l = answers.length; i < l; i++) {
                answers[i].classList.remove("stk-hvr");
            }

            var parameters = {
                ids: [exercise.id],
                contents: [[exercise.data]],
                status: status,
                time: (new Date().getTime() - time) / 1000,
                offsetDate: window.user.offsetDate
            };

            return parameters;
        };

        formula[3] = function(exercise, element) {
            if ($(element).data("answer") === exercise.data) {
                element.classList.add("stk-vl");
                return true;
            } else {
                element.classList.add("stk-ivl");
                return false;
            }
        };

        finished[3] = function(exercise, status, time) {
            document.getElementById("stk-snd").classList.add("stk-snd-act");
            var answers = document.getElementsByClassName("stk-awr");
            for (var i = 0, l = answers.length; i < l; i++) {
                answers[i].classList.remove("stk-hvr");
            }

            var parameters = {
                ids: [exercise.id],
                contents: [null],
                status: status,
                time: (new Date().getTime() - time) / 1000,
                offsetDate: window.user.offsetDate
            };

            return parameters;
        };

        formula[4] = function(exercise, element) {
            document.getElementById("stk-snd").classList.add("stk-snd-act");
            if (element.value.trim().toLowerCase() === exercise.data.trim().toLowerCase()) {
                element.classList.add("stk-vl");
                element.readOnly = true;
                return true;
            } else {
                element.classList.add("stk-ivl");
                element.readOnly = true;
                return false;
            }
        };

        finished[4] = function(exercise, status, time) {
            var parameters = {
                ids: [exercise.id],
                contents: [null],
                status: status,
                time: (new Date().getTime() - time) / 1000,
                offsetDate: window.user.offsetDate
            };

            return parameters;
        };

        formula[5] =
            formula[6] = function(exercise, element, element2) {
                var $element = $(element), $element2 = $(element2);
                $element2.draggable("disable");

                element2.style.display = "none";

                for (var i = 0, l = exercise.data.length; i < l; i++) {
                    if (exercise.data[i].data === $element.data("data")) {
                        var valid = false;

                        for (var j = 0, k = exercise.data[i].otherData.length; j < k; j++) {
                            if (exercise.data[i].otherData[j] === $(element2).data("answer")) {
                                valid = true;
                                break;
                            }
                        }

                        if (valid) {
                            if (data.indexOf(exercise.data[i].id) === -1) {
                                data.push(exercise.data[i].id);
                            }
                            var count = $element.data("count");
                            $element.data("count", --count);
                            if (count === 0) {
                                element.classList.add("stk-vl");
                                $element.droppable("disable");
                            }
                            return true;
                        } else {
                            data.length = 0;
                            data.push(exercise.data[i].id);
                            element.classList.add("stk-ivl");
                            return false;
                        }
                    }
                }

                data.length = 0;
                return false;
            };

        finished[5] =
            finished[6] = function(exercise, status, time) {
                var questions = document.getElementsByClassName("stk-qst");
                for (var i = 0, l = questions.length; i < l; i++) {
                    $(questions[i]).droppable("disable");
                }
                var answers = document.getElementsByClassName("stk-awr");
                for (var i = 0, l = answers.length; i < l; i++) {
                    answers[i].classList.remove("stk-hvr");
                    $(answers[i]).draggable("disable");
                }

                var contents = [];

                for (var i = 0, l = data.length; i < l; i++) {
                    for (var j = 0, k = exercise.data.length; j < k; j++) {
                        if (exercise.data[j].id === data[i]) {
                            contents.push(exercise.data[j].otherData);
                        }
                    }
                }

                var parameters = {
                    ids: data,
                    contents: contents,
                    status: status,
                    time: (new Date().getTime() - time) / 1000,
                    offsetDate: window.user.offsetDate
                };

                return parameters;
            };

        formula[7] = function(exercise, element, element2) {
            var $element = $(element), $element2 = $(element2);
            $element2.draggable("disable");

            element2.style.visibility = "hidden";
            element2.classList.remove("stk-hvr");
            $element2.draggable("disable");

            if ($element.data("answer") === $element2.data("data")) {
                element.classList.add("stk-vl");
                $element.droppable("disable");

                for (var i = 0, l = exercise.datas.length; i < l; i++) {
                    if (exercise.datas[i] === $element.data("answer")) {
                        data.push(exercise.ids[i]);
                        element.getElementsByClassName("stk-bx-ctn")[0].innerText = exercise.sentences[i];
                        break;
                    }
                }

                return true;
            } else {
                data.length = 0;
                for (var i = 0, l = exercise.datas.length; i < l; i++) {
                    if (exercise.datas[i] === $element.data("answer")) {
                        data.push(exercise.ids[i]);
                        break;
                    }
                }
                element.classList.add("stk-ivl");
                return false;
            }
        };

        finished[7] = function(exercise, status, time) {
            var questions = document.getElementsByClassName("stk-qst");
            for (var i = 0, l = questions.length; i < l; i++) {
                $(questions[i]).draggable("disable");
            }
            var answers = document.getElementsByClassName("stk-awr");
            for (var i = 0, l = answers.length; i < l; i++) {
                answers[i].classList.remove("stk-hvr");
                $(answers[i]).droppable("disable");
            }

            var contents = [];

            for (var i = 0, l = data.length; i < l; i++) {
                contents.push(null);
            }

            var parameters = {
                ids: data,
                contents: contents,
                status: status,
                time: (new Date().getTime() - time) / 1000,
                offsetDate: window.user.offsetDate
            };

            return parameters;
        };

        return (function(f, ff) {
            return function(exercise) {
                return function(answersLeft) {
                    var time = new Date().getTime();
                    data.length = 0;

                    return function(element, element2) {
                        if (element === false) {
                            if (timeLimitTimeout !== null) {
                                clearTimeout(timeLimitTimeout);
                            }
                            newExercise(element2, false);
                        } else if (answersLeft > 0) {
                            var status = f(exercise, element, element2);

                            if (element !== undefined) {
                                $(element).trigger("mouseleave");
                            }

                            if (element2 !== undefined) {
                                $(element).trigger("mouseleave");
                            }

                            if (--answersLeft === 0 || !status) {
                                if (timeLimitTimeout !== null) {
                                    clearTimeout(timeLimitTimeout);
                                }
                                var parameters = ff(exercise, status, time);
                                transitionEnd = false;
                                setTimeout(function() {
                                    dom.formulaData.addClass("stk-dta-ctn-inv");
                                    dom.formulaData.transition({ opacity: 0 }, 250, function() {
                                        transitionEnd = true;
                                    });
                                    dom.countdown.style.display = "none";
                                }, 500);

                                saveExercise(parameters, function() {
                                    window.stax.loadStax(
                                        api.loadStax,
                                        false,
                                        function() {
                                            window.stax.loadFormulaStax(window.model);
                                            setTimeout(function() {
                                                checkFlow(newExercise, errorMessage, parameters.status ? parameters.ids : undefined);
                                            }, 250, parameters.status);
                                        },
                                        function() {
                                            window.setInternalErrorMessage(dom.formulaWrapper);
                                        },
                                        true);
                                });
                            }
                        }
                    }
                }
            }
        })(formula[stackNum], finished[stackNum]);
    })(window.model.stackNum);

    var loadExercise = (function(stackNum) {
        var data, preformula = [];

        preformula[3] =
            preformula[4] = function() {
                var soundElement = document.getElementById("stk-snd");

                function playSound(preventKeyPress) {
                    if (!soundElement.classList.contains("stk-snd-ply") && !soundElement.classList.contains("stk-snd-act")) {
                        soundElement.classList.add("stk-snd-ply");

                        window.sm.play(data, 1, {
                            autoplay: true,
                            destroyable: true,
                            callback: undefined,
                            finishback: function() {
                                soundElement.classList.remove("stk-snd-ply");
                            }
                        });

                        if (stackNum === 4 && !preventKeyPress) {
                            $(document.getElementById("stk-awr")).focus();
                        }
                    }
                }

                $(soundElement).on("click", function() {
                    playSound();
                });

                $(document).on("keypress", function(e) {
                    if ((e.which === 83 || e.which === 115)) {
                        if (!$(e.target).is("input")) {
                            playSound(true);
                        }
                    }
                });
            }

        preformula[5] =
            preformula[6] =
            preformula[7] = function() {
                /* jQuery UI tweak tolerance */
                /*jQuery.ui.intersect = function (draggable, droppable) {
                if (!droppable.offset) return false;

                var x1 = (draggable.positionAbs || draggable.position.absolute).left,
                    x2 = x1 + draggable.helperProportions.width,
                    y1 = (draggable.positionAbs || draggable.position.absolute).top,
                    y2 = y1 + draggable.helperProportions.height;

                var l = droppable.offset.left,
                    r = l + droppable.proportions.width,
                    t = droppable.offset.top,
                    b = t + droppable.proportions.height;

                return (l < x1 + (draggable.helperProportions.width / 1.25) // Right Half
                    && x2 - (draggable.helperProportions.width / 1.25) < r // Left Half
                    && t < y1 + (draggable.helperProportions.height / 1.25) // Bottom Half
                    && y2 - (draggable.helperProportions.height / 1.25) < b); // Top Half
            };*/
            }

        var formula = [];

        formula[1] =
            formula[2] = function(formula, checkExercise) {
                document.getElementById("stk-qst").getElementsByTagName("span")[0].innerText = formula.data;

                var answers = formula.otherData.slice(0);
                answers.push(formula.answer);
                answers.shuffle();

                var answersNode = document.getElementById("stk-aws");
                for (var i = 0, l = answers.length; i < l; i++) {
                    var span = document.createElement("span");
                    span.className = "stk-bx stk-lbx stk-awr stk-hvr";
                    $(span).data("answer", answers[i]);
                    answersNode.appendChild(span);

                    var content = document.createElement("span");
                    content.innerText = answers[i];
                    span.appendChild(content);
                }

                var check = checkExercise(1);
                $(".stk-awr").on("click", function() {
                    check(this);
                });

                return check;
            };

        formula[3] = function(formula, checkExercise) {
            data = formula.data;
            var answers = formula.otherData.slice(0);
            answers.push(formula.data);
            answers.shuffle();

            var answersNode = document.getElementById("stk-aws");
            for (var i = 0, l = answers.length; i < l; i++) {
                var span = document.createElement("span");
                span.className = "stk-bx stk-lbx stk-awr stk-hvr";
                $(span).data("answer", answers[i]);
                answersNode.appendChild(span);

                var content = document.createElement("span");
                content.innerText = answers[i];
                span.appendChild(content);
            }

            var check = checkExercise(1);
            $(document.getElementsByClassName("stk-awr")).on("click", function() {
                check(this);
            });

            document.getElementById("stk-snd").classList.remove("stk-snd-act");

            return check;
        };

        formula[4] = function(formula, checkExercise) {
            data = formula.data;

            var check = checkExercise(1);
            $(document.getElementById("stk-awr")).off("keyup");
            $(document.getElementById("stk-awr")).on("keyup", function(e) {
                if (e.which === 13) {
                    check(this);
                }
            });

            document.getElementById("stk-snd").classList.remove("stk-snd-act");

            return check;
        };

        formula[5] =
            formula[6] = function(formula, checkExercise, stackNum) {
                var questions = [], answers = [];
                for (var i = 0, l = formula.data.length; i < l; i++) {
                    questions.push([formula.data[i].data, formula.data[i].otherData.length]);
                    answers.pushRange(formula.data[i].otherData);
                }

                answers.pushRange(formula.otherData);
                questions.shuffle();
                answers.shuffle();

                var questionsNode = document.getElementById("stk-qts");
                for (var i = 0, l = questions.length; i < l; i++) {
                    var span = document.createElement("span");
                    span.className = "stk-bx stk-qst";
                    if (stackNum === 5) {
                        span.className += " stk-dbx";
                    } else {
                        span.className += " stk-tbx";
                    }
                    $(span).data("data", questions[i][0]);
                    $(span).data("count", questions[i][1]);
                    questionsNode.appendChild(span);

                    var content = document.createElement("span");
                    content.innerText = questions[i][0];
                    span.appendChild(content);
                }

                var answersNode = document.getElementById("stk-aws");
                for (var i = 0, l = answers.length; i < l; i++) {
                    var span = document.createElement("span");
                    span.className = "stk-bx stk-lbx stk-awr stk-hvr";
                    $(span).data("answer", answers[i]);
                    answersNode.appendChild(span);

                    var content = document.createElement("span");
                    content.innerText = answers[i];
                    span.appendChild(content);
                }

                var check = checkExercise(answers.length - formula.otherData.length);

                $(document.getElementsByClassName("stk-awr")).draggable({
                    scope: "stk-vcb",
                    containment: "#stk-dta-ctn",
                    scroll: false,
                    revert: "invalid",
                    revertDuration: 300,
                    opacity: 0.75
                });

                $(document.getElementsByClassName("stk-qst")).droppable({
                    scope: "stk-vcb",
                    hoverClass: "stk-bl",
                    drop: function(event, ui) {
                        check(this, ui.draggable[0]);
                    }
                });

                return check;
            };

        formula[7] = function(formula, checkExercise) {
            var questions = [], sentences = [];
            for (var i = 0, l = formula.datas.length; i < l; i++) {
                questions.push(formula.datas[i]);
                sentences.push([formula.sentences[i], formula.datas[i]]);
            }

            questions.shuffle();
            sentences.shuffle();

            var questionsNode = document.getElementById("stk-qts");
            for (var i = 0, l = questions.length; i < l; i++) {
                var span = document.createElement("span");
                span.className = "stk-bx stk-tbx stk-qst stk-hvr";
                $(span).data("data", questions[i]);
                questionsNode.appendChild(span);

                var content = document.createElement("span");
                content.innerText = questions[i];
                span.appendChild(content);
            }

            var answersNode = document.getElementById("stk-aws");
            for (var i = 0, l = sentences.length; i < l; i++) {
                var span = document.createElement("span");
                span.className = "stk-bx stk-lbx stk-awr";
                $(span).data("answer", sentences[i][1]);
                answersNode.appendChild(span);

                var content = document.createElement("span");
                content.className = "stk-bx-ctn";
                var sentenceWords = sentences[i][0].split(" "),
                    regExp = new RegExp(sentences[i][1], "g");
                for (var j = 0, k = sentenceWords.length; j < k; j++) {
                    if (regExp.test(sentenceWords[j])) {
                        var gap = document.createElement("span");
                        gap.className = "stk-gp";
                        gap.innerText = sentenceWords[j];
                        content.appendChild(gap);
                        var space = document.createTextNode(" ");
                        content.appendChild(space);
                    } else {
                        var textNode = document.createTextNode(sentenceWords[j] + " ");
                        content.appendChild(textNode);
                    }
                }
                span.appendChild(content);

                $(span).mCustomScrollbar({
                    axis: "y",
                    scrollbarPosition: "inside",
                    alwaysShowScrollbar: 2,
                    scrollButtons: { enable: true },
                    theme: "dark-3"
                });
            }

            var gaps = document.getElementsByClassName("stk-gp");
            for (var i = 0, l = gaps.length; i < l; i++) {
                gaps[i].style.width = window.getComputedStyle(gaps[i]).width;
                gaps[i].innerText = "";
            }

            var check = checkExercise(questions.length);

            $(document.getElementsByClassName("stk-qst")).draggable({
                scope: "stk-vcb",
                containment: "#stk-dta-ctn",
                scroll: false,
                revert: "invalid",
                revertDuration: 300,
                opacity: 0.75
            });

            $(document.getElementsByClassName("stk-awr")).droppable({
                scope: "stk-vcb",
                hoverClass: "stk-bl",
                drop: function(event, ui) {
                    check(this, ui.draggable[0]);
                }
            });

            return check;
        };

        function clearElement(node) {
            if (node.nodeType === 1) {
                if (node.nodeName === "SPAN") {
                    node.parentNode.removeChild(node);
                } else if (node.nodeName === "INPUT") {
                    node.value = "";
                    node.classList.remove("stk-vl", "stk-ivl");
                    node.readOnly = false;
                }
            }
        }

        if (typeof preformula[stackNum] === "function") {
            preformula[stackNum]();
        }

        return (function(f) {
            var x = function (exercise) {
                if (!transitionEnd) {
                    setTimeout(function() {
                        x(exercise);
                    }, 100);

                    return;
                }

                document.getElementsByClassName("stk-qst").forEach(function(n) {
                    clearElement(n);
                });

                document.getElementsByClassName("stk-awr").forEach(function(n) {
                    clearElement(n);
                });

                var check = f(exercise, checkExercise(exercise), stackNum);
                dom.formulaData.show();
                dom.countdown.innerText = secondsToDisplayClock(window.model.timeLimit);
                dom.countdown.style.display = "";

                function timeoutFunction() {
                    timeLimitEllapsed++;
                    dom.countdown.innerText = secondsToDisplayClock(window.model.timeLimit - timeLimitEllapsed);

                    if (timeLimitEllapsed === window.model.timeLimit) {
                        dom.formulaData.css("opacity", 0).hide();
                        dom.countdown.style.display = "none";

                        var ids;
                        switch (stackNum) {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            ids = [exercise.id];
                            break;

                        case 5:
                        case 6:
                            ids = exercise.data.map(function(x) {
                                return x.id;
                            });
                            break;

                        case 7:
                            ids = exercise.ids;
                            break;

                        default:
                            ids = [];
                            break;
                        }

                        setTimeout(function() {
                            check(false, ids);
                        }, 1000);
                    } else {
                        timeLimitTimeout = setTimeout(timeoutFunction, 1000);
                    }
                }

                timeLimitEllapsed = 0;
                dom.formulaData.transition({ opacity: 1 }, 250, "ease", function() {
                    timeLimitTimeout = setTimeout(timeoutFunction, 1000);
                });

                dom.formulaData.removeClass("stk-dta-ctn-inv");

                $(".stk-bx")
                    .tipsy({
                        gravity: "n",
                        fade: true,
                        trigger: "manual",
                        html: true,
                        title: function() {
                            var p = document.createElement("p");
                            p.style.fontSize = "2em";
                            p.style.lineHeight = "1em";
                            p.style.margin = "0";
                            p.innerText = this.innerText;
                            return p.outerHTML;
                        }
                    })
                    .on({
                        mouseenter: function() {
                            if (!dom.formulaData.hasClass("stk-dta-ctn-inv")) {
                                clearTimeout(this.tipsyShow);
                                var $this = $(this),
                                    contents = this.getElementsByTagName("span")[0];

                                if (contents !== undefined && contents !== null
                                    && contents.offsetWidth < contents.scrollWidth
                                    && !this.classList.contains("ui-draggable-dragging")) {
                                    this.tipsyShow = setTimeout(function() {
                                        $this.tipsy("show");
                                    }, 100);
                                }
                            }
                        },

                        mouseleave: function() {
                            clearTimeout(this.tipsyShow);
                            var $this = $(this);

                            setTimeout(function() {
                                $this.tipsy("hide");
                            }, 100);
                        }
                    });
            }

            return x;
        })(formula[stackNum]);
    })(window.model.stackNum);

    function newExercise(excluded, checkExcluded) {
        if (timeLimitTimeout !== null) {
            clearTimeout(timeLimitTimeout);
        }

        $.ajax({
            type: "POST",
            url: api.newExercise + window.model.stackNum,
            dataType: "json",
            data: {
                excluded: excluded,
                checkExcluded: checkExcluded
            },

            success: function(result) {
                if (result.stackStatus !== errors.NOERROR) {
                    errorMessage(result.stackStatus);
                } else {
                    dom.formulaSpinner.classList.remove("app-spinner-active");
                    loadExercise(result.formula);
                }
            },

            error: function() {
                window.setInternalErrorMessage(dom.formulaWrapper);
                dom.formulaSpinner.classList.remove("app-spinner-active");
            }
        });
    }

    window.stax.loadStax(
        api.loadStax,
        false,
        function() {
            window.stax.loadFormulaStax(window.model);
            checkFlow(newExercise, errorMessage);
        },
        function() {
            window.setInternalErrorMessage(dom.formulaWrapper);
        });
});
