window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    var index = new window.desks.index();

    var ENUM_TYPES = Object.create({
        Index: 1
    });

    var rows = [];

    //#region Draw
    function drawColumnTitle(title, container) {
        var t = document.createElement("div");
        t.className = "pec-thdr";
        t.innerText = title;
        container.appendChild(t);
    }

    function drawRowTitle(title, container) {
        var t = document.createElement("div");
        t.className = "pec-hdr";
        t.innerText = title;
        container.appendChild(t);
    }

    function drawColumnRow(row, container) {
        var r = document.createElement("div");
        r.className = "pec-clm-cr";
        r.style.flexGrow = row.rows;
        container.appendChild(r);

        drawRowTitle(row.name, r);

        var rc = document.createElement("div");
        rc.className = "pec-clm-rc";
        r.appendChild(rc);

        var rcw = document.createElement("div");
        rcw.className = "pec-clm-rcw";
        rc.appendChild(rcw);

        var rcc = document.createElement("div");
        rcc.className = "pec-clm-rcc";
        rcw.appendChild(rcc);

        rows.push([row, rcc]);
    }

    function drawColumnCRows(crows, container) {
        var rc = document.createElement("div");
        rc.className = "pec-clm-crc";
        container.appendChild(rc);

        for (var i = 0, l = crows.length; i < l; i++) {
            drawColumnRow(crows[i], rc);
        }
    }

    function drawColumn(column, container) {
        var c = document.createElement("div");
        c.className = "pec-clm";
        c.style.flexGrow = column.columns;
        container.appendChild(c);

        var cc = document.createElement("div");
        cc.className = "pec-clm-c";
        c.appendChild(cc);

        if (column.name !== null) {
            drawColumnTitle(column.name, cc);
        }

        var crs = document.createElement("div");
        crs.className = "pec-clm-crs";
        cc.appendChild(crs);

        for (var i = 0, l = column.rows.length; i < l; i++) {
            drawColumnCRows(column.rows[i], crs);
        }

        return c;

    }

    function drawGrid(columns, container) {
        var cols = [];

        for (var i = 0, l = columns.length; i < l; i++) {
            var c = drawColumn(columns[i], container);
            cols.push(c);
        }

        return cols;
    }

    function drawIndexRows(data) {
        var indexRows = {};

        for (var i = 0, l = rows.length; i < l; i++) {
            if (rows[i][0].type === ENUM_TYPES.Index) {
                indexRows[rows[i][0].area] = rows[i][1];
            }
        }

        index.draw(indexRows, data.index, null);
    }

    function draw(columns, data, container) {
        var cols = drawGrid(columns, container);
        drawIndexRows(data);

        $(document.getElementsByClassName("pec-clm-rcc")).mCustomScrollbar({
            axis: "y",
            scrollbarPosition: "inside",
            alwaysShowScrollbar: 0,
            scrollButtons: { enable: true },
            theme: "dark-3"
        });

        return cols;
    }

//#endregion

    //#region Assignments
    function assignments(assignments) {
    }

//#endregion

    window.prodesks = Object.create({
        draw: draw,
        assignments: assignments
    });
});
