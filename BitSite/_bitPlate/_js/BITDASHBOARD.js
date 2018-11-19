$(document).ready(function () {
    BITDASHBOARD.initialize();
});

var BITDASHBOARD = {
    newsVisable: false,
    newsItems: null,
    initialize: function () {
        var noPermissionDialog = $('#noPermissionsDialog.show');
        //if (noPermissionDialog) {
        //    $(noPermissionDialog).dialog({
        //        modal: true,
        //        width: 625,
        //        height: 200,
        //        resizable: false,
        //        buttons: {
        //            Ok: function () {
        //                $(this).dialog("close");
        //            }
        //        }
        //    });
        //}
        $('#newsItemDetailsPopup').initDialog();
    },

    toggleNewsItems: function () {
        if (BITDASHBOARD.newsVisable) {
            BITDASHBOARD.hideNewsItems();
            BITDASHBOARD.newsVisable = false;
        }
        else {
            BITDASHBOARD.showNewsItems();
            BITDASHBOARD.newsVisable = true;
        }
    },

    showNewsItems: function () {
        $('.theBitMenu').animate({ left: 262 }, 'fast');

        BITAJAX.dataServiceUrl = "/_bitplate/Autorisation/AuthService.asmx";
        BITAJAX.callWebServiceASync("GetNewsItems", null, function (data) {
            $('#newsItemList').dataBindList(data.d);
            BITDASHBOARD.newsItems = data.d;
        });
    },

    hideNewsItems: function () {
        $('.theBitMenu').animate({ left: -39 }, 'fast');
    },

    openNewsItem: function (index) {
        var newsItem = BITDASHBOARD.newsItems[index];
        $('#newsItemDetailsPopup').dataBind(newsItem);
        $('#newsItemDetailsPopup').dialog('open');
    }
}