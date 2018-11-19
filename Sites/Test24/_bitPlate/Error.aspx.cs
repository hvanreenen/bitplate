using BitPlate.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            if (ex != null)
            {
                try
                {
                    BitPlate.Domain.Logging.EventLog.LogException(ex, Request.Url.ToString());
                }
                catch (Exception exc)
                {
                }
                
                string err = "<br /><br /><br /><b>Error Caught in Page_Error event</b><hr><br>" +
                    "<br><b>Error in: </b>" + Request.Url.ToString() +
                    "<br><b>Error Message: </b>" + ex.Message.ToString() +
                    "<br><b>Stack Trace:</b><br>" +
                                  ex.StackTrace.ToString();

                Server.ClearError();
                //string supportEmail = System.Configuration.ConfigurationManager.AppSettings["ExceptionSupportEmailAddress"];
                this.LiteralErrorMsg.Text = ex.Message.ToString();
                this.LiteralErrorDetails.Text = "<pre>" + err + "</pre>"; //<!--<br /><div><a href=\"mailto:" + supportEmail + "?SUBJECT=Bitplate.CMS error&BODY=" + err + "\" \">Verstuur de fout details</a></div>-->";

                try
                {
                    string path = ConfigurationManager.AppSettings["ErrorLogPath"];
                    if (path == null || path == "") path = Server.MapPath("");
                    Logger.Log(path + @"\error_log_.txt", err);
                }
                catch (Exception exc)
                {
                }
                
            }
        }
    }
}