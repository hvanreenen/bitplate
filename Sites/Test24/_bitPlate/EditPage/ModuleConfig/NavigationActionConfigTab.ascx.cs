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
namespace BitSite._bitPlate.EditPage.ModuleConfig
{
    public partial class NavigationActionConfigTab : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Guid PageId;
            Guid.TryParse(Request.QueryString["pageid"], out PageId);

            BaseCollection<CmsPage> Pages = BaseCollection<CmsPage>.Get("FK_Site = '" + SessionObject.CurrentSite.ID + "'", "RelativeUrl, Name");
            foreach (CmsPage page in Pages)
            {
                SelectNavigationPage_0.Items.Add(new ListItem()
                {
                    Text = page.RelativeUrl,
                    Value = page.ID.ToString()
                });
            }

            //MODULES CHECKBOX wordt in javascript gedaan
            //string where = " EXISTS (SELECT * FROM moduleperpage where moduleperpage.FK_Module = module.ID AND moduleperpage.FK_Page = '" + PageId.ToString() + "')";

            //BaseCollection<BaseModule> Modules = BaseCollection<BaseModule>.Get(where, "ContainerName, Name");
            //foreach (BaseModule module in Modules)
            //{
            //    string Item = "<input id=\"" + module.ID.ToString() + "_checkbox" + "\" value=\"" + module.ID.ToString() + "\" type=\"checkbox\"/> <label for=\"" + module.ID.ToString() + "_checkbox" + "\">" + module.Name + " @ " + module.ContainerName + "</label><br/>";
            //    BaseModule convertedModule = module.ConvertToType();
            //    if (!(convertedModule is IRefreshableModule))
            //    {
            //        //niet beschikbaar
            //        Item = "<input disable=\"disabled\" id=\"" + module.ID.ToString() + "_checkbox" + "\" type=\"checkbox\"/> <label for=\"" + module.ID.ToString() + "_checkbox" + "\">" + module.Name + " @ " + module.ContainerName + " (niet beschikbaar)</label><br/>";
            //    }
            //    Item += @"<div style=""displayxxx:none"" class=""bitModuleIdField"">#bitModule" + module.ID.ToString() + "</div>";
            //    //checkboxListModules_0.InnerHtml += Item;
            //}
        }
    }
}