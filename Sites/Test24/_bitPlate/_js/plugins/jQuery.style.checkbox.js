/*
-----::: ABCnet.nl (c) 2012 :::-----
DATE: 09/05/2012
EDITED: 10/05/2012
DESCRIPTION: This is the JavaScript required to toggle checkboxes and radiobuttons Requires jQuery library
*/
$(document).ready(function () {
    // assuming all checkboxes are unchecked at first
    $("span[class='checkbox']").addClass("unchecked");

    $(".checkbox").click(function () {
        if ($(this).children("input").attr("checked")) {
            // uncheck
            $(this).children("input").attr({ checked: "" });
            $(this).removeClass("checked");
            $(this).addClass("unchecked");
        }
        else {
            // check
            $(this).children("input").attr({ checked: "checked" });
            $(this).removeClass("unchecked");
            $(this).addClass("checked");
        }
        //alert($(this).children("input").attr("checked"));
    });


    /*
			#s_abc_group_0 = (s)pan for radio button
			#l_abc_group_0 = (l)abel for radiobutton

		*/
    // assuming all radio are unchecked at first

    $("span[class='radio']").addClass("runchecked");

    $(".radio").click(function () {
        var radio = $(this).children("input");
        var group = $(radio).attr("name");
        var value = $(radio).attr("value");

        //eerst alle van deze group unchecken
        var elements = $("input[type=radio][name=" + group + "]"); //IE fix
        $(elements).each(function (i) {
            $(elements[i]).removeAttr('checked');
        });
        elements = $("span[title='" + group + "']");
        $(elements).each(function (i) {
            $(elements[i]).toggleClass('radio');
            $(elements[i]).removeClass('rchecked');
        });
        //daarna geklikte item checken
        $("input[type=radio][value=" + value + "]").attr('checked', 'checked');
        $(this).addClass('rchecked');
    });


});