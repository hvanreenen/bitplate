using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;
using HJORM;

namespace BitMetaServer.Auth
{
    public partial class MultiSiteUsers : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLogin();
            literalSitesList.Text = BuildSitesList();

        }

        private string BuildSitesList()
        {
            string format = @"          <li class=""ui-state-default bitScript"" style=""margin-top: 2px; margin-bottom: 2px;padding: 5px"">
                <a href=""javascript:BITMULTISITEUSERS.addSitePerUser('{0}', '{1}');"">
                    <input type=""hidden"" value=""{0}"" />
                    <span>{1}</span><span class=""remove-script btn-remove-script""></span>
                </a>
            </li>
";
            string returnValue = "";
            
            BaseCollection<LicensedEnvironment> sites = BaseCollection<LicensedEnvironment>.Get("", "Name");
            foreach (LicensedEnvironment site in sites)
            {
                returnValue += String.Format(format, site.ID, site.DomainName);
            }
            return returnValue;

        }



    }
}