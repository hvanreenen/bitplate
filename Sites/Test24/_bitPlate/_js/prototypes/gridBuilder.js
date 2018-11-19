String.prototype.startsWith = function(needle)
{
    return(this.indexOf(needle) == 0);
};

(function ($) {

    $.fn.extend({

        createGrid: function (options) {
            var gridHtml = '<table><thead><tr></tr></thead><tbody><tr></tr></tbody></table>';
            $(this).html(gridHtml);
            var table = $(this).find('table');
            for (var tableAttr in options.table) {
                $(table).attr(tableAttr, options.table[tableAttr]);
            }
            for (var tableHeaderRowAttr in options.headerRow) {
                $(table).find('thead').find('tr').attr(tableHeaderRowAttr, options.headerRow[tableHeaderRowAttr]);
            }
            for (var tableBodyRowAttr in options.bodyRow) {
                $(table).find('tbody').find('tr').attr(tableBodyRowAttr, options.bodyRow[tableBodyRowAttr]);
            }
           
            for (var col in options.columns) {
                var tempGridHeader = '<td></td>';
                var tempGridBody = '<td></td>';
                tempGridHeader = $(tempGridHeader).appendTo($(table).find('thead').find('tr'));
                tempGridBody = $(tempGridBody).appendTo($(table).find('tbody').find('tr'));
                $(table).find('tbody').find('tr').append(tempGridBody);
                for (var tableHeaderColumnAttr in options.columns[col].headerAttr) {
                    $(tempGridHeader).attr(tableHeaderColumnAttr, options.columns[col].headerAttr[tableHeaderColumnAttr]);
                }

                for (var tableBodyColumnAttr in options.columns[col].bodyAttr) {
                    $(tempGridBody).attr(tableBodyColumnAttr, options.columns[col].bodyAttr[tableBodyColumnAttr]);
                }
                $(tempGridHeader).html(options.columns[col].caption);
                $(tempGridBody).html(options.columns[col].html);
            }
        },

        createListGrid: function (options) {
            var gridHtml = '<div class="listGrid"></div>';
            $(this).html(gridHtml);
            var listGrid = $(this).find('.listGrid');
            var listGridHeader = $(listGrid).append('<div class="listGridHeader"></div>').find('div');
            var listGridBody = $(listGrid).append('<ul class="listGridBody"><li><div></div></li></ul>').find('ul');
            var listGridBodyRow = $(listGridBody).find('li');
            var listGridBodyRowDiv = $(listGridBodyRow).find('div');

            for (var tableAttr in options.table) {
                var orginalValue = $(listGrid).attr(tableAttr);
                orginalValue = (orginalValue == undefined) ? '' : ' ' + orginalValue;
                $(listGrid).attr(tableAttr, options.table[tableAttr] + orginalValue);
            }

            for (var tableHeaderRowAttr in options.headerRow) {
                var orginalValue = $(listGridHeader).attr(tableHeaderRowAttr);
                orginalValue = (orginalValue == undefined) ? '' : ' ' + orginalValue;
                $(listGridHeader).attr(tableHeaderRowAttr, options.headerRow[tableHeaderRowAttr] + orginalValue);
            }
            for (var tableBodyRowAttr in options.bodyRow) {
                var orginalValue = $(listGridBody).attr(tableBodyRowAttr);
                orginalValue = (orginalValue == undefined) ? '' : ' ' + orginalValue;
                $(listGridBody).attr(tableBodyRowAttr, options.bodyRow[tableBodyRowAttr] + orginalValue);
            }

            for (var col in options.columns) {
                var listGridHeaderColumn = '<div class="listCell"></div>'
                var listGridBodyColumn = '<div class="listCell"></div>';
                listGridHeaderColumn = $(listGridHeaderColumn).appendTo(listGridHeader);
                listGridBodyColumn = $(listGridBodyColumn).appendTo(listGridBodyRowDiv);


                for (var tableHeaderColumnAttr in options.columns[col].headerAttr) {
                    $(listGridHeaderColumn).attr(tableHeaderColumnAttr, options.columns[col].headerAttr[tableHeaderColumnAttr]);
                }

                for (var tableBodyColumnAttr in options.columns[col].bodyAttr) {
                    $(listGridBodyColumn).attr(tableBodyColumnAttr, options.columns[col].bodyAttr[tableBodyColumnAttr]);
                }

                $(listGridHeaderColumn).append(options.columns[col].caption);
                $(listGridBodyColumn).append(options.columns[col].html);

                //alert($(listGridHeaderColumn).width());
                $(listGridBodyColumn).css('width', $(listGridHeaderColumn).width());
            }
        },

        getAttributes: function () {
            var attributes = {};

            if (this.length) {
                $.each(this[0].attributes, function (index, attr) {
                    attributes[attr.name] = attr.value;
                });
            }

            return attributes;
        }
    });
})(jQuery);

var GRIDTOOLS = {
    createFromHtmlDefinition: function (html) {
        var definition = new Object();
        var htmlObject = $('<div></div>').append(html);
        if (html.startsWith('<table')) {
            definition.table = $(htmlObject).find('table').getAttributes();
            definition.headerRow = $(htmlObject).find('table thead tr').getAttributes();
            definition.bodyRow = $(htmlObject).find('table tbody tr').getAttributes();
            definition.columns = new Object();
            $($(htmlObject).find('table thead td')).each(function (i) {
                var column = new Object();
                column.headerAttr = $(this).getAttributes();
                var bodyColumn = $($(htmlObject).find('table tbody tr')[0]).find('td')[i];
                column.bodyAttr = $(bodyColumn).getAttributes();
                column.caption = $(this).html();
                column.html = $(bodyColumn).html();
                definition.columns[i] = (column);
            });
        }
        else {
        }
        return definition;
    }

}