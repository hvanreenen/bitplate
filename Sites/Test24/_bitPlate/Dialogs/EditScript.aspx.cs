using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;


namespace BitSite._bitPlate.bitDetails
{
    public partial class EditScript : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string scriptContent = "";
            int rows = 0;
            int cols = 80;
            if (Request.QueryString["ScriptUrl"] != null)
            {
                string path = SessionObject.CurrentSite.Path;
                string scriptUrl = Request.QueryString["ScriptUrl"];
                scriptUrl = scriptUrl.Replace("/", "\\");
                
                //scriptUrl = scriptUrl.Substring(0, scriptUrl.IndexOf("?"));
                string[] lines = File.ReadAllLines(path + "\\" + scriptUrl);
                rows = lines.Length;

                StringBuilder builder = new StringBuilder();
                foreach (string value in lines)
                {
                    builder.Append(value);
                    builder.Append("\r\n");
                    if (value.Length > 80)
                    {
                        rows += value.Length / cols;
                    }
                }
                scriptContent = builder.ToString();

                if (rows < 24)
                {
                    rows = 24;
                }
                
            }

            string html = String.Format("<textarea id='bitTextAreaScript' name='script' rows='{1}' cols='280'>{0}</textarea>", scriptContent, rows);
            
            LiteralTextBoxScript.Text = html;
        }
    }
}