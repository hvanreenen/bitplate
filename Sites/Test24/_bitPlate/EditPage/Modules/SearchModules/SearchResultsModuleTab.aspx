<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchResultsModuleTab.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.SearchModules.SearchResultsModuleTab" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    
    <div id="standardTabPageResults" class="bitTabPage">
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
