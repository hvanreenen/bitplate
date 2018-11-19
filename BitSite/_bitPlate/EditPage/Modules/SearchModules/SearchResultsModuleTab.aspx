<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchResultsModuleTab.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.SearchModules.SearchResultsModuleTab" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    
    <div id="standardTabPageResults" class="bitTabPage">
        <span class="bitInfo" title="">info</span> <strong>LET WEL: </strong> De zoekresultaten worden geindexeerd bij het publiceren van de website. Om de laatste wijzigingen in pagina's en dataitems vindbaar te maken voor de bezoekers, dient u eerst de website te publiceren met de optie 'zoekindex opnieuw aanmaken'.  
        <br clear="all" />
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Aantal resultaten per pagina</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="Aantal resultaten per lijst (pagina) bij het gebruik van paginering ({Pager}-tag). Pagineren is het opsplitsen van alle gevonden resultaten in meerdere 'pagina's'. {Pager} tag produceert een navigatiepanel met 1,2,3,4 enz. De hyperlinks in dit panel krijgen de class 'bitPagerLink'. Met behulp van deze class is de paginering op te maken.">info</span> 
        </div>
        <div class="bitPageSettingsCollumnC">
            <input type="text" data-field="Settings.MaxNumberOfRows" data-validation="number" />
            (0 = oneindig)
        </div>
        <br clear="all" />

        <!-- NEXT ROW -->
   
        <div class="bitPageSettingsCollumnA">Zoeken <u>niet</u> in gehele inhoud</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="Vindt pagina's en data-items alleen op basis van de tekst in titel, meta-keywords en meta description en niet op basis van de gehele inhoud. Hiermee kunt u het zoeken controleren en specifieker maken, als u metatags inricht bij items en pagina's.">info</span> 
        
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
