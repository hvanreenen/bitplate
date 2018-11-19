using BitPlate.Domain;
using BitPlate.Domain.Modules;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate.EditPage.ModuleConfig
{
    public partial class GeneralConfigTab : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string moduleId = Request.QueryString["moduleid"];
            //Guid moduleID = Guid.Parse(moduleId);
            //BaseModule module = BaseObject.GetById<BaseModule>(moduleID);
            modulePerPageSelect.Multiple = true;
            //modulePerPageSelect
            foreach (CmsPage page in SessionObject.CurrentSite.Pages)
            {
                ListItem li;
                if (page.Folder != null)
                {
                    li = new ListItem("/" + page.Folder.RelativePath + "/" + page.Name, page.ID.ToString());
                }
                else
                {
                    li = new ListItem("/" + page.Name, page.ID.ToString());
                }
                //if (module.Pages.Contains(page))
                //{
                //    li.Selected = true;
                //}
                modulePerPageSelect.Items.Add(li);
            }
        }
    }
}