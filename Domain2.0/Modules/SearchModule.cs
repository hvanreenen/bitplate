using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;

namespace BitPlate.Domain.Modules
{
    [Persistent("Module")]
    public class SearchModule : BaseModule
    {
        public SearchModule()
            : base()
        {
            Type = "SearchModule";
            Name = "Zoek module";
            UseInNewsletter = false;
            Tags_old.Add("{textbox}");
            Tags_old.Add("{submitbutton}{/submitbutton}");
            Tags_old.Add("{submitUrl}");
            ContentSamples.Add(@"{textbox} {submitbutton}Zoek{/submitbutton}");
        }

        public override string ToString(ModeEnum mode)
        {
            if (Content == null)
            {
                Content = FirstContentSample;
            }
            //string returnHTML = Content;
            string drillDownUrl = "";
            if (mode == ModeEnum.EditPageMode)
            {
                drillDownUrl = "_bitplate/EditPage.aspx";
            }
            else
            {
                //CmsPage drillDownPage = BaseObject.GetById<CmsPage>(this.FK_DrillDownPage);
                //if (drillDownPage != null)
                //{
                //    drillDownUrl = drillDownPage.Url;
                //}
                if (DrillDownPage != null)
                {
                    drillDownUrl = this.DrillDownPage.Url;
                }
            }
            string moduleStartHTML = String.Format(@"<div id='divModule{0}' class='bitModule' data-control-type='table' >
<input type='hidden' id='hiddenDrillDownUrl{0}' value='{1}' />
", this.ID, drillDownUrl);
            if (mode == ModeEnum.EditPageMode && DrillDownPage != null)
            {
                //Guid pageid = CmsPage.GetPageIDByUrl(this.DrillDownUrl, this.Site.ID.ToString());
                moduleStartHTML += String.Format("<input type='hidden' id='hiddenDrillDownPageID{0}' value='{1}' />\r\n", this.ID, this.DrillDownPage.ID);
            }
            string moduleContentHTML = Content;
            string moduleEndHTML = " </div>";

            moduleContentHTML = moduleContentHTML.Replace("{textbox}", String.Format("<input type=text id=\"textboxSearch\" name=\"search\" onkeypress=\"javascript:BITSEARCHMODULES.checkEnter(event, '{0}');\" placeholder=\"zoek...\"/>", this.ID));
            moduleContentHTML = moduleContentHTML.Replace("{submitbutton}", String.Format("<button type=\"button\" id=\"buttonSearch\" onclick=\"javascript:BITSEARCHMODULES.navigateToResultPage('{0}');\">", this.ID));
            moduleContentHTML = moduleContentHTML.Replace("{/submitbutton}", "</button>");
            moduleContentHTML = moduleContentHTML.Replace("{submitUrl}", String.Format("javascript:BITSEARCHMODULES.navigateToResultPage('{0}');", this.ID));
            return moduleStartHTML + moduleContentHTML + moduleEndHTML;
        }
    }
}
