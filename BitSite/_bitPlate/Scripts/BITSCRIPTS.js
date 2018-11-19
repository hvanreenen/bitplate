/// <reference path="../_js/jquery-1.7.2-vsdoc.js" />
/// <reference path="../../_js/prototypes/databind.js" />

$(document).ready(function () {
    BITSCRIPTS.initialize();
    BITSCRIPTS.loadScripts('');

});

var BITSCRIPTS = {
    currentSort: "Name ASC",
    searchString: "",
    script: null,
    codeMirrorEditor: null,
    authTabIsLoaded: false,
    currentScriptType: null,

    initialize: function () {
        $('#bitScriptDialog').initDialog(BITSCRIPTS.saveScript, { width: '90%', height: window.innerHeight });
        
        $('#bitPopups').formEnrich();
        $(".bitTabs").tabs({
            show: function (event, ui) { BITSCRIPTS.codeMirrorEditor.mirror.refresh(); }
        });
        $(".bitTabs").tabs({ "cache": true });
        $(".bitTabs").tabs("add", "/_bitplate/Dialogs/AutorisationTab.aspx", "Autorisatie");
        $(".bitTabs").bind("tabsload", function (event, ui) {
            BITAUTORISATIONTAB.hideSiteUserGroupsAndUsers = true;
            BITAUTORISATIONTAB.currentObject = BITSCRIPTS.script;
            BITAUTORISATIONTAB.initialize();
            BITAUTORISATIONTAB.dataBind();
            BITSCRIPTS.authTabIsLoaded = true;
        });
        var type = (BITUTILS.getQueryString()) ? BITUTILS.getQueryString()["type"] : 'css';
        if (type == 'css') {
            $('#selectScriptType').val('0');
            $('#bitplateTitlePage').html('Stylesheets');
            BITSCRIPTS.currentScriptType = 'css';
            $('input[type="file"]').attr('accept', 'text/css');

        }
        else {
            $('#selectScriptType').val('1');
            $('#bitplateTitlePage').html('Javascripts');
            BITSCRIPTS.currentScriptType = 'js';
            $('input[type="file"]').attr('accept', 'text/javascript');
        }
        $('#bitSearchTextbox').searchable(BITSCRIPTS.search);
        $('#fileupload').unbind().bind('fileuploadstopped', function (e, data) { BITSCRIPTS.UploadComplete(e, data); });
        
    },

    loadScripts: function (sort) {
        if (sort) BITSCRIPTS.currentSort = sort;
        var parametersObject = { sort: BITSCRIPTS.currentSort, searchString: BITSCRIPTS.searchString, scriptMode: $('#selectScriptType').val() };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "ScriptService.asmx";
        BITAJAX.callWebServiceASync("GetScripts", jsonstring, function (data) {
            $("#tableScripts").dataBindList(data.d, {
                onSort: function (sort) {
                    BITSCRIPTS.loadScripts(sort);
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
                    if (obj.IsSystemValue) {
                        $(tempWrapperDiv).find("#tdScriptRemove").find("a").hide();
                    }
                    html = $(tempWrapperDiv).html();
                    return html;
                }
            });
        });
    },

    openDetailsPopup: function (id) {
        var uiOptions = { path: '/_bitplate/_js/plugins/codemirror-ui/js/', searchMode: 'inline', buttons: ['undo', 'redo', 'jump', 'reindentSelection', 'reindent'] }
        var codeMirrorOptions;
        var parametersObject = { id: id };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "ScriptService.asmx";
        BITAJAX.callWebServiceASync("GetScript", jsonstring, function (data) {
            //script = data.d;
            if (BITSCRIPTS.codeMirrorEditor != null) {
                BITSCRIPTS.codeMirrorEditor.toTextArea();
            }
            BITSCRIPTS.script = data.d;
            if (BITSCRIPTS.script.IsSystemValue) {
                $("#divSystemValue").show();
                $("#textNaam").attr('disabled', 'disabled');
            }
            else {
                $("#divSystemValue").hide();
                $("#textNaam").removeAttr('disabled');
            }
            $("#bitScriptDialog").dataBind(BITSCRIPTS.script);
            if (BITSCRIPTS.authTabIsLoaded) {
                BITAUTORISATIONTAB.currentObject = BITSCRIPTS.script;
                BITAUTORISATIONTAB.dataBind();
            }
            var scriptType = $('#selectScriptType').val();
            if (scriptType == '1') {
                CodeMirror.commands.autocomplete = function (cm) {
                    CodeMirror.showHint(cm, CodeMirror.javascriptHint);
                }
                codeMirrorOptions = {
                    mode: "javascript",
                    tabMode: "indent",
                    lineNumbers: true,
                    extraKeys: { "Ctrl-Space": "autocomplete" },
                    gutters: ["CodeMirror-lint-markers"],
                    lintWith: CodeMirror.javascriptValidator
                };
                BITSCRIPTS.codeMirrorEditor = new CodeMirrorUI(document.getElementById('textareaContent'), uiOptions, codeMirrorOptions); //CodeMirror.fromTextArea(document.getElementById('textareaContent'), {
            }
            else {
                codeMirrorOptions = { mode: "css", tabMode: "indent", lineNumbers: true };
                BITSCRIPTS.codeMirrorEditor = new CodeMirrorUI(document.getElementById('textareaContent'), uiOptions, codeMirrorOptions);//CodeMirror.fromTextArea(document.getElementById('textareaContent'), { mode: "css", tabMode: "indent", lineNumbers: true });
            }

            if (BITSCRIPTS.script.IsNew) {
                $('#bitScriptDialog').dialog({ title: "Nieuw Script" });
                $('.bitTabs').tabs("select", 0);
                $('#textNaam').focus();
            }
            else {
                $('#bitScriptDialog').dialog({ title: "Script: " + BITSCRIPTS.script.Name });
                $('.bitTabs').tabs("select", 1);
                $('#textareaContent').focus();
            }
            
            $("#bitScriptDialog").dialog('open');
            
            $('.bitTabs').height($("#bitScriptDialog").height() - 20);
            var height = $('#textareaContent').parent().parent().parent().height() - 100;
            $('#textareaContent').parent().height(height);
            $('.bitTabs').tabs('option', 'heightStyle', 'fill');
            $('.bitTabs').tabs('refresh');
            BITSCRIPTS.codeMirrorEditor.mirror.refresh();
        });
    },

    newScript: function () {
        this.openDetailsPopup(null);
    },

    saveScript: function () {
        BITSCRIPTS.codeMirrorEditor.mirror.save();
        //opslaan page config
        $('#ScriptType').val($('#selectScriptType').val());
        var validation = $('#bitScriptDialog').validate();
        if (validation) {
            //get data from panel
            BITSCRIPTS.script = $('#bitScriptDialog').collectData(BITSCRIPTS.script);
            jsonstring = convertToJsonString(BITSCRIPTS.script);

            BITAJAX.dataServiceUrl = "ScriptService.asmx";
            BITAJAX.callWebServiceASync("SaveScript", jsonstring, function (data) {
                $('#bitScriptDialog').dialog('close');
                //reload grid
                BITSCRIPTS.loadScripts();
            });
        }

    },

    remove: function (id) {
        $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, "Wilt u dit script verwijderen?", null, function () {

            var parametersObject = { id: id };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "ScriptService.asmx";

            BITAJAX.callWebServiceASync("DeleteScript", jsonstring, function (data) {
                BITSCRIPTS.loadScripts();
            });
        });
    },

    search: function (searchName) {
        BITSCRIPTS.searchString = searchName;
        BITSCRIPTS.loadScripts();
    },


    copy: function (id, name) {

        var newName = name + ' (kopie)';
        $.inputBox('Kopie naam:', 'Kopiëren', newName, function (e, value) {
            var newName = value;
            var parametersObject = { scriptId: id, newScriptName: newName };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "ScriptService.asmx";
            BITAJAX.callWebServiceASync("CopyScript", jsonstring, function (data) {
                BITSCRIPTS.loadScripts();
            });
        });

        //$('#CopyName').val(name + ' (kopie)');
        //$('#bitScriptsCopyDialog').dialog({
        //    width: 260,
        //    Height: 174,
        //    modal: false,
        //    buttons: {
        //        "Kopieer": function () {
        //            var parametersObject = { scriptId: id, TemplateName: $('#CopyName').val() };
        //            var jsonstring = JSON.stringify(parametersObject);
        //            BITAJAX.dataServiceUrl = "TemplateService.asmx";
        //            BITAJAX.dataServiceUrl = "ScriptService.asmx";
        //            BITAJAX.callWebServiceASync("CopyScript", jsonstring, function (data) {
        //                BITSCRIPTS.loadScripts();
        //                $('#bitScriptsCopyDialog').dialog('close');
        //            });
        //        },
        //        "Sluiten": function () {
        //            $('#bitScriptsCopyDialog').dialog('close');
        //        },
        //    },
        //    close: function () {
        //        $('#bitPageScriptDropDown').slideUp(function () {
        //            $('#bitPageScriptDropDown').remove();
        //        });
        //    }
        //});


        //var parametersObject = { scriptId: id };
        //var jsonstring = JSON.stringify(parametersObject);
       
    },

    openUploadPanel: function () {

        /* $('#ScriptUploadForm').iframePostForm({
            iframeID: 'iframe-post-form',
            json: false,
            post: function () {
                $('#SelectScript').slideUp('fast');
                $('#UploadScript').slideDown('fast');

                var percentComplete = 0; //Update this in your other script
                $("#uploadProgressbar").data("progress", setInterval(function () {
                    if (percentComplete == 100) {
                        percentComplete = 0;
                        clearInterval($("#uploadProgressbar").data("progress")); //Stop updating
                    }
                    $("#uploadProgressbar").progressbar({ value: percentComplete });
                }, 200));
            },
            complete: function (response) {
                $('#UploadScript').slideUp('fast');
                $('#SelectScript').slideDown('fast');
                BITSCRIPTS.loadScripts(BITSCRIPTS.currentSort);
                $('#bitScriptUploadDialog').dialog('close');
                $('input[name="SelectedScript"]').val('');
                $.messageBox(MSGBOXTYPES.INFO, 'Script upload bewerking is voltooid.');
            }
        }); */
        /* $('#bitScriptUploadDialog').initDialog(null, { width: 300, heigt: 200 }, false, {
            'Upload': function () {
                if ($('input[name="SelectedScript"]').val()) {
                    var validation = false;
                    var fileElement = $('input[name="SelectedScript"]')[0];
                    var fileExtension = "";
                    if (fileElement.value.lastIndexOf(".") > 0) {
                        fileExtension = fileElement.value.substring(fileElement.value.lastIndexOf(".") + 1, fileElement.value.length);
                    }
                    validation = (fileExtension == BITSCRIPTS.currentScriptType);

                    if (validation) {
                        $('#ScriptUploadForm').submit();
                    }
                    else {
                        $.messageBox(MSGBOXTYPES.ERROR, 'Dit is geen ' + BITSCRIPTS.currentScriptType + ' script.');
                    }
                    
                }
            },
            'Annuleren': function () {
                $('#bitScriptUploadDialog').dialog('close');
            }
        }); */
        

        $('#bitScriptUploadDialogv2').initDialog(null, { width: 1200  }, false, {
        });
        $('#bitScriptUploadDialogv2').dialog('open');
    },

    UploadComplete: function (e, data) {
        $('#bitScriptUploadDialogv2').dialog('close');
        BITSCRIPTS.loadScripts();
    },

    widgets: [],
    updateHints: function() {
        BITSCRIPTS.codeMirrorEditor.operation(function () {
            for (var i = 0; i < BITSCRIPTS.widgets.length; ++i)
                BITSCRIPTS.codeMirrorEditor.removeLineWidget(BITSCRIPTS.widgets[i]);
            BITSCRIPTS.widgets.length = 0;

            JSHINT(BITSCRIPTS.codeMirrorEditor.getValue());
            for (var i = 0; i < JSHINT.errors.length; ++i) {
                var err = JSHINT.errors[i];
                if (!err) continue;
                var msg = document.createElement("div");
                var icon = msg.appendChild(document.createElement("span"));
                icon.innerHTML = "!!";
                icon.className = "lint-error-icon";
                msg.appendChild(document.createTextNode(err.reason));
                msg.className = "lint-error";
                BITSCRIPTS.widgets.push(BITSCRIPTS.codeMirrorEditor.addLineWidget(err.line - 1, msg, { coverGutter: false, noHScroll: true }));
            }
        });
        var info = BITSCRIPTS.codeMirrorEditor.getScrollInfo();
        var after = BITSCRIPTS.codeMirrorEditor.charCoords({ line: BITSCRIPTS.codeMirrorEditor.getCursor().line + 1, ch: 0 }, "local").top;
        if (info.top + info.clientHeight < after)
            BITSCRIPTS.codeMirrorEditor.scrollTo(null, after - info.clientHeight + 3);
    }
};


function checkFile() {
    
}
