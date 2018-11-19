using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain.Newsletters;

namespace BitSite._bitPlate.EditPage.Modules.ContactFormModule
{
    public partial class SubscribeModuleTab : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BaseCollection<NewsletterGroup> Groups = BaseCollection<NewsletterGroup>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "'");
            
            ListItem li = new ListItem("(Keuzelijst)", "choose");
            SelectNewsGroup.Items.Add(li);

            foreach (NewsletterGroup group in Groups)
            {
                ListItem liGroup = new ListItem(group.Name, group.Name);
                SelectNewsGroup.Items.Add(liGroup);
            }
            
            //SelectNewsGroup
        }
    }
}