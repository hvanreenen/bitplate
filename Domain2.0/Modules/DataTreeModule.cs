using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM.Attributes;

namespace BitPlate.Domain.Modules
{
    [Persistent("Module")]
    public class DataTreeModule: BaseModule
    {
        public DataTreeModule()
            : base()
        {
            Type = "DataTreeModule";
            Name = "Boom";
            UseInNewsletter = false;
            Tags_old.Add("{list}{/list}");
            Tags_old.Add("{Name}");
            Tags_old.Add("{Title}");
            Tags_old.Add("{DrillDownUrl}");
            
            ContentSamples.Add(@"
<table>
<thead>
        <tr>
            <td>
                Naam</td>
            <td>
                Title</td>
        </tr>
</thead>
<tbody>
        <tr>
            <td>
                {Name}</td>
            <td>
                {Title}</td>
        </tr>
</tbody>
</table>
");
        }

        public override string ToString(ModeEnum mode)
        {
            string returnHTML = Content;
            returnHTML = returnHTML.Replace("{}", "");
            returnHTML = returnHTML.Replace("{Name}", "<span data-field='Name'></span>");
            returnHTML = returnHTML.Replace("{Title}", "<span data-field='Title'></span>");

            
            return returnHTML;
        }
    }
}
