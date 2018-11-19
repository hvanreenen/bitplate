
ConfigActionsTab.Initialize();

var ConfigActionsTab = {

    Initialize: function () {
        $('#bitConfigModuleStandardDialog').tabs("add", "#standardTabPageNavigation", "Navigatie", 1);
        var template = $("#divNavigationActions").html();
        $("#divNavigationActions").html('')
        $(data.d.NavigationActions).each(function (i) {
            var tempTemplate = $('<div></div>').append(template);
            var NavigationAction = this;
            $(tempTemplate).find('#navigationTagName_0').attr('id', 'navigationTagName_' + i).html(this.Name);
            $(tempTemplate).find('#fieldsetNavigation_0').attr('id', 'fieldsetNavigation_' + i);
            $(tempTemplate).find('#selectNavigationType_0').attr('id', 'selectNavigationType_' + i).attr('data-field', 'NavigationActions[' + i + '].NavigationType').attr('onchange', 'javascript: BITEDITPAGE.changeNavigationType(this, ' + i + ');').val(this.NavigationType);
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
            $("#divNavigationActions").append(tempTemplate);
            BITEDITPAGE.changeNavigationType($('#selectNavigationType_' + i)[0], i);
        });
    }

}

