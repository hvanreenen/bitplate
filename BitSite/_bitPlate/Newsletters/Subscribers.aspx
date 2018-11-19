<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Subscribers.aspx.cs" Inherits="BitSite._bitPlate.Newsletters.Subscribers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="/_bitplate/_js/prototypes/dockable.js"></script>
    <link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/chosen/chosen.css" />
    <link type="text/css" href="/_bitplate/_themes/bitplate/css/multiSelect2Boxes.css" rel="stylesheet" />
    <link type="text/css" href="/_bitplate/_themes/bitplate/css/newsletterpages.css" rel="stylesheet" />
    <script src="/_bitplate/_js/plugins/chosen/chosen.jquery.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITNEWSLETTERSUBSCRIBER.initialize();
        });
    </script>
    <script type="text/javascript" src="BITNEWSLETTERSUBSCRIBER.js"></script>

    <script type="text/javascript" src="/_js/jquery.iframe-post-form.js"></script>
    <asp:Literal ID="DefaultPermissionSets" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        <li id="liMailingGroups" runat="server"><a id="aMailingGroups" runat="server" href="MailingGroups.aspx"
            class="bitBoardAdmins">Groepen</a>
            <div>Groepen</div>
        </li>
        <li>
            <a href="javascript:void(0);" onclick="BITNEWSLETTERSUBSCRIBER.newSubscriber();" class="bitNavBarButtonAddUser">adduser</a>
            <div>Nieuwe abonnee...</div>
        </li>

        <li>
            <a href="javascript:void(0);" onclick="BITNEWSLETTERSUBSCRIBER.showImportSubscribers();" class="bitNavBarButtonAddUser">importUsers</a>
            <div>Importeer abonnees...</div>
        </li>



    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Nieuwsbrief abonnees
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">

    <table id="tableUsers" class="bitGrid" data-control-type="table" cellspacing="0" cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 200px" data-sort-field="Email">Subscriber</td>
                <td class="bitTableColumn" style="width: 200px" data-sort-field="Name">Naam</td>
                <td class="bitTableColumn" style="width: 200px">Lid van</td>
                <td class="bitTableColumn" style="width: 70px" data-sort-field="Confirmed">Bevestigd via opt-in</td>
                <td class="bitTableActionButton">Bewerk</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon"><a href="#" class="bitTablePageIcon"></a></td>
                <td class="nameColumn" data-field="Field1"></td>
                <td class="nameColumn" data-field="Field3"></td>
                <td class="nieuwsgroepen" data-field="Field2"></td>
                <td class="nameColumn" data-field="Status"></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:void(0);" onclick="BITNEWSLETTERSUBSCRIBER.editSubscriber('[data-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:void(0);" onclick="BITNEWSLETTERSUBSCRIBER.deleteSubscriber('[data-field]');"
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
    <%--
        <table id="tableImportDefinitions" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2" style="display: none" >
        <thead> 
            <tr>
                <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 250px" data-sort-field="Name">Template naam</td>
                <td class="bitTableColumn" style="width: 100px" data-sort-field="FileExtension">Type</td>
                <td class="bitTableColumn" style="width: 150px" data-sort-field="Delimiter">Scheidingsteken</td>
                <td class="bitTableActionButton">Bekijk</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableDateColumn" style="width: 130px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon"><a href="#" class="bitTablePageIcon"></a></td>
                <td class="nameColumn" data-field="Name"></td>
                <td class="fileExtension" data-field="Field1"></td>
                <td class="delimiter" data-field="Field2"></td>
                
                
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITNEWSLETTERSUBSCRIBER.openImportDefinitionDetails('[data-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:void(0)" onclick="BITNEWSLETTERSUBSCRIBER.deleteImportDefinition('[data-field]');"
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
    --%>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <div id="bitNewsletterSubscriberDialog" title="Nieuwsbrief abonnee" style="display: none;">
        <!--<div class="bitAccordion">
            <h3>Abonnee</h3>-->
        <div id="subscriberProfile" class="bitTabs">
            <ul>
                <li><a href="#generalSettingsTab">Algemene gegevens</a></li>
                <li><a href="#newsletterGroupsTab">Nieuwsgroepen</a></li>
            </ul>
            <div id="generalSettingsTab" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">E-mail adres</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voeg hier de bestandsnaam in van de pagina. De pagina krijgt zelf achtervoegsel .aspx">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Email" data-validation="email" type="text" id="NewsletterAbbonenementEmail" />
                </div>
                <br clear="all" />

                <div class="bitPageSettingsCollumnA">Bevestigd</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Confirmed" type="checkbox" />
                </div>
                <br clear="all" />

                <div class="bitPageSettingsCollumnA">Voornaam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voeg hier de bestandsnaam in van de pagina. De pagina krijgt zelf achtervoegsel .aspx">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="ForeName" type="text" />
                </div>
                <br clear="all" />

                <div class="bitPageSettingsCollumnA">Tussenvoegsel</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voeg hier de bestandsnaam in van de pagina. De pagina krijgt zelf achtervoegsel .aspx">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="NamePrefix" type="text" />
                </div>
                <br clear="all" />

                <div class="bitPageSettingsCollumnA">Achternaam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voeg hier de bestandsnaam in van de pagina. De pagina krijgt zelf achtervoegsel .aspx">info</span>
                </div>
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

                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Geregistreerd via</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <span data-field="RegistrationTypeString"></span>
                </div>
                <br clear="all" />

                <%--<div class="bitPageSettingsCollumnA">Koppel gebruikersprofiel</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Voeg hier de bestandsnaam in van de pagina. De pagina krijgt zelf achtervoegsel .aspx">
                            info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="checkbox" id="connectUserProfile" />
                    </div>
                    <br clear="all" />--%>
            </div>
            <div id="newsletterGroupsTab" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Groepen</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Koppel groepen. Via een groep krijgt de gebruiker rechten op een site">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="button" value="Toevoegen" onclick="BITNEWSLETTERSUBSCRIBER.openNewsGroupsPopup();" />
                    <table id="tableNewsGroupsPerUser">
                        <tbody>
                            <tr>
                                <td><a data-child-field="ID" title="verwijder" href="javascript:void(0);" onclick="BITNEWSLETTERSUBSCRIBER.removeNewsgroupPerUser([list-index]);"
                                    class="bitDeleteButton">delete</a></td>
                                <td data-field="Site"></td>
                                <td data-field="Name"></td>
                                <td class="notes"></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <br clear="all" />
            </div>
        </div>
        <%--<h3 id="userProfileAccordionHeader">Gekoppeld gebruikersprofiel</h3>
            <div id="userProfile" class="bitTabs">
                <div id="UserProfileAccordionTitle"></div>
                <ul>
                    <li><a href="#tabPageDetails1">Algemene gegevens</a></li>
                    <li><a href="#tabPageDetails2">Adresgegevens</a></li>
                    <li><a href="#tabPageDetails3">Gebruikersgroepen</a></li>
                    <!--<li><a href="#tabPageDetails4">Nieuwsgroupen</a></li>-->
                    <li><a href="#tabPageDetails5">Wachtwoord reset</a></li>
                </ul>
                <!-- TAB1 -->
                <div id="tabPageDetails1" class="bitTabPage">
                    <div class="bitPageSettingsCollumnA">Email</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input id="UserEmailAddress" data-field="User.Email" type="text" data-validation="required, email" />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Voornaam</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="User.ForeName" type="text" data-validation="required" />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Tussenvoegsels</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="User.NamePrefix" type="text"  />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Achternaam</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="User.Name" type="text" data-validation="required" />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Geslacht</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <div>
                            <input type="radio" name="Gender" data-field="User.Gender" value="1" />Man
                        </div>
                        <div>
                            <input type="radio" name="Gender" data-field="User.Gender" value="2" />Vrouw
                        </div>
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Geboortedatum</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="User.BirthDate" type="date" />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Status</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <div>
                            <input checked="checked" type="radio" name="status" data-field="User.Active" value="1" />Actief
                        </div>
                        <div>
                            <input type="radio" name="status" data-field="User.Active" value="2" />Actief vanaf:
                            <input type="date" data-field="DateFrom" />
                            tot:
                            <input type="date" data-field="DateTill" />
                        </div>
                        <div>
                            <input type="radio" name="status" data-field="User.Active" value="0" />Niet actief
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
                        <input data-field="User.Address" type="text" />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Postcode</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="User.Postalcode" type="text" />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Plaats</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="User.City" type="text" />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Land</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="User.Country" type="text" />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Telefoon</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="User.Telephone" type="text" />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA">Mobiel</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="User.MobilePhone" type="text" />
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
                        <input type="button" value="Toevoegen" onclick="BITNEWSLETTERSUBSCRIBER.openUserGroupsPopup();" />
                        <table id="tableUserGroupsPerUser">
                            <tbody>
                                <tr>
                                    <td><a data-child-field="ID" title="verwijder" href="javascript:BITNEWSLETTERSUBSCRIBER.removeUserGroupPerUser([list-index]);"
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
                </div>-->
                <!--TAB4-->
                <!--TAB4: WW VERSTUREN-->
                <!--TAB4-->
                <div id="tabPageDetails5" class="bitTabPage">
                    <div class="bitPageSettingsCollumnA">Laatste wijziging</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <span data-field="User.PasswordLastChanged" data-format="dd-MM-yyyy HH:mm:ss"></span>
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
            </div>--%>
        <%--</div>--%>
    </div>
    <%--
    <div id="bitUserGroupDetailsDialog" title="Gebruikersgroep" style="display: none;">
        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageDetailsUserGroup1">Algemene gegevens</a></li>
                <li><a href="#tabPageDetailsUserGroup2">Rechten</a></li>
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

            <!-- TAB2 -->
            <div id="tabPageDetailsUserGroup2" class="bitTabPage">
                <select id="selectDefaultPermissions" onchange="javascript:BITAUTHSITEUSERS.setDefaultPermissions();">
                    <option value="">*** Kies standaard rechten ***</option>
                    <option value="Moderator">Site moderator rechten</option>
                    <option value="Designer">Designer rechten</option>
                    <option value="Admin">Admin rechten</option>
                </select>
                <asp:Literal ID="LiteralUserGroupPermissions" runat="server"></asp:Literal>
            </div>
        </div>
    </div>

    <!-- USERGROUPS PER USER DIALOG -->
    <div id="bitUserGroupPerUserDialog" title="Gebruikergroep kiezen" style="display: none;">
        <ul id="bitUserGroupList" style="list-style-type: none" data-control-type="list">
            <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px;
                padding: 5px"><a data-field="ID" data-title-field="Name" href="javascript:void(0);" onclick="BITNEWSLETTERSUBSCRIBER.addUserGroupPerUser([list-index]);">
                    <input type="hidden" data-field="ID" />
                    <span data-field="Name"></span>@<span data-field="Site.Name"></span><span class="remove-script btn-remove-script">
                    </span></a></li>
        </ul>
    </div>
    --%>
    <!-- ADD NEWSGROUP -->
    <div id="bitNewsletterGroupDialog" title="Nieuwe nieuwsbrief groep." style="display: none;">
        <div class="bitPageSettingsCollumnA">groep naam</div>
        <div class="bitPageSettingsCollumnB"></div>
        <div class="bitPageSettingsCollumnC">
            <input type="text" data-field="Name" id="NewsletterGroupName" data-validation="required" />
        </div>
        <br clear="all" />

        <!-- <div class="bitPageSettingsCollumnA">Groep is online te kiezen:</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="De gebruiker kan bij het aanmelden kieze voor welke groep ze aanmelden.">
                info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <input id="IsChoosableGroupCheckBox" type="checkbox" data-field="IsChoosableGroup" />
        </div>
        <br clear="all" />-->

        <!--<div class="bitPageSettingsCollumnA">Abonnees altijd in deze groep plaatsen:</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="De groep verplicht toevoegen aan de abonnees">
                info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
          <input id="IsMandatoryGroupCheckBox" type="checkbox" data-field="IsMandatoryGroup" />
        </div>
        <br clear="all" />-->
    </div>

    <!-- NEWSGROUP PER USER DIALOG -->
    <div id="bitNewsGroupPerUserDialog" title="Gebruikergroep kiezen" style="display: none;">
        <ul id="bitNewsUserGroupList" style="list-style-type: none" data-control-type="list">
            <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px; padding: 5px"><a data-field="ID" data-title-field="Name" href="javascript:BITNEWSLETTERSUBSCRIBER.addNewsGroupPerUser([list-index]);">
                <input type="hidden" data-field="ID" />
                <span data-field="Name"></span><span data-field="Site.Name"></span><span class="remove-script btn-remove-script"></span></a></li>
        </ul>
    </div>

    <!-- import adressen -->
    <div id="bitImportSubscribersDialog" title="Abonnees importeren" style="display: none;">
        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageImportSubscribers1">1. Kies bestand</a></li>
                <li><a href="#tabPageImportSubscribers2">2. Koppel velden</a></li>
                <li><a href="#tabPageImportSubscribers3">3. Kies groep(en)     </a></li>
                <li><a href="#tabPageImportSubscribers4">4. Start import   </a></li>
                <li><a href="#tabPageImportSubscribers5">5. Logging         </a></li>
            </ul>
            <!-- TAB1 -->
            <div id="tabPageImportSubscribers1" class="bitTabPage">
                <form method="post" id="UPLOADFORM" action="/_bitAjaxServices/PostHandler.ashx?service=BitSite._bitPlate.Newsletters.NewsletterService&method=UploadSubscriberFile" enctype="multipart/form-data">



                    <div class="bitPageSettingsCollumnA">
                        Selecteer bestand
                        <br />
                        (.CSV <del>.XML .TXT</del>)
                    </div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="U dient te beginnen met het uploaden van een bestand met daarin de abonnees. Voorlopig zijn alleen .CSV bestanden mogelijk.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="file" name="subscriberFile" accept=".csv" data-validation="required" id="subscriberFile" />
                    </div>
                    <div class="bitPageSettingsCollumnA"></div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="button" value="Upload bestand" onclick="BITNEWSLETTERSUBSCRIBER.uploadFile();" />
                    </div>
                    <br clear="all" />
                    <div class="bitPageSettingsCollumnA"></div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC"><span id="uploadReadyMsg"></span></div>
                </form>
            </div>

            <!-- TAB2-->
            <div id="tabPageImportSubscribers2" class="bitTabPage">
                <div class="importsettingsSplash">
                    Upload eerst een bestand.
                </div>
                <div id="instellingenCSV">
                    <div class="bitPageSettingsCollumnA">
                        Laad definitie
                    </div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Laad een eerder opgeslagen importdefinitie. Instellingen uit deze definitie worden overgenomen.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <div id="importSelector">
                            <!-- data-field="id" weg laten-->
                            <select id="loadImportDefinitions" data-text-field="Name" name="importdefinitions" onchange="BITNEWSLETTERSUBSCRIBER.loadImportDefinition()">
                                <option value="">Er ging iets fout met het laden van deze gegevens</option>
                            </select>
                        </div>
                    </div>
                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA">Eerste rij bevat veldnamen</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="De eerste rij bevat de namen van de velden. Deze rij wordt zelf niet geimporteerd.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="FirstRowIsColumnName" type="checkbox" id="firstrowiscolomnname" onchange="BITNEWSLETTERSUBSCRIBER.firstRowisName()" />
                    </div>
                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA">Veld scheidingsteken</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Het teken waarmee de gegevens bin een rij worden gescheiden in de .CSV">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="delimitercsv" onchange="BITNEWSLETTERSUBSCRIBER.changeDelimiterCSV() ;" data-validation="required" data-field="Delimiter">
                            <option value="">Kies een scheidings teken</option>
                            <option value=";">;</option>
                            <option value=",">,</option>
                            <option value="#">#</option>
                        </select>
                    </div>
                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA">Voornaam kolom:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="selectForeNameColumn" data-field="ForeNameColumnNo"></select>
                    </div>
                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA">Tussenvoegsel kolom::</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="selectNamePrefixColumn" data-field="NamePrefixColumnNo"></select>
                    </div>
                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA">Achternaam kolom:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="selectNameColumn" data-field="NameColumnNo"></select>
                    </div>
                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA">Email kolom:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="selectEmailColumn" data-validation="required" data-field="EmailColumnNo"></select>
                    </div>
                    <br clear="all" />

                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA">
                        Voorbeeld: 
                        <div id="importVoorbeeld">
                            <table id="FileExample" border="0">
                                <tbody></tbody>
                            </table>
                        </div>
                        <br clear="all" />
                    </div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC"></div>

                    <br clear="all" />
                </div>

                <div id="instellingenXML">
                    <div class="bitPageSettingsCollumnA"></div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <span>instellingen pagina XML volgt nog</span>
                    </div>
                    <br clear="all" />
                </div>
            </div>

            <!-- TAB3 -->
            <!-- groep kiezen -->
            <div id="tabPageImportSubscribers3" class="bitTabPage">
                <div class="importsettingsSplash">
                    Upload eerst een bestand.
                </div>
                <div class="settingsTab">
                    <div class="bitPageSettingsCollumnA">Nieuwe groep</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Maak nieuwe nieuwsbrief groep aan. Bij het aanmaken wordt deze groep in de database opgeslagen.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="button" onclick="BITNEWSLETTERSUBSCRIBER.showAddNewGroup()" value="Nieuwe groep" />
                    </div>
                    <br clear="all" />


                    <div class="bitPageSettingsCollumnA">
                        Groep(en):
                    </div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Geimporteerde abonnees worden lid van de gekozen groepen. Abonnees worden zowiezo lid van de verplichte groepen.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">

                        <select id="importNewsgroupSelector" multiple="true" name="subscribedNewsgroups" runat="server" data-validation="required" data-field="Groups" style="height:30px"></select>
                    </div>

                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA">Gekozen groep(en) eerst legen</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="De abonees worden uit de gekozen groepen gehaald mits ze niet verplicht zijn.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <label>
                            <input data-field="EmptyGroups" type="checkbox" id="emptygroups" />Koppeling met abonnees verwijderen (geldt niet voor verplichte groepen)</label>
                    </div>
                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA">Abonnees verwijderen uit gekozen groep(en)</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="De abonnees in de gekozen groepen worden verwijderd, ook uit de verplichte groepen.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <label><input data-field="DeleteGroupSubscribers" type="checkbox" id="deletegroupsubscribers" />Abonnees definitief verwijderen (geldt alleen voor geimporteerde abonnees, geldt ook voor verplichte groepen indien hierboven gekozen)</label>
                    </div>
                    <br clear="all" />

                    

                    <div class="bitPageSettingsCollumnA">Bestaande email adressen overslaan</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Een abonnee met een email adres en achternaam die al bestaan worden overgeslagen. Anders wordt de abonnee overschreven.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input data-field="SkipDoubleRecords" type="checkbox" id="skipdoublerecords" />

                    </div>
                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA">Voor bestaande abonnees:</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        Voeg bestaande groepen samen met gekozen groepen<input data-field="AppendGroups" type="checkbox" id="overridegroups" />
                    </div>
                    <br clear="all" />
                    

                </div>
            </div>

            <!-- TAB4 -->
            <!-- importeer -->
            <div id="tabPageImportSubscribers4" class="bitTabPage">
                <div class="importsettingsSplash">
                    Upload eerst een bestand.
                </div>

                <div class="settingsTab">
                    <!-- SAVE IMPORT DEFINITIE-->
                    <div class="bitPageSettingsCollumnA">
                        Importdefinitie opslaan
                    </div>

                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Sla de gemaakte template op met een naam zodat deze later opnieuw kan worden gebruikt.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="checkbox" onchange="BITNEWSLETTERSUBSCRIBER.saveImportDefinition();" name="savedefinition" /> 
                        <div id="saveimportdefinition" style="display: none;">
                            <label>Met naam</label>
                            <input type="text" data-field="Name" data-validation="required" maxlength="50" id="definitionname" />
                        </div>
                    </div>
                    <br clear="all" />

                    <!-- anders wordt het automatisch gedaan-->
                    <div class="bitPageSettingsCollumnA">Abonnees automatisch bevestigen (opt-in)</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="De abonnees worden geregistreerd als bevestigd via opt-in. Indien deze optie uitstaat kunnen de bezoekers geen nieuwsbrief krijgen. Zij dienen dan hun account eerst te bevestigen via email. ">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        Ja<input type="radio" name="autoConfirm" value="true" data-field="AutoConfirm" />
                        Nee<input type="radio" name="autoConfirm" value="false" data-field="AutoConfirm" />
                    </div>
                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA"></div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">Controleer svp nogmaals of alle gegevens kloppen!</div>
                    <br clear="all" />

                    <div class="bitPageSettingsCollumnA">Importeer</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="button" value="Start import" onclick="BITNEWSLETTERSUBSCRIBER.ImportSubscribers()" />
                        <!-- knop om te importeren, als 't importeert een loader tekentje laten zien of progess-->

                        <!-- Een teller met de geimporteerde records, mislukte records / van totaal. 
                        Als 't klaar is automatisch naar de foutlogtabblad gaan met meer informatie-->
                    </div>
                    <br clear="all" />
                </div>
            </div>

            <div id="tabPageImportSubscribers5" class="bitTabPage">
                <div class="importsettingsSplash">
                    Upload en importeer eerst een bestand.
                </div>
                <div class="settingsTab">
                    <div class="bitPageSettingsCollumnA">
                        <div id="importProgress"></div>

                        <!--<input type="button" value="Download Log.txt" onclick="BITNEWSLETTERSUBSCRIBER.DownloadImportLog();" /><!-- ?Download log? Een txt met de algemene/import gegevens/progress en de foutieve records.-->
                        <br clear="all" />
                    </div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <table id="foutlog" style="overflow: auto;">
                            <tbody></tbody>
                        </table>
                    </div>
                    <br clear="all" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>
