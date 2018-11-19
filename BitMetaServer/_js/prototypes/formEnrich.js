//voert allerlei functie uit die op elk formulier moeten werken
//zoals tabs, vergroot verklein knoppen, help knoppen, tabs in textarea's enz.
(function ($) {
    $.fn.extend({
        formEnrich: function () {

            // Textarea vergrootknop
            $('.bitTextareaEnlargeButton').click(function () {
                $(this).toggleClass('bitTextareaShrinkButton');
                $(this).parent().next().find("textarea").toggleClass('bitTextareaLarge');
            });
            // Textarea verkleinknop
            $('.bitTextareaShrinkButton').click(function () {
                $(this).toggleClass('bitTextareaEnlargeButton');
                $(this).parent().next().find("textarea").toggleClass('bitTextarea');
            });

            //tabs toestaan in textarea
            $("textarea").tabby();

            // create datepickers
            if (!Modernizr.inputtypes.date) {
                var future = new Date().getFullYear() + 5;
                var past = future - 100;
                $('input[type=date]').datepicker({ changeMonth: true, changeYear: true, yearRange: past + ':' + future, dateFormat: 'dd-mm-yy' });
            }

            if (!Modernizr.inputtypes.datetime) {
                $('input[type=datetime]').datetimepicker({
                    dateFormat: 'dd-mm-yy'
                });
            }

            // tabs
            $(".bitTabs").tabs();

            // accorion
            $('.bitAccordion').accordion({ heightStyle: 'content' });

            //helpknoppen

            $('.bitInfo').tooltip();
            $('.bitInput').tooltip();

            /* $(this).find('.bitInfo').click(function () {
                if ($(this).next().hasClass('helpMsg') && $(this).next().is(':visible')) {
                    $(this).next().fadeOut(200);
                }
                else {
                    if (!$(this).next().hasClass('helpMsg')) {
                        $(this).after("<span class='helpMsg'><img alt='close' src='_img/icons/close_small.png' class='helpMsgCloseButton'/>" + $(this).attr('title') + "</span>");
                        $('.helpMsg').click(function () {
                            $(this).fadeOut(200);
                        });
                    }
                    $(this).next().fadeIn(200);
                }
            }); */
        }
    });

})(jQuery);