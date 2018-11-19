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
/// <reference path="utils/HTMLHELPER.js" />


function Editor(divWrapper) {
    var self = this;
    if (divWrapper.jquery) {
        divWrapper = document.getElementById(divWrapper.attr('id'));
    }
    
    this.id = divWrapper.getAttribute('id');
    this.name = divWrapper.getAttribute('title');
    this.mode = BITEDITOR.EDITORMODE.WYSIWYG;
    this.toolbar = null;
    this.statusbar = null;
    var fullscreenMode = false;
    //var readonlyMode = false;

    var html = divWrapper.innerHTML;
    var div = document.createElement("div");
    var textareaCode = document.createElement("textarea");
    var codeMirror; 
    var size = { width: 0, height: 0 };
    this.includeHeadTag = false;
    var completeHtml = ""; //html waarbij ook de html-tag head-tag worden meegenomen 

    function Editor() {
        divWrapper.innerHTML = "";
        self.setHtml(html);
        div.setAttribute("ContentEditable", true);
        div.setAttribute("id", self.name);
        div.setAttribute("class", "bitEditor");
        textareaCode.setAttribute("class", "codeview");
        //code view verbergen tijdens init
        textareaCode.style.display = "none";

        //attach events
        div.onclick = function () {
            selectEditor();
            self.whichElement();
        }
        div.ondblclick = function () {
            BITEDITOR.storeSelection();
        }
        div.onactivate = function () {
            selectEditor();
        }
        div.onmouseup = function () {
            BITEDITOR.storeSelection();
        }
        div.onkeyup = function () {
            BITEDITOR.storeSelection();
            self.whichElement();
        }
        //textareaCode heeft geen event nodig, want wordt omgezet in codemirror
        
        divWrapper.appendChild(textareaCode);
        divWrapper.appendChild(div);
        
    }

    this.focus = function () {
        selectEditor();
        BITEDITOR.storeSelection();
        this.whichElement();
        div.focus();
    }

    function selectEditor() {
        if (self.mode == BITEDITOR.EDITORMODE.WYSIWYG) {
            BITEDITOR.currentEditor = self;
            if (self.toolbar != null) {
                //eerst andere toolbars disablen
                for (var i in BITEDITOR.editors) {
                    if(BITEDITOR.editors[i]) BITEDITOR.editors[i].toolbar.disable();
                }
                self.toolbar.attach(self);
            }
            else {
                BITEDITOR.toolbar.attach(self);
            }
            //if (BITEDITPAGE) BITEDITPAGE.refreshTagsPopup(self.id);
        }
        
        //BITEDITOR.statusbar.setText("Editor: " + self.name);
    }

    function deactivateEditor() {
        if (self.toolbar != null) {
            self.toolbar.disable();
        }
    }

    this.whichElement = function () {
        
        if (self.mode != BITEDITOR.EDITORMODE.WYSIWYG) return;

        if (document.selection && document.selection.createRange) {
            //INTERNET EXPLORER
            selection = document.selection.createRange();
            var test = selection.htmlText;
            var test2 = document.selection.type;
            if (selection.parentElement) {
                tag = selection.parentElement();
            }
            else if (selection.item) {
                tag = selection.item(0);
            }
            //            if (document.selection.type == 'Tekst') {
            //                tag = selection.parentElement();
            //            }
            //            else {
            //                //controls
            //                tag = selection.item(0);
            //                
            //            }
        }
        else {
            var selection = window.getSelection();
            var range = selection.getRangeAt(0);
            tag = range.startContainer;

        }

        if (tag.nodeType == 3) // defeat Safari bug
        {
            tag = tag.parentNode;
        }
        var tagnames = [];
        while (tag != null && tag.id != self.id) {
            tagnames.push(tag.tagName);
            //tagnames += tag.tagName + ", ";
            tag = tag.parentNode;
        }
        if (self.toolbar != null) {
            self.toolbar.toggleButtons(tagnames);
        }
        else if (BITEDITOR.toolbar != null) {
            BITEDITOR.toolbar.toggleButtons(tagnames);
        }
        tagnames = tagnames.reverse();

        var tagnamesString = tagnames.join('&gt; &lt;');
        tagnamesString = '&lt;' + tagnamesString + '&gt;';
        if (self.statusbar != null) {
            self.statusbar.setText("Editor: " + self.name + " | elementen: " + tagnamesString);
        }
        else if (BITEDITOR.statusbar != null) {
            BITEDITOR.statusbar.setText("Editor: " + self.name + " | elementen: " + tagnamesString);
        }
        
    }

    this.getHtml = function () {
        if (self.mode == BITEDITOR.EDITORMODE.CODEMIRROR) {
            codeMirror.toTextArea();
            return $(textareaCode).val();
        }
        else {
            return div.innerHTML;
        }
    }

    this.setHtml = function (value) {
        div.innerHTML = value;
    }

    this.getCompleteHtml = function () {
        var html = '' ;
        if (self.mode != 'WYSIWYG') {
            html = $(textareaCode).val();
        }
        else {
            html = self.getHtml();
            if (self.includeHeadTag) {
                //bij templates ook de html-head laten zien in code view
                html = HTMLHELPER.replaceBodyContent(html, completeHtml);
            }
        }
        return html;
        /* $(textareaCode).val(html);

        return (this.viewCode) ? $(textareaCode).val() :  */
        //return completeHtml;
    }

    this.setCompleteHtml = function (value) {
        completeHtml = value;
    }

    function getWidth() {
        return $(div).width();
    }
    function setWidth(value) {
        $(div).width(value);
    }
    function getHeight() {
        return $(div).height();
    }
    function setHeight(value) {
        return $(div).height(value);
    }

    function storeSize() {
        size.width = $(div).width();
        size.height = $(div).height();
    }
    this.viewCode = function (codeview) {
        if (codeview) {

            if (!fullscreenMode) {
                storeSize();
                $(textareaCode).width(size.width);
                $(textareaCode).height(size.height);
            }
            var html = self.getHtml();
            if (self.includeHeadTag) {
                //bij templates ook de html-head laten zien in code view
                html = HTMLHELPER.replaceBodyContent(html, completeHtml);
            }
            $(textareaCode).val(html);
            
            self.mode = BITEDITOR.EDITORMODE.CODEMIRROR;
            self.toolbar.setMode(BITEDITOR.EDITORMODE.CODEMIRROR);
            $(div).hide();

            //maak codemirror aan van textarea
            codeMirror = CodeMirror.fromTextArea(textareaCode, {
                mode: "text/html",
                tabMode: "indent",
                lineNumbers: true,
                matchBrackets: true,
                smartIndent: true,
                onCursorActivity: function () {
                    BITEDITOR.storeSelection();
                }
            });
            CodeMirror.commands["selectAll"](codeMirror);
            codeMirror.autoFormatRange(codeMirror.getCursor(true), codeMirror.getCursor(false));
            

            
            //if (self.id.indexOf("Template") >= 0) {
            //    $(".CodeMirror").css("margin-left", "60px");
            //    $(".CodeMirror").css("top", "5px");
            //    $(".CodeMirror").css("width", screen.width - 90);
            //    $(".CodeMirror").css("height", screen.height - 110);
            //    $(".CodeMirror").css("background-color", "#FFF");
            //    $(".CodeMirror").css("overflow", "scroll");
            //    //$(".CodeMirror").css("margin-top", "70px");
            //}
        }
        else {
            codeMirror.toTextArea();
            var html = $(textareaCode).val();
            $(textareaCode).hide();
            if (self.includeHeadTag) {
                //bij template de html-head weer uit de code halen
                //eerst complete html bewaren in var
                completeHtml = html;
                //hierna head from body splitsen
                html = HTMLHELPER.getBodyContent(html);
                //self.setHtml(bodyContent);
            }
            
            self.setHtml(html);
           
            //BITEDITOR.toolbar.enable();
            self.mode = BITEDITOR.EDITORMODE.WYSIWYG;

            $(div).show();
            $(div).focus();
        }
    }

    this.getCodeMirror = function () {
        return codeMirror;
    }

    

    this.get = function () {
        if (this.mode == BITEDITOR.EDITORMODE.CODEMIRROR) {
            return codeMirror;
        }
        else {
            return div;
        }
    }

    this.remove = function () {
        divWrapper.innerHTML = "";
    }
    Editor();
}


//OUDE FUNCTIES NIET IN GEBRUIK
//this.fullscreen = function (fullscreen) {
//    fullscreenMode = fullscreen;
//    if (fullscreen) {
//        storeSize();


//        $('#editorFullScreen').html("");
//        $('#editorFullScreen').css("background-color", "#fff");
//        $('#editorFullScreen').width(window.innerWidth);
//        $('#editorFullScreen').height(window.innerHeight);

//        $('#editorFullScreen').append($(divWrapper).html());
//        $('#editorFullScreen').show();
//        textareaCode.setAttribute("class", "fullscreenMode");
//        $(textareaCode).width(window.innerWidth);
//        $(textareaCode).height(window.innerHeight);

//        var toolbarDiv;
//        if (self.toolbar != null) {
//            toolbarDiv = self.toolbar.get();
//        }
//        else {
//            toolbarDiv = BITEDITOR.toolbar.get();
//        }
//        $(toolbarDiv).css("position", "fixed");
//        $(toolbarDiv).css("display", "block");
//        $(toolbarDiv).css("z-index", "99999");
//        $(toolbarDiv).css("top", "39px");
//        $(toolbarDiv).css("left", "0px");
//        $(toolbarDiv).css("background-color", "#eee");
//        $(toolbarDiv).width(window.innerWidth);
//    }
//    else {
//        textareaCode.setAttribute("class", "codeview");
//        $(textareaCode).width(size.width);
//        $(textareaCode).height(size.height);

//        $(div).width(size.width);
//        $(div).height(size.height);

//        $('#editorFullScreen').hide();
//        $(divWrapper).html("");
//        $(divWrapper).append($('#editorFullScreen').html());

//        var toolbarDiv;
//        if (self.toolbar != null) {
//            toolbarDiv = self.toolbar.get();
//        }
//        else {
//            toolbarDiv = BITEDITOR.toolbar.get();
//        }
//        $(toolbarDiv).css("position", "");
//        $(toolbarDiv).css("display", "");
//        $(toolbarDiv).css("z-index", "9");
//        $(toolbarDiv).css("top", "");
//        $(toolbarDiv).css("left", "");
//        $(toolbarDiv).width(size.width);
//    }
//}
//this.readOnly = function (readonly) {
//    if (readonly) {
//        if (fullscreenMode) {
//            self.fullscreen(false);
//        }
//        if (self.mode == BITEDITOR.EDITORMODE.CODEMIRROR) {
//            self.viewCode(false);
//        }
//        div.removeAttribute('contenteditable');
//        div.setAttribute('class', 'bitEditor');
//        BITEDITOR.toolbar.readOnlyMode();
//        self.mode = BITEDITOR.EDITORMODE.READONLY;
//    }
//    else {

//        div.setAttribute('contenteditable', true);
//        div.setAttribute('class', 'bitEditor editMode');
//        BITEDITOR.toolbar.enable();
//        self.mode = BITEDITOR.EDITORMODE.WYSIWYG;
//    }

//}

