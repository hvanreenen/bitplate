<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Resellers.aspx.cs" Inherits="BitSite._bitPlate.Autorisation.Resellers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="BITAUTHRESELLERS.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITAUTHRESELLERS.initialize();
            BITAUTHRESELLERS.loadResellers();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack" runat="server" id="BackLink">back</a>
            <div>Terug</div>
        </li>
        <li><a href="javascript:BITAUTHRESELLERS.newReseller();" class="bitNavBarButtonAddPage">ADDDEALER</a>
            <div>Nieuwe handelaar...</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Bitplate handelaren
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <table id="tableResellers" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 240px" data-sort-field="Name">Naam</td>
                <td class="bitTableActionButton">Bewerk</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableActionButton">Klanten</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID" >
                <td class="icon" style="width: 40px"><a href="#" class="bitTablePageIcon"></a></td>
                <td class="nameColumn" style="width: 240px" data-field="Name"></td>
                
                <td class="bitTableActionButton"><a data-field="ID" data-title-field="Type" href="javascript:BITAUTHRESELLERS.openResellerDetailsPopup('[data-field]', '[data-title-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITAUTHRESELLERS.removeReseller('[data-field]');"
                    class="bitDeleteButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" href="Companies.aspx#[data-field]"
                    class="bitLiveButton"></a></td>
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
    <div id="bitCompanyDetailsDialog" title="Relatie">
        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageDetails1">Algemene gegevens</a></li>
                <li><a href="#tabPageDetails2">Adres gegevens</a></li>
                <li><a href="#tabPageDetails3">Email instellingen</a></li>
            </ul>
            <!-- TAB1 -->
            <!-- TAB1 -->
            <!-- TAB1 -->
            <div id="tabPageDetails1" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Naam</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Name" type="text" data-validation="required" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Email</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Email" type="text" data-validation="required"/>
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Contactpersoon</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="ContactPerson" type="text" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Telefoon</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Telephone" type="text" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Website</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Website" type="text" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Logo</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <img data-field="Logo" /> (sla .png op in _img/ResellerLogos/ met zelfde naam als reseller-naam)
                </div>
                <br clear="all" />
            </div>
            <!--TAB2-->
            <!--TAB2-->
            <!--TAB2-->
            <div id="tabPageDetails2" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Adres</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Address" type="text" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Postcode</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Postalcode" type="text" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Plaats</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="City" type="text" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Land</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Country" type="text" />
                </div>
                <br clear="all" />
            </div>
            <!--TAB3-->
            <!--TAB3 Email instellingen-->
            <!--TAB3-->
            <div id="tabPageDetails3" class="bitTabPage">
                <em>Email instellingen voor verzenden van een email aan nieuwe bitplate-gebuikers en bij nieuwe wachtwoord aanvragen</em>
                <div class="bitPageSettingsCollumnA">Wachtwoord email afzender</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="PasswordEmailFrom" type="text" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Wachtwoord email onderwerp</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="PasswordEmailSubject" type="text" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Wachtwoord email template<br />
                    Gebruik tags:<br />
                    [name]<br />
                    [email]<br />
                    [password]<br />
                    [url]<br />
                </div>
                <div class="bitPageSettingsCollumnB">
                    <a href="javascript:;" class="bitTextareaEnlargeButton">enlarge </a>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <textarea data-field="PasswordEmailTemplate" class="bitTextArea"></textarea>
                </div>
                <br clear="all" />
            </div>
        </div>
    </div>

</asp:Content>
