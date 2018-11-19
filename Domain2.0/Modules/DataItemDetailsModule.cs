using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM.Attributes;
using BitPlate.Domain.DataCollections;

namespace BitPlate.Domain.Modules
{
    [Persistent("Module")]
    public class DataItemDetailsModule : BaseDataModule
    {
        public DataItemDetailsModule()
            : base()
        {
            Type = "DataItemDetailsModule";
            Name = "Itemdetailsmodule";
            UseInNewsletter = false;
            Tags_old.Add("{Name}");
            Tags_old.Add("{Title}");
            Tags_old.Add("{Introduction}");
            Tags_old.Add("{Content}");
            Tags_old.Add("{Image}");
            Tags_old.Add("{Thumbnail}");
            Tags_old.Add("{DrillDownUrl}{/DrillDownUrl}");

            ContentSamples.Add(@"
<div><label>Naam: </label>{Name}</div>
<div><label>Title: </label>{Title}</div>
<div><label>Intro: </label>{Introduction}</div>
<div><label>Content: </label>{Content}</div>
<div><label>Afbeelding: </label>{Image}</div>
<div><label>Thumbnail: </label>{Thumbnail}</div>
");
        }

        public override string ToString(ModeEnum mode)
        {
            string moduleStartHTML = getModuleStartHTML(mode);
            string moduleContentHTML = Content;
            string moduleEndHTML = " </div>";

            moduleContentHTML = moduleContentHTML.Replace("{Name}", "<span data-field='Name'></span>");
            moduleContentHTML = moduleContentHTML.Replace("{Title}", "<span data-field='Title'></span>");
            moduleContentHTML = moduleContentHTML.Replace("{Introduction}", "<div data-field='Introduction'></div>");
            moduleContentHTML = moduleContentHTML.Replace("{Content}", "<div data-field='Content'></div>");
            moduleContentHTML = moduleContentHTML.Replace("{Image}", "<img src='null' alt='' data-field='ImageUrl'/>");
            moduleContentHTML = moduleContentHTML.Replace("{Thumbnail}", "<img src='null' alt='' data-field='ThumnailUrl'/>");

            return moduleStartHTML + moduleContentHTML + moduleEndHTML; 
           
        }
    }
}
