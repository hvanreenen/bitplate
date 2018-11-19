//js voor zoekbox bovenin

(function ($) {

    $.fn.extend({

        //pass the options variable to the function
        searchable: function (searchfunction) {
            if ($(this).length > 0) {
                var textbox = $(this);
                var searchButton = $('#bitSearchTextboxEnterSearch');
                var clearButton = $('#bitClearSearchButton');

                $(searchButton).click(function () {
                    var searchString = $(textbox).val();
                    if (searchfunction) {
                        searchfunction(searchString);
                    }
                });

                $(clearButton).click(function () {
                    $(textbox).val("");
                    if (searchfunction) {
                        searchfunction("");
                    }
                    $(clearButton).hide('fast');
                });

                if (textbox.attr("type") != "text") {
                    alert("searchable moet een textbox zijn.");
                    return;
                }

                $(textbox).focusin(function () {
                    var text = $(this).val();
                    if (text != '') {
                        //$('a.bitTextboxResetX').fadeIn('slow');
                        //clearButton
                        $(clearButton).fadeIn('slow');
                    }
                });
                $(textbox).focusout(function () {
                    var text = $(this).val();
                    if (text == '') {
                        $(clearButton).hide('fast');
                    }
                    //    //$('a.bitTextboxResetX').hide('fast');
                });
                $(textbox).keydown(function (event) {
                    $(clearButton).fadeIn('slow');
                    //$('a.bitTextboxResetX').fadeIn('slow');
                    //check if enter

                    if (event.which == 13) {
                        if (searchfunction) {
                            var text = $(textbox).val();
                            searchfunction(text);
                        }
                    }
                });
            }
        }
    });
})(jQuery);