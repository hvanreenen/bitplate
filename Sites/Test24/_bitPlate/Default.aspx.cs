using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain;
using BitPlate.Domain.DataCollections;
using HJORM;
using BitPlate.Domain.Licenses;

namespace BitSite._bitPlate
{
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckSite();
            base.CheckLogin();
            BuildFrameRecentChangeItems();

            //if (SessionObject.CurrentBitplateUser == null || SessionObject.CurrentBitplateUser.GetAllPermissions().Count == 0)
            //{
            //    noPermissionsDialog.Attributes.Add("class", "show");
            //}

            //BLOK: SITE CONTENT 
            if (SessionObject.HasPermission(FunctionalityEnum.SiteContentManagement))
            {
                if (!SessionObject.HasPermission(FunctionalityEnum.Pages))
                {
                    liPages.Disabled = true;
                    liPages.Attributes["class"] = "bitItemDisabled";
                    aPages.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.FileManager))
                {
                    liFileManager.Disabled = true;
                    liFileManager.Attributes["class"] = "bitItemDisabled";
                    aFileManager.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.DataCollections))
                {
                    liDataCollections.Disabled = true;
                    liDataCollections.Attributes["class"] = "bitItemDisabled";
                    aDataCollections.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.Publish))
                {
                    liPublish.Disabled = true;
                    liPublish.Attributes["class"] = "bitItemDisabled";
                    aPublish.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.MenuManager))
                {
                    liMenuManager.Disabled = true;
                    liMenuManager.Attributes["class"] = "bitItemDisabled";
                    aMenuManager.HRef = "#";

                }
            }
            else
            {
                //hele blok "Site content" disablen
                this.bitBox02.Disabled = true;
                //alle urls in blok uitzetten
                aPages.HRef = "#";
                aFileManager.HRef = "#";
                aDataCollections.HRef = "#";
                aPublish.HRef = "#";
                aMenuManager.HRef = "#";
                liPages.Attributes["class"] = "bitItemDisabled";
                liFileManager.Attributes["class"] = "bitItemDisabled";
                liDataCollections.Attributes["class"] = "bitItemDisabled";
                liPublish.Attributes["class"] = "bitItemDisabled";
                liMenuManager.Attributes["class"] = "bitItemDisabled";

            }
           
            //BLOK: SITE BEHEER
            if (SessionObject.HasPermission(FunctionalityEnum.SiteManagement, false))
            {
                if (!SessionObject.HasPermission(FunctionalityEnum.SiteConfig, false))
                {
                    liSiteConfig.Disabled = true;
                    liSiteConfig.Attributes["class"] = "bitItemDisabled";
                    aSiteConfig.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.Templates))
                {
                    liTemplates.Disabled = true;
                    liTemplates.Attributes["class"] = "bitItemDisabled";
                    aTemplates.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.Backups))
                {
                    liBackups.Disabled = true;
                    liBackups.Attributes["class"] = "bitItemDisabled";
                    aBackups.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.Scripts))
                {
                    liScripts.Disabled = true;
                    liScripts.Attributes["class"] = "bitItemDisabled";
                    aScripts.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.Stylesheets))
                {
                    liStylesheets.Disabled = true;
                    liStylesheets.Attributes["class"] = "bitItemDisabled";
                    aStylesheets.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.EventLog))
                {
                    liEventlog.Disabled = true;
                    liEventlog.Attributes["class"] = "bitItemDisabled";
                    aEventlog.HRef = "#";
                }
            }
            else
            {
                //hele blok "Site management" disablen
                this.bitBox03.Disabled = true;
                //alle urls in blok uitzetten
                aSiteConfig.HRef = "#";
                aTemplates.HRef = "#";
                aBackups.HRef = "#";
                aStylesheets.HRef = "#";
                aScripts.HRef = "#";
                aEventlog.HRef = "#";
                liSiteConfig.Attributes["class"] = "bitItemDisabled";
                liTemplates.Attributes["class"] = "bitItemDisabled";
                liBackups.Attributes["class"] = "bitItemDisabled";
                liScripts.Attributes["class"] = "bitItemDisabled";
                liStylesheets.Attributes["class"] = "bitItemDisabled";
                liEventlog.Attributes["class"] = "bitItemDisabled";
            }

            //BLOK: NIEUWSBRIEVEN
            if (SessionObject.HasPermission(FunctionalityEnum.NewsLetters))
            {
                if (!SessionObject.HasPermission(FunctionalityEnum.NewsLettersOverview))
                {
                    liNewsLetters.Disabled = true;
                    liNewsLetters.Attributes["class"] = "bitItemDisabled";
                    aNewsLetters.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.NewsLettersOverview))
                {
                    liNewsletterTemplates.Disabled = true;
                    liNewsletterTemplates.Attributes["class"] = "bitItemDisabled";
                    aNewsLetterTemplates.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.NewsLetterSubscriptions))
                {
                    liNewsLetterSubscriptions.Disabled = true;
                    liNewsLetterSubscriptions.Attributes["class"] = "bitItemDisabled";
                    aNewsLetterSubscriptions.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.NewsLetterSettings))
                {
                    liNewsLetterSettings.Disabled = true;
                    liNewsLetterSettings.Attributes["class"] = "bitItemDisabled";
                    aNewsLetterSettings.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.NewsLetterStats))
                {
                    liNewsLetterStats.Disabled = true;
                    liNewsLetterStats.Attributes["class"] = "bitItemDisabled";
                    aNewsLetterStats.HRef = "#";
                }
            }
            else
            {
                //hele blok "Nieuwsbrieven" disablen
                this.bitBox04.Disabled = true;
                //alle urls in blok uitzetten
                aNewsLetters.HRef = "#";
                aNewsLetterTemplates.HRef = "#";
                aNewsLetterSubscriptions.HRef = "#";
                aNewsLetterSettings.HRef = "#";
                aNewsLetterStats.HRef = "#";
                liNewsLetters.Attributes["class"] = "bitItemDisabled";
                liNewsletterTemplates.Attributes["class"] = "bitItemDisabled";
                liNewsLetterSubscriptions.Attributes["class"] = "bitItemDisabled";
                liNewsLetterStats.Attributes["class"] = "bitItemDisabled";
                liNewsLetterSettings.Attributes["class"] = "bitItemDisabled";
            }

            //BLOK: Serverbeheer
            if (SessionObject.HasPermission(FunctionalityEnum.ServerManagement, false))
            {
                if (!(SessionObject.HasPermission(FunctionalityEnum.ServerSites, false) && SessionObject.CurrentBitplateUser.IsMultiSiteUser))
                {
                    //is alleen beschikbaar voor multisite users die ook rechten hebben
                    liServerSites.Disabled = true;
                    liServerSites.Attributes["class"] = "bitItemDisabled";
                    aServerSites.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.ServerUsers, false))
                {
                    liServerUsers.Disabled = true;
                    liServerUsers.Attributes["class"] = "bitItemDisabled";
                    aServerUsers.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.ServerBackups, false))
                {
                    liServerBackups.Disabled = true;
                    liServerBackups.Attributes["class"] = "bitItemDisabled";
                    aServerBackups.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.ServerEventLog, false))
                {
                    liServerEventlog.Disabled = true;
                    liServerEventlog.Attributes["class"] = "bitItemDisabled";
                    aServerEventlog.HRef = "#";
                }
            }
            else
            {
                //hele blok "Server beheer" disablen
                this.bitBox05.Disabled = true;
                //alle urls in blok uitzetten
                aServerSites.HRef = "#";
                aServerUsers.HRef = "#";
                aServerBackups.HRef = "#";
                aServerEventlog.HRef = "#";
                liServerSites.Attributes["class"] = "bitItemDisabled";
                liServerUsers.Attributes["class"] = "bitItemDisabled";
                liServerEventlog.Attributes["class"] = "bitItemDisabled";
                liServerBackups.Attributes["class"] = "bitItemDisabled";

            }

            //BLOK: Webshop
            if (SessionObject.HasPermission(FunctionalityEnum.Webshop))
            {
                if (!SessionObject.HasPermission(FunctionalityEnum.WebshopOrders))
                {
                    liWebshopOrders.Disabled = true;
                    aWebshopOrders.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.WebshopProducts))
                {
                    liWebshopProducts.Disabled = true;
                    aWebshopProducts.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.WebshopSettings))
                {
                    liWebshopSettings.Disabled = true;
                    aWebshopSettings.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.WebshopUsers))
                {
                    liWebshopUsers.Disabled = true;
                    aWebshopUsers.HRef = "#";
                }
            }
            else
            {
                //hele blok "Webshop" disablen
                this.bitBox07.Disabled = true;
                //alle urls in blok uitzetten
                aWebshopOrders.HRef = "#";
                aWebshopProducts.HRef = "#";
                aWebshopSettings.HRef = "#";
                aWebshopUsers.HRef = "#";
                liWebshopOrders.Attributes["class"] = "bitItemDisabled";
                liWebshopProducts.Attributes["class"] = "bitItemDisabled";
                liWebshopUsers.Attributes["class"] = "bitItemDisabled";
                liWebshopUsers.Attributes["class"] = "bitItemDisabled";
            }

            //BLOK: UserManagement
            if (SessionObject.HasPermission(FunctionalityEnum.SiteUserManagement))
            {
                //tijdelijk uitzetten vanwege bug
                if (!SessionObject.HasPermission(FunctionalityEnum.SiteUsers))
                {
                    liWebUsers.Disabled = true;
                    aWebUsers.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.SiteUserGroups))
                {
                    liWebUserGroups.Disabled = true;
                    aWebUserGroups.HRef = "#";
                }

            }
            else
            {
                //hele blok "User management" disablen
                this.bitBox08.Disabled = true;
                //alle urls in blok uitzetten
                aWebUsers.HRef = "#";
                aWebUserGroups.HRef = "#";
                liWebUserGroups.Attributes["class"] = "bitItemDisabled";
                liWebUsers.Attributes["class"] = "bitItemDisabled";
            }

            if (SessionObject.CurrentLicense != null)
            //&& SessionObject.CurrentLicense.Owner != null && SessionObject.CurrentLicense.Owner.Reseller != null)
            {
                //Company reseller = SessionObject.CurrentLicense.Owner.Reseller;

                //string resellerHtml = String.Format(@"T.: {0}<br />" +
                //        "<a href=\"{1}\" target=\"\\_blank\">{1}</a><br />" +
                //        "<a href=\"mailto:{2}\">{2}</a>", reseller.Telephone, reseller.Website, reseller.Email);
                bitResellerContactInfo.InnerHtml = SessionObject.CurrentLicense.ResellerInfo;
                string licenseServerUrl = ConfigurationManager.AppSettings["LicenseHost"];
                //bitResellerLogo.Src = licenseServerUrl + "_img/ResellerLogos/" + reseller.Name.ToLower() + ".png";
                bitResellerLogo.Src = licenseServerUrl + SessionObject.CurrentLicense.ResellerLogoSrc;
                bitResellerLogo.Alt = "reseller";
            }

            else
            {
                bitResellerContactInfo.InnerHtml = "";
                bitResellerLogo.Visible = false;
            }

        }

        public void BuildFrameRecentChangeItems()
        {
            if (SessionObject.HasPermission(FunctionalityEnum.DataCollectionsEditData))
            {
                BaseCollection<DataCollection> dataCollectionList = BaseCollection<DataCollection>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "'", "ModifiedDate DESC", 1, 5);
                //List<DataCollection> TopDataCollections = DataCollectionList.Take(5).ToList();

                TopDataCollectionList.Text = "<ul>";
                foreach (DataCollection collection in dataCollectionList)
                {
                    TopDataCollectionList.Text += String.Format("<li><a href=\"DataCollections/DataCollectionData.aspx?datacollectionid={0}\"><div class=\"lastChangedItemName bitTableColumnEllipsis\"  title=\"{1}\">{1}</div><div class=\"lastChangedItemDate\">{2:dd-MM-yy HH:mm}</div></a></li>", collection.ID, collection.Name, collection.ModifiedDate);
                }
                TopDataCollectionList.Text += "</ul>";
            }

            if (SessionObject.HasPermission(FunctionalityEnum.PagesEdit))
            {
                //Is LoadFromSQL misschien een betere oplossing ipv get?
                BaseCollection<CmsPage> PageList = BaseCollection<CmsPage>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "'", "ModifiedDate DESC", 1, 5);
                //List<CmsPage> TopCMSPages = PageList.Take(5).ToList();

                TopPageList.Text = "<ul>";
                foreach (CmsPage page in PageList)
                {
                    //TopPageList.Text += string.Format("<li><a href=\"/_bitplate/EditPage/EditPage.aspx?pageid={0}\"><div class=\"lastChangedItemName bitTableColumnEllipsis\"  title=\"{1}\">{1}</div><div class=\"lastChangedItemDate\">{2:dd-MM-yy HH:mm}</div></a></li>", page.ID, page.Name, page.ModifiedDate);
                    TopPageList.Text += string.Format("<li><a href=\"{1}?mode=edit\"><div class=\"lastChangedItemName bitTableColumnEllipsis\"  title=\"{1}\">{1}</div><div class=\"lastChangedItemDate\">{2:dd-MM-yy HH:mm}</div></a></li>", page.ID, page.RelativeUrl, page.ModifiedDate);
                }
                TopPageList.Text += "</ul>";
            }
        }
    }
}