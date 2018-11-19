using BitPlate.Domain.Licenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate.Dialogs
{
    public partial class AutorisationTab : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SessionObject.HasPermission(FunctionalityEnum.UserRights))
            {
                this.PanelMain.Visible = false;
                this.LiteralMsg.Text = "U heeft geen rechten om autorisatie in te stellen. Vraag de websitebeheerder voor meer toegang.";
            }
        }
    }
}