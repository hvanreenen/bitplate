
$(document).ready(function () {
    BITEDITTEMPLATE.initialize();
});

var BITEDITTEMPLATE = {
    template: null,
    loadedScriptsCounter: 0,
    totalScripts: 0,
    siteDomein: null,
    oldMode: null,
    editor: null,
    HtmlCompleteContent: null,

    initialize: function () {
        $('#bitTemplateEditDialog').initDialog(function () {
            BITEDITTEMPLATE.saveTemplate();
            $('#bitTemplateEditDialog').dialog('close');
        }, { width: window.innerWidth, height: window.innerHeight }, true, undefined);
    },

    loadTemplate: function (id) {
        var parametersObject = { id: id, type: BITTEMPLATES.templateType };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "TemplateService.asmx";
        BITAJAX.callWebService("GetTemplate", jsonstring, function (data) {
            BITEDITTEMPLATE.template = data.d;
            //console.log(BITEDITTEMPLATE.template);
            BITEDITTEMPLATE.editTemplate();
        });
    },

    editTemplate: function () {
        //BITEDITTEMPLATE.template = BITEDITTEMPLATE.template;
        $('title').html("Bewerk template: " + BITEDITTEMPLATE.template.Name);
        var completeContent = BITEDITTEMPLATE.template.Content; //head + body
        //var bodyContent = HTMLHELPER.getBodyContent(completeContent); //only body
        $('#bitTemplateEditDialog').dialog('open');
        $('#bitTemplateEditorWrapper').show();
        $('#BITEDITTEMPLATEBody').hide();
        BITEDITTEMPLATE.HtmlCompleteContent = '';
        var editor = CKEDITOR.replace('bitTemplateEditor', {
            height: $('#bitTemplateEditDialog').height() - 145,
            filebrowserBrowseUrl: '/_bitplate/Dialogs/Finder.aspx',
            filebrowserImageBrowseUrl: '/_bitplate/Dialogs/Finder.aspx',
            on: {
                instanceReady: function (e) {
                    oldMode = e.editor.mode;
                    BITAJAX.dataServiceUrl = "TemplateService.asmx";
                    var parametersObject = { NewsletterMode: (BITTEMPLATES.templateType == 0) ? false : true };
                    var jsonstring = JSON.stringify(parametersObject);
                    BITAJAX.callWebService("GetTags", jsonstring, function (data) {
                        setTags(data.d);
                    });
                },
                mode: function (e) {
                    if (e.editor) {
                        if (e.editor.mode == 'wysiwyg') {
                            e.editor.config.fullPage = false;
                            if (BITEDITTEMPLATE.oldMode) {
                                BITEDITTEMPLATE.template.Content = BITEDITTEMPLATE.HtmlCompleteContent;
                            }
                            e.editor.setData(HTMLHELPER.getBodyContent(BITEDITTEMPLATE.HtmlCompleteContent));
                        }
                        else {
                            BITEDITTEMPLATE.template.Content = HTMLHELPER.replaceBodyContent(BITEDITTEMPLATE.HtmlCompleteContent, BITEDITTEMPLATE.template.Content);
                            e.editor.config.fullPage = true;
                            e.editor.setData(BITEDITTEMPLATE.template.Content);
                            if (e.editor.plugins.codemirror) {
                                if (window["codemirror_" + e.editor.id]) window["codemirror_" + e.editor.id].setValue(BITEDITTEMPLATE.template.Content);
                            }
                        }
                        BITEDITTEMPLATE.oldMode = e.editor.mode;
                    }
                },
                beforeSetMode: function (e) {
                    if (BITEDITTEMPLATE.oldMode) {
                        if (e.editor.plugins.codemirror && BITEDITTEMPLATE.oldMode == 'source') {
                            BITEDITTEMPLATE.HtmlCompleteContent = window["codemirror_" + e.editor.id].getValue();
                        }
                        else {
                            BITEDITTEMPLATE.HtmlCompleteContent = e.editor.getData();
                        }
                    }
                    else {
                        BITEDITTEMPLATE.HtmlCompleteContent = BITEDITTEMPLATE.template.Content;
                        e.editor.mode = 'wysiwyg';
                    }
                }
            }
        });
        editor.config.fullPage = false;
        editor.config.extraPlugins = 'tags,codemirror';
        //editor.config.extraPlugins = 'tags';
        //editor.config.extraPlugins = 'codemirror'; //Experimenteel
        //console.log(HTMLHELPER.getBodyContent(completeContent));
        


        //zet de save functie
        ///BITEDITOR.onSave = BITEDITTEMPLATE.saveTemplate;

        //maak pagina en module menu disabled
        $('ul#bitMenuModules').attr("disabled", "disabled");
        $('li#bitMenuConfigPage').attr("disabled", "disabled");

        //BITEDITTEMPLATE.fillMenuScripts();
        BITEDITTEMPLATE.attachScriptsToHead();

        $('#bitTemplateEditDialog').on("dialogclose", function (event, ui) {
            if (CKEDITOR.instances.bitTemplateEditor) {
                CKEDITOR.instances.bitTemplateEditor.destroy();
                BITEDITTEMPLATE.removeScriptsFromHead();
                BITEDITTEMPLATE.oldMode = null;
            }
        });
        BITEDITTEMPLATE.editor = editor;
        //editor.focus();
        //console.log(CKEDITOR.config.contentsCss);
    },

    saveTemplate: function () {
        /* if (BITEDITOR.currentEditor.mode == BITEDITOR.EDITORMODE.VIEWCODE) {
            var html = BITEDITOR.currentEditor.getHtml();
            BITEDITTEMPLATE.template.Content = html;
        }
        else {
            var completeContent = BITEDITOR.currentEditor.getCompleteHtml();
            var bodyContent = HTMLHELPER.getBodyContent(completeContent); //BITEDITOR.currentEditor.getHtml(); //laatste wijzigingen
            var html = HTMLHELPER.replaceBodyContent(bodyContent, completeContent);
            BITEDITTEMPLATE.template.Content = html;
        } */
        ///BITEDITTEMPLATE.template.Content = BITEDITOR.currentEditor.getCompleteHtml();

        BITEDITTEMPLATE.HtmlCompleteContent = BITEDITTEMPLATE.editor.getData();

        if (BITEDITTEMPLATE.editor.mode == 'wysiwyg') {
            BITEDITTEMPLATE.template.Content = HTMLHELPER.replaceBodyContent(BITEDITTEMPLATE.HtmlCompleteContent, BITEDITTEMPLATE.template.Content);
        }
        else {
            BITEDITTEMPLATE.template.Content = BITEDITTEMPLATE.HtmlCompleteContent;
        }

        var jsonstring = convertToJsonString(BITEDITTEMPLATE.template);

        BITAJAX.dataServiceUrl = "TemplateService.asmx";

        BITAJAX.callWebService("SaveTemplate", jsonstring, function () {
            $('#bitTemplateEditDialog').dialog('close');
        });
    },


    attachScriptsToHead: function () {

        BITEDITTEMPLATE.loadedScriptsCounter = 0;
        var scripts = BITEDITTEMPLATE.template.Scripts;
        var head = document.getElementsByTagName('head')[0];

        //bereken totaal aantal js-scripts
        for (var i in scripts) {
            var script = scripts[i];
            if (script.ActiveInEditor && script.ScriptType == 1) {
                BITEDITTEMPLATE.totalScripts++;
            }
        }

        CKEDITOR.config.contentsCss = Array();
        for (var i in scripts) {
            var script = scripts[i];
            if (script.ActiveInEditor) {
                if (script.ScriptType == 0) {
                    var link = document.createElement('link');
                    link.type = 'text/css';
                    link.rel = "stylesheet";
                    $(link).attr('data-attached-script', true);
                    var timeticks = new Date().getTime();
                    var serviceUrl = "CSSParser.aspx?url=" + script.Url + "&type=template";
                    //link.href = '../' + script.Url + '?time=' + timeticks;
                    var httpLink = serviceUrl + '&time=' + timeticks;
                    link.href = httpLink;
                    link.id = script.ID;
                    //head.appendChild(link);
                    
                    CKEDITOR.config.contentsCss.push(BITEDITTEMPLATE.siteDomein + '/' + script.Url);
                }
                else {
                    var jsscript = document.createElement('script');
                    $(jsscript).attr('data-attached-script', true);
                    jsscript.type = 'text/javascript';
                    var timeticks = new Date().getTime();
                    var serviceUrl = "JSParser.aspx?url=" + script.Url;
                    /*HACK*/
                    if (script.Url.indexOf('bitModule') !== -1) {
                        //jsscript.src = '../' + script.Url + '?time=' + timeticks;
                        jsscript.src = script.Url + '?time=' + timeticks;
                    }
                    else {
                        jsscript.src = '../' + script.Url + '?time=' + timeticks;
                    }
                    //jsscript.src = script.Url + '?time=' + timeticks;
                    //jsscript.src = serviceUrl;
                    jsscript.id = script.ID;
                    jsscript.onreadystatechange = function () {
                        if (jsscript.readyState == 'loaded') BITEDITTEMPLATE.onScriptLoaded();
                    }
                    jsscript.onload = BITEDITTEMPLATE.onScriptLoaded;
                    try {
                        //head.appendChild(jsscript);
                    }
                    catch (ex) {
                    }

                }
            }
        }

    },

    removeScriptsFromHead: function () {
        $('[data-attached-script="true"]').remove();
    },

    onScriptLoaded: function () {
        //Template
        BITEDITTEMPLATE.loadedScriptsCounter++;
        var totalScripts = BITEDITTEMPLATE.template.Scripts.length;
        if (BITEDITTEMPLATE.loadedScriptsCounter == BITEDITTEMPLATE.totalScripts) {
            //BITALLMODULES.loadAllModules();
            $(document).trigger('CMSReady', ['Custom', 'Event']);
        }
    }
}