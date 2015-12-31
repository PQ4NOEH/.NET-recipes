function secondsToDisplayClock(seconds, alwaysFull, minDigits) {
    if (typeof alwaysFull !== "boolean") {
        alwaysFull = false;
    }

    if (typeof minDigits !== "number") {
        minDigits = 2;
    }

    var hours = parseInt(seconds / 3600);
    seconds = seconds % 3600;

    var minutes = parseInt(seconds / 3600);
    seconds = seconds % 3600;

    var text = "";

    if (hours > 0 || alwaysFull) {
        hours = hours + "";

        while (hours.length < minDigits) {
            hours = "0" + hours;
        }

        text = hours + ":";
    }

    minutes = minutes + "";
    while (minutes.length < minDigits) {
        minutes = "0" + minutes;
    }

    text += minutes + ":";

    seconds = seconds + "";
    while (seconds.length < minDigits) {
        seconds = "0" + seconds;
    }

    text += seconds;
    return text;
}