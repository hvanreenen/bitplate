<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreviewPage.aspx.cs" Inherits="BitSite._bitPlate.PreviewPage"
    EnableViewState="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="/_bitplate/_js/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="/_bitplate/_js/jquery-ui-1.9.1.custom.js"></script>
    <script type="text/javascript" src="/_bitplate/_js/JSON.js"></script>
    <script type="text/javascript" src="/_bitplate/_js/BITAJAX.js"></script>
    <script type="text/javascript" src="/_bitplate/_js/BITUTILS.js"></script>
    <script type="text/javascript" src="/_bitplate/_js/prototypes/initDialog.js"></script>
    <script type="text/javascript" src="/_bitplate/_js/prototypes/validation.js"></script>
    <script type="text/javascript" src="/_bitplate/_js/prototypes/date.js"></script>
    <script type="text/javascript" src="/_bitplate/_js/prototypes/databind.js"></script>
    <script type="text/javascript" src="/_bitplate/_js/prototypes/formEnrich.js"></script>


    <asp:Literal ID="LiteralHead" runat="server"></asp:Literal>
    <script type="text/javascript">
        $(document).ready(function () {
            BITALLMODULES.loadAllModules();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal ID="LiteralBody" runat="server"></asp:Literal>
    </form>
</body>
</html>
