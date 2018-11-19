

//bestand voor licentie company
var BITRESELLERS = {
    currentSort: "Name ASC",
    searchString: "",
    reseller: null,


    initialize: function () {
        //jQuery.support.cors = true; //
        BITAJAX.dataServiceUrl = "LicenseService.aspx";
        $('#bitCompanyDetailsDialog').initDialog(BITRESELLERS.saveReseller);

        $('#bitPopups').formEnrich();
        $('#bitDivSearch').hide();

    },

    loadResellers: function (sort) {

        if (sort != undefined) BITRESELLERS.currentSort = sort;
        
        var parametersObject = {
            sort: BITRESELLERS.currentSort,
            searchString: BITRESELLERS.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.callWebService("GetResellers", jsonstring, function (data) {
            $("#tableResellers").dataBindList(data.d, {
                onSort: function (sort) {
                    BITRESELLERS.loadResellers(sort);
                }
            });

        });
    },


    newReseller: function () {

        BITRESELLERS.openResellerDetailsPopup(null);
    },

    openResellerDetailsPopup: function (id) {

        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetCompany", jsonstring, function (data) {
            BITRESELLERS.reseller = data.d;
            console.log(data);
            $('#bitCompanyDetailsDialog').dataBind(data.d);
            console.log('halo');
            if (BITRESELLERS.reseller.IsNew) {
                $('#bitCompanyDetailsDialog').dialog({ title: "Nieuwe reseller" });

                $('.bitTabs').tabs("select", 0);
                $('#textNaam').focus();
            }
            else {
                $('#bitCompanyDetailsDialog').dialog({ title: "Reseller: " + BITRESELLERS.reseller.Name });
            }
        });
        $('#bitCompanyDetailsDialog').dialog('open');
    },

    saveReseller: function () {
        var validation = $('#bitCompanyDetailsDialog').validate();
        if (validation) {
            //get data from panel
            BITRESELLERS.reseller = $('#bitCompanyDetailsDialog').collectData(BITRESELLERS.reseller);

            jsonstring = convertToJsonString(BITRESELLERS.reseller);
            BITAJAX.callWebService("SaveCompany", jsonstring, function (data) {
                $('#bitCompanyDetailsDialog').dialog("close");
                BITRESELLERS.loadResellers();
            });
        }
    },

    removeReseller: function (id, type) {
        $.deleteConfirmBox("Wilt u deze relatie verwijderen?", null, function () {
            BITRESELLERS.type = type;
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteCompany", jsonstring, function (data) {
                BITRESELLERS.loadResellers();
            });
        });
    }
}

