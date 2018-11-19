/// <reference path="../_js/jquery-1.7.2-vsdoc.js" />
/// <reference path="../../_js/prototypes/databind.js" />

$(document).ready(function () {
    BITTEMPLATES.initialize();
    BITTEMPLATES.loadTemplates('');
    BITTEMPLATES.loadScripts();
});

var BITTEMPLATES = {
    currentSort: "Name ASC",
    searchString: "",
    template: null,
    popup: null,
    scriptsPopup: null,
    templateHtmlEditor: null,
    authTabIsLoaded: false,
    templateType: 0, // 0 = PaginaTemplate, 1 = NieuwsbriefTemplate;
    tags: null,

    initialize: function () {
        var type = BITUTILS.getQueryString()["type"];
        if (type == 'page') {
            BITTEMPLATES.templateType = 0;
            $('#bitplateTitlePage').html('Pagina Templates');
        }
        else {
            BITTEMPLATES.templateType = 1;
            $('#bitplateTitlePage').html('Nieuwsbrief Templates');
            $('.pagepropperty').remove();
        }

        BITAJAX.dataServiceUrl = "TemplateService.asmx";
        $('.button').button();
        $('#bitTemplateDialog').initDialog(BITTEMPLATES.saveTemplate, { width: '90%', height: window.innerHeight });
        $('#bitScriptDialog').initDialog(null, { width: 200, height: 400 }, false);
        $('#bitPopups').formEnrich();
        $(".bitTabs").tabs({
            show: function (event, ui) { BITTEMPLATES.templateHtmlEditor.mirror.refresh(); }
        });
        $(".bitTabs").tabs({ "cache": true });
        $(".bitTabs").tabs("add", "/_bitplate/Dialogs/AutorisationTab.asmx", "Autorisatie");
        $(".bitTabs").bind("tabsload", function (event, ui) {
            BITAUTORISATIONTAB.hideSiteUserGroupsAndUsers = true; //alleen auth op bitplate users
            BITAUTORISATIONTAB.currentObject = BITTEMPLATES.template;
            BITAUTORISATIONTAB.initialize();
            BITAUTORISATIONTAB.dataBind();
            BITTEMPLATES.authTabIsLoaded = true;
        });
        $('#bitSearchTextbox').searchable(BITTEMPLATES.search);
        $('#bitTagsDialog').initDialog(null, { width: 300, height: 500 }, false);
    },

    loadTemplates: function (sort) {
        BITAJAX.dataServiceUrl = "TemplateService.asmx";
        if (sort) BITTEMPLATES.currentSort = sort;
        var parametersObject = { sort: BITTEMPLATES.currentSort, searchString: BITTEMPLATES.searchString, templateType: BITTEMPLATES.templateType };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "TemplateService.asmx";
        BITAJAX.callWebServiceASync("GetTemplatesLite", jsonstring, function (data) {
            $("#tableTemplates").dataBindList(data.d, {
                onSort: function (sort) {
                    BITTEMPLATES.loadTemplates(sort);
                },
                onRowBound: function (obj, index, html) {
                    var tempWrapperDiv = document.createElement('div');
                    $(tempWrapperDiv).html(html);
                    if (!obj.IsActive) {
                        $(tempWrapperDiv).find('tr').addClass("inactive");
                        $(tempWrapperDiv).find('tr').css("color", "gray");
                    }
                    if (obj.HasAutorisation) {
                        $(tempWrapperDiv).find('.bitTableAuthIcon').css("display", "inline-block");
                    }
                    html = $(tempWrapperDiv).html();
                    return html;
                }
            });
        });
    },

    loadScripts: function () {
        BITAJAX.dataServiceUrl = "/_bitplate/Scripts/ScriptService.asmx";
        BITAJAX.callWebServiceASync("LoadScripts", null, function (data) {
            $('#bitScriptList').dataBindList(data.d);
        });
    },

    openDetailsPopup: function (id, activeTab) {
        var parametersObject = { id: id, type: BITTEMPLATES.templateType };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "TemplateService.asmx";
        BITAJAX.callWebServiceASync("GetTemplate", jsonstring, function (data) {
            BITTEMPLATES.template = data.d;
            if (BITTEMPLATES.templateHtmlEditor != null) {
                BITTEMPLATES.templateHtmlEditor.toTextArea();
            }
            
            $("#bitTemplateDialog").dataBind(BITTEMPLATES.template);
            
            if (BITTEMPLATES.template.IsNew) {
                $('#bitTemplateDialog').dialog({ title: "Nieuwe template" });
                BITTEMPLATES.template.IsNewsletterTemplate = (BITTEMPLATES.templateType == 0) ? false: true; //set templatetype)
                $("#tableScriptsPerSite").dataBindList(BITTEMPLATES.template.SiteScripts);
                $("#tableScriptsPerTemplate").dataBindList(BITTEMPLATES.template.Scripts);
            }
            else {
                $('#bitTemplateDialog').dialog({ title: "Template: " + BITTEMPLATES.template.Name });
                $("#tableScriptsPerSite").dataBindList(BITTEMPLATES.template.SiteScripts);
                $("#tableScriptsPerTemplate").dataBindList(BITTEMPLATES.template.Scripts);
            }
            if (!BITTEMPLATES.template.LanguageCode) {
                $('#dropdownLanguages').val($('#dropdownLanguages option[data-cms-default-language="true"]').val());
            }
            
            if (BITTEMPLATES.templateType == 1) {
                $('#liScripts').hide();
                $('#tabPageDetails2').hide();
            }

            $('#bitTemplateUsedInPages').dataBindList(data.d.UsedInPages);
            activeTab = (activeTab) ? activeTab : 0;
            $('#bitTemplateDialog .bitTabs').tabs("option", "active", activeTab);

            if (!BITTEMPLATES.tags) {
                BITAJAX.dataServiceUrl = "TemplateService.asmx";
                var parametersObject = { NewsletterMode: (BITTEMPLATES.templateType == 0) ? false : true };
                var jsonstring = JSON.stringify(parametersObject);
                BITAJAX.callWebServiceASync("GetTags", jsonstring, function (data) {
                    BITTEMPLATES.tags = data.d;
                    $('#bitTagsDialog').html('');
                    var tagElementList = '';
                    $(BITTEMPLATES.tags).each(function () {
                        tagElementList += '<div class="tagToInsert">' + this.toString() + '</div>';
                    });
                    $('#bitTagsDialog').html(tagElementList);
                    BITTEMPLATES.initializeCodeMirror();
                    
                    $('#bitTemplateDialog').dialog('open');
                    $('.bitTabs').height($("#bitTemplateDialog").height() - 20);
                    var height = $('#bitTemplateHtmlEditor').parent().parent().parent().height() - 100;
                    $('#bitTemplateHtmlEditor').parent().height(height);
                    $('.bitTabs').tabs('option', 'heightStyle', 'fill');
                    $('.bitTabs').tabs('refresh');
                    BITTEMPLATES.templateHtmlEditor.mirror.refresh();
                });
            }
            else {
                BITTEMPLATES.initializeCodeMirror();
                
                $('#bitTemplateDialog').dialog('open');
                $('.bitTabs').height($("#bitTemplateDialog").height() - 20);
                var height = $('#bitTemplateHtmlEditor').parent().parent().parent().height() - 100;
                $('#bitTemplateHtmlEditor').parent().height(height);
                $('.bitTabs').tabs('option', 'heightStyle', 'fill');
                $('.bitTabs').tabs('refresh');
                BITTEMPLATES.templateHtmlEditor.mirror.refresh();
            }
            

            //BITTEMPLATES.templateHtmlEditor = CodeMirror.fromTextArea(document.getElementById('bitTemplateHtmlEditor'), { mode: "text/html", tabMode: "indent", lineNumbers: true});
            //BITTEMPLATES.templateHtmlEditor.refresh();
            if (BITTEMPLATES.authTabIsLoaded) {
                BITAUTORISATIONTAB.currentObject = BITTEMPLATES.template;
                BITAUTORISATIONTAB.dataBind();
            }
        });
    },

    initializeCodeMirror: function () {

        CodeMirror.commands.autocomplete = function (cm) {
            /***********************************************************************
            {list: getCompletions(token, context, keywords, options),
                from: Pos(cur.line, token.start),
                to: Pos(cur.line, token.end)};
            ***********************************************************************/

            CodeMirror.showHint(cm, function (cm, options) {
                var htmlHintObject = CodeMirror.htmlHint(cm, options);



                htmlHintObject['list'] = BITTEMPLATES.tags.concat(htmlHintObject['list']);

                var cur = cm.getCursor(), token = cm.getTokenAt(cur);
                var inner = CodeMirror.innerMode(cm.getMode(), token.state);
                if (inner.mode.name != "xml") return;
                var result = [], replaceToken = false, prefix;
                var isTag = token.string.charAt(0) == "{";
                if (!inner.state.tagName || isTag) { // Tag completion
                    if (isTag) {
                        prefix = token.string.slice(1);
                        htmlHintObject['list'] = Array();

                        for (var tag in BITTEMPLATES.tags) {
                            if (BITTEMPLATES.tags[tag].toLowerCase().indexOf(prefix.toLowerCase()) !== -1) {
                                htmlHintObject['list'].push(BITTEMPLATES.tags[tag].replace('{' + prefix, ''));
                            }
                        }
                    }
                }
                return htmlHintObject;
            });
        }

        var uiOptions = { path: '/_bitplate/_js/plugins/codemirror-ui/js/', searchMode: 'inline', buttons: ['undo', 'redo', 'jump', 'reindentSelection', 'reindent', 'tag'], tagCallBack: BITTEMPLATES.showTagsPopup };
        var codeMirrorOptions = {
            mode: "text/html",
            tabMode: "indent",
            lineNumbers: true,
            extraKeys: { "Ctrl-Space": "autocomplete" },
            theme: 'default',
            matchBrackets: true,
            workDelay: 300,
            workTime: 35,
            readOnly: false,
            lineNumbers: true,
            lineWrapping: true,
            autoCloseTags: true,
            autoCloseBrackets: true,
            highlightSelectionMatches: true,
            continueComments: true
        }

        BITTEMPLATES.templateHtmlEditor = new CodeMirrorUI(document.getElementById('bitTemplateHtmlEditor'), uiOptions, codeMirrorOptions);
        //BITTEMPLATES.templateHtmlEditor = CodeMirror.fromTextArea(document.getElementById('bitTemplateHtmlEditor'), {
        //    mode: "text/html",
        //    tabMode: "indent",
        //    lineNumbers: true,
        //    extraKeys: { "Ctrl-Space": "autocomplete" },
        //    theme: 'default',
        //    matchBrackets: true,
        //    workDelay: 300,
        //    workTime: 35,
        //    readOnly: false,
        //    lineNumbers: true,
        //    lineWrapping: true,
        //    autoCloseTags: true,
        //    autoCloseBrackets: true,
        //    highlightSelectionMatches: true,
        //    continueComments: true
        //});
    },
    isTagsPopupOpen: false,
    showTagsPopup: function () {
        $("#bitTagsDialog").dialog("open");
        $('.tagToInsert').unbind().dblclick(function () {
            var tag = $(this).html();

            BITTEMPLATES.templateHtmlEditor.mirror.replaceSelection(tag, 'start');
            var currentCursorPosition = BITTEMPLATES.templateHtmlEditor.mirror.getCursor();
            BITTEMPLATES.templateHtmlEditor.mirror.setCursor(currentCursorPosition);
        });
        BITTEMPLATES.isTagsPopupOpen = true;
        /* if (BITEDITOR.currentEditor) {
            var id = BITEDITOR.currentEditor.id;
            BITEDITPAGE.refreshTagsPopup(id);
        } */

    },

    newTemplate: function () {
        this.openDetailsPopup(null);
    },

    saveTemplate: function () {
        BITTEMPLATES.templateHtmlEditor.mirror.save();
        //opslaan 
        var validation = $('#bitTemplateDialog').validate();
        if (validation) {
            //get data from panel
            BITTEMPLATES.template = $('#bitTemplateDialog').collectData(BITTEMPLATES.template);
            jsonstring = convertToJsonString(BITTEMPLATES.template);

            BITAJAX.dataServiceUrl = "TemplateService.asmx";
            BITAJAX.callWebServiceASync("SaveTemplate", jsonstring, function (data) {
                $('#bitTemplateDialog').dialog('close');
                //reload grid
                BITTEMPLATES.loadTemplates();
            });
        }
    },

    remove: function (id) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u deze template verwijderen?", null, function () {

            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "TemplateService.asmx";


            BITAJAX.callWebServiceASync("DeleteTemplate", jsonstring, function (data) {
                BITTEMPLATES.loadTemplates();
            });
        });
    },

    search: function (searchName) {
        BITTEMPLATES.searchString = searchName;
        BITTEMPLATES.loadTemplates();
    },

    copy: function (id, name) {
        var newName = name + ' (kopie)';
        $.inputBox('Kopie naam:', 'Template kopiëren.', newName, function (e, value) {
            var newName = value;
            var parametersObject = { TemplateId: id, newTemplateName: newName };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "TemplateService.asmx";
            BITAJAX.callWebServiceASync("CopyTemplate", jsonstring, function (data) {
                BITTEMPLATES.loadTemplates();
                
            });
        });
    },

    openScriptsPopup: function () {
        $('#bitScriptDialog').dialog('open');
    },

    addScript: function (id, name) {
        var script = new Object();
        script.ID = id;
        script.CompleteName = name;
        

        if (BITTEMPLATES.template.Scripts == null) {
            BITTEMPLATES.template.Scripts = [];
        }
        //kijk of die er al in zit
        var contains = false;
        for (var i in BITTEMPLATES.template.SiteScripts) {
            var scriptCheck = BITTEMPLATES.template.SiteScripts[i];
            if (scriptCheck.ID == script.ID) {
                contains = true;
                break;
            }
        }
        if (!contains) {
            for (var i in BITTEMPLATES.template.Scripts) {
                var scriptCheck = BITTEMPLATES.template.Scripts[i];
                if (scriptCheck.ID == script.ID) {
                    contains = true;
                    break;
                }
            }
        }
        if (contains) {
            alert("Koppeling met dit script bestaat al");
        }
        else {
            BITTEMPLATES.template.Scripts.push(script);
            $("#tableScriptsPerTemplate").dataBindList(BITTEMPLATES.template.Scripts);

        }
    },

    removeScript: function (id, index) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DETACHCONFIRM, "Weet u zeker dat u dit script wilt los koppelen van deze template?", null, function () {
            BITTEMPLATES.template.Scripts.splice(index, 1);
            $("#tableScriptsPerTemplate").dataBindList(BITTEMPLATES.template.Scripts);
        });
    }
};