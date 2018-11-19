$(document).ready(function () {
    BITDASHBOARD.initialize();
});

var BITDASHBOARD = {

    initialize: function () {
        var noPermissionDialog = $('#noPermissionsDialog.show');
        if (noPermissionDialog) {
            $(noPermissionDialog).dialog({
                modal: true,
                width: 625,
                height: 200,
                resizable: false,
                buttons: {
                    Ok: function () {
                        $(this).dialog("close");
                    }
                }
            });
        }
    }
}