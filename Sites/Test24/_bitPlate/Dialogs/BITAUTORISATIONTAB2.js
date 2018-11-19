//script voor de autorisation tab
//met deze tab kun je per object (page, template, module, enz) autorisatie zetten
//Object Autorisatie dus
var BITAUTORISATIONTAB2 = {
    currentObject: null,
    hideSiteUserGroupsAndUsers: false,

    initialize: function () {

        $('#bitUserGroupsDialog2').initDialog(null, { width: 600, height: 400 }, false);
        $('#bitUsersDialog2').initDialog(null, { width: 600, height: 400 }, false);
        //$('#bitPopups').formEnrich();
        this.loadUserGroups();
        this.loadUsers();

        if (BITAUTORISATIONTAB2.hideSiteUserGroupsAndUsers) {
            $(".siteAuthorisation").hide();
        }

    },
    
    loadUserGroups: function () {
        if (!BITAUTORISATIONTAB2.hideSiteUserGroupsAndUsers) {
            var parametersObject = {
                sort: "Name",
                pageNumber: 0,
                pageSize: 0,
                searchString: ""
            };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/SiteUsers/SiteAuthService.asmx";
            BITAJAX.callWebService("GetUserGroups", jsonstring, function (data) {
                $('#bitSiteUserGroupsList2').dataBindList(data.d);
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
            $('#bitBitplateUserGroupsList2').dataBindList(data.d);
        });
    },

    loadUsers: function () {
        if (!BITAUTORISATIONTAB2.hideSiteUserGroupsAndUsers) {
            var parametersObject = {
                sort: "Name",
                pageNumber: 0,
                pageSize: 0,
                searchString: ""
            };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/SiteUsers/SiteAuthService.asmx";
            BITAJAX.callWebService("GetUsers", jsonstring, function (data) {
                $('#bitSiteUsersList2').dataBindList(data.d);
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
            $('#bitBitplateUsersList2').dataBindList(data.d);
        });
    },

    dataBind: function () {
        if (BITAUTORISATIONTAB2.currentObject.IsNew) {
            $("#tableBitplateUserGroups2").dataBindList([]);
            $("#tableSiteUserGroups2").dataBindList([]);
            $("#tableBitplateUsers2").dataBindList([]);
            $("#tableSiteUsers2").dataBindList([]);
        }
        else {
            if (BITAUTORISATIONTAB2.currentObject.HasAutorisation) {
                $("#radioHasAutorisation2").attr("checked", "checked");
                if (BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups && BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups.length > 0) {
                    $("#divBitplateUserGroups2").show();
                    $("#tableBitplateUserGroups2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups);
                }
                else {
                    $("#tableBitplateUserGroups2").dataBindList([]);
                }
                if (BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups && BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups.length > 0) {
                    $("#divSiteUserGroups2").show();
                    $("#tableSiteUserGroups2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups);
                }
                else {
                    $("#tableSiteUserGroups2").dataBindList([]);
                }
                if (BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers && BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers.length > 0) {
                    $("#divBitplateUsers2").show();
                    $("#tableBitplateUsers2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers);
                }
                else {
                    $("#tableBitplateUsers2").dataBindList([]);
                }
                if (BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers && BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers.length > 0) {
                    $("#divSiteUsers2").show();
                    $("#tableSiteUsers2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers);
                }
                else {
                    $("#tableSiteUsers2").dataBindList([]);
                }
            }
            else {
                $("#radioNoAutorisation").attr("checked", "checked");
                $("#tableBitplateUserGroups2").dataBindList([]);
                $("#tableSiteUserGroups2").dataBindList([]);
                $("#tableBitplateUsers2").dataBindList([]);
                $("#tableSiteUsers2").dataBindList([]);
            }
            BITAUTORISATIONTAB2.setDivsDisabled(BITAUTORISATIONTAB2.currentObject.HasAutorisation);
        }
    },

    changeHasAutorisation: function () {
        var hasAuth = $("#radioHasAutorisation2").is(":checked");
        BITAUTORISATIONTAB2.setDivsDisabled(hasAuth);
    },

    setDivsDisabled: function (hasAuth) {

        if (hasAuth) {
            $("#divAutorisation2").fadeIn('fast');
            //$("#divAutorisation2").removeAttr("disabled");
        }
        else {
            if ((BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups && BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups.length > 0) ||
                (BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups && BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups.length > 0) ||
                (BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers && BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers > 0) ||
                (BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers && BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers > 0)) {
                $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u alle autorisatie van deze pagina verwijderen?", null, function () {
                    //$("#divAutorisation2").attr("disabled", "disabled");
                    $("#divAutorisation2").fadeOut('fast');
                    BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups = [];
                    BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups = [];
                    BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers = [];
                    BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers = [];
                    $("#tableBitplateUserGroups2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups);
                    $("#tableSiteUserGroups2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedUsers);
                    $("#tableBitplateUsers2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups);
                    $("#tableSiteUsers2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedUsers);

                    $("#radioNoAutorisation2").attr("checked", "checked");
                },
                null,
                //cancel function
                function () {
                    $("#radioHasAutorisation2").attr("checked", "checked");
                });

            }
            else {
                //$("#divAutorisation2").attr("disabled", "disabled");
                $("#divAutorisation2").fadeOut('fast');
                //BITAUTORISATIONTAB2.currentObject.AutorizedUserGroups = [];
                //$("#tableUserGroups").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedUserGroups);
                //BITAUTORISATIONTAB2.currentObject.AutorizedUsers = [];
                //$("#tableUsers").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedUsers);
            }


        }
    },

    openUserGroupsPopup: function () {
        $('#bitUserGroupsDialog2').dialog('open');
    },

    addUserGroup: function (id, name, type) {
        //alert(id + name);
        var userGroup = new Object();
        userGroup.ID = id;
        userGroup.Name = name;
        if (type == 'SiteUserGroup') {
            if (BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups == null) {
                BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups = [];
            }
            //kijk of die er al in zit
            var contains = false;
            if (BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups) {
                for (var i in BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups) {
                    var userGroupCheck = BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups[i];
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
                BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups.push(userGroup);
                $("#tableSiteUserGroups2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups);
                $("#divSiteUserGroups2").show();
            }
        }
        else if (type == 'BitplateUserGroup') {
            if (BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups == null) {
                BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups = [];
            }
            //kijk of die er al in zit
            var contains = false;
            if (BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups) {
                for (var i in BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups) {
                    var userGroupCheck = BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups[i];
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
                BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups.push(userGroup);
                $("#tableBitplateUserGroups2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups);
                $("#divBitplateUserGroups2").show();
            }
        }
    },

    removeUserGroup: function (id, index, type) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u deze gebruikersgroep wilt los koppelen van deze pagina?", null, function () {
            if (type == 'SiteUserGroup') {
                BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups.splice(index, 1);
                $("#tableSiteUserGroups2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedSiteUserGroups);
            }
            else {
                BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups.splice(index, 1);
                $("#tableBitplateUserGroups2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUserGroups);
            }
        });
    },

    openUsersPopup: function () {
        $('#bitUsersDialog2').dialog('open');
    },

    addUser: function (id, name, type) {
        var user = new Object();
        user.ID = id;
        user.Name = name;
        user.CompleteName = name;
        if (type == 'SiteUser') {
            if (BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers == null) {
                BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers = [];
            }
            //kijk of die er al in zit
            var contains = false;
            if (BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers) {
                for (var i in BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers) {
                    var userCheck = BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers[i];
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
                BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers.push(user);
                //console.log(user);
                $("#tableSiteUsers2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers);
                $("#divSiteUsers2").show();
            }
        }
        else {
            if (BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers == null) {
                BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers = [];
            }
            //kijk of die er al in zit
            var contains = false;
            if (BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers) {
                for (var i in BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers) {
                    var userCheck = BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers[i];
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
                BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers.push(user);
                $("#tableBitplateUsers2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers);
                $("#divBitplateUsers2").show();
            }
        }
    },

    removeUser: function (id, index, type) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u deze gebruiker wilt los koppelen van deze pagina?", null, function () {
            if (type == 'SiteUser') {
                BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers.splice(index, 1);
                $("#tableSiteUsers2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedSiteUsers);
            }
            else {
                BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers.splice(index, 1);
                $("#tableBitplateUsers2").dataBindList(BITAUTORISATIONTAB2.currentObject.AutorizedBitplateUsers);
            }
        });
    }

};