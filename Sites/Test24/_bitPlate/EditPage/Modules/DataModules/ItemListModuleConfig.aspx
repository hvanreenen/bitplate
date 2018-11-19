<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ItemListModuleConfig.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.DataModules.ItemListModuleConfig" %>
<%@ Register Src="../../ModuleConfig/GeneralConfigTab.ascx" TagName="GeneralConfigTab" TagPrefix="uc1" %>

<%@ Register src="../../ModuleConfig/NavigationActionConfigTab.ascx" tagname="NavigationActionConfigTab" tagprefix="uc2" %>

<%@ Register src="../../ModuleConfig/DataListConfigTab.ascx" tagname="DataListConfigTab" tagprefix="uc3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <ul>
            <li id="tabPage1"><a href="#standardTabPageGeneral">Algemene instellingen</a></li>
            <li id="tabPage2"><a href="#standardTabPageNavigation">Navigatie</a></li>
            <li id="tabPage3"><a href="#standardTabPageData">Gegevens</a></li>
        </ul>
        <div id="standardTabPageGeneral" style="display: none">
            <uc1:GeneralConfigTab ID="GeneralConfigTab1" runat="server" />
        </div>
        <div id="standardTabPageNavigation" class="bitTabPage">
            <uc2:NavigationActionConfigTab ID="NavigationActionConfigTab1" runat="server" />
        </div>
        <div id="standardTabPageData" class="bitTabPage">
            <uc3:DataListConfigTab ID="DataListConfigTab1" runat="server" />
        </div>
    </form>
</body>
</html>