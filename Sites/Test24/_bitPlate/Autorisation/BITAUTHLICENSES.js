

//bestand voor licentie license
var BITAUTHLICENSES = {
    currentSort: "Name ASC",
    searchString: "",
    license: null,
    company: null,
    currentPage: 1,
    pageSize: 25,
    defaultPermissionSets: [],
    defaultValues: [],

    initialize: function () {
        //jQuery.support.cors = true; //
        BITAJAX.dataServiceUrl = "AuthService.aspx";
        $('#bitLicenseDetailsDialog').initDialog(BITAUTHLICENSES.saveLicense);

        $('#bitPopups').formEnrich();
        $('#tabPageDetails2').dockable(); //rechtentab in- en uitklapbaar maken 

        $('#bitSearchTextbox').searchable(BITAUTHLICENSES.search);
        this.loadCompanies();
        this.attachCheckBoxClickEvents(); //voor echten tab
    },

    attachCheckBoxClickEvents: function () {
        $(".checkParent").click(function (event) {
            event.stopPropagation(); //om te voorkomen dat dockpanel weer dichtklapt
            var value = $(this).val();
            var checked = $(this).is(":checked");
            if (checked) {
                $("#bitPermissionsPanelLicense" + value).removeAttr("disabled");
            }
            else {
                $("#bitPermissionsPanelLicense" + value).attr("disabled", "disabled");
                $("#bitPermissionsPanelLicense" + value).find("input[type=checkbox]").removeAttr("checked");
            }
        });

        $(".checkAll").click(function () {
            var value = $(this).val();
            var checked = $(this).is(":checked");
            if (checked) {
                $("#bitPermissionsPanelLicense" + value).find("input[type=checkbox]").attr("checked", "checked");
                $("#bitPermissionsPanelLicense" + value).find("div").removeAttr("disabled");
            }
            else {
                $("#bitPermissionsPanelLicense" + value).find("div").attr("disabled", "disabled");
                $("#bitPermissionsPanelLicense" + value).find("input[type=checkbox]").removeAttr("checked");
            }
        });
    },

    loadCompanies: function () {
        var parametersObject = {
            resellerId: null,
            sort: "Name Asc",
            searchString: ""
        };

        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.callWebService("GetCompanies", jsonstring, function (data) {
            $("#selectCompany").fillDropDown(data.d);
            //als die terug komt van nieuwe klant dialog, meteen klant selecteren
            if (this.company != null) {
                $("#selectCompany").val(this.company.Name);
            }
            $("#selectCompanyFilter").fillDropDown(data.d, { value: "", text: "Alle" });
        });
    },

    loadLicenses: function (companyId, sort, searchString) {
        if (sort != undefined) BITAUTHLICENSES.currentSort = sort;
        //if (pagenumber != undefined) BITAUTHLICENSES.currentPage = pagenumber;
        if (!companyId) companyId = "";
        if (!searchString) searchString = "";
        //if (searchString) BITAUTHLICENSES.searchString = searchString;

        var parametersObject = {
            companyId: companyId,
            sort: BITAUTHLICENSES.currentSort,
            searchString: searchString
        };

        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.callWebService("GetLicenses", jsonstring, function (data) {
            $("#tableLicenses").dataBindList(data.d, {
                onSort: function (sort) {
                    var companyId = $("#selectCompanyFilter").val();
                    BITAUTHLICENSES.loadLicenses(companyId, sort);
                }
            });
        });
    },

    filterOnCompanies: function (select) {
        var companyId = $(select).val();
        BITAUTHLICENSES.loadLicenses(companyId);
    },

    search: function (searchText) {
        var companyId = $("#selectCompanyFilter").val();
        BITAUTHLICENSES.loadLicenses(companyId, "", searchText);
    },


    newLicense: function () {

        BITAUTHLICENSES.openLicenseDetailsPopup(null);
    },


    openLicenseDetailsPopup: function (id) {
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetLicense", jsonstring, function (data) {
            BITAUTHLICENSES.license = data.d;

            $('#bitLicenseDetailsDialog').dataBind(data.d);

            BITAUTHLICENSES.changeMultipleSites($("#checkMultipleSites"));

            var permissions = BITAUTHLICENSES.license.Permissions.split(",");
            //eerst alle uitzetten
            $('#tabPageDetails2').find("input[type=checkbox]").removeAttr("checked");
            //dan per permission aanzetten
            for (var i in permissions) {
                var funcNumber = permissions[i];
                $("input[type=checkbox][value=" + funcNumber + "]").attr("checked", "checked");
                $("#bitPermissionsPanelLicense" + funcNumber).removeAttr("disabled");
            }

            //titel zetten
            if (BITAUTHLICENSES.license.IsNew) {
                $('#bitLicenseDetailsDialog').dialog({ title: "Nieuwe licentie" });

                $('.bitTabs').tabs("select", 0);
                $('#textNaam').focus();
            }
            else {
                $('#bitLicenseDetailsDialog').dialog({ title: "Licentie: " + BITAUTHLICENSES.license.Name });

            }
        });
        $('#bitLicenseDetailsDialog').dialog('open');
    },

    setDefaults: function () {
        var licenseType = $('#selectLicenseType option:selected').val();
        var newsletterLicenseType = $('#selectNewsletterLicenseType option:selected').val();
        var webshopLicenseType = $('#selectWebshopLicenseType option:selected').val();
        //eerst alle uitzetten
        $('#tabPageDetails2').find("input[type=checkbox]").removeAttr("checked");
        //daarna per type aanzetten
        this.bindDefaults(licenseType);
        this.bindDefaults(newsletterLicenseType);
        this.bindDefaults(webshopLicenseType);
    },

    bindDefaults: function (licenseType) {
        if (BITAUTHLICENSES.defaultPermissionSets[licenseType]) {
            var defaultPermissions = BITAUTHLICENSES.defaultPermissionSets[licenseType];

            for (var i in defaultPermissions) {
                var funcID = defaultPermissions[i].ID;
                $("input[type=checkbox][value=" + funcID + "]").attr("checked", "checked");
                $("#bitPermissionsPanelLicense" + funcID).removeAttr("disabled");
            }
        }
        if (BITAUTHLICENSES.defaultValues[licenseType]) {
            var defaults = BITAUTHLICENSES.defaultValues[licenseType];
            $("input[type=text][data-field=MaxNumberOfSites]").val(defaults.MaxNumberOfSites);
            $("input[type=text][data-field=MaxNumberOfPages]").val(defaults.MaxNumberOfPages);
            $("input[type=text][data-field=MaxNumberOfPageFolders]").val(defaults.MaxNumberOfPageFolders);
            $("input[type=text][data-field=MaxNumberOfTemplates]").val(defaults.MaxNumberOfTemplates);
            $("input[type=text][data-field=MaxNumberOfScripts]").val(defaults.MaxNumberOfScripts);
            $("input[type=text][data-field=MaxNumberOfStylesheets]").val(defaults.MaxNumberOfStylesheets);
            $("input[type=text][data-field=MaxNumberOfNewsletters]").val(defaults.MaxNumberOfNewsletters);
        }
    },

    saveLicense: function () {
        var validation = $('#bitLicenseDetailsDialog').validate();
        if (validation) {
            //get data from panel
            BITAUTHLICENSES.license = $('#bitLicenseDetailsDialog').collectData(BITAUTHLICENSES.license);
            var permissions = [];
            $('#tabPageDetails2').find('input[type=checkbox]').each(function (i) {
                var checked = $(this).is(":checked");
                if (checked) {
                    //var functionality = new Object();
                    //functionality.ID = $(this).val();
                    //permissions.push(functionality);
                    var cssClass = $(this).attr("class");
                    if (cssClass != "checkAll") {
                        var number = $(this).val();
                        permissions.push(number);
                    }
                }
            });
            BITAUTHLICENSES.license.Permissions = permissions;

            var propList = "";
            //var dummyLicObj = new Object()
            for (var prop in BITAUTHLICENSES.license) {
                if (prop == "__type") {
                }
                else if (prop == "Owner") {
                    propList += "FK_Company=" + BITAUTHLICENSES.license[prop].ID + ";";
                    //dummyLicObj["FK_Company"] = BITAUTHLICENSES.license[prop].ID;
                }
                else {
                    propList += prop + "=" + BITAUTHLICENSES.license[prop] + ";";
                    //dummyLicObj[prop] = BITAUTHLICENSES.license[prop];
                }
            }
            BITAUTHLICENSES.license.TempPropertyList = propList;
            //BITAUTHLICENSES.license.Name = "";
            //BITAUTHLICENSES.license.TempPropertyList = "";
            //BITAUTHLICENSES.license.LicenseFile = "";
            //BITAUTHLICENSES.license.TempPropertyList = JSON.stringify(dummyLicObj);
            //jsonstring = convertToJsonString(BITAUTHLICENSES.license);
            //BITAUTHLICENSES.license.TempPropertyList = jsonstring;
            jsonstring = convertToJsonString(BITAUTHLICENSES.license);
            BITAJAX.callWebService("SaveLicense", jsonstring, function (data) {
                $('#bitLicenseDetailsDialog').dialog("close");
                BITAUTHLICENSES.loadLicenses();
            });
        }
    },

    removeLicense: function (id) {
        $.deleteConfirmBox("Wilt u deze licentie verwijderen?", null, function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteLicense", jsonstring, function (data) {
                BITAUTHLICENSES.loadLicenses();
            });
        });
    },

    newCompany: function () {
        //eerst html laden
        var test = $("#bitCompanyDetailsDialogWrapper").html();
        if ($("#bitCompanyDetailsDialogWrapper").html() == "") {
            $("#bitCompanyDetailsDialogWrapper").load("Companies.aspx #bitCompanyDetailsDialog", function () {
                $('#bitCompanyDetailsDialog').initDialog(BITAUTHLICENSES.saveCompany);
                $('#bitCompanyDetailsDialogWrapper').formEnrich();
                BITAUTHLICENSES.openCompanyDetailsPopup(null);
            });
        }
        else {
            BITAUTHLICENSES.openCompanyDetailsPopup(null);
        }
    },

    openCompanyDetailsPopup: function (id) {

        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetCompany", jsonstring, function (data) {
            BITAUTHLICENSES.company = data.d;

            $('#bitCompanyDetailsDialog').dataBind(data.d);
            if (BITAUTHLICENSES.company.IsNew) {
                $('#bitCompanyDetailsDialog').dialog({ title: "Nieuwe klant" });

                $('.bitTabs').tabs("select", 0);
                $('#textNaam').focus();
            }
            else {
                $('#bitCompanyDetailsDialog').dialog({ title: "Klant: " + BITAUTHLICENSES.company.Name });

            }
        });
        $('#bitCompanyDetailsDialog').dialog('open');

    },

    saveCompany: function () {
        var validation = $('#bitCompanyDetailsDialog').validate();
        if (validation) {
            //get data from panel
            BITAUTHLICENSES.company = $('#bitCompanyDetailsDialog').collectData(BITAUTHLICENSES.company);

            jsonstring = convertToJsonString(BITAUTHLICENSES.company);
            BITAJAX.callWebService("SaveCompany", jsonstring, function (data) {
                $('#bitCompanyDetailsDialog').dialog("close");
                BITAUTHLICENSES.loadCompanies();
            });
        }
    },

    changeMultipleSites: function (checkbox) {
        var checked = $(checkbox).is(":checked");
        if (checked) {
            $("#textMaxNumberOfSites").removeAttr("disabled");
            //$("#textUrl").attr("disabled", "disabled");
            //$("#textUrl").removeAttr("data-validation");
            //$("#textUrl").val("(meerdere sites)");
            //$("#textUrl").next('span.validationMsg').remove();
        }
        else {
            $("#textMaxNumberOfSites").attr("disabled", "disabled");
            //$("#textUrl").removeAttr("disabled");
            //$("#textUrl").attr("data-validation", "required");
            //if ($("#textUrl").val() == "(meerdere sites)") {
            //    $("#textUrl").val("");
            //}
            $("#textMaxNumberOfSites").val("");
        }
    }

}

