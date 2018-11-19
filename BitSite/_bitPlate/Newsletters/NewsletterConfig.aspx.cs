using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Licenses;

namespace BitSite._bitPlate.Site
{
    public partial class NewsletterConfig : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckSite();
            base.CheckLogin();
            if (SessionObject.CurrentLicense == null)
            {
                base.CheckPermissions(BitPlate.Domain.Licenses.FunctionalityEnum.SiteConfig, false);
            }
            else
            {
                base.CheckPermissions(BitPlate.Domain.Licenses.FunctionalityEnum.SiteConfig, true);
            }

            if (Request.QueryString["InvalidLicense"] != null)
            {
                //LiteralMsg.Text = "<h2 style='color:darkred'>Licentie is ongeldig</h2>";
            }
            if (Request.QueryString["Referer"] != null)
            {
                this.BackLink.HRef = Request.QueryString["Referer"];
                //if (Request.QueryString["Template"] != null)
                //{
                //    this.BackLink.HRef += "?Template";
                //}
                //if (Request.QueryString["id"] != null)
                //{
                //    this.BackLink.HRef += "#" + Request.QueryString["id"];
                //}
            }

            if (!SessionObject.HasPermission(FunctionalityEnum.SiteConfig, false))
            {
                throw new Exception("U heeft geen rechten op deze pagina");
            }
            else
            {
                if (SessionObject.CurrentLicense != null)
                {
                    if (!SessionObject.CurrentLicense.AllowMultipleLanguages)
                    {
                        //languageWrapper.InnerHtml = "Uw licentie staat het gebruik van meertaligheid niet toe.";
                    }
                }
            }

            foreach (CmsPage page in SessionObject.CurrentSite.Pages)
            {
                
                ListItem li = new ListItem(page.RelativeUrl, page.ID.ToString());
                if (this.hasPageOptinModule(page))
                {
                    selectOptInEmailPage.Items.Add(li);
                }

                if (this.hasPageOptOutModule(page)) 
                {
                    selectOptOutEmailPage.Items.Add(li);
                }
            }
        }

        public bool hasPageOptinModule(CmsPage page)
        {
            foreach (BaseModule module in page.Modules)
            {
                if (module.Type == "OptInModule")
                {
                    return true;
                }
            }
            return false;
        }

        public bool hasPageOptOutModule(CmsPage page)
        {
            foreach (BaseModule module in page.Modules)
            {
                if (module.Type == "UnsubscribeModule" || module.Type == "OptOutModule")
                {
                    return true;
                }
            }
            return false;
        }
    }
}