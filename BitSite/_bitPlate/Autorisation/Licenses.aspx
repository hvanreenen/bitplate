<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Licenses.aspx.cs" Inherits="BitSite._bitPlate.Autorisation.Licences" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="/_bitplate/_js/prototypes/dockable.js"></script>
    <script type="text/javascript" src="BITAUTHLICENSES.js"></script>
    <script type="text/javascript" src="BITAUTHCOMPANIES.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITAUTHLICENSES.initialize();
            if (location.hash) {
                var companyid = location.hash.replace('#', '');
                BITAUTHLICENSES.loadLicenses(companyid);
                $("select.selectCompany").val(companyid);
            }
            else {
                BITAUTHLICENSES.loadLicenses();
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
        <li><a href="Companies.aspx?Referer=Licenses.aspx" class="bitNavBarButtonAddPage">
            ADDCOMPANY</a>
            <div>Klanten</div>
        </li>
        <li><a href="Resellers.aspx?Referer=Licenses.aspx" class="bitNavBarButtonAddPage">
            ADDCOMPANY</a>
            <div>Handelaren</div>
        </li>
        <li><a href="javascript:BITAUTHLICENSES.newLicense();" class="bitNavBarButtonAddPage">
            ADDCOMPANY</a>
            <div>Nieuwe licentie...</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Licenties
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <fieldset><legend>Filter</legend>
        <label>Kies klant:</label> <select id="selectCompanyFilter" class="selectCompany" onchange="BITAUTHLICENSES.filterOnCompanies(this);"></select><br />
    </fieldset>
    <table id="tableLicenses" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 140px" data-sort-field="Name">Licentie</td>
                <td class="resellerColumn" style="width: 140px" data-sort-field="FK_Company">
                    Eigenaar</td>
                <td class="resellerColumn" style="width: 100px" data-sort-field="IPAddress">
                    Server</td>
                <td class="resellerColumn" style="width: 140px" data-sort-field="DomainName">
                    Site</td>
                
                <td class="bitTableActionButton">Bewerk</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon" style="width: 40px"><a href="#" class="bitTablePageIcon"></a></td>
                <td class="nameColumn" style="width: 140px" data-field="Name"></td>
                <td class="companyColumn" data-field="Owner.Name"></td>
                <td class="nameColumn" data-field="IPAddress"></td>
                <td class="nameColumn" data-field="DomainName"></td>
                <td class="bitTableActionButton"><a data-field="ID" data-title-field="Type" href="javascript:BITAUTHLICENSES.openLicenseDetailsPopup('[data-field]', '[data-title-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITAUTHLICENSES.removeLicense('[data-field]');"
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
    <!-- in deze div wordt via ajax de dialog uit Companies.aspx geladen. LET OP div moet helemaal leeg zijn (ook geen spaties of enters) -->
    <div id="bitCompanyDetailsDialogWrapper" title="Klant"></div>
    
    <!--LICENSE DIALOG -->
    <div id="bitLicenseDetailsDialog" title="Licentie">
        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageDetails1">Algemene gegevens</a></li>
                <li><a href="#tabPageDetails2">Rechten</a></li>
                <li><a href="#tabPageDetails3">Statistieken</a></li>
            </ul>
            <!-- TAB1 -->
            <!-- TAB1 -->
            <!-- TAB1 -->
            <div id="tabPageDetails1" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Licentie</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectLicenseType" 
                        data-field="LicenseType" 
                        data-validation="required" onchange="javascript:BITAUTHLICENSES.setDefaults();">
                        <option value="1" >Bitplate Lite</option>
                        <option value="2">Bitplate Standard</option>
                        <option value="3">Bitplate Corporate</option>
                        <option value="4">Bitplate Enterprise</option>
                        <option value="9">Bitplate Custom</option>
                    </select>
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Nieuwsbrief licentie</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectNewsletterLicenseType"  data-field="NewsletterLicenseType" 
                        data-validation="required" onchange="javascript:BITAUTHLICENSES.setDefaults();">
                        <option value="0" >Geen</option>
                        <option value="11">Bitnewsletter Lite</option>
                        <option value="12">Bitnewsletter Corporate</option>
                        <option value="19">Bitnewsletter Custom</option>
                    </select>
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Webshop licentie</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectWebshopLicenseType" data-field="WebshopLicenseType" 
                        data-validation="required" onchange="javascript:BITAUTHLICENSES.setDefaults();">
                        <option value="0">Geen</option>
                        <option value="101">Bitshop Lite</option>
                        <option value="102">Bitshop Corporate</option>
                        <option value="103" >Bitshop Xtra</option>
                        <option value="109" >Bitshop Custom</option>
                    </select>
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Server computernaam</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="IPAddress" type="text" data-validation="required" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Meerdere sites</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input id="checkMultipleSites" data-field="MultipleSites" type="checkbox" onclick="javascript:BITAUTHLICENSES.changeMultipleSites(this);"/> <label for="checkMultipleSites">Met deze licentiecode meerdere sites mogen aanmaken</label><br />
                    Max. aantal sites: <input id="textMaxNumberOfSites" data-field="MaxNumberOfSites" type="text" class="textboxInteger" disabled="disabled"/> 
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Url(s) (en eventueel aliassen)</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input id="textUrl" data-field="DomainName" type="text" data-validation="required" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Licentiecode</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Code" type="text" data-validation="required" />
                </div>
                <br clear="all" />

                <div class="bitPageSettingsCollumnA">Eigenaar</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectCompany" data-field="Owner.ID" data-text-field="Owner.Name"
                        data-validation="required"></select>
                    <button onclick="javascript:BITAUTHLICENSES.newCompany();">Nieuwe klant...</button>
                </div>

                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Geldig vanaf</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="DateFrom" type="date" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Geldig tot</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="DateTill" type="date" />
                </div>
            </div>
            <!--TAB2-->
            <!--TAB2 Rechten-->
            <!--TAB2-->
            <div id="tabPageDetails2" class="bitTabPage">
                <div style="overflow-y:scroll; height:400px">
                <asp:Literal ID="LiteralPermissions" runat="server"></asp:Literal>
                    </div>
            </div>
            <!--TAB3-->
            <!--TAB3 Statistieken-->
            <!--TAB3-->
            <div id="tabPageDetails3" class="bitTabPage">
                
            </div>
        </div>
    </div>
    </div>
</asp:Content>
