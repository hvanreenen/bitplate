<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditPage.aspx.cs" Inherits="BitSite._bitPlate.EditPage.EditPage" EnableViewState="false"%>

<%@ Register Src="~/_bitPlate/EditPage/EditPageMenu2.ascx" TagPrefix="uc1" TagName="EditPageMenu2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="0" />
    <title></title>
    <style>
        html, body, iframe {
            margin: 0px !important;
            padding: 0px !important;
        }
    </style>
   <%--<script src="/_js/jquery-1.8.2.js"></script>
    <script src="/_bitplate/_js/jquery-ui-1.9.1.custom.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/JSON.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/BITUTILS.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/BITAJAX.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/modernizr.custom.59383.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/prototypes/centerScreen.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/prototypes/databind.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/prototypes/date.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/prototypes/formEnrich.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/prototypes/initDialog.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/prototypes/string.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/prototypes/tabby.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/prototypes/validation.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/prototypes/searchable.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/BITAUTH.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/ckeditor/ckeditor.js" type="text/javascript"></script>
    <script src="/_bitPlate/_js/prototypes/contextMenu.js" type="text/javascript"></script>
    <script src="/_bitPlate/pages/BITPAGES.js" type="text/javascript"></script>
    <script src="/_bitPlate/Dialogs/BITAUTORISATIONTAB.js" type="text/javascript"></script>
    <script src="/_bitPlate/EditPage/BITEDITPAGE2.js"></script>
    <script src="/_bitPlate/_js/prototypes/insertAtCaret.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/CodeMirror-3.12/lib/codemirror.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/CodeMirror-3.12/mode/javascript/javascript.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/CodeMirror-3.12/mode/css/css.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/CodeMirror-3.12/mode/xml/xml.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/CodeMirror-3.12/addon/search/search.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/CodeMirror-3.12/addon/search/searchcursor.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/CodeMirror-3.12/addon/dialog/dialog.js" type="text/javascript"></script>--%>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/_bitplate/_themes/" + BitSite.SessionObject.CurrentBitplateUser.Theme + "/css/pageEdit") %>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/_bitplate/_themes/" + BitSite.SessionObject.CurrentBitplateUser.Theme + "/css/codemirror") %>
    <link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/chosen/chosen.css" />
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/BitCore") %>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/PageEditor") %>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/Plugins/jsTree") %>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/Plugins/CodeMirror") %>
    <script src="/_bitplate/newsletters/BITNEWSLETTER.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/chosen/chosen.jquery.min.js" type="text/javascript"></script>
    <%--<script src="/_bitplate/_js/plugins/jsTree/_lib/jquery.cookie.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/jsTree/jquery.jstree.js" type="text/javascript"></script>
    <link href="/_bitplate/_themes/bitplate/jquery-ui-1.9.1/css/custom-theme/jquery-ui-1.9.1.custom.css" rel="stylesheet" type="text/css" />
    <link href="/_bitplate/_js/plugins/CodeMirror-3.12/lib/codemirror.css" rel="stylesheet" type="text/css" />
    <link href="/_bitplate/_themes/bitplate/css/topMenu.css" rel="stylesheet" type="text/css" />
    <link href="/_bitplate/_themes/bitplate/css/pageEdit.css" rel="stylesheet" type="text/css" />
    <link href="/_bitplate/_themes/bitplate/css/form.css" rel="stylesheet" type="text/css" />
    <link href="/_bitplate/_themes/bitplate/css/formElements.css" rel="stylesheet" type="text/css" />
    <link href="/_bitplate/_themes/bitplate/css/bitPlateIcons.css" rel="stylesheet" type="text/css" />
    <link href="/_bitplate/_themes/bitplate/css/popup.css" rel="stylesheet" type="text/css" />
    <link href="/_bitplate/_themes/bitplate/plugins/jstree/themes/default/style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="/_css/jquery.treeview.css" runat="server" id="BitStyleScriptadc4059d33834066b1b79488798f6a87" />
    <link rel="stylesheet" type="text/css" href="/_css/online.css" runat="server" id="BitStyleScript3bcb150267724dd4b1c7343da40aeb32" />
    <link rel="stylesheet" type="text/css" href="/_css/stylesheet.css" runat="server" id="BitStyleScript67bd67cfd77f48cb824c901b44e8b29e" />
    <script type="text/javascript" src="/_js/active.link.js" id="BitScript355ce71d608942359fdee949ada498f2"></script>
    <script type="text/javascript" src="/_js/fadin.divs.js" id="BitScriptca992f45e2c540a0a330788cf3d1032e"></script>
    <script type="text/javascript" src="/_js/home.click.js" id="BitScript8bd33c955a0d4963b39db108355bbc65"></script>
    <script type="text/javascript" src="/_js/jquery.ba-dotimeout.min.js" id="BitScript974588c4a406488ea24da90c88cc90fb"></script>
    <script type="text/javascript" src="/_js/jquery.treeview.js" id="BitScript4a2ee63194e94c6faf03e6bdb38ce45f"></script>
    <script type="text/javascript" src="/_js/jquery.treeview.min.js" id="BitScriptf9896fe829b8473fbc6175d76b0878e4"></script>
    <script type="text/javascript" src="/_js/tsJQuery.menu.js" id="BitScript7777e0bb09674860a319c8d87c920a93"></script>--%>

    <!--<script src="_bitPlate/EditPage/BITEDITPAGE2.js"></script>-->
    <script>
        $(document).ready(function () {
            BITEDITPAGE.initialize();
            var pageid = '<%=this.PageID %>';
            if (pageid) BITEDITPAGE.loadPage(pageid);
            var newsletterid = '<%=this.NewsletterID %>';
            if (newsletterid) BITEDITPAGE.loadNewsletter(newsletterid);
            
        });
    </script>
</head>
<body>
    <form runat="server">
        <uc1:EditPageMenu2 runat="server" ID="EditPageMenu2" />
        <iframe id="iframe" style="position: absolute; top: 27px; width: 100%; display: block;" frameborder=0 onload="BITEDITPAGE.changeIframeToPageHeight(this)"></iframe>
    </form>

</body>
</html>
