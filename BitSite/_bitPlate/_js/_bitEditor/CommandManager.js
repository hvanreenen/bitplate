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

//CommandManager deelt de commando's uit afkomstig van de toobarbuttons.
//elke toolbarbutton heeft een (commando)naam. Deze naam wordt doorgegeven aan de CommandManager
//

function CommandManager() {
    var self = this;

    this.exec = function (commandName, selected, parameters) {
        if (commandName == 'back') {
            BITEDITOR.navigateBack();
        }
        else if (commandName == 'save') {
            BITEDITOR.save();
            //if (BITEDITOR.currentEditor.id.indexOf("Template") >= 0) {
            //    BITEDITPAGE.saveTemplate();
            //}
            //else {
            //    BITEDITPAGE.saveAllModules(); 
            //}
        }
        else if (commandName == 'code') {
            BITEDITOR.currentEditor.viewCode(selected);
        }
        else if (commandName == 'fullscreen') {
            BITEDITOR.currentEditor.fullscreen(selected);
        }
        else if (commandName == 'styles') {
            showStylesPopup();
        }
        else if (commandName == 'hyperlink') {
            showHyperlinksPopup();
        }
        else if (commandName == 'insertimage') {
            showImagesPopup();
        }
        else if (commandName == 'tags') {
            showTagsPopup();
        }
        else if (commandName == 'heading') {
            var header = parameters;
            if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.WYSIWYG) {
                BITEDITOR.currentEditor.get().focus();
                document.execCommand("formatblock", false, "<" + header + ">");
            }
            else {
                var elm = document.createElement(header);
                BITEDITOR.selection.wrap(elm);
            }
        }
         
        else {
            if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.WYSIWYG) {
                //standaard functies zoals bold, italic, align, orderlist enz...
                BITEDITOR.currentEditor.get().focus();
                document.execCommand(commandName, false, null);

                if (commandName == 'bold' || commandName == 'italic' || commandName == 'underline' || commandName == 'strikethrough') {
                    BITEDITOR.currentEditor.whichElement();
                }
            }
            else {
                var elm;
                if (commandName == 'bold') {
                    elm = document.createElement("b");
                }
                else if (commandName == 'italic') {
                    elm = document.createElement("i");
                }
                else if (commandName == 'underline') {
                    elm = document.createElement("u");
                }
                if (elm) {
                    BITEDITOR.selection.wrap(elm);
                }
            }
        }
        if (BITEDITOR.currentEditor) BITEDITOR.currentEditor.get().focus();

    }

    function showTagsPopup() {
        BITEDITPAGE.showTagsPopup();
    }

    function showImagesPopup() {
        $('#bitImagesDialog').dialog("open");
    }

    function showHyperlinksPopup() {
        $('#bitHyperlinksDialog').dialog("open");
    }

    function showStylesPopup() {
        $('#bitStylesDialog').dialog("open");
        //$('#bitStylesDialog').html(StylesheetParser.getDocumentRulesHtml());

        $('#bitStylesDialog').find('div').click(function () {
            var classname = $(this).attr("class");
            BITEDITOR.selection.setCssClass(classname);
        });
    }
}




