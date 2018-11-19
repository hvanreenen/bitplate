using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Newsletters;
using BitPlate.Domain.Licenses;

namespace BitSite._bitPlate.EditPage
{
    public partial class EditPageMenu2 : System.Web.UI.UserControl
    {
        protected CmsPage cmsPage { get; set; }
        public bool InNewslettersMode = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SessionObject.HasPermission(FunctionalityEnum.PagesEdit))
            {
                throw new Exception("U heeft geen rechten om een pagina te bewerken.");
            }
            if (SessionObject.CurrentBitplateUser != null)
            {
                if (!IsPostBack)
                {
                    System.Diagnostics.Trace.WriteLine("Page_Load", "EditPageMenu");


                    System.Diagnostics.Trace.WriteLine("fillMenuPages Start", "EditPageMenu");
                    if (!InNewslettersMode)
                    {
                        fillMenuPages();
                    }
                    System.Diagnostics.Trace.WriteLine("fillMenuPages End", "EditPageMenu");

                    System.Diagnostics.Trace.WriteLine("fillMenuModules Start", "EditPageMenu");
                    fillMenuModules();
                    System.Diagnostics.Trace.WriteLine("fillMenuModules End", "EditPageMenu");

                    System.Diagnostics.Trace.WriteLine("fillMenuScripts Start", "EditPageMenu");
                    //fillMenuScripts();
                    System.Diagnostics.Trace.WriteLine("fillMenuScripts End", "EditPageMenu");

                    //linkPreviewPage.HRef = Request.Url.ToString().Replace("?" + Request.QueryString.ToString(), "");
                    this.ltrlLoggedInAs.Text = SessionObject.CurrentBitplateUser.Email;


                    System.Diagnostics.Trace.WriteLine("Page_Load End", "EditPageMenu");

                    if (InNewslettersMode)
                    {
                        bitMenuEditPage.Visible = false;
                        bitMenuScripts.Visible = false;

                    }
                }
            }

            if (!SessionObject.HasPermission(BitPlate.Domain.Licenses.FunctionalityEnum.Pages))
            {
                this.liPages.Visible = false;
            }
            if (!SessionObject.HasPermission(BitPlate.Domain.Licenses.FunctionalityEnum.DataCollections))
            {
                this.liDataCollections.Visible = false;
            }
            if (!SessionObject.HasPermission(BitPlate.Domain.Licenses.FunctionalityEnum.Templates))
            {
                this.liTemplates.Visible = false;
            }
            linkSiteConfig.HRef += "?Referer=" + Request.Url.ToString();
            linkFileManager.HRef += "?Referer=" + Request.Url.ToString();
            linkPublish.HRef += "?Referer=" + Request.Url.ToString();
            if (!SessionObject.HasPermission(FunctionalityEnum.PagesConfig))
            {
                linkConfigPage.HRef = "#";
                linkConfigPage.Disabled = true;
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.SiteConfig))
            {
                linkSiteConfig.HRef = "#";
                linkSiteConfig.Disabled = true;
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.FileManager))
            {
                linkFileManager.HRef = "#";
                linkFileManager.Disabled = true;
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.Publish))
            {
                linkPublish.HRef = "#";
                linkPublish.Disabled = true;
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.ScriptsEdit))
            {
                bitMenuScripts.Visible = false;
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.StylesheetsEdit))
            {
                bitMenuStylesheets.Visible = false;
            }
            
            

        }





        private void fillMenuPages()
        {
            string html = "";
            BaseCollection<CmsPage> pages = SessionObject.CurrentSite.Pages;
            foreach (CmsPage page in pages)
            {
                html += String.Format("<li><a href=\"javascript:BITEDITPAGE.loadPage('{0}');\">{1}</a></li>", page.ID, page.RelativeUrl);

            }
            ulMenuPages.InnerHtml = html;
        }

        private void fillMenuModules()
        {
            foreach (ModuleDefinition moduleDef in SessionObject.AvailableModules)
            {
                if (hasPermissionsOnModule(moduleDef))
                {
                    if ((moduleDef.PageProof && !InNewslettersMode) || (moduleDef.NewsletterProof && InNewslettersMode))
                    {
                        if (moduleDef.MenuFolder == "General")
                        {
                            System.Web.UI.HtmlControls.HtmlGenericControl newLi = new System.Web.UI.HtmlControls.HtmlGenericControl("li");
                            newLi.InnerHtml = String.Format("<div class='moduleToDrag' data-module-type='{0}'>{1}</div>", moduleDef.ModuleType, moduleDef.MenuName);
                            ulBitModulesGeneral.Controls.Add(newLi);

                        }
                        else if (moduleDef.MenuFolder == "Data")
                        {
                            System.Web.UI.HtmlControls.HtmlGenericControl newLi = new System.Web.UI.HtmlControls.HtmlGenericControl("li");
                            newLi.InnerHtml = String.Format("<div class='moduleToDrag' data-module-type='{0}'>{1}</div>", moduleDef.ModuleType, moduleDef.MenuName);
                            ulBitModulesData.Controls.Add(newLi);
                        }
                        else if (moduleDef.MenuFolder == "Auth")
                        {

                            System.Web.UI.HtmlControls.HtmlGenericControl newLi = new System.Web.UI.HtmlControls.HtmlGenericControl("li");
                            newLi.InnerHtml = String.Format("<div class='moduleToDrag' data-module-type='{0}'>{1}</div>", moduleDef.ModuleType, moduleDef.MenuName);
                            ulBitModulesAuth.Controls.Add(newLi);

                        }
                        else if (moduleDef.MenuFolder == "Webshop")
                        {

                            System.Web.UI.HtmlControls.HtmlGenericControl newLi = new System.Web.UI.HtmlControls.HtmlGenericControl("li");
                            newLi.InnerHtml = String.Format("<div class='moduleToDrag' data-module-type='{0}'>{1}</div>", moduleDef.ModuleType, moduleDef.MenuName);
                            ulBitModulesWebshop.Controls.Add(newLi);

                        }
                        else if (moduleDef.MenuFolder == "Newsletter")
                        {

                            System.Web.UI.HtmlControls.HtmlGenericControl newLi = new System.Web.UI.HtmlControls.HtmlGenericControl("li");
                            newLi.InnerHtml = String.Format("<div class='moduleToDrag' data-module-type='{0}'>{1}</div>", moduleDef.ModuleType, moduleDef.MenuName);
                            ulBitModulesNewsletter.Controls.Add(newLi);

                        }
                    }
                }
            }

            if (this.InNewslettersMode)
            {
                liBitModuleHtml.Visible = false;
                ulBitModulesNewsletter.Visible = false;
            }
        }

        private bool hasPermissionsOnModule(ModuleDefinition moduleDef)
        {
            bool returnValue = false;
            if (moduleDef.ModuleType == "HtmlModule")
            {
                returnValue = true;
            }
            else
            {
                returnValue = SessionObject.HasPermission(getFunctionalityNumberByModuleName(moduleDef.ModuleType));
            }

            return returnValue;
        }

        private FunctionalityEnum getFunctionalityNumberByModuleName(string moduleName)
        {
            if (moduleName == "ContactFormModule")
            {
                return FunctionalityEnum.ModuleInputForm;
            }
            else if (moduleName == "SearchModule")
            {
                return FunctionalityEnum.ModuleSearch;
            }
            else if (moduleName == "SearchResultsModule")
            {
                return FunctionalityEnum.ModuleSearchResults;
            }
            else if (moduleName == "GroupListModule")
            {
                return FunctionalityEnum.ModuleDataGroups;
            }
            else if (moduleName == "GroupDetailsModule")
            {
                return FunctionalityEnum.ModuleDataGroupDetails;
            }
            else if (moduleName == "ItemListModule")
            {
                return FunctionalityEnum.ModuleDataItems;
            }
            else if (moduleName == "ItemDetailsModule")
            {
                return FunctionalityEnum.ModuleDataItemDetails;
            }
            else if (moduleName == "TreeViewModule")
            {
                return FunctionalityEnum.ModuleDataTree;
            }
            else if (moduleName == "BreadCrumbModule")
            {
                return FunctionalityEnum.ModuleDataBreadCrumb;
            }
            else if (moduleName == "GoogleMapsModule")
            {
                return FunctionalityEnum.ModuleDataGoogleMaps;
            }
            else if (moduleName == "FilterModule")
            {
                return FunctionalityEnum.ModuleDataFilter;
            }
            else if (moduleName == "LoginModule")
            {
                return FunctionalityEnum.ModuleAuthLogin;
            }
            else if (moduleName == "LoginStatusModule")
            {
                return FunctionalityEnum.ModuleAuthLoginStatus;
            }
            else if (moduleName == "MyProfileModule")
            {
                return FunctionalityEnum.ModuleAuthLoginData;
            }
            else if (moduleName == "SubscribeModule")
            {
                return FunctionalityEnum.NewsletterModulesSubscribe;
            }
            else if (moduleName == "OptInModule")
            {
                return FunctionalityEnum.NewsletterModulesOptin;
            }
            else if (moduleName == "UnsubscribeModule")
            {
                return FunctionalityEnum.NewsletterModulesUnsubscribe;
            }
            else if (moduleName == "NewsletterHtmlModule")
            {
                return FunctionalityEnum.ModuleHTML;
            }
            
            else
            {
                return FunctionalityEnum.ModuleHTML;
            }
        }



 

        protected void lbtnUitloggen_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/_bitplate/login.aspx");
        }

    }
}