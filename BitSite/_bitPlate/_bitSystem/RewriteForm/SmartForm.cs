using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.HtmlControls;

namespace BitSite._bitPlate.RewriteForm
{

    [ToolboxData("<{0}:RewriteForm runat=server></{0}:RewriteForm>")]
    [Designer("BitSite._bitPlate.RewriteForm.GenericControlDesigner")]
    public class SmartForm : HtmlForm
    {
        public SmartForm()
            : base()
        {
        }

        private string action = null;
        private bool removeAction = false;

        [DefaultValue(null), Category("Behavior"),
        EditorBrowsable(EditorBrowsableState.Always)]
        [Description("The action attribute of the form tag. This is for assignment only and will not return the new value.")]
        public string Action
        {
            get { return action; }
            set { action = value; }
        }

        [DefaultValue(false), Category("Behavior"),
        EditorBrowsable(EditorBrowsableState.Always)]
        [Description("If true the control will remove the action attribute of the form tag.")]
        public bool RemoveAction
        {
            get { return removeAction; }
            set { removeAction = value; }
        }

        protected override void RenderAttributes(HtmlTextWriter writer)
        {
            SelectiveHtmlTextWriter customWriter = new SelectiveHtmlTextWriter(writer);
            customWriter.WritingAttribute += 
                new SubstituteValueEventHandler(customWriter_WritingAttribute);
            base.RenderAttributes(customWriter);
        }

        void customWriter_WritingAttribute(object sender, SubstituteValueEventArgs e)
        {
            if (e.Name == "action")
            {
                //use action attribute only if one is explicitly provided
                if (action != null)
                    e.NewValue = action;
                else if (removeAction == true)
                    e.Cancel = true;
                else
                {
                    e.NewValue = Context.Request.RawUrl;
                }
            }
        }
    }
}
