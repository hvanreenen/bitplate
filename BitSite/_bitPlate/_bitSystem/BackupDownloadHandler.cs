using BitPlate.Domain;
using BitSite._bitPlate._bitSystem;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BitSite._bitPlate
{
    public class BackupDownloadHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/zip";
            //
            // Write the requested image
            //
            string file = context.Request.QueryString["file"];
            file = SessionObject.CurrentSite.Path + "\\..\\Backups\\" + file;
            context.Response.WriteFile(file);
        }
    }
}