using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM.Attributes;
using HJORM;
namespace BitPlate.Domain.Modules
{
    [Persistent("Module")]
    public class PopupModule : BaseContainerModule
    {
        
        public PopupModule()
            : base()
        {
            Type = "PopupModule";
            Name = "Popup module";
            UseInNewsletter = false;
            ContentSamples.Add(@"<div class='bitContainer'>&nbsp;</div>");
        }

        

        public override string ToString(ModeEnum mode)
        {
            string content = string.Format("Popup: <br/><div style='margin:10px;' class='bitContainer' id='bitContainerModule{0}'>[CONTENT]</div>", this.ID);
            
            //content += "</div>";
            return content;
        }
    }
}
