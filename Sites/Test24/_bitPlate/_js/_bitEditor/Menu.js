function Menu(items) {
    var self = this;
    var div = document.createElement("div");
    this.menuItems = new Array();

    function Menu() {
        
        
    }

    this.add = function (menuItem) {
        self.menuItems[menuItem.text] = menuItem;
    }

    this.fill = function (mainMenuItemName, commandName, items) {
        
        for (var index in items) {
            var item = items[index];

            self.menuItems[mainMenuItemName].add(new MenuItem(commandName, item.Text, item.Value));
        }
    }

    this.empty = function (mainMenuItemName) {
        self.menuItems[mainMenuItemName].removeAll();
    }
    this.get = function () {
        var test = div.innerHTML;
        return div;
    }

    Menu();
}