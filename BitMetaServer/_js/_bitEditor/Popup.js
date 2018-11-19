function Popup(title, content, position, size) {
    var self = this;
    this.position = position;
    this.size = size;
    this.target = "";
    
    var div = document.createElement("div");
    var titlediv = document.createElement("div");
    var contentdiv = document.createElement("div");
    var bottomdiv = document.createElement("div");
    var okbutton = document.createElement("button");
    var cancelbutton = document.createElement("button");
    var loaded = false;

    function Popup() {
        div.setAttribute("class", "bitPopup");

        titlediv.setAttribute("class", "bitPopupTitle");
        contentdiv.setAttribute("class", "bitPopupContent");
        bottomdiv.setAttribute("class", "bitPopupBottom");
        titlediv.innerHTML = title;
        titlediv.setAttribute("id", "bitPopupTitle");
        //contentdiv.setAttribute("contenteditable", true);
        contentdiv.innerHTML = content;


        okbutton.innerHTML = "OK";
        cancelbutton.innerHTML = "Cancel";
        div.appendChild(titlediv);
        div.appendChild(contentdiv);
        div.appendChild(bottomdiv);
        bottomdiv.appendChild(okbutton);
        bottomdiv.appendChild(cancelbutton);
        $(div).draggable({ handle: '#bitPopupTitle', cursor: 'crosshair' });
        //$(div).offset({ left: 300, top: 200 });
        $(div).resizable();
        cancelbutton.onclick = function () {
            self.hide();
        }
        okbutton.onclick = function () {
            self.onOk();
        }
    }

    this.hideOkButton = function () {
        $(okbutton).hide();
    }

    this.hideCancelButton = function () {
        $(cancelbutton).hide();
    }

    this.setContent = function (html) {
        $(contentdiv).html(html);
    }

    this.setContentDiv = function (divContent) {
        $(contentdiv).replaceWith(divContent);
        contentdiv = divContent;
    }
    this.getContentDiv = function () {
        return contentdiv;
    }

    this.load = function (url, callback) {
        //definieer een toemstamp om browser cahching te voorkomen
        var timestamp = new Date().getTime();
        if (url.indexOf('?') <= 0) url += '?' + timestamp;
        else url += '&' + timestamp;
        $(contentdiv).load(url, function () {
            //$(contentdiv).dockable();
            $(contentdiv).helpable();
            self.onload();
            if (callback) {
                callback();
            }
        });
    }

    this.onload = function () {
        
    }

    this.onOk = function () {
        self.hide();
    }

    this.show = function () {
        if (!loaded) {
            $('body').append(div);
            if (position == null) {
                $(div).center();
            }
            else {
                $(div).css("position", "absolute");
                $(div).css('top', position.top + 'px');
                $(div).css('left', position.left + 'px');
            }
            if (size != null) {
                $(div).css('width', size.width + 'px');
                $(div).css('height', size.height + 'px');
                $(contentdiv).css('height', (size.height - 75) + 'px');
            }
            loaded = true;
        }
        else {
            $(div).fadeIn(500);
            //$(div).center();
        }
        $(document).scrollTop(0);
        self.onShow();
    }

    this.hide = function () {
        $(div).fadeOut(500);
        self.onHide();
    }

    this.setTitle = function (title) {
        titlediv.innerHTML = title;
    }

    this.onHide = function () { }
    this.onShow = function () { }
    this.get = function () {
        self.onload();
        return div;
    }
    Popup();
}


//functie om element (popup) te centreren in het scherm
//aanroep: $(div).center();
jQuery.fn.center = function () {
    this.css("position", "absolute");
    this.css("top", (($(window).height() - this.outerHeight()) / 2) +
                                                $(window).scrollTop() + "px");
    this.css("left", (($(window).width() - this.outerWidth()) / 2) +
                                                $(window).scrollLeft() + "px");
    return this;
}