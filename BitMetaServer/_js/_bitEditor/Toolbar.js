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

function Toolbar() {
    var self = this;
    this.toolbarItems = new Array();
    var div = document.createElement("div");
    var mode = BITEDITOR.EDITORMODE.WYSIWYG;

    function Toolbar() {
        self.add(new ToolbarButton("back", "<-- terug "));
        self.add(new ToolbarButton("save", "opslaan"));
        self.add(new ToolbarButton("code", "code"));
        //self.add(new ToolbarButton("fullscreen", "volledig scherm"));
        //self.add(new ToolbarButton("readonly", "alleen lezen"));
        self.add(new ToolbarSeparator());
        self.add(new ToolbarButton("bold", "vet"));
        self.add(new ToolbarButton("italic", "cursief"));
        self.add(new ToolbarButton("underline", "ondersteept"));
        self.add(new ToolbarButton("strikethrough", "doorgehaald"));
        self.add(new ToolbarSeparator());
        self.add(new ToolbarButton("indent", "vergroot inspringen"));
        self.add(new ToolbarButton("outdent", "verklein inspringen"));
        self.add(new ToolbarSeparator());
        self.add(new ToolbarButton("insertorderedlist", "nummers"));
        self.add(new ToolbarButton("insertunorderedlist", "bullets"));
        self.add(new ToolbarSeparator());
        self.add(new ToolbarDropDown("heading", "headers", ["div", "p", "h1", "h2", "h3", "h4"]));
        self.add(new ToolbarSeparator());
        self.add(new ToolbarButton("justifyleft", "links uitlijnen"));
        self.add(new ToolbarButton("justifycenter", "centreren"));
        self.add(new ToolbarButton("justifyright", "rechts uitlijnen"));
        self.add(new ToolbarButton("justifyfull", "opvullen"));
        self.add(new ToolbarSeparator());
        self.add(new ToolbarButton("hyperlink", "hyperlink"));
        self.add(new ToolbarButton("unlink", "verwijder hyperlink"));
        self.add(new ToolbarButton("anchor", "anker"));
        self.add(new ToolbarSeparator());
        self.add(new ToolbarButton("insertimage", "afbeeldingen"));
        self.add(new ToolbarSeparator());
        //self.add(new ToolbarButton("modules", "modules"));
        //        self.add(new ToolbarButton("moduleProperties", "module eigenschappen"));
        //        self.add(new ToolbarButton("deleteModule", "verwijder module"));
        self.add(new ToolbarButton("tags", "tags"));
        self.add(new ToolbarButton("styles", "styles"));
        //self.add(new ToolbarButton("configpage", "pagina eigenschappen"));
        //self.add(new ToolbarDropDown("selectpage", "kies pagina", [{ name: "Pagina 1", value: "d9192a64-970e-470d-a2c8-e5e75420a535" }, { name: "Pagina 2", value: "e4174392-81e2-43bb-b347-a813f8b05959"}]));
        //self.add(new ToolbarDropDown("selectpage", "kies pagina"));


        //toolbarItems["readonly"].isToggleable = true;
        self.toolbarItems["code"].isToggleable = true;
        //self.toolbarItems["fullscreen"].isToggleable = true;
        self.toolbarItems["bold"].isToggleable = true;
        self.toolbarItems["italic"].isToggleable = true;
        self.toolbarItems["underline"].isToggleable = true;
        self.toolbarItems["strikethrough"].isToggleable = true;

        self.toolbarItems["back"].hide();
        self.toolbarItems["back"].displayStyle = BITEDITOR.BUTTON_DISPLAY_STYLE.TEXTONLY;
        self.toolbarItems["tags"].displayStyle = BITEDITOR.BUTTON_DISPLAY_STYLE.TEXTONLY;
        self.toolbarItems["styles"].displayStyle = BITEDITOR.BUTTON_DISPLAY_STYLE.TEXTONLY;
        //toolbarItems["readonly"].displayStyle = BITEDITOR.BUTTON_DISPLAY_STYLE.TEXTONLY;
        //toolbarItems["modules"].displayStyle = BITEDITOR.BUTTON_DISPLAY_STYLE.TEXTONLY;
        //toolbarItems["configpage"].displayStyle = BITEDITOR.BUTTON_DISPLAY_STYLE.TEXTONLY;



        div.style.float = "left";
        for (var index in self.toolbarItems) {
            var toolbarItem = self.toolbarItems[index].get();
            div.appendChild(toolbarItem);
        }

        self.readOnlyMode();
    }

    this.get = function () {
        return div;
    }

    this.add = function (toolbarItem) {
        if (toolbarItem.name) {
            self.toolbarItems[toolbarItem.name] = toolbarItem;
        }
        else {
            //zonder naam = separator
            self.toolbarItems["sep" + (BITUTILS.getArraySize(self.toolbarItems) + 1)] = toolbarItem;
        }
    }

    this.attach = function (editor) {
        if (editor.mode == BITEDITOR.EDITORMODE.CODEMIRROR) {
            self.toolbarItems["code"].setSelected(true);
        }
        else if (editor.mode == BITEDITOR.EDITORMODE.TEXTAREA) {
            self.toolbarItems["code"].setSelected(true);
        }
        else if (editor.mode == BITEDITOR.EDITORMODE.WYSIWYG) {
                //this.enable();
            self.toolbarItems["code"].setSelected(false);
        }
        this.setMode(editor.mode);

    }

    this.setMode = function (value) {
        mode = value;
        this.enable();
    }

    this.readOnlyMode = function () {
        for (var index in self.toolbarItems) {
            var toolbarItem = self.toolbarItems[index];
            if (!(toolbarItem.name == 'pageconfig' || toolbarItem.name == 'modules' || toolbarItem.name == 'selectpage')) {
                toolbarItem.setEnabled(false);
            }
        }
    }
    this.enable = function () {
        for (var index in self.toolbarItems) {
            var toolbarItem = self.toolbarItems[index];
            if (mode == BITEDITOR.EDITORMODE.WYSIWYG) {
                toolbarItem.setEnabled(true);
            }
            else { //coeview modus dan sommige aan zetten en sommige niet
                if (toolbarItem.name == 'indent' ||
                    toolbarItem.name == 'outdent' ||
                    toolbarItem.name == 'insertorderedlist' ||
                    toolbarItem.name == 'insertunorderedlist' ||
                    toolbarItem.name == 'justifyleft' ||
                    toolbarItem.name == 'justifycenter' ||
                    toolbarItem.name == 'justifyright' ||
                    toolbarItem.name == 'justifyfull' ||
                    toolbarItem.name == 'unlink') {
                    toolbarItem.setEnabled(false);
                }
                else {
                    toolbarItem.setEnabled(true);
                }
            }
        }
    }
    this.disable = function () {
        for (var index in self.toolbarItems) {
            var toolbarItem = self.toolbarItems[index];
            toolbarItem.setEnabled(false);
        }
    }
    this.toggleButtons = function (tagnames) {
        self.toolbarItems["bold"].setSelected(false);
        self.toolbarItems["italic"].setSelected(false);
        self.toolbarItems["underline"].setSelected(false);
        self.toolbarItems["strikethrough"].setSelected(false);
        for (var i in tagnames) {
            var tagname = tagnames[i];
            if (tagname == 'B' || tagname == 'STRONG') {
                self.toolbarItems["bold"].setSelected(true);
            }
            else if (tagname == 'I' || tagname == 'EM') {
                self.toolbarItems["italic"].setSelected(true);
            }
            else if (tagname == 'U') {
                self.toolbarItems["underline"].setSelected(true);
            }
            else if (tagname == 'STRIKE') {
                self.toolbarItems["strikethrough"].setSelected(true);
            }
        }
    }

    this.fillDropDown = function (name, items) {
        var select = self.toolbarItems[name];
        select.fill(items);
    }

    Toolbar();
}