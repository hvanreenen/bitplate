using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BitPlate.Domain;
using BitPlate.Domain.Licenses;

namespace BitSite._bitPlate.Site
{
    public partial class Publish : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();
            base.CheckPermissions(FunctionalityEnum.Publish);

            this.literalEnvironments.Text = createEnvironmentsHtml();

            if (Request.QueryString["Referer"] != null)
            {
                this.BackLink.HRef = Request.QueryString["Referer"];
                if (Request.QueryString["Template"] != null)
                {
                    this.BackLink.HRef += "?Template";
                }
                if (Request.QueryString["id"] != null)
                {
                    this.BackLink.HRef += "#" + Request.QueryString["id"];
                }
            }
            //this.LabelPublishFolder.Text = SessionObject.CurrentSite.Path;
            //this.LabelPublishFolderTestEnvironment.Text = SessionObject.CurrentSite.PathTestEnvironment;
            //this.checkboxTestModeImmediate.Checked = SessionObject.CurrentSite.ImmediatePublishToTestEnvironment;
        }

        private string createEnvironmentsHtml()
        {
            string html = @"<table>
<thead>
            <tr><td>Kies</td><td>Type</td><td>Map op Server</td><td>Url</td></tr>
</thead>
<tbody>
";
            string format = @"<tr><td><input type =""radio"" id=""radioEnvironment{0}"" {5} name=""environment"" value=""{0}"" onchange=""javascript:BITPUBLISH.changeEnvironment('{7}');""/>
<label for=""radioEnvironment{0}"">Publiceer naar {1} omgeving {6}</label>&nbsp;&nbsp;&nbsp;</td><td>{2}&nbsp;&nbsp;&nbsp;</td><td>{3}&nbsp;&nbsp;&nbsp;</td><td><a href='{4}' target='_blanc'>{4}</a></td></tr>
    ";
            string rowChecked = "checked='checked'";
            foreach (CmsSiteEnvironment environment in SessionObject.CurrentSite.Environments)
            {
                string current = "";
                if (environment.Equals(SessionObject.CurrentSite.CurrentWorkingEnvironment))
                {
                    current = "(huidige)";
                }
                //uitgezet: mag nu alle omgevingen publiceren ook de eigen: if (!environment.Equals(SessionObject.CurrentSite.CurrentWorkingEnvironment))
                {
                    string type = "Live";
                    if (environment.SiteEnvironmentType == SiteEnvironmentTypeEnum.Editable)
                    {
                        type = "Live + CMS";
                    }
                    html += String.Format(format, environment.ID, environment.Name, type, environment.Path, environment.DomainName, rowChecked, current, SessionObject.CurrentSite.CurrentWorkingEnvironment.ID);
                    //nadat eerst rij op checked is gezet, string leegmaken
                    rowChecked = "";
                }
            }
            html += @"</tbody>
                </table>";
            return html;
        }
    }
}