<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/bitMasterPages/CKEditWysiwyg.master"
    AutoEventWireup="true" CodeBehind="CKEditNewsletter.aspx.cs" Inherits="BitSite._bitPlate.CKEditNewsletter" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">

    <%= BitBundler.ResolveBundleUrl("~/Plugins/ckeditor") %>
    <script type="text/javascript" src="_js/pages/popups/BITIMAGESPOPUP.js"></script>
    <script type="text/javascript" src="_js/pages/popups/BITHYPERLINKSPOPUP.js"></script>
    <script type="text/javascript" src="_bitModules/BITALLMODULES.js"></script>
    <script src="_js/pages/BITCKEDITNEWSLETTER.js" type="text/javascript"></script>
    <script src="_js/pages/BITNEWSLETTER.js" type="text/javascript"></script>
    <script type="text/javascript" src="_js/prototypes/contextMenu.js"></script>
    <script type="text/javascript" src="_js/prototypes/insertAtCaret.js"></script>
    <link rel="stylesheet" type="text/css" href="_themes/bitplate/plugins/jsTree/themes/default/style.css" />
    
    <!--
    <script type="text/javascript" src="_js/_bitModules/BITAUTHMODULES.js"></script>
    <script type="text/javascript" src="_js/_bitModules/BITDATAMODULES.js"></script>
    <script type="text/javascript" src="_js/_bitModules/BITSEARCHMODULES.js"></script>
    <script type="text/javascript" src="_js/_bitModules/HELLOWORLDMODULES.js"></script>
    <script type="text/javascript" src="_js/_bitModules/LoadData.js"></script>

    <script type="text/javascript" src="_js/_bitModules/plugins/QapTcha/jquery/jquery.ui.touch.js"></script>
    <script type="text/javascript" src="_js/_bitModules/plugins/QapTcha/jquery/QapTcha.jquery.js"></script>
    <script type="text/javascript" src="_js/_bitModules/plugins/QapTcha/jquery/QapTcha.jquery.css"></script>
    -->
    <link rel="stylesheet" type="text/css" href="_js/plugins/CodeMirror-2.32/lib/codemirror.css" />
    <%= BitBundler.ResolveBundleUrl("~/Plugins/CodeMirror") %>
    <%= BitBundler.ResolveBundleUrl("~/Plugins/jsTree") %>

    <script type="text/javascript">
        /// <reference path="_js/BITEDITPAGE.js" />
        //startup functie
        $(document).ready(function () {

            /* BITEDITPAGE.toolbarTopMenu = BITEDITOR.createToolbar("bitToolbar");
            BITEDITPAGE.toolbarTopMenu.toolbarItems["back"].show(); //terug knop zichtbaar maken

            BITEDITPAGE.statusbarTopMenu = BITEDITOR.createStatusbar("bitStatusbar");
            BITEDITPAGE.toolbarInPopup = BITEDITOR.createToolbar("bitToolbarInPopup");
            BITEDITPAGE.statusbarInPopup = BITEDITOR.createStatusbar("bitStatusbarInPopup"); */

            $(document).scrollTop(0);

            //alle pagina's uit de site in de selectpages zetten, waarmee de gebruiker naar andere pagina kan switchen
            BITEDITPAGE.fillMenuPages();
            BITEDITPAGE.initialize();
            BITEDITPAGE.siteDomainName = '<%=BitSite.SessionObject.CurrentSite.DomainName %>';
            if (location.hash != "") {
                var hash = location.hash.replace('#', '');
                var queryString = BITUTILS.getQueryString();
                if (queryString && queryString["referrer"] != null) {
                    //BITEDITOR.referrerUrl = queryString["referrer"];
                }
                if (hash.indexOf("Template") >= 0) {
                    var id = hash.replace("Template", "");
                    BITEDITPAGE.loadTemplate(id);
                }
                else {
                    var id = hash;
                    BITEDITPAGE.loadPage(id);
                }
            }

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SlideMenuPlaceHolder" runat="server">

    <ul class="bitModuleContextMenu" style="display: none; width: 150px; z-index: 500">
        <li><a href="javascript:BITEDITPAGE.openEditModulePopup('{0}');"><span class="ui-icon ui-icon-pencil"></span>Bewerken</a></li>
        <li><a href="javascript:BITEDITPAGE.configModule('{0}', '{2}');"><span class="ui-icon ui-icon-gear"></span>Configureren</a></li>
        <li><a href="javascript:BITEDITPAGE.moveModule('{0}');"><span class="ui-icon ui-icon-arrow-4"></span>Verplaatsen</a></li>
        <li><a href="javascript:BITEDITPAGE.deleteModule('{0}');"><span class="ui-icon ui-icon-trash"></span>Verwijderen</a></li>
    </ul>

    <!--<div id="bitToolbarWrapper">
        <div id="bitToolbar" class="bitToolbar">
        </div>
    </div>-->
    <br style="clear: both" />

    <div id="bitStatusbarWrapper">
        <div id="bitStatusbar" class="bitSatusbar">
            <div id="hiddenModuleTemp" style="display: none;"></div>
        </div>
    </div>
    <br style="clear: both" />

    <div id="bitSideBar">
        <!-- bitPlateDealer -->
        <div class="bitSideBarLock"></div>
        <ul id="bitplateULDealer">
            <li class="bitLiNoMenu">
                   <span><img src="_themes/bitplate/images/BPCMS_sidebar_icon.png" id="imgBitPlateDealer" alt="abcnet" /></span><span class="bitSideBarText">Abcnet</span>
            </li>
        </ul>
        
        <!-- .bitPlateDealer -->
        <hr />

        <!-- pages -->
        <div class="bitSidebarGroup"><span class="bitSideBarText">Paginas</span></div>
        <ul>
            <li id="bitMenuPreviewPage" class="bitLiNoMenu"><a id="linkPreviewPage" runat="server" href="" target="_blanc">
                <span id="bitSideBarIconPreviewPage"></span><span class="bitSideBarText">Preview</span></a>
            <li id="bitMenuConfigPage" class="bitLiNoMenu"><a href="javascript:BITEDITPAGE.configPage();">
                <span id="bitSideBarIconConfigPage"></span><span class="bitSideBarText">Eigenschappen</span></a>
            <li><a href="#"><span id="bitSideBarIconPages"></span><span class="bitSideBarText">Pagina's</span></a>
                <ul id="bitMenuPages">
                    <!--<div >
                        <li><a href="javascript:BITPAGES.newPage();">Nieuwe pagina...</a></li>
                    </div>-->
                </ul>
            </li>
            <li class="bitLiNoMenu"><a href="javascript:BITEDITPAGE.navigateToPage('Publish.aspx');">
                <span id="bitSideBarIconPublishPages"></span><span
                    class="bitSideBarText">Publiceer</span></a>
        </ul>
        <hr />
        <!-- .pages -->
        <!-- MODULES -->
        <div class="bitSidebarGroup"><span class="bitSideBarText">Modules</span></div>
        <ul id="bitMenuModules">
            <li style="display: none">
                <input type="checkbox" checked="checked" id="bitCheckboxAllowDrag" onclick="javascript:BITEDITPAGE.allowDragChanged();" /><label
                    for="bitCheckboxAllowDrag">slepen toestaan</label></li>
            <li><a href="#"><span id="bitSideBarIconGeneral"></span><span class="bitSideBarText">
                Algemeen</span></a>
                <ul id="ulBitModulesGeneral" runat="server">
                    <li id="liBitModuleHtml" runat="server">
                        <div class="moduleToDrag" data-module-type='bitModuleHtml'>Html</div>
                    </li>
                </ul>
            </li>
            <li><a href="#"><span id="bitSideBarIconData"></span><span class="bitSideBarText">Data
            </span></a>
                <ul id="ulBitModulesData" runat="server">
                    <!--
                    <li id="liBitModuleDataMainGroups" runat="server">
                        <div class="moduleToDrag" id='bitModuleDataMainGroups'>Hoofdgroepen</div>
                    </li>
                    <li id="liBitModuleDataGroups" runat="server">
                        <div class="moduleToDrag" id='bitModuleDataGroups'>Subgroepen</div>
                    </li>
                    <li id="liBitModuleDataItems" runat="server">
                        <div class="moduleToDrag" id='bitModuleDataItems'>Items</div>
                    </li>
                    <li id="liBitModuleDataGroupDetails" runat="server">
                        <div class="moduleToDrag" id='bitModuleDataGroupDetails'>Groep details</div>
                    </li>
                    <li id="liBitModuleDataItemDetails" runat="server">
                        <div class="moduleToDrag" id='bitModuleDataItemDetails'>Item details</div>
                    </li>
                    <li id="liBitModuleDataTree" runat="server">
                        <div class="moduleToDrag" id='bitModuleDataTree'>Boomstructuur</div>
                    </li>
                    <li id="liBitModuleDataImage" runat="server">
                        <div class="moduleToDrag" id='bitModuleDataImages'>Afbeeldingen</div>
                    </li>
                    <li id="li1" runat="server">
                        <div class="moduleToDrag" id='bitModuleHelloWorld'>Test</div>
                    </li>
                    -->
                </ul>
            </li>
            <li><a href="#" class="webshop"><span id="bitSideBarIconWebshop"></span><span class="bitSideBarText">
                Webshop</span></a>
                <ul id="ulBitModulesWebshop" runat="server">
                    <li id="liBitModuleWebshopProducts" runat="server"><a href="#">Producten</a></li>
                    <li id="liBitModuleWebshopProductDetails" runat="server"><a href="#">Product details</a>
                    </li>
                    <li id="liBitModuleWebshopCard" runat="server"><a href="#">Winkelwagen</a></li>
                    <li id="liBitModuleWebshopOrderForm" runat="server"><a href="#">Bestelformulier</a>
                    </li>
                    <li id="liBitModuleWebshopCheckout" runat="server"><a href="#">Betaal/bezorgwijze</a>
                    </li>
                    <li id="liBitModuleWebshopInvoice" runat="server"><a href="#">Bestelbon</a></li>
                    <li id="liBitModuleWebshopPayment" runat="server"><a href="#">Betalingsmodule</a>
                    </li>
                </ul>
            </li>
            <li><a href="#"><span id="bitSideBarIconLogin"></span><span class="bitSideBarText">Login</span></a>
                <ul id="ulBitModulesAuth" runat="server">
                    <!--<li id="liBitModuleAuthLogin" runat="server">
                        <div class="moduleToDrag" id='bitModuleLogin'>Inloggen</div>
                    </li>
                    <li id="liBitModuleAuthLoginData" runat="server">
                        <div class="moduleToDrag" id='bitModuleLoginGegevens'>Login gegevens</div>
                    </li>-->
                </ul>
            </li>
        </ul>
        <!-- .data collections -->
        <hr />
        <!-- site -->
        <div class="bitSidebarGroup"><span class="bitSideBarText">Site</span></div>
        <ul>
            <li class="bitLiNoMenu"><a href="javascript:BITEDITPAGE.navigateToPage('SiteConfig.aspx');">
                <span id="bitSideBarIconSiteProperties"></span><span class="bitSideBarText">Eigenschappen</span></a></li>
            <li class="bitLiNoMenu"><a href="javascript:BITEDITPAGE.navigateToPage('FileManager.aspx');">
                <span id="bitSideBarIconSiteFileManagement"></span><span class="bitSideBarText">Bestandsbeheer</span></a></li>
        </ul>
        <!-- .site -->
        <hr />
        <!-- design -->
        <div class="bitSidebarGroup"><span class="bitSideBarText">Design</span></div>
        <ul>
            <li class="bitLiNoMenu"><a href="javascript:BITEDITPAGE.editTemplate();">
                <span id="bitSideBarIconDesignTemplates"></span><span class="bitSideBarText">Bewerk template</span></a></li>
            <li><a href="#"><span id="bitSideBarIconDesignScripts"></span><span class="bitSideBarText">Scripts</span></a>
                <ul id="bitMenuScripts">
                </ul>
            </li>
            <li><a href="#"><span id="bitSideBarIconCSS"></span><span class="bitSideBarText">Stylesheets</span></a>
                <ul id="bitMenuStylesheets">
                </ul>
            </li>
        </ul>

        <hr />
        <!-- bitplate logo -->
        <div id="bitplateLogo">
            <img src="_themes/bitplate/images/BPCMS_sidebar_logo.png" width="137" height="26"
                alt="bitplate logo">
        </div>

    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <!--bij pagina's wordt de inhoud van de template + moduels in onderstaande div geladen -->
    <div id="bitEditPageBody"></div>

    <!--Voor template is hele pagina de editor-->
    <div id="bitTemplateEditorWrapper">
        <textarea id="bitTemplateEditor"></textarea>
    </div>

    <div id="bitPopups">
        <!--PAGE CONFIG DIALOG -->
        <div id="bitConfigPagePopup"></div>

        <!--MODULE EDIT DIALOG -->
        <div id="bitEditModuleDialog" title="Bewerk module">
            <div id="bitToolbarInPopup" style="display: none;"></div>
            <br style="clear: both" />
            <div id="bitStatusbarInPopup" style="display: none;"></div>
            <br style="clear: both" />
            <div id="bitModuleTagsInPopup" style="display: none;"></div>
            <div id="bitModuleEditorInPopup" style="">
                <textarea name="EditorInPopup" id="EditorInPopup"></textarea>
            </div>
            <br style="clear: both" />
        </div>

        <!--TAGS DIALOG -->
        <div id="bitTagsDialog" title="Beschikbare tags">
        </div>

        <!--IMAGES DIALOG -->
        <div id="bitImagesDialog" title="Afbeeldingen"></div>

        <!--HYPERLINKS DIALOG -->
        <div id="bitHyperlinksDialog" title="Hyperlinks"></div>

        <!--STYLES DIALOG -->
        <div id="bitStylesDialog" title="Styles"></div>

        <!--SCRIPT EDIT DIALOG -->
        <div id="bitScriptEditDialog" title="Script">
            <textarea data-field="Content" class="bitTextarea" style="width: 100%" id="textareaContent"></textarea>
        </div>

        <!--MODULE CONFIG DIALOG -->
        <div id="bitConfigModuleDialog">
        </div>

        <!--EXTRA STANDARD TABS -->
        <div id="bitConfigModuleStandardDialog">
            <ul>

                <li id="tabPage1"><a href="#standardTabPageGeneral">Algemene instellingen</a></li>
                <li id="tabPage2"><a href="#standardTabPageNavigation">Navigatie</a></li>
                <li id="tabPage3"><a href="#standardTabPageData">Gegevens</a></li>
                <li id="tabPage4"><a href="#standardTabPageListSettings">Lijst instellingen</a></li>
                <li id="tabPage9"><a href="#standardTabPageAuth">Autorisatie</a></li>
            </ul>
            <!-- TAB1 -->
            <!-- TAB1 -->
            <!-- TAB1 -->
            <div id="standardTabPageGeneral" style="display: none">
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
                    <span class="bitInfo" title="">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <span data-field="Type"></span>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Hergebruik</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input type="checkbox" id="checkVisibleCompleteSite" data-field="IsVisibleOnAllPages" />
                    <label for="checkVisibleCompleteSite">
                        Zichtbaar op alle pagina's
        van deze site</label>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA"></div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input type="checkbox" id="checkVisibleCompleteLayout" data-field="IsVisibleOnAllPagesInLayout" />
                    <label for="checkVisibleCompleteLayout">
                        Zichtbaar op alle pagina's binnen de huidige
                        layouttemplate (<span data-field="Page.Template.Name"></span>)</label>
                </div>
                <br clear="all" />
            </div>
            <!-- TAB2 -->
            <!-- TAB2 -->
            <!-- TAB2 -->
            <div id="standardTabPageNavigation" class="bitTabPage" style="display: none">
                <div class="bitPageSettingsCollumnA">Actie bij klik:</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectDrillDownType" data-field="DrillDownType" onchange="javascript:BITEDITPAGE.showDrillDownTypeDiv();">
                        <option value="0">Navigeer naar pagina</option>
                        <option value="1">Laad data in module(s)</option>
                        <option value="2" disabled="disabled">Open popup</option>
                        <option value="3">Run javascript</option>
                    </select>

                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div id="divDrillDownType0">

                    <div class="bitPageSettingsCollumnA">Pagina</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="SelectDrillDownPage" runat="server" data-field="DrillDownPage.ID"
                            data-text-field="DrillDownPage.Name">
                        </select>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                </div>
                <div id="divDrillDownType1" style="display: none">
                    <div class="bitPageSettingsCollumnA">Bij klik ververs module(s)</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <div id="checkboxListModules" data-field="DrillDownModules" data-list-type="stringArray"
                            data-control-type="checkboxlist">
                        </div>
                    </div>
                    <br clear="all" />
                </div>
                <div id="divDrillDownType3" style="display: none">
                    <div class="bitPageSettingsCollumnA">Javascript</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <textarea data-field="DrillDownJsFunction" class="bitTextArea"></textarea>
                    </div>
                    <br clear="all" />
                </div>
            </div>
            <!-- TAB3 -->
            <!-- TAB3 -->
            <!-- TAB3 -->
            <div id="standardTabPageData" class="bitTabPage" style="display: none">
                <div class="bitPageSettingsCollumnA">Dataverzameling</div>
                <div class="bitPageSettingsCollumnB">
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select id="SelectDataCollections" runat="server" data-field="DataCollection.ID"
                        data-text-field="DataCollection.Name" onchange="BITEDITPAGE.changeDataCollection(this);">
                    </select>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div id="standardTabPageData_DetailsSettings" style="display: none">
                    <div class="bitPageSettingsCollumnA">Standaard element</div>
                    <div class="bitPageSettingsCollumnB">
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="selectDefaultDataElement" data-field="Settings.DefaultDataElement">
                        </select>
                    </div>
                    <br clear="all" />
                </div>
                <div id="standardTabPageData_ListSettings" style="display: none">
                    <div class="bitPageSettingsCollumnA">Toon</div>
                    <div class="bitPageSettingsCollumnB">
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="selectShowData" data-field="Settings.ShowDataBy">
                            <option value="0">Gebruikersselectie afhankelijk</option>
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
                        <select id="bitSelectFilterFields" data-field="SelectField.ID" data-text-field="SelectField.Name">
                        </select>
                        <select data-field="SelectOperator" class="small">
                            <option value=""></option>
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
                        <input type="text" data-field="SelectValue" />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Filter op datagroep</div>
                    <div class="bitPageSettingsCollumnB">
                    </div>
                    <div class="bitPageSettingsCollumnC">
                            <span id="Span1" class="button" onclick="BITEDITPAGE.showSelectDataGroupDialog()">Bladeren</span>
                            <span id="dataCollectionPathName" data-field="SelectGroupPath"></span>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                    <div class="bitPageSettingsCollumnA">Toon ook inactieve items</div>
                    <div class="bitPageSettingsCollumnB">
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="checkbox" data-field="ShowInactive" />
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->

                    <div class="bitPageSettingsCollumnA">Sortering</div>
                    <div class="bitPageSettingsCollumnB">
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="bitSelectSortFields" data-field="SortField.ID" data-text-field="SortField.Name">
                        </select>
                        <select data-field="SortOrder" class="small">
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
                        <input type="text" data-field="ShowFromRowNumber" data-validation="number" />
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                    <div class="bitPageSettingsCollumnA">Toon max. aantal rijen</div>
                    <div class="bitPageSettingsCollumnB">
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="text" data-field="MaxNumberOfRows" data-validation="number" />
                        (0 = oneindig)
                    </div>
                    <br clear="all" />
                </div>
                <!-- NEXT ROW -->
            </div>
            <!-- TAB4 -->
            <!-- TAB4 LIST SETTINGS-->
            <!-- TAB4 -->

            <div id="standardTabPageListSettings" class="bitTabPage" style="display: none">
                <!-- NEXT ROW -->
                    <div class="bitPageSettingsCollumnA">Paginering: max.  aantal rijen per lijst</div>
                    <div class="bitPageSettingsCollumnB">
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="text" data-field="Settings.PageSize" data-validation="number" />
                        (0 = oneindig)
                    </div>
                    <br clear="all" />  
            </div>
            <!-- TAB9 -->
            <!-- TAB9 AUTORISATIE-->
            <!-- TAB9 -->

            <div id="standardTabPageAuth" class="bitTabPage" style="display: none">
                
            </div>
        </div>
    </div>



    <div id="bitFileManagementDialog"></div>

    <div id="selectDataGroupDialog" title="Selecteer Datacollectiegroep">
        <div id="dataCollectionGroupTree"></div>
    </div>
</asp:Content>
