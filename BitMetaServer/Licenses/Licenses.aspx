<%@ Page Title="" Language="C#" MasterPageFile="~/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Licenses.aspx.cs" Inherits="BitMetaServer.Licenses.Licenses" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="/_js/prototypes/dockable.js"></script>
    <script type="text/javascript" src="BITLICENSES.js"></script>
    <script type="text/javascript" src="BITCOMPANIES.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITLICENSES.initialize();
            if (location.hash) {
                var companyid = location.hash.replace('#', '');
                BITLICENSES.loadLicenses(companyid);
                $("select.selectCompany").val(companyid);
            }
            else {
                BITLICENSES.loadLicenses();
            }
        });
    </script>
    <asp:Literal ID="DefaultPermissionSets" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack" runat="server" id="BackLink">back</a>
            <div>Terug</div>
        </li>

        <li><a href="javascript:BITLICENSES.newLicense();" class="bitNavBarButtonAddPage">ADDLICENSE</a>
            <div>Nieuwe licentie...</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Licenties
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <fieldset>
        <legend>Filter</legend>
        <label>Kies klant:</label>
        <select id="selectCompanyFilter" class="selectCompany" onchange="BITLICENSES.filterOnCompanies(this);"></select><br />
    </fieldset>
    <table id="tableLicenses" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 240px" data-sort-field="Name">Licentie</td>
                <!--<td class="resellerColumn" style="width: 140px" data-sort-field="FK_Company">Eigenaar</td>-->
                <td class="resellerColumn" style="width: 100px" data-sort-field="ServerName">Server</td>
                <td class="resellerColumn" style="width: 352px" data-sort-field="DomainNames">Url(s)</td>

                <td class="bitTableActionButton">Bewerk</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon" style="width: 40px"><a href="#" class="bitTablePageIcon"></a></td>
                <td class="nameColumn bitTableColumnEllipsis" data-field="Name"></td>

                <td class="nameColumn" data-field="ServerName"></td>
                <td class="nameColumn bitTableColumnEllipsis" data-field="DomainNames"></td>
                <td class="bitTableActionButton"><a data-field="ID" data-title-field="Type" href="javascript:BITLICENSES.openLicenseDetailsPopup('[data-field]', '[data-title-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITLICENSES.removeLicense('[data-field]');"
                    class="bitDeleteButton"></a></td>

                <td class="bitTableDateColumn" data-field="CreateDate"></td>
            </tr>
        </tbody>
        <tfoot>
            <tr>
                <td colspan="8">
                    <div class="bitEndTable"></div>
                </td>
            </tr>
        </tfoot>
    </table>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <div style="display: none;">
        <!--COMPANY DIALOG -->
        <!-- in deze div wordt via ajax de dialog uit Companies.aspx geladen. 
            LET OP div moet helemaal leeg zijn (ook geen spaties of enters) -->
        <div id="bitCompanyDetailsDialogWrapper" title="Klant"></div>

        <!--LICENSE DIALOG -->
        <div id="bitLicenseDetailsDialog" title="Licentie">
            <div class="bitTabs">
                <ul>
                    <li><a href="#tabPageDetails1">Algemene gegevens</a></li>
                    <li><a href="#tabPageDetails2">CMS omgevingen</a></li>
                    <li><a href="#tabPageDetails3">Rechten</a></li>
                    <li><a href="#tabPageDetails4">Statistieken</a></li>

                </ul>
                <!-- TAB1 -->
                <!-- TAB1 -->
                <!-- TAB1 -->
                <div id="tabPageDetails1" class="bitTabPage">
                    <div class="bitPageSettingsCollumnA">Licentie</div>
                    <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Kies de licentiesoort. De licentiesoort verschijnt samen met de klantnaam in de naam van de licentie. Aan de hand van deze soort worden standaard rechten ingevuld. Deze zijn later aan te passen. Standaard rechten staan in .lic files op de server" ></span></div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="selectLicenseType"
                            data-field="LicenseType"
                            data-validation="required" onchange="javascript:BITLICENSES.setDefaults();">
                            <option value="1">Bitplate Lite</option>
                            <option value="2">Bitplate Standard</option>
                            <option value="3">Bitplate Corporate</option>
                            <option value="4">Bitplate Enterprise</option>
                            <option value="9">Bitplate Custom</option>
                        </select>
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Nieuwsbrief licentie</div>
                    <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Kies de nieuwsbrief-licentiesoort. De nieuwsbrieflicentiesoort verschijnt samen met de klantnaam en de licentiesoort in de naam van de licentie. Aan de hand van deze soort worden de standaard rechten en aantallen voor de nieuwsbrief ingevuld. Deze zijn later aan te passen. Standaard rechten staan in .lic files op de server" ></span></div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="selectNewsletterLicenseType" data-field="NewsletterLicenseType"
                            data-validation="required" onchange="javascript:BITLICENSES.setDefaults();">
                            <option value="0">Geen</option>
                            <option value="11">Bitnewsletter Lite</option>
                            <option value="12">Bitnewsletter Corporate</option>
                            <option value="19">Bitnewsletter Custom</option>
                        </select>
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Webshop licentie</div>
                    <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Heeft nog geen functie omdat er nog geen webshop is." ></span></div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="selectWebshopLicenseType" data-field="WebshopLicenseType"
                            data-validation="required" onchange="javascript:BITLICENSES.setDefaults();">
                            <option value="0">Geen</option>
                            <option value="101">Bitshop Lite</option>
                            <option value="102">Bitshop Corporate</option>
                            <option value="103">Bitshop Xtra</option>
                            <option value="109">Bitshop Custom</option>
                        </select>
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Server computernaam</div>
                    <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Naam van de server waarvoor deze licentie geldig is. Bitplate maakt er zelf hoofdletters van. Moet exact kloppende servernaam zijn en geen ip-adres." ></span></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="ServerName" type="text" data-validation="required" />
                    </div>
                    <br clear="all" />

                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Licentiecode</div>
                    <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Licentiecode. Dient uniek te zijn; aan de hand van deze code wordt licentie aangevraagd voor een site. Deze code dient bij een site te worden ingevuld bij algemene site gegevens." ></span></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="Code" type="text" data-validation="required" />
                    </div>
                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA">Eigenaar</div>
                    <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="De klant waarvoor de licentie geldt. De klant valt op zijn beurt weer onder een reseller." ></span></div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="selectCompany" data-field="Owner.ID" data-text-field="Owner.Name"
                            data-validation="required">
                        </select>
                        <button onclick="javascript:BITLICENSES.newCompany();">Nieuwe klant...</button>
                    </div>

                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Geldig vanaf</div>
                    <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Vanaf welke datum is de licentie geldig? Indien leeg gelaten, doet de vanaf-datum niets." ></span></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="DateFrom" type="date" />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Geldig tot</div>
                    <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Tot wanneer is de licentie geldig? Indien leeg gelaten, doet de tot-datum niets." ></span></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="DateTill" type="date" />
                    </div>
                </div>
                <!--TAB2-->
                <!--TAB2 Urls-->
                <!--TAB2-->
                <div id="tabPageDetails2" class="bitTabPage">
                    <!--
                    <div class="bitPageSettingsCollumnA">Meerdere CMS-omgevingen</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input id="checkMultipleSites" data-field="MultipleSites" type="checkbox" onclick="javascript: BITLICENSES.changeMultipleSites(this);" />
                        <label for="checkMultipleSites">Deze licentiecode is geschikt voor meerdere CMS-omgevingen (sites)</label><br />
                        Max. aantal sites met CMS:
                        <input id="textMaxNumberOfSites" data-field="MaxNumberOfSites" type="text" class="textboxInteger" disabled="disabled" />
                    </div>
                    -->
                    <div>
                        <button onclick="javascript:BITLICENSES.addLicensedEnvironment();" class="button">Toevoegen</button>
                    </div>
                    <table id="tableLicensedEnvironments" class="bitGrid" style="width: 970px" data-control-type="table" cellspacing="0" cellpadding="2">
                        <thead>
                            <tr>
                                <td class="bitTableColumn">Domein</td>
                                <td class="bitTableColumn">Pad</td>
                                <td class="bitTableActionButton"></td>
                                <td class="bitTableActionButton"></td>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td data-child-field="DomainName"></td>
                                <td data-child-field="Path"></td>

                                <td class="bitTableActionButton">
                                    <div data-child-field="ID" onclick="BITLICENSES.editLicensedEnvironment([list-index], '[data-field]');" class="bitConfigButton">Config</div>
                                </td>

                                <td class="bitTableActionButton">
                                    <div data-child-field="ID" onclick="BITLICENSES.deleteLicensedEnvironment([list-index], '[data-field]');"
                                        class="bitDeleteButton">
                                        delete
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <!--TAB3-->
                <!--TAB3 Rechten-->
                <!--TAB3-->
                <div id="tabPageDetails3" class="bitTabPage">
                    <div style="overflow-y: scroll; height: 400px">
                        <asp:Literal ID="LiteralPermissions" runat="server"></asp:Literal>
                    </div>
                </div>
                <!--TAB4-->
                <!--TAB4 Statistieken-->
                <!--TAB5-->
                <div id="tabPageDetails4" class="bitTabPage">
                    <div id="divStats"></div>
                </div>
                <!--
                <div id="tabPageDetails5" class="bitTabPage">
                    <div id="div2">MOET PER ENVIRONMENT WANT VERSLEUTELING IS AFHANKELIJK VAN URL EN PATH
                    Normaal gesproken wordt het bestand automatisch op de server waar de site draait gezet en hoef je hier niks mee te doen.<br />
                        Echter, in het geval van calamiteiten kan worden gewerkt met een .lic bestand.<br />
                        Dit bestand moet dezelfde naam hebben als de licentiecode en het achtervoegsel .lic. zet het bestand in de _lic\ map op de server.<br />
                        <strong>Sla licentie eerst op om laatste wijzigingen in bestand te krijgen</strong>
                        <div id="divFile"></div>
                        <a href=""></a>
                    </div>
                </div>
                    -->
            </div>
        </div>
    </div>




    <div id="LicensedEnvironmentDialog" title="Nieuwe url" style="display: none">

        <div>
            <div class="bitPageSettingsCollumnA">Domein</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Url van de site. Bitplate zet er zelf http:// voor als protocol ontbreekt. Voor aliassen moeten een extra omgeving worden opgegeven. Een site met of zonder www geldt hierbij niet als alias. Dus http://www.nu.nl geldt automatisch ook als http://nu.nl. ">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="DomainName" data-validation="required" class="required" />
            </div>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div>
            <div class="bitPageSettingsCollumnA">Map op server</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Locatie (map) op de server. Laat de servernaam zelf weg uit de locatie."></span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="Path" data-validation="required" class="required" />
            </div>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div>
            <div class="bitPageSettingsCollumnA">Versienummer</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Versienummer van de bitplate versie. Versienummer wordt automatisch aangepast na het doen van updates."></span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="Version"  />
            </div>
        </div>
        <br clear="all" />
        <br clear="all" />
        <!-- NEXT ROW -->
        <b>Onderstaande waardes dienen voor het ophalen van statistieken & uitvoeren van db-updates</b>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div>
            <div class="bitPageSettingsCollumnA">SiteID</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Optioneel: SiteID zoals in de database staat. Deze id is alleen nodig mochten er meer sites in dezelfde database staan en is nodig om statistics op te halen. "></span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="SiteID" />
            </div>
        </div>

        <br clear="all" />
        <!-- NEXT ROW -->
        <div>
            <div class="bitPageSettingsCollumnA">Database server</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Database server (ip adres)"></span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="DatabaseServer" />
            </div>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div>
            <div class="bitPageSettingsCollumnA">Database naam</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Database naam op server"></span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="DatabaseName" />
            </div>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div>
            <div class="bitPageSettingsCollumnA">Database user</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Gebruiker (hoeft geen database change rechten te hebben, want voor doen van updates wordt root-user gebruikt)"></span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="DatabaseUser" />
            </div>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div>
            <div class="bitPageSettingsCollumnA">Database password</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Wachtwoord dat hoort bij user"></span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="DatabasePassword" />
            </div>
        </div>
        <br clear="all" />
    </div>

    <div id="newSiteWizzard" title="Nieuwe site aanmaken" style="display: none">

        <div>
            <div class="bitPageSettingsCollumnA">Domein</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Url van de site.  Als u er zelf geen http:// voor zet, zal de software dat zelf doen.">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="DomainName" data-validation="required" class="required" />
            </div>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div>
            <div class="bitPageSettingsCollumnA">Map op server</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Als de map nog niet bestaat, zal die worden aangemaakt"></span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="Path" data-validation="required" class="required" />
            </div>
        </div>
        <br clear="all" />
        <br clear="all" />
        <br clear="all" />
        <!-- NEXT ROW -->
        <b>Onderstaande waardes invullen voor aanmaken van database</b>
        <br clear="all" />

        <!-- NEXT ROW -->
        <div>
            <div class="bitPageSettingsCollumnA">Database server</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Database server (ip adres)"></span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="DatabaseServer" data-validation="required" class="required" />
            </div>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div>
            <div class="bitPageSettingsCollumnA">Database naam</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Database met deze naam zal worden aangemaakt"></span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="DatabaseName" data-validation="required" class="required" />
            </div>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div>
            <div class="bitPageSettingsCollumnA">Database user</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="User wordt aangemaakt"></span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="DatabaseUser" data-validation="required" class="required" />
            </div>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div>
            <div class="bitPageSettingsCollumnA">Database password</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Wachtwoord dat hoort bij user"></span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="DatabasePassword" data-validation="required" class="required" />
            </div>
        </div>
        <br clear="all" />
        <br clear="all" />
        <br clear="all" />
        <b>Onderstaande SMPT instellingen dienen voor creeëren van web.config</b>
        <br clear="all" />

        
        <!-- NEXT ROW -->
        <div>
            <div>
                <div class="bitPageSettingsCollumnA">SMTP server</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Email host. Dit gegeven wordt gebruikt bij het aanmaken van de web.config.">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" data-field="EmailSettingsHost" />
                </div>
            </div>
            <br clear="all" />
            <!-- NEXT ROW -->

            <div>
                <div class="bitPageSettingsCollumnA">SMTP emailadres</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Verzend adres voor deze host. Dit gegeven wordt gebruikt bij het aanmaken van de web.config.">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" data-field="EmailSettingsFrom" />
                </div>
            </div>
            <br clear="all" />
            <!-- NEXT ROW -->
            <div>
                <div class="bitPageSettingsCollumnA">SMTP gebruiker</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Gebruikersnaam voor dit email account. Dit gegeven wordt gebruikt bij het aanmaken van de web.config.">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" data-field="EmailSettingsUser" />
                </div>
            </div>
            <br clear="all" />
            <!-- NEXT ROW -->
            <div>
                <div class="bitPageSettingsCollumnA">SMTP wachtwoord</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Wachtwoord voor dit email account. Dit gegeven wordt gebruikt bij het aanmaken van de web.config.">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" data-field="EmailSettingsPassword" />
                </div>
            </div>
            <br clear="all" />
        </div>
    </div>
</asp:Content>
