var BITNEWS = {
    currentNewsMessage: null,

    initialize: function () {
        $('#newsMessageEditPopup').initDialog(BITNEWS.saveNewsMessage);
        BITNEWS.getNewsMessages();
    },

    getNewsMessages: function () {
        BITAJAX.dataServiceUrl = "NewsService.asmx";
        BITAJAX.callWebServiceASync("GetNewsMessages", null, function (data) {
            $("#tableNewsMessages").dataBindList(data.d);
        });
    },

    newNewsMessage: function () {
        BITNEWS.openNewsMessageDetailsPopup(BITUTILS.EMPTYGUID);
    },

    openNewsMessageDetailsPopup: function (ID) {
        BITAJAX.dataServiceUrl = "NewsService.asmx";
        var parameters = { ID: ID };
        var jsonstring = JSON.stringify(parameters);
        BITAJAX.callWebServiceASync("GetNewsMessage", jsonstring, function (data) {
            BITNEWS.currentNewsMessage = data.d;
            $('#newsMessageEditPopup').dataBind(BITNEWS.currentNewsMessage);
            $('#newsMessageEditPopup').dialog('open');
        });
    },

    saveNewsMessage: function () {
        var isValid = $('#newsMessageEditPopup').validate();
        if (isValid) {
            BITNEWS.currentNewsMessage = $('#newsMessageEditPopup').collectData(BITNEWS.currentNewsMessage);
            var jsonstring = convertToJsonString(BITNEWS.currentNewsMessage);
            BITAJAX.callWebServiceASync("SaveNewsMessage", jsonstring, function (data) {
                $('#newsMessageEditPopup').dialog('close');
                BITNEWS.getNewsMessages();
            });
        }
    },

    removeNewsMessage: function (ID) {
        $.deleteConfirmBox('Weet u zeker dat u dit niews item wilt verwijderen?', 'Nieuws item verwijderen', function () {
            BITAJAX.dataServiceUrl = "NewsService.asmx";
            var parameters = { ID: ID };
            var jsonstring = JSON.stringify(parameters);
            BITAJAX.callWebServiceASync("DeleteNewsMessage", jsonstring, function (data) {
                BITNEWS.getNewsMessages();
            });
        });
    }
}