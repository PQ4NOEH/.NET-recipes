window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        page: document.getElementById("dsk-exr"),
        spinner: null,
        report: null,
        content: null,
        statement: null,
        number: null,
        question: null,
        gaps: null,
        answers: null,
        $answers: null
    });

    var type = window.desks.engine.types.INDEX;
    var data;

    var questionId = -1;
    var actualQuestion = -1, questionsLeft;
    var answersSpan = null;

    function clearPage() {
        if (dom.statement !== null) {
            dom.statement.innerText = null;
        }

        if (dom.content !== null) {
            dom.number = null;
            dom.question = null;
            dom.gaps = null;
            dom.answers = null;
            dom.$answers = null;

            while (dom.content.firstChild) {
                dom.content.removeChild(dom.content.firstChild);
            }
        }
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

        dom.gaps = [];
        for (var q = data.questions[actualQuestion].question.split("\x1a"), g = 0, i = 0, l = q.length; i < l; i++) {
            if (q[i].length !== 0) {
                questionContent.appendChild(document.createTextNode(q[i]));
            }

            if (i + 1 < l) {
                var spanGap = document.createElement("span");
                spanGap.className = "dsk-exr-idx-gap";
                spanGap.dataset.id = ++g;
                questionContent.appendChild(spanGap);
                dom.gaps.push(spanGap);
            }
        }

        questionId = data.questions[actualQuestion].id;

        window.desks.engine.resetTimeout();
        window.desks.engine.startTimer();

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

        window.desks.engine.check(questions, answers, function(result) {
            if (result.status[0]) {
                dom.number.classList.add("dsk-exr-idx-ok");
                dom.question.classList.add("dsk-exr-idx-ok");
                dom.gap.classList.add("dsk-exr-idx-ok");
                if (answers[0][0] !== null) {
                    answersSpan[answers[0][0]].content.classList.add("dsk-exr-idx-ok");
                }
                window.desks.engine.addSuccessCount();
            } else {
                dom.number.classList.add("dsk-exr-idx-nok");
                dom.question.classList.add("dsk-exr-idx-nok");
                dom.gap.classList.add("dsk-exr-idx-nok");
                if (answers[0][0] !== null) {
                    answersSpan[answers[0][0]].content.classList.add("dsk-exr-idx-nok");
                }
                window.desks.engine.addErrorCount();
            }

            dom.spinner.classList.remove("app-spinner-active");
            dom.report.disabled = false;

            if (next) {
                setTimeout(function() {
                    if (actualQuestion < questionsLeft - 1) {
                        dom.report.disabled = true;
                        drawQuestion();
                    } else {
                        window.desks.engine.finish();
                    }
                }, answers[0][0] === null ? 0 : 3000);
            }
        });
    }

    function loadCallback(result) {
        data = result;
        window.desks.engine.start();
    }

    function startCallback() {
        questionsLeft = data.questions.length;
        window.desks.engine.createIndex(dom);
        drawQuestion();
    }

    function questionTimeoutCallback() {
        //dom.$answers.off("click", clickAnswer);
        checkAnswer([questionId], [[null]], true);
    }

    function timeoutCallback() {
        //dom.$answers.off("click", clickAnswer);
        checkAnswer([questionId], [[null]], false);
        window.desks.engine.finish();
    }

    window.desks.engine.load(
        type,
        window.model.id,
        true,
        true,
        loadCallback,
        startCallback,
        questionTimeoutCallback,
        timeoutCallback);
});
