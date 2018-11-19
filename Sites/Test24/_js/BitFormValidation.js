var isFormValid;
$.fn.validate = function () {
    isFormValid = true;
    //var elements = $(this).find('[data-validation]');
    //for (var i in elements) {
    //    var elm = elements[i];
    //    if (!validate($(elm))) {
    //        isFormValid = false;
    //    }
    //}
    $(this).find('[data-validation]').each(function (i) {
        if (!validate($(this))) {
            isFormValid = false;
        }
    });
    return isFormValid;
};

jQuery.validationReset = function () {
    $(document).find('[data-validation]').each(function (i) {
        $(this).next('span.validationMsg').remove();
        $(this).removeClass('invalid');
        $(this).unbind('keyup');
        $('a[href^="#"]').css({ 'border': 'none', 'border-bottom': 'none' });
    });
}

function validate(control) {
    var isValid = true;
    //onzichtbare controls niet checken
    var bool = $(control).is(":visible");
    if (!$(control).is(":visible")) return true;
    //kunnen meerdere validaties zijn, met komma gescheiden
    var datavalidations = $(control).attr('data-validation').split(',');
    var msg = "";
    for (var i in datavalidations) {
        var validationType = datavalidations[i].toLowerCase().trim();
        if (validationType == "required") {
            var val = $(control).val();
            if (control[0].tagName == "IMG") {
                if ($(control).attr('src') == 'null' || $(control).attr('src') == '') {
                    isValid = false;
                    msg = "verplicht veld";
                }
            }
            else if (val == '' || val == null) {
                isValid = false;
                msg = "verplicht veld";
            }
        }
        if (validationType == "number") {
            if (!isInteger($(control).val())) {
                isValid = false;
                if (msg != "") {
                    msg += ", ";
                }
                msg += "alleen gehele getallen";
            }
        }
        if (validationType == "decimal") {
            if (!isNumber($(control).val())) {
                isValid = false;
                if (msg != "") {
                    msg += ", ";
                }
                msg += "geen geldig nummer";
            }
        }
        if (validationType == "date") {
            if (!isDate($(control).val())) {
                isValid = false;
                if (msg != "") {
                    msg += ", ";
                }
                msg += "geen geldige datum";
            }
        }
        if (validationType == "email") {
            var email = $(control).val();
            apos = email.indexOf("@");
            dotpos = email.lastIndexOf(".");
            if (apos < 1 || dotpos - apos < 2) {
                isValid = false;
                if (msg != "") {
                    msg += ", ";
                }
                msg += "geen geldige email";
            }
        }
    }

    $(control).next('span.validationMsg').remove();
    $(control).removeClass('invalid');
    $(control).unbind('keyup');
    
    if (!isValid) {
        $(control).addClass('invalid');
        $(control).focus();
        $(control).keyup(function () { validate(control) });
        $(control).after("<span class='validationMsg'> " + msg + "</span>");
    }
    
    return isValid;
}

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function isInteger(n) {
    var valid = false;
    if (n == "" || n == null) return true;
    var test = !isNaN(parseInt(n)) && isFinite(n);
    if (test && parseInt(n) == n) {
        valid = true;
    }
    return valid;
}

function isDate(dt) {
    return !isNaN(new Date(dt).getTime());
}
