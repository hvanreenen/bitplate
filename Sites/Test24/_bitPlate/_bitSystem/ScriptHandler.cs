using BitPlate.Domain;
using BitSite._bitPlate._bitSystem;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitSite._bitPlate
{
    public class ScriptHandler: IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string scriptid = "";
            if (context.Request.QueryString["scriptid"] != null)
            {
                scriptid = context.Request.QueryString["scriptid"];
                if (scriptid != "")
                {
                    context.Response.Clear();
                    CmsScript script = BitCaching.FromCache<CmsScript>(scriptid);
                    if (script == null)
                    {
                        script = BaseObject.GetById<CmsScript>(new Guid(scriptid));
                        if (script.ScriptType == ScriptTypeEnum.Css)
                        {
                            script.Content = MinifyStyleSheet(script.Content);
                        }
                        else
                        {
                            script.Content = MinifyJavascript(script.Content);
                        }
                        script.ToCache(scriptid);
                    }
                    context.Response.ContentType = script.ContentType;
                    context.Response.Write(script.Content);
                    context.Response.End();
                }
            }
        }

        private string MinifyStyleSheet(string script)
        {
            Microsoft.Ajax.Utilities.Minifier minifier = new Microsoft.Ajax.Utilities.Minifier();
            script = minifier.MinifyStyleSheet(script);
            return script;
        }

        public string MinifyJavascript(string script)
        {
            Microsoft.Ajax.Utilities.Minifier minifier = new Microsoft.Ajax.Utilities.Minifier();
            script = minifier.MinifyJavaScript(script);
            return script;
        }
    }
}