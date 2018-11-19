function MenuItem(commandName, text, parameters) {
    var self = this;
    this.name = commandName;
    this.text = text;
    this.parameters = parameters;
    this.checkable = false;
    var checked = false;
    var div = document.createElement("div");
    var divChildMenus = document.createElement("div");
    var childMenuItems = new Array();
    this.menuItems = childMenuItems;
    var hasChildMenuItems = false;

    function MenuItem() {
        
        divChildMenus.setAttribute("class", "bitSubMenu");
    }

    this.add = function (menuItem) {
        if (!hasChildMenuItems) {
            //eerste keer: toevoegen
            div.appendChild(divChildMenus);
            //en tonen bij hover vaan hoofdmenu item
            $(div).hover(
            function () {
                $(this).find('.bitSubMenu').show();
                var left = $(div).position().left;
                $(this).find('.bitSubMenu').css('left', left + 'px');
            },
            function () {
                $(this).find('.bitSubMenu').hide();
            });
        }
        hasChildMenuItems = true;
        var key = menuItem.text;
        //        if (menuItem.parameters) {
        //            key = key + menuItem.parameters;
        //        }
        childMenuItems[key] = menuItem;

        var menuItemDiv = menuItem.get();
        if (menuItemDiv.tagName.toLowerCase() == "div") {
            menuItemDiv.setAttribute("class", "bitSubMenuItem");
        }
        divChildMenus.appendChild(menuItemDiv);
    }

    this.command = function (e) {
        //alert(self.name);
        if (self.name != "") {
            if (self.checkable) {
                if (checked) {
                    checked = false;
                }
                else {
                    checked = true;
                }
                self.setChecked(checked);
            }

            BITEDITOR.commandManager.exec(self.name, checked, parameters);

            $(div).parent('.bitSubMenu').hide();

            stopPropagation(e);
        }
    }

    //stop event bubbling. Anders gaat command van hoofdmenuitem ook af
    function stopPropagation(e)
    {
        e = e||event;/* get IE event ( not passed ) */
        e.stopPropagation? e.stopPropagation() : e.cancelBubble = true;
    }

    this.setChecked = function (bool) {
        checked = bool;
        if (checked) {
            div.innerHTML = self.text + " (image: aan)";
        }
        else {
            div.innerHTML = self.text + " (image: uit)";
        }
    }
    this.setEnabled = function (enabled) {
        if (enabled) {
            div.removeAttribute("disabled");
        }
        else
            div.setAttribute("disabled", "disabled");
    }

    this.removeAll = function () {
        divChildMenus.innerHTML = "";
        childMenuItems.length = 0;
    }

    this.get = function () {
        div.setAttribute("class", "bitMenuItem");
        div.innerHTML = text;
        div.onclick = self.command;
        if (self.checked) {
            div.innerHTML += " (aan)";
        }
        if (self.text == "-") {
            return document.createElement("hr");
        }
        else {

            return div;
        }
    }

    MenuItem();
}