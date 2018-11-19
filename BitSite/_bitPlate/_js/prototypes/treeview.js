(function ($) {
    $.fn.extend({
        treeview: function (options) {
            var treeView = this;
            var treeViewId = $(this).attr("id");
            $(treeView).children('ul').children('.groupNode').each(function () {
                buildTree(this, options);
            });
            $(treeView).find('.itemNode').each(function () {

                var html = $(this).html();
                html = "<span class='itemIcon'>&nbsp; </span>" + html;
                $(this).html(html);
            });
            //var html = $(treeView).html();

            function buildTree(groupNode, options) {
                var html = $(groupNode).html();
                var id = $(groupNode).attr('id');
                if ($(groupNode).children('ul')[0]) {
                    html = "<span class='groupIcon'>&#9660; </span>" + html;
                    $(groupNode).html(html);
                    $(groupNode).children('ul').children('.groupNode').each(function () {
                        buildTree(this);
                    });
                }
                else {
                    html = "<span class='groupIcon'>&nbsp; </span>" + html;
                    if (options && options.sortable) {
                        html += "<ul><ul>";
                    }
                    $(groupNode).html(html);
                }

            }

            $(treeView).find('.groupIcon, .openCloseIcon').click(function () {
                if ($(this).parent().find('ul').is(':visible')) {
                    $(this).parent().find('ul:first').slideUp(400);
                    $(this).parent().find('.groupIcon:first').html('&#9658;');
                }
                else {
                    $(this).parent().find('ul:first').slideDown(400);
                    $(this).parent().find('.groupIcon:first').html('&#9660;');
                }
            });

            $(treeView).find('a').click(function () {
                var liParent = $(this).parent('li');
                var id = $(liParent).attr('id');
                setSelected(id);
            });

            var firstSelect = true;
            if (options && options.setSelected != null) {
                firstSelect = false;
            }
            if (firstSelect) {
                selectFirst();
            }

            if (options && options.sortable) {
                $(treeView).children("ul").sortable({
                    connectWith: "#" + treeViewId + " ul",
                    placeholder: "highlight",
                    //forcePlaceholderSize: true,
                    revert: 200
                    //items: 'li',
                    //tolerance: 'pointer',
                    
                }).disableSelection();

            }

            if (options && options.sortstop) {
                $(treeView).find("ul").bind("sortstop", function (event, ui) {
                    var item = ui.item;
                    id = $(item).attr('id');
                    var parent = $(item).parent('ul').parent('li');
                    var parentid = $(parent).attr('id');
                    var test = $(parent).children('ul').html()
                    if ($(parent).children('ul').html() =='') {
                        $(parent).find('span.groupIcon').html('&#9660; ');
                    }
                    else {
                        $(parent).find('span.groupIcon').html('&nbsp;');
                    }

                    var cssclass = $(item).attr('class');
                    var index = $(parent).children('ul').children().index(item);
                    var type = "item";
                    if (cssclass == "groupNode") {
                        type = "group";
                    }

                    var data = { type: type, id: id, parentid: parentid, orderingnumber: index };

                    event.stopImmediatePropagation();
                    options.sortstop(data);
                });
            }

            function selectFirst() {
                var firstLi = $(treeView).find('li:first');
                var id = $(firstLi).attr('id');
                setSelected(id);
            }

            function setSelected(id) {
                $(treeView).find('a').removeClass('selected');
                $(treeView).find('li#' + id + " a:first").addClass('selected');
                if (options && options.selectedIndexChanged) {
                    var cssClass = $('li#' + id).attr('class');
                    if (cssClass == "groupNode") {
                        type = "group";
                    }
                    else {
                        type = "item";
                    }
                    options.selectedIndexChanged(id, type);
                }
            }

            if (options && options.setSelected != null) {
                setSelected(options.setSelected);
            }
        }
    });
})(jQuery);
