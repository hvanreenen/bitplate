using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite
{
    public partial class EditPage : System.Web.UI.Page
    {
        public string PageID { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["pageid"] != null)
            {
                PageID = Request.QueryString["pageid"];
            }
        }
    }
}