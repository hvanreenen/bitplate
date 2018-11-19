using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM.Attributes;

namespace BitPlate.Domain.Modules
{
    [Persistent("Module")]
    public class DataGroupsModule : BaseDataModule
    {
        public DataGroupsModule()
            : base()
        {
            Type = "DataGroupsModule";
            Name = "Groepen lijst module";
            UseInNewsletter = true;
            Tags_old.Add("{list}{/list}");
            Tags_old.Add("{Name}");
            Tags_old.Add("{Title}");
            Tags_old.Add("{Introduction}");
            Tags_old.Add("{Content}");
            Tags_old.Add("{Thumbnail}");
            Tags_old.Add("{Image}");
            Tags_old.Add("{DrillDownUrl}{/DrillDownUrl}");

            ContentSamples.Add(@"
<table>
<thead>
        <tr>
            <td>
                </td>
            <td>
                Naam</td>
            <td>
                Title</td>
        </tr>
</thead>
<tbody>
        <tr>
            <td>
                {Thumbnail}
                </td>
            <td>
                {DrillDownUrl}{Name}{/DrillDownUrl}</td>
            <td>
                {Title}</td>
        </tr>
</tbody>
</table>
");
            ContentSamples.Add(@"{list}
{DrillDownUrl}<div style='border: 1px solid #666; margin:20px'>
{Thumbnail}<br/>
{Title}<br/>
{Introduction}<br/>
</div>{/DrillDownUrl}
{/list}
");
        }

        public override string ToString(ModeEnum mode)
        {
            string moduleStartHTML = getModuleStartHTML(mode);
            string moduleContentHTML = Content;
            string moduleEndHTML = " </div>";

            string drillDownLink = getDrillDownLink(mode);

            moduleContentHTML = moduleContentHTML.Replace("{list}", "<div data-control-type='list'>");
            moduleContentHTML = moduleContentHTML.Replace("{/list}", "</div>");
            moduleContentHTML = moduleContentHTML.Replace("{Name}", "<span data-field='Name'></span>");
            moduleContentHTML = moduleContentHTML.Replace("{Title}", "<span data-field='Title'></span>");
            moduleContentHTML = moduleContentHTML.Replace("{Introduction}", "<div data-field='Introduction'></div>");
            moduleContentHTML = moduleContentHTML.Replace("{Content}", "<div data-field='Content'></div>");
            moduleContentHTML = moduleContentHTML.Replace("{Image}", "<img data-field='ImageUrl' alt='' src=''/>");
            moduleContentHTML = moduleContentHTML.Replace("{Thumbnail}", "<img data-field='ThumbnailUrl' alt='' src=''/>");
            moduleContentHTML = moduleContentHTML.Replace("{DrillDownUrl}", drillDownLink);
            moduleContentHTML = moduleContentHTML.Replace("{/DrillDownUrl}", "</a>");

            return moduleStartHTML + moduleContentHTML + moduleEndHTML;
        }

        
    }
}
