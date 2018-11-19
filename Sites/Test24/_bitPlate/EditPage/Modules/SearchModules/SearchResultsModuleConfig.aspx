<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchResultsModuleConfig.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.SearchModules.SearchResultsModuleConfig" %>

<%@ Register Src="../../ModuleConfig/GeneralConfigTab.ascx" TagName="GeneralConfigTab" TagPrefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <ul>
        <li id="tabPage1"><a href="#standardTabPageGeneral">Algemene instellingen</a></li>
        <li id="tabPage3"><a href="#standardTabPageList">Resultaten</a></li>
    </ul>
    <div id="standardTabPageGeneral" style="display: none">
        <uc1:GeneralConfigTab ID="GeneralConfigTab1" runat="server" />
    </div>
    <div id="standardTabPageList" class="bitTabPage">
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Aantal resultaten per pagina</div>
        <div class="bitPageSettingsCollumnB">
        </div>
        <div class="bitPageSettingsCollumnC">
            <input type="text" data-field="Settings.MaxNumberOfRows" data-validation="number" />
            (0 = oneindig)
        </div>
        <br clear="all" />

        <!-- NEXT ROW -->
   
        <div class="bitPageSettingsCollumnA">Zoeken niet in gehele inhoud</div>
        <div class="bitPageSettingsCollumnB">
        </div>
        <div class="bitPageSettingsCollumnC">
            <input type="checkbox" id="checkboxSearchOnlyMetaContent" data-field="Settings.SearchOnlyMetaContent" />
            <label for="checkboxSearchOnlyMetaContent">Vindt pagina's alleen op basis van titel, naam, meta-keywords en meta-description. </label>
        </div>
        <br clear="all" />

        <!-- NEXT ROW -->
    </div>
</body>
</html>
