//constants
var CONTROLTYPES = { TEXTBOX: "TEXTBOX", DROPDOWN: "DROPDOWN", CHECKBOX: "CHECKBOX", RADIO: "RADIO", CHECKBOXLIST: "CHECKBOXLIST", IMAGE: "IMAGE", HYPERLINK: "HYPERLINK", TEXTAREA: "TEXTAREA", OTHERHTML: "OTHERHTML", WYSIWYG: "WYSIWYG", LIST: "LIST", TABLE: "TABLE", TABLEROW: "TABLEROW", MULTISELECT: "MULTISELECT" };
//voor lists
var _htmlOriginalFormats = new Array();


(function ($) {
    $.fn.extend({
        //BIND DIV TO DATAOBJECT
        dataBind: function (dataObj, options) {

            if (!dataObj) return;
            /*
            $("input[type=date]").each(function (i) {
                $(this).datepicker({ changeMonth: true, changeYear: true });
            }); */

            function clearDates(dateText, inst) {
                if (dateText == null) {
                    $($(this).data("datepicker").settings.altField).val('');
                }
            }
            //$(this).find('[data-field]').each(function (i) {
            $(this).find('[data-field], [data-id-field]').each(function (i) {
                var dataControl = $(this)[0];
                var controlType = getControlType(dataControl);
                if (controlType) {
                    //get fieldnames
                    var dataFields = getDataFields(dataControl);
                    //get datavalues
                    var dataValues = getDataValues(dataObj, dataFields);
                    //bind to control
                    setControlValues(dataControl, controlType, dataValues);
                    $(dataControl).trigger('change'); //trigger jquery value change event.
                }
            });
            
            //if (dataObj.listIndex) {
            //    var html = $(this).html();
            //    html = html.replaceAll('[list-index]', dataObj.listIndex);
            //    $(this).html(html);
            //}
            return this;
        },

        dataBindList: function (list, options) {
            var rowFormat = "", bodyFormat = "";
            var controlId = $(this).attr('id');
            if (!controlId) {
                controlId = $(this).attr('class');
            }
            var dataControl = $(this)[0];
            //mocht control niet bestaan in de html dan niks doen
            if (!dataControl) return;
            var controlType = getControlType(dataControl);

            //if (controlType == CONTROLTYPES.OTHERHTML) controlType = CONTROLTYPES.TABLE;
            if (!_htmlOriginalFormats[controlId]) {
                //if (controlType == CONTROLTYPES.LIST) {
                //    rowFormat = $(this).html();
                //}
                //else
                if (controlType == CONTROLTYPES.TABLE) {
                    rowFormat = $(dataControl).find('tbody').html();
                    createHeader(dataControl, options);
                }
                else {
                    rowFormat = $(this).html();
                }
                rowFormat = rowFormat.replace(/data-child-field/g, 'data-field');
                _htmlOriginalFormats[controlId] = rowFormat;
            }
            else {
                rowFormat = _htmlOriginalFormats[controlId];
            }

            var html = document.createElement('div'); //TempElement

            for (var i in list) {
                var obj = list[i];
                //voeg een index toe aan het object
                obj.listIndex = i;
                var elm = document.createElement('div');
                
                $(elm).html(rowFormat);
                var rowHtml = $(elm).dataBind(obj).children();
                //var rowHtml = $(elm).dataBind(obj).html();

                if (options && options.onRowBound) {
                    rowHtml = options.onRowBound(obj, i, rowHtml);
                }
                
                $(rowHtml).appendTo(html)// += rowHtml;
            }
            if (list == null || list.length == 0) {
                //html = "<tr>Er zijn geen gegevens</tr>";
            }
            //hack: om  te voorkomen dat de elementen met de datafield nog een keer worden gebonden aan data
            //html = html.replace(/data-field/g, "data-child-field");
            //if (controlType == CONTROLTYPES.LIST) {
            //    $(this).html(html);
            //}
            //else
            var childeren = $(html).children();

            if (controlType == CONTROLTYPES.TABLE) {
                $(dataControl).find('tbody').html('');
                $(childeren).appendTo($(dataControl).find('tbody'));
                DATABINDER.putArrow($(this).attr("id"));
            }
            else {
                $(this).html('');
                $(childeren).appendTo(this);// $(this).html(html);
            }
            if (options && options.onDataLoaded) {
                options.onDataLoaded();
            }
        },

        fillDropDown: function (list, firstValue, valuefield) {
            //als niet bestaat dan doe niks
                 if (!$(this).get(0)) return;

            var itemFields = getItemFields(this);
            if (!valuefield) {
                valuefield = itemFields.valuefield;
            }
            var textfield = itemFields.textfield;
            //clear all options
            $(this).get(0).length = 0;
            if (firstValue && typeof firstValue != 'number') {
                var id = firstValue;
                var text = firstValue;
                if (firstValue.value != undefined) {
                    id = firstValue.value;
                }
                if (firstValue.text != undefined) {
                    text = firstValue.text;
                }
                $(this).append($('<option></option>').val(id).html(text));
            }
            for (var row in list) {
                var dataObj = list[row];
                dataObj.listIndex = row; //add listIndex to listitem
                var id = dataObj;
                var text = dataObj;
                if (dataObj[valuefield]) {
                    id = dataObj[valuefield];
                }
                if (dataObj[textfield]) {
                    text = dataObj[textfield];
                }
                $(this).append($('<option></option>').val(id).html(text));
            }

            if (typeof firstValue == 'number') {
                if ($(this).children().length >= firstValue) {
                    $($(this).children()[firstValue]).attr('selected', 'selected');
                }
            }
        },

        fillCheckboxList: function (list) {
            this.html("");
            var itemFields = getItemFields(this);
            var valuefield = "ID";
            var textfield = itemFields.textfield;

            for (var row in list) {
                var dataObj = list[row];
                var id = dataObj;
                var text = dataObj;
                var ctlId = randomID();
                if (dataObj[valuefield]) {
                    id = dataObj[valuefield];
                }
                if (dataObj[textfield]) {
                    text = dataObj[textfield];
                }
                $(this).append('<input id="' + ctlId + '" value="' + id + '" type="checkbox"/> <label for="' + ctlId + '">' + text + '</label><br />');
            }
        },

        fillRadioList: function (list, groupName) {
            this.html("");
            var itemFields = getItemFields(this);
            var valuefield = itemFields.valuefield;
            var textfield = itemFields.textfield;

            if (!groupName) {
                groupName = typeof (list).toString();
            }
            for (var row in list) {
                var dataObj = list[row];
                var id = dataObj;
                var text = dataObj;
                var ctlId = randomID();
                if (dataObj[valuefield]) {
                    id = dataObj[valuefield];
                }
                if (dataObj[textfield]) {
                    text = dataObj[textfield];
                }
                $(this).append('<input id="' + ctlId + '" value="' + id + '" type="radio" name="' + groupName + '"/> <label for="' + ctlId + '">' + text + '</label><br />');

            }
        },

        collectData: function (dataObj) {
            //Maak nieuw dataobj als dataobj niet geinitieerd is.
            dataObj = (!dataObj) ? dataObj = new Object() : dataObj;

            var dataControls = $(this).find('[data-field], [data-control-type]');
            for (var i in dataControls) {
                var dataControl = dataControls[i];
                var controlType = getControlType(dataControl);
                if (controlType) {
                    //get control value
                    var value = getControlValue(dataControl, controlType);
                    if (value != undefined) { //ervan uitgaande dat lege waardes "" krijgen
                        //get fieldname
                        var fieldName = $(dataControl).attr('data-field');
                        //set value op dataobject by fieldname
                        setDataValue(dataObj, fieldName, value);
                    }
                }
            }
            return dataObj;
        }
    });
})(jQuery);

///FUNCTIES
function getControlType(dataControl) {
    var controltype;
    if (dataControl && dataControl.tagName) {
        var tag = dataControl.tagName;
        var type = $(dataControl).attr('type');
        if (tag == "INPUT") {
            if (type == "checkbox") {
                controltype = CONTROLTYPES.CHECKBOX;
            }
            else if (type == "radio") {
                controltype = CONTROLTYPES.RADIO;
            }
            else {
                controltype = CONTROLTYPES.TEXTBOX;
            }
        }
        else if (tag == "TEXTAREA") {
            controltype = CONTROLTYPES.TEXTAREA;
        }
        else if (tag == "SELECT") {
            var multiple = $(dataControl).attr('multiple');
            if (multiple) {
                controltype = CONTROLTYPES.MULTISELECT;
            }
            else {
                controltype = CONTROLTYPES.DROPDOWN;
            }
        }
        else if (tag == "IMG") {
            controltype = CONTROLTYPES.IMAGE;
        }
        else if (tag == "A") {
            controltype = CONTROLTYPES.HYPERLINK;
        }
        else if (tag == "TABLE") {
            controltype = CONTROLTYPES.TABLE;
        }
        else if (tag == "TR") {
            controltype = CONTROLTYPES.TABLEROW;
        }
        else {
            var test = $(dataControl).attr("data-control-type");
            if ($(dataControl).attr("data-control-type") == "checkboxlist") {
                controltype = CONTROLTYPES.CHECKBOXLIST;
            }
            else if ($(dataControl).attr("data-control-type") == "radiolist") {
                controltype = CONTROLTYPES.RADIO;
            }
            else if ($(dataControl).attr("data-control-type") == "list") {
                controltype = CONTROLTYPES.LIST;
            }
            else if ($(dataControl).attr("data-control-type") == "table") {
                controltype = CONTROLTYPES.TABLE;
            }
            else if ($(dataControl).attr("data-control-type") == "wysiwyg" || $(dataControl).attr("contenteditable") == "true") {
                controltype = CONTROLTYPES.WYSIWYG;
            }
            else {
                controltype = CONTROLTYPES.OTHERHTML;
            }
        }

    }
    return controltype;
}

//GET ALL DATAFIELDS
//voor sommige controls worden meerdere attributen met data gezet
//bijv. een img waarbij de src en de alt met data wordt gezet
function getDataFields(dataControl) {
    var dataField = $(dataControl).attr('data-field');
    var dataTextField = $(dataControl).attr('data-text-field');
    var dataTitleField = $(dataControl).attr('data-title-field');
    var dataIDField = $(dataControl).attr('data-id-field');
    return { dataField: dataField, dataTextField: dataTextField, dataTitleField: dataTitleField, dataIDField: dataIDField };
}

//GET CONTROL VALUE
function getControlValue(dataControl, controlType) {
    var value;
    if (controlType == CONTROLTYPES.CHECKBOX) {
        value = $(dataControl).is(':checked');
    }
    else if (controlType == CONTROLTYPES.RADIO) {
        if ($(dataControl).is(':checked')) {
            value = $(dataControl).val();
        }
    }
    else if (controlType == CONTROLTYPES.DROPDOWN) {
        value = $(dataControl).val();
        if (value == "") value = null;
    }
    else if (controlType == CONTROLTYPES.TEXTBOX) {
        value = $(dataControl).val();
        var format = $(dataControl).attr("data-format");

        if ($(dataControl).attr("class") == "hasDatepicker") {
            if (!format) {
                format = "dd-MM-yyyy HH:mm";
            }
            value = value.toDateFromFormat(format);
        }
        if (format == "C") {
            value = value.replace(",", ".");
        }
    }
    else if (controlType == CONTROLTYPES.TEXTAREA) {
        value = $(dataControl).val();
    }
    else if (controlType == CONTROLTYPES.WYSIWYG) {
        var id = $(dataControl).attr("id");
        //value = BITEDITOR.editors[id].getHtml();
        value = CKEDITOR.editors[id].getData();
    }
    else if (controlType == CONTROLTYPES.CHECKBOXLIST) {
        var list = []; //nieuwe lege loadScripts
        //alle uit zetten
        $(dataControl).find('input[type=checkbox]').each(function (i) {
            if ($(this).is(':checked')) {
                var obj = new Object();
                obj.ID = $(this).val();
                var attr = $(dataControl).attr('data-list-type');
                if (attr == "stringArray") {
                    //voor string-only arrays
                    obj = $(this).val();
                }

                list.push(obj);
            }
        });
        value = list;
    }
    else if (controlType == CONTROLTYPES.MULTISELECT) {
        value = $(dataControl).val();
    }
    else if (controlType == CONTROLTYPES.IMAGE) {
        value = $(dataControl).attr('src');
        //onsole.log(value);
    }
    return value;
}

//SET CONTROL VALUE
function setControlValues(dataControl, controlType, dataValues) {
    var dataValue = dataValues.dataField;
    //dataValue = (dataValue == undefined) ? '' : dataValue;
    if (typeof (dataValue) != "string") {
        //dataValue = dataValue.toString();
    }
    var dataTitleValue = dataValues.dataTitleField;
    var dataID = dataValues.dataIDField;
    if (dataID) {
        var id = $(dataControl).attr("id");
        if (id) {
            $(dataControl).attr("id", id + dataID)
        }
        else {
            $(dataControl).attr("id", dataID)
        }
    }

    //dataTitleValue = (dataTitleValue == undefined) ? '' : dataTitleValue;

    //onclick data-field replacement support
    var onclick = $(dataControl).attr('onclick');
    if ($(dataControl).data('onclick')) {
        onclick = $(dataControl).data('onclick');
    }
    else {
        //onclick opslaan in variable voor vervanging van [data-field]
        onclick = $(dataControl).attr('onclick');
        $(dataControl).data('onclick', onclick)
        
    }
    if (onclick) {
        onclick = onclick.replace("[data-field]", dataValue ? dataValue : "");
        onclick = onclick.replace("[list-index]", dataValues.listIndex ? dataValues.listIndex : "");
        var title = dataValues.dataTitleField ? dataValues.dataTitleField : "";
        //escapen single quote voor als je er iets mee wilt doen in js, zie bug#154
        title = title.replace("'", "\\'");
        onclick = onclick.replace("[data-title-field]", title);
        var text = dataValues.dataTextField ? dataValues.dataTextField : "";
        text = text.replace("'", "\\'");
        onclick = onclick.replace("[data-text-field]", text);

        onclick = onclick.replace("data-field", dataValue ? dataValue : "");
        if (onclick == "") onclick = dataValue;
        $(dataControl).attr('onclick', onclick);
        $(dataControl).css('cursor', 'pointer');

        if ($(dataControl).attr('data-text-field')) {
            if (dataValues.dataTextField) {
                $(dataControl).html(dataValues.dataTextField);
            }
        }

        if ($(dataControl).attr('data-title-field')) {
            if (dataValues.dataTitleField) {
                $(dataControl).attr('title', dataValues.dataTitleField);
            }
        }
    }
    //end onclick support


    if (controlType == CONTROLTYPES.CHECKBOX) {
        //$(dataControl).attr('checked', dataValue.toString() == "true" ? 'checked' :
        if (dataValue == true || dataValue == 'True') {
            $(dataControl).attr('checked', 'checked');
        }
        else {
            $(dataControl).removeAttr('checked');
        }
        //$(dataControl).attr('checked', (dataValue) ? 'checked' : '');
        $(dataControl).attr('title', dataTitleValue ? dataTitleValue : "");
    }
    else if (controlType == CONTROLTYPES.RADIO) {
        var val = $(dataControl).val();
        //javascript zie 0 ook als null, hierom expliciet 0 als string zetten
        if (dataValue == 0) dataValue = "0";
        if (dataValue && val == dataValue.toString()) {
            $(dataControl).attr('checked', 'checked');
        }
        else {
            $(dataControl).removeAttr('checked');
        }
        //$(dataControl).attr('checked', $(dataControl).val() == dataValue.toString() ? 'checked' : '');
    }
    else if (controlType == CONTROLTYPES.DROPDOWN) {
        //javascript zie 0 ook als null, hierom expliciet 0 als string zetten
        if (dataValue == 0) dataValue = "0";
        $(dataControl).val(dataValue ? dataValue.toString() : "");
        //zorg dat er altijd 1 geselecteerd is
        if ($(dataControl)[0].selectedIndex == -1) $(dataControl)[0].selectedIndex = 0;

    }
    else if (controlType == CONTROLTYPES.TEXTBOX) {
        var format = $(dataControl).attr('data-format');
        if (dataValue && dataValue.constructor == Date) {
            if (!format) {
                format = "dd-MM-yyyy HH:mm";
            }
            //console.log($.browser);
            //if ($($.browser.webkit)) {
            //    format = "yyyy-MM-ddTHH:mm";                  //!! HTML5 datefieldformat (VERPLICHT)
            //}

            var type2 = typeof dataValue;
            dataValue = dataValue.format(format);
            var type3 = typeof dataValue;
        }
        //0 wordt als lege waarde gezien in js?
        var type = typeof dataValue;
        if (format == "C") {
            dataValue = dataValue.toString().replace(",", ".");
            if (isNaN(dataValue)) {
                dataValue = "";
                $(dataControl).val(dataValue);
            }
            else {

                dataValue = parseFloat(dataValue).toFixed(2);
                $(dataControl).val(dataValue);
            }
        }
        else if (format == "N") {
            if (isNaN(dataValue)) {
                dataValue = "";
                $(dataControl).val(dataValue);
            }
            else {
                dataValue = parseFloat(dataValue).toFixed(0);
                $(dataControl).val(dataValue);
            }
        }
        else if (type.toLowerCase() == "string") {
            $(dataControl).val(dataValue);
        }
        else if (type.toLowerCase() == "number") {
            $(dataControl).val(dataValue);
        }
        else {
            $(dataControl).val(dataValue ? dataValue : "");
        }
        //if (dataValue && dataValue.toString() == "") {
        //    //doe niks
        //}
        //else if (dataValue == 0) {
        //    dataValue = "0";
        //}


        $(dataControl).attr('title', dataTitleValue ? dataTitleValue : "");
    }
    else if (controlType == CONTROLTYPES.TEXTAREA) {
        $(dataControl).val(dataValue ? dataValue : "");
        $(dataControl).attr('title', dataTitleValue ? dataTitleValue : "");
    }
    else if (controlType == CONTROLTYPES.HYPERLINK) {
        var href = $(dataControl).attr('href');
        if ($(dataControl).data('href')) {
            href = $(dataControl).data('href');
        }
        else {
            //href opslaan in variable voor vervanging van [data-field]
            href = $(dataControl).attr('href');
            $(dataControl).data('href', href)
        }
        if (!href) href = "";
        href = href.replace("[data-field]", dataValue ? dataValue : "");
        href = href.replace("[list-index]", dataValues.listIndex ? dataValues.listIndex : "");
        var title = dataValues.dataTitleField ? dataValues.dataTitleField : "";
        //escapen single quote voor als je er iets mee wilt doen in js, zie bug#154
        title = title.toString().replace("'", "\\'");
        href = href.replace("[data-title-field]", title);
        var text = dataValues.dataTextField ? dataValues.dataTextField : "";
        text = text.replace("'", "\\'");
        href = href.replace("[data-text-field]", text);

        href = href.replace("data-field", dataValue ? dataValue : "");
        if (href == "") href = dataValue;
        $(dataControl).attr('href', href);
        var dataTextValue = dataValues.dataTextField;
        //$(dataControl).attr('data-text-field');
        if (dataTextValue) {
            //var dataTextValue = getDataValue(dataObj, textFieldName);
            $(dataControl).html(dataTextValue);
        }
        $(dataControl).attr('title', dataValues.dataTitleValue ? dataValues.dataTitleValue : ""); 
    }
    else if (controlType == CONTROLTYPES.IMAGE) {
        var src = $(dataControl).attr('src');
        if ($(dataControl).data('src')) {
            src = $(dataControl).data('src');
        }
        else {
            //src opslaan in variable voor vervanging van [data-field]
            src = $(dataControl).attr('src');
            $(dataControl).data('src', src)
        }
        if (src && src.indexOf('[data-field]') >= 0) {
            src = src.replace('[data-field]', dataValue ? dataValue : "");
            $(dataControl).attr('src', src);
        }
        else {
            $(dataControl).attr('src', dataValue ? dataValue : "");
        }
        var dataTitleValue = dataValues.dataTitleField;
        $(dataControl).attr('alt', dataTitleValue ? dataTitleValue : "");
        $(dataControl).attr('title', dataTitleValue ? dataTitleValue : "");
        //src = $(dataControl).attr('src');
        //if (src == '') {
        //    $(dataControl).hide();
        //}
        //else {
        //    //src = '../' + src;
        //    $(dataControl).attr('src', src);
        //    $(dataControl).show();
        //}
        if (!dataValue) {
            $(dataControl).hide();
        }
        else {
            $(dataControl).show();
        }
    }
    else if (controlType == CONTROLTYPES.CHECKBOXLIST) {
        //eerst alle uit zetten
        $(dataControl).find('input[type=checkbox]').each(function (i) {
            $(this).removeAttr('checked');
        });
        //die in de lijst vookomen aanzetten
        var list = dataValue;
        for (var i in list) {
            var id = list[i];
            if (list[i].ID) {
                id = list[i].ID;
            }
            $(dataControl).find('input[type=checkbox][value=' + id + ']').attr('checked', 'checked');
        }
    }
    else if (controlType == CONTROLTYPES.MULTISELECT) {
        //eerst alle uit zetten
        $(dataControl).find('option').each(function (i) {
            $(this).removeAttr('selected');
        });
        //die in de lijst vookomen aanzetten
        var list = dataValue;
        for (var i in list) {
            var id = list[i];
            if (list[i].ID) {
                id = list[i].ID;
            }
            $(dataControl).find('option[value=' + id + ']').attr('selected', 'selected');
        }
    }
    else if (controlType == CONTROLTYPES.LIST || controlType == CONTROLTYPES.TABLE) {
        var rowFormat = "", bodyFormat = "";

        var controlId = $(dataControl).attr('id');
        if (!controlId) {
            controlId = controlType;
        }
        if (!_htmlOriginalFormats[controlId]) {
            if (controlType == CONTROLTYPES.LIST) {
                rowFormat = $(dataControl).html();
            }
            else if (controlType == CONTROLTYPES.TABLE) {
                rowFormat = $(dataControl).find('tbody').html();
                bodyFormat = $(dataControl).html();
            }
            _htmlOriginalFormats[controlId + "body"] = bodyFormat;
            _htmlOriginalFormats[controlId] = rowFormat;
        }
        else {
            rowFormat = _htmlOriginalFormats[controlId];
            bodyFormat = _htmlOriginalFormats[controlId + "body"];
        }

        var html = "";
        var list = dataValue;
        for (var i in list) {
            var obj = list[i];
            var elm = document.createElement("div");
            if (controlType == CONTROLTYPES.TABLE) {
                elm = document.createElement("tbody");
            }
            $(elm).html(rowFormat);
            html += $(elm).dataBind(obj).html();
        }
        if (controlType == CONTROLTYPES.TABLE) {
            html = bodyFormat.replace(rowFormat, html);
        }
        $(dataControl).html(html);
    }
    else if (controlType == CONTROLTYPES.TABLEROW) {
        var dataTitleValue = dataValues.dataTitleField;
        if (dataTitleValue) {
            $(dataControl).attr('title', dataTitleValue ? dataTitleValue : "");
        }
    }
    else if (controlType == CONTROLTYPES.WYSIWYG) {
        var id = $(dataControl).attr("id");
        //BITEDITOR.editors[id].setHtml(dataValue ? dataValue : "");
        CKEDITOR.editors[id].setData(dataValue ? dataValue : "");
        //$(dataControl).html(dataValue);
    }
    else { //other
        var format = $(dataControl).attr('data-format');
        if (dataValue && dataValue.constructor == Date) {

            if (!format) {
                format = "dd-MM-yyyy HH:mm";
            }
            dataValue = dataValue.format(format);
        }
        else if (format == "C") {
            //num = isNaN(num) || num === '' || num === null ? 0.00 : num;
            dataValue = parseFloat(dataValue).toFixed(2);
        }

        if (($(dataControl).html() != dataValues.dataTextField || $(dataControl).html() == '') && !$(dataControl).attr('data-text-field')) { //if onclick data-text-field support fix
            $(dataControl).html(dataValue ? dataValue : "");
        }
        
        var dataTitleValue = dataValues.dataTitleField;
        if (dataTitleValue) {
            $(dataControl).attr('title', dataTitleValue ? dataTitleValue : "");
        }
    }
}

//GET DATA VALUES
function getDataValues(dataObj, fieldNames) {
   
    var dataValues = {};
    for (var key in fieldNames) {

        var fieldName = fieldNames[key];
        if (fieldName) {
            var dataValue = getDataValue(dataObj, fieldName);
            dataValues[key] = dataValue;
        }

    }
    if (dataObj.listIndex) {
        dataValues.listIndex = dataObj.listIndex;
    }
    return dataValues;
}

//GET DATA VALUE
function getDataValue(dataObj, fieldName) {
    var dataValue;
    if (fieldName.indexOf(".") > 0) {
        subFieldNames = fieldName.split('.');
        if (subFieldNames.length == 2) {
            if (dataObj[subFieldNames[0]]) {
                dataValue = dataObj[subFieldNames[0]][subFieldNames[1]];
            }
        }
        else if (subFieldNames.length == 3) {
            if (dataObj[subFieldNames[0]] && dataObj[subFieldNames[0]][subFieldNames[1]]) {
                dataValue = dataObj[subFieldNames[0]][subFieldNames[1]][subFieldNames[2]];
            }
        }
        else if (subFieldNames.length == 4) {
            if (dataObj[subFieldNames[0]] && dataObj[subFieldNames[0]][subFieldNames[1]] && dataObj[subFieldNames[0]][subFieldNames[1]][subFieldNames[2]]) {
                dataValue = dataObj[subFieldNames[0]][subFieldNames[1]][subFieldNames[2]][subFieldNames[3]];
            }
        }
        else if (subFieldNames.length > 4) {
            throw "Maximale niveau van subobjecten bereikt (4)";
        }
    }
    else {
        dataValue = dataObj[fieldName];
    }
    if (JSON.isJsonDate(dataValue)) {
        dataValue = JSON.json2Date(dataValue);
    }
    return dataValue;
}

//SET DATA VALUE
function setDataValue(dataObj, fieldName, dataValue) {
    if (fieldName && fieldName.indexOf(".") > 0) {
        subFieldNames = fieldName.split('.');
        if (subFieldNames.length == 2) {
            if (!dataObj[subFieldNames[0]]) {
                dataObj[subFieldNames[0]] = new Object();
            }
            dataObj[subFieldNames[0]][subFieldNames[1]] = dataValue;
        }
        else if (subFieldNames.length == 3) {
            if (!dataObj[subFieldNames[0]]) {
                dataObj[subFieldNames[0]] = new Object();
            }
            if (!dataObj[subFieldNames[0]][subFieldNames[1]]) {
                dataObj[subFieldNames[0]][subFieldNames[1]] = new Object();
            }
            dataObj[subFieldNames[0]][subFieldNames[1]][subFieldNames[2]] = dataValue;
        }
        else if (subFieldNames.length == 4) {
            if (!dataObj[subFieldNames[0]]) {
                dataObj[subFieldNames[0]] = new Object();
            }
            if (!dataObj[subFieldNames[0]][subFieldNames[1]]) {
                dataObj[subFieldNames[0]][subFieldNames[1]] = new Object();
            }
            if (!dataObj[subFieldNames[0]][subFieldNames[1]][subFieldNames[2]]) {
                dataObj[subFieldNames[0]][subFieldNames[1]][subFieldNames[2]] = new Object();
            }
            dataObj[subFieldNames[0]][subFieldNames[1]][subFieldNames[2]][subFieldNames[3]] = dataValue;
        }
        else if (subFieldNames.length > 4) {
            throw "Maximale niveau van subobjecten bereikt (4)";
        }
    }
    else {
        dataObj[fieldName] = dataValue;
    }
}

//voor dropdowns en checkbox lists
//return object met valuefield en textfield
function getItemFields(control) {
    var valuefield = $(control).attr('data-field');
    var textfield = $(control).attr('data-text-field');
    //als het leeg is: de standaard nemen
    if (!valuefield) {
        valuefield = "ID";
    }
    if (!textfield) {
        textfield = "Name";
    }
    //als er punt in staat bij subobjecten: laatste veldnaam is de naam die je hier moet hebben
    if (valuefield.indexOf(".") > 0) {
        var properties = valuefield.split('.');
        valuefield = properties[properties.length - 1];
    }
    if (textfield.indexOf(".") > 0) {
        var properties = textfield.split('.');
        textfield = properties[properties.length - 1];
    }

    return { valuefield: valuefield, textfield: textfield };
}
//voor label-for en ctlID's
function randomID() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}

//voor Tabel
//dit is een hulp class voor sorteren en pagineren.
//de class geeft ook de events door, doordat DATABINDER een vaste variabele is.
var DATABINDER = {
    //als er meer grids op 1 pagina staan, moet per grid de sorteer en pagineer functie afzonderlijk werken
    //in onderstaande array worden de tableBinderHelper Objecten per grid bewaard
    tableBinderHelpers: [],

    sort: function (sortField, tableid) {
        var tableBinderHelper = DATABINDER.tableBinderHelpers[tableid];
        tableBinderHelper.sort(sortField);

        //geef door aan aanroeper
        //DATABINDER.onSort(sortField + " " + DATABINDER.currentsortorder);
        //sorter.onSort(sortField + " " + DATABINDER.currentsortorder);
    },

    putArrow: function (tableid) {
        var tableBinderHelper = DATABINDER.tableBinderHelpers[tableid];
        if (tableBinderHelper) {
            tableBinderHelper.putArrow();
        }
    }
};

function TableBinderHelper() {
    var self = this;
    this.tableid = "";
    this.currentsortfield = "Name";
    this.currentsortorder = "ASC";
    this.pagesize = 5;
    this.numberOfPagesShown = 10;

    this.onSort = function (fieldName) { }
    this.onPageIndexCheched = function (fieldName) { }

    this.sort = function (sortField) {
        if (self.currentsortfield != sortField) {
            self.currentsortorder = "ASC";
        }
        else {
            if (self.currentsortorder == "ASC" || self.currentsortorder == "") {
                self.currentsortorder = "DESC";
            }
            else if (self.currentsortorder == "DESC") {
                self.currentsortorder = "ASC";
            }
        }
        self.currentsortfield = sortField;
        self.putArrow();
        self.onSort(sortField + " " + self.currentsortorder);
    }

    this.putArrow = function () {
        $('#' + self.tableid).find('.sortDirectionArrow').html("");
        var span = $('#' + self.tableid).find('[data-sort-field=' + self.currentsortfield + ']').find('span.sortDirectionArrow');
        if (self.currentsortorder == "ASC") {
            $(span).html(" &#9660;");
        }
        else if (self.currentsortorder == "DESC") {
            $(span).html(" &#9650;");
        }
    }
}

function createHeader(table, options) {
    //var sortFunction = $(table).attr('data-sort-function');
    //if (!sortFunction) {
    //    sortFunction = "sort";
    //}
    var tableid = $(table).attr("id");
    if (options && options.onSort) {
        var tableBinderHelper = new TableBinderHelper();
        tableBinderHelper.tableid = tableid;
        tableBinderHelper.onSort = options.onSort;
        //DATABINDER.onSort = options.onSort;
        DATABINDER.tableBinderHelpers[tableid] = tableBinderHelper;
    }
    $(table).find('[data-sort-field]').each(function (i) {

        var html = $(this).html();
        var datasortfield = $(this).attr("data-sort-field");
        var newhtml = "<a href=\"javascript:DATABINDER.sort('" + datasortfield + "', '" + tableid + "');\">" + html + "</a> <span class='sortDirectionArrow'></span>";
        $(this).html(newhtml);
    });
}

//function sort(sortField) {

//    if (DATABINDER.currentsortfield != sortField) {
//        DATABINDER.currentsortorder = "ASC";
//    }
//    else {
//        if (DATABINDER.currentsortorder == "ASC") {
//            DATABINDER.currentsortorder = "DESC";
//        }
//        else if (DATABINDER.currentsortorder == "DESC") {
//            DATABINDER.currentsortorder = "ASC";
//        }
//    }
//    DATABINDER.currentsortfield = sortField;
//    var currentsort = DATABINDER.currentsortfield + " " + DATABINDER.currentsortorder;

//    if (DATABINDER.options && DATABINDER.options.onSort) {
//        DATABINDER.options.onSort(sortField + " " + DATABINDER.currentsortorder);
//    }
//}


//PAGINERING

(function ($) {
    $.fn.extend({
        createPager: function (totalRowCount, options) {
            var pageSize = 25;
            if (options && options.pageSize) pageSize = options.pageSize;
            var numberOfPagesShown = 10; //bij meer dan 10 komen er puntjes (...) ipv 11,12,13 enz.
            if (options && options.numberOfPagesShown) numberOfPagesShown = options.numberOfPagesShown;
            currentPage = 1;
            if (options && options.currentPage) currentPage = options.currentPage;

            if (options && options.onPageIndexChanged) {
                //DATABINDER HOUDT FUNCTIE (EVENT) VAST EN GEEFT HET DOOR.
                DATABINDER.loadPagingPage = options.onPageIndexChanged;
            }
            else {
                alert("Code error: pager.options.onPageIndexChanged moet worden gedefinieerd, anders kan niet naar een ander pagina worden genavigeerd.");
            }
            //opbouwen paginering
            var html = "";
            var pagesCount = parseInt(totalRowCount / pageSize) + 1;

            var start = 1;
            if (currentPage > numberOfPagesShown) {
                start = (parseInt(currentPage / numberOfPagesShown) * numberOfPagesShown);
            }
            if (start > 1) {
                html += "<a href='javascript:DATABINDER.loadPagingPage(" + (start - 1) + ");'>...</a> ";
            }
            var end = start + numberOfPagesShown;
            if ((start + numberOfPagesShown) > pagesCount) {
                end = pagesCount;
            }
            for (var i = start; i < end; i++) {
                if (i == currentPage) {
                    html += "<a href='javascript:DATABINDER.loadPagingPage(" + i + ");'><b>" + i + "</b></a> ";
                }
                else {
                    html += "<a href='javascript:DATABINDER.loadPagingPage(" + i + ");'>" + i + "</a> ";
                }
            }
            if (end < pagesCount) {
                html += "<a href='javascript:DATABINDER.loadPagingPage(" + (end) + ");'>...</a> ";
            }
            else if (end == pagesCount) {
                html += "<a href='javascript:DATABINDER.loadPagingPage(" + (end) + ");'>" + (end) + "</a> ";
            }
            $(this).html('');
            if (html != "" && end > 1) {
                $(this).html("Pagina's: " + html);
            }
        }
    });
})(jQuery);