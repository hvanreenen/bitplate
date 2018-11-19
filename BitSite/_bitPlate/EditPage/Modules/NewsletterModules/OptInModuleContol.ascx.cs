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
    public partial class OptInModuleControl : BaseModuleUserControl
    {
        protected HtmlControl errorTemplate;
        protected HtmlControl succesTemplate;

        

        private BaseCollection<NewsletterGroup> NewsletterGroups;
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Load(sender, e);
            if (base.CheckAutorisation())
            {
                this.Load();

               
                if (this.errorTemplate != null) this.errorTemplate.Visible = false;
                if (this.succesTemplate != null) this.succesTemplate.Visible = false;

                if (Request.QueryString.AllKeys.Contains("subscriber"))
                {
                    this.VerificateEmailAddress(Request.QueryString["subscriber"]);
                }
                else
                {
                    if (this.errorTemplate != null)  this.errorTemplate.Visible = true;
                    this.LabelMsg.Text = "Geen geldig subscriber ID in parameter 'subscriber'.";
                }
            }
        }

        public override void Reload(BaseModuleUserControl sender, NavigationParameterObject args = null)
        {
            //Skippen deze module kan niets met een navigatie acties.
        }

        public void Load()
        {
                this.succesTemplate = (HtmlControl)this.FindControl("SuccessVerificationTemplate" + this.ModuleID.ToString("N"));
                this.errorTemplate = (HtmlControl)this.FindControl("ErrorVerificationTemplate" + this.ModuleID.ToString("N"));
        }




        private void SendVerificationEmail(NewsletterSubscriber subscriber)
        {
            CmsSite site = SessionObject.CurrentSite;
            string content = site.NewsletterOptInEmailContent;
            content = content.Replace("[OPTINURL]", site.DomainName + "/" + site.NewsletterOptInEmailPage.LastPublishedUrl + "?svid=" + subscriber.ID.ToString());
            EmailManager.SendMail(site.NewsletterSender, subscriber.Email, site.NewsletterOptInEmailSubject, content, true);
        }

        private void VerificateEmailAddress(string id)
        {
            BaseCollection<NewsletterSubscriber> subscribers = BaseCollection<NewsletterSubscriber>.Get("ID = '" + id + "'");
            if (subscribers.Count == 1)
            {
                NewsletterSubscriber subscriber = subscribers[0];
                subscriber.Confirmed = true;
                subscriber.Save();
                this.Load();
                if (this.succesTemplate != null) this.succesTemplate.Visible = true;
            }
            else
            {
                if (this.errorTemplate != null) this.errorTemplate.Visible = true;
            }
        }
    }
}