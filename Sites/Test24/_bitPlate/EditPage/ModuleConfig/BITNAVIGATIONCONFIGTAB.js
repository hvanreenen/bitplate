var BITNAVIGATIONCONFIGTAB = {
    dataBind: function () {
        var template = $("#divNavigationActionsTemplate").html();
        
        $("#divNavigationActions").empty();
        $(BITEDITPAGE.currentModule.NavigationActions).each(function (i) {
            var tempTemplate = $('<div></div>').html(template);
            var NavigationAction = this;
            $(tempTemplate).find('#navigationTagName_0').attr('id', 'navigationTagName_' + i).html(this.Name);
            $(tempTemplate).find('#fieldsetNavigation_0').attr('id', 'fieldsetNavigation_' + i);
            $(tempTemplate).find('#selectNavigationType_0').attr('id', 'selectNavigationType_' + i).attr('data-field', 'NavigationActions[' + i + '].NavigationType').attr('onchange', 'javascript: BITNAVIGATIONCONFIGTAB.changeNavigationType(this, ' + i + ');').val(this.NavigationType);
            var PageId = (this.NavigationPage != null) ? this.NavigationPage.ID : null;

            $(tempTemplate).find('#SelectNavigationPage_0').attr('id', 'SelectNavigationPage_' + i).attr('data-field', 'NavigationActions[' + i + '].NavigationPage.ID').val(PageId);


            $(tempTemplate).find('#checkboxListModules_0').attr('id', 'checkboxListModules_' + i).attr('data-field', 'NavigationActions[' + i + '].RefreshModules');
            $(tempTemplate).find('#checkboxListModules_' + i + ' input').each(function () {
                //console.log($.inArray($(this).val(), NavigationAction.RefreshModules));
                if ($.inArray($(this).val(), NavigationAction.RefreshModules) != -1) {
                    $(this).attr('checked', 'checked');
                }
            });

            $(tempTemplate).find('#javascriptModules_0').attr('id', 'javascriptModules_' + i).val(this.JsFunction);


            $(tempTemplate).find('[id^=divNavigationType]').each(function () {
                var id = $(this).attr('id');
                id = id.substring(0, id.length - 1);
                $(this).attr('id', id + i);
            });
            var test = $("#divNavigationActions").html();
            var test2 = $(tempTemplate).html();
            $("#divNavigationActions").append(tempTemplate);
            test = $("#divNavigationActions").html();
            BITNAVIGATIONCONFIGTAB.changeNavigationType($('#selectNavigationType_' + i)[0], i);
        });
    },

    changeNavigationType: function (selectElm, navigationActionIndex) {
        var fieldset = $(selectElm).parent().parent();
        $(fieldset).find('[id^=divNavigationType]').hide();
        $(fieldset).find('#divNavigationType' + $(selectElm).val() + '_' + navigationActionIndex).show();
    },

    changeNavigationTypeDiv: function (select, index) {
        var navigationType;
        if (select) {
            navigationType = $(select).val();
            BITNAVIGATIONCONFIGTAB.showNavigationTypeDiv(navigationType, index);
        }
    },

    idfieldsShown: false,
    showModuleIdFields: function () {
        
        if (BITNAVIGATIONCONFIGTAB.idfieldsShown) {
            $('.bitModuleIdField').hide();
            BITNAVIGATIONCONFIGTAB.idfieldsShown = false;
        }
        else {
            $('.bitModuleIdField').show();
            BITNAVIGATIONCONFIGTAB.idfieldsShown = true;
        }
    }
};


//registeer dit object bij BITEDITPAGE, zodat die de init en databind functie kan aanroepen
//BITEDITPAGE.registerModuleConfigJsClass(BITNAVIGATIONCONFIGTAB, "BITNAVIGATIONCONFIGTAB");

