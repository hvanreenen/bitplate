Date.prototype.format = function (mask) {
    var date = new Date(this);
    var dateString = "";
    if (!isNaN(date.getTime())) {

        var dateString = mask;
        if (date.getDay() == 0) date.setDate(1);
        if (date.getMonth() == 0) date.setMonth(1);
        var day = date.getDate();
        var month = date.getMonth() + 1; //!!maanden in js van 0-11
        if (day < 10 && dateString.indexOf("dd") >= 0) {
            day = "0" + day;
        }
        if (month < 10 && dateString.indexOf("MM") >= 0) {
            month = "0" + month;
        }
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();
        if (hours < 10 && dateString.indexOf("HH") >= 0) {
            hours = "0" + hours;
        }
        if (minutes < 10 && dateString.indexOf("mm") >= 0) {
            minutes = "0" + minutes;
        }
        if (seconds < 10 && dateString.indexOf("ss") >= 0) {
            seconds = "0" + seconds;
        }
        dateString = dateString.replace("dd", day);
        dateString = dateString.replace("MM", month);
        dateString = dateString.replace("yyyy", date.getFullYear());
        dateString = dateString.replace("yy", date.getYear());
        dateString = dateString.replace("HH", hours);
        dateString = dateString.replace("mm", minutes);
        dateString = dateString.replace("ss", seconds);
    }
    return dateString;
}

String.prototype.toDateFromFormat = function (format) {

    var day, month, year;
    var dateString = this;
    if (dateString == "") return "";
    if (format == "") return new Date(dateString);
    var posStart = format.indexOf("dd");
    if (posStart >= 0) {
        day = dateString.substr(posStart, 2);
        if(day.indexOf("0") ==0){
            day = day.replace("0", "");
        }
        day = parseInt(day);
    }
    posStart = format.indexOf("MM");
    if (posStart >= 0) {
        month = dateString.substr(posStart, 2);
        if (month.indexOf("0") == 0) {
            month = month.replace("0", "");
        }
        month = parseInt(month);
        month--; //!! maand in js telst van 0-11
    }
    posStart = format.indexOf("yyyy");
    if (posStart >= 0) {
        year = dateString.substr(posStart, 4);
        year = parseInt(year);
    }
    var date = new Date(year, month, day);
    return date;
}