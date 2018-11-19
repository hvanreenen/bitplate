using BitPlate.Domain.Newsletters;
using BitPlate.Domain.Utils;
using HJORM;
using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BitPlate.Domain.Modules.Newsletter
{
    [Persistent("Module")]
    public class SubscribeModule : BasePostableModule
    {
        string errorMsg = "";
        public SubscribeModule()
        {
            //IncludeScripts.Add("/_js/BitFormValidation.js");

            ContentSamples.Add(@"<div>{SubscriberForm}
                                    <div>
                                    <h1>Aanmelden voor de nieuwsbrief</h1>
                                    </div>

                                    <div>Emailadres: {TextboxEmailAddress}</div>
                                    <div>Voornaam: {TextboxForeName}</div>
                                    <div>Tussenvoegsel: {TextboxNamePrefix}</div>
                                    <div>Achternaam: {TextboxName}</div>
                                    <div>Bedrijf: {TextboxCompany}</div>

                                    <div>Geslacht: {RadioSexeMale}Man {RadioSexeFemale}Vrouw</div>

                                    <div>{NewsGroupCheckboxList}</div>

                                    <div>{SubmitButton}Aanmelden{/SubmitButton}</div>

                                    {/SubscriberForm} 
                                    {ErrorTemplate}Kan uw abonnement niet opslaan: <br />{ErrorMessage}{/ErrorTemplate}
                                    {SuccessTemplate}Hartelijk dank voor uw aanmelding. U ontvangt binnenkort onze nieuwsbrief.{/SuccessTemplate}
                                    
                                </div>");

            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Aanmelding", IsExternal = true, Url = "/_bitplate/EditPage/Modules/NewsletterModules/SubscribeModuleTab.aspx" });

        }

        public override void SetAllTags()
        {

            this._tags.Add(new Tag() { Name = "{TextboxEmailAddress}", ReplaceValue = "<input type=\"text\" id=\"textboxEmail\" name=\"textboxEmail\" data-validation=\"required\"/>" });
            this._tags.Add(new Tag() { Name = "{TextboxForeName}", ReplaceValue = "<input type=\"text\" id=\"textboxForeName\" name=\"textboxForeName\" data-validation=\"required\"/>" });
            this._tags.Add(new Tag() { Name = "{TextboxNamePrefix}", ReplaceValue = "<input type=\"text\" id=\"textboxNamePrefix\" name=\"textboxNamePrefix\"  />" });
            this._tags.Add(new Tag() { Name = "{TextboxName}", ReplaceValue = "<input type=\"text\" id=\"textboxName\" name=\"textboxName\" data-validation=\"required\"/>" });
            this._tags.Add(new Tag() { Name = "{TextboxCompany}", ReplaceValue = "<input type=\"text\" id=\"textboxCompany\" name=\"textboxCompany\" />" });

            this._tags.Add(new Tag() { Name = "{RadioSexeMale}", ReplaceValue = String.Format("<input type=\"radio\" id=\"radioSexeMale\"  name=\"radioGender\" value=\"{0}\" /> <label for=\"radioSexeMale\">", (int)BitPlate.Domain.Autorisation.BaseUser.SexeEnum.Male), HasCloseTag = true, ReplaceValueCloseTag = "</label>" });
            this._tags.Add(new Tag() { Name = "{RadioSexeFemale}", ReplaceValue = String.Format("<input type=\"radio\" id=\"radioSexeFemale\"  name=\"radioGender\" value=\"{0}\" /> <label for=\"radioSexeFemale\">", (int)BitPlate.Domain.Autorisation.BaseUser.SexeEnum.Female), HasCloseTag = true, ReplaceValueCloseTag = "</label>" });

            this._tags.Add(new Tag() { Name = "{NewsGroupCheckboxList}", ReplaceValue = createNewsGroupCheckboxList() });

            this._tags.Add(new Tag() { Name = "{SubmitButton}", HasCloseTag = true, ReplaceValue = createSubmitButton("Save"), ReplaceValueCloseTag = "</button>" });
            this._tags.Add(new Tag() { Name = "{SubmitLinkButton}", HasCloseTag = true, ReplaceValue = "<a id=\"submitLinkButton\" href=\"javascript:void(0)\" onclick=\"BITSITESCRIPT.setCurrentButtonAction('Save', '" + this.ID.ToString("N") + "', true);\" >", ReplaceValueCloseTag = "</a>" });

            this._tags.Add(new Tag() { Name = "{SubscriberForm}", HasCloseTag = true, ReplaceValue = "<div  id=BitPanelRegisterForm" + this.ID.ToString("N") + ">", ReplaceValueCloseTag = "</div>" });
            this._tags.Add(new Tag() { Name = "{SuccessTemplate}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{ResendOptInEmailButton}", HasCloseTag = true });
            base.SetAllTags();
        }

        

        protected string createNewsGroupCheckboxList(NewsletterSubscriber subscriber = null)
        {
            if (subscriber == null) subscriber = new NewsletterSubscriber();
            string html = "";
            BaseCollection<NewsletterGroup> newsletterGroups = BaseCollection<NewsletterGroup>.Get("FK_Site = '" + Site.ID.ToString() + "'");
            bool showCheckboxList = true;
            if (Settings.ContainsKey("Newsgroup")) //unsubscribemodule bevat deze setting niet, dan altijd tonen
            {
                showCheckboxList = getSetting<string>("Newsgroup") == "choose";
            }
            if (showCheckboxList)
            {
                foreach (NewsletterGroup group in newsletterGroups)
                {
                    if (group.IsChoosableGroup)
                    {
                        bool ischecked = subscriber.HasGroup(group);
                        if (ischecked)
                        {
                            html += String.Format("<input type=\"checkbox\" id=\"checkboxNewsletterGroup{0}\" name=\"checkboxNewsletterGroup{0}\" checked=\"on\"/> <label for=\"checkboxNewsletterGroup{0}\">{1}</label><br />", group.ID, group.Name);
                        }
                        else
                        {
                            html += String.Format("<input type=\"checkbox\" id=\"checkboxNewsletterGroup{0}\" name=\"checkboxNewsletterGroup{0}\"/> <label for=\"checkboxNewsletterGroup{0}\">{1}</label><br />", group.ID, group.Name);
                        }
                    }
                }
            }
            return html;
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

        protected override ValidationResult validateModule()
        {
            ValidationResult returnValue = base.validateModule();
            if (!returnValue.IsValid) return returnValue;

            if (!this.Content.Contains("{TextboxEmailAddress}"))
            {
                returnValue.Message += "Plaats de tag {TextboxEmailAddress}.<br />";
            }

            if (getSetting<string>("Newsgroup") == "choose" && !this.Content.Contains("{NewsGroupCheckboxList}"))
            {
                returnValue.Message += "Plaats de tag {NewsGroupCheckboxList}.<br />";
            }

            foreach (Tag tag in GetAllTags())
            {
                if (Utils.StringHelper.CountOccurences(this.Content, tag.Name) > 1)
                {
                    returnValue.Message += "Module mag maar 1 keer tag bevatten: " + tag.Name + " (controleer ook code)<br/>";
                }

                if (tag.HasCloseTag && Utils.StringHelper.CountOccurences(this.Content, tag.CloseTag) > 1)
                {
                    returnValue.Message += "Module mag maar 1 keer sluit-tag bevatten: " + tag.CloseTag + " (controleer ook code)<br/>";
                }
            }

            if (getSetting<bool>("SendVerifiationEmail"))
            {
                if (Site.NewsletterOptInEmailContent == null || Site.NewsletterOptInEmailSubject == null)
                {
                    returnValue.Message += "WAARSCHUWING: Deze site bevat nog geen globale nieuwsbriefinstellingen. Aanmedlingen zullen niet goed worden afgehandeld.<br/>";
                }
            }

            return returnValue;
        }

        public override string Publish2(CmsPage page)
        {
            string html = base.Publish2(page);
            try
            {
                //html = base.EmptySubTemplate("{ErrorTemplate}", html);
                html = base.EmptySubTemplate("{SuccessTemplate}", html);
                foreach (Tag tag in GetAllTags())
                {
                    html = html.Replace(tag.Name, tag.ReplaceValue);
                    if (tag.HasCloseTag)
                    {
                        html = html.Replace(tag.CloseTag, tag.ReplaceValueCloseTag);
                    }
                }
            }
            catch (Exception ex)
            {
                html = base.HandlePublishError(ex);
            }
            return html;
        }

        public override PostResult HandlePost(CmsPage page, Dictionary<string, string> FormParameters)
        {
            //ModuleNavigationAction navigationAction = GetNavigationActionByTagName("{SubmitButton}");
            //PostResult postResult = base.getNewPostResult(navigationAction);
            PostResult postResult = base.validateCaptchaIfExists(FormParameters);
            if (!postResult.Success) return postResult;
            if (this is UnsubscribeModule) return postResult;
            try
            {
                string html = getModuleStartDiv();
                html += Content;
                NewsletterSubscriber subscriber = null;
                //form kan in twee soorten worden gepost: normale en resendverifactionemail
                string submitAction = FormParameters["hiddenCurrentSubmitAction"]; //submitAction is gekoppeld aan de submitbutton.value
                if (submitAction == "ResendEmail")
                {
                    subscriber = ResendEmail(FormParameters);
                }
                else
                {
                    subscriber = InsertSubscriber(FormParameters);
                }

                if (errorMsg != "")
                {
                    postResult.Success = false;
                    postResult.ErrorMessage = errorMsg;
                    //string errorTemplate = base.GetSubTemplate("{ErrorTemplate}");
                    //errorTemplate = errorTemplate.Replace("{ErrorMessage}", errorMsg);
                    //html = base.ReplaceSubTemplate("{ErrorTemplate}", html, errorTemplate);
                    //html = base.EmptySubTemplate("{SuccessTemplate}", html);
                }
                else
                {
                    string successTemplate = base.GetSubTemplate("{SuccessTemplate}");
                    html = base.ReplaceSubTemplate("{SuccessTemplate}", html, successTemplate);
                    //html = base.EmptySubTemplate("{ErrorTemplate}", html);
                }
                //haal form weg (zou ook kunnen blijven?)
                html = base.EmptySubTemplate("{SubscriberForm}", html);
                if (html.Contains("{ResendOptInEmailButton}"))
                {
                    if (subscriber != null)
                    {
                        html += String.Format(@"
<input type=""hidden"" name=""hiddenSubscriberID"" value=""{0}""/>
", subscriber.ID);
                    }

                    html = html.Replace("{ResendOptInEmailButton}", @"<button type=""submit"" onclick=""BITSITESCRIPT.setCurrentButtonAction('ResendEmail', '" + this.ID.ToString("N") + @"', false);"">");
                    html = html.Replace("{/ResendOptInEmailButton}", @"</button>");
                }

                postResult.HtmlResult = html;

            }
            catch (Exception ex)
            {
                postResult = base.HandlePostError(ex);
            }
            return postResult;
        }

        private NewsletterSubscriber InsertSubscriber(Dictionary<string, string> FormParameters)
        {
            NewsletterSubscriber subscriber = new NewsletterSubscriber();
            string email = FormParameters["textboxEmail"].Trim();
            BaseCollection<NewsletterSubscriber> subscribers = BaseCollection<NewsletterSubscriber>.Get("Name = '" + email + "'");
            if (subscribers.Count > 0)
            {
                errorMsg = "Dit emailadres is al geabonneerd.";

            }
            else
            {

                subscriber.Email = email;
                subscriber.RegistrationType = RegistrationTypeEnum.Website;
                subscriber.Name = FormParameters.ContainsKey("textboxName") ? FormParameters["textboxName"] : "";
                subscriber.NamePrefix = FormParameters.ContainsKey("textboxNamePrefix") ? FormParameters["textboxNamePrefix"] : "";
                subscriber.ForeName = FormParameters.ContainsKey("textboxForeName") ? FormParameters["textboxForeName"] : "";
                subscriber.CompanyName = FormParameters.ContainsKey("textboxCompany") ? FormParameters["textboxCompany"] : "";
                subscriber.Gender = FormParameters.ContainsKey("radioGender") ? (BitPlate.Domain.Autorisation.BaseUser.SexeEnum)Convert.ToInt32(FormParameters["radioGender"]) : Autorisation.BaseUser.SexeEnum.Undefined;

                subscriber.Save();

                //groepen toevoegen
                BaseCollection<NewsletterGroup> requiredNewsletterGroups = BaseCollection<NewsletterGroup>.Get("FK_Site = '" + Site.ID.ToString() + "' AND IsMandatoryGroup=1");
                foreach (NewsletterGroup group in requiredNewsletterGroups)
                {
                    subscriber.SubscribedGroups.Add(group);
                }

                foreach (string key in FormParameters.Keys)
                {
                    if (key.StartsWith("checkboxNewsletterGroup"))
                    {
                        Guid ID;
                        Guid.TryParse(key.Replace("checkboxNewsletterGroup", ""), out ID);
                        NewsletterGroup group = BaseObject.GetById<NewsletterGroup>(ID);
                        subscriber.SubscribedGroups.Add(group);
                    }
                }
                subscriber.Save();


                if (getSetting<bool>("SendVerifiationEmail"))
                {
                    SendVerificationEmail(subscriber);
                }
            }
            return subscriber;
        }

        protected NewsletterSubscriber ResendEmail(Dictionary<string, string> FormParameters)
        {

            string subscriberId = FormParameters["hiddenSubscriberID"];

            NewsletterSubscriber subscriber = BaseObject.GetById<NewsletterSubscriber>(new Guid(subscriberId));

            if (getSetting<bool>("SendVerifiationEmail"))
            {
                SendVerificationEmail(subscriber);
            }
            return subscriber;

        }


        //protected string HandlePostResendEmail(Dictionary<string, string> FormParameters)
        //{
        //    string html = getModuleStartDiv();
        //    html += Content;
        //    string errorMsg = "";

        //    string subscriberId = FormParameters["hiddenSubscriberID"];

        //    NewsletterSubscriber subscriber = BaseObject.GetById<NewsletterSubscriber>(new Guid(subscriberId));

        //    if (getSetting<bool>("SendVerifiationEmail"))
        //    {
        //        SendVerificationEmail(subscriber);
        //    }

        //    if (errorMsg != "")
        //    {
        //        string errorTemplate = base.GetSubTemplate("{ErrorTemplate}");
        //        errorTemplate = errorTemplate.Replace("{ErrorMessage}", errorMsg);
        //        html = base.ReplaceSubTemplate("{ErrorTemplate}", html, errorTemplate);
        //        html = base.EmptySubTemplate("{SuccessTemplate}", html);
        //    }
        //    else
        //    {
        //        string successTemplate = base.GetSubTemplate("{SuccessTemplate}");
        //        //successTemplate = successTemplate.Replace("{SuccessMessage}", successMsg);
        //        html = base.ReplaceSubTemplate("{SuccessTemplate}", html, successTemplate);
        //        html = base.EmptySubTemplate("{ErrorTemplate}", html);
        //    }
        //    html = base.EmptySubTemplate("{RegisterForm}", html);
        //    if (html.Contains("{ResendOptInEmailButton}"))
        //    {
        //        html = replaceResendEmailButton(html, subscriber.ID);
        //    }
        //    html += getModuleEndDiv();

        //    handleNavigation(errorMsg);

        //    return html;
        //}

//        private string replaceResendEmailButton(string html, Guid subscriberID)
//        {
//            //string.string form = getModuleStartDiv();
//            html += String.Format(@"
//<input type=""hidden"" name=""hiddenFormAction"" value=""ResendEmail""/>
//<input type=""hidden"" name=""hiddenSubscriberID"" value=""{0}""/>
//", subscriberID);
//            //html = html.Replace("{ResendOptInEmailButton}", form + @"<button type=""submit"">");
//            //html = html.Replace("{/ResendOptInEmailButton}", @"</button></form>");
//            html = html.Replace("{ResendOptInEmailButton}", @"<button type=""submit"">");
//            html = html.Replace("{/ResendOptInEmailButton}", @"</button>");
//            return html;
//        }

        public void SendVerificationEmail(NewsletterSubscriber subscriber)
        {
            string content = ReplaceVerificationEmailTags(Site.NewsletterOptInEmailContent, subscriber);
            EmailManager.SendMail(Site.NewsletterSender, subscriber.Email, Site.NewsletterOptInEmailSubject, content, true);
        }

        private string ReplaceVerificationEmailTags(string content, NewsletterSubscriber subscriber)
        {

            string newsgroups = "";
            foreach (NewsletterGroup group in subscriber.SubscribedGroups.Where(c => c.IsMandatoryGroup == false))
            {
                newsgroups += group.Name + ", ";
            }
            if (newsgroups != "") newsgroups = newsgroups.Substring(0, newsgroups.Length - 2);

            string optinUrl = Site.DomainName + "/?ErrorMsg=geen opt-in ingesteld. Raadpleeg de site-admin: " + Site.NewsletterSender;
            if (Site.NewsletterOptInEmailPage != null)
            {
                optinUrl = Site.DomainName + "/" + Site.NewsletterOptInEmailPage.RelativeUrl + "?subscriber=" + subscriber.ID.ToString();
            }
            content = content.Replace("[OPTINURL]", optinUrl)
                .Replace("[NAME]", subscriber.Name)
                .Replace("[FORENAME]", subscriber.ForeName)
                .Replace("[NAMEPREFIX]", subscriber.ForeName)
                .Replace("[USERCODE]", subscriber.ID.ToString())
                .Replace("[NEWSGROUPS]", newsgroups)
                .Replace("[OPTINLINK]", "<a href=\"" + optinUrl + "\">").Replace("[/OPTINLINK]", "</a>");
            return content;
        }

        protected PostResult handleNavigation(string errorMsg, PostResult result)
        {
            ModuleNavigationAction navigationAction = GetNavigationActionByTagName("{SubmitButton}");
            result.NavigationType = navigationAction.NavigationType;
            if (navigationAction.NavigationType == NavigationTypeEnum.NavigateToPage && navigationAction.NavigationPage != null)
            {
                string queryString = "?errorMsg=" + errorMsg;
                //eigenlijk moet deze code verhuizen naar webform
                result.NavigationUrl = navigationAction.NavigationPage.RelativeUrl + queryString;
                //HttpContext.Current.Response.Redirect(navigationAction.NavigationPage.RelativeUrl + queryString);
            }
            else
            {
                string refreshModules = navigationAction.RefreshModules == null ? "" : String.Join(",", navigationAction.RefreshModules);
                result.RefreshModules = refreshModules;
                if (navigationAction.NavigationType == NavigationTypeEnum.NavigateToPage)
                {
                    result.ErrorMessage = "Kan niet naar pagina navigeren, omdat er geen pagina gezet is.";
                }
            }
            return result;
        }
    }
}
