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
    public class MyProfileModule : BasePostableModule
    {

        public MyProfileModule()
            : base()
        {

            ContentSamples.Add(@"{MyProfileForm}
<h2>Eigen gegevens wijzigen</h2>

<div>Emailadres: {TextboxEmail}</div>
<div>Voornaam: {TextboxForeName}</div>
<div>Tussenvoegsel: {TextboxNamePrefix}</div>
<div>Achternaam: {TextboxName}</div>
<div>Geslacht: {RadioSexeMale}Man {RadioSexeFemale}Vrouw</div>
<div>Geboortedatum: {TextboxBirthDate}</div>
<div>&nbsp;</div>
<div>Huidige wachtwoord: {TextboxBirthDate}</div>
<div>Nieuwe wachtwoord: {TextboxBirthDate}</div>
<div>Herhaal nieuwe wachtwoord: {TextboxBirthDate}</div>
<div>&nbsp;</div>
<div>{SubmitButton}Wijzigen{/SubmitButton}</div>
{/MyProfileForm} 
{ErrorTemplate}Kan gegevens niet opslaan: <br />{ErrorMessage}{/ErrorTemplate}
{SaveSuccessTemplate}De gegevens zijn opgeslagen.{/SaveSuccessTemplate}
</div>");

            ConfigPageUrl = ""; //geen extrene configpage url 
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
        }

        public override void SetAllTags()
        {
            this._tags.Add(new Tag() { Name = "{TextboxName}" });
            this._tags.Add(new Tag() { Name = "{TextboxForeName}" });
            this._tags.Add(new Tag() { Name = "{TextboxNamePrefix}" });
            this._tags.Add(new Tag() { Name = "{TextboxEmail}" });
            this._tags.Add(new Tag() { Name = "{CurrentPassword}" });
            this._tags.Add(new Tag() { Name = "{NewPassword}" });
            this._tags.Add(new Tag() { Name = "{RetypePassword}" });
            this._tags.Add(new Tag() { Name = "{RadioSexeMale}" });
            this._tags.Add(new Tag() { Name = "{RadioSexeFemale}" });
            this._tags.Add(new Tag() { Name = "{TextboxAddress}" });
            this._tags.Add(new Tag() { Name = "{TextboxPostalCode}" });
            this._tags.Add(new Tag() { Name = "{TextboxCity}" });
            this._tags.Add(new Tag() { Name = "{TextboxBirthDate}" });
            this._tags.Add(new Tag() { Name = "{TextboxCountry}" });
            this._tags.Add(new Tag() { Name = "{SubmitButton}", HasCloseTag = true, ReplaceValue = string.Format("<button type=\"submit\" id=\"SubmitButton{0}\" >", this.ID.ToString("N")), ReplaceValueCloseTag = "</button>" });
            this._tags.Add(new Tag() { Name = "{SubmitLinkButton}", HasCloseTag = true, ReplaceValue = string.Format("<a id=\"SubmitLinkButton{0}\" >", this.ID.ToString("N")), ReplaceValueCloseTag = "</a>" });
            this._tags.Add(new Tag() { Name = "{MyProfileForm}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{SaveSuccesTemplate}", HasCloseTag = true });
            base.SetAllTags();
        }

        protected override void setNavigationActions()
        {
            base.setNavigationActions();
            if (this.NavigationActions.Count == 0)
            {
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    Name = "{SubmitButton}",
                    NavigationType = NavigationTypeEnum.ShowDetailsInModules
                });
            }
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

            }
            else
            {
                html = html.Replace("{MyProfileForm}", "<div>");
                html = html.Replace("{/MyProfileForm}", "</div>");
                html = base.EmptySubTemplate("{SuccessTemplate}", html);

                html = html.Replace("{TextboxName}", string.Format("<input type=\"text\" id=\"NameTextbox{0:N}\" name=\"NameTextbox\" value=\"{1}\"/>", this.ID, user.Name));
                html = html.Replace("{TextboxForeName}", string.Format("<input type=\"text\" id=\"ForeNameTextbox{0:N}\" name=\"ForeNameTextbox\" value=\"{1}\"/>", this.ID, user.ForeName));
                html = html.Replace("{TextboxNamePrefix}", string.Format("<input type=\"text\" id=\"NamePrefixTextbox{0:N}\" name=\"NamePrefixTextbox\" value=\"{1}\"/>", this.ID, user.NamePrefix));
                html = html.Replace("{TextboxEmail}", string.Format("<input type=\"text\" id=\"EmailTextbox{0:N}\" name=\"EmailTextbox\" data-validation=\"required\" value=\"{1}\"/>", this.ID, user.Email));

                html = html.Replace("{CurrentPassword}", string.Format("<input type=\"text\" id=\"CurrentPasswordTextbox{0:N}\" TextMode=\"password\" name=\"CurrentPasswordTextbox\" />", this.ID));
                html = html.Replace("{NewPassword}", string.Format("<input type=\"text\" id=\"NewPasswordTextbox{0:N}\" TextMode=\"password\" name=\"NewPasswordTextbox\" />", this.ID));
                html = html.Replace("{RetypePassword}", string.Format("<input type=\"text\" id=\"RetypePasswordTextbox{0:N}\" TextMode=\"password\" name=\"RetypePasswordTextbox\" />", this.ID));
                if (user.Gender == BaseUser.SexeEnum.Male)
                {
                    html = html.Replace("{RadioSexeMale}", string.Format("<input type=\"radio\" id=\"SexeMaleRadio{0:N}\" name=\"Gender\" checked=\"checked\" value=\"{1}\"/>", this.ID, (int)BaseUser.SexeEnum.Male));
                    html = html.Replace("{RadioSexeFemale}", string.Format("<input type=\"radio\" id=\"SexeFemaleRadio{0:N}\" name=\"Gender\" value=\"{1}\"/>", this.ID, (int)BaseUser.SexeEnum.Female));
                }
                else
                {
                    html = html.Replace("{RadioSexeMale}", string.Format("<input type=\"radio\" id=\"SexeMaleRadio{0:N}\" name=\"Gender\" value=\"{1}\"/>", this.ID, (int)BaseUser.SexeEnum.Male));
                    html = html.Replace("{RadioSexeFemale}", string.Format("<input type=\"radio\" id=\"SexeFemaleRadio{0:N}\" name=\"Gender\" checked=\"checked\" value=\"{1}\"/>", this.ID, (int)BaseUser.SexeEnum.Female));
                }
                html = html.Replace("{TextboxAddress}", string.Format("<input type=\"text\" id=\"AddressTextbox{0:N}\" name=\"AddressTextbox\" value=\"{1}\"/>", this.ID, user.Address));
                html = html.Replace("{TextboxPostalCode}", string.Format("<input type=\"text\" id=\"PostcodeTextbox{0:N}\" name=\"PostcodeTextbox\" value=\"{1}\"/>", this.ID, user.Postalcode));
                html = html.Replace("{TextboxCity}", string.Format("<input type=\"text\" id=\"CityTextbox{0:N}\" name=\"CityTextbox\" value=\"{1}\"/>", this.ID, user.City));
                html = html.Replace("{TextboxBirthDate}", string.Format("<input type=\"text\" id=\"BirthDateTextbox{0:N}\" name=\"BirthDateTextbox\" type=\"date\" value=\"{1}\"/>", this.ID, user.BirthDate));
                html = html.Replace("{TextboxCountry}", string.Format("<input type=\"text\" id=\"CountryTextbox{0:N}\" name=\"CountryTextbox\" value=\"{1}\"/>", this.ID, user.Country));
            }
            html = base.ConvertTags(html);

            return html;
        }

        public override PostResult HandlePost(CmsPage page, Dictionary<string, string> FormParameters)
        {
            ModuleNavigationAction navigationAction = GetNavigationActionByTagName("{SubmitButton}");
            PostResult postResult = base.getNewPostResult(navigationAction);
            string errorMsg = "";
            try
            {
                //op save button geklikt
                BaseUser user = WebSessionHelper.CurrentSiteUser;
                if (user == null)
                {
                    errorMsg += "Geen geldige gebruiker geladen. U moet eerst aanmelden";
                }
                else
                {
                    bool validPassword = true;
                    if (FormParameters.ContainsKey("CurrentPasswordTextbox"))
                    {
                        //validPassword = user...;
                    }
                    user.ForeName = FormParameters.ContainsKey("ForeNameTextbox") ? FormParameters["ForeNameTextbox"] : "";
                    user.NamePrefix = FormParameters.ContainsKey("NamePrefixTextbox") ? FormParameters["NamePrefixTextbox"] : "";
                    user.Name = FormParameters.ContainsKey("NameTextbox") ? FormParameters["NameTextbox"] : "";
                    user.Email = FormParameters.ContainsKey("EmailTextbox") ? FormParameters["EmailTextbox"] : "";
                    //user.Gender = FormParameters.ContainsKey("Gender") ? FormParameters["Gender"] : "";
                    //user.BirthDate = FormParameters.ContainsKey("BirthDateTextbox") ? Convert.ToDateTime(FormParameters["BirthDateTextbox"]) : "";
                    user.Address = FormParameters.ContainsKey("AddressTextbox") ? FormParameters["AddressTextbox"] : "";
                    user.Postalcode = FormParameters.ContainsKey("PostcodeTextbox") ? FormParameters["PostcodeTextbox"] : "";
                    user.City = FormParameters.ContainsKey("CityTextbox") ? FormParameters["CityTextbox"] : "";
                    user.Country = FormParameters.ContainsKey("CountryTextbox") ? FormParameters["CountryTextbox"] : "";

                    
                   string newPassword = FormParameters.ContainsKey("NewPasswordTextbox") ? FormParameters["NewPasswordTextbox"] : "";
                   if (newPassword != null && newPassword != String.Empty)
                   {
                       //check
                   }

                    
                }
                if (errorMsg != String.Empty)
                {
                    new PostResult() { ErrorMessage = errorMsg };
                }
                else
                {
                    postResult.HtmlResult += Regex.Match(this.Content, "{SuccessTemplate}(.*?){/SuccessTemplate}", RegexOptions.Singleline).ToString().Replace("{SuccessTemplate}", "").Replace("{/SuccessTemplate}", ""); //Publish2(page);
                }
            }
            catch (Exception ex)
            {
                postResult = base.HandlePostError(ex);
            }
            return postResult;
        }
    }
}
