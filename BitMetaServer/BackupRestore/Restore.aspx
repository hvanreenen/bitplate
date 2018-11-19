<%@ Page Title="" Language="C#" MasterPageFile="~/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Restore.aspx.cs" Inherits="BitMetaServer.Licenses.Restore" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="BITRESTORE.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITRESTORE.initialize();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Bitplate restore
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <br />
    <div id="restoreStatus"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">


</asp:Content>
