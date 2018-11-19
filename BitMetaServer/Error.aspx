<%@ Page Title="" Language="C#" MasterPageFile="~/_masterPages/Details.master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="BitMetaServer.Error" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript">
        var ErrorPage = {
            showDetails: function () {
                $('#ErrorDetails').show();
                $('#AShowDetails').hide();
                $('#AHideDetails').show();
            },

            hideDetails: function () {
                $('#ErrorDetails').hide();
                $('#AShowDetails').show();
                $('#AHideDetails').hide();
            },
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
     <ul>
        <li><a href="/_bitplate/Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        <li id="AShowDetails"><a href="javascript:void(0)" onclick="ErrorPage.showDetails()" class="bitNavBarButtonShowInfo">ShowDetails</a>
            <div>Toon details</div>
        </li>
         <li id="AHideDetails" style="display: none;"><a href="javascript:void(0)" onclick="ErrorPage.hideDetails()" class="bitNavBarButtonHideInfo">HideDetails</a>
            <div>Verberg details</div>
        </li>
        <!-- <li><a href="../Default.aspx" class="bitNavBarButtonBack">Verstuur</a>
            <div>Verstuur</div>
        </li>-->
     </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="DetailsPlaceHolder" runat="server">
    <div class="ui-state-error ui-corner-all" style="padding: 0 .7em;">
		<p><span class="ui-icon ui-icon-alert" style="float: left; margin-right: .3em;"></span>
		<strong>Fout:</strong> Er is een fout op getreden. Klik op op de terug knop hier boven om terug te keren naar het dashboard.</p>
        <div id="ErrorDetails" style="display: none; word-wrap: break-word;">
            <asp:Literal runat="server" ID="LiteralError"></asp:Literal>
        </div>
	</div>
</asp:Content>
