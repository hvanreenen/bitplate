using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM.Attributes;
namespace BitPlate.Domain.Modules
{
    [Persistent("Module")]
    public class HTMLModule: BaseModule
    {
        
        public HTMLModule(): base()
        {
            Type = "HTMLModule";
            Name = "HTML module";
            UseInNewsletter = true;
            ContentSamples.Add(@"<p>&nbsp;</p>");
        }
    }
}
