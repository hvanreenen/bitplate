(function ($) {

    $.fn.extend({

        checkFunctionalityElements: function (func) {
            $(this).find('[data-functionality]').each(function () {
                var foundFunc = $(this).attr('data-functionality');
                if (foundFunc.indexOf(func) != -1) {
                    $(this).show();
                }
                else {
                    $(this).hide();
                }
            });
        }
    });
})(jQuery);