using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.Design;

namespace BitSite._bitPlate.RewriteForm
{
    public class GenericControlDesigner : ControlDesigner
    {
        public GenericControlDesigner()
            : base()
        {
            
        }

        protected override string GetErrorDesignTimeHtml(Exception e)
        {
            // Write the error message text in red, bold.
            string errorRendering =
                "<span style=\"font-weight:bold; color:Red; \">hello" +
                e.Message + "<br>" + e.StackTrace + "</span>";

            return CreatePlaceHolderDesignTimeHtml(errorRendering);

        }
    }
}
