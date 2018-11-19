
//voor users en usergroups

var BITSERVERUSERS = {
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
        $('#bitUserDetailsDialog').initDialog(BITSERVERUSERS.saveUser);
        
        

        $('#bitPopups').formEnrich();

        //$('#tabPageDetails4').dockable();
        //$('#tabPageDetailsUserGroup2').dockable();

        $('#bitSearchTextbox').searchable(BITSERVERUSERS.search);
        //this.attachCheckBoxClickEvents();
    },

    

    //filterData: function () {
    //    this.loadUsers();
        
    //},

    search: function (searchText) {
        BITSERVERUSERS.searchString = searchText;
        
            BITSERVERUSERS.loadUsers();
            
    },
    //////////////////////////////////////////////
    // USERS
    //////////////////////////////////////////////
    loadUsers: function (sort, pagenumber) {
        if (sort != undefined) BITSERVERUSERS.currentSortUsers = sort;
        
        var parametersObject = {
            sort: BITSERVERUSERS.currentSortUsers,
            searchString: BITSERVERUSERS.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUsers", jsonstring, function (data) {
            $("#tableUsers").dataBindList(data.d, {
                onSort: function (sort) {
                    BITSERVERUSERS.loadUsers(sort);
                }
            });

        });
    },

    

    newUser: function () {
        BITSERVERUSERS.openUserDetailsPopup(null);
    },

    openUserDetailsPopup: function (id) {

        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetUser", jsonstring, function (data) {
            BITSERVERUSERS.user = data.d;
            $('#bitUserDetailsDialog').dataBind(data.d);
          

        });
        $('#bitUserDetailsDialog').dialog('open');
    },

    saveUser: function () {
        var validation = $('#bitUserDetailsDialog').validate();
        
        if (validation) {
            //get data from panel
            BITSERVERUSERS.user = $('#bitUserDetailsDialog').collectData(BITSERVERUSERS.user);

            

            jsonstring = convertToJsonString(BITSERVERUSERS.user);
            BITAJAX.callWebService("SaveUser", jsonstring, function (data) {
                $('#bitUserDetailsDialog').dialog("close");
                BITSERVERUSERS.loadUsers();
                
            });
        }
    },

    removeUser: function (id) {
        $.deleteConfirmBox("Wilt u deze metaserver-gebruiker verwijderen?", null, function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteUser", jsonstring, function (data) {
                BITSERVERUSERS.loadUsers();
            });
        });
    },

    sendNewPassword: function () {
        var email = BITSERVERUSERS.user.Email;
        var parametersObject = { email: email };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("SendNewMetaServerUserPassword", jsonstring, function (data) {
            $.messageBox(MSGBOXTYPES.INFO, "Er is een nieuw wachtwoord aan deze gebruiker (" + email + ") gestuurd.");
        });
    }
    
}

