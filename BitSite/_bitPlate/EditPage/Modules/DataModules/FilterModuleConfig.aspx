<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FilterModuleConfig.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.DataModules.FilterModuleConfig" EnableViewState="false"%>

<%@ Register Src="../../ModuleConfig/GeneralConfigTab.ascx" TagName="GeneralConfigTab" TagPrefix="uc1" %>

<%@ Register Src="../../ModuleConfig/NavigationActionConfigTab.ascx" TagName="NavigationActionConfigTab" TagPrefix="uc2" %>

<%@ Register Src="../../ModuleConfig/DataListConfigTab.ascx" TagName="DataListConfigTab" TagPrefix="uc3" %>

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
            <li id="tabPage4"><a href="#tabPageFilter">Filter</a></li>
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
        <div id="tabPageFilter" class="bitTabPage">
            <div class="bitPageSettingsCollumnA">Autopostback</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Deze optie verhoogt de performance maar toon geen live data! Als deze optie aan staat, wordt de data uit de datacollectie alvast gepubliceerd nadat u deze module opslaat. Wijzigingen in de data worden dan niet onmiddelijk getoond, maar pas bij opnieuw opslaan.">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input id="checkAutoPostback" type="checkbox" data-field="Settings.Autopostback" />
                <label for="checkAutoPostback">Filter direct aanpassen zonder dat er op de submit knop geklikt hoeft te worden.</label>
            </div>
            <br clear="all" />
            <!-- NEXT ROW -->
        </div>
    </form>
</body>
</html>
