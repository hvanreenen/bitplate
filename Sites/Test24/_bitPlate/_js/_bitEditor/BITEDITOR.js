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

//globale variabele -- Singleton -- vergelijk AppObject in een applicatie
//van alle elementen opgeslagen in deze vaiabele is er maar 1 van:
//er is 1 toolbar, 1 statusbar, 1 actieve editor en 1 huidige selectie
//ook van sommige de popups is er maar 1
var BITEDITOR = {
    editors: new Array(),
    currentEditor: null,
    menubar: null,
    toolbar: null,
    statusbar: null,
    commandManager: new CommandManager(),
    selection: null,
    //modulesPopup: null,
    //tagsPopup: null,
    //imagesPopup: null,
    //hyperlinksPopup: null,
    //stylesPopup: null,
    referrerUrl: "",
    onSave: null, //function(){}
    onSelectEditor: null, //function () { }
    

    createEditor: function (elementWrapper) {
        //var elementWrapper = document.getElementById(elementId);
        if (elementWrapper.jquery) {
            var id = elementWrapper.attr('id');
            elementWrapper = document.getElementById(id);
        }
        else if (typeof(elementWrapper) == "string") {
            elementWrapper = document.getElementById(elementWrapper);
        }
        var editor = new Editor(elementWrapper);
        var id = elementWrapper.getAttribute("id");
        BITEDITOR.editors[id] = editor;
        return editor;
    },

    createEditors: function (selector) {
        $(selector).each(function () {
            BITEDITOR.createEditor(this);
        });
        //eerste krijgt focus
        if (getArraySize(BITEDITOR.editors) > 0) {

            getFirstFromArray(BITEDITOR.editors).focus();
        }
    },

    createToolbar: function (elementId) {
        var elementWrapper = document.getElementById(elementId);
        var toolbar = new Toolbar();
        if (elementWrapper.getAttribute("class") == "") {
            elementWrapper.setAttribute("class", "bitToolbar");
        }
        elementWrapper.appendChild(toolbar.get());
        BITEDITOR.toolbar = toolbar;
        return toolbar;
    },

    createMenubar: function (elementId) {
        var elementWrapper = document.getElementById(elementId);
        var menu = new Menu();

        elementWrapper.setAttribute("class", "bitMenu");
        elementWrapper.appendChild(menu.get());
        BITEDITOR.menubar = menu;
    },

    createStatusbar: function (elementId) {
        var elementWrapper = document.getElementById(elementId);
        var statusbar = new Statusbar();
        elementWrapper.appendChild(statusbar.get());
        elementWrapper.setAttribute("class", "bitStatusbar");
        BITEDITOR.statusbar = statusbar;
        return statusbar;
    },

    storeSelection: function () {

        BITEDITOR.selection = new Selection();
        BITEDITOR.selection.get();
    },

    navigateBack: function () {
        if (this.referrerUrl) {
            location.href = this.referrerUrl;
        }
        else {
            location.href = "Default.aspx";
        }
    },

    save: function(){
        if(this.onSave){
            this.onSave();
        }
    },

    selectEditor: function () {
        if (this.onSelectEditor) {
            this.onSelectEditor();
        }
    },

    //constants
    //EDITORTYPE: { DIV: "DIV", IFRAME: "IFRAME" },
    EDITORMODE: { WYSIWYG: "WYSIWYG", CODEMIRROR: "CODEMIRROR", READONLY: "READONLY", TEXTAREA: "TEXTAREA" },
    BUTTON_DISPLAY_STYLE: { IMAGEONLY: "IMAGEONLY", TEXTONLY: "TEXTONLY" }
};

//toolbar separator ook de functie enabled geven:
ToolbarSeparator.prototype = new ToolbarButton();
ToolbarDropDown.prototype = new ToolbarButton();