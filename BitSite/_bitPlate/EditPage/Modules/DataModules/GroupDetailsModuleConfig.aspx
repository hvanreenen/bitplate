<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupDetailsModuleConfig.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.DataModules.GroupDetailsModuleConfig" %>

<%@ Register Src="../../ModuleConfig/GeneralConfigTab.ascx" TagName="GeneralConfigTab" TagPrefix="uc1" %>

<%@ Register src="../../ModuleConfig/DataDetailsConfigTab.ascx" tagname="DataDetailsConfigTab" tagprefix="uc3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <ul>
            <li id="tabPage1"><a href="#standardTabPageGeneral">Algemene instellingen</a></li>
            <li id="tabPage3"><a href="#standardTabPageData">Gegevens</a></li>
        </ul>
        <div id="standardTabPageGeneral" style="display: none">
            <uc1:GeneralConfigTab ID="GeneralConfigTab1" runat="server" />
        </div>
        <div id="standardTabPageData" class="bitTabPage">
            <uc3:DataDetailsConfigTab ID="DataDetailsConfigTab1" runat="server" />
        </div>
    </form>
</body>
</html>
