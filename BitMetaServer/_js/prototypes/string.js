String.prototype.capitalize = function () {
    return this.charAt(0).toUpperCase() + this.slice(1);
}
if (!String.prototype.trim) {
    String.prototype.trim = function () {
        return this.replace(/^\s+|\s+$/g, "");
    };
}
String.prototype.replaceAll = function (search, replacement) {
    return this.split(search).join(replacement);
};
String.prototype.replaceAll2 = function (search, replacement) {
    return this.replace(new RegExp(search, 'g'), replacement);
}

String.prototype.highlight = function (searchString, className) {
    
    if (!className) {
        className = '.highlight';
    }
    var ret = this.replaceAll2(searchString, "<span class=\"" + className + "\">" + searchString + "</span>");
    return ret;
    
};

String.prototype.toUnicode = function() {
    var unicodeString = '';
    var theString = this;
    for (var i = 0; i < theString.length; i++) {
        var theUnicode = theString.charCodeAt(i).toString(16).toUpperCase();
        while (theUnicode.length < 4) {
            theUnicode = '0' + theUnicode;
        }
        theUnicode = '\\u' + theUnicode;
        unicodeString += theUnicode;
    }
    return unicodeString;
}

String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

String.prototype.escape = function () {
    return escape(this);
}

String.prototype.jEscape = function () {
    //return JSON.stringify(this).replace(/&/, "&amp;").replace(/"/g, "&quot;")
    //return JSEncode(this);
    return this.toString();
}