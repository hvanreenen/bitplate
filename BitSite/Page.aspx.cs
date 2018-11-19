using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MySql.Data;

using HJORM;
using BitPlate.Domain;
using System.Text.RegularExpressions;
using System.Configuration;
using MySql.Data.MySqlClient;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Utils;
using System.Web.SessionState;
using BitPlate.Domain.Newsletters;
using BitPlate.Domain.DataCollections;

namespace BitSite
{
    public partial class Page : System.Web.UI.Page, IRequiresSessionState
    {
        public string PageID { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["pageid"] != null)
            {
                PageID = Request.QueryString["pageid"];
            }
            CmsPage page = BaseObject.GetById<CmsPage>(new Guid(PageID));
            if (page == null)
            {
                throw new Exception("Geen pagina geladen met id: " + PageID);
            }
           
            //je kunt niet zien of een pagina in editmode zit, want zit in iframe
            //hierom kijken we of er een bitplateuser is ingelogd. 
            //zo ja, dan wordt er geen check gedaan op site-autorisatie en of de pagina actief is.
            bool allowEdit = (SessionObject.CurrentBitplateUser != null);
            if (allowEdit)
            {
                if (!CheckBitplateAutorisation(page))
                {
                    throw new Exception("U heeft geen rechten om deze pagina te bewerken.");
                }
            }
            else
            {
                if (!CheckIfActive(page))
                {
                    throw new Exception("Deze pagina is momenteel niet actief.");
                }

                if (!CheckSiteAutorisation(page))
                {
                    throw new Exception("U heeft geen rechten op deze pagina.");
                }
            }

            if (Request.Form["hiddenIFramePost"] != null)
            {
                string id = Request.Form["hiddenModuleID"].ToString();
                BaseModule module = BaseObject.GetById<BaseModule>(new Guid(id));
                IPostableModule postableModule = module.ConvertToType() as IPostableModule;
                PostResult postResult = postableModule.HandlePost(page, CollectionsHelper.ConvertFormParametersToDictionary(Request.Form));
                Response.Clear();
                Response.Write(postResult.ToJsonString());
                Response.Flush();
                Response.End();
            }

            if (Request.QueryString.AllKeys.Contains("mailing") && Request.QueryString["mailing"] != "")
            {
                Guid mailingId = Guid.Empty;
                Guid.TryParse(Request.QueryString["mailing"], out mailingId);
                if (mailingId != Guid.Empty)
                {
                    RegisterMailingLink(mailingId, page);
                }
            }

            if (!IsPostBack)
            {
                //if (SessionObject.CurrentLicense != null && SessionObject.CurrentLicense.IsValid)
                //{
                    string pageHtml = page.Publish2();
                    Response.Write(pageHtml);
                //}
                //else
                //{
                //    Response.Write("<div style=\"width: 350px; padding-top: 150px; margin: auto; font-size: 24px; text-align: center;\">Uw bitplate licentie is ongeldig!!</div>");
                //}
            }
        }

        


        private bool CheckIfActive(CmsPage page)
        {
            return page.IsActive;
        }

        private bool CheckSiteAutorisation(CmsPage page)
        {
            bool isAutorized = true;
            if (page.HasSiteAutorisation())
            {
                isAutorized = false;
                //string autorizedSiteUserGroupIDs = "", autorizedSiteUserIDs = "";
                //foreach(SiteUserGroup userGroup in page.AutorizedSiteUserGroups){
                //    autorizedSiteUserGroupIDs += userGroup.ID + ",";
                //}
                //foreach (SiteUser user in page.AutorizedSiteUsers)
                //{
                //    autorizedSiteUserIDs += user.ID + ",";
                //}

                if (SessionObject.CurrentSiteUser != null)
                {
                    //isAutorized = SessionObject.CurrentSiteUser.IsAutorized(autorizedSiteUserGroupIDs, autorizedSiteUserIDs);
                    isAutorized = SessionObject.CurrentSiteUser.IsAutorized(page);
                }
            }
            return isAutorized;
        }

        private bool CheckBitplateAutorisation(CmsPage page)
        {
            bool isAutorized = true;
            if (page.HasBitplateAutorisation())
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

                if (SessionObject.CurrentBitplateUser != null)
                {
                    //als er geen bitplategroepen en users zijn ingesteld ( dan is er alleen site authorisatie en geen bitplate authorisatie)  
                    if (autorizedBitplateUserGroupIDs == "" && autorizedBitplateUserIDs == "")
                    {
                        isAutorized = true;
                    }
                    else
                    {
                        isAutorized = SessionObject.CurrentBitplateUser.IsAutorized(autorizedBitplateUserGroupIDs, autorizedBitplateUserIDs);
                    }
                }
            }
            return isAutorized;
        }
        private string PerformanceTest()
        {
            string pageHtml = "";
            DateTime dt3 = DateTime.Now;
            //10 keer laden
            for (int i = 0; i < 10; i++)
            {
                CmsPage page = BaseObject.GetById<CmsPage>(new Guid(PageID));
                if (page == null)
                {
                    throw new Exception("Geen pagina geladen met id: " + PageID);
                }
                pageHtml = page.Publish2();
            }
            DateTime dt4 = DateTime.Now;
            TimeSpan ts2 = dt4.Subtract(dt3);
            double millisec2 = ts2.TotalMilliseconds;
            return pageHtml;
        }


        private void RegisterMailingLink(Guid mailingId, CmsPage page)
        {
            NewsletterMailingStatistics statistics = new NewsletterMailingStatistics();
            NewsletterMailing mailing = BaseObject.GetById<NewsletterMailing>(mailingId);
            if (Request.QueryString.AllKeys.Contains("dataid"))
            {
                Guid dataId;
                Guid.TryParse(Request.QueryString["dataid"], out dataId);
                if (dataId != Guid.Empty)
                {
                    DataGroup dg = BaseObject.GetById<DataGroup>(dataId);
                    if (dg != null)
                    {
                        statistics.FK_Group = dataId;
                    }
                    else
                    {
                        statistics.FK_Item = dataId;
                    }
                }

                statistics.FK_Page = page.ID;
                //statistics.FK_User = (mailing.Subscriber.User != null) ? mailing.Subscriber.User.ID : Guid.Empty;
                statistics.IPAddress = Request.UserHostAddress;
                statistics.Mailing = mailing;
                statistics.Newsletter = mailing.Newsletter;
                statistics.Url = Request.RawUrl.Replace("?mailing=" + mailingId.ToString(), "");
                statistics.UserEmail = mailing.EmailAddress;
                statistics.Save();
            }
        }
    }
}