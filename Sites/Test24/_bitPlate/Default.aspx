<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Dashboard.master"
    AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BitSite._bitPlate.Default" ViewStateMode="Disabled" %>

<asp:Content ID="Content1" ContentPlaceHolderID="SlideMenuPlaceHolder" runat="server">
    <div class="theBitMenu">
        <div class="bitLeftSlideMenu" id="bitLeftMenu01"><a href="#">Nieuws</a></div>
        <div class="bitLeftSlideMenuItems">
            <ul>
                <li><a href="#">Item1</a></li>
                <li><a href="#">Item2</a></li>
                <li><a href="#">Item3</a></li>
                <li><a href="#">Item4</a></li>
                <li><a href="#">Item5</a></li>
            </ul>
        </div>
        
    </div>
    <!--<div class="bitLeftSlideMenu" id="bitLeftMenu02"><a href="#">Menu2</a></div>-->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="divWrapperBoxes">
        <!-- bitBox01 -->
        <div class="bitBoxWrapper" id="bitBox01">
            <div class="bitBoxTitle"><%=this.Translator.Translate("Recentelijk aangepast") %></div>
            <div class="bitBoxMain" id="bitBoxMain01">
                <strong>Datacollecties:</strong>
                <br />
               <asp:Literal ID="TopDataCollectionList" runat="server">

                </asp:Literal>
                <br />
                <strong>Pagina's:</strong>
                <asp:Literal ID="TopPageList" runat="server">

                </asp:Literal>
            </div>
        </div>
        <!-- .bitBox01 -->
        <!-- bitBox02 -->
        <div class="bitBoxWrapper bitBox02" id="bitBox02" runat="server">
            <div class="bitBoxTitle"><%=this.Translator.Translate("Site content") %></div>
            <div class="bitBoxMain" id="bitBoxMain02">
                <!-- icons -->
                <ul>
                    <li runat="server" id="liPages"><a href="Pages/Pages.aspx" class="bitBoardPages" runat="server" id="aPages">Pages</a>
                        <div><%=this.Translator.Translate("Pagina's") %></div>
                        <div class="ui-bitTooltip hidden">Hier vindt u een overzicht van de verschillende pagina's. Nadat u hier bent aangekomen kunt elke individuele pagina bewerken</div>
                    </li>
                    <li runat="server" id="liFileManager"><a href="FileManager/ELFileManager.aspx" class="bitBoardFileManager" runat="server" id="aFileManager">filemanager</a>
                        <div><%=this.Translator.Translate("Bestandsbeheer") %></div>
                        <div class="ui-bitTooltip hidden">Beheer hier je bestanden: Uploaden Downloaden Bekijken - Afbeeldingen verkleinen/vergroten - hernoemen - kopiëren - verwijderen</div>
                    </li>
                    <li runat="server" id="liDataCollections"><a href="DataCollections/DataCollections.aspx" class="bitBoardModules " runat="server" id="aDataCollections">datacollecties</a>
                        <div><%=this.Translator.Translate("Datacollecties") %></div>
                        <div class="ui-bitTooltip hidden">Maak datacollectie's aan en pas de inhoud naar eigen wensen aan.</div>
                    </li>
                    <li runat="server" id="liPublish"><a href="Site/Publish.aspx" class="bitBoardPublish" runat="server" id="aPublish">publish</a>
                        <div><%=this.Translator.Translate("Publiceer") %></div>
                        <div class="ui-bitTooltip hidden">Publiceer hier de gewijzigde bestanden. Zodat ze online komen te staan</div>
                    </li>
                    <li runat="server" id="liMenuManager"><a href="Menus/Menus.aspx" class="bitBoardMenuManager" runat="server" id="aMenuManager">menumanager</a>
                        <div><%=this.Translator.Translate("Menu manager") %></div>
                        <div class="ui-bitTooltip hidden">Menu manager</div>
                    </li>
                </ul>
                <!-- .icons -->
            </div>
        </div>
        <!-- .bitBox02 -->
        <!-- bitBox03 -->
        <div class="bitBoxWrapper bitBox03" id="bitBox03" runat="server">
            <div class="bitBoxTitle"><%=this.Translator.Translate("Site beheer") %></div>
            <div class="bitBoxMain" id="bitBoxMain03">
                <!-- icons -->
                <ul class="bitBoxMain-ul02">
                    <li runat="server" id="liSiteConfig"><a href="Site/SiteConfig.aspx" class="bitBoardGeneral" runat="server" id="aSiteConfig">general</a>
                        <div><%=this.Translator.Translate("Algemeen") %></div>
                        <div class="ui-bitTooltip hidden">Pas hier de Algemene istellingen - Header gegevens - MetaDescription - Standaard Instellingen voor afbeeldingen - Zoekmachine instellingen - Emailinstellingen - Talen</div>
                    </li>
                    <li runat="server" id="liTemplates"><a href="Templates/Templates.aspx?type=page" class="bitBoardTemplates" runat="server" id="aTemplates">templates</a>
                        <div><%=this.Translator.Translate("Templates") %></div>
                        <div class="ui-bitTooltip hidden">Templates aanmaken en wijzigen</div>
                    </li>
                    <li runat="server" id="liBackups" class=""><a href="Backup/Backups.aspx" class="bitBoardBackups" runat="server" id="aBackups">backups</a>
                        <div><%=this.Translator.Translate("Backups") %></div>
                        <div class="ui-bitTooltip hidden">Een site backup maken</div>
                    </li>
                    <li runat="server" id="liStylesheets"><a href="Scripts/Scripts.aspx?type=css" class="bitBoardStylesheets" runat="server" id="aStylesheets">stylesheets</a>
                        <div><%=this.Translator.Translate("Stylesheets") %></div>
                        <div class="ui-bitTooltip hidden">Stylesheets aanmaken en wijzigen</div>
                    </li>
                    <li runat="server" id="liScripts"><a href="Scripts/Scripts.aspx?type=js" class="bitBoardScripts" runat="server" id="aScripts">scripts</a>
                        <div><%=this.Translator.Translate("Scripts") %></div>
                        <div class="ui-bitTooltip hidden">Scripts aanmaken en wijzigen</div>
                    </li>
                    <li runat="server" id="liEventlog"><a href="EventLog/EventLogList.aspx" class="bitBoardEventlog" runat="server" id="aEventlog">eventlog</a>
                        <div><%=this.Translator.Translate("Event log") %></div>
                        <div class="ui-bitTooltip hidden">Wat gebeurt er allemaal</div>
                    </li>
                </ul>
                <!-- .icons -->
            </div>
        </div>
        <!-- .bitBox03 -->
        <!-- bitBox04 -->
        <div class="bitBoxWrapper bitBox04" id="bitBox04" runat="server">
            <div class="bitBoxTitle"><%=this.Translator.Translate("Nieuwsbrief") %></div>
            <div class="bitBoxMain" id="bitBoxMain04">
                <!-- icons -->
                <ul class="bitBoxMain-ul03">
                    <!--class="bitItemDisabled"-->
                    <li runat="server" id="liNewsLetters" ><a runat="server" id="aNewsLetters" href="Newsletters/newsletters.aspx" class="bitBoardNewsLetters">newsletters</a>
                        <div><%=this.Translator.Translate("Nieuwsbrieven") %></div>
                        <div class="ui-bitTooltip hidden">Nieuwsbrieven template aanmaken - de nieuws brief maken - en vervolgens versturen</div>
                    </li>
                    <li runat="server" id="liNewsletterTemplates"><a runat="server" id="aNewsLetterTemplates"  href="Templates/Templates.aspx?type=newsletter" class="bitBoardTemplates">templates</a>
                        <div><%=this.Translator.Translate("Templates") %></div>
                        <div class="ui-bitTooltip hidden">Templates aanmaken en wijzigen</div>
                    </li>
                    <li runat="server" id="liNewsLetterSubscriptions" class=""><a runat="server" id="aNewsLetterSubscriptions" href="Newsletters/Subscribers.aspx" class="bitBoardSubscriptions">subscriptions</a>
                        <div><%=this.Translator.Translate("Abonnees") %></div>
                        <div class="ui-bitTooltip hidden">Beheer hier uw Abonnees voor uw nieuwsbrief</div>
                    </li>
                    <li runat="server" id="liNewsLetterSettings" class=""><a runat="server" id="aNewsLetterSettings" href="Newsletters/NewsletterConfig.aspx" class="bitBoardSettings">settings</a>
                        <div><%=this.Translator.Translate("Instellingen") %></div>
                        <div class="ui-bitTooltip hidden">Instellingen voor de uw nieuwsbrief</div>
                    </li>
                    <!--<li runat="server" id="liNewsLetterStats" class="bitItemDisabled"><a runat="server" id="aNewsLetterStats" href="#" class="bitBoardFolders">stats</a>
                        <div><%=this.Translator.Translate("Stats") %></div>
                        <div class="ui-bitTooltip hidden">Wat is er allemaal verzonden?</div>
                    </li>-->
                </ul>
                <!-- .icons -->
            </div>
        </div>
        <!-- .bitBox04 -->
        <!-- bitBox05 -->
        <div class="bitBoxWrapper bitBox05" id="bitBox05" runat="server">
            <div class="bitBoxTitle"><%=this.Translator.Translate("Serverbeheer") %></div>
            <div class="bitBoxMain" id="bitBoxMain05">
                <!-- icons -->
                <ul class="bitBoxMain-ul02">
                    <li runat="server" id="liServerSites"><a runat="server" id="aServerSites" href="Autorisation/Sites.aspx" class="bitBoardSites">sites</a>
                        <div><%=this.Translator.Translate("Sites") %></div>
                        <div class="ui-bitTooltip hidden">Beheer de verschillende sites die op de server worden gehost</div>
                    </li>
                    <li runat="server" id="liServerUsers"><a runat="server" id="aServerUsers" href="Autorisation/BitplateUsers.aspx" class="bitBoardAdmins">admins</a>
                        <div><%=this.Translator.Translate("Admins & Users") %></div>
                        <div class="ui-bitTooltip hidden">Maak gebruikers en beheerders aan voor de server</div>
                    </li>
                    <li runat="server" id="liServerBackups" class="bitItemDisabled"><a runat="server" id="aServerBackups" href="#" class="bitBoardBackups">backups</a>
                        <div><%=this.Translator.Translate("Backups") %></div>
                        <div class="ui-bitTooltip hidden">Backup de server</div>
                    </li>
                    <li runat="server" id="liServerEventlog" class="bitItemDisabled"><a runat="server" id="aServerEventlog" href="#" class="bitBoardEventlog">eventlog</a>
                        <div><%=this.Translator.Translate("Eventlog") %></div>
                        <div class="ui-bitTooltip hidden">Wat doet elke gebruiker?</div>
                    </li>
                    
                </ul>
                <!-- .icons -->
            </div>
        </div>
        <!-- .bitBox05 -->
        <!-- bitBox06 -->
        <div class="bitBoxWrapper bitBox06" id="bitBox06">
            <div class="bitBoxTitle">Uw BITplate dealer</div>
            <div class="bitBoxMain" id="bitBoxMain06">
                <div>
                    <img id="bitResellerLogo" runat="server" width="80" height="50"  />
                </div>
                <div id="bitResellerContactInfo" runat="server" class="div"></div>
            </div>
        </div>
        <!-- .bitBox06 -->
        <!-- bitBox07 -->
        <div class="bitBoxWrapper bitBox07" id="bitBox07" runat="server">
            <div class="bitBoxTitle"><%=this.Translator.Translate("Webshop") %></div>
            <div class="bitBoxMain" id="bitBoxMain07">
                <!-- icons -->
                <ul>
                    <li runat="server" id="liWebshopProducts" class="bitItemDisabled"><a runat="server" id="aWebshopProducts" href="#" class="bitBoardProducts">products</a>
                        <div><%=this.Translator.Translate("Producten") %></div>
                        <div class="ui-bitTooltip hidden">Beheer je producten voor de webshop</div>
                    </li>
                    <li runat="server" id="liWebshopOrders" class="bitItemDisabled"><a runat="server" id="aWebshopOrders" href="#" class="bitBoardOrders">orders</a>
                        <div><%=this.Translator.Translate("Bestellingen") %></div>
                        <div class="ui-bitTooltip hidden">Wat is er allemaal besteld</div>
                    </li>
                    <li runat="server" id="liWebshopUsers" class="bitItemDisabled"><a runat="server" id="aWebshopUsers" href="#" class="bitBoardAdmins">klanten</a>
                        <div><%=this.Translator.Translate("Klanten") %></div>
                        <div class="ui-bitTooltip hidden">Wie zijn uw klanten</div>
                    </li>
                    <li runat="server" id="liWebshopSettings" class="bitItemDisabled"><a runat="server" id="aWebshopSettings" href="#" class="bitBoardSettings">settings</a>
                        <div><%=this.Translator.Translate("Instellingen") %></div>
                        <div class="ui-bitTooltip hidden">Pas hier de instellingen van de webshop aan</div>
                    </li>
                </ul>
                <!-- .icons -->
            </div>
        </div>
        <!-- .bitBox07 -->
        <!-- bitBox08 -->
        <div class="bitBoxWrapper bitBox08" id="bitBox08" runat="server">
            <div class="bitBoxTitle">Klanten & gebruikers</div>
            <div class="bitBoxMain" id="bitBoxMain08">
                <!-- icons -->
                <ul>
                    <li runat="server" id="liWebUsers"><a runat="server" id="aWebUsers" href="SiteUsers/SiteUsers.aspx" class="bitBoardUsers">users</a>
                        <div><%=this.Translator.Translate("Gebruikers") %></div>
                        <div class="ui-bitTooltip hidden">Maak gebruikers aan</div>
                    </li>
                    <li runat="server" id="liWebUserGroups"><a runat="server" id="aWebUserGroups" href="SiteUsers/SiteUsers.aspx#showgroups" class="bitBoardAdmins">admins</a>
                        <div><%=this.Translator.Translate("Groepen") %></div>
                        <div class="ui-bitTooltip hidden">Groepen beheren/aanmaken - gebruikers toevoegen/verwijderen</div>
                    </li>
                    
                </ul>
                <!-- .icons -->
            </div>
        </div>
        <!-- .bitBox08 -->
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContentPlaceHolder" runat="server">
    <div style="display: none">
        <div id="noPermissionsDialog" runat="server" title="Geen rechten op deze site.">
            <div class="ui-widget">
	            <div class="ui-state-error ui-corner-all" style="padding: 0 .7em;">
		            <p><span class="ui-icon ui-icon-alert" style="float: left; margin-right: .3em;"></span>
		            <strong>Welkom op het dashboard van Bitplate.CMS</strong><br />
                    U krijgt dit scherm te zien omdat aan uw account geen rechten zijn verleend op deze website.</p>
	            </div>
            </div>
        </div>
    </div>
</asp:Content>