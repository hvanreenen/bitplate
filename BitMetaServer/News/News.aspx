<%@ Page Title="" Language="C#" MasterPageFile="~/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="News.aspx.cs" Inherits="BitMetaServer.News.News" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="/_js/prototypes/dockable.js"></script>
    <script type="text/javascript" src="BITNEWS.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITNEWS.initialize();
        });
    </script>
    <asp:Literal ID="DefaultPermissionSets" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack" runat="server" id="BackLink">back</a>
            <div>Terug</div>
        </li>

        <li><a href="javascript:BITNEWS.newNewsMessage();" class="bitNavBarButtonAddPage">ADDNEWSMESSAGE</a>
            <div>Nieuw bericht...</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Bitplate nieuws berichten
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <!--<fieldset>
        <legend>Filter</legend>
        <label>Kies klant:</label>
        <select id="selectCompanyFilter" class="selectCompany" onchange="BITLICENSES.filterOnCompanies(this);"></select><br />
    </fieldset>-->
    <table id="tableNewsMessages" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 240px" data-sort-field="Name">Titel</td>
                <!--<td class="resellerColumn" style="width: 140px" data-sort-field="FK_Company">Eigenaar</td>-->
                <td class="resellerColumn" style="width: 450px" data-sort-field="Message">Bericht</td>

                <td class="bitTableActionButton">Bewerk</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon" style="width: 40px"><a href="#" class="bitTablePageIcon"></a></td>
                <td class="nameColumn bitTableColumnEllipsis" data-field="Name"></td>

                <td class="nameColumn bitTableColumnEllipsis" data-field="Message"></td>
                <td class="bitTableActionButton"><a data-field="ID" data-title-field="Type" href="javascript:BITNEWS.openNewsMessageDetailsPopup('[data-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITNEWS.removeNewsMessage('[data-field]');"
                    class="bitDeleteButton"></a></td>

                <td class="bitTableDateColumn" data-field="CreateDate"></td>
            </tr>
        </tbody>
        <tfoot>
            <tr>
                <td colspan="8">
                    <div class="bitEndTable"></div>
                </td>
            </tr>
        </tfoot>
    </table>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <div id="newsMessageEditPopup" title="Nieuwsbericht bewerken">
        <input type="hidden" data-field="ID" />
        <div class="bitPageSettingsCollumnA">Titel</div>
        <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="" ></span></div>
        <div class="bitPageSettingsCollumnC"><input type="text" data-field="Name" /></div>

        <div class="bitPageSettingsCollumnA">Aanmaak datum</div>
        <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="" ></span></div>
        <div class="bitPageSettingsCollumnC" data-field="CreateDate"></div>

        <div class="bitPageSettingsCollumnA">Bericht</div>
        <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="" ></span></div>
        <div class="bitPageSettingsCollumnC"><textarea data-field="Message"></textarea></div>
    </div>
</asp:Content>
