<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditPageMenu2.ascx.cs" Inherits="BitSite._bitPlate.EditPage.EditPageMenu2" EnableViewState="false" %>

<%@ Register Src="ModuleConfig/GeneralConfigTab.ascx" TagName="GeneralConfigTab" TagPrefix="uc1" %>

<%@ Register Src="ModuleConfig/NavigationActionConfigTab.ascx" TagName="NavigationActionConfigTab" TagPrefix="uc2" %>

<%@ Register Src="ModuleConfig/DataConfigTab.ascx" TagName="DataConfigTab" TagPrefix="uc3" %>




<div id="bitEditPageMenusWrapper">
    <script type="text/javascript">

        CKEDITOR.basePath = '/_bitplate/_js/plugins/ckeditor/'; // SET ckeditor basepath.
    </script>
    <div id="bitAjaxLoaderContainer">
        <div>Wachten op server.</div>
        <div id="ajaxloaderImage"></div>
    </div>

    <!-- TOPMENU -->
    <div id="divBitplateTopMenu">
        <!-- wrap main menu -->

        <div id="divWrapperMainMenu">
            <ul class="bitUlMenu">
                <li><a href="/_bitplate/Default.aspx">Dashboard</a></li>
                <li runat="server" id="liPages"><a href="/_bitPlate/Pages/Pages.aspx">Pagina's</a></li>
                <li runat="server" id="liDataCollections"><a href="/_bitPlate/DataCollections/DataCollections.aspx">Datacollecties</a></li>
                <li runat="server" id="liTemplates"><a href="/_bitPlate/Templates/Templates.aspx?type=page">Templates</a></li>
                <!--<li><a href="#">Contact</a></li>-->
                <!--<li><a href="#">FAQ</a></li>-->
            </ul>
            <ul class="bitUlMenu bitUlMenuRight">
                <li><a href="#" id="bitHelp" title="Klik en ga dan met je muis over de verschillende items die je op de pagina ziet, dan krijg je uitleg over elke sectie">?</a></li>
                <li><a href="#">Ingelogd als: 
                        <asp:Literal ID="ltrlLoggedInAs" runat="server"></asp:Literal></a></li>
                <li><a href="javascript:BITAUTH.userSettingsDialog();">Eigen gegevens</a></li>
                <li>
                    <a href="/_bitPlate/Login.aspx">Uitloggen</a>
                    <asp:LinkButton ID="lbtnUitloggen" runat="server" Visible="false" OnClick="lbtnUitloggen_Click">Uitloggen</asp:LinkButton></li>
            </ul>
        </div>
        <!-- .wrap main menu -->
    </div>
    <!-- .topmenu -->
    <!-- banner -->
    <div id="bitBanner">
        <div id="divBannerWrapper">
            <div id="bitBannerLogo"></div>
        </div>
    </div>

    <div style="display: none">
        <div id="bitMasterInputDialog">
            <!--<p><span id="bitMasterInputBoxIcon" class="ui-icon ui-icon-alert" style="float: left; margin-right: .3em;"></span></p>-->
            <div id="bitMasterInputMessage"></div>
            <div>
                <input id="bitMasterInput" type="text" />
            </div>
        </div>

        <div id="bitMasterMsgDialog">
            <!--<p><span id="Span1" class="ui-icon ui-icon-alert" style="float: left; margin-right: .3em;"></span>-->
            <p>
                <span id="bitMasterMsgBoxIcon" class="bitMessageBoxQuestion" style="float: left; margin-right: .3em;"></span>

                <span id="bitMasterMessage" style="float: left; margin-top: .3em;"></span>
            </p>
        </div>

        <div id="bitUserSettingsDialog" title="Gebruiker"></div>
    </div>

    <ul class="bitModuleContextMenu" style="display: none; width: 150px; z-index: 500">
        <li><a href="javascript:BITEDITPAGE.openEditModulePopup('{0}');"><span class="ui-icon ui-icon-pencil"></span>Bewerken</a></li>
        <li><a href="javascript:BITEDITPAGE.configModule('{0}', '{2}');"><span class="ui-icon ui-icon-gear"></span>Configureren</a></li>
        <!--<li><a href="javascript:BITEDITPAGE.configModuleByModule('{0}', '{2}');"><span class="ui-icon ui-icon-gear"></span>ModuleConfigurerenByControl</a></li>-->
        <li><a href="javascript:BITEDITPAGE.moveModule('{0}');"><span class="ui-icon ui-icon-arrow-4"></span>Verplaatsen</a></li>
        <li><a href="javascript:BITEDITPAGE.deleteModule('{0}');"><span class="ui-icon ui-icon-trash"></span>Verwijderen</a></li>
    </ul>

    <div id="moduleInformationDiv"></div>

    <div id="bitSideBar">

        <!-- bitPlateDealer -->
        <div class="bitSideBarLock"></div>
        <ul id="bitplateULDealer">
            <li class="bitLiNoMenu">
                <span>
                    <img src="/_bitplate/_themes/bitplate/images/BPCMS_sidebar_icon.png" id="imgBitPlateDealer" alt="abcnet" /></span><span class="bitSideBarText">Abcnet</span>
            </li>
        </ul>
        <hr />

        <!-- pages -->
        <div class="bitSidebarGroup"><span class="bitSideBarText">Paginas</span></div>
        <ul>
            <li id="bitMenuPreviewPage" class="bitLiNoMenu"><a id="linkPreviewPage" href="javascript:BITEDITPAGE.previewPage();">
                <span id="bitSideBarIconPreviewPage"></span><span class="bitSideBarText">Preview</span></a>
            <li id="bitMenuConfigPage" class="bitLiNoMenu"><a href="javascript:BITEDITPAGE.configPage();" runat="server" id="linkConfigPage">
                <span id="bitSideBarIconConfigPage"></span><span class="bitSideBarText">Eigenschappen</span></a>
            <li id="bitMenuEditPage" runat="server"><a href="#"><span id="bitSideBarIconPages"></span><span class="bitSideBarText">Pagina's</span></a>
                <ul id="ulMenuPages" runat="server">
                    <!--<div >
                        <li><a href="javascript:BITPAGES.newPage();">Nieuwe pagina...</a></li>
                    </div>-->
                </ul>
            </li>

        </ul>
        <hr />
        <!-- .pages -->
        <!-- MODULES -->
        <div class="bitSidebarGroup"><span class="bitSideBarText">Modules</span></div>
        <ul id="bitMenuModules">
            <li><a href="#"><span id="bitSideBarIconGeneral"></span><span class="bitSideBarText">Algemeen</span></a>
                <ul id="ulBitModulesGeneral" runat="server" >
                    <li id="liBitModuleHtml" runat="server">
                        <div class="moduleToDrag" data-module-type='bitModuleHtml'>Html</div>
                    </li>
                </ul>
            </li>
            <li><a href="#"><span id="bitSideBarIconData"></span><span class="bitSideBarText">Data
            </span></a>
                <ul id="ulBitModulesData" runat="server" >
                </ul>
            </li>
            <li><a href="#" class="webshop"><span id="bitSideBarIconWebshop"></span><span class="bitSideBarText">Webshop</span></a>
                <ul id="ulBitModulesWebshop" runat="server">
                </ul>
            </li>
            <li><a href="#"><span id="bitSideBarIconLogin"></span><span class="bitSideBarText">Login</span></a>
                <ul id="ulBitModulesAuth" runat="server" >
                </ul>
            </li>
            <li><a href="#"><span id="bitSideBarIconNewsletter"></span><span class="bitSideBarText">Nieuwsbrief</span></a>
                <ul id="ulBitModulesNewsletter" runat="server">
                </ul>
            </li>

        </ul>
        <!-- .data collections -->
        <hr />
        <!-- site -->
        <div class="bitSidebarGroup"><span class="bitSideBarText">Site</span></div>
        <ul>
            <li class="bitLiNoMenu"><a runat="server" id="linkSiteConfig" href="/_bitplate/site/SiteConfig.aspx">
                <span id="bitSideBarIconSiteProperties"></span><span class="bitSideBarText">Eigenschappen</span></a></li>
            <li class="bitLiNoMenu"><a runat="server" id="linkFileManager" href="/_bitplate/filemanager/ELFileManager.aspx">
                <span id="bitSideBarIconSiteFileManagement"></span><span class="bitSideBarText">Bestandsbeheer</span></a></li>
            <li class="bitLiNoMenu"><a runat="server" id="linkPublish" href="/_bitplate/site/Publish.aspx">
                <span id="bitSideBarIconPublishPages"></span><span
                    class="bitSideBarText">Publiceer</span></a>
        </ul>
        <!-- .site -->
        <hr />
        <!-- design -->
        <div class="bitSidebarGroup"><span class="bitSideBarText">Design</span></div>
        <ul>
            <!--<li class="bitLiNoMenu"><a href="javascript:BITEDITPAGE.editTemplate();">
                <span id="bitSideBarIconDesignTemplates"></span><span class="bitSideBarText">Bewerk template</span></a></li>-->
            <li id="bitMenuScripts" runat="server"><a href="#"><span id="bitSideBarIconDesignScripts"></span><span class="bitSideBarText">Scripts</span></a>
                <ul id="ulMenuScripts" style="max-height: 600px; width: 300px; overflow-y: scroll; overflow-x: hidden">
                </ul>
            </li>
            <li id="bitMenuStylesheets" runat="server"><a href="#"><span id="bitSideBarIconCSS"></span><span class="bitSideBarText">Stylesheets</span></a>
                <ul id="ulMenuStylesheets" style="max-height: 600px; width: 300px; overflow-y: scroll; overflow-x: hidden">
                </ul>
            </li>
        </ul>

        <hr />
        <!-- bitplate logo -->
        <div id="bitplateLogo">
            <img src="/_bitplate/_themes/bitplate/images/BPCMS_sidebar_logo.png" width="137" height="26"
                alt="bitplate logo">
        </div>
    </div>

    <div id="bitPopups">
        <!--PAGE CONFIG DIALOG -->
        <div id="bitConfigPagePopup"></div>

        <!--MODULE EDIT DIALOG -->
        <div id="bitEditModuleDialog" title="Bewerk module">
            <div id="bitToolbarInPopup" style="display: none;"></div>
            <br style="clear: both" />
            <div id="bitStatusbarInPopup" style="display: none;"></div>
            <br style="clear: both" />
            <div id="bitModuleTagsInPopup" style="display: none;"></div>
            <div id="bitModuleEditorInPopup" style="">
                <textarea name="EditorInPopup" id="EditorInPopup"></textarea>
            </div>
            <br style="clear: both" />
        </div>

        <!--TAGS DIALOG -->
        <div id="bitTagsDialog" title="Beschikbare tags">
        </div>

        <!--IMAGES DIALOG -->
        <div id="bitImagesDialog" title="Afbeeldingen"></div>

        <!--HYPERLINKS DIALOG -->
        <div id="bitHyperlinksDialog" title="Hyperlinks"></div>

        <!--STYLES DIALOG -->
        <div id="bitStylesDialog" title="Styles"></div>

        <!--SCRIPT EDIT DIALOG -->
        <div id="bitScriptEditDialog" title="Script">
            <div>
                <textarea data-field="Content" class="bitTextarea" style="width: 100%" id="textareaContent"></textarea>
            </div>
        </div>

        <!--MODULE CONFIG DIALOG -->
        <div id="bitConfigModuleDialog">
            <ul id="bitConfigModuleDialogTabs">
                <li id="tabLinkGeneral" class="tabLink"><a href="#tabPageGeneral">Algemene instellingen</a></li>
                <li id="tabLinkNavigation" class="tabLink"><a href="#tabPageNavigation">Navigatie</a></li>
                <li id="tabLinkData" class="tabLink"><a href="#tabPageData">Gegevens</a></li>
                <li id="tabLinkAuth" class="tabLink"><a href="/_bitplate/Dialogs/AutorisationTab.aspx">Autorisatie</a></li>
            </ul>
            <div id="tabPageGeneral" style="display: none">
                <uc1:GeneralConfigTab ID="GeneralConfigTab1" runat="server" />
            </div>
            <div id="tabPageNavigation" class="bitTabPage">
                <uc2:NavigationActionConfigTab ID="NavigationActionConfigTab1" runat="server" />
            </div>
            <div id="tabPageData" class="bitTabPage">
                <uc3:DataConfigTab ID="DataConfigTab1" runat="server" />
            </div>
            
            
        </div>



        <div id="bitFileManagementDialog"></div>

        <div id="selectDataGroupDialog" title="Selecteer Datacollectiegroep">
            <div id="dataCollectionGroupTree"></div>
        </div>

        
    </div>
</div>
