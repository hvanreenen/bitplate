<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Templates.aspx.cs" Inherits="BitSite._bitPlate.Templates.Templates" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/_bitplate/_themes/" + BitSite.SessionObject.CurrentBitplateUser.Theme + "/css/codemirror") %>
    <script type="text/javascript" src="BITTEMPLATES.js"></script>
    <script type="text/javascript" src="BITCKEDITTEMPLATE.js"></script>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/Plugins/ckeditor") %>
    <script type="text/javascript" src="HTMLHELPER.js"></script>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/Plugins/CodeMirror") %>
    <%--<link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/CodeMirror-3.14/lib/codemirror.css" />
    <link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/CodeMirror-3.14/addon/hint/show-hint.css" />
    <link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/CodeMirror-3.14/addon/lint/lint.css" />
    <link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/CodeMirror-3.14/addon/dialog/dialog.css" />--%>
    <script type="text/javascript">
        CKEDITOR.basePath = '/_bitplate/_js/plugins/ckeditor/'; // SET ckeditor basepath.
        BITEDITTEMPLATE.siteDomein = '<%=BitSite.SessionObject.CurrentSite.DomainName %>';
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        <li id="liAddTemplate" runat="server"><a id="aAddTemplate" runat="server" href="javascript:BITTEMPLATES.newTemplate();" class="bitNavBarButtonAddPage"></a>
            <div>Nieuwe template</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Layout templates
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <table id="tableTemplates" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 200px; ">&nbsp;</td>
                <td class="bitTableColumn" style="width: 25px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 240px" data-sort-field="Name">Naam</td>
                <td id="tdLangTitle" runat="server" visible="false" class="bitTableColumn" style="width: 40px" data-sort-field="LanguageCode">Taal</td>

                <td class="bitTableActionButton">Bewerk</td>
                <td class="bitTableActionButton">Config</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableActionButton">Kopieer</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td ><a href="javascript:void(0);" onclick="BITTEMPLATES.openDetailsPopup('[data-field]', 2);" data-field="ID"><img data-field="Icon" /></a></td>
                <td class="iconAuth" ><a href="#" class="bitTableAuthIcon" style="display:none"></a></td>
                <td class="nameColumn bitTableColumnEllipsis" data-field="Name" data-title-field="Name"></td>
                <td id="tdLanguageField" runat="server" visible="false" data-field="LanguageCode"></td>
                <td class="bitTableActionButton" id="tdTemplateEdit" runat="server"><a id="aTemplateEdit" runat="server" data-field="ID" href="javascript:void(0);" onclick="BITTEMPLATES.openDetailsPopup('[data-field]', 2);"
                    class="bitEditButton"></a></td>
                <td class="bitTableActionButton" id="tdTemplateConfig" runat="server"><a id="aTemplateConfig" runat="server" data-field="ID" href="javascript:void(0);" onclick="BITTEMPLATES.openDetailsPopup('[data-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton" id="tdTemplateRemove" runat="server"><a id="aTemplateRemove" runat="server" data-field="ID" href="javascript:void(0);" onclick="BITTEMPLATES.remove('[data-field]');"
                    class="bitDeleteButton"></a></td>
                <td class="bitTableActionButton" id="tdTemplateCopy" runat="server"><a id="aTemplateCopy" runat="server" data-field="ID" data-title-field="Name" href="javascript:void(0);" onclick="BITTEMPLATES.copy('[data-field]', '[data-title-field]');"
                    class="bitCopyButton"></a></td>
                <td class="bitTableDateColumn" data-field="CreateDate"></td>
            </tr>
        </tbody>
    </table>
    <div class="bitEndTable"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <div id="bitTemplateDialog" title="Template eigenschappen" style="display: none">
        <!-- <div id="bitFormWindowResize"><span class="ui-state-default ui-state-default-arrow-4-diag"></span></div> -->
        <!-- ROW -->
        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageDetails1">Algemene instellingen</a></li>
                <li id="liScripts"><a href="#tabPageDetails2">Scripts</a></li>
                <li><a href="#tabPageDetails3">HTML (Geavanceerd)</a></li>
            </ul>
            <!-- TAB1: Home page -->
            <div id="tabPageDetails1" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Naam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voeg hier de naam van de template. Aan de hand van deze naam kunt u de template later herkennen.">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="textNaam" type="text" data-field="Name" data-validation="required" class="required" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <!--<div class="bitPageSettingsCollumnA">Nieuwsbrief compatible</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Templates welke nieuwsbrief compatible zijn mogen worden gebruikt in een nieuwsbrief. Designers LETOP email clients zijn niet het zelfde als webbrowsers. Maak gebruik van oude html techniek om compatible te blijven.">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="checkbox" data-field="IsNewsletterTemplate" />
                    </div>
                <br clear="all" />-->
                <!-- NEXT ROW -->
                <div id="divLanguage" runat="server" visible="false">
                    <div class="bitPageSettingsCollumnA">Taal</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Voeg hier de taal in. Als de gebruiker een taal heeft gekozen worden alleen de pagina's die deze template hebben weergegeven.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="dropdownLanguages" runat="server" data-field="LanguageCode"></select>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                </div>
                <div class="bitPageSettingsCollumnA">Status</div>
                <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Voer hier in of de template actief is, in actief, of actief binnen een bepaalde periode. Als u deze template inactief maakt, worden alle pagina's die deze template gebruiken ook inactief.">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <!--<span data-field="ChangeStatusString"></span>
                    <br />
                    <br />-->
                    <div>
                        <input checked="checked" type="radio" name="status" data-field="Active" value="1" />Actief
                    </div>
                    <div>
                        <input type="radio" name="status" data-field="Active" value="2" />Actief vanaf:
                        <input type="date" data-field="DateFrom" />
                        tot:
                        <input type="date" data-field="DateTill" />
                    </div>
                    <div>
                        <input type="radio" name="status" data-field="Active" value="0" />Niet actief
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Template is in gebruik door de volgende pagina's</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Voeg hier de taal in. Als de gebruiker een taal heeft gekozen worden alleen de pagina's die deze template hebben weergegeven.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <ul id="bitTemplateUsedInPages">
                            <li data-field="Name"></li>
                        </ul>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->

            </div>
            <div id="tabPageDetails2" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Scripts</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Deze scripts & stylesheets worden (samen met de scripts & stylesheets van de site) in de header gezet van de pagina's die deze template gebruiken.">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="button" value="Toevoegen" onclick="javascript:BITTEMPLATES.openScriptsPopup();" />
                    <table id="tableScriptsPerSite" style="width: 100%">
                        <tbody>
                            <tr>
                                <td style="width: 250px" data-field="CompleteName"></td>
                                <td>(overgenomen van site)</td>
                            </tr>
                        </tbody>
                    </table>
                    <table id="tableScriptsPerTemplate">
                        <tbody>
                            <tr>
                                <td data-field="CompleteName" style="width: 250px"></td>
                                <td><a data-child-field="ID" title="verwijder" href="javascript:BITTEMPLATES.removeScript('[data-field]', [list-index]);"
                                    class="bitDeleteButton">delete</a></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
            </div>
            <div id="tabPageDetails3" class="bitTabPage" style="display: none;">
                <div>
                    <textarea data-field="Content" id="bitTemplateHtmlEditor" ></textarea>
                </div>
            </div>
        </div>
        <!-- SCRIPT DIALOG -->
        <div id="bitScriptDialog" title="Scripts kiezen" style="display: none;max-height:600px; overflow-y:scroll; overflow-x:hidden">
            <ul id="bitScriptList" style="list-style-type: none" data-control-type="list">
                <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px;
                    padding: 5px"><a data-field="ID" data-title-field="CompleteName" href="javascript:BITTEMPLATES.addScript('[data-field]', '[data-title-field]');">
                        <span data-field="CompleteName"></span><span class="remove-script btn-remove-script">
                        </span></a></li>
            </ul>
        </div>
        
        <!--COPY DIALOG -->
        <div id="bitTemplateCopyDialog" title="Template Kopiëren." style="display: none">
            <div>Kopie templatenaam:</div>
            <div><input type="text" id="CopyName" /></div>
        </div>

        <div id="bitTemplateEditDialog" title="Template bewerken.">
            <textarea id="bitTemplateEditor"></textarea>
        </div>
    </div>
</asp:Content>
