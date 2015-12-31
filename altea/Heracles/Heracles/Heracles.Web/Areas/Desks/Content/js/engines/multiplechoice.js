window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var engine = new window.desks.engine();

    var dom = Object.create({
        page: document.getElementById("dsk-exr"),
        analysePage: document.getElementById("dsk-anl"),
        finishPage: document.getElementById("dsk-fns"),

        spinner: null,
        report: null,
        content: null,
        statement: null,
        number: null,
        question: null,
        gap: null,
        answers: null,
        $answers: null
    });

    var type = engine.types.INDEX;
    var data;

    var questionId = -1;
    var actualQuestion = -1, questionsLeft;
    var actualAnswers = null, answersSpan = null;

    function clearPage() {
        if (dom.statement !== null) {
            dom.statement.innerText = null;
        }

        if (dom.content !== null) {
            dom.number = null;
            dom.question = null;
            dom.gap = null;
            dom.answers = null;
            dom.$answers = null;

            actualAnswers = null;
            answersSpan = null;

            while (dom.content.firstChild) {
                dom.content.removeChild(dom.content.firstChild);
            }
        }
    }

    function setAnswersWidth() {
        var width = parseFloat(window.getComputedStyle(dom.answers).getPropertyValue("width"));

        for (var i = 0, l = actualAnswers.length; i < l; i++) {
            actualAnswers[i].style.width = null;
            var answerWidth = parseFloat(window.getComputedStyle(actualAnswers[i]).getPropertyValue("width"));
            var percentage = answerWidth / width;

            if (percentage > 0.875) {
                actualAnswers[i].style.width = "100%";
            } else if (percentage > 0.75) {
                actualAnswers[i].style.width = "87.5%";
            } else if (percentage > 0.625) {
                actualAnswers[i].style.width = "75%";
            } else if (percentage > 0.50) {
                actualAnswers[i].style.width = "62.5%";
            } else if (percentage > 0.375) {
                actualAnswers[i].style.width = "50%";
            } else if (percentage > 0.25) {
                actualAnswers[i].style.width = "37.5%";
            } else if (percentage > 0.125) {
                actualAnswers[i].style.width = "25%";
            } else {
                actualAnswers[i].style.width = "12.5%";
            }
        }
    }

    function clickAnswer(e) {
        e.stopPropagation();
        e.preventDefault();

        for (var i in answersSpan) {
            if (Object.prototype.hasOwnProperty.call(answersSpan, i)) {
                answersSpan[i].span.classList.add("dsk-exr-idx-awrcc-nh");

                if (answersSpan[i].span === e.target) {
                    engine.stopTimer();
                    dom.$answers.off("click", clickAnswer);
                    checkAnswer([questionId], [[i]], true);
                }
            }
        }

        return;
    }

    function drawQuestion() {
        actualQuestion++;
        clearPage();

        for (var i = 0, l = data.exercises.length; i < l; i++) {
            if (data.exercises[i].id === data.questions[actualQuestion].exercise) {
                dom.statement.innerText = data.exercises[i].statement;
                break;
            }
        }

        var question = document.createElement("div");
        question.className = "dsk-exr-idx-qst";
        dom.content.appendChild(question);

        var number = document.createElement("span");
        number.className = "dsk-exr-idx-qstn";
        number.innerText = (actualQuestion + 1) + ".";
        question.appendChild(number);
        dom.number = number;

        var questionContent = document.createElement("div");
        questionContent.className = "dsk-exr-idx-qstc";
        question.appendChild(questionContent);
        dom.question = questionContent;

        var q = data.questions[actualQuestion].question.split("\x1a");
        questionContent.appendChild(document.createTextNode(q[0]));
        var spanGap = document.createElement("span");
        spanGap.className = "dsk-exr-idx-gap";
        spanGap.dataset.id = 1;
        questionContent.appendChild(spanGap);
        dom.gap = spanGap;
        questionContent.appendChild(document.createTextNode(q[1]));

        var gaps = question.getElementsByClassName("dsk-exr-idx-gap");
        var gapWidths = [];

        var answers = document.createElement("ol");
        answers.className = "dsk-exr-idx-aws clearfix";
        dom.content.appendChild(answers);
        dom.answers = answers;
        dom.$answers = $(answers);

        questionId = data.questions[actualQuestion].id;
        actualAnswers = [];
        answersSpan = {};

        data.questions[actualQuestion].answers.shuffle();

        for (var i = 0, l = data.questions[actualQuestion].answers.length; i < l; i++) {
            var answer = document.createElement("li");
            answer.className = "dsk-exr-idx-awr";
            answers.appendChild(answer);

            var answerContent = document.createElement("span");
            answerContent.className = "dsk-exr-idx-awrc";
            answer.appendChild(answerContent);

            var content = document.createElement("span");
            content.className = "dsk-exr-idx-awrcc";
            content.innerText = data.questions[actualQuestion].answers[i].answer;
            answerContent.appendChild(content);

            var gapNum = data.questions[actualQuestion].answers[i].gap - 1;
            var gap = gaps[gapNum];
            gap.innerText = data.questions[actualQuestion].answers[i].answer;
            var width = parseFloat(window.getComputedStyle(gap).getPropertyValue("width"));
            if (gapWidths[gapNum] === undefined || gapWidths[gapNum] < width) {
                gapWidths[gapNum] = width;
                gap.style.minWidth = width + "px";
            }

            actualAnswers.push(answer);
            answersSpan[data.questions[actualQuestion].answers[i].id] = {
                content: answer,
                span: content,
                text: data.questions[actualQuestion].answers[i].answer
            };
        }

        for (var i = 0, l = gaps.length; i < l; i++) {
            gaps[i].innerText = null;
        }

        dom.$answers.on("click", clickAnswer);
        setAnswersWidth();
        engine.resetTimeout();
        engine.startTimer();

        dom.spinner.classList.remove("app-spinner-active");
        dom.report.disabled = false;
    }

    function checkAnswer(questions, answers, next) {
        if (typeof next !== "boolean") {
            next = true;
        }

        dom.spinner.classList.add("app-spinner-active");
        dom.report.disabled = true;

        for (var i in answersSpan) {
            if (Object.prototype.hasOwnProperty.call(answersSpan, i)) {
                answersSpan[i].span.classList.add("dsk-exr-idx-awrcc-nh");
            }
        }

        if (answers[0][0] === null) {
            dom.number.classList.add("dsk-exr-idx-nok");
            dom.question.classList.add("dsk-exr-idx-nok");
            dom.gap.classList.add("dsk-exr-idx-nok");
            engine.addErrorCount();
        }

        engine.check(questions, answers, function(result) {
            if (answers[0][0] !== null) {
                if (result.status[0]) {
                    dom.number.classList.add("dsk-exr-idx-ok");
                    dom.question.classList.add("dsk-exr-idx-ok");
                    dom.gap.classList.add("dsk-exr-idx-ok");
                    answersSpan[answers[0][0]].content.classList.add("dsk-exr-idx-ok");
                    engine.addSuccessCount();
                } else {
                    dom.number.classList.add("dsk-exr-idx-nok");
                    dom.question.classList.add("dsk-exr-idx-nok");
                    dom.gap.classList.add("dsk-exr-idx-nok");
                    answersSpan[answers[0][0]].content.classList.add("dsk-exr-idx-nok");
                    engine.addErrorCount();
                }
            }

            dom.spinner.classList.remove("app-spinner-active");
            dom.report.disabled = false;

            if (next) {
                setTimeout(function() {
                    if (actualQuestion < questionsLeft - 1) {
                        dom.report.disabled = true;
                        drawQuestion();
                    } else {
                        engine.stopTimer();
                        engine.finish(finish);
                    }
                }, 3000);
            }
        });
    }

    function finish(result) {
        dom.page.classList.remove("dsk-actr");
        engine.createFinishIndex(result, true);
        dom.analysePage.classList.add("dsk-actr");
    }

    function loadCallback(result) {
        data = result;
        engine.start();
    }

    function startCallback() {
        questionsLeft = data.questions.length;
        engine.createIndex(dom, false);
        drawQuestion();
    }

    function questionTimeoutCallback() {
        dom.$answers.off("click", clickAnswer);
        checkAnswer([questionId], [[null]], true);
    }

    function timeoutCallback() {
        dom.$answers.off("click", clickAnswer);
        checkAnswer([questionId], [[null]], false);
        engine.finish(finish);
    }

    engine.load(
        type,
        window.model.id,
        true,
        true,
        loadCallback,
        startCallback,
        questionTimeoutCallback,
        timeoutCallback);

    $(window).on("resize", setAnswersWidth);
});
