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

function ToolbarButton(name, tooltip, parameters) {
    var self = this;
    var button = document.createElement("button");
    
    this.isToggleable = false;
    this.selected = false;
    this.name = name;
    this.tooltip = tooltip;
    this.displayStyle = BITEDITOR.BUTTON_DISPLAY_STYLE.IMAGEONLY;
    //this.arguments = arguments;

    function ToolbarButton() {
        
            button.onclick = self.command;
            if (self.displayStyle == BITEDITOR.BUTTON_DISPLAY_STYLE.IMAGEONLY) {
                button.innerHTML = "<span class='" + self.name + "' title='" + self.tooltip + "'>&nbsp;</span>";
            }
            else if (self.displayStyle == BITEDITOR.BUTTON_DISPLAY_STYLE.TEXTONLY) {
                button.innerHTML = self.tooltip;
            }
            button.setAttribute("class", "bitToolbarButton");
            button.setAttribute("title", self.tooltip);
            button.setAttribute("type", "button");
        
    }

    this.command = function () {
        //in events slaat het keyword this op het object dat het event aanroept, daarom gebruik van self
        if (self.isToggleable)
            self.toggle();
        //if (!parameters) parameters = self.selected; //aan uit van de knop
        BITEDITOR.commandManager.exec(self.name, self.selected, parameters);
    }

    this.toggle = function () {
        if (this.selected)
            this.selected = false;
        else
            this.selected = true;
        this.setSelected(this.selected);
    }

    this.hide = function () {
        $(button).hide();
    }

    this.show = function () {
        $(button).show();
    }

    this.setSelected = function (value) {
        this.selected = value;
        if (this.selected)
            button.setAttribute("class", "bitToolbarButton selected");
        else
            button.setAttribute("class", "bitToolbarButton");
    }

    this.setEnabled = function (enabled) {
        if (enabled) {
            button.removeAttribute("disabled");
            button.setAttribute("class", "bitToolbarButton");
        }
        else {
            button.setAttribute("disabled", "disabled");
            button.setAttribute("class", "bitToolbarButton disabled");
        }
    }

    this.get = function () {
        ToolbarButton();
        return button;
    }

    //ToolbarButton();
}


