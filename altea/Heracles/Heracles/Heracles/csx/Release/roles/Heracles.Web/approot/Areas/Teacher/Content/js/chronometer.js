window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var dom = Object.create({
        button: document.getElementById("tchr-chrono")
    });

    // ReSharper disable once InconsistentNaming
    function Chrono(id) {
        var actualTime = 60, maxTime = 60, backwards = true;
        var interval, started = false, shown = false;

        var dom = Object.create(null);
        var jdom = Object.create(null);
        var dialog;

        // DOM nodes creation
        (function(document) {
            dom.chrono = document.createElement('div');
            dom.chrono.id = 'altea-chronometer-' + id;
            dom.chrono.className = 'altea-chronometer';
            dom.chrono.title = i18n.t('addons.chronometer.chronometer');
            window.document.body.appendChild(dom.chrono);
            dialog = $(dom.chrono).dialog({
                autoOpen: false,
                resizable: false,
                width: '400px',
                close: function() {
                    shown = false;
                    stopClick();
                }
            });

            var setTime = document.createElement('div');
            setTime.className = 'altea-chronometer-set-time';
            dom.chrono.appendChild(setTime);

            var setMinutes = document.createElement('div');
            setMinutes.className = 'altea-chronometer-set-minutes';
            setTime.appendChild(setMinutes);

            dom.setMinutesUpArrow = document.createElement('div');
            dom.setMinutesUpArrow.className = 'altea-chronometer-up-arrow';
            setMinutes.appendChild(dom.setMinutesUpArrow);

            dom.setMinutesText = document.createElement('div');
            dom.setMinutesText.className = 'altea-chronometer-set-text altea-chronometer-set-minutes-text';
            dom.setMinutesText.innerText = '00';
            setMinutes.appendChild(dom.setMinutesText);

            dom.setMinutesDownArrow = document.createElement('div');
            dom.setMinutesDownArrow.className = 'altea-chronometer-down-arrow';
            setMinutes.appendChild(dom.setMinutesDownArrow);

            var setColon = document.createElement('div');
            setColon.className = 'altea-chronometer-set-colon';
            setColon.innerText = ':';
            setTime.appendChild(setColon);

            var setSeconds = document.createElement('div');
            setSeconds.className = 'altea-chronometer-set-seconds';
            setTime.appendChild(setSeconds);

            dom.setSecondsUpArrow = document.createElement('div');
            dom.setSecondsUpArrow.className = 'altea-chronometer-up-arrow';
            setSeconds.appendChild(dom.setSecondsUpArrow);

            dom.setSecondsText = document.createElement('div');
            dom.setSecondsText.className = 'altea-chronometer-set-text altea-chronometer-set-seconds-text';
            dom.setSecondsText.innerText = '00';
            setSeconds.appendChild(dom.setSecondsText);

            dom.setSecondsDownArrow = document.createElement('div');
            dom.setSecondsDownArrow.className = 'altea-chronometer-down-arrow';
            setSeconds.appendChild(dom.setSecondsDownArrow);

            jdom.numbers = $([dom.setMinutesText, dom.setSecondsText]);
            jdom.arrows = $([dom.setMinutesUpArrow, dom.setMinutesDownArrow, dom.setSecondsUpArrow, dom.setSecondsDownArrow]);


            var backwardsCbox = document.createElement('div');
            backwardsCbox.className = 'altea-chronometer-backwards';
            dom.chrono.appendChild(backwardsCbox);
            jdom.backwards = $(backwardsCbox);

            dom.backwards = document.createElement('input');
            dom.backwards.id = 'altea-chronometer-backwards-' + id;
            dom.backwards.className = 'altea-chronometer-backwards-cbox';
            dom.backwards.type = 'checkbox';
            backwardsCbox.appendChild(dom.backwards);
            jdom.backwardsCbox = $(dom.backwards);

            var backwardsLabel = document.createElement('label');
            backwardsLabel.className = 'altea-chronometer-backwards-label';
            backwardsLabel.htmlFor = 'altea-chronometer-backwards-' + id;
            backwardsLabel.innerText = i18n.t('addons.chronometer.backwards');
            backwardsCbox.appendChild(backwardsLabel);


            var time = document.createElement('div');
            time.className = 'altea-chronometer-time';
            dom.chrono.appendChild(time);

            dom.minutes = document.createElement('div');
            dom.minutes.className = 'altea-chronometer-minutes';
            dom.minutes.innerText = '00';
            time.appendChild(dom.minutes);

            var colon = document.createElement('div');
            colon.className = 'altea-chronometer-colon';
            colon.innerText = ':';
            time.appendChild(colon);

            dom.seconds = document.createElement('div');
            dom.seconds.className = 'altea-chronometer-seconds';
            dom.seconds.innerText = '00';
            time.appendChild(dom.seconds);


            var buttons = document.createElement('div');
            buttons.className = 'altea-chronometer-buttons';
            dom.chrono.appendChild(buttons);

            dom.start = document.createElement('div');
            dom.start.className = 'altea-chronometer-start button orange';
            dom.start.innerText = i18n.t('start');
            buttons.appendChild(dom.start);
            jdom.start = $(dom.start);

            dom.stop = document.createElement('div');
            dom.stop.className = 'altea-chronometer-stop button blueA disabled';
            dom.stop.innerText = i18n.t('stop');
            buttons.appendChild(dom.stop);
            jdom.stop = $(dom.stop);

            dom.restart = document.createElement('div');
            dom.restart.className = 'altea-chronometer-restart button blueA disabled';
            dom.restart.innerText = i18n.t('restart');
            buttons.appendChild(dom.restart);
            jdom.restart = $(dom.restart);
        }(window.document));

        this.id = id;

        this.getActualTime = function() {
            return actualTime;
        }

        this.getMaxTime = function() {
            return maxTime;
        }

        this.setMaxTime = function(seconds) {
            clearTimeout(interval);

            if (seconds <= 0) {
                return false;
            } else if (backwards) {
                actualTime = seconds;
            } else {
                actualTime = 0;
            }

            maxTime = seconds;
            redraw();
            return true;
        }

        this.isBackwards = function() {
            return backwards;
        }

        var setBackwards = function(evt) {
            if (started) {
                return false;
            }

            window.aaa = evt.target;
            backwards = evt.target.checked;

            if (backwards) {
                actualTime = maxTime;
            } else {
                actualTime = 0;
            }

            jdom.start.removeClass('disabled');
            jdom.restart.addClass('disabled');
            redraw();
            return true;
        }

        dom.backwards.onchange = setBackwards;

        var setArrow = function(evt) {
            if (started) {
                return false;
            }

            var target = evt.target;
            var parent = target.parentNode;
            var increase, minutes;
            var actualCount, actualOther, finalCount;

            switch (target.className) {
            case 'altea-chronometer-up-arrow':
                increase = true;
                break;

            case 'altea-chronometer-down-arrow':
                increase = false;
                break;

            default:
                return false;
            }

            switch (parent.className) {
            case 'altea-chronometer-set-minutes':
                minutes = true;
                actualCount = (maxTime / 60) | 0;
                actualOther = (maxTime % 60) | 0;
                break;

            case 'altea-chronometer-set-seconds':
                minutes = false;
                actualCount = (maxTime % 60) | 0;
                actualOther = (maxTime / 60) | 0;
                break;

            default:
                return false;
            }

            if (increase) {
                if (actualCount + 1 > 59) {
                    finalCount = 0;
                } else {
                    finalCount = actualCount + 1;
                }
            } else {
                if (actualCount - 1 < 0) {
                    finalCount = 59;
                } else {
                    finalCount = actualCount - 1;
                }
            }

            if (minutes) {
                finalCount *= 60;
                finalCount += actualOther;
            } else {
                finalCount += (actualOther * 60);
            }

            maxTime = finalCount;

            if (backwards) {
                actualTime = maxTime;
            } else {
                actualTime = 0;
            }

            jdom.start.removeClass('disabled');
            jdom.restart.addClass('disabled');
            redraw();
            return true;
        }

        dom.setMinutesUpArrow.onclick = setArrow;
        dom.setMinutesDownArrow.onclick = setArrow;
        dom.setSecondsUpArrow.onclick = setArrow;
        dom.setSecondsDownArrow.onclick = setArrow;

        this.show = function() {
            if (!shown) {
                redraw();
                shown = true;
                dialog.dialog('open');
                return true;
            } else {
                return false;
            }
        }

        this.hide = function() {
            if (shown) {
                shown = false;
                dialog.dialog('close');
                return true;
            } else {
                return false;
            }
        }

        var start = function() {
            if (started) {
                return false;
            }

            var x;

            if (backwards) {
                x = function() {
                    actualTime--;
                    redraw();

                    if (actualTime === 0) {
                        stopClick();
                    } else {
                        interval = setTimeout(x, 1000);
                    }
                }
            } else {
                x = function() {
                    actualTime++;
                    redraw();

                    if (actualTime === maxTime) {
                        stopClick();
                    } else {
                        interval = setTimeout(x, 1000);
                    }
                }
            }

            interval = setTimeout(function() {
                x();
                jdom.restart.removeClass('disabled');
            }, 1000);
            started = true;
            return true;
        }

        dom.start.onclick = function() {
            if (!jdom.start.hasClass('disabled')) {
                jdom.start.addClass('disabled');
                jdom.stop.removeClass('disabled');
                jdom.arrows.addClass('inactive');
                jdom.numbers.addClass('inactive');
                jdom.backwards.addClass('inactive');
                jdom.backwardsCbox.prop('disabled', true);
                start();
            }
        }

        var stop = function() {
            if (!started) {
                return false;
            }

            clearTimeout(interval);
            started = false;
            return true;
        }

        var stopClick = function() {
            if (!jdom.stop.hasClass('disabled')) {
                if ((backwards && actualTime === 0) || (!backwards && actualTime === maxTime)) {
                    jdom.start.addClass('disabled');
                } else {
                    jdom.start.removeClass('disabled');
                }

                jdom.stop.addClass('disabled');
                jdom.numbers.removeClass('inactive');
                jdom.arrows.removeClass('inactive');
                jdom.backwards.removeClass('inactive');
                jdom.backwardsCbox.prop('disabled', false);
                stop();
            }
        }

        dom.stop.onclick = stopClick;

        var restart = function() {
            if (started) {
                clearTimeout(interval);
            }

            if (backwards) {
                actualTime = maxTime;
            } else {
                actualTime = 0;
            }

            redraw();

            if (started) {
                started = false;
                start();
            } else {
                jdom.start.removeClass('disabled');
            }
            return true;
        }

        dom.restart.onclick = function() {
            if (!jdom.restart.hasClass('disabled')) {
                jdom.restart.addClass('disabled');
                restart();
            }
        }


        this.destroy = function() {
            stop();
            dom.chrono = undefined;
            dom = undefined;
        }

        function redraw() {
            var maxTimeMinutes = (maxTime / 60) | 0,
                maxTimeSeconds = (maxTime % 60) | 0;

            var actualTimeMinutes = (actualTime / 60) | 0,
                actualTimeSeconds = (actualTime % 60) | 0;

            dom.setMinutesText.innerText = maxTimeMinutes.padZeros(2);
            dom.setSecondsText.innerText = maxTimeSeconds.padZeros(2);

            dom.backwards.checked = backwards;

            dom.minutes.innerText = actualTimeMinutes.padZeros(2);
            dom.seconds.innerText = actualTimeSeconds.padZeros(2);
        }
    }

    // ReSharper disable once InconsistentNaming
    var chronometer = new function() {
        var chronometers = Object.create(null);

        this.create = function(id) {
            if (id === undefined ||
                    id === null ||
                    Object.prototype.hasOwnProperty.call(chronometers, id)
            ) {
                return false;
            }

            var chronometer = new Chrono(id);
            chronometers[id] = chronometer;
            return chronometer;
        };

        this.get = function(id) {
            if (id === undefined ||
                    id === null ||
                    !Object.prototype.hasOwnProperty.call(chronometers, id)
            ) {
                return false;
            }

            return chronometers[id];
        }

        this.delete = function(id) {
            if (id === undefined ||
                    id === null ||
                    !Object.prototype.hasOwnProperty.call(chronometers, id)
            ) {
                return false;
            }

            var chronometer = chronometers[id];
            chronometer.destroy();
            delete chronometers[id];
            return true;
        }
    }

    var chrono = chronometer.create("teacher-chrono");

    $(dom.button).on("click", function() {
        chrono.show();
    });
});
