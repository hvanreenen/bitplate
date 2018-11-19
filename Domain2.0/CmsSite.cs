using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using HJORM;
using HJORM.Attributes;

using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Utils;
using System.IO;
using System.Reflection;

namespace BitPlate.Domain
{
    public enum ModeEnum { View, Edit };
    [Persistent("Site")]
    
    public class CmsSite : BaseDomainObject
    {
        public CmsSite()
            : base()
        {

        }

        private BaseCollection<CmsSiteEnvironment> _environments;
        [NonPersistent()]
        public BaseCollection<CmsSiteEnvironment> Environments
        {
            get
            {
                //if (_pages == null)
                //{
                _environments = BaseCollection<CmsSiteEnvironment>.Get("FK_Site='" + this.ID + "'", "Name");
                //}
                return _environments;
            }
            set
            {
                _environments = value;
            }
        }
        private CmsSiteEnvironment _currentWorkingEnvironment;
        [NonPersistent()]
        [System.Xml.Serialization.XmlIgnore()]
        public CmsSiteEnvironment CurrentWorkingEnvironment
        {
            get
            {
                //if (_currentWorkingEnvironment == null)
                //{
                //   _currentWorkingEnvironment = Utils.WebSessionHelper.CurrentWorkingEnvironment;
                //}
                return _currentWorkingEnvironment;
            }
            set
            {
                _currentWorkingEnvironment = value;
            }
        }
        
        [NonPersistent()]
        public string DomainName
        {
            get
            {
                if (CurrentWorkingEnvironment == null)
                {
                    CurrentWorkingEnvironment = GetWorkingEnvironment();
                }
                return CurrentWorkingEnvironment.DomainName;
            }
        }
        [NonPersistent()]
        public string Path
        {
            get
            {
                if (CurrentWorkingEnvironment == null)
                {
                    CurrentWorkingEnvironment = GetWorkingEnvironment();
                }
                return CurrentWorkingEnvironment.Path;
            }
        }
        //[NonPersistent()]
        //public string GetBitplateVersion()
        //{
        //    string version = "";
        //    string path = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\VersionNumber.txt";
        //    if (File.Exists(path))
        //    {
        //        version = File.ReadAllText(path);
        //    }
        //    if (version == "")
        //    {
        //        version = "Assembly: V" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        //    }
        //    return version;
        //}

        private CmsSiteEnvironment GetWorkingEnvironment()
        {
            CmsSiteEnvironment returnValue = null;
            //eerste editable omgeving wordt standaard
            string where = String.Format("FK_Site='{0}' AND SiteEnvironmentType={1}", this.ID, (int)SiteEnvironmentTypeEnum.Editable);
            returnValue = BaseObject.GetFirst<CmsSiteEnvironment>(where);
            return returnValue;
        }

        
        public string HeadContent { get; set; }
        public bool UseSearchModule { get; set; }

        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public bool UseGoogleAnalystics { get; set; }
        public string GoogleAnalysticsCode { get; set; }
        public string GoogleMapsKey { get; set; }
        public string DefaultLanguage { get; set; }
        
        //public string SiteAdminEmail { get; set; }

        public int MaxWidthImages { get; set; }
        public int MaxWidthThumbnails { get; set; }

        [NonPersistent()]
        public string VirtualPath
        {
            get
            {
                string relativePath = "";
                int dept = System.Web.HttpContext.Current.Request.Path.Split(new char[] { '/' }).Length;
                for (int i = 1; i < dept - 1; i++)
                {
                    relativePath += "../";
                }
                if (System.Web.HttpContext.Current.Request.Path.Contains("bitSites/")
                    || System.Web.HttpContext.Current.Request.Path.Contains("bitAdminAjaxServices/")
                    || System.Web.HttpContext.Current.Request.Path.Contains("bitAdminPages/"))
                {
                    return relativePath + @"../bitSites/" + Name + @"/wwwroot/";
                }
                else
                {
                    return relativePath;
                }
            }
        }

        public string LicenceCode { get; set; }

        [NonPersistent()]
        public bool IsValidLicense { get; set; }
        BaseCollection<CmsPage> _pages;
        [System.Web.Script.Serialization.ScriptIgnore()]
        [NonPersistent()]
        public BaseCollection<CmsPage> Pages
        {
            get
            {
                //if (_pages == null)
                {
                    _pages = BaseCollection<CmsPage>.Get("FK_Site='" + this.ID + "'", "RelativeUrl, Name");
                }
                return _pages;
            }
            set
            {
                _pages = value;
            }
        }

        BaseCollection<CmsTemplate> _templates;
        [System.Web.Script.Serialization.ScriptIgnore()]
        [NonPersistent()]
        public BaseCollection<CmsTemplate> Templates
        {
            get
            {
                //if (_availableLayouts == null)
                {

                    _templates = BaseCollection<CmsTemplate>.Get("FK_Site='" + this.ID + "'", "Name");
                }
                return _templates;
            }
            set
            {
                _templates = value;
            }
        }

        /* BaseCollection<CmsScript> _scripts;
        [System.Web.Script.Serialization.ScriptIgnore()]
        [NonPersistent()]
        public BaseCollection<CmsScript> Scripts
        {
            get
            {
                //if (_availableLayouts == null)
                {
                    _scripts = BaseCollection<CmsScript>.Get("FK_Site='" + this.ID + "'", "ScriptType, Name");
                }
                return _scripts;
            }
            set
            {
                _scripts = value;
            }
        } */

        BaseCollection<BaseModule> _allwaysVisibleModules;
        [NonPersistent()]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public BaseCollection<BaseModule> AllwaysVisibleModules
        {
            get
            {
                BaseCollection<BaseModule> _allwaysVisibleModules = BaseCollection<BaseModule>.Get("FK_Site='" + this.ID + "' AND CrossPagesMode=2");
                //_allwaysVisibleModules = new BaseCollection<Module>();
                //foreach (Module mod in temp)
                //{
                //    _allwaysVisibleModules.Add(mod.ConvertToType());
                //}

                return _allwaysVisibleModules;
            }
            set
            {
                _allwaysVisibleModules = value;
            }
        }

        BaseCollection<DataCollections.DataCollection> _datacollections;
        [System.Web.Script.Serialization.ScriptIgnore()]
        [NonPersistent()]
        public BaseCollection<DataCollections.DataCollection> DataCollections
        {
            get
            {
                //if (_pages == null)
                {
                    _datacollections = BaseCollection<DataCollections.DataCollection>.Get("FK_Site='" + this.ID + "'", "Name");
                }
                return _datacollections;
            }
            set
            {
                _datacollections = value;
            }
        }


        BaseCollection<CmsLanguage> _languages;
        [NonPersistent()]
        public BaseCollection<CmsLanguage> Languages
        {
            get
            {
                if (Utils.WebSessionHelper.CurrentLicense != null && Utils.WebSessionHelper.CurrentLicense.AllowMultipleLanguages)
                {
                    if (_languages == null || _languages.Count == 0)//(_pages == null)
                    {
                        _languages = BaseCollection<CmsLanguage>.Get("FK_Site='" + this.ID + "'", "Name");
                    }
                }
                else
                {
                    //lege collectie
                    _languages = new BaseCollection<CmsLanguage>();
                }
                return _languages;
            }
            set
            {
                _languages = value;
            }
        }
        [NonPersistent()]
        public bool IsMultiLingual
        {
            get
            {
                return (Languages.Count > 1);
            }
        }

        public override void Save()
        {

            foreach (CmsLanguage lang in this.Languages)
            {
                if (lang.IsNew)
                {
                    lang.Site = this;
                    lang.Save();
                }
            }
            base.Save();

        }

        public override void Delete()
        {
            this.DateTill = DateTime.Today.AddDays(-1);
            this.Save();
        }


        private BaseCollection<CmsPageFolder> _folders;
        [NonPersistent()]
        public BaseCollection<CmsPageFolder> PageFolders
        {
            get
            {
                {
                    _folders = BaseCollection<CmsPageFolder>.Get("FK_Site='" + this.ID + "'", "Name");
                }
                return _folders;
            }
            set
            {
                _folders = value;
            }
        }
        public bool Publish(Guid environmentID, bool cleanUp, bool publishPages,
            bool publishFilesAndImages, bool publishData, bool publishBin,
            bool reGenerateSearchIndex, bool reGenerateSitemap)
        {
            CmsSiteEnvironment environment = BaseObject.GetById<CmsSiteEnvironment>(environmentID);
            return Publish(environment, cleanUp, publishPages, publishFilesAndImages, publishData, publishBin, reGenerateSearchIndex, reGenerateSitemap);
        }

        public bool Publish(CmsSiteEnvironment ToEnvironment, bool cleanUp, bool publishPages,
            bool publishFilesAndImages, bool publishData, bool publishBin,
            bool reGenerateSearchIndex, bool reGenerateSitemap)
        {
            CmsSiteEnvironment FromEnvironment = CurrentWorkingEnvironment;

            if (!FromEnvironment.Equals(ToEnvironment))
            {

                if (!System.IO.Directory.Exists(ToEnvironment.Path))
                {
                    System.IO.Directory.CreateDirectory(ToEnvironment.Path);
                }

                if (publishPages)
                {
                    FileHelper.EmptySiteDirectory(ToEnvironment.Path, publishFilesAndImages);
                    PublishDataBase(ToEnvironment);
                }

                if ((!Directory.Exists(ToEnvironment.Path + "\\bin") || publishBin))
                {
                    PublishBinAndBitplate(ToEnvironment);
                }

                if (!File.Exists(ToEnvironment.Path + "\\web.config") || publishBin)
                {
                    CreateWebConfig(ToEnvironment);
                }

                if (publishFilesAndImages)
                {
                    PublishFiles(ToEnvironment, false);
                }

                if (publishData)
                {
                    PublishDataCollection(ToEnvironment);
                }
                

                //if (publishData)
                //{
                //    PublishDataBase(ToEnvironment, cleanUp);
                //}
                //if (publishBin)
                //{
                //    PublishBinAndBitplate(ToEnvironment);
                //}
                //if (publishFilesAndImages)
                //{
                //    PublishFiles(ToEnvironment, cleanUp);
                //}
                //if (publishPages)
                //{
                //    PublishScripts(ToEnvironment, cleanUp);
                //    PublishPages(ToEnvironment, cleanUp);
                //}
            }

            if (reGenerateSearchIndex)
            {
                CreateSearchIndex(ToEnvironment);
            }
            if (reGenerateSitemap)
            {
                CreateSiteMap(ToEnvironment);
                CreateRobotsTxt(ToEnvironment);
            }


            return true;
        }

        private List<string> Tables;

        private void PublishDataBase(CmsSiteEnvironment environment)
        {
            DatabaseHelper databaseHelper = new DatabaseHelper();
            databaseHelper.DropSiteEnvironmentTables(this, environment);
            databaseHelper.CloneEnvironmentTables(this, CurrentWorkingEnvironment, environment);
            databaseHelper.InsertEnvironmentDataInTables(this, CurrentWorkingEnvironment, environment);
        }

        public void PublishDataCollection(CmsSiteEnvironment environment)
        {
            DatabaseHelper databaseHelper = new DatabaseHelper();
            databaseHelper.DropDataCollectionEnvironmentTables(this, environment);
            databaseHelper.CloneDataCollectionEnvironmentTables(this, CurrentWorkingEnvironment, environment);
            databaseHelper.InsertEnvironmentDataCollectionInTables(this, CurrentWorkingEnvironment, environment);
        }

        private void PublishFiles(CmsSiteEnvironment Environment, bool cleanup)
        {
            if (cleanup)
            {
                //eerst alles weggooien
                Utils.FileHelper.DeleteFilesFromDirectory(Environment.Path + "\\_files", "", true);
                Utils.FileHelper.DeleteDir(Environment.Path + "\\_files");
            }
            Utils.FileHelper.CopyDir(this.CurrentWorkingEnvironment.Path + "\\_files", Environment.Path + "\\_files");
        }

        private void PublishScripts(CmsSiteEnvironment Environment, bool cleanup)
        {
            if (cleanup)
            {
                //eerst alles weggooien
                Utils.FileHelper.DeleteFilesFromDirectory(Environment.Path + "\\_css", "", true);
                Utils.FileHelper.DeleteFilesFromDirectory(Environment.Path + "\\_js", "", true);

                Utils.FileHelper.DeleteDir(Environment.Path + "\\_css");
                Utils.FileHelper.DeleteDir(Environment.Path + "\\_js");
            }

            Utils.FileHelper.CopyDir(this.CurrentWorkingEnvironment.Path + "\\_css", Environment.Path + "\\_css");
            Utils.FileHelper.CopyDir(this.CurrentWorkingEnvironment.Path + "\\_js", Environment.Path + "\\_js");
        }

        private void PublishPages(CmsSiteEnvironment Environment, bool cleanup)
        {
            string[] directories;
            if (cleanup)
            {
                //eerst weggooien
                Utils.FileHelper.DeleteFilesFromDirectory(Environment.Path, "*.aspx", false);
                Utils.FileHelper.DeleteFilesFromDirectory(Environment.Path, "*.master", false);
                Utils.FileHelper.DeleteFilesFromDirectory(Environment.Path + "\\_moduleUserControls", "*.ascx", false);
                Utils.FileHelper.DeleteFile(Environment.Path + "\\global.asax");


                directories = System.IO.Directory.GetDirectories(Environment.Path);
                foreach (string path in directories)
                {
                    string dir = System.IO.Path.GetFileName(path); //bij een dir geeft GetFileName de naam van de dir terug
                    if (!(dir.StartsWith("_") || dir.StartsWith("!") || dir == "bin" || dir == "obj" || dir == "Properties" || dir == "WebUserControls" || dir == "aspnet_client"))
                    {
                        Utils.FileHelper.DeleteFilesFromDirectory(path, "", true);
                        Utils.FileHelper.DeleteDir(path);

                    }
                }
            }
            //daarna kopieren
            Utils.FileHelper.CopyDir(CurrentWorkingEnvironment.Path, Environment.Path, "*.aspx", false, false);
            Utils.FileHelper.CopyDir(CurrentWorkingEnvironment.Path, Environment.Path, "*.master", false, false);
            Utils.FileHelper.CopyDir(CurrentWorkingEnvironment.Path + "\\_moduleUserControls", Environment.Path + "\\_moduleUserControls", "*.ascx", false, false);
            if (Environment.SiteEnvironmentType == SiteEnvironmentTypeEnum.Editable)
            {
                Utils.FileHelper.CopyFile(CurrentWorkingEnvironment.Path + "\\global.asax", Environment.Path + "\\global.asax");
            }

            directories = System.IO.Directory.GetDirectories(CurrentWorkingEnvironment.Path);
            foreach (string path in directories)
            {
                if (path == Environment.Path) continue;
                string dir = System.IO.Path.GetFileName(path); //bij een dir geeft GetFileName de naam van de dir terug
                if (!(dir.StartsWith("_") || dir.StartsWith("!") || dir == "bin" || dir == "obj" || dir == "Properties" || dir == "WebUserControls" || dir == "aspnet_client"))
                {
                    Utils.FileHelper.CopyDir(path, Environment.Path + "\\" + dir, "*.aspx", false, true);
                }
            }
        }

        private void PublishBinAndBitplate(CmsSiteEnvironment Environment)
        {
            Utils.FileHelper.CopyDir(CurrentWorkingEnvironment.Path + "\\bin", Environment.Path + "\\bin", "*.xml|*.dll", false, false);
            Utils.FileHelper.CopyDir(CurrentWorkingEnvironment.Path + "\\_js", Environment.Path + "\\_js", "*.xml|*.js", false, false);
            if (Environment.SiteEnvironmentType == SiteEnvironmentTypeEnum.Editable)
            {
                //heel bitplate copieren
                Utils.FileHelper.CopyDir(CurrentWorkingEnvironment.Path + "\\_bitplate", Environment.Path + "\\_bitplate", "*.*", false, true);
            }
            FileHelper.CopyFile(CurrentWorkingEnvironment.Path + "\\Page.aspx", Environment.Path + "\\Page.aspx");
            FileHelper.CopyFile(CurrentWorkingEnvironment.Path + "\\Newsletter.aspx", Environment.Path + "\\Newsletter.aspx");
            //FileHelper.CopyFile(CurrentWorkingEnvironment.Path + "\\Captcha.aspx", Environment.Path + "\\Captcha.aspx");
            FileHelper.CopyFile(CurrentWorkingEnvironment.Path + "\\script.handler", Environment.Path + "\\script.handler");
            FileHelper.CopyFile(CurrentWorkingEnvironment.Path + "\\Global.asax", Environment.Path + "\\Global.asax");
        }

        private void CreateSearchIndex(CmsSiteEnvironment ToEnvironment)
        {

            Search.SearchHelper.FillSearchIndex(this);

            CmsSiteEnvironment FromEnvironment = CurrentWorkingEnvironment;
            if (!FromEnvironment.Equals(ToEnvironment))
            {
                //zet om naar live database
                //create
                string sql = String.Format(@"CREATE TABLE IF NOT EXISTS {0}.searchindex LIKE {1}.searchindex;", ToEnvironment.DatabaseName, FromEnvironment.DatabaseName);
                DataBase.Get().Execute(sql);

                //delete
                sql = String.Format(@"DELETE FROM {0}.searchindex WHERE FK_Site='{1}';", ToEnvironment.DatabaseName, this.ID);
                DataBase.Get().Execute(sql);
                //insert
                sql = String.Format(@"INSERT INTO {0}.searchindex SELECT * FROM {1}.searchindex WHERE FK_Site='{2}';", ToEnvironment.DatabaseName, FromEnvironment.DatabaseName, this.ID);
                DataBase.Get().Execute(sql);
            }


        }

        private void CreateWebConfig(CmsSiteEnvironment environment)
        {
            string webConfigContent = environment.CreateWebConfig(this.DomainName);
            Utils.FileHelper.WriteFile(environment.Path + "\\Web.config", webConfigContent);
        }

        public static Guid GetHomePageIDBySiteID(string siteid)
        {
            Guid returnValue;
            CmsPage homePage;
            BaseCollection<CmsPage> pagesInFolder = BaseCollection<CmsPage>.Get("FK_Folder IS NULL AND FK_Site = '" + siteid + "'", "Name");
            homePage = pagesInFolder.Where(c => c.IsHomePage).FirstOrDefault();

            if (homePage == null)
            {
                homePage = pagesInFolder.Where(c => c.Name.ToLower() == "default").FirstOrDefault();
            }

            if (homePage == null)
            {
                homePage = pagesInFolder.Where(c => c.Name.ToLower() == "index").FirstOrDefault();
            }

            if (homePage == null)
            {
                homePage = pagesInFolder.FirstOrDefault();
            }

            if (homePage != null)
            {
                returnValue = homePage.ID;
            }
            else
            {
                returnValue = Guid.Empty;
            }
            return returnValue;
        }



        private void CreateSiteMap(CmsSiteEnvironment Environment)
        {

            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n";
            xml += "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"\r\n";
            xml += "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n";
            xml += "xsi:schemaLocation=\"http://www.sitemaps.org/schemas/sitemap/0.9\r\n";
            xml += "http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd\">\r\n";
            xml += "\r\n";

            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            foreach (CmsPage page in Pages)
            {
                if (page.InSiteMap && page.IsActive)
                {
                    xml += String.Format(culture, @"   <url>
        <loc>{0}</loc>
        <lastmod>{1:yyyy-MM-dd}</lastmod>
        <changefreq>{2}</changefreq>
        <priority>{3:0.0}</priority>
    </url>
", Environment.DomainName + page.RelativeUrl, page.ModifiedDate, page.SiteMapChangeFreq, page.SiteMapPriority);

                }
            }
            xml += "</urlset>";
            //opslaan

            Utils.FileHelper.WriteFile(Environment.Path + "\\Sitemap.xml", xml);
        }

        public void CreateRobotsTxt(CmsSiteEnvironment Environment)
        {
            string txt = "User-agent: *\r\n";

            foreach (CmsPage page in Pages)
            {
                if (page.DisallowRobots && page.IsActive && page.RelativeUrl != null)
                {
                    txt += "Disallow: " + page.RelativeUrl + "\r\n";
                }
            }

            Utils.FileHelper.WriteFile(Environment.Path + "\\Robots.txt", txt);

        }

        private static bool ArrayContainsValue(string[] array, string value)
        {
            bool returnValue = false;
            foreach (string item in array)
            {
                if (item == value)
                {
                    returnValue = true;
                    break;
                }
            }
            return returnValue;
        }

        private CmsPage _OptInPage;
        [System.Xml.Serialization.XmlIgnore()]
        //[System.Web.Script.Serialization.ScriptIgnore()]
        [Association("FK_NewsletterOptInEmailPage")]
        public CmsPage NewsletterOptInEmailPage
        {
            get
            {
                if (_OptInPage != null && !_OptInPage.IsLoaded)
                {
                    _OptInPage.Load();
                }
                return _OptInPage;
            }
            set
            {
                _OptInPage = value;
            }
        }

        private CmsPage _OptOutPage;
        [System.Xml.Serialization.XmlIgnore()]
        [Association("FK_NewsletterOptOutEmailPage")]
        public CmsPage NewsletterOptOutEmailPage
        {
            get
            {
                if (_OptOutPage != null && !_OptOutPage.IsLoaded)
                {
                    _OptOutPage.Load();
                }
                return _OptOutPage;
            }
            set
            {
                _OptOutPage = value;
            }
        }

        public string NewsletterSender { get; set; }
        public string NewsletterOptInEmailSubject { get; set; }
        public string NewsletterOptInEmailContent { get; set; }
        public string NewsletterOptOutEmailSubject { get; set; }
        public string NewsletterOptOutEmailContent { get; set; }

        BaseCollection<CmsScript> _scripts;
        [NonPersistent()]
        public BaseCollection<CmsScript> Scripts
        {
            get
            {
                //if (_scripts == null || (_scripts != null && !_scripts.IsLoaded))
                {
                    string where = String.Format(" FK_Site = '{0}' AND LoadInWholeSite = 1", this.ID);
                    _scripts = BaseCollection<CmsScript>.Get(where, "ScriptType, Name");
                    _scripts.IsLoaded = true;
                }
                return _scripts;
            }
            set
            {
                _scripts = value;
                _scripts.IsLoaded = true;
            }
        }

        public string GetAllSiteScriptsInHtml(bool editmode)
        {
            string html = "";
            foreach (CmsScript script in this.Scripts)
            {
                if (editmode && script.ActiveInEditor)
                {
                    html += script.GetTag();
                }
                else
                {
                    html += script.GetTag();
                }
            }
            return html;
        }
    }
}
