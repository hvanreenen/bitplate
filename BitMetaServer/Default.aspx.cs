using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain;
using BitPlate.Domain.DataCollections;
using HJORM;

namespace BitMetaServer
{
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLogin();
            BuildFrameRecentChangeItems();
        }

        public void BuildFrameRecentChangeItems()
        {
            BaseCollection<BitPlate.Domain.Licenses.License> licenseList = BaseCollection<BitPlate.Domain.Licenses.License>.Get("", "ModifiedDate DESC", 1, 5);

            RecentLicensesList.Text = "<ul>";
            foreach (BitPlate.Domain.Licenses.License lic in licenseList)
            {
                RecentLicensesList.Text += String.Format("<li><a href=\"/Licenses/Licenses.aspx\"><div class=\"lastChangedItemName bitTableColumnEllipsis\"  title=\"{0}\">{0}</div><div class=\"lastChangedItemDate\">{1:dd-MM-yy HH:mm}</div></a></li>", lic.Name, lic.ModifiedDate);
            }
            RecentLicensesList.Text += "</ul>";

        }
    }
}