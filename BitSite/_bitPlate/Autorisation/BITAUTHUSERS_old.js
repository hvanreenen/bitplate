
//voor users en usergroups

var SUPERUSERTYPES = { NONSUPERUSER: 0, SITEADMINS: 1, COMPANYADMINS: 2, RESELLERADMINS: 3, SERVERADMINS: 4, DEVELOPERS: 9};
var BITAUTHUSERS = {
    currentSortUsers: "Name ASC",
    currentSortUserGroups: "Name ASC",
    searchString: "",
    type: "Users",
    userGroups: [], //bewaar de usergroups in variabele, want we hebben de rechten ervan nodig
    user: null,
    userGroup: null,
    defaultPermissionSets: [],
    currentPageUsers: 1,
    currentPageUserGroups: 1,
    pageSize: 25,

    initialize: function () {
        //jQuery.support.cors = true; //
        BITAJAX.dataServiceUrl = "../bitAjaxServices/AuthService.aspx";
        $('#bitUserDetailsDialog').initDialog(BITAUTHUSERS.saveUser);
        $('#bitUserGroupDetailsDialog').initDialog(BITAUTHUSERS.saveUserGroup);

        $('#bitUserGroupPerUserDialog').initDialog(null, { width: 200, height: 400 }, false, {
            "Sluiten": function () {
                $(this).dialog('close');
            }
        });

        $('#bitPopups').formEnrich();

        $('#tabPageDetails4').dockable();
        $('#tabPageDetailsUserGroup2').dockable();

        $('#bitSearchTextbox').searchable(BITAUTHUSERS.search);
        this.attachCheckBoxClickEvents();
    },

    attachCheckBoxClickEvents: function () {
        $(".checkParent").click(function (event) {
            event.stopPropagation(); //om te voorkomen dat dockpanel weer dichtklapt
            var value = $(this).val();
            var checked = $(this).is(":checked");
            if (checked) {
                $("#bitPermissionsPanelUser" + value).removeAttr("disabled");
                $("#bitPermissionsPanelGroup" + value).removeAttr("disabled");
            }
            else {
                $("#bitPermissionsPanelUser" + value).attr("disabled", "disabled");
                $("#bitPermissionsPanelUser" + value).find("input[type=checkbox]").removeAttr("checked");
                $("#bitPermissionsPanelGroup" + value).attr("disabled", "disabled");
                $("#bitPermissionsPanelGroup" + value).find("input[type=checkbox]").removeAttr("checked");

            }
        });

        $(".checkAll").click(function () {
            var value = $(this).val();
            var checked = $(this).is(":checked");
            if (checked) {
                $("#bitPermissionsPanelUser" + value).find("input[type=checkbox]").attr("checked", "checked");
                $("#bitPermissionsPanelUser" + value).find("div").removeAttr("disabled");
                $("#bitPermissionsPanelGroup" + value).find("input[type=checkbox]").attr("checked", "checked");
                $("#bitPermissionsPanelGroup" + value).find("div").removeAttr("disabled");

            }
            else {
                $("#bitPermissionsPanelUser" + value).find("div").attr("disabled", "disabled");
                $("#bitPermissionsPanelUser" + value).find("input[type=checkbox]").removeAttr("checked");
                $("#bitPermissionsPanelGroup" + value).find("div").attr("disabled", "disabled");
                $("#bitPermissionsPanelGroup" + value).find("input[type=checkbox]").removeAttr("checked");

            }
        });
    },

    filterData: function () {
        this.loadUsers();
        this.loadUserGroups();
    },

    search: function (searchText) {
        BITAUTHUSERS.searchString = searchText;
        if (BITAUTHUSERS.type == "Users") {
            BITAUTHUSERS.loadUsers();
            BITAUTHUSERS.showUsers();
        }
        else {
            BITAUTHUSERS.loadUserGroups();
            BITAUTHUSERS.showUserGroups();
        }
    },
    //////////////////////////////////////////////
    // USERS
    //////////////////////////////////////////////
    loadUsers: function (sort, pagenumber) {
        if (sort != undefined) BITAUTHUSERS.currentSortUsers = sort;
        if (pagenumber != undefined) BITAUTHUSERS.currentPageUsers = pagenumber;
        var onlyCurrentSite = ($("input[type=radio][name=radioGroupSelect]:checked").val() == "OnlyCurrentSite");

        var parametersObject = {
            sort: BITAUTHUSERS.currentSortUsers,
            onlyCurrentSite: onlyCurrentSite,
            pageNumber: BITAUTHUSERS.currentPageUsers,
            pageSize: BITAUTHUSERS.pageSize,
            searchString: BITAUTHUSERS.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUsers", jsonstring, function (data) {
            $("#tableUsers").dataBindList(data.d, {
                onSort: function (sort) {
                    BITAUTHUSERS.loadUsers(sort);
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
        BITAUTHUSERS.openUserDetailsPopup(null);
    },

    openUserDetailsPopup: function (id) {

        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUser", jsonstring, function (data) {
            BITAUTHUSERS.user = data.d;
            $('#bitUserDetailsDialog').dataBind(data.d);
            $('#tableUserGroupsPerUser').dataBindList(data.d.UserGroups);

            BITAUTHUSERS.bindUserPermissions();

        });
        $('#bitUserDetailsDialog').dialog('open');
    },

    bindUserPermissions: function () {
        //Eerst per usergroep (waar gebruiker inzit) kijken naar de rechten. Daarna aanvullen mer userrechten
        //Zit een recht in de usergroep? Dan staat het recht bij de user aan en wordt die disabled
        //achter de tekst kom te staan: " (rechten overgenomen van groep:")
        //eerst alle uitzetten
        $('#tabPageDetails4').find("input[type=checkbox]").each(function (i) {
            $(this).removeAttr("checked");
            $(this).removeAttr("disabled");
        });
        //alle inhereted berichten uit zetten: tekst " (rechten overgenomen van groep:") weghalen
        $('#tabPageDetails4').find("label").each(function (i) {
            var labelText = $(this).html();
            var posMsg = labelText.indexOf(" (rechten overgenomen van groep:");
            if (posMsg > 0) {
                labelText = labelText.substring(0, posMsg);
                $(this).html(labelText);
            }
        });
        //rechten per groep
        for (var i in BITAUTHUSERS.user.UserGroups) {
            var userGroup = BITAUTHUSERS.user.UserGroups[i];
            var permissions = userGroup.UserGroupPermissions;
            //per permission aanzetten en disablen
            for (var i in permissions) {
                var funcID = permissions[i].ID;
                $('#tabPageDetails4').find("input[type=checkbox][value=" + funcID + "]").attr("checked", "checked");
                if ($('#tabPageDetails4').find("input[type=checkbox][value=" + funcID + "]").is(":disabled")) {
                    //als die al disabled was door andere group: alleen group naam toevoegen
                    var labelText = $("input[type=checkbox][value=" + funcID + "]").next().html();
                    labelText = labelText.replace(")", ", " + userGroup.Name + ")");
                    $('#tabPageDetails4').find("input[type=checkbox][value=" + funcID + "]").next().html(labelText);
                }
                else {
                    $('#tabPageDetails4').find("input[type=checkbox][value=" + funcID + "]").attr("disabled", "disabled");
                    $('#tabPageDetails4').find("input[type=checkbox][value=" + funcID + "]").next().append(" (rechten overgenomen van groep: " + userGroup.Name + ")");
                }
                $("#bitPermissionsPanelUser" + funcID).removeAttr("disabled");
            }
        }
        //aanvullen met userrechten
        var userPermissions = BITAUTHUSERS.user.UserPermissions;
        for (var i in userPermissions) {
            var funcID = userPermissions[i].ID;
            $('#tabPageDetails4').find("input[type=checkbox][value=" + funcID + "]").attr("checked", "checked");
            $("#bitPermissionsPanelUser" + funcID).removeAttr("disabled");
        }
    },

    openUserGroupsPopup: function () {
        $('#bitUserGroupPerUserDialog').dialog('open');
    },

    addUserGroupPerUser: function (id, name, site) {
        var userGroup = this.findUserGroup(id);

        if (BITAUTHUSERS.user.UserGroups == null) {
            BITAUTHUSERS.user.UserGroups = [];
        }
        //kijk of die er al in zit
        var contains = false;
        for (var i in BITAUTHUSERS.user.UserGroups) {
            var groupCheck = BITAUTHUSERS.user.UserGroups[i];
            if (groupCheck.ID == userGroup.ID) {
                contains = true;
                break;
            }
        }
        if (contains) {
            $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "Koppeling met deze group bestaat al.");
        }
        else {
            BITAUTHUSERS.user.UserGroups.push(userGroup);
            $("#tableUserGroupsPerUser").dataBindList(BITAUTHUSERS.user.UserGroups);
            BITAUTHUSERS.bindUserPermissions();
        }
    },

    findUserGroup: function (id) {
        var returnValue = null;
        for (var i in BITAUTHUSERS.userGroups) {
            if (BITAUTHUSERS.userGroups[i].ID == id) {
                returnValue = BITAUTHUSERS.userGroups[i];
                break;
            }
        }
        return returnValue;
    },

    removeUserGroupPerUser: function (id, index) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u deze gebruikersgroep wilt los koppelen van deze gebruiker?", null, function () {
            BITAUTHUSERS.user.UserGroups.splice(index, 1);
            $("#tableUserGroupsPerUser").dataBindList(BITAUTHUSERS.user.UserGroups);
            BITAUTHUSERS.bindUserPermissions();
        });
    },

    saveUser: function () {
        var validation = $('#bitUserDetailsDialog').validate();
        if (validation) {
            if (!BITAUTHUSERS.user.UserGroups ||
                (BITAUTHUSERS.user.UserGroups && BITAUTHUSERS.user.UserGroups.length == 0)) {
                validation = false;
                $.messageBox(MSGBOXTYPES.WARNING, "Kan niet opslaan, want tenminste 1 groep is verplicht");
            }
        }
        if (validation) {
            //get data from panel
            BITAUTHUSERS.user = $('#bitUserDetailsDialog').collectData(BITAUTHUSERS.user);

            var permissions = [];
            $('#bitUserDetailsDialog').find('input[type=checkbox]').each(function (i) {
                var checked = $(this).is(":checked");
                var disabled = $(this).is(":disabled");
                if (checked && !disabled) {
                    var functionality = new Object();
                    functionality.ID = $(this).val();
                    permissions.push(functionality);
                }
            });
            BITAUTHUSERS.user.UserPermissions = permissions;

            jsonstring = convertToJsonString(BITAUTHUSERS.user);
            BITAJAX.callWebService("SaveUser", jsonstring, function (data) {
                $('#bitUserDetailsDialog').dialog("close");
                BITAUTHUSERS.loadUsers();
                BITAUTHUSERS.showUsers();
            });
        }
    },

    removeUser: function (id) {
        $.deleteConfirmBox("Wilt u deze gebruiker verwijderen?", null, function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteUser", jsonstring, function (data) {
                BITAUTHUSERS.loadUsers();
            });
        });
    },

    sendNewPassword: function () {
        var email = BITAUTHUSERS.user.Email;
        var parametersObject = { email: email };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("SendNewPassword", jsonstring, function (data) {
            $.messageBox(MSGBOXTYPES.INFO, "Er is een nieuw wachtwoord aan deze gebruiker (" + email + ") gestuurd.");
        });
    },
    //////////////////////////////////////////////
    // USER GROUPS
    //////////////////////////////////////////////
    loadUserGroups: function (sort, pagenumber) {
        if (sort != undefined) BITAUTHUSERS.currentSortUserGroups = sort;
        if (pagenumber != undefined) BITAUTHUSERS.currentPageUserGroups = pagenumber;
        var onlyCurrentSite = ($("input[type=radio][name=radioGroupSelect]:checked").val() == "OnlyCurrentSite");

        var parametersObject = {
            sort: BITAUTHUSERS.currentSortUserGroups,
            onlyCurrentSite: onlyCurrentSite,
            pageNumber: BITAUTHUSERS.currentPageUserGroups,
            pageSize: BITAUTHUSERS.pageSize,
            searchString: BITAUTHUSERS.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUserGroups", jsonstring, function (data) {
            BITAUTHUSERS.userGroups = data.d;
            $("#tableUserGroups").dataBindList(data.d, {
                onSort: function (sort) {
                    BITAUTHUSERS.loadUserGroups(sort);
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
            if (BITAUTHUSERS.searchString == "") {
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
        BITAUTHUSERS.openUserGroupDetailsPopup(null);
    },

    openUserGroupDetailsPopup: function (userGroupId) {
        var parametersObject = { id: userGroupId };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUserGroup", jsonstring, function (data) {
            BITAUTHUSERS.userGroup = data.d;
            $('#bitUserGroupDetailsDialog').dataBind(data.d);
            if (data.d.SuperUserGroupType == SUPERUSERTYPES.DEVELOPERS ||
                data.d.SuperUserGroupType == SUPERUSERTYPES.SERVERADMINS)
            {
                $("#divUserGroupSite").hide();
                $("#divUserGroupSuperType").show();
                $("#divUserGroupCompany").hide();
                $("#divUserGroupSite").find("select").removeAttr("data-validation");
                $("#tabPageDetailsUserGroup2").attr("disabled", "disabled");
            }
            else if (data.d.SuperUserGroupType == SUPERUSERTYPES.RESELLERADMINS ||
                data.d.SuperUserGroupType == SUPERUSERTYPES.COMPANYADMINS) {
                $("#divUserGroupSite").hide();
                $("#divUserGroupSuperType").show();
                $("#divUserGroupCompany").show();

                $("#divUserGroupSite").find("select").removeAttr("data-validation");
                $("#tabPageDetailsUserGroup2").attr("disabled", "disabled");
            }
            else {
                $("#divUserGroupSite").show();
                $("#divUserGroupSuperType").hide();
                $("#divUserGroupCompany").hide();
                $("#divUserGroupSite").find("select").attr("data-validation", "required");
                $("#tabPageDetailsUserGroup2").removeAttr("disabled");
            }
            var permissions = BITAUTHUSERS.userGroup.UserGroupPermissions;
            //eerst alle uitzetten
            $('#bitUserGroupDetailsDialog').find("input[type=checkbox]").removeAttr("checked");
            //dan per permission aanzetten
            for (var i in permissions) {
                var funcID = permissions[i].ID;
                $("input[type=checkbox][value=" + funcID + "]").attr("checked", "checked");
                $("#bitPermissionsPanelGroup" + funcID).removeAttr("disabled");
            }
        });
        $('#bitUserGroupDetailsDialog').dialog('open');

    },

    setDefaultPermissions: function () {
        var defaultPermissionSetName = $('#selectDefaultPermissions option:selected').val();
        //eerst alle uitzetten
        $('#bitUserGroupDetailsDialog').find("input[type=checkbox]").removeAttr("checked");

        var defaultPermissions = BITAUTHUSERS.defaultPermissionSets[defaultPermissionSetName];
        //eerst alle uitzetten
        $('#bitUserGroupDetailsDialog').find("input[type=checkbox]").removeAttr("checked");
        //dan per permission aanzetten
        for (var i in defaultPermissions) {
            var funcID = defaultPermissions[i].ID;
            $("input[type=checkbox][value=" + funcID + "]").attr("checked", "checked");
            $("#bitPermissionsPanelGroup" + funcID).removeAttr("disabled");
        }
    },

    saveUserGroup: function () {
        var validation = $('#bitUserGroupDetailsDialog').validate();
        if (validation) {
            //get data from panel
            BITAUTHUSERS.userGroup = $('#bitUserGroupDetailsDialog').collectData(BITAUTHUSERS.userGroup);

            var permissions = [];
            $('#bitUserGroupDetailsDialog').find('input[type=checkbox]').each(function (i) {
                var checked = $(this).is(":checked");
                if (checked) {
                    var functionality = new Object();
                    functionality.ID = $(this).val();
                    permissions.push(functionality);
                }
            });
            BITAUTHUSERS.userGroup.UserGroupPermissions = permissions;

            jsonstring = convertToJsonString(BITAUTHUSERS.userGroup);
            BITAJAX.callWebService("SaveUserGroup", jsonstring, function (data) {
                $('#bitUserGroupDetailsDialog').dialog("close");
                BITAUTHUSERS.loadUserGroups();
                BITAUTHUSERS.showUserGroups();
            });
        }
    },

    removeUserGroup: function (id) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u deze gebruikersgroep verwijderen?", null, function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteUserGroup", jsonstring, function (data) {
                BITAUTHUSERS.loadUserGroups();
            });
        });
    }
}

