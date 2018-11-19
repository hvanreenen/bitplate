using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Utils;
using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules.Auth
{
    [Persistent("Module")]
    public class LoginModule : BasePostableModule
    {
        public LoginModule(): base()
        {
            ContentSamples.Add(@"
<fieldset>
    <legend>Login</legend>
    <table>
        <tr>
            <td>Gebruikersnaam:</td>
            <td>{UsernameTextbox}</td>
        
        </tr>
        <tr>
            <td>Wachtwoord:</td>
            <td>{PasswordTextbox}</td>
        </tr>
        <tr>
            <td></td>
            <td>{LoginButton}Login{/LoginButton}</td>
        </tr>
    </table>
    {ErrorTemplate}{ErrorMessage}{/ErrorTemplate}
    
</fieldset>");
            ConfigPageUrl = ""; //geen extrene configpage url 
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });

        }

        protected override void setNavigationActions()
        {
            base.setNavigationActions();
            if (this.NavigationActions.Count == 0)
            {
                //set DefaulActions
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    TagName = "{LoginButton}",
                    Module = this,
                    Site = this.Site,
                    NavigationType = NavigationTypeEnum.NavigateToPage
                });
            }
        }

        public override void SetAllTags()
        {
            this._tags.Add(new Tag() { Name = "{UsernameTextbox}", ReplaceValue = string.Format("<input type=\"text\" id=\"bitUsernameTextbox{0}\"  name=\"bitUsernameTextbox\"/>", this.ID.ToString("N")) });
            this._tags.Add(new Tag() { Name = "{PasswordTextbox}", ReplaceValue = string.Format("<input type=\"password\" id=\"bitPasswordTextbox{0}\" name=\"bitPasswordTextbox\" />", this.ID.ToString("N")) });
            this._tags.Add(new Tag() { Name = "{LoginButton}", HasCloseTag = true, ReplaceValue = string.Format("<button type=\"submit\" id=\"bitLoginButton{0:N}\" >", this.ID), ReplaceValueCloseTag = "</button>" });
            this._tags.Add(new Tag() { Name = "{LoginLinkButton}", HasCloseTag = true, ReplaceValue = string.Format("<a href=\"javascript:void(0)\" onclick=\"BITSITESCRIPT.submitPostableModule('{0}');\" id=\"bitLoginLinkButton{0:N}\" >", this.ID), ReplaceValueCloseTag = "</button>" });
            
            //this._tags.Add(new Tag() { Name = "{MessageLabel}", ReplaceValue = string.Format("<span id=\"bitMessageLabel{0:N}\"></span>", this.ID) });
            base.SetAllTags();
        }


        public override string Publish2(CmsPage page)
        {
            string html= base.Publish2(page);
            html = ConvertTags(html);
            return html;
        }

        public override PostResult HandlePost(CmsPage page, Dictionary<string, string> FormParameters)
        {
            ModuleNavigationAction navigationAction = GetNavigationActionByTagName("{LoginButton}");
            PostResult result = base.getNewPostResult(navigationAction);
            string userName = FormParameters.ContainsKey("bitUsernameTextbox") ? FormParameters["bitUsernameTextbox"] : "";
            string password = FormParameters.ContainsKey("bitPasswordTextbox") ? FormParameters["bitPasswordTextbox"] : "";

            SiteUser user = SiteUser.Login(userName, password);
            if (user != null)
            {
                WebSessionHelper.CurrentSiteUser = user;
                result.HtmlResult = Publish2(page);
            }
            else
            {
                result = new PostResult() { ErrorMessage = "Kon niet aanmelden." };
            }

            return result;
        }

        

    }
}
