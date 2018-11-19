

//bestand voor licentie company
var BITAUTHRESELLERS = {
    currentSort: "Name ASC",
    searchString: "",
    reseller: null,


    initialize: function () {
        //jQuery.support.cors = true; //
        BITAJAX.dataServiceUrl = "AuthService.aspx";
        $('#bitCompanyDetailsDialog').initDialog(BITAUTHRESELLERS.saveReseller);

        $('#bitPopups').formEnrich();
        $('#bitDivSearch').hide();

    },

    loadResellers: function (sort) {

        if (sort != undefined) BITAUTHRESELLERS.currentSort = sort;
        
        var parametersObject = {
            sort: BITAUTHRESELLERS.currentSort,
            searchString: BITAUTHRESELLERS.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.callWebService("GetResellers", jsonstring, function (data) {
            $("#tableResellers").dataBindList(data.d, {
                onSort: function (sort) {
                    BITAUTHRESELLERS.loadResellers(sort);
                }
            });

        });
    },


    newReseller: function () {

        BITAUTHRESELLERS.openResellerDetailsPopup(null);
    },

    openResellerDetailsPopup: function (id) {

        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetCompany", jsonstring, function (data) {
            BITAUTHRESELLERS.reseller = data.d;
            console.log(data);
            $('#bitCompanyDetailsDialog').dataBind(data.d);
            console.log('halo');
            if (BITAUTHRESELLERS.reseller.IsNew) {
                $('#bitCompanyDetailsDialog').dialog({ title: "Nieuwe reseller" });

                $('.bitTabs').tabs("select", 0);
                $('#textNaam').focus();
            }
            else {
                $('#bitCompanyDetailsDialog').dialog({ title: "Reseller: " + BITAUTHRESELLERS.reseller.Name });
            }
        });
        $('#bitCompanyDetailsDialog').dialog('open');
    },

    saveReseller: function () {
        var validation = $('#bitCompanyDetailsDialog').validate();
        if (validation) {
            //get data from panel
            BITAUTHRESELLERS.reseller = $('#bitCompanyDetailsDialog').collectData(BITAUTHRESELLERS.reseller);

            jsonstring = convertToJsonString(BITAUTHRESELLERS.reseller);
            BITAJAX.callWebService("SaveCompany", jsonstring, function (data) {
                $('#bitCompanyDetailsDialog').dialog("close");
                BITAUTHRESELLERS.loadResellers();
            });
        }
    },

    removeReseller: function (id, type) {
        $.deleteConfirmBox("Wilt u deze relatie verwijderen?", null, function () {
            BITAUTHRESELLERS.type = type;
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteCompany", jsonstring, function (data) {
                BITAUTHRESELLERS.loadResellers();
            });
        });
    }
}

