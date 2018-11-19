
//voor users en usergroups

var BITAUTHSITEUSERS = {
    currentSortUsers: "Name ASC",
    currentSortUserGroups: "Name ASC",
    searchString: "",
    type: "Users",
    userGroups: [], //bewaar de usergroups in variabele, want we hebben de rechten ervan nodig
    user: null,
    userGroup: null,
    defaultPermissionSets : [],
    currentPageUsers: 1,
    currentPageUserGroups: 1,
    pageSize: 25,

    initialize: function () {
        //jQuery.support.cors = true; //
        BITAJAX.dataServiceUrl = "SiteAuthService.asmx";
        $('#bitUserDetailsDialog').initDialog(BITAUTHSITEUSERS.saveUser);
        $('#bitUserGroupDetailsDialog').initDialog(BITAUTHSITEUSERS.saveUserGroup);

        $('#bitUserGroupPerUserDialog').initDialog(null, { width: 200, height: 400 }, false, {
            "Sluiten": function () {
                $(this).dialog('close');
            }
        });

        $('#bitPopups').formEnrich();


        $('#bitSearchTextbox').searchable(BITAUTHSITEUSERS.search);
    },

    

    filterData: function () {
        this.loadUsers();
    },

    search: function (searchText) {
        BITAUTHSITEUSERS.searchString = searchText;
        if (BITAUTHSITEUSERS.type == "Users") {
            BITAUTHSITEUSERS.loadUsers();
            BITAUTHSITEUSERS.showUsers();
        }
        else {
            BITAUTHSITEUSERS.loadUserGroups();
            BITAUTHSITEUSERS.showUserGroups();
        }
    },
    //////////////////////////////////////////////
    // USERS
    //////////////////////////////////////////////
    loadUsers: function (sort, pagenumber) {
        if (sort != undefined) BITAUTHSITEUSERS.currentSortUsers = sort;
        if (pagenumber != undefined) BITAUTHSITEUSERS.currentPageUsers = pagenumber;

        var parametersObject = {
            sort: BITAUTHSITEUSERS.currentSortUsers,
            pageNumber: BITAUTHSITEUSERS.currentPageUsers,
            pageSize: BITAUTHSITEUSERS.pageSize,
            searchString: BITAUTHSITEUSERS.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUsers", jsonstring, function (data) {
            $("#tableUsers").dataBindList(data.d, {
                onSort: function (sort) {
                    BITAUTHSITEUSERS.loadUsers(sort);
                }
            });
            
        });
    },

    showUsers: function () {
        this.type = "Users";
        $("#tableUserGroups").hide();
        $("#tableUsers").show();
        $("#bitplateTitlePage").html("Gebruikers");
    },

    newUser: function () {
        BITAUTHSITEUSERS.openUserDetailsPopup(null);
    },

    openUserDetailsPopup: function (id) {

        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUser", jsonstring, function (data) {
            BITAUTHSITEUSERS.user = data.d;
            $('#bitUserDetailsDialog').dataBind(data.d);
            $('#tableUserGroupsPerUser').dataBindList(data.d.UserGroups);

            //BITAUTHSITEUSERS.bindUserPermissions();

        });
        $('#bitUserDetailsDialog').dialog('open');
    },

    

    openUserGroupsPopup: function () {
        $('#bitUserGroupPerUserDialog').dialog('open');
    },

    addUserGroupPerUser: function (id, name, site) {
        var userGroup = this.findUserGroup(id);

        if (BITAUTHSITEUSERS.user.UserGroups == null) {
            BITAUTHSITEUSERS.user.UserGroups = [];
        }
        //kijk of die er al in zit
        var contains = false;
        for (var i in BITAUTHSITEUSERS.user.UserGroups) {
            var groupCheck = BITAUTHSITEUSERS.user.UserGroups[i];
            if (groupCheck.ID == userGroup.ID) {
                contains = true;
                break;
            }
        }
        if (contains) {
            $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "Koppeling met deze group bestaat al.");
        }
        else {
            BITAUTHSITEUSERS.user.UserGroups.push(userGroup);
            $("#tableUserGroupsPerUser").dataBindList(BITAUTHSITEUSERS.user.UserGroups);
            //BITAUTHSITEUSERS.bindUserPermissions();
        }
    },

    findUserGroup: function (id) {
        var returnValue = null;
        for (var i in BITAUTHSITEUSERS.userGroups) {
            if (BITAUTHSITEUSERS.userGroups[i].ID == id) {
                returnValue = BITAUTHSITEUSERS.userGroups[i];
                break;
            }
        }
        return returnValue;
    },

    removeUserGroupPerUser: function (id, index) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u deze gebruikersgroep wilt los koppelen van deze gebruiker?", null, function () {
            BITAUTHSITEUSERS.user.UserGroups.splice(index, 1);
            $("#tableUserGroupsPerUser").dataBindList(BITAUTHSITEUSERS.user.UserGroups);
            //BITAUTHSITEUSERS.bindUserPermissions();
        });
    },

    saveUser: function () {
        var validation = $('#bitUserDetailsDialog').validate();
        if (validation) {
            if (!BITAUTHSITEUSERS.user.UserGroups ||
                (BITAUTHSITEUSERS.user.UserGroups && BITAUTHSITEUSERS.user.UserGroups.length == 0)) {
                validation = false;
                $.messageBox(MSGBOXTYPES.ERROR, "Kan niet opslaan, want tenminste 1 groep is verplicht");
            }
        }
        if (validation) {
            //get data from panel
            BITAUTHSITEUSERS.user = $('#bitUserDetailsDialog').collectData(BITAUTHSITEUSERS.user);
            
            //var permissions = [];
            //$('#bitUserDetailsDialog').find('input[type=checkbox]').each(function (i) {
            //    var checked = $(this).is(":checked");
            //    var disabled = $(this).is(":disabled");
            //    if (checked && !disabled) {
            //        var functionality = new Object();
            //        functionality.ID = $(this).val();
            //        permissions.push(functionality);
            //    }
            //});
            //BITAUTHSITEUSERS.user.UserPermissions = permissions;

            jsonstring = convertToJsonString(BITAUTHSITEUSERS.user);
            BITAJAX.callWebService("SaveUser", jsonstring, function (data) {
                $('#bitUserDetailsDialog').dialog("close");
                BITAUTHSITEUSERS.loadUsers();
                BITAUTHSITEUSERS.showUsers();
            });
        }
    },

    removeUser: function (id) {
        $.deleteConfirmBox("Wilt u deze gebruiker verwijderen?", 'Gebruiker verwijderen?', function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteUser", jsonstring, function (data) {
                BITAUTHSITEUSERS.loadUsers();
            });
        });
    },

    sendNewPassword: function () {
        var email = BITAUTHSITEUSERS.user.Email;
        var parametersObject = { email: email };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("SendNewPassword", jsonstring, function (data) {
            $.messageBox(MSGBOXTYPES.ERROR, "Er is een nieuw wachtwoord aan deze gebruiker (" + email + ") gestuurd.");
        });
    },
    //////////////////////////////////////////////
    // USER GROUPS
    //////////////////////////////////////////////
    loadUserGroups: function (sort, pagenumber) {
        if (sort != undefined) BITAUTHSITEUSERS.currentSortUserGroups = sort;
        if (pagenumber != undefined) BITAUTHSITEUSERS.currentPageUserGroups = pagenumber;
        var onlyCurrentSite = ($("input[type=radio][name=radioGroupSelect]:checked").val() == "OnlyCurrentSite");

        var parametersObject = {
            sort: BITAUTHSITEUSERS.currentSortUserGroups,
            onlyCurrentSite: onlyCurrentSite,
            pageNumber: BITAUTHSITEUSERS.currentPageUserGroups,
            pageSize: BITAUTHSITEUSERS.pageSize,
            searchString: BITAUTHSITEUSERS.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUserGroups", jsonstring, function (data) {
            BITAUTHSITEUSERS.userGroups = data.d;
            $("#tableUserGroups").dataBindList(data.d, {
                onSort: function (sort) {
                    BITAUTHSITEUSERS.loadUserGroups(sort);
                }
            });
            if (BITAUTHSITEUSERS.searchString == "") {
                $("#bitUserGroupList").dataBindList(data.d); //in kies groep-dialog
            }
        });
    },

    showUserGroups: function () {
        this.type = "UserGroups";
        $("#tableUserGroups").show();
        $("#tableUsers").hide();

        $("#bitplateTitlePage").html("Gebruikersgroepen");
    },

    newUserGroup: function () {
        BITAUTHSITEUSERS.openUserGroupDetailsPopup(null);
    },

    openUserGroupDetailsPopup: function (userGroupId) {
        var parametersObject = { id: userGroupId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUserGroup", jsonstring, function (data) {
            BITAUTHSITEUSERS.userGroup = data.d;
            $('#bitUserGroupDetailsDialog').dataBind(data.d);
            //var permissions = BITAUTHSITEUSERS.userGroup.UserGroupPermissions;
            ////eerst alle uitzetten
            //$('#bitUserGroupDetailsDialog').find("input[type=checkbox]").removeAttr("checked");
            ////dan per permission aanzetten
            //for (var i in permissions) {
            //    var funcID = permissions[i].ID;
            //    $("input[type=checkbox][value=" + funcID + "]").attr("checked", "checked");
            //    $("#bitPermissionsPanelGroup" + funcID).removeAttr("disabled");
            //}
        });
        $('#bitUserGroupDetailsDialog').dialog('open');

    },

    //setDefaultPermissions: function(){
    //    var defaultPermissionSetName = $('#selectDefaultPermissions option:selected').val();
    //    //eerst alle uitzetten
    //    $('#bitUserGroupDetailsDialog').find("input[type=checkbox]").removeAttr("checked");
        
    //    var defaultPermissions = BITAUTHSITEUSERS.defaultPermissionSets[defaultPermissionSetName];
    //    //eerst alle uitzetten
    //    $('#bitUserGroupDetailsDialog').find("input[type=checkbox]").removeAttr("checked");
    //    //dan per permission aanzetten
    //    for (var i in defaultPermissions) {
    //        var funcID = defaultPermissions[i].ID;
    //        $("input[type=checkbox][value=" + funcID + "]").attr("checked", "checked");
    //        $("#bitPermissionsPanelGroup" + funcID).removeAttr("disabled");
    //    }
    //},

    saveUserGroup: function () {
        var validation = $('#bitUserGroupDetailsDialog').validate();
        if (validation) {
            //get data from panel
            BITAUTHSITEUSERS.userGroup = $('#bitUserGroupDetailsDialog').collectData(BITAUTHSITEUSERS.userGroup);

            //var permissions = [];
            //$('#bitUserGroupDetailsDialog').find('input[type=checkbox]').each(function (i) {
            //    var checked = $(this).is(":checked");
            //    if (checked) {
            //        var functionality = new Object();
            //        functionality.ID = $(this).val();
            //        permissions.push(functionality);
            //    }
            //});
            //BITAUTHSITEUSERS.userGroup.UserGroupPermissions = permissions;

            jsonstring = convertToJsonString(BITAUTHSITEUSERS.userGroup);
            BITAJAX.callWebService("SaveUserGroup", jsonstring, function (data) {
                $('#bitUserGroupDetailsDialog').dialog("close");
                BITAUTHSITEUSERS.loadUserGroups();
                BITAUTHSITEUSERS.showUserGroups();
            });
        }
    },

    removeUserGroup: function (id) {
        $.deleteConfirmBox("Wilt u deze gebruikersgroep verwijderen?", 'Gebruikersgroep verwijderen?', function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteUserGroup", jsonstring, function (data) {
                BITAUTHSITEUSERS.loadUserGroups();
            });
        });
    }
}

