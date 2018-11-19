<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationActionConfigTab.ascx.cs" Inherits="BitSite._bitPlate.EditPage.ModuleConfig.NavigationActionConfigTab" %>

<script src="/_bitplate/EditPage/ModuleConfig/BITNAVIGATIONCONFIGTAB.js"></script>
<div id="divNavigationActions"></div>
<div id="divNavigationActionsTemplate" style="display:none">
    <fieldset id="fieldsetNavigation_0">
        <legend>Navigatie</legend>
        <div class="bitPageSettingsCollumnA">Tag</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="Soort actie. Deze is gekoppeld aan de tag. De tag wordt omgevormd in een hyperlink/button waar de gebruiker op kan klikken. Een module kan meerdere acties in zich bergen. Per soort actie kunt u een eigen navigatie-afhandeling kiezen">info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <span id="navigationTagName_0"></span>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Actie bij klik:</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="Twee soorten afhandeling na een gebruiker-klik-actie op de bovenstaande tag: naar een andere pagina gaan, of op dezelfde pagina blijven en andere modules op deze pagina verversen (Ajax). Bij het navigeren naar een andere pagina wordt de informatie mee gestuurd welke nodig is om de vervolg pagina te laden; bij verversen van andere modules wordt deze informatie naar de betreffende modules gestuurd.">info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <select id="selectNavigationType_0" onchange="javascript:BITNAVIGATIONCONFIGTAB.changeNavigationType(this, 0);">
                <option value="0">Navigeer naar andere pagina</option>
                <option value="1">Blijf in dezelfde pagina</option>
                <!--<option value="2" disabled="disabled">Open popup</option>
                <option value="3">Run javascript</option>-->
            </select>

        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div id="divNavigationType0_0">

            <div class="bitPageSettingsCollumnA">Navigeer naar pagina</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Kies hier de pagina waar naartoe moet worden genavigeerd na de gebruiker-klik-actie. U kunt eventueel dezelfde pagina kiezen voor het herladen van dezelfde pagina.">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <select id="SelectNavigationPage_0" runat="server"
                    data-text-field="NavigationPage.Name">
                </select>
            </div>
            <br clear="all" />
            <!-- NEXT ROW -->
        </div>
        <div id="divNavigationType1_0" style="display: none">
            <div class="bitPageSettingsCollumnA">Bij klik ververs module(s):</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Kies welke modules (via ajax) moeten worden herladen. U dient de modules te kiezen welke logisch verbonden zijn aan deze module. Betreffende informatie van deze module klik actie wordt naar de ander modules gestuurd. Gegevens in deze modules wordt opnieuw geladen. U kunt eventueel ook de huidige module kiezen om te herladen.">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <div runat="server" id="checkboxListModules_0" data-list-type="stringArray"
                    data-control-type="checkboxlist">
                </div>
            </div>
            <br clear="all" />
        
            <div class="bitPageSettingsCollumnA">Voer javascript uit na klik<br />
                <button type="button" onclick="javascript:BITNAVIGATIONCONFIGTAB.showModuleIdFields();">Toon html-id's</button>
            </div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Bij een klik kan extra javascript worden uitgevoerd, bijvoorbeeld om een andere module te verbergen. Schrijf hier geldige extra javascript; gebruik geen <script>-tag. Met de knop 'toon module id's' krijgt u de jquery-id's van de modules te zien, zodat u hier jquery acties op kunt uitvoeren, zoals .hide() en .show().">info</span>
                <a href="javascript:;" class="bitTextareaEnlargeButton">enlarge </a>
            </div>
            <div class="bitPageSettingsCollumnC">
                <textarea id="javascriptModules_0" class="bitTextArea" data-field="JsFunction"></textarea>
            </div>
            <br clear="all" />
        </div>
    </fieldset>
</div>
