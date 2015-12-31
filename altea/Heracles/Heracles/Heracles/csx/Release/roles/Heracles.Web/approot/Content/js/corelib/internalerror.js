(function (window, document) {
    "use strict";

    window.setInternalErrorMessage = function (wrapper, text) {
        var element = document.createElement('p');
        element.className = 'alt-err';
        element.innerText = text === undefined || text === null ? i18n.t('internal-error') : text;
        wrapper.mCustomScrollbar('destroy');

        wrapper.empty();
        wrapper.html(element);
        wrapper.addClass('alt-err-w');
    }
})(window, document);
