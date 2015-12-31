function getSelectionData(selection, onlySelected) {
    if (typeof onlySelected === "undefined") {
        onlySelected = false;
    }

    var focusObject = selection.focusNode;
    var parentObject = focusObject.parentNode;

    var selectionText = selection.toString().trim();

    if (onlySelected) {
        return selectionText;
    }

    return Object.create({
        selection: selectionText,
        context: (typeof focusObject.textContent === "undefined" ? focusObject.wholeText : focusObject.textContent).trim(),
        parent: (typeof parentObject.textContent === "undefined" ? parentObject.innerText : parentObject.textContent).trim()
    });
}

function getPhrase(w, c, p) {
    // There be dragons, enjoy :)

    c = c.replace(/\n/g, ' ');
    p = p.replace(/\n/g, ' ');
    var erx = /([[\^\$\*\+\{\}\[\]\(\)\\\.\?])/g;
    var we = w.replace(erx, '\\$1');
    var c1 = new RegExp('[\\.¿¡…:][^\\.¿¡…:]*' + we + '[^\\.\\?!…:]*[\\.\\?!…:]');
    var c2 = new RegExp('[\\.¿¡…:][^\\.¿¡…:]*' + we + '[^\\.\\?!…:]*');
    var c3 = new RegExp('[^\\.¿¡…:]*' + we + '[^\\.\\?!…:]*[\\.\\?!…:]');
    var r = '';
    if (c.indexOf(w) < 0) return null;
    if (c1.test(c)) {
        r = c.match(c1)[0];
    } else if (c2.test(c)) {
        var cs = c.match(c2)[0];
        var ce = cs.replace(erx, '\\$1');
        if (new RegExp(ce + '$').test(p)) {
            r = cs;
        } else {
            var e = new RegExp(ce + '[^\\.\\?!…]*[\\.\\?!…$]', 'g');
            var t = p.match(e);
            if (t !== null)
                r = t[0];
            else {
                e = new RegExp(ce + '[^\\.\\?!…:]*[\\.\\?!…:$]', 'g');
                t = p.match(e);
                if (t !== null)
                    r = t[0];
            }
        }
    } else if (c3.test(c)) {
        var cs = c.match(c3)[0];
        var ce = cs.replace(erx, '\\$1');
        if (new RegExp('^' + ce).test(p)) {
            r = cs;
        } else {
            var e = new RegExp('(^|[\\.¿¡…])[^\\.¿¡…]*' + ce, 'g');
            var t = p.match(e);
            if (t !== null)
                r = t[0];
            else {
                e = new RegExp('(^|[\\.¿¡…:])[^\\.¿¡…:]*' + ce, 'g');
                t = p.match(e);
                if (t !== null)
                    r = t[0];
            }
        }
    } else {
        var ce = c.replace(erx, '\\$1');
        var c41 = new RegExp('[\\.¿¡…][^\\.¿¡…]*' + ce + '[^\\.\\?!…]*[\\.\\?!…]');
        var c412 = new RegExp('[\\.¿¡…:][^\\.¿¡…:]*' + ce + '[^\\.\\?!…:]*[\\.\\?!…:]');
        var c42 = new RegExp('[\\.¿¡…][^\\.¿¡…]*' + ce + '[^\\.\\?!…]*');
        var c422 = new RegExp('[\\.¿¡…:][^\\.¿¡…:]*' + ce + '[^\\.\\?!…:]*');
        var c43 = new RegExp('[^\\.¿¡…]*' + ce + '[^\\.\\?!…]*[\\.\\?!…]');
        var c432 = new RegExp('[^\\.¿¡…:]*' + ce + '[^\\.\\?!…:]*[\\.\\?!…:]');

        if (c41.test(p)) r = p.match(c41)[0];
        else if (c412.test(p)) r = p.match(c412)[0];
        else if (c42.test(p)) r = p.match(c42)[0];
        else if (c422.test(p)) r = p.match(c422)[0];
        else if (c43.test(p)) r = p.match(c43)[0];
        else if (c432.test(p)) r = p.match(c432)[0];
        else if (c === p) r = p;
    } if (r.length !== 0 && (r[0] === '.' || r[0] === '…' || r[0] === ':'))
        r = r.substring(1).trim();
    return r;
}

function clearSelection(w) {
    if (w.getSelection) {
        if (w.getSelection().empty) {  // Chrome
            w.getSelection().empty();
        } else if (w.getSelection().removeAllRanges) {  // Firefox
            w.getSelection().removeAllRanges();
        }
    } else if (document.selection) {  // IE?
        document.selection.empty();
    }
}