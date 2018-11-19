var SHOWDATATYPES = { USERSELECT: 0, MAINGROUPS: 1, SUBGROUPS: 2, ALLGROUPS: 3, ALLITEMS: 4, USERSELECTNODEFAULTDATA: 5 };

var BITDATACONFIGTAB = {
    dataBind: function () {
        $('.button').button();
        if ($("#selectShowData").val() == SHOWDATATYPES.USERSELECT) {
            $('#dataCollectionPathBrowse').show();
            $('#dataCollectionPathName').show();
        }
        else {
            $('#dataCollectionPathBrowse').hide();
            $('#dataCollectionPathName').hide();
        }
        if (BITEDITPAGE.currentModule.Type == "ItemDetailsModule" ||
            BITEDITPAGE.currentModule.Type == "GroupDetailsModule") {
            $('#divModuleListSettings').hide();
            $('#dataCollectionPathBrowse').hide();
            $('#divDetailsModuleSettings').show();
            $('#divModuleInputSettings').hide();
        }
        else if (BITEDITPAGE.currentModule.Type == "ItemInputModule") {
            $('#dataCollectionPathBrowse').show();
            $('#divModuleInputSettings').show();
            $('#divDetailsModuleSettings').hide();
            $('#divModuleListSettings').hide();
        }
        else {
            $('#divModuleListSettings').show();
            $('#dataCollectionPathBrowse').show();
            $('#divDetailsModuleSettings').hide();
            $('#divModuleInputSettings').hide();
            
        }
        
        if (BITEDITPAGE.currentModule.Type == "ItemListModule") {
            $("#selectShowData").empty();
            $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.USERSELECT).html("Gebruikersselectie afhankelijk"));
            $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.ALLITEMS).html("Alle items"));
        }
        else if (BITEDITPAGE.currentModule.Type == "GroupListModule") {
            $("#selectShowData").empty();
            $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.USERSELECT).html("Gebruikersselectie afhankelijk"));
            $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.USERSELECTNODEFAULTDATA).html("Gebruikersselectie afhankelijk (geen standaard data)"));
            $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.MAINGROUPS).html("Alleen hoofdgroepen"));
            $("#selectShowData").append($('<option></option>').val(SHOWDATATYPES.ALLGROUPS).html("Alle groepen"));
        }

        if (BITEDITPAGE.currentModule.DataCollection) {
            datacollectionid = BITEDITPAGE.currentModule.DataCollection.ID;
            
            BITDATACONFIGTAB.bindDefaultDataElements(datacollectionid, BITEDITPAGE.currentModule);
            //BITEDITPAGE.selectDataCollection(datacollectionid);
            if (BITEDITPAGE.currentModule.Type.indexOf("List") > 0) {
                BITDATACONFIGTAB.bindSortAndFilterFields(datacollectionid, BITEDITPAGE.currentModule);
            }
        }
        else {
            $("#selectDefaultDataElement").empty();
        }
    },

    changeDataCollection: function (select) {
        var datacollectionid = $(select).val();

        BITDATACONFIGTAB.bindSortAndFilterFields(datacollectionid, BITEDITPAGE.currentModule);
        BITDATACONFIGTAB.bindDefaultDataElements(datacollectionid, BITEDITPAGE.currentModule);
        if (!BITEDITPAGE.currentModule.DataCollection) { BITEDITPAGE.currentModule.DataCollection = new Object(); }
        BITEDITPAGE.currentModule.DataCollection.ID = datacollectionid;
    },

    changeShowDataType: function (select) {
        if ($(select).val() == SHOWDATATYPES.USERSELECT) {
            $('#dataCollectionPathBrowse').show();
            $('#dataCollectionPathName').show();
        }
        else {
            $('#dataCollectionPathBrowse').hide();
            $('#dataCollectionPathName').hide();
            $('#dataCollectionPathName').val('');
            BITEDITPAGE.currentModule.Settings.SelectGroupID = '';
        }
    },

    bindSortAndFilterFields: function (datacollectionid, module) {
        //eerst leegmaken
        $("#bitSelectFilterFields").empty();
        $("#bitSelectSortFields").empty();
        var parametersObject = { datacollectionid: datacollectionid, moduleType: module.Type };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.asmx";
        BITAJAX.callWebService("GetSelectAndSortFieldsByDataCollectionID", jsonstring, function (data) {
            //BITEDITPAGE.currentModule = data.d;
            //koppel velden aan dropdowns
            $("#bitSelectFilterFields").fillDropDown(data.d, { value: "null", text: "" }, "Value");
            $("#bitSelectSortFields").fillDropDown(data.d, { value: "null", text: "" }, "Value");
            $("#standardTabPageData_ListSettings").dataBind(module);
        });
    },

    bindDefaultDataElements: function (datacollectionid, module) {
        $("#selectDefaultDataElement").empty();
        var parametersObject = { datacollectionid: datacollectionid, moduleType: module.Type };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/DataCollections/DataCollectionService.asmx";
        var ajaxCall = "GetAllGroupsLite";
        if (module.Type == "ItemDetailsModule" || module.Type == 'ItemInputModule') {
            ajaxCall = "GetAllItemsLite";
        }
        BITAJAX.callWebService(ajaxCall, jsonstring, function (data) {
            //BITEDITPAGE.currentModule = data.d;
            //koppel velden aan dropdowns
            $("#selectFixedDataElement").fillDropDown(data.d, { value: "00000000-0000-0000-0000-000000000000", text: "Geen" }, "ID");
            $("#selectDefaultDataElement").fillDropDown(data.d, { value: "00000000-0000-0000-0000-000000000000", text: "Leeg" }, "ID");
            $("#selectDefaultDataElement option:first").after("<option value='11111111-1111-1111-1111-111111111111'>Eerste element uit verzameling</option>");
            $("#standardTabPageData_DetailsSettings").dataBind(module);
        });
    },

    showSelectDataGroupDialog: function () {
        var dataCollectionSelected = true;
        if (!BITEDITPAGE.currentModule.DataCollection) dataCollectionSelected = false;
        if (dataCollectionSelected && (!BITEDITPAGE.currentModule.DataCollection.ID || BITEDITPAGE.currentModule.DataCollection.ID == '00000000-0000-0000-0000-000000000000')) dataCollectionSelected = false;

        if (!dataCollectionSelected) {
            $.messageBox(MSGBOXTYPES.ERROR, 'Selecteer een datacollectie.', 'Selecteer een datacollectie.');
        }
        else {
            //console.log(BITEDITPAGE.currentModule.DataCollection.ID);
            var dataCollectionGroup = new Object()
            $('#selectDataGroupDialog').initDialog(null, { width: 400, height: 600 }, true, {
                'Selecteer': function () {
                    if (BITEDITPAGE.currentModule.DataCollection.ID != dataCollectionGroup.Id) { //if selectedId == Datacollection id)
                        BITEDITPAGE.currentModule.Settings = (!BITEDITPAGE.currentModule.Settings) ? new Object() : BITEDITPAGE.currentModule.Settings;
                        BITEDITPAGE.currentModule.Settings.SelectGroupID = dataCollectionGroup.Id;
                        //$('#dataCollectionPathName').html(dataCollectionGroup.Name);
                        $('#dataCollectionPathName').val(dataCollectionGroup.Name);
                    }
                    else {
                        BITEDITPAGE.currentModule.Settings.SelectGroupID = '';


                    }
                    $('#selectDataGroupDialog').dialog('close');
                },
                "Annuleer": function () {
                    $('#selectDataGroupDialog').dialog('close');
                }
            });
            var parametersObject = { datacollectionID: BITEDITPAGE.currentModule.DataCollection.ID, searchString: '' };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/DataCollections/DataCollectionService.asmx";
            BITAJAX.callWebService("GetTreeHtml", jsonstring, function (data) {
                $('#dataCollectionGroupTree').html(data.d);
                //console.log(data.d);
                $('#dataCollectionGroupTree').jstree({ "plugins": ["themes", "html_data", "ui"] }).bind("select_node.jstree", function (event, data) {
                    //`data.rslt.obj` is the jquery extended node that was clicked          
                    // To get path [ID or Name] from root node to selected node 
                    var ids = data.inst.get_path('#' + data.rslt.obj.attr('id'), true);
                    // Returns IDs from root to selected node
                    var names = data.inst.get_path('#' + data.rslt.obj.attr('id'), false);
                    // Returns Name's from root to selected node 

                    dataCollectionGroup.Name = names.toString().replace(/,/g, '/');
                    dataCollectionGroup.Id = ids[ids.length - 1];
                });
                $('#selectDataGroupDialog').dialog('open');
            });
        }
    },

    clearSelectGroup: function () {
        $('#dataCollectionPathName').val('');
        BITEDITPAGE.currentModule.Settings.SelectGroupID = '';
    },

    validate: function () {
        var result = true;
        if (BITEDITPAGE.currentModule.Type != "ItemDetailsModule" && BITEDITPAGE.currentModule.Type != "GroupDetailsModule") {
            var filterField = $('#bitSelectFilterFields').val();
            var filterOperator = $('#bitSelectFilterOperator').val();
            if (filterField && filterField.trim() != 'null' && filterOperator == 'null') {
                result = false;
                $('#bitSelectFilterOperator').attr('style', 'border: 1px red solid');
            }
            else {
                $('#bitSelectFilterOperator').attr('style', 'border: none');
            }
        }
        else {
            //Doe iets
        }
        return result;
    },

    changeFixedData: function () {
        var fixedData = $('#checkFixedData').is(':checked');
        if (fixedData) {
            $("#divInitialDataElement").hide();
            $("#selectDefaultDataElement").attr("disabled", "disabled");
            //$("#selectDefaultDataElement")[0].selectedIndex = 0;
            $("#selectFixedDataElement").removeAttr("disabled");
            $("#selectFixedDataElement").show();
        }
        else {
            $("#divInitialDataElement").show();
            $("#selectFixedDataElement").hide();
            $("#selectDefaultDataElement").removeAttr("disabled");
            $("#selectFixedDataElement").attr("disabled", "disabled");
            //$("#selectFixedDataElement")[0].selectedIndex = 0;
        }
    },

    changeDefaultDataElement: function () {
        var defaultData = $("#selectDefaultDataElement").val();
        if (defaultData == '11111111-1111-1111-1111-111111111111') {
            $('#defaultDataElementSettings').show();
        }
        else {
            $('#defaultDataElementSettings').hide();
        }
    }
};
//registeer dit object bij BITEDITPAGE, zodat die de init functie kan aanroepen
//BITEDITPAGE.registerModuleConfigJsClass(BITDATACONFIGTAB, "BITDATACONFIGTAB");