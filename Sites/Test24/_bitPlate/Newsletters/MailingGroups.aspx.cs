using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;

namespace BitSite._bitPlate.Newsletters
{
    public partial class MailingGroups : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();
            base.CheckPermissions(BitPlate.Domain.Licenses.FunctionalityEnum.Pages);

            if (SessionObject.CurrentSite.IsMultiLingual && SessionObject.CurrentLicense.AllowMultipleLanguages)
            {
                //divLanguage.Visible = true;
                //tdLangTitle.Visible = true;
                //tdLanguageField.Visible = true;
            }
        }
    }
}