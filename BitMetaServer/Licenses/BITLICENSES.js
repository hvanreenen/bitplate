

//bestand voor licentie license
var BITLICENSES = {
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
        BITAJAX.dataServiceUrl = "LicenseService.aspx";
        $('#bitLicenseDetailsDialog').initDialog(BITLICENSES.saveLicense);
        $('#LicensedEnvironmentDialog').initDialog(BITLICENSES.saveLicensedEnvironment, { width: 600, height: 500 });
        $('#newSiteWizzard').initDialog(BITLICENSES.startNewSiteWizzard, { width: 600, height: 600 }, true, {
            "Nieuwe site & database aanmaken": BITLICENSES.startNewSiteWizzard,
            "Alleen opslaan": BITLICENSES.saveNewLicensedEnvironment,
            "Annuleer": function () {
                $(this).dialog('close');
            }
        });
        $('#bitPopups').formEnrich();
        $('#tabPageDetails3').dockable(); //rechtentab in- en uitklapbaar maken 

        $('#bitSearchTextbox').searchable(BITLICENSES.search);
        $('#bitSearchTextbox').attr('placeholder', 'Zoek op naam/code/url...');
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
        if (sort != undefined) BITLICENSES.currentSort = sort;
        //if (pagenumber != undefined) BITLICENSES.currentPage = pagenumber;
        if (!companyId) companyId = "";
        if (!searchString) searchString = "";
        //if (searchString) BITLICENSES.searchString = searchString;

        var parametersObject = {
            companyId: companyId,
            sort: BITLICENSES.currentSort,
            searchString: searchString
        };

        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.callWebService("GetLicenses", jsonstring, function (data) {
            $("#tableLicenses").dataBindList(data.d, {
                onSort: function (sort) {
                    var companyId = $("#selectCompanyFilter").val();
                    BITLICENSES.loadLicenses(companyId, sort);
                }
            });
        });
    },

    filterOnCompanies: function (select) {
        var companyId = $(select).val();
        BITLICENSES.loadLicenses(companyId);
    },

    search: function (searchText) {
        var companyId = $("#selectCompanyFilter").val();
        BITLICENSES.loadLicenses(companyId, "", searchText);
    },


    newLicense: function () {

        BITLICENSES.openLicenseDetailsPopup(null);
    },


    openLicenseDetailsPopup: function (id) {
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetLicense", jsonstring, function (data) {
            BITLICENSES.license = data.d;
            $('#bitLicenseDetailsDialog .bitTabs').tabs({
                activate: function (event, ui) {
                    //statistics apprt laden, want maakt verbinding met andere database en haalt dan gegevens op.
                    if (ui.newTab.index() == 3) //tab met statistics
                    {
                        BITLICENSES.retrieveStatistics(BITLICENSES.license.ID);
                    }
                }
            });
            $('#bitLicenseDetailsDialog').dataBind(data.d);
            BITLICENSES.bindLicensedEnvironments(BITLICENSES.license);

            //Rechten tab vullen
            //eerst alle uitzetten
            $('#tabPageDetails3').find("input[type=checkbox]").removeAttr("checked");
            $('.bitPermissionsPanelLevel1').attr("disabled", "disabled");

            //dan per permission aanzetten
            var functionNumbers = BITLICENSES.license.Permissions;
            if (functionNumbers) {
                for (var i in functionNumbers) {
                    var funcNumber = functionNumbers[i];
                    $("input[type=checkbox][value=" + funcNumber + "]").attr("checked", "checked");
                    $("#bitPermissionsPanelLicense" + funcNumber).removeAttr("disabled");
                }
            }

            //titel zetten
            if (BITLICENSES.license.IsNew) {
                $('#bitLicenseDetailsDialog').dialog({ title: "Nieuwe licentie" });
                BITLICENSES.setDefaults();
                $('.bitTabs').tabs("select", 0);
                $('#textNaam').focus();
            }
            else {
                $('#bitLicenseDetailsDialog').dialog({ title: "Licentie: " + BITLICENSES.license.Name });

            }
        });
        $('#bitLicenseDetailsDialog').dialog('open');
    },

    retrieveStatistics: function (licenseID) {
        if (licenseID == BITUTILS.EMPTYGUID) return; //bij nieuwe 
        var parametersObject = { id: licenseID };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetStatisticsByLicense", jsonstring, function (data) {
            var html = data.d;
            $("#divStats").html(html);
        });
    },

    setDefaults: function () {
        var licenseType = $('#selectLicenseType option:selected').val();
        var newsletterLicenseType = $('#selectNewsletterLicenseType option:selected').val();
        var webshopLicenseType = $('#selectWebshopLicenseType option:selected').val();
        //eerst alle uitzetten
        $('#tabPageDetails3').find("input[type=checkbox]").removeAttr("checked");
        //daarna per type aanzetten
        this.bindDefaults(licenseType);
        this.bindDefaults(newsletterLicenseType);
        this.bindDefaults(webshopLicenseType);
    },

    bindDefaults: function (licenseType) {
        if (BITLICENSES.defaultPermissionSets[licenseType]) {
            var defaultPermissions = BITLICENSES.defaultPermissionSets[licenseType];

            for (var i in defaultPermissions) {
                var funcNumber = defaultPermissions[i];
                $("input[type=checkbox][value=" + funcNumber + "]").attr("checked", "checked");
                $("#bitPermissionsPanelLicense" + funcNumber).removeAttr("disabled");
            }
        }
        if (BITLICENSES.defaultValues[licenseType]) {
            var defaults = BITLICENSES.defaultValues[licenseType];
            $("input[type=text][data-field='MaxNumberOfSites']").val(defaults.MaxNumberOfSites);
            $("input[type=text][data-field='MaxNumberOfPages']").val(defaults.MaxNumberOfPages);
            $("input[type=text][data-field='MaxNumberOfPageFolders']").val(defaults.MaxNumberOfPageFolders);
            $("input[type=text][data-field='MaxNumberOfTemplates']").val(defaults.MaxNumberOfTemplates);
            $("input[type=text][data-field='MaxNumberOfScripts']").val(defaults.MaxNumberOfScripts);
            $("input[type=text][data-field='MaxNumberOfStylesheets']").val(defaults.MaxNumberOfStylesheets);
            $("input[type=text][data-field='MaxNumberOfNewsletters']").val(defaults.MaxNumberOfNewsletters);
        }
    },

    saveLicense: function () {
        var validation = $('#bitLicenseDetailsDialog').validate();
        if (validation) {
            if (BITLICENSES.license.Environments.length < 1) {
                validation = false;
                $.messageBox(MSGBOXTYPES.WARNING, 'Voor een licentie is tenminste 1 omgeving verplicht. Vul tenminste de url en het pad in van de CMS omgeving.', 'Fout');
            }
        }
        if (validation) {
            //get data from panel
            BITLICENSES.license = $('#bitLicenseDetailsDialog').collectData(BITLICENSES.license);
            var functionNumbers = [];
            $('#tabPageDetails3').find('input[type=checkbox]').not('.checkAll').each(function (i) {
                var checked = $(this).is(":checked");
                if (checked) {
                    var functionNumber = $(this).val();
                    functionNumbers.push(functionNumber);
                }
            });
            BITLICENSES.license.Permissions = functionNumbers;

            //var propsString = "";
            //for(var prop in BITLICENSES.license.Permissions){
            //    propsString += prop + "=" + BITLICENSES.license.Permissions[prop] + ";"
            //}
            //BITLICENSES.license.TempPermissionsString = propsString;
            jsonstring = convertToJsonString(BITLICENSES.license);
            BITAJAX.callWebService("SaveLicense", jsonstring, function (data) {
                $('#bitLicenseDetailsDialog').dialog("close");
                BITLICENSES.loadLicenses();
            });
        }
    },

    removeLicense: function (id) {
        $.deleteConfirmBox("Wilt u deze licentie verwijderen?", null, function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService("DeleteLicense", jsonstring, function (data) {
                BITLICENSES.loadLicenses();
            });
        });
    },

    newCompany: function () {
        //eerst html laden
        var test = $("#bitCompanyDetailsDialogWrapper").html();
        if ($("#bitCompanyDetailsDialogWrapper").html() == "") {
            $("#bitCompanyDetailsDialogWrapper").load("Companies.aspx #bitCompanyDetailsDialog", function () {
                $('#bitCompanyDetailsDialog').initDialog(BITLICENSES.saveCompany);
                $('#bitCompanyDetailsDialogWrapper').formEnrich();
                BITLICENSES.openCompanyDetailsPopup(null);
            });
        }
        else {
            BITLICENSES.openCompanyDetailsPopup(null);
        }
    },

    openCompanyDetailsPopup: function (id) {

        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("GetCompany", jsonstring, function (data) {
            BITLICENSES.company = data.d;

            $('#bitCompanyDetailsDialog').dataBind(data.d);
            if (BITLICENSES.company.IsNew) {
                $('#bitCompanyDetailsDialog').dialog({ title: "Nieuwe klant" });

                $('.bitTabs').tabs("select", 0);
                $('#textNaam').focus();
            }
            else {
                $('#bitCompanyDetailsDialog').dialog({ title: "Klant: " + BITLICENSES.company.Name });

            }
        });
        $('#bitCompanyDetailsDialog').dialog('open');

    },

    saveCompany: function () {
        var validation = $('#bitCompanyDetailsDialog').validate();
        if (validation) {
            //get data from panel
            BITLICENSES.company = $('#bitCompanyDetailsDialog').collectData(BITLICENSES.company);

            jsonstring = convertToJsonString(BITLICENSES.company);
            BITAJAX.callWebService("SaveCompany", jsonstring, function (data) {
                $('#bitCompanyDetailsDialog').dialog("close");
                BITLICENSES.loadCompanies();
            });
        }
    },

    changeMultipleSites: function (checkbox) {
        var checked = $(checkbox).is(":checked");
        if (checked) {
            $("#textMaxNumberOfSites").removeAttr("disabled");
            $("#textUrl").attr("disabled", "disabled");
            $("#textUrl").removeAttr("data-validation");
            $("#textUrl").val("(meerdere sites)");
            $("#textUrl").next('span.validationMsg').remove();
        }
        else {
            $("#textMaxNumberOfSites").attr("disabled", "disabled");
            $("#textUrl").removeAttr("disabled");
            $("#textUrl").attr("data-validation", "required");
            if ($("#textUrl").val() == "(meerdere sites)") {
                $("#textUrl").val("");
            }
            $("#textMaxNumberOfSites").val("");
        }
    },

    /////////////////////////
    // CMS OMGEVINGEN
    /////////////////////////
    bindLicensedEnvironments: function () {
        $('#tableLicensedEnvironments').dataBindList(BITLICENSES.license.Environments);
    },

    addLicensedEnvironment: function (licence) {
        var environment = new Object();
        environment.IsNew = true;
        BITLICENSES.openPopupNewSiteWizzard(environment);
    },

    editLicensedEnvironment: function (index, id) {

        var environment = BITLICENSES.license.Environments[index];
        environment.index = index;
        BITLICENSES.openPopupLicensedEnvironment(environment);
    },

    openPopupLicensedEnvironment: function (environment) {
        BITLICENSES.currentEnvironment = environment;
        $('#LicensedEnvironmentDialog').dataBind(environment);

        $('#LicensedEnvironmentDialog').dialog({ title: 'Bewerk CMS omgeving: ' + environment.Name });

        $('#LicensedEnvironmentDialog').dialog('open');
    },

    openPopupNewSiteWizzard: function (environment) {
        //eerst licentie valideren, want servername is verplicht
        var validation = $('#bitLicenseDetailsDialog').validate();

        if (validation) {
            BITLICENSES.license = $('#bitLicenseDetailsDialog').collectData(BITLICENSES.license);
            BITLICENSES.currentEnvironment = environment;
            $('#newSiteWizzard').dataBind(environment);

            $('#newSiteWizzard').dialog('open');
        }
        else {
            $.messageBox(MSGBOXTYPES.INFO, "Kan geen nieuwe omgeving aanmaken, want niet alle gegevens van de licentie zijn ingevuld (zoals code en servernaam).", "Waarschuwing");
        }
    },

    startNewSiteWizzard: function () {
        var validation = $('#newSiteWizzard').validate();

        if (validation) {
            //get data from panel
            BITLICENSES.currentEnvironment = $('#newSiteWizzard').collectData(BITLICENSES.currentEnvironment);
            BITLICENSES.currentEnvironment.License = BITLICENSES.license;
            jsonstring = convertToJsonString(BITLICENSES.currentEnvironment);
            BITAJAX.callWebService("CreateNewSite", jsonstring, function (data) {
                BITLICENSES.currentEnvironment = data.d;
                $('#newSiteWizzard').dialog('close');

                $.messageBox(MSGBOXTYPES.INFO, data.d.LogMsg, "Aanmaken nieuwe omgeving afgerond", function () {
                    $('#bitLicenseDetailsDialog').dialog("close");
                    BITLICENSES.loadLicenses();
                });
            });
        }
    },

    saveNewLicensedEnvironment: function () {
        if ($('#newSiteWizzard').validate()) {

            var environment = BITLICENSES.currentEnvironment;
            environment = $('#newSiteWizzard').collectData(environment);
            if (environment.IsNew) {
                BITLICENSES.license.Environments.push(environment);
            }
            else {
                var index = BITLICENSES.currentEnvironment.index;
                BITLICENSES.license.Environments[index] = environment;
            }
            BITLICENSES.bindLicensedEnvironments();
            $('#newSiteWizzard').dialog('close');
        }
    },

    saveLicensedEnvironment: function () {
        if ($('#LicensedEnvironmentDialog').validate()) {

            var environment = BITLICENSES.currentEnvironment;
            environment = $('#LicensedEnvironmentDialog').collectData(environment);
            if (environment.IsNew) {
                BITLICENSES.license.Environments.push(environment);
            }
            else {
                var index = BITLICENSES.currentEnvironment.index;
                BITLICENSES.license.Environments[index] = environment;
            }
            BITLICENSES.bindLicensedEnvironments();
            $('#LicensedEnvironmentDialog').dialog('close');
        }
    },


    deleteLicensedEnvironment: function (index, id) {
        $.deleteConfirmBox('Weet u zeker dat u deze omgeving wilt verwijderen? (u moet zelf database en pad op server verwijderen)', 'Omgeving verwijderen?', function () {
            BITLICENSES.license.Environments.splice(index, 1);
            BITLICENSES.bindLicensedEnvironments();
            if (id != "" && id != BITUTILS.EMPTYGUID) {
                //haal ook weg uit db
                var parametersObject = { id: id };
                var jsonstring = JSON.stringify(parametersObject);

                BITAJAX.callWebService("DeleteLicensedEnvironment", jsonstring, function (data) {
                    //    BITLICENSES.loadLicensedEnvironments(BITLICENSES.license.ID);
                });
            }
        });
    }

}