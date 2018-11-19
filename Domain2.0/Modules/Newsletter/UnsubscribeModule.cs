using BitPlate.Domain.Newsletters;
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
    public class UnsubscribeModule : SubscribeModule
    {
        NewsletterSubscriber loadedSubscriber;

        public UnsubscribeModule()
            : base()
        {
            //IncludeScripts.Add("/_js/BitNewsletterFormModule.js"); //wordt al in base gedaan
            ContentSamples.Clear();
            ContentSamples.Add(@"<div>{SubscriberForm}
    <h1>Afmelden voor de nieuwsbrief/Wijzigen gegevens</h1>
    <div>Emailadres: {LabelEmail}</div>
    <div>Voornaam: {TextboxForeName}</div>
    <div>Tussenvoegsel: {TextboxNamePrefix}</div>
    <div>Achternaam: {TextboxName}</div>
    <div>Bedrijf: {TextboxCompany}</div>
    <div>Geslacht: {RadioSexeMale}Man {RadioSexeFemale}Vrouw</div>
    <div>{NewsGroupCheckboxList}</div>
    <div>{SaveButton}Wijzigingen opslaan{/SaveButton} {UnsubscribeButton}Definitief beëindigen{/UnsubscribeButton}</div>
{/SubscriberForm}
 
{ErrorTemplate}Wijzigen zijn niet doorgevoerd: <br />{ErrorMessage}{/ErrorTemplate}

{SaveSuccessTemplate}U wijzigingen zijn opgeslagen.{/SaveSuccessTemplate}

{UnsubscribeSuccessTemplate}U wijzigingen zijn opgeslagen.{/UnsubscribeSuccessTemplate}

{UnsubscribeConfirmationTemplate}Weet u zeker dat u onze nieuwsbrief niet meer wil ontvangen?<br />
    {AcceptButton}Ja{/AcceptButton} {CancelButton}Nee{/CancelButton} 
{/UnsubscribeConfirmationTemplate}
");
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Afmeld instellingen", IsExternal = true, Url = "/_bitplate/EditPage/Modules/NewsletterModules/OptOutModuleTab.aspx" });

        }

        public override void SetAllTags()
        {
            this._tags.Add(new Tag() { Name = "{LabelEmail}" });
            this._tags.Add(new Tag() { Name = "{TextboxForeName}" });
            this._tags.Add(new Tag() { Name = "{TextboxNamePrefix}" });
            this._tags.Add(new Tag() { Name = "{TextboxName}" });
            this._tags.Add(new Tag() { Name = "{TextboxCompany}" });

            this._tags.Add(new Tag() { Name = "{RadioSexeMale}" });
            this._tags.Add(new Tag() { Name = "{RadioSexeFemale}" });

            this._tags.Add(new Tag() { Name = "{NewsGroupCheckboxList}" });

            this._tags.Add(new Tag() { Name = "{SaveButton}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{UnsubscribeButton}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{SubscriberForm}", HasCloseTag = true });

            this._tags.Add(new Tag() { Name = "{SaveSuccessTemplate}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{UnsubscribeSuccessTemplate}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{UnsubscribeConfirmationTemplate}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{AcceptButton}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{CancelButton}", HasCloseTag = true });
            base.SetAllTags();
        }

        protected override void setNavigationActions()
        {
            base.setNavigationActions();
            if (this.NavigationActions.Count == 0)
            {
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    Name = "{SaveButton}",
                    NavigationType = NavigationTypeEnum.ShowDetailsInModules
                });
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    Name = "{UnsubscribeButton}",
                    NavigationType = NavigationTypeEnum.ShowDetailsInModules
                });
            }
        }

        protected override string getModuleStartDiv()
        {
            string html = base.getModuleStartDiv();

            html += String.Format(@"
<input type=""hidden"" name=""hiddenSubscriberID"" value=""{1}""/>
", this.ID, loadedSubscriber.ID);

            return html;
        }
        public override string Publish2(CmsPage page)
        {
            return Publish2(page, "");
        }
        public string Publish2(CmsPage page, string submitAction)
        {
            string html = "";
            try
            {
                loadSubscriber(null);
                html = getModuleStartDiv();
                html += Content;
                //afhankelijk van op welke actionbutton is geklikt templates tonen.
                if (submitAction == "" || submitAction == "CancelConfirmation") //default
                {
                    html = fillSubscriberForm(html);

                    //html = base.EmptySubTemplate("{ErrorTemplate}", html);
                    html = base.EmptySubTemplate("{SaveSuccessTemplate}", html);
                    html = base.EmptySubTemplate("{UnsubscribeSuccessTemplate}", html);
                    html = base.EmptySubTemplate("{UnsubscribeConfirmationTemplate}", html);
                }
                else if (submitAction == "ShowConfirmationTemplate")
                {
                    html = fillSubscriberForm(html);
                    html = html.Replace("{UnsubscribeConfirmationTemplate}", "<div>");
                    html = html.Replace("{/UnsubscribeConfirmationTemplate}", "</div>");
                    html = base.EmptySubTemplate("{SaveSuccessTemplate}", html);
                    html = base.EmptySubTemplate("{UnsubscribeSuccessTemplate}", html);
                    html = base.EmptySubTemplate("{UnsubscribeButton}", html);
                    html = base.EmptySubTemplate("{SaveButton}", html);
                    html = html.Replace("{AcceptButton}", createSubmitButton("UnsubscribeSubscriber", false));
                    html = html.Replace("{/AcceptButton}", "</button>");
                    html = html.Replace("{CancelButton}", createSubmitButton("CancelConfirmation", false));
                    html = html.Replace("{/CancelButton}", "</button>");
                }
                else if (submitAction == "SaveSubscriber")
                {
                    //success save
                    html = html.Replace("{SaveSuccessTemplate}", "<div>");
                    html = html.Replace("{/SaveSuccessTemplate}", "</div>");
                    html = base.EmptySubTemplate("{UnsubscribeSuccessTemplate}", html);
                    html = base.EmptySubTemplate("{SubscriberForm}", html);
                    html = base.EmptySubTemplate("{UnsubscribeConfirmationTemplate}", html);
                }
                else if (submitAction == "UnsubscribeSubscriber")
                {
                    //success unsubscribe
                    html = html.Replace("{UnsubscribeSuccessTemplate}", "<div>");
                    html = html.Replace("{/UnsubscribeSuccessTemplate}", "</div>");
                    html = base.EmptySubTemplate("{SaveSuccessTemplate}", html);
                    html = base.EmptySubTemplate("{SubscriberForm}", html);
                    html = base.EmptySubTemplate("{UnsubscribeConfirmationTemplate}", html);
                }

                foreach (Tag tag in GetAllTags())
                {
                    if (tag.ReplaceValue != String.Empty)
                    {
                        html = html.Replace(tag.Name, tag.ReplaceValue);
                    }
                    if (tag.HasCloseTag && tag.ReplaceValueCloseTag != String.Empty)
                    {
                        html = html.Replace(tag.CloseTag, tag.ReplaceValueCloseTag);
                    }
                }
                html += getModuleEndDiv();

            }
            catch (Exception ex)
            {
                return base.HandlePublishError(ex);
            }
            return html;
        }

        private string fillSubscriberForm(string html)
        {
            html = html.Replace("{SubscriberForm}", "<div>");
            html = html.Replace("{/SubscriberForm}", "</div>");

            html = html.Replace("{LabelEmail}", loadedSubscriber.Email);
            html = html.Replace("{TextboxForeName}", "<input type=\"text\" id=\"textboxForeName\" name=\"textboxForeName\" data-validation=\"required\" value=\"" + loadedSubscriber.ForeName + "\" />");
            html = html.Replace("{TextboxNamePrefix}", "<input type=\"text\" id=\"textboxNamePrefix\" name=\"textboxNamePrefix\" value=\"" + loadedSubscriber.NamePrefix + "\" />");
            html = html.Replace("{TextboxName}", "<input type=\"text\" id=\"textboxName\" name=\"textboxName\" data-validation=\"required\" value=\"" + loadedSubscriber.Name + "\" />");
            html = html.Replace("{TextboxCompany}", "<input type=\"text\" id=\"textboxCompany\" name=\"textboxCompany\" value=\"" + loadedSubscriber.CompanyName + "\" />");
            if (loadedSubscriber.Gender == Autorisation.BaseUser.SexeEnum.Female)
            {
                html = html.Replace("{RadioSexeMale}", String.Format("<input type=\"radio\" id=\"radioSexeMale\"  name=\"radioGender\" value=\"{0}\" /> <label for=\"radioSexeMale\"></label>", (int)BitPlate.Domain.Autorisation.BaseUser.SexeEnum.Male));
                html = html.Replace("{RadioSexeFemale}", String.Format("<input type=\"radio\" id=\"radioSexeFemale\"  name=\"radioGender\" value=\"{0}\" checked=\"checked\"/> <label for=\"radioSexeFemale\"></label>", (int)BitPlate.Domain.Autorisation.BaseUser.SexeEnum.Female));
            }
            else
            {
                html = html.Replace("{RadioSexeMale}", String.Format("<input type=\"radio\" id=\"radioSexeMale\"  name=\"radioGender\" value=\"{0}\" checked=\"checked\"/> <label for=\"radioSexeMale\"></label>", (int)BitPlate.Domain.Autorisation.BaseUser.SexeEnum.Male));
                html = html.Replace("{RadioSexeFemale}", String.Format("<input type=\"radio\" id=\"radioSexeFemale\"  name=\"radioGender\" value=\"{0}\" /> <label for=\"radioSexeFemale\"></label>", (int)BitPlate.Domain.Autorisation.BaseUser.SexeEnum.Female));
            }

            html = html.Replace("{NewsGroupCheckboxList}", createNewsGroupCheckboxList(loadedSubscriber));

            html = html.Replace("{SaveButton}", createSubmitButton("SaveSubscriber"));
            html = html.Replace("{/SaveButton}", "</button>");

            //html = html.Replace("{UnsubscribeButton}", "<button type=\"submit\" onclick=\"javascript:$('#hiddenFormAction"+this.ID.ToString("N")+"').val('Unsubscribe');\">");
            //html = html.Replace("{UnsubscribeButton}", createUnsubScribeInnerForm(subscriber));
            //html = html.Replace("{UnsubscribeButton}", "<button type=\"button\" onclick=\"javascript:BITNEWSLETTERMODULES.unsubcribeSubscriber('" + this.ID.ToString("N") + "');\">");
            string buttonAction = "UnsubscribeSubscriber";
            if (html.Contains("{UnsubscribeConfirmationTemplate}"))
            {
                buttonAction = "ShowConfirmationTemplate";
            }
            html = html.Replace("{UnsubscribeButton}", createSubmitButton(buttonAction, false));
            html = html.Replace("{/UnsubscribeButton}", "</button>");
            return html;
        }



        public override PostResult HandlePost(CmsPage page, Dictionary<string, string> FormParameters)
        {
            //we hebben een loadedsubscriber nodig voor in getStartDiv()
            loadSubscriber(FormParameters);

            PostResult postResult = base.validateCaptchaIfExists(FormParameters);
            if (!postResult.Success) return postResult;
            try
            {
                string errorMsg = "";

                //form kan in 3 soorten worden gepost: wijzigen, confirmation en afmelden, afhankelijk van op welke button is geklikt
                string submitAction = FormParameters["hiddenCurrentSubmitAction"]; //submitAction is gekoppeld aan de submitbutton.value
                if (submitAction == "UnsubscribeSubscriber")
                {
                    errorMsg = UnsubscribeSubscriber(loadedSubscriber);
                }
                else if (submitAction == "SaveSubscriber")
                {
                    errorMsg = SaveSubscriber(FormParameters, loadedSubscriber);
                }


                if (errorMsg != "")
                {
                    postResult = new PostResult() { ErrorMessage = errorMsg };
                }
                else
                {
                    string html = Publish2(page, submitAction);
                    postResult.HtmlResult = html;
                    return postResult;
                }
            }
            catch (Exception ex)
            {
                return base.HandlePostError(ex);
            }
            return postResult;
        }

        private void loadSubscriber(Dictionary<string, string> FormParameters)
        {
            string subscriberId = Guid.Empty.ToString();
            if (FormParameters != null && FormParameters.ContainsKey("hiddenSubscriberID"))
            {
                subscriberId = FormParameters["hiddenSubscriberID"];
            }
            else
            {
                //laden uit querystring
                if (HttpContext.Current.Request.QueryString["subscriber"] != null)
                {

                    subscriberId = HttpContext.Current.Request.QueryString["subscriber"];
                    //??? er staat komma is en bevat waarde twee keer
                    if (subscriberId.Contains(","))
                    {
                        subscriberId = subscriberId.Split(new char[] { ',' })[0];
                    }
                }
            }
            loadedSubscriber = BaseObject.GetById<NewsletterSubscriber>(new Guid(subscriberId));
            if (loadedSubscriber == null)
            {
                loadedSubscriber = new NewsletterSubscriber();
            }
        }

        private string SaveSubscriber(Dictionary<string, string> FormParameters, NewsletterSubscriber subscriber)
        {
            if (subscriber.IsNew) return "Geen geldige abonnee geladen";
            subscriber.Name = FormParameters.ContainsKey("textboxName") ? FormParameters["textboxName"] : "";
            subscriber.NamePrefix = FormParameters.ContainsKey("textboxNamePrefix") ? FormParameters["textboxNamePrefix"] : "";
            subscriber.ForeName = FormParameters.ContainsKey("textboxForeName") ? FormParameters["textboxForeName"] : "";
            subscriber.CompanyName = FormParameters.ContainsKey("textboxCompany") ? FormParameters["textboxCompany"] : "";
            subscriber.Gender = FormParameters.ContainsKey("radioGender") ? (BitPlate.Domain.Autorisation.BaseUser.SexeEnum)Convert.ToInt32(FormParameters["radioGender"]) : Autorisation.BaseUser.SexeEnum.Undefined;
            subscriber.SubscribedGroups.Clear();
            //subscriber.Save();

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
            return "";
        }

        private string UnsubscribeSubscriber(NewsletterSubscriber subscriber)
        {
            if (subscriber.IsNew) return "Geen geldige abonnee geladen";
            //subscriber.UnsubscribeDate = DateTime.Now;
            //subscriber.Save();
            subscriber.Delete();
            return "";
        }

    }
}
