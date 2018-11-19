
//voor users en usergroups

var BITMULTISITEUSERS = {
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
        BITAJAX.dataServiceUrl = "/Auth/AuthService.aspx";
        $('#bitUserDetailsDialog').initDialog(BITMULTISITEUSERS.saveUser);

        $('#bitSitesPerUserDialog').initDialog(null, { width: 200, height: 400 }, false, {
            "Sluiten": function () {
                $(this).dialog('close');
            }
        });

        $('#bitPopups').formEnrich();

        //$('#tabPageDetails4').dockable();
        //$('#tabPageDetailsUserGroup2').dockable();

        $('#bitSearchTextbox').searchable(BITMULTISITEUSERS.search);
        //this.attachCheckBoxClickEvents();
    },



    //filterData: function () {
    //    this.loadUsers();

    //},

    search: function (searchText) {
        BITMULTISITEUSERS.searchString = searchText;

        BITMULTISITEUSERS.loadUsers();

    },
    //////////////////////////////////////////////
    // USERS
    //////////////////////////////////////////////
    loadUsers: function (sort, pagenumber) {
        if (sort != undefined) BITMULTISITEUSERS.currentSortUsers = sort;

        var parametersObject = {
            sort: BITMULTISITEUSERS.currentSortUsers,
            searchString: BITMULTISITEUSERS.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetMultiSiteUsers", jsonstring, function (data) {
            $("#tableUsers").dataBindList(data.d, {
                onSort: function (sort) {
                    BITMULTISITEUSERS.loadUsers(sort);
                }
            });

        });
    },



    newUser: function () {
        BITMULTISITEUSERS.openUserDetailsPopup(null);
    },

    openUserDetailsPopup: function (id) {

        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetMultiSiteUser", jsonstring, function (data) {
            BITMULTISITEUSERS.user = data.d;
            $('#bitUserDetailsDialog').dataBind(data.d);
            $('#tableSitesPerUser').dataBindList(data.d.Sites);

        });
        $('#bitUserDetailsDialog').dialog('open');
    },

    saveUser: function () {
        var validation = $('#bitUserDetailsDialog').validate();
        if (validation) {
            if (!BITMULTISITEUSERS.user.Sites ||
                (BITMULTISITEUSERS.user.Sites && BITMULTISITEUSERS.user.Sites.length == 0)) {
                validation = false;
                $.messageBox(MSGBOXTYPES.WARNING, "Kan niet opslaan, want tenminste 1 site is verplicht.");
            }
        }
        if (validation) {
            //get data from panel
            BITMULTISITEUSERS.user = $('#bitUserDetailsDialog').collectData(BITMULTISITEUSERS.user);

            jsonstring = convertToJsonString(BITMULTISITEUSERS.user);
            BITAJAX.callWebService("SaveMultiSiteUser", jsonstring, function (data) {
                $('#bitUserDetailsDialog').dialog("close");
                BITMULTISITEUSERS.loadUsers();

            });
        }
    },

    removeUser: function (id) {
        $.deleteConfirmBox("Wilt u deze metaserver-gebruiker verwijderen?", null, function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteMultiSiteUser", jsonstring, function (data) {
                BITMULTISITEUSERS.loadUsers();
            });
        });
    },

    openSitesPopup: function () {
        $('#bitSitesPerUserDialog').dialog('open');
    },

    addSitePerUser: function (id, name) {
        //var userGroup = this.findUserGroup(id);
        //var userGroup = new Object();
        //userGroup.ID = id;
        //userGroup.Name = name;
        //var returnValue = null;
        //var parametersObject = { id: id };
        //var jsonstring = JSON.stringify(parametersObject);
        //BITAJAX.callWebService("GetSite", jsonstring, function (data) {
        //var site = data.d;
        var site = new Object();
        site.ID = id;
        site.Name = name;
        site.DomainName = name;
            if (BITMULTISITEUSERS.user.Sites == null) {
                BITMULTISITEUSERS.user.Sites = [];
            }
            //kijk of die er al in zit
            var contains = false;
            for (var i in BITMULTISITEUSERS.user.Sites) {
                var siteCheck = BITMULTISITEUSERS.user.Sites[i];
                if (siteCheck.ID == site.ID) {
                    contains = true;
                    break;
                }
            }
            if (contains) {
                $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "Koppeling met deze site bestaat al.");
            }
            else {
                BITMULTISITEUSERS.user.Sites.push(site);
                $("#tableSitesPerUser").dataBindList(BITMULTISITEUSERS.user.Sites);
            }
        //});
    },



    removeSitePerUser: function (id, index) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u deze site wilt los koppelen van deze gebruiker?", null, function () {
            BITMULTISITEUSERS.user.Sites.splice(index, 1);
            $("#tableSitesPerUser").dataBindList(BITMULTISITEUSERS.user.Sites);
           
        });
    },

    sendNewPassword: function () {
        var email = BITMULTISITEUSERS.user.Email;
        var parametersObject = { email: email };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("SendNewMultiSiteUserPassword", jsonstring, function (data) {
            $.messageBox(MSGBOXTYPES.INFO, "Er is een nieuw wachtwoord aan deze gebruiker (" + email + ") gestuurd.");
        });
    }


}

