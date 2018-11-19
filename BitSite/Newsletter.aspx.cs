using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BitPlate.Domain.Newsletters;
using BitPlate.Domain.Autorisation;

namespace BitSite
{
    public partial class NewsletterPage : System.Web.UI.Page
    {
        public string NewsletterID { get; set; }
        public string MailingID { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["newsletterid"] != null)
            {
                NewsletterID = Request.QueryString["newsletterid"];
            }

            if (Request.QueryString["mailing"] != null)
            {
                MailingID = Request.QueryString["mailing"];
            }

            Newsletter newsletter = BaseObject.GetById<Newsletter>(new Guid(NewsletterID));
            if (newsletter == null)
            {
                throw new Exception("Geen nieuwsbrief geladen met id: " + NewsletterID);
            }

            //je kunt niet zien of een pagina in editmode zit, want zit in iframe
            //hierom kijken we of er een bitplateuser is ingelogd. 
            //zo ja, dan wordt er geen check gedaan op site-autorisatie en of de pagina actief is.
            bool allowEdit = (SessionObject.CurrentBitplateUser != null);
            if (allowEdit)
            {
                if (!CheckBitplateAutorisation(newsletter))
                {
                    throw new Exception("U heeft geen rechten om deze nieuwsbrief te bewerken.");
                }
            }
            else
            {
                if (!CheckIfActive(newsletter))
                {
                    throw new Exception("Deze nieuwsbrief is momenteel niet actief.");
                }

                if (!CheckSiteAutorisation(newsletter))
                {
                    throw new Exception("U heeft geen rechten op deze nieuwsbrief.");
                }
            }
            string newsletterHtml; 

            Guid mailingId = Guid.Empty;
            Guid.TryParse(this.MailingID, out mailingId);
            if (mailingId != Guid.Empty)
            {
                newsletterHtml = newsletter.Publish2(NewsletterMode.Online, BaseObject.GetById<NewsletterMailing>(mailingId));
            }
            else
            {
                newsletterHtml = newsletter.Publish2();
            }
             

            Response.Write(newsletterHtml);


        }


        private bool CheckIfActive(Newsletter page)
        {
            return page.IsActive;
        }

        private bool CheckSiteAutorisation(Newsletter page)
        {
            bool isAutorized = true;
            if (page.HasAutorisation)
            {
                isAutorized = false;
                string autorizedSiteUserGroupIDs = "", autorizedSiteUserIDs = "";
                foreach (SiteUserGroup userGroup in page.AutorizedSiteUserGroups)
                {
                    autorizedSiteUserGroupIDs += userGroup.ID + ",";
                }
                foreach (SiteUser user in page.AutorizedSiteUsers)
                {
                    autorizedSiteUserIDs += user.ID + ",";
                }

                if (SessionObject.CurrentSiteUser != null)
                {
                    isAutorized = SessionObject.CurrentSiteUser.IsAutorized(autorizedSiteUserGroupIDs, autorizedSiteUserIDs);
                }
            }
            return isAutorized;
        }

        private bool CheckBitplateAutorisation(Newsletter page)
        {
            bool isAutorized = true;
            if (page.HasAutorisation)
            {
                isAutorized = false;
                string autorizedBitplateUserGroupIDs = "", autorizedBitplateUserIDs = "";
                foreach (BitplateUserGroup userGroup in page.AutorizedBitplateUserGroups)
                {
                    autorizedBitplateUserGroupIDs += userGroup.ID + ",";
                }
                foreach (BitplateUser user in page.AutorizedBitplateUsers)
                {
                    autorizedBitplateUserIDs += user.ID + ",";
                }

                if (SessionObject.CurrentSiteUser != null)
                {
                    isAutorized = SessionObject.CurrentBitplateUser.IsAutorized(autorizedBitplateUserGroupIDs, autorizedBitplateUserIDs);
                }
            }
            return isAutorized;
        }
    }
}