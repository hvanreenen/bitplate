$(document).ready(function () {
    BITEVENTLOG.initialize();
});

var BITEVENTLOG = {

    currentSort: null,
    eventLog: null,

    initialize: function () {
        $('#bitSearchTextboxEnterSearch').click(function () { BITEVENTLOG.loadEventLog(); return false; });
        $('#eventDetails').initDialog(null, { height: 300, width: 700 });
        BITEVENTLOG.loadEventLog();
    },

    loadEventLog: function (pager, orderby) {
        if (!pager) pager = 1;
        if (!orderby) orderby = 'Date DESC';
        var searchstring = $('#bitSearchTextbox').val();
        if (!searchstring) searchstring = '';

        BITEVENTLOG.currentSort = orderby;

        var parametersObject = { pager: pager, orderby: BITEVENTLOG.currentSort, searchstring: searchstring, recordLimit: 50 };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/eventlog/EventLogService.asmx";
        BITAJAX.callWebServiceASync("GetAllErrors", jsonstring, function (data) {
            $('#tableData').dataBindList(data.d);
            BITEVENTLOG.eventLog = data.d;
        });
    },

    moreDetails: function (index) {
        $('#eventDetails').dataBind(BITEVENTLOG.eventLog[index]);
        $('#eventDetails').dialog('open');
    }
}