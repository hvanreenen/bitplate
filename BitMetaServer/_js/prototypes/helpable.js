(function ($) {
    $.fn.extend({
        helpable: function () {
            $(this).find('.bitInfo').click(function () {
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
            });
        }
    });
})(jQuery);