<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Finder.aspx.cs" Inherits="BitSite._bitPlate.bitDetails.ElFiles" EnableViewState="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Finder</title>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/BitCore") %>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/_bitplate/_themes/" + BitSite.SessionObject.CurrentBitplateUser.Theme + "/css/core") %>
    <link href="../_themes/bitplate/plugins/elFinder-2.0/css/elfinder.min.css" rel="stylesheet" type="text/css" />
    <link href="../_themes/bitplate/plugins/elFinder-2.0/css/theme.css" rel="stylesheet" type="text/css" />

    <script src="../_js/plugins/elFinder-2.0/elfinder.min.js" type="text/javascript"></script>
    <script src="../_js/plugins/elFinder-2.0/i18n/elfinder.nl.js" type="text/javascript"></script>

    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/_bitplate/_themes/" + BitSite.SessionObject.CurrentBitplateUser.Theme + "/css/grid") %>
    <script type="text/javascript" src="<%=ResolveClientUrl("/_bitplate/_js/plugins/modernizr.custom.59383.js")%>"></script>
    <script type="text/javascript" src="<%=ResolveClientUrl("/_bitplate/_js/plugins/jQuery.placeholder.js")%>"></script>
    <script src="/_bitplate/Pages/BITPAGES.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="/_bitplate/_themes/bitplate/plugins/jbreadcrump/Styles/BreadCrumb.css" />
    <script src="/_bitplate/_js/plugins/jbreadcrump/js/jquery.easing.1.3.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/jbreadcrump/js/jquery.jBreadCrumb.1.1.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITPAGES.initialize();
            BITPAGES.loadPages("");

            var querystring = BITUTILS.getQueryString();
            if (querystring['type'] && querystring['type'] == 'image') {
                $("#bitTabsSelect").tabs({ active: 1 });
            }
            else {
                $("#bitTabsSelect").tabs({ active: 0 });
            }
        });
    </script>
</head>
<body>
    <div id="bitTabsSelect"> <!-- <div id="bitTabsSelect" class="bitTabs">-->
        <ul>
            <li><a href="#tabPageSelectPage">Pagina</a></li>
            <li><a href="#tabPageSelectFile">Bestand</a></li>
        </ul>
        <!-- TAB1 -->
        <div id="tabPageSelectPage" class="bitTabPage">
            <div class="breadCrumb module">
                <ul data-control-type="list" id="breadcrump">
                    <li><a data-field="Path" data-title-field="Name" data-text-field="Name" href="javascript:BITPAGES.loadPages('', '[data-field]')"></a></li>
                </ul>
            </div>
            <table id="tablePages" class="bitGrid" data-control-type="table" cellspacing="0"
                cellpadding="2">
                <thead>
                    <tr>
                        <td class="bitTableColumn" style="width: 25px">&nbsp;</td>
                        <td class="bitTableColumn" style="width: 30px">&nbsp;</td>
                        <td class="bitTableColumn" style="width: 140px" data-sort-field="Name">Naam</td>
                        <td class="bitTableColumnTitle" style="width: 140px" data-sort-field="Title">Title</td>
                        <td class="bitTableColumnPath" style="display: none; width: 220px" data-sort-field="Path">Map</td>
                        <td class="bitTableColumn" style="width: 100px" data-sort-field="ChangeStatus">Status</td>
                        <td id="tdLangTitle" runat="server" visible="false" class="bitTableColumn" style="width: 40px" data-sort-field="LanguageCode">Taal</td>
                        <td runat="server" id="htd4" class="bitTableActionButton">Selecteer</td>
                        <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                        </td>
                    </tr>
                </thead>
                <tbody>
                    <tr data-id-field="ID">
                        <td class="icon"><a data-field="ID" href="CKEditPage.aspx?referrer=Pages.aspx#[data-field]" class="bitTablePageIcon"></a></td>
                        <td class="iconAuth"><a href="#" class="bitTableAuthIcon" style="display: none"></a></td>
                        <td class="bitTableColumnName bitTableColumnEllipsis" data-field="Name" data-title-field="Name"></td>
                        <td class="bitTableColumnTitle bitTableColumnEllipsis" data-field="Title" data-title-field="Title"></td>
                        <td class="bitTableColumnPath bitTableColumnEllipsis" data-field="Path" data-title-field="Path"></td>

                        <td data-field="Status"></td>
                        <td id="tdLanguageField" runat="server" visible="false" data-field="Field2">LanguageCode</td>
                        <td class="bitTableActionButton" id="td4" runat="server"><a id="a4"
                            runat="server" data-field="ID" data-title-field="Url"
                            href="javascript:BITPAGES.select('[data-field]', '[data-title-field]');"
                            class="bitSelectButton"></a></td>
                        <td class="bitTableDateColumn" data-field="CreateDate"></td>
                    </tr>
                </tbody>
            </table>
            <div class="bitEndTable"></div>
        </div>
        <div id="tabPageSelectFile" class="bitTabPage">
            <!-- Element where elFinder will be created (REQUIRED) -->
            <div id="elfinder"></div>
            <script type="text/javascript" charset="utf-8">
                // Helper function to get parameters from the query string.
                function getUrlParam(paramName) {
                    var reParam = new RegExp('(?:[\?&]|&amp;)' + paramName + '=([^&]+)', 'i');
                    var match = window.location.search.match(reParam);

                    return (match && match.length > 1) ? match[1] : '';
                }

                $(document).ready(function () {
                    var funcNum = getUrlParam('CKEditorFuncNum');

                    var myCommands = elFinder.prototype._options.commands;
                    var disabled = ['extract', 'archive', 'help', 'select'];
                    $.each(disabled, function (i, cmd) {
                        (idx = $.inArray(cmd, myCommands)) !== -1 && myCommands.splice(idx, 1);
                    });
                    var selectedFile = null;
                    var options = {
                        url: '/elfinder.connector',
                        commands: myCommands,
                        height: 500,
                        lang: 'nl',
                        uiOptions: {
                            toolbar: [
                                ['back', 'forward'],
                                ['reload'],
                                ['home', 'up'],
                                ['mkdir', 'mkfile', 'upload'],
                                ['open', 'download'],
                                ['info'],
                                ['quicklook'],
                                ['copy', 'cut', 'paste'],
                                ['rm'],
                                ['duplicate', 'rename', 'edit', 'resize'],
                                ['view', 'sort']
                            ]
                        },
                        getFileCallback: function (file) {
                            var startPoint = file.url.toLowerCase().indexOf('/_files');
                            var url = file.url.substr(startPoint);
                            window.opener.CKEDITOR.tools.callFunction(funcNum, url);
                            window.close();
                        },
                        resizable: false
                        /* ,
                handlers: {
                    select: function (event, elfinderInstance) {

                        if (event.data.selected.length == 1) {
                            var item = $('#' + event.data.selected[0]);
                            if (!item.hasClass('directory')) {
                                selectedFile = event.data.selected[0];
                                $('#elfinder-selectFile').show();
                                return;
                            }
                        }
                        $('#elfinder-selectFile').hide();
                        selectedFile = null;
                    }
                } */
                    };

                    var elf = $('#elfinder').elfinder(options).elfinder('instance');
                });
            </script>
        </div>
    </div>
</body>
</html>
