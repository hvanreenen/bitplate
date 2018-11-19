<%@ Page Title="Pagina's" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Pages.aspx.cs" Inherits="BitSite._bitPlate.Pages.Pages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="/_bitplate/_themes/bitplate/plugins/jbreadcrump/Styles/BreadCrumb.css" />
    <script type="text/javascript" src="BITPAGES.js"></script>
    
    <!--<link rel="stylesheet" type="text/css" href="_js/plugins/jbreadcrump/Styles/Base.css" />-->
    
    <script src="/_bitplate/_js/plugins/jbreadcrump/js/jquery.easing.1.3.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/jbreadcrump/js/jquery.jBreadCrumb.1.1.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITPAGES.initialize();
            BITPAGES.loadPages("");
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        <li id="liAddPage" runat="server"><a id="aAddPage" runat="server" href="javascript:BITPAGES.newPage();"
            class="bitNavBarButtonAddPage">addpage</a>
            <div>Nieuwe pagina</div>
        </li>
        <li id="liAddFolder" runat="server"><a id="aAddFolder" runat="server" href="javascript:BITPAGES.newFolder();"
            class="bitNavBarButtonAddFolder">nieuwe folder</a>
            <div>Nieuwe folder</div>
        </li>

    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Pagina's
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <br />
    Locatie: 
        <div class="breadCrumb module">
            <ul data-control-type="list" id="breadcrump">
                <li><a data-field="Path" data-title-field="Name" data-text-field="Name" href="javascript:BITPAGES.loadPages('root', '[data-field]')">
                </a></li>
            </ul>
        </div>
    <table id="tablePages" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 25px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 30px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 140px" data-sort-field="Name">Naam</td>
                <td class="bitTableColumnTitle" style="width: 140px" data-sort-field="Title">Title</td>
                <td class="bitTableColumnPath" style="display: none; width: 220px" data-sort-field="Path">Map</td>
                <!--<td class="bitTableColumn" style="width: 100px" data-sort-field="ChangeStatus">Status</td>-->
                <td id="tdLangTitle" runat="server" visible="false" class="bitTableColumn" style="width: 40px">Taal</td>

                <td runat="server" id="htdEdit" class="bitTableActionButton">Bewerk</td>
                <!--<td class="bitTableActionButton">Live</td>-->
                <td class="bitTableActionButton">Preview</td>
                <td runat="server" id="htdConfig" class="bitTableActionButton">Config</td>
                <td runat="server" id="htdDelete" class="bitTableActionButton">Verwijder</td>
                <td runat="server" id="htdCopy" class="bitTableActionButton">Kopieer</td>
                <td runat="server" id="htdSelect" visible="false" class="bitTableActionButton">Selecteer</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon"><a data-field="Url" href="[data-field]?mode=edit" class="bitTablePageIcon"></a></td>
                <td class="iconAuth" ><a href="#" class="bitTableAuthIcon" style="display:none"></a></td>
                <td class="bitTableColumnName bitTableColumnEllipsis" data-field="Name" data-title-field="Name"></td>
                <td class="bitTableColumnTitle bitTableColumnEllipsis" data-field="Title" data-title-field="Title"></td>
                <td class="bitTableColumnPath bitTableColumnEllipsis" data-field="Path" data-title-field="Path"></td>
                
                <!--<td data-field="Status"></td>-->
                <td id="tdLanguageField" runat="server" visible="false" data-field="Field2">LanguageCode</td>

                <td class="bitTableActionButton" id="tdEdit" runat="server"><a data-field="Url" href="[data-field]?mode=edit"
                    class="bitEditButton"></a></td>
                <!--<td class="bitTableActionButton"><a data-field1="Field1" target="_blank" href="[data-field]"
                    class="bitLiveButton"></a></td>-->
                <td class="bitTableActionButton"><a data-field="Url" target="_blank" href="[data-field]"
                    class="bitPreviewButton"></a></td>
                <td class="bitTableActionButton" id="tdConfig" runat="server"><a id="aConfig"
                    runat="server" data-field="ID" data-title-field="Type" href="javascript:BITPAGES.openDetailsPopup('[data-field]', '[data-title-field]');"
                    class="bitConfigButton"><div class="ui-bitTooltip hidden">Pas hier de naam - titel aan; selecteer welke template de pagina gebruikt - Maak de pagina actief - welke scripts er op de pagina worden gebruikt - Zoekmachine instellingen</div></a>
                    
                </td>
                <td class="bitTableActionButton" id="tdDelete" runat="server"><a id="aDelete"
                    runat="server" data-field="ID" data-title-field="Type" href="javascript:BITPAGES.remove('[data-field]', '[data-title-field]');"
                    class="bitDeleteButton"></a></td>
                <td class="bitTableActionButton" id="tdCopy" runat="server"><a id="aCopy"
                    runat="server" data-field="ID" data-title-field="Type" data-text-field="Name"
                    href="javascript:BITPAGES.copy('[data-field]', '[data-title-field]', '[data-text-field]');"
                    class="bitCopyButton"></a></td>
                <td class="bitTableActionButton" id="tdSelect" runat="server" visible="false" ><a id="aSelect"
                    runat="server" data-field="ID" data-title-field="Url" 
                    href="javascript:BITPAGES.select('[data-field]', '[data-title-field]');"
                    class="bitSelectButton"></a></td>
                <td class="bitTableDateColumn" data-field="CreateDate"></td>
            </tr>
        </tbody>
    </table>
    <div class="bitEndTable"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <div id="bitPageDialog" title="Pagina eigenschappen" style="display: none">
        <div id="bitTabsPages" class="bitTabs">
            <ul>
                <li><a href="#tabPageDetails1">Algemene instellingen</a></li>
                <li><a href="#tabPageDetails2">Scripts & Header</a></li>
                <li><a href="#tabPageDetails3">Zoek machine instellingen</a></li>
                <li><a href="#tabPageDetails4" id="moduleRecoverButton">Modules</a></li>
            </ul>
            <!-- TAB1 -->
            <div id="tabPageDetails1" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Bestandsnaam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voeg hier de bestandsnaam in van de pagina. De pagina krijgt zelf achtervoegsel .aspx. ">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" data-field="Name" data-validation="required" name="bitPageName"
                        id="bitPageName" title="Voeg hier de bestandsnaam in van de pagina. De pagina krijgt zelf achtervoegsel .aspx" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Titel</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voeg hier de titel in van de pagina. De titel wordt weergegeven in de head van de pagina. De titel is belangrijk voor zoekmachines. Verwerk hierin dus zoektermen.">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" data-field="Title" data-validation="required" name="bitPageName"
                        id="bitPageTitle" title="Voeg hier de titel in van de pagina. De title wordt weergegeven in de head van de pagina" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Layout template</div>
                <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Kies de layout template. In de template staan vakken gedefineerd waarin u bij het bewerken van de pagina, stukken html en functionaliteit kan zetten. ">
                        info</span></div>
                <div class="bitPageSettingsCollumnC">
                    <div class="imgContainer">
                        <img id="imgTemplateScreenShot" src="null" data-field="Template.Screenshot" alt="Template.Name"
                            title="Template.Screenshot" /><br />
                        <span id="spanTemplateName" data-field="Template.Name"></span>
                    </div>
                    <button type="button" onclick="javascript:BITPAGES.showTemplateSelector();">
                        wijzig
                    </button>
                    <br />

                </div>
                <br clear="all" />

                

                <div id="divLanguage" runat="server" visible="false">
                    <div class="bitPageSettingsCollumnA">Taal</div>
                    <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="De taal van de template.">
                        info</span></div>
                    <div class="bitPageSettingsCollumnC">
                        <span id="spanLanguageCode" data-field="Template.LanguageCode"></span>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                </div>
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Map</div>
                <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Binnen welke (sub)map valt deze pagina?">
                        info</span></div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectPageFolders" runat="server" data-field="Folder.ID" data-text-field="Folder.RelativePath">
                    </select>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Status</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voer hier in of de pagina actief is, in actief, of actief binnen een bepaalde periode. (Om te testen of inactieve pagina's daadwerkelijk inactief zijn, dient u nieuwe browsersessie te starten zonder in te loggen in Bitplate.) ">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <span data-field="IsActiveString"></span>
                    <br />
                    <br />
                    <div>
                        <span class="radio1" title="status">
                            <input checked="checked" type="radio" id="radioStatus1" name="status" data-field="Active" value="1" /><label for="radioStatus1">Actief
                            </label>
                        </span>
                    </div>
                    <div>
                        <span class="radio1" title="status">
                            <input type="radio" id="radioStatus2" name="status" data-field="Active" value="2" /><label for="radioStatus2">Actief vanaf:
                            </label>
                            <input type="date" data-field="DateFrom" class="bitDatepicker" />
                            Tot:
                        <input type="date" data-field="DateTill" class="bitDatepicker" />
                        </span>
                    </div>
                    <div>
                        <span class="radio1" title="status">
                            <input type="radio" id="radioStatus3" name="status" data-field="Active" value="0" /><label for="radioStatus3">Niet actief
                            </label>
                        </span>
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->


                <div class="bitPageSettingsCollumnA">Homepage</div>
                <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Is deze pagina homepage binnen deze folder. Bij navigeren via domeinnaam.nl/foldernaam/ wordt de bezoeker doorgelinkt naar deze pagina.">
                        info</span></div>
                <div class="bitPageSettingsCollumnC">
                    <span class="checkbox1">
                        <input type="checkbox" data-field="IsHomePage" name="bitDefaultPage" id="bitDefaultPage" /><label
                            for="bitDefaultPage" title="Is standaardpagina?">Is homepage binnen deze folder?
                        </label>
                    </span>
                </div>
                <br clear="all" />
            </div>
            <!-- TAB2 -->
            <div id="tabPageDetails2" class="bitTabPage">
                <!-- ROW 
                <div class="bitPageSettingsCollumnA"></div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <button id="bitButtonAddScript">Toevoegen</button>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <!-- ROW -->
                <div class="bitPageSettingsCollumnA">Scripts</div>
                <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Voeg hier eventueel extra scripts & stylesheets toe. De pagina krijgt zowiezo de scripts & stylesheets die aan de site en de template zijn gekoppeld.">
                        info</span></div>
                <div class="bitPageSettingsCollumnC">
                    <input type="button" value="Toevoegen" onclick="javascript:BITPAGES.openScriptsPopup();" />
                    <table id="tableScriptsPerSite" style="width: 100%">
                        <tbody>
                            <tr>
                                <td style="width: 250px" data-field="CompleteName"></td>
                                <td>(overgenomen van site)</td>
                            </tr>
                        </tbody>
                    </table>
                    <table id="tableScriptsPerTemplate" style="width: 100%">
                        <tbody>
                            <tr>
                                <td style="width: 250px" data-field="CompleteName"></td>
                                <td>(overgenomen van template)</td>
                            </tr>
                        </tbody>
                    </table>
                    <table id="tableScriptsPerPage">
                        <tbody>
                            <tr>
                                <td style="width: 250px" data-field="CompleteName"></td>
                                <td style="width: 50px"><a data-child-field="ID" title="verwijder" href="javascript:BITPAGES.removeScript('[data-field]', [list-index]);"
                                    class="bitDeleteButton">verwijder </a></td>
                            </tr>
                        </tbody>
                    </table>
                    <!--
                    <ul id="bitPageScriptList" data-control-type="list">
                        <li class="ui-state-default">
                            <input type="hidden" data-field="ID" />
                            <span data-field="Name"></span><span data-field="Type"></span><span class="ui-icon ui-icon-trash btn-remove-script"
                                style="display: inline-block; float: right; cursor: pointer"></span></li>
                    </ul>
                    -->
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Extra header-inhoud</div>
                <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="De inhoud van dit veld wordt extra in de head van de pagina gezet.">
                        info</span>
                    <a href="javascript:;" class="bitTextareaEnlargeButton">enlarge </a>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <textarea name="bitExtraHeaderContent" id="bitExtraHeaderContent" class="bitTextArea"
                        data-field="HeadContent"></textarea>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Body class</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="U kunt de <body> tag van een pagina een eigen css-class geven. Vul hier de naam in van die class.">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" data-field="BodyTagContent"  name="bitPageBodyClass"
                        id="bitPageBodyClass" />
                </div>
                <br clear="all" />
            </div>

            <!-- TAB3: Zoek machine instellingen -->
            <div id="tabPageDetails3" class="bitTabPage">
                <!-- ROW -->
                <div class="bitPageSettingsCollumnA">Pagina beschrijving (Meta-Description)</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Beschrijving is belangrijk voor zoekmachines. Geet hier een korte samenvatting welke in de resultaten van de zoekmachine verschijnt. 
                        Indien hier iets wordt ingevuld worden de beschrijving bij algemene instellingen (site beheer) overschreven.">
                        info</span>
                    <a href="javascript:;" class="bitTextareaEnlargeButton"></a>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <textarea data-field="MetaDescription" class="bitTextArea"></textarea>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Sleutelwoorden (Meta-Keywords)"</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Sleutelwoorden zijn belangrijk voor zoekmachines. Voer ze komma gescheiden in. Let erop dat de zoekwoorden ook in de tekst op de pagina voorkomen. 
                        Indien hier iets wordt ingevuld worden de sleutelwoorden bij algemene instellingen (site beheer) overschreven.">
                        info</span>
                    <a href="javascript:;" class="bitTextareaEnlargeButton">enlarge </a>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <textarea data-field="MetaKeywords" class="bitTextArea"></textarea>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Sitemap.xml</div>
                <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Een sitemap.xml is een extra 'plattegrond' voor zoekmachines. Geef hier aan of de pagina in de sitemap dient te worden opgenomen. LET WEL: sitmap.xml wordt pas geupdate bij publiceren van de site.">
                        info</span></div>
                <div class="bitPageSettingsCollumnC">
                    <span class="checkbox">
                        <input type="checkbox" data-field="InSiteMap" onchange="javascript:BITPAGES.inSideMapChange();"
                            name="bitInSitemap" id="bitInSitemap" /><label for="bitInSitemap" title="In sitemap">In
                                sitemap
                            </label>
                    </span>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Wijzigingsfrequentie</div>
                <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Geef hier aan wat de verwachte frequentie is waarmee nieuwe inhoud op deze pagina verschijnt. De zoekmachine heeft dan een indruk om de hoeveel tijd de pagina dient te worden geherindexeerd">
                        info</span></div>
                <div class="bitPageSettingsCollumnC">
                    <select name="SiteMapChangeFreq" id="SiteMapChangeFreq" data-field="SiteMapChangeFreq">
                        <option value="Always">Voortdurend</option>
                        <option value="Hourly">Per uur</option>
                        <option value="Daily">Dagelijks</option>
                        <option value="Weekly">Wekelijks</option>
                        <option value="Monthly">Maandelijks</option>
                        <option value="Yearly">Jaarlijks</option>
                        <option value="Never">Nooit</option>
                    </select>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Prioriteit</div>
                <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Geef voor de zoekmachine hier aan hoe groot de prioriteit is van deze pagina binnen de gehele site.">
                        info</span></div>
                <div class="bitPageSettingsCollumnC">
                    <select name="SiteMapPriority" id="SiteMapPriority">
                        <option selected="selected" value="0.0">0,0</option>
                        <option value="0.1">0,1</option>
                        <option value="0.2">0,2</option>
                        <option value="0.3">0,3</option>
                        <option value="0.4">0,4</option>
                        <option value="0.5">0,5</option>
                        <option value="0.6">0,6</option>
                        <option value="0.7">0,7</option>
                        <option value="0.8">0,8</option>
                        <option value="0.9">0,9</option>
                        <option value="1.0">1,0</option>
                    </select>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Robots.txt</div>
                <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Als u het vinkje hier aan zet verschijnt de url van deze pagina in het bestand 'robots.txt' (zie site beheer, algemene instellingen). Hiermee geeft u aan dat deze pagina NIET moet worden geindexeerd door zoekmachines. LET WEL: robots.txt wordt pas geupdate bij publiceren van de site.">
                        info</span></div>
                <div class="bitPageSettingsCollumnC">
                    <span class="checkbox">
                        <input type="checkbox" data-field="DisallowRobots" name="bitDenySearchengines" id="bitDenySearchengines" onchange="javascript:BITPAGES.inRobotsTxtChange();"/>
                        <label for="bitDenySearchengines" title="Zoekmachines weigeren">Zoekmachines weigeren
                        </label>
                    </span>
                </div>
                <br clear="all" />
            </div>
            <div id="tabPageDetails4" class="bitTabPage">
                Modules op deze pagina:
                <table id="tableModules" class="bitGrid">
                    <thead>
                        <tr>
                            <td>Naam</td>
                            <td>Type</td>
                            <td>Container</td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td data-field="Name"></td>
                            <td data-field="Type"></td>
                            <td data-field="ContainerName"</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>

    <!-- FOLDER DIALOG -->
    <div id="bitFolderDialog" title="Folder eigenschappen" style="display: none">
        <!-- <div id="bitFormWindowResize"><span class="ui-state-default ui-state-default-arrow-4-diag"></span></div> -->
        <!-- ROW -->
        <div id="bitTabsFolders" class="bitTabs">
            <ul>
                <li><a href="#tabFolderDetails1">Basis gegevens</a></li>
            </ul>
            <div id="tabFolderDetails1" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Naam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Naam van de map op de server. Paden tellen mee voor zoekmachines. Verwerk hierin dus zoektermen voor uw site.">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" data-field="Name" data-validation="required" />
                </div>
                <br clear="all" />
                <!--NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Valt onder</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Vul hier in onder welke (hoofd)map deze map dient te vallen.">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectFolderParentFolders" runat="server" data-field="ParentFolder.ID" data-text-field="ParentFolder.RelativePath">
                    </select>
                </div>
                <br clear="all" />
                <!--NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Status</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voer hier in of de map actief is, in actief, of actief binnen een bepaalde periode. Als u deze map inactief maakt, worden alle pagina's en sub-mappen die onder deze map vallen ook inactief.">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <!--<span data-field="ChangeStatusString"></span>
                    <br />
                    <br />-->
                    <div>
                        <span class="radio1" title="status">
                            <input checked="checked" type="radio" id="radio1" name="status" data-field="Active" value="1" /><label for="radioStatus1">Actief
                            </label>
                        </span>
                    </div>
                    <div>
                        <span class="radio1" title="status">
                            <input type="radio" id="radio2" name="status" data-field="Active" value="2" /><label for="radioStatus2">Actief vanaf:
                            </label>
                            <input type="date" data-field="DateFrom" class="bitDatepicker" />
                            Tot:
                        <input type="date" data-field="DateTill" class="bitDatepicker" />
                        </span>
                    </div>
                    <div>
                        <span class="radio1" title="status">
                            <input type="radio" id="radio3" name="status" data-field="Active" value="0" /><label for="radioStatus3">Niet actief
                            </label>
                        </span>
                    </div>
                </div>
                <br clear="all" />
            </div>
        </div>
    </div>
    
    <!-- TEMPLATE CHOOSER DIALOG -->
    <div id="bitTemplateDialog" title="Template kiezen" style="display: none;">
        <div id="divChooseTemplate" style="overflow-x: hidden; width: 585px;max-height:400px; overflow-y:scroll;"
            data-control-type="list">
            <!--<div onclick="javascript:BITPAGES.selectTemplate(this);" style="width: 150px; border: 1px solid #ddd;
                float: left">
                <input type="hidden" class='hiddenID' data-field="ID" />
                <input type="hidden" class='hiddenScripts' data-field="Scripts" />
                <img src="null" data-field="Screenshot" /><br />
                <span data-field="Name"></span> <strong data-field="LanguageCode"></strong>
            </div>-->
            <asp:Literal ID="LiteralLayoutTemplates" runat="server"></asp:Literal>
        </div>
    </div>

    <!-- SCRIPT DIALOG -->
    <div id="bitScriptDialog" title="Scripts kiezen" style="display: none; max-height:600px; overflow-y:scroll; overflow-x:hidden">
        <ul id="bitScriptList" style="list-style-type: none" data-control-type="list">
            <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px;
                padding: 5px">
                <a data-field="ID" data-title-field="CompleteName" href="javascript:BITPAGES.addScript('[data-field]', '[data-title-field]');">
                    <input type="hidden" data-field="ID" />
                    <span data-field="CompleteName"></span><span class="remove-script btn-remove-script">
                    </span></a></li>
        </ul>
    </div>

    <!--COPY SCRIPT DIALOG -->
    <div id="bitPageCopyDialog" title="Script Kopiëren." style="display: none;">
        <div>Kopie scriptnaam:</div>
        <div>
            <input type="text" id="CopyName" />
        </div>
    </div>
</asp:Content>
