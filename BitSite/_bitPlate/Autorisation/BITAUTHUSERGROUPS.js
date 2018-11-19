
//voor usergroups

var USERTYPES = { NONSUPERUSER: 0, SITEADMINS: 9, DEVELOPERS: 99};
var BITAUTHUSERGROUPS = {
    
    currentSortUserGroups: "Name ASC",
    searchString: "",
    userGroups: [], //bewaar de usergroups in variabele, want we hebben de rechten ervan nodig
    userGroup: null,
    defaultPermissionSets: [],
    
    currentPageUserGroups: 1,
    pageSize: 25,

    initialize: function () {
        //jQuery.support.cors = true; //
        BITAJAX.dataServiceUrl = "AuthService.asmx";
       
        $('#bitUserGroupDetailsDialog').initDialog(BITAUTHUSERGROUPS.saveUserGroup);

        $('#bitPopups').formEnrich();

        $('#tabPageDetails4').dockable();
        $('#tabPageDetailsUserGroup2').dockable();

        $('#bitSearchTextbox').searchable(BITAUTHUSERGROUPS.search);
        this.attachCheckBoxClickEvents();
    },

    attachCheckBoxClickEvents: function () {
        $(".checkParent").click(function (event) {
            event.stopPropagation(); //om te voorkomen dat dockpanel weer dichtklapt
            var value = $(this).val();
            var checked = $(this).is(":checked");
            if (checked) {
                $("#bitPermissionsPanelGroup" + value).removeAttr("disabled");
            }
            else {
                $("#bitPermissionsPanelGroup" + value).attr("disabled", "disabled");
                $("#bitPermissionsPanelGroup" + value).find("input[type=checkbox]").removeAttr("checked");
            }
        });

        $(".checkAll").click(function () {
            var value = $(this).val();
            var checked = $(this).is(":checked");
            if (checked) {
                $("#bitPermissionsPanelGroup" + value).find("input[type=checkbox]").attr("checked", "checked");
                $("#bitPermissionsPanelGroup" + value).find("div").removeAttr("disabled");

            }
            else {
                $("#bitPermissionsPanelGroup" + value).find("div").attr("disabled", "disabled");
                $("#bitPermissionsPanelGroup" + value).find("input[type=checkbox]").removeAttr("checked");
            }
        });
    },

    filterData: function () {
        
        this.loadUserGroups();
    },

    search: function (searchText) {
        BITAUTHUSERGROUPS.searchString = searchText;
        
            BITAUTHUSERGROUPS.loadUserGroups();
           
        
    },
   

    //////////////////////////////////////////////
    // USER GROUPS
    //////////////////////////////////////////////
    loadUserGroups: function (sort, pagenumber) {
        if (sort != undefined) BITAUTHUSERGROUPS.currentSortUserGroups = sort;
        if (pagenumber != undefined) BITAUTHUSERGROUPS.currentPageUserGroups = pagenumber;
        var onlyCurrentSite = ($("input[type=radio][name=radioGroupSelect]:checked").val() == "OnlyCurrentSite");

        var parametersObject = {
            sort: BITAUTHUSERGROUPS.currentSortUserGroups,
            onlyCurrentSite: onlyCurrentSite,
            pageNumber: BITAUTHUSERGROUPS.currentPageUserGroups,
            pageSize: BITAUTHUSERGROUPS.pageSize,
            searchString: BITAUTHUSERGROUPS.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUserGroups", jsonstring, function (data) {
            BITAUTHUSERGROUPS.userGroups = data.d;
            $("#tableUserGroups").dataBindList(data.d, {
                onSort: function (sort) {
                    BITAUTHUSERGROUPS.loadUserGroups(sort);
                },
                onRowBound: function (obj, index, html) {
                    //maak wrapper div, zet hierin de html van de tr
                    //doe bewerkingen in deze html
                    //zet weer terug in de tr (onderaan)
                    var tempWrapperDiv = document.createElement('div');
                    $(tempWrapperDiv).html(html);
                    if (obj.IsSystemValue) {
                        //verberg delete knop
                        $(tempWrapperDiv).find('.bitDeleteButton').hide();
                        $(tempWrapperDiv).find('.bitCopyButton').hide();
                    }
                    html = $(tempWrapperDiv).html();
                    return html;
                }

            });
            if (BITAUTHUSERGROUPS.searchString == "") {
                $("#bitUserGroupList").dataBindList(data.d); //in kies groep-dialog
            }
        });
    },


    newUserGroup: function () {
        BITAUTHUSERGROUPS.openUserGroupDetailsPopup(null);
    },

    openUserGroupDetailsPopup: function (userGroupId) {
        var parametersObject = { id: userGroupId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUserGroup", jsonstring, function (data) {
            BITAUTHUSERGROUPS.userGroup = data.d;
            $('#bitUserGroupDetailsDialog').dataBind(data.d);
            if (BITAUTHUSERGROUPS.userGroup.IsSystemValue) {
                $("#divSystemValue").show();
                $('#bitUserGroupDetailsDialog').find("input").attr("disabled", "disabled");
                $(".ui-dialog-buttonpane button:contains('Opslaan')").button("disable");
            }
            else {
                $("#divSystemValue").hide();
                $('#bitUserGroupDetailsDialog').find("input").removeAttr("disabled");
                $(".ui-dialog-buttonpane button:contains('Opslaan')").button("enable");
            }
            //if (data.d.SuperUserGroupType == USERTYPES.DEVELOPERS ||
            //    data.d.SuperUserGroupType == USERTYPES.SITEADMINS)
            //{
            //    //$("#divUserGroupSite").hide();
            //    $("#divSystemValue").show();
            //    //$("#divUserGroupCompany").hide();
            //    //$("#divUserGroupSite").find("select").removeAttr("data-validation");
            //    //$("#tabPageDetailsUserGroup2").attr("disabled", "disabled");
            //}
            ////else if (data.d.SuperUserGroupType == USERTYPES.RESELLERADMINS ||
            ////    data.d.SuperUserGroupType == USERTYPES.COMPANYADMINS) {
            ////    $("#divUserGroupSite").hide();
            ////    $("#divUserGroupSuperType").show();
            ////    $("#divUserGroupCompany").show();

            ////    $("#divUserGroupSite").find("select").removeAttr("data-validation");
            ////    $("#tabPageDetailsUserGroup2").attr("disabled", "disabled");
            ////}
            //else {
            //    //$("#divUserGroupSite").show();
            //    $("#divSystemValue").hide();
            //    //$("#divUserGroupCompany").hide();
            //    //$("#divUserGroupSite").find("select").attr("data-validation", "required");
            //    //$("#tabPageDetailsUserGroup2").removeAttr("disabled");
            //}
            var permissions = BITAUTHUSERGROUPS.userGroup.Permissions;
            //eerst alle uitzetten
            $('#bitUserGroupDetailsDialog').find("input[type=checkbox]").removeAttr("checked");
            $('.bitPermissionsPanelLevel1').attr("disabled", "disabled");
            //dan per permission aanzetten
            for (var i in permissions) {
                var funcNumber = permissions[i];
                $("input[type=checkbox][value=" + funcNumber + "]").attr("checked", "checked");
                $("#bitPermissionsPanelGroup" + funcNumber).removeAttr("disabled");
            }
        });
        $('#bitUserGroupDetailsDialog').dialog('open');

    },

    setDefaultPermissions: function () {
        var defaultPermissionSetName = $('#selectDefaultPermissions option:selected').val();
        //eerst alle uitzetten
        $('#bitUserGroupDetailsDialog').find("input[type=checkbox]").removeAttr("checked");

        var defaultPermissions = BITAUTHUSERGROUPS.defaultPermissionSets[defaultPermissionSetName];
        //eerst alle uitzetten
        $('#bitUserGroupDetailsDialog').find("input[type=checkbox]").removeAttr("checked");
        //dan per permission aanzetten
        for (var i in defaultPermissions) {
            var funcNumber = defaultPermissions[i];
            $("input[type=checkbox][value=" + funcNumber + "]").attr("checked", "checked");
            $("#bitPermissionsPanelGroup" + funcNumber).removeAttr("disabled");
        }
    },

    saveUserGroup: function () {
        var validation = $('#bitUserGroupDetailsDialog').validate();
        if (validation) {
            //get data from panel
            BITAUTHUSERGROUPS.userGroup = $('#bitUserGroupDetailsDialog').collectData(BITAUTHUSERGROUPS.userGroup);

            var permissions = [];
            $('#bitUserGroupDetailsDialog').find('input[type=checkbox]').not('.checkAll').each(function (i) {
                var checked = $(this).is(":checked");
                if (checked) {
                    funcNumber = $(this).val();
                    permissions.push(funcNumber);
                }
            });
            BITAUTHUSERGROUPS.userGroup.Permissions = permissions;

            jsonstring = convertToJsonString(BITAUTHUSERGROUPS.userGroup);
            BITAJAX.callWebService("SaveUserGroup", jsonstring, function (data) {
                $('#bitUserGroupDetailsDialog').dialog("close");
                BITAUTHUSERGROUPS.loadUserGroups();
                
            });
        }
    },

    removeUserGroup: function (id) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u deze gebruikersgroep verwijderen?", null, function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteUserGroup", jsonstring, function (data) {
                BITAUTHUSERGROUPS.loadUserGroups();
            });
        });
    }
}

