using BitPlate.Domain;
using BitPlate.Domain.Menu;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate.EditPage.Modules.Menu
{
    public partial class MenuModuleTab : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);
            BaseCollection<CmsMenu> menus = BaseCollection<CmsMenu>.Get(where, "Name");
            foreach (CmsMenu menu in menus) { 
                selectMenu.Items.Add(new ListItem(menu.Name + "(" + menu.TypeString + ")", menu.ID.ToString()));

                string html = "<div class='menuScripts' id='div" + menu.ID.ToString("N") + "'>";
                foreach (CmsScript script in menu.Scripts)
                {
                    html += "<a href=\"javascript:BITEDITPAGE.editScript('" + script.ID.ToString() + "');\">" + script.Url + "</a><br/>";
                }
                html += "</div><br/>";

                LiteralScripts.Text += html;

            }
        }
    }
}