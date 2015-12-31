jQuery.fn.wordcounter = function () {
    var doc = this.clone();
    doc.find("style, script").remove();

    var words = $.trim(doc.text().replace(/[\s\n]+/g, " ")).split(" ");
    var numWords = words.length;
    var numChars = 0;
    words.forEach(function (word) { numChars += word.length; });

    return [numWords, numChars];
}
