<%@ Page Title="Nieuwsbrieven" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Newsletters.aspx.cs" Inherits="BitSite._bitPlate.Newsletters.Newsletters" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="/_bitplate/_themes/bitplate/plugins/jbreadcrump/Styles/BreadCrumb.css" />
    <link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/chosen/chosen.css" />
    <link type="text/css" href="/_bitplate/_themes/bitplate/css/multiSelect2Boxes.css" rel="stylesheet" />
    <link type="text/css" href="/_bitplate/_themes/bitplate/css/newsletterpages.css" rel="stylesheet" />
    <!--<link rel="stylesheet" type="text/css" href="_js/plugins/jbreadcrump/Styles/Base.css" />-->
    
    <script type="text/javascript" src="BITNEWSLETTER.js"></script>
    <script type="text/javascript" src="/_bitplate/_js/prototypes/checkFunctionalityElements.js"></script>
    <script src="/_bitplate/_js/plugins/jbreadcrump/js/jquery.easing.1.3.js" type="text/javascript"></script>
    <script src="/_bitplate/_js/plugins/jbreadcrump/js/jquery.jBreadCrumb.1.1.js" type="text/javascript"></script>

    <script src="/_bitplate/_js/plugins/chosen/chosen.jquery.min.js" type="text/javascript"></script>
    
    <style>
        #standardTabPageMailing > div {
            display: inline-block;
        }
    </style>

    <style>
  .ui-progressbar {
    position: relative;
  }
  .progress-label {
    position: absolute;
    left: 50%;
    top: 4px;
    color: #FFF;
    font-weight: bold;
    text-shadow: 1px 1px 0 #000;
  }
  </style>
    <script type="text/javascript">
        $(document).ready(function () {
            //CKEDITOR.basePath = '/_bitplate/_js/plugins/ckeditor/'; // SET ckeditor basepath.
            BITNEWSLETTER.SentFromAddress = '<%= BitSite.SessionObject.CurrentSite.NewsletterSender %>';
            BITNEWSLETTER.initialize();
            /* BITPAGES.initialize();
            BITPAGES.loadPages("");
            BITPAGES.fillDropDownFolders();
            BITPAGES.loadTemplates();
            BITPAGES.loadScripts(); */

        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        <li id="liAddFolder" runat="server"><a id="aAddFolder" runat="server" href="javascript:BITNEWSLETTER.showAddNewsletter();"
            class="bitNavBarButtonAddPage">Nieuwe nieuwsbrief</a>
            <div>Nieuwsbrief</div>
        </li>
        <li id="liMailingGroups" runat="server"><a id="aMailingGroups" runat="server" href="MailingGroups.aspx"
            class="bitNavBarButtonAddPage">Distributiegroepen</a>
            <div>Distributiegroepen</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Nieuwsbrieven
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
     <table data-functionality="newsletterList" id="tableNewsletters" class="bitGrid" data-control-type="table" cellspacing="0" cellpadding="2" style="display: none">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 25px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 30px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 140px" data-sort-field="Name">Naam</td>
                <td class="bitTableColumn" style="width: 140px">Gekoppelde groep(en)</td>
                <td class="bitTableActionButton">Bewerken</td>
                <td class="bitTableActionButton">Config</td>
                <td class="bitTableActionButton">Verstuur</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum</td>
                
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon">
                    <div data-field="listIndex" class="bitTablePageIcon"></div>
                </td>
                <td class="iconAuth" ><a href="#" class="bitTableAuthIcon" style="display:none"></a></td>
                <td data-field="Name"  class="bitTableColumnName bitTableColumnEllipsis">
                </td>
                
                <td class="bitTableColumn" data-field="Field1"></td>
                <td class="bitTableActionButton" ><a data-field="ID" href="/_bitplate/EditPage/EditPage.aspx?newsletterid=[data-field]"
                    class="bitEditButton"><div class="ui-bitTooltip hidden">Pas hier de naam - titel aan; selecteer welke template de pagina gebruikt - Maak de pagina actief - welke scripts er op de pagina worden gebruikt - Zoekmachine instellingen</div></a>
                </td>
                <td class="bitTableActionButton" ><div data-field="ID" onclick="BITNEWSLETTER.editNewsletter('[data-field]')"
                    class="bitConfigButton"><div class="ui-bitTooltip hidden">Pas hier de naam - titel aan; selecteer welke template de pagina gebruikt - Maak de pagina actief - welke scripts er op de pagina worden gebruikt - Zoekmachine instellingen</div></div>
                    
                </td>
                <td data-functionality="newsletterList" class="bitTableActionButton"><div 
                    data-field="ID" data-title-field="Url" 
                    onclick="BITNEWSLETTER.sendNewsletter('[data-field]');"
                    class="bitSelectButton"></div></td>
                <td class="bitTableActionButton" ><div 
                    data-field="ID" data-title-field="Type" data-text-field="Name"
                    onclick="BITNEWSLETTER.deleteNewsletter([list-index]);"
                    class="bitDeleteButton"></div></td>
                <td class="bitTableDateColumn" data-field="CreateDate"></td>
            </tr>
        </tbody>
    </table>

    <div class="bitEndTable"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <div id="bitNewsletterDialog" title="Nieuwe nieuwsletter" style="display: none;">
        <div class="bitTabs">
            <ul>
                <li id="tabPage1"><a href="#standardTabPageGeneral">Algemeen</a></li>
                <li id="tabPage2"><a href="#standardTabPageAdditional">Additioneel </a></li>
                <!--<li id="tabPage2"><a href="#standardTabPageMailing">Mailing</a></li>-->
                <li id="tabPage3"><a href="#standardTabPageSentLog">Verzendlog</a></li>
                <li id="tabPage4"><a href="#standardTabPageStatics">Statistieken</a></li>
            </ul>
            <!-- TAB1 -->
            <!-- TAB1 -->
            <!-- TAB1 -->
            <div id="standardTabPageGeneral" style="display: none">
                <div class="bitPageSettingsCollumnA">Distributiegroep(en)</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Koppel hier de distributiegroep(en). Een groep bevat de abonnees (emailadressen) aan wie u de nieuwsbrief gaat versturen. Bij het versturen worden de gekoppelde groepen standaard getoond, vlak voor het verzenden zijn deze nog aan te passen. ">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select multiple="multiple" id="bitNewsletterGroupSelect" data-field="Groups" data-text-field="Name" data-validation="required"></select>
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Verzend emailadres</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Het afzender emailadres van de nieuwsbrief. Gebruik eventueel de email notatie om uw naam toe te voegen. Voorbeeld: Voornaam Achternaam <voornaamachternaam@bitplate.com>">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" data-field="SentFromAddress" data-validation="required" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Naam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="De naam van de nieuwsbrief.">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" data-field="Name" data-validation="required" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Email onderwerp</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Het email onderwerp">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" data-field="Subject" data-validation="required" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Layout template</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <div class="imgContainer">
                        <img id="imgTemplateScreenShot" src="null" data-field="Template.Screenshot" alt="Template.Name"
                            title="Template.Screenshot" data-validation="required" /><br />
                        <span id="spanTemplateName" data-field="Template.Name"></span>
                    </div>
                    <button type="button" onclick="javascript:BITNEWSLETTER.showTemplateSelector();">
                        wijzig
                    </button>
                    <br />

                </div>
                <br clear="all" />
            </div>
            <div id="standardTabPageAdditional">
                <div class="bitPageSettingsCollumnA">Verzend datum</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="datetime" data-field="SendDate"  /><!--data-validation="required" -->
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Verstuur direct aan nieuw geregistreerde gebruiker</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Een nieuwe aanmelding ontvangt na aanmelding en opt-in meteen deze nieuwbrief">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="checkbox" data-field="SendNewsletterAfterRegistration" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Uiterste verloopdatum voor direct versturen</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Verloop datum voor directe ontvangst na nieuwe aanmeldingen">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="datetime" data-field="ExpirationDate" />
                </div>
                <br clear="all" />
            </div>
            <!--<div id="standardTabPageMailing">
                <div id="bitAccordion" class=".bitAccordion">
                    <h3>Nieuwsgroep</h3>
                    <div>
                        
                    </div>
                    <h3>Adressen bestand</h3>
                    <div>

                    </div>
                </div>
                <div></div>
            </div>-->
            <div id="standardTabPageSentLog">
                <div class="bitPageSettingsCollumnA">Totaal verstuurd (aantal emailadressen):</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <div id="newsletterSentAmount"></div>
                </div>
                <br clear="all" />
                <hr />
                <table id="tableSentLog">
                    <thead>
                        <tr>
                            <td>Datum</td>
                            <td>Email</td>
                            <td>Status</td>
                            <td>Hits</td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td data-field="CreateDate"></td>
                            <td data-field="EmailAddress"></td>
                            <td data-field="NewsletterSent"></td>
                            <td>-</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div id="standardTabPageStatics">
                <table id="tableStatics">
                    <thead>
                        <tr>
                            <td>Datum</td>
                            <td>Email</td>
                            <td>Hyperlink</td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td data-field="CreateDate"></td>
                            <td data-field="UserEmail"></td>
                            <td><a data-field="Url" data-text-field="Url" target="_blank" href="[data-field]"></a></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>

    <!-- TEMPLATE CHOOSER DIALOG -->
    <div id="bitTemplateDialog" title="Template kiezen" style="display: none;">
        <div id="divChooseTemplate" style="overflow-x: scroll; width: 600px"
            data-control-type="list">
            <div onclick="javascript:BITNEWSLETTER.selectTemplate(this);" style="width: 150px; border: 1px solid #ddd;
                float: left">
                <input type="hidden" class="hiddenListIndex" data-field="listIndex" />
                <input type="hidden" class='hiddenID' data-field="ID" />
                <input type="hidden" class='hiddenScripts' data-field="Scripts" />
                <img src="null" data-field="Screenshot" /><br />
                <span data-field="Name"></span> <strong data-field="LanguageCode"></strong>
            </div>
            <asp:Literal ID="LiteralLayoutTemplates" runat="server"></asp:Literal>
        </div>
    </div>

    <div id="bitNewsletterSubscriberDialog" title="Nieuw abbonenement" style="display: none;">
        <div class="bitPageSettingsCollumnA">E-mail adres</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="Voeg hier de bestandsnaam in van de pagina. De pagina krijgt zelf achtervoegsel .aspx">
                info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <input data-field="EmailAdres" data-validation="email" type="text" id="NewsletterAbbonenementEmail" />
        </div>
        <br clear="all" />
    </div>

    <div id="bitSendNewsletterDialog" title="Verstuur nieuwsbrief" style="display: none;">
        <div class="bitPageSettingsCollumnA">Verstuur nieuwsbrief aan de volgende groepen:</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="Selecteer de nieuwsgroepen waarnaar deze nieuwsbrief verzonden moet worden.">
                info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <select id="bitNewsletterGroupSendSelect" multiple="multiple" data-field="Groups" data-text-field="Name" data-validation="required"></select>
        </div>
        <br clear="all" />
        <div class="bitPageSettingsCollumnA">Versturen alleen naar nieuwe abonnees.</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="Versturen alleen naar de abonnees (binnen de gekozen groep(en)) die deze nieuwsbrief nog niet eerder hebben ontvangen.">
                info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <input type="checkbox" data-field="sendToNewSubscribers" id="checkboxSendToNewSubscribers" />
        </div>
        <br clear="all" />
    </div>

    <div id="bitMailBot" title="Bezig met verzenden van nieuwsbrief.">
        <div id="progressbar"><div class="progress-label">Loading...</div></div>
    </div>
</asp:Content>
