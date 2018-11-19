using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;
namespace BitMetaServer.Licenses
{
    public partial class Restore : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //base.CheckLogin();
            //base.CheckPermissions(FunctionalityEnum.LicenseManagement, checkLicenseAlso: false);

        }
    }
}