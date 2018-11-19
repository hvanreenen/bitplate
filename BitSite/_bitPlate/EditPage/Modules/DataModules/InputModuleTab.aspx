<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InputModuleTab.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.DataModules.InputModuleTab" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="/_bitplate/EditPage/Modules/DataModules/INPUTMODULECONFIGTAB.js"></script>
</head>
<body>
    
    <div id="tabPagePublish" class="bitTabPage">
        
        <div class="bitPageSettingsCollumnA">Parent datagroup:</div>
        <div class="bitPageSettingsCollumnB">
        </div>
        <div class="bitPageSettingsCollumnC">
            <span id="Span2" class="button" onclick="INPUTMODULECONFIGTAB.showSelectDataGroupDialog();">Bladeren</span>
            <input type="text" id="inputDataCollectionPathName" readonly="readonly" style="border: none" data-field="Settings.SelectInputGroupPath" />
            <input type="button" value="x" title="verwijder groep" onclick="INPUTMODULECONFIGTAB.clearSelectGroup();" />
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->

        <div class="bitPageSettingsCollumnA">DrillDownParent</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="Deze optie verhoogt de performance maar toon geen live data! Als deze optie aan staat, wordt de data uit de datacollectie alvast gepubliceerd nadat u deze module opslaat. Wijzigingen in de data worden dan niet onmiddelijk getoond, maar pas bij opnieuw opslaan.">info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <input type="checkbox" data-field="Settings.DrillDownParent" />
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
</body>
</html>
