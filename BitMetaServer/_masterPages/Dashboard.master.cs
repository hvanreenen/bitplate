using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitMetaServer._MasterPages
{
    public partial class Dashboard : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
                        this.LabelVersion.Text = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(); //"V2.0.0.0.9";
        }
    }
}