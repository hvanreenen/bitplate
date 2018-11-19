var BITHYPERLINKSPOPUP = {
    target: "EditPage",

    initialize: function(){
        $('#bitHyperlinksDialog').initDialog(null, { width: 300, height: 600 }, false);
        $('#bitHyperlinksDialog').load('bitPopups/Hyperlinks.aspx', this.onHyperlinksLoaded);
    },

    show: function(){
        $('#bitHyperlinksDialog').dialog("open");
    },

    getHyperlinksAndSubFolders: function (folderid) {
        var parametersObject = { folderid: folderid };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "bitAjaxServices/PageService.asmx";
        BITAJAX.callWebService("GetHyperlinksAndSubFolders", jsonstring, this.onHyperlinksLoaded);
    },

    onHyperlinksLoaded: function (data) {
        $('#bitHyperlinksDialog').html(data.d);
        if (BITHYPERLINKSPOPUP.target == "EditPage") {
            $('.linkToInsert').dblclick(function () {
                var href = $(this).attr('title');
                if (BITEDITOR.selection.htmlText != '') {
                    var link = document.createElement("a");
                    link.setAttribute("href", "/" + href);
                    BITEDITOR.selection.wrap(link);
                }
                else {
                    var linkText = $(this).html();
                    //linkinfo eruit halen
                    linkText = linkText.replace("<a href='#'>", "");
                    linkText = linkText.replace("<a href=\"#\">", "");
                    linkText = linkText.replace("</a>", "");
                    var openingTag = '<a href="' + href + '">';
                    var closeTag = '</a>';

                    var html = openingTag + linkText + closeTag;
                    BITEDITOR.selection.replaceHTML(html);
                }
            });
        }
    }
};