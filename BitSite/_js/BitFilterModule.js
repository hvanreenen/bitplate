var BITFILTERMODULE = {
    applyFilter: function (moduleId) {
        var valuesPerMappingColumn = {};
        //uit alle filtermodules op pagina alle waardes halen en opsturen naar server

        $("[data-filter-mapping-column]").each(function (i) {
            var fieldType = $(this).attr("data-filter-field-type");
            var mappingColumn = $(this).attr("data-filter-mapping-column");
            var value = $(this).val();
            if (fieldType == "lookups") {
                if ($(this).is(":checked")) {
                    //veldtype = checkboxlist
                    //stop meer waarden in 1 entry
                    if (!valuesPerMappingColumn[mappingColumn]) {
                        valuesPerMappingColumn[mappingColumn] = new Array();
                    }
                    value = $(this).attr("data-id");
                    valuesPerMappingColumn[mappingColumn].push(value);
                }
            }
            else {
                //veldtype = drop down, textbox, radio
                if ($(this).attr("type") == "radio") {
                    if ($(this).is(":checked")) {
                        valuesPerMappingColumn[mappingColumn] = value;
                    }
                }
                else {
                    valuesPerMappingColumn[mappingColumn] = value;
                }
            }
        });

        //NAVIGEER
        var navigationType = $('#hiddenModuleNavigationType' + moduleId.replace(/-/g, '')).val();
        if (navigationType == 'NavigateToPage') {
            //todo
        }
        else {
            var refreshModulesString = $('#hiddenRefreshModules'+ moduleId.replace(/-/g, '')).val();
            BITSITESCRIPT.reloadModulesOnSamePage(refreshModulesString, valuesPerMappingColumn);
        }

    },


    clearAll: function (moduleId) {
        //alle modules op pagina vervangen. moduleid is om door te geven aan applyFilter
        $("[data-filter-mapping-column]").each(function (i) {
            var type = $(this).attr("type");
            if (type == "radio" || type == "checkbox") {
                $(this).removeAttr("checked");
            }
            else {
                $(this).val('');
            }
        });
        BITFILTERMODULE.applyFilter(moduleId);
    }
};