var HTMLHELPER = {
    getBodyContent: function (html) {
        var posBodyStartTagStart = html.indexOf("<body");
        var posBodyStartTagEnd = html.indexOf(">", posBodyStartTagStart + 1) + 1;
        var posBodyEndTag = html.indexOf("</body>");
        var bodyHtml = html.substring(posBodyStartTagEnd, posBodyEndTag);
        return bodyHtml;
    },

    splitHeadFromBody: function (html) {
        //find head
        var posHeadStartTag = html.indexOf("<head>") + 6;
        var posHeadEndTag = html.indexOf("</head>");
        var head = html.substring(posHeadStartTag, posHeadEndTag);
        // eerste line-break weghalen
        head = head.replace('\n', '');
        head = head.replace('\r', '');
        //find body
        var posBodyStartTagStart = html.indexOf("<body");
        var posBodyStartTagEnd = html.indexOf(">", posBodyStartTagStart + 1) + 1;
        var posBodyEndTag = html.indexOf("</body>");
        var bodyTag = html.substring(posBodyStartTagStart, posBodyStartTagEnd);
        var bodyHtml = html.substring(posBodyStartTagEnd, posBodyEndTag);
        // eerste line-break weghalen
        bodyHtml = bodyHtml.replace('\n', '');
        bodyHtml = bodyHtml.replace('\r', '');
        
        return { head: head, bodyTag: bodyTag, bodyHtml: bodyHtml }
    },


    replaceBodyContent: function (newBodyContent, oldCompleteContent) {

        var oldBodyContent = this.getBodyContent(oldCompleteContent);
        var completeContent = oldCompleteContent.replace(oldBodyContent, newBodyContent);

        return completeContent;
    }
       
}