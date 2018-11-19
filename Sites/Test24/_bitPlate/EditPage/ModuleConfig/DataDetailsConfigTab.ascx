<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataDetailsConfigTab.ascx.cs" Inherits="BitSite._bitPlate.EditPage.ModuleConfig.DataDetailsConfigTab" %>
<script src="/_bitplate/EditPage/ModuleConfig/BITDATACONFIGTAB.js"></script>
<div class="bitPageSettingsCollumnA">Dataverzameling</div>
<div class="bitPageSettingsCollumnB">
    <span class="bitInfo" title="Kies de datacollectie waaruit de details moeten worden geladen.">info</span>

</div>
<div class="bitPageSettingsCollumnC">
    <select id="SelectDataCollectionsDetails" runat="server" data-field="DataCollection.ID"
        data-text-field="DataCollection.Name" onchange="BITDATACONFIGTAB.changeDataCollection(this);">
    </select>
</div>
<br clear="all" />
<!-- NEXT ROW -->

<div class="bitPageSettingsCollumnA">Verberg module</div>
<div class="bitPageSettingsCollumnB">
    <span class="bitInfo" title="Als deze optie aan staat, wordt de gehele module niet getoond, wanneer er geen data is geladen.">info</span>
</div>
<div class="bitPageSettingsCollumnC">
    <input id="checkHideWhenNoData" type="checkbox" data-field="Settings.HideWhenNoData" />
    <label for="checkHideWhenNoData">Verberg bij geen data</label>
</div>
<br clear="all" />
<!-- NEXT ROW -->

<div id="defaultDataElementSettings">
    <div class="bitPageSettingsCollumnA">Toon ook inactieve items</div>
    <div class="bitPageSettingsCollumnB">
    </div>
    <div class="bitPageSettingsCollumnC">
        <input type="checkbox" data-field="Settings.ShowInactive" />
    </div>
    <br clear="all" />
    <!-- NEXT ROW -->
</div>
<!-- NEXT ROW -->
<div class="bitPageSettingsCollumnA">Vaste waarde</div>
<div class="bitPageSettingsCollumnB">
</div>
<div class="bitPageSettingsCollumnC">
    <input id="checkFixedData" type="checkbox" data-field="Settings.HasFixedData" onclick="BITDATACONFIGTAB.changeFixedData();" />
    <label for="checkFixedData">Module toont altijd vaste data, en reageert niet op gebruikersselectie.</label><br />
    <select style="display:none" id="selectFixedDataElement" data-field="Settings.FixedDataObjectID">
    </select>
</div>
<br clear="all" />
<!-- NEXT ROW -->
<div id="divInitialDataElement">
    <div class="bitPageSettingsCollumnA">Initiële waarde:</div>
    <div class="bitPageSettingsCollumnB">
    </div>
    <div class="bitPageSettingsCollumnC">

        <select onchange="BITDATACONFIGTAB.changeDefaultDataElement()" id="selectDefaultDataElement" data-field="Settings.DefaultDataObjectID"></select>
    </div>
</div>
<br clear="all" />



