using BitSite._bitPlate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate.EditPage
{
    public partial class EditPage : BasePage
    {
        public string PageID { get; set; }
        public string NewsletterID { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckSite();
            base.CheckLogin();

            if (Request.QueryString["pageid"] != null)
            {
                PageID = Request.QueryString["pageid"];
            }
            if (Request.QueryString["newsletterid"] != null)
            {
                NewsletterID = Request.QueryString["newsletterid"];
                this.EditPageMenu2.InNewslettersMode = true;
            }
        }
    }
}