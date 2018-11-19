using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;

namespace BitSite._bitPlate.Templates
{
    public partial class Templates : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();

            base.CheckPermissions(FunctionalityEnum.Templates);

            if (SessionObject.CurrentSite.IsMultiLingual && SessionObject.CurrentLicense.AllowMultipleLanguages)
            {
                divLanguage.Visible = true;
                tdLangTitle.Visible = true;
                tdLanguageField.Visible = true;
                //vullen dropdown met talen
                //int selectedIndex = 0;
                //int index = 0;
                foreach (CmsLanguage language in SessionObject.CurrentSite.Languages)
                {
                    ListItem li = new ListItem(language.Name, language.LanguageCode);
                    if (language.LanguageCode == SessionObject.CurrentSite.DefaultLanguage)
                    {
                        li.Attributes.Add("data-cms-default-language", "true");
                    }
                    
                    dropdownLanguages.Items.Add(li);
                    //index++;
                }
                //dropdownLanguages.SelectedIndex = selectedIndex;
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.TemplatesConfig))
            {
                tdTemplateConfig.Disabled = true;
                aTemplateConfig.HRef = "#";
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.TemplatesCreate))
            {
                liAddTemplate.Disabled = true;
                aAddTemplate.HRef = "#";
                tdTemplateCopy.Disabled = true;
                aTemplateCopy.HRef = "#";
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.TemplatesDelete))
            {
                tdTemplateRemove.Disabled = true;
                aTemplateRemove.HRef = "#";
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.TemplatesEdit))
            {
                tdTemplateEdit.Disabled = true;
                aTemplateEdit.HRef = "#";
            }

        }
    }
}