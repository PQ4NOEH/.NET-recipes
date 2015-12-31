(function (window, document, $, undefined) {
    var dom = Object.create({
        topCount: document.getElementById("ach-tp-p"),
        box: document.getElementById("ach-w"),
        boxMessage: document.getElementById("ach-n"),
        boxPoints: document.getElementById("ach-p")
    });

    var api = Object.create({
        get: "/Achievements/Get",
        unlocked: "/Achievements/Unlocked",
        unlock: "/Achievements/Unlock"
    });

    var achievements, userAchievements, points, l;


    function setAchievements(data) {
        if (data.achievements === undefined || data.userAchievements === undefined) {
            window.achievements.active = false;
            throw new Error("Invalid achievements data");
        }

        achievements = data.achievements;
        userAchievements = data.userAchievements;
        points = 0;
        l = achievements.length;
        window.achievements.active = true;
    }

    var validStorageAchievements;
    var storageAchievementsKey = "altea_6e373659f3c93670f67f66279ce312f1";
    if (window.sessionStorage) {
        var storageValue = window.sessionStorage.getItem(storageAchievementsKey);
        if (storageValue === undefined || storageValue === null) {
            validStorageAchievements = false;
        } else {
            var decodedStorageValue;
            if (window.atob) {
                decodedStorageValue = window.atob(storageValue);
            } else {
                decodedStorageValue = storageValue;
            }
            try {
                setAchievements(JSON.parse(decodedStorageValue));
                validStorageAchievements = true;
            } catch (e) {
                validStorageAchievements = false;
            }
        }
    } else {
        validStorageAchievements = false;
    }

    function isUnlocked(achievement, level) {
        for (var j = 0, m = userAchievements.length; j < m; j++) {
            if (userAchievements[j].id === achievement && userAchievements[j].level === level) {
                return true;
            }
        }

        return false;
    }

    function show(name, level, points) {
        box.classList.remove("ach-wa");
        boxMessage.innerText = i18n.t("achievements." + name + "." + level);
        boxPoints.innerText = points;
        box.classList.add("ach-wa");
    }

    function unlock(name, level) {
        if (window.achievements.active === true) {
            var achievement = null;
            var achievementLevel = null;

            for (var i = 0; i < l; i++) {
                if (achievements[i].name === name) {
                    achievement = achievements[i];

                    for (var j = 0, m = achievements[i].levels.length; j < m; j++) {
                        if (achievements[i].levels[j].level === level) {
                            achievementLevel = achievements[i].levels[j];
                            break;
                        }
                    }

                    break;
                }
            }

            if (achievement === null || achievementLevel === null || isUnlocked(achievement.id, level)) {
                return false;
            } else {
                $.ajax({
                    async: true,
                    type: "POST",
                    url: api.unlock,
                    dataType: "json",
                    data: {
                        achievement: achievement.id,
                        level: level
                    },

                    success: function (result) {
                        if (result.status) {
                            userAchievements.push(result.achievement);

                            if (window.sessionStorage) {
                                var storageAchievements = JSON.stringify({
                                    achievements: achievements,
                                    userAchievements: userAchievements
                                });
                                if (window.btoa) {
                                    window.sessionStorage.setItem(storageAchievementsKey, window.btoa(storageAchievements));
                                } else {
                                    window.sessionStorage.setItem(storageAchievementsKey, storageAchievements);
                                }
                            }

                            var parameters = {
                                title: i18n.t('achievements.unlocked') + '\n' + achievementLevel.title,
                                text: achievementLevel.description,
                                allowOutsideClick: true,
                                confirmButtonText: i18n.t('close'),
                                confirmButtonColor: '#81C4E4'
                            };

                            if (achievementLevel.image !== undefined && achievementLevel.image !== null) {
                                parameters.type = 'success';
                            } else {
                                parameters.imageUrl = achievementLevel.image;
                            }

                            swal(parameters);
                            show(name, level, points);
                            points += points;
                            dom.topCount.innerText = points;
                        }
                    }
                });

                return true;
            }
        }
    }

    var achievementsObject = Object.create({
        active: true,
        unlock: unlock
    });

    if (validStorageAchievements) {
        window.achievements = achievementsObject;
    } else {
        $.ajax({
            async: true,
            type: "POST",
            url: api.get,
            dataType: "json",

            success: function (result) {
                setAchievements(result);

                if (window.sessionStorage) {
                    var parsedResult = JSON.stringify(result);
                    if (window.btoa) {
                        window.sessionStorage.setItem(storageAchievementsKey, window.btoa(parsedResult));
                    } else {
                        window.sessionStorage.setItem(storageAchievementsKey, parsedResult);
                    }
                }

                window.achievements = achievementsObject;
            },
            error: function () {
                // TODO
            }
        });
    }
}(window, document, jQuery));