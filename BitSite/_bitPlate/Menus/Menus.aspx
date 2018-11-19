<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Menus.aspx.cs" Inherits="BitSite._bitPlate.Menus.Menus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/_bitplate/_themes/" + BitSite.SessionObject.CurrentBitplateUser.Theme + "/css/codemirror") %>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/Plugins/CodeMirror") %>
    <script type="text/javascript" src="BITMENUS.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITMENUS.initialize();
            BITMENUS.loadMenus();
            BITMENUS.loadScripts();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        <li id="liAddMenu" runat="server"><a id="aAddMenu" runat="server" href="javascript:BITMENUS.newMenu();" class="bitNavBarButtonAddPage"></a>
            <div>Nieuw menu</div>
        </li>

    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Menus
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <table id="tableMenus" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 25px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 30px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 240px" data-sort-field="Name">Naam</td>
                <td class="bitTableColumn" style="width: 240px" data-sort-field="Type">Soort</td>
                <td id="tdLangTitle" runat="server" visible="false" class="bitTableColumn" style="width: 60px" data-sort-field="LanguageCode">Taal</td>

                <td class="bitTableActionButton" id="htdMenuData" runat="server">Items</td>
                <td class="bitTableActionButton" id="htdMenuConfig" runat="server">Config</td>
                <td class="bitTableActionButton" id="htdMenuRemove" runat="server">Verwijder</td>
                <td class="bitTableActionButton" id="htdMenuCopy" runat="server">Kopieer</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon"><a href="#" class="bitTablePageIcon"></a></td>
                <td class="iconAuth"><a href="#" class="bitTableAuthIcon" style="display: none"></a></td>
                <td class="bitTableColumnEllipsis" data-field="Name" data-title-field="Name"></td>
                <td class="bitTableColumn" data-field="TypeString"></td>
                <td id="tdLanguageField" runat="server" visible="false" data-field="LanguageCode"></td>
                <td class="bitTableActionButton" id="tdMenuData" runat="server"><a id="aMenuData" runat="server" data-field="ID" data-title-field="Type" href="MenuItems.aspx?menuId=[data-field]&menuType=[data-title-field]"
                    class="bitEditButton"></a></td>
                <td class="bitTableActionButton" id="tdMenuConfig" runat="server"><a id="aMenuConfig" runat="server" data-field="ID" href="javascript:BITMENUS.openMenuDetailsPopup('[data-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton" id="tdMenuRemove" runat="server"><a id="aMenuRemove" runat="server" data-field="ID" href="javascript:BITMENUS.removeMenu('[data-field]');"
                    class="bitDeleteButton"></a></td>
                <td class="bitTableActionButton" id="tdMenuCopy" runat="server"><a id="aMenuCopy" runat="server" data-field="ID" data-title-field="Name" href="javascript:BITMENUS.copyMenu('[data-field]', '[data-title-field]');"
                    class="bitCopyButton"></a></td>
                <td class="bitTableDateColumn" data-field="CreateDate"></td>
            </tr>
        </tbody>
    </table>
    <div class="bitEndTable"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <!--DATACOLLECTION CONFIG DIALOG -->
    <div id="bitMenuConfigDialog" title="Menu eigenschappen"
        style="display: none">
        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageDetails1">Algemene instellingen</a></li>
                <li><a href="#tabPageDetails2">Gekoppelde scripts</a></li>

            </ul>
            <!-- TAB1: Home page -->
            <div id="tabPageDetails1" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Naam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voeg hier de naam van de menuverzameling. Aan de hand van deze naam kunt u de template later herkennen."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="textNaam" type="text" data-field="Name" data-validation="required" class="required" maxlength="50" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Soort</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Per soort is er eigen js en css (zie scripts)"></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select data-field="Type">
                        <option value="1">Eenvoudig menu: horizontaal (1 dimensioneel)</option>
                        <option value="2">Eenvoudig menu: verticaal (1 dimensioneel)</option>
                        <option value="3">Dropdown menu: horizontaal</option>
                        <option value="4">Dropdown menu: verticaal</option>
                        <option value="5">Accordion menu</option>
                        <option value="0">Custom</option>
                    </select>(Per soort is er eigen js en css (zie scripts-tab))
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div id="divLanguage" runat="server" visible="false">
                    <div class="bitPageSettingsCollumnA">Vaste taal voor deze menucollectie</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Voeg hier eventueel een vaste taal in. Als u geen vaste taal kiest zijn de datacollectievelden per veld meertalig te maken.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="dropdownLanguages" runat="server" data-field="LanguageCode" onchange="javascript: BITDATACOLLECTIONS.changeFixedLanguage();"></select>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                </div>
            </div>
            <div id="tabPageDetails2" class="bitTabPage">

                <div class="bitPageSettingsCollumnA">Scripts</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voeg hier scripts & stylesheets toe, die gedrag en lay-out van het menu bepalen.">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="button" value="Toevoegen" onclick="javascript: BITMENUS.openScriptsPopup();" />

                    <table id="tableScriptsPerMenu">
                        <tbody>
                            <tr>
                                <td style="width: 250px" data-field="CompleteName"></td>
                                <td style="width: 50px"><a data-child-field="ID" title="bewerk" href="javascript:BITMENUS.editScript('[data-field]');"
                                    class="bitEditButton">Bewerk </a></td>
                                <td style="width: 50px"><a data-child-field="ID" title="verwijder" href="javascript:BITMENUS.removeScript('[data-field]', [list-index]);"
                                    class="bitDeleteButton">verwijder </a></td>
                            </tr>
                        </tbody>
                    </table>

                </div>
                <br clear="all" />

            </div>
        </div>
    </div>

    <!-- SCRIPT DIALOG -->
    <div id="bitScriptDialog" title="Scripts kiezen" style="display: none; max-height: 600px; overflow-y: scroll; overflow-x: hidden">
        <ul id="bitScriptList" style="list-style-type: none" data-control-type="list">
            <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px; padding: 5px">
                <a data-field="ID" data-title-field="CompleteName" href="javascript:BITMENUS.addScript('[data-field]', '[data-title-field]');">
                    <input type="hidden" data-field="ID" />
                    <span data-field="CompleteName"></span><span class="remove-script btn-remove-script"></span></a></li>
        </ul>
    </div>

    <!--SCRIPT EDIT DIALOG -->
    <div id="bitScriptEditDialog" title="Script">
        <div>
            <textarea data-field="Content" class="bitTextarea" style="width: 100%" id="textareaContent"></textarea>
        </div>
    </div>
</asp:Content>
