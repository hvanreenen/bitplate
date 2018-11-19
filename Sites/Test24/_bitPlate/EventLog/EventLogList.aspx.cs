using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;
using BitSite._bitPlate._MasterPages;
using BitPlate.Domain.Licenses;

namespace BitSite._bitPlate.EventLog
{
    public partial class EventLogList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();
            base.CheckPermissions(FunctionalityEnum.FileManager);
        }
    }
}