using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain;

using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;

namespace BitSite._bitPlate.Backup
{
    public partial class Backups : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();
            base.CheckPermissions(BitPlate.Domain.Licenses.FunctionalityEnum.Backups);
        }
    }
}