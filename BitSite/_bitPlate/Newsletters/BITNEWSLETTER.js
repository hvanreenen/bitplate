/// <reference path="../_js/jquery-1.7.2-vsdoc.js" />
/// <reference path="../_js/JSON.js" />
/// <reference path="../../_js/prototypes/databind.js" />
/// <reference path="../prototypes/initDialog.js" />

var BITNEWSLETTER = {
    newsletter: null,
    currentNewsletters: null,
    SentFromAddress: null,
    currentSort: "Name ASC",

    initialize: function () {
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITNEWSLETTER.loadNewsletters(BITNEWSLETTER.currentSort, '');
        BITNEWSLETTER.loadNewsletterMailingGroups();
        BITNEWSLETTER.loadTemplates();
        
        $('#bitSearchTextbox').searchable(BITNEWSLETTER.search);

        $('#bitMailBot').initDialog(null, { width: '1000px', height: '30px'});
    },

    search: function (searchText) {
        BITNEWSLETTER.loadNewsletters(BITNEWSLETTER.currentSort, searchText);
    },

    loadNewsletters: function (sort, searchstring) {
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        var parameterObject = { sort: sort, searchstring: searchstring };
        var jsonstring = JSON.stringify(parameterObject);
        BITAJAX.callWebServiceASync("LoadNewsletters", jsonstring, function (data) {
            $("#tableNewsletters").dataBindList(data.d, {
                onSort: function (sort) {
                    BITNEWSLETTER.loadNewsletters(sort, searchstring);
                },
            });
            BITNEWSLETTER.currentNewsletters = data.d;
            $('#divWrapperMain').checkFunctionalityElements('newsletterList');
        });
    },

    loadNewsletterMailingGroup: function (index) {
        
        var newsletterGroup = BITNEWSLETTER.newsletterGroups[index];
        BITNEWSLETTER.currentNewsletterGroup = newsletterGroup;
        BITNEWSLETTER.currentPath = newsletterGroup.Name;
        BITNEWSLETTER.updateBreadCrump(BITNEWSLETTER.currentPath);
        var jsonstring = JSON.fromObject(newsletterGroup);
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebServiceASync("LoadNewsletterList", jsonstring, function (data) {
            $("#tableNewsletters").dataBindList(data.d);
            BITNEWSLETTER.currentNewsletterGroup.Newsletters = data.d;
            $('#divWrapperMain').checkFunctionalityElements('newsletterList');
        });
        
    },
    //IS DEZE FUNCTIE NODIG?
    getNewsGroupIndexByName: function (name) {
        var index = null;
        $(BITNEWSLETTER.newsletterGroups).each(function (i) {
            if (this.Name == name) {
                index = i;
            }
        });
        return index;
    },

    loadNewsletterMailingGroups: function () {
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        var parameters = { sort: 'Name ASC' };
        var jsonstring = JSON.stringify(parameters);
        BITAJAX.callWebServiceASync("loadNewsletterGroupList", jsonstring, function (data) {
            BITNEWSLETTER.newsletterGroups = data.d;
        });
    },

    clearNewsletterGroupDialog: function() {
        $('#bitNewsletterGroupDialog input').val('');
        $('#bitNewsletterGroupDialog input[type="checkbox"]').attr('checked', false);
    },

    showAddNewsletter: function () {
        BITNEWSLETTER.editNewsletter(BITUTILS.EMPTYGUID);
    },

    editNewsletter: function (id) {
        var mailing;
        var parameterObject = { id: id }
        var jsonstring = JSON.stringify(parameterObject);
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebServiceASync("GetNewsletter", jsonstring, function (data) {
            BITNEWSLETTER.newsletter = data.d;
            if (BITNEWSLETTER.newsletter.IsNew && BITNEWSLETTER.currentNewsletterGroup) {
                BITNEWSLETTER.newsletter.Groups = Array();
                BITNEWSLETTER.newsletter.Groups[0] = BITNEWSLETTER.currentNewsletterGroup.ID;
            }

            // = BITNEWSLETTER.currentNewsletterGroup.Newsletters[index];
            //console.log(newsletter);
            $('#bitNewsletterDialog').initDialog(BITNEWSLETTER.saveNewsletter, null);

            //BITNEWSLETTER.newsletter = new Object();
            $('#bitNewsletterDialog').formEnrich();
            $('#bitNewsletterGroupSelect').fillDropDown(BITNEWSLETTER.newsletterGroups, null, 'ID');
            //console.log(BITNEWSLETTER.newsletterGroups);
            $('#bitNewsletterDialog').dataBind(BITNEWSLETTER.newsletter);
            $('#tableSentLog').dataBindList(BITNEWSLETTER.newsletter.Mailings, {
                onRowBound: function (obj, i, rowHtml) {
                    var status = (obj.NewsletterSent) ? 'Verzonden' : 'Niet verzonden';
                    $(rowHtml).find('td[data-field="NewsletterSent"]').html(status);
                    return rowHtml;
                }
            });
            $('#newsletterSentAmount').html(BITNEWSLETTER.newsletter.Mailings.length);

            $('#tableStatics').dataBindList(BITNEWSLETTER.newsletter.Statistics);

            $('#bitNewsletterGroupSelect').chosen();
            $('#bitNewsletterGroupSelect').trigger("liszt:updated");
            $('#bitAccordion').accordion({
                heightStyle: "content"
            });
            $('#bitNewsletterDialog').dialog('open');
        });
    },

    saveNewsletter: function () {
        var validation = $('#bitNewsletterDialog').validate();
        if (validation) {
            BITNEWSLETTER.newsletter = $('#bitNewsletterDialog').collectData(BITNEWSLETTER.newsletter);
            //console.log(BITNEWSLETTER.newsletter);
            //if (!$.isEmptyObject(BITNEWSLETTER.newsletter.Template)) {
            //    BITNEWSLETTER.newsletter.Template = BITNEWSLETTER.newsletter.Template;
            //}
            BITNEWSLETTER.newsletter.Groups = Array();
            //'#bitNewsletterGroupSelect'
            $(BITNEWSLETTER.newsletterGroups).each(function () {
                var group = this;
                $('#bitNewsletterGroupSelect option:selected').each(function () {
                    var id = $(this).attr('value');
                    if (group.ID == id) {
                        var groupObj = new Object();
                        groupObj.ID = id;
                        BITNEWSLETTER.newsletter.Groups.push(groupObj);
                    }
                });
            });

            /* $(BITNEWSLETTER.newsletter.Groups).each(function () {
                var selectedGroupId = this;
                $('#bitNewsletterGroupSelect').each(function () {
                    if (this.ID == selectedGroupId) {
                        BITNEWSLETTER.newsletter.Groups.push(this);
                    }
                });
            }); */

            if (BITNEWSLETTER.newsletter.Groups.length > 0) {
                //BITNEWSLETTER.newsletter.Groups = BITNEWSLETTER.newsletter.Groups;
                //BITNEWSLETTER.newsletter = newsletter;
                var parameterObject = { newsletter: BITNEWSLETTER.newsletter }
                var jsonstring = JSON.fromObject(parameterObject, true);
                BITAJAX.dataServiceUrl = "/_bitplate/Newsletters/NewsletterService.asmx";
                if ($('#bitNewsletterDialog').validate()) {
                    BITAJAX.callWebServiceASync("SaveNewsletter", jsonstring, function (data) {
                        BITNEWSLETTER.Newsletters = data.d.DataObject;
                        $("#tableNewsletters").dataBindList(BITNEWSLETTER.Newsletters);
                        if ($('#divWrapperMain').checkFunctionalityElements) {
                            $('#divWrapperMain').checkFunctionalityElements('newsletterList');
                        }
                        $('#bitNewsletterDialog').dialog('close');
                    });
                }
            }
            else {
                $.messageBox(MSGBOXTYPES.WARNING, 'Een nieuwsbrief moet minimaal onder 1 categoriegroep vallen', '');
            }
        }
    },

    deleteNewsletter: function (index) {
        var newsletter = BITNEWSLETTER.currentNewsletterGroup.Newsletters[index];
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        $.deleteConfirmBox('Weet u zeker dat u de nieuwsbrief: <strong>' + newsletter.Name + '</strong> wilt verwijderen?', 'Nieuwsbrief verwijderen?', function () {
            var parameterObject = { ID: newsletter.ID }
            var jsonstring = JSON.fromObject(parameterObject);
            BITAJAX.callWebServiceASync("DeleteNewsletter", jsonstring, function (data) {
                if (data.d.Success) {
                    BITNEWSLETTER.currentNewsletterGroup.Newsletters.splice(index, 1);
                    $("#tableNewsletters").dataBindList(BITNEWSLETTER.currentNewsletterGroup.Newsletters);
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
        var template = BITNEWSLETTER.templates[i];
        if (template.ID == id) {
            if (BITNEWSLETTER.newsletter != null) {
                //om fout te voorkomen " doesn't fit the state of the object" niet rechtstreeks template zetten
                BITNEWSLETTER.newsletter.Template = new Object();
                BITNEWSLETTER.newsletter.Template.ID = template.ID;
                BITNEWSLETTER.newsletter.Template.Name = template.Name;
                //BITNEWSLETTER.newsletter.Template.Scripts = template.Scripts;
                //$("#tableScriptsPerTemplate").dataBindList(BITNEWSLETTER.newsletter.Template.Scripts);
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
            BITNEWSLETTER.templates = data.d;
            $("#divChooseTemplate").dataBindList(data.d);
            //$(BITPAGES.pagePopup.getContentDiv()).find("#divChooseTemplate").dataBindList(data.d);
        });
    },

    sendNewsletter: function (id) {
        var parameterObject = { id: id }
        var jsonstring = JSON.stringify(parameterObject);
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebServiceASync("GetNewsletter", jsonstring, function (data) {
            BITNEWSLETTER.newsletter = data.d;

            $('#bitSendNewsletterDialog').initDialog(null, { width: 800, height: 500 }, false, {
                'Verzend': function () {
                    $('#bitAjaxLoaderContainer').fadeIn();
                    var newsletterGroupList = $('#bitNewsletterGroupSendSelect').val();
                    if (newsletterGroupList && newsletterGroupList.length > 0) {
                        var checkboxSendToNewSubscribers = ($('#checkboxSendToNewSubscribers').is(":checked"));
                        var parametersObject = { newsletterId: id, NewsletterGroups: newsletterGroupList, sendToNewSubscribers: checkboxSendToNewSubscribers };
                        var jsonstring = JSON.stringify(parametersObject);
                        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
                        BITAJAX.callWebServiceASync("GetNewsletterRecipients", jsonstring, function (data) {
                            //$('#bitAjaxLoaderContainer').fadeOut();
                            BITNEWSLETTER.startMailBot(id, data.d);
                            $('#bitSendNewsletterDialog').dialog('close');
                        });
                    }
                    else {
                        $('#bitAjaxLoaderContainer').fadeOut();
                        $.messageBox(MSGBOXTYPES.WARNING, 'Selecteer minimaal 1 nieuwsgroep', 'Nieuwsbrief versturen');
                    }
                },
                'Annuleer': function () {
                    $('#bitSendNewsletterDialog').dialog('close');
                }
            });
            $('#bitNewsletterGroupSendSelect').fillDropDown(BITNEWSLETTER.newsletterGroups, null, 'ID');
            $('#bitSendNewsletterDialog').dataBind(BITNEWSLETTER.newsletter);
            $('#bitNewsletterGroupSendSelect').chosen();
            
            $('#bitNewsletterGroupSendSelect').trigger("liszt:updated");
            $('#bitSendNewsletterDialog').dialog('open');
        });
    },

    startMailBot: function (newsletterId, subscribers) {
        var progressbar = $("#progressbar"),
            progressLabel = $(".progress-label");

        progressbar.progressbar({
            value: false,
            change: function () {
                progressLabel.text(progressbar.progressbar("value") + "%");
            },
            complete: function () {
                progressLabel.text("Nieusbrief verzonden.");
            }
        });
        var i = 0;
        progressbar.progressbar("value", 0);

        $('#bitMailBot').dialog('open');
        $(".ui-dialog-buttonpane button:contains('Sluiten')").button("disable");
        var next = false;
        $(subscribers).each(function () {
            next = false;
            var progress = (i / subscribers.length) * 100;
            progressbar.progressbar("value", progress);
            var parametersObject = { obj: this, newsletterId: newsletterId };
            var jsonstring = convertToJsonString(parametersObject, true);
            BITAJAX.dataServiceUrl = "NewsletterService.asmx";
            BITAJAX.callWebService("SendNewsletterToSubscriber", jsonstring, function (data) {
  
            });
            i++;
        });
        $(".ui-dialog-buttonpane button:contains('Sluiten')").button("enable");
        progressbar.progressbar("value", 100);
    }
}