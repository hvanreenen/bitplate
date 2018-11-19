using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BitPlate.Domain.Autorisation;

namespace BitSite._bitPlate.Autorisation
{
    public partial class Sites : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLogin();
            //base.CheckPermissions(FunctionalityEnum.ServerSites, checkLicenseAlso: false);
        }
    }
}