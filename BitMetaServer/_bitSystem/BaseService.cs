using BitPlate.Domain.Logging;
using BitPlate.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BitMetaServer
{
    public class SessionLostException : Exception
    {
        public SessionLostException()
            : base("Sessie is verloren. U dient opnieuw aan te melden.")
        {
            
        }
    }

    

    public class BaseService:  System.Web.UI.Page
    {
        

        protected static void CheckLogin()
        {
            
            //check if user
            if (SessionObject.CurrentUser == null)
            {
                throw new SessionLostException();
                
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