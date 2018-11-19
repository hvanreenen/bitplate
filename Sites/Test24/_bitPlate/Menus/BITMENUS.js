/// <reference path="BITBACKEND.js" />
/// <reference path="../prototypes/databind.js" />

var BITMENUS = {
    menuId: null,
    menu: null,
    //type: "Group", //group of Item
    //searchString: "",
    currentSort: "Name ASC",
    //currentPage: 0, //voor paginering: wordt niet gebruikt
    //pageSize: 0, //voor paginering: wordt niet gebruikt
    currentParentId: "",
    currentParentPath: "",
    //showTabs: [],
    isTreeVisible: false, //wordt niet gebruikt
    currentLanguageCode: "",
    selectFileType: null,

    initialize: function () {
        BITAJAX.isHistorySupported = true;


        $('#bitMenuConfigDialog').initDialog(BITMENUS.saveMenuConfig, { width: 800, height: 600 });
        $('#bitMenuItemDetailsDialog').initDialog(BITMENUS.saveMenuItem, { width: 900, height: 700 });
        $('#bitFileSelectDialog').initDialog(null, { height: 500, width: 1000 });
        $('#bitScriptDialog').initDialog(null, { width: 200, height: 400 }, false);
        $('#bitScriptEditDialog').initDialog(BITMENUS.saveScript, { width: '90%', height: window.innerHeight });

        $('#bitPopups').formEnrich();

        $('#bitSearchTextbox').hide();

    },

    loadMenus: function (sort) {
        BITAJAX.dataServiceUrl = "MenuService.asmx";
        if (sort) BITMENUS.currentSort = sort;
        var parametersObject = { sort: BITMENUS.currentSort };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebServiceASync("GetMenus", jsonstring, function (data) {
            $("#tableMenus").dataBindList(data.d, {
                onSort: function (sort) {
                    BITMENUS.loadMenus(sort);
                }
                
            });
        });
    },

    loadScripts: function () {
        BITAJAX.dataServiceUrl = "/_bitplate/Scripts/ScriptService.asmx";
        BITAJAX.callWebServiceASync("LoadScripts", null, function (data) {
            $('#bitScriptList').dataBindList(data.d);
        });
    },

    newMenu: function () {
        this.openMenuDetailsPopup(null);
    },

    openMenuDetailsPopup: function (id) {
        //$('#DefaultValueField').hide();
        BITAJAX.dataServiceUrl = "MenuService.asmx";
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetMenu", jsonstring, function (data) {
            BITMENUS.menu = data.d;

            $('#bitMenuConfigDialog').dataBind(data.d);
            $("#tableScriptsPerMenu").dataBindList(BITMENUS.menu.Scripts);
            if (BITMENUS.menu.IsNew) {
                $('#bitMenuConfigDialog').dialog({ title: "Nieuw menu" });
            }
            else {
                $('#bitMenuConfigDialog').dialog({ title: "Menu: " + BITMENUS.menu.Name });
            }

            $('#bitMenuConfigDialog').dialog('open');

        });
    },



    saveMenuConfig: function (e) {
        var validation = $('#bitMenuConfigDialog').validate();
        if (validation) {
            //get data from panel
            BITMENUS.menu = $('#bitMenuConfigDialog').collectData(BITMENUS.menu);

            jsonstring = convertToJsonString(BITMENUS.menu);
            BITAJAX.dataServiceUrl = "MenuService.asmx";

            BITAJAX.callWebServiceASync("SaveMenu", jsonstring, function (data) {
                BITMENUS.menu = data.d;

                //reload grid
                BITMENUS.loadMenus();
                $('#bitMenuConfigDialog').dialog("close");
                $('#bitScriptDialog').dialog("close");
                
            }, null, e.srcElement);

        }
    },

    removeMenu: function (id) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u dit menu met alle items verwijderen?", null, function () {

            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);

            BITAJAX.callWebServiceASync("DeleteMenu", jsonstring, function (data) {
                BITMENUS.menu = null;

                BITMENUS.loadMenus();
            });
        });
    },

    loadMenuItems: function (parentId, parentPath, sort, searchString) {
        BITMENUS.currentParentId = parentId;
        BITMENUS.currentParentPath = parentPath;
        BITMENUS.updateBreadCrump(parentPath);
        if (!parentPath) parentPath = "";
        if (!searchString) searchString = "";
        if (sort) BITMENUS.currentSort = sort;

        var parametersObject = {
            menuId: BITMENUS.menuId,
            parentId: parentId,
            parentPath: parentPath,
            sort: BITMENUS.currentSort
        };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "MenuService.asmx";

        BITAJAX.callWebServiceASync("GetMenuItems", jsonstring, function (data) {
            $("#tableMenuItems").dataBindList(data.d, {
                onRowBound: function (obj, index, html) {
                    if (parentId == "" && parentPath != "") {
                        //navigeren vanuit template: group id = "", path is not ""
                        //currentgroup id moet worden bewaard
                        BITMENUS.currentParentId = obj.ID;
                    }

                    var tempWrapperDiv = document.createElement('div');
                    $(tempWrapperDiv).html(html);

                    if (!obj.IsActive) {
                        $(tempWrapperDiv).find('tr').addClass("inactive");
                        $(tempWrapperDiv).find('tr').css("color", "gray");
                    }
                    if (BITMENUS.menuType == 1 || BITMENUS.menuType == 2) {
                        //voorkom drilldown naar sub menu's bij platte menu's
                        $(tempWrapperDiv).find('a.drillDown').attr('href', '#');
                        $(tempWrapperDiv).find('a.drillDown').css('text-decoration', 'none');
                        $(tempWrapperDiv).find('a.drillDown').css('border-bottom-style', 'none');
                        
                    }
                    if (BITMENUS.menuType == 5 && parentId != "") {
                        //bij accorionMenu's maar 1 laag diep
                        $(tempWrapperDiv).find('a.drillDown').attr('href', '#');
                        $(tempWrapperDiv).find('a.drillDown').css('text-decoration', 'none');
                        $(tempWrapperDiv).find('a.drillDown').css('border-bottom-style', 'none');
                    }
                    html = $(tempWrapperDiv).html();
                    return html;
                }
            });

            $('#tbodyItems').sortable({
                start: function (event, ui) {
                    var start_pos = ui.item.index();
                    ui.item.data('start_pos', start_pos);
                },
                stop: function (event, ui) {
                    var item = ui.item;
                    id = $(item).attr('id');
                    var parent = $(item).parent('tbody');
                    var start_pos = ui.item.data('start_pos');
                    var index = $(parent).children('tr').index(item);
                    //als index niet is veranderd, niets doen
                    if (start_pos == index) return;
                    //var containsReturnItem = ($(parent).children('tr').find('[id="00000000-0000-0000-0000-000000000000"]'))
                    var newOrderingNumer = index + 1//(containsReturnItem) ? 1 : 0;
                    BITAJAX.dataServiceUrl = "MenuService.asmx";
                    var parentItemId = BITMENUS.currentParentId;
                    if (!parentItemId) parentItemId = "";
                    var parametersObject = { itemId: id, parentItemId: parentItemId, menuId: BITMENUS.menuId, newOrderingNumber: newOrderingNumer };
                    var jsonstring = JSON.stringify(parametersObject);
                    BITAJAX.callWebServiceASync("UpdateOrderingNummerItem", jsonstring, function (data) {
                        BITMENUS.loadMenuItems(BITMENUS.currentParentId);
                    });
                }
            }).disableSelection();

        });


    },

    //search: function (searchText) {

    //    BITMENUS.loadData("", "", null, searchText);
    //},

    updateBreadCrump: function (path) {
        var pathList = [];
        var rootItem = new Object();
        rootItem.Path = "";
        rootItem.Name = "root";
        pathList.push(rootItem);

        if (path) {
            var pathItems = path.split("/");
            var subpath = "";
            for (var i in pathItems) {
                var subname = pathItems[i];
                if (subname != "") {
                    if (subpath != "") {
                        subpath += '/'
                    }
                    subpath += subname;
                    var item = new Object();
                    item.Path = subpath;
                    item.Name = subname;
                    pathList.push(item);
                }
            }
        }
        $('#breadcrump').dataBindList(pathList);
        //<ul>list stylen naar breadcrump
        $('#breadcrump').jBreadCrumb();

    },

    //loadDataTree_old: function (selectedId) {
    //    var timestamp = new Date().getTime();
    //    $('#treePanel').load('bitTrees/MenuDataTree.aspx?id=' + BITMENUS.menu.ID + '&' + timestamp, function () {
    //        $('#treePanel').treeview({
    //            sortable: true,
    //            sortstop: function (data) {
    //                var jsonstring = JSON.stringify(data);
    //                BITAJAX.callWebServiceASync("SaveOrderingNummerItem", jsonstring, null);
    //                //als zelfde item is geselecteerd dan veld volgnummer updaten
    //                if (BITMENUS.menuItem && BITMENUS.menuItem.ID == data.id) {
    //                    $("input[data-field=OrderingNumber]").val(data.orderingnumber);
    //                }
    //            },
    //            selectedIndexChanged: function (id, type) {
    //                if (id == 'treeRoot' || id == '') return;
    //                if (type == "item") {
    //                    BITMENUS.loadItem(id);
    //                }
    //                else if (type == "group") {
    //                    BITMENUS.loadGroup(id);
    //                }
    //                else if (type == "root") {
    //                    BITMENUS.loadCollectionDetails(id);
    //                }
    //            },
    //            setSelected: selectedId
    //        });
    //        $('#bitButtonSearch').click(function () {
    //            //BITPAGES.searchPage();
    //        });
    //    });
    //},

    fillDropDownParents: function () {
        BITAJAX.dataServiceUrl = "MenuService.asmx";
        var parametersObject = { menuId: BITMENUS.menuId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebServiceASync("GetAllMenuItemsLite", jsonstring, function (data) {
            BITMENUS.parentMenuItems = data.d;
            $("#selectParentMenuItems").fillDropDown(data.d, { value: BITUTILS.EMPTYGUID, text: "" });
        });

    },

    newMenuItem: function () {
        this.openMenuItemPopup(null);
    },





    openMenuItemPopup: function (id) {
        BITAJAX.dataServiceUrl = "MenuService.asmx";
        var parentId = BITMENUS.currentParentId;

        var parametersObject = { id: id, parentId: parentId, menuId: BITMENUS.menuId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebServiceASync("GetMenuItem", jsonstring, function (data) {

            var item = data.d;
            BITMENUS.menuItem = item;

            $('#bitMenuItemDetailsDialog').dataBind(item);
            BITMENUS.changeMenuItemUrlType();

            $('#selectParentMenuItems').val(parentId);

            if (BITMENUS.menuItem.IsNew) {
                $('#bitMenuItemDetailsDialog').dialog({ title: "Nieuw menuitem" });
                $('.bitTabs').tabs("select", 0);
            }
            else {
                $('#bitMenuItemDetailsDialog').dialog({ title: "Menuitem: " + item.Name });
            }

            $('#bitMenuItemDetailsDialog').dialog("open");
        });
    },

    changeMenuItemUrlType: function () {
        var type = $("input[name=urlType]:checked").val();
        if (type == "0") {
            $("#divPageLink").show();
            $("#divExternalLink").hide();
            $("#divTarget").show();
            $("#textExternalUrl").removeAttr("data-validation");
        }
        else if (type == "1") {
            $("#divExternalLink").show();
            $("#divPageLink").hide();
            $("#divTarget").show();
            $("#textExternalUrl").attr("data-validation", "required");
        }
        else if (type == "2") {
            $("#divExternalLink").hide();
            $("#divPageLink").hide();
            $("#divTarget").hide();
            $("#textExternalUrl").removeAttr("data-validation");
            $("#textExternalUrl").val("#");
        }
    },

    changePage: function () {
        if (BITMENUS.menuItem.IsNew || $("#textName").val() == "") {
            var pageUrl = $("#selectPages option:selected").text();;
            var pageName = pageUrl.split('/').pop();
            $("#textName").val(pageName);
        }
    },

    saveMenuItem: function (e) {

        var validation = $('#bitMenuItemDetailsDialog').validate();
        if (validation) {
            //get data from panel
            var item = BITMENUS.menuItem;
            item = $('#bitMenuItemDetailsDialog').collectData(item);
            if (!item.Menu) item.Menu = new Object();
            item.Menu.ID = BITMENUS.menuId;
            jsonstring = convertToJsonString(item);
            BITAJAX.dataServiceUrl = "MenuService.asmx";

            BITAJAX.callWebServiceASync("SaveMenuItem", jsonstring, function (data) {
                //reload tree
                //BITMENUS.loadDataTree(BITMENUS.menuItem.ID);
                //reload grid
                BITMENUS.loadMenuItems(BITMENUS.currentParentId, BITMENUS.currentParentPath);
                BITMENUS.fillDropDownParents();
            }, null, e.srcElement);

            $('#bitMenuItemDetailsDialog').dialog("close");
        }
    },



    removeMenuItem: function (id, type) {
        var msg = "Wilt u dit menuitem verwijderen?";


        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, msg, null, function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);

            BITAJAX.callWebServiceASync("DeleteMenuItem", jsonstring, function (data) {

                //reload tree
                //BITMENUS.loadDataTree();
                //reload grid
                BITMENUS.loadMenuItems(BITMENUS.currentParentId);
                BITMENUS.fillDropDownParents();
            });

        });
    },




    openImagePopup: function (target) {

        BITIMAGESPOPUP.target = target;
        $('#bitFileSelectDialog').dialog("open");
    },



    setImageUrl: function (src) {
        var target = BITIMAGESPOPUP.target;

        $("#img" + target).attr("src", src);
        $("#img" + target).show();
        $("#fileName" + target).html(src);
    },



    clearImage: function (target) {
        $("#img" + target).attr("src", "");
        $("#img" + target).hide();
        $("#fileName" + target).html("");
    },



    copyMenu: function (id, name) {
        if (name == undefined) {
            name = $('#' + id).find('.nameColumn').html();
        }

        var newName = name + ' - kopie';
        $.inputBox('Kopie naam:', 'Menu kopiëren', newName, function (e, value) {
            BITAJAX.dataServiceUrl = "MenuService.asmx";
            var newName = value;


            var parametersObject = { menuId: id, newName: newName };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebServiceASync("CopyMenu", jsonstring, function (data) {
                //reload grid
                BITMENUS.loadMenus();
            }, null);

        });
    },

    copyMenuItem: function (id, name) {
        if (name == undefined) {
            name = $('#' + id).find('.nameColumn').html();
        }

        var newName = name + ' - kopie';
        $.inputBox('Kopie naam:', 'Menuitem kopiëren', newName, function (e, value) {
            BITAJAX.dataServiceUrl = "MenuService.asmx";
            var newName = value;


            var parametersObject = { itemId: id, newName: newName };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebServiceASync("CopyMenuItem", jsonstring, function (data) {
                //reload grid
                BITMENUS.loadMenuItems(BITMENUS.currentParentId);
            }, null);

        });
    },

    //SCRIPTS

    openScriptsPopup: function () {
        $('#bitScriptDialog').dialog('open');
    },

    addScript: function (id, name) {
        var script = new Object();
        script.ID = id;
        script.CompleteName = name;

        if (BITMENUS.menu.Scripts == null) {
            BITMENUS.menu.Scripts = [];
        }
        //kijk of die er al in zit
        var contains = false;
        for (var i in BITMENUS.menu.Scripts) {
            var scriptCheck = BITMENUS.menu.Scripts[i];
            if (scriptCheck.ID == script.ID) {
                contains = true;
                break;
            }
        }
        
        if (contains) {
            $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "Koppeling met dit script bestaat al");
            //alert("Koppeling met dit script bestaat al");
        }
        else {
            BITMENUS.menu.Scripts.push(script);
            $("#tableScriptsPerMenu").dataBindList(BITMENUS.menu.Scripts);
        }
    },

    removeScript: function (id, index) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u dit script wilt los koppelen van dit menu?", null, function () {
            BITMENUS.menu.Scripts.splice(index, 1);
            $("#tableScriptsPerMenu").dataBindList(BITMENUS.menu.Scripts);
        });
    },


    editScript: function (id) {
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/Scripts/ScriptService.asmx";
        BITAJAX.callWebService("GetScript", jsonstring, function (data) {
            BITMENUS.currentScript = data.d;
            if (BITMENUS.codeMirrorScriptEditor != null) {
                BITMENUS.codeMirrorScriptEditor.toTextArea();
            }
            //BITSCRIPTS.script = data.d;
            //$("#bitScriptDialog").dataBind(BITSCRIPTS.script);
            $('#bitScriptEditDialog').dataBind(BITMENUS.currentScript);

            var scriptType = BITMENUS.currentScript.ScriptType;
            var uiOptions = { path: '/_bitplate/_js/plugins/codemirror-ui/js/', searchMode: 'inline', buttons: ['undo', 'redo', 'jump', 'reindentSelection', 'reindent'] };
            var codeMirrorOptions;
            if (data.d.ScriptType == 1) {
                codeMirrorOptions = { mode: "javascript", tabMode: "indent", lineNumbers: true };
                BITMENUS.codeMirrorScriptEditor = new CodeMirrorUI(document.getElementById('textareaContent'), uiOptions, codeMirrorOptions); //CodeMirror.fromTextArea(document.getElementById('textareaContent'), { mode: "javascript", tabMode: "indent", lineNumbers: true });
            }
            else {
                codeMirrorOptions = { mode: "css", tabMode: "indent", lineNumbers: true };
                BITMENUS.codeMirrorScriptEditor = new CodeMirrorUI(document.getElementById('textareaContent'), uiOptions, codeMirrorOptions); //CodeMirror.fromTextArea(document.getElementById('textareaContent'), { mode: "css", tabMode: "indent", lineNumbers: true });
            }

            $('#bitScriptEditDialog').dialog({ title: "Script: " + BITMENUS.currentScript.Name });
            $('#bitScriptEditDialog').dialog('open');

            var height = $('#bitScriptEditDialog').height() - 30;
            //$('#textareaContent').parent().height(height);
            BITMENUS.codeMirrorScriptEditor.mirror.refresh();

        });
    },

    saveScript: function () {
        BITMENUS.codeMirrorScriptEditor.toTextArea();
        BITMENUS.codeMirrorScriptEditor = null;
        BITMENUS.currentScript = $('#bitScriptEditDialog').collectData(BITMENUS.currentScript);
        //module.Page = BITMENUS.page;
        var jsonstring = convertToJsonString(BITMENUS.currentScript);

        BITAJAX.dataServiceUrl = "/_bitplate/Scripts/ScriptService.asmx";
        BITAJAX.callWebService("SaveScript", jsonstring, function (data) {
            //reload page
            $('#bitScriptEditDialog').dialog("close");
            var scriptID = BITMENUS.currentScript.ID;
            var scriptUrl = BITMENUS.currentScript.Url;
            if (BITMENUS.currentScript.Type == 'css') {
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


};