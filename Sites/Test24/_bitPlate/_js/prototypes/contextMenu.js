(function ($) {
    $.fn.extend({
        contextMenu: function (menu, options) {
            $(menu).menu();
            $(menu).hide();

            $(this).bind("contextmenu", function (e) {
                var menuProperties = new Object();
                menuProperties.element = menu;
                menuProperties.visable = true;
                if (options) {
                    if (options.onload) {
                        menuProperties = options.onload(this, menuProperties);
                    }
                }
                if (menuProperties.visable) {
                    $(menu).css({
                        top: e.pageY + 'px',
                        left: e.pageX + 'px',
                        position: 'absolute',
                        'z-index': 5000
                    }).show();
                }
                return false;

            });

            $(menu).click(function () {
                $(menu).hide();
            });
            $(document).click(function () {
                $(menu).hide();
            });
            $('iframe').contents().click(function () {
                $(menu).hide();
            });
        }
    });
})(jQuery);