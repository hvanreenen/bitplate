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
    public partial class ContactFormModuleControl : BaseModuleUserControl
    {
        protected TextBox CustomerEmailAddressTextBox;
        protected Panel PanelErrorMessage;
        protected Panel PanelSuccesMessage;
        protected Label Messagelabel;

        
        private string emailBody;
     
        private List<string> attachments = new List<string>();

        //private bool showFieldErrorMessageInLabel;
        //private bool showFieldErrorClass;

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Load(sender, e);
            if (base.CheckAutorisation())
            {
                
                this.LoadSettings();
                //if (this.Settings != null && this.Settings.ContainsKey("showFieldErrorMessageInLabel"))
                //{
                //    this.showFieldErrorMessageInLabel = (bool)this.Settings["showFieldErrorMessageInLabel"];
                //    this.showFieldErrorClass = (bool)this.Settings["showFieldErrorClass"];
                //}
                string formValues = "";
                NameValueCollection col = Request.Form;
                foreach (string key in col.AllKeys)
                {
                    if (key != null && key.StartsWith(this.ModuleID.ToString()))
                    {
                        formValues += "<tr>";
                        if (key.Contains("CustomerEmailAddressTextBox")) {
                            formValues += "<td>Emailadres</td>" + "<td>" + col[key] + "</td>";
                            this.CustomerEmailAddressTextBox = (TextBox)this.FindControl("CustomerEmailAddressTextBox" + this.ModuleID.ToString("N"));
                        }
                        else {
                            formValues += "<td>" + key.Replace(this.ModuleID.ToString(), "") + "</td><td>" + col[key] + "</td>";
                        }
                        formValues += "</tr>";
                    }
                }
                HttpFileCollection files = Request.Files;
                foreach (string key in files.AllKeys)
                {
                    if (key.StartsWith(this.ModuleID.ToString()) && files[key].FileName != "")
                    {
                        //save file
                        string fileName = Server.MapPath("") + @"\_temp\" + files[key].FileName;
                        files[key].SaveAs(fileName);
                        attachments.Add(fileName);
                    }
                }
                if (formValues != "")
                {
                    formValues = "<table>" + formValues + "</table>";
                    emailBody = getSetting<string>("ContactFormTemplate");
                    if (emailBody == null || emailBody == "") emailBody = "Resultaten ContactFormulier: <br/><br/>[FormResult]";
                    emailBody = emailBody.Replace("[FormResult]", formValues);

                    SendEmail();
                    showEmailSend();
                    if (this.PanelSuccesMessage != null) this.PanelSuccesMessage.Visible = true;
                }
                else
                {
                    if (this.PanelErrorMessage != null) this.PanelErrorMessage.Visible = true;
                }
                
            }
        }

        public void ButtonSubmit_Click(object sender, EventArgs e)
        {
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

            if (this.Settings["BCCEmailAddress"].ToString() != "" && Regex.Match(this.Settings["BCCEmailAddress"].ToString(), @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                EmailManager.SendMail(fromEmailAddress, this.Settings["ContactFormRecipient"].ToString(), this.Settings["ContactFormSubject"].ToString(), emailBody, true, attachments.ToArray(), this.Settings["DebugEmailAddress"].ToString());
            }
            else
            {
                EmailManager.SendMail(fromEmailAddress, this.Settings["ContactFormRecipient"].ToString(), this.Settings["ContactFormSubject"].ToString(), emailBody, true, attachments.ToArray());
            }
        }

        
        private void showEmailSend()
        {
            ModuleNavigationActionLite action = GetFirstNavigationAction();
            Dictionary<string, object> DataObject = new Dictionary<string,object>();
            DataObject.Add("NavigationType", action.NavigationType);
            DataObject.Add("NavigationUrl", action.NavigationUrl);
            Response.Clear();
            Response.Write(JsonResult.CreateResult(true, DataObject).ToJsonString());
            Response.End();

            /* ModuleNavigationActionLite action = GetFirstNavigationAction();
            if (action.NavigationType == NavigationTypeEnum.NavigateToPage)
            {
                

                Response.Redirect(action.NavigationUrl);
            }
            else 
            {
                if (this.Messagelabel != null)
                {
                    this.Messagelabel.Text = this.Settings["SendCompleteMessage"].ToString();
                }
                
            } */
        }
    }
}