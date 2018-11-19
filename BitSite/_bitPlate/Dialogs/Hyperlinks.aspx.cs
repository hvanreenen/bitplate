using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitSite._bitPlate.Pages;

namespace BitSite._bitPlate.bitDetails
{
    public partial class Hyperlinks : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (PageService pageService = new PageService())
            {
                LiteralLinks.Text = pageService.GetHyperlinksAndSubFolders("");
            }
        }
    }
}