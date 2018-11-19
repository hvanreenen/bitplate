<%@ Page Title="" Language="C#" MasterPageFile="~/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="MultiSiteUsers.aspx.cs" Inherits="BitMetaServer.Auth.MultiSiteUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="/_js/prototypes/dockable.js"></script>
    <script type="text/javascript" src="BITMULTISITEUSERS.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITMULTISITEUSERS.initialize();
            BITMULTISITEUSERS.loadUsers();
        });
    </script>
    <asp:Literal ID="DefaultPermissionSets" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>


        <li><a href="javascript:BITMULTISITEUSERS.newUser();" class="bitNavBarButtonAddPage">adduser</a>
            <div>Nieuwe gebruiker...</div>
        </li>


    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Multisites-gebruikers 
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <br />
    <br />
    <div>
        <i>Hieronder overzicht van Bitplate gebruikers die bij meer dan 1 site mogen inloggen. Beheer hier de koppeling aan de sites en de synchronisatie-gegevens. Gebruikersgroepen en rechten dienen per site te worden ingesteld.</i>
    </div>
    <table id="tableUsers" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 200px" data-sort-field="Name">Naam</td>
                <td class="bitTableColumn" style="width: 200px" data-sort-field="Email">Email</td>
                <td class="bitTableColumn" style="width: 100px">Status</td>
                <td class="bitTableActionButton">Bewerk</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon"><a href="#" class="bitTablePageIcon"></a></td>
                <td class="nameColumn" data-field="Name"></td>
                <td data-field="Email"></td>
                <td data-field="IsActiveString"></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITMULTISITEUSERS.openUserDetailsPopup('[data-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITMULTISITEUSERS.removeUser('[data-field]');"
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
    <div id="bitUserDetailsDialog" title="Gebruiker">
        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageDetails1">Algemene gegevens</a></li>
                <li><a href="#tabPageDetails2">Adresgegevens</a></li>
                <li><a href="#tabPageDetails4">Sites</a></li>
                <li><a href="#tabPageDetails5">Wachtwoord reset</a></li>
            </ul>
            <!-- TAB1 -->
            <div id="tabPageDetails1" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Email</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Email" type="text" data-validation="required" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Voornaam</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="ForeName" type="text" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Tussenvoegsels</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="NamePrefix" type="text" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Achternaam</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Name" type="text" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Geslacht</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <div>
                        <input type="radio" name="Gender" data-field="Gender" value="1" />Man
                    </div>
                    <div>
                        <input type="radio" name="Gender" data-field="Gender" value="2" />Vrouw
                    </div>
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Geboortedatum</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="BirthDate" type="date" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Status</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
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
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Administrator</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <label><input data-field="IsAdmin" type="checkbox" /> Gebruiker krijgt automatisch de administrators gebruikersgroep met alle rechten. </label>
                </div>


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
                <div class="bitPageSettingsCollumnA">Telefoon</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Telephone" type="text" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Mobiel</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="MobilePhone" type="text" />
                </div>
                <br clear="all" />
            </div>
            <div id="tabPageDetails4" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Sites</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Koppel sites. Hierdoor krijgt de gebruiker rechten op deze site. Rechten moeten bij de site zelf worden ingesteld.">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="button" value="Toevoegen" onclick="javascript: BITMULTISITEUSERS.openSitesPopup();" />
                    <table id="tableSitesPerUser">
                        <tbody>
                            <tr>
                                <td><a data-child-field="ID" title="verwijder" href="javascript:BITMULTISITEUSERS.removeSitePerUser('[data-field]', [list-index]);"
                                    class="bitDeleteButton">delete</a></td>
                                <td data-field="DomainName"></td>
                                <td data-field="Path"></td>
                            </tr>
                        </tbody>
                    </table>
                </div>


            </div>
            
            <!--TAB4-->
            <!--TAB4: WW VERSTUREN-->
            <!--TAB4-->
            <div id="tabPageDetails5" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Laatste wijziging</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <span data-field="PasswordLastChanged" data-format="dd-MM-yyyy HH:mm:ss"></span>
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA"></div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <button onclick="javascript:BITMULTISITEUSERS.sendNewPassword();">
                        Nieuw wachtwoord versturen aan
                        gebruiker</button>
                </div>
                <br clear="all" />
            </div>
        </div>
    </div>

    <!-- SITES PER USER DIALOG -->
    <div id="bitSitesPerUserDialog" title="Gebruikergroep kiezen" style="display: none;">
        <ul id="bitSitesList" style="list-style-type: none" data-control1-type="list">
            <asp:Literal runat="server" ID="literalSitesList"></asp:Literal>
        </ul>
    </div>

</asp:Content>
