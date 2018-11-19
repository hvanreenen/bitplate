using BitPlate.Domain.Modules;
using BitPlate.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate.EditPage.Modules.ContactFormModule
{
    public partial class ContactFormModuleControl_emiel : BaseModuleUserControl
    {
        protected TextBox CustomerEmailAddressTextBox;
        protected List<HtmlControl> CustomInputs;
        protected Button ButtonSubmit;
        protected LinkButton LinkButtonSubmit;
        protected Label Messagelabel;

        private string emailValues;
        private string emailBody;
        private List<HtmlInputFile> fileInputControls;
        private List<string> attachments = new List<string>();

        private bool showFieldErrorMessageInLabel;
        private bool showFieldErrorClass;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (base.CheckAutorisation())
            {
                this.FindControls();
                this.LoadSettings();
                if (this.Settings != null && this.Settings.ContainsKey("showFieldErrorMessageInLabel"))
                {
                    this.showFieldErrorMessageInLabel = (bool)this.Settings["showFieldErrorMessageInLabel"];
                    this.showFieldErrorClass = (bool)this.Settings["showFieldErrorClass"];
                }
                if (ButtonSubmit != null) ButtonSubmit.Click += ButtonSubmit_Click;
                if (LinkButtonSubmit != null) LinkButtonSubmit.Click += ButtonSubmit_Click;
            }
        }

        protected void FindControls()
        {
            /* this.LoadModuleObject();
            string updatePanelId = "BitUpdatePanel" + this.module.ContainerName;
            UpdatePanel updatePanel = (UpdatePanel)this.FindControlRecursive(this.Page.Master, updatePanelId); */

            this.CustomerEmailAddressTextBox = (TextBox)this.FindControl(string.Format("CustomerEmailAddressTextBox{0}", this.ModuleID.ToString("N")));
            this.ButtonSubmit = (Button)this.FindControl("ButtonSubmit" + this.ModuleID.ToString("N"));


            /* if (updatePanel != null && this.ButtonSubmit != null)
            {
                updatePanel.Triggers.Add(new PostBackTrigger()
                {
                    ControlID = "ButtonSubmit" + this.ModuleID.ToString("N")
                });
            } */
            

            this.LinkButtonSubmit = (LinkButton)this.FindControl("LinkButtonSubmit" + this.ModuleID.ToString("N"));
            /* if (updatePanel != null && this.LinkButtonSubmit != null)
            {
                updatePanel.Triggers.Add(new PostBackTrigger()
                {
                    ControlID = "LinkButtonSubmit" + this.ModuleID.ToString("N")
                });
            } */
            this.Messagelabel = (Label)this.FindControl("MessageLabel" + this.ModuleID.ToString("N"));

            this.CustomInputs = new List<HtmlControl>();
            foreach (Control control in this.Controls)
            {
                if (control.GetType().BaseType == typeof(HtmlInputControl) || 
                    control.GetType().BaseType == typeof(HtmlSelect) ||
                    control.GetType().BaseType == typeof(HtmlContainerControl) ||
                    control.GetType().BaseType == typeof(HtmlTextArea))
                {
                    if (control.ID.Contains(this.ModuleID.ToString("N"))) this.CustomInputs.Add((HtmlControl)control);
                }
            }
        }

        public void ButtonSubmit_Click(object sender, EventArgs e)
        {
            this.LoadModuleObject();
            
            bool validation = true;
            if (this.Messagelabel != null) this.Messagelabel.Text = "";

            if (this.CustomerEmailAddressTextBox != null)
            {
                emailBody = this.Settings["ContactFormTemplate"].ToString();
                emailValues = "<table><tr><td>Emailadres:</td><td>" + this.CustomerEmailAddressTextBox.Text + "</td></tr>";

                if (!Regex.IsMatch(this.CustomerEmailAddressTextBox.Text.Trim(), @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                {
                    if (this.Messagelabel != null) this.Messagelabel.Text += "Emailadres<br />";
                    validation = false;
                }

                foreach (HtmlControl htmlControl in this.CustomInputs)
                {
                    bool currentValueValidation = SetHtmlControlValueInEmail(htmlControl);
                    validation = (validation) ? currentValueValidation : validation;
                }

                /* if (validation)
                {
                    foreach (HtmlInputFile htmlInputFile in fileInputControls)
                    {
                        bool currentValueValidation = HandleAttachments(htmlInputFile);
                        validation = (validation) ? currentValueValidation : validation;
                    }
                } */
                
                    
                if (!validation)
                {
                    if (this.Messagelabel != null)
                    {
                        if (this.Messagelabel.Text != "")
                        {
                            if (this.showFieldErrorMessageInLabel)
                            {
                                this.Messagelabel.Text = "De volgende invoervelden zijn onjuist ingevuld: <br />" + this.Messagelabel.Text;
                            }
                        }
                    }
                }
                else
                {
                    emailBody = emailBody.Replace("[FormResult]", emailValues); //string.Format(emailBody, emailValues);
                    this.SendEmail();
                    this.showEmailSend();
                }
            }
            else
            {
                if (this.Messagelabel != null) this.Messagelabel.Text = "De tag {CustomerEmailAddressTextBox} kan niet worden gevonden.";
            } 
        }

        private bool SetHtmlControlValueInEmail(HtmlControl htmlControl)
        {
            bool validation = true;
            if (htmlControl.GetType() == typeof(HtmlInputText))
            {
                HtmlInputControl inputControl = (HtmlInputControl)htmlControl;
                string validationRules = htmlControl.Attributes["data-validation"];
                if (validationRules == null || Validate(inputControl.Value, validationRules.Split(',')))
                {
                    emailValues += "<tr><td>" + IdToLabelName(htmlControl.ID) + "</td><td>" + inputControl.Value + "</td></tr>";
                }
                else
                {
                    validation = false;
                    if (this.Messagelabel != null) this.Messagelabel.Text += IdToLabelName(htmlControl.ID) + "<br />";
                    if (this.showFieldErrorClass) inputControl.Attributes.Add("class", this.Settings["showFieldErrorClassName"].ToString());
                }
            }

            if (htmlControl.GetType() == typeof(HtmlInputCheckBox))
            {
                HtmlInputCheckBox inputCheckboxControl = (HtmlInputCheckBox)htmlControl;
                string validationRules = htmlControl.Attributes["data-validation"];
                if (validationRules == null || Validate(inputCheckboxControl.Value, validationRules.Split(',')))
                {
                    emailValues += "<tr><td>" + IdToLabelName(htmlControl.ID) + "</td><td>" + inputCheckboxControl.Checked.ToString() + "</td></tr>";
                }
                else
                {
                    validation = false;
                    if (this.Messagelabel != null) this.Messagelabel.Text += IdToLabelName(htmlControl.ID) + "<br />";
                    if (this.showFieldErrorClass) inputCheckboxControl.Attributes.Add("class", this.Settings["showFieldErrorClassName"].ToString());
                }
            }

            if (htmlControl.GetType() == typeof(HtmlInputRadioButton))
            {
                HtmlInputRadioButton inputRadioControl = (HtmlInputRadioButton)htmlControl;
                string validationRules = htmlControl.Attributes["data-validation"];
                if (validationRules == null || Validate(inputRadioControl.Value, validationRules.Split(',')))
                {
                    emailValues += "<tr><td>" + IdToLabelName(htmlControl.ID) + "</td><td>" + inputRadioControl.Value + "</td></tr>";
                }
                else
                {
                    validation = false;
                    if (this.Messagelabel != null) this.Messagelabel.Text += IdToLabelName(htmlControl.ID) + "<br />";
                    if (this.showFieldErrorClass && this.Settings["showFieldErrorClassName"].ToString() != "") inputRadioControl.Attributes.Add("class", this.Settings["showFieldErrorClassName"].ToString());
                }
            }

            if (htmlControl.GetType() == typeof(HtmlSelect))
            {
                HtmlSelect selectControl = (HtmlSelect)htmlControl;
                emailValues += "<tr><td>" + IdToLabelName(htmlControl.ID) + "</td><td>" + selectControl.Value + "</td></tr>";
            }

            if (htmlControl.GetType() == typeof(HtmlInputFile))
            {
            }

            if (htmlControl.GetType() == typeof(HtmlTextArea))
            {
                HtmlTextArea inputControl = (HtmlTextArea)htmlControl;
                string validationRules = htmlControl.Attributes["data-validation"];
                if (validationRules == null || Validate(inputControl.Value, validationRules.Split(',')))
                {
                    emailValues += "<tr><td>" + IdToLabelName(htmlControl.ID) + "</td><td><pre>" + inputControl.Value + "</pre></td></tr>";
                }
                else
                {
                    validation = false;
                    if (this.Messagelabel != null) this.Messagelabel.Text += IdToLabelName(htmlControl.ID) + "<br />";
                    if (this.showFieldErrorClass) inputControl.Attributes.Add("class", this.Settings["showFieldErrorClassName"].ToString());
                }
            }

            return validation;
        }

        private bool HandleAttachments(HtmlInputFile inputControl)
        {
            string validationRules = inputControl.Attributes["data-validation"];
            if (Validate(inputControl.Value, validationRules.Split(',')))
            {
                inputControl.PostedFile.SaveAs(Server.MapPath("") + "/_temp/" + inputControl.PostedFile.FileName);
                if (attachments == null)
                {
                    attachments = new List<string>();
                }
                attachments.Add(Server.MapPath("") + "/_temp/" + inputControl.PostedFile.FileName);
                return true;
            }
            else
            {
                if (this.Messagelabel != null) this.Messagelabel.Text += inputControl.ID + "<br />";
                return false;
            }
        }

        private void SendEmail()
        {
            string fromEmailAddress = "";
            if (this.Settings["EmailAddressFrom"].ToString() == "0") {
                fromEmailAddress = SessionObject.CurrentSite.EmailSettingsFrom;
            }

            if (this.Settings["EmailAddressFrom"].ToString() == "1")
            {
                fromEmailAddress = this.CustomerEmailAddressTextBox.Text;
            }

            if (this.Settings["EmailAddressFrom"].ToString() == "2")
            {
                fromEmailAddress = this.Settings["ContactFormSender"].ToString();
            }
            EmailManager.SendMail(fromEmailAddress, this.Settings["ContactFormRecipient"].ToString(), this.Settings["ContactFormSubject"].ToString(), emailBody, true, attachments.ToArray());

            if (this.Settings["DebugEmailAddress"].ToString() != "" && Regex.Match(this.Settings["DebugEmailAddress"].ToString(), @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success) {
                EmailManager.SendMail(this.Settings["DebugEmailAddress"].ToString(), this.Settings["ContactFormRecipient"].ToString(), this.Settings["ContactFormSubject"].ToString(), emailBody, true, attachments.ToArray());
            }
        }

        private bool Validate(string value, string[] validations)
        {
            bool validationResult = true;
            foreach (string validation in validations)
            {
                switch (validation)
                {
                    case "required":
                        validationResult = (value.Trim() != "");
                        break;

                    case "email":
                        validationResult = Regex.IsMatch(value.Trim(), @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                        break;

                    case "number":
                        validationResult = value.IsNummeric();
                        break;

                    default:
                        break;
                }
                if (!validationResult && (value.Trim() != "" || validations.Contains("required"))) return validationResult;
            }
            return validationResult;
        }

        private void showEmailSend()
        {
            ModuleNavigationActionLite action = GetFirstNavigationAction();
            if (action.NavigationType == NavigationTypeEnum.NavigateToPage)
            {
                Response.Redirect(action.NavigationUrl);
            }
            else //if (action.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {
                //if (this.CustomerEmailAddressTextBox != null) this.CustomerEmailAddressTextBox.Visible = false;
                if (this.ButtonSubmit != null) this.ButtonSubmit.Visible = false;
                if (this.LinkButtonSubmit != null) this.LinkButtonSubmit.Visible = false;
                if (this.Messagelabel != null) this.Messagelabel.Text = this.Settings["SendCompleteMessage"].ToString();
                /* foreach (HtmlControl htmlControl in this.CustomInputs)
                {
                    htmlControl.Visible = false;
                } */
            }
        }

        private string IdToLabelName(string Id)
        {
            return Id.Replace(this.ModuleID.ToString("N"), "").Replace("__", " ").Replace("___", "-");
        }
    }
}