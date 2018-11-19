<%@ Page Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master" AutoEventWireup="true" CodeBehind="ElFileManager.aspx.cs" Inherits="BitSite._bitPlate.FileManager.ElFileManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="/_bitplate/_themes/bitplate/plugins/jsTree/themes/default/style.css" />
    <!--<link rel="stylesheet" type="text/css" href="_js/plugins/fileUpload/css/jquery.image-gallery.min.css" />-->
    <link rel="stylesheet" type="text/css" href="/_bitplate/_themes/bitplate/css/filemanager.css" />
    <link href="/_bitplate/_themes/bitplate/plugins/elFinder-2.0/css/elfinder.min.css" rel="stylesheet" type="text/css" />
    <link href="/_bitplate/_themes/bitplate/plugins/elFinder-2.0/css/theme.css" rel="stylesheet" type="text/css" />
    <link href="/_bitplate/_themes/bitplate/plugins/elFinder-2.0/css/bitTheme.css" rel="stylesheet" type="text/css" />

    <script src="/_bitplate/_js/plugins/elFinder-2.0/elfinder.min.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/elFinder-2.0/i18n/elfinder.nl.js" type="text/javascript"></script>
  
    <asp:Literal ID="LiteralScript" runat="server"></asp:Literal>
    <style type="text/css">
        #bitDivSearch {
            display: none;
        }
    </style>
    <link rel="stylesheet" type="text/css" href="/_bitplate/_themes/bitplate/css/tree.css" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <!-- icons -->
    <ul>
        <li id="liBack"><a href="../Default.aspx" class="bitNavBarButtonBack" runat="server" id="BackLink">back</a>
            <div>Terug</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Bestandsbeheer
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <div class="fileManager" style="margin-top: 10px;">finder</div>   

    <script type="text/javascript" charset="utf-8">
        $(function () {
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
                }/* ,
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

            $('.fileManager').elfinder(options);
        });
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    
</asp:Content>
