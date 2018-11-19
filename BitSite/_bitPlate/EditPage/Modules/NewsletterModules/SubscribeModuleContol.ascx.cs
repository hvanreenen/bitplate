using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.Modules;
using BitSite._bitPlate.bitAjaxServices;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Utils;
using BitPlate.Domain.Modules.Auth;
using BitPlate.Domain.Modules.Newsletter;
using BitPlate.Domain.Newsletters;
using System.Web.UI.HtmlControls;
using BitSite._bitPlate.Newsletters;

namespace BitSite._bitPlate.EditPage.Modules.NewsletterModules
{
    public partial class SubscribeModuleControl : BaseInputModuleUserControl
    {
        protected TextBox TextboxEmail;
        protected TextBox TextboxForeName;
        protected TextBox TextboxNamePrefix;
        protected TextBox TextboxName;
        protected TextBox TextboxCompany;

        protected RadioButton RadioSexeMale;
        protected RadioButton RadioSexeFemale;
        protected RadioButton RadioSexeCompany;

        protected CheckBoxList NewsGroupCheckBoxList;
        protected LinkButton LinkSubmitButton;
        protected Button SubmitButton;
        protected LinkButton ResendVerificationButton;

        protected HtmlControl PanelRegisterFrom;
        protected HtmlControl PanelErrorRegister;
        protected HtmlControl PanelSuccessRegister;

        protected HtmlControl PanelVerificationRequest;

        

        private BaseCollection<NewsletterGroup> NewsletterGroups;
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Load(sender, e);
            if (base.CheckAutorisation())
            {
                this.Load();

                if (this.PanelRegisterFrom != null) this.PanelRegisterFrom.Visible = true;
                if (this.PanelErrorRegister != null) this.PanelErrorRegister.Visible = false;
                if (this.PanelSuccessRegister != null) this.PanelSuccessRegister.Visible = false;
                if (this.PanelVerificationRequest != null) this.PanelVerificationRequest.Visible = false;
            }
        }

        public override void Reload(BaseModuleUserControl sender, NavigationParameterObject args = null)
        {
            if (base.CheckAutorisation())
            {
                this.Load();
                if (this.PanelRegisterFrom != null) this.PanelRegisterFrom.Visible = true;
            }
        }

        public void Load()
        {
                
                this.NewsletterGroups = BaseCollection<NewsletterGroup>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "'");

                TextboxEmail = (TextBox)this.FindControl("TextboxEmail" + this.ModuleID.ToString("N"));
                TextboxForeName = (TextBox)this.FindControl("TextboxForeName" + this.ModuleID.ToString("N"));
                TextboxName = (TextBox)this.FindControl("TextboxName" + this.ModuleID.ToString("N"));
                TextboxNamePrefix = (TextBox)this.FindControl("TextboxNamePrefix" + this.ModuleID.ToString("N"));
                TextboxCompany = (TextBox)this.FindControl("TextboxCompany" + this.ModuleID.ToString("N"));

                RadioSexeCompany = (RadioButton)this.FindControl("RadioSexeCompany" + this.ModuleID.ToString("N"));
                RadioSexeFemale = (RadioButton)this.FindControl("RadioSexeFemale" + this.ModuleID.ToString("N"));
                RadioSexeMale = (RadioButton)this.FindControl("RadioSexeMale" + this.ModuleID.ToString("N"));

                NewsGroupCheckBoxList = (CheckBoxList)this.FindControl("NewsGroupenCheckBoxList" + this.ModuleID.ToString("N"));
                LinkSubmitButton = (LinkButton)this.FindControl("SubmitLinkButton" + this.ModuleID.ToString("N"));
                SubmitButton = (Button)this.FindControl("SubmitButton" + this.ModuleID.ToString("N"));
                ResendVerificationButton = (LinkButton)this.FindControl("ResendVerificationButton" + this.ModuleID.ToString("N"));
                this.PanelRegisterFrom = (HtmlControl)this.FindControl("PanelRegisterFrom" + this.ModuleID.ToString("N"));
                this.PanelSuccessRegister = (HtmlControl)this.FindControl("PanelSuccessRegister" + this.ModuleID.ToString("N"));
                this.PanelErrorRegister = (HtmlControl)this.FindControl("PanelErrorRegister" + this.ModuleID.ToString("N"));
                this.PanelVerificationRequest = (HtmlControl)this.FindControl("PanelVerificationRequest" + this.ModuleID.ToString("N"));  


                if (TextboxEmail == null)
                {
                    if (this.LabelMsg != null)  this.LabelMsg.Text += "Plaats de tag {TextboxEmailAddress}.<br />";
                    this.TextboxEmail = new TextBox();
                }

                this.LoadSettings();

                this.SetNewsletterMode();

                if (LinkSubmitButton != null)
                {
                    LinkSubmitButton.Click += SubmitButton_Click;
                }

                if (SubmitButton != null)
                {
                    SubmitButton.Click += SubmitButton_Click;
                }

                if (ResendVerificationButton != null)
                {
                    ResendVerificationButton.Click += ResendVerificationButton_Click;
                }
        }

        public void ResendVerificationButton_Click(object sender, EventArgs e)
        {
            BaseCollection<NewsletterSubscriber> subscribers = BaseCollection<NewsletterSubscriber>.Get("Name = '" + TextboxEmail.Text.Trim() + "'");
            if (subscribers.Count > 0)
            {
                NewsletterService.SendVerificationEmail(subscribers[0]);
            }
        }

        public void SubmitButton_Click(object sender, EventArgs e)
        {
            if ((TextboxEmail.Text.Trim() != "" && BitPlate.Domain.Utils.EmailManager.isValidEmailAddress(TextboxEmail.Text.Trim())) || (TextboxEmail.Text.Trim() == "info@bitplate.local"))
            {
                BaseCollection<NewsletterSubscriber> subscribers = BaseCollection<NewsletterSubscriber>.Get("Name = '" + TextboxEmail.Text.Trim() + "'");
                if (subscribers.Count > 0)
                {
                   //if (this.PanelErrorRegister != null) this.PanelErrorRegister.Visible = true;
                    this.ErrorPanel.Visible = true;
                   this.ErrorLabel.Text = "Dit emailadres is al geabonneerd.";
                }
                else
                {
                    NewsletterSubscriber subscriber = new NewsletterSubscriber();
                    subscriber.Email = TextboxEmail.Text.Trim();
                    subscriber.Name = (this.TextboxName != null) ? this.TextboxName.Text : "";
                    subscriber.NamePrefix = (this.TextboxNamePrefix != null) ? this.TextboxNamePrefix.Text : "";
                    subscriber.ForeName = (this.TextboxForeName != null) ? this.TextboxForeName.Text : "";
                    subscriber.CompanyName = (this.TextboxCompany != null) ? this.TextboxCompany.Text : "";

                    if (RadioSexeMale != null && RadioSexeMale.Checked)
                    {
                        subscriber.Gender = BaseUser.SexeEnum.Male;
                    }

                    if (RadioSexeFemale != null && RadioSexeFemale.Checked)
                    {
                        subscriber.Gender = BaseUser.SexeEnum.Female;
                    }

                    if (RadioSexeCompany != null && RadioSexeCompany.Checked)
                    {
                        subscriber.Gender = BaseUser.SexeEnum.Company;
                    }

                    //simple user link
                    //BaseCollection<SiteUser> Users = BaseCollection<SiteUser>.Get("Email = '" + subscriber.Email + "'");
                    //if (Users.Count > 0)
                    //{
                    //    subscriber.User = Users[0];
                    //}
                    subscriber.Save();

                    foreach (NewsletterGroup group in this.NewsletterGroups.Where(c => c.IsMandatoryGroup)) {
                        subscriber.SubscribedGroups.Add(group);
                    }

                    foreach (ListItem li in this.NewsGroupCheckBoxList.Items)
                    {
                        if (li.Selected)
                        {
                            Guid ID;
                            Guid.TryParse(li.Value, out ID);
                            NewsletterGroup group = BaseObject.GetById<NewsletterGroup>(ID);
                            subscriber.SubscribedGroups.Add(group);
                        }
                    }

                    subscriber.Save();

                    if (subscriber.SubscribedGroups.Count > 0)
                    {
                        this.TextboxEmail.Visible = false;
                        this.LoadSettings();
                        //Koppel siteuser met subscriber (als aanwezig)
                        //SiteUser user = BaseObject.GetFirst<SiteUser>("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "' AND Email = '" + subscriber.Email + "'");
                        //if (user != null)
                        //{
                        //    subscriber.User = user;
                        //    subscriber.Save();
                        //}

                        if (this.PanelRegisterFrom != null) this.PanelRegisterFrom.Visible = false;
                        if (this.Settings.ContainsKey("SendVerifiationEmail") && (bool)this.Settings["SendVerifiationEmail"])
                        {
                            NewsletterService.SendVerificationEmail(subscriber);
                            this.LoadModuleObject();
                            //if (this.PanelVerificationRequest != null) this.PanelVerificationRequest.Visible = true;
                            this.SuccessPanel.Visible = true;
                            this.SuccessLabel.Text = "Wij hebben u een verificatie email verstuurd. In deze email zit een link om uw emailadres te activeren.";
                        }
                        else
                        {
                            //if (this.PanelSuccessRegister != null) this.PanelSuccessRegister.Visible = true;
                            this.SuccessPanel.Visible = true;
                            this.SuccessLabel.Text = "U ontvangt vanaf nu onze nieuwsbrief.";
                            subscriber.Confirmed = true;
                            subscriber.Save();
                        }
                        this.HandleNavigation();
                    }
                    else
                    {
                        //if (this.PanelErrorRegister != null) this.PanelErrorRegister.Visible = true;
                        this.ErrorPanel.Visible = true;
                        this.ErrorLabel.Text = "U heeft geen interessegebied(en) geselecteerd.";
                        subscriber.Delete();
                    }
                }
            }
            else
            {
                this.ErrorPanel.Visible = true;
                this.ErrorLabel.Text = "Dit emailadres is ongeldig";
                //if (this.PanelErrorRegister != null) this.PanelErrorRegister.Visible = true;
            }
        }

        private void HandleNavigation()
        {
            ModuleNavigationActionLite navigationAction = GetNavigationActionByTagName("{SubmitButton}");

            switch (navigationAction.NavigationType)
            {
                case NavigationTypeEnum.NavigateToPage:
                    Response.Redirect(navigationAction.NavigationUrl);
                    break;
                case NavigationTypeEnum.ShowDetailsInModules:
                    foreach (string drillDownModuleId in navigationAction.RefreshModules)
                    {
                        BaseModuleUserControl moduleControl = (BaseModuleUserControl)FindControlRecursive(this.Page.Master, "Mod" + drillDownModuleId.Replace("-", ""));
                        if (moduleControl != null)
                        {
                            moduleControl.Reload(this);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetNewsletterMode()
        {
            

            if (NewsGroupCheckBoxList == null)
            {
                if (!this.Settings.Keys.Contains("Newsgroup") || (this.Settings.Keys.Contains("Newsgroup") && this.Settings["Newsgroup"].ToString() == "choose"))
                {
                    if (this.LabelMsg != null) this.LabelMsg.Text += "Plaats de tag {NewsGroupCheckboxList}.<br />";
                    this.NewsGroupCheckBoxList = new CheckBoxList();
                }
            }
            else
            {
                if (((this.Settings.Keys.Contains("Newsgroup") && this.Settings["Newsgroup"].ToString() == "choose") || !this.Settings.Keys.Contains("Newsgroup")) && this.NewsGroupCheckBoxList.Items.Count == 0)
                {
                    foreach (NewsletterGroup group in this.NewsletterGroups)
                    {
                        if (group.IsChoosableGroup)
                        {
                            ListItem item = new ListItem();
                            item.Text = group.Name;
                            item.Value = group.ID.ToString();
                            this.NewsGroupCheckBoxList.Items.Add(item);
                        }
                    }
                    this.NewsGroupCheckBoxList.Visible = true;
                }
                else
                {
                    if (this.Settings.Keys.Contains("Newsgroup") && this.Settings["Newsgroup"].ToString() != "choose")
                    {
                        this.NewsGroupCheckBoxList.Visible = false;
                    }
                    else
                    {
                        this.NewsGroupCheckBoxList.Visible = true;
                    }
                }
            }
        }
    }
}