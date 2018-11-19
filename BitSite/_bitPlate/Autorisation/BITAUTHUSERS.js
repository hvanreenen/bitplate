
//voor users en usergroups

var SUPERUSERTYPES = { NONSUPERUSER: 0, SITEADMINS: 1, COMPANYADMINS: 2, RESELLERADMINS: 3, SERVERADMINS: 4, DEVELOPERS: 9};
var BITAUTHUSERS = {
    currentSortUsers: "Name ASC",
    searchString: "",
    type: "Users",
    userGroups: [], //bewaar de usergroups in variabele, want we hebben de rechten ervan nodig
    user: null,
    defaultPermissionSets: [],
    currentPageUsers: 1,
    pageSize: 25,

    initialize: function () {
        //jQuery.support.cors = true; //
        BITAJAX.dataServiceUrl = "AuthService.asmx";
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
        
    },

    search: function (searchText) {
        BITAUTHUSERS.searchString = searchText;
        
            BITAUTHUSERS.loadUsers();
            
    },
    //////////////////////////////////////////////
    // USERS
    //////////////////////////////////////////////
    loadUsers: function (sort, pagenumber) {
        if (sort != undefined) BITAUTHUSERS.currentSortUsers = sort;
        
        var parametersObject = {
            sort: BITAUTHUSERS.currentSortUsers,
            searchString: BITAUTHUSERS.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUsers", jsonstring, function (data) {
            $("#tableUsers").dataBindList(data.d, {
                onSort: function (sort) {
                    BITAUTHUSERS.loadUsers(sort);
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

        });
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

            if (BITAUTHUSERS.user.IsSystemValue) {
                $('#bitUserDetailsDialog').find("input").attr("disabled", "disabled");
                $(".ui-dialog-buttonpane button:contains('Opslaan')").button("disable");
            }
            else {
                $('#bitUserDetailsDialog').find("input").removeAttr("disabled");
                $(".ui-dialog-buttonpane button:contains('Opslaan')").button("enable");
            }

            BITAUTHUSERS.bindUserPermissions();

        });
        $('#bitUserDetailsDialog').dialog('open');
    },

    bindUserPermissions: function () {
        //Eerst per usergroep (waar gebruiker inzit) kijken naar de rechten. Daarna aanvullen met userrechten
        //Zit een recht in de usergroep? Dan staat het recht bij de user aan en wordt die disabled
        //achter de tekst kom te staan: " (rechten overgenomen van groep:")
        //eerst alle uitzetten
        $('#tabPageDetails4').find("input[type=checkbox]").each(function (i) {
            $(this).removeAttr("checked");
            $(this).removeAttr("disabled");
        });
        //alle inherited berichten uit zetten: tekst " (rechten overgenomen van groep:") weghalen
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
            var permissions = userGroup.Permissions;
            //per permission aanzetten en disablen
            for (var i in permissions) {
                var funcNumber = permissions[i];
                $("#bitPermissionsPanelUser" + funcNumber).removeAttr("disabled");
                $('#tabPageDetails4').find("input[type=checkbox][value=" + funcNumber + "]").attr("checked", "checked");
                
                if ($('#tabPageDetails4').find("input[type=checkbox][value=" + funcNumber + "]").is(":disabled")) {
                    //als die al disabled was door andere group: alleen group naam toevoegen
                    var labelText = $("input[type=checkbox][value=" + funcNumber + "]").next().html();
                    labelText = labelText.replace(")", ", " + userGroup.Name + ")");
                    $('#tabPageDetails4').find("input[type=checkbox][value=" + funcNumber + "]").next().html(labelText);
                }
                else {
                    $('#tabPageDetails4').find("input[type=checkbox][value=" + funcNumber + "]").attr("disabled", "disabled");
                    //$('#tabPageDetails4').find("input[type=checkbox][value=" + funcNumber + "]").attr("readonly", "readonly");
                    //$('#tabPageDetails4').find("input[type=checkbox][value=" + funcNumber + "]").hide();
                    $('#tabPageDetails4').find("input[type=checkbox][value=" + funcNumber + "]").next().append(" (rechten overgenomen van groep: " + userGroup.Name + ")");
                }
                
            }
        }
        //aanvullen met userrechten
        var userPermissions = BITAUTHUSERS.user.Permissions;
        for (var i in userPermissions) {
            var funcNumber = userPermissions[i];
            $('#tabPageDetails4').find("input[type=checkbox][value=" + funcNumber + "]").attr("checked", "checked");
            $("#bitPermissionsPanelUser" + funcNumber).removeAttr("disabled");
        }

        
    },

    openUserGroupsPopup: function () {
        $('#bitUserGroupPerUserDialog').dialog('open');
    },

    addUserGroupPerUser: function (id, name, site) {
        //var userGroup = this.findUserGroup(id);
        //var userGroup = new Object();
        //userGroup.ID = id;
        //userGroup.Name = name;
        //var returnValue = null;
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUserGroup", jsonstring, function (data) {
            var userGroup = data.d;
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
        });
    },

    findUserGroup: function (id) {
        var returnValue = null;
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUserGroup", jsonstring, function (data) {
            return data.d;
        });
        //for (var i in BITAUTHUSERS.userGroups) {
        //    if (BITAUTHUSERS.userGroups[i].ID == id) {
        //        returnValue = BITAUTHUSERS.userGroups[i];
        //        break;
        //    }
        //}
        
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
           $('#tabPageDetails4').find('input[type=checkbox]').not('.checkAll').each(function (i) {
                var checked = $(this).is(":checked");
                var disabled = $(this).is(":disabled");
                if (checked && !disabled) {
                    var funcNumber = $(this).val();
                    permissions.push(funcNumber);
                }
            });
            BITAUTHUSERS.user.Permissions = permissions;

            jsonstring = convertToJsonString(BITAUTHUSERS.user);
            BITAJAX.callWebService("SaveUser", jsonstring, function (data) {
                $('#bitUserDetailsDialog').dialog("close");
                //voor het geval hij nog open mocht staan
                $('#bitUserGroupPerUserDialog').dialog('close');
                BITAUTHUSERS.loadUsers();
                
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
    }
    
}

