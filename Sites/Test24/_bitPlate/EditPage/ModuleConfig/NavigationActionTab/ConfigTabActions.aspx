<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigTabActions.aspx.cs" Inherits="BitSite._bitPlate.EditPage.ModuleConfig.Actions.ConfigTabActions" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <div id="defaultActionTab">
        <div id="divNavigationActions">
            <fieldset id="fieldsetNavigation_0">
                <legend>Navigatie</legend>
                <div class="bitPageSettingsCollumnA">Tag</div>
                <div class="bitPageSettingsCollumnB">
                    <span  class="bitInfo" title="">info</span>
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
                    <select id="selectNavigationType_0" onchange="javascript:BITEDITPAGE.changeNavigationType(this, 0);">
                        <option value="0">Navigeer naar pagina</option>
                        <option value="1">Laad data in module(s)</option>
                        <option value="2" disabled="disabled">Open popup</option>
                        <option value="3">Run javascript</option>
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
                </div>
                <div id="divNavigationType3_0" style="display: none">
                    <div class="bitPageSettingsCollumnA">Javascript</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <textarea id="javascriptModules_0" class="bitTextArea"></textarea>
                    </div>
                    <br clear="all" />
                </div>
            </fieldset>
        </div>
    </div>
</body>
</html>
