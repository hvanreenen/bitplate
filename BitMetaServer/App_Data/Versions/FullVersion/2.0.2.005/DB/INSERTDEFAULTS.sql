-- insert scripts voor menu
INSERT INTO Script (ID, FK_Site, Name, scripttype, active, issystemvalue, createdate, content )
SELECT UUID(), ID, 'siteMenu', 1, 1, 1, NOW(), '$(document).ready(function () {
    loadMenus();
});

var MENUDIRECTION = { HORIZONTAL: "HORIZONTAL", VERTICAL: "VERTICAL" };

function loadMenus() {
    $(".bitMenu").each(function () {
        var type = $(this).attr("data-menu-type");
        var animationSpeed = $(this).attr("data-menu-animationSpeed");
        if (type == "dropdownHorizontal") {
            $(this).find("ul").bitMenu({ direction: MENUDIRECTION.HORIZONTAL, animationSpeed: animationSpeed });
        }
        else if (type == "dropdownVertical") {
            $(this).find("ul").bitMenu({ direction: MENUDIRECTION.VERTICAL, animationSpeed: animationSpeed });
        }
    });
    $(".bitAccordionMenu").each(function () {
        var animationSpeed = $(this).attr("data-menu-animationSpeed");

        $(this).find("ul").bitAccordion({ animationSpeed: animationSpeed });
    });
}

if (parent && window.location != window.parent.location) {
    //vanuit iframe geladen
    parent.BITEDITPAGE.registerModuleReloadFunctions(loadMenus, "MENUS");
}

(function ($) {
    $.fn.extend({
        bitMenu: function (options) {
            if (!options) {
                options = { direction: MENUDIRECTION.HORIZONTAL, animationSpeed: 200 };
            }
            if (!options.direction) {
                options.direction = MENUDIRECTION.HORIZONTAL;
            }
            if (!options.animationSpeed) {
                options.animationSpeed = 200;
            }
            buildRecursiveMenu($(this), options);
            setSelectedMenuItem($(this));
        },

        bitAccordion: function (options) {
            var rootElm = $(this);
            if (!options) {
                options = { animationSpeed: 500 };
            }

            rootElm.find("li ul").hide();
            rootElm.find("li").click(function () {
                rootElm.find("li ul").slideUp(options.animationSpeed);
                $(this).find("ul").slideDown(options.animationSpeed);
            });
            rootElm.find("li ul:first").show();
            setSelectedAccordionItem(rootElm);
        }
    });
})(jQuery);


function setSelectedMenuItem(root) {
    var currentPage = location.pathname;
    $(root).find("a[href=\'" + currentPage + "\']").parent("li").addClass("selectedMenuItem");
}

function setSelectedAccordionItem(root) {
    var currentPage = location.pathname;
    $(root).find("a[href=\'" + currentPage + "\']").parent("li").addClass("selectedMenuItem");
    var elm = $(root).find("a[href=\'" + currentPage + "\']");
    if (elm.html()) { //kijken of die bestaat

        //eerst alle dicht
        $(root).find("li ul").hide();
        //dan parent van geselecteerde openen
        elm.parent().show();
        elm.parent().parent().show();
    }
}

function buildRecursiveMenu(elm, options, dept) {
    if (!dept) dept = 0;
    if (options.direction == MENUDIRECTION.HORIZONTAL && dept == 0) {
        $(elm).find("li").css("display", "table-cell");
        $(elm).find("li ul li").css("display", "list-item");
        $(elm).find("li ul li").css("width", $(elm).find("li").width());
    }
    $(elm).find("li ul").hide();
    $(elm).find("li").hover(
        function () {
            $(this).find("ul:first").slideDown(options.animationSpeed);
            dept = $(this).parents("ul").length;

            if (options.direction == MENUDIRECTION.VERTICAL || dept > 1) {
                //er rechts naast van plaatsen
                $(this).find("ul").css("position", "absolute");
                $(this).find("ul").css("left", $(this).width());
                $(this).find("ul").css("top", $(this).position().top);
            }
            else if (options.direction == MENUDIRECTION.HORIZONTAL && dept == 1) {
                //er onder plaatsen
                $(this).find("ul").css("position", "absolute");
                $(this).find("ul").css("left", $(this).position().left);
                $(this).find("ul").css("top", $(this).height);
            }

            buildRecursiveMenu($(this), options, dept);
        },
        function () {
            $(this).find("ul").hide();
        }
    );
}' FROM Site;


INSERT INTO Script (ID, FK_Site, Name, scripttype, active, issystemvalue, createdate, content )
SELECT UUID(), ID, 'siteMenu', 0, 1, 1, NOW(), '/* DROPDOWN MENU */
.bitMenu{
    width: 100%;
}

.bitMenu ul {
   margin: 0;
   padding: 0px; 
}

    .bitMenu ul li {
		background-color: #322f32;
        margin: 0;
        list-style-type: none;
        color: #fff;
        padding: 0px;
        width: 250px;
    }

        .bitMenu ul li a, .bitMenu ul li span {
			padding: 10px;
			text-transform: uppercase;
			display: inline-block;
        }


            .bitMenu ul ul li {
                background-color: #ddd;
                width: 250px;
            }



            .bitMenu ul li:hover {
                background-color: #888;
            }

        /* SELECTED ITEM */
        .selectedMenuItem {
            background-color: #889 !important;
            color: red !important;
        }

        /* ACCORDION */
        .bitAccordionMenu {
            width:250px;
            border: ridge 2px #ddd;
            padding: 0px;
        }

        .bitAccordionMenu ul {
           margin: 0;
           padding: 0px;
        }

        .bitAccordionMenu ul li {
            background-color: #322f32;
            margin: 0;
            list-style-type: none;
            color: #fff;
            padding: 0px;
            width: 100%;
            border: solid 1px gray;
        }

            .bitAccordionMenu ul li:hover {
                background-color: #888;
            }

            .bitAccordionMenu ul li a, .bitAccordionMenu ul li span {
                padding: 10px;
                text-transform: uppercase;
                display: inline-block;
            }


            .bitAccordionMenu ul li ul {
                padding-left: 0px;
            }

                .bitAccordionMenu ul li ul li {
                    background-color: white;
                    color: black;
                    border: none;
                }' FROM Site;