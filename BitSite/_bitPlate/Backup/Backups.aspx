<%@ Page Title="Pagina's" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Backups.aspx.cs" Inherits="BitSite._bitPlate.Backup.Backups" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="/_bitplate/_themes/bitplate/plugins/jbreadcrump/Styles/BreadCrumb.css" />
    <script type="text/javascript" src="BITBACKUPS.js"></script>
    
    <!--<link rel="stylesheet" type="text/css" href="_js/plugins/jbreadcrump/Styles/Base.css" />-->
    
    <script src="/_bitplate/_js/plugins/jbreadcrump/js/jquery.easing.1.3.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/jbreadcrump/js/jquery.jBreadCrumb.1.1.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITBACKUP.initialize();
            BITBACKUP.getAllBackups();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        <li id="liCreateBackup" runat="server"><a id="aMakeBackup" runat="server" href="javascript:BITBACKUP.createBackup();"
            class="bitNavBarButtonAddPage">createbackup</a>
            <div>Maak backup</div>
        </li>
        <%--<li id="liAddFolder" runat="server"><a id="aAddFolder" runat="server" href="javascript:BITPAGES.newFolder();"
            class="bitNavBarButtonAddFolder">nieuwe folder</a>
            <div>Nieuwe folder</div>
        </li>--%>

    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Backups
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <p>Deze functie is in beta fase. Gelieve deze functie niet te gebruiken in productie omgeving.</p>
    <table id="tableBackupFiles" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 25px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 30px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 140px" data-sort-field="Name">Naam</td>
                <td runat="server" id="htdSelect" class="bitTableActionButton">Download</td>
                <td runat="server" id="htdEdit" class="bitTableActionButton">Restore</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
                <td runat="server" id="htdDelete" class="bitTableActionButton">Verwijder</td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon"><a data-field="Url" href="[data-field]?mode=edit" class="bitTablePageIcon"></a></td>
                <td class="iconAuth" ><a href="#" class="bitTableAuthIcon" style="display:none"></a></td>
                <td class="bitTableColumnName bitTableColumnEllipsis" data-field="Name" data-title-field="Name"></td>
                <td class="bitTableActionButton" id="tdSelect" runat="server" ><a id="aSelect"
                    runat="server" data-field="Url" data-title-field="Url" 
                    href="[data-field]"
                    class="bitSelectButton"></a></td>
                <td class="bitTableActionButton" id="tdEdit" runat="server"><a data-field="Name" href="javascript:BITBACKUP.restoreBackup('[data-field]');"
                    class="bitEditButton"></a></td>
                <td class="bitTableDateColumn" data-field="CreateDate"></td>
                <td class="bitTableActionButton" id="tdDelete" runat="server"><a id="aDelete"
                    runat="server" data-field="Name" href="javascript:BITBACKUP.deleteBackup('[data-field]');"
                    class="bitDeleteButton"></a></td>
            </tr>
        </tbody>
    </table>
    <div class="bitEndTable"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <div title="Backup maken." id="CreateBackupDialog">
        Bezig met het maken van backup.
    </div>

    <div title="Backup restore" id="RestoreBackupDialog">
        Bezig met initializeren van restore.
    </div>
</asp:Content>
