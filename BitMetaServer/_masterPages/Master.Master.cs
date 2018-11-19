using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;

namespace BitMetaServer._MasterPages
{
    public partial class Master : System.Web.UI.MasterPage
    {
        private MetaServerUser _user;

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (SessionObject.CurrentBitplateUser.Name != "Dummy")
            {
                this._user = SessionObject.CurrentUser;
                if (this._user != null)
                {
                    this.ltrlLoggedInAs.Text = this._user.Email;
                }
                
            }
        }

        protected void lbtnUitloggen_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/login.aspx");
        }
    }
}