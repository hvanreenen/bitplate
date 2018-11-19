using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitPlate.Domain.Utils;
using BitPlate.Domain.Logging;
using System.IO;

namespace BitSite._bitPlate
{
    public class BasePage : System.Web.UI.Page
    {
        public Translator Translator {get; set;}

        public BasePage()
        {
            string Language = (SessionObject.CurrentBitplateUser != null) ? SessionObject.CurrentBitplateUser.Language : "NL";
            this.Translator = new Translator(this.GetType().Name, Language);
            
        }
        /// <summary>
        /// kijk of er een site is geladen
        /// </summary>
        protected void CheckSite()
        {
            if (SessionObject.CurrentSite == null)
            {
                Response.Redirect("~/_bitplate/Autorisation/Sites.aspx?NoSiteSelected=true");
            }
        }

        protected void CheckLoginAndLicense()
        {
            CheckSite();
            //check if user
            if (SessionObject.CurrentBitplateUser == null)
            {
                Response.Redirect("~/_bitplate/Login.aspx?SessionExpired=true&referer=" + HttpContext.Current.Request.Url.ToString());
            }
            //check license
            if (SessionObject.CurrentLicense == null)
            {
                Response.Redirect("~/_bitplate/SiteConfig.aspx?InvalidLicense=true");
            }
            Session.Timeout = 600;
        }

        protected void CheckLogin()
        {
            //check if user
            if (SessionObject.CurrentBitplateUser == null)
            {
                Response.Redirect("~/_bitplate/Login.aspx?referer=" + HttpContext.Current.Request.Url.ToString());
            }
           
        }

        internal void CheckPermissions(BitPlate.Domain.Licenses.FunctionalityEnum functionality, bool checkLicenseAlso = true)
        {
            if (!SessionObject.HasPermission(functionality, checkLicenseAlso))
            {
                throw new Exception("U heeft geen rechten voor deze functionaliteit.");
            }
        }

        public void Page_Error(object sender, EventArgs e)
        {
            Server.Transfer("/_bitplate/error.aspx");
        }
    }
}