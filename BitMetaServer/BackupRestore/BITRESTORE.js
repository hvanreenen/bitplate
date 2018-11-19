
var BITRESTORE = {

    restoreInterval: null,

    initialize: function () {
        BITRESTORE.restoreInterval = setInterval(BITRESTORE.getRestoreStatus, 2000);
    },

    getRestoreStatus: function () {
        BITAJAX.dataServiceUrl = "RestoreService.asmx";
        var parameterobject = { siteId: getParameterByName('siteId') };
        var jsonstring = JSON.stringify(parameterobject);
        BITAJAX.callWebServiceASync("GetBackupStatus", jsonstring, function (data) {
            $('#restoreStatus').html(data.d.StatusMessage);
            if (data.d.BackupCompleted) {
                clearInterval(BITRESTORE.restoreInterval);
                setTimeout(function () { $('#RestoreBackupDialog').dialog('close'); document.location = getParameterByName('returnUrl') + '?ReloadScripts'; }, 2000);
            }
        });
    }
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}