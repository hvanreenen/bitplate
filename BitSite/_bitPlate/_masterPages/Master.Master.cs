using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;

namespace BitSite._bitPlate._MasterPages
{
    public partial class Master : System.Web.UI.MasterPage
    {
        private BitplateUser _user;

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (SessionObject.CurrentBitplateUser.Name != "Dummy")
            {
                this._user = SessionObject.CurrentBitplateUser;
                this.ltrlLoggedInAs.Text = this._user.Email;
            }
            if (!SessionObject.HasPermission(BitPlate.Domain.Licenses.FunctionalityEnum.Pages))
            {
                this.liPages.Visible = false;
            }
            if (!SessionObject.HasPermission(BitPlate.Domain.Licenses.FunctionalityEnum.DataCollections))
            {
                this.liDataCollections.Visible = false;
            }
            if (!SessionObject.HasPermission(BitPlate.Domain.Licenses.FunctionalityEnum.Templates))
            {
                this.liTemplates.Visible = false;
            }
        }

        protected void lbtnUitloggen_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/_bitplate/login.aspx");
        }
    }
}