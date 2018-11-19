/// <reference path="BITEDITOR.js" />
/// <reference path="CommandManager.js" />
/// <reference path="Editor.js" />
/// <reference path="EditorIFrame.js" />
/// <reference path="Menu.js" />
/// <reference path="MenuItem.js" />
/// <reference path="Popup.js" />
/// <reference path="Selection.js" />
/// <reference path="Statusbar.js" />
/// <reference path="Toolbar.js" />
/// <reference path="ToolbarButton.js" />
/// <reference path="ToolbarDropDown.js" />
/// <reference path="ToolbarSeperator.js" />

function Selection() {
    var self = this;
    self.type = '';
    self.plainText = '';
    self.htmlText = '';
    self.elements = [];
    var selection;
    var range;
    //ten boehove van de textarea selection
    var startPos = 0;
    var endPos = 0;
    var length = 0;

    function Selection() {

    }

    self.get = function () {
        //BITEDITOR.currentEditor.focus();
        startPos = 0;
        endPos = 0;
        length = 0;
        if (!BITEDITOR.currentEditor) return;
        if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.WYSIWYG) {
            //if (BITEDITOR.currentEditor.editorType == BITEDITOR.EDITORTYPE.DIV) {
                setContentEditableDivSelection();
            //}
            //else {
            //    setIFrameSelection();
            //}
        }
        else if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.CODEMIRROR) {
            setCodeMirrorSelection();
        }
        else if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.TEXTAREA) {
            setTextareaSelection();
        }
    }

    function setContentEditableDivSelection() {
        if (document.selection && document.selection.createRange) {
            //INTERNET EXPLORER
            selection = document.selection.createRange();
            //if (document.selection.type == "Control") {
            //var sel = document.selection;
            //self.type = selection.type;
            self.type = document.selection.type;

            if (self.type == 'Control') {
                self.htmlText = '';
                for (var i = 0; i < selection.length; i++) {
                    var elm = selection.item(i);
                    self.htmlText += elm.outerHTML;
                }

            }
            else {
                self.htmlText = selection.htmlText;
                var parent = selection.parentElement();
            }
        }
        else {
            //OTHER BROWSERS
            selection = window.getSelection();
            range = selection.getRangeAt(0);
            if (selection.isCollapsed) {
                self.plainText = "";
                self.htmlText = "";
                self.type = "none";
            }
            else {

                var test = range.toString();
                //                var documentfragment = selection.cloneContents();

                //                var span = document.createElement('span');

                //                span.appendChild(documentfragment);
                //                range.insertNode(span);
                self.plainText = window.getSelection().toString();
                //even voor de time being, anders werkt wrap niet
                self.htmlText = self.plainText;
            }
            //self.type = selection.type;
        }
        return selection;
    }

    //function setIFrameSelection() {
    //    if (document.selection && document.selection.createRange) {
    //        //INTERNET EXPLORER

    //        var iFrame = BITEDITOR.currentEditor.get().get(0);
    //        var idoc = (iFrame.contentWindow || iFrame.contentDocument);
    //        if (idoc.document) idoc = idoc.document;
    //        selection = idoc.selection.createRange();
    //        //if (document.selection.type == "Control") {
    //        //var sel = document.selection;
    //        //self.type = selection.type;
    //        self.type = idoc.selection.type;


    //        if (self.type == 'Control') {
    //            self.htmlText = '';
    //            for (var i = 0; i < selection.length; i++) {
    //                var elm = selection.item(i);
    //                self.htmlText += elm.outerHTML;
    //            }

    //        }
    //        else {
    //            self.htmlText = selection.htmlText;
    //            var parent = selection.parentElement();
    //        }
    //    }
    //    else {
    //        //OTHER BROWSERS
    //        selection = window.getSelection();
    //        range = selection.getRangeAt(0);
    //        if (selection.isCollapsed) {
    //            self.plainText = "";
    //            self.htmlText = "";
    //            self.type = "none";
    //        }
    //        else {

    //            var test = range.toString();
    //            //                var documentfragment = selection.cloneContents();

    //            //                var span = document.createElement('span');

    //            //                span.appendChild(documentfragment);
    //            //                range.insertNode(span);
    //            self.plainText = window.getSelection().toString();
    //            //even voor de time being, anders werkt wrap niet
    //            self.htmlText = self.plainText;
    //        }
    //        //self.type = selection.type;
    //    }
    //    return selection;
    //}

    function setCodeMirrorSelection() {
        var codemirror = BITEDITOR.currentEditor.getCodeMirror();
        self.htmlText = codemirror.getSelection();
    }

    function setTextareaSelection() {
        var textArea = BITEDITOR.currentEditor.get();

        //Mozilla and DOM 3.0
        if ('selectionStart' in textArea) {
            startPos = textArea.selectionStart;
            endPos = textArea.selectionEnd;
            length = endPos - startPos;
            self.htmlText = textArea.value.substr(startPos, length);

            //return { start: textArea.selectionStart, end: textArea.selectionEnd, length: l, text: e.value.substr(e.selectionStart, l) };
        }
            //IE
        else if (document.selection) {
            textArea.focus();
            var range = document.selection.createRange();

            var textrange = textArea.createTextRange();
            if (range == null || textrange == null) {
                self.htmlText = '';
                return;
            }
            var textrange2 = textrange.duplicate();
            textrange2.moveToBookmark(range.getBookmark());
            textrange.setEndPoint('EndToStart', textrange2);


            var text_part = range.text.replace(/[\r\n]/g, '.'); //for some reason IE doesn't always count the \n and \r in the length
            var text_whole = textarea.value.replace(/[\r\n]/g, '.');
            startPos = text_whole.indexOf(text_part, textrange.text.length);
            endPos = startPos + text_part.length;
            length = text_part.length;
            self.htmlText = range.text;
        }
                //Browser not supported
        else {

        };

    }
    self.replaceHTML = function (html) {
        if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.WYSIWYG) {
            if (document.selection && document.selection.createRange) {
                //INTERNET EXPLORER
                if (self.type == "Control") {
                    for (var i = 0; i < selection.length; i++) {
                        var elm = selection.item(i);
                        elm.outerHTML = html;
                    }
                }
                else {
                    selection.pasteHTML(html);
                }
            }
            else {
                //var range = selection.getRangeAt(0);
                var node = range.createContextualFragment(html);
                range.deleteContents();
                range.insertNode(node);
            }
        }
        else if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.TEXTAREA) {
            //TEXTAREA MODE
            var textArea = BITEDITOR.currentEditor.get();

            textArea.value = textArea.value.substr(0, startPos) + html + textArea.value.substr(endPos, textArea.value.length);
            //set_selection(the_id, start_pos, end_pos);
        }
        else if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.CODEMIRROR) {
            //CODEMIRROR MODE
            var codemirror = BITEDITOR.currentEditor.getCodeMirror();
            codemirror.replaceSelection(html);
        }
    }

    self.wrap = function (elementToWrapWith) {

        var elm = elementToWrapWith;
        elm.innerHTML = self.htmlText;
        var html = elm.outerHTML;
        if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.WYSIWYG) {
            if (document.selection && document.selection.createRange) {
                //INTERNET EXPLORER
                //var html = startHtml + self.htmlText + endHtml;
                if (selection.pasteHTML) {
                    selection.pasteHTML(html);
                }
                    //                if (self.type == "Text") {
                    //                    selection.pasteHTML(html);
                    //                }
                else {
                    for (var i = 0; i < selection.length; i++) {
                        var elm = selection.item(i);
                        elm.outerHTML = html;
                    }
                }
            }
            else {
                //NON-IE BROWSERS
                range.surroundContents(elementToWrapWith);

            }
        }
        else if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.TEXTAREA) {
            //TEXTAREA MODE
            self.replaceHTML(html);
            var tagname = elm.nodeName.toLowerCase();
            var closingTag = "</" + tagname + ">";
            var openingTag = html.replace(elm.innerHTML, "").replace(closingTag, '');
            setSelection(startPos + openingTag.length, endPos + openingTag.length);
        }
        else if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.CODEMIRROR) {
            var codemirror = BITEDITOR.currentEditor.getCodeMirror();
            codemirror.replaceSelection(html);
        }
    }

    function setSelection(start_pos, end_pos) {
        //VOOR TEXTAREAS
        var textArea = BITEDITOR.currentEditor.get();

        //Mozilla and DOM 3.0
        if ('selectionStart' in textArea) {
            textArea.focus();
            textArea.selectionStart = start_pos;
            textArea.selectionEnd = end_pos;
        }
            //IE
        else if (document.selection) {
            textArea.focus();
            var tr = textArea.createTextRange();

            //Fix IE from counting the newline characters as two seperate characters
            var stop_it = start_pos;
            for (i = 0; i < stop_it; i++) if (e.value[i].search(/[\r\n]/) != -1) start_pos = start_pos - .5;
            stop_it = end_pos;
            for (i = 0; i < stop_it; i++) if (e.value[i].search(/[\r\n]/) != -1) end_pos = end_pos - .5;

            tr.moveEnd('textedit', -1);
            tr.moveStart('character', start_pos);
            tr.moveEnd('character', end_pos - start_pos);
            tr.select();
        }
        self.get();
    }

    self.setCssClass = function (className) {
        //alert(className);
        if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.WYSIWYG) {
            if (document.selection && document.selection.createRange) {
                //INTERNET EXPLORER

                if (selection.htmlText) {
                    //text selection
                    var html = "<span class='" + className + "'>" + selection.htmlText + "</span>";
                    if (selection.pasteHTML) {
                        selection.pasteHTML(html);
                    }
                }
                else {
                    for (var i = 0; i < selection.length; i++) {
                        var elm = selection.item(i);
                        elm.setAttribute("class", className);
                    }
                }
            }
            else {
                var range = selection.getRangeAt(0);
                var html = "<span class='" + className + "'>" + window.getSelection().toString() + "</span>";
                var node = range.createContextualFragment(html);
                range.deleteContents();
                range.insertNode(node);
            }
        }
    }
    Selection();
}