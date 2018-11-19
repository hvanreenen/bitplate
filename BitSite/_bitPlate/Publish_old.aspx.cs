using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate
{
    public partial class Publish_old : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();
            base.CheckPermissions(BitPlate.Domain.Autorisation.FunctionalityEnum.Publish);

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
            this.LabelPublishFolder.Text = SessionObject.CurrentSite.Path;
            //this.LabelPublishFolderTestEnvironment.Text = SessionObject.CurrentSite.PathTestEnvironment;
            this.checkboxTestModeImmediate.Checked = SessionObject.CurrentSite.ImmediatePublishToTestEnvironment;
        }
    }
}