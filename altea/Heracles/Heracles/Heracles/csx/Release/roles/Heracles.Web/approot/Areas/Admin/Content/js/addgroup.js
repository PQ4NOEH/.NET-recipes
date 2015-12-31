window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    if (window.admin === undefined) {
        window.admin = {};
    }

    var api = Object.create({
        createUser: "/Admin/Users/Create"
    });

    var dom = Object.create({
        mpdId: "adm-add-grp",
        mpdContent: null,
        timetableWrapper: null
    });

    function addTimetableBox(active) {
        var box = document.createElement("div");
        box.className = "adm-add-grp-tt-bx";
        dom.timetableWrapper.appendChild(box);

        if (!active) {
            box.className += " adm-add-grp-tt-bx-ds";
            $(box).one("click", function() {
                this.classList.remove("adm-add-grp-tt-bx-ds");
                addTimetableBox(false);
                window.mpd.resize();
            });
        }

        var close = document.createElement("button");
        close.className = "adm-add-grp-tt-bx-cl";
        close.innerText = "×";
        box.appendChild(close);

        $(close).on("click", function() {
            dom.timetableWrapper.removeChild(this.parentNode);
            window.mpd.resize();
        });

        var weekDay = document.createElement("select");
        weekDay.className = "adm-add-grp-tt-bx-wd input";
        box.appendChild(weekDay);

        var weekDayPlaceholder = document.createElement("option");
        weekDayPlaceholder.value = -1;
        weekDayPlaceholder.disabled = true;
        weekDayPlaceholder.selected = true;
        weekDayPlaceholder.innerText = i18n.t("DayOfWeek");
        weekDay.appendChild(weekDayPlaceholder);

        var timeWrapper = document.createElement("div");
        timeWrapper.className = "adm-add-grp-tt-bx-tw";
        box.appendChild(timeWrapper);

        var time = document.createElement("input");
        time.className = "adm-add-grp-tt-bx-tm input time start";
        time.placeholder = i18n.t("Hour");
        timeWrapper.appendChild(time);

        var duration = document.createElement("input");
        duration.className = "adm-add-grp-tt-bx-dr input time end";
        duration.placeholder = i18n.t("Duration");
        timeWrapper.appendChild(duration);

        var teacher = document.createElement("select");
        teacher.className = "adm-add-grp-tt-bx-th input";
        box.appendChild(teacher);

        var teacherPlaceholder = document.createElement("option");
        teacherPlaceholder.value = -1;
        teacherPlaceholder.disabled = true;
        teacherPlaceholder.selected = true;
        teacherPlaceholder.innerText = i18n.t("Teacher");
        teacher.appendChild(teacherPlaceholder);

        var station = document.createElement("select");
        station.className = "adm-add-grp-tt-bx-st input";
        box.appendChild(station);

        var stationPlaceholder = document.createElement("option");
        stationPlaceholder.value = -1;
        stationPlaceholder.disabled = true;
        stationPlaceholder.selected = true;
        stationPlaceholder.innerText = i18n.t("Station");
        station.appendChild(stationPlaceholder);

        $(timeWrapper).find(".input.time").timepicker({
            step: 15,
            showDuration: true,
            disableTextInput: true
        });
    }

    function createPopup() {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        dom.mpdContent = window.mpd.create(
            dom.mpdId,
            "900px",
            i18n.t("admin.addgroup"),
            { closebtn: true }
        );

        var name = document.createElement("input");
        name.id = "adm-add-grp-gn";
        name.className = "input";
        name.placeholder = i18n.t("GroupName");
        dom.mpdContent.appendChild(name);

        var levelWrapper = document.createElement("div");
        levelWrapper.id = "adm-add-grp-lw";
        dom.mpdContent.appendChild(levelWrapper);

        var level = document.createElement("select");
        level.id = "adm-add-grp-lv";
        level.className = "input";
        levelWrapper.appendChild(level);

        var levelPlaceholder = document.createElement("option");
        levelPlaceholder.value = -1;
        levelPlaceholder.disabled = true;
        levelPlaceholder.selected = true;
        levelPlaceholder.innerText = i18n.t("Level");
        level.appendChild(levelPlaceholder);

        var call = document.createElement("select");
        call.id = "adm-add-grp-cl";
        call.className = "input";
        levelWrapper.appendChild(call);

        var callPlaceholder = document.createElement("option");
        callPlaceholder.value = -1;
        callPlaceholder.disabled = true;
        callPlaceholder.selected = true;
        callPlaceholder.innerText = i18n.t("ExamCall");
        call.appendChild(callPlaceholder);

        var datesWrapper = document.createElement("div");
        datesWrapper.id = "adm-add-grp-dw";
        dom.mpdContent.appendChild(datesWrapper);

        var start = document.createElement("input");
        start.id = "adm-add-grp-sd";
        start.className = "input";
        start.placeholder = i18n.t("StartDate");
        datesWrapper.appendChild(start);

        var end = document.createElement("input");
        end.id = "adm-add-grp-ed";
        end.className = "input";
        end.placeholder = i18n.t("EndDate");
        datesWrapper.appendChild(end);

        var notes = document.createElement("input");
        notes.id = "adm-add-grp-nt";
        notes.className = "input";
        notes.placeholder = i18n.t("Notes");
        dom.mpdContent.appendChild(notes);

        var timetable = document.createElement("div");
        timetable.id = "adm-add-grp-tt";
        dom.mpdContent.appendChild(timetable);
        dom.timetableWrapper = timetable;

        addTimetableBox(false);

        var create = document.createElement("button");
        create.id = "adm-usr-add-cte";
        create.className = "input input-bl";
        create.innerText = i18n.t("Create");
        create.disabled = true;
        dom.mpdContent.appendChild(create);
    }

    window.admin.addGroup = function() {
        createPopup();
        window.mpd.show();
    }
});
