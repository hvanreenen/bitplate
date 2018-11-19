using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;

namespace BitSite._bitPlate.Site
{
    public partial class SiteConfig : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckSite();
            base.CheckLogin();
            if (SessionObject.CurrentLicense.IsValid)
            {
                base.CheckPermissions(BitPlate.Domain.Licenses.FunctionalityEnum.SiteConfig, true);
            }
            else
            {
                base.CheckPermissions(BitPlate.Domain.Licenses.FunctionalityEnum.SiteConfig, false);
            }

            if (Request.QueryString["InvalidLicense"] != null)
            {
                //LiteralMsg.Text = "<h2 style='color:darkred'>Licentie is ongeldig</h2>";
            }
            if (Request.QueryString["Referer"] != null)
            {
                this.BackLink.HRef = Request.QueryString["Referer"];
                this.cancelButton.Attributes["onclick"] = "javascript:location.href='" + Request.QueryString["Referer"] + "';";
                
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
                        languageWrapper.InnerHtml = "Uw licentie staat het gebruik van meertaligheid niet toe.";
                    }
                }
            }
        }
    }
}