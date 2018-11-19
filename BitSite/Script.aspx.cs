using BitPlate.Domain;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitSite._bitPlate._bitSystem;

namespace BitSite
{
    public partial class Script : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string scriptid = "";
            if (Request.QueryString["scriptid"] != null)
            {
                scriptid = Request.QueryString["scriptid"];
                if (scriptid != "")
                {
                    Response.Clear();
                    CmsScript script  = BitCaching.FromCache<CmsScript>(scriptid);
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
                    Response.ContentType = script.ContentType;
                    Response.Write(script.Content);
                    Response.End();
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