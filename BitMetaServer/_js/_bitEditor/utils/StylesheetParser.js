var StylesheetParser = {
    getDocumentRulesHtml: function () {
        var div = document.createElement("div");
        for (var i in document.styleSheets) {
            var styleSheet = document.styleSheets[i];
            if (styleSheet.id) {
                this.addRulesToDiv(styleSheet, div);
            }
        }
        return div.innerHTML;
    },

    addRulesToDiv: function (stylesheet, divAllContent) {
        var rules = stylesheet.cssRules ? stylesheet.cssRules : stylesheet.rules
        if (rules) {
            var h2 = document.createElement("h2");
            h2.innerHTML = stylesheet.id ? stylesheet.id : stylesheet.href;
            divAllContent.appendChild(h2);
        }
        for (var i in rules) {
            var rule = rules[i];
            var name = rule.selectorText;
            var classname = '';
            var isPseudoClass = false;
            if (name && name.indexOf('.') == 0) {
                classname = name.replace('.', '');
            }
            if (classname.indexOf(":") > 0) {
                isPseudoClass = true;
            }
            if (classname != '' && !isPseudoClass) {
                var div = document.createElement("div");
                div.setAttribute("class", classname);
                div.innerHTML = name;
                div.style.position = "relative";
                div.style.width = "90%";
                div.style.height = "50px";
                div.style.top = "0px";
                div.style.left = "0px";
                div.style.border = "solid 1px black";
                div.style.margin = "4px 4px 4px 4px";
                div.style.padding = "4px 4px 4px 4px";
                div.style.float = "none";
                div.style.display = "block";
                div.title = rule.cssText;
                divAllContent.appendChild(div);
            }
        }
    }
};