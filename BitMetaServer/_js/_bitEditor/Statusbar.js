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
function Statusbar() {
    this.self = this;
    var _div = document.createElement("div");
    
    this.get = function (){
        return _div;
    }

    this.setText = function (text) {
        _div.innerHTML = text;
    }
}