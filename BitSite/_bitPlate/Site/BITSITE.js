var BITSITE = {
    site: null,
    currentEnvironment: null,

    initialize: function () {
        BITSITE.loadScripts();
        BITSITE.loadSiteConfig();

        $('#environmentDialog').initDialog(BITSITE.saveEnvironment, { width: 1000, height: 700 });
        $('#bitScriptDialog').initDialog(null, { width: 200, height: 400 }, false);
        $('#environmentDialog').formEnrich();
        $('#panelDetails').formEnrich();
    },

    saveSiteConfig: function () {
        if ($('#panelDetails').validate()) {
            BITSITE.site = $('#panelDetails').collectData(BITSITE.site);


            var collumns = $('input[name="defaultLanguage"]:checked').parent().parent().find('td');
            var languageName = $(collumns[1]).html();
            //languageName = $('input[name=defaultLanguage]:checked').val()

            BITSITE.site.DefaultLanguage = languageName;

            jsonstring = convertToJsonString(BITSITE.site);
            BITAJAX.dataServiceUrl = "/_bitplate/Site/SiteService.asmx";
            BITAJAX.callWebService("SaveSite", jsonstring, function (data) {
                BITSITE.site = data.d;
                $('#tableLanguages').dataBindList(data.d.Languages);
                
                if (BITSITE.site.IsValidLicense) {
                    //bericht 'ongeldige licentie' weghalen
                    $("#bitInvalidLicenseMessage").hide();
                    $.messageBox(MSGBOXTYPES.INFO, 'Uw wijzigingen zijn opgeslagen.', 'Instellingen opgeslagen.');
                }
                else {
                    $("#bitInvalidLicenseMessage").show();
                    $.messageBox(MSGBOXTYPES.INFO, 'Uw wijzigingen zijn opgeslagen. De ingevoerde licentie is ongeldig.', 'Instellingen opgeslagen. (licentie ongeldig)');
                }
                BITSITE.selectDefaultLanguage();
            });
        }
    },

    loadSiteConfig: function () {
        $('#panelDetails').formEnrich();
        BITAJAX.dataServiceUrl = "/_bitplate/Site/SiteService.asmx";
        BITAJAX.callWebService("LoadSite", null, function (data) {
            $('#panelDetails').dataBind(data.d);
            BITSITE.site = data.d;
            $('#tableLanguages').dataBindList(data.d.Languages);
            $('#tableEnvironments').dataBindList(data.d.Environments);
            $("#tableScriptsPerSite").dataBindList(data.d.Scripts);
            if (!BITSITE.site.IsValidLicense) {
                //bericht 'ongeldige licentie' weghalen
                $("#bitInvalidLicenseMessage").show();
            }

            if (BITSITE.site.DefaultLanguage == null || BITSITE.site.DefaultLanguage == "") {
                if (BITSITE.site.Languages.length > 0) {
                    BITSITE.site.DefaultLanguage = BITSITE.site.Languages[0].LanguageCode;
                }
                else {
                    BITSITE.site.DefaultLanguage = "nl";
                }
            }
            BITSITE.selectDefaultLanguage();
        });
    },

    selectDefaultLanguage: function () {
        $('#tableLanguages').find('tr').each(function () {
            var collums = $(this).find('td');
            var languageName = $(collums[1]).html();
            if (languageName == BITSITE.site.DefaultLanguage) {
                $(collums[0]).find('input').attr('checked', 'checked');
            }
        });
    },

    addLanguage: function () {
        var cmsLanguage;
        BITAJAX.dataServiceUrl = "/_bitplate/Site/SiteService.asmx";
        BITAJAX.callWebService("AddLanguage", null, function (data) {
            cmsLanguage = data.d;
            BITAJAX.callWebService("GetAllLanguages", null, function (data) {
                $('#languages').fillDropDown(data.d);
                $('#dialoglanguages').initDialog(null, { height: 200, width: 300 }, true, {
                    "Toevoegen": function () {
                        cmsLanguage.Name = $('#languages option:selected').text();
                        cmsLanguage.LanguageCode = $('#languages').val();
                        if (BITSITE.checkIfLanguageIsUnique(cmsLanguage)) {
                            BITSITE.site.Languages.push(cmsLanguage);
                            $('#tableLanguages').dataBindList(BITSITE.site.Languages);
                            BITSITE.selectDefaultLanguage();
                            $('#dialoglanguages').dialog('close');
                        }
                        else {
                            $.messageBox(MSGBOXTYPES.WARNING, 'Deze taal bestaat al. De taal moet uniek zijn', 'Controle');
                        }
                    },
                    "Annuleren": function () {
                        $('#dialoglanguages').dialog('close');
                    }
                });
                $('#dialoglanguages').dialog('open');
            });
        });
    },
    checkIfLanguageIsUnique: function (languageObj) {
        var returnValue = true;
        for (var i in BITSITE.site.Languages) {
            var lang = BITSITE.site.Languages[i];
            if (lang.LanguageCode == languageObj.LanguageCode) {
                returnValue = false;
            }
        }
        return returnValue;
    },

    deleteLanguage: function (id, index) {
        if (BITSITE.site.Languages[index].LanguageCode == BITSITE.site.DefaultLanguage) {
            $.messageBox(MSGBOXTYPES.WARNING, 'Kan de standaard taal niet verwijderen!', 'Fout');
        }
        else {
            $.deleteConfirmBox('Weet u zeker dat u deze taal wilt verwijderen?', 'Taal verwijderen?', function () {

                //BITSITE.site.Languages.splice(index, 1);
                //$('#tableLanguages').dataBindList(BITSITE.site.Languages);
                //BITSITE.selectDefaultLanguage();
                var parametersObject = { languageId: id };
                var jsonstring = JSON.stringify(parametersObject);
                BITAJAX.dataServiceUrl = "/_bitplate/Site/SiteService.asmx";
                BITAJAX.callWebService("DeleteLanguage", jsonstring, function (data) {
                    BITSITE.site.Languages = data.d;
                    $('#tableLanguages').dataBindList(data.d);
                    BITSITE.selectDefaultLanguage();
                });
            });
        }
    },


    addEnvironment: function () {
        var environment = new Object();
        environment.IsNew = true;
        //zet defaults
        //eerst laatste waardes ophalen van formulier
        BITSITE.site = $('#panelDetails').collectData(BITSITE.site);
        environment.EmailSettingsHost = BITSITE.site.EmailSettingsHost;
        environment.EmailSettingsFrom = BITSITE.site.EmailSettingsFrom;
        environment.EmailSettingsUser = BITSITE.site.EmailSettingsUser;
        environment.EmailSettingsPassword = BITSITE.site.EmailSettingsPassword;

        BITSITE.openEnvironmentPopup(environment);
    },

    //addEnvironment_old: function () {
    //    $('#environmentDialog').initDialog(function () {
    //        if ($('#environmentDialog').validate()) {
    //            var environment = new Object();
    //            environment = $('#environmentDialog').collectData(environment);
    //            var jsonstring = JSON.fromObject(environment);
    //            BITAJAX.dataServiceUrl = "bitAjaxServices/SiteService.asmx";
    //            BITAJAX.callWebService("SaveEnvironment", jsonstring, function (data) {
    //                if (data.d.Success) {
    //                    $('#tableEnvironments').dataBindList(data.d.DataObject);
    //                    BITSITE.site.Environments = data.d.DataObject;
    //                    $('#environmentDialog').dialog('close');
    //                }
    //                else {
    //                    $.messageBox(MSGBOXTYPES.WARNING, 'Kan environment niet aanmaken:<br />' + data.d.Message, 'Fout');
    //                }
    //            });
    //        }
    //    }, null, true);
    //    $('#environmentDialog').formEnrich();
    //    $('#environmentDialog').dataBind(BITSITE.site);

    //    $('#environmentDialog').dialog({ title: 'Nieuwe omgeving.' });
    //    $('#environmentDialog .bitTabs').tabs('select', '0');
    //    $('#environmentDialog').dialog('open');
    //},

    editEnvironment: function (index) {
        var environment = BITSITE.site.Environments[index];
        BITSITE.openEnvironmentPopup(environment);
    },

    //editEnvironment_old: function (index) {
    //    var environment = BITSITE.site.Environments[index];
    //    $('#environmentDialog').initDialog(function () {
    //        if ($('#environmentDialog').validate()) {

    //            environment = $('#environmentDialog').collectData(environment);
    //            var jsonstring = JSON.fromObject(environment);
    //            BITAJAX.dataServiceUrl = "bitAjaxServices/SiteService.asmx";
    //            BITAJAX.callWebService("SaveEnvironment", jsonstring, function (data) {
    //                if (data.d.Success) {
    //                    $('#tableEnvironments').dataBindList(data.d.DataObject);
    //                    BITSITE.site.Environments = data.d.DataObject;
    //                    $('#environmentDialog').dialog('close');
    //                }
    //                else {
    //                    $.messageBox(MSGBOXTYPES.WARNING, 'Kan omgeving niet aanmaken:<br />' + data.d.Message, 'Fout');
    //                }
    //            });
    //        }
    //    }, null, true);
    //    $('#environmentDialog').formEnrich();
    //    //$('#environmentDialog .bitTabs').tabs('select', '0');
    //    $('#environmentDialog').dataBind(environment);

    //    $('#environmentDialog').dialog({ title: 'Bewerk omgeving: ' + environment.Name });
    //    $('#environmentDialog').dialog('open');
    //},

    openEnvironmentPopup: function (environment) {
        BITSITE.currentEnvironment = environment;
        $('#environmentDialog').dataBind(environment);
        if (environment.IsNew) {
            $('#environmentDialog').dialog({ title: 'Nieuwe omgeving.' });
        }
        else {
            if (BITSITE.site.CurrentWorkingEnvironment.ID == BITSITE.currentEnvironment.ID) {
                $('#environmentDialog').dialog({ title: 'Bewerk omgeving: ' + environment.Name + ' (huidig)' });
                $('#selectSiteEnvironmentType').attr('disabled', 'disabled');
                $('#tabEnvironmentDetails2 input').attr('disabled', 'disabled');
                $('#tabEnvironmentDetails3 input').attr('disabled', 'disabled');
            }
            else {
                $('#environmentDialog').dialog({ title: 'Bewerk omgeving: ' + environment.Name });
                $('#selectSiteEnvironmentType').removeAttr('disabled');
                $('#tabEnvironmentDetails2 input').removeAttr('disabled');
                $('#tabEnvironmentDetails3 input').removeAttr('disabled');
            }
        }
        $('#environmentDialog').dialog('open');
    },

    saveEnvironment: function () {
        if ($('#environmentDialog').validate()) {
            var environment = BITSITE.currentEnvironment;
            environment = $('#environmentDialog').collectData(environment);
            var jsonstring = JSON.fromObject(environment);
            BITAJAX.dataServiceUrl = "/_bitplate/Site/SiteService.asmx";
            BITAJAX.callWebService("SaveEnvironment", jsonstring, function (data) {
                if (data.d.Success) {
                    $('#tableEnvironments').dataBindList(data.d.DataObject);
                    BITSITE.site.Environments = data.d.DataObject;
                    $('#environmentDialog').dialog('close');
                }
                else {
                    $.messageBox(MSGBOXTYPES.WARNING, 'Kan omgeving niet aanmaken:<br />' + data.d.Message, 'Fout');
                }
            });
        }
    },

    deleteEnvironment: function (index) {
        var environment = BITSITE.site.Environments[index];
        $.deleteConfirmBox('Weet u zeker dat u deze omgeving: ' + environment.Name + ' wilt verwijderen?', 'Omgeving verwijderen?', function () {
            var jsonstring = JSON.fromObject(environment);
            BITAJAX.dataServiceUrl = "/_bitplate/Site/SiteService.asmx";
            BITAJAX.callWebService("DeleteEnvironment", jsonstring, function (data) {
                $('#tableEnvironments').dataBindList(data.d.DataObject);
                BITSITE.site.Environments = data.d.DataObject;
            });
        });
    },
    ///////////////////////////////
    //SCRIPTS
    ///////////////////////////////
    loadScripts: function () {
        BITAJAX.dataServiceUrl = "/_bitplate/Scripts/ScriptService.asmx";
        BITAJAX.callWebService("LoadScripts", null, function (data) {
            $('#bitScriptList').dataBindList(data.d);
        });
    },

    openScriptsPopup: function () {
        $('#bitScriptDialog').dialog('open');
    },

    addScript: function (id, name) {
        var script = new Object();
        script.ID = id;
        script.CompleteName = name;
        script.LoadInWholeSite = true;

        if (BITSITE.site.Scripts == null) {
            BITSITE.site.Scripts = [];
        }
        //kijk of die er al in zit
        var contains = false;
        for (var i in BITSITE.site.Scripts) {
            var scriptCheck = BITSITE.site.Scripts[i];
            if (scriptCheck.ID == script.ID) {
                contains = true;
                break;
            }
        }
        if (contains) {
            alert("Koppeling met dit script bestaat al");
        }
        else {
            BITSITE.site.Scripts.push(script);
            $("#tableScriptsPerSite").dataBindList(BITSITE.site.Scripts);
            
            BITSITE.setScriptInWholeSite(id, true);

        }
    },

    removeScript: function (id, index) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u dit script wilt los koppelen?", null, function () {
            BITSITE.site.Scripts.splice(index, 1);
            $("#tableScriptsPerSite").dataBindList(BITSITE.site.Scripts);

            var script = new Object();
            script.ID = id;
            script.LoadInWholeSite = false;
            BITSITE.setScriptInWholeSite(id, false);
        });
    },

    setScriptInWholeSite: function (id, loadInWholeSite) {
        BITAJAX.dataServiceUrl = "/_bitplate/Site/SiteService.asmx";
        var parametersObject = { scriptid: id, loadInWholeSite: loadInWholeSite };
        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.callWebService("SetScriptInWholeSite", jsonstring, function (data) {
            return true;
        });
    }
};