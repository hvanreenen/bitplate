/// <reference path="BITBACKEND.js" />

var BITDATACOLLECTIONDATA = {
    datacollectionId: null,
    datacollection: null,
    type: "Group",
    searchString: "",
    currentSort: "OrderingNumber ASC",
    currentPage: 1,
    pageSize: 25,
    currentGroup: null,
    currentGroupId: null,
    showTabs: [],
    isTreeVisible: false,

    initialize: function () {
        //laden van datacollection
        this.loadDataCollection();

        $('#bitDataDetailsDialog').initDialog(BITDATACOLLECTIONDATA.save);

        BITIMAGESPOPUP.initialize();
        BITIMAGESPOPUP.target = "DataCollectionsData";

        $('#bitPopups').formEnrich();

        BITDATACOLLECTIONDATA.fillDropDownGroups();

        //maken van editors
        var toolbar1 = BITEDITOR.createToolbar("bitToolbar1");
        var toolbar2 = BITEDITOR.createToolbar("bitToolbar2");
        toolbar1.toolbarItems["tags"].hide();
        toolbar1.toolbarItems["styles"].hide();
        toolbar2.toolbarItems["tags"].hide();
        toolbar2.toolbarItems["styles"].hide();

        BITEDITOR.createEditor("bitEditor1");
        BITEDITOR.createEditor("bitEditor2");
        BITEDITOR.editors["bitEditor1"].toolbar = toolbar1;
        BITEDITOR.editors["bitEditor2"].toolbar = toolbar2;

        $('#tableView tr[title="item"]').sortable();
    },

    loadDataCollection: function () {
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
        var parametersObject = { id: BITDATACOLLECTIONDATA.datacollectionId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetDataCollection", jsonstring, function (data) {
            BITDATACOLLECTIONDATA.datacollection = data.d;
            $("#bitplateTitlePage").html("Gegevensverzameling: " + BITDATACOLLECTIONDATA.datacollection.Name);
            BITDATACOLLECTIONDATA.makeFieldsVisible();
        });

    },

    makeFieldsVisible: function () {
        //in datacollectie kun je velden aan en uitzetten
        //hier worden deze velden getoond.
        if (!this.datacollection) return;
        var showTabs = BITDATACOLLECTIONDATA.showTabs;
        var fields = this.datacollection.Fields;
        for (var i in fields) {
            var fieldName = fields[i];
            if (fieldName == "Image" || fieldName == "Thumbnail" || fieldName == "ExtraImages") {
                if (!showTabs["tabPage2"]) showTabs.push("tabPage2");
            }
            else if (fieldName == "Introduction") {
                showTabs.push("tabPage3");
            }
            else if (fieldName == "Content") {
                showTabs.push("tabPage4");
            }
            if (fieldName == "Address" || fieldName == "PostalCode" || fieldName == "City") {
                if (!showTabs["tabPage5"]) showTabs.push("tabPage5");
            }
            $("#bitField" + fieldName).show();
        }

        for (var i in showTabs) {
            var tabName = showTabs[i];
            $("#" + tabName).show();
        }
    },

    loadData: function (groupid, sort, pagenumber, groupName) {
        if (groupid != BITDATACOLLECTIONDATA.currentGroupId) {
            BITDATACOLLECTIONDATA.currentGroupId = groupid;
            BITDATACOLLECTIONDATA.currentPage = 1;
            if($("#treeView").html() != ""){
                $.jstree._focused().select_node(groupid);
            }
        }
        //folderName = (folderName == '...') ? 'Root' : folderName;
        //folderid = (folderid == '') ? '00000000-0000-0000-0000-000000000000' : folderid;
        //if (folderName != undefined) {
        //    BITPAGES2.updateBreadCrumpFolderCollection(folderid, folderName);
        //}

        //Console.log(BITPAGES2.folderCollection);
        //$('#treePanel').treeview({
        //    setSelected: folderid
        //});
        if (sort) BITDATACOLLECTIONDATA.currentSort = sort;
        if (pagenumber) BITDATACOLLECTIONDATA.currentpage = pagenumber;
        var parametersObject = {
            datacollectionid: BITDATACOLLECTIONDATA.datacollectionId,
            groupid: groupid,
            sort: BITDATACOLLECTIONDATA.currentSort,
            searchString: BITDATACOLLECTIONDATA.searchString,
            pageNumber: BITDATACOLLECTIONDATA.currentPage,
            pageSize: BITDATACOLLECTIONDATA.pageSize
        };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
        BITAJAX.callWebService("GetDataTotalCount", jsonstring, function (data) {
            var totalRowCount = data.d;
            $("#paging").createPager(totalRowCount, {
                onPageIndexChanged: function (pageNumber) {
                    BITDATACOLLECTIONDATA.currentPage = pageNumber;
                    BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId, null);
                },
                pageSize: BITDATACOLLECTIONDATA.pageSize,
                pageNumber: BITDATACOLLECTIONDATA.currentPage
            });
        });
        BITAJAX.callWebService("GetData", jsonstring, function (data) {
            //if (BITPAGES2.treemode && insertIntoRow || insertIntoRow >= 0) {
            //    var copy = $("#tablePages").clone();
            //    $(copy).dataBindList(data.d, {
            //        onRowBound: function (obj, index, html) {
            //            if (obj.Type == "Folder") {
            //                var div = document.createElement('div');
            //                $(div).html(html);
            //                $(div).find('.editButton').hide();
            //                $(div).find('.liveButton').hide();
            //                if (BITPAGES2.treeMode) {
            //                    $(div).find('.icon').html("&nbsp;&nbsp;&nbsp;" + $(div).find('.icon').html());
            //                    if (!obj.IsLeaf) {
            //                        $(div).find('.expand').html("<a href='javascript:BITPAGES2.loadPages(\"" + obj.ID + "\", + " + index + "\"," + obj.Name + "\" );'>&#9658;</a>");

            //                    }
            //                    $(div).find('.expand').html("&nbsp;&nbsp;&nbsp;" + $(div).find('.expand').html());
            //                }
            //                else {
            //                }
            //                html = $(div).html();
            //            }
            //            return html;
            //        }

            //    });

            //    var html = $(copy).find('tbody').html();
            //    $('#tablePages > tbody > tr').eq(insertIntoRow).after(html);
            //}

            //else {
            $("#selectGroups").val(groupid);
            if (BITDATACOLLECTIONDATA.searchString != "") {
                //toon kolom met pad er in bij zoeken, want zoeken is in alle mappen
                $("#tableData .groupColumn").show();
            }
            else {
                $("#tableData .groupColumn").hide();
            }
            $("#tableData").dataBindList(data.d, {
                onSort: function (sort) {
                    BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId, sort);
                },

                onRowBound: function (obj, index, html) {
                    if (BITDATACOLLECTIONDATA.searchString != "") {
                        //toon kolom met pad er in bij zoeken, want zoeken is in alle mappen
                        var div = document.createElement('div');
                        $(div).html(html);
                        $(div).find('.groupColumn').show();
                        //html = html.highlight(BITPAGES2.searchString);
                        html = $(div).html();
                    }
                    if (obj.Status == "Niet actief") {
                        var div = document.createElement('div');
                        $(div).html(html);
                        $(div).find('tr').addClass("inactive");
                        $(div).find('tr').css("color", "gray");
                        html = $(div).html();
                    }

                    if (obj.Type == "Group") {
                        var div = document.createElement('div');
                        $(div).html(html);
                        $(div).find('.icon').find('a').attr('class', 'bitTableFolderIcon');

                        if (obj.Name == '...') {
                            $(div).find('.bitConfigButton').hide();
                            $(div).find('.bitDeleteButton').hide();
                            $(div).find('.bitCopyButton').hide();
                        }

                        var text = $(div).find('.nameColumn').html();
                        var htmlLink = "<a href='javascript:BITDATACOLLECTIONDATA.loadData(\"" + obj.ID + "\", undefined, undefined, \"" + obj.Name + "\");'>" + obj.Name + "</a>";
                        $(div).find('.nameColumn').html(htmlLink);


                        html = $(div).html();
                    }
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
            $('#tableView tbody').sortable();

            //}

            //$("#tablePages tbody").selectable();
        });
    },

    loadDataTree_old: function (selectedId) {
        var timestamp = new Date().getTime();
        $('#treePanel').load('bitTrees/DataCollectionDataTree.aspx?id=' + BITDATACOLLECTIONDATA.dataCollection.ID + '&' + timestamp, function () {
            $('#treePanel').treeview({
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
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
        var parametersObject = { datacollectionid: BITDATACOLLECTIONDATA.datacollectionId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetGroups", jsonstring, function (data) {
            BITDATACOLLECTIONDATA.groups = data.d;
            $("#selectGroups").fillDropDown(data.d, { value: "", text: "" });
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
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
        var groupid = null;
        if (BITDATACOLLECTIONDATA.currentGroup) groupid = BITDATACOLLECTIONDATA.currentGroup.ID;
        var parametersObject = { id: id, groupid: groupid, datacollectionid: BITDATACOLLECTIONDATA.datacollectionId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetItem", jsonstring, function (data) {
            BITDATACOLLECTIONDATA.loadedDomainObject = data.d;
            BITDATACOLLECTIONDATA.type = "Item";
            $('#bitDataDetailsDialog').dataBind(data.d);
            $('#tableExtraImages').dataBindList(data.d.ExtraImages);
            if (BITDATACOLLECTIONDATA.loadedDomainObject.IsNew) {
                $('#bitDataDetailsDialog').dialog({ title: "Nieuw item" });
            }
            else {
                $('#bitDataDetailsDialog').dialog({ title: "Item: " + BITDATACOLLECTIONDATA.loadedDomainObject.Name });
            }
            //bij items show tabpages
            for (var i in BITDATACOLLECTIONDATA.showTabs) {
                var tabName = BITDATACOLLECTIONDATA.showTabs[i];
                $("#" + tabName).show();
            }
            $('#bitDataDetailsDialog').dialog("open");
        });
    },

    openGroupPopup: function (id) {
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
        var groupid = null;
        if (BITDATACOLLECTIONDATA.currentGroup) groupid = BITDATACOLLECTIONDATA.currentGroup.ID;
        var parametersObject = { id: id, parentgroupid: groupid, datacollectionid: BITDATACOLLECTIONDATA.datacollectionId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetGroup", jsonstring, function (data) {
            BITDATACOLLECTIONDATA.loadedDomainObject = data.d;
            BITDATACOLLECTIONDATA.currentGroup = data.d;
            BITDATACOLLECTIONDATA.type = "Group";
            $('#bitDataDetailsDialog').dataBind(data.d);
            $('#tableExtraImages').dataBindList(data.d.ExtraImages);

            if (BITDATACOLLECTIONDATA.loadedDomainObject.IsNew) {
                $('#bitDataDetailsDialog').dialog({ title: "Nieuwe groep" });
            }
            else {
                $('#bitDataDetailsDialog').dialog({ title: "Group: " + BITDATACOLLECTIONDATA.loadedDomainObject.Name });
            }
            //bij groepen hide tabpages
            $("#tabPage5, #tabPage6, #tabPage7, #tabPage8").hide();

            $('#bitDataDetailsDialog').dialog("open");
        });
    },

    save: function () {

        var validation = $('#bitDataDetailsDialog').validate();
        if (validation) {
            //get data from panel
            BITDATACOLLECTIONDATA.loadedDomainObject = $('#bitDataDetailsDialog').collectData(BITDATACOLLECTIONDATA.loadedDomainObject);
            jsonstring = convertToJsonString(BITDATACOLLECTIONDATA.loadedDomainObject);
            BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
            if (BITDATACOLLECTIONDATA.type == "Item") {
                BITAJAX.callWebService("SaveItem", jsonstring, function (data) {
                    //reload tree
                    //BITDATACOLLECTIONDATA.loadDataTree(BITDATACOLLECTIONDATA.loadedDomainObject.ID);
                    //reload grid
                    BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId);
                });
            }
            else {
                BITAJAX.callWebService("SaveGroup", jsonstring, function (data) {
                    //reload tree
                    //BITDATACOLLECTIONDATA.loadDataTree(BITDATACOLLECTIONDATA.loadedDomainObject.ID);
                    //relaod valt onder select box
                    BITDATACOLLECTIONDATA.fillDropDownGroups();
                    //reload grid
                    BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId);
                });
            }
            $('#bitDataDetailsDialog').dialog("close");
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
            if (BITDATACOLLECTIONDATA.type == "Item") {
                BITAJAX.callWebService("DeleteItem", jsonstring, function (data) {

                    //reload tree
                    //BITDATACOLLECTIONDATA.loadDataTree();
                    //reload grid
                    BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId);
                });
            }
            else if (BITDATACOLLECTIONDATA.type == "Group") {
                BITAJAX.callWebService("DeleteGroup", jsonstring, function (data) {

                    //reload tree
                    //BITDATACOLLECTIONDATA.loadDataTree();
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


    openImagePopup: function (target) {
        BITIMAGESPOPUP.target = target;
        $('#bitImagesDialog').dialog("open");

    },

    setImageUrl: function (src) {
        var target = BITIMAGESPOPUP.target;
        if (target == "Thumbnail") {
            $("#imgThumbnail").attr("src", src);
            $("#imgThumbnail").show();
        }
        else if (target == "Image") {
            $("#imgImage").attr("src", src);
            $("#imgImage").show();
        }
        else if (target == "ExtraImage") {
            var extraImage = new Object();
            extraImage.Url = src;
            extraImage.Title = src;
            extraImage.ID = "00000000-0000-0000-0000-000000000000";
            extraImage.Type = "ExtraImage";
            if (BITDATACOLLECTIONDATA.loadedDomainObject.ExtraImages == null) {
                BITDATACOLLECTIONDATA.loadedDomainObject.ExtraImages = [];
            }
            BITDATACOLLECTIONDATA.loadedDomainObject.ExtraImages.push(extraImage);
            $('#tableExtraImages').dataBindList(BITDATACOLLECTIONDATA.loadedDomainObject.ExtraImages);
        }
    },

    clearImage: function (target) {
        if (target == "Thumbnail") {
            $("#imgThumbnail").attr("src", "");
            $("#imgThumbnail").hide();
        }
        else if (target == "Image") {
            $("#imgImage").attr("src", "");
            $("#imgImage").hide();
        }
    },

    removeExtraFile: function (id, index) {
        if (id == "00000000-0000-0000-0000-000000000000") {
            //nieuwe files, alleen uit array halen
            BITDATACOLLECTIONDATA.loadedDomainObject.ExtraImages.splice(index, 1);
            $('#tableExtraImages').dataBindList(BITDATACOLLECTIONDATA.loadedDomainObject.ExtraImages);
        }
        else {
            //meteen op server
            BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";

            var parametersObject = { fileId: id, itemId: BITDATACOLLECTIONDATA.loadedDomainObject.ID };
            var jsonstring = JSON.stringify(parametersObject);

            BITAJAX.callWebService("RemoveExtraImage", jsonstring, function (data) {
                BITDATACOLLECTIONDATA.loadedDomainObject = data.d;
                $('#tableExtraImages').dataBindList(data.d.ExtraImages);
            });
        }

    },

    download: function () {
        document.getElementById("iframeTransferFiles").src = "bitAjaxServices/DataCollectionService.aspx?download=" + BITDATACOLLECTIONDATA.dataCollection.ID;
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
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";

        var parametersObject = { datacollectionId: BITDATACOLLECTIONDATA.dataCollection.ID };
        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.callWebService("ImportData", jsonstring, function (data) {
            //reload tree
            BITBACKEND.loadDataCollectionData(BITDATACOLLECTIONDATA.dataCollection.ID);
            $('#iframeTransferFiles').html("");
        });

        //hide div
        $('#divUploadFiles').hide(700);
    },
    cancelSend: function () {
        //hide div
        $('#divUploadFiles').hide(700);
    },

    copy: function (id, type) {
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
        if (type == "Item") {
            var parentGroupId = BITDATACOLLECTIONDATA.currentGroupId;

            var parametersObject = { datacollectionId: BITDATACOLLECTIONDATA.datacollectionId, parentGroupId: parentGroupId, itemId: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("CopyItem", jsonstring, function (data) {
                //reload grid
                BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId);
            });
        }
        else if (type == "Group") {
            var parentGroupId = BITDATACOLLECTIONDATA.currentGroupId;
            var parametersObject = { datacollectionId: BITDATACOLLECTIONDATA.datacollectionId, parentGroupId: parentGroupId, groupId: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("CopyGroup", jsonstring, function (data) {
                //reload grid
                BITDATACOLLECTIONDATA.loadData(BITDATACOLLECTIONDATA.currentGroupId);
            });
        }
    },

    loadTree: function () {
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
        var parametersObject = {datacollectionID: BITDATACOLLECTIONDATA.datacollection.ID, searchString: '' };
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
        var html = "<a href='javascript:BITDATACOLLECTIONDATA.hideTree();' class='bitConfigButton'></a><div>Verberg tree</div>";
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
        var html = "<a href='javascript:BITDATACOLLECTIONDATA.showTree();' class='bitConfigButton'></a><div>Toon tree</div>";
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
        console.log(data.rslt.r.context);

        /* var sourceFolders = [];
        $(data.rslt.o).each(function (i) {
            var sourceFolder = $(data.rslt.o[i]).find('a').attr('data-path');
            sourceFolders[sourceFolders.length] = sourceFolder;
            var destination = targetFolder + sourceFolder.substr(sourceFolder.lastIndexOf('\\'));
            $(data.rslt.o[i]).find('a').attr('data-path', destination);
        }); */
    }
};