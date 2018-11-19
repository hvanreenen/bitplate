<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Details.master"
    AutoEventWireup="true" CodeBehind="NewsletterConfig.aspx.cs" Inherits="BitSite._bitPlate.Site.NewsletterConfig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="BITNEWSLETTERCONFIG.js"></script>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/Plugins/ckeditor") %>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.button, button').button();
            CKEDITOR.basePath = '/_bitplate/_js/plugins/ckeditor/'; // SET ckeditor basepath.
            BITNEWSLETTERCONFIG.initialize();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack" runat="server" id="BackLink">back</a>
            <div>Terug</div>
        </li>
        <li id="liSaveSiteConfig" runat="server"><a id="aSaveSiteConfig" runat="server" href="javascript:BITNEWSLETTERCONFIG.saveNewsletterConfig();" class="bitNavBarButtonSavePage">Opslaan</a>
            <div>Opslaan</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Nieuwsbrief instellingen
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
                <li><a href="#tabPageDetails2">Opt-in</a></li>
                <li><a href="#tabPageDetails3">Opt-out</a></li>
            </ul>
            <!-- TAB1 -->
            <div id="tabPageDetails1" class="bitTabPage">
                <div>
                    <div class="bitPageSettingsCollumnA">Standaard verzend adres (email):</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Vul hier de standaard verzend adres. Deze is per nieuwsbrief eventueel te overschrijven.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="text" name="Name" data-field="NewsletterSender" data-validation="required, email" class="required" />
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div id="newsletterLicenseInfo">
                    <div>
                    <div class="bitPageSettingsCollumnA">Licentie voor:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Aantal verzendingen (elk emailadres telt als 1 verzending) waarvoor de licentie geldig is">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <span data-field="sentNewletterCount"></span>(elke verzonden email telt als 1)
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div>
                    <div class="bitPageSettingsCollumnA">Gebruikt:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Aantal verzendingen gedaan (elk emailadres telt als 1 verzending). Voor alle nieuwsbrieven tesamen.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <span data-field="sendNewletterCount"></span>
                    </div>
                </div>
                <br clear="all" />
                <div>
                    <div class="bitPageSettingsCollumnA">Over: </div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Licentie aantal - gebruikt">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <span data-field="UnusedSentTickets"></span>
                    </div>
                </div>
                <br clear="all" />
                </div>
                <%-- <div>
                    <div class="bitPageSettingsCollumnA">Huidige locatie op server:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <span data-field="CurrentWorkingEnvironment.Path"></span>
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                    --%>
                
            </div>
            <!-- TAB2 -->
            <div id="tabPageDetails2" class="bitTabPage">
                <fieldset>
                    <legend>Opt-in instellingen <span class="bitInfo" title="Nieuwe aanmeldingen (abonnees) die via de website binnen komen, dienen een bevestigingsemail te ontvangen waarmee zij moeten bevestigen dat hun aanmelding juist is. Hier kunt u de instellingen aanpassen voor die email. Dit is bij wet geregeld en dient om fictieve aanmeldingen te voorkomen waarbij mensen een nieuwsbrief zouden krijgen waar ze niet om hebben gevraagd.">info</span></legend>

                    <div>
                        <div class="bitPageSettingsCollumnA">Opt-in email onderwerp:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="Onderwerp van de opt-in email, zoals 'Welkom bij onze nieuwsbrief, bevestig svp uw inschrijving'">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="NewsletterOptInEmailSubject" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->

                    <div>
                        <div class="bitPageSettingsCollumnA">Opt-in pagina:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="Kies de pagina waar de opt-in module op staat.">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <select runat="server" id="selectOptInEmailPage" data-text-field="NewsletterOptInEmailPage.Name" data-field="NewsletterOptInEmailPage.ID"></select>
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                    <div>
                        <div class="bitPageSettingsCollumnA">Opt-in email:</div>
                    </div>
                    <br clear="all" />
                    <textarea data-field="NewsletterOptInEmailContent"></textarea>
                    <!-- NEXT ROW -->
                </fieldset>
            </div>
            <!-- TAB3 -->
            <!-- TAB3 Omgevingen -->
            <!-- TAB3 -->
            <div id="tabPageDetails3" class="bitTabPage">
                <fieldset>
                    <legend>Opt-out instellingen <span class="bitInfo" title="Hier de instellingen voor de email die gebruikers ontvangen na afmelding. In elke nieuwsbrief dient een link te worden opgenomen met daarin de mogelijkheid tot afmelding [UNSUBSCRIBEURL]">info</span></legend>

                    <div>
                        <div class="bitPageSettingsCollumnA">Opt-out email onderwerp:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="Email onderwerp ">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <input type="text" data-field="NewsletterOptOutEmailSubject" />
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->

                    <div>
                        <div class="bitPageSettingsCollumnA">Opt-out pagina:</div>
                        <div class="bitPageSettingsCollumnB">
                            <span class="bitInfo" title="De pagina die de afmelding regelt. De tag in de template, [UNSUBSCRIBEURL], wordt vervangen door url naar deze pagina ">info</span>
                        </div>
                        <div class="bitPageSettingsCollumnC">
                            <select runat="server" id="selectOptOutEmailPage" data-text-field="NewsletterOptOutEmailPage.Name" data-field="NewsletterOptOutEmailPage.ID"></select>
                        </div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                    <div>
                        <div class="bitPageSettingsCollumnA">Opt-out email:</div>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                    <textarea data-field="NewsletterOptOutEmailContent" ></textarea>
                </fieldset>
            </div>
        </div>
    </div>
    <div style="display: none">

    </div>
</asp:Content>
