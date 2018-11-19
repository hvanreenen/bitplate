<%@ Page Title="" Language="C#" MasterPageFile="~/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="ServerUsers.aspx.cs" Inherits="BitMetaServer.Auth.ServerUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="/_js/prototypes/dockable.js"></script>
    <script type="text/javascript" src="BITSERVERUSERS.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITSERVERUSERS.initialize();
            BITSERVERUSERS.loadUsers();
        });
    </script>
    <asp:Literal ID="DefaultPermissionSets" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        
        
        <li><a href="javascript:BITSERVERUSERS.newUser();" class="bitNavBarButtonAddPage">adduser</a>
            <div>Nieuwe gebruiker...</div>
        </li>
        

    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Metaserver-gebruikers
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    
    <table id="tableUsers" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 592px" data-sort-field="Name">Naam</td>
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
                <td data-field="IsActiveString"></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITSERVERUSERS.openUserDetailsPopup('[data-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITSERVERUSERS.removeUser('[data-field]');"
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
                <!--<li><a href="#tabPageDetails4">Rechten</a></li>-->
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
            
            <!--TAB4-->
            <!--TAB4: RECHTEN-->
            <!--TAB4-->
            <div id="tabPageDetails4" class="bitTabPage">
                <asp:Literal ID="LiteralUserPermissions" runat="server"></asp:Literal>
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
                    <button onclick="javascript:BITSERVERUSERS.sendNewPassword();">
                        Nieuw wachtwoord versturen aan
                        gebruiker</button>
                </div>
                <br clear="all" />
            </div>
        </div>
    </div>

</asp:Content>
