using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitMetaServer.Dialogs
{
    public partial class ConfigCurrentUser : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLogin();
            foreach (string theme in  Directory.GetDirectories(Server.MapPath("") + "..\\..\\_themes\\")) {
                DirectoryInfo di = new DirectoryInfo(theme);
                this.selectThema.Items.Add(di.Name);
            }
        }
    }
}