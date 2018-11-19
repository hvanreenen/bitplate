<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModuleConfig.aspx.cs" Inherits="BitSite._bitPlate.EditPage.ModuleConfig.ModuleConfig" %>
<%@ Register Src="GeneralConfigTab.ascx" TagName="GeneralConfigTab" TagPrefix="uc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <ul>
            <li id="tabPage1"><a href="#standardTabPageGeneral">Algemene instellingen</a></li>
        </ul>
        <div id="standardTabPageGeneral" style="display: none">
            <uc1:generalconfigtab id="GeneralConfigTab1" runat="server" />
        </div>

    </form>
</body>
</html>
