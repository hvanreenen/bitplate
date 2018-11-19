var BITUPDATES = {
    currentSort: "Name ASC",

    initialize: function () {
        BITAJAX.dataServiceUrl = "/Updates/UpdateService.aspx";

        $('#bitDivSearch').hide();

        this.loadUpdates();

    },

    loadUpdates: function () {
        BITAJAX.callWebService("GetVersions", null, function (data) {
            $("#selectVersionNumbers").fillDropDown(data.d);
        });
    },

    loadLicensedEnvironments: function (sort) {
        var versionNumber = $("#selectVersionNumbers").val();

        if (sort != undefined) BITUPDATES.currentSort = sort;

        var parametersObject = {
            versionNumber: versionNumber,
            sort: BITUPDATES.currentSort,
        };

        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.callWebService("GetLicensedEnvironmentsWithLesserVersion", jsonstring, function (data) {
            if (data.d.length == 0) {
                $("#noUpdatesMsg").show();
                $("#tableLicensedEnvironments").hide();
            }
            else {
                $("#tableLicensedEnvironments").dataBindList(data.d, {
                    onSort: function (sort) {
                        BITUPDATES.loadLicensedEnvironments(sort);
                    }
                });
                $("#checkAll").attr("checked", "checked");
                BITUPDATES.selectAll();
            }
        });
    },

    selectAll: function () {
        var checkAll = $("#checkAll").is(":checked");
        if (checkAll) {
            $(".checkLicensedEnvironment").attr("checked", "checked");
        }
        else {
            $(".checkLicensedEnvironment").removeAttr("checked");
        }
    },

    doUpdate: function () {
        var versionNumber = $("#selectVersionNumbers").val();
        var ids = [];
        $(".checkLicensedEnvironment").each(function () {
            if ($(this).is(":checked")) {
                ids.push($(this).parent().find('.siteId').val());
            }
        });

        var parametersObject = {
            LicensedEnvironmentsIDs: ids,
            versionNumber: versionNumber
        };

        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.callWebService("DoUpdates", jsonstring, function (data) {
            var logMsg = data.d;
            if (logMsg) {
                $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, logMsg);
            }
            BITUPDATES.loadLicensedEnvironments();
        });
    }

};