window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        spinner: document.getElementById("dsk-spn"),
        start: document.getElementById("dsk-str-btn"),

        successCount: document.getElementById("dsk-str-scs"),
        errorCount: document.getElementById("dsk-str-ivl"),
        timer: document.getElementById("dsk-str-tme"),

        startPage: document.getElementById("dsk-str"),
        exercisePage: document.getElementById("dsk-exr"),
        analysePage: document.getElementById("dsk-anl"),
        finishPage: document.getElementById("dsk-fns")
    });

    var api = Object.create({
        index: Object.create({
            load: "/Index/Load",
            start: "/Index/Start",
            check: "/Index/Check",
            finish: "/Index/Finish"
        }),

        exams: Object.create({
            load: "/Exams/Load",
            start: "/Exams/Start",
            check: "/Exams/Check",
            finish: "/Exams/Finish"
        }),

        books: Object.create({
            load: "/Books/Load",
            start: "/Books/Start",
            check: "/Books/Check",
            finish: "/Books/Finish"
        }),

        extra: Object.create({
            load: "/Extra/Load",
            start: "/Extra/Start",
            check: "/Extra/Check",
            finish: "/Extra/Finish"
        })
    });

    if (window.desks === undefined) {
        window.desks = Object.create(null);
    }

    var engine = function() {
        window.desks.engine = null;

        var that = this;

        that.types = Object.create({
            INDEX: "index",
            EXAMS: "exams",
            BOOKS: "books"
        });

        that.statuses = Object.create({
            LOAD: 0,
            START: 1,
            ANALYSE: 2,
            FINISH: 3
        });

        var status;
        var type, id, code;
        var callbacks = {};

        var successCount, errorCount;

        var timer = {
            time: -1,
            questionTime: -1,
            active: false,
            maxTime: 0,
            maxQuestionTime: 0,
            maxAnalyseTime: 0,
            questionTimeout: false,
            timeout: null,
            backwards: false,
            func: function() {
                if (timer.active) {
                    timer.time++;
                    timer.draw();

                    if (status === that.statuses.START) {
                        if (timer.questionTimeout) {
                            timer.questionTime--;
                        }

                        if (timer.questionTimeout && timer.questionTime === 0) {
                            if (typeof callbacks.questionTimeout === "function") {
                                timer.active = false;
                                callbacks.questionTimeout();
                            }
                        } else if (timer.time >= timer.maxTime) {
                            if (typeof callbacks.timeout === "function") {
                                timer.active = false;
                                callbacks.timeout();
                            }
                        } else {
                            timer.timeout = setTimeout(timer.func, 1000);
                        }
                    } else if (status === that.statuses.ANALYSE && timer.time >= timer.maxAnalyseTime) {
                        timer.timeout = setTimeout(timer.func, 1000);
                    }

                }
            },

            draw: function() {
                var time;
                if (timer.backwards) {
                    var max;

                    if (status === that.statuses.START) {
                        max = timer.maxTime;
                    } else if (status === that.statuses.ANALYSE) {
                        max = timer.maxAnalyseTime;
                    } else {
                        max = timer.time;
                    }

                    time = max - timer.time;
                } else {
                    time = timer.time;
                }

                time = Math.max(0, time);
                if (status === that.statuses.START) {
                    time = Math.min(time, timer.maxTime);
                } else if (status === that.statuses.ANALYSE) {
                    time = Math.min(time, timer.maxAnalyseTime);
                }

                dom.timer.innerText = secondsToDisplayClock(time, false, 2);
            },

            setActive: function() {
                timer.active = true;
                timer.func();
            },

            setInactive: function() {
                timer.active = false;
                if (timer.timeout !== null) {
                    clearTimeout(timer.timeout);
                }
            }
        };

        if (Object.freeze) {
            Object.freeze(that.types);
            Object.freeze(that.statuses);
        }

        this.load = function(
            deskType,
            assignmentId,
            timerBackwards,
            questionTimeout,
            loadCallback,
            startCallback,
            questionTimeoutCallback,
            timeoutCallback) {
            status = that.statuses.LOAD;

            type = deskType;
            id = assignmentId;
            code = null;

            timer.backwards = timerBackwards;

            callbacks.load = loadCallback;
            callbacks.start = startCallback;
            callbacks.questionTimeout = questionTimeoutCallback;
            callbacks.timeout = timeoutCallback;

            $(dom.start).one("click", function() {
                if (status === that.statuses.LOAD) {
                    dom.spinner.classList.add("app-spinner-active");

                    $.ajax({
                        async: true,
                        type: "POST",
                        url: api[type].load,
                        data: {
                            id: id
                        },

                        success: function(result) {
                            timer.maxTime = result.time * result.questions.length;
                            timer.maxQuestionTime = result.time;
                            timer.questionTimeout = questionTimeout;
                            callbacks.load(result);
                        },

                        error: function() {
                            window.setInternalErrorMessage($("#ct-c"));
                        }
                    });
                }
            });
        };

        this.start = function() {
            status = that.statuses.START;

            $.ajax({
                async: true,
                type: "POST",
                url: api[type].start,
                data: {
                    id: id,
                    offset: window.user.offsetDate
                },

                success: function(result) {
                    code = result;

                    if (result.success === undefined || result.success === null) {
                        successCount = 0;
                    } else {
                        successCount = result.success;
                    }

                    dom.successCount.innerText = successCount;

                    if (result.error === undefined || result.error === null) {
                        errorCount = 0;
                    } else {
                        errorCount = result.error;
                    }

                    dom.errorCount.innerText = errorCount;

                    callbacks.start();
                    dom.startPage.classList.remove("dsk-actr");
                    dom.exercisePage.classList.add("dsk-actr");
                    dom.spinner.classList.remove("app-spinner-active");
                },

                error: function() {
                    window.setInternalErrorMessage($("#ct-c"));
                }
            });
        };

        this.addSuccessCount = function() {
            successCount++;
            dom.successCount.innerText = successCount;
        }

        this.addErrorCount = function() {
            errorCount++;
            dom.errorCount.innerText = errorCount;
        }

        this.startTimer = function() {
            timer.setActive();
        }

        this.stopTimer = function() {
            timer.setInactive();
        }

        this.resetTimeout = function() {
            timer.questionTime = timer.maxQuestionTime + 1;
        }

        this.check = function(questions, answers, callback) {
            $.ajax({
                async: true,
                type: "POST",
                url: api[type].check,
                data: {
                    id: id,
                    code: code,
                    questions: questions,
                    answers: answers,
                    executionTime: timer.maxQuestionTime - timer.questionTime
                },

                success: function(result) {
                    code = result.code;

                    if (typeof callback === "function") {
                        callback(result);
                    }
                },

                error: function() {
                    window.setInternalErrorMessage($("#ct-c"));
                }
            });
        };

        this.finish = function(callback) {
            status = that.statuses.FINISH;
            timer.setInactive();

            $.ajax({
                async: true,
                type: "POST",
                url: api[type].finish,
                data: {
                    id: id,
                    code: code,
                    executionTime: timer.time,
                    offsetDate: window.user.offsetDate
                },

                success: function(result) {
                    if (typeof callback === "function") {
                        callback(result);
                    }
                },

                error: function() {
                    window.setInternalErrorMessage($("#ct-c"));
                }
            });
        };

        this.createIndex = function(d, next) {
            if (typeof next !== "boolean") {
                next = true;
            }

            var main = document.createElement("div");
            main.className = "dsk-exr-idx-mn";
            d.page.appendChild(main);

            var header = document.createElement("div");
            header.className = "dsk-exr-idx-hdr";
            main.appendChild(header);

            var top = document.createElement("div");
            top.className = "dsk-exr-idx-top";
            header.appendChild(top);

            var exercise = document.createElement("span");
            exercise.className = "dsk-exr-idx-ext";
            exercise.innerText = window.model.title;
            top.appendChild(exercise);

            var subject = document.createElement("span");
            subject.className = "dsk-exr-idx-sbj";
            subject.innerText = window.model.subtitle;
            top.appendChild(subject);

            var statement = document.createElement("div");
            statement.className = "dsk-exr-idx-stm";
            top.appendChild(statement);
            d.statement = statement;

            var right = document.createElement("div");
            right.className = "dsk-exr-idx-rgt";
            header.appendChild(right);

            var rightTable = document.createElement("div");
            rightTable.className = "dsk-exr-idx-rgt-tb";
            right.appendChild(rightTable);

            var rightSpinnerCell = document.createElement("div");
            rightSpinnerCell.className = "dsk-exr-idx-rgt-spn";
            rightTable.appendChild(rightSpinnerCell);

            var rightSpinner = document.createElement("div");
            rightSpinner.className = "app-spinner";
            rightSpinnerCell.appendChild(rightSpinner);
            d.spinner = rightSpinner;

            var reportCell = document.createElement("div");
            reportCell.className = "dsk-exr-idx-rgt-rpt-cll";
            rightTable.appendChild(reportCell);

            var report = document.createElement("button");
            report.id = "dsk-exr-idx-rgt-rpt";
            report.className = "input input-bl";
            report.innerText = i18n.t("desks.index.report");
            report.disabled = true;
            reportCell.appendChild(report);
            d.report = report;

            var content = document.createElement("div");
            content.className = "dsk-exr-idx-ctn";
            main.appendChild(content);

            var wrapper = document.createElement("div");
            wrapper.className = "dsk-exr-idx-wrp";
            content.appendChild(wrapper);

            var table = document.createElement("div");
            table.className = "dsk-exr-idx-tb";
            wrapper.appendChild(table);

            var cell = document.createElement("div");
            cell.className = "dsk-exr-idx-tbc";
            table.appendChild(cell);

            var datas = document.createElement("div");
            datas.className = "dsk-exr-idx-dta";
            cell.appendChild(datas);
            d.content = datas;

            if (next) {
                var bottom = document.createElement("div");
                bottom.className = "dsk-exr-idx-btm";
                cell.appendChild(bottom);

                var nextBtn = document.createElement("button");
                nextBtn.id = "dsk-exr-idx-nxt";
                nextBtn.className = "input input-bl";
                nextBtn.innerText = i18n.t("Next");
                bottom.appendChild(nextBtn);
            }
        }

        this.createFinishIndex = function(result, analyse) {
            var main = document.createElement("div");
            main.className = "dsk-exr-idx-mn";

            var content = document.createElement("div");
            content.className = "dsk-exr-idx-ctn";
            main.appendChild(content);

            var wrapper = document.createElement("div");
            wrapper.className = "dsk-exr-idx-wrp";
            content.appendChild(wrapper);

            var table = document.createElement("div");
            table.className = "dsk-exr-idx-tb";
            wrapper.appendChild(table);

            var cell = document.createElement("div");
            cell.className = "dsk-exr-idx-tbc";
            table.appendChild(cell);

            var datas = document.createElement("div");
            datas.className = "dsk-exr-idx-dta";
            cell.appendChild(datas);

            if (result.hasAnalyse && analyse) {
                var analyse = document.createElement("button");
                analyse.className = "input input-bl";
                analyse.innerText = "AAA";
                datas.appendChild(analyse);
            } else {
                if (!result.isBlocked) {
                    var round = document.createElement("button");
                    round.className = "input input-bl";
                    round.innerText = "BBB";
                    datas.appendChild(round);
                }

                var back = document.createElement("button");
                back.className = "input input-bl";
                back.innerText = "CCC";
                datas.appendChild(back);
            }
        }
    }

    window.desks.engine = engine;
});
