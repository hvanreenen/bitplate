<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataConfigTab.ascx.cs" Inherits="BitSite._bitPlate.EditPage.ModuleConfig.DataConfigTab" %>
<script src="/_bitplate/EditPage/ModuleConfig/BITDATACONFIGTAB.js"></script>
<div class="bitPageSettingsCollumnA">Dataverzameling</div>
<div class="bitPageSettingsCollumnB">
</div>
<div class="bitPageSettingsCollumnC">
    <select id="SelectDataCollections" runat="server" data-field="DataCollection.ID"
        data-text-field="DataCollection.Name" onchange="BITDATACONFIGTAB.changeDataCollection(this);">
    </select>
</div>
<br clear="all" />
<!-- NEXT ROW -->

<div id="dataCollectionPathBrowse">
    <div class="bitPageSettingsCollumnA">Filter op datagroep (parent)</div>
    <div class="bitPageSettingsCollumnB">
    </div>
    <div class="bitPageSettingsCollumnC">
        <span id="Span1" class="button" onclick="javascript:BITDATACONFIGTAB.showSelectDataGroupDialog();">Bladeren</span>
        <input type="text" id="dataCollectionPathName" readonly="readonly" style="border: none" data-field="Settings.SelectGroupPath" />
        <input type="button" value="x" title="verwijder groep" onclick="javascript: BITDATACONFIGTAB.clearSelectGroup();" />
    </div>
    <br clear="all" />

</div>

<div id="divModuleInputSettings">
    <div class="bitPageSettingsCollumnA">Navigeer op groep</div>
    <div class="bitPageSettingsCollumnB">
        <span class="bitInfo" title="">info</span>
    </div>
    <div class="bitPageSettingsCollumnC">
        <input type="checkbox" data-field="Settings.DrillDownParent" id="DrillDownParentCheckbox" /><label for="DrillDownParentCheckbox">Items worden toegevoegd bij groep die door gebruiker is gekozen (drilldown).</label>
    </div>
    <br clear="all" />
    <div class="bitPageSettingsCollumnA">Invoer door geregistreerde gebruikers</div>
    <div class="bitPageSettingsCollumnB">
        <span class="bitInfo" title="">info</span>
    </div>
    <div class="bitPageSettingsCollumnC">
        <select data-field="Settings.IsNewItemActiveKownUsers">
            <option value="True">Zet nieuwe items meteen actief en zichtbaar</option>
            <option value="False">Zet nieuwe items eerst inactief</option>
        </select>
    </div>

    <br clear="all" />
    <div class="bitPageSettingsCollumnA">Openbare bezoekers</div>
    <div class="bitPageSettingsCollumnB">
        <span class="bitInfo" title="">info</span>
    </div>
    <div class="bitPageSettingsCollumnC">
        <label>
            <input type="checkbox" data-field="Settings.AllowNewItemsFromUnkownUsers" />
            Niet geregistreerde bezoekers toestaan om items toe te voegen</label>
    </div>

    <br clear="all" />
    <div class="bitPageSettingsCollumnA">Invoer door openbare bezoekers</div>
    <div class="bitPageSettingsCollumnB">
        <span class="bitInfo" title="">info</span>
    </div>
    <div class="bitPageSettingsCollumnC">
        <select data-field="Settings.IsNewItemActiveUnkownUsers">
            <option value="True">Zet nieuwe items meteen actief en zichtbaar</option>
            <option value="False">Zet nieuwe items eerst inactief</option>
        </select>
    </div>
    <br clear="all" />
</div>

<div id="divModuleListSettings">

    <div class="bitPageSettingsCollumnA">Toon</div>
    <div class="bitPageSettingsCollumnB">
    </div>
    <div class="bitPageSettingsCollumnC">
        <select id="selectShowData" data-field="Settings.ShowDataBy" onchange="BITDATACONFIGTAB.changeShowDataType(this);">
            <option value="0">Gebruikersselectie afhankelijk</option>
            <option value="5">Gebruikersselectie afhankelijk (geen standaard data)</option>
            <option value="1">Alleen hoofdgroepen</option>
            <option value="2">Alle groepen</option>
            <option value="3">Alle items</option>
        </select>
    </div>
    <br clear="all" />
    <!-- NEXT ROW -->

    <div class="bitPageSettingsCollumnA">Filter</div>
    <div class="bitPageSettingsCollumnB">
    </div>
    <div class="bitPageSettingsCollumnC">
        <select id="bitSelectFilterFields" data-field="Settings.SelectField" data-text-field="SelectField.Text">
        </select>
        <select id="bitSelectFilterOperator" data-field="Settings.SelectOperator" class="small">
            <option value="null"></option>
            <option value="=">=</option>
            <option value="!=">Ongelijk aan</option>
            <option value="like">Bevat</option>
            <option value="not like">Bevat niet</option>
            <option value="likeStart">Begint met</option>
            <option value="not likeStart">Begint niet met</option>
            <option value="likeEnd">Eindigt met</option>
            <option value="not likeEnd">Eindigt niet met</option>
            <option value="<"><</option>
            <option value="<="><=</option>
            <option value=">=">>=</option>
            <option value=">">></option>
        </select>
        <input type="text" data-field="Settings.SelectValue" />
    </div>
    <br clear="all" />


    <div class="bitPageSettingsCollumnA">Sortering</div>
    <div class="bitPageSettingsCollumnB">
    </div>
    <div class="bitPageSettingsCollumnC">
        <select id="bitSelectSortFields" data-field="Settings.SortField" data-text-field="Settings.SortField.Text">
        </select>
        <select data-field="Settings.SortOrder" class="small">
            <option value="ASC">Oplopend</option>
            <option value="DESC">Aflopend</option>
        </select>
    </div>
    <br clear="all" />
    <!-- NEXT ROW -->
    <div class="bitPageSettingsCollumnA">Toon vanaf rij</div>
    <div class="bitPageSettingsCollumnB">
    </div>
    <div class="bitPageSettingsCollumnC">
        <input type="text" data-field="Settings.ShowFromRowNumber" data-validation="number" />
    </div>
    <br clear="all" />
    <!-- NEXT ROW -->
    <div class="bitPageSettingsCollumnA">Toon max. aantal rijen</div>
    <div class="bitPageSettingsCollumnB">
    </div>
    <div class="bitPageSettingsCollumnC">
        <input type="text" data-field="Settings.MaxNumberOfRows" data-validation="number" />
        (0 = oneindig)
    </div>
    <br clear="all" />
</div>
<!-- NEXT ROW -->
<div class="bitPageSettingsCollumnA">Verberg module</div>
<div class="bitPageSettingsCollumnB">
    <span class="bitInfo" title="Als deze optie aan staat, wordt de gehele module niet getoond, wanneer er geen data is gevonden in de selectie.">info</span>
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
</div>
<!-- NEXT ROW -->
<div id="divDetailsModuleSettings">
    <div class="bitPageSettingsCollumnA">Vaste waarde</div>
    <div class="bitPageSettingsCollumnB">
    </div>
    <div class="bitPageSettingsCollumnC">
        <input id="checkFixedData" type="checkbox" data-field="Settings.HasFixedData" onclick="BITDATACONFIGTAB.changeFixedData();" />
        <label for="checkFixedData">Module toont altijd vaste data, en reageert niet op gebruikersselectie.</label><br />
        <select style="display: none" id="selectFixedDataElement" data-field="Settings.FixedDataObjectID">
        </select>
    </div>
    <br clear="all" />
    <!-- NEXT ROW -->
    <div id="divInitialDataElement">
        <div class="bitPageSettingsCollumnA">Initiële waarde:</div>
        <div class="bitPageSettingsCollumnB">
        </div>
        <div class="bitPageSettingsCollumnC">
            <select onchange="javascript:BITDATACONFIGTAB.changeDefaultDataElement();" id="selectDefaultDataElement" data-field="Settings.DefaultDataObjectID"></select>
        </div>
    </div>
    <br clear="all" />
</div>

