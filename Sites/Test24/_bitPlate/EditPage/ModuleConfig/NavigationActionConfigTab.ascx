<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationActionConfigTab.ascx.cs" Inherits="BitSite._bitPlate.EditPage.ModuleConfig.NavigationActionConfigTab" %>

<script src="/_bitplate/EditPage/ModuleConfig/BITNAVIGATIONCONFIGTAB.js"></script>
<div id="divNavigationActions"></div>
<div id="divNavigationActionsTemplate" style="display:none">
    <fieldset id="fieldsetNavigation_0">
        <legend>Navigatie</legend>
        <div class="bitPageSettingsCollumnA">Tag</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="">info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <span id="navigationTagName_0"></span>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Actie bij klik:</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="">info</span>
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

            <div class="bitPageSettingsCollumnA">Pagina</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
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
            <div class="bitPageSettingsCollumnA">Bij klik ververs module(s)</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
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
                <span class="bitInfo" title="">info</span>
                <a href="javascript:;" class="bitTextareaEnlargeButton">enlarge </a>
            </div>
            <div class="bitPageSettingsCollumnC">
                <textarea id="javascriptModules_0" class="bitTextArea" data-field="JsFunction"></textarea>
            </div>
            <br clear="all" />
        </div>
    </fieldset>
</div>
