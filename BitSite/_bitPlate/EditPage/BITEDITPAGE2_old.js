var EDITPAGETYPES = { PAGE: "PAGE", TEMPLATE: "TEMPLATE" };

//de edit page is de pagina die wordt geedit

var BITEDITPAGE = {
    type: EDITPAGETYPES.PAGE, //PAGE of TEMPLATE
    pageId: null,
    page: null,
    language: '', 
    newsletterId: null,
    newsletter: null,
    attachedDraggable: true,
    currentModule: null,
    loadData: true, //kijk of de data in de modules moet worden geladen
    codeMirrorScriptEditor: null,
    currentModuleTags: '',
    contextMenuTemplate: null,
    loadedModuleScripts: [],
    moduleConfigJsClasses: [],

    //Inline editor variables
    activeInlineModuleEditor: null,
    activeInlineEditorElement: null,
    activeInlineEditorId: '',
    activeInlineEditorOrginalContent: null,
    currentModuleContent: Array(),

    isNewsletterMode: function () {
        //kijk of er een newsletterId is
        return (BITEDITPAGE.newsletterId && BITEDITPAGE.newsletterId != "");
    },

    ckeditorToolbartoolbar: [
                { "name": "document", "groups": ["mode", "document", "doctools"], "items": ["Source", "-", "NewPage", "-", "Templates"] },
                { "name": "clipboard", "groups": ["clipboard", "undo"], "items": ["Cut", "Copy", "Paste", "PasteText", "PasteFromWord", "-", "Undo", "Redo"] },
                { "name": "editing", "groups": ["find", "selection", "spellchecker"], "items": ["Find", "Replace", "-", "SelectAll"] },
                { "name": "forms", "items": ["Form", "Checkbox", "Radio", "TextField", "Textarea", "Select", "Button", "ImageButton", "HiddenField"] },
                "/",
                { "name": "paragraph", "groups": ["list", "indent", "blocks", "align", "bidi"], "items": ["NumberedList", "BulletedList", "-", "Outdent", "Indent", "-", "Blockquote", "CreateDiv", "-", "JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock", "-", "BidiLtr", "BidiRtl"] },
                //"/", //Enter verzoek van Misja
                { "name": "links", "items": ["Link", "Unlink", "Anchor"] }, { "name": "insert", "items": ["Image", "Flash", "Table", "HorizontalRule", "Smiley", "SpecialChar", "PageBreak", "Iframe", "Tags"] },
                "/",
                { "name": "basicstyles", "groups": ["basicstyles", "cleanup"], "items": ["Bold", "Italic", "Underline", "Strike", "Subscript", "Superscript", "-", "RemoveFormat"] },
                { "name": "styles", "items": ["Styles", "Format", "Font", "FontSize"] },
                { "name": "colors", "items": ["TextColor", "BGColor"] },
                { "name": "tools", "items": ["ShowBlocks"] }
                //{ "name": "others", "items": ["-"] },
                //{ "name": "about", "items": ["About"] }
    ],

    initialize: function () {

        $('body').css('padding-top', '25px');
        $('#bitConfigModuleDialog').initDialog(BITEDITPAGE.saveConfigModule, { width: '90%', height: window.innerHeight });
        $('#bitEditModuleDialog').initDialog(BITEDITPAGE.saveModuleContentFromPopup, { width: '90%', height: window.innerHeight });
        $('#bitScriptEditDialog').initDialog(BITEDITPAGE.saveScript, { width: '90%', height: window.innerHeight });


        $('#bitTagsDialog').initDialog(null, { width: 300, height: 500 }, false);

        CKEDITOR.disableAutoInline = true;
        BITEDITPAGE.initializeCkePopupEditor();
        $('#bitPopups').formEnrich();

        $('#bitSideBar').hover(
	        function () {
		        // alert('ok werkt hover?');
		        $('#bitSideBar').css('width', '166px');
		        $('#bitSideBar ul li a').css('width', '130px');
	        },
	        function () {
		        $('#bitSideBar').css('width', '50px');
		        $('#bitSideBar ul li a').css('width', '40px');
	        }
        );

        // bij kleine schermen moet er een scrollbar komen dus de position moet absolute worden en een px height ipv % height

        var sHeight = window.innerHeight
        //var sHeight = screen.height
        //var sWidth = screen.availWidth
        // alert(sHeight);
        if ((sHeight <= 800)) {
            $('#bitSideBar').css('position', 'fixed');
            $('#bitSideBar').css('height', '777px');
        }
        if ((sHeight <= 800) && $.browser.msie) {
            $('#bitSideBar').css('position', 'fixed');
            $('#bitSideBar').css('height', '900px');
        }

        $('#divTopMenu').css({
            'position': 'fixed',
            'z-index': '9'
        });

        // functie om sidebar te locken en unlocken
        $('.bitSideBarLock').click(
                function () {
                    //alert('click');
                    $(this).toggleClass('bitSideBarLocked');
                    $('#bitSideBar').toggleClass('bitSideBarLockedSecure');
                });
        $('#bitSideBar').draggable({
            cursor: 'crosshair',
            handle: "#bitplateULDealer"
        });

        //window.window.onpopstate = BITEDITPAGE.onpopstate;
           

        $('#bitEditModuleDialog').on("dialogresize", function (event, ui) {
            var y = $(event.target).height();
            $('#bitModuleEditorInPopup').height(y - 80);
            if (BITEDITPAGE.EditorInPopup.resize) BITEDITPAGE.EditorInPopup.resize("99%", y - 90);
        });
    },

    initializeModuleEditor: function (id) {

        var editorChoice = $('#iframe').contents().find('#bitModule' + id).attr('data-module-editor');
        if (editorChoice === 'CKEDITOR') {
            //CKEDITOR
            BITEDITPAGE.initializeCkePopupEditor();
        }
        else {
            //CODEMIRROR
            BITEDITPAGE.initializeCodeMirrorPopupEditor();
        }
    },

    initializeCkePopupEditor: function () {
        if (BITEDITPAGE.EditorInPopup && !BITEDITPAGE.EditorInPopup.mirror) {
            BITEDITPAGE.EditorInPopup.destroy();
        }
        var editor = CKEDITOR.replace('EditorInPopup', {
            filebrowserBrowseUrl: '/_bitplate/Dialogs/Finder.aspx?type=url',
            filebrowserImageBrowseUrl: '/_bitplate/Dialogs/Finder.aspx?type=image',
            filebrowserWindowWidth: 1000,
            filebrowserWindowHeight: 600,
            //toolbar: BITEDITPAGE.ckeditorToolbartoolbar,
            enterMode: CKEDITOR.ENTER_BR
        });
        editor.config.extraPlugins = 'tags,codemirror';
        BITEDITPAGE.EditorInPopup = editor;
    },

    initializeCodeMirrorPopupEditor: function () {
        CodeMirror.commands.autocomplete = function (cm) {
            /***********************************************************************
            {list: getCompletions(token, context, keywords, options),
                from: Pos(cur.line, token.start),
                to: Pos(cur.line, token.end)};
            ***********************************************************************/

            CodeMirror.showHint(cm, function (cm, options) {
                var htmlHintObject = CodeMirror.htmlHint(cm, options);



                htmlHintObject['list'] = BITEDITPAGE.currentModuleTags.CMTags.concat(htmlHintObject['list']);

                var cur = cm.getCursor(), token = cm.getTokenAt(cur);
                var inner = CodeMirror.innerMode(cm.getMode(), token.state);
                if (inner.mode.name != "xml") return;
                var result = [], replaceToken = false, prefix;
                var isTag = token.string.charAt(0) == "{";
                if (!inner.state.tagName || isTag) { // Tag completion
                    if (isTag) {
                        prefix = token.string.slice(1);
                        htmlHintObject['list'] = Array();

                        for (var tag in BITEDITPAGE.currentModuleTags.CMTags) {
                            if (BITEDITPAGE.currentModuleTags.CMTags[tag].toLowerCase().indexOf(prefix.toLowerCase()) !== -1) {
                                htmlHintObject['list'].push(BITEDITPAGE.currentModuleTags.CMTags[tag].replace('{' + prefix, ''));
                            }
                        }
                    }
                }
                return htmlHintObject;
            });
        }
        var uiOptions = { path: '/_bitplate/_js/plugins/codemirror-ui/js/', searchMode: 'inline', buttons: ['undo', 'redo', 'jump', 'reindentSelection', 'reindent', 'tag'], tagCallBack: BITEDITPAGE.showTagsPopup }
        var codeMirrorOptions = {
            mode: "text/html",
            tabMode: "indent",
            lineNumbers: true,
            extraKeys: { "Ctrl-Space": "autocomplete" },
            theme: 'default',
            matchBrackets: true,
            workDelay: 300,
            workTime: 35,
            readOnly: false,
            lineNumbers: true,
            lineWrapping: true,
            autoCloseTags: true,
            autoCloseBrackets: true,
            highlightSelectionMatches: true,
            continueComments: true
        }
        BITEDITPAGE.EditorInPopup = new CodeMirrorUI(document.getElementById('EditorInPopup'), uiOptions, codeMirrorOptions);
        /* BITEDITPAGE.EditorInPopup = CodeMirror.fromTextArea(document.getElementById('EditorInPopup'), {
            mode: "text/html",
            tabMode: "indent",
            lineNumbers: true,
            extraKeys: { "Ctrl-Space": "autocomplete" },
            theme: 'default',
            matchBrackets: true,
            workDelay: 300,
            workTime: 35,
            readOnly: false,
            lineNumbers: true,
            lineWrapping: true,
            autoCloseTags: true,
            autoCloseBrackets: true,
            highlightSelectionMatches: true,
            continueComments: true
        }); */

        //$('#bitEditModuleDialog').on("dialogresize", function (event, ui) {
        //});
    },

    ///////////////////////
    // PAGE
    ///////////////////////
    loadPage: function (pageid) {
        if (pageid == "") return;
        BITEDITPAGE.pageId = pageid;
        var url = "/Page.aspx?pageid=" + pageid;
        $("#iframe").unbind('load'); //om te vookomen dat load vaker afgaat
        $("#iframe").attr("src", url);
        $("#iframe").load(function () {
            BITEDITPAGE.insertCssInIframe("/_bitPlate/_themes/bitplate/css/pageEdit.css");

            BITEDITPAGE.attachModuleCommands();
            BITEDITPAGE.attachDraggable();
            BITEDITPAGE.fillModulesCheckBoxList(BITEDITPAGE.pageId);
            BITEDITPAGE.fillMenuScripts();
            BITEDITPAGE.pageNavigationChange();
        });
    },

    pageNavigationChange: function () {
        var parametersObject = { id: BITEDITPAGE.pageId, folderid: null };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/Pages/PageService.aspx";
        BITAJAX.callWebService("GetPage", jsonstring, function (data) {
            var page = data.d;
            BITEDITPAGE.page = page;
            window.history.replaceState({ "html": page.ID, "pageTitle": page.Title }, page.Name, page.RelativeUrl + '?mode=edit');
        });
    },

    //onpopstate: function(e){
    //    if(e.state){
    //        BITEDITPAGE.loadPage(history.state.html);
    //        //document.title = e.state.pageTitle;
    //    }
    //},

    previewPage: function () {
        if (BITEDITPAGE.pageId) {
            if (BITEDITPAGE.page && BITEDITPAGE.page.ID == BITEDITPAGE.pageId) {
                var page = BITEDITPAGE.page;
                var url = page.RelativeUrl;
                window.open(url);
            }
            else {
                var parametersObject = { id: BITEDITPAGE.pageId, folderid: null };
                var jsonstring = JSON.stringify(parametersObject);
                BITAJAX.dataServiceUrl = "/_bitplate/Pages/PageService.aspx";
                BITAJAX.callWebService("GetPage", jsonstring, function (data) {
                    var page = data.d;
                    BITEDITPAGE.page = page;
                    var url = page.RelativeUrl;
                    window.open(url);
                });
            }
        }

        if (BITEDITPAGE.newsletterId) {
            if (BITEDITPAGE.newsletter && BITEDITPAGE.newsletter.ID == BITEDITPAGE.newsletterId) {
                var newsletter = BITEDITPAGE.newsletter;
                var url = newsletter.RelativeUrl;
                window.open(url);
            }
            else {
                var parametersObject = { id: BITEDITPAGE.newsletterId };
                var jsonstring = JSON.stringify(parametersObject);
                BITAJAX.dataServiceUrl = "/_bitplate/Newsletters/NewsletterService.aspx";
                BITAJAX.callWebService("GetNewsletter", jsonstring, function (data) {
                    BITEDITPAGE.newsletter = data.d;
                    var newsletter = BITEDITPAGE.newsletter;
                    var url = newsletter.RelativeUrl;
                    window.open(url);
                });
            }
        }
        
    },

    insertCssInIframe: function (url) {
        var link = document.createElement('link');
        link.type = 'text/css';
        link.rel = "stylesheet";
        var timeticks = new Date().getTime();
        link.href = url; //+ '?time=' + timeticks;

        $('#iframe').contents().find("head").append(link);
    },

    //DEZE DRAGGABLE EN SORTABLE WERKEN ALLEEN VOOR NIEUWE MODULES DIE VANUIT DE SIDEBAR KOMEN.
    //MODULLES DIE VERPLAATST WORDEN GEBRUIKEN EEN JAVASCRIPT IN CMSPAGE.CS !!!!
    attachDraggable: function () {
        $('.moduleToDrag').draggable({
            cursor: 'move',
            //opacity: 0.8,
            helper: 'clone',
            connectToSortable: $("#iframe").contents().find('.bitContainer'),
            revert: 'invalid',
            stop: function (event, ui) {
                $('#bitSideBar').css('width', '50px');
                $('#bitSideBar ul li a').css('width', '40px');
                BITEDITPAGE.calulateOrderingNumber(ui.item);
            },
            iframeFix: true
        });

        $("#iframe").contents().find(".bitContainer").sortable({
            connectWith: ".bitContainer",
            //tolerance: 'pointer',
            placeholder: "bitContainerPlaceholder",
            handle: '.moduleMoveGrip', 
            //containment: 'parent',
            items: ":not(.editMode)",


            //out: function (event, ui) {
            //    $(ui.item.context.parentElement).css('border-color', 'red');
            //},

            update: function (event, ui) {
                if (this === ui.item.parent()[0]) {
                    var item = ui.item;
                    var isNew = ($(item).is('.moduleToDrag'));
                    var moduleType = $(item).attr('data-module-type');
                    var id = $(item).attr('id');
                    var containerName = $(this).attr('id').replace("bitContainer", "");
                    var index = $(this).children().index(ui.item[0]);


                    BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";

                    if (isNew) {
                        var parametersObject = { type: moduleType, pageid: BITEDITPAGE.pageId, containername: containerName, sortorder: index, newsletterid: BITEDITPAGE.newsletterId };
                        var jsonstring = JSON.stringify(parametersObject);
                        BITAJAX.callWebService("AddNewModule", jsonstring, function (data) {
                            item.replaceWith(data.d[0]);
                            BITEDITPAGE.attachModuleCommands();
                            BITEDITPAGE.addModuleScriptsToHead(data.d[1]);
                            BITEDITPAGE.fillModulesCheckBoxList(BITEDITPAGE.pageId);
                        });
                    }
                    else {
                        //sleep van ene container naar andere of van ene positie naar andere
                        var parametersObject = { moduleid: id, pageid: BITEDITPAGE.pageId, containername: containerName, sortorder: index, newsletterid: BITEDITPAGE.newsletterId };
                        var jsonstring = JSON.stringify(parametersObject);
                        BITAJAX.callWebService("MoveModule", jsonstring, null);
                    }
                }
            },

            stop: function (event, ui) {
                $(ui.item).find('.bitEditor').removeClass('bitEditorInDragg');
                var moduleMoveGrip = $(ui.item).find('.moduleMoveGrip');
                $(moduleMoveGrip).css({ 'width': 0, 'height': 0, 'display': 'none' });
                BITEDITPAGE.calulateOrderingNumber(ui.item);
            }
        });

        $("#iframe").contents().find(".bitContainer").disableSelection();
        BITEDITPAGE.attachedDraggable = true;
    },

    configPage: function () {
        if (BITEDITPAGE.pageId) {
            var parametersObject = { id: BITEDITPAGE.pageId, folderid: null };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/Pages/PageService.aspx";
            BITAJAX.callWebService("GetPage", jsonstring, function (data) {
                var page = data.d;
                BITPAGES.page = page;
                BITEDITPAGE.page = data.d;
                if ($('#bitConfigPagePopup').html() == "") {
                    //als configPage niet is geladen: maken we de popup aan voor de eerste keer
                    $('#bitConfigPagePopup').load('/_bitplate/Pages/Pages.aspx #bitPopups', function () {
                        BITPAGES.initialize(); //om de popups te maken
                        //BITPAGES.fillDropDownFolders(); //wordt in c# gedaan
                        //BITPAGES.loadTemplates();//wordt in c# gedaan

                        //databind
                        $('#bitPageDialog').dataBind(page);
                        $("#tableScriptsPerSite").dataBindList(page.SiteScripts);
                        $("#tableScriptsPerTemplate").dataBindList(page.Template.Scripts);
                        $("#tableScriptsPerPage").dataBindList(page.Scripts);

                        $('#bitPageDialog').initDialog(BITEDITPAGE.saveConfigPage);

                        $('#bitPageDialog').dialog('open');
                        $('#bitConfigPagePopup').show();

                    });
                }
                else {
                    $('#bitPageDialog').dataBind(page);
                    $("#tableScriptsPerSite").dataBindList(page.SiteScripts);
                    $("#tableScriptsPerTemplate").dataBindList(page.Template.Scripts);
                    $("#tableScriptsPerPage").dataBindList(page.Scripts);
                    $('#bitPageDialog').dialog('open');
                }
            });
        }
        if (BITEDITPAGE.newsletterId) {
            var parametersObject = { id: BITEDITPAGE.newsletterId };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/Newsletters/NewsletterService.aspx";
            BITAJAX.callWebService("GetNewsletter", jsonstring, function (data) {
                var newsletter = data.d;
                BITNEWSLETTER.newsletter = newsletter;
                BITEDITPAGE.newsletter = newsletter;
                if ($('#bitConfigPagePopup').html() == "") {
                    //als configPage niet is geladen: maken we de popup aan voor de eerste keer
                    $('#bitConfigPagePopup').load('/_bitplate/Newsletters/Newsletters.aspx #bitNewsletterDialog', function () {
                        $('.bitTabs').formEnrich();
                        //BITNEWSLETTER.initialize(); //om de popups te maken
                        //BITPAGES.fillDropDownFolders(); //wordt in c# gedaan
                        //BITPAGES.loadTemplates();//wordt in c# gedaan

                        //databind
                        BITAJAX.dataServiceUrl = "/_bitplate/Newsletters/NewsletterService.aspx";
                        BITAJAX.callWebService("LoadNewsletterGroupList", null, function (data) {
                            $('#tableNewsletterGroupList').dataBindList(data.d);
                            BITNEWSLETTER.newsletterGroups = data.d;
                            $('#bitNewsletterGroupSelect').fillDropDown(BITNEWSLETTER.newsletterGroups, null, 'ID');
                            $('#bitNewsletterDialog').dataBind(newsletter);
                            $("#tableScriptsPerSite").dataBindList(newsletter.SiteScripts);
                            $("#tableScriptsPerTemplate").dataBindList(newsletter.Template.Scripts);
                            $("#tableScriptsPerPage").dataBindList(newsletter.Scripts);
                            BITNEWSLETTER.currentNewsletterGroup = new Object();
                            BITNEWSLETTER.currentNewsletterGroup.ID = BITUTILS.EMPTYGUID;
                            $('#bitNewsletterDialog').initDialog(function () { BITNEWSLETTER.saveNewsletter(); BITEDITPAGE.newsletter = null; });
                            $('#bitNewsletterGroupSelect').chosen();
                            $('#bitNewsletterGroupSelect').trigger("liszt:updated");
                            $('#bitNewsletterDialog').dialog('option', 'title', 'Nieuwsbrief configuratie');
                            $('#bitNewsletterDialog').dialog('open');
                            $('#bitConfigPagePopup').show();
                        });
                    });
                }
                else {
                    $('#bitNewsletterDialog').dataBind(newsletter);
                    $("#tableScriptsPerSite").dataBindList(newsletter.SiteScripts);
                    $("#tableScriptsPerTemplate").dataBindList(newsletter.Template.Scripts);
                    $("#tableScriptsPerPage").dataBindList(newsletter.Scripts);
                    $('#bitPageDialog').dialog('open');
                }
            });

        }
        
    },

    saveConfigPage: function () {
        //opslaan page config
        var validation = $('#bitPageDialog').validate();
        if (validation) {
            //get data from panel
            BITEDITPAGE.page = $('#bitPageDialog').collectData(BITEDITPAGE.page);
            jsonstring = convertToJsonString(BITEDITPAGE.page);

            BITAJAX.dataServiceUrl = "/_bitplate/Pages/PageService.aspx";
            BITAJAX.callWebService("SavePage", jsonstring, function (data) {
                $('#bitPageDialog').dialog('close');
                BITEDITPAGE.page = data.d;
                //reload page
                BITEDITPAGE.loadPage(BITEDITPAGE.pageId);

            });
        }
    },

    ////////////////////////
    // MODULES
    ///////////////////////
    fillModulesCheckBoxList: function (pageid) {
        var parametersObject = { pageid: pageid };
        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
        BITAJAX.callWebServiceASync("GetModulesLiteByPageID", jsonstring, function (data) {
            $("#checkboxListModules_1").fillCheckboxList(data.d);
            $("#checkboxListModules_2").fillCheckboxList(data.d);
        });
    },

    //from addNewModule
    addModuleScriptsToHead: function (moduleScripts) {
        var head = document.getElementsByTagName('head')[0];
        for (var i in moduleScripts) {
            var script = moduleScripts[i];
            var jsscript = document.createElement('script');
            jsscript.type = 'text/javascript';
            var timeticks = new Date().getTime();
            jsscript.src = script + '?time=' + timeticks;
            //jsscript.id = script.ID;
            head.appendChild(jsscript);
        }
    },

    moveModule: function (id) {
        var moduleMoveGrip = $("<div/>");
        var w = $("#iframe").contents().find('#bitModule' + id).width(), h = $("#iframe").contents().find('#bitModule' + id).height()
        $(moduleMoveGrip).css({ 'width': w, 'height': h, 'display': 'block', 'position': 'absolute', 'cursor': 'move', 'z-index': '1000' });
        moduleMoveGrip.attr("class", "moduleMoveGrip");
        $("#iframe").contents().find('#bitModule' + id).prepend(moduleMoveGrip);

        //BITEDITPAGE.attachDraggable();
    },

    calulateOrderingNumber: function (module) {
        //console.log(module);
        var parentContainer = $(module).parent();
        var containerName = $(parentContainer).attr('id');
        if (containerName) {
            var moduleOrderingObject = { ContainerName: containerName, Modules: Array() }
            $(parentContainer).children().each(function (i) {
                var moduleId = $(this).attr('id').replace('bitModule', '');
                var module = { ID: moduleId, OrderingNumber: i };
                moduleOrderingObject.Modules.push(module);
            });
            var jsonstring = JSON.fromObject(moduleOrderingObject);
            BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
            BITAJAX.callWebService("UpdateModuleOrderingNumbers", jsonstring, function (data) {

            });
            BITEDITPAGE.hideModuleInformationOnHover();
        }
    },

    attachModuleCommandsById: function (moduleId) {

        //hover
        $("#iframe").contents().find("#bitModule" + moduleId).hover(
            function () {

                BITEDITPAGE.showModuleInformationOnHover(moduleId);
            },
            function () {

                BITEDITPAGE.hideModuleInformationOnHover(moduleId);
            });
        //contextmenu
        var menuPerModule = $("#iframe").contents().find("#bitModule" + moduleId).find('.bitModuleContextMenu');
        $("#iframe").contents().find("#bitModule" + moduleId).contextMenu($('.bitModuleContextMenu')[0], {
            onload: function (context, menu) {

                var moduleName = $(context).attr('data-module-title');
                if (!BITEDITPAGE.contextMenuTemplate) {
                    BITEDITPAGE.contextMenuTemplate = $(menu.element).html();
                }
                var currentMenuTemplate = BITEDITPAGE.contextMenuTemplate.replace(/\{0\}/g, moduleId).replace(/\{2\}/g, moduleName);
                $(menu.element).html(currentMenuTemplate);
                menu.visable = ($(module).data('editMode')) ? false : true;
                ////console.log(menu);
                return menu;
            }
        });
        //dbl click
        $("#iframe").contents().find("#bitModule" + moduleId).unbind().bind("dblclick", function () {
            //var moduleId = $(this).attr('id').replace('bitModule', '');
            //BITEDITPAGE.openEditModulePopup(moduleId);
            //BITEDITPAGE.initializeCkModuleEditor(moduleId);
            BITEDITPAGE.initializeModuleEditor(moduleId);
        });
    },

    attachModuleCommands: function () {
        $("#iframe").contents().find('.bitModule').each(function () {

            //var moduleId = $(this).attr('id').replace('bitModule', '');
            //BITEDITPAGE.attachModuleCommandsById(moduleId);
            //$(this).tooltip({ content: 'test', track: true });
            $(this).unbind().bind("dblclick", function () {
                var moduleId = $(this).attr('id').replace('bitModule', '');
                //BITEDITPAGE.openEditModulePopup(moduleId);
                var editorChoice = $(this).attr('data-module-editor');
                if (editorChoice === 'CKEDITOR') {
                    //inline CKEDITOR
                    BITEDITPAGE.initializeCkModuleEditor(moduleId);
                }
                else {
                    //CODEMIRROR popup 
                    BITEDITPAGE.openEditModulePopup(moduleId);
                }
            });

            $(this).hover(function () {
                var moduleId = $(this).attr('id').replace('bitModule', '');
                BITEDITPAGE.showModuleInformationOnHover(moduleId);
            },
            function () {
                var moduleId = $(this).attr('id').replace('bitModule', '');
                BITEDITPAGE.hideModuleInformationOnHover(moduleId);
            }
            );

            var menuPerModule = $(this).find('.bitModuleContextMenu');
            $(this).contextMenu($('.bitModuleContextMenu')[0], {
                onload: function (context, menu) {
                    var moduleId = $(context).attr('id').replace('bitModule', '');
                    var moduleName = $(context).attr('data-module-title');
                    if (!BITEDITPAGE.contextMenuTemplate) {
                        BITEDITPAGE.contextMenuTemplate = $(menu.element).html();
                    }
                    var currentMenuTemplate = BITEDITPAGE.contextMenuTemplate.replace(/\{0\}/g, moduleId).replace(/\{2\}/g, moduleName);
                    $(menu.element).html(currentMenuTemplate);
                    menu.visable = ($('#bitModule' + moduleId).data('editMode')) ? false : true;
                    ////console.log(menu);
                    return menu;
                }
            });
            var moduleId = $(this).find('.moduleId').val();



        });
    },

    showModuleInformationOnHover: function (id) {
        //var moduleInformationDiv = $("<div/>");// $('#bitModule' + id).find('.moduleMoveGrip');
        var position = $("#iframe").contents().find('#bitModule' + id).offset();
        var height = $("#iframe").contents().find('#bitModule' + id).height();
        var width = $("#iframe").contents().find('#bitModule' + id).width();
        var moduleType = $("#iframe").contents().find('#bitModule' + id).attr('data-module-type');
        var moduleName = $("#iframe").contents().find('#bitModule' + id).attr('data-module-name');
        var moduleInformationDiv = $('#moduleInformationDiv');
        $(moduleInformationDiv).html(moduleType + ' | ' + moduleName);
        //$(moduleInformationDiv).attr('id', 'ModuleInfo' + id);
        $(moduleInformationDiv).css({ 'top': position.top + height + $('#moduleInformationDiv').height() + 13, 'left': position.left + width - $('#moduleInformationDiv').width() });
        $(moduleInformationDiv).show();
    },

    hideModuleInformationOnHover: function (id) {
        $('#moduleInformationDiv').hide();
        //$("#iframe").contents().find('#ModuleInfo' + id).remove();
    },

    initializeCkModuleEditor: function (moduleId) {
        var editorId = 'bitModule' + moduleId;
        BITEDITPAGE.hideModuleInformationOnHover(moduleId);
        BITEDITPAGE.currentModuleContent[moduleId] = $("#iframe").contents().find('#' + editorId).html();
        //Is there an active editor?
        if (BITEDITPAGE.activeInlineModuleEditor) {
            if (BITEDITPAGE.activeInlineModuleEditor.element.getId() != BITEDITPAGE.activeInlineEditorId) {
                //The current editor is not our editor so destroy the current editor.
                BITEDITPAGE.activeInlineModuleEditor.destroy();
            }
        }

        //Store orignal content in variable 
        BITEDITPAGE.activeInlineEditorOrginalContent = $(editorId).html();
        //Remove LabelMsg if exist in content. BUG FIX
        $(editorId).find('.bitModuleMessage').remove();

        // Find the element that we want to
        // attach the editor to
        if (!(BITEDITPAGE.activeInlineEditorElement = $("#iframe").contents().find('#' + editorId))) {
            return;
        }

        //unbind module context menu.
        $(BITEDITPAGE.activeInlineEditorElement).unbind();
        $(BITEDITPAGE.activeInlineEditorElement).parent().enableSelection();

        // Make the element editable
        $(BITEDITPAGE.activeInlineEditorElement).attr('contenteditable', 'true');

        // Now set the focus to our editor so
        // that it will open up for business


        // Create a new inline editor for this div
        BITEDITPAGE.activeInlineModuleEditor = CKEDITOR.inline($("#iframe").contents().find('#' + editorId)[0], {
            enterMode: CKEDITOR.ENTER_BR,
            extraPlugins: 'tags',
            filebrowserBrowseUrl: '/_bitplate/Dialogs/Finder.aspx',
            filebrowserImageBrowseUrl: '/_bitplate/Dialogs/Finder.aspx',
            filebrowserWindowWidth: 1000,
            filebrowserWindowHeight: 600,
            toolbar: BITEDITPAGE.ckeditorToolbartoolbar

        });
        BITEDITPAGE.activeInlineEditorId = editorId;

        BITEDITPAGE.activeInlineModuleEditor.on('instanceReady', function () {
            if ($("#iframe").contents().find('#' + editorId).attr('data-module-type') != 'HtmlModule') {
                var parametersObject = { id: moduleId, mode: 'edit' };
                var jsonstring = JSON.stringify(parametersObject);
                BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
                BITAJAX.callWebService("GetModuleContent", jsonstring, function (data) {
                    BITEDITPAGE.activeInlineModuleEditor.setData(data.d);
                    BITEDITPAGE.currentModule = new Object();
                    BITEDITPAGE.currentModule.ID = moduleId;


                    //get tags
                    var parametersObject = { id: moduleId };
                    var jsonstring = JSON.stringify(parametersObject);
                    BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
                    BITAJAX.callWebService("GetTags", jsonstring, function (data) {
                        BITEDITPAGE.currentModuleTags = data.d;
                        setTags(BITEDITPAGE.currentModuleTags);
                    });
                });
            }
            //BITEDITPAGE.activeInlineEditorOrginalContent = this.getData();
            BITEDITPAGE.activeInlineEditorElement.focus();
        });


        // Set up a destruction function that will occur
        // when the user clicks out of the editable space
        BITEDITPAGE.activeInlineModuleEditor.on('blur', function () {
            this.element.setAttribute('contenteditable', 'false');
            $(BITEDITPAGE.activeInlineEditorElement).parent().disableSelection();
            BITEDITPAGE.activeInlineEditorId = '';
            BITEDITPAGE.activeInlineModuleEditor = 0;
            BITEDITPAGE.activeInlineEditorElement = null;
            var content = this.getData();
            this.destroy();
            if (content != BITEDITPAGE.activeInlineEditorOrginalContent) {
                $.confirmBox('Wilt u de gemaakte wijzigen toepassen?', 'Wijzigen toepassen?', function () {
                    BITEDITPAGE.saveModuleContent(moduleId, content);
                },
                function () {
                    $('#' + editorId).html(BITEDITPAGE.currentModuleContent[moduleId]);
                    BITEDITPAGE.attachModuleCommands();
                    //BITEDITPAGE.setAllSiteLinksToEditMode();
                    //BITEDITPAGE.saveModuleContent(moduleId, BITEDITPAGE.activeInlineEditorOrginalContent);
                });
            }
            else {
                $("#iframe").contents().find('#' + editorId).html(BITEDITPAGE.currentModuleContent[moduleId]);
                BITEDITPAGE.attachModuleCommands();
                //BITEDITPAGE.setAllSiteLinksToEditMode();
                //BITEDITPAGE.saveModuleContent(moduleId, BITEDITPAGE.activeInlineEditorOrginalContent);
            }
        });


        /* var moduleType = $('#bitModuleWrapper' + moduleId).find("#hiddenModuleType" + moduleId).val();
        if (!moduleType) {
            $("#hiddenModuleTemp").find('#bitModuleHiddenTemp' + moduleId).remove();
            $("#hiddenModuleTemp").append('<div id="bitModuleHiddenTemp' + moduleId + '"></div>');
            $('#bitModuleWrapper' + moduleId).find('.bitEditor').find('input[type="hidden"]').appendTo('#bitModuleHiddenTemp' + moduleId);
            $('#bitModuleWrapper' + moduleId).find('.bitEditor').find('input[type="hidden"]').remove();
            var editor = CKEDITOR.inline(editorId, {
                on:
                {
                    blur: function (e) {
                        if (e.editor.checkDirty()) {
                            if (BITAJAX.isBuzzy) { //Is BitAjax nog bezig? Ja koppel aan event AjaxReady / Nee voer functie direct uit.
                                $(document).bind('AjaxReady', closeCkEditor);
                            }
                            else {
                                closeCkEditor();
                            }
                            function closeCkEditor() { //FIX: het laden van module html moetvoltooid zijn.
                                if (BITEDITPAGE.currentModule != null) {
                                    if (BITEDITPAGE.currentModule.ModuleContent.trim().replace(/\n/g, '') != e.editor.getData().trim().replace(/\n/g, '')) {
                                        $.confirmBox('Wilt u de wijzigingen opslaan?', 'Module wijzigingen opslaan?', function () {
                                            BITEDITPAGE.saveModuleEditMode(moduleId, $('#bitModuleHiddenTemp' + moduleId).find('input[type="hidden"]').html() + e.editor.getData());
                                        },
                                        function () {
                                            $('#bitEditor' + BITEDITPAGE.currentModule.ID).html(BITEDITPAGE.currentModule.ModuleContent);
                                            BITEDITPAGE.currentModule = null;
                                            $('.bitModuleWrapper').data('editMode', false);
                                        });
                                    }
                                    else {
                                        BITEDITPAGE.currentModule = null;
                                        $('.bitModuleWrapper').data('editMode', false);
                                    }
                                    //$('#' + CKEDITOR.editors['inline'].name).css('position', 'initial');
                                }
                                $(document).unbind('AjaxReady');
                            }
                        }
                    }
                }
            });
            editor.config.extraPlugins = 'tags';
            editor.config.enterMode = CKEDITOR.ENTER_BR;
        } */
    },

    /* SetRenderdModuleContent: function (moduleId) {
        var parametersObject = { moduleId: moduleId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
        BITAJAX.callWebService("RenderModule", jsonstring, function (data) {
    
        });
    }, */

    openEditModulePopup: function (id) {
        //Als er ckeditor bestaat van andere module vernietig het.
        if (CKEDITOR.instances.EditorInPopup) {
            CKEDITOR.instances.EditorInPopup.destroy();
        }
        else if (BITEDITPAGE.EditorInPopup) {
            BITEDITPAGE.EditorInPopup.toTextArea();
        }
        var parametersObject = { id: id, mode: 'edit' };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
        BITAJAX.callWebService("GetModuleContent", jsonstring, function (data) {
            $('#bitEditModuleDialog').dialog("open");
            $('#EditorInPopup').val(data.d);
            BITEDITPAGE.initializeModuleEditor(id);
            //CKEDITOR.instances.EditorInPopup.setData(data.d);
            BITEDITPAGE.currentModule = new Object();
            BITEDITPAGE.currentModule.ID = id;


            //get tags
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
            BITAJAX.callWebService("GetTags", jsonstring, function (data) {
                BITEDITPAGE.currentModuleTags = data.d;
                if (CKEDITOR.instances.EditorInPopup) {
                    setTags(BITEDITPAGE.currentModuleTags.CKTags);
                }
                else {
                    $("#bitTagsDialog").html(data.d.CKTags);
                }
            });
        });
    },

    saveModuleContentFromPopup: function () {
        var id = BITEDITPAGE.currentModule.ID;
        var content;
        if (CKEDITOR.instances.EditorInPopup) {
            content = CKEDITOR.instances.EditorInPopup.getData();

        }
        else {
            content = BITEDITPAGE.EditorInPopup.mirror.getValue();
        }
        //var content = CKEDITOR.instances.EditorInPopup.getData();
        BITEDITPAGE.saveModuleContent(id, content);
        $('#bitEditModuleDialog').dialog("close");
    },

    saveModuleEditMode: function (id, content) {

        BITEDITPAGE.currentModule = null;
        BITEDITPAGE.saveModuleContent(id, content);
    },

    saveModuleContent: function (id, content) {
        BITEDITPAGE.createModuleBuzyOverlay(id);
        var guid = id;
        var parametersObject = { id: id, content: content, languageCode: BITEDITPAGE.language, pageid: BITEDITPAGE.pageId, newsletterid: BITEDITPAGE.newsletterId, };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
        BITAJAX.callWebServiceASync("SaveModuleContent", jsonstring, function (data) {

            $('#iframe').contents().find('#bitModule' + guid).replaceWith(data.d);
            var moduleType = $("#hiddenModuleType" + guid).val();
            BITEDITPAGE.attachModuleCommands();
            //twee scripts uit BitSiteScript.js aanroepen 
            window.frames["iframe"].BITSITESCRIPT.replaceLinksWithAjax(guid);
            window.frames["iframe"].BITSITESCRIPT.initializePostableForms();

            //BITEDITPAGE.setAllSiteLinksToEditMode();
            for (var i in BITEDITPAGE.moduleConfigJsClasses) {
                var jsClass = BITEDITPAGE.moduleConfigJsClasses[i];
                if (jsClass) {
                    if (typeof jsClass.onModuleSaved == 'function') {
                        jsClass.onModuleSaved();
                    }
                }
            }
        },
        function () {
            BITEDITPAGE.attachModuleCommands();
            //BITEDITPAGE.setAllSiteLinksToEditMode();
        });
    },
    createModuleBuzyOverlay: function (moduleID) {
        var height = $('#bitModule' + moduleID).height();
        var width = $('#bitModule' + moduleID).width();
        $('#iframe').contents().find('#bitModule' + moduleID).prepend('<div style="height: ' + height + 'px ; width:' + width + 'px; z-index: 999; position: absolute;" class="moduleLoader"><div class="moduleLoaderImg"></div></div>');
    },
    deleteCurrentModule: function () {
        var id = '';
        BITEDITPAGE.deleteModule(id);
    },

    deleteModule: function (id) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u deze module verwijderen?", null, function () {

            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
            BITAJAX.callWebService("DeleteModule", jsonstring, function () {
                BITEDITPAGE.fillModulesCheckBoxList(BITEDITPAGE.pageId);
            });
            $('#iframe').contents().find('#bitModule' + id).remove();

        });
    },


    configModule: function (id, type) {
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
        BITAJAX.callWebService("GetModule", jsonstring, function (data) {
            BITEDITPAGE.currentModule = data.d;
            var settingsJsonString = data.d.SettingsJsonString;
            var settingsObj = $.parseJSON(settingsJsonString);
            BITEDITPAGE.currentModule.Settings = settingsObj;

            $('#bitConfigModuleDialog').html("");
            if (!BITEDITPAGE.currentModule.ConfigPageUrl) {
                BITEDITPAGE.currentModule.ConfigPageUrl = "/_bitplate/EditPage/ModuleConfig/ModuleConfig.aspx";
            }
            var timestamp = new Date().getTime();

            var configUrl = BITEDITPAGE.currentModule.ConfigPageUrl + "?moduleid=" + id + "&pageid=" + BITEDITPAGE.pageId + "&timestamp=" + timestamp;
            if (BITEDITPAGE.isNewsletterMode()) {
                configUrl = BITEDITPAGE.currentModule.ConfigPageUrl + "?moduleid=" + id + "&newsletterid=" + BITEDITPAGE.newsletterId + "&timestamp=" + timestamp;
            }
            $('#bitConfigModuleDialog').load(configUrl, function () {
                if (BITEDITPAGE.page) {
                    $('[name="VisibleMode"]').change(function () {
                        if ($('#checkVisibleCustomPages').is(':checked')) {
                            $('#PageSelector').show();
                        }
                        else {
                            $('#PageSelector').hide();
                        }
                    });
                }
                else {
                    $('#PageSelector').hide();
                    $('[for="checkVisibleCustomPages"]').html('Alleen zichtbaar op deze nieuwsbrief.');
                }
                
                
                $('#bitConfigModuleDialog').dialog({ title: "Module: " + BITEDITPAGE.currentModule.Name });
                $('#bitConfigModuleDialog').dialog('open');

                //autorisatie tab toevoegen
                //$("#bitConfigModuleDialog").tabs({ "cache": true });
                //if ($("#bitConfigModuleDialog").hasClass('ui-tabs-nav')) {
                //    $("#bitConfigModuleDialog").tabs('destroy');
                //}
                //var tabs = $("#bitConfigModuleDialog").tabs();
                ////$("#bitConfigModuleDialog").tabs("add", "/_bitPlate/Dialogs/AutorisationTab.aspx", "Autorisatie");
                //var ul = tabs.find("ul");
                //$("<li><a href=\"/_bitPlate/Dialogs/AutorisationTab.aspx\">Autorisatie</a></li>").appendTo(ul);
                //tabs.tabs("refresh");

                $("#bitConfigModuleDialog").tabs({ "cache": true });
                $("#bitConfigModuleDialog").tabs("add", "/_bitplate/Dialogs/AutorisationTab.aspx", "Autorisatie");

                $("#bitConfigModuleDialog").bind("tabsload", function (event, ui) {
                    BITAUTORISATIONTAB.currentObject = BITEDITPAGE.currentModule;
                    BITAUTORISATIONTAB.initialize();
                    BITAUTORISATIONTAB.dataBind();
                    //BITTEMPLATES.authTabIsLoaded = true;
                });

                for (var i in BITEDITPAGE.moduleConfigJsClasses) {
                    var jsClass = BITEDITPAGE.moduleConfigJsClasses[i];
                    if (jsClass) {
                        if (typeof jsClass.initialize == 'function') {
                            jsClass.initialize();
                        }
                    }
                }

                $('#bitConfigModuleDialog').dataBind(BITEDITPAGE.currentModule);

                for (var i in BITEDITPAGE.moduleConfigJsClasses) {
                    var jsClass = BITEDITPAGE.moduleConfigJsClasses[i];
                    if (jsClass) {
                        if (typeof jsClass.dataBind == 'function') {
                            jsClass.dataBind();
                        }
                    }
                }
                $('#modulePerPageSelect').chosen();
            });
        });
    },

    //configModule_old: function (id, type) {
    //    var parametersObject = { id: id };
    //    var jsonstring = JSON.stringify(parametersObject);
    //    BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
    //    BITAJAX.callWebService("GetModule", jsonstring, function (data) {
    //        BITEDITPAGE.currentModule = data.d;
    //        ////console.log(data.d);
    //        //in module worden settings als json string opgeslagen in db (zodat verschillende settings bij verschillende modules in zelfde tabel kunnen worden opgeslagen)
    //        //hier zetten we die string om in een object en koppelen dat object aan de module
    //        //hierdoor kun je databind gebruiken
    //        var settingsJsonString = data.d.SettingsJsonString;
    //        var settingsObj = $.parseJSON(settingsJsonString);
    //        BITEDITPAGE.currentModule.Settings = settingsObj;

    //        $('#bitConfigModuleDialog').html("");
    //        var configUrl = data.d.ConfigPageUrl;
    //        if (configUrl) {
    //            if (!configUrl.indexOf("_bitModules/" + data.d.Type) >= 0) {
    //                configUrl = "_bitModules/" + data.d.Type + "/" + configUrl;
    //            }
    //            //externe config page laden, hierin de eventuele standaard tabs laden (navigatie en data) 
    //            $('#bitConfigModuleDialog').load(configUrl, function () {

    //                $('#bitConfigModuleDialog').dialog({ title: "Module: " + BITEDITPAGE.currentModule.Name });
    //                $('#bitConfigModuleDialog').dialog('open');
    //                $('#bitConfigModuleDialog').tabs();
    //                $('#bitConfigModuleDialog').append("<div id='tabPageGeneral'>" + $("#standardTabPageGeneral").html() + "</div>");
    //                $('#bitConfigModuleDialog').tabs("add", "#tabPageGeneral", "Algemene instellingen", 0);
    //                //eerst verbergen
    //                $("#standardTabPageData_ListSettings").hide();
    //                //standaard tabs toevoegen/tonen
    //                for (var i in data.d.ConfigPageStandardElements) {
    //                    var elm = data.d.ConfigPageStandardElements[i];
    //                    if (elm == "#standardTabPageNavigation") {
    //                        $('#bitConfigModuleDialog').append("<div id='tabPageNavigation'>" + $("#standardTabPageNavigation").html() + "</div>");
    //                        $('#bitConfigModuleDialog').tabs("add", "#tabPageNavigation", "Navigatie", 1);
    //                        BITEDITPAGE.showNavigationTypeDiv(BITEDITPAGE.currentModule.NavigationType, 1);
    //                        BITEDITPAGE.showNavigationTypeDiv(BITEDITPAGE.currentModule.NavigationType2, 2);
    //                    }
    //                    else if (elm == "#standardTabPageData") {
    //                        $('#bitConfigModuleDialog').append("<div id='tabPageData'>" + $("#standardTabPageData").html() + "</div>");
    //                        $('#bitConfigModuleDialog').tabs("add", "#tabPageData", "Data", 1);

    //                    }
    //                    else if (elm == "#standardTabPageData_ListSettings") {
    //                        //onderdeel van tabpage: data

    //                        //BITEDITPAGE.changeDataCollection($('#SelectDataCollections'));
    //                        //var datacollectionid = "";
    //                        //if (BITEDITPAGE.currentModule.DataCollection) {
    //                        //    datacollectionid = BITEDITPAGE.currentModule.DataCollection.ID;
    //                        //    BITEDITPAGE.bindSortAndFilterFields(datacollectionid, BITEDITPAGE.currentModule);
    //                        //    BITEDITPAGE.selectDataCollection(datacollectionid);
    //                        //}
    //                        //else {

    //                        //}
    //                        if (BITEDITPAGE.currentModule.Type == "ItemListModule") {
    //                            $("#selectShowData").empty();
    //                            $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.USERSELECT).html("Gebruikersselectie afhankelijk"));
    //                            $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.ALLITEMS).html("Alle items"));
    //                        }
    //                        else if (BITEDITPAGE.currentModule.Type == "GroupListModule") {
    //                            $("#selectShowData").empty();
    //                            $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.USERSELECT).html("Gebruikersselectie afhankelijk"));
    //                            $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.MAINGROUPS).html("Alleen hoofdgroepen"));
    //                            $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.ALLGROUPS).html("Alle groepen"));
    //                        }

    //                        $(elm).show();
    //                    }
    //                    else if (elm == "#standardTabPageData_DetailsSettings") {
    //                        //onderdeel van tabpage: data
    //                        //BITEDITPAGE.changeDataCollection($('#SelectDataCollections'));
    //                        //var datacollectionid = "";
    //                        //if (BITEDITPAGE.currentModule.DataCollection) {
    //                        //    datacollectionid = BITEDITPAGE.currentModule.DataCollection.ID;
    //                        //    BITEDITPAGE.bindDefaultDataElements(datacollectionid, BITEDITPAGE.currentModule);
    //                        //    BITEDITPAGE.selectDataCollection(datacollectionid);
    //                        //}
    //                        //else {
    //                        //    $("#selectDefaultDataElement").empty();
    //                        //}
    //                        $(elm).show();
    //                    }
    //                    else if (elm == "#standardTabPageListSettings") {
    //                        $('#bitConfigModuleDialog').append("<div id='tabPageListSettings'>" + $("#standardTabPageListSettings").html() + "</div>");
    //                        $('#bitConfigModuleDialog').tabs("add", "#tabPageListSettings", "Lijst instellingen", 1);
    //                    }

    //                    $(elm).show();
    //                }

    //                //autorisatie tab toevoegen
    //                $("#bitConfigModuleDialog").tabs({ "cache": true });
    //                $("#bitConfigModuleDialog").tabs("add", "/_bitPlate/Dialogs/AutorisationTab.aspx", "Autorisatie");
    //                $("#bitConfigModuleDialog").bind("tabsload", function (event, ui) {
    //                    BITAUTORISATIONTAB.currentObject = BITEDITPAGE.currentModule;
    //                    BITAUTORISATIONTAB.initialize();
    //                    BITAUTORISATIONTAB.dataBind();
    //                    //BITTEMPLATES.authTabIsLoaded = true;
    //                });
    //                //databind
    //                $('#bitConfigModuleDialog').dataBind(BITEDITPAGE.currentModule);
    //                BITEDITPAGE.changeDataCollection($('#SelectDataCollections'));

    //                $(document).trigger('onConfigLoaded');
    //                //eerste tab focus geven
    //                $('#bitConfigModuleDialog').tabs('select', 0);
    //            });
    //        }
    //        else {
    //            //geen externe configmodulepage gedefinieerd in de xml: de interne gebruiken
    //            $('#bitConfigModuleStandardDialog').dialog({ title: "Module: " + BITEDITPAGE.currentModule.Name });
    //            $('#bitConfigModuleStandardDialog').dialog('open');
    //            $('#bitConfigModuleStandardDialog').tabs();
    //            //eerst alle verbergen
    //            $("#bitConfigModuleStandardDialog ul li").hide();
    //            $("#standardTabPageData_ListSettings").hide();
    //            //algemeen: altijd tonen
    //            $('#tabPage1').show();
    //            $('#standardTabPageGeneral').show();
    //            for (var i in data.d.ConfigPageStandardElements) {
    //                var elm = data.d.ConfigPageStandardElements[i];
    //                if (elm == "#standardTabPageNavigation") {
    //                    $("#standardTabPageNavigation").html('');
    //                    $("#standardTabPageNavigation").load('/_bitplate/Editpage/ModuleConfig/NavigationActionTab/ConfigTabActions.aspx?pageid=' + BITEDITPAGE.pageId, function () {
    //                        $('#bitConfigModuleStandardDialog').tabs("add", "#standardTabPageNavigation", "Navigatie", 1);
    //                        var template = $("#divNavigationActions").html();
    //                        $("#divNavigationActions").html('')
    //                        $(data.d.NavigationActions).each(function (i) {
    //                            var tempTemplate = $('<div></div>').append(template);
    //                            var NavigationAction = this;
    //                            $(tempTemplate).find('#navigationTagName_0').attr('id', 'navigationTagName_' + i).html(this.Name);
    //                            $(tempTemplate).find('#fieldsetNavigation_0').attr('id', 'fieldsetNavigation_' + i);
    //                            $(tempTemplate).find('#selectNavigationType_0').attr('id', 'selectNavigationType_' + i).attr('data-field', 'NavigationActions[' + i + '].NavigationType').attr('onchange', 'javascript: BITEDITPAGE.changeNavigationType(this, ' + i + ');').val(this.NavigationType);
    //                            var PageId = (this.NavigationPage != null) ? this.NavigationPage.ID : null;
    //                            $(tempTemplate).find('#SelectNavigationPage_0').attr('id', 'SelectNavigationPage_' + i).attr('data-field', 'NavigationActions[' + i + '].NavigationPage.ID').val(PageId);


    //                            $(tempTemplate).find('#checkboxListModules_0').attr('id', 'checkboxListModules_' + i).attr('data-field', 'NavigationActions[' + i + '].RefreshModules');
    //                            $(tempTemplate).find('#checkboxListModules_' + i + ' input').each(function () {
    //                                //console.log($.inArray($(this).val(), NavigationAction.RefreshModules));
    //                                if ($.inArray($(this).val(), NavigationAction.RefreshModules) != -1) {
    //                                    $(this).attr('checked', 'checked');
    //                                }
    //                            });

    //                            $(tempTemplate).find('#javascriptModules_0').attr('id', 'javascriptModules_' + i).val(this.JsFunction);


    //                            $(tempTemplate).find('[id^=divNavigationType]').each(function () {
    //                                var id = $(this).attr('id');
    //                                id = id.substring(0, id.length -1);
    //                                $(this).attr('id', id + i);
    //                            });
    //                            $("#divNavigationActions").append(tempTemplate);
    //                            BITEDITPAGE.changeNavigationType($('#selectNavigationType_' + i)[0], i);
    //                        });
    //                    });

    //                    /* $('#tabPage2').show();
    //                    BITEDITPAGE.showNavigationTypeDiv(BITEDITPAGE.currentModule.NavigationType, 1);
    //                    BITEDITPAGE.showNavigationTypeDiv(BITEDITPAGE.currentModule.NavigationType2, 2);
    //                    if (!BITEDITPAGE.currentModule.NavigationTagName2) {
    //                        $("#fieldsetNavigation_2").hide();
    //                    } */
    //                    //BITEDITPAGE.bindNavigationActions();
    //                    //BITEDITPAGE.showNavigationTypeDiv(BITEDITPAGE.currentModule.NavigationType);

    //                }
    //                else if (elm == "#standardTabPageData") {
    //                    $('#tabPage3').show();

    //                }
    //                else if (elm == "#standardTabPageData_ListSettings") {
    //                    if ($("#selectShowData").val() == SHOWDATATYPES.USERSELECT) {
    //                        $('#dataCollectionPathBrowse').show();
    //                        $('#dataCollectionPathName').show();
    //                    }
    //                    else {
    //                        $('#dataCollectionPathBrowse').hide();
    //                        $('#dataCollectionPathName').hide();
    //                    }

    //                    if (BITEDITPAGE.currentModule.Type == "ItemListModule") {
    //                        $("#selectShowData").empty();
    //                        $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.USERSELECT).html("Gebruikersselectie afhankelijk"));
    //                        $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.ALLITEMS).html("Alle items"));
    //                    }
    //                    else if (BITEDITPAGE.currentModule.Type == "GroupListModule") {
    //                        $("#selectShowData").empty();
    //                        $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.USERSELECT).html("Gebruikersselectie afhankelijk"));
    //                        $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.MAINGROUPS).html("Alleen hoofdgroepen"));
    //                        $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.ALLGROUPS).html("Alle groepen"));
    //                    }
    //                    $(elm).show();
    //                }
    //                else if (elm == "#standardTabPageData_DetailsSettings") {
    //                    //onderdeel van tabpage: data
    //                    var datacollectionid = "";
    //                    if (BITEDITPAGE.currentModule.DataCollection) {
    //                        datacollectionid = BITEDITPAGE.currentModule.DataCollection.ID;
    //                        BITEDITPAGE.bindDefaultDataElements(datacollectionid, BITEDITPAGE.currentModule);
    //                        //BITEDITPAGE.selectDataCollection(datacollectionid);
    //                    }
    //                    else {
    //                        $("#selectDefaultDataElement").empty();
    //                    }
    //                    $(elm).show();
    //                }
    //                else if (elm == "#standardTabPageListSettings") {
    //                    $('#tabPage4').show();
    //                }

    //                $(elm).show();
    //            }
    //            //autorisatie tab toevoegen
    //            $("#bitConfigModuleStandardDialog").tabs({ "cache": true });
    //            $("#bitConfigModuleStandardDialog").bind("tabsload", function (event, ui) {
    //                BITAUTORISATIONTAB.currentObject = BITEDITPAGE.currentModule;
    //                BITAUTORISATIONTAB.initialize();
    //                BITAUTORISATIONTAB.dataBind();
    //                //BITTEMPLATES.authTabIsLoaded = true;
    //            });
    //            $("#bitConfigModuleStandardDialog").tabs("add", "/_bitplate/Dialogs/AutorisationTab.aspx", "Autorisatie");

    //            //databind
    //            $('#bitConfigModuleStandardDialog').dataBind(BITEDITPAGE.currentModule);
    //            //$('#divNavigationActions').dataBindList(BITEDITPAGE.currentModule.NavigationActions);
    //            BITEDITPAGE.changeDataCollection($('#SelectDataCollections'));

    //            $(document).trigger('onConfigLoaded');
    //            //eerste tab focus geven
    //            $('#bitConfigModuleStandardDialog').tabs('select', 0);
    //        }
    //    });
    //},



    //bindModulesCheckBoxList: function () {
    //    var mod = BITEDITPAGE.currentModule;
    //    $("#checkboxListModules").find("input[type=checkbox]").removeAttr("checked");
    //    if (mod.NavigationType == "") {
    //        var modules = mod.NavigationUrl.split('');
    //        for (var i in modules) {

    //        }
    //    }
    //},

    saveConfigModule: function (e) {
        BITEDITPAGE.createModuleBuzyOverlay(BITEDITPAGE.currentModule.ID);
        //$(document).trigger('onConfigSave');
        //Update all texteara values from CKEDITOR
        for (var instanceName in CKEDITOR.instances)
            CKEDITOR.instances[instanceName].updateElement();
        var validation = $('#bitConfigModuleDialog').validate();
        for (var i in BITEDITPAGE.moduleConfigJsClasses) {
            var jsClass = BITEDITPAGE.moduleConfigJsClasses[i];
            if (jsClass) {
                if (typeof jsClass.validate == 'function') {
                    var jsClassValidation = jsClass.validate();
                    if (validation) validation = jsClassValidation;
                }
            }
        }
        if (validation) {
            //get data from panel
            if ($('#bitConfigModuleDialog').dialog("isOpen")) {
                BITEDITPAGE.currentModule = $('#bitConfigModuleDialog').collectData(BITEDITPAGE.currentModule);
            }
            else {
                BITEDITPAGE.currentModule = $('#bitConfigModuleStandardDialog').collectData(BITEDITPAGE.currentModule);
            }

            for (var i in BITEDITPAGE.currentModule.NavigationActions) {
                var action = BITEDITPAGE.currentModule.NavigationActions[i];
                if (action.Name) {
                    //action = $('#fieldsetNavigation_'+i).collectData(action);
                    action.RefreshModules = new Array();
                    $('#checkboxListModules_' + i).find('input:checked').each(function () {
                        action.RefreshModules.push($(this).val());
                    });
                    action.NavigationType = $('#selectNavigationType_' + i).val();
                    action.NavigationPage = new Object();
                    action.NavigationPage.ID = $('#SelectNavigationPage_' + i).val();
                    action.JsFunction = $('#javascriptModules_' + i).val();
                    BITEDITPAGE.currentModule.NavigationActions[i] = action;
                }
            }

            BITEDITPAGE.currentModule.Pages = Array();
            //Voer dit niet uit bij een nieuwsbrief.
            if (BITEDITPAGE.page) {
                //'#bitNewsletterGroupSelect'
                $('#modulePerPageSelect option:selected').each(function () {
                    var id = $(this).attr('value');
                    var page = new Object();
                    page.ID = id;
                    BITEDITPAGE.currentModule.Pages.push(page);
                });
                //Een module moet minimaal aan 1 pagina direct zijn gekoppeld.
                if (BITEDITPAGE.currentModule.Pages.length == 0) {
                    BITEDITPAGE.currentModule.Pages.push(BITEDITPAGE.page);
                }
            }
            

            //BITEDITPAGE.currentModule.NavigationType = parseInt(BITEDITPAGE.currentModule.NavigationType);
            //gegevens uit checkbox list in variale zetten
            //BITEDITPAGE.currentModule.NavigationUrl = BITEDITPAGE.currentModule.RefreshModules.toString();
            //module.Page = BITEDITPAGE.page;
            //maak van module settings een string, deze string wordt in db opgeslagen
            //hierdoor zijn verschillende settingsmogelijk bij verschillende modules
            var settingsObject = BITEDITPAGE.currentModule.Settings;
            var settingsJsonString = JSON.stringify(settingsObject);
            BITEDITPAGE.currentModule.SettingsJsonString = settingsJsonString;
            BITEDITPAGE.currentModule.Settings = null;

            //BITEDITPAGE.currentModule.SelectGroup = JSON.stringify(BITEDITPAGE.currentModule.SelectGroup);
            var parametersObject = { module: BITEDITPAGE.currentModule, pageid: BITEDITPAGE.pageId, newsletterid: BITEDITPAGE.newsletterId, languageCode: BITEDITPAGE.language };
            var jsonstring = JSON.stringify(parametersObject);
            jsonstring = replaceDates(jsonstring);
            BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
            BITAJAX.callWebServiceASync("SaveModule", jsonstring, function (data) {
                //reload page
                var moduleId = BITEDITPAGE.currentModule.ID;
                $('#bitConfigModuleDialog').dialog("close");
                //$('#bitConfigModuleStandardDialog').dialog("close");

                $('#iframe').contents().find('#bitModule' + moduleId).replaceWith(data.d);

                //?? ter test: var html = $('#bitModule' + moduleId).html();


                BITEDITPAGE.attachModuleCommands();
                //twee scripts uit BitSiteScript aanroepen, zodat je meteen de werking krijgt 
                window.frames["iframe"].BITSITESCRIPT.replaceLinksWithAjax(moduleId);
                window.frames["iframe"].BITSITESCRIPT.initializePostableForms();


                for (var i in BITEDITPAGE.moduleConfigJsClasses) {
                    var jsClass = BITEDITPAGE.moduleConfigJsClasses[i];
                    if (jsClass) {
                        if (typeof jsClass.onModuleSaved == 'function') {
                            jsClass.onModuleSaved();
                        }
                    }
                }
            }, null, e.srcElement);
        }

    },

    ///////////////////////////////////////
    //SCRIPTS
    ///////////////////////////////////////
    fillMenuScripts: function () {
        //eerst leeg maken
        $("#ulMenuScripts").html('');
        $("#ulMenuStylesheets").html('');
        if (BITEDITPAGE.pageId) {
            var parametersObject = { pageid: BITEDITPAGE.pageId };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/Scripts/ScriptService.aspx";
            BITAJAX.callWebService("GetScriptsByPageId", jsonstring, function (data) {
                var scripts = data.d;
                BITEDITPAGE.appendScriptsToMenu(scripts);
            });
        }

        if (BITEDITPAGE.newsletterId) {
            var parametersObject = { newsletterid: BITEDITPAGE.newsletterId };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/Scripts/ScriptService.aspx";
            BITAJAX.callWebService("GetScriptsByNewsletterId", jsonstring, function (data) {
                var scripts = data.d;
                BITEDITPAGE.appendScriptsToMenu(scripts);
            });
        }
        
    },

    appendScriptsToMenu: function(scripts) {
        for (var i in scripts) {
            var script = scripts[i];
            if (script.ScriptType == 1) { //js
                $("#ulMenuScripts").append("<li><a href=\"javascript:BITEDITPAGE.editScript('" + script.ID + "');\">" + script.Name + "</a></li>");
            }
            else { //css
                $("#ulMenuStylesheets").append("<li><a href=\"javascript:BITEDITPAGE.editScript('" + script.ID + "');\">" + script.Name + "</a></li>");
            }
        }
    },

    editScript: function (id) {
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/Scripts/ScriptService.aspx";
        BITAJAX.callWebService("GetScript", jsonstring, function (data) {
            BITEDITPAGE.currentScript = data.d;
            if (BITEDITPAGE.codeMirrorScriptEditor != null) {
                BITEDITPAGE.codeMirrorScriptEditor.toTextArea();
            }
            //BITSCRIPTS.script = data.d;
            //$("#bitScriptDialog").dataBind(BITSCRIPTS.script);
            $('#bitScriptEditDialog').dataBind(BITEDITPAGE.currentScript);

            var scriptType = $('#selectScriptType').val();
            var uiOptions = { path: '/_bitplate/_js/plugins/codemirror-ui/js/', searchMode: 'inline', buttons: ['undo', 'redo', 'jump', 'reindentSelection', 'reindent'] };
            var codeMirrorOptions;
            if (data.d.ScriptType == 1) {
                codeMirrorOptions = { mode: "javascript", tabMode: "indent", lineNumbers: true };
                BITEDITPAGE.codeMirrorScriptEditor = new CodeMirrorUI(document.getElementById('textareaContent'), uiOptions, codeMirrorOptions); //CodeMirror.fromTextArea(document.getElementById('textareaContent'), { mode: "javascript", tabMode: "indent", lineNumbers: true });
            }
            else {
                codeMirrorOptions = { mode: "css", tabMode: "indent", lineNumbers: true };
                BITEDITPAGE.codeMirrorScriptEditor = new CodeMirrorUI(document.getElementById('textareaContent'), uiOptions, codeMirrorOptions); //CodeMirror.fromTextArea(document.getElementById('textareaContent'), { mode: "css", tabMode: "indent", lineNumbers: true });
            }

            $('#bitScriptEditDialog').dialog({ title: "Script: " + BITEDITPAGE.currentScript.Name });
            $('#bitScriptEditDialog').dialog('open');

            var height = $('#bitScriptEditDialog').height() - 30;
            $('#textareaContent').parent().height(height);
            BITEDITPAGE.codeMirrorScriptEditor.mirror.refresh();
            
        });
    },

    saveScript: function () {
        BITEDITPAGE.codeMirrorScriptEditor.toTextArea();
        BITEDITPAGE.codeMirrorScriptEditor = null;
        BITEDITPAGE.currentScript = $('#bitScriptEditDialog').collectData(BITEDITPAGE.currentScript);
        //module.Page = BITEDITPAGE.page;
        var jsonstring = convertToJsonString(BITEDITPAGE.currentScript);

        BITAJAX.dataServiceUrl = "/_bitplate/Scripts/ScriptService.aspx";
        BITAJAX.callWebService("SaveScript", jsonstring, function (data) {
            //reload page
            $('#bitScriptEditDialog').dialog("close");
            var scriptID = BITEDITPAGE.currentScript.ID;
            var scriptUrl = BITEDITPAGE.currentScript.Url;
            if (BITEDITPAGE.currentScript.Type == 'css') {
                var link = $("#iframe").contents().find("link[id='BitStyleScript" + scriptID.replace(/-/g, "") + "']");
                var timeticks = new Date().getTime();
                link.attr('href', scriptUrl + '?time=' + timeticks);
            }
            else {
                var jsscript = $("#iframe").contents().find('script[id=BitScript' + scriptID.replace(/-/g, "") + ']');
                var timeticks = new Date().getTime();
                jsscript.attr('src', '../' + scriptUrl + '?time=' + timeticks);
            }
        });

    },


    loadCodeMirrorTagPopup: function () {
        BITEDITPAGE.refreshTagsPopup(BITEDITPAGE.currentModule.ID);
        BITEDITPAGE.showTagsPopup();
    },

    showTagsPopup: function () {
        $("#bitTagsDialog").dialog("open");
        $('.tagToInsert').dblclick(function () {
            var tag = $(this).html();
           
            BITEDITPAGE.EditorInPopup.mirror.replaceSelection(tag, 'start');
            var currentCursorPosition = BITEDITPAGE.EditorInPopup.mirror.getCursor();
            BITEDITPAGE.EditorInPopup.mirror.setCursor(currentCursorPosition);
        });
        BITEDITPAGE.isTagsPopupOpen = true;
        /* if (BITEDITOR.currentEditor) {
            var id = BITEDITOR.currentEditor.id;
            BITEDITPAGE.refreshTagsPopup(id);
        } */

    },

    //refreshTagsPopup: function (id) {
    //    //als de tagspopup open staat, moeten de tags van bijbehorende module worden getoond
    //    if (BITEDITPAGE.isTagsPopupOpen) {

    //        var parametersObject = { id: id };
    //        var jsonstring = JSON.stringify(parametersObject);
    //        BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";
    //        BITAJAX.callWebService("GetTags", jsonstring, function (data) {
    //            $("#bitTagsDialog").html(data.d.CKTags);
    //            $('.tagToInsert').dblclick(function () {
    //                var tag = $(this).html();

    //            });
    //        });
    //    }
    //},

    

    registerModuleConfigJsClass: function (jsClass, name) {
        BITEDITPAGE.moduleConfigJsClasses[name] = jsClass;
    },


    ///////////////////////
    // NEWSLETTERS
    ///////////////////////
    loadNewsletter: function (newsletterid) {
        if (newsletterid == "") return;
        BITEDITPAGE.newsletterId = newsletterid;
        var url = "/Newsletter.aspx?newsletterid=" + newsletterid;
        $("#iframe").unbind('load'); //om te vookomen dat load vaker afgaat
        $("#iframe").attr("src", url);
        $("#iframe").load(function () {
            BITEDITPAGE.insertCssInIframe("/_bitPlate/_themes/bitplate/css/pageEdit.css");

            BITEDITPAGE.attachModuleCommands();
            BITEDITPAGE.attachDraggable();
            //BITEDITPAGE.fillModulesCheckBoxList(BITEDITPAGE.pageId);
            BITEDITPAGE.fillMenuScripts();
        });
    },

};


