using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM.Attributes;
namespace BitPlate.Domain.Modules
{
    [Persistent("Module")]
    public class SearchResultsModule : BaseModule
    {
        public SearchResultsModule()
        {
            Type = "SearchResultsModule";
            Name = "Zoekresultaten module";
            UseInNewsletter = false;
            Tags_old.Add("{list}{/list}");
            Tags_old.Add("{Name}");
            Tags_old.Add("{Type}");
            Tags_old.Add("{Title}");
            Tags_old.Add("{Url}");
            Tags_old.Add("{/Url}");

            ContentSamples.Add(@"
<table>
<thead>
        <tr>
            <td>
                Naam</td>
            <td>
                Type</td>
            <td>
                Inhoud</td>
        </tr>
</thead>
<tbody>
        <tr>
            <td>
                {DrillDownUrl}{Name}{/DrillDownUrl}</td>
            <td>
                {Type}</td>
            <td>
                {Content}</td>
        </tr>
</tbody>
</table>
");
        }

        public override string ToString(ModeEnum mode)
        {
            
            //string drillDownUrl = this.DrillDownUrl;
            //if (mode == ModeEnum.EditPageMode)
            //{
            //    Guid pageid = CmsPage.GetPageIDByUrl(drillDownUrl, this.Site.ID.ToString());
            //    drillDownUrl = String.Format("EditPage.aspx#{0}", this.Page.ID);
            //}


            string moduleStartHTML = String.Format(@"<div id='divSearchResultsModule' class='bitModule' data-control-type='table' >
<input type='hidden' id='hiddenModuleType{0}' value='{1}' />", this.ID, this.Type);

            string moduleEndHTML = " </div>";

            string moduleContentHTML = Content;


            moduleContentHTML = moduleContentHTML.Replace("{}", "");
            moduleContentHTML = moduleContentHTML.Replace("{Name}", "<span data-field='Name'></span>");
            moduleContentHTML = moduleContentHTML.Replace("{Title}", "<span data-field='Title'></span>");
            moduleContentHTML = moduleContentHTML.Replace("{Type}", "<span data-field='Type'></span>");
            moduleContentHTML = moduleContentHTML.Replace("{Content}", "<span data-field='ContentFirst80Chars'></span>");
            moduleContentHTML = moduleContentHTML.Replace("{DrillDownUrl}", "<a data-field='Url'>");
            moduleContentHTML = moduleContentHTML.Replace("{/DrillDownUrl}", "</a>");

            return moduleStartHTML + moduleContentHTML + moduleEndHTML;
        }
    }
}
