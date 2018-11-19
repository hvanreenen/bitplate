using BitPlate.Domain.Newsletters;
using HJORM;
using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BitPlate.Domain.Modules.Newsletter
{
    [Persistent("Module")]
    public class OptInModule : BaseModule
    {
        public OptInModule()
        { 
            ContentSamples.Add(@"<div>
                                    {SuccessTemplate}Uw emailadres is geverifieerd. U staat nu ingeschreven bij onze nieuwsbrief mailing.{/SuccessTemplate} 
                                    {ErrorTemplate}Dit verificatienummer kon niet worden geverifieerd.{/ErrorTemplate}
                                </div>
                                    ");

            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Optin instellingen", IsExternal = true, Url = "/_bitplate/EditPage/Modules/NewsletterModules/OptInModuleTab.aspx" });

        }

        public override void SetAllTags()
        {
            this._tags.Add(new Tag() { Name = "{SuccessTemplate}{/SuccessTemplate}" });
            this._tags.Add(new Tag() { Name = "{ErrorTemplate}{/ErrorTemplate}" });
            //Vervangen door input templates (baseInputModule)
            //this._tags.Add(new Tag() { Name = "{ErrorTemplate}", HasCloseTag = true, ReplaceValue = "<div runat=\"server\" id=ErrorVerificationTemplate" + this.ID.ToString("N") + ">", ReplaceValueCloseTag = "</div>" });
            //this._tags.Add(new Tag() { Name = "{SuccesTemplate}", HasCloseTag = true, ReplaceValue = "<div runat=\"server\" id=SuccessVerificationTemplate" + this.ID.ToString("N") + ">", ReplaceValueCloseTag = "</div>" });
        }

        //protected override void setNavigationActions()
        //{
        //    base.setNavigationActions();
        //    if (this.NavigationActions.Count == 0)
        //    {
        //        //this.NavigationActions.Add(new ModuleNavigationAction()
        //        //{
        //        //    Name = "{SubmitButton}",
        //        //    NavigationType = NavigationTypeEnum.ShowDetailsInModules
        //        //});
        //    }
        //}

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

        public override string Publish2(CmsPage page)
        {
            HttpContext context = HttpContext.Current;
            bool success = false;
            string html = this.Content;
            if (context.Request.QueryString.AllKeys.Contains("subscriber"))
            {
                success = this.VerificateEmailAddress(context.Request.QueryString["subscriber"]);
            }
            else
            {
                html = "Geen subscriber verificatie nummer opgegeven.";
            }
            

            if (success)
            {
                html = Regex.Replace(html, "{ErrorTemplate}(.*?){/ErrorTemplate}", "");
                html = html.Replace("{SuccessTemplate}", "").Replace("{/SuccessTemplate}", "");
            }
            else
            {
                html = Regex.Replace(html, "{SuccessTemplate}(.*?){/SuccessTemplate}", "");
                html = html.Replace("{ErrorTemplate}", "").Replace("{/ErrorTemplate}", "");
            }
            this.Content = html;
            return base.Publish2(page);
        }

        private bool VerificateEmailAddress(string id)
        {
            BaseCollection<NewsletterSubscriber> subscribers = BaseCollection<NewsletterSubscriber>.Get("ID = '" + id + "'");
            if (subscribers.Count == 1)
            {
                NewsletterSubscriber subscriber = subscribers[0];
                subscriber.Confirmed = true;
                subscriber.Save();
                this.Load();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
