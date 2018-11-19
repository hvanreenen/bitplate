/// <reference path="../_js/jquery-1.7.2-vsdoc.js" />
/// <reference path="../_js/JSON.js" />
/// <reference path="../../_js/prototypes/databind.js" />
/// <reference path="../prototypes/initDialog.js" />

var BITMAILINGROUPS = {
    newsletterGroups: null,
    newsletter: null,
    currentPath: null,
    currentNewsletterGroup: null,
    currentNewsletters: null,
    SentFromAddress: null,

    initialize: function () {
        //BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITMAILINGROUPS.loadNewsletterMailingGroups();
        BITMAILINGROUPS.loadTemplates();
        
        
        $('#bitDivSearch').hide();

        $('#IsMandatoryGroupCheckBox').change(function () {
            BITMAILINGROUPS.updateNewsgroupDialogCheckboxes();
        });
    },

    updateNewsgroupDialogCheckboxes: function() {
        if (!$('#IsMandatoryGroupCheckBox').is(':checked')) {
            $('#IsChoosableGroupCheckBox').attr('disabled', false);
            $('#IsChoosableGroupCheckBox').removeClass('ui-state-disabled');
        }
        else {
            $('#IsChoosableGroupCheckBox').attr('checked', false);
            $('#IsChoosableGroupCheckBox').addClass('ui-state-disabled');
            $('#IsChoosableGroupCheckBox').attr('disabled', true);
        }
    },

    updateBreadCrump: function (folderPath) {
        var folderList = [];
        var rootFolderItem = new Object();
        rootFolderItem.Path = "";
        rootFolderItem.Name = "root";
        folderList.push(rootFolderItem);

        if (folderPath) {
            var folderNames = folderPath.split("/");
            var path = "";
            for (var i in folderNames) {
                var folderName = folderNames[i];
                if (folderName != "") {
                    if (path != "") {
                        path += '/'
                    }
                    path += folderName;
                    var folderItem = new Object();
                    folderItem.Path = path;
                    folderItem.Name = folderName;
                    folderList.push(folderItem);
                }
            }
        }
        $('#breadcrump').dataBindList(folderList);
        //<ul>list stylen naar breadcrump
        $('#breadcrump').jBreadCrumb();

    },

    loadNewsletters: function () {
        //BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebServiceASync("LoadNewsletters", null, function (data) {
            $("#tableNewsletters").dataBindList(data.d);
            BITMAILINGROUPS.currentNewsletters = data.d;
            $('#divWrapperMain').checkFunctionalityElements('newsletterList');
        });
    },

    loadNewsletterMailingGroup: function (index) {
        
        var newsletterGroup = BITMAILINGROUPS.newsletterGroups[index];
        BITMAILINGROUPS.currentNewsletterGroup = newsletterGroup;
        BITMAILINGROUPS.currentPath = newsletterGroup.Name;
        BITMAILINGROUPS.updateBreadCrump(BITMAILINGROUPS.currentPath);
        var jsonstring = JSON.fromObject(newsletterGroup);
        //BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebServiceASync("LoadNewsletterList", jsonstring, function (data) {
            $("#tableNewsletters").dataBindList(data.d);
            BITMAILINGROUPS.currentNewsletterGroup.Newsletters = data.d;
            $('#divWrapperMain').checkFunctionalityElements('newsletterList');
        });
        
    },

    //LoadNewsletters: function () {
    //    if (BITMAILINGROUPS.currentPath.indexOf('Nieuwsbrieven') == -1) {
    //        BITMAILINGROUPS.currentPath = BITMAILINGROUPS.currentPath + '/' + 'Nieuwsbrieven';
    //    }
    //    BITMAILINGROUPS.updateBreadCrump(BITMAILINGROUPS.currentPath);
    //    $("#tableNewsletters").dataBindList(BITMAILINGROUPS.currentNewsletterGroup.Newsletters);
    //    $('#divWrapperMain').checkFunctionalityElements('newsletterList');

    //},

    LoadSubscribers: function () {
        if (BITMAILINGROUPS.currentPath.indexOf('Abonnementen') == -1) {
            BITMAILINGROUPS.currentPath = BITMAILINGROUPS.currentPath + '/' + 'Abonnementen';
        }
        BITMAILINGROUPS.updateBreadCrump(BITMAILINGROUPS.currentPath);
        $("#tableNewsletterAbonnementen").dataBindList(BITMAILINGROUPS.currentNewsletterGroup.Subscribers);
        $('#divWrapperMain').checkFunctionalityElements('newsletterAbonnementen');
        $('#liAddAbbonement').show();
    },

    navigateFromBreadCrump: function (name) {
        $('#liAddAbbonement').hide();
        if (name == 'root') {
            BITMAILINGROUPS.loadNewsletterMailingGroups();
            BITMAILINGROUPS.currentNewsletterGroup = null;
        }
        else {
            if (name == 'Nieuwsbrieven' || name == 'Abonnementen') {
                if (name == 'Nieuwsbrieven') {
                    BITMAILINGROUPS.LoadNewsletters();
                }
                else {
                    BITMAILINGROUPS.LoadSubscribers();
                }
            }
            else {
                var NewsletterGroup = BITMAILINGROUPS.getNewsGroupIndexByName(name)
                BITMAILINGROUPS.loadNewsletterMailingGroup(NewsletterGroup);
            }
        }
    },

    getNewsGroupIndexByName: function (name) {
        var index = null;
        $(BITMAILINGROUPS.newsletterGroups).each(function (i) {
            if (this.Name == name) {
                index = i;
            }
        });
        return index;
    },

    loadNewsletterMailingGroups: function (sort) {
        if (!sort) sort = 'Name ASC';
        //var parametersObject = { id: id };
        //var jsonstring = JSON.stringify(parametersObject);
        BITMAILINGROUPS.updateBreadCrump('');
        //BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        var parameters = { sort: sort };
        var jsonstring = JSON.stringify(parameters);
        BITAJAX.callWebServiceASync("LoadNewsletterGroupList", jsonstring, function (data) {
        //BITAJAX.callWebServiceASync("LoadNewsletterGroupList", null, function (data) {
            BITMAILINGROUPS.newsletterGroups = data.d;
            $('#tableNewsletterGroupList').dataBindList(data.d, {
                onSort: function (sort) {
                    BITMAILINGROUPS.loadNewsletterMailingGroups(sort);// .loadPages(BITPAGES.selectedFolderId, BITPAGES.selectedPath, sort);
                }
            });
            $('#divWrapperMain').checkFunctionalityElements('newsletterGroupList');
        });
    },

    clearNewsletterGroupDialog: function() {
        $('#BITMAILINGROUPSGroupDialog input').val('');
        $('#BITMAILINGROUPSGroupDialog input[type="checkbox"]').attr('checked', false);
    },

    showAddNewsletterGroup: function () {
        //BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        $('#BITMAILINGROUPSGroupDialog').initDialog(function () {
            var NewsletterGroup = new Object();
            NewsletterGroup = $('#BITMAILINGROUPSGroupDialog').collectData(NewsletterGroup);
            if ($('#BITMAILINGROUPSGroupDialog').validate()) {
                var jsonstring = JSON.fromObject(NewsletterGroup);
                BITAJAX.callWebServiceASync("SaveNewsletterGroup", jsonstring, function (data) {
                    $('#BITMAILINGROUPSGroupDialog').dialog('close');

                    if (data.d.Success) {
                       // $('#tableNewsletterGroupList').dataBindList(data.d.DataObject[0].val);
                        //BITMAILINGROUPS.newsletterGroups = data.d.DataObject[0].val;
                        BITMAILINGROUPS.loadNewsletterMailingGroups()
                    }
                    else {
                        alert('Nieuwsgroep is niet toegevoegd! \r\n' + data.d.Message);
                    }
                });
            }
        }, { width: 700, height: 300 });
        BITMAILINGROUPS.clearNewsletterGroupDialog();
        BITMAILINGROUPS.updateNewsgroupDialogCheckboxes();
        $('#BITMAILINGROUPSGroupDialog').dialog('open');
    },

    editNewsletterGroup: function (i) {
        var newsletterGroup = BITMAILINGROUPS.newsletterGroups[i];
        $('#BITMAILINGROUPSGroupDialog').initDialog(function () {
            if ($('#BITMAILINGROUPSGroupDialog').validate()) {
                newsletterGroup = $('#BITMAILINGROUPSGroupDialog').collectData(newsletterGroup);
                var jsonstring = JSON.fromObject(newsletterGroup);
                //BITAJAX.dataServiceUrl = "NewsletterService.asmx";
                BITAJAX.dataServiceUrl = "NewsletterService.asmx";
                BITAJAX.callWebServiceASync("SaveNewsletterGroup", jsonstring, function (data) {
                    $('#BITMAILINGROUPSGroupDialog').dialog('close');
                    if (data.d.Success) {
                        //$('#tableNewsletterGroupList').dataBindList(data.d.DataObject);
                        //BITMAILINGROUPS.newsletterGroups = data.d.DataObject;
                        BITMAILINGROUPS.loadNewsletterMailingGroups()
                    }
                    else {
                        alert('Nieuwsgroep is niet gewijzigd.<br />' + data.d.Message);
                    }
                });
            }
        }, { width: 700, height: 300 });
        $('#BITMAILINGROUPSGroupDialog').dataBind(newsletterGroup);
        BITMAILINGROUPS.updateNewsgroupDialogCheckboxes();
        $('#BITMAILINGROUPSGroupDialog').dialog('open');
    },

    deleteNewsletterGroup: function (i) {
        var newsletterGroup = BITMAILINGROUPS.newsletterGroups[i];
        //BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        $.deleteConfirmBox('Weet u zeker dat u de nieuwsgroep: <strong>' + newsletterGroup.Name + '</strong> wilt verwijderen?', 'Nieuwsgroep verwijderen?', function () {
            var jsonstring = JSON.fromObject(newsletterGroup);
            BITAJAX.callWebServiceASync("DeleteNewsletterGroup", jsonstring, function (data) {
                $('#tableNewsletterGroupList').dataBindList(data.d.DataObject);
                BITMAILINGROUPS.newsletterGroups = data.d.DataObject;
            });
        });
    },

    showAddNewsletter: function () {
         /*var newsletter = new Object();
        newsletter.Name = '';
        newsletter.CreateDate = '';
        newsletter.ModifiedDate = '';
        newsletter.DateFrom = '';
        newsletter.DateTill = '';
        newsletter.Subject = '';
        newsletter.Active = '';
        newsletter.ChangeStatus = '';
        newsletter.LastPublishedDate = '';
        newsletter.LastPublishedFileName = '';
        newsletter.ExpirationDate = '';
        newsletter.IsSent = false;
        newsletter.SendNewsletterAfterRegistration = '';
        newsletter.SendDate = '';
        newsletter.Template = '';
        newsletter.Active = 1;
        newsletter.IsActive = true;
        newsletter.ChangeStatus = 2;
        newsletter.ChangeStatusString = 'New';
        newsletter.SentFromAddress = BITMAILINGROUPS.SentFromAddress;
        newsletter.Groups = Array();
        /* if (BITMAILINGROUPS.currentNewsletterGroup.Details) {
            newsletter.Groups[0] = BITMAILINGROUPS.currentNewsletterGroup.Details.ID;
        } */
        /*

        if (BITMAILINGROUPS.currentNewsletterGroup) {
            newsletter.Groups[0] = BITMAILINGROUPS.currentNewsletterGroup.ID;
        }
        $('#BITMAILINGROUPSGroupSelect').fillDropDown(BITMAILINGROUPS.newsletterGroups, null, 'ID');
        $('#BITMAILINGROUPSDialog').dataBind(newsletter);
        $('#BITMAILINGROUPSDialog').initDialog(function () {
            newsletter = $('#BITMAILINGROUPSDialog').collectData(newsletter);
            newsletter.Template = BITMAILINGROUPS.newsletter.Template;
            //console.log(newsletter);
            BITMAILINGROUPS.newsletter.Groups = Array();
            $(newsletter.Groups).each(function () {
                //console.log(this);
                var selectedGroupId = this;
                $(BITMAILINGROUPS.newsletterGroups).each(function () {
                    if (this.ID == selectedGroupId) {
                        BITMAILINGROUPS.newsletter.Groups.push(this);
                    }
                });
            });
            if (BITMAILINGROUPS.newsletter.Groups.length > 0) {
                newsletter.Groups = BITMAILINGROUPS.newsletter.Groups;
                BITMAILINGROUPS.newsletter = newsletter;
                var parameterObject = { newsletter: BITMAILINGROUPS.newsletter, newsletterGroup: BITMAILINGROUPS.currentNewsletterGroup }
                var jsonstring = JSON.fromObject(parameterObject, true);
                BITAJAX.dataServiceUrl = "NewsletterService.asmx";
                if ($('#BITMAILINGROUPSDialog').validate()) {
                    BITAJAX.callWebServiceASync("SaveNewsletter", jsonstring, function (data) {
                        BITMAILINGROUPS.currentNewsletterGroup.Newsletters = data.d.DataObject;
                        $("#tableNewsletters").dataBindList(BITMAILINGROUPS.currentNewsletterGroup.Newsletters);
                        $('#BITMAILINGROUPSDialog').dialog('close');
                    });
                }
            }
            else {
                $.messageBox(MSGBOXTYPES.WARNING, 'Een nieuwsbrief moet minimaal onder 1 categoriegroep vallen', '');
            }
        }, null);
        $('#BITMAILINGROUPSDialog').formEnrich();
        BITMAILINGROUPS.newsletter = new Object();
        $('#BITMAILINGROUPSGroupSelect').chosen();
        $('#BITMAILINGROUPSGroupSelect').trigger("liszt:updated");
        $('#BITMAILINGROUPSDialog').dialog('open'); */

        BITMAILINGROUPS.editNewsletter(BITUTILS.EMPTYGUID);
    },

    editNewsletter: function (id) {
        var mailing;
        var parameterObject = { id: id }
        var jsonstring = JSON.stringify(parameterObject);
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebServiceASync("GetNewsletter", jsonstring, function (data) {
            BITMAILINGROUPS.newsletter = data.d;
        });

        if (BITMAILINGROUPS.newsletter.IsNew && BITMAILINGROUPS.currentNewsletterGroup) {
            BITMAILINGROUPS.newsletter.Groups = Array();
            BITMAILINGROUPS.newsletter.Groups[0] = BITMAILINGROUPS.currentNewsletterGroup.ID;
        }

        // = BITMAILINGROUPS.currentNewsletterGroup.Newsletters[index];
        //console.log(newsletter);
        $('#BITMAILINGROUPSDialog').initDialog(BITMAILINGROUPS.saveNewsletter, null);

        //BITMAILINGROUPS.newsletter = new Object();
        $('#BITMAILINGROUPSDialog').formEnrich();
        $('#BITMAILINGROUPSGroupSelect').fillDropDown(BITMAILINGROUPS.newsletterGroups, null, 'ID');
        //console.log(BITMAILINGROUPS.newsletterGroups);
        $('#BITMAILINGROUPSDialog').dataBind(BITMAILINGROUPS.newsletter);
        $('#tableSentLog').dataBindList(BITMAILINGROUPS.newsletter.Mailings, {
            onRowBound: function (obj, i, rowHtml) {
                var status = (obj.NewsletterSent) ? 'Verzonden' : 'Niet verzonden';
                $(rowHtml).find('td[data-field="NewsletterSent"]').html(status);
                return rowHtml;
            }
        });
        $('#newsletterSentAmount').html(BITMAILINGROUPS.newsletter.Mailings.length);

        $('#tableStatics').dataBindList(BITMAILINGROUPS.newsletter.Statistics);

        $('#BITMAILINGROUPSGroupSelect').chosen();
        $('#BITMAILINGROUPSGroupSelect').trigger("liszt:updated");
        $('#bitAccordion').accordion({
            heightStyle: "content"
        });
        $('#BITMAILINGROUPSDialog').dialog('open');
    },

    saveNewsletter: function () {
        var validation = $('#BITMAILINGROUPSDialog').validate();
        if (validation) {
            BITMAILINGROUPS.newsletter = $('#BITMAILINGROUPSDialog').collectData(BITMAILINGROUPS.newsletter);
            //console.log(BITMAILINGROUPS.newsletter);
            //if (!$.isEmptyObject(BITMAILINGROUPS.newsletter.Template)) {
            //    BITMAILINGROUPS.newsletter.Template = BITMAILINGROUPS.newsletter.Template;
            //}
            BITMAILINGROUPS.newsletter.Groups = Array();
            //'#BITMAILINGROUPSGroupSelect'
            $(BITMAILINGROUPS.newsletterGroups).each(function () {
                var group = this;
                $('#BITMAILINGROUPSGroupSelect option:selected').each(function () {
                    var id = $(this).attr('value');
                    if (group.ID == id) {
                        var groupObj = new Object();
                        groupObj.ID = id;
                        BITMAILINGROUPS.newsletter.Groups.push(groupObj);
                    }
                });
            });

            /* $(BITMAILINGROUPS.newsletter.Groups).each(function () {
                var selectedGroupId = this;
                $('#BITMAILINGROUPSGroupSelect').each(function () {
                    if (this.ID == selectedGroupId) {
                        BITMAILINGROUPS.newsletter.Groups.push(this);
                    }
                });
            }); */

            if (BITMAILINGROUPS.newsletter.Groups.length > 0) {
                //BITMAILINGROUPS.newsletter.Groups = BITMAILINGROUPS.newsletter.Groups;
                //BITMAILINGROUPS.newsletter = newsletter;
                var parameterObject = { newsletter: BITMAILINGROUPS.newsletter, newsletterGroupID: BITMAILINGROUPS.currentNewsletterGroup.ID }
                var jsonstring = JSON.fromObject(parameterObject, true);
                BITAJAX.dataServiceUrl = "/_bitplate/Newsletters/NewsletterService.asmx";
                if ($('#BITMAILINGROUPSDialog').validate()) {
                    BITAJAX.callWebServiceASync("SaveNewsletter", jsonstring, function (data) {
                        BITMAILINGROUPS.currentNewsletterGroup.Newsletters = data.d.DataObject;
                        $("#tableNewsletters").dataBindList(BITMAILINGROUPS.currentNewsletterGroup.Newsletters);
                        if ($('#divWrapperMain').checkFunctionalityElements) {
                            $('#divWrapperMain').checkFunctionalityElements('newsletterList');
                        }
                        $('#BITMAILINGROUPSDialog').dialog('close');
                    });
                }
            }
            else {
                $.messageBox(MSGBOXTYPES.WARNING, 'Een nieuwsbrief moet minimaal onder 1 categoriegroep vallen', '');
            }
        }
    },

    /* editNewsletter: function (index) {
        var newsletter = BITMAILINGROUPS.currentNewsletterGroup.Newsletters[index];
        //console.log(newsletter);
        $('#BITMAILINGROUPSDialog').initDialog(function () {
            newsletter = $('#BITMAILINGROUPSDialog').collectData(newsletter);
            //console.log(BITMAILINGROUPS.newsletter);
            if (!$.isEmptyObject(BITMAILINGROUPS.newsletter.Template)) {
                newsletter.Template = BITMAILINGROUPS.newsletter.Template;
            }
            BITMAILINGROUPS.newsletter.Groups = Array();
            $(newsletter.Groups).each(function () {
                var selectedGroupId = this;
                $(BITMAILINGROUPS.newsletterGroups).each(function () {
                    if (this.ID == selectedGroupId) {
                        BITMAILINGROUPS.newsletter.Groups.push(this);
                    }
                });
            });
            newsletter.Groups = BITMAILINGROUPS.newsletter.Groups;
            BITMAILINGROUPS.newsletter = newsletter;
            var jsonstring = JSON.fromObject({ newsletter: BITMAILINGROUPS.newsletter, newsletterGroup: BITMAILINGROUPS.currentNewsletterGroup }, true);
            BITAJAX.dataServiceUrl = "NewsletterService.asmx";
            if ($('#BITMAILINGROUPSDialog').validate()) {
                BITAJAX.callWebServiceASync("SaveNewsletter", jsonstring, function (data) {
                    BITMAILINGROUPS.currentNewsletterGroup.Newsletters = data.d.DataObject;
                    $("#tableNewsletters").dataBindList(BITMAILINGROUPS.currentNewsletterGroup.Newsletters);
                    $('#BITMAILINGROUPSDialog').dialog('close');
                });
            }
        }, null);

        BITMAILINGROUPS.newsletter = new Object();
        $('#BITMAILINGROUPSDialog').formEnrich();
        $('#BITMAILINGROUPSGroupSelect').fillDropDown(BITMAILINGROUPS.newsletterGroups, null, 'ID');
        //console.log(BITMAILINGROUPS.newsletterGroups);
        $('#BITMAILINGROUPSDialog').dataBind(newsletter);
        
        $('#BITMAILINGROUPSGroupSelect').chosen();
        $('#BITMAILINGROUPSGroupSelect').trigger("liszt:updated");
        $('#bitAccordion').accordion({
            heightStyle: "content"
        });
        $('#BITMAILINGROUPSDialog').dialog('open');
    }, */

    deleteNewsletter: function (index) {
        var newsletter = BITMAILINGROUPS.currentNewsletterGroup.Newsletters[index];
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        $.deleteConfirmBox('Weet u zeker dat u de nieuwsbrief: <strong>' + newsletter.Name + '</strong> wilt verwijderen?', 'Nieuwsbrief verwijderen?', function () {
            var parameterObject = { ID: newsletter.ID }
            var jsonstring = JSON.fromObject(parameterObject);
            BITAJAX.callWebServiceASync("DeleteNewsletter", jsonstring, function (data) {
                if (data.d.Success) {
                    BITMAILINGROUPS.currentNewsletterGroup.Newsletters.splice(index, 1);
                    $("#tableNewsletters").dataBindList(BITMAILINGROUPS.currentNewsletterGroup.Newsletters);
                }
                else {
                    $.messageBox(MSGBOXTYPE.ERROR, 'Kan nieuwsbrief niet verwijderen:<br />' + data.d.Message, 'WARNING');
                }
            });
        });
    },

    showTemplateSelector: function () {
        $('#bitTemplateDialog').initDialog(null, { width: 600, height: 400 }, false);
        $('#bitTemplateDialog').dialog('open');
    },

    selectTemplate: function (div) {
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        var id = $(div).find("input[type=hidden].hiddenID").val();
        var i = $(div).find("input[type=hidden].hiddenListIndex").val();
        var template = BITMAILINGROUPS.templates[i];
        if (template.ID == id) {
            if (BITMAILINGROUPS.newsletter != null) {
                //om fout te voorkomen " doesn't fit the state of the object" niet rechtstreeks template zetten
                BITMAILINGROUPS.newsletter.Template = new Object();
                BITMAILINGROUPS.newsletter.Template.ID = template.ID;
                BITMAILINGROUPS.newsletter.Template.Name = template.Name;
                //BITMAILINGROUPS.newsletter.Template.Scripts = template.Scripts;
                //$("#tableScriptsPerTemplate").dataBindList(BITMAILINGROUPS.newsletter.Template.Scripts);
            }

            $('#imgTemplateScreenShot').show();
            $("#imgTemplateScreenShot").attr("src", template.Screenshot);
            $("#spanTemplateName").html(template.Name);
            $("#spanTemplateName").parent().find('.validationMsg').hide();
            $("#spanLanguageCode").html(template.LanguageCode);
            //$(BITPAGES.pagePopup.getContentDiv()).find("#tableScriptsPerTemplate_AtPage").dataBindList(template.Scripts);

        }
        $('#bitTemplateDialog').dialog('close');

    },

    loadTemplates: function () {
        BITAJAX.dataServiceUrl = "/_bitplate/Templates/TemplateService.asmx";
        BITAJAX.callWebServiceASync("GetNewsletterTemplates", null, function (data) {
            BITMAILINGROUPS.templates = data.d;
            $("#divChooseTemplate").dataBindList(data.d);
            //$(BITPAGES.pagePopup.getContentDiv()).find("#divChooseTemplate").dataBindList(data.d);
        });
    },

    openNewsletterGroup: function (index) {
                     
    },

    addAbonnement: function () {
        $('#BITMAILINGROUPSSubscriberDialog').initDialog(function () {
            var parametersObject = { NewsLetterGroupId: BITMAILINGROUPS.currentNewsletterGroup.Details.ID, Email: $('#NewsletterAbbonenementEmail').val() };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "NewsletterService.asmx";
            BITAJAX.callWebServiceASync("SaveAbbonementEmail", jsonstring, function (data) {
                if (data.d.Success) {
                    BITMAILINGROUPS.currentNewsletterGroup.Subscribers = data.d.DataObject;
                    $("#tableNewsletter").dataBindList(BITMAILINGROUPS.currentNewsletterGroup.Subscribers);
                }
                else {
                    $.messageBox(MSGBOXTYPE.ERROR, data.d.Message);
                }
                $('#BITMAILINGROUPSSubscriberDialog').dialog('close');
            });
        }, { width: 400, height: 200 }, false);
        $('#BITMAILINGROUPSSubscriberDialog').dialog('open');
    },

    editAbonnement: function (index) {
        var subscriber = BITMAILINGROUPS.currentNewsletterGroup.Subscribers[index];
        $('#BITMAILINGROUPSSubscriberDialog').initDialog(function () {
            if ($('#BITMAILINGROUPSSubscriberDialog').validate()) {
                $('#BITMAILINGROUPSSubscriberDialog').collectData(subscriber);
                var jsonParameters = new Object();
                jsonParameters.obj = subscriber;
                jsonParameters.obj.Name = subscriber.EmailAdres;
                jsonParameters.NewsletterID = BITMAILINGROUPS.currentNewsletterGroup.Details.ID;
                var jsonstring = JSON.fromObject(jsonParameters, true);
                BITAJAX.dataServiceUrl = "NewsletterService.asmx";
                BITAJAX.callWebServiceASync("ChangeAbbonementEmail", jsonstring, function (data) {
                    if (data.d.Success) {
                        BITMAILINGROUPS.currentNewsletterGroup.Subscribers = data.d.DataObject;
                        $("#tableNewsletterAbonnementen").dataBindList(BITMAILINGROUPS.currentNewsletterGroup.Subscribers);
                        $('#BITMAILINGROUPSSubscriberDialog').dialog('close');
                    }
                    else {
                        $.messageBox(data.d.Message);
                        $('#BITMAILINGROUPSSubscriberDialog').dialog('close');
                    }
                });
            }
        });
        $('#BITMAILINGROUPSSubscriberDialog').dataBind(subscriber);
        $('#BITMAILINGROUPSSubscriberDialog').dialog('open');
    },

    deleteAbonnement: function (index) {
        var subscriber = BITMAILINGROUPS.currentNewsletterGroup.Subscribers[index];
        $.deleteConfirmBox('Weet u zeker dat u dit abbonement wilt verwijderen uit deze groep?', 'Abonnement verwijderen uit groep.', function () {
            var parametersObject = { NewsLetterGroupId: BITMAILINGROUPS.currentNewsletterGroup.Details.ID, SubscriberId: subscriber.ID };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "NewsletterService.asmx";
            BITAJAX.callWebServiceASync("DeleteAbbonementFromGroup", jsonstring, function (data) {
                BITMAILINGROUPS.currentNewsletterGroup.Subscribers = data.d.DataObject;
                $("#tableNewsletter").dataBindList(BITMAILINGROUPS.currentNewsletterGroup.Subscribers);
            });
        });
    },

    sendNewsletter: function (id) {
        var parameterObject = { id: id }
        var jsonstring = JSON.stringify(parameterObject);
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebServiceASync("GetNewsletter", jsonstring, function (data) {
            BITMAILINGROUPS.newsletter = data.d;

            $('#bitSendNewsletterDialog').initDialog(null, { width: 800, height: 500 }, false, {
                'Verzend': function () {
                    $('#bitAjaxLoaderContainer').fadeIn();
                    $('#bitSendNewsletterDialog').dialog('close');
                    var newsletterGroupList = $('#BITMAILINGROUPSGroupSendSelect').val();
                    if (newsletterGroupList.length > 0) {
                        var checkboxSendToNewSubscribers = ($('#checkboxSendToNewSubscribers').val() == 'on');
                        var parametersObject = { newsletterId: id, NewsletterGroups: newsletterGroupList, sendToNewSubscribers: checkboxSendToNewSubscribers };
                        var jsonstring = JSON.stringify(parametersObject);
                        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
                        BITAJAX.callWebServiceASync("SendNewsletter", jsonstring, function (data) {
                            $('#bitAjaxLoaderContainer').fadeOut();
                            $.messageBox(MSGBOXTYPES.INFO, 'Nieuwsbrief verzonden.', 'Nieuwsbrief verzonden');
                        });
                    }
                    else {
                        $.messageBox(MSGBOXTYPES.WARNING, 'Selecteer minimaal 1 nieuwsgroep', 'Nieuwsbrief versturen');
                    }
                },
                'Annuleer': function () {
                    $('#bitSendNewsletterDialog').dialog('close');
                }
            });
            $('#BITMAILINGROUPSGroupSendSelect').fillDropDown(BITMAILINGROUPS.newsletterGroups, null, 'ID');
            $('#bitSendNewsletterDialog').dataBind(BITMAILINGROUPS.newsletter);
            $('#BITMAILINGROUPSGroupSendSelect').chosen();
            
            $('#BITMAILINGROUPSGroupSendSelect').trigger("liszt:updated");
            $('#bitSendNewsletterDialog').dialog('open');
        });
    }
}