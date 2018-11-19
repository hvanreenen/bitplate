using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitPlate.Domain.Utils;
using BitPlate.Domain.Logging;
using System.IO;

namespace BitMetaServer
{
    public class BasePage : System.Web.UI.Page
    {
        public Translator Translator {get; set;}

        public BasePage()
        {
            string Language = (SessionObject.CurrentUser != null) ? SessionObject.CurrentUser.Language : "NL";
            this.Translator = new Translator(this.GetType().Name, Language);
            
        }
        /// <summary>
        /// kijk of er een site is geladen
        /// </summary>
        protected void CheckSite()
        {
            
        }

        

        protected void CheckLogin()
        {
            //check if user
            if (SessionObject.CurrentUser == null)
            {
                Response.Redirect("~/Login.aspx?referer=" + HttpContext.Current.Request.Url.ToString());
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
            Server.Transfer("/Error.aspx");
        }
    }
}