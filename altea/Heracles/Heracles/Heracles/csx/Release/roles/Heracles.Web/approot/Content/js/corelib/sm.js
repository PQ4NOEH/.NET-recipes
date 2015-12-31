(function (window, document, undefined) {
    "use strict";

    function sm() {
        var soundManager = new SoundManager();


        var smOptions = {},
            smSound = undefined;

        delete window.soundManager;
        delete window.SoundManager;

        soundManager.setup({
            url: "/Content/swf/soundManager2_flash9.swf",
            flashVersion: 9,
            debugMode: false,
            debugFlash: false,
            preferFlash: false,
            useFlashBlock: false,
            useHTML5Audio: true
        });

        var isSetup = false,
            speechTypes = [],
            speechSelected = [];

        function play(id, url, options) {
            if (smSound === undefined || smSound.id !== id) {
                smSound = soundManager.getSoundById(id);

                if (smSound === undefined) {
                    smSound = soundManager.createSound({
                        id: id,
                        url: url,
                        onload: function () {
                            if (typeof options.callback === "function") {
                                options.callback();
                            }
                        },
                        onfinish: function() {
                            if (typeof options.finishback === "function") {
                                options.finishback();
                            }
                        }
                    });

                    smSound.destroyable = options.destroyable;
                } else if (typeof options.callback === "function") {
                    options.callback();
                }
            } else if (typeof options.callback === "function") {
                options.callback();
            }
        }

        function stop(id) {
            if (smSound !== undefined) {
                smSound.stop();

                if (smSound.id !== id) {
                    if (smSound.destroyable) {
                        smSound.destruct();
                    }

                    smSound = undefined;
                }
            }
        }

        function playFile(args) {
            var file = args[0],
                options = args[1] || {};

            stop(file);
            play(file, file, options);

            if (options.autoplay) {
                smSound.play({
                    whileplaying: function() {
                        if (typeof options.whileplaying === "function") {
                            options.whileplaying();
                        }
                    }
                });
            }

            smOptions = options;
        }

        function playSpeech(args) {
            var word = args[0],
                type = args[1],
                options = args[2] || {};

            if (typeof type !== "string" || (type !== "from" && type !== "to")) {
                type = 1;
            } else {
                type = type === "from" ? 1 : 0;
            }

            var id = word + "_" + type + "_" + speechSelected[type];

            stop(id);
            play(id, "/WiseLab/Speech/" + type + "/" + String.fromCharCode(160) + word, options);

            if (options.autoplay) {
                smSound.play({
                    whileplaying: function () {
                        if (typeof options.whileplaying === "function") {
                            options.whileplaying();
                        }
                    }
                });
            }

            smOptions = options;
        }

        this.setup = function(options) {
            if (isSetup) {
                return false;
            }

            isSetup = true;
            speechTypes = options.speechTypes;
            speechSelected = options.speechSelected;
            return true;
        };

        this.play = function () {
            if (!isSetup) {
                return false;
            }

            if (arguments.length === 0) {
                if (smSound === undefined || !smOptions.control) {
                    return false;
                }

                play(smSound.id, null, {});
            } else if (arguments.length === 2) {
                playFile(arguments);
            } else if (arguments.length === 3) {
                playSpeech(arguments);
            }

            return true;
        }
    }

    window.sm = new sm();
})(window, document);
