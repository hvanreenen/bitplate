<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Details.master"
    AutoEventWireup="true" CodeBehind="SiteConfig.aspx.cs" Inherits="BitSite._bitPlate.Site.SiteConfig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="BITSITE.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.button, button').button();
            BITSITE.initialize();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack" runat="server" id="BackLink">back</a>
            <div>Terug</div>
        </li>
        <li id="liSaveSiteConfig" runat="server"><a id="aSaveSiteConfig" runat="server" href="javascript:BITSITE.saveSiteConfig();" class="bitNavBarButtonSavePage">Opslaan</a>
            <div>Opslaan</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Site instellingen
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="DetailsPlaceHolder" runat="server">
    <br />
    <h2 id="bitInvalidLicenseMessage" style="display: none">Licentie is ongeldig</h2>
    <br />
    <div id="panelDetails" title="Site instellingen">
        <!-- <div id="bitFormWindowResize"><span class="ui-state-default ui-state-default-arrow-4-diag"></span></div> -->
        <!-- ROW -->
        <div class="bitTabs">

            <ul>
                <li><a href="#tabPageDetails1">Algemeen</a></li>
                <!--<li><a href="#tabPageDetails2">SMTP</a></li>-->
                <li><a href="#tabPageDetails3">Omgevingen</a></li>
                <li><a href="#tabPageDetails4">Head & Scripts</a></li>
                <li><a href="#tabPageDetails5">Afbeelding</a></li>
                <li><a href="#tabPageDetails6">Zoekmachine</a></li>
                <li><a href="#tabPageDetails7">Talen</a></li>
            </ul>
            <!-- TAB1 -->
            <div id="tabPageDetails1" class="bitTabPage">

                <div>
                    <div class="bitPageSettingsCollumnA">Naam:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Vul hier de naam van de site in. Deze naam is onafhankelijk van de domeinnaam (www), maar dient om de site te herkennen als er meerdere sites onder deze licentie hangen.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="text" name="Name" data-field="Name" data-validation="required" class="required" />
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div>
                    <div class="bitPageSettingsCollumnA">Licentiecode:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Vult u hier uw licentie sleutel in. Aan de hand van deze sleutel wordt gevalideerd welke functies door u gebruikt mogen worden in het CMS.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="text" name="LicenceCode" data-field="LicenceCode" data-validation="required"
                            class="required" />
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div>
                    <div class="bitPageSettingsCollumnA">Huidige omgeving:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Per site zijn meerdere (test)omgevingen in te stellen. Hier staat de huidige werk-omgeving.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <span data-field="CurrentWorkingEnvironment.Name"></span>
                    </div>
                </div>
                <div>
                    <div class="bitPageSettingsCollumnA">Huidige domein:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="De domeinnaam van de huidige omgeving.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <span data-field="CurrentWorkingEnvironment.DomainName"></span>
                    </div>
                </div>
                <div>
                    <div class="bitPageSettingsCollumnA">Huidige locatie op server:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Het pad op de server van de huidige omgeving.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <span data-field="CurrentWorkingEnvironment.Path"></span>
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                
            </div>
            <!-- TAB2 -->
            <%/* 
            <div id="tabPageDetails2" class="bitTabPage">
                <fieldset>
                    <legend>Standaard SMTP instellingen <span class="bitInfo" title="Standaard STMP kan per omgeving worden overschreven">info</span></legend>

                    <div>
                        <div class="bitPageSettingsCollumnA">SMTP Server:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="EmailSettingsHost" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->

                    <div>
                        <div class="bitPageSettingsCollumnA">Verzend adres:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="EmailSettingsFrom" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                    <div>
                        <div class="bitPageSettingsCollumnA">Gebruiker:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="EmailSettingsUser" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                    <div>
                        <div class="bitPageSettingsCollumnA">Wachtwoord:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="EmailSettingsPassword" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                </fieldset>
            </div>
                */ %>
            <!-- TAB3 -->
            <!-- TAB3 Omgevingen -->
            <!-- TAB3 -->
            <div id="tabPageDetails3" class="bitTabPage">
                <div id="Div1">
                    <div onclick="BITSITE.addEnvironment()" class="button">Toevoegen</div>
                </div>
                <table id="tableEnvironments" class="bitGrid" style="width:970px" data-control-type="table" cellspacing="0" cellpadding="2">
                    <thead>
                        <tr>
                            <td class="bitTableColumn">Naam</td>
                            <td class="bitTableColumn">Url</td>
                            <td class="bitTableColumn">Publiceer</td>
                            <td class="bitTableColumn">Laatste publicatie</td>

                            <td class="bitTableActionButton"></td>
                            <td class="bitTableActionButton"></td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td data-child-field="Name"></td>
                            <td data-child-field="DomainName"></td>
                            <td data-child-field="SiteEnvironmentTypeString"></td>
                            <td data-child-field="LastPublishedDate"></td>

                            <td class="bitTableActionButton">
                                <div data-child-field="ID" onclick="BITSITE.editEnvironment([list-index]);" class="bitConfigButton">Config</div>
                            </td>

                            <td class="bitTableActionButton">
                                <div data-child-field="ID" onclick="BITSITE.deleteEnvironment([list-index]);"
                                    class="bitDeleteButton">
                                    delete
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <!-- TAB4 -->
            <div id="tabPageDetails4" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Site scripts</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Deze scripts (javascript en css) worden in de header gezet van ALLE pagina's van de site.">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="button" value="Toevoegen" onclick="javascript: BITSITE.openScriptsPopup();" />
                    <table id="tableScriptsPerSite">
                        <tbody>
                            <tr>
                                <td><a data-child-field="ID" title="verwijder" href="javascript:BITSITE.removeScript('[data-field]', [list-index]);"
                                    class="bitDeleteButton">delete</a></td>
                                <td data-field="CompleteName"></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
   
                <div>
                    <div class="bitPageSettingsCollumnA">Standaard header</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Inhoud van dit veld wordt altijd in de head geplaatst van iedere pagina van de site.">info</span>
                        <a href="javascript:;" class="bitTextareaEnlargeButton">enlarge </a>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <textarea data-field="HeadContent" class="bitTextArea"></textarea>
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
            </div>
            <!-- TAB5 -->
            <div id="tabPageDetails5" class="bitTabPage">
                <div>
                    <div class="bitPageSettingsCollumnA">Standaard max. breedte images:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Standaard max. breedte voor images tijdens uploaden onder bestandsbeheer. Deze waarde is bij het uploaden nog te overschrijven.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="text" data-field="MaxWidthImages" data-validation="number" />
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div>
                    <div class="bitPageSettingsCollumnA">Standaard max. breedte thumbnails:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Standaard max. breedte voor thumbnails tijdens uploaden onder bestandsbeheer. Deze waarde is bij het uploaden nog te overschrijven.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="text" data-field="MaxWidthThumbnails" data-validation="number" />
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
            </div>
            <!-- TAB6 -->
            <div id="tabPageDetails6" class="bitTabPage">
                <div>
                    <div class="bitPageSettingsCollumnA">Meta-Keywords:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="In dit veld kunt u meta zoekwoorden plaatsen. Deze woorden worden niet getoond aan een bezoeker, maar zoekmachines lezen deze woorden wel. Op deze manier vergroot u de kans dat een zoekmachine uw website passend indexeert.
                            Deze worden alleen in de pagina gezet als de meta zoekwoorden van de pagina leeg zijn gelaten. 
                            Staan er bij de pagina wel zoektermen, dan worden die van de pagina getoond. 
                            ">info</span>
                        <a href="javascript:;" class="bitTextareaEnlargeButton">enlarge </a>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <textarea data-field="MetaKeywords" class="bitTextArea"></textarea>
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div>
                    <div class="bitPageSettingsCollumnA">Meta-Description:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="In dit veld kunt u een korte beschrijving van uw site plaatsen. Sommige zoekmachines lezen deze omschrijving uit en tonen die aan de bezoeker. Op deze manier vergroot u de kans dat een zoekmachine uw website passend indexeert.
                            Deze worden alleen in de pagina gezet als de meta zoekwoorden van de pagina leeg zijn gelaten. 
                            Staan er bij de pagina wel zoektermen, dan worden die van de pagina getoond. ">info</span>
                        <a href="javascript:;" class="bitTextareaEnlargeButton">enlarge </a>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <textarea data-field="MetaDescription" class="bitTextArea"></textarea>
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div>
                    <div class="bitPageSettingsCollumnA">Sitemap.xml (instellingen per pagina):</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Een sitemap is een extra plattegrond voor zoekmachines. Per pagina kunt u opgeven of deze in de sitemap dient te verschijnen.">info</span>

                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <a href="/Sitemap.xml" target="_blank">Toon sitemap.xml</a>
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div>
                    <div class="bitPageSettingsCollumnA">Robots.txt (instellingen per pagina):</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="In robots.txt worden de pagina's opgenomen die een zoekmachine NIET dienen te indexeren. Per pagina kunt u opgeven of deze in robots.txt dient te verschijnen.">info</span>

                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <a href="/Robots.txt" target="_blank">Toon robots.txt</a>
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div>
                    <div class="bitPageSettingsCollumnA">Google analytics:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Met google analytics kunt u de frequentie van het bezoek aan uw site bijhouden. Indien deze aan staat wordt op iedere pagina een speciaal stukje javascript geplaatst.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="checkbox" data-field="UseGoogleAnalystics" />
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div>
                    <div class="bitPageSettingsCollumnA">Google analytics code:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Vul hier de code in van uw google analytics account. Deze wordt in het googleanalytics script op iedere pagina geplaatst.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="text" data-field="GoogleAnalysticsCode" />
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div>
                    <div class="bitPageSettingsCollumnA">Googlemaps key:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Voor het gebruik van een google maps module, vul hier de code in van uw google maps account.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="text" data-field="GoogleMapsKey" />
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
            </div>


            <!-- TAB7 -->
            <!-- TAB7 TALEN -->
            <!-- TAB7 -->
            <div id="tabPageDetails7" class="bitTabPage">
                <div id="languageWrapper" runat="server">
                    <div>
                    <div class="bitPageSettingsCollumnA">Talen</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Indien uw site meertalig is, dient u hier de talen toe te voegen. Er dient altijd 1 taal de standaard taal te zijn. Aan een template kunt u een taal koppelen. Eventuele data op de pagina's die deze template gebruiken worden alleen in de gekozen taal getoond.">
                        info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <div onclick="BITSITE.addLanguage()" class="button">Toevoegen</div> 
                    </div>
                </div>
                <br clear="all" />
                    <div id="bitMenu">
                        
                        
                    </div>
                    <table id="tableLanguages" class="bitGrid" style="width: 500px" data-control-type="table" cellspacing="0" cellpadding="2">
                        <thead>
                            <tr>
                                <td style="width: 125px">Standaard taal</td>
                                <td class="bitTableColumn" style="width: 25px"; >Code</td>
                                <td class="bitTableColumn" style="width: 150px" >Taal</td>

                                <td class="bitTableActionButton"></td>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>
                                    <input type="radio" name="defaultLanguage" /></td>
                                <td data-field="LanguageCode"></td>
                                <td data-field="Name"></td>

                                <td class="bitTableActionButton"><a data-child-field="ID" href="javascript:BITSITE.deleteLanguage('[data-field]', [list-index]);"
                                    class="bitDeleteButton">delete</a></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div style="text-align:right; border-top:solid 1px #ddd; padding: 5px">
                <button onclick="javascript:BITSITE.saveSiteConfig();">Opslaan</button>
                <button id="cancelButton" runat="server" onclick="javascript:location.href='/_bitplate/';">Annuleer</button>
            </div>
        
    </div>
    </div>
    <div style="display: none">
        <div id="dialoglanguages" title="Taal toevoegen.">
            <div id="language">
                <select name="language" id="languages" data-field="LanguageCode" data-text-field="Name">
                </select>
            </div>
        </div>

        <div id="environmentDialog" title="Nieuwe omgeving">
            <div class="bitTabs">
                <ul>
                    <li><a href="#tabEnvironmentDetails1">Algemeen</a></li>
                    <li><a href="#tabEnvironmentDetails2">SMTP</a></li>
                    <li><a href="#tabEnvironmentDetails3">Database</a></li>
                </ul>
                <div id="tabEnvironmentDetails1">
                    <div>
                        <div class="bitPageSettingsCollumnA">Naam:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="De naam van de omgeving.">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="Name" data-validation="required" class="required" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->

                    <div>
                        <div class="bitPageSettingsCollumnA">Standaard domein:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="Domain naam. U dient er eventueel zelf https:// voor te zetten. (Indien weggelaten wordt http:// er door het systeem voor gezet)">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="DomainName" data-validation="required" class="required" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                    <div>
                        <div class="bitPageSettingsCollumnA">Map:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="Locatie (pad) op de server.">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="Path" data-validation="required" class="required" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                    <div>
                        <div class="bitPageSettingsCollumnA">Publicatietype:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="Kies of het een omgeving is welke later is te bewerken of niet.">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <select data-field="SiteEnvironmentType" id="selectSiteEnvironmentType">
                                <option value="0" disabled="disabled">Editable</option>
                                <option value="1">PublishOnly</option>
                            </select>
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                </div>
                <div id="tabEnvironmentDetails2">
                    <div>
                        <div class="bitPageSettingsCollumnA">Server:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="Email host. Dit gegeven wordt gebruikt bij het aanmaken van de web.config tijdens publiceren van een omgeving anders dan de huidige. Bij de huidige omgeving is dit veld readonly. Wanneer u de smpt instelling van de huidige omgeving wilt wijzigen, dient u de web.config op de server aan te passen.">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="EmailSettingsHost" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->

                    <div>
                        <div class="bitPageSettingsCollumnA">Verzend adres:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="Verzend adres voor deze host. Dit gegeven wordt gebruikt bij het aanmaken van de web.config tijdens publiceren van een omgeving anders dan de huidige. Bij de huidige omgeving is dit veld readonly. Wanneer u de smpt instelling van de huidige omgeving wilt wijzigen, dient u de web.config op de server aan te passen.">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="EmailSettingsFrom" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                    <div>
                        <div class="bitPageSettingsCollumnA">Gebruiker:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="Gebruikersnaam voor dit email account. Dit gegeven wordt gebruikt bij het aanmaken van de web.config tijdens publiceren van een omgeving anders dan de huidige. Bij de huidige omgeving is dit veld readonly. Wanneer u de smpt instelling van de huidige omgeving wilt wijzigen, dient u de web.config op de server aan te passen.">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="EmailSettingsUser" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                    <div>
                        <div class="bitPageSettingsCollumnA">Wachtwoord:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="Wachtwoord voor dit email account. Dit gegeven wordt gebruikt bij het aanmaken van de web.config tijdens publiceren van een omgeving anders dan de huidige. Bij de huidige omgeving is dit veld readonly. Wanneer u de smpt instelling van de huidige omgeving wilt wijzigen, dient u de web.config op de server aan te passen.">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="EmailSettingsPassword" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                </div>
                <div id="tabEnvironmentDetails3">
                    <div>
                        <div class="bitPageSettingsCollumnA">Database naam:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="Vul de naam in van de database. Deze database dient wel reeds te bestaan. Bij de huidige omgeving is dit veld readonly. Wanneer u de database van de huidige omgeving wilt wijzigen, dient u de web.config op de server aan te passen.">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="DatabaseName" data-validation="required" class="required" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                </div>
            </div>
        </div>

        <!-- SCRIPT DIALOG -->
        <div id="bitScriptDialog" title="Scripts kiezen" style="display: none;max-height:600px; overflow-y:scroll; overflow-x:hidden">
            <ul id="bitScriptList" style="list-style-type: none" data-control-type="list">
                <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px;
                    padding: 5px"><a data-field="ID" data-title-field="CompleteName" href="javascript:BITSITE.addScript('[data-field]', '[data-title-field]');">
                        <span data-field="CompleteName"></span><span class="remove-script btn-remove-script">
                        </span></a></li>
            </ul>
        </div>
    </div>
</asp:Content>
