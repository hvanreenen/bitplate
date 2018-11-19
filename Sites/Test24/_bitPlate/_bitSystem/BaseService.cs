using BitPlate.Domain.Logging;
using BitPlate.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BitSite._bitPlate
{
    public class SessionLostException : Exception
    {
        public SessionLostException()
            : base("Sessie is verloren. U dient opnieuw aan te melden.")
        {
            
        }
    }

    public class SiteNullException : Exception
    {
        public SiteNullException()
            : base("Geen site gekozen. Kies eerst een site.")
        {

        }
    }

    public class InvalidLicenseException : Exception
    {
        public InvalidLicenseException()
            : base("Licentie is ongeldig.")
        {

        }
    }
    
    public class BaseService:  System.Web.UI.Page
    {
        protected static void CheckSite()
        {
            //HttpContext.Current.Response.Clear();
            //HttpContext.Current.Response.StatusCode = 307;
            //HttpContext.Current.Response.AddHeader("Location", "~/_bitplate/Login.aspx?SessionExpired=true");
            //HttpContext.Current.Response.Flush();
           
            
            if (SessionObject.CurrentSite == null)
            {
                throw new SiteNullException();
                //HttpContext.Current.Response.Redirect("~/_bitplate/Autorisation/Sites.aspx?NoSiteSelected=true");
            }
        }

        protected static void CheckLoginAndLicense()
        {
            CheckSite();
            
            //check if user
            if (SessionObject.CurrentBitplateUser == null)
            {
                throw new SessionLostException();
                //HttpContext.Current.Response.Redirect("~/_bitplate/Login.aspx?SessionExpired=true");
            }
            //check license
            if (SessionObject.CurrentLicense == null)
            {
                //if (!HttpContext.Current.Request.UrlReferrer.ToString().Contains("SiteConfig.aspx"))
                //{
                throw new InvalidLicenseException();
                    //HttpContext.Current.Response.Redirect("~/_bitplate/SiteConfig.aspx?InvalidLicense=true");
                //}
            }
        }

        public void Page_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            BitPlate.Domain.Logging.EventLog.LogException(ex);
            string err = "<b>Error Caught in Page_Error event</b><hr><br>" +
                "<br><b>Error in: </b>" + Request.Url.ToString() +
                "<br><b>Error Message: </b>" + ex.Message.ToString() +
                "<br><b>Stack Trace:</b><br>" +
                              ex.StackTrace.ToString();
            Response.Write(err.ToString()); 
            Server.ClearError();
            string path = ConfigurationManager.AppSettings["ErrorLogPath"];
            if (path == null || path == "") path = Server.MapPath("");
            Logger.Log(path + @"\error_log_.txt", err);
        }
    }
}