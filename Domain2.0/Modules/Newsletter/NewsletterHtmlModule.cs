using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules.Newsletter
{
    enum ModeEnum { OnlineMode, NewsletterMode };

    [Persistent("Module")]
    public class NewsletterHtmlModule : BaseModule
    {
        public NewsletterHtmlModule()
        {
            ContentSamples.Add(@"<div>
                                    {NewsletterModeTemplate}Plaats hier content/tags welke zichtbaar moeten zijn in een nieuwsbrief.{/NewsletterModeTemplate} 
                                    {OnlineModeTemplate}Plaatst hier de content/tags welke te zien is als de nieuwsbrief online wordt bekeken.{/OnlineModeTemplate}
                                </div>
                                    ");
        }

        public override void SetAllTags()
        {
            this._tags.Add(new Tag() { Name = "{NewsletterModeTemplate}{/NewsletterModeTemplate}" });
            this._tags.Add(new Tag() { Name = "{OnlineModeTemplate}{/OnlineModeTemplate}" });
            this._tags.Add(new Tag() { Name = "[NAME]" });
            this._tags.Add(new Tag() { Name = "[FORENAME]" });
            this._tags.Add(new Tag() { Name = "[NAMEPREFIX]" });
            this._tags.Add(new Tag() { Name = "[COMPANYNAME]" });
            this._tags.Add(new Tag() { Name = "[DATE]" });
            this._tags.Add(new Tag() { Name = "[SEXE]" });
            this._tags.Add(new Tag() { Name = "[BEGINNING]" });
            this._tags.Add(new Tag() { Name = "[USERCODE]" });
            this._tags.Add(new Tag() { Name = "[LIVEURL]" });
            this._tags.Add(new Tag() { Name = "[UNSUBSCRIBEURL]" });

            //Vervangen door input templates (baseInputModule)
            //this._tags.Add(new Tag() { Name = "{ErrorTemplate}", HasCloseTag = true, ReplaceValue = "<div runat=\"server\" id=ErrorVerificationTemplate" + this.ID.ToString("N") + ">", ReplaceValueCloseTag = "</div>" });
            //this._tags.Add(new Tag() { Name = "{SuccesTemplate}", HasCloseTag = true, ReplaceValue = "<div runat=\"server\" id=SuccessVerificationTemplate" + this.ID.ToString("N") + ">", ReplaceValueCloseTag = "</div>" });
        }

        public override string ConvertTags(string html)
        {
            foreach (Tag tag in this.GetAllTags())
            {
                html = html.Replace(tag.Name, tag.ReplaceValue);
                if (tag.HasCloseTag)
                {
                    html = html.Replace(tag.CloseTag, tag.ReplaceValueCloseTag);
                }
            }
            return html;
        }

        //public string Publish2(ModeEnum onlineMode)
        //{
        //}

        public override string Publish2(CmsPage page)
        {
            
            return base.Publish2(page);
        }
    }
}
