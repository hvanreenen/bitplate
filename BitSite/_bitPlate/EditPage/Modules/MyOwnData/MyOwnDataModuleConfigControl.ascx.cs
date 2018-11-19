using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitSite._bitPlate._bitModules;

namespace BitSite._bitPlate._bitModules.MyOwnData
{
    public partial class MyOwnDataModuleConfigControl : BaseConfigModuleUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override void DataBind()
        {
            TextBoxID.Text = module.ID.ToString("N");
            LabelModId.Text = module.ID.ToString("N");
            TextBoxName.Text = module.Name;
        }

        protected override void ButtonSave_Click(object sender, EventArgs e)
        {
            base.LoadModule(this.ModuleID);
            module.Name = TextBoxName.Text;
            base.ButtonSave_Click(sender, e);
        }

        

        protected void Button1_Click(object sender, EventArgs e)
        {
            TextBoxName.Text = "ashdg";
        }
        
    }
}