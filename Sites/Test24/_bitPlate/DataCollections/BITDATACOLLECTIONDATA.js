/// <reference path="BITBACKEND.js" />
/// <reference path="../prototypes/databind.js" />

var BITDATACOLLECTIONDATA = {
    datacollectionId: null,
    datacollection: null,
    type: "Group", //group of Item
    searchString: "",
    currentSort: "OrderNumber ASC",
    currentPage: 0, //voor paginering: wordt niet gebruikt
    pageSize: 0, //voor paginering: wordt niet gebruikt
    currentGroupId: "",
    currentGroupPath: "",
    showTabs: [],
    isTreeVisible: false, //wordt niet gebruikt
    currentLanguageCode: "",
    selectFileType: null,

    initialize: function () {
        BITAJAX.isHistorySupported = true;
        //laden van datacollection
        BITDATACOLLECTIONDATA.loadDataCollection();
        $('#bitDataGroupDetailsDialog').initDialog(BITDATACOLLECTIONDATA.saveGroup, { width: '90%', height: window.innerHeight });
        $('#bitDataItemDetailsDialog').initDialog(BITDATACOLLECTIONDATA.saveItem, { width: '90%', height: window.innerHeight });
        $('#bitFileSelectDialog').initDialog(null, { height: 500, width: 1000 });

        $('#bitPopups').formEnrich();

        BITDATACOLLECTIONDATA.fillDropDownGroups();

        $('#bitSearchTextbox').searchable(BITDATACOLLECTIONDATA.search);
        //maken van editors
        $(".wysiwygEditor").each(function (i) {
            var elmId = $(this).attr('id');
            CKEDITOR.editors[elmId] = CKEDITOR.replace(this, {
                filebrowserBrowseUrl: '/_bitplate/Dialogs/Finder.aspx',
                filebrowserImageBrowseUrl: '/_bitplate/Dialogs/Finder.aspx',
                filebrowserUploadUrl: '/_bitplate/Dialogs/Finder.aspx',
                filebrowserImageUploadUrl: '/_bitplate/Dialogs/Finder.aspx'
            });
        });

        $('#tableView tr[title="item"]').sortable();

        //BITIMAGESPOPUP.initialize();
        BITIMAGESPOPUP.target = "DataCollectionsData";

        //BITFILESPOPUP.initialize();
        BITFILESPOPUP.target = "DataCollectionsData";
    },

    loadDataCollection: function () {
        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
        var parametersObject = { id: BITDATACOLLECTIONDATA.datacollectionId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebServiceASync("GetDataCollection", jsonstring, function (data) {
            BITDATACOLLECTIONDATA.datacollection = data.d;
            $("#bitplateTitlePage").html("Datacollectie: " + BITDATACOLLECTIONDATA.datacollection.Name);
        });
    },


    loadData: function (groupid, groupPath, sort, searchString) {
        BITDATACOLLECTIONDATA.currentGroupId = groupid;
        BITDATACOLLECTIONDATA.currentGroupPath = groupPath;
        BITDATACOLLECTIONDATA.updateBreadCrump(groupPath);
        if (!groupPath) groupPath = "";
        if (!searchString) searchString = "";
        if (sort) BITDATACOLLECTIONDATA.currentSort = sort;

        var parametersObject = {
            datacollectionid: BITDATACOLLECTIONDATA.datacollectionId,
            groupid: groupid,
            groupPath: groupPath,
            sort: BITDATACOLLECTIONDATA.currentSort,
            searchString: searchString,
            pageNumber: BITDATACOLLECTIONDATA.currentPage,
            pageSize: BITDATACOLLECTIONDATA.pageSize
        };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
        BITAJAX.callWebServiceASync("GetGroups", jsonstring, function (data) {
            BITDATACOLLECTIONDATA.currentGroupId = groupid;
            $("#tbodyGroups").dataBindList(data.d, {
                onRowBound: function (obj, index, html) {
                    if (groupid == "" && groupPath != "") {
                        //navigeren vanuit template: group id = "", path is not ""
                        //currentgroup id moet worden bewaard
                        BITDATACOLLECTIONDATA.currentGroupId = obj.Field3;
                    }

                    var tempWrapperDiv = document.createElement('div');
                    $(tempWrapperDiv).html(html);

                    if (searchString != "") {
                        //toon kolom met groep-pad er in bij zoeken, want zoeken is in alle mappen
                        $(tempWrapperDiv).find('td:nth-child(2)').hide(); //volgnr
                        $(tempWrapperDiv).find('td:nth-child(4)').hide(); //titel
                        $(tempWrapperDiv).find('td:nth-child(5)').show(); //groep-pad
                    }
                    else {
                        $(tempWrapperDiv).find('td:nth-child(2)').show(); //volgnr
                        $(tempWrapperDiv).find('td:nth-child(4)').show();//titel
                        $(tempWrapperDiv).find('td:nth-child(5)').hide();//groep-pad
                    }
                    if (!obj.IsActive) {
                        $(tempWrapperDiv).find('tr').addClass("inactive");
                        $(tempWrapperDiv).find('tr').css("color", "gray");
                    }
                    if (obj.Name == '...') {
                        $(tempWrapperDiv).find('.bitConfigButton').hide();
                        $(tempWrapperDiv).find('.bitDeleteButton').hide();
                        $(tempWrapperDiv).find('.bitCopyButton').hide();
                    }
                    html = $(tempWrapperDiv).html();
                    return html;
                },
                onDataLoaded: function () {
                    if (BITDATACOLLECTIONDATA.isTreeVisible) {
                        $("#tableData .bitTitleColumn").hide();
                        $("#tableData .bitStatusColumn").hide();
                        $("#tableData .bitTableDateColumn").hide();
                    }
                    else {
                        $("#tableData .bitTitleColumn").show();
                        $("#tableData .bitStatusColumn").show();
                        $("#tableData .bitTableDateColumn").show();
                    }

                }
            });
            if (searchString == '') {
                $('#tbodyGroups').sortable({
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
                        var containsReturnItem = ($(parent).find('tr[id="00000000-0000-0000-0000-000000000000"]'))
                        var correction = (containsReturnItem.length == 1) ? 0 : 1;
                        var newOrderingNumer = index + correction;
                        //var newOrderingNumer = index + 1;
                        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
                        var parentGroupId = BITDATACOLLECTIONDATA.currentGroupId;
                        if (!parentGroupId) parentGroupId = "";
                        var parametersObject = { groupId: id, parentGroupId: parentGroupId, datacollectionId: BITDATACOLLECTIONDATA.datacollectionId, newOrderingNumber: newOrderingNumer };
                        var jsonstring = JSON.stringify(parametersObject);
                        BITAJAX.callWebServiceASync("UpdateOrderingNummerGroup", jsonstring, function (data) {
                            BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId);
                        });
                    }
                }).disableSelection();
            }
            else {
                if ($("#tbodyGroups").data('sortable')) {
                    $("#tbodyGroups").sortable("destroy");
                }
                
            }
        });
        BITAJAX.callWebServiceASync("GetItems", jsonstring, function (data) {
            $("#tbodyItems").dataBindList(data.d, {
                onRowBound: function (obj, index, html) {
                    if (groupid == "" && groupPath != "") {
                        //navigeren vanuit template: group id = "", path is not ""
                        //currentgroup id moet worden bewaard
                        BITDATACOLLECTIONDATA.currentGroupId = obj.Field3;
                    }

                    var tempWrapperDiv = document.createElement('div');
                    $(tempWrapperDiv).html(html);

                    if (searchString != "") {
                        //toon kolom met groep-pad er in bij zoeken, want zoeken is in alle mappen
                        $(tempWrapperDiv).find('td:nth-child(2)').hide(); //volgnr
                        $(tempWrapperDiv).find('td:nth-child(4)').hide(); //titel
                        $(tempWrapperDiv).find('td:nth-child(5)').show(); //groep-pad
                    }
                    else {
                        $(tempWrapperDiv).find('td:nth-child(2)').show(); //volgnr
                        $(tempWrapperDiv).find('td:nth-child(4)').show();//titel
                        $(tempWrapperDiv).find('td:nth-child(5)').hide();//groep-pad
                    }
                    if (!obj.IsActive) {
                        $(tempWrapperDiv).find('tr').addClass("inactive");
                        $(tempWrapperDiv).find('tr').css("color", "gray");
                    }
                    html = $(tempWrapperDiv).html();
                    return html;
                },
                onDataLoaded: function () {
                    if (BITDATACOLLECTIONDATA.isTreeVisible) {
                        $("#tableData .bitTitleColumn").hide();
                        $("#tableData .bitStatusColumn").hide();
                        $("#tableData .bitTableDateColumn").hide();
                    }
                    else {
                        $("#tableData .bitTitleColumn").show();
                        $("#tableData .bitStatusColumn").show();
                        $("#tableData .bitTableDateColumn").show();
                    }

                }
            });
            if (searchString == '') {
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
                        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
                        var parentGroupId = BITDATACOLLECTIONDATA.currentGroupId;
                        if (!parentGroupId) parentGroupId = "";
                        var parametersObject = { itemId: id, parentGroupId: parentGroupId, datacollectionId: BITDATACOLLECTIONDATA.datacollectionId, newOrderingNumber: newOrderingNumer };
                        var jsonstring = JSON.stringify(parametersObject);
                        BITAJAX.callWebServiceASync("UpdateOrderingNummerItem", jsonstring, function (data) {
                            BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId);
                        });
                    }
                }).disableSelection();
           }
            else {
                if ($("#tbodyItems").data('sortable')) {
                    $("#tbodyItems").sortable("destroy");
                }
           }
        });
        //titel rij in tabel
        if (searchString != "") {
            //toon kolom met groep-pad er in bij zoeken, want zoeken is in alle mappen
            $('#tableView td:nth-child(2)').hide(); //volgnr
            $('#tableView td:nth-child(4)').hide(); //titel
            $('#tableView td:nth-child(5)').show(); //groep-pad
        }
        else {
            $('#tableView td:nth-child(2)').show(); //volgnr
            $('#tableView td:nth-child(4)').show(); //titel
            $('#tableView td:nth-child(5)').hide(); //groep-pad 
        }
    },

    search: function (searchText) {

        BITDATACOLLECTIONDATA.loadData("", "", null, searchText);
    },

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
    loadDataTree_old: function (selectedId) {
        var timestamp = new Date().getTime();
        $('#treePanel').load('bitTrees/DataCollectionDataTree.aspx?id=' + BITDATACOLLECTIONDATA.dataCollection.ID + '&' + timestamp, function () {
            $('#treePanel').treeview({
                sortable: true,
                sortstop: function (data) {
                    var jsonstring = JSON.stringify(data);
                    BITAJAX.callWebServiceASync("SaveOrderingNummerItem", jsonstring, null);
                    //als zelfde item is geselecteerd dan veld volgnummer updaten
                    if (BITDATACOLLECTIONDATA.loadedDomainObject && BITDATACOLLECTIONDATA.loadedDomainObject.ID == data.id) {
                        $("input[data-field=OrderingNumber]").val(data.orderingnumber);
                    }
                },
                selectedIndexChanged: function (id, type) {
                    if (id == 'treeRoot' || id == '') return;
                    if (type == "item") {
                        BITDATACOLLECTIONDATA.loadItem(id);
                    }
                    else if (type == "group") {
                        BITDATACOLLECTIONDATA.loadGroup(id);
                    }
                    else if (type == "root") {
                        BITDATACOLLECTIONDATA.loadCollectionDetails(id);
                    }
                },
                setSelected: selectedId
            });
            $('#bitButtonSearch').click(function () {
                //BITPAGES.searchPage();
            });
        });
    },

    fillDropDownGroups: function () {
        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
        var parametersObject = { datacollectionid: BITDATACOLLECTIONDATA.datacollectionId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebServiceASync("GetAllGroupsLite", jsonstring, function (data) {
            BITDATACOLLECTIONDATA.groups = data.d;
            $("#selectParentGroup_Groups").fillDropDown(data.d, { value: BITUTILS.EMPTYGUID, text: "" });
            $("#selectParentGroup_Items").fillDropDown(data.d, { value: BITUTILS.EMPTYGUID, text: "" });
        });

    },

    newDataItem: function () {
        this.openItemPopup(null);
    },

    newDataGroup: function () {
        this.openGroupPopup(null);
    },

    openDetailsPopup: function (id, type) {
        if (type == "Item") {
            BITDATACOLLECTIONDATA.openItemPopup(id);
        }
        else if (type == "Group") {
            BITDATACOLLECTIONDATA.openGroupPopup(id);
        }
    },

    openItemPopup: function (id) {
        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
        var groupid = BITDATACOLLECTIONDATA.currentGroupId;

        var parametersObject = { id: id, groupid: groupid, datacollectionid: BITDATACOLLECTIONDATA.datacollectionId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebServiceASync("GetItem", jsonstring, function (data) {
            //var jsonString = data.d;
            //var item = $.parseJSON(jsonString);
            var item = data.d;
            BITDATACOLLECTIONDATA.loadedDomainObject = item;
            BITDATACOLLECTIONDATA.type = "Item";
            //voor databind aan languages
            for (var i in item.DataItemLanguages) {
                //we stoppen de taal-item object in een associatieve array
                //dus item.DataItemLanguage[0] komt in bijvoorbeeld item["EN"] te staan
                //databind gaat met databind="EN.Title"
                var language = item.DataItemLanguages[i];
                item[language.LanguageCode] = language;
            }

            $('#bitDataItemDetailsDialog').dataBind(item);
            $('#tableExtraImages').dataBindList(item.ImageList1);
            $('#tableExtraFiles').dataBindList(item.FileList1);
            $('#selectParentGroup_Items').val(groupid);
            //BITDATACOLLECTIONDATA.setLanguageValuesInControls("Item");

            if (BITDATACOLLECTIONDATA.loadedDomainObject.IsNew) {
                $('#bitDataItemDetailsDialog').dialog({ title: "Nieuw item" });
                $('.bitTabs').tabs("select", 0);
            }
            else {
                $('#bitDataItemDetailsDialog').dialog({ title: "Item: " + item.Name });
            }

            $('#bitDataItemDetailsDialog').dialog("open");
            BITDATACOLLECTIONDATA.changeLanguage("Items");
        });
    },

    openGroupPopup: function (id) {
        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
        var groupid = BITDATACOLLECTIONDATA.currentGroupId;
        var parametersObject = { id: id, parentgroupid: groupid, datacollectionid: BITDATACOLLECTIONDATA.datacollectionId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebServiceASync("GetGroup", jsonstring, function (data) {
            var group = data.d;
            BITDATACOLLECTIONDATA.loadedDomainObject = group;
            BITDATACOLLECTIONDATA.type = "Group";

            //voor databind aan languages
            for (var i in group.DataGroupLanguages) {
                //we stoppen de taal-item object in een associatieve array
                //dus item.DataGroupLanguages[0] komt in bijvoorbeeld group["EN"] te staan
                //databind gaat dan met databind="datagroup.EN.Title"
                var language = group.DataGroupLanguages[i];
                group[language.LanguageCode] = language;
            }

            $('#bitDataGroupDetailsDialog').dataBind(group);
            $('#tableExtraImages').dataBindList(group.ImageList1);
            $('#tableExtraFiles').dataBindList(group.FileList1);
            $('#selectParentGroup_Groups').val(groupid);
            if (BITDATACOLLECTIONDATA.loadedDomainObject.IsNew) {
                $('#bitDataGroupDetailsDialog').dialog({ title: "Nieuwe groep" });
                $('.bitTabs').tabs("select", 0);
            }
            else {
                $('#bitDataGroupDetailsDialog').dialog({ title: "Group: " + BITDATACOLLECTIONDATA.loadedDomainObject.Name });
            }

            $('#bitDataGroupDetailsDialog').dialog("open");
            BITDATACOLLECTIONDATA.changeLanguage("Groups");
        });
    },

    saveItem: function (e) {

        var validation = $('#bitDataItemDetailsDialog').validate();
        if (validation) {
            //get data from panel
            var item = BITDATACOLLECTIONDATA.loadedDomainObject;
            item = $('#bitDataItemDetailsDialog').collectData(item);
            item.DataCollection = BITDATACOLLECTIONDATA.datacollection;
            //taal is gedatabind als bijvoorbeeld item.EN.Text1.
            //hieronder wordt taal weer uit taalobject met naam =taalcode in dataitem.DataItemLanguages gezet
            for (var i in item.DataItemLanguages) {
                var language = item.DataItemLanguages[i];
                item.DataItemLanguages[i] = item[language.LanguageCode];
            }
            jsonstring = convertToJsonString(item);
            BITAJAX.dataServiceUrl = "DataCollectionService.asmx";

            BITAJAX.callWebServiceASync("SaveItem", jsonstring, function (data) {
                //reload tree
                //BITDATACOLLECTIONDATA.loadDataTree(BITDATACOLLECTIONDATA.loadedDomainObject.ID);
                //reload grid
                BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId, BITDATACOLLECTIONDATA.currentGroupPath);
            }, null, e.srcElement);

            $('#bitDataItemDetailsDialog').dialog("close");
        }
    },

    saveGroup: function (e) {

        var validation = $('#bitDataGroupDetailsDialog').validate();
        if (validation) {
            //get data from panel
            var group = BITDATACOLLECTIONDATA.loadedDomainObject;
            group = $('#bitDataGroupDetailsDialog').collectData(group);
            group.DataCollection = BITDATACOLLECTIONDATA.datacollection;

            //taal is gedatabind als bijvoorbeeld item.EN.Text1.
            //hieronder wordt taal weer uit taalobject met naam =taalcode in datagroup.DataGroupLanguages gezet
            for (var i in group.DataGroupLanguages) {
                var language = group.DataGroupLanguages[i];
                group.DataGroupLanguages[i] = group[language.LanguageCode];
            }

            jsonstring = convertToJsonString(group);
            BITAJAX.dataServiceUrl = "DataCollectionService.asmx";

            BITAJAX.callWebServiceASync("SaveGroup", jsonstring, function (data) {
                //reload tree
                //BITDATACOLLECTIONDATA.loadDataTree(BITDATACOLLECTIONDATA.loadedDomainObject.ID);
                //relaod valt onder select box
                BITDATACOLLECTIONDATA.fillDropDownGroups();
                //reload grid
                BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId, BITDATACOLLECTIONDATA.currentGroupPath);
            }, null, e.srcElement);

            $('#bitDataGroupDetailsDialog').dialog("close");
        }
    },

    remove: function (id, type) {
        var msg = "Wilt u dit item verwijderen?";
        if (type == "Group") {
            msg = "Wilt u deze groep (met alle subgroepen en items) verwijderen?";
        }

        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, msg, null, function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            if (type == "Item") {
                BITAJAX.callWebServiceASync("DeleteItem", jsonstring, function (data) {

                    //reload tree
                    //BITDATACOLLECTIONDATA.loadDataTree();
                    //reload grid
                    BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId);
                });
            }
            else if (type == "Group") {
                BITAJAX.callWebServiceASync("DeleteGroup", jsonstring, function (data) {

                    //reload tree
                    //BITDATACOLLECTIONDATA.loadDataTree();
                    BITDATACOLLECTIONDATA.fillDropDownGroups();
                    //reload grid
                    BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId);
                });
            }
        });
    },

    activate: function (button) {
        if ($(button).html() == "activeer") {
            $("#dateFrom").val("");
            $("#dateTill").val("");
            $(button).html("deactiveer");
        }
        else {
            var date = new Date();
            var day = date.getDate();
            day--;
            date = new Date(date.getFullYear(), date.getMonth(), day);
            $("#dateTill").val(date.format("dd-MM-yyyy"));
            $(button).html("activeer");
        }
    },


    openImagePopup: function (target, language) {
        /* if (!BITIMAGESPOPUP.isInitialized) {
            BITIMAGESPOPUP.initialize();
        }
        BITIMAGESPOPUP.target = target;
        BITIMAGESPOPUP.language = language;
        //BITIMAGESPOPUP.type = type;
        //BITIMAGESPOPUP.fieldName = fieldName;
        $('#bitImagesDialog').dialog("open"); */
        BITDATACOLLECTIONDATA.selectFileType = 'image';
        BITIMAGESPOPUP.target = target;
        BITIMAGESPOPUP.language = language;
        $('#bitFileSelectDialog').dialog("open");
    },

    openFilesPopup: function (target, language) {
        /* if (!BITFILESPOPUP.isInitialized) {
            BITFILESPOPUP.initialize();
        }
        BITFILESPOPUP.target = target;
        BITFILESPOPUP.language = language;
        $('#bitFilesDialog').dialog("open"); */
        BITDATACOLLECTIONDATA.selectFileType = 'file';
        BITFILESPOPUP.target = target;
        BITFILESPOPUP.language = language;
        $('#bitFileSelectDialog').dialog("open");
    },

    setImageUrl: function (src) {
        var target = BITIMAGESPOPUP.target;
        var language = BITIMAGESPOPUP.language;
        //var type = BITIMAGESPOPUP.type;
        if (target == "ExtraImages") {
            var extraImage = new Object();

            extraImage.Url = src;
            extraImage.Title = src;
            extraImage.ID = "00000000-0000-0000-0000-000000000000";
            extraImage.Type = "ExtraImage";
            extraImage.Language = language;
            if (BITDATACOLLECTIONDATA.loadedDomainObject.ImageList1 == null) {
                BITDATACOLLECTIONDATA.loadedDomainObject.ImageList1 = [];
            }
            BITDATACOLLECTIONDATA.loadedDomainObject.ImageList1.push(extraImage);
            var imageList = BITDATACOLLECTIONDATA.loadedDomainObject.ImageList1;
            if (language) {
                imageList = BITDATACOLLECTIONDATA.getImagesByLanguage(language);
            }
            $('#tableExtraImages').dataBindList(imageList);
        }
        else {
            if (!language) language = "";
            $("#img" + target + language).attr("src", src);
            $("#img" + target + language).show();
            $("#fileName" + target + language).html(src);
        }
    },

    setFileUrl: function (src) {
        var target = BITFILESPOPUP.target;
        var language = BITFILESPOPUP.language;
        //var type = BITIMAGESPOPUP.type;
        if (target == "ExtraFiles") {
            var extraFile = new Object();
            extraFile.Url = src;
            extraFile.Title = src;
            extraFile.ID = "00000000-0000-0000-0000-000000000000";
            extraFile.Type = "ExtraFile";
            extraFile.Language = "ExtraFile";
            if (BITDATACOLLECTIONDATA.loadedDomainObject.FileList1 == null) {
                BITDATACOLLECTIONDATA.loadedDomainObject.FileList1 = [];
            }
            BITDATACOLLECTIONDATA.loadedDomainObject.FileList1.push(extraFile);

            var fileList = BITDATACOLLECTIONDATA.loadedDomainObject.FileList1;
            if (language) {
                fileList = BITDATACOLLECTIONDATA.getFilesByLanguage(language);
            }
            $('#tableExtraFiles').dataBindList(fileList);
            //$('#tableExtraFiles').dataBindList(BITDATACOLLECTIONDATA.loadedDomainObject.FileList1);
        }
        else {
            if (!language) language = "";
            $("#fileIcon" + target + language).attr("src", src);
            $("#fileIcon" + target + language).show();
            $("#fileName" + target + language).html(src);
        }
    },

    clearImage: function (target, language) {
        if (!language) language = "";
        $("#img" + target + language).attr("src", "");
        $("#img" + target + language).hide();
        $("#fileName" + target + language).html("");
    },

    clearFile: function (target, language) {
        if (!language) language = "";
        $("#fileIcon" + target + language).attr("src", "");
        $("#fileIcon" + target + language).hide();
        $("#fileName" + target + language).html("");
    },

    removeExtraFile: function (id, index) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Wilt u deze afbeelding los koppelen?", null, function () {
            if (id == "00000000-0000-0000-0000-000000000000") {
                //nieuwe files, alleen uit array halen
                BITDATACOLLECTIONDATA.loadedDomainObject.ImageList1.splice(index, 1);
                $('#tableExtraImages').dataBindList(BITDATACOLLECTIONDATA.loadedDomainObject.ImageList1);
            }
            else {
                //meteen op server
                BITAJAX.dataServiceUrl = "DataCollectionService.asmx";

                var parametersObject = { fileId: id, itemId: BITDATACOLLECTIONDATA.loadedDomainObject.ID };
                var jsonstring = JSON.stringify(parametersObject);

                BITAJAX.callWebServiceASync("RemoveExtraImage", jsonstring, function (data) {
                    BITDATACOLLECTIONDATA.loadedDomainObject.ImageList1.splice(index, 1);
                    $('#tableExtraImages').dataBindList(BITDATACOLLECTIONDATA.loadedDomainObject.ImageList1);
                });
            }
        });

    },

    download: function () {
        document.getElementById("iframeTransferFiles").src = "DataCollectionService.asmx?download=" + BITDATACOLLECTIONDATA.dataCollection.ID;
    },



    showUploadDiv: function () {
        $('#divUploadFiles').show();
    },

    selectFile: function () {
        var fileInput = document.getElementById("file1");
        document.form1.target = 'iframeTransferFiles';
        document.form1.submit();
        //reset
        fileInput.setAttribute('type', 'input');
        fileInput.setAttribute('type', 'file');
    },

    importData: function () {
        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";

        var parametersObject = { datacollectionId: BITDATACOLLECTIONDATA.dataCollection.ID };
        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.callWebServiceASync("ImportData", jsonstring, function (data) {
            //reload tree
            BITBACKEND.loadDataCollectionData(BITDATACOLLECTIONDATA.dataCollection.ID);
            $('#iframeTransferFiles').html("");
            //hide div
            $('#divUploadFiles').hide(700);
        });

        
    },
    cancelSend: function () {
        //hide div
        $('#divUploadFiles').hide(700);
    },

    copy: function (id, type, name) {

        if (type == 'Item') {                                                   //BUG #39
            if (name == undefined) {
                name = $('#' + id).find('.nameColumn').html();
            }
        }
        else {
            if (name == undefined) {
                name = $('#' + id).find('.nameColumn').find('a').html();
            }
        }


        var newName = name + ' - kopie';
        $.inputBox('Kopie naam:', 'Data kopiëren', newName, function (e, value) {
            BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
            var newName = value;
            if (type == "Item") {
                //var parentGroupId = BITDATACOLLECTIONDATA.currentGroupId;

                var parametersObject = { itemId: id, newName: newName };
                var jsonstring = JSON.stringify(parametersObject);
                BITAJAX.callWebServiceASync("CopyItem", jsonstring, function (data) {
                    //reload grid
                    BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId);
                },null);
            }
            else if (type == "Group") {
                //var parentGroupId = BITDATACOLLECTIONDATA.currentGroupId;
                var parametersObject = { groupId: id, newName: newName }; var jsonstring = JSON.stringify(parametersObject);
                BITAJAX.callWebServiceASync("CopyGroup", jsonstring, function (data) {
                    //reload grid
                    BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId);
                    BITDATACOLLECTIONDATA.fillDropDownGroups();
                }, null);
            }
        });
    },

    changeLanguage: function (type) {
        //make controls visible 
        var languageCode = $("#dropdownLanguages_" + type).val();
        if (languageCode) {
            //eerst alle verbergen
            $("[data-language]").hide();
            //daarna taal aanzetten
            $("[data-language=" + languageCode + "]").show();
            var imageList = BITDATACOLLECTIONDATA.getImagesByLanguage(languageCode);
            $('#tableExtraImages').dataBindList(imageList);
            var fileList = BITDATACOLLECTIONDATA.getFilesByLanguage(languageCode);
            $('#tableExtraFiles').dataBindList(fileList);
        }
    },

    getImagesByLanguage: function (languageCode) {
        //selecteer alleen degene met de taal
        var imageList = [];
        for (var i in BITDATACOLLECTIONDATA.loadedDomainObject.ImageList1) {
            var extraImg = BITDATACOLLECTIONDATA.loadedDomainObject.ImageList1[i];
            if (extraImg.Language == languageCode) {
                imageList.push(extraImg);
            }
        }
        return imageList;
    },

    getFilesByLanguage: function (languageCode) {
        //selecteer alleen degene met de taal
        var fileList = [];
        for (var i in BITDATACOLLECTIONDATA.loadedDomainObject.FileList1) {
            var extraFile = BITDATACOLLECTIONDATA.loadedDomainObject.FileList1[i];
            if (extraFile.Language == languageCode) {
                fileList.push(extraFile);
            }
        }
        return fileList;
    },

    changeLanguage_old: function (type) {
        BITDATACOLLECTIONDATA.keepCurrentLanguageValuesFromControls(type);
        BITDATACOLLECTIONDATA.setLanguageValuesInControls(type);
    },

    keepCurrentLanguageValuesFromControls_old: function (type) {
        var item = BITDATACOLLECTIONDATA.loadedDomainObject;
        var languages = item.DataItemLanguages;
        var fields = BITDATACOLLECTIONDATA.datacollection.DataItemFields;

        var language;
        for (var j in languages) {
            if (languages[j].LanguageCode == BITDATACOLLECTIONDATA.currentLanguageCode) {
                language = languages[j];
            }
        }

        for (var i in fields) {
            var field = fields[i];
            if (field.IsMultiLanguageField) {
                var fieldName = field.MappingColumn;
                var fieldValue = $("input[data-language-field=" + fieldName + "]").val();
                //var fieldValue = $("input[data-field=" + fieldName + "][data-language=" + BITDATACOLLECTIONDATA.currentLanguageCode + "]").val();
                //eerst huidige waardes opslaan
                if (BITDATACOLLECTIONDATA.currentLanguageCode == "") {
                    //item = $('#bitDataItemDetailsDialog').collectData(item);
                    item[fieldName] = fieldValue;
                }
                else {
                    language[fieldName] = fieldValue;
                }
            }
        }

    },

    setLanguageValuesInControls_new: function (type) {
        var item = BITDATACOLLECTIONDATA.loadedDomainObject;
        var languages = item.DataItemLanguages;
        var fields = BITDATACOLLECTIONDATA.datacollection.DataItemFields;
        var languageCode = $("#dropdownLanguages_Items").val();
        //de default language heeft lege string als taalcode. Deze wordt niet opgeslagen als taal
        //eerst alle onzichtbaar maken
        $("input[data-language]").hide();
        //taal zichtbaar maken
        $("input[data-language=" + languageCode + "]").show();

        BITDATACOLLECTIONDATA.currentLanguageCode = languageCode;
    },

    setLanguageValuesInControls: function (type) {
        var item = BITDATACOLLECTIONDATA.loadedDomainObject;
        var languages = item.DataItemLanguages;
        var fields = BITDATACOLLECTIONDATA.datacollection.DataItemFields;
        var languageCode = $("#dropdownLanguages_Items").val();
        //de default language heeft lege string als taalcode. Deze wordt niet opgeslagen als taal

        var language;
        for (var j in languages) {
            if (languages[j].LanguageCode == languageCode) {
                language = languages[j];
            }
        }

        for (var i in fields) {
            var field = fields[i];
            if (field.IsMultiLanguageField) {
                var fieldName = field.MappingColumn;
                var fieldValue;
                if (languageCode == "") {
                    fieldValue = BITDATACOLLECTIONDATA.loadedDomainObject[fieldName];
                }
                else {
                    fieldValue = language[fieldName];
                }
                //per type afhankelijk
                if (field.FieldType == FIELD_TYPES.IMAGE) {
                }
                $("input[data-language-field=" + fieldName + "]").val(fieldValue);
            }
        }
        //}
        BITDATACOLLECTIONDATA.currentLanguageCode = languageCode;
    },
    ///////////////////////
    //TREE
    ////////////////////////
    loadTree: function () {
        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
        var parametersObject = { datacollectionID: BITDATACOLLECTIONDATA.datacollection.ID, searchString: '' };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetTreeHtml", jsonstring, function (data) {
            $('#treeView').html(data.d);
            $('#treeView').treeview({
                sortable: true,
                sortstop: function (data) {
                    var jsonstring = JSON.stringify(data);
                    BITAJAX.callWebService("SaveOrderingNummerItem", jsonstring, null);
                    //als zelfde item is geselecteerd dan veld volgnummer updaten
                    if (BITDATACOLLECTIONDATA.loadedDomainObject && BITDATACOLLECTIONDATA.loadedDomainObject.ID == data.id) {
                        $("input[data-field=OrderingNumber]").val(data.orderingnumber);
                    }
                },
                selectedIndexChanged: function (id, type) {
                    if (id == 'treeRoot' || id == '') return;
                    if (type == "item") {
                        //BITDATACOLLECTIONDATA.loadItem(id);
                    }
                    else if (type == "group") {
                        //BITDATACOLLECTIONDATA.loadGroup(id);
                    }
                    else if (type == "root") {
                        // BITDATACOLLECTIONDATA.loadCollectionDetails(id);
                    }
                }
            });
        });
    },

    showTree: function () {
        BITDATACOLLECTIONDATA.isTreeVisible = true;
        $("#treeView").show();
        if ($("#treeView").html() == "") { BITDATACOLLECTIONDATA.loadTree(); }
        var html = "<a href='javascript:BITDATACOLLECTIONDATA.hideTree();' class='bitNavBarButtonHideTree'></a><div>Verb.tree</div>";
        $("#toolbarMenuItemShowTree").html(html);
        tableData
        $("#tableData .bitTitleColumn").hide();
        $("#tableData .bitStatusColumn").hide();
        $("#tableData .bitTableDateColumn").hide();

        $("#tableView").width(670);
    },

    hideTree: function () {
        BITDATACOLLECTIONDATA.isTreeVisible = false;
        $("#treeView").hide();
        var html = "<a href='javascript:BITDATACOLLECTIONDATA.showTree();' class='bitNavBarButtonShowTree'></a><div>Toon tree</div>";
        $("#toolbarMenuItemShowTree").html(html);
        $("#tableData .bitTitleColumn").show();
        $("#tableData .bitStatusColumn").show();
        $("#tableData .bitTableDateColumn").show();
        $("#tableView").width(1000);
    },

    saveJStreeDragFile: function (data) {
        var targetGroupId = $(data.r.context).attr('data-groupid');
        var fileRecord = $(data.o).parent().attr('id');
        var itemType = $(data.o).parent().attr('title')
        var index = $(data.o).parent().parent().children().index($(data.o).parent());

        var parametersObject = { parentid: targetGroupId, id: fileRecord, type: itemType, orderingnumber: index };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("SaveOrderingNummerItem", jsonstring, function (data) {
            //BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder);
        });

    },

    saveJStreeMove: function (data) {
        var id = $(data.rslt.r.context).attr('id');
        //console.log(data.rslt.r.context);

        /* var sourceFolders = [];
        $(data.rslt.o).each(function (i) {
            var sourceFolder = $(data.rslt.o[i]).find('a').attr('data-path');
            sourceFolders[sourceFolders.length] = sourceFolder;
            var destination = targetFolder + sourceFolder.substr(sourceFolder.lastIndexOf('\\'));
            $(data.rslt.o[i]).find('a').attr('data-path', destination);
        }); */
    }
};