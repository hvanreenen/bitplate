

//bestand voor licentie company
var BITCOMPANIES = {
    currentSort: "Name ASC",
    searchString: "",
    company: null,
    currentPage: 1,
    pageSize: 25,

    initialize: function () {
        //jQuery.support.cors = true; //
        BITAJAX.dataServiceUrl = "LicenseService.aspx";
        $('#bitCompanyDetailsDialog').initDialog(BITCOMPANIES.saveCompany);

        $('#bitPopups').formEnrich();
        $('#bitDivSearch').hide();
    },

    loadCompanies: function (resellerId, sort, pagenumber) {
        if (sort != undefined) BITCOMPANIES.currentSort = sort;
        if (pagenumber != undefined) BITCOMPANIES.currentPage = pagenumber;
        if (!resellerId) resellerId = "";

        var parametersObject = {
            resellerId: resellerId,
            sort: BITCOMPANIES.currentSort,
            pageNumber: BITCOMPANIES.currentPage,
            pageSize: BITCOMPANIES.pageSize,
            searchString: BITCOMPANIES.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);
        
        BITAJAX.callWebService("GetCompanies", jsonstring, function (data) {
            $("#tableCompanies").dataBindList(data.d, {
                onSort: function (sort) {
                    BITCOMPANIES.loadCompanies(resellerId, sort);
                }
            });
        });
    },

    filterCompanies: function(select){
        var resellerID = $(select).val();
        BITCOMPANIES.loadCompanies(resellerID);
    },

    newCompany: function () {
        
        BITCOMPANIES.openCompanyDetailsPopup(null);
    },


    openCompanyDetailsPopup: function (id) {
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetCompany", jsonstring, function (data) {
            BITCOMPANIES.company = data.d;
            
            $('#bitCompanyDetailsDialog').dataBind(data.d);
            if (BITCOMPANIES.company.IsNew) {
                $('#bitCompanyDetailsDialog').dialog({ title: "Nieuwe klant" });
                
                $('.bitTabs').tabs("select", 0);
                $('#textNaam').focus();
            }
            else {
                $('#bitCompanyDetailsDialog').dialog({ title: "Klant: " + BITCOMPANIES.company.Name });
                
            }
        });
        $('#bitCompanyDetailsDialog').dialog('open');
    },

    saveCompany: function () {
        var validation = $('#bitCompanyDetailsDialog').validate();
        if (validation) {
            //get data from panel
            BITCOMPANIES.company = $('#bitCompanyDetailsDialog').collectData(BITCOMPANIES.company);
            
            jsonstring = convertToJsonString(BITCOMPANIES.company);
            BITAJAX.callWebService("SaveCompany", jsonstring, function (data) {
                $('#bitCompanyDetailsDialog').dialog("close");
                BITCOMPANIES.loadCompanies(BITCOMPANIES.type);
            });
        }
    },

    removeCompany: function (id, type) {
        $.deleteConfirmBox("Wilt u deze Bitplate klant verwijderen?", null, function () {
            BITCOMPANIES.type = type;
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteCompany", jsonstring, function (data) {
                BITCOMPANIES.loadCompanies(BITCOMPANIES.type);
            });
        });
    }
}

