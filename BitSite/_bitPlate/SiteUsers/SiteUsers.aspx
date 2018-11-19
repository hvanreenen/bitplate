<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="SiteUsers.aspx.cs" Inherits="BitSite._bitPlate.SiteUsers.SiteUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="/_bitplate/_js/prototypes/dockable.js"></script>
    <script type="text/javascript" src="BITAUTHSITEUSERS.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITAUTHSITEUSERS.initialize();
            BITAUTHSITEUSERS.loadUserGroups();
            BITAUTHSITEUSERS.loadUsers();
            if (location.hash == "#showgroups") {
                BITAUTHSITEUSERS.showUserGroups();
            }
            else {
                BITAUTHSITEUSERS.showUsers();
            }
        });
    </script>
    <asp:Literal ID="DefaultPermissionSets" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        <li><a href="javascript:BITAUTHSITEUSERS.showUsers();" class="bitBoardUsers bitUsers">
            Users</a>
            <div>Gebruikers</div>
        </li>
        <li><a href="javascript:BITAUTHSITEUSERS.showUserGroups();" class="bitBoardAdmins bitUsergroups">
            USERGROUPS</a>
            <div>Groepen</div>
        </li>
        <li><a href="javascript:BITAUTHSITEUSERS.newUser();" class="bitNavBarButtonAddUser">adduser</a>
            <div>Nieuwe gebruiker</div>
        </li>
        <li><a href="javascript:BITAUTHSITEUSERS.newUserGroup();" class="bitNavBarButtonAddUserGroup">addpusergroup</a>
            <div>Nieuwe gebruikersgroep</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Gebruikers
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
   
    <table id="tableUsers" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 240px" data-sort-field="Name, ForeName">Naam</td>
                <td class="bitTableColumn" style="width: 240px">Lid van</td>
                <td class="bitTableColumn" style="width: 100px" data-sort-field="DateTill">Status
                </td>
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
                <td data-field="Field1"></td>
                <td data-field="Status"></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITAUTHSITEUSERS.openUserDetailsPopup('[data-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITAUTHSITEUSERS.removeUser('[data-field]');"
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


    <table id="tableUserGroups" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2" style="display: none">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 240px" data-sort-field="Name">Naam</td>
                <td class="bitTableColumn" style="width: 240px" data-sort-field="FK_Site">Soort</td>
                <td class="bitTableActionButton">Bewerk</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableActionButton">Kopieer</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon"><a href="#" class="bitTablePageIcon"></a></td>
                <td class="nameColumn" data-field="Name"></td>
                <td data-field="TypeString"></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITAUTHSITEUSERS.openUserGroupDetailsPopup('[data-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITAUTHSITEUSERS.removeUserGroup('[data-field]');"
                    class="bitDeleteButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" data-title-field="Name" href="javascript:BITAUTHSITEUSERS.copyUserGroup('[data-field]', '[data-title-field]');"
                    class="bitCopyButton"></a></td>
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
                <li><a href="#tabPageDetails3">Gebruikersgroepen</a></li>
                <!--<li><a href="#tabPageDetails4">Rechten</a></li> BUG #128-->
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
            <!--TAB3-->
            <!--TAB3-->
            <!--TAB3-->
            <div id="tabPageDetails3" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Groepen</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Koppel groepen. Via een groep krijgt de gebruiker rechten op een site">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="button" value="Toevoegen" onclick="javascript:BITAUTHSITEUSERS.openUserGroupsPopup();" />
                    <table id="tableUserGroupsPerUser">
                        <tbody>
                            <tr>
                                <td><a data-child-field="ID" title="verwijder" href="javascript:BITAUTHSITEUSERS.removeUserGroupPerUser('[data-field]', [list-index]);"
                                    class="bitDeleteButton">delete</a></td>
                                <td data-field="Site"></td>
                                <td data-field="Name"></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <br clear="all" />
            </div>
            <!--TAB4-->
            <!--TAB4: RECHTEN-->
            <!--TAB4-->
            <!--<div id="tabPageDetails4" class="bitTabPage">
                <asp:Literal ID="LiteralUserPermissions" runat="server"></asp:Literal>
            </div> BUG #128-->
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
                    <button onclick="javascript:BITAUTHSITEUSERS.sendNewPassword();">Nieuw wachtwoord versturen aan
                        gebruiker</button>
                </div>
                <br clear="all" />
            </div>
        </div>
    </div>

    <div id="bitUserGroupDetailsDialog" title="Gebruikersgroep">
        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageDetailsUserGroup1">Algemene gegevens</a></li>

            </ul>
            <!-- TAB1 -->
            <div id="tabPageDetailsUserGroup1" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Naam</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Name" type="text" data-validation="required" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">GroupType</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <select data-field="Type" data-validation="required">
                    <option value="30">AutorisedUsersGroup</option>
                    <option value="10">NewsletterSubscribersGroup</option>
                    <option value="20">WebshopCustomersGroup</option>
                    <option value="100">CustomGroup</option>
                </select>
                </div>
                <br clear="all" />
            </div>

            
        </div>
    </div>

    <!-- USERGROUPS PER USER DIALOG -->
    <div id="bitUserGroupPerUserDialog" title="Gebruikergroep kiezen" style="display: none;">
        <ul id="bitUserGroupList" style="list-style-type: none" data-control-type="list">
            <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px;
                padding: 5px"><a data-field="ID" data-title-field="Name" href="javascript:BITAUTHSITEUSERS.addUserGroupPerUser('[data-field]', '[data-title-field]');">
                    <input type="hidden" data-field="ID" />
                    <span data-field="Name"></span><span class="remove-script btn-remove-script">
                    </span></a></li>
        </ul>
    </div>
</asp:Content>
