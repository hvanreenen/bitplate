///// <reference path="../_js/jquery-1.7.2-vsdoc.js" />
///// <reference path="../../_js/prototypes/databind.js" />
///// <reference path="../prototypes/initDialog.js" />


/// <reference path="../_js/jquery-1.7.2-vsdoc.js" />
/// <reference path="../../_js/prototypes/databind.js" />
/// <reference path="../prototypes/initDialog.js" />



var BITPAGES = {
    type: null, //pages, of folders
    folders: null,
    templates: null,
    selectedFolderId: null,
    selectedPath: "",
    searchString: "",
    currentSort: "Name ASC",
    pagePopup: null,
    folderPopup: null,
    templatePopup: null,
    treeMode: false,
    page: null,
    folder: null,
    //folderCollection: Array(),
    authTabIsLoaded: false,

    initialize: function () {
        BITAJAX.isHistorySupported = true;
        BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
        
        $('#bitPageDialog').initDialog(BITPAGES.savePage, { width: '90%', height: window.innerHeight });
        //$('#bitPageDialog').initDialog(BITPAGES.savePage, { width: 1000, height: 700 });
        $('#bitFolderDialog').initDialog(BITPAGES.saveFolder, { width: '90%', height: window.innerHeight });
        $('#bitScriptDialog').initDialog(null, { width: 200, height: 400 }, false);
        $('#bitTemplateDialog').initDialog(null, { width: 600, height: 400 }, false);
        $('#bitUserGroupsDialog').initDialog(null, { width: 600, height: 400 }, false);
        $('#bitUsersDialog').initDialog(null, { width: 600, height: 400 }, false);
        $('#bitPopups').formEnrich();

        $("#bitTabsPages").tabs({ "cache": true });
        $("#bitTabsPages").tabs("add", "/_bitplate/Dialogs/AutorisationTab.aspx", "Autorisatie");
        $("#bitTabsPages").bind("tabsload", function (event, ui) {
            BITAUTORISATIONTAB.currentObject = BITPAGES.page;
            BITAUTORISATIONTAB.initialize();
            BITAUTORISATIONTAB.dataBind();
            BITPAGES.authTabIsLoaded = true;
        });

        //uitgezet want overerving werkt hier nog niet
        //TODO: dus nog maken dat alle pagina's binnen een folder met auth dezelfde auth krijgen
        //$("#bitTabsFolders").tabs({ "cache": true });
        //$("#bitTabsFolders").tabs("add", "/_bitplate/Dialogs/AutorisationTab2.aspx", "Autorisatie");
        //$("#bitTabsFolders").bind("tabsload", function (event, ui) {
        //    BITAUTORISATIONTAB2.currentObject = BITPAGES.folder;
        //    BITAUTORISATIONTAB2.initialize();
        //    BITAUTORISATIONTAB2.dataBind();
        //    BITPAGES.authTab2IsLoaded = true;
        //});

        $('#bitSearchTextbox').searchable(BITPAGES.search);

        $('#moduleRecoverButton').click(BITPAGES.loadModules);
    },


    updateBreadCrump: function (folderPath) {
        var folderList = [];
        var rootFolderItem = new Object();
        rootFolderItem.Path = "";
        rootFolderItem.Name = "root";
        folderList.push(rootFolderItem);

        if (folderPath) {
            var folderNames = folderPath.split("/");
            var path = "";
            for (var i in folderNames) {
                var folderName = folderNames[i];
                if (folderName != "") {
                    if (path != "") {
                        path += '/'
                    }
                    path += folderName;
                    var folderItem = new Object();
                    folderItem.Path = path;
                    folderItem.Name = folderName;
                    folderList.push(folderItem);

                }
            }
        }
        $('#breadcrump').dataBindList(folderList);
        //<ul>list stylen naar breadcrump
        $('#breadcrump').jBreadCrumb();

    },

    loadPages: function (folderId, folderPath, sort, searchString) {

        folderId = (folderId != 'root') ? folderId : '';
        BITPAGES.selectedFolderId = folderId;


        if (!folderPath) folderPath = "";
        if (!searchString) searchString = "";
        if (sort) BITPAGES.currentSort = sort;
        var parametersObject = { folderId: folderId, folderPath: folderPath, sort: BITPAGES.currentSort, searchString: searchString };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
        BITAJAX.callWebService("GetFoldersAndPages#historyFunction", jsonstring, function (data) {
            folderId = (folderId == '') ? 'root' : folderId;
            if ((folderPath && folderPath != '') || folderId == 'root' || folderId == '00000000-0000-0000-0000-000000000000') {
                BITPAGES.selectedPath = folderPath;
                BITPAGES.updateBreadCrump(folderPath);
            }
            folderId = (folderId != 'root') ? folderId : '';
            BITPAGES.bindPageAndFolders(data.d);
        });
    },

    bindPageAndFolders: function (data, searchString) {
        $("#tablePages").dataBindList(data, {
            onSort: function (sort) {
                BITPAGES.loadPages(BITPAGES.selectedFolderId, BITPAGES.selectedPath, sort);
            },
            onRowBound: function (obj, index, html) {
                //maak wrapper div, zet hierin de html van de tr
                //doe bewerkingen in deze html
                //zet weer terug in de tr (onderaan)
                var tempWrapperDiv = document.createElement('div');
                $(tempWrapperDiv).html(html);
                if (searchString && searchString != "") {
                    //toon kolom met pad er in bij zoeken, want zoeken is in alle mappen
                    $(tempWrapperDiv).find('.bitTableColumnTitle').hide();
                    $(tempWrapperDiv).find('.bitTableColumnPath').show();
                    //toon pad-titel in tabel
                    $("#tablePages thead").find(".bitTableColumnTitle").hide();
                    $("#tablePages thead").find(".bitTableColumnPath").show();
                }
                else {
                    //toon titel-titel rij in tabel
                    $("#tablePages thead").find(".bitTableColumnTitle").show();
                    $("#tablePages thead").find(".bitTableColumnPath").hide();
                }
                if (!obj.IsActive) {
                    $(tempWrapperDiv).find('tr').addClass("inactive");
                    $(tempWrapperDiv).find('tr').css("color", "gray");
                }

                if (obj.IsHomePage) {
                    $(tempWrapperDiv).find('.icon').find('a').attr('class', 'bitTableHomePageIcon');
                }
                if (obj.HasAutorisation) {
                    $(tempWrapperDiv).find('.bitTableAuthIcon').css("display", "inline-block");
                }
                //if (obj.StatusEnum == 2) { //nieuwe
                //    $(tempWrapperDiv).find('.bitLiveButton').attr('disabled', 'disabled');
                //    $(tempWrapperDiv).find('.bitTableActionButton').find('.bitLiveButton').hide();
                //}
                if (obj.Type == "Folder") {
                    $(tempWrapperDiv).find('.icon').find('a').attr('class', 'bitTableFolderIcon');
                    $(tempWrapperDiv).find('.bitEditButton').hide();
                    $(tempWrapperDiv).find('.bitLiveButton').hide();
                    $(tempWrapperDiv).find('.bitPreviewButton').hide();
                    if (obj.Name == '...') {
                        $(tempWrapperDiv).find('.bitConfigButton').hide();
                        $(tempWrapperDiv).find('.bitDeleteButton').hide();
                        $(tempWrapperDiv).find('.bitCopyButton').hide();
                    }
                    var text = $(tempWrapperDiv).find('.bitTableColumnName').html();
                    var htmlLink = "<a href=\"javascript:BITPAGES.loadPages('" + obj.ID + "', '" + obj.Path + "');\">" + obj.Name + "</a>";
                    $(tempWrapperDiv).find('.bitTableColumnName').html(htmlLink);
                }
                html = $(tempWrapperDiv).html();
                return html;
            }
        });
    },

    loadTemplates: function () {
        //BITAJAX.dataServiceUrl = "/_bitplate/Templates/TemplateService.asmx";
        //BITAJAX.callWebServiceASync("GetTemplates", null, function (data) {
        //    BITPAGES.templates = data.d;
        //    $("#divChooseTemplate").dataBindList(data.d);
        //    //$(BITPAGES.pagePopup.getContentDiv()).find("#divChooseTemplate").dataBindList(data.d);
        //});
    },

    loadScripts: function () {
        BITAJAX.dataServiceUrl = "/_bitplate/Scripts/ScriptService.asmx";
        BITAJAX.callWebServiceASync("LoadScripts", null, function (data) {
            $('#bitScriptList').dataBindList(data.d);
        });
    },

    showTemplateSelector: function () {
        $('#bitTemplateDialog').dialog('open');
    },

    selectTemplate: function (div) {
        var id = $(div).find("input[type=hidden].hiddenID").val();
        var parametersObject = { id: id, type: 0 };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/Templates/TemplateService.asmx";
        BITAJAX.callWebServiceASync("GetTemplate", jsonstring, function (data) {
            var template = data.d;
            if (BITPAGES.page != null) {
                //om fout te voorkomen " doesn't fit the state of the object" niet rechtstreeks template zetten
                BITPAGES.page.Template = new Object();
                BITPAGES.page.Template.ID = template.ID;
                BITPAGES.page.Template.Name = template.Name;
                BITPAGES.page.Template.Scripts = template.Scripts;
                $("#tableScriptsPerTemplate").dataBindList(BITPAGES.page.Template.Scripts);
                $("#tableScriptsPerPage").dataBindList(BITPAGES.page.Scripts);
            }

            $('#imgTemplateScreenShot').show();
            $("#imgTemplateScreenShot").attr("src", template.Screenshot);
            $("#spanTemplateName").html(template.Name);
            $("#spanTemplateName").parent().find('.validationMsg').hide();
            $("#spanLanguageCode").html(template.LanguageCode);
        });

        //for (var i in BITPAGES.templates) {
        //    var template = BITPAGES.templates[i];
        //    if (template.ID == id) {
        //        if (BITPAGES.page != null) {
        //            //om fout te voorkomen " doesn't fit the state of the object" niet rechtstreeks template zetten
        //            BITPAGES.page.Template = new Object();
        //            BITPAGES.page.Template.ID = template.ID;
        //            BITPAGES.page.Template.Name = template.Name;
        //            BITPAGES.page.Template.Scripts = template.Scripts;
        //            $("#tableScriptsPerTemplate").dataBindList(BITPAGES.page.Template.Scripts);
        //            $("#tableScriptsPerPage").dataBindList(BITPAGES.page.Scripts);
        //        }

        //        $('#imgTemplateScreenShot').show();
        //        $("#imgTemplateScreenShot").attr("src", template.Screenshot);
        //        $("#spanTemplateName").html(template.Name);
        //        $("#spanTemplateName").parent().find('.validationMsg').hide();
        //        $("#spanLanguageCode").html(template.LanguageCode);
        //        //$(BITPAGES.pagePopup.getContentDiv()).find("#tableScriptsPerTemplate_AtPage").dataBindList(template.Scripts);

        //    }
        //}
        $('#bitTemplateDialog').dialog('close');

    },

    fillDropDownFolders: function () {
        BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
        BITAJAX.callWebServiceASync("GetFolders", null, function (data) {
            BITPAGES.folders = data.d;
            $("#selectPageFolders").fillDropDown(data.d, { value: "00000000-0000-0000-0000-000000000000", text: "" });
            $("#selectFolderParentFolders").fillDropDown(data.d, { value: "00000000-0000-0000-0000-000000000000", text: "" });
        });
    },


    search: function (searchText) {
        //BITPAGES.searchString = searchText;
        BITPAGES.loadPages("", "", null, searchText);
    },

    copy: function (id, type, name) {
        var title = "Pagina kopiëren";
        if (type == "Folder") title = "Folder kopiëren";
        var newName = name + ' (kopie)';
        $.inputBox('Kopie naam:', title, newName, function (e, value) {
            if (type == "Item") {
                //var parentFolderId = "";
                //if (BITPAGES.selectedFolder) {
                //    parentFolderId = BITPAGES.selectedFolder.ID;
                //}
                var newName = value;
                var parametersObject = { pageId: id, newPageName: newName };
                //var parametersObject = { parentFolderId: parentFolderId, pageId: id, pageName: $('#CopyName').val() };
                var jsonstring = JSON.stringify(parametersObject);
                BITAJAX.callWebService("CopyPage", jsonstring, function (data) {
                    //reload tree
                    BITPAGES.loadPages(BITPAGES.selectedFolderId);
                    //$('#bitPageCopyDialog').dialog('close');
                });
            }
            else if (type == "Folder") {
                //var parentFolderId = (BITPAGES.folderCollection.length > 1) ? BITPAGES.folderCollection[BITPAGES.folderCollection.length - 2]['ID'] : '';
                var newName = value;
                var parametersObject = { folderId: id, newFolderName: newName };
                var jsonstring = JSON.stringify(parametersObject);
                BITAJAX.callWebService("CopyFolder", jsonstring, function (data) {
                    //reload tree
                    BITPAGES.loadPages(BITPAGES.selectedFolderId);
                    BITPAGES.fillDropDownFolders();
                    //$('#bitPageCopyDialog').dialog('close');
                });
            }
        });
    },

    newPage: function () {
        BITPAGES.openPagePopup(null);
    },

    newFolder: function () {
        BITPAGES.openFolderPopup(null);
    },

    openDetailsPopup: function (id, type) {
        if (type == "Page" || type == "Item") {
            BITPAGES.openPagePopup(id);
        }
        else if (type == "Folder") {
            BITPAGES.openFolderPopup(id);
        }
    },

    openPagePopup: function (id) {
        $.validationReset();
        var parametersObject = { id: id, folderid: BITPAGES.selectedFolderId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
        BITAJAX.callWebService("GetPage", jsonstring, function (data) {
            BITPAGES.page = data.d;
            $('#bitPageDialog').dataBind(BITPAGES.page);
            BITPAGES.type = "page";
            if (BITPAGES.page.IsNew) {
                $('#bitPageDialog').dialog({ title: "Nieuwe pagina" });
                $("#tableScriptsPerSite").dataBindList(BITPAGES.page.SiteScripts);
                $("#tableScriptsPerTemplate").dataBindList(BITPAGES.page.Template.Scripts);
                $("#tableScriptsPerPage").dataBindList(BITPAGES.page.Scripts);

            }
            else {
                $('#bitPageDialog').dialog({ title: "Pagina: " + BITPAGES.page.Name });
                $("#tableScriptsPerSite").dataBindList(BITPAGES.page.SiteScripts);
                $("#tableScriptsPerTemplate").dataBindList(BITPAGES.page.Template.Scripts);
                $("#tableScriptsPerPage").dataBindList(BITPAGES.page.Scripts);

            }
            if (BITPAGES.authTabIsLoaded) {
                BITAUTORISATIONTAB.currentObject = BITPAGES.page;
                BITAUTORISATIONTAB.dataBind();
            }
            BITPAGES.loadScripts();
            $('#bitPageDialog').dialog('open');
        });

    },

    loadModules: function () {
        var parametersObject = { pageID: BITPAGES.page.ID };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
        BITAJAX.callWebService("GetModulesByPageId", jsonstring, function (data) {
            $('#tableModules').dataBindList(data.d);
        });
    },

    recoverModule: function (button) {
        $.confirmBox('Weet u zeker dat u deze module wilt herstellen?', 'Module herstellen?', function () {
            var id = $(button).parent().find('div').html(); //Hack ik krijg het moduleID niet goed in de html.
            var parametersObject = { moduleID: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
            BITAJAX.callWebService("RecoverModuleById", jsonstring, function (data) {
                if (data.d) {
                    $.messageBox(MSGBOXTYPES.INFO, 'De module is hersteld', 'Module hersteld');
                }
                else {
                    $.messageBox(MSGBOXTYPES.INFO, 'Er is geen module met dit ID (' + id + ') gevonden.', 'Module hersteld');
                }
                $('#bitPageDialog').dialog('close');
            });
        });
    },

    savePage: function (e) {
        //opslaan page config
        var validation = $('#bitPageDialog').validate();
        if (validation) {
            //get data from panel
            BITPAGES.page = $('#bitPageDialog').collectData(BITPAGES.page);
            if (BITPAGES.page.Template.ID == null) {
                $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.WARNING, "Een layouttemplate is verplicht. Kies eerst een layout.");
                return;
            }
            jsonstring = convertToJsonString(BITPAGES.page);

            BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
            BITAJAX.callWebServiceASync("SavePage", jsonstring, function (data) {
                $('#bitPageDialog').dialog('close');
                //reload grid
                BITPAGES.loadPages(BITPAGES.selectedFolderId);
            }, null);
        }
    },

    openFolderPopup: function (id) {
        $.validationReset();
        var parametersObject = { id: id, folderid: BITPAGES.selectedFolderId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
        BITAJAX.callWebService("GetFolder", jsonstring, function (data) {
            BITPAGES.folder = data.d;
            $('#bitFolderDialog').dataBind(BITPAGES.folder);
            BITPAGES.type = "folder";
            if (BITPAGES.folder.IsNew) {
                $('#bitFolderDialog').dialog({ title: "Nieuwe folder" });
            }
            else {
                $('#bitFolderDialog').dialog({ title: "Folder: " + BITPAGES.folder.Name });
            }
            if (BITPAGES.authTab2IsLoaded) {
                BITAUTORISATIONTAB2.currentObject = BITPAGES.folder;
                BITAUTORISATIONTAB2.dataBind();
            }
            $('#bitFolderDialog').dialog('open');
        });
    },

    saveFolder: function (e) {
        //opslaan page config
        var validation = $('#bitFolderDialog').validate();
        if (validation) {
            //get data from panel
            BITPAGES.folder = $('#bitFolderDialog').collectData(BITPAGES.folder);
            jsonstring = convertToJsonString(BITPAGES.folder);

            BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
            BITAJAX.callWebServiceASync("SaveFolder", jsonstring, function (data) {
                $('#bitFolderDialog').dialog('close');
                //reload grid
                BITPAGES.loadPages(BITPAGES.selectedFolderId);
                BITPAGES.fillDropDownFolders();
            }, null, e.srcElement);
        }
    },

    remove: function (id, type) {
        //rij een selectie kleur geven
        $("#" + id).css("background-color", "#d38933");

        if (type == "Page" || type == "Item") {
            BITPAGES.removePage(id);
        }
        else if (type == "Folder") {
            BITPAGES.removeFolder(id);
        }
    },

    removePage: function (id) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u deze pagina verwijderen?", null, function (e) {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
            BITAJAX.callWebServiceASync("DeletePage", jsonstring, function (data) {
                BITPAGES.loadPages(BITPAGES.selectedFolderId, BITPAGES.selectedPath);
            }, null);
        }, null,
        function () {
            $("#" + id).css("background-color", "");
        });
    },

    removeFolder: function (id) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u deze map verwijderen?", null, function (e) {

            var parametersObject = { id: id, deleteAllPages: false };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";

            BITAJAX.callWebServiceASync("DeleteFolder", jsonstring, function (data) {
                if (!data.d.Success) {
                    $.deleteConfirmBox(data.d.Message, 'Map verwijderen?', function (e) {
                        parametersObject = { id: id, deleteAllPages: true };
                        jsonstring = JSON.stringify(parametersObject);
                        BITAJAX.callWebServiceASync("DeleteFolder", jsonstring, function (data) {
                            BITPAGES.bindPageAndFolders(data.d.DataObject);
                        }, null);
                    });
                }
                else {
                    BITPAGES.bindPageAndFolders(data.d.DataObject);
                }
                //var folderId = (BITPAGES.folderCollection.length > 0) ? BITPAGES.folderCollection[BITPAGES.folderCollection.length - 1]['ID'] : '';
                //BITPAGES.loadPages(folderId);
                //$("#" + id).css("background-color", "");
                //BITPAGES.loadPages(BITPAGES.selectedFolderId, BITPAGES.selectedPath);
            }, null, e.srcElement);
        });
    },

    inSideMapChange: function () {
        if ($('#bitInSitemap').attr('checked')) {
            $('#SiteMapChangeFreq').removeAttr('disabled');
            $('#SiteMapPriority').removeAttr('disabled');

            //$('#bitDenySearchengines').attr('disabled', 'disabled');
            $('#bitDenySearchengines').removeAttr('checked');
        }
        else {
            $('#bitDenySearchengines').removeAttr('disabled');
            $('#SiteMapChangeFreq').attr('disabled', 'disabled');
            $('#SiteMapPriority').attr('disabled', 'disabled');
        }
    },

    inRobotsTxtChange: function () {
        if ($('#bitDenySearchengines').attr('checked')) {
            //$('#bitInSitemap').attr('disabled', 'disabled');
            $('#bitInSitemap').removeAttr('checked');
            $('#SiteMapChangeFreq').attr('disabled', 'disabled');
            $('#SiteMapPriority').attr('disabled', 'disabled');
        }
    },

    openScriptsPopup: function () {
        $('#bitScriptDialog').dialog('open');
    },

    addScript: function (id, name) {
        var script = new Object();
        script.ID = id;
        script.CompleteName = name;

        if (BITPAGES.page.Scripts == null) {
            BITPAGES.page.Scripts = [];
        }
        //kijk of die er al in zit
        var contains = false;
        for (var i in BITPAGES.page.SiteScripts) {
            var scriptCheck = BITPAGES.page.SiteScripts[i];
            if (scriptCheck.ID == script.ID) {
                contains = true;
                break;
            }
        }
        if (!contains && BITPAGES.page.Template) {
            for (var i in BITPAGES.page.Template.Scripts) {
                var scriptCheck = BITPAGES.page.Template.Scripts[i];
                if (scriptCheck.ID == script.ID) {
                    contains = true;
                    break;
                }
            }
        }
        if (!contains) {
            for (var i in BITPAGES.page.Scripts) {
                var scriptCheck = BITPAGES.page.Scripts[i];
                if (scriptCheck.ID == script.ID) {
                    contains = true;
                    break;
                }
            }
        }
        if (contains) {
            $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "Koppeling met dit script bestaat al");
            //alert("Koppeling met dit script bestaat al");
        }
        else {
            BITPAGES.page.Scripts.push(script);
            $("#tableScriptsPerPage").dataBindList(BITPAGES.page.Scripts);
        }
    },

    removeScript: function (id, index) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u dit script wilt los koppelen van deze pagina?", null, function () {
            BITPAGES.page.Scripts.splice(index, 1);
            $("#tableScriptsPerPage").dataBindList(BITPAGES.page.Scripts);
        });
    },

    select: function (id, url) {
        var link = '/' + url.replace(/^(?:\/\/|[^\/]+)*\//, "");
        var
       urlArg = window.location.search.substr(1).split('&'), // Split search strings to "name=value" pair
       tmp,
       params = {},
       i
        ;
        // P
        for (var i = urlArg.length; i-- > 0;) {
            // Split to param name and value ("name=value")
            tmp = urlArg[i].split('=');
            // Save it to object
            params[tmp[0]] = tmp[1];
        }

        // Check eviroment
        if (params.CKEditorFuncNum) {
            // Call CKEditor function to insert the URL
            window.opener.CKEDITOR.tools.callFunction(params.CKEditorFuncNum, link);
            // Close Window
            window.close();
        } else {
            // Else do nothing
            return false;
        }
    }

};

/********************************************OUDE VERSIE**************************************************/


//var BITPAGES = {
//    type: null, //pages, of folders
//    folders: null,
//    templates: null,
//    selectedFolderId: null,
//    selectedPath: "",
//    searchString: "",
//    currentSort: "Name ASC",
//    pagePopup: null,
//    folderPopup: null,
//    templatePopup: null,
//    treeMode: false,
//    page: null,
//    folder: null,
//    //folderCollection: Array(),
//    authTabIsLoaded: false,

//    initialize: function () {
//        BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";

//        $('#bitPageDialog').initDialog(BITPAGES.savePage, { width: 1000, height: 700 });
//        $('#bitFolderDialog').initDialog(BITPAGES.saveFolder);
//        $('#bitScriptDialog').initDialog(null, { width: 200, height: 400 }, false);
//        $('#bitTemplateDialog').initDialog(null, { width: 600, height: 400 }, false);
//        $('#bitUserGroupsDialog').initDialog(null, { width: 600, height: 400 }, false);
//        $('#bitUsersDialog').initDialog(null, { width: 600, height: 400 }, false);
//        $('#bitPopups').formEnrich();

//        $("#bitTabsPages").tabs({ "cache": true });
//        $("#bitTabsPages").tabs("add", "/_bitplate/Dialogs/AutorisationTab.aspx", "Autorisatie");
//        $("#bitTabsPages").bind("tabsload", function (event, ui) {
//            BITAUTORISATIONTAB.currentObject = BITPAGES.page;
//            BITAUTORISATIONTAB.initialize();
//            BITAUTORISATIONTAB.dataBind();
//            BITPAGES.authTabIsLoaded = true;
//        });

//        $("#bitTabsFolders").tabs({ "cache": true });
//        $("#bitTabsFolders").tabs("add", "/_bitplate/Dialogs/AutorisationTab2.aspx", "Autorisatie");
//        $("#bitTabsFolders").bind("tabsload", function (event, ui) {
//            BITAUTORISATIONTAB2.currentObject = BITPAGES.folder;
//            BITAUTORISATIONTAB2.initialize();
//            BITAUTORISATIONTAB2.dataBind();
//            BITPAGES.authTab2IsLoaded = true;
//        });

//        $('#bitSearchTextbox').searchable(BITPAGES.search);

//        $('#moduleRecoverButton').click(BITPAGES.loadModules);
//    },


//    updateBreadCrump: function (folderPath) {
//        var folderList = [];
//        var rootFolderItem = new Object();
//        rootFolderItem.Path = "";
//        rootFolderItem.Name = "root";
//        folderList.push(rootFolderItem);

//        if (folderPath) {
//            var folderNames = folderPath.split("/");
//            var path = "";
//            for (var i in folderNames) {
//                var folderName = folderNames[i];
//                if (folderName != "") {
//                    if (path != "") {
//                        path += '/'
//                    }
//                    path += folderName;
//                    var folderItem = new Object();
//                    folderItem.Path = path;
//                    folderItem.Name = folderName;
//                    folderList.push(folderItem);

//                }
//            }
//        }
//        $('#breadcrump').dataBindList(folderList);
//        //<ul>list stylen naar breadcrump
//        $('#breadcrump').jBreadCrumb();

//    },

//    loadPages: function (folderId, folderPath, sort, searchString) {
//        if ((folderPath && folderPath != '') || folderId == 'root' || folderId == '00000000-0000-0000-0000-000000000000') {
//            BITPAGES.selectedPath = folderPath;
//            BITPAGES.updateBreadCrump(folderPath);
//        }
//        folderId = (folderId != 'root') ? folderId : '';
//        BITPAGES.selectedFolderId = folderId;


//        if (!folderPath) folderPath = "";
//        if (!searchString) searchString = "";
//        if (sort) BITPAGES.currentSort = sort;
//        var parametersObject = { folderId: folderId, folderPath: folderPath, sort: BITPAGES.currentSort, searchString: searchString };
//        var jsonstring = JSON.stringify(parametersObject);
//        BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
//        BITAJAX.callWebService("GetFoldersAndPages", jsonstring, function (data) {
//            BITPAGES.bindPageAndFolders(data.d);
//        });
//    },

//    bindPageAndFolders: function (data, searchString) {
//        $("#tablePages").dataBindList(data, {
//            onSort: function (sort) {
//                BITPAGES.loadPages(BITPAGES.selectedFolderId, BITPAGES.selectedPath, sort);
//            },
//            onRowBound: function (obj, index, html) {
//                //maak wrapper div, zet hierin de html van de tr
//                //doe bewerkingen in deze html
//                //zet weer terug in de tr (onderaan)
//                var tempWrapperDiv = document.createElement('div');
//                $(tempWrapperDiv).html(html);
//                if (searchString && searchString != "") {
//                    //toon kolom met pad er in bij zoeken, want zoeken is in alle mappen
//                    $(tempWrapperDiv).find('.bitTableColumnTitle').hide();
//                    $(tempWrapperDiv).find('.bitTableColumnPath').show();
//                    //toon pad-titel in tabel
//                    $("#tablePages thead").find(".bitTableColumnTitle").hide();
//                    $("#tablePages thead").find(".bitTableColumnPath").show();
//                }
//                else {
//                    //toon titel-titel rij in tabel
//                    $("#tablePages thead").find(".bitTableColumnTitle").show();
//                    $("#tablePages thead").find(".bitTableColumnPath").hide();
//                }
//                if (!obj.IsActive) {
//                    $(tempWrapperDiv).find('tr').addClass("inactive");
//                    $(tempWrapperDiv).find('tr').css("color", "gray");
//                }

//                if (obj.IsHomePage) {
//                    $(tempWrapperDiv).find('.icon').find('a').attr('class', 'bitTableHomePageIcon');
//                }
//                if (obj.HasAutorisation) {
//                    $(tempWrapperDiv).find('.bitTableAuthIcon').css("display", "inline-block");
//                }
//                //if (obj.StatusEnum == 2) { //nieuwe
//                //    $(tempWrapperDiv).find('.bitLiveButton').attr('disabled', 'disabled');
//                //    $(tempWrapperDiv).find('.bitTableActionButton').find('.bitLiveButton').hide();
//                //}
//                if (obj.Type == "Folder") {
//                    $(tempWrapperDiv).find('.icon').find('a').attr('class', 'bitTableFolderIcon');
//                    $(tempWrapperDiv).find('.bitEditButton').hide();
//                    $(tempWrapperDiv).find('.bitLiveButton').hide();
//                    $(tempWrapperDiv).find('.bitPreviewButton').hide();
//                    if (obj.Name == '...') {
//                        $(tempWrapperDiv).find('.bitConfigButton').hide();
//                        $(tempWrapperDiv).find('.bitDeleteButton').hide();
//                        $(tempWrapperDiv).find('.bitCopyButton').hide();
//                    }
//                    var text = $(tempWrapperDiv).find('.bitTableColumnName').html();
//                    var htmlLink = "<a href=\"javascript:BITPAGES.loadPages('" + obj.ID + "', '" + obj.Path + "');\">" + obj.Name + "</a>";
//                    $(tempWrapperDiv).find('.bitTableColumnName').html(htmlLink);
//                }
//                html = $(tempWrapperDiv).html();
//                return html;
//            }
//        });
//    },

//    loadTemplates: function () {
//        //BITAJAX.dataServiceUrl = "/_bitplate/Templates/TemplateService.asmx";
//        //BITAJAX.callWebServiceASync("GetTemplates", null, function (data) {
//        //    BITPAGES.templates = data.d;
//        //    $("#divChooseTemplate").dataBindList(data.d);
//        //    //$(BITPAGES.pagePopup.getContentDiv()).find("#divChooseTemplate").dataBindList(data.d);
//        //});
//    },

//    loadScripts: function () {
//        BITAJAX.dataServiceUrl = "/_bitplate/Scripts/ScriptService.asmx";
//        BITAJAX.callWebServiceASync("LoadScripts", null, function (data) {
//            $('#bitScriptList').dataBindList(data.d);
//        });
//    },

//    showTemplateSelector: function () {
//        $('#bitTemplateDialog').dialog('open');
//    },

//    selectTemplate: function (div) {
//        var id = $(div).find("input[type=hidden].hiddenID").val();
//        var parametersObject = { id: id, type: 0 };
//        var jsonstring = JSON.stringify(parametersObject);
//        BITAJAX.dataServiceUrl = "/_bitplate/Templates/TemplateService.asmx";
//        BITAJAX.callWebServiceASync("GetTemplate", jsonstring, function (data) {
//            var template = data.d;
//            if (BITPAGES.page != null) {
//                //om fout te voorkomen " doesn't fit the state of the object" niet rechtstreeks template zetten
//                BITPAGES.page.Template = new Object();
//                BITPAGES.page.Template.ID = template.ID;
//                BITPAGES.page.Template.Name = template.Name;
//                BITPAGES.page.Template.Scripts = template.Scripts;
//                $("#tableScriptsPerTemplate").dataBindList(BITPAGES.page.Template.Scripts);
//                $("#tableScriptsPerPage").dataBindList(BITPAGES.page.Scripts);
//            }

//            $('#imgTemplateScreenShot').show();
//            $("#imgTemplateScreenShot").attr("src", template.Screenshot);
//            $("#spanTemplateName").html(template.Name);
//            $("#spanTemplateName").parent().find('.validationMsg').hide();
//            $("#spanLanguageCode").html(template.LanguageCode);
//        });

//        //for (var i in BITPAGES.templates) {
//        //    var template = BITPAGES.templates[i];
//        //    if (template.ID == id) {
//        //        if (BITPAGES.page != null) {
//        //            //om fout te voorkomen " doesn't fit the state of the object" niet rechtstreeks template zetten
//        //            BITPAGES.page.Template = new Object();
//        //            BITPAGES.page.Template.ID = template.ID;
//        //            BITPAGES.page.Template.Name = template.Name;
//        //            BITPAGES.page.Template.Scripts = template.Scripts;
//        //            $("#tableScriptsPerTemplate").dataBindList(BITPAGES.page.Template.Scripts);
//        //            $("#tableScriptsPerPage").dataBindList(BITPAGES.page.Scripts);
//        //        }

//        //        $('#imgTemplateScreenShot').show();
//        //        $("#imgTemplateScreenShot").attr("src", template.Screenshot);
//        //        $("#spanTemplateName").html(template.Name);
//        //        $("#spanTemplateName").parent().find('.validationMsg').hide();
//        //        $("#spanLanguageCode").html(template.LanguageCode);
//        //        //$(BITPAGES.pagePopup.getContentDiv()).find("#tableScriptsPerTemplate_AtPage").dataBindList(template.Scripts);

//        //    }
//        //}
//        $('#bitTemplateDialog').dialog('close');

//    },

//    fillDropDownFolders: function () {
//        BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
//        BITAJAX.callWebServiceASync("GetFolders", null, function (data) {
//            BITPAGES.folders = data.d;
//            $("#selectPageFolders").fillDropDown(data.d, { value: "00000000-0000-0000-0000-000000000000", text: "" });
//            $("#selectFolderParentFolders").fillDropDown(data.d, { value: "00000000-0000-0000-0000-000000000000", text: "" });
//        });
//    },


//    search: function (searchText) {
//        //BITPAGES.searchString = searchText;
//        BITPAGES.loadPages("", "", null, searchText);
//    },

//    copy: function (id, type, name) {
//        var title = "Pagina kopiëren";
//        if (type == "Folder") title = "Folder kopiëren";
//        var newName = name + ' (kopie)';
//        $.inputBox('Kopie naam:', title, newName, function (value) {
//            if (type == "Item") {
//                //var parentFolderId = "";
//                //if (BITPAGES.selectedFolder) {
//                //    parentFolderId = BITPAGES.selectedFolder.ID;
//                //}
//                var newName = value;
//                var parametersObject = { pageId: id, newPageName: newName };
//                //var parametersObject = { parentFolderId: parentFolderId, pageId: id, pageName: $('#CopyName').val() };
//                var jsonstring = JSON.stringify(parametersObject);
//                BITAJAX.callWebService("CopyPage", jsonstring, function (data) {
//                    //reload tree
//                    BITPAGES.loadPages(BITPAGES.selectedFolderId);
//                    //$('#bitPageCopyDialog').dialog('close');
//                });
//            }
//            else if (type == "Folder") {
//                //var parentFolderId = (BITPAGES.folderCollection.length > 1) ? BITPAGES.folderCollection[BITPAGES.folderCollection.length - 2]['ID'] : '';
//                var newName = value;
//                var parametersObject = { folderId: id, newFolderName: newName };
//                var jsonstring = JSON.stringify(parametersObject);
//                BITAJAX.callWebService("CopyFolder", jsonstring, function (data) {
//                    //reload tree
//                    BITPAGES.loadPages(BITPAGES.selectedFolderId);
//                    BITPAGES.fillDropDownFolders();
//                    //$('#bitPageCopyDialog').dialog('close');
//                });
//            }
//        });
//    },

//    newPage: function () {
//        BITPAGES.openPagePopup(null);
//    },

//    newFolder: function () {
//        BITPAGES.openFolderPopup(null);
//    },

//    openDetailsPopup: function (id, type) {
//        if (type == "Page" || type == "Item") {
//            BITPAGES.openPagePopup(id);
//        }
//        else if (type == "Folder") {
//            BITPAGES.openFolderPopup(id);
//        }
//    },

//    openPagePopup: function (id) {
//        $.validationReset();
//        var parametersObject = { id: id, folderid: BITPAGES.selectedFolderId };
//        var jsonstring = JSON.stringify(parametersObject);
//        BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
//        BITAJAX.callWebService("GetPage", jsonstring, function (data) {
//            BITPAGES.page = data.d;
//            $('#bitPageDialog').dataBind(BITPAGES.page);
//            BITPAGES.type = "page";
//            if (BITPAGES.page.IsNew) {
//                $('#bitPageDialog').dialog({ title: "Nieuwe pagina" });
//                $("#tableScriptsPerSite").dataBindList([]);
//                $("#tableScriptsPerTemplate").dataBindList([]);
//                $("#tableScriptsPerPage").dataBindList([]);

//            }
//            else {
//                $('#bitPageDialog').dialog({ title: "Pagina: " + BITPAGES.page.Name });
//                $("#tableScriptsPerSite").dataBindList(BITPAGES.page.SiteScripts);
//                $("#tableScriptsPerTemplate").dataBindList(BITPAGES.page.Template.Scripts);
//                $("#tableScriptsPerPage").dataBindList(BITPAGES.page.Scripts);

//            }
//            if (BITPAGES.authTabIsLoaded) {
//                BITAUTORISATIONTAB.currentObject = BITPAGES.page;
//                BITAUTORISATIONTAB.dataBind();
//            }

//            $('#bitPageDialog').dialog('open');
//        });

//    },

//    loadModules: function() {
//        var parametersObject = { pageID: BITPAGES.page.ID };
//        var jsonstring = JSON.stringify(parametersObject);
//        BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
//        BITAJAX.callWebService("GetModulesByPageId", jsonstring, function (data) {
//            $('#tableModules').dataBindList(data.d);
//        });
//    },

//    recoverModule: function (button) {
//        $.confirmBox('Weet u zeker dat u deze module wilt herstellen?', 'Module herstellen?', function () {
//            var id = $(button).parent().find('div').html(); //Hack ik krijg het moduleID niet goed in de html.
//            var parametersObject = { moduleID: id };
//            var jsonstring = JSON.stringify(parametersObject);
//            BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
//            BITAJAX.callWebService("RecoverModuleById", jsonstring, function (data) {
//                if (data.d) {
//                    $.messageBox(MSGBOXTYPES.INFO, 'De module is hersteld', 'Module hersteld');
//                }
//                else {
//                    $.messageBox(MSGBOXTYPES.INFO, 'Er is geen module met dit ID (' + id + ') gevonden.', 'Module hersteld');
//                }
//                $('#bitPageDialog').dialog('close');
//            });
//        });
//    },

//    savePage: function (e) {
//        //opslaan page config
//        var validation = $('#bitPageDialog').validate();
//        if (validation) {
//            //get data from panel
//            BITPAGES.page = $('#bitPageDialog').collectData(BITPAGES.page);
//            if (BITPAGES.page.Template.ID == null) {
//                $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.WARNING, "Een layouttemplate is verplicht. Kies eerst een layout.");
//                return;
//            }
//            jsonstring = convertToJsonString(BITPAGES.page);

//            BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
//            BITAJAX.callWebServiceASync("SavePage", jsonstring, function (data) {
//                $('#bitPageDialog').dialog('close');
//                //reload grid
//                BITPAGES.loadPages(BITPAGES.selectedFolderId);
//            }, null, e.srcElement);
//        }
//    },

//    openFolderPopup: function (id) {
//        $.validationReset();
//        var parametersObject = { id: id, folderid: BITPAGES.selectedFolderId };
//        var jsonstring = JSON.stringify(parametersObject);
//        BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
//        BITAJAX.callWebService("GetFolder", jsonstring, function (data) {
//            BITPAGES.folder = data.d;
//            $('#bitFolderDialog').dataBind(BITPAGES.folder);
//            BITPAGES.type = "folder";
//            if (BITPAGES.folder.IsNew) {
//                $('#bitFolderDialog').dialog({ title: "Nieuwe folder" });
//            }
//            else {
//                $('#bitFolderDialog').dialog({ title: "Folder: " + BITPAGES.folder.Name });
//            }
//            if (BITPAGES.authTab2IsLoaded) {
//                BITAUTORISATIONTAB2.currentObject = BITPAGES.folder;
//                BITAUTORISATIONTAB2.dataBind();
//            }
//            $('#bitFolderDialog').dialog('open');
//        });
//    },

//    saveFolder: function (e) {
//        //opslaan page config
//        var validation = $('#bitFolderDialog').validate();
//        if (validation) {
//            //get data from panel
//            BITPAGES.folder = $('#bitFolderDialog').collectData(BITPAGES.folder);
//            jsonstring = convertToJsonString(BITPAGES.folder);

//            BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
//            BITAJAX.callWebServiceASync("SaveFolder", jsonstring, function (data) {
//                $('#bitFolderDialog').dialog('close');
//                //reload grid
//                BITPAGES.loadPages(BITPAGES.selectedFolderId);
//                BITPAGES.fillDropDownFolders();
//            }, null, e.srcElement);
//        }
//    },

//    remove: function (id, type) {
//        //rij een selectie kleur geven
//        $("#" + id).css("background-color", "#d38933");

//        if (type == "Page" || type == "Item") {
//            BITPAGES.removePage(id);
//        }
//        else if (type == "Folder") {
//            BITPAGES.removeFolder(id);
//        }
//    },

//    removePage: function (id) {
//        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u deze pagina verwijderen?", null, function (e) {
//            var parametersObject = { id: id };
//            var jsonstring = JSON.stringify(parametersObject);
//            BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";
//            BITAJAX.callWebServiceASync("DeletePage", jsonstring, function (data) {
//                BITPAGES.loadPages(BITPAGES.selectedFolderId, BITPAGES.selectedPath);
//            }, null, e.srcElement);
//        }, null,
//        function () {
//            $("#" + id).css("background-color", "");
//        });
//    },

//    removeFolder: function (id) {
//        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u deze map verwijderen?", null, function (e) {

//            var parametersObject = { id: id, deleteAllPages: false };
//            var jsonstring = JSON.stringify(parametersObject);
//            BITAJAX.dataServiceUrl = "/_bitplate/pages/pageService.asmx";

//            BITAJAX.callWebServiceASync("DeleteFolder", jsonstring, function (data) {
//                if (!data.d.Success) {
//                    $.deleteConfirmBox(data.d.Message, 'Map verwijderen?', function (e) {
//                        parametersObject = { id: id, deleteAllPages: true };
//                        jsonstring = JSON.stringify(parametersObject);
//                        BITAJAX.callWebServiceASync("DeleteFolder", jsonstring, function (data) {
//                            BITPAGES.bindPageAndFolders(data.d.DataObject);
//                        }, null, e.srcElement);
//                    });
//                }
//                else {
//                    BITPAGES.bindPageAndFolders(data.d.DataObject);
//                }
//                //var folderId = (BITPAGES.folderCollection.length > 0) ? BITPAGES.folderCollection[BITPAGES.folderCollection.length - 1]['ID'] : '';
//                //BITPAGES.loadPages(folderId);
//                //$("#" + id).css("background-color", "");
//                //BITPAGES.loadPages(BITPAGES.selectedFolderId, BITPAGES.selectedPath);
//            }, null, e.srcElement);
//        });

//    },

//    inSideMapChange: function () {
//        if ($('#bitInSitemap').attr('checked')) {
//            $('#SiteMapChangeFreq').removeAttr('disabled');
//            $('#SiteMapPriority').removeAttr('disabled');

//            //$('#bitDenySearchengines').attr('disabled', 'disabled');
//            $('#bitDenySearchengines').removeAttr('checked');
//        }
//        else {
//            $('#bitDenySearchengines').removeAttr('disabled');
//            $('#SiteMapChangeFreq').attr('disabled', 'disabled');
//            $('#SiteMapPriority').attr('disabled', 'disabled');
//        }
//    },

//    inRobotsTxtChange: function () {
//        if ($('#bitDenySearchengines').attr('checked')) {
//            //$('#bitInSitemap').attr('disabled', 'disabled');
//            $('#bitInSitemap').removeAttr('checked');
//            $('#SiteMapChangeFreq').attr('disabled', 'disabled');
//            $('#SiteMapPriority').attr('disabled', 'disabled');
//        }
//    },

//    openScriptsPopup: function () {
//        $('#bitScriptDialog').dialog('open');
//    },

//    addScript: function (id, name) {
//        var script = new Object();
//        script.ID = id;
//        script.CompleteName = name;

//        if (BITPAGES.page.Scripts == null) {
//            BITPAGES.page.Scripts = [];
//        }
//        //kijk of die er al in zit
//        var contains = false;
//        for (var i in BITPAGES.page.SiteScripts) {
//            var scriptCheck = BITPAGES.page.SiteScripts[i];
//            if (scriptCheck.ID == script.ID) {
//                contains = true;
//                break;
//            }
//        }
//        if (!contains && BITPAGES.page.Template) {
//            for (var i in BITPAGES.page.Template.Scripts) {
//                var scriptCheck = BITPAGES.page.Template.Scripts[i];
//                if (scriptCheck.ID == script.ID) {
//                    contains = true;
//                    break;
//                }
//            }
//        }
//        if (!contains) {
//            for (var i in BITPAGES.page.Scripts) {
//                var scriptCheck = BITPAGES.page.Scripts[i];
//                if (scriptCheck.ID == script.ID) {
//                    contains = true;
//                    break;
//                }
//            }
//        }
//        if (contains) {
//            $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "Koppeling met dit script bestaat al");
//            //alert("Koppeling met dit script bestaat al");
//        }
//        else {
//            BITPAGES.page.Scripts.push(script);
//            $("#tableScriptsPerPage").dataBindList(BITPAGES.page.Scripts);
//        }
//    },

//    removeScript: function (id, index) {
//        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u dit script wilt los koppelen van deze pagina?", null, function () {
//            BITPAGES.page.Scripts.splice(index, 1);
//            $("#tableScriptsPerPage").dataBindList(BITPAGES.page.Scripts);
//        });
//    },

//    select: function (id, url) {
//        var link = '/' + url.replace(/^(?:\/\/|[^\/]+)*\//, "");
//        var
//       urlArg = window.location.search.substr(1).split('&'), // Split search strings to "name=value" pair
//       tmp,
//       params = {},
//       i
//        ;
//        // P
//        for (var i = urlArg.length; i-- > 0;) {
//            // Split to param name and value ("name=value")
//            tmp = urlArg[i].split('=');
//            // Save it to object
//            params[tmp[0]] = tmp[1];
//        }

//        // Check eviroment
//        if (params.CKEditorFuncNum) {
//            // Call CKEditor function to insert the URL
//            window.opener.CKEDITOR.tools.callFunction(params.CKEditorFuncNum, link);
//            // Close Window
//            window.close();
//        } else {
//            // Else do nothing
//            return false;
//        }
//    }

//};

