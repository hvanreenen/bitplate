/// <reference path="../prototypes/initDialog.js" />
var FIELD_TYPES = {
    TEXT: 1,
    LONGTEXT: 2,
    HTML: 3,
    NUMERIC: 4,
    CURRENCY: 5,
    YESNO: 6,
    DROPDOWN: 7,
    IMAGE: 8,
    FILE: 9,
    IMAGELIST: 10,
    FILELIST: 11,
    DATETIME: 12,
    READONLY: 13,
    CHECKBOXLIST: 14
};

var DATACOLLECTION_TYPES = {
    SIMPLE: 0,
    DEFAULT: 1,
    NEWS : 2,
    PRODUCTS : 3,
    FAQ : 4,
    GOOGLEMAPS : 5
};

var BITDATACOLLECTIONS = {
    dataCollection: null,
    dataField: null,
    currentSort: "Name ASC",
    searchString: "",
    authTabIsLoaded: false,
    defaultFieldsPerType: [],
    initialize: function () {
        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";

        $('#bitDataCollectionConfigDialog').initDialog(BITDATACOLLECTIONS.saveDataCollectionConfig, { width: '90%', height: window.innerHeight });
        $('#bitDataFieldConfigDialog').initDialog(BITDATACOLLECTIONS.saveDataField);
        $('#bitDataLookupValueDialog').initDialog(BITDATACOLLECTIONS.setDataLookupValue, { width: 600, height: 350 });

        $('#bitPopups').formEnrich();

        $(".bitTabs").tabs({ "cache": true });
        $(".bitTabs").tabs("add", "/_bitplate/Dialogs/AutorisationTab.aspx", "Autorisatie");
        $(".bitTabs").bind("tabsload", function (event, ui) {
            BITAUTORISATIONTAB.hideSiteUserGroupsAndUsers = true;
            BITAUTORISATIONTAB.currentObject = BITDATACOLLECTIONS.dataCollection;
            BITAUTORISATIONTAB.initialize();
            BITAUTORISATIONTAB.dataBind();
            BITDATACOLLECTIONS.authTabIsLoaded = true;
        });

        $('#bitDivSearch').hide();
        BITDATACOLLECTIONS.fillCustomFieldSets();
    },

    loadDataCollections: function (sort) {
        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
        if (sort) BITDATACOLLECTIONS.currentSort = sort;
        var parametersObject = { sort: BITDATACOLLECTIONS.currentSort, searchString: BITDATACOLLECTIONS.searchString };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
        BITAJAX.callWebServiceASync("GetDataCollections", jsonstring, function (data) {
            $("#tableDataCollections").dataBindList(data.d, {
                onSort: function (sort) {
                    BITDATACOLLECTIONS.loadDataCollections(sort);
                },
                onRowBound: function (obj, i, rowHtml) {
                    //if (!obj.IsActive) {
                    //    $(rowHtml).css('color', '#808080');
                    //}
                    var tempWrapperDiv = document.createElement('div');
                    $(tempWrapperDiv).html(rowHtml);
                    if (!obj.IsActive) {
                        $(tempWrapperDiv).find('tr').addClass("inactive");
                        $(tempWrapperDiv).find('tr').css("color", "gray");
                    }
                    if (obj.HasAutorisation) {
                        $(tempWrapperDiv).find('.bitTableAuthIcon').css("display", "inline-block");
                    }
                    html = $(tempWrapperDiv).html();
                    return html;

                }
            });
        });
    },

    newDataCollection: function () {
        this.openDetailsPopup(null);
    },

    openDetailsPopup: function (id) {
        $('#DefaultValueField').hide();
        BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetDataCollection", jsonstring, function (data) {
            BITDATACOLLECTIONS.dataCollection = data.d;

            $('#bitDataCollectionConfigDialog').dataBind(data.d);
            if (BITDATACOLLECTIONS.authTabIsLoaded) {
                BITAUTORISATIONTAB.currentObject = BITDATACOLLECTIONS.dataCollection;
                BITAUTORISATIONTAB.dataBind();
            }
            BITDATACOLLECTIONS.bindDataFields();
            BITDATACOLLECTIONS.changeFixedLanguage();
            if (BITDATACOLLECTIONS.dataCollection.IsNew) {
                $('#bitDataCollectionConfigDialog').dialog({ title: "Nieuwe gegevensverzameling" });
                //zet velden
                BITDATACOLLECTIONS.changeDefaultFields();
            }
            else {
                $('#bitDataCollectionConfigDialog').dialog({ title: "Dataverzameling: " + BITDATACOLLECTIONS.dataCollection.Name });
            }

            $('#bitDataCollectionConfigDialog').dialog('open');

        });
    },



    saveDataCollectionConfig: function (e) {
        var validation = $('#bitDataCollectionConfigDialog').validate();
        if (validation) {
            //get data from panel
            BITDATACOLLECTIONS.dataCollection = $('#bitDataCollectionConfigDialog').collectData(BITDATACOLLECTIONS.dataCollection);

            jsonstring = convertToJsonString(BITDATACOLLECTIONS.dataCollection);
            BITAJAX.dataServiceUrl = "DataCollectionService.asmx";

            BITAJAX.callWebServiceASync("SaveDataCollection", jsonstring, function (data) {
                BITDATACOLLECTIONS.dataCollection = data.d;

                //reload grid
                BITDATACOLLECTIONS.loadDataCollections();
                $('#bitDataCollectionConfigDialog').dialog("close");
            }, null, e.srcElement);

        }
    },

    remove: function (id) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u deze gegevensverzameling verwijderen?", null, function () {

            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);

            BITAJAX.callWebServiceASync("DeleteDataCollection", jsonstring, function (data) {
                BITDATACOLLECTIONS.dataCollection = null;
                BITDATACOLLECTIONS.currentGroup = null;
                //reload grid
                BITDATACOLLECTIONS.loadDataCollections();
            });
        });
    },

    copy: function (id, name) {
        var newName = name + ' (kopie)';
        $.inputBox('Kopie naam:', 'Gegevens kopiëren', newName, function (e, value) {
            var newName = value;
            var parametersObject = { datacollectionId: id, newName: newName };
            BITAJAX.dataServiceUrl = "DataCollectionService.asmx";

            var parametersObject = { datacollectionId: id, newName: newName };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("CopyDataCollection", jsonstring, function (data) {
                //reload grid
                BITDATACOLLECTIONS.loadDataCollections();
            });
        });
    },

    changeDataCollectionType: function () {
        if (BITDATACOLLECTIONS.dataCollection.DataGroupFields.length == 0 && BITDATACOLLECTIONS.dataCollection.DataItemFields.length == 0) {
            //lege zonder te vragen wijzigen
            BITDATACOLLECTIONS.changeDefaultFields();
        }
        else if (BITDATACOLLECTIONS.dataCollection.IsNew) {
            $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.YESNO, "Wilt u bestaande velden overschrijven?", null, function () {
                BITDATACOLLECTIONS.changeDefaultFields();
            });
        }
        else {
            $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "Bij bestaande datacollecties worden velden niet automatisch aangepast na wijzigen van type.");
        }
    },

    changeDefaultFields: function () {
        var type = $("#selectCollectionType").val();
        var defaultFields = BITDATACOLLECTIONS.defaultFieldsPerType[type];
        BITDATACOLLECTIONS.dataCollection.DataGroupFields = defaultFields.groupFields;
        BITDATACOLLECTIONS.dataCollection.DataItemFields = defaultFields.itemFields;
        BITDATACOLLECTIONS.bindDataFields();
    },

    changeFixedLanguage: function () {
        var lang = $("#dropdownLanguages").val();
        if (lang != "" && lang != "none") {
            $("#divMultiLanguage").hide();
            $(".multiLanguage").hide();
        }
        else {
            $("#divMultiLanguage").show();
            $(".multiLanguage").show();
        }
    },

    /////////////////////////////
    // DATAFIELDS
    /////////////////////////////
    bindDataFields: function () {
        //eerst sorteren 
        BITDATACOLLECTIONS.dataCollection.DataGroupFields.sort(BITDATACOLLECTIONS.sortFieldsByTabAndOrderingNumber);
        BITDATACOLLECTIONS.dataCollection.DataItemFields.sort(BITDATACOLLECTIONS.sortFieldsByTabAndOrderingNumber);
        $("#tableGroupFields").dataBindList(BITDATACOLLECTIONS.dataCollection.DataGroupFields, {
            onRowBound: function (obj, index, html) {
                //maak wrapper div, zet hierin de html van de tr
                //doe bewerkingen in deze html
                //zet weer terug in de tr (onderaan)
                var tempWrapperDiv = document.createElement('div');
                $(tempWrapperDiv).html(html);
                if (obj.IsSystemField) {
                    $(tempWrapperDiv).find('.bitDeleteButton').hide();
                    $(tempWrapperDiv).find('tr').css("color", "gray");
                }
                html = $(tempWrapperDiv).html();
                return html;
            }
        });
        $("#tableItemFields").dataBindList(BITDATACOLLECTIONS.dataCollection.DataItemFields, {
            onRowBound: function (obj, index, html) {
                //maak wrapper div, zet hierin de html van de tr
                //doe bewerkingen in deze html
                //zet weer terug in de tr (onderaan)
                var tempWrapperDiv = document.createElement('div');
                $(tempWrapperDiv).html(html);
                if (obj.IsSystemField) {
                    $(tempWrapperDiv).find('.bitDeleteButton').hide();
                    $(tempWrapperDiv).find('tr').css("color", "gray");
                }
                html = $(tempWrapperDiv).html();
                return html;
            }
        });
    },

    sortFieldsByTabAndOrderingNumber: function (field1, field2) {
        //concat
        var sort1 = field1.TabPage + field1.OrderingNumber + field1.Name;
        var sort2 = field2.TabPage + field2.OrderingNumber + field2.Name;

        if (sort1 < sort2) {
            return -1; //sort string ascending
        }
        else if (sort1 > sort2) {
            return 1;
        }
        else {
            return 0;
        }
    },

    newDataField: function (type) {
        BITDATACOLLECTIONS.openDataFieldDetailsPopup(null, type);
    },

    openDataFieldDetailsPopup: function (index, type) {
        BITDATACOLLECTIONS.dataField = BITDATACOLLECTIONS.getDataFieldFromArray(index, type);
        if (index == null) {
            $('#bitDataFieldConfigDialog').dialog({ title: "Nieuwe veld" });
        }
        else {
            $('#bitDataFieldConfigDialog').dialog({ title: "Veld: " + BITDATACOLLECTIONS.dataField.Name });
        }
        $('#bitDataFieldConfigDialog').dataBind(BITDATACOLLECTIONS.dataField);
        if (BITDATACOLLECTIONS.dataField.IsSystemField) {
            $("#textFieldName").attr("disabled", "disabled");
            $("#selectFieldType").attr("disabled", "disabled");
        }
        else {
            $("#textFieldName").removeAttr("disabled");
            $("#selectFieldType").removeAttr("disabled");
        }
        ////bij bestaande velden mag type niet worden gewijzigd
        //if (BITDATACOLLECTIONS.dataField.MappingColumn) {
        //    $("#selectFieldType").attr("disabled", "disabled");
        //}
        //else {
        //    $("#selectFieldType").removeAttr("disabled");
        //}

        BITDATACOLLECTIONS.changeFieldType();
        $('#bitDataFieldConfigDialog').dialog('open');
    },

    changeFieldType: function () {
        var fieldType = $("#selectFieldType").val();
        BITDATACOLLECTIONS.dataField.FieldType = fieldType;
        if (fieldType == FIELD_TYPES.DROPDOWN.toString() || fieldType == FIELD_TYPES.CHECKBOXLIST.toString()) //dropdown
        {
            $("#divLookupValues").show();
            BITDATACOLLECTIONS.bindLookupValues();
        }
        else {
            $("#divLookupValues").hide();
        }
        if (
            fieldType == FIELD_TYPES.DATETIME.toString() ||
            fieldType == FIELD_TYPES.NUMERIC.toString() ||
            //fieldType == FIELD_TYPES.CURRENCY.toString() ||
            fieldType == FIELD_TYPES.YESNO.toString()||
             BITDATACOLLECTIONS.dataField.MappingColumn == "Name" ) 
        {
            //niet meertalig
            $("input[data-field=IsMultiLanguageField]").attr("disabled", "disabled");
            $("input[data-field=IsMultiLanguageField]").removeAttr("checked");
        }
        else {
            $("input[data-field=IsMultiLanguageField]").removeAttr("disabled");
        }
    },

    saveDataField: function (e) {

        var validation = $('#bitDataFieldConfigDialog').validate();
        if (validation) {
            BITDATACOLLECTIONS.dataField = $('#bitDataFieldConfigDialog').collectData(BITDATACOLLECTIONS.dataField);
            //als datacollectie nieuw is, veld alleen in array zetten
            if (BITDATACOLLECTIONS.dataCollection.ID == BITUTILS.EMPTYGUID) {
                BITDATACOLLECTIONS.dataField.FieldTypeString = $("#selectFieldType option:selected").text();
                if (BITDATACOLLECTIONS.dataField.IsMultiLanguageField) {
                    BITDATACOLLECTIONS.dataField.IsMultiLanguageFieldString = "Ja";
                }
                else {
                    BITDATACOLLECTIONS.dataField.IsMultiLanguageFieldString = "Nee";
                }
                BITDATACOLLECTIONS.setDataFieldToArray(BITDATACOLLECTIONS.dataField);
                BITDATACOLLECTIONS.bindDataFields();
                $('#bitDataFieldConfigDialog').dialog('close');
            }
            else {
                //als datacollectie niet nieuw is, veld meteen opslaan in db
                BITDATACOLLECTIONS.dataField.DataCollection = new Object();
                BITDATACOLLECTIONS.dataField.DataCollection.ID = BITDATACOLLECTIONS.dataCollection.ID;
                jsonstring = convertToJsonString(BITDATACOLLECTIONS.dataField);
                BITAJAX.dataServiceUrl = "DataCollectionService.asmx";
                BITAJAX.callWebServiceASync("SaveDataField", jsonstring, function (data) {
                    BITDATACOLLECTIONS.setDataFieldToArray(data.d);
                    BITDATACOLLECTIONS.bindDataFields();
                    $('#bitDataFieldConfigDialog').dialog('close');
                }, null, e.srcElement);
            }

        }
    },

    removeDataField: function (id, index, type) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u dit veld verwijderen?", null, function () {
            if (type == 'group') {
                BITDATACOLLECTIONS.dataCollection.DataGroupFields.splice(index, 1);
            }
            else {
                BITDATACOLLECTIONS.dataCollection.DataItemFields.splice(index, 1);
            }
            BITDATACOLLECTIONS.dataField = null;
            BITDATACOLLECTIONS.bindDataFields();
            //nog niet opgeslagen velden alleen uit array halen
            if (id != BITUTILS.EMPTYGUID) {
                //reeds opgeslagen velden ook in db verwijderen
                //onmiddelijk verwijderen, want data in groepen en items moet ook worden geupdate
                var parametersObject = { id: id };
                var jsonstring = JSON.stringify(parametersObject);

                BITAJAX.callWebServiceASync("DeleteDataField", jsonstring, function (data) {
                });
            }
        });
    },

    getDataFieldFromArray: function (index, type) {
        var returnField = null;
        if (index == null) {
            //nieuw leeg object
            returnField = { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: type, Name: "", FieldType: 1, IsMultiLanguageField: false, TabPage: "", OrderingNumber: 0 };
            index = -1;
        }
        else {
            var dataFields = BITDATACOLLECTIONS.dataCollection.DataItemFields;
            if (type == 'group') {
                var dataFields = BITDATACOLLECTIONS.dataCollection.DataGroupFields;
            }
            returnField = dataFields[index];
        }
        returnField.index = index;
        return returnField;
    },

    setDataFieldToArray: function (dataField) {
        var fields = [];
        if (dataField.Type == 'group') {
            fields = BITDATACOLLECTIONS.dataCollection.DataGroupFields;
        }
        else {
            fields = BITDATACOLLECTIONS.dataCollection.DataItemFields;
        }
        if (dataField.index != null) {
            if (dataField.index == -1) {
                dataField.index = fields.length;
                fields.push(dataField);
            }
            fields[dataField.index] = dataField;
        }
        else {
            var exists = false;
            for (var i in fields) {
                if (fields[i].ID == dataField.ID) {
                    fields[i] = dataField;
                    exists = true;
                    break;
                }
            }
            if (!exists) {
                fields.push(dataField);
            }
        }
        if (dataField.Type == 'group') {
            BITDATACOLLECTIONS.dataCollection.DataGroupFields = fields;
        }
        else {
            BITDATACOLLECTIONS.dataCollection.DataItemFields = fields;
        }
    },

    //validateDataField: function (dataField) {
    //    var dataFieldsArray = BITDATACOLLECTIONS.dataCollection.DataItemFields
    //    if (dataField.Type == 'group') {
    //        dataFieldsArray = BITDATACOLLECTIONS.dataCollection.DataGroupFields;
    //    }
    //    //imageList = 10
    //    //fileList = 11
    //    if (dataField.FieldType == 10 || dataField.FieldType == 11) {
    //        //mag maar 1 ImageList voorkomen
    //        for (var i in dataFieldsArray) {
    //            var field = dataFieldsArray[i];
    //            if (field.FieldType == dataField.FieldType && field.ID != dataField.ID) {
    //                $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "Er mag maar 1 imageList en 1 fileList worden gebruikt.");
    //                return false;
    //            }
    //        }
    //    }
    //    return true;
    //},

    /////////////////////////////
    // DATALOOKUP VALUES
    /////////////////////////////
    bindLookupValues: function () {
        if (BITDATACOLLECTIONS.dataField.FieldType == FIELD_TYPES.DROPDOWN.toString() || BITDATACOLLECTIONS.dataField.FieldType == FIELD_TYPES.CHECKBOXLIST.toString()) {
            $("#tableLookupValues").dataBindList(BITDATACOLLECTIONS.dataField.LookupValues);
            $("#divLookupValues").show();
        }
        else {
            $("#divLookupValues").hide();
        }
    },

    newDataLookupValue: function () {
        BITDATACOLLECTIONS.openLookupValuePopup(null);
    },

    openLookupValuePopup: function (index) {
        BITDATACOLLECTIONS.dataLookupValue = BITDATACOLLECTIONS.getLookupValueFromArray(index);
        if (index == null) {
            $('#bitDataLookupValueDialog').dialog({ title: "Nieuwe waarde" });
        }
        else {
            $('#bitDataLookupValueDialog').dialog({ title: "Bewerk waarde: " + BITDATACOLLECTIONS.dataLookupValue.Name });
        }

        var languagesJsonString = BITDATACOLLECTIONS.dataLookupValue.LanguagesJsonString;
        var languages = $.parseJSON(languagesJsonString);

        BITDATACOLLECTIONS.dataLookupValue.Languages = languages;
        $('#bitDataLookupValueDialog').dataBind(BITDATACOLLECTIONS.dataLookupValue);
        BITDATACOLLECTIONS.dataField.IsMultiLanguageField = $("#checkIsMultiLanguageField").is(":checked");
        if (BITDATACOLLECTIONS.dataField.IsMultiLanguageField) {
            $('#divExtraLanguagesLookupValues').show();
        }
        else {
            $('#divExtraLanguagesLookupValues').hide();
        }
        $('#bitDataLookupValueDialog').dialog('open');
    },

    setDataLookupValue: function () {
        var validation = $('#bitDataLookupValueDialog').validate();
        if (validation) {
            BITDATACOLLECTIONS.dataLookupValue = $('#bitDataLookupValueDialog').collectData(BITDATACOLLECTIONS.dataLookupValue);

            var languagesObject = BITDATACOLLECTIONS.dataLookupValue.Languages;
            var languagesJsonString = JSON.stringify(languagesObject);
            BITDATACOLLECTIONS.dataLookupValue.LanguagesJsonString = languagesJsonString;
            BITDATACOLLECTIONS.dataLookupValue.Languages = null;

            BITDATACOLLECTIONS.setLookupValueToArray(BITDATACOLLECTIONS.dataLookupValue);
            BITDATACOLLECTIONS.bindLookupValues();
            $('#bitDataLookupValueDialog').dialog('close');
        }
    },

    removeLookupValue: function (index, id) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u deze waarde verwijderen?", null, function () {
            BITDATACOLLECTIONS.dataField.LookupValues.splice(index, 1);
            BITDATACOLLECTIONS.dataLookupValue = null;
            BITDATACOLLECTIONS.bindLookupValues();

            if (id != BITUTILS.EMPTYGUID) {
                //reeds opgeslagen velden ook in db verwijderen
                //onmiddelijk verwijderen, want data in groepen en items moet ook worden geupdate
                var parametersObject = { id: id };
                var jsonstring = JSON.stringify(parametersObject);

                BITAJAX.callWebService("DeleteDataLookupValue", jsonstring);
            }
        });
    },

    getLookupValueFromArray: function (index) {
        var returnValue = null;
        if (index == null) {
            //nieuw leeg object
            returnValue = { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Name: "", OrderingNumber: 0 };
        }
        else {
            var lookupValues = BITDATACOLLECTIONS.dataField.LookupValues;

            returnValue = lookupValues[index];
        }
        returnValue.index = index;
        return returnValue;
    },

    setLookupValueToArray: function (lookupValue) {
        if (lookupValue.IsNew) {
            if (!BITDATACOLLECTIONS.dataField.LookupValues) {
                BITDATACOLLECTIONS.dataField.LookupValues = [];
            }
            lookupValue.IsNew = false;
            BITDATACOLLECTIONS.dataField.LookupValues.push(lookupValue);
        }
        else {
            BITDATACOLLECTIONS.dataField.LookupValues[lookupValue.index] = lookupValue;
        }
    },

    fillCustomFieldSets: function () {
        this.defaultFieldsPerType[DATACOLLECTION_TYPES.SIMPLE] = {
            groupFields: [
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Naam", IsSystemField: true, MappingColumn: "Name", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Titel", IsSystemField: true, MappingColumn: "Title", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Volgnummer", IsSystemField: true, MappingColumn: "OrderNumber", FieldType: FIELD_TYPES.NUMERIC, FieldTypeString: "Numeriek", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 2 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Afbeelding", FieldType: FIELD_TYPES.IMAGE, FieldTypeString: "Image", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 3 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Thumbnail", FieldType: FIELD_TYPES.IMAGE, FieldTypeString: "Image", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 4 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Inhoud", FieldType: FIELD_TYPES.HTML, FieldTypeString: "Html", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "03 Inhoud", OrderingNumber: 5 }
            ],
            itemFields: [
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Naam", IsSystemField: true, MappingColumn: "Name", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Titel", IsSystemField: true, MappingColumn: "Title", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Volgnummer", IsSystemField: true, MappingColumn: "OrderNumber", FieldType: FIELD_TYPES.NUMERIC, FieldTypeString: "Numeriek", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 2 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Afbeelding", FieldType: FIELD_TYPES.IMAGE, FieldTypeString: "Image", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 3 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Thumbnail", FieldType: FIELD_TYPES.IMAGE, FieldTypeString: "Image", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 4 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Inhoud", FieldType: FIELD_TYPES.HTML, FieldTypeString: "Html", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "03 Inhoud", OrderingNumber: 5 }
            ]
        };

        this.defaultFieldsPerType[DATACOLLECTION_TYPES.DEFAULT] = {
            groupFields: [
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Naam", IsSystemField: true, MappingColumn: "Name", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Titel", IsSystemField: true, MappingColumn: "Title", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Volgnummer", IsSystemField: true, MappingColumn: "OrderNumber", FieldType: FIELD_TYPES.NUMERIC, FieldTypeString: "Numeriek", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 2 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Meta-Description", IsSystemField: true, MappingColumn: "MetaDescription", FieldType: FIELD_TYPES.LONGTEXT, FieldTypeString: "Lange tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 3 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Meta-Keywords", IsSystemField: true, MappingColumn: "MetaKeywords", FieldType: FIELD_TYPES.LONGTEXT, FieldTypeString: "Lange tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 4 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Afbeelding", FieldType: FIELD_TYPES.IMAGE, FieldTypeString: "Image", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Thumbnail", FieldType: FIELD_TYPES.IMAGE, FieldTypeString: "Image", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Intro", FieldType: FIELD_TYPES.HTML, FieldTypeString: "Html", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "03 Intro", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Inhoud", FieldType: FIELD_TYPES.HTML, FieldTypeString: "Html", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "04 Inhoud", OrderingNumber: 0 }
            ],
            itemFields: [
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Naam", IsSystemField: true, MappingColumn: "Name", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Titel", IsSystemField: true, MappingColumn: "Title", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Volgnummer", IsSystemField: true, MappingColumn: "OrderNumber", FieldType: FIELD_TYPES.NUMERIC, FieldTypeString: "Numeriek", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 2 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Meta-Description", IsSystemField: true, MappingColumn: "MetaDescription", FieldType: FIELD_TYPES.LONGTEXT, FieldTypeString: "Lange tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 3 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Meta-Keywords", IsSystemField: true, MappingColumn: "MetaKeywords", FieldType: FIELD_TYPES.LONGTEXT, FieldTypeString: "Lange tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 4 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Afbeelding", FieldType: FIELD_TYPES.IMAGE, FieldTypeString: "Image", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Thumbnail", FieldType: FIELD_TYPES.IMAGE, FieldTypeString: "Image", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Extra afbeeldingen", FieldType: FIELD_TYPES.IMAGELIST, FieldTypeString: "ImageList", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "03 Afbeeldingen", OrderingNumber: 2 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Extra bestanden", FieldType: FIELD_TYPES.FILELIST, FieldTypeString: "FileList", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "03 Afbeeldingen", OrderingNumber: 3 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Intro", FieldType: FIELD_TYPES.HTML, FieldTypeString: "Html", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "04 Intro", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Inhoud", FieldType: FIELD_TYPES.HTML, FieldTypeString: "Html", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "04 Inhoud", OrderingNumber: 0 }
            ]
        };

        this.defaultFieldsPerType[DATACOLLECTION_TYPES.NEWS] = {
            groupFields: [
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Naam", IsSystemField: true, MappingColumn: "Name", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Titel", IsSystemField: true, MappingColumn: "Title", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Volgnummer", IsSystemField: true, MappingColumn: "OrderNumber", FieldType: FIELD_TYPES.NUMERIC, FieldTypeString: "Numeriek", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 2 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Meta-Description", IsSystemField: true, MappingColumn: "MetaDescription", FieldType: FIELD_TYPES.LONGTEXT, FieldTypeString: "Lange tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 3 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Meta-Keywords", IsSystemField: true, MappingColumn: "MetaKeywords", FieldType: FIELD_TYPES.LONGTEXT, FieldTypeString: "Lange tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 4 }
            ],
            itemFields: [
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Naam", IsSystemField: true, MappingColumn: "Name", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Titel", IsSystemField: true, MappingColumn: "Title", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Volgnummer", IsSystemField: true, MappingColumn: "OrderNumber", FieldType: FIELD_TYPES.NUMERIC, FieldTypeString: "Numeriek", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 2 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Meta-Description", IsSystemField: true, MappingColumn: "MetaDescription", FieldType: FIELD_TYPES.LONGTEXT, FieldTypeString: "Lange tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 3 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Meta-Keywords", IsSystemField: true, MappingColumn: "MetaKeywords", FieldType: FIELD_TYPES.LONGTEXT, FieldTypeString: "Lange tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 4 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Afbeelding", FieldType: FIELD_TYPES.IMAGE, FieldTypeString: "Image", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Thumbnail", FieldType: FIELD_TYPES.IMAGE, FieldTypeString: "Image", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Extra afbeeldingen", FieldType: FIELD_TYPES.IMAGELIST, FieldTypeString: "ImageList", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 2 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Intro", FieldType: FIELD_TYPES.HTML, FieldTypeString: "Html", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "03 Intro", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Inhoud", FieldType: FIELD_TYPES.HTML, FieldTypeString: "Html", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "03 Inhoud", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Datum", FieldType: FIELD_TYPES.DATETIME, FieldTypeString: "DateTime", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Plaats", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 0 }
            ]
        };

        this.defaultFieldsPerType[DATACOLLECTION_TYPES.GOOGLEMAPS] = {
            groupFields: [
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Naam", IsSystemField: true, MappingColumn: "Name", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Titel", IsSystemField: true, MappingColumn: "Title", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Volgnummer", IsSystemField: true, MappingColumn: "OrderNumber", FieldType: FIELD_TYPES.NUMERIC, FieldTypeString: "Numeriek", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 2 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Meta-Description", IsSystemField: true, MappingColumn: "MetaDescription", FieldType: FIELD_TYPES.LONGTEXT, FieldTypeString: "Lange tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 3 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'group', Name: "Meta-Keywords", IsSystemField: true, MappingColumn: "MetaKeywords", FieldType: FIELD_TYPES.LONGTEXT, FieldTypeString: "Lange tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 4 },
            ],
            itemFields: [
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Naam", IsSystemField: true, MappingColumn: "Name", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Titel", IsSystemField: true, MappingColumn: "Title", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Volgnummer", IsSystemField: true, MappingColumn: "OrderNumber", FieldType: FIELD_TYPES.NUMERIC, FieldTypeString: "Numeriek", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 2 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Meta-Description", IsSystemField: true, MappingColumn: "MetaDescription", FieldType: FIELD_TYPES.LONGTEXT, FieldTypeString: "Lange tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 3 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Meta-Keywords", IsSystemField: true, MappingColumn: "MetaKeywords", FieldType: FIELD_TYPES.LONGTEXT, FieldTypeString: "Lange tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "01 Algemeen", OrderingNumber: 4 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Afbeelding", FieldType: FIELD_TYPES.IMAGE, FieldTypeString: "Image", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Thumbnail", FieldType: FIELD_TYPES.IMAGE, FieldTypeString: "Image", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "02 Afbeeldingen", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Intro", FieldType: FIELD_TYPES.HTML, FieldTypeString: "Html", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "03 Intro", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Inhoud", FieldType: FIELD_TYPES.HTML, FieldTypeString: "Html", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "03 Inhoud", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Adres", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "04 Geografisch", OrderingNumber: 0 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Postcode", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "04 Geografisch", OrderingNumber: 1 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Plaats", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "04 Geografisch", OrderingNumber: 2 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Land", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "04 Geografisch", OrderingNumber: 3 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Website", FieldType: FIELD_TYPES.TEXT, FieldTypeString: "Tekst", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "04 Geografisch", OrderingNumber: 4 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Latitude", IsSystemField: true, MappingColumn: "Latitude", FieldType: FIELD_TYPES.READONLY, FieldTypeString: "Alleen lezen", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "04 Geografisch", OrderingNumber: 5 },
                { ID: "00000000-0000-0000-0000-000000000000", IsNew: true, Type: 'item', Name: "Longitude", IsSystemField: true, MappingColumn: "Longitude", FieldType: FIELD_TYPES.READONLY, FieldTypeString: "Alleen lezen", IsMultiLanguageField: false, IsMultiLanguageFieldString: "Nee", TabPage: "04 Geografisch", OrderingNumber: 6 }
            ]
        };
    }
}; 