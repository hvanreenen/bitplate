using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;

namespace BitSite._bitPlate.Scripts
{
    public partial class Scripts : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();

            if (Request.QueryString["type"] == "css")
            {
                base.CheckPermissions(BitPlate.Domain.Licenses.FunctionalityEnum.Stylesheets);

                if (!SessionObject.HasPermission(FunctionalityEnum.StylesheetsCreate))
                {
                    liAddScript.Disabled = true;
                    aAddScript.HRef = "#";
                    tdScriptCopy.Disabled = true;
                    aScriptCopy.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.StylesheetsDelete))
                {
                    tdScriptRemove.Disabled = true;
                    aScriptRemove.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.StylesheetsConfig))
                {
                    tdScriptConfig.Disabled = true;
                    aScriptConfig.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.StylesheetsEdit))
                {
                    //Doe iets
                }

                StylesheetProperties.Visible = true;
            }
            else
            {
                base.CheckPermissions(BitPlate.Domain.Licenses.FunctionalityEnum.Scripts);


                if (!SessionObject.HasPermission(FunctionalityEnum.ScriptsCreate))
                {
                    liAddScript.Disabled = true;
                    aAddScript.HRef = "#";
                    tdScriptCopy.Disabled = true;
                    aScriptCopy.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.ScriptsDelete))
                {
                    tdScriptRemove.Disabled = true;
                    aScriptRemove.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.ScriptsConfig))
                {
                    tdScriptConfig.Disabled = true;
                    aScriptConfig.HRef = "#";
                }
                if (!SessionObject.HasPermission(FunctionalityEnum.ScriptsEdit))
                {
                    //Doe iets
                }

                StylesheetProperties.Visible = false;
            }
        }
    }
}