using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;

namespace BitSite._bitPlate.Menus
{
    public partial class Menus : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();

            base.CheckPermissions(FunctionalityEnum.MenuManager);

            if (SessionObject.CurrentSite.IsMultiLingual && SessionObject.CurrentLicense.AllowMultipleLanguages)
            {
                divLanguage.Visible = true;
                tdLangTitle.Visible = true;
                tdLanguageField.Visible = true;
                //vullen dropdown met talen
                //int selectedIndex = 0;
                //int index = 0;
                //ListItem li = new ListItem("***Taal per veld instellen***", "none");
                //dropdownLanguages.Items.Add(li);
                foreach (CmsLanguage language in SessionObject.CurrentSite.Languages)
                {
                    ListItem listItem = new ListItem(language.Name, language.LanguageCode);
                    if (language.LanguageCode == SessionObject.CurrentSite.DefaultLanguage)
                    {
                        listItem.Attributes.Add("data-cms-default-language", "true");
                    }

                    dropdownLanguages.Items.Add(listItem);
                    //index++;
                }
                //dropdownLanguages.SelectedIndex = selectedIndex;


            }

        }
    }
}