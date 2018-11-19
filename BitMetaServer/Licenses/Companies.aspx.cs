using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;

namespace BitMetaServer.Licenses
{
    public partial class Companies : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLogin();
            base.CheckPermissions(FunctionalityEnum.LicenseManagement, checkLicenseAlso: false);
            if (Request.QueryString["Referer"] != null)
            {
                this.BackLink.HRef = Request.QueryString["Referer"];
            }

            selectReseller.Items.Add("");
            selectResellerFilter.Items.Add(new ListItem("Alle" , ""));
            BaseCollection<Company> resellers = LicenseService.GetResellers("Name Asc", "");
            foreach (Company reseller in resellers)
            {
                selectResellerFilter.Items.Add(new ListItem(reseller.Name, reseller.ID.ToString()));
                selectReseller.Items.Add(new ListItem(reseller.Name, reseller.ID.ToString()));
            }
        }
    }
}