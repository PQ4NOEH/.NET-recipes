function createNoty(type, layout, text, timeout, closeWith, parameters) {
    if (typeof timeout === "undefined") {
        timeout = 1500;
    }

    if (typeof closeWith === "undefined") {
        closeWith = [];
    }

    if (typeof parameters !== "object" || parameters === null) {
        parameters = Object.create(null);
    }

    parameters.layout = layout;
    parameters.theme = "relax";
    parameters.text = text;
    parameters.type = type;
    parameters.timeout = timeout;
    parameters.closeWith = closeWith;

    if (parameters.animation === undefined || typeof parameters.animation !== "object") {
        parameters.animation = {
            open: 'animated flipInX',
            close: 'animated flipOutX'
        }
    }

    return noty(parameters);
}

function createConfirmNoty(layout, text, buttons, onShow) {
    return noty({
        layout: layout,
        theme: "relax",
        text: text,
        type: "confirm",
        buttons: buttons,
        animation: {
            open: 'animated bounceIn',
            close: 'animated bounceOut'
        },
        callback: {
            onShow: onShow
        }
    });
}