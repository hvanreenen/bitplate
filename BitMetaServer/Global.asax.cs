using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Security;
using System.Web.SessionState;
using BitSite._bitPlate;
using BitPlate.Domain;
using BitPlate.Domain.Utils;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using BitPlate.Domain.Newsletters;
using HJORM.Attributes;

namespace BitMetaServer
{
    public class Global : System.Web.HttpApplication
    {


        protected void Application_Start(object sender, EventArgs e)
        {
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            BitBundler.Init();
            Translator.InitStaticValues();

            //Test
            //System.Reflection.PropertyInfo p = typeof(System.Web.HttpRuntime).GetProperty("FileChangesMonitor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            //object o = p.GetValue(null, null);

            //System.Reflection.FieldInfo f = o.GetType().GetField("_dirMonSubdirs", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);

            //object monitor = f.GetValue(o); //Returns NULL

            //System.Reflection.MethodInfo m = monitor.GetType().GetMethod("StopMonitoring", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic); m.Invoke(monitor, new object[] { });
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            //SessionObject.SetSiteObject();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //string inputPath = Request.Url.LocalPath.ToLower();
            //string inputQueryString = Request.QueryString.ToString();
            
            //string siteid = ConfigurationManager.AppSettings["SiteID"];
            //string outputUrl = "";

            //bool isEditMode = (Request.QueryString["mode"] == "edit");

            ////Genereer het werkelijke path en controlleer of het een echt bestand is of dat het een virtueel bestand is.
            ////string serverPath = Server.MapPath("") + inputPath;
            ////serverPath = serverPath.Replace("/", "\\");
            //if (inputPath.StartsWith("/_bit") || inputPath == "/page.aspx") //|| File.Exists(serverPath))
            //{
            //    return;
            //}
            //else if (inputPath.EndsWith(".css") || inputPath.EndsWith(".js"))
            //{
            //    if (inputPath.Contains("jquery-1.8.2.js") || inputPath.Contains("bit") || inputPath.Contains("jquery.iframe-post-form.js") || inputPath.Contains("JSON.js"))
            //    {
            //        return;
            //    }
            //    string scriptid = CmsScript.GetScriptIDByUrl(inputPath, siteid);
            //    //Geen script ID? Dan gewoon de orginele URL doorsturen.
            //    if (scriptid == "" || scriptid == null)
            //    {
            //        return;
            //    }
            //    outputUrl = "/script.handler?scriptid=" + scriptid;
            //    Context.RewritePath(outputUrl);
            //}
            //else
            //{
            //    outputUrl = UrlRewriter.GetOriginalUrl(inputPath, inputQueryString, siteid, isEditMode);
            //}

            //if (outputUrl != string.Empty)
            //{
            //    Context.RewritePath(outputUrl);
            //}
        }

        
        //private string CompleteOutputUrlWithQueryString(string outputUrl)
        //{
        //    if (Request.Url.Query != "")
        //    {
        //        string querystring = Request.Url.Query;
        //        querystring = "&" + querystring.Substring(1);
        //        outputUrl += querystring;
        //    }
        //    return outputUrl;
        //}

        
        

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            string Message = "\r\nMESSAGE: " + ex.Message +
                "\r\nURL: " + Request.RawUrl +
             "\r\nSOURCE: " + ex.Source +
             "\r\nFORM: " + Request.Form.ToString() +
             "\r\nQUERYSTRING: " + Request.QueryString.ToString() +
             "\r\nTARGETSITE: " + ex.TargetSite +
             "\r\nSTACKTRACE: " + ex.StackTrace;
            string path = ConfigurationManager.AppSettings["ErrorLogPath"];
            if (path == null || path == "") path = Server.MapPath("");
            Logger.Log(path + @"\error_log_.txt", Message);

            //En dan nog iets. Weet even niet HOE.
        }

        protected void Session_End(object sender, EventArgs e)
        {
            //Response.Redirect("~/Login.aspx?SessionExpired=true");
        }

        protected void Application_End(object sender, EventArgs e)
        {
            HttpRuntime runtime = (HttpRuntime)typeof(System.Web.HttpRuntime).InvokeMember("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
            string shutDownMessage = (string)runtime.GetType().InvokeMember("_shutDownMessage", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);
            string shutDownStack = (string)runtime.GetType().InvokeMember("_shutDownStack", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);
            string path = ConfigurationManager.AppSettings["ErrorLogPath"];
            if (path == null || path == "") path = Server.MapPath("");
            string filename = path + "\\AppDomain_log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            BitPlate.Domain.Utils.Logger.Log(filename, shutDownMessage + "\r\n\r\n" + shutDownStack + "\r\n\r\n");
            //System.Diagnostics.Debugger.Break();
            
        }
    }
}