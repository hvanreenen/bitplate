

//bestand voor licentie site
var BITAUTHSITE = {
    currentSort: "Name ASC",
    searchString: "",
    site: null,
    license: null,
    currentPage: 1,
    pageSize: 25,

    initialize: function () {
        //jQuery.support.cors = true; //
        BITAJAX.dataServiceUrl = "AuthService.asmx";
        //$('#bitSiteDetailsDialog').initDialog(BITAUTHSITE.saveSite);

        //$('#bitPopups').formEnrich();

    },

    loadSites: function (sort) {
        if (sort != undefined) BITAUTHSITE.currentSort = sort;
        
        var parametersObject = {
            sort: BITAUTHSITE.currentSort,
            searchString: BITAUTHSITE.searchString
        };

        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetSites", jsonstring, function (data) {
            $("#tableSites").dataBindList(data.d, {
                onSort: function (sort) {
                    BITAUTHSITE.loadSites(sort);
                }
            });
        });
    },

    //newSite: function () {
    //    BITAUTHSITE.openSiteDetailsPopup(null);
    //},

    //openSiteDetailsPopup: function (id) {
    //    var parametersObject = { id: id };
    //    var jsonstring = JSON.stringify(parametersObject);
    //    BITAJAX.callWebService("GetSite", jsonstring, function (data) {
    //        BITAUTHSITE.site = data.d;
    //        if (BITAUTHSITE.site.IsNew) {
    //            $("#divClientSiteID").show();
    //        }
    //        else {
    //            $("#divClientSiteID").hide();
    //        }
    //        $('#bitSiteDetailsDialog').dataBind(data.d);

    //    });
    //    $('#bitSiteDetailsDialog').dialog('open');
    //},

    //saveSite: function () {
    //    var validation = $('#bitSiteDetailsDialog').validate();
    //    if (validation) {
    //        //get data from panel
    //        BITAUTHSITE.site = $('#bitSiteDetailsDialog').collectData(BITAUTHSITE.site);

    //        jsonstring = convertToJsonString(BITAUTHSITE.site);
    //        BITAJAX.callWebService("SaveSite", jsonstring, function (data) {
    //            $('#bitSiteDetailsDialog').dialog("close");
    //            BITAUTHSITE.loadSites();
    //            if (BITAUTHSITE.site.IsNew) {
    //                $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, "De site is bewaard. Standaard gebruikersgroepen en de administrator zijn automatisch aangemakt. Zie email voor de wachtwoorden.");
    //            }
    //        });
    //    }
    //},

    //removeSite: function (id) {
    //    $.deleteConfirmBox("Wilt u deze site verwijderen?", null, function () {
    //        var parametersObject = { id: id };
    //        var jsonstring = JSON.stringify(parametersObject);
    //        BITAJAX.callWebService("DeleteSite", jsonstring, function (data) {
    //            BITAUTHSITE.loadSites();
    //        });
    //    });
    //},

    selectSite: function (id, url) {
        //geen eigen database
        //var useOwnDatabase = false;
        //if (useOwnDatabase) {
        //    //TODO: ingelogde gebruiker doorgeven, zodat die niet opnieuw hoeft in te loggen 
        //    //bij licentie database maak hash aan voor ingelogde 

        //    var siteUrl = url;
        //    location.href = siteUrl + "/_bitplate/Default.aspx";
        //}
        //else {
            var parametersObject = { id: id, url: url };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("ChangeSite", jsonstring, function (data) {
                location.href = data.d;
            });
        //}
        //alert("2do: navigeer naar deze site en laadt bitplate van deze site");
    },

    //changeLicense: function (select) {
    //    var licenseId = $(select).val();
    //    var parametersObject = { id: licenseId };
    //    var jsonstring = JSON.stringify(parametersObject);
    //    BITAJAX.callWebService("GetLicense", jsonstring, function (data) {
    //        var license = data.d;
    //        if (license.DomainName != "" && license.DomainName != "(meerdere sites)") {
    //            var siteName = license.DomainName.replace("http://", "");
    //            siteName = siteName.replace("www", "");
    //            $("#textName").val(siteName);
    //            $("#textUrl").val(license.DomainName);
    //        }
    //        $("#hiddenLicenseCode").val(license.Code);
    //        $("#textServerName").val(license.IPAddress);
    //        $(".selectOwner").val(license.Owner.ID);
    //        $("#textSiteAdminEmail").val(license.Owner.Email);
            
    //    });
    //}
}

