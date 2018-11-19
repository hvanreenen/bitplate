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

namespace BitSite
{
    public partial class Page : System.Web.UI.Page
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

            if (Request.Form["hiddenModuleType"] != null)
            {
                string moduleType = Request.Form["hiddenModuleType"].ToString();
                string id = Request.Form["hiddenModuleID"].ToString();
                BaseModule module = BaseObject.GetById<BaseModule>(new Guid(id));
                IPostableModule postableModule = module.ConvertToType() as IPostableModule;
                postableModule.HandlePost(Request.Form);
            }

            string pageHtml = page.Publish2();

            Response.Write(pageHtml);

            
        }


        private bool CheckIfActive(CmsPage page)
        {
            return page.IsActive;
        }

        private bool CheckSiteAutorisation(CmsPage page)
        {
            bool isAutorized = true;
            if (page.HasAutorisation)
            {
                isAutorized = false;
                string autorizedSiteUserGroupIDs = "", autorizedSiteUserIDs = "";
                foreach(SiteUserGroup userGroup in page.AutorizedSiteUserGroups){
                    autorizedSiteUserGroupIDs += userGroup.ID + ",";
                }
                foreach (SiteUser user in page.AutorizedSiteUsers)
                {
                    autorizedSiteUserIDs += user.ID + ",";
                }

                if (SessionObject.CurrentBitSiteUser != null)
                {
                    isAutorized = SessionObject.CurrentBitSiteUser.IsAutorized(autorizedSiteUserGroupIDs, autorizedSiteUserIDs);
                }
            }
            return isAutorized;
        }

        private bool CheckBitplateAutorisation(CmsPage page)
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

                if (SessionObject.CurrentBitSiteUser != null)
                {
                    isAutorized = SessionObject.CurrentBitplateUser.IsAutorized(autorizedBitplateUserGroupIDs, autorizedBitplateUserIDs);
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

    }
}