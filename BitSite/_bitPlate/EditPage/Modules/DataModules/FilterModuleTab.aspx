<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FilterModuleTab.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.DataModules.FilterModuleTab" EnableViewState="false"%>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        
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

        </div>
    </form>
</body>
</html>
