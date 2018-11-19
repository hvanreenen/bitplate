using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;

namespace BitMetaServer.News
{
    public partial class News : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLogin();

        }
    }
}