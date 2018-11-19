using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate.Dialogs
{
    public partial class ConfigCurrentUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (string theme in  Directory.GetDirectories(Server.MapPath("") + "..\\..\\_themes\\")) {
                DirectoryInfo di = new DirectoryInfo(theme);
                this.selectThema.Items.Add(di.Name);
            }
        }
    }
}