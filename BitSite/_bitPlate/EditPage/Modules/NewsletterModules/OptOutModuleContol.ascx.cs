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

namespace BitSite._bitPlate.EditPage.Modules.NewsletterModules
{
    public partial class OptOutModuleControl : BaseModuleUserControl
    {
        protected Button EndSubscriptionButton;
        protected Button AcceptEndSubscriptionButton;
        protected Button CancelEndSubscriptionButton;
        protected CheckBoxList NewsGroupCheckBoxList;
        protected LinkButton LinkSubmitButton;
        protected Button SubmitButton;
        protected Label ResultLabel;
        protected HtmlControl PanelSubscriberFrom;
        protected HtmlControl PanelEndSubscriptionConfirmationDialog;
        protected HtmlControl PanelSubscriptionDeleted;

        protected HtmlControl PanelSubscriptionChangeSaved;

        private BaseCollection<NewsletterGroup> NewsletterGroups;

        private NewsletterSubscriber Subscriber;
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Load(sender, e);
            if (base.CheckAutorisation())
            {
                if (Request.QueryString.AllKeys.Contains("subscriber"))
                {
                    Guid ID;
                    Guid.TryParse(Request.QueryString["subscriber"].ToString(), out ID);
                    if (ID != Guid.Empty)
                    {
                        this.Subscriber = BaseObject.GetById<NewsletterSubscriber>(ID);
                        if (this.Subscriber == null)
                        {
                            this.LabelMsg.Text = "Parameter subscriber wordt niet herkend.";
                        }

                    }
                }
                else
                {
                    this.LabelMsg.Text = "Parameter subscriber ontbreekt.";
                }

                this.LoadSettings();
                this.LoadControls();

                if (this.PanelSubscriberFrom != null) this.PanelSubscriberFrom.Visible = false;
                if (this.PanelEndSubscriptionConfirmationDialog != null) this.PanelEndSubscriptionConfirmationDialog.Visible = false;
                if (this.PanelSubscriptionDeleted != null) this.PanelSubscriptionDeleted.Visible = false;
                if (this.PanelSubscriptionChangeSaved != null) this.PanelSubscriptionChangeSaved.Visible = false;

                if (this.Subscriber != null)
                {
                    if (this.PanelSubscriberFrom != null) this.PanelSubscriberFrom.Visible = true;
                }
            }
        }

        public override void Reload(BaseModuleUserControl sender, NavigationParameterObject args = null)
        {
            if (base.CheckAutorisation())
            {
                this.LoadControls();
                //if (this.PanelSubscriberFrom != null) this.PanelSubscriberFrom.Visible = true;
            }
        }

        public void LoadControls()
        {
                
                this.NewsletterGroups = BaseCollection<NewsletterGroup>.Get("FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "'");

                NewsGroupCheckBoxList = (CheckBoxList)this.FindControl("NewsGroupenCheckBoxList" + this.ModuleID.ToString("N"));
                LinkSubmitButton = (LinkButton)this.FindControl("SubmitLinkButton" + this.ModuleID.ToString("N"));
                SubmitButton = (Button)this.FindControl("SubmitButton" + this.ModuleID.ToString("N"));
                ResultLabel = (Label)this.FindControl("MessageLabel" + this.ModuleID.ToString("N"));
                EndSubscriptionButton = (Button)this.FindControl("EndSubscriptionButton" + this.ModuleID.ToString("N"));
                AcceptEndSubscriptionButton = (Button)this.FindControl("AcceptEndSubscriptionButton" + this.ModuleID.ToString("N"));
                CancelEndSubscriptionButton = (Button)this.FindControl("CancelEndSubscriptionButton" + this.ModuleID.ToString("N"));

                if (this.AcceptEndSubscriptionButton != null) this.AcceptEndSubscriptionButton.Click += AcceptEndSubscriptionButton_Click;
                if (this.CancelEndSubscriptionButton != null) this.CancelEndSubscriptionButton.Click += CancelEndSubscriptionButton_Click;
                if (this.EndSubscriptionButton != null) this.EndSubscriptionButton.Click += EndSubscriptionButton_Click;
                                    
                                    /* {PanelEndSubscriptionConfirmationDialog}Weet u zeker dat u onze nieuwsbrief niet meer wil ontvangen?<br />
                                        {AcceptEndAbonnementButton}Ja{/AcceptEndAbonnementButton}{CancelEndAbonnementButton}Nee{/CancelAbonnementButton}
{/PanelEndSubscriptionConfirmationDialog}
                                    {PanelSubscriptionDeleted}Uw abonnement is verwijderd.{/PanelSubscriptionDeleted}
                                    {PanelSubscriptionChangeSaved}Abonnementwijzigingen zijn opgeslagen.{/PanelSubscriptionChangeSaved} */


                this.PanelSubscriberFrom = (HtmlControl)this.FindControl("PanelSubscriberFrom" + this.ModuleID.ToString("N"));
                this.PanelEndSubscriptionConfirmationDialog = (HtmlControl)this.FindControl("PanelEndSubscriptionConfirmationDialog" + this.ModuleID.ToString("N"));
                this.PanelSubscriptionDeleted = (HtmlControl)this.FindControl("PanelSubscriptionDeleted" + this.ModuleID.ToString("N"));
                this.PanelSubscriptionChangeSaved = (HtmlControl)this.FindControl("PanelSubscriptionChangeSaved" + this.ModuleID.ToString("N"));

                if (this.ResultLabel != null) this.ResultLabel.Text = "";

                if (NewsGroupCheckBoxList == null)
                {
                    if (this.LabelMsg != null && this.getSetting<bool>("ShowNewsGroupList")) this.LabelMsg.Text += "Plaats de tag {NewsGroupCheckboxList}.<br />";
                    this.NewsGroupCheckBoxList = new CheckBoxList();
                    if (this.SubmitButton != null) this.SubmitButton.Visible = false;
                }
                else
                {
                    if (this.NewsletterGroups.Count > 1 && this.NewsGroupCheckBoxList.Items.Count == 0 && this.getSetting<bool>("ShowNewsGroupList"))
                    {
                        foreach (NewsletterGroup group in this.NewsletterGroups)
                        {
                            if (group.IsChoosableGroup)
                            {
                                ListItem item = new ListItem();
                                item.Text = group.Name;
                                item.Value = group.ID.ToString();
                                item.Selected = (this.Subscriber != null && this.Subscriber.SubscribedGroups.Any(c => c.ID == group.ID)) ? true : false;
                                this.NewsGroupCheckBoxList.Items.Add(item);
                            }
                        }
                    }
                    else
                    {
                        if (this.SubmitButton != null) this.SubmitButton.Visible = false;
                    }
                }

                if (LinkSubmitButton != null)
                {
                    LinkSubmitButton.Click += SubmitButton_Click;
                }

                if (SubmitButton != null)
                {
                    SubmitButton.Click += SubmitButton_Click;
                }
        }

        public void EndSubscriptionButton_Click(object sender, EventArgs e)
        {
            if (this.PanelEndSubscriptionConfirmationDialog != null) this.PanelEndSubscriptionConfirmationDialog.Visible = true;
            if (this.PanelSubscriberFrom != null) this.PanelSubscriberFrom.Visible = false;
        }

        public void CancelEndSubscriptionButton_Click(object sender, EventArgs e)
        {
            if (this.PanelSubscriberFrom != null) this.PanelSubscriberFrom.Visible = true;
        }

        public void AcceptEndSubscriptionButton_Click(object sender, EventArgs e)
        {
            this.LoadSettings();
            if (this.getSetting<bool>("SendOptOutEmail"))
            {
                EmailManager.SendMail(SessionObject.CurrentSite.NewsletterSender, Subscriber.Email, SessionObject.CurrentSite.NewsletterOptOutEmailSubject, SessionObject.CurrentSite.NewsletterOptOutEmailContent, true);
            }

            if (this.PanelSubscriptionDeleted != null) this.PanelSubscriptionDeleted.Visible = true;
            if (this.PanelSubscriberFrom != null) this.PanelSubscriberFrom.Visible = false;

            if (this.Subscriber != null)
            {
                this.Subscriber.Unsubscribe();
            }
            this.HandleNavigation();
        }

        public void SubmitButton_Click(object sender, EventArgs e)
        {
            if (this.Subscriber != null && this.NewsGroupCheckBoxList != null)
            {
                this.Subscriber.SubscribedGroups.Clear();

                foreach (ListItem li in this.NewsGroupCheckBoxList.Items)
                {
                    if (li.Selected)
                    {
                        Guid ID;
                        Guid.TryParse(li.Value, out ID);
                        NewsletterGroup group = BaseObject.GetById<NewsletterGroup>(ID);
                        this.Subscriber.SubscribedGroups.Add(group);
                    }
                }

                foreach (NewsletterGroup group in this.NewsletterGroups)
                {
                    if (group.IsMandatoryGroup)
                    {
                        this.Subscriber.SubscribedGroups.Add(group);
                    }
                }

                this.Subscriber.Save();
            }
            if (this.PanelSubscriptionChangeSaved != null) this.PanelSubscriptionChangeSaved.Visible = true;
        }

        private void HandleNavigation()
        {
            ModuleNavigationActionLite navigationAction = GetNavigationActionByTagName("{AcceptEndSubscriptionButton}");

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

        private void SendVerificationEmail(NewsletterSubscriber subscriber)
        {
            CmsSite site = SessionObject.CurrentSite;
            string content = site.NewsletterOptInEmailContent;
            content = content.Replace("[verificatielink]", "<a href=\"" + site.DomainName + "/"  + site.NewsletterOptInEmailPage.LastPublishedUrl + "?svid=" + subscriber.ID.ToString() + "\" >").Replace("[/verificatielink]", "</a>");
            EmailManager.SendMail(site.NewsletterSender, subscriber.Email, site.NewsletterOptInEmailSubject, content, true);
        }
    }
}