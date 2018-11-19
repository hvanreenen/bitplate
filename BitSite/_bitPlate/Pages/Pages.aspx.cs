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

namespace BitSite._bitPlate.Pages
{
    public partial class Pages : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();
            base.CheckPermissions(BitPlate.Domain.Licenses.FunctionalityEnum.Pages);

            if (SessionObject.CurrentSite.IsMultiLingual && SessionObject.CurrentLicense.AllowMultipleLanguages)
            {
                divLanguage.Visible = true;
                tdLangTitle.Visible = true;
                tdLanguageField.Visible = true;
            }

            if (Request.QueryString["cmd"] == "select")
            {
                tdConfig.Visible = false;
                tdDelete.Visible = false;
                tdCopy.Visible = false;
                tdSelect.Visible = true;

                htdConfig.Visible = false;
                htdDelete.Visible = false;
                htdCopy.Visible = false;
                htdSelect.Visible = true;
            }
            //else
            //{
            //    tdConfig.Visible = true;
            //    tdDelete.Visible = true;
            //    tdCopy.Visible = true;
            //    tdSelect.Visible = false;

            //    htd1.Visible = true;
            //    htd2.Visible = true;
            //    htd3.Visible = true;
            //    htd4.Visible = false;
            //}

            if (!SessionObject.HasPermission(FunctionalityEnum.PagesCreate))
            {
                liAddPage.Disabled = true;
                aAddPage.HRef = "#";
                liAddPage.Attributes["class"] = "bitItemDisabled";
                tdCopy.Visible = false;
                htdCopy.Visible = false;
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.PagesCreateFolder))
            {
                liAddFolder.Disabled = true;
                aAddFolder.HRef = "#";
                liAddFolder.Attributes["class"] = "bitItemDisabled";
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.PagesEdit))
            {
                tdEdit.Visible = false;
                htdEdit.Visible = false;
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.PagesDelete))
            {
                tdDelete.Visible = false;
                htdDelete.Visible = false;
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.PagesConfig))
            {
                tdConfig.Visible = false;
                htdConfig.Visible = false;

                //tdConfig.Disabled = true;
                //aConfig.HRef = "#";
                //tdConfig.Attributes["class"] = "bitItemDisabled";
            }
            
            BuildTemplatesDialog();
            FillDropDownFolders();

        }

        private void FillDropDownFolders()
        {
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);

            BaseCollection<CmsPageFolder> folders = BaseCollection<CmsPageFolder>.Get(where, "RelativePath");
            selectPageFolders.Items.Add(new ListItem("", Guid.Empty.ToString()));
            selectFolderParentFolders.Items.Add(new ListItem("", Guid.Empty.ToString()));
            foreach (CmsPageFolder folder in folders)
            {
                selectPageFolders.Items.Add(new ListItem(folder.RelativePath, folder.ID.ToString()));
                selectFolderParentFolders.Items.Add(new ListItem(folder.RelativePath, folder.ID.ToString()));
            }
        }

        private void BuildTemplatesDialog()
        {
            string html = "";
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);

            BaseCollection<CmsTemplate> templates = BaseCollection<CmsTemplate>.Get(where, "Name");
            foreach (CmsTemplate template in templates){
            
            html += String.Format(@"<div onclick=""javascript:BITPAGES.selectTemplate(this);"" style=""width: 150px; border: 1px solid #ddd; float: left"">
                <input type=""hidden"" class='hiddenID' value=""{0}"" />
                <input type=""hidden"" class='hiddenScripts' data-field=""Scripts"" />
                <img src=""{1}"" alt=""{2}"" title=""{2}"" /><br />
                <span>{2}</span> <strong>{3}</strong>
            </div>", template.ID, template.Screenshot,template.Name, template.LanguageCode );
    }
            LiteralLayoutTemplates.Text = html;
        }
    }
}