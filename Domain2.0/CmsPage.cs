using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Utils;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Modules.Data;
using System.Text.RegularExpressions;
using System.Web.Optimization;
using System.Web;
using System.Reflection;

namespace BitPlate.Domain
{
    [Persistent("Page")]
    public partial class CmsPage : BaseDomainPublishableObject
    {

        private string _title;
        /// <summary>
        /// Titel van pagina. Is Titel leeg dan wordt Name overgenomen
        /// </summary>
        public string Title
        {
            get
            {
                if (_title == null || _title == "")
                {
                    _title = Name;
                }
                return _title;
            }
            set { _title = value; }
        }
        /// <summary>
        /// IsHomepage per folder
        /// </summary>
        public bool IsHomePage { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaRobots { get; set; }
        public string BodyTagContent { get; set; }

        [NonPersistent()]
        public override bool IsActive
        {
            get
            {
                bool returnValue = base.IsActive;
                string isActiveStringInherited = "";
                if (Folder != null && !Folder.IsActive)
                {
                    returnValue = false;
                    isActiveStringInherited = "folder";
                }
                if (Template != null && !Template.IsActive)
                {
                    returnValue = false;
                    if (isActiveStringInherited != "")
                    {
                        isActiveStringInherited += " & ";
                    }
                    isActiveStringInherited += "template";
                }
                if (!returnValue && isActiveStringInherited != "")
                {
                    _isActiveString = "Niet actief (overgenomen van " + isActiveStringInherited + ")";
                }
                return returnValue;
            }
        }


        [NonPersistent()]
        public string LanguageCode
        {
            get
            {
                if (Template != null)
                {
                    return Template.LanguageCode;
                }
                else
                {
                    return "";
                }
            }
        }
        private string _headContent = "";
        public string HeadContent { get { return _headContent; } set { _headContent = value; } }
        /// <summary>
        /// pagina komt in robots.txt (voor zoekmachines: dat deze pagina niet wordt geindexeerd)
        /// </summary>
        public bool DisallowRobots { get; set; }
        /// <summary>
        /// Pagina kom in sitemap.xml (voor zoekmachines)
        /// </summary>
        public bool InSiteMap { get; set; }
        /// <summary>
        /// Sitemap info: wijzig frequentie van pagina
        /// </summary>

        public string SiteMapChangeFreq { get; set; }

        private double _siteMapPriority;
        public double SiteMapPriority
        {
            get
            {
                //FIX voor rare comma getallen. Getal verandert in de setValue functie
                return Math.Round(_siteMapPriority, 1);
            }
            set
            {
                _siteMapPriority = value;
            }
        }

        private BaseCollection<BaseModule> _pageModules;
        [System.Web.Script.Serialization.ScriptIgnore()]
        [NonPersistent()]
        //cascade delete in db
        public BaseCollection<BaseModule> ModulesDeprecated
        {
            get
            {
                if (_pageModules == null)
                {
                    _pageModules = BaseCollection<BaseModule>.Get("FK_Page='" + this.ID + "'", "OrderingNumber");
                    //foreach (Module mod in pageModules)
                    //{
                    //    _pageModules.Add(mod.ConvertToType());
                    //}
                }
                return _pageModules;
            }
            //set
            //{
            //    _pageModules = value;
            //}
        }

        private BaseCollection<BaseModule> _modules = null;
        [System.Web.Script.Serialization.ScriptIgnore()]
        [Persistent("moduleperpage")]
        [Association("FK_Page", "FK_Module")]
        public BaseCollection<BaseModule> Modules
        {
            get
            {
                if (this._modules == null)
                {
                    string where = "EXISTS (SELECT * FROM moduleperpage WHERE FK_Page = '" + this.ID + "' AND module.ID = moduleperpage.FK_Module)";
                    this._modules = BaseCollection<BaseModule>.Get(where, "OrderingNumber");
                    this._modules.IsLoaded = true;
                }

                //Convert module references;
                if (this.ModulesDeprecated.Count > 0)
                {
                    foreach (BaseModule module in this.ModulesDeprecated)
                    {
                        //module.PageDeprecated = null;
                        if (!this._modules.Any(c => c.ID == module.ID))
                        {
                            this._modules.Add(module);
                        }
                        //module.Save();
                    }
                    this._pageModules.Clear();
                    //this.Save();
                }
                return this._modules;
            }
            set
            {
                this._modules = value;
                this._modules.IsLoaded = true;
            }
        }

        private BaseCollection<BaseModule> _allModules;
        /// <summary>
        /// alle modules ophalen dus ook de zichtbaar op alle pagina's in site of template
        /// </summary>
        /// <returns></returns>
        public BaseCollection<BaseModule> GetAllModules()
        {
            if (_allModules == null || _allModules.Count == 0)
            {
            List<BaseModule> modules = new List<BaseModule>();

            string where = String.Format("FK_Site='{0}' AND (CrossPagesMode=2)", this.Site.ID);
            BaseCollection<BaseModule> siteModules = BaseCollection<BaseModule>.Get(where, "OrderingNumber");
            modules.AddRange(siteModules);
            //foreach (BaseModule module in siteModules)
            //{
            //    if (!modulesHtmlByContainerName.ContainsKey(module.ContainerName))
            //    {
            //        modulesHtmlByContainerName.Add(module.ContainerName, "");
            //    }
            //    module.LanguageCode = languageCode;
            //    modulesHtmlByContainerName[module.ContainerName] += module.ConvertToType().Publish2(this);
            //}
            //dan modules zichtbaar in de template
            if (this.Template != null)
            {
                where = String.Format("FK_Site='{0}' AND (CrossPagesMode=1 AND FK_Template = '{1}')", this.Site.ID, this.Template.ID);
                BaseCollection<BaseModule> templateModules = BaseCollection<BaseModule>.Get(where, "OrderingNumber");
                modules.AddRange(templateModules);
                //foreach (BaseModule module in templateModules)
                //{
                //    if (!modulesHtmlByContainerName.ContainsKey(module.ContainerName))
                //    {
                //        modulesHtmlByContainerName.Add(module.ContainerName, "");
                //    }
                //    module.LanguageCode = languageCode;
                //    modulesHtmlByContainerName[module.ContainerName] += module.ConvertToType().Publish2(this);
                //}
            }
            //en alle page modules
            //BaseCollection<BaseModule> pageModules = BaseCollection<BaseModule>.Get("FK_Page='" + this.ID + "'", "OrderingNumber");
            modules.AddRange(this.Modules);
            modules = modules.GroupBy(c => c.ID).Select(c => c.FirstOrDefault()).OrderBy(c => c.OrderingNumber).ToList();
            _allModules = new BaseCollection<BaseModule>();
            //DIT IS NODIG OM DE INCLUDESCRIPTS TE LADEN. DEZE STAP WORDT OVERGESLAGEN IN PUBLISH()
            foreach (BaseModule module in modules)
            {
                _allModules.Add(module.ConvertToType());
            }

            //if (_allModules == null ||_allModules.Count == 0)
            //{
            //    _allModules = new BaseCollection<BaseModule>();
            //    //eerst site modules
            //    string where = String.Format("FK_Site='{0}' AND IsVisibleOnAllPages=true", this.Site.ID);
            //    BaseCollection<BaseModule> siteModules = BaseCollection<BaseModule>.Get(where, "OrderingNumber");
            //    foreach (BaseModule mod in siteModules)
            //    {
            //        BaseModule convertedModule = mod.ConvertToType();
            //        _allModules.Add(convertedModule);
            //    }
            //    //dan modules zichtbaar in de template
            //    if (this.Template != null)
            //    {
            //        where = String.Format("FK_Site='{0}' AND IsVisibleOnAllPagesInLayout=true AND FK_Template = '{1}'", this.Site.ID, this.Template.ID);
            //        BaseCollection<BaseModule> templateModules = BaseCollection<BaseModule>.Get(where, "OrderingNumber");
            //        foreach (BaseModule mod in templateModules)
            //        {
            //            BaseModule convertedModule = mod.ConvertToType();
            //            _allModules.Add(convertedModule);
            //        }
            //    }
            //    //en alle page modules
            //    BaseCollection<BaseModule> pageModules = BaseCollection<BaseModule>.Get("FK_Page='" + this.ID + "'", "OrderingNumber");
            //    foreach (BaseModule mod in pageModules)
            //    {
            //        BaseModule convertedModule = mod.ConvertToType();
            //        _allModules.Add(convertedModule);
            //    }

            //    //en alle page modules
            //    BaseCollection<BaseModule> newsletterModules = BaseCollection<BaseModule>.Get("FK_Newsletter='" + this.ID + "'", "OrderingNumber");
            //    foreach (BaseModule mod in newsletterModules)
            //    {
            //        BaseModule convertedModule = mod.ConvertToType();
            //        _allModules.Add(convertedModule);
            //    }
            }
            return _allModules;
        }

        private CmsPageFolder _folder;
        /// <summary>
        /// Folder waarin page valt als die er is. Bij pagina's in root is Folder null 
        /// </summary>
        [Association("FK_Folder")]
        public CmsPageFolder Folder
        {
            get
            {
                if (_folder != null && !_folder.IsLoaded)
                {
                    _folder.Load();
                }
                return _folder;
            }
            set { _folder = value; }
        }

        /// <summary>
        /// Template van pagina.
        /// Template wordt MasterPage tijdens opslaan (publiceren)
        /// </summary>
        private CmsTemplate _template;
        [Association("FK_Layout")]
        public CmsTemplate Template
        {
            get
            {
                if (_template != null && !_template.IsLoaded)
                {
                    _template.Load();
                }
                return _template;
            }
            set { _template = value; }
        }

        BaseCollection<CmsScript> _scripts;
        [Persistent("ScriptPerPage")]
        [Association("FK_Page", "FK_Script")]
        public BaseCollection<CmsScript> Scripts
        {
            get
            {
                if (_scripts == null || (_scripts != null && !_scripts.IsLoaded))
                {
                    string where = "EXISTS (SELECT * FROM ScriptPerPage WHERE FK_Page = '" + this.ID + "' AND Script.ID = ScriptPerPage.FK_Script)";
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
        /// <summary>
        /// SiteScripts zijn alle scripts van de site
        /// Dit is een aparte property want Site is ScriptIgnore en wordt niet over het lijntje gegooid.
        /// Om in javascript wel toegang te hebben tot de sitescript: hier een aparte prop
        /// </summary>
        public BaseCollection<CmsScript> SiteScripts
        {
            get
            {
                return Site.Scripts;
            }
        }

        BaseCollection<CmsScript> _allScripts;
        [NonPersistent()]
        public BaseCollection<CmsScript> GetAllScripts()
        {
            if (_allScripts == null)
            {

                //eerst alle scripts van site
                _allScripts = Site.Scripts;

                //mocht die dannog leeg zijn, dan nieuwe lege aanmaken:
                if (_allScripts == null)
                {
                    _allScripts = new BaseCollection<CmsScript>();
                }

                //dan alle van template eraan toevoegen
                BaseCollection<CmsScript> templateScripts = this.Template.Scripts;
                foreach (CmsScript script in templateScripts)
                {
                    if (!_allScripts.Contains(script))
                    {
                        _allScripts.Add(script);
                    }
                }
                //dan alle van pagina eraan toevoegen
                BaseCollection<CmsScript> pageScripts = this.Scripts;
                foreach (CmsScript script in pageScripts)
                {
                    if (!_allScripts.Contains(script))
                    {
                        _allScripts.Add(script);
                    }
                }
                //dan nog alle module specifieke scripts toevoegen
                foreach (string scriptUrl in GetModuleDependentScripts())
                {
                    CmsScript dummy = new CmsScript();
                    if (scriptUrl.EndsWith("css"))
                    {
                        dummy.ScriptType = ScriptTypeEnum.Css;
                    }
                    else
                    {
                        dummy.ScriptType = ScriptTypeEnum.Js;
                    }
                    dummy.Url = scriptUrl;
                    //dummy.UrlLive = scriptUrl;
                    dummy.ID = Guid.NewGuid();
                    _allScripts.Add(dummy);
                }
            }
            return _allScripts;
        }

        #region URLS, BESTANDSNAMEN EN PADEN
        /// <summary>
        /// Relatieve pad, wordt aangemaakt tijdens opslaan van page
        /// </summary>
        [NonPersistent()]
        public string RelativePath
        {
            get
            {
                return (RelativeUrl != null) ? RelativeUrl.Replace("/" + Name, "") : "";
            }
        }

        public string RelativeUrl { get; set; }
        /// <summary>
        /// Relative url
        /// </summary>
        //[NonPersistent()]
        //public string LastPublishedUrl
        //{
        //    get
        //    {
        //        if (LastPublishedFileName == null) return null;
        //        string lastPublishedUrl = LastPublishedFileName.Replace(Site.CurrentWorkingEnvironment.Path + "\\", "");
        //        lastPublishedUrl = lastPublishedUrl.Replace("\\", "/");
        //        return lastPublishedUrl;
        //    }
        //}


        /// <summary>
        /// Diepte van het pad ten opzicht van de root
        /// Contact.aspx = 0
        /// Folder/Contact.aspx = 1
        /// Folder/SubFolder/Contact.aspx = 2
        /// </summary>
        [NonPersistent()]
        public int RelativePathDept
        {
            get
            {
                int returnValue = (RelativeUrl != null) ? RelativeUrl.Split(new char[] { '/' }).Length - 1 : 0;
                return returnValue;
            }
        }

        //[NonPersistent()]
        //public string AbsoluteFileLocation
        //{
        //    get
        //    {
        //        //Gaat soms hier nog de fout in.
        //        if (RelativePath != null)
        //        {
        //            return Site.Path + RelativePath + Name + ".aspx";
        //        }
        //        else
        //        {
        //            return Site.Path + Name + ".aspx";
        //        }
        //    }
        //}

        //[NonPersistent()]
        //public string AbsoluteFileName
        //{
        //    get
        //    {
        //        string path = "";
        //        if (this.RelativePath != null)
        //        {
        //            path = AppDomain.CurrentDomain.BaseDirectory + this.RelativePath.Replace("/", "\\");
        //        }
        //        else
        //        {
        //            path = AppDomain.CurrentDomain.BaseDirectory;
        //        }

        //        string fileName = path + RelativeUrl.Replace("/", "\\");
        //        return fileName;
        //    }
        //}
        #endregion


        #region SAVE, DELETE EN COPY
        public override void Save()
        {
            if (IsNew && WebSessionHelper.CurrentLicense != null && WebSessionHelper.CurrentLicense.HasMaxNumberExceeded("Page"))
            {
                throw new Exception("Kan niet opslaan: licentie heeft maximale aantal pagina's (" + WebSessionHelper.CurrentLicense.MaxNumberOfPages + ") overschreden.");
            }
            this.Name = FileHelper.CleanFileName(this.Name);    //Haal vreemde tekens uit de paginanaam.
            //Er mag maar 1 pagina per forlder de hiome page zijn
            //als deze pagina de homepage is, dan andere uit die folder geen homepage maken
            if (this.IsHomePage)
            {
                string where = "";
                if (this.Folder == null)
                {
                    where = "FK_Folder is null AND IsHomePage='1'";
                }
                else
                {
                    where = "FK_Folder = '" + this.Folder.ID.ToString() + "' AND IsHomePage='1'";
                }

                BaseCollection<CmsPage> Pages = BaseCollection<CmsPage>.Get(where);

                foreach (CmsPage page in Pages)
                {
                    if (page.ID != this.ID)
                    {

                        page.IsHomePage = false;
                        page.Save();
                    }
                }
            }

            //zet pad (wordt opgeslagen in db voor performance)
            if (this.Folder == null && this.GetType() != typeof(Newsletters.Newsletter))
            {
                RelativeUrl = "/" + Name;
            }
            
            if (this.Folder != null && this.GetType() != typeof(Newsletters.Newsletter))
            {
                RelativeUrl = "/" + Folder.GetCompletePath() + "/" + Name;
            }
            //Relative URL van nieuwsbrief is verhuist naar newsletter.save();
            //unique check
            //workaraound voor mysql: uniek is combi van naam, site en folder
            //als folder leeg is (NULL), gaat uniqueconstraint niet af in mySql
            //hierom in c# checken
            if (this.Folder == null)
            {
                string pageName = this.Name;
                //escape char voor mysql
                pageName = pageName.Replace("'", "''");
                string sql = String.Format("FK_Site='{0}' AND FK_Folder IS NULL AND name = '{1}' AND ID != '{2}'", Site.ID, pageName, this.ID);
                CmsPage checkPage = BaseObject.GetFirst<CmsPage>(sql);
                if (checkPage != null)
                {
                    throw new Exception("Er bestaat al een pagina met deze naam.");
                }
            }

            base.Save();

            //this.Publish();
        }

        public override void Delete()
        {
            foreach (BaseModule module in this.Modules)
            {
                //Modules zichtbaar op alle pagina's
                if (module.CrossPagesMode == CrossPagesModeEnum.VisibleOnOnePage ||
                    module.Pages.Count == 1 && module.CrossPagesMode == CrossPagesModeEnum.VisibleOnAllSelectedPages)
                {
                    module.Delete();
                }

                //if (module.CrossPagesMode == CrossPagesModeEnum.VisibleOnAllPages && (module.CrossPagesMode != CrossPagesModeEnum.VisibleOnAllPagesInTemplate || module.Template == null) && module.Pages.Count == 1)
                //{
                //    module.Delete();
                //}
                //else if (module.CrossPagesMode != CrossPagesModeEnum.VisibleOnAllPages && module.CrossPagesMode != CrossPagesModeEnum.VisibleOnAllPagesInTemplate)
                //{
                //    module.Delete();
                //}
            }
            base.Delete();
            UnpublishedItem.Set(this, "Page", ChangeStatusEnum.Deleted);
        }

        public CmsPage Copy(string newName, Guid? folderID)
        {
            CmsPage newPage = base.CreateCopy<CmsPage>(false);
            newPage.Name = newName;
            if (folderID.HasValue)
            {
                newPage.Folder = new CmsPageFolder();
                newPage.Folder.ID = folderID.Value;
            }
            newPage.IsHomePage = false; //ishome nooit meekopieren
            newPage.LastPublishedDate = null;
            
            //bij scripts, usergroups & users is het niet het script zelf dat onder een template hangt, maar alleen de koppeling (veel op veel).
            foreach (CmsScript script in this.Scripts)
            {
                newPage.Scripts.Add(script);
            }
            if (this.HasAutorisation)
            {
                if (this.HasBitplateAutorisation())
                {
                    foreach (Autorisation.BitplateUserGroup userGroup in this.AutorizedBitplateUserGroups)
                    {
                        newPage.AutorizedBitplateUserGroups.Add(userGroup);
                    }
                    foreach (Autorisation.BitplateUser user in this.AutorizedBitplateUsers)
                    {
                        newPage.AutorizedBitplateUsers.Add(user);
                    }
                }
                if (this.HasSiteAutorisation())
                {
                    foreach (Autorisation.SiteUserGroup userGroup in this.AutorizedSiteUserGroups)
                    {
                        newPage.AutorizedSiteUserGroups.Add(userGroup);
                    }
                    foreach (Autorisation.SiteUser user in this.AutorizedSiteUsers)
                    {
                        newPage.AutorizedSiteUsers.Add(user);
                    }
                }
            }

            newPage.Save();
            //1 op veel relatie: pas na save doen
            foreach (BaseModule module in this.Modules)
            {
                BaseModule newModule = module.Copy(module.Name, newPage.ID);
            }

           

            return newPage;
        }

        #endregion

        #region PUBLISH (OPSLAAN VAN BESTAND)



        //public string GetContainerContent(CmsTemplateContainer container, bool publishAllModules)
        //{
        //    string containerContent = "";

        //    foreach (BaseModule mod in this.GetAllModules())
        //    {
        //        if (mod.ContainerName == container.Name)
        //        {

        //            if (publishAllModules)
        //            {
        //                mod.Publish();
        //            }
        //            if (mod.Type == "HtmlModule" && !mod.HasAutorisation)
        //            {
        //                containerContent += mod.ToString();
        //            }
        //            else
        //            {
        //                containerContent += mod.GetUserControlTag();
        //            }
        //        }
        //    }
        //    return containerContent;
        //}

        protected virtual string getHeader()
        {
            string head = "\r\n<meta name=\"generator\" content=\"Bitplate.CMS " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\" />\r\n";
            //head += GetHeaderMetaTags();
            head += this.Site.HeadContent;
            List<CmsScript> allScripts = GetAllScripts(); //new List<CmsScript>();
            //allScripts.AddRange(this.Site.Scripts);
            //allScripts.AddRange(this.Template.Scripts);
            //allScripts.AddRange(this.Scripts);

            //foreach (CmsScript script in this.Site.Scripts)
            //{
            //    head += script.GetTag();
            //}
            //head += Site.HeadContent;
            //foreach (CmsScript script in this.Template.Scripts)
            //{
            //    head += script.GetTag();
            //}
            //foreach (CmsScript script in this.Scripts)
            //{
            //    head += script.GetTag();
            //}
            allScripts = allScripts.Distinct().ToList();

            //Attach css files to head;
            foreach (CmsScript script in allScripts.Where(c => c.ScriptType == ScriptTypeEnum.Css))
            {
                head += script.GetTag();
            }

            //Attach defaultscripts first
            string jQueryVersion = ConfigurationManager.AppSettings["jQueryVersion"];
            if (jQueryVersion == null || jQueryVersion == "")
            {
                jQueryVersion = "jquery-1.8.2.js";
            }
            CmsScript jquery = new CmsScript() { ID = Guid.NewGuid(), ScriptType = ScriptTypeEnum.Js, Url = "/_js/" + jQueryVersion };
            head += jquery.GetTag();
            CmsScript jqueryUI = new CmsScript() { ID = Guid.NewGuid(), ScriptType = ScriptTypeEnum.Js, Url = "/_js/jquery-ui-1.9.2.custom.min.js" };
            head += jqueryUI.GetTag();
            CmsScript bitAjax = new CmsScript() { ID = Guid.NewGuid(), ScriptType = ScriptTypeEnum.Js, Url = "/_js/bitAjax.js" };
            head += bitAjax.GetTag();
            CmsScript bitDragDrop = new CmsScript() { ID = Guid.NewGuid(), ScriptType = ScriptTypeEnum.Js, Url = "/_js/bitDragDropModules.js" };
            head += bitDragDrop.GetTag();
            
            //Attach scripts
            foreach (CmsScript script in allScripts.Where(c => c.ScriptType == ScriptTypeEnum.Js))
            {
                head += script.GetTag();
            }
            head += HeadContent;

            CmsScript bitSiteScript = new CmsScript() { ID = Guid.NewGuid(), ScriptType = ScriptTypeEnum.Js, Url = "/_js/bitSiteScript.js" };
            head += bitSiteScript.GetTag();
            //head += GetHeaderModuleDependentScripts();

//            head += @"<script type=""text/javascript"" src=""/_js/bitAjax.js""></script>
//<script type=""text/javascript"" src=""/_js/bitSiteScript.js""></script>
//<script type=""text/javascript"" src=""/_js/bitDragDropModules.js""></script>
//";
//            //ADD Jquery
//            string jQueryVersion = ConfigurationManager.AppSettings["jQueryVersion"];
//            if (jQueryVersion == null || jQueryVersion == "")
//            {
//                jQueryVersion = "jquery-1.8.2.js";
//            }
//            string jqueryScriptInclude = String.Format(@"
//<script type=""text/javascript"" src=""/_js/{0}""></script>
//", jQueryVersion);
//            head += jqueryScriptInclude;
//            string jQueryUIVersion = "/_bitPlate/_js/jquery-ui-1.9.1.custom.js";
//            string jqueryUIScriptInclude = String.Format(@"<script type=""text/javascript"" src=""{0}""></script>
//", jQueryUIVersion);
//            head += jqueryUIScriptInclude;

            head += @"
<script type=""text/javascript"">
    BITSITESCRIPT.pageId = '" + this.ID.ToString() + @"';
    BITSITESCRIPT.pageLanguage = '" + this.Template.LanguageCode + @"';
</script>
";
            if (Site.UseGoogleAnalystics)
            {
                //plaats google analytics code aan einde van head
                head += @"
<script type=""text/javascript"">
    //GOOGLE ANALYTICS
    var _gaq = _gaq || [];
    _gaq.push(['_setAccount', '" + Site.GoogleAnalysticsCode + @"']);
    _gaq.push(['_trackPageview']);

    (function() {
        var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
        ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
    })();
</script>
";

            }
            return head;
        }

        //public string GetHeaderMetaTags()
        //{
        //    string header = "";
        //    if (this.MetaKeywords != "")
        //    {
        //        header += String.Format("    <meta name=\"keywords\" runat=\"server\" id=\"MetaKeywords\" content=\"{0}\" />", this.MetaKeywords);
        //        header += "\r\n";
        //    }
        //    else
        //    {
        //        header += String.Format("    <meta name=\"keywords\" runat=\"server\" id=\"MetaKeywords\" content=\"{0}\" />", Site.MetaKeywords);
        //        header += "\r\n";
        //    }
        //    if (this.MetaDescription != "")
        //    {
        //        header += String.Format("    <meta name=\"description\" runat=\"server\" id=\"MetaDescription\" content=\"{0}\" />", this.MetaDescription);
        //        header += "\r\n";
        //    }
        //    else
        //    {
        //        header += String.Format("    <meta name=\"description\" runat=\"server\" id=\"MetaDescription\" content=\"{0}\" />", Site.MetaDescription);
        //        header += "\r\n";
        //    }
        //    return header;
        //}

        public List<string> GetModuleDependentScripts()
        {
            List<string> scripts = new List<string>();
            foreach (BaseModule mod in GetAllModules())
            {
                foreach (string script in mod.IncludeScripts)
                {
                    string scriptUrl = script;

                    if (!scripts.Contains(scriptUrl) && !scriptUrl.Contains("/.js"))
                    {
                        scripts.Add(scriptUrl);
                    }
                }
            }
            return scripts;
        }

        public string GetHeaderModuleDependentScripts()
        {
            string header = "";
            foreach (string script in GetModuleDependentScripts())
            {
                if (script.EndsWith(".js"))
                {
                    header += String.Format(@"<script type=""text/javascript"" src=""{0}""></script>
", script);
                }
                else if (script.EndsWith(".css"))
                {
                    header += String.Format(@"<link type=""text/css"" rel=""stylesheet"" href=""{0}"" />
", script);
                }

                header += "\r\n";
            }

            return header;
        }
        #endregion

        public static Guid GetPageIDByUrl(string url, string siteid)
        {
            if (url == null) url = "";
            url = url.Replace("'", "");
            if (!url.StartsWith("/"))
            {
                url = "/" + url;
            }
            if (url.EndsWith(".aspx"))
            {
                url = url.Replace(".aspx", "");
            }
            else if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            Guid returnValue = Guid.Empty;
            string where = String.Format("FK_Site='{0}' AND RelativeUrl = '{1}'", siteid, url);

            CmsPage page = BaseObject.GetFirst<CmsPage>(where);

            if (page != null)
            {
                returnValue = page.ID;
            }

            return returnValue;
        }

        public static Guid GetPageIDByFolderUrl(string relativePath, string siteid)
        {
            if (relativePath == null) relativePath = "";
            if (relativePath.StartsWith("/"))
            {
                relativePath = relativePath.Substring(1);
            }

            if (relativePath.EndsWith("/"))
            {
                relativePath = relativePath.Substring(0, relativePath.Length - 1);
            }
            Guid returnValue = Guid.Empty;
            string where = String.Format("FK_Site='{0}' AND RelativePath = '{1}'", siteid, relativePath);
            CmsPageFolder folder = BaseObject.GetFirst<CmsPageFolder>(where);

            if (folder != null)
            {
                //where = "FK_Folder = '" + folder.ID.ToString() + "' AND IsHomePage='1'";
                //CmsPage page = BaseObject.GetFirst<CmsPage>(where);

                CmsPage page = folder.GetHomePage();
                if (page != null)
                {
                    returnValue = page.ID;
                }
            }
            return returnValue;
        }

        public string GetRewriteUrl()
        {
            string Url = this.RelativeUrl.Replace(".aspx", "/") + this.Title.Replace(" ", "-") + ".html";
            return Url;
        }

        public string Publish2()
        {
            string html = Template.Content;
            string languageCode = Template.LanguageCode;

            string head = getHeader();
            
            //waardes title en meta-tags kunnen latyer worden overschreven door een details module
            //voor SEO
            string pageTitle = this.Title;
            string metaDescription = this.MetaDescription;
            string metaKeywords = this.MetaKeywords;
            if (metaDescription == null || metaDescription == "") metaDescription = Site.MetaDescription;
            if (metaKeywords == null || metaKeywords == "") metaKeywords = Site.MetaKeywords;

            Dictionary<string, string> modulesHtmlByContainerName = new Dictionary<string, string>();
            //List<BaseModule> modules = new List<BaseModule>();

            //string where = String.Format("FK_Site='{0}' AND (IsVisibleOnAllPages=true OR VisibleMode=2)", this.Site.ID);
            //BaseCollection<BaseModule> siteModules = BaseCollection<BaseModule>.Get(where, "OrderingNumber");
            //modules.AddRange(siteModules);
            ////foreach (BaseModule module in siteModules)
            ////{
            ////    if (!modulesHtmlByContainerName.ContainsKey(module.ContainerName))
            ////    {
            ////        modulesHtmlByContainerName.Add(module.ContainerName, "");
            ////    }
            ////    module.LanguageCode = languageCode;
            ////    modulesHtmlByContainerName[module.ContainerName] += module.ConvertToType().Publish2(this);
            ////}
            ////dan modules zichtbaar in de template
            //if (this.Template != null)
            //{
            //    where = String.Format("FK_Site='{0}' AND (IsVisibleOnAllPagesInLayout=true AND FK_Template = '{1}' OR VisibleMode=1 AND FK_Template = '{1}')", this.Site.ID, this.Template.ID);
            //    BaseCollection<BaseModule> templateModules = BaseCollection<BaseModule>.Get(where, "OrderingNumber");
            //    modules.AddRange(templateModules);
            //    //foreach (BaseModule module in templateModules)
            //    //{
            //    //    if (!modulesHtmlByContainerName.ContainsKey(module.ContainerName))
            //    //    {
            //    //        modulesHtmlByContainerName.Add(module.ContainerName, "");
            //    //    }
            //    //    module.LanguageCode = languageCode;
            //    //    modulesHtmlByContainerName[module.ContainerName] += module.ConvertToType().Publish2(this);
            //    //}
            //}
            ////en alle page modules
            ////BaseCollection<BaseModule> pageModules = BaseCollection<BaseModule>.Get("FK_Page='" + this.ID + "'", "OrderingNumber");
            //modules.AddRange(this.Modules);
            //modules = modules.GroupBy(c => c.ID).Select(c => c.FirstOrDefault()).OrderBy(c => c.OrderingNumber).ToList(); //Distinct().OrderBy(c => c.OrderingNumber).ToList();
            List<BaseModule> modules = this.GetAllModules();
            foreach (BaseModule module in modules)
            {
                bool allowEdit = (Utils.WebSessionHelper.CurrentBitplateUser != null);
                bool isAutorized = true;
                if (!allowEdit && module.HasSiteAutorisation())
                {
                    isAutorized = false;

                    if (Utils.WebSessionHelper.CurrentSiteUser != null)
                    {
                        isAutorized = Utils.WebSessionHelper.CurrentSiteUser.IsAutorized(module);
                    }
                }
                if (isAutorized)
                {
                    if (!modulesHtmlByContainerName.ContainsKey(module.ContainerName))
                    {
                        modulesHtmlByContainerName.Add(module.ContainerName, "");
                    }
                    //BaseModule convertedModule = module;//module.ConvertToType();
                    module.LanguageCode = languageCode;
                    modulesHtmlByContainerName[module.ContainerName] += module.Publish2(this);

                    if (module is ItemDetailsModule)
                    {
                        //get title en meta tags van item en vervang de page-title (zie einde van deze functie) voor SEO
                        string itemTitle = ((ItemDetailsModule)module).ItemTitle;
                        string itemMetaDescription = ((ItemDetailsModule)module).ItemMetaDescription;
                        string itemMetaKeywords = ((ItemDetailsModule)module).ItemMetaKeywords;

                        if (itemTitle != null && itemTitle != "") pageTitle = itemTitle;
                        if (itemMetaDescription != null && itemMetaDescription != "") metaDescription = itemMetaDescription;
                        if (itemMetaKeywords != null && itemMetaKeywords != "") metaKeywords = itemMetaKeywords;
                    }
                    else if (module is GroupDetailsModule)
                    {
                        //get title en meta tags van item en vervang de page-title (zie einde van deze functie) voor SEO
                        string groupTitle = ((GroupDetailsModule)module).GroupTitle;
                        string groupMetaDescription = ((GroupDetailsModule)module).GroupMetaDescription;
                        string groupMetaKeywords = ((GroupDetailsModule)module).GroupMetaKeywords;

                        if (groupTitle != null && groupTitle != "") pageTitle = groupTitle;
                        if (groupMetaDescription != null && groupMetaDescription != "") metaDescription = groupMetaDescription;
                        if (groupMetaKeywords != null && groupMetaKeywords != "") metaKeywords = groupMetaKeywords;
                    }
                }
            }

            //foreach (BaseModule module in this.GetAllModules())
            //{
            //    if (!modulesHtmlByContainerName.ContainsKey(module.ContainerName))
            //    {
            //        modulesHtmlByContainerName.Add(module.ContainerName, "");
            //    }
            //    modulesHtmlByContainerName[module.ContainerName] += module.Publish2();
            //}

            foreach (CmsTemplateContainer container in this.Template.Containers)
            {
                string containerContent = "";
                modulesHtmlByContainerName.TryGetValue(container.Name, out containerContent);

                containerContent = "<div id='bitContainer" + container.Name + "' class='bitContainer'>" + containerContent + "</div>";
                html = html.Replace("[" + container.Name.ToUpper() + "]", containerContent);
            }
            
            string metaTags = String.Format(@"
    <meta name=""keywords"" runat=""server"" id=""MetaKeywords"" content=""{0}"" />
    <meta name=""description"" runat=""server"" id=""MetaDescription"" content=""{1}"" />
            ", metaKeywords, metaDescription);

            html = html.Replace("[HEAD]", metaTags + head);
            html = html.Replace("[PAGETITLE]", pageTitle);
            //head += Template.GetHeadContent();

            html = replaceBodyClass(html);
            
            return html;
        }

        private string replaceBodyClass(string html)
        {
            if (this.BodyTagContent != null && this.BodyTagContent != "")
            {
                string bodyTag = Regex.Match(html, "<body(.*?)>", RegexOptions.Singleline).ToString();
                string classAttribute = Regex.Match(bodyTag, "class=\"(.*?)\"", RegexOptions.Singleline).ToString();
                string currentClasses = "";
                if (classAttribute != "")
                {
                    currentClasses = classAttribute.Replace("class=\"", "").Replace("\"", "") + " ";
                }
                currentClasses += this.BodyTagContent;
                string newClasseAtribute = "class=\"" + currentClasses + "\"";
                string newBodyTag;
                if (classAttribute != "")
                {
                    newBodyTag = bodyTag.Replace(classAttribute, newClasseAtribute);
                }
                else
                {
                    newBodyTag = bodyTag.Replace(">", " ") + newClasseAtribute + " >";
                }
                html = html.Replace(bodyTag, newBodyTag);
            }
            return html;
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            this.Title = dataRow["Title"].ToString();
            this.RelativeUrl = dataRow["RelativeUrl"].ToString();
            if (dataRow["FK_Layout"] != DBNull.Value)
            {
                this.Template = new CmsTemplate();
                this.Template.ID = DataConverter.ToGuid(dataRow["FK_Layout"]);
            }
            if (dataRow["FK_Folder"] != DBNull.Value)
            {
                this.Folder = new CmsPageFolder();
                this.Folder.ID = DataConverter.ToGuid(dataRow["FK_Folder"]);
            }
            this.IsHomePage = DataConverter.ToBoolean(dataRow["IsHomePage"]);
            this.HeadContent = dataRow["HeadContent"].ToString();
            this.MetaDescription = dataRow["MetaDescription"].ToString();
            this.MetaKeywords = dataRow["MetaKeywords"].ToString();
            this.MetaRobots = dataRow["MetaRobots"].ToString();
            this.InSiteMap = DataConverter.ToBoolean(dataRow["InSiteMap"]);
            this.SiteMapChangeFreq = dataRow["SiteMapChangeFreq"].ToString();
            this.SiteMapPriority = DataConverter.ToDouble(dataRow["SiteMapPriority"]);
            this.DisallowRobots = DataConverter.ToBoolean(dataRow["DisallowRobots"]);
            this.BodyTagContent = dataRow["BodyTagContent"].ToString();
            this.IsLoaded = true;
        }


    }
}
