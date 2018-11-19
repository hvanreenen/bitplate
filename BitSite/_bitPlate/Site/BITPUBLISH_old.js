var BITPUBLISH = {
    currentSort: "Type, Name ASC",

    loadUnpublishedItems: function (sort) {
        if (sort) BITPUBLISH.currentSort = sort;
        BITAJAX.dataServiceUrl = "SiteService.aspx";
        var parametersObject = { sort: BITPUBLISH.currentSort, page: 1, pagesize: 1000 };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUnpublishedItems", jsonstring, function (data) {
            $('#tableUnpublishedItems').dataBindList(data.d, {
                onSort: function (sort) {
                    BITPUBLISH.loadUnpublishedItems(sort);
                }
            });
        });
        $('#bitDivSearch').hide();
    },

    loadUnpublishedItemsPerType: function (type, sort) {
        if (sort) BITPUBLISH.currentSort = sort;
        BITAJAX.dataServiceUrl = "SiteService.aspx";
        var parametersObject = { type: type, sort: BITPUBLISH.currentSort, page: 1, pagesize: 1000 };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUnpublishedItemsPerType", jsonstring, function (data) {
            $('#tablePublish' + type + "s").dataBindList(data.d, {
                onSort: function (sort) {
                    BITPUBLISH.loadUnpublishedItems("Script", sort);
                    BITPUBLISH.loadUnpublishedItems("Template", sort);
                    BITPUBLISH.loadUnpublishedItems("Page", sort);
                    BITPUBLISH.loadUnpublishedItems("DataItem", sort);
                }
            });
        });
    },

    selectAll: function (checkbox, type) {
        var checked = $(checkbox).attr("checked");

        if (type == 'CompleteSite') {
            if (checked) {
                $("#checkCleanUp").removeAttr("disabled");
                $("#checkReGenerateSearchIndex").removeAttr("disabled");
                $("#checkAll").attr("disabled", "disabled");
                $("#checkPublishFilesAndImages").attr("disabled", "disabled");
                $("#tableUnpublishedItems").attr("disabled", "disabled");
                $('#bitSpanHidden').show();
            }
            else {
                $("#checkCleanUp").attr("disabled", "disabled");
                $("#checkCleanUp").removeAttr("checked");
                $("#checkAll").removeAttr("disabled");
                $("#checkPublishFilesAndImages").removeAttr("disabled");
                $("#checkReGenerateSearchIndex").removeAttr("disabled");
                $("#tableUnpublishedItems").removeAttr("disabled");
                $('#bitSpanHidden').hide();
            }
        }
        else if (type == 'All') {
            $(".checkItem").each(function (i) {
                if (!checked) {
                    $(this).removeAttr("checked");
                }
                else {
                    $(this).attr("checked", checked);
                }
            });
        }
        else {
            $(".check" + type).each(function (i) {
                if (!checked) {
                    $(this).removeAttr("checked");
                }
                else {
                    $(this).attr("checked", checked);
                }
            });
        }
    },

    publish: function () {
        
        var environmentID = $('input[name=environment]:radio:checked').val();

        var publishFilesAndImages = $("#checkPublishFilesAndImages").is(":checked");
        var reGenerateSearchIndex = $("#checkReGenerateSearchIndex").is(":checked");
        var cleanUp = $("#checkCleanUp").is(":checked");
        var publishBin = $("#checkPublishBin").is(":checked");
        var parametersObject = { environmentID: environmentID, publishFilesAndImages: publishFilesAndImages, reGenerateSearchIndex: reGenerateSearchIndex, cleanUp: cleanUp, publishBin: publishBin };

        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "SiteService.aspx";
        BITAJAX.callWebService("PublishSite", jsonstring, function (data) {
            var msg = "Publiceren gereed!";
            if (data.d == false) {
                msg = "Er zijn fouten op getreden tijdens het publiceren. Sommige onderdelen zijn niet gepubliceerd.";
            }
            $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, msg);
        });
    },

    publish_old: function () {
        BITAJAX.dataServiceUrl = "SiteService.aspx";
        var selectedItems = [];
        $('.checkItem').each(function (i) {
            if ($(this).is(":checked")) {
                selectedItems.push($(this).attr('title'));
            }
        });
        //$('.checkScript').each(function (i) {
        //    if ($(this).is(":checked")) {
        //        selectedItems.push($(this).attr('title'));
        //    }
        //});
        //$('.checkTemplate').each(function (i) {
        //    if ($(this).is(":checked")) {
        //        selectedItems.push($(this).attr('title'));
        //    }
        //});
        //$('.checkPage').each(function (i) {
        //    if ($(this).is(":checked")) {
        //        selectedItems.push($(this).attr('title'));
        //    }
        //});
        var publishToTestEnvironment = $("#radioTestEnvironment").is(":checked");
        var publishCompleteSite = $("#checkPublishComplete").is(":checked");
        var cleanUp = $("#checkCleanUp").is(":checked");
        var publishFilesAndImages = $("#checkPublishFilesAndImages").is(":checked");
        var reGenerateSearchIndex = $("#checkReGenerateSearchIndex").is(":checked");
        var parametersObject = { selectedItems: selectedItems, publishToTestEnvironment: publishToTestEnvironment, publishCompleteSite: publishCompleteSite, publishFilesAndImages: publishFilesAndImages, reGenerateSearchIndex: reGenerateSearchIndex, cleanUp: cleanUp };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("PublishSite", jsonstring, function (data) {
            BITPUBLISH.loadUnpublishedItems();
            //BITPUBLISH.loadUnpublishedItems("Template");
            //BITPUBLISH.loadUnpublishedItems("Page");
            //BITPUBLISH.loadUnpublishedItems("DataCollection");
            var msg = "Publiceren gereed!";
            if (data.d == false) {
                msg = "Er zijn fouten op getreden tijdens het publiceren. Sommige onderdelen zijn niet gepubliceerd.";
            }
            $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, msg);
        });
    },

    openUnpublishedChangesPopup: function (elm) {
        var changes = $(elm).next('div').html();
        changes = changes.replaceAll(";", "<br/>");

        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, changes, "Wijzigingen");

    },

    changeEnvironment: function (currentEnvironmentID) {
        var environmentID = $('input[name=environment]:radio:checked').val();
        if (environmentID == currentEnvironmentID) {
            $("#divExternalEnviroment").hide();
        }
        else {
            $("#divExternalEnviroment").show();
        }
    },

    changeLiveTestEnvironment: function () {
        var testEnvironmentMode = $("#radioTestEnvironment").is(":checked");
        if (testEnvironmentMode) {
            $("#publishLiveFolder").hide();
            $("#publishTestFolder").show();
            $("#publishTestModeImmediate").show();
            
        }
        else {
            $("#publishLiveFolder").show();
            $("#publishTestFolder").hide();
            $("#publishTestModeImmediate").hide();
        }

    }
};