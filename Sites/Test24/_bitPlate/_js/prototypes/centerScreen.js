
//functie om element (popup) te centreren in het scherm
//aanroep: $(div).center();
(function ($) {
    $.fn.extend({
        center: function () {
            this.css("position", "absolute");
            this.css("top", (($(window).height() - this.outerHeight()) / 2) +
                                                $(window).scrollTop() + "px");
            this.css("left", (($(window).width() - this.outerWidth()) / 2) +
                                                $(window).scrollLeft() + "px");
            return this;
        }
    });
})(jQuery);