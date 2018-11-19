

//bestand voor licentie company
var BITAUTHCOMPANIES = {
    currentSort: "Name ASC",
    searchString: "",
    company: null,
    currentPage: 1,
    pageSize: 25,

    initialize: function () {
        //jQuery.support.cors = true; //
        BITAJAX.dataServiceUrl = "AuthService.aspx";
        $('#bitCompanyDetailsDialog').initDialog(BITAUTHCOMPANIES.saveCompany);

        $('#bitPopups').formEnrich();
        $('#bitDivSearch').hide();
    },

    loadCompanies: function (resellerId, sort, pagenumber) {
        if (sort != undefined) BITAUTHCOMPANIES.currentSort = sort;
        if (pagenumber != undefined) BITAUTHCOMPANIES.currentPage = pagenumber;
        if (!resellerId) resellerId = "";

        var parametersObject = {
            resellerId: resellerId,
            sort: BITAUTHCOMPANIES.currentSort,
            pageNumber: BITAUTHCOMPANIES.currentPage,
            pageSize: BITAUTHCOMPANIES.pageSize,
            searchString: BITAUTHCOMPANIES.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);
        
        BITAJAX.callWebService("GetCompanies", jsonstring, function (data) {
            $("#tableCompanies").dataBindList(data.d, {
                onSort: function (sort) {
                    BITAUTHCOMPANIES.loadCompanies(sort);
                }
            });
        });
    },

    filterCompanies: function(select){
        var resellerID = $(select).val();
        BITAUTHCOMPANIES.loadCompanies(resellerID);
    },

    newCompany: function () {
        
        BITAUTHCOMPANIES.openCompanyDetailsPopup(null);
    },


    openCompanyDetailsPopup: function (id) {
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetCompany", jsonstring, function (data) {
            BITAUTHCOMPANIES.company = data.d;
            
            $('#bitCompanyDetailsDialog').dataBind(data.d);
            if (BITAUTHCOMPANIES.company.IsNew) {
                $('#bitCompanyDetailsDialog').dialog({ title: "Nieuwe klant" });
                
                $('.bitTabs').tabs("select", 0);
                $('#textNaam').focus();
            }
            else {
                $('#bitCompanyDetailsDialog').dialog({ title: "Klant: " + BITAUTHCOMPANIES.company.Name });
                
            }
        });
        $('#bitCompanyDetailsDialog').dialog('open');
    },

    saveCompany: function () {
        var validation = $('#bitCompanyDetailsDialog').validate();
        if (validation) {
            //get data from panel
            BITAUTHCOMPANIES.company = $('#bitCompanyDetailsDialog').collectData(BITAUTHCOMPANIES.company);
            
            jsonstring = convertToJsonString(BITAUTHCOMPANIES.company);
            BITAJAX.callWebService("SaveCompany", jsonstring, function (data) {
                $('#bitCompanyDetailsDialog').dialog("close");
                BITAUTHCOMPANIES.loadCompanies(BITAUTHCOMPANIES.type);
            });
        }
    },

    removeCompany: function (id, type) {
        $.deleteConfirmBox("Wilt u deze Bitplate klant verwijderen?", null, function () {
            BITAUTHCOMPANIES.type = type;
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteCompany", jsonstring, function (data) {
                BITAUTHCOMPANIES.loadCompanies(BITAUTHCOMPANIES.type);
            });
        });
    }
}

