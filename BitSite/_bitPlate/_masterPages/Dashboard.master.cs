using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate._MasterPages
{
    public partial class Dashboard : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.LabelLastBackup.Text = "Nog niet geimplementeerd";
            if (SessionObject.CurrentLicense != null)
            {
                this.LabelLicense.Text = SessionObject.CurrentLicense.Name;
            }
            else
            {
                this.LabelLicense.Text = "<B>Geen geldige licentie</B>";
                this.LabelLicense.Style.Add("text-decoration", "blink");
            }
            this.LabelVersion.Text = "V" + SessionObject.CurrentLicense.BitplateVersion; // // "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(); //"V2.0.0.0.9";
        }
    }
}