
(function ($) {

    $.fn.extend({

        //pass the options variable to the function
        dockable: function () {

            //var test = $(this).html();
            $(this).find('.dockPanelTitle').click(function () {
                var panel = $(this).next('.dockPanelContent');
                if (panel.is(':visible')) {
                    panel.slideUp(300);
                    $(this).find('.arrow').html('&#9658;');
                }
                else {
                    panel.slideDown(300);
                    $(this).find('.arrow').html('&#9660;');
                }
            });

            $(this).find('.dockPanelContent').hide();
            $(this).find('.arrow').html('&#9658;');
            $(this).find('.dockPanelTitle.open').each(function (i) {
                var panel = $(this).next('.dockPanelContent');
                if (panel.is(':visible')) {
                    panel.slideUp(300);
                    $(this).find('.arrow').html('&#9658;');
                }
                else {
                    panel.slideDown(300);
                    $(this).find('.arrow').html('&#9660;');
                }
            });
           
        }
    });

})(jQuery);

