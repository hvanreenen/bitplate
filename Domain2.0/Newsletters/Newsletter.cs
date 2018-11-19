using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitPlate.Domain.Modules;
using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Utils;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Web.Compilation;
using System.Web.UI;
using System.IO;
using System.Web;
using System.Configuration;

namespace BitPlate.Domain.Newsletters
{
    public enum NewsletterMode { GuestOnline, Online, Newsletter };

    [Persistent("newsletter")]
    public class Newsletter: CmsPage
    {
        /* private CmsTemplate _template;
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
        } */

        [NonPersistent]
        private BaseCollection<BaseModule> _Modules;

        [NonPersistent]
        public BaseCollection<BaseModule> Modules
        {
            get
            {
                if (this._Modules == null)
                {
                    this._Modules = BaseCollection<BaseModule>.Get("FK_Newsletter = '" + this.ID + "'");
                }
                return this._Modules;
            }
            set
            {
                this._Modules = value;
            }
        }

        [NonPersistent]
        private BaseCollection<NewsletterMailingStatistics> _Statistics;

        [NonPersistent]
        public BaseCollection<NewsletterMailingStatistics> Statistics
        {
            get
            {
                if (this._Statistics == null)
                {
                    this._Statistics = BaseCollection<NewsletterMailingStatistics>.Get("FK_Newsletter = '" + this.ID + "'");
                }
                return this._Statistics;
            }
        }

        private BaseCollection<NewsletterGroup> _Groups;
        [Persistent("newsletterpergroup")]
        [Association("FK_Newsletter", "FK_NewsletterGroup")]
        public BaseCollection<NewsletterGroup> Groups
        {
            get
            {
                if (this._Groups == null)
                {
                    string where = "EXISTS (SELECT * FROM newsletterpergroup WHERE FK_Newsletter = '" + this.ID + "' AND newslettergroup.ID = newsletterpergroup.FK_NewsletterGroup)";
                    this._Groups = BaseCollection<NewsletterGroup>.Get(where);
                    this._Groups.IsLoaded = true;
                }
                return this._Groups;
            }
            set
            {
                this._Groups = value;
                this._Groups.IsLoaded = true;
            }
        }

        private BaseCollection<NewsletterMailing> _Mailings;

        public BaseCollection<NewsletterMailing> Mailings
        {
            get
            {
                if (this._Mailings == null)
                {
                    string where = "FK_Newsletter = '" + this.ID + "'";
                    this._Mailings = BaseCollection<NewsletterMailing>.Get(where);
                }
                return this._Mailings;
            }
            set
            {
                this._Mailings = value;
            }
        }

        [NonPersistent]
        public string MailingGroupsString
        {
            get
            {
                string mailingGroups = "";
                foreach (NewsletterGroup group in this.Groups)
                {
                    if (mailingGroups == "")
                    {
                        mailingGroups += group.Name;
                    }
                    else
                    {
                        mailingGroups += ", " + group.Name;
                    }
                }
                return mailingGroups;
            }
        }

        public bool IsSent { get; set; }
        public string Title { get; set; }
        public bool SendNewsletterAfterRegistration { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? SendDate { get; set; }
        public string SentFromAddress { get; set; }
        public string Subject { get; set; }

        public void Send(NewsletterSubscriber Subscriber)
        {
            string content = Publish2(NewsletterMode.Newsletter, null);
            var Mailings = Subscriber.Mailings.Where(c => c.Newsletter.ID == this.ID);
            NewsletterMailing Mailing;
            Mailing = new NewsletterMailing();
            Mailing.Site = Subscriber.Site;
            Mailing.Subscriber = Subscriber;
            Mailing.Newsletter = this;
            this.Mailings.Add(Mailing);
           
            Mailing.Name = this.Name + "_" + DateTime.Now;
            Mailing.EmailAddress = Subscriber.Email;
            Mailing.Save();
            string subscriberContent = this.ReplaceSubscriberTags(content, Subscriber, NewsletterMode.Newsletter, Mailing);
            Mailing.NewsletterSent = EmailManager.SendMail(this.SentFromAddress, Subscriber.Email, this.Subject, subscriberContent, true);
            Mailing.Save();
        }

        private string ReplaceSubscriberTags(string content, NewsletterSubscriber subscriber, NewsletterMode showMode, NewsletterMailing mailing = null)
        {
            content = content.Replace("[NAME]", (subscriber.Name != "") ? subscriber.Name : subscriber.Email)
            .Replace("[FORENAME]", subscriber.ForeName)
            .Replace("[NAMEPREFIX]", subscriber.NamePrefix)
            .Replace("[COMPANYNAME]", subscriber.CompanyName)
            .Replace("[DATE]", DateTime.Now.ToString("yyyy-MM-dd / hh:mm:ss"))
            .Replace("[SEXE]", subscriber.GenderString)
            .Replace("[BEGINNING]", subscriber.GenderString)
            .Replace("[LIVEURL]", this.Site.DomainName + "/" + this.RelativeUrl)
            .Replace("[UNSUBSCRIBEURL]", (mailing != null && this.Site.NewsletterOptOutEmailPage != null) ? this.Site.DomainName + this.Site.NewsletterOptOutEmailPage.RelativeUrl + "?subscriber=" + mailing.Subscriber.ID.ToString() : "")
            .Replace("[USERCODE]", subscriber.ID.ToString());

            if (showMode == NewsletterMode.Online || showMode == NewsletterMode.GuestOnline)
            {
                content = content.Replace("{OnlineModeTemplate}", "").Replace("{/OnlineModeTemplate}", "");
                content = Regex.Replace(content, "{NewsletterModeTemplate}(.*?){/NewsletterModeTemplate}", "");
            }
            else
            {
                content = content.Replace("{NewsletterModeTemplate}", "").Replace("{/NewsletterModeTemplate}", "");
                content = Regex.Replace(content, "{OnlineModeTemplate}(.*?){/OnlineModeTemplate}", "");
            }

            //Is er een mailing mee gegeven?
            if (mailing != null)
            {
                content = ReplaceAttribute("href", content, mailing);
                content = ReplaceAttribute("src", content, mailing);
            }
            return content;
        }

        public string ReplaceAttribute(string attribute, string content, NewsletterMailing mailing)
        {
            foreach (Match match in Regex.Matches(content, attribute + "=\"(.*?)\""))
            {
                string hrefValue = match.ToString().Replace(attribute + "=\"", "").Replace("\"", "");
                if (!hrefValue.Contains("?subscriber"))
                {
                    if (!hrefValue.StartsWith("http://"))
                    {
                        if (!hrefValue.StartsWith("/"))
                        {
                            hrefValue = "/" + hrefValue;
                        }

                        hrefValue = Site.DomainName + hrefValue;
                    }
                    hrefValue += "?mailing=" + mailing.ID.ToString(); //subscriber.ID.ToString();

                    content = content.Replace(match.ToString(), attribute + "=\"" + hrefValue + "\"");
                }
            }
            return content;
        }

        protected override string getHeader()
        {
            string head = "";
            //head += GetHeaderMetaTags();
            BaseCollection<CmsScript> scripts = this.Site.Scripts;
            scripts.AddRange(this.Template.Scripts);
            scripts.AddRange(this.Scripts);

            head += Site.HeadContent;
            head += HeadContent;
            foreach (CmsScript script in scripts)
            {
                head += script.GetTag();
            }
            
            head += GetHeaderModuleDependentScripts();

            head += @"<script>
                    $(document).ready(function () {
                        if(parent && window.location != window.parent.location){
                            //vanuit iframe geladen
                            parent.BITEDITPAGE.newsletterId= '" + this.ID.ToString() + @"';
                            parent.BITEDITPAGE.language= '" + this.Template.LanguageCode + @"';
                            $('.bitContainer').sortable({
                                placeholder: 'bitContainerPlaceholder',
                                connectWith: '.bitContainer',
                                handle: '.moduleMoveGrip',
    
                                update: function (event, ui) {
                                    if (this === ui.item.parent()[0]) {
                                        var item = ui.item;
                                        var isNew = ($(item).is('.moduleToDrag'));
                                        var moduleType = $(item).attr('data-module-type');
                                        var id = $(item).attr('id');
                                        var containerName = $(this).attr('id').replace(""bitContainer"", """");
                                        var index = $(this).children().index(ui.item[0]);


                                        BITAJAX.dataServiceUrl = ""/_bitplate/EditPage/ModuleService2.aspx"";

                                        if (isNew) {
                                            var parametersObject = { type: moduleType, pageid: parent.BITEDITPAGE.pageId, containername: containerName, sortorder: index, newsletterid: parent.BITEDITPAGE.newsletterId };
                                            var jsonstring = JSON.stringify(parametersObject);
                                            BITAJAX.callWebService(""AddNewModule"", jsonstring, function (data) {
                                                item.replaceWith(data.d[0]);
                                                BITEDITPAGE.attachModuleCommands();
                                                BITEDITPAGE.addModuleScriptsToHead(data.d[1]);
                                                BITEDITPAGE.fillModulesCheckBoxList(parent.BITEDITPAGE.pageId);
                                            });
                                        }
                                        else {
                                            //sleep van ene container naar andere of van ene positie naar andere
                                            var parametersObject = { moduleid: id, pageid: parent.BITEDITPAGE.pageId, containername: containerName, sortorder: index, newsletterid: parent.BITEDITPAGE.newsletterId };
                                            var jsonstring = JSON.stringify(parametersObject);
                                            BITAJAX.callWebService(""MoveModule"", jsonstring, null);
                                        }
                                    }
                                },                
    
                                stop: function (event, ui) {
                                    var moduleMoveGrip = $(ui.item).find('.moduleMoveGrip');
                                    $(moduleMoveGrip).css({ 'width': 0, 'height': 0, 'display': 'none' });
                                    parent.BITEDITPAGE.calulateOrderingNumber(ui.item);
                                    var moduleId = $(ui.item).attr('id').replace('bitModule', '');
                                    //verberg infodivje
                                    $('#ModuleInfo' + moduleId).remove();
                                }
                                //$('.bitContainer').disableSelection();
                            });
                        }
                    });
            </script>";

            head += @"<script type=""text/javascript"" src=""/_js/bitAjax.js""></script>";
                      /* <script type=""text/javascript"" src=""/_js/bitSiteScript.js""></script>"; */
            return head;
        }

        public string Publish2()
        {
            return Publish2(NewsletterMode.GuestOnline, null);
        }

        public string Publish2(NewsletterMode newsletterMode, NewsletterMailing mailing)
        {
            string html = Template.Content;

            string head = "";
            if (newsletterMode != NewsletterMode.Newsletter)
            {
                string jQueryVersion = ConfigurationManager.AppSettings["jQueryVersion"];
                if (jQueryVersion == null || jQueryVersion == "")
                {
                    jQueryVersion = "jquery-1.8.2.js";
                }
                string jqueryScriptInclude = String.Format(@"
<script type=""text/javascript"" src=""/_js/{0}""></script>
", jQueryVersion);
                head = jqueryScriptInclude;
                string jQueryUIVersion = "/_bitPlate/_js/jquery-ui-1.9.1.custom.js";
                string jqueryUIScriptInclude = String.Format(@"<script type=""text/javascript"" src=""{0}""></script>
", jQueryUIVersion);
                head += jqueryUIScriptInclude;
                head += getHeader();
            }
            foreach (CmsScript script in this.Template.Scripts)
            {
                if (script.ScriptType == ScriptTypeEnum.Css)
                {
                    head += @"<style>
" + script.Content + @"
</style>";
                }
            }
            foreach (CmsScript script in this.Scripts)
            {
                if (script.ScriptType == ScriptTypeEnum.Css)
                {
                    head += @"<style>
" + script.Content + @"
</style>";
                }
            }

            
            html = html.Replace("[HEAD]", head);

            Dictionary<string, string> modulesHtmlByContainerName = new Dictionary<string, string>();

            string where = "";
            // modules zichtbaar in de template
            if (this.Template != null)
            {
                where = String.Format("FK_Site='{0}' AND (CrossPagesMode=1 AND FK_Template = '{1}')", this.Site.ID, this.Template.ID);
                BaseCollection<BaseModule> templateModules = BaseCollection<BaseModule>.Get(where, "OrderingNumber");
                this.Modules.AddRange(templateModules);
                //foreach (BaseModule module in templateModules)
                //{
                //    if (!modulesHtmlByContainerName.ContainsKey(module.ContainerName))
                //    {
                //        modulesHtmlByContainerName.Add(module.ContainerName, "");
                //    }
                //    modulesHtmlByContainerName[module.ContainerName] += module.ConvertToType().Publish2(this);
                //}
            }
            //en alle newsletter modules
            BaseCollection<BaseModule> newsletterModules = BaseCollection<BaseModule>.Get("FK_Newsletter='" + this.ID + "'", "OrderingNumber");
            List<BaseModule> modules = this.Modules.GroupBy(c => c.ID).Select(c => c.FirstOrDefault()).OrderBy(c => c.OrderingNumber).ToList();
            foreach (BaseModule module in modules)
            {
                if (!modulesHtmlByContainerName.ContainsKey(module.ContainerName))
                {
                    modulesHtmlByContainerName.Add(module.ContainerName, "");
                }
                modulesHtmlByContainerName[module.ContainerName] += module.ConvertToType().Publish2(this);
            }

            foreach (CmsTemplateContainer container in this.Template.Containers)
            {
                string containerContent = "";
                modulesHtmlByContainerName.TryGetValue(container.Name, out containerContent);

                containerContent = "<div id='bitContainer" + container.Name + "' class='bitContainer'>" + containerContent + "</div>";
                html = html.Replace("[" + container.Name.ToUpper() + "]", containerContent);
            }
            //head += Template.GetHeadContent();

            switch (newsletterMode)
            {
                case NewsletterMode.GuestOnline:
                    NewsletterSubscriber subscriber = new NewsletterSubscriber();
                    subscriber.ForeName = "Gast";
                    subscriber.Gender = Autorisation.BaseUser.SexeEnum.Undefined;
                    html = ReplaceSubscriberTags(html, subscriber, newsletterMode);
                    break;

                case NewsletterMode.Online:
                    html = ReplaceSubscriberTags(html, mailing.Subscriber, newsletterMode);
                    break;

                case NewsletterMode.Newsletter:
                    //Het plaatsen van de waardes in een nieuwsbrief gebeurd tijdens het verzenden, omdat elke nieuwsbrief voorzien moet worden van andere gegevens.
                    break;
            }

            return html;
        }

        public void Send()
        {
            //List<NewsletterSubscriber> Subscribers = new List<NewsletterSubscriber>();
            //foreach (NewsletterGroup Group in this.Groups)
            //{
            //    Subscribers.AddRange(Group.Subscribers);
            //}
            //Send(Subscribers);
        }

        public static Guid GetNewsletterIDByUrl(string url, string siteid)
        {
            if (url == null) url = "";
            url = url.Replace("'", "");
            if (url.IndexOf("/") < 0)
            {
                url = "/" + url;
            }
            if (url.EndsWith(".aspx"))
            {
                url = url.Replace(".aspx", "");
            }

            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            Guid returnValue = Guid.Empty;
            string where = String.Format("FK_Site='{0}' AND RelativeUrl = '{1}'", siteid, url);

            Newsletter page = BaseObject.GetFirst<Newsletter>(where);

            if (page != null)
            {
                returnValue = page.ID;
            }

            return returnValue;
        }

        public void Copy()
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            base.Delete();
        }

        public void SendPreview()
        {
            throw new System.NotImplementedException();
        }

        public override void Save()
        {
            if (IsNew && WebSessionHelper.CurrentLicense != null && WebSessionHelper.CurrentLicense.HasMaxNumberExceeded("Newsletter"))
            {
                throw new Exception("Kan niet opslaan: licentie heeft maximale aantal nieuwsbrieven (" + WebSessionHelper.CurrentLicense.MaxNumberOfNewsletters + ") overschreden.");
            }
            RelativeUrl = "/newsletter/" + Name;
            base.Save();
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            this.SentFromAddress = dataRow["SentFromAddress"].ToString();
            this.Subject = dataRow["Subject"].ToString();
            this.SendNewsletterAfterRegistration = Convert.ToBoolean(dataRow["SendNewsletterAfterRegistration"]);
            this.IsSent = (dataRow["IsSent"] != DBNull.Value) ? Convert.ToBoolean(dataRow["IsSent"]) : false;
            this.SendDate = dataRow["SendDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dataRow["SendDate"]) : null;
            this.ExpirationDate = dataRow["ExpirationDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dataRow["ExpirationDate"]) : null;
            this.IsLoaded = true;
        }

    }
}