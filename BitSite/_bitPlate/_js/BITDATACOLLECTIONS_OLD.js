/// <reference path="BITBACKEND.js" />

var BITDATACOLLECTIONS = {
    loadedDomainObject: null,
    type: "DataCollection",
    dataCollection: null,
    //dataCollectionId: null,
    currentGroup: null,
    imagesPopup: null,

    loadCollectionTree: function (selectedId) {
        var timestamp = new Date().getTime();
        $('#treePanel').load('bitTrees/DataCollectionsTree.aspx?' + timestamp, function () {
            $('#treePanel').treeview({
                selectedIndexChanged: function (id, type) {
                    BITDATACOLLECTIONS.loadCollectionDetails(id);
                },
                setSelected: selectedId
            });
        });
    },

    loadDataTree: function (selectedId) {
        var timestamp = new Date().getTime();
        $('#treePanel').load('bitTrees/DataCollectionDataTree.aspx?id=' + BITDATACOLLECTIONS.dataCollection.ID + '&' + timestamp, function () {
            $('#treePanel').treeview({
                sortable: true,
                sortstop: function (data) {
                    var jsonstring = JSON.stringify(data);
                    BITAJAX.callWebService("SaveOrderingNummerItem", jsonstring, null);
                    //als zelfde item is geselecteerd dan veld volgnummer updaten
                    if (BITDATACOLLECTIONS.loadedDomainObject && BITDATACOLLECTIONS.loadedDomainObject.ID == data.id) {
                        $("input[data-field=OrderingNumber]").val(data.orderingnumber);
                    }
                },
                selectedIndexChanged: function (id, type) {
                    if (id == 'treeRoot' || id == '') return;
                    if (type == "item") {
                        BITDATACOLLECTIONS.loadItem(id);
                    }
                    else if (type == "group") {
                        BITDATACOLLECTIONS.loadGroup(id);
                    }
                    else if (type == "root") {
                        BITDATACOLLECTIONS.loadCollectionDetails(id);
                    }
                },
                setSelected: selectedId
            });
            $('#bitButtonSearch').click(function () {
                //BITPAGES.searchPage();
            });
        });
    },

    fillCheckboxListFields: function () {
        var fields = ["Name", "Title", "Introduction", "Content", "Image", "Thumbnail", "ExtraImages", "Date", "Address", "PostalCode", "City", "Country", "Category", "ProductNumber", "ProductSize", "ProductContent", "enz"];
        $("#checkboxListFields").fillCheckboxList(fields);

        $(".checkFieldsAll").click(function () {
            var checked = $(this).attr("checked");
            $(this).parent().find("input[type=checkbox]").attr("checked", checked);
            $("#divCheckboxes1").find("input[value=Name]").attr("checked", "checked");
            $("#divCheckboxes1").find("input[value=Title]").attr("checked", "checked");
        });
    },

    fillDropDownGroups: function () {
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
        var parametersObject = { datacollectionid: BITDATACOLLECTIONS.dataCollection.ID };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetGroups", jsonstring, function (data) {
            BITPAGES.folders = data.d;
            $("#selectGroups").fillDropDown(data.d, { value: "", text: "" });
        });

    },

    showFields: function () {
        $(".field").hide();
        $(".dockPanelTitle").hide();
        $(".dockPanelContent").hide();
        var fields = BITDATACOLLECTIONS.dataCollection.Fields;
        for (var i in fields) {
            var name = fields[i];
            $(".field[title=" + name + "]").show();
            $(".dockPanelTitle[title=" + name + "]").show();
            if (name == "Image" || name == "Thumbnail" || name == "ExtraImages") {
                $(".dockPanelTitle[title=Images]").show();
            }
        }
        if (BITDATACOLLECTIONS.type == "Group") {
            $('.dockPanelTitle[title=Extra]').hide();
        }
        else if (BITDATACOLLECTIONS.type == "DataCollection") {
            $(".dockPanelTitle[title=Fields").show();
            $(".dockPanelTitle[title=ExtraFields").show();
        }
    },

    changeDataCollectionType: function () {
        var type = $("#selectType option:selected").val();
        var fields = [];
        if (type == "default") {
            var fields = ["Name", "Title", "Introduction", "Content", "Image", "Thumbnail", "ExtraImages"];
        }
        else if (type == "news") {
            var fields = ["Name", "Title", "Introduction", "Content", "Image", "Thumbnail", "ExtraImages", "Date"];
        }
        else if (type == "maps") {
            var fields = ["Address", "PostalCode", "City", "Country"];
        }
        $("#divCheckboxes1, #divCheckboxes2, #divCheckboxes3").find("input[type=checkbox]").attr("checked", "");
        for (var i in fields) {
            var name = fields[i];
            $("#divCheckboxes1").find("input[value=" + name + "]").attr("checked", "checked");
            $("#divCheckboxes2").find("input[value=" + name + "]").attr("checked", "checked");
            $("#divCheckboxes3").find("input[value=" + name + "]").attr("checked", "checked");
        }
        $("#divCheckboxes1").find("input[value=Name]").attr("checked", "checked");
        $("#divCheckboxes1").find("input[value=Title]").attr("checked", "checked");
    },

    newDataCollection: function () {
        this.loadCollectionDetails(null);
    },

    loadCollectionDetails: function (id) {
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
        var timestamp = new Date().getTime();
        $('#collectionPanel').load('bitDetails/DataCollectionDetails.aspx?' + timestamp, function () {
            $('#detailsPanel').helpable();
            $('#detailsPanel').dockable();

            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("GetDataCollection", jsonstring, function (data) {
                BITDATACOLLECTIONS.dataCollection = data.d;
                BITDATACOLLECTIONS.loadedDomainObject = data.d;
                BITDATACOLLECTIONS.type = "DataCollection";
                $('#collectionPanel').dataBind(data.d);
                $('#collectionPanel').show();
                $('#itemPanel').hide();
                //$('#groupPanel').hide();
                BITDATACOLLECTIONS.currentGroup = null;
                //bind checkboxListFields
                //deze halen we van een verborgen checklist met engelse veldnamen
                //in code hieronder worden ze naar nederlandse lijsten geplaatst
                $("#checkboxListFields").find("input[type=checkbox]").each(function (i) {
                    var checked = $(this).attr("checked");
                    var value = $(this).attr("value");
                    $("#divCheckboxes1").find("input[value=" + value + "]").attr("checked", checked);
                    $("#divCheckboxes2").find("input[value=" + value + "]").attr("checked", checked);
                    $("#divCheckboxes3").find("input[value=" + value + "]").attr("checked", checked);
                });
            });
            if (id == null) {
                $('#collectionPanel').find('h1').html("Nieuwe gegevensverzameling");
                $('#treePanel').treeview({ setSelected: "" });
            }
            else {
                $('#collectionPanel').find('h1').html("Dataverzameling eigenschappen");
            }
            BITDATACOLLECTIONS.fillCheckboxListFields();
        });
    },

    newItem: function () {
        this.loadItem(null);
    },

    newGroup: function () {
        this.loadGroup(null);
    },

    loadItem: function (id) {
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
        var groupid = null;
        if (BITDATACOLLECTIONS.currentGroup) groupid = BITDATACOLLECTIONS.currentGroup.ID;
        var parametersObject = { id: id, groupid: groupid, datacollectionid: BITDATACOLLECTIONS.dataCollection.ID };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetItem", jsonstring, function (data) {
            BITDATACOLLECTIONS.loadedDomainObject = data.d;
            BITDATACOLLECTIONS.currentGroup = data.d.ParentGroup;
            BITDATACOLLECTIONS.type = "Item";
            $('#itemPanel').dataBind(data.d);
            if (data.d.IsActive) {
                $('#buttonItemActivate').html("deactiveer");
            }
            else {
                $('#buttonItemActivate').html("activeer");
            }
            $('#tableExtraImages').dataBindList(data.d.ExtraImages);
            $('#itemPanel').show();
            $('#collectionPanel').hide();
        });

        if (id == null) {
            $('#itemPanel').find('h1').html("Nieuw item");
            $('#treePanel').treeview({ setSelected: "" });
        }
        else {
            $('#itemPanel').find('h1').html("Item");
        }
    },

    loadGroup: function (id) {
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
        var groupid = null;
        if (BITDATACOLLECTIONS.currentGroup) groupid = BITDATACOLLECTIONS.currentGroup.ID;
        var parametersObject = { id: id, parentgroupid: groupid, datacollectionid: BITDATACOLLECTIONS.dataCollection.ID };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetGroup", jsonstring, function (data) {
            BITDATACOLLECTIONS.loadedDomainObject = data.d;
            BITDATACOLLECTIONS.currentGroup = data.d;
            BITDATACOLLECTIONS.type = "Group";
            $('#itemPanel').dataBind(data.d);
            if (data.d.IsActive) {
                $('#buttonItemActivate').html("deactiveer");
            }
            else {
                $('#buttonItemActivate').html("activeer");
            }
            $('#tableExtraImages').dataBindList(data.d.ExtraImages);
            $('#itemPanel').show();
            $('#collectionPanel').hide();

        });
        if (id == null) {
            $('#itemPanel').find('h1').html("Nieuwe groep");
            $('#treePanel').treeview({ setSelected: "" });
        }
        else {
            $('#itemPanel').find('h1').html("Groep");
        }
    },

    save: function () {
        var divId = "#itemPanel";
        //if (BITDATACOLLECTIONS.type == "Group") {
        //    divId = "#groupPanel";
        //}
        //else
        if (BITDATACOLLECTIONS.type == "DataCollection") {
            divId = "#collectionPanel";
            //waardes uit checkboxlijsten weer terug zetten in verborgen checklist die is verbonden met de data
            $("#divCheckboxes1, #divCheckboxes2, #divCheckboxes3").find("input[type=checkbox]").each(function (i) {
                var checked = $(this).attr("checked");
                var value = $(this).attr("value");
                $("#checkboxListFields").find("input[value=" + value + "]").attr("checked", checked);
            });
        }
        var validation = $(divId).validate();
        if (validation) {
            //get data from panel
            BITDATACOLLECTIONS.loadedDomainObject = $(divId).collectData(BITDATACOLLECTIONS.loadedDomainObject);
            jsonstring = convertToJsonString(BITDATACOLLECTIONS.loadedDomainObject);
            BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
            if (BITDATACOLLECTIONS.type == "DataCollection") {
                BITAJAX.callWebService("SaveDataCollection", jsonstring, function (data) {
                    BITDATACOLLECTIONS.loadedDomainObject = data.d;
                    BITDATACOLLECTIONS.currentGroup = null;
                    BITDATACOLLECTIONS.type = "DataCollection";
                    //reload tree
                    BITDATACOLLECTIONS.loadCollectionTree(BITDATACOLLECTIONS.loadedDomainObject.ID);
                });
            }
            else if (BITDATACOLLECTIONS.type == "Item") {
                BITAJAX.callWebService("SaveItem", jsonstring, function (data) {
                    BITDATACOLLECTIONS.loadedDomainObject = data.d;
                    BITDATACOLLECTIONS.currentGroup = data.d.ParentGroup;
                    BITDATACOLLECTIONS.type = "Item";
                    //reload tree
                    BITDATACOLLECTIONS.loadDataTree(BITDATACOLLECTIONS.loadedDomainObject.ID);
                });
            }
            else {
                BITAJAX.callWebService("SaveGroup", jsonstring, function (data) {
                    BITDATACOLLECTIONS.loadedDomainObject = data.d;
                    BITDATACOLLECTIONS.currentGroup = data.d;
                    BITDATACOLLECTIONS.type = "Group";
                    //reload tree
                    BITDATACOLLECTIONS.loadDataTree(BITDATACOLLECTIONS.loadedDomainObject.ID);
                    //relaod valt onder select box
                    BITDATACOLLECTIONS.fillDropDownGroups();
                });
            }
        }
    },

    remove: function () {
        var msg = "Wilt u dit item verwijderen?";
        if (BITDATACOLLECTIONS.type == "Group") {
            msg = "Wilt u deze groep (met alle subgroepen en items) verwijderen?";
        }
        else if (BITDATACOLLECTIONS.type == "DataCollection") {
            msg = "Wilt u deze dataverzameling met alle groepen en items helemaal verwijderen?";
        }
        if (confirm(msg)) {
            var parametersObject = { id: BITDATACOLLECTIONS.loadedDomainObject.ID };
            var jsonstring = JSON.stringify(parametersObject);
            if (BITDATACOLLECTIONS.type == "Item") {
                BITAJAX.callWebService("DeleteItem", jsonstring, function (data) {
                    BITDATACOLLECTIONS.loadedDomainObject = null;
                    BITDATACOLLECTIONS.currentGroup = null;
                    //reload tree
                    BITDATACOLLECTIONS.loadDataTree();
                });
            }
            else if (BITDATACOLLECTIONS.type == "Group") {
                BITAJAX.callWebService("DeleteGroup", jsonstring, function (data) {
                    BITDATACOLLECTIONS.loadedDomainObject = null;
                    BITDATACOLLECTIONS.currentGroup = null;
                    //reload tree
                    BITDATACOLLECTIONS.loadDataTree();
                });
            }
            else if (BITDATACOLLECTIONS.type == "DataCollection") {
                BITAJAX.callWebService("DeleteDataCollection", jsonstring, function (data) {
                    BITDATACOLLECTIONS.loadedDomainObject = null;
                    BITDATACOLLECTIONS.currentGroup = null;
                    //reload tree
                    BITDATACOLLECTIONS.loadCollectionTree();
                });
            }
        }
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
        var popup = BITEDITOR.imagesPopup;

        if (!popup) {
            popup = new Popup("Afbeeldingen", "", { top: 100, left: 800 }, { width: 300, height: 600 });

            popup.load('bitDetails/Images.aspx');

            BITEDITOR.imagesPopup = popup;
        }
        popup.target = target;
        popup.show();

        //click event van img wordt in commandmanager afgehandeld
    },

    setImageUrl: function (target, src) {
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
            if (BITDATACOLLECTIONS.loadedDomainObject.ExtraImages == null) {
                BITDATACOLLECTIONS.loadedDomainObject.ExtraImages = [];
            }
            BITDATACOLLECTIONS.loadedDomainObject.ExtraImages.push(extraImage);
            $('#tableExtraImages').dataBindList(BITDATACOLLECTIONS.loadedDomainObject.ExtraImages);
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
            BITDATACOLLECTIONS.loadedDomainObject.ExtraImages.splice(index, 1);
            $('#tableExtraImages').dataBindList(BITDATACOLLECTIONS.loadedDomainObject.ExtraImages);
        }
        else {
            //meteen op server
            BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";

            var parametersObject = { fileId: id, itemId: BITDATACOLLECTIONS.loadedDomainObject.ID };
            var jsonstring = JSON.stringify(parametersObject);

            BITAJAX.callWebService("RemoveExtraImage", jsonstring, function (data) {
                BITDATACOLLECTIONS.loadedDomainObject = data.d;
                $('#tableExtraImages').dataBindList(data.d.ExtraImages);
            });
        }

    },

    download: function () {
        document.getElementById("iframeTransferFiles").src = "bitAjaxServices/DataCollectionService.aspx?download=" + BITDATACOLLECTIONS.dataCollection.ID;
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

        var parametersObject = { datacollectionId: BITDATACOLLECTIONS.dataCollection.ID};
        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.callWebService("ImportData", jsonstring, function (data) {
            //reload tree
            BITBACKEND.loadDataCollectionData(BITDATACOLLECTIONS.dataCollection.ID);
            $('#iframeTransferFiles').html("");
        });

        //hide div
        $('#divUploadFiles').hide(700);
    },
    cancelSend: function () {
        //hide div
        $('#divUploadFiles').hide(700);
    },

    copy: function () {
        BITAJAX.dataServiceUrl = "bitAjaxServices/DataCollectionService.aspx";
        if (BITDATACOLLECTIONS.type == "Item") {
            var parentGroupId = null;
            if (BITDATACOLLECTIONS.currentGroup) {
                parentGroupId = BITDATACOLLECTIONS.currentGroup.ID;
            }
            var parametersObject = { datacollectionId: BITDATACOLLECTIONS.dataCollection.ID, parentGroupId: parentGroupId, itemId: BITDATACOLLECTIONS.loadedDomainObject.ID };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("CopyItem", jsonstring, function (data) {
                //reload tree
                BITDATACOLLECTIONS.loadDataTree();
            });
        }
        else if (BITDATACOLLECTIONS.type == "Group") {
            var parentGroupId = null;
            if (BITDATACOLLECTIONS.currentGroup.ParentGroup) {
                parentGroupId = BITDATACOLLECTIONS.currentGroup.ParentGroup.ID;
            }
            var parametersObject = { datacollectionId: BITDATACOLLECTIONS.dataCollection.ID, parentGroupId: parentGroupId, groupId: BITDATACOLLECTIONS.loadedDomainObject.ID };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("CopyGroup", jsonstring, function (data) {
                //reload tree
                BITDATACOLLECTIONS.loadDataTree();
            });
        }
        else if (BITDATACOLLECTIONS.type == "DataCollection") {
            var parametersObject = { datacollectionId: BITDATACOLLECTIONS.dataCollection.ID };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("CopyDataCollection", jsonstring, function (data) {
                //reload tree
                BITDATACOLLECTIONS.loadCollectionTree();
            });
        }
    }

};