using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM.Attributes;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using BitPlate.Domain.Utils;
using System.Web;
using BitPlate.Domain.Newsletters;
using HJORM;
using System.Collections;

namespace BitPlate.Domain.Modules.ContactForm
{
    [Persistent("Module")]
    public class ContactFormModule : BasePostableModule
    {
        public ContactFormModule()
        {
            ContentSamples.Add(@"
<table>
	<tbody>
		<tr>
			<td>Naam:</td>
			<td><input data-validation=""required"" name=""Naam"" type=""text"" /></td>
		</tr>
		<tr>
			<td>Emailadres:</td>
			<td>{CustomerEmailAddress}</td>
		</tr>
		<tr>
			<td>Onderwerp</td>
			<td><input data-validation=""required"" name=""Onderwerp"" type=""text"" /></td>
		</tr>
		<tr>
			<td>Beschrijving</td>
			<td><textarea data-validation=""required"" name=""Beschrijving"" ></textarea></td>
		</tr>
	</tbody>
</table>

<p>{SubmitButton}Verstuur{/SubmitButton}</p>

<p>{ErrorTemplate}{ErrorMessage}{/ErrorTemplate}</p>");

            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Email instellingen", IsExternal = true, Url = "/_bitplate/EditPage/Modules/ContactFormModule/EmailConfigTab.aspx" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Email template", IsExternal = true, Url = "/_bitplate/EditPage/Modules/ContactFormModule/EmailTemplateTab.aspx" });
        }

        public override void SetAllTags()
        {
            this._tags.Add(new Tag() { Name = "{CustomerEmailAddress}" });
            this._tags.Add(new Tag() { Name = "{CheckboxRegisterToNewsletter}" });
            this._tags.Add(new Tag() { Name = "{SubmitButton}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{SubmitLink}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{SuccessTemplate}", HasCloseTag = true, ReplaceValue = "<div style=\"display: none\" id=SuccessTemplate" + this.ID.ToString("N") + ">", ReplaceValueCloseTag = "</div>" });
            this._tags.Add(new Tag() { Name = "{DropDownTo}" });

            //eventueel voor nieuwsbrief aanmelding
            this._tags.Add(new Tag() { Name = "{TextboxForeName}", ReplaceValue = "<input type=\"text\" id=\"textboxForeName\" name=\"Voornaam\" data-validation=\"required\"/>" });
            this._tags.Add(new Tag() { Name = "{TextboxNamePrefix}", ReplaceValue = "<input type=\"text\" id=\"textboxNamePrefix\" name=\"Tussenvoegsels\"  />" });
            this._tags.Add(new Tag() { Name = "{TextboxName}", ReplaceValue = "<input type=\"text\" id=\"textboxName\" name=\"Naam\" data-validation=\"required\"/>" });
            this._tags.Add(new Tag() { Name = "{TextboxCompany}", ReplaceValue = "<input type=\"text\" id=\"textboxCompany\" name=\"Bedrijf\" />" });
            this._tags.Add(new Tag() { Name = "{RadioSexeMale}", ReplaceValue = String.Format("<input type=\"radio\" id=\"radioSexeMale\"  name=\"radioGender\" value=\"{0}\" /> <label for=\"radioSexeMale\">", (int)BitPlate.Domain.Autorisation.BaseUser.SexeEnum.Male), HasCloseTag = true, ReplaceValueCloseTag = "</label>" });
            this._tags.Add(new Tag() { Name = "{RadioSexeFemale}", ReplaceValue = String.Format("<input type=\"radio\" id=\"radioSexeFemale\"  name=\"radioGender\" value=\"{0}\" /> <label for=\"radioSexeFemale\">", (int)BitPlate.Domain.Autorisation.BaseUser.SexeEnum.Female), HasCloseTag = true, ReplaceValueCloseTag = "</label>" });

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
                    NavigationType = NavigationTypeEnum.NavigateToPage
                });
            }
        }


        public override string Publish2(CmsPage page)
        {

            string html = base.Publish2(page);
            
            try
            {
                html = html.Replace("{SubmitButton}", String.Format(@"<button type=""submit"" ID=""button{0:N}"">", ID))
                    .Replace("{/SubmitButton}", "</button>")
                    .Replace("{SubmitLink}", String.Format(@"<a href=""javascript:void(0)"" onclick=""BITSITESCRIPT.submitPostableModule('form{0}');"">", ID))
                    .Replace("{/SubmitLink}", @"</a>")
                    .Replace("{DropDownTo}", "<select name=\"bitDropDownTo\">{TOOPSIONS}</select>")
                    //.Replace("{CustomerEmailAddress}", string.Format("<input type=\"text\" data-validation=\"required,email\" id=\"CustomerEmailAddressTextBox{0}\" name=\"CustomerEmailAddressTextBox\"/>", this.ID.ToString("N")))
                    .Replace("{CheckboxRegisterToNewsletter}", string.Format("<input type=\"checkbox\" id=\"bitCheckboxRegisterToNewsletter{0}\" name=\"bitCheckboxRegisterToNewsletter\" />", this.ID.ToString("N")));

                //.Replace("{MessageLabel}", string.Format("<ul ID=\"Messagelabel{0:N}\"></ul>", this.ID));
                //html += "<input type=\"hidden\" name=\"bitToEmailAddress\" id=\"bitToEmailAddress" + this.ID.ToString("N") + " value=\"" + this.getSetting<string>("ContactFormRecipient") + "\" />";
                string toType = getSetting<string>("RecipientTypeTo");
                if (toType == "dynamic")
                {
                    ArrayList recipients = getSetting<ArrayList>("Recipients");
                    if (recipients != null && recipients.Count > 0)
                    {
                        string options = "";
                        foreach (Dictionary<string, object> option in recipients)
                        {
                            options += "<option>" + option["Name"] + "</option>";
                        }
                        html = html.Replace("{TOOPSIONS}", options);
                    }
                }
                
                foreach (Tag tag in this.GetAllTags())
                {
                    html = html.Replace(tag.Name, tag.ReplaceValue);
                    if (tag.HasCloseTag) html = html.Replace(tag.CloseTag, tag.ReplaceValueCloseTag);
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
            PostResult result = base.validateCaptchaIfExists(FormParameters);
            if (!result.Success) return result;
            
            string errorMsg = "";
            try
            {
                string toAddress = "";
                string formValues = "";
                string emailCustomer = "";
                if (FormParameters.ContainsKey("CustomerEmailAddressTextBox"))
                {
                    emailCustomer = FormParameters["CustomerEmailAddressTextBox"];
                    formValues += "<tr><td>Emailadres</td><td><a href='mailto:'" + emailCustomer + "'>" + emailCustomer + "</a></td></tr>";
                }
                foreach (string key in FormParameters.Keys)
                {
                    if (key != null && !key.StartsWith("hidden") && key != "CaptchaValidationField" && key != "bitToEmailAddress")
                    {
                        formValues += "<tr>";

                        if (key != "bitDropDownTo")
                        {
                            formValues += "<td>" + key.Replace(this.ID.ToString(), "") + "</td><td>" + FormParameters[key] + "</td>";
                        }
                        formValues += "</tr>";
                    }

                    if (key == "bitDropDownTo")
                    {
                        string toType = getSetting<string>("RecipientTypeTo");
                        if (toType == "dynamic")
                        {
                            toAddress = GetRecipientByName(FormParameters, toAddress, key);
                        }
                    }
                }
                List<string> attachments = new List<string>();
                HttpFileCollection files = HttpContext.Current.Request.Files;
                foreach (string key in files.AllKeys)
                {
                    //if (key == "hiddenModuleID" || key == "hiddenModuleType" || key == "hiddenCurrentSubmitAction")
                    if (key.StartsWith("hidden") || key == "CaptchaValidationField")
                    {
                        continue;
                    }
                    // if (key.StartsWith(this.ModuleID.ToString()) && files[key].FileName != "")
                    if (files[key].FileName != "")
                    {
                        //save file
                        string fileName = HttpContext.Current.Server.MapPath("") + @"\_temp\" + files[key].FileName;
                        files[key].SaveAs(fileName);
                        attachments.Add(fileName);
                    }
                }
                if (formValues != "")
                {
                    formValues = "<table>" + formValues + "</table>";
                    string emailBody = getSetting<string>("ContactFormTemplate");
                    if (emailBody == null || emailBody == "") emailBody = "Resultaten ContactFormulier: <br/><br/>[FormResult]";
                    if (emailBody.Contains("FormResult"))
                    {
                        emailBody = emailBody.Replace("[FormResult]", formValues);
                    }
                    else
                    {
                        emailBody += formValues;
                    }

                    SendEmail(toAddress, emailCustomer, emailBody, attachments);

                    if (FormParameters.ContainsKey("bitCheckboxRegisterToNewsletter"))
                    {
                        bool registerToNewsletter = (FormParameters["bitCheckboxRegisterToNewsletter"] == "on");
                        if (registerToNewsletter)
                        {
                            errorMsg += insertSubscriber(emailCustomer, FormParameters);
                        }
                    }

                    result.HtmlResult = getModuleStartDiv();
                    result.HtmlResult += Regex.Match(this.Content, "{SuccessTemplate}(.*?){/SuccessTemplate}", RegexOptions.Singleline).ToString().Replace("{SuccessTemplate}", "").Replace("{/SuccessTemplate}", "");
                    result.HtmlResult += getModuleEndDiv();
                }
                else if (formValues == "")
                {
                    errorMsg += "<li>Er zijn geen geldige invoervelden gevonden in het formulier.</li>";
                }
                if (errorMsg != String.Empty)
                {
                    result = new PostResult() { ErrorMessage = errorMsg };
                }

            }
            catch (Exception ex)
            {
                result = base.HandlePostError(ex);
            }
            return result;
        }

        private string GetRecipientByName(Dictionary<string, string> FormParameters, string toAddress, string key)
        {
            ArrayList recipients = getSetting<ArrayList>("Recipients");
            if (recipients != null && recipients.Count > 0)
            {
                foreach (Dictionary<string, object> option in recipients)
                {
                    if (FormParameters[key] == option["Name"].ToString())
                    {
                        toAddress = option["EmailAddress"].ToString();
                    }
                }
            }
            return toAddress;
        }




        private void SendEmail(string toAddress, string emailCustomer, string emailBody, List<string> attachments)
        {
            string fromEmailAddress = "";
            if (getSetting<string>("EmailAddressFrom") == "0")
            {
                fromEmailAddress = Site.CurrentWorkingEnvironment.EmailSettingsFrom;
            }

            if (getSetting<string>("EmailAddressFrom") == "1")
            {
                fromEmailAddress = emailCustomer;
            }

            if (getSetting<string>("EmailAddressFrom") == "2")
            {
                fromEmailAddress = getSetting<string>("ContactFormSender");
            }
            string bcc = getSetting<string>("BCCEmailAddress");

            string toType = getSetting<string>("RecipientTypeTo");
            if (toType == null || toType == "" || toType == "static")
            {
                toAddress = getSetting<string>("ContactFormRecipient");
            }

            string debugEmailAddress = getSetting<string>("DebugEmailAddress");
            if (bcc != null && bcc != "" && Regex.Match(bcc, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                if (debugEmailAddress != null && debugEmailAddress != "") bcc = bcc + ";" + debugEmailAddress;
                EmailManager.SendMail(fromEmailAddress, toAddress, getSetting<string>("ContactFormSubject"), emailBody, true, attachments.ToArray(), bcc);
            }
            else
            {
                EmailManager.SendMail(fromEmailAddress, toAddress, getSetting<string>("ContactFormSubject"), emailBody, true, attachments.ToArray(), debugEmailAddress);
            }
        }


        private string insertSubscriber(string email, Dictionary<string, string> FormParameters)
        {
            string errorMsg = "";
            BaseCollection<NewsletterSubscriber> subscribers = BaseCollection<NewsletterSubscriber>.Get("Name = '" + email + "'");
            if (subscribers.Count > 0)
            {
                errorMsg = "Dit emailadres is al geabonneerd.";

            }
            else
            {
                NewsletterSubscriber subscriber = new NewsletterSubscriber();
                subscriber.Email = email;
                subscriber.Name = FormParameters.ContainsKey("Naam") ? FormParameters["Naam"] : "";
                subscriber.NamePrefix = FormParameters.ContainsKey("Tussenvoegsels") ? FormParameters["Tussenvoegsels"] : "";
                subscriber.ForeName = FormParameters.ContainsKey("Voornaam") ? FormParameters["Voornaam"] : "";
                subscriber.CompanyName = FormParameters.ContainsKey("Bedrijf") ? FormParameters["Bedrijf"] : "";
                subscriber.Gender = FormParameters.ContainsKey("Geslacht") ? (BitPlate.Domain.Autorisation.BaseUser.SexeEnum)Convert.ToInt32(FormParameters["Geslacht"]) : Autorisation.BaseUser.SexeEnum.Undefined;
                subscriber.Confirmed = true; //Mag direct worden goedgekeurd.


                //verplichte groepen toevoegen
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


                //if (getSetting<bool>("SendVerifiationEmail"))
                //{
                //    SendVerificationEmail(subscriber);
                //}

            }
            return errorMsg;

        }
    }
}
