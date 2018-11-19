<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Sites.aspx.cs" Inherits="BitSite._bitPlate.Autorisation.Sites" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="BITAUTHSITE.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITAUTHSITE.initialize();
            BITAUTHSITE.loadSites();
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Sites
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <br />
    <asp:Label CssClass="warningMsg" ID="LabelMsg" runat="server" Text=""></asp:Label>
    <table id="tableSites" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 40px"></td>
                
                <td class="bitTableActionButton" data-sort-field="Url">Kies</td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon"><a href="#" class="bitTablePageIcon"></a></td>
                <td class="nameColumn"><a data-field="ID" data-text-field="Url" data-title-field="Url" href="javascript:BITAUTHSITE.selectSite('[data-field]', '[data-title-field]');"></a></td>
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
    
</asp:Content>
