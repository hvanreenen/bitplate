<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GeneralConfigTab.ascx.cs" Inherits="BitSite._bitPlate.EditPage.ModuleConfig.GeneralConfigTab" %>

<div class="bitPageSettingsCollumnA">Naam</div>
<div class="bitPageSettingsCollumnB">
    <span class="bitInfo" title="Hier de naam van de module, zodat u
        hem later nog kunt herkennen.">info</span>
</div>
<div class="bitPageSettingsCollumnC">
    <input type="text" data-field="Name" data-validation="required"
        name="bitPageName"
        id="bitPageName" />
</div>
<br clear="all" />

<!-- NEXT ROW -->
<div class="bitPageSettingsCollumnA">Soort</div>
<div class="bitPageSettingsCollumnB">
    <span class="bitInfo" title="Soort module. Dit is een vast gegeven, engelstalig.">info</span>
</div>
<div class="bitPageSettingsCollumnC">
    <span data-field="Type"></span>
</div>
<br clear="all" />
<div class="bitPageSettingsCollumnA">Verberg wysiwyg-editor (geavanceerd)</div>
<div class="bitPageSettingsCollumnB">
    <span class="bitInfo" title="Als u deze optie aanvinkt, krijgt u alleen html-code te zien zonder wysiwyg-editor (what you see is what you get). Deze optie is vooral handig om de html precies in te voeren zoals u wilt. ">info</span>
</div>
<div class="bitPageSettingsCollumnC">
    <input type="checkbox" id="HideCkEditor" data-field="Settings.HideCkEditor" />
    <label for="HideCkEditor">Verberg layout, toon alleen html-code tijdens bewerken van deze module.</label>
</div>
<br clear="all" />
<!-- NEXT ROW -->
<div class="bitPageSettingsCollumnA">Hergebruik</div>
<div class="bitPageSettingsCollumnB"></div>
<div class="bitPageSettingsCollumnC">
    <input type="radio" id="checkVisibleCompleteSite" data-field="CrossPagesMode" name="CrossPagesMode" value="2" />
    <label for="checkVisibleCompleteSite">
        Zichtbaar op alle pagina's
        van deze site</label>
</div>
<br clear="all" />
<!-- NEXT ROW -->
<div class="bitPageSettingsCollumnA"></div>
<div class="bitPageSettingsCollumnB"></div>
<div class="bitPageSettingsCollumnC">
    <input type="radio" id="checkVisibleCompleteLayout" data-field="CrossPagesMode" name="CrossPagesMode" value="1" />
    <label for="checkVisibleCompleteLayout">
        Zichtbaar op alle pagina's binnen de huidige
                        layouttemplate (<span data-field="Page.Template.Name"></span>)</label>
</div>
<br clear="all" />
<!-- NEXT ROW -->
<div class="bitPageSettingsCollumnA"></div>
<div class="bitPageSettingsCollumnB"></div>
<div class="bitPageSettingsCollumnC">
    <input type="radio" id="checkVisibleCustomPages" data-field="CrossPagesMode" name="CrossPagesMode" value="0" />
    <label for="checkVisibleCustomPages">
        Zichtbaar op volgende pagina's</label>
    <div id="PageSelector" style="display:none;">
        <select runat="server" id="modulePerPageSelect" data-field="Pages" style="width: 600px;"></select>
    </div>
</div>
<br clear="all" />