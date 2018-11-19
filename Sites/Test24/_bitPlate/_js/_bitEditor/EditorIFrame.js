/// <reference path="BITEDITOR.js" />

//WORDT NIET GEBRUIKT
function EditorIFrame(iFrame) {
    var self = this;
    if (!iFrame.jquery) {
        iFrame = $(iFrame);
    }
    this.editorType = BITEDITOR.EDITORTYPE.IFRAME;
    this.id = iFrame.attr('id');
    this.name = iFrame.attr('title');
    this.mode = BITEDITOR.EDITORMODE.WYSIWYG;
    var fullscreenMode = false;
    //var readonlyMode = false;

    var html = iFrame.contents().html();
    
    var textareaCode = document.createElement("textarea");
    textareaCode = $(textareaCode);
    var size = { width: 0, height: 0 };


    function EditorIFrame() {
        
        iFrame.contents().get(0).designMode = 'on';
        
        iFrame.attr("class", "bitEditor");
        textareaCode.attr("class", "codeview");
        //code view verbergen tijdens init
        textareaCode.hide();
        $('#divBitEditPageBody').append(textareaCode);
        //attach events
        textareaCode.click(function () {
            selectEditor();
        });

        textareaCode.focus(function () {
            selectEditor();
        });

        iFrame.click(function () {
            selectEditor();
            self.whichElement();
        });
        
        iFrame.focus(function () {
            selectEditor();
        });
        
        iFrame.contents().find('body').mouseup(function () {
            BITEDITOR.storeSelection();
        });

        iFrame.contents().find('body').keyup(function () {
            BITEDITOR.storeSelection();
            self.whichElement();
        });

        textareaCode.mouseup(function () {
            BITEDITOR.storeSelection();
        });

        textareaCode.keyup(function () {
            BITEDITOR.storeSelection();
        });
        //divWrapper.appendChild(textareaCode);
        //divWrapper.appendChild(div);
    }



    this.focus = function () {
        selectEditor();
        iFrame.focus();
    }

    function selectEditor() {
        if (self.mode == BITEDITOR.EDITORMODE.WYSIWYG) {
            BITEDITOR.currentEditor = self;
            BITEDITOR.toolbar.attach(self);
            BITEDITOR.refreshTagsPopup(self.id);
        }
        
        //BITEDITOR.statusbar.setText("Editor: " + self.name);
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

        BITEDITOR.toolbar.toggleButtons(tagnames);
        tagnames = tagnames.reverse();

        var tagnamesString = tagnames.join('&gt; &lt;');
        tagnamesString = '&lt;' + tagnamesString + '&gt;';
        BITEDITOR.statusbar.setText("Editor: " + self.name + " | elementen: " + tagnamesString);
    }

    this.getHtml = function () {
        if (self.mode == BITEDITOR.EDITORMODE.VIEWCODE) {
            return textareaCode.val();
        }
        else {
            return iFrame.contents().find('html').html();
        }
    }
    this.setHtml = function (value) {
        iFrame.contents().find('html').html(value);
    }
    function getWidth() {
        return iFrame.width();
    }
    function setWidth(value) {
        iFrame.width(value);
    }
    function getHeight() {
        return iFrame.height();
    }
    function setHeight(value) {
        return iFrame.height(value);
    }

    function storeSize() {
        size.width = iFrame.width();
        size.height = iFrame.height();
    }
    this.viewCode = function (codeview) {
        if (codeview) {

            if (!fullscreenMode) {
                storeSize();
                textareaCode.width(size.width);
                textareaCode.height(size.height);
            }
            textareaCode.val(self.getHtml());

            BITEDITOR.toolbar.viewCodeMode();
            self.mode = BITEDITOR.EDITORMODE.VIEWCODE;
            iFrame.hide();

            textareaCode.show();
            textareaCode.focus();

        }
        else {
            textareaCode.hide();
            self.setHtml(textareaCode.val());

            BITEDITOR.toolbar.enable();
            self.mode = BITEDITOR.EDITORMODE.WYSIWYG;

            iFrame.show();
            iFrame.focus();
        }
    }

    this.fullscreen = function (fullscreen) {
        fullscreenMode = fullscreen;
        if (fullscreen) {
            storeSize();
            iFrame.attr("class", "bitEditor fullscreen");
            textareaCode.attr("class", "fullscreen");
            $(textareaCode).width(window.innerWidth);
            $(textareaCode).height(window.innerHeight);
            //$(div).width(window.innerWidth);
            //$(div).height(window.innerHeight);
        }
        else {
            iFrame.attr("class", "bitEditor");
            textareaCode.attr("class", "codeview");
            $(textareaCode).width(size.width);
            $(textareaCode).height(size.height);
            //$(div).width(size.width);
            //$(div).height(size.width);
        }
    }
    this.get = function () {
        if (this.mode == BITEDITOR.EDITORMODE.VIEWCODE) {
            return textareaCode;
        }
        else {
            return iFrame;
        }
    }

    this.readOnly = function (readonly) {
        if (readonly) {
            if (fullscreenMode) {
                self.fullscreen(false);
            }
            if (self.mode == BITEDITOR.EDITORMODE.VIEWCODE) {
                self.viewCode(false);
            }
            div.removeAttribute('contenteditable');
            div.setAttribute('class', 'bitEditor');
            BITEDITOR.toolbar.readOnlyMode();
            self.mode = BITEDITOR.EDITORMODE.READONLY;
        }
        else {

            div.setAttribute('contenteditable', true);
            div.setAttribute('class', 'bitEditor editMode');
            BITEDITOR.toolbar.enable();
            self.mode = BITEDITOR.EDITORMODE.WYSIWYG;
        }

    }

    this.remove = function () {
        divWrapper.innerHTML = "";
    }
    EditorIFrame();
}

