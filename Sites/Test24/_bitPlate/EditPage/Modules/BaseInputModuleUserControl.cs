using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate.EditPage.Modules
{
    public class BaseInputModuleUserControl : BaseModuleUserControl
    {
        public Panel ErrorPanel;
        public Label ErrorLabel;
        //Soms gaat er ook iets goed....
        public Panel SuccessPanel;
        public Label SuccessLabel;

        protected override void Load(object sender, EventArgs e)
        {
            //Find panels and labels
            this.ErrorLabel = (Label)this.FindControl("ErrorMessage" + this.ModuleID.ToString("N"));
            if (this.ErrorLabel == null) this.ErrorLabel = new Label();
            this.ErrorPanel = (Panel)this.FindControl("ErrorTemplate" + this.ModuleID.ToString("N"));
            if (this.ErrorPanel == null) this.ErrorPanel = new Panel();

            this.SuccessLabel = (Label)this.FindControl("SuccessMessage" + this.ModuleID.ToString("N"));
            if (this.SuccessLabel == null) this.SuccessLabel = new Label();
            this.SuccessPanel = (Panel)this.FindControl("SuccessTemplate" + this.ModuleID.ToString("N"));
            if (this.SuccessPanel == null) this.SuccessPanel = new Panel();

            //Make them invisable
            this.ErrorPanel.Visible = false;
            this.SuccessPanel.Visible = false;  

            base.Load(sender, e);
        }
    }
}