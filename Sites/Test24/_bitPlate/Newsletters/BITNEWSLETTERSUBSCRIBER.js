var BITNEWSLETTERSUBSCRIBER = {
    Subscribers: null,
    currentSubscriber: null,
    newGroupList: null,
    allNewsGroups: null,
    allUserGroups: null,
    currentSort: 'Name ASC',
    searchString: '',
    importDefinition: null,
    columsNames: null,
    definitions: null,
    type: 'Subscribers',
    newsGroups: null,
    currentSelectedGroups: null,
    log: null,
    currentTab: 0,

    initialize: function () {
        $('#panelDetails').formEnrich();
        $('#bitNewsGroupPerUserDialog').initDialog(null, { width: 200 });

        BITNEWSLETTERSUBSCRIBER.importDefinition = new Object();
      
        $(".bitTabs").tabs();

        $('#bitImportSubscribersDialog').initDialog(null, { width: "80%", height: 600 }, true, {
           

            'Vorige': function () {
                BITNEWSLETTERSUBSCRIBER.GetCurrentSelectedTab();
                var currentTab = BITNEWSLETTERSUBSCRIBER.currentTab;
                var c = $('#bitImportSubscribersDialog > .bitTabs').tabs("length");
                currentTab = currentTab == 0 ? currentTab : (currentTab - 1);
                $('#bitImportSubscribersDialog > .bitTabs').tabs('select', currentTab);

              /*  if (currentTab == 0) {
                    $(".ui-dialog-buttonpane button:contains('Volgende') span").show();
                    $(".ui-dialog-buttonpane button:contains('Vorige') span").hide();
                }
                if (currentTab < (c - 1)) {
                    $(".ui-dialog-buttonpane button:contains('Volgende') span").show();
                }*/
            },

            'Volgende': function () {
                BITNEWSLETTERSUBSCRIBER.GetCurrentSelectedTab();
                var currentTab = BITNEWSLETTERSUBSCRIBER.currentTab;
                var c = $('#bitImportSubscribersDialog > .bitTabs').tabs("length");
                currentTab = currentTab == (c - 1) ? currentTab : (currentTab + 1);
                $('#bitImportSubscribersDialog > .bitTabs').tabs('select', currentTab);

               /* $(".ui-dialog-buttonpane button:contains('Vorige') span").show();
                if (currentTab == (c - 1)) {
                    $(".ui-dialog-buttonpane button:contains('Annuleer') span").text("Sluiten");
                    $(".ui-dialog-buttonpane button:contains('Volgende') span").hide();
                } else {
                    $(".ui-dialog-buttonpane button:contains('Volgende') span").show();
                }*/
            },

            'Annuleer': function () {
                $(".ui-dialog-buttonpane button:contains('Sluiten') span").text("Annuleer");
                $('#bitImportSubscribersDialog').dialog('close');
                $(".bitTabs").tabs("option", "active", 0);
            },
        });
        
        $('#bitNewsletterSubscriberDialog').initDialog(BITNEWSLETTERSUBSCRIBER.saveSubscriber);
        BITNEWSLETTERSUBSCRIBER.loadNewsletterSubscribers();

        BITNEWSLETTERSUBSCRIBER.loadImportDefinitions();

        $('#connectUserProfile').change(function () {
            if ($('#connectUserProfile').is(':checked')) {
                BITNEWSLETTERSUBSCRIBER.enableUserProfileAccordionHeader();
            }
            else {
                BITNEWSLETTERSUBSCRIBER.disableUserProfileAccordionHeader();
            }
        });

        $('#bitSearchTextbox').searchable(BITNEWSLETTERSUBSCRIBER.search);
    },

    
    GetCurrentSelectedTab: function () {
        $("#bitImportSubscribersDialog>.bitTabs").tabs({
            select: function (e, i) {
                BITNEWSLETTERSUBSCRIBER.currentTab = i.index;
            }
        });
    },
    clearImportDefinitionDialog: function()
    {
        var parametersObject = { id: BITUTILS.EMPTYGUID };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebService("GetImportDefinition", jsonstring, function (data) {

            BITNEWSLETTERSUBSCRIBER.importDefinition = data.d;

            $('#bitImportSubscribersDialog').dataBind(BITNEWSLETTERSUBSCRIBER.importDefinition);

            $('#selectForeNameColumn').val('');
            $('#selectNamePrefixColumn').val('');
            $('#selectNameColumn').val('');
            $('#selectEmailColumn').val('');
            //$('#selectGenderColumn').val('');
            $('#foutlog tbody').html();

            $('#importNewsgroupSelector').val('').trigger("liszt:updated");
            $('#importNewsgroupSelector option[value= \' \' ]').prop('selected', true);
                $('#importNewsgroupSelector').trigger("liszt:updated");
            }); 
    },

    loadNewsletterSubscribers: function (sort, searchString) {
        $('#panelDetails').formEnrich();
        if (sort) BITNEWSLETTERSUBSCRIBER.currentSort = sort;
        if (searchString || searchString == '') BITNEWSLETTERSUBSCRIBER.searchString = searchString;
        var parametersObject = { sortering: BITNEWSLETTERSUBSCRIBER.currentSort, searchString: BITNEWSLETTERSUBSCRIBER.searchString };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/Newsletters/NewsletterService.asmx";
        BITAJAX.callWebService("GetSubscribers", jsonstring, function (data) {
            
            $('#tableUsers').dataBindList(data.d, {
                onSort: function (sort) {
                    BITNEWSLETTERSUBSCRIBER.loadNewsletterSubscribers(sort);
                },
                /* onRowBound: function (obj, i, rowHtml) {
                    var groups = '';
                    $(obj.SubscribedGroups).each(function () {
                        if (groups != '') groups += ', ';
                        groups += this.Name
                    });
                    $(rowHtml).find('.nieuwsgroepen').html(groups);
                    return rowHtml;
                } */
            });
            BITNEWSLETTERSUBSCRIBER.Subscribers = data.d;
        });
    },

    loadImportDefinitions: 
        function (sort, searchstring) {
            var parametersObject = { sortering: BITNEWSLETTERSUBSCRIBER.currentSort, searchString: BITNEWSLETTERSUBSCRIBER.searchString };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "NewsletterService.asmx";
            BITAJAX.callWebService("GetImportDefinitions", jsonstring, function (data) {
            
                $('#tableImportDefinitions').dataBindList(data.d ,{
                    onSort: function (sort) {
                        BITNEWSLETTERSUBSCRIBER.loadImportDefinitions(sort);
                    },
                });
                BITNEWSLETTERSUBSCRIBER.definitions = data.d;
            });
        },
   
    search: function (searchString) {

        if (BITNEWSLETTERSUBSCRIBER.type == "Subscribers") {
            BITNEWSLETTERSUBSCRIBER.loadNewsletterSubscribers(null, searchString);
        }
        else {
            BITNEWSLETTERSUBSCRIBER.loadImportDefinitions(null, searchString);
        }
    },

    newSubscriber: function () {
        BITNEWSLETTERSUBSCRIBER.editSubscriber(BITUTILS.EMPTYGUID);
    },

    editSubscriber: function (id) {

        $(".bitAccordion").accordion({ active: 0, collapsible: true });
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/Newsletters/NewsletterService.asmx";
        BITAJAX.callWebService("GetSubscriber", jsonstring, function (data) {
            var subscriber = data.d;

            BITNEWSLETTERSUBSCRIBER.currentSubscriber = subscriber;
            $('#bitNewsletterSubscriberDialog').dataBind(subscriber);
            if (subscriber.User && subscriber.User.ID != '00000000-0000-0000-0000-000000000000') {
                $('#UserProfileAccordionTitle').html('Deze subscriber heeft een gekoppeld site account.');
                $('#connectUserProfile').prop('disabled', true);
                $('#tableUserGroupsPerUser').dataBindList(subscriber.User.UserGroups);
            }
            else {
                $('#tableUserGroupsPerUser').dataBindList({});
                $('#UserProfileAccordionTitle').html('Deze subscriber heeft nog geen site account. Vul het formulier in om een account aan te maken.');
                $('#connectUserProfile').prop('disabled', false);
            }
            BITNEWSLETTERSUBSCRIBER.dataBindNewsletterGroupList();
            //BITNEWSLETTERSUBSCRIBER.currentSubscriber = subscriber;

            /* if (subscriber.User.ID != null && subscriber.User.ID != BITUTILS.EMPTYGUID) {
                $('#connectUserProfile').attr('checked', true);
                BITNEWSLETTERSUBSCRIBER.enableUserProfileAccordionHeader();
            }
            else {
                BITNEWSLETTERSUBSCRIBER.disableUserProfileAccordionHeader();
                $('#connectUserProfile').attr('checked', false);
            } */

            $('#bitNewsletterSubscriberDialog').dialog('open');
        });
    },

    dataBindNewsletterGroupList: function () {
        $('#tableNewsGroupsPerUser').dataBindList(BITNEWSLETTERSUBSCRIBER.currentSubscriber.SubscribedGroups, {
            onRowBound: function (obj, i, rowHtml) {
                if (obj.IsMandatoryGroup) {
                    $(rowHtml).find('a').hide();
                    $(rowHtml).find('.notes').html('(Verplichte nieuwsgroep.)');
                }
                return rowHtml;
            }
        });
    },

    disableUserProfileAccordionHeader: function () {
        // User Profile Accordion Header disable
        // Add the class ui-state-disabled to the headers that you want disabled
        $("#userProfileAccordionHeader").addClass("ui-state-disabled");

        // Now the hack to implement the disabling functionnality
        var accordion = $(".bitAccordion").data("accordion");

        accordion._std_clickHandler = accordion._clickHandler;

        accordion._clickHandler = function (event, target) {
            var clicked = $(event.currentTarget || target);
            if (!clicked.hasClass("ui-state-disabled")) {
                this._std_clickHandler(event, target);
            }
        };
        //END
    },

    enableUserProfileAccordionHeader: function () {
        // Remove the class ui-state-disabled to the headers that you want to enable
        $("#userProfileAccordionHeader").removeClass("ui-state-disabled");
        var email = $('#NewsletterAbbonenementEmail').val();
        var userEmail = $('#UserEmailAddress').val();
        if (userEmail.trim() == '') {
            $('#UserEmailAddress').val(email);
        }
    },

    saveSubscriber: function () {
        var messages = '<ul>';
        var subscriber = BITNEWSLETTERSUBSCRIBER.currentSubscriber
        var validation = $('#subscriberProfile').validate();

        if (!subscriber.SubscribedGroups ||
                (subscriber.SubscribedGroups && subscriber.SubscribedGroups.length == 0)) {
            validation = false;
            messages += "<li>Kan niet opslaan, want tenminste 1 nieuwsgroep is verplicht.</li>";
        }

        /* if ($('#connectUserProfile').is(':checked')) {
            var uvalidation = $('#userProfile').validate();
        } */

        if (validation) {
            subscriber = $('#bitNewsletterSubscriberDialog').collectData(subscriber);
            //subscriber.Name = subscriber.EmailAdres;
            //subscriber.User.IsLoaded = true;
            var jsonstring = JSON.fromObject(subscriber);
            BITAJAX.dataServiceUrl = "NewsletterService.asmx";
            BITAJAX.callWebService("SaveSubscriber", jsonstring, function (data) {
                if (data.d.Success) {
                    BITNEWSLETTERSUBSCRIBER.loadNewsletterSubscribers();
                    $('#bitNewsletterSubscriberDialog').dialog('close');
                }
                else {
                    $.messageBox(data.d.Message);
                    $('#bitNewsletterSubscriberDialog').dialog('close');
                }
            });
        }
        else {
            messages += '<li>Niet alle velden zijn juist ingevuld. Probeer opnieuw.</li></ul>';
            $.messageBox(MSGBOXTYPES.INFO, messages, 'Invoer fout');
        }
    },

    deleteSubscriber: function (id) {
        $.deleteConfirmBox('Weet u zeker dat u deze subscriber wilt verwijderen?', 'Subscriber verwijderen?', function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "NewsletterService.asmx";
            BITAJAX.callWebService("DeleteSubscriber", jsonstring, function (data) {

                BITNEWSLETTERSUBSCRIBER.loadNewsletterSubscribers();
            });
        });
    },

    deleteImportDefinition: function (id) {
        $.deleteConfirmBox('Weet u zeker dat u deze template wilt verwijderen?', 'Template verwijderen?', function () {
            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "NewsletterService.asmx";
            BITAJAX.callWebService("DeleteImportDefinition", jsonstring, function (data) {

                BITNEWSLETTERSUBSCRIBER.loadImportDefinitions();
            });
        });
    },

    removeNewsgroupPerUser: function (index) {
        BITNEWSLETTERSUBSCRIBER.currentSubscriber.SubscribedGroups.splice(index, 1);
        BITNEWSLETTERSUBSCRIBER.dataBindNewsletterGroupList();
    },

    openNewsGroupsPopup: function () {
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebService("LoadChoosableNewsletterGroupList", null, function (data) {

            $('#bitNewsUserGroupList').dataBindList(data.d);
            $('#bitNewsGroupPerUserDialog').dialog('open');
            BITNEWSLETTERSUBSCRIBER.allNewsGroups = data.d;
        });
    },

    addNewsGroupPerUser: function (index) {
        var NewsGroup = BITNEWSLETTERSUBSCRIBER.allNewsGroups[index];
        var exist = false;
        $(BITNEWSLETTERSUBSCRIBER.currentSubscriber.SubscribedGroups).each(function () {
            if (this.ID == NewsGroup.ID) {
                exist = true;
                return;
            }
        });
        if (exist) {
            $.messageBox(MSGBOXTYPES.INFO, "Koppeling met deze group bestaat al.");
        }
        else {
            BITNEWSLETTERSUBSCRIBER.currentSubscriber.SubscribedGroups.push(NewsGroup);
            BITNEWSLETTERSUBSCRIBER.dataBindNewsletterGroupList();
        }
    },

    openUserGroupsPopup: function () {
        var parametersObject = {
            sort: '',
            onlyCurrentSite: true,
            pageNumber: 0,
            pageSize: 0,
            searchString: ''
        };

        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/users/siteauthService.asmx";
        BITAJAX.callWebService("GetUserGroups", jsonstring, function (data) {
            $("#bitUserGroupList").dataBindList(data.d); //in kies groep-dialog
            BITNEWSLETTERSUBSCRIBER.allUserGroups = data.d;
 
            $('#bitUserGroupPerUserDialog').dialog('open');
        });
    },

    addUserGroupPerUser: function (index) {
        var UserGroup = BITNEWSLETTERSUBSCRIBER.allUserGroups[index];

        if (!BITNEWSLETTERSUBSCRIBER.currentSubscriber.User) {
            BITNEWSLETTERSUBSCRIBER.currentSubscriber.User = new Object();
        }

        if (!BITNEWSLETTERSUBSCRIBER.currentSubscriber.User.UserGroups) {
            BITNEWSLETTERSUBSCRIBER.currentSubscriber.User.UserGroups = [];
        }

        if ($.inArray(UserGroup, BITNEWSLETTERSUBSCRIBER.currentSubscriber.User.UserGroups) === -1) { //BUG met check.
            BITNEWSLETTERSUBSCRIBER.currentSubscriber.User.UserGroups.push(UserGroup);
            $('#tableUserGroupsPerUser').dataBindList(BITNEWSLETTERSUBSCRIBER.currentSubscriber.User.UserGroups);
        }
        else {
            //SaveNewsletterGroup();
            $.messageBox(MSGBOXTYPES.INFO, "Koppeling met deze group bestaat al.");
        }
    },

    removeUserGroupPerUser: function (index) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u deze gebruikersgroep wilt los koppelen van deze gebruiker?", null, function () {
            BITNEWSLETTERSUBSCRIBER.currentSubscriber.User.UserGroups.splice(index, 1);
            $('#tableUserGroupsPerUser').dataBindList(BITNEWSLETTERSUBSCRIBER.currentSubscriber.User.UserGroups);
        });
    },

    newsGroups: null, 
    showImportSubscribers: function () {
        BITNEWSLETTERSUBSCRIBER.clearImportDefinitionDialog();
        BITNEWSLETTERSUBSCRIBER.fillDropDownNewsLetterGroups();
           
        BITAJAX.callWebService("GetMandatoryGroups", null, function (data) {

            $('#importNewsgroupSelector').val('').trigger("liszt:updated");
            $.each(data.d, function (i, val) {
                $('#importNewsgroupSelector option[value=' + val.ID + ']').prop('selected', true);
                $('#importNewsgroupSelector').trigger("liszt:updated");
            });
        });
        $('#bitImportSubscribersDialog').dialog('open');
        $(".ui-dialog-buttonpane button:contains('Volgende') span").show();
        $(".ui-dialog-buttonpane button:contains('Vorige') span").show();
        $(".ui-dialog-buttonpane button:contains('Annuleer') span").show();

    },

    showAddNewGroup: function () {
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        $('#bitNewsletterGroupDialog').initDialog(function () {
            var NewsletterGroup = new Object();
            NewsletterGroup = $('#bitNewsletterGroupDialog').collectData(NewsletterGroup);

            if ($('#bitNewsletterGroupDialog').validate()) {
                var jsonstring = JSON.fromObject(NewsletterGroup);
                BITAJAX.callWebService("SaveNewsletterGroup", jsonstring, function (data) {
                    $('#bitNewsletterGroupDialog').dialog('close');

                    if (data.d.Success) {
                        BITNEWSLETTERSUBSCRIBER.currentSelectedGroups = $('#importNewsgroupSelector').val();
                        BITNEWSLETTERSUBSCRIBER.fillDropDownNewsLetterGroups();
                        var newID = data.d.DataObject[0];
                        
                        $(BITNEWSLETTERSUBSCRIBER.currentSelectedGroups).each(function (){
                            $('#importNewsgroupSelector option[value=' + this + ']').prop('selected', true);
                        }); 

                        $('#importNewsgroupSelector option[value=' + newID + ']').prop('selected', true);
                        $('#importNewsgroupSelector').trigger("liszt:updated");
                    }
                    else {
                        $.messageBox(MSGBOXTYPES.INFO, 'Nieuwsgroep is niet toegevoegd. \r\n\r\n' + data.d.Message, 'Toevoegen mislukt');
                    }
                });
            }
        },
        { width: 400, height: 100}, true);
        $('#bitNewsletterGroupDialog').dialog('open');
    },
   
    ImportSubscribers: function () {
        $("html").append($.ui.dialog.overlay.create());
        $('.ui-widget-overlay').css("z-index", "2000");
        //ss$('.ui-widget-overlay').append('<span style="color:white;size:30px;font-family:Verdana;">IMPORTEREN</span>');

        $('#bitImportSubscribersDialog').collectData(BITNEWSLETTERSUBSCRIBER.importDefinition);
        BITNEWSLETTERSUBSCRIBER.importDefinition.ID = $('[name="importdefinitions"] option:selected').val();
        BITNEWSLETTERSUBSCRIBER.importDefinition.ForeNameColumn = $('#selectForeNameColumn option:selected').text()
        BITNEWSLETTERSUBSCRIBER.importDefinition.NamePrefixColumn = $('#selectNamePrefixColumn option:selected').text();
        BITNEWSLETTERSUBSCRIBER.importDefinition.NameColumn = $('#selectNameColumn option:selected').text();
        BITNEWSLETTERSUBSCRIBER.importDefinition.EmailColumn = $('#selectEmailColumn option:selected').text();
        // BITNEWSLETTERSUBSCRIBER.importDefinition.GenderColumn = $('#selectGenderColumn option:selected').text();

        if ($('[name="savedefinition"]').is(':checked') == true && $('#definitionname').val() != "") {
            BITNEWSLETTERSUBSCRIBER.importDefinition.Name = $('#definitionname').val();
            BITNEWSLETTERSUBSCRIBER.importDefinition.saveSubscriber = $('[name="savedefinition"]').is(':checked');
        }
        BITNEWSLETTERSUBSCRIBER.importDefinition.Groups = Array();

        //De groepen toevoegen aan het object
        $(BITNEWSLETTERSUBSCRIBER.newsGroups).each(function () {
            var group = this;
            $('#importNewsgroupSelector option:selected').each(function () {
                var id = $(this).attr('value');
                if (group.ID == id) {
                    var groupObj = new Object();
                    groupObj.ID = id;
                    BITNEWSLETTERSUBSCRIBER.importDefinition.Groups.push(groupObj);
                }
            });
        });

            var jsonstring = JSON.fromObject(BITNEWSLETTERSUBSCRIBER.importDefinition);
            BITAJAX.dataServiceUrl = "NewsletterService.asmx";
            BITAJAX.callWebServiceASync("ImportSubscribers", jsonstring, function (data) {

                if (data.d.Success) {
                    $('.ui-widget-overlay').remove();
                
                    BITNEWSLETTERSUBSCRIBER.loadNewsletterSubscribers();
                    $('#foutlog tbody').html('');
                    var countsucces = 0;
                    var countfailure = 0;
                    var totaal = countsucces + countfailure;


                    // BITNEWSLETTERSUBSCRIBER.importDefinition.ImportLogFile = data.d.DataObject.ImportLogFile;
                    var importLogFile= data.d.DataObject.ImportLogFile;
                    $.each(data.d.DataObject.ErrorLog, function (key, value) {

                        //Mogelijk met data-fields
                        if (value[5] == "True") {
                            $('#foutlog tbody').append('<tr><td>' + value[0] + '/' + value[4] + ':</td><td>' + value[2] + '</td><td>: ' + value[3] + '</td></tr>');
                            countsucces++
                        }
                        else {
                            $('#foutlog tbody').append('<tr style="color:red;"><td>' + value[0] + '/' + value[4] + ':</td><td>' + value[2] + '</td><td>: ' + value[3] + '</td></tr>'); //<input style="color:red;size:11px;" type="text" value = "' + value[2] + '"/>
                            countfailure++
                        }
                    });
                    $('#importProgress').html('<div>Het importeren is voltooid. <br /> Hier vind u de log van alle gelukte en mislukte addressen.</div>  <br />' + countsucces + ' Ingevoerd <br /> ' + countfailure + ' Niet ingevoerd <br /> ' + (countsucces + countfailure) + ' Totaal' + '<form action="'+importLogFile+'" ><input type="submit" value="Download Log.txt" ></form>')

                    $(".bitTabs").tabs("option", "active", 4);
                }
                else {
                    $('.ui-widget-overlay').remove();
                    $.messageBox(MSGBOXTYPES.INFO, data.d.Message);
                }
            });
           
        
        var validation = $('#bitImportSubscribersDialog').validate();
        if (validation) {
        $('#bitImportSubscribersDialog')
        }
    },

    firstRowisName: function () {
        BITNEWSLETTERSUBSCRIBER.importDefinition.FirstRowIsColumnName = $('#firstrowiscolomnname').is(":checked");
        var jsonstring = JSON.fromObject(BITNEWSLETTERSUBSCRIBER.importDefinition);
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebService("SetIsFirstRowColomnName", jsonstring, function (data) {

            BITNEWSLETTERSUBSCRIBER.columsNames = data.d;
            if (data.d != null) {

                $('#FileExample').empty();
                $('#selectForeNameColumn').empty();
                $('#selectNamePrefixColumn').empty();
                $('#selectNameColumn').empty();
                $('#selectEmailColumn').empty();
                // $('#selectGenderColumn').empty();
                $('#FileExample').append("<tbody><tr></tr></tbody>")

                $('#selectForeNameColumn').append("<option value=\"9999\">  = Kies een kolom =</option>");
                $('#selectNamePrefixColumn').append("<option value=\"9999\">= Kies een kolom =</option>");
                $('#selectNameColumn').append("<option value=\"9999\">      = Kies een kolom =</option>");
                // $('#selectGenderColumn').append("<option value=\"9999\"> = kies een kolom =</option>");
                
                $.each(data.d, function (key, value) {
                    $('#FileExample tr').append("<td>" + value + "</td>");

                    $('#selectForeNameColumn').append("<option value=\"" + key + "\">" + value + "</option>");
                    $('#selectNamePrefixColumn').append("<option value=\"" + key + "\">" + value + "</option>");
                    $('#selectNameColumn').append("<option value=\"" + key + "\">" + value + "</option>");
                    $('#selectEmailColumn').append("<option value=\"" + key + "\">" + value + "</option>");
                    //$('#selectGenderColumn').append("<option value=\"" + key + "\">"+ value+ "</option>");
                });
                BITNEWSLETTERSUBSCRIBER.readFile();
            }
        });
    },

    changeDelimiterCSV: function () {

        var delimiter = $('#delimitercsv option:selected').val();
        var parametersObject = { Delimiter: delimiter };
        var jsonstring = JSON.stringify({ obj: parametersObject });
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebService("SetDelimiter", jsonstring, function (data) {
            if (data.d != null) {
                BITNEWSLETTERSUBSCRIBER.importDefinition.Delimiter = data.d.Delimiter;

                BITNEWSLETTERSUBSCRIBER.firstRowisName();
              
            }
        });
    },

    readFile: function () {
        var jsonstring = JSON.fromObject(BITNEWSLETTERSUBSCRIBER.importDefinition);
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebService("ReadFile", jsonstring, function (data) {
            if (data.d != null) {
                $('#FileExample tbody .record').empty();
                $('#FileExample tbody').append("<tr class=\"record\"> </tr>");

                $.each(data.d, function (key, value) {
                    $('#FileExample .record').append("<td>" + value + "</td>");
                });
            }
        });
    },

    uploadFile: function () {
        $("#instellingenCSV").hide();
        $("#instellingenXML").hide();
        $("#instellingen").show();

        var Name = $('#subscriberFile').val()
       
        $('#bitImportSubscribersDialog form').iframePostForm({
                iframeID: 'importSubscriberFrame',
                json: true,
                URL: "NewsletterService.asmx",
                type: "POST",
                post: function () {
                    $('#bitAjaxLoaderContainer').fadeIn();
                },

                complete: function (data) {
                    BITNEWSLETTERSUBSCRIBER.loadNewsletterSubscribers();

                    BITNEWSLETTERSUBSCRIBER.importDefinition.FileName = data.FileName;
                    BITNEWSLETTERSUBSCRIBER.importDefinition.FileExtension = data.FileExtension
                    var Extension = data.FileExtension;

                    if (Extension == ".csv" || Extension == ".xml" || Extension == ".txt") {

                        $("#Upload-postback").html('Het uploaden is gelukt ');
                     
                        switch (Extension) {
                            case ".csv":
                                $("#instellingenCSV").show();
                                $(".settingsTab").show();
                                $(".importsettingsSplash").hide();
                                break;
                            case ".xml":
                                $("#instellingenXML").show();
                                $(".settingsTab").show();
                                $(".importsettingsSplash").hide();
                                break;
                            //case ".txt":
                                //   $("#instellingen").html('Het importeren van tekstbstanden volgt nog.');
                                //   break;
                            default:
                                $("#instellingenCSV").hide();
                                $("#instellingenXML").hide();
                                $(".settingsTab").hide();
                                // $("#instellingenTXT").hide();
                                $(".importsettingsSplash").show();
                                break;      
                        }
                        BITNEWSLETTERSUBSCRIBER.fillDropDownImportDefinitions()
                    }
                    else {
                        $("#Upload-postback").html(Extension + ' is niet het juiste bestands type, kan niet uploaden.');
                    }
                },
            });
        
        var validation = $('#bitImportSubscribersDialog form').validate();
        if (validation) {
        $('#bitImportSubscribersDialog form').submit();
        }
    },

    loadImportDefinition: function (name) {

        var name = $('[name="importdefinitions"] option:selected').text();
        if (BITNEWSLETTERSUBSCRIBER.importDefinition.Name != name) {
            if (name != null) {
                BITNEWSLETTERSUBSCRIBER.importDefinition.Name = name;
                var id = $('[name="importdefinitions"] option:selected').val();
                var parametersObject = { id: id };
                var jsonstring = JSON.stringify(parametersObject);
                BITAJAX.dataServiceUrl = "NewsletterService.asmx";
                BITAJAX.callWebService("GetImportDefinition", jsonstring, function (data) {
           
                    BITNEWSLETTERSUBSCRIBER.newGroupList = data.d.Groups;

                    BITNEWSLETTERSUBSCRIBER.importDefinition.Delimiter = data.d.Delimiter;
                    BITNEWSLETTERSUBSCRIBER.importDefinition.ID = id
                    $('#bitImportSubscribersDialog').dataBind(data.d)

                    $('#selectForeNameColumn').val(data.d.ForeNameColumnNo);
                    $('#selectNamePrefixColumn').val(data.d.NamePrefixColumnNo);
                    $('#selectNameColumn').val(data.d.NameColumnNo);
                    $('#selectEmailColumn').val(data.d.EmailColumnNo);
                    //$('#selectGenderColumn').val(data.d.GenderColumnNo);

                    //vull de select met de groepen uit de definitie
                    $('#importNewsgroupSelector').val('').trigger("liszt:updated");
                    $.each(BITNEWSLETTERSUBSCRIBER.newGroupList, function (i, val) {
                        $('#importNewsgroupSelector option[value=' + val.ID + ']').prop('selected', true);
                        $('#importNewsgroupSelector').trigger("liszt:updated");
                    });
                });
            }
        }
    },

    fillDropDownImportDefinitions: function () {
        var parametersObject = { FileExtension: BITNEWSLETTERSUBSCRIBER.importDefinition.FileExtension };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebService("GetChoosableImportDefinitions", jsonstring, function (data) {
            BITNEWSLETTERSUBSCRIBER.definitions = data.d;
            $('#loadImportDefinitions').fillDropDown(data.d, { value: "00000000-0000-0000-0000-000000000000", text: "" });
        });
    },

    fillDropDownNewsLetterGroups: function () {

        BITAJAX.dataServiceUrl = "NewsletterService.asmx";
        BITAJAX.callWebService("LoadNewsletterGroupList", null, function (data) {
            BITNEWSLETTERSUBSCRIBER.newsGroups = data.d;
            $('#importNewsgroupSelector').fillDropDown(BITNEWSLETTERSUBSCRIBER.newsGroups, null, 'ID');
            $('#importNewsgroupSelector').chosen();

        });
    },

    saveImportDefinition: function () {

            if ($('[name="savedefinition"]').is(':checked') == true) {
                $('#saveimportdefinition').show();
            }
            else {
                $('#saveimportdefinition').hide();
                $('definitionname').empty();
            }
        },

    showImportDefinitionPage: function () {
        BITNEWSLETTERSUBSCRIBER.type = "Templates";
        $('#tableUsers').hide();
        $('#tableImportDefinitions').show();
        $('#bitplateTitlePage').html('Import templates');
    },

    showSubscribers: function () {
        BITNEWSLETTERSUBSCRIBER.type = "Subscribers";
        $('#tableImportDefinitions').hide();
        $('#tableUsers').show();
        $('#bitplateTitlePage').html('Nieuwsbrief abonnees');
    },

    DownloadImportLog: function () {
        var ImportLogFile = BITNEWSLETTERSUBSCRIBER.importDefinition.ImportLogFile;
        if (ImportLogFile != "") {
            //response.setHeader("Set-Cookie", "fileDownload=true; path=/");
            
            window.location =ImportLogFile;


        }
    }

 /*$('#bitImportSubscribersDialog').collectData(BITNEWSLETTERSUBSCRIBER.importDefinition);
        BITNEWSLETTERSUBSCRIBER.importDefinition.ForeNameColumn = $('#selectForeNameColumn option:selected').text();
        BITNEWSLETTERSUBSCRIBER.importDefinition.NamePrefixColumn = $('#selectNamePrefixColumn option:selected').text();
        BITNEWSLETTERSUBSCRIBER.importDefinition.NameColumn = $('#selectNameColumn option:selected').text();
        BITNEWSLETTERSUBSCRIBER.importDefinition.EmailColumn = $('#selectEmailColumn option:selected').text();
        BITNEWSLETTERSUBSCRIBER.importDefinition.Name = $('#definitionname').val();
            var jsonstring = JSON.fromObject(BITNEWSLETTERSUBSCRIBER.importDefinition);
            BITAJAX.dataServiceUrl = "NewsletterService.asmx";
            BITAJAX.callWebService("SaveImportDefinition", jsonstring, function (data) {
                if (data.d.Success) {
                    //$('#savedialog').dialog('close');
                    $('#save').html('Het opslaan is gelukt');
                }
                else {
                    $('#definitionname').append('VUL IN');
                }
            });
            var validation = $('savedialog').validate();
            if (validation) {
                $('savedialog').submit();
        }  */
    

}