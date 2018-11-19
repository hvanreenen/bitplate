using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Utils;
using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BitPlate.Domain.Modules.Auth
{
    [Persistent("Module")]
    public class LoginStatusModule : BasePostableModule
    {
        public LoginStatusModule()
            : base()
        {
            ContentSamples.Add(@"{UserTemplate}U bent ingelogd als {CompleteName} {LogoutButton}Uitloggen{/LogoutButton}{/UserTemplate}<br/>
{NoUserTemplate}U bent niet ingelogd{/NoUserTemplate}<br/>
{ErrorTemplate}Er is een fout opgetreden: {ErrorMessage}{/ErrorTemplate}
");

            ConfigPageUrl = ""; //geen extrene configpage url 
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
        }
        protected override void setNavigationActions()
        {
            base.setNavigationActions();
            if (this.NavigationActions.Count == 0)
            {
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    TagName = "{LogoutButton}",
                    NavigationType = NavigationTypeEnum.ShowDetailsInModules
                });
            }
        }

        public override void SetAllTags()
        {
            this._tags.Add(new Tag() { Name = "{ForeName}" });
            this._tags.Add(new Tag() { Name = "{NamePrefix}" });
            this._tags.Add(new Tag() { Name = "{Name}" });
            this._tags.Add(new Tag() { Name = "{CompleteName}" });
            this._tags.Add(new Tag() { Name = "{Country}" });
            this._tags.Add(new Tag() { Name = "{CompanyName}" });
            this._tags.Add(new Tag() { Name = "{Gender}" });

            this._tags.Add(new Tag() { Name = "{LogoutButton}", HasCloseTag = true, ReplaceValue = string.Format("<button type=\"submit\" id=\"LogoutButton{0:N}\" >", this.ID), ReplaceValueCloseTag = "</button>" });
            this._tags.Add(new Tag() { Name = "{LogoutLinkButton}", HasCloseTag = true, ReplaceValue = string.Format("<a id=\"LogoutLinkButton{0}\" >", this.ID.ToString("N")), ReplaceValueCloseTag = "</a>" });
            this._tags.Add(new Tag() { Name = "{UserTemplate}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{NoUserTemplate}",  HasCloseTag = true });
            base.SetAllTags();
        }



        public override string Publish2(CmsPage page)
        {
            string html = base.Publish2(page);

            BaseUser user = WebSessionHelper.CurrentSiteUser;
            if (user == null)
            {
                user = WebSessionHelper.CurrentBitplateUser;
            }
            if (user == null)
            {
                html = html.Replace("{NoUserTemplate}", "<div>");
                html = html.Replace("{/NoUserTemplate}", "</div>");
                html = base.EmptySubTemplate("{UserTemplate}", html);
            }
            else
            {
                html = html.Replace("{UserTemplate}", "<div>");
                html = html.Replace("{/UserTemplate}", "</div>");
                html = base.EmptySubTemplate("{NoUserTemplate}", html);
                html = html.Replace("{ForeName}", user.ForeName);
                html = html.Replace("{NamePrefix}", user.NamePrefix);
                html = html.Replace("{Name}", user.Name);
                html = html.Replace("{CompleteName}", user.CompleteName);
                html = html.Replace("{Country}", user.Country);
                if (user is SiteUser)
                {
                    html = html.Replace("{CompanyName}", ((SiteUser)user).CompanyName);
                }
                else
                {
                    html = html.Replace("{CompanyName}", "");
                }
                html = html.Replace("{Gender}", user.GenderString);
            }
            html = base.ConvertTags(html);

            return html;
        }

        public override PostResult HandlePost(CmsPage page, Dictionary<string, string> FormParameters)
        {
            ModuleNavigationAction navigationAction = GetNavigationActionByTagName("{LogoutButton}");
            PostResult postResult = base.getNewPostResult(navigationAction);
            
            try
            {
                //op logout button geklikt
                WebSessionHelper.CurrentSiteUser = null;
                postResult.HtmlResult = Publish2(page);
            }
            catch (Exception ex)
            {
                postResult = base.HandlePostError(ex);
            }
            return postResult;
        }

    }
}
