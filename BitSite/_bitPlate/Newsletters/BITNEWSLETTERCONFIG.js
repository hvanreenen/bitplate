var BITNEWSLETTERCONFIG = {
    Site: null,
    Pages: null,

    ckeditorToolbar: [
                { "name": "document", "groups": ["mode", "document", "doctools"], "items": ["Source", "-", "NewPage", "-", "Templates"] },
                { "name": "clipboard", "groups": ["clipboard", "undo"], "items": ["Cut", "Copy", "Paste", "PasteText", "PasteFromWord", "-", "Undo", "Redo"] },
                { "name": "editing", "groups": ["find", "selection", "spellchecker"], "items": ["Find", "Replace", "-", "SelectAll"] },
                { "name": "forms", "items": ["Form", "Checkbox", "Radio", "TextField", "Textarea", "Select", "Button", "ImageButton", "HiddenField"] },
                "/",
                { "name": "paragraph", "groups": ["list", "indent", "blocks", "align", "bidi"], "items": ["NumberedList", "BulletedList", "-", "Outdent", "Indent", "-", "Blockquote", "CreateDiv", "-", "JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock", "-", "BidiLtr", "BidiRtl"] },
                //"/", //Enter verzoek van Misja
                { "name": "links", "items": ["Link", "Unlink", "Anchor"] }, { "name": "insert", "items": ["Image", "Flash", "Table", "HorizontalRule", "Smiley", "SpecialChar", "PageBreak", "Iframe", "Tags"] },
                "/",
                { "name": "basicstyles", "groups": ["basicstyles", "cleanup"], "items": ["Bold", "Italic", "Underline", "Strike", "Subscript", "Superscript", "-", "RemoveFormat"] },
                { "name": "styles", "items": ["Styles", "Format", "Font", "FontSize"] },
                { "name": "colors", "items": ["TextColor", "BGColor"] },
                { "name": "tools", "items": ["ShowBlocks"] }
                //{ "name": "others", "items": ["-"] },
                //{ "name": "about", "items": ["About"] }
    ],

    initialize: function () {
        $('#panelDetails').formEnrich();
        BITNEWSLETTERCONFIG.loadNewsletterConfig();
        if ($('#selectOptInEmailPage option').size() == 0) {
            $('#selectOptInEmailPage').parent().html('U kunt geen optin pagina selecteren omdat er op geen van alle pagina\'s een optin module geplaatst is.');
        }

        if ($('#selectOptOutEmailPage option').size() == 0) {
            $('#selectOptOutEmailPage').parent().html('U kunt geen optout pagina selecteren omdat er op geen van alle pagina\'s een optin module geplaatst is.');
        }

        $('textarea').each(function (i) {
            var editor = CKEDITOR.replace(this, {
                filebrowserBrowseUrl: '/_bitplate/Dialogs/Finder.aspx',
                filebrowserImageBrowseUrl: '/_bitplate/Dialogs/Finder.aspx',
                filebrowserUploadUrl: '/_bitplate/Dialogs/Finder.aspx',
                filebrowserImageUploadUrl: '/_bitplate/Dialogs/Finder.aspx',
                toolbar: BITNEWSLETTERCONFIG.ckeditorToolbar,
            });
            if (i == 0) {
                editor.config.extraPlugins = 'tags';
                editor.on('loaded', function (evt) {
                    BITAJAX.dataServiceUrl = "NewsletterService.asmx";
                    BITAJAX.callWebService("GetOptInTemplateTags", null, function (data) {
                        setTagArray(data.d);
                    });
                });
            }
        });
    },

    loadNewsletterConfig: function () {
        $('#panelDetails').formEnrich();
        BITAJAX.dataServiceUrl = "/_bitplate/Site/SiteService.asmx";
        BITAJAX.callWebServiceASync("LoadSite", null, function (data) {
            $('#panelDetails').dataBind(data.d);
            BITNEWSLETTERCONFIG.Site = data.d;
            BITAJAX.dataServiceUrl = "NewsletterService.asmx";
            BITAJAX.callWebServiceASync("GetNewsletterLicenseInfo", null, function (data) {
                $('#newsletterLicenseInfo').dataBind(data.d);
            });
        });
        
    },

    saveNewsletterConfig: function () {
        //SYNC optin/out textarea with CKEDITOR
        for (var instanceName in CKEDITOR.instances)
            CKEDITOR.instances[instanceName].updateElement();

        if ($('#panelDetails').validate()) {
            BITNEWSLETTERCONFIG.Site = $('#panelDetails').collectData(BITNEWSLETTERCONFIG.Site);
            jsonstring = convertToJsonString(BITNEWSLETTERCONFIG.Site);
            BITAJAX.dataServiceUrl = "/_bitplate/Site/SiteService.asmx";
            BITAJAX.callWebService("SaveSite", jsonstring, function (data) {
                if (data.d.ID) {
                    $.messageBox(MSGBOXTYPES.INFO, 'Nieuwsbrief configuratie is opgeslagen', 'Configuratie opgeslagen.');
                }
            });
        }
    }

}