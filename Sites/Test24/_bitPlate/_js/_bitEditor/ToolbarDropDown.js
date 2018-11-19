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
function ToolbarDropDown(id, title, items) {
    var self = this;
    var select = document.createElement("select");
    this.name = id;
    this.title = title;

    function ToolbarDropDown() {

        select.name = "id";
        select.options.add(new Option("<" + self.title + ">", ""));
        for (var index in items) {
            var item = items[index];
            if (item.name) {
                select.options.add(new Option(item.name, item.value));
            }
            else {
                select.options.add(new Option(item, item));
            }

        }
        select.onchange = self.command;
        
    }

    this.command = function () {
        var item = select.options[select.selectedIndex].value;
        BITEDITOR.commandManager.exec(self.name, false, item);
    }

    this.setEnabled = function (enabled) {
        if (enabled) {
            select.removeAttribute("disabled");
        }
        else
            select.setAttribute("disabled", "disabled");
    }

    this.fill = function (items) {
        select.options.length = 0;
        select.options.add(new Option("<" + self.title + ">", ""));
        for (var index in items) {
            var item = items[index];
            select.options.add(new Option(item.Text, item.Value));
        }
    }

    this.get = function () {
        return select;
    }

    ToolbarDropDown();
}
