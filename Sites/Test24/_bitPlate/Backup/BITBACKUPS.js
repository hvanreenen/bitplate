///// <reference path="../_js/jquery-1.7.2-vsdoc.js" />
///// <reference path="../../_js/prototypes/databind.js" />
///// <reference path="../prototypes/initDialog.js" />


/// <reference path="../_js/jquery-1.7.2-vsdoc.js" />
/// <reference path="../../_js/prototypes/databind.js" />
/// <reference path="../prototypes/initDialog.js" />



var BITBACKUP = {
    backupCreationStatusInterval: null,

    initialize: function () {
        $('#CreateBackupDialog').initDialog(null, { width: 400, height: 100 }, true, {});
        $('#RestoreBackupDialog').initDialog(null, { width: 400, height: 100 }, true, {});
    },

    createBackup: function () {
        $('#liCreateBackup a').attr({ 'href': '', 'onclick': '' });
        BITAJAX.dataServiceUrl = "/_bitplate/Backup/BackupService.asmx";
        
        $('#CreateBackupDialog').dialog('open');
        BITAJAX.callWebServiceASync("CreateBackup", null, function (data) {
           BITBACKUP.backupCreationStatusInterval = setInterval(BITBACKUP.getBackupStatus, 2000);
        });
    },

    getAllBackups: function () {
        BITAJAX.dataServiceUrl = "/_bitplate/Backup/BackupService.asmx";
        BITAJAX.callWebServiceASync("GetAllBackups", null, function (data) {
            $('#tableBackupFiles').dataBindList(data.d);
        });
    },

    getBackupStatus: function () {
        BITAJAX.dataServiceUrl = "/_bitplate/Backup/BackupService.asmx";
        BITAJAX.callWebServiceASync("GetBackupStatus", null, function (data) {
            $('#CreateBackupDialog').html(data.d.StatusMessage);
            if (data.d.BackupCompleted) {
                clearInterval(BITBACKUP.backupCreationStatusInterval);
                BITBACKUP.getAllBackups();
                setTimeout(function () { $('#CreateBackupDialog').dialog('close'); }, 2000);
            }
        });
    },

    deleteBackup: function (backup) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u deze backup verwijderen?", null, function (e) {
            BITAJAX.dataServiceUrl = "/_bitplate/Backup/BackupService.asmx";
            var parameterObject = { backupName: backup };
            var jsonstring = JSON.stringify(parameterObject);
            BITAJAX.callWebServiceASync("DeleteBackup", jsonstring, function (data) {
                BITBACKUP.getAllBackups();
            });
        });
    },

    restoreBackup: function (backup) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.YESNO, "Wilt u deze backup terug zetten? <br />LETOP!! Alle gegevens die op de huidige website staan zullen worden verwijderd!.", null, function (e) {
            BITAJAX.dataServiceUrl = "/_bitplate/Backup/BackupService.asmx";
            var parameterObject = { backupName: backup };
            var jsonstring = JSON.stringify(parameterObject);
            BITAJAX.callWebServiceASync("RestoreBackup", jsonstring, function (data) {
                document.location = data.d;
            });
        });
    },

    getRestoreStatus: function () {
        BITAJAX.dataServiceUrl = "BackupService.asmx";
        BITAJAX.callWebServiceASync("GetBackupStatus", null, function (data) {
            $('#RestoreBackupDialog').html(data.d.StatusMessage);
            if (data.d.BackupCompleted) {
                clearInterval(BITBACKUP.backupCreationStatusInterval);
                setTimeout(function () { $('#RestoreBackupDialog').dialog('close'); document.location = '/'; }, 2000);
            }
        });
    },

};