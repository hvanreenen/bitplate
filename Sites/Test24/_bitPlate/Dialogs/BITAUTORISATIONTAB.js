//script voor de autorisation tab
//met deze tab kun je per object (page, template, module, enz) autorisatie zetten
//Object Autorisatie dus
var BITAUTORISATIONTAB = {
    currentObject: null,
    hideSiteUserGroupsAndUsers: false,

    initialize: function () {

        $('#bitUserGroupsDialog').initDialog(null, { width: 600, height: 400 }, false);
        $('#bitUsersDialog').initDialog(null, { width: 600, height: 400 }, false);
        //$('#bitPopups').formEnrich();
        this.loadUserGroups();
        this.loadUsers();

        if (BITAUTORISATIONTAB.hideSiteUserGroupsAndUsers) {
            $(".siteAuthorisation").hide();
        }

    },

    loadUserGroups: function () {
        if (!BITAUTORISATIONTAB.hideSiteUserGroupsAndUsers) {
            var parametersObject = {
                sort: "Name",
                pageNumber: 0,
                pageSize: 0,
                searchString: ""
            };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/SiteUsers/SiteAuthService.asmx";
            BITAJAX.callWebService("GetUserGroups", jsonstring, function (data) {
                $('#bitSiteUserGroupsList').dataBindList(data.d);
            });
        }

        var parametersObject = {
            sort: "Name",
            onlyCurrentSite: true,
            pageNumber: 0,
            pageSize: 0,
            searchString: ""
        };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/Autorisation/AuthService.asmx";
        BITAJAX.callWebService("GetUserGroups", jsonstring, function (data) {
            $('#bitBitplateUserGroupsList').dataBindList(data.d);
        });
    },

    loadUsers: function () {
        if (!BITAUTORISATIONTAB.hideSiteUserGroupsAndUsers) {
            var parametersObject = {
                sort: "Name",
                pageNumber: 0,
                pageSize: 0,
                searchString: ""
            };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/SiteUsers/SiteAuthService.asmx";
            BITAJAX.callWebService("GetUsers", jsonstring, function (data) {
                $('#bitSiteUsersList').dataBindList(data.d);
            });
        }

        var parametersObject = {
            sort: "Name",
            onlyCurrentSite: true,
            pageNumber: 0,
            pageSize: 0,
            searchString: ""
        };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/Autorisation/AuthService.asmx";
        BITAJAX.callWebService("GetUsers", jsonstring, function (data) {
            $('#bitBitplateUsersList').dataBindList(data.d);
        });
    },

    dataBind: function () {
        if (BITAUTORISATIONTAB.currentObject.IsNew) {
            $("#tableBitplateUserGroups").dataBindList([]);
            $("#tableSiteUserGroups").dataBindList([]);
            $("#tableBitplateUsers").dataBindList([]);
            $("#tableSiteUsers").dataBindList([]);
        }
        else {
            if (BITAUTORISATIONTAB.currentObject.HasAutorisation) {
                $("#radioHasAutorisation").attr("checked", "checked");
                if (BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups && BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups.length > 0) {
                    $("#divBitplateUserGroups").show();
                    $("#tableBitplateUserGroups").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups);
                }
                else {
                    $("#tableBitplateUserGroups").dataBindList([]);
                }
                if (BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups && BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups.length > 0) {
                    $("#divSiteUserGroups").show();
                    $("#tableSiteUserGroups").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups);
                }
                else {
                    $("#tableSiteUserGroups").dataBindList([]);
                }
                if (BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers && BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers.length > 0) {
                    $("#divBitplateUsers").show();
                    $("#tableBitplateUsers").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers);
                }
                else {
                    $("#tableBitplateUsers").dataBindList([]);
                }
                if (BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers && BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers.length > 0) {
                    $("#divSiteUsers").show();
                    $("#tableSiteUsers").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers);
                }
                else {
                    $("#tableSiteUsers").dataBindList([]);
                }
            }
            else {
                $("#radioNoAutorisation").attr("checked", "checked");
                $("#tableBitplateUserGroups").dataBindList([]);
                $("#tableSiteUserGroups").dataBindList([]);
                $("#tableBitplateUsers").dataBindList([]);
                $("#tableSiteUsers").dataBindList([]);
            }
            BITAUTORISATIONTAB.setDivsDisabled(BITAUTORISATIONTAB.currentObject.HasAutorisation);
        }
    },

    changeHasAutorisation: function () {
        var hasAuth = $("#radioHasAutorisation").is(":checked");
        BITAUTORISATIONTAB.currentObject.HasAutorisation = hasAuth;
        BITAUTORISATIONTAB.setDivsDisabled(hasAuth);
    },

    setDivsDisabled: function (hasAuth) {

        if (hasAuth) {
            $("#divAutorisation").fadeIn('fast');
            //$("#divAutorisation").removeAttr("disabled");
        }
        else {
            if ((BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups && BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups.length > 0) ||
                (BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups && BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups.length > 0) ||
                (BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers && BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers > 0) ||
                (BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers && BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers > 0)) {
                $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u alle autorisatie van deze pagina verwijderen?", null, function () {
                    //$("#divAutorisation").attr("disabled", "disabled");
                    $("#divAutorisation").fadeOut('fast');
                    BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups = [];
                    BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups = [];
                    BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers = [];
                    BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers = [];
                    $("#tableBitplateUserGroups").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups);
                    $("#tableSiteUserGroups").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedUsers);
                    $("#tableBitplateUsers").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups);
                    $("#tableSiteUsers").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedUsers);

                    $("#radioNoAutorisation").attr("checked", "checked");
                },
                null,
                //cancel function
                function () {
                    $("#radioHasAutorisation").attr("checked", "checked");
                });

            }
            else {
                //$("#divAutorisation").attr("disabled", "disabled");
                $("#divAutorisation").fadeOut('fast');
                //BITAUTORISATIONTAB.currentObject.AutorizedUserGroups = [];
                //$("#tableUserGroups").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedUserGroups);
                //BITAUTORISATIONTAB.currentObject.AutorizedUsers = [];
                //$("#tableUsers").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedUsers);
            }


        }
    },

    openUserGroupsPopup: function () {
        $('#bitUserGroupsDialog').dialog('open');
    },

    addUserGroup: function (id, name, type) {
        //alert(id + name);
        var userGroup = new Object();
        userGroup.ID = id;
        userGroup.Name = name;
        if (type == 'SiteUserGroup') {
            if (BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups == null) {
                BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups = [];
            }
            //kijk of die er al in zit
            var contains = false;
            if (BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups) {
                for (var i in BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups) {
                    var userGroupCheck = BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups[i];
                    if (userGroupCheck.ID == userGroup.ID) {
                        contains = true;
                        break;
                    }
                }
            }

            if (contains) {
                $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "Koppeling met deze gebruikersgroep bestaat al");
                //alert("Koppeling met dit script bestaat al");
            }
            else {
                BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups.push(userGroup);
                $("#tableSiteUserGroups").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups);
                $("#divSiteUserGroups").show();
            }
        }
        else if (type == 'BitplateUserGroup') {
            if (BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups == null) {
                BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups = [];
            }
            //kijk of die er al in zit
            var contains = false;
            if (BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups) {
                for (var i in BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups) {
                    var userGroupCheck = BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups[i];
                    if (userGroupCheck.ID == userGroup.ID) {
                        contains = true;
                        break;
                    }
                }
            }

            if (contains) {
                $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "Koppeling met deze gebruikersgroep bestaat al");
                //alert("Koppeling met dit script bestaat al");
            }
            else {
                BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups.push(userGroup);
                $("#tableBitplateUserGroups").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups);
                $("#divBitplateUserGroups").show();
            }
        }
    },

    removeUserGroup: function (id, index, type) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u deze gebruikersgroep wilt los koppelen van deze pagina?", null, function () {
            if (type == 'SiteUserGroup') {
                BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups.splice(index, 1);
                $("#tableSiteUserGroups").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedSiteUserGroups);
            }
            else {
                BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups.splice(index, 1);
                $("#tableBitplateUserGroups").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedBitplateUserGroups);
            }
        });
    },

    openUsersPopup: function () {
        $('#bitUsersDialog').dialog('open');
    },

    addUser: function (id, name, type) {
        var user = new Object();
        user.ID = id;
        user.Name = name;
        user.CompleteName = name;
        if (type == 'SiteUser') {
            if (BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers == null) {
                BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers = [];
            }
            //kijk of die er al in zit
            var contains = false;
            if (BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers) {
                for (var i in BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers) {
                    var userCheck = BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers[i];
                    if (userCheck.ID == user.ID) {
                        contains = true;
                        break;
                    }
                }
            }

            if (contains) {
                $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "Koppeling met deze gebruiker bestaat al");

            }
            else {
                BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers.push(user);
                console.log(user);
                $("#tableSiteUsers").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers);
                $("#divSiteUsers").show();
            }
        }
        else {
            if (BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers == null) {
                BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers = [];
            }
            //kijk of die er al in zit
            var contains = false;
            if (BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers) {
                for (var i in BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers) {
                    var userCheck = BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers[i];
                    if (userCheck.ID == user.ID) {
                        contains = true;
                        break;
                    }
                }
            }

            if (contains) {
                $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "Koppeling met deze gebruiker bestaat al");

            }
            else {
                BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers.push(user);
                $("#tableBitplateUsers").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers);
                $("#divBitplateUsers").show();
            }
        }
    },

    removeUser: function (id, index, type) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u deze gebruiker wilt los koppelen van deze pagina?", null, function () {
            if (type == 'SiteUser') {
                BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers.splice(index, 1);
                $("#tableSiteUsers").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedSiteUsers);
            }
            else {
                BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers.splice(index, 1);
                $("#tableBitplateUsers").dataBindList(BITAUTORISATIONTAB.currentObject.AutorizedBitplateUsers);
            }
        });
    }

};