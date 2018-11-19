using BitPlate.Domain;
using BitPlate.Domain.Utils;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace BitSite._bitPlate.Site
{
    [System.Web.Script.Services.ScriptService]
    public class SiteService : BaseService
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<UnpublishedItem> GetUnpublishedItems(string sort, int page, int pagesize)
        {
            BaseService.CheckLoginAndLicense();
            CmsSite site = SessionObject.CurrentSite;
            string where = String.Format("FK_Site = '{0}'", site.ID);
            BaseCollection<UnpublishedItem> items = BaseCollection<UnpublishedItem>.Get(where, sort, page, pagesize);
            return items;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<UnpublishedItem> GetUnpublishedItemsPerType(string type, string sort, int page, int pagesize)
        {
            BaseService.CheckLoginAndLicense();
            CmsSite site = SessionObject.CurrentSite;
            string where = String.Format("FK_Site = '{0}' AND Type='{1}'", site.ID, type);
            BaseCollection<UnpublishedItem> items = BaseCollection<UnpublishedItem>.Get(where, sort, page, pagesize);
            return items;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public bool PublishSite(string environmentID, bool cleanUp, bool publishPages,
            bool publishFilesAndImages, bool publishData, bool publishBin,
            bool reGenerateSearchIndex, bool reGenerateSitemap)
        {
            BaseService.CheckLoginAndLicense();
            CmsSite site = SessionObject.CurrentSite;
            bool returnValue = site.Publish(new Guid(environmentID), cleanUp, publishPages, publishFilesAndImages, publishData, publishBin, reGenerateSearchIndex, reGenerateSitemap);
            return returnValue;
        }

        //public bool PublishSite_old(string[] selectedItems, bool publishToTestEnvironment, bool publishCompleteSite, bool publishFilesAndImages, bool reGenerateSearchIndex, bool cleanUp)
        //{
        //    BaseService.CheckLoginAndLicense();
        //    CmsSite site = SessionObject.CurrentSite;
        //    bool returnValue = site.Publish(selectedItems, publishToTestEnvironment, publishCompleteSite, publishFilesAndImages, reGenerateSearchIndex, cleanUp);
        //    return returnValue;
        //}

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsSite LoadSite()
        {
            BaseService.CheckSite();
            if (SessionObject.CurrentBitplateUser == null)
            {
                HttpContext.Current.Response.Redirect("~/_bitplate/Login.aspx?SessionExpired=true");
            }
            return SessionObject.CurrentSite;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsSite SaveSite(CmsSite obj)
        {
            BaseService.CheckSite();
            if (SessionObject.CurrentBitplateUser == null)
            {
                HttpContext.Current.Response.Redirect("~/_bitplate/Login.aspx?SessionExpired=true");
            }
            obj.Save();

            //foreach (CmsLanguage lang in SessionObject.CurrentSite.Languages)
            //{
            //    lang.Delete();
            //}
            SessionObject.CurrentSite = obj;
            //bij wijzigingen in veld licentiecode
            if (SessionObject.CurrentLicense != null && SessionObject.CurrentLicense.Code != obj.LicenceCode)
            {
                //herladen van licentie moet worden afgedwongen
                SessionObject.CurrentLicense = SessionObject.LoadLicense();

            }
            SessionObject.CurrentSite.IsValidLicense = (SessionObject.CurrentLicense != null);


            return SessionObject.CurrentSite;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<CmsLanguage> GetAllLanguages()
        {
            BaseService.CheckLoginAndLicense();
            List<CmsLanguage> languages = new List<CmsLanguage>();
            //Voorlopig even alleen bepaalde veelvookomende talen in kunnen voeren.
            //LETOP: In DataLookupValue wordt vertaling nu geregeld doordat deze standaard talen een veldnaam (=TaalCode) hebben
            //Wil je meer talen, dan zul je daar of velden aan moeten toevoegen of een andere (betere!) oplossing verzinnen
            //Talen uit DataLoopupValue worden via sql opgehaald in BaseDataModule

            //foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            //{
            //    if (!SessionObject.CurrentSite.Languages.Any(c => c.LanguageCode == culture.TwoLetterISOLanguageName))
            //    {
            //        CmsLanguage Lang = new CmsLanguage();
            //        Lang.Name = culture.DisplayName;
            //        Lang.LanguageCode = culture.TwoLetterISOLanguageName.ToUpper();
            //        languages.Add(Lang);
            //    }
            //}
            //languages.Sort(delegate(CmsLanguage l1, CmsLanguage l2) { return l1.Name.CompareTo(l2.Name); });
            languages.Add(new CmsLanguage() { Name = "Nederlands", LanguageCode = "NL" });
            languages.Add(new CmsLanguage() { Name = "Engels", LanguageCode = "EN" });
            languages.Add(new CmsLanguage() { Name = "Duits", LanguageCode = "DE" });
            languages.Add(new CmsLanguage() { Name = "Frans", LanguageCode = "FR" });
            languages.Add(new CmsLanguage() { Name = "Spaans", LanguageCode = "SP" });
            languages.Add(new CmsLanguage() { Name = "Pools", LanguageCode = "PL" });
            languages.Add(new CmsLanguage() { Name = "Italiaans", LanguageCode = "IT" });




            return languages;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsLanguage AddLanguage()
        {
            BaseService.CheckLoginAndLicense();
            CmsLanguage Lang = new CmsLanguage();
            Lang.Site = SessionObject.CurrentSite;
            return Lang;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveLanguage(CmsLanguage obj)
        {
            BaseService.CheckLoginAndLicense();
            obj.Save();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<CmsLanguage> DeleteLanguage(string languageId)
        {
            BaseService.CheckLoginAndLicense();
            CmsLanguage Lang = SessionObject.CurrentSite.Languages.Where(c => c.ID == new Guid(languageId)).FirstOrDefault();
            if (Lang != null)
            {
                CmsSite site = SessionObject.CurrentSite;
                if (Lang.Name != site.DefaultLanguage)
                {
                    Lang.Delete();
                    site.Languages.Remove(Lang);
                }
                SessionObject.CurrentSite = site;
            }
            return SessionObject.CurrentSite.Languages;
        }

        [WebMethod(EnableSession = true)]
        public void GetResellerLogo()
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.ContentType = "image/png";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=/_bitplate/_img/bitnetmedia.png");
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
            //HttpContext.Current.Response.BinaryWrite(SessionObject.CurrentBitplateUser.Company.LogoBitmap);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<CmsSiteEnvironment> LoadEnvironments()
        {
            SessionObject.CurrentSite.Environments = BaseCollection<CmsSiteEnvironment>.Get("FK_Site = '" + SessionObject.CurrentSite.ID + "'");
            return SessionObject.CurrentSite.Environments;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult SaveEnvironment(CmsSiteEnvironment obj)
        {
            try
            {
                obj.Save();
            }
            catch (Exception ex)
            {
                return JsonResult.CreateResult(false, ex.Message);
            }
            return JsonResult.CreateResult(true, LoadEnvironments());
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult DeleteEnvironment(CmsSiteEnvironment obj)
        {
            obj.Delete();
            return JsonResult.CreateResult(true, LoadEnvironments());
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SetScriptInWholeSite(string scriptid, bool loadInWholeSite)
        {
            CmsScript script = BaseObject.GetById<CmsScript>(new Guid(scriptid));
            script.LoadInWholeSite = loadInWholeSite;
            script.Save();

        }

        [WebMethod(EnableSession = true)]
        public string PrintSession()
        {
            string str = null;
            /* foreach (string key in HttpContext.Current.Session.Keys)
            {

                str += string.Format("{0}: {1}<br />", key, HttpContext.Current.Session[key].ToString());
            } */
            str = HttpContext.Current.Session.ToJsonString();
            return str;
        }
    }
}
