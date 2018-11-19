using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Utils;
using BitPlate.Domain.Newsletters;

using BitPlate.Domain.DataCollections;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using BitPlate.Domain.Autorisation;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Configuration;
using System.Web.Script.Serialization;

namespace BitPlate.Domain.Modules
{
    public enum CrossPagesModeEnum
    {
        VisibleOnAllSelectedPages = 0,
        VisibleOnAllPagesInTemplate = 1,
        VisibleOnAllPages = 2,
        VisibleOnOnePage = 3,
    }

    [Persistent("Module")]
    public class BaseModule : BaseDomainPublishableObject
    {
        public BaseModule()
            : base()
        {
            ContentSamples = new List<string>();
            IncludeScripts = new List<string>();
            ConfigPageTabPages = new List<ModuleConfigTabPage>();
            Settings = new Dictionary<string, object>();

            //Convert page reference
            //if (this.PageDeprecated != null)
            //{
            //    this.Pages.Add(this.PageDeprecated);
            //    this._page = null;
            //    this.Save();
            //}
        }

        private bool saveAfterLoad = false;

        public string Type { get; set; }
        [XmlIgnore()]
        public string ContainerName { get; set; }

        
        public CrossPagesModeEnum CrossPagesMode {get; set;}
        

        [XmlIgnore()]
        public int OrderingNumber { get; set; }

        public string Content { get; set; }
        /// <summary>
        /// voor tree module. Bij grote trees duurt het (te) lang om tree op te bouwen. 
        /// data komt dan niet live uit DB, maar html wordt in deit veld gezet
        /// </summary>
        public string PrePublishedContent { get; set; }
        

        private DataCollection _dataCollection;
        [Association("FK_DataCollection")]
        public DataCollection DataCollection
        {
            get
            {
                if (_dataCollection != null && !_dataCollection.IsLoaded)
                {
                    _dataCollection.Load();
                }

                return _dataCollection;
            }
            set
            {
                _dataCollection = value;
            }
        }

        /// <summary>
        /// Maak van de module zijn subtype via de ModuleLoader
        /// </summary>
        /// <returns></returns>
        public BaseModule ConvertToType()
        {
            if (this.Type == "HtmlModule") return this;
            BaseModule returnModule = ModuleLoader.Load(this.Type);
            returnModule.ID = this.ID;
            returnModule.IsNew = this.IsNew;
            returnModule.Name = this.Name;
            returnModule.CreateDate = this.CreateDate;
            returnModule.ModifiedDate = this.ModifiedDate;
            returnModule.Content = this.Content;
            returnModule.Site = this.Site;
            returnModule.HasAutorisation = this.HasAutorisation;
            returnModule.AutorizedBitplateUserGroups = this.AutorizedBitplateUserGroups;
            returnModule.AutorizedBitplateUsers = this.AutorizedBitplateUsers;
            returnModule.AutorizedSiteUserGroups = this.AutorizedSiteUserGroups;
            returnModule.AutorizedSiteUsers = this.AutorizedSiteUsers;
            returnModule.PrePublishedContent = this.PrePublishedContent;
            returnModule.SettingsJsonString = this.SettingsJsonString;
            returnModule.ContainerName = this.ContainerName;
            returnModule.DataCollection = this.DataCollection;
            returnModule.CrossPagesMode = this.CrossPagesMode;
            returnModule.Pages = this.Pages;
            returnModule.Template = this.Template;
            returnModule.Newsletter = this.Newsletter;
            returnModule.OrderingNumber = this.OrderingNumber;
            returnModule.CrossPagesMode = this.CrossPagesMode;
            returnModule.Parameters = this.Parameters;
            returnModule.LanguageCode = this.LanguageCode;
            return returnModule;
        }

        //[NonPersistent()]
        //public bool InEditMode { get; set; }
        private CmsPage _page;
        [Association("FK_Page")]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public CmsPage PageDeprecated
        {
            get
            {
                if (_page != null && !_page.IsLoaded && CrossPagesMode != CrossPagesModeEnum.VisibleOnAllPagesInTemplate)
                {
                    _page.Load();
                }

                return _page;
            }
            set
            {
                _page = value;
            }
        }
        //Lazy test

        private BaseCollection<CmsPage> _pages;
        [Persistent("moduleperpage")]
        [Association("FK_Module", "FK_Page")]
        public BaseCollection<CmsPage> Pages
        {
            get
            {
                if (this._pages == null)
                {

                    string where = "EXISTS (SELECT * FROM moduleperpage WHERE FK_Module = '" + this.ID + "' AND page.ID = moduleperpage.FK_Page)";
                    this._pages = BaseCollection<CmsPage>.Get(where);


                    if (this.PageDeprecated != null)
                    {
                        if (!this._pages.Contains(PageDeprecated))
                        {
                            this._pages.Add(PageDeprecated);
                        }
                    }
                }
                return this._pages;
            }
            set
            {
                this._pages = value;
                //this._pages.IsLoaded = true;
            }
        }

        //[NonPersistent()]
        //public bool InEditMode { get; set; }
        private Domain.Newsletters.Newsletter _newsletter;
        [Association("FK_Newsletter")]
        [XmlIgnore]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public Domain.Newsletters.Newsletter Newsletter
        {
            get
            {
                if (_newsletter != null && !_newsletter.IsLoaded && CrossPagesMode != CrossPagesModeEnum.VisibleOnAllPagesInTemplate)
                {
                    _newsletter.Load();
                }

                return _newsletter;
            }
            set
            {
                _newsletter = value;
            }
        }

        [NonPersistent()]
        [System.Web.Script.Serialization.ScriptIgnore()]
        [XmlIgnore()]
        private string _settingsJsonString = "";

        [Persistent("Settings")]
        public string SettingsJsonString
        {
            get { return this._settingsJsonString; }
            set
            {
                this._settingsJsonString = value;
                _settings = JSONSerializer.Deserialize<Dictionary<string, object>>(value);
            }
        }

        private Dictionary<string, object> _settings;
        [NonPersistent()]
        [System.Web.Script.Serialization.ScriptIgnore()]
        [XmlIgnore()]
        public Dictionary<string, object> Settings
        {
            get
            {
                if (_settings == null)
                {
                    if (_settingsJsonString == null || _settingsJsonString == "")
                    {
                        Settings = new Dictionary<string, object>();
                    }
                    else
                    {
                        _settings = JSONSerializer.Deserialize<Dictionary<string, object>>(_settingsJsonString);
                    }
                }


                return _settings;
            }
            set
            {
                _settings = value;
            }
        }

        private Dictionary<string, object> _parameters;
        [System.Web.Script.Serialization.ScriptIgnore()]
        [XmlIgnore()]
        [NonPersistent()]
        public Dictionary<string, object> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new Dictionary<string, object>();
                }

                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        private string _languageCode = "";

        [NonPersistent()]
        public string LanguageCode
        {
            get
            {
                return this._languageCode;
            }
            set
            {
                this._languageCode = value;
            }
        }


        protected List<Tag> _tags = null;



        /// <summary>
        /// tags uit SetAllTags + tags op basis van Fields uit datacollection
        /// </summary>
        /// <returns></returns>
        public List<Tag> GetAllTags()
        {
            if (_tags == null)
            {
                _tags = new List<Tag>();
                SetAllTags();

            }
            return this._tags;
        }

        public virtual void SetAllTags()
        {
        }


        protected List<string> _includeScripts;
        [NonPersistent()]
        public virtual List<string> IncludeScripts
        {
            get
            {

                return _includeScripts;
            }
            set
            {
                _includeScripts = value;
            }
        }



        [NonPersistent()]
        public string ConfigPageUrl { get; set; }

        [NonPersistent()]
        public List<ModuleConfigTabPage> ConfigPageTabPages { get; set; }


        [NonPersistent()]
        public List<string> ContentSamples { get; set; }



        [NonPersistent()]
        public bool IsNavigationModuleRequired { get; set; }

        [NonPersistent()]
        public string FirstContentSample
        {
            get
            {
                string returnValue = "";
                if (ContentSamples != null && ContentSamples.Count > 0)
                {
                    returnValue = ContentSamples[0];
                }
                return returnValue;
            }
        }



        private CmsTemplate _template;
        [XmlIgnore()]
        [Association("FK_Template")]
        [System.Web.Script.Serialization.ScriptIgnore()]
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
            set
            {
                _template = value;
            }
        }




        public virtual string GetErrorHtml(string errorMessage)
        {

            errorMessage = errorMessage.Replace("{", "&#123;").Replace("}", "&#125;");
            string returnValue = @"
<!--ERROR IN MODULE -->
<div style=""padding: 5px; border: 3px solid red;"" class= ""bitModuleError"">
<strong style=""color: black;"">Er is een fout opgetreden in deze module.</strong><br />
<span style=""color: red;"">Melding: " + errorMessage + @"</span>
</div>
";
            return returnValue;
        }

        protected virtual ValidationResult validateModule()
        {
            ValidationResult result = new ValidationResult(true);

            foreach (Tag tag in this.GetAllTags())
            {
                if (tag.HasCloseTag)
                {
                    if (this.Content.Contains(tag.Name) && !this.Content.Contains(tag.CloseTag))
                    {
                        result.Message += "Module is niet geldig. Module moet sluittag bevatten: " + tag.CloseTag + "<br/>";
                    }
                    else if (!this.Content.Contains(tag.Name) && this.Content.Contains(tag.CloseTag))
                    {
                        result.Message += "Module is niet geldig. Module moet openingstag bevatten: " + tag.Name + "<br/>";
                    }
                }
            }
            return result;
        }

        public virtual string ConvertTags(string html)
        {

            foreach (Tag tag in GetAllTags())
            {
                if (tag.ReplaceValue != String.Empty)
                {
                    html = html.Replace(tag.Name, tag.ReplaceValue);
                }
                if (tag.HasCloseTag && tag.ReplaceValueCloseTag != String.Empty)
                {
                    html = html.Replace(tag.CloseTag, tag.ReplaceValueCloseTag);
                }
            }

            return html;
        }

        public override string ToString()
        {
            string html = getModuleStartDiv();
            html += Content;
            html += "</div>";
            return html;
        }
        private string getTagContent(string ModuleContent, Tag tag)
        {

            string searchPattern = tag.Name + "(.*?)" + tag.CloseTag;
            Match ContentMatch = Regex.Match(ModuleContent, searchPattern, RegexOptions.Singleline);
            return (ContentMatch.Success) ? ContentMatch.ToString().Replace(tag.Name, "").Replace(tag.CloseTag, "") : "";
        }

        private void RemoveUnkownModuleTags() //Tijdelijke fix (Conversie prog genereerd niet bestaande tags)
        {
            if (this.NavigationActions != null)
            {
                List<ModuleNavigationAction> UnkownActions = new List<ModuleNavigationAction>();
                GetAllTags();
                foreach (ModuleNavigationAction action in this.NavigationActions)
                {
                    if (!this._tags.Any(c => c.Name == action.Name))
                    {
                        UnkownActions.Add(action);
                    }
                }
                foreach (ModuleNavigationAction action in UnkownActions)
                {
                    this.NavigationActions.Remove(action);
                }
            }
        }

        public override void Save()
        {
            //RemoveUnkownModuleTags();
            base.Save();
            
            //UnpublishedItem.Set(this, "Module", true);
            
            if (this.Newsletter != null)
            {
                this.Newsletter.Save();
            }

            foreach (CmsPage page in this.Pages)
            {
                page.Save(); //UPDATE MODIFIED DATE
            }

        }

        internal BaseModule Copy(string newName, Guid pageID)
        {
            BaseModule newModule = base.CreateCopy<BaseModule>(false);

            if (!this.hasReferenceToPage(pageID))
            {
                CmsPage page = BaseObject.GetById<CmsPage>(pageID);
                newModule.Pages.Add(page);
            }
            //newModule.Page = new CmsPage();
            //newModule.Page.ID = pageID;
            newModule.Save();
            return newModule;
        }

        //Misschien moet dit ergens anders komen te staan. Vind het hier niet echt super mooi, maar weet even geen betere plek.
        public bool hasReferenceToPage(Guid pageid)
        {
            string sql = "SELECT COUNT(ID) AS amount FROM moduleperpage WHERE FK_Module = '" + this.ID.ToString() + "' AND FK_Page = '" + pageid.ToString() + "'";
            System.Data.DataTable referenceAmountTable = DataBase.Get().GetDataTable(sql);
            if (referenceAmountTable.Rows.Count > 0)
            {
                int amount = int.Parse(referenceAmountTable.Rows[0]["amount"].ToString());
                return (amount > 0);
            }
            return false;
        }

        protected virtual string getModuleStartDiv()
        {
            //Extra setting voor module editor. HIDECKEDITOR ja;nee?
            string moduleEditorType = "CKEDITOR";
            if (this.getSetting<bool>("HideCkEditor"))
            //if (this.Settings == null || !this.Settings.ContainsKey("UseCkeditor") || this.Settings.ContainsKey("UseCkeditor") && !(bool)this.Settings["UseCkeditor"])
            {
                moduleEditorType = "CODEMIRROR";
            }
            string extraClass = "";
            if (this.HasBitplateAutorisation())
            {
                extraClass = " bitModuleHasBitplateAutorisation";
            }
            else if (this.HasSiteAutorisation())
            {
                extraClass = " bitModuleHasSiteAutorisation";
            }
            string moduleStartDiv = String.Format(@"
<!-- MODULE: {1} -->
<div id=""bitModule{0}"" class=""bitModule{4}"" data-module-type=""{1}"" data-module-name=""{2}"" data-module-editor=""{3}"" >
", this.ID, this.Type, this.Name, moduleEditorType, extraClass);


            return moduleStartDiv;
        }

        protected virtual string getModuleEndDiv()
        {
            if (this is IPostableModule)
            {
                return @"
</form></div>
";
            }
            else
            {
                return @"</div>
";
            }
        }



        /// <summary>
        /// In het geval er list tags worden gebruikt (bijv {list} en {/list} of {files} en {/files})
        /// Wordt hier de waarde hiertussen weergegeven
        /// </summary>
        /// <returns></returns>
        internal string GetSubTemplate(string subTemplateTag)
        {
            string subTemplateEndTag = subTemplateTag.Replace("{", "{/");
            string subTemplate = Content;
            subTemplate = Regex.Replace(subTemplate, "<!--" + subTemplateTag + "-->", subTemplateTag);
            subTemplate = Regex.Replace(subTemplate, "<!--" + subTemplateEndTag + "-->", subTemplateEndTag);
            return GetSubTemplate(subTemplateTag, subTemplate);
        }

        internal string GetSubTemplate(string subTemplateTag, string template)
        {
            string subTemplateEndTag = subTemplateTag.Replace("{", "{/");
            string subTemplate = template;
            if (template.Contains(subTemplateTag) && template.Contains(subTemplateEndTag))
            {
                int start = template.IndexOf(subTemplateTag) + subTemplateTag.Length;
                int end = template.IndexOf(subTemplateEndTag);

                subTemplate = template.Substring(start, end - start);
            }
            return subTemplate;
        }

        internal string ReplaceSubTemplate(string subTemplateTag, string html, string replaceValue)
        {
            string originalTemplate = GetSubTemplate(subTemplateTag);
            html = ReplaceSubTemplate(subTemplateTag, html, replaceValue, originalTemplate);
            return html;
        }

        internal string ReplaceSubTemplate(string subTemplateTag, string html, string replaceValue, string originalTemplate)
        {
            string subTemplateEndTag = subTemplateTag.Replace("{", "{/");
            string subTemplateTagCommented = "<!--" + subTemplateTag + "-->", subTemplateEndTagCommented = "<!--" + subTemplateEndTag + "-->";
            html = html.Replace(subTemplateTag + originalTemplate + subTemplateEndTag, replaceValue);
            html = html.Replace(subTemplateTagCommented + originalTemplate + subTemplateEndTagCommented, replaceValue);
            return html;
        }

        internal string EmptySubTemplate(string subTemplateTag, string html)
        {
            return ReplaceSubTemplate(subTemplateTag, html, "");
        }


        public override void Delete()
        {
            base.Delete();
        }

        public string GetFormat(string tagWithFormat)
        {
            string returnValue = "";
            int pos = tagWithFormat.IndexOf(":");
            if (pos > 0)
            {
                returnValue = tagWithFormat.Substring(pos + 1, tagWithFormat.Length - pos - 2);
                //voor de zekerheid } erafhalen, voor het geval er spaties in tag zitten
                returnValue = returnValue.Replace("}", "");
            }

            return returnValue.Trim();

        }

        protected BaseCollection<ModuleNavigationAction> _navigationActions;
        ////cascade delete in db
        [Persistent("ModuleNavigationAction")]
        [Association(ForeignObjectName = "Module")]
        public BaseCollection<ModuleNavigationAction> NavigationActions
        {
            get
            {
                if (_navigationActions == null)
                {
                    setNavigationActions();
                }

                return _navigationActions;
            }
            set
            {
                _navigationActions = value;
            }
        }


        protected virtual void setNavigationActions()
        {
            _navigationActions = BaseCollection<ModuleNavigationAction>.Get("FK_Module='" + this.ID + "'");
        }

        public ModuleNavigationAction GetNavigationActionByTagName(string tagName)
        {
            ModuleNavigationAction returnValue = null;
            if (this.NavigationActions.Count > 0)
            {
                tagName = "{" + tagName.Replace("{", "").Replace("}", "") + "}";
                returnValue = this.NavigationActions.Where(c => c.Name == tagName).FirstOrDefault();
                returnValue.Module = this;
            }
            return returnValue;
        }



        public virtual string Publish2(CmsPage page)
        {
            ValidationResult result = validateModule();
            if (!result.IsValid)
            {
                this.Content = GetErrorHtml(result.Message);
            }

            return this.getModuleStartDiv() + this.Content + this.getModuleEndDiv();
        }

        internal T getSetting<T>(string key)
        {
            T returnValue = default(T);
            //if (Settings == null)
            //{
            //    LoadSettings();
            //}
            if (Settings != null && Settings.ContainsKey(key))
            {
                object value = Settings[key];
                if (value != null && value.ToString() == "" && typeof(T) == typeof(String))
                {
                    returnValue = (T)value;
                }
                else if (value != null && value.ToString() != "")
                {

                    if (typeof(T).IsEnum)
                    {
                        returnValue = (T)Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(T)));
                    }
                    else if (typeof(T) == typeof(Guid))
                    {
                        returnValue = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value.ToString());

                        //returnValue = new Guid(value.ToString()); // (T)new Guid(value.ToString()); // Guid.TryParse(value.ToString(), out returnValue);
                    }
                    else
                    {
                        returnValue = (T)Convert.ChangeType(value, typeof(T)); //T.TryParse( (T)Settings[key];
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Maakt paginering aan voor searchresultmodule, itemlistmodule
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalResults"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        protected string createPager(int currentPage, int totalResults, int pageSize)
        {

            StringBuilder sb = new StringBuilder();
            //stuur selectie parameters mee
            string jsonParameters = JSONSerializer.Serialize(Parameters);
            jsonParameters = jsonParameters.Replace("\"", "'");

            if (totalResults == 0 || pageSize == 0 || (totalResults < pageSize))
            {
                return "";
            }
            else
            {
                int numberOfPages = (totalResults / pageSize);
                int firstPage = 0;
                int lastPage = numberOfPages;
                if (numberOfPages > 10)
                {
                    //om ervoor te zorgen dat er niet meer dan 10 pagina nummers verschijnen
                    firstPage = currentPage - 5;
                    if (firstPage < 0) firstPage = 0;
                    lastPage = currentPage + 5;
                    if (lastPage < 9) lastPage = 9;
                    if (lastPage > numberOfPages) lastPage = numberOfPages;
                }
                if (firstPage > 0)
                {
                    sb.AppendFormat(@"<a class=""bitPagerLink"" href=""javascript:void(0)"" onclick=""BITSITESCRIPT.doPaging('{0}', {1}, {2}, {3});"">...</a> ", this.ID, firstPage - 1, totalResults, jsonParameters);
                }
                for (int i = firstPage; i <= lastPage; i++)
                {
                    if (i == currentPage)
                    {
                        sb.AppendFormat(@"<a class=""bitPagerLink"" href=""javascript:void(0);""><strong>{0}</strong></a> ", (i + 1));
                    }
                    else
                    {
                        sb.AppendFormat(@"<a class=""bitPagerLink"" href=""javascript:void(0)"" onclick=""BITSITESCRIPT.doPaging('{0}', {1}, {2}, {3});"">{4}</a> ", this.ID, i, totalResults, jsonParameters, (i + 1));
                    }
                }
                if (lastPage < numberOfPages)
                {
                    sb.AppendFormat(@"<a class=""bitPagerLink"" href=""javascripts:void(0)"" onclick=""BITSITESCRIPT.doPaging('{0}', {1}, {2}, {3});"">...</a> ", this.ID, lastPage + 1, totalResults, jsonParameters);
                }
            }
            string html = sb.ToString();
            return sb.ToString();

        }

        internal virtual string HandlePublishError(Exception ex)
        {
            string Message = "\r\nMESSAGE: " + ex.Message +
             "\r\nSOURCE: " + ex.Source +
             "\r\nTARGETSITE: " + ex.TargetSite +
             "\r\nSTACKTRACE: " + ex.StackTrace;
            string path = ConfigurationManager.AppSettings["ErrorLogPath"];
            if (path == null || path == "") path = AppDomain.CurrentDomain.BaseDirectory;
            Logger.Log(path + @"\error_log_.txt", Message);

            string html = getModuleStartDiv() + GetErrorHtml(ex.Message) + getModuleEndDiv();

            return html;
        }


        public override void FillObject(Type type, System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            this.Type = dataRow["Type"].ToString();

            this.Content = dataRow["Content"].ToString();
            this.PrePublishedContent = dataRow["PrePublishedContent"].ToString();
            this.SettingsJsonString = dataRow["Settings"].ToString();
            this.ContainerName = dataRow["ContainerName"].ToString();

            this.CrossPagesMode = (CrossPagesModeEnum)int.Parse(dataRow["CrossPagesMode"].ToString());
            if (dataRow["FK_DataCollection"] != DBNull.Value)
            {
                this.DataCollection = new DataCollection();
                this.DataCollection.ID = new Guid(dataRow["FK_DataCollection"].ToString());
            }
            if (dataRow["FK_Page"] != DBNull.Value)
            {
                this._page = new CmsPage();
                this._page.ID = new Guid(dataRow["FK_Page"].ToString());
            }
            if (dataRow["FK_Template"] != DBNull.Value)
            {
                this.Template = new CmsTemplate();
                this.Template.ID = new Guid(dataRow["FK_Template"].ToString());
            }
            if (dataRow["FK_Newsletter"] != DBNull.Value)
            {
                this.Newsletter = new Domain.Newsletters.Newsletter();
                this.Newsletter.ID = new Guid(dataRow["FK_Newsletter"].ToString());
            }
            if (dataRow["OrderingNumber"] != DBNull.Value)
            {
                this.OrderingNumber = int.Parse(dataRow["OrderingNumber"].ToString());
            }
            this.IsLoaded = true;
            if (saveAfterLoad)
            {
                this.Save();
            }

        }
    }
}
