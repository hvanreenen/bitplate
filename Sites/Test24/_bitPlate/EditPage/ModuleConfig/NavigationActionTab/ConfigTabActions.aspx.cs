using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using HJORM.Attributes;
using BitPlate.Domain;
using BitPlate.Domain.Modules;

namespace BitSite._bitPlate.EditPage.ModuleConfig.Actions
{
    public partial class ConfigTabActions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Guid PageId;
            Guid.TryParse(Request.QueryString["pageid"], out PageId);

            BaseCollection<CmsPage> Pages = BaseCollection<CmsPage>.Get("FK_Site = '" + SessionObject.CurrentSite.ID + "'");
            foreach (CmsPage page in Pages)
            {
                SelectNavigationPage_0.Items.Add(new ListItem()
                {
                    Text = page.Name,
                    Value = page.ID.ToString()
                });
            }

            
            BaseCollection<BaseModule> Modules = BaseCollection<BaseModule>.Get("FK_Page = '" + PageId.ToString() + "'");
            foreach (BaseModule module in Modules)
            {

                string Item = "<input id=\"" + module.ID.ToString() + "_checkbox" + "\" value=\"" + module.ID.ToString() + "\" type=\"checkbox\"/> <label for=\"" + module.ID.ToString() + "_checkbox" + "\">" + module.Name + " @ " + module.ContainerName + "</label><br />";
                checkboxListModules_0.InnerHtml += Item;
            }
        }
    }
}