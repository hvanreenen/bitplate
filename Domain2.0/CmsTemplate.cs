using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Utils;
using System.Text.RegularExpressions;
using BitPlate.Domain.Modules;
using System.IO;
using System.Web.Script.Serialization;

namespace BitPlate.Domain
{
    [Persistent("Template")]
    
    public class CmsTemplate : BaseDomainPublishableObject
    {
        [NonPersistent]
        private static string DefaultEmptyTemplate = @"<!DOCTYPE html>
        <html>
            <head>
                <title>[PAGETITLE]</title>
                <meta charset=""utf-8"" />
                <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/>
                [HEAD]
            </head>
            <body>
                [CONTENT]
            </body>
        </html>";

//        [NonPersistent]
//        private static string DefaultEmptyTemplate = @"<!DOCTYPE html>
//        <html>
//            <head>
//                <title>[PAGETITLE]</title>
//                [HEAD]
//            </head>
//            <body>
//                [CONTENT]
//            </body>
//        </html>";

        public CmsTemplate()
        {

        }

        public static CmsTemplate New()
        {
            CmsTemplate template = new CmsTemplate();
            template.Content = DefaultEmptyTemplate;
            template.Site = (CmsSite)System.Web.HttpContext.Current.Session["CurrentSite"];
            template.LanguageCode = template.Site.DefaultLanguage;
            return template;
        }
        private BaseCollection<CmsTemplateContainer> _containers;
        [System.Web.Script.Serialization.ScriptIgnore()]
        [NonPersistent()]
        public virtual BaseCollection<CmsTemplateContainer> Containers
        {
            get
            {
                if (_containers == null)
                {
                    //voorlopig geen versie van Container in Database opslaan.
                    //er hangt immer niks verder aan dan alleen de naam
                    //_containers = BaseCollection<CmsTemplateContainer>.Get("FK_Layout='" + this.ID + "'");

                    _containers = CreateContainers();
                }
                return _containers;
            }
            //set
            //{
            //    _containers = value;
            //}
        }

        private string _content;
        public string Content
        {
            get
            {
                if (_content == null || _content == "")
                {
                    _content = DefaultEmptyTemplate;
                }
                return _content;
            }
            set
            {
                _content = value;
            }
        }
        //public string BodyTagContent { get; set; }
        //public string HeadContent { get; set; }
        private string _screenshot;
        [NonPersistent()]
        public string Screenshot
        {
            get
            {
                if (!IsNew)
                {
                    return "/_bitplate/_img/screenshots/template_" + ID.ToString() + ".jpg";
                }
                else
                {
                    return "";
                }
            }
            set
            {
                _screenshot = value;
            }
        }

        BaseCollection<CmsScript> _scripts;
        [Persistent("ScriptPerTemplate")]
        [Association("FK_Template", "FK_Script")]
        public BaseCollection<CmsScript> Scripts
        {
            get
            {
                if (_scripts == null || (_scripts != null && !_scripts.IsLoaded))
                {
                    string where = "EXISTS (SELECT * FROM ScriptPerTemplate WHERE FK_Template = '" + this.ID + "' AND Script.ID = ScriptPerTemplate.FK_Script)";
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
        private string _languageCode = "";
        public string LanguageCode
        {
            get
            {
                if (_languageCode == "")
                {
                    _languageCode = Site.DefaultLanguage;
                }
                return _languageCode;
            }
            set {
                _languageCode = value; 
            }
        }

        public string GetBodyContent()
        {
            string html = Content;
            html = Utils.HtmlHelper.ReplaceLinks(html, ModeEnum.Edit, this.Site);
            int posBodyStartTagStart = html.IndexOf("<body");
            int posBodyStartTagEnd = html.IndexOf(">", posBodyStartTagStart + 1) + 1;
            int posBodyEndTag = html.IndexOf("</body>");
            //string bodyTag = html.Substring(posBodyStartTagStart, posBodyStartTagEnd - posBodyStartTagStart);
            string bodyHtml = html.Substring(posBodyStartTagEnd, posBodyEndTag - posBodyStartTagEnd);
            bodyHtml = HtmlHelper.ReplaceImageSources(bodyHtml, ModeEnum.Edit, this.Site);
            return bodyHtml;
        }

        public string GetHeadContent()
        {
            string html = Content;
            html = Utils.HtmlHelper.ReplaceLinks(html, ModeEnum.Edit, this.Site);
            int posHeadStartTagStart = html.IndexOf("<head");
            int posHeadStartTagEnd = html.IndexOf(">", posHeadStartTagStart + 1) + 1;
            int posHeadEndTag = html.IndexOf("</head>");
            //string bodyTag = html.Substring(posBodyStartTagStart, posBodyStartTagEnd - posBodyStartTagStart);
            string headHtml = html.Substring(posHeadStartTagEnd, posHeadEndTag - posHeadStartTagEnd);
            headHtml = HtmlHelper.ReplaceImageSources(headHtml, ModeEnum.Edit, this.Site);
            return headHtml;
        }

        private string GetAllScriptsInHtml()
        {
            string html = "";
            foreach (CmsScript script in this.Scripts)
            {
                html += script.GetTag();
            }
            return html;
        }

        public override void Save()
        {
            // bool isNew = this.IsNew;
            if (IsNew && WebSessionHelper.CurrentLicense != null && WebSessionHelper.CurrentLicense.HasMaxNumberExceeded("Template"))
            {
                throw new Exception("Kan niet opslaan: licentie heeft maximale aantal templates (" + WebSessionHelper.CurrentLicense.MaxNumberOfTemplates + ") overschreden.");
            }
            string parseErrors = HtmlHelper.ValidateHtml(this.Content);
            if (parseErrors != "")
            {
                //throw new Exception("Html is niet geldig: " + parseErrors);
            }
            base.Save();

            string stylesToGenerateScreenshot = "";
            BaseCollection<CmsScript> allScripts = new BaseCollection<CmsScript>();
            allScripts.AddRange(this.Scripts);
            allScripts.AddRange(this.SiteScripts);
            foreach (CmsScript script in allScripts)
            {
                string scriptContent = script.Content;
                if (script.ScriptType == ScriptTypeEnum.Css)
                {
                    stylesToGenerateScreenshot += "<style>" + scriptContent + "</style>\r\n";
                }
                else
                {
                    stylesToGenerateScreenshot += "<script type=\"text/javascript\">" + scriptContent + "</script>\r\n";
                }
            }
            string htmlToGenerateScreenshot = String.Format(@"<!DOCTYPE html>
<html>
    <head>
        {0}
    </head>
    <body>
        {1}
    </body>
</html>", stylesToGenerateScreenshot, GetBodyContent());

            GenerateScreenshotHelper.GenerateScreenshotFromHTML(this.ID, htmlToGenerateScreenshot);

            //UnpublishedItem.Set(this, "Template");
            //BaseCollection<CmsPage> pages = BaseCollection<CmsPage>.Get("FK_Layout = '" + this.ID + "'");
            //foreach (CmsPage page in pages)
            //{
            //    UnpublishedItem.Set(page, "Page");
            //}
            //this.Publish();
            //bugfix
            foreach(CmsScript script in Scripts)
            {
                //mocht een script al zijn gekoppeld aan een page dan hier koppeling verwijderen, zodat je geen dubbele koppelingen krijgt
                string sql = String.Format("DELETE FROM ScriptPerPage WHERE FK_Script =  '{0}' AND EXISTS (SELECT * FROM Page WHERE Page.ID = FK_Page AND Page.FK_Layout='{1}')", script.ID, this.ID);
                DataBase.Get().Execute(sql);
            }
        }

        private BaseCollection<CmsTemplateContainer> CreateContainers()
        {
            BaseCollection<CmsTemplateContainer> containers = new BaseCollection<CmsTemplateContainer>();
            string content = GetBodyContent();

            //Verborgen html verwijderen uit CreateContainer functie. Voorkomt verborgen (uitgecommentarieerde) containers. BUG FIX
            MatchCollection matches = Regex.Matches(content, "<!--(.*?)-->", RegexOptions.Singleline);
            foreach (Match match in matches)
            {
                content = content.Replace(match.ToString(), "");
            }
            int posTagStart = content.IndexOf("[");
            int posTagEnd = content.IndexOf("]", posTagStart + 1) + 1;
            while (posTagStart >= 0 && posTagEnd >= 0)
            {
                string tag = content.Substring(posTagStart, posTagEnd - posTagStart);
                if (tag.Contains("CONTENT") || tag.Contains("TOP") || tag.Contains("LEFT") || tag.Contains("RIGHT") || tag.Contains("BOTTOM") || tag.Contains("BLOCK"))
                {
                    string containerName = tag.Replace("[", "").Replace("]", "");
                    CmsTemplateContainer container = new CmsTemplateContainer();
                    container.Site = this.Site;
                    container.Template = this;
                    container.Name = containerName;
                    containers.Add(container);
                }
                posTagStart = content.IndexOf("[", posTagStart + 1);
                posTagEnd = content.IndexOf("]", posTagStart + 1) + 1;
            }
            return containers;
        }

        public override void Delete()
        {
            //template modules verwijderen
            BaseCollection<BaseModule> Modules = this.GetTemplateModules();
            foreach (BaseModule module in Modules)
            {
                module.Delete();
            }
            base.Delete();
            UnpublishedItem.Set(this, "Template", ChangeStatusEnum.Deleted);

        }

        //public virtual void Publish()
        //{
        //    //maak er een master page van.
        //    string html = getPublishHtml();

        //    //save
        //    string fileName = this.Site.Path + "\\" + this.Name + ".Master";
        //    if (this.IsNewsletterTemplate)
        //    {
        //        if (!Directory.Exists(this.Site.Path + "\\Newsletters\\")) //BUG FIX
        //        {
        //            Directory.CreateDirectory(this.Site.Path + "\\Newsletters\\");
        //        }
        //        fileName = this.Site.Path + "\\Newsletters\\" + this.Name + ".Master";
        //    }
        //    FileHelper.WriteFile(fileName, html);

        //    if (LastPublishedFileName != null && LastPublishedFileName != fileName)
        //    {
        //        //naam is gewijzigd
        //        //dan oude master weggooien
        //        FileHelper.DeleteFile(LastPublishedFileName);
        //        //een alle pagina's opnieuw publiceren, zodat er verwijzing is naar juiste master.
        //        BaseCollection<CmsPage> pages = BaseCollection<CmsPage>.Get("FK_Layout = '" + this.ID + "'");
        //        foreach (CmsPage page in pages)
        //        {
        //            page.Publish();
        //        }
        //    }
        //    base.SaveLastPublishInfo("Template", fileName);

        //    //base.SavePublishInfo();
        //}

        //private collection
        private BaseCollection<BaseModule> _Modules;

        public BaseCollection<BaseModule> GetTemplateModules()
        {
            if (this._Modules == null)
            {
                string where = "FK_Template='" + this.ID.ToString() + "' AND CrossPagesMode=1"; //Laad templatemodules met uitzondering van modules die ook op alle pagina's zichtbaar moeten zijn.
                this._Modules = BaseCollection<BaseModule>.Get(where);
            }
            return this._Modules;
        }

//        protected string getPublishHtml()
//        {
//            string html = @"<%@ Master Language=""C#""  CodeBehind=""~/DefaultMaster.Master.cs"" Inherits=""BitSite.DefaultMaster""  ";
//            if (Site.IsMultiLingual)
//            {
//                html += String.Format(@" LanguageCode=""{0}""", this.LanguageCode);
//            }
//            html += @"%>
//";
//            html += this.Content;

//            //HEAD
//            html = html.Replace("<head", @"<head runat=""server""");
//            html = html.Replace("[PAGETITLE]", "<%= Page.Title %>");

//            //jquery bestand wordt in literal gezet. ook de site headcontent
//            string headLiteral = @"<asp:Literal runat=""server"" ID=""LiteralPageHeader""></asp:Literal>";
//            string contentPlaceHolderHead = @"<asp:ContentPlaceHolder ID=""Head"" runat=""server""></asp:ContentPlaceHolder>";
//            html = html.Replace("[HEAD]", headLiteral + GetAllScriptsInHtml() + contentPlaceHolderHead);

//            //BODY START
//            string bodyTag = Regex.Match(html, "<body(.*?)>", RegexOptions.Singleline).ToString();
//            //enctype=multipart/form-data staat aan voor formulieren waarmee je bestanden naar server moet kunnen verzenden -->
//            html = html.Replace(bodyTag, bodyTag + @"<form ID=""Form1"" runat=""server"" enctype=""multipart/form-data"">
//                   <asp:ScriptManager ID=""BitScriptManager"" runat=""server""></asp:ScriptManager>
//                    <script type=""text/javascript"">
//	                        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);
//	                        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);
//                            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(pageLoadedHandler);
//
//                            //Raised before the processing of an asynchronous postback starts and the postback request is sent to the server.
//	                        function beginRequestHandler(sender, args) {
//                                $(document).trigger('beginRequestHandler', [sender, args]);
//                            }
//
//                            //Raised after an asynchronous postback is finished and control has been returned to the browser.
//                            function endRequestHandler(sender, args) {
//                                $(document).trigger('endRequestHandler', [sender, args]);
//                            }
//
//                            //Raised after all content on the page is refreshed as a result of either a synchronous or an asynchronous postback.
//                            function pageLoadedHandler(sender, args) {
//                                $(document).trigger('pageLoadedHandler', [sender, args]);
//                            }
//                    </script>
//                   <asp:PlaceHolder ID=""PlaceHolderEditPageMenu"" runat=""server"" ></asp:PlaceHolder>
//            ");

//            //INHOUD BODY
//            foreach (CmsTemplateContainer container in this.Containers)
//            {
//                string contentPlaceHolder = String.Format(@"<asp:UpdatePanel ID=""BitUpdatePanel{0}"" runat=""server"">
//                        <ContentTemplate>
//                            <div class=""bitContainer"" id=""{0}"">
//                            <asp:ContentPlaceHolder ID=""{0}"" runat=""server""></asp:ContentPlaceHolder>
//                           </div>
//                        </ContentTemplate>
//                </asp:UpdatePanel>", container.Name);
//                html = html.Replace("[" + container.Name.ToUpper() + "]", contentPlaceHolder);
//            }

//            html = html.Replace("</body>", "</form>\r\n</body>");
//            return html;
//        }



        internal List<string> GetTags(string tag)
        {
            List<string> returnValue = new List<string>();

            //foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(this.Content, tag))
            for (int i = 0; i < StringHelper.CountOccurences(this.Content, tag); i++)
            {
                returnValue.Add(tag);
            }
            return returnValue;
        }

        public CmsTemplate Copy(string newTemplateName)
        {
            CmsTemplate newTemplate = this.CreateCopy<CmsTemplate>(true);
            newTemplate.Name = newTemplateName;
            newTemplate.LastPublishedDate = null;
            
            //bij scripts, usergroups & users is het niet het script zelf dat onder een template hangt, maar alleen de koppeling (veel op veel).
            foreach (CmsScript script in this.Scripts)
            {
                newTemplate.Scripts.Add(script);
            }
            if (this.HasAutorisation)
            {
                foreach (Autorisation.SiteUserGroup userGroup in this.AutorizedSiteUserGroups)
                {
                    newTemplate.AutorizedSiteUserGroups.Add(userGroup);
                }
                foreach (Autorisation.SiteUser user in this.AutorizedSiteUsers)
                {
                    newTemplate.AutorizedSiteUsers.Add(user);
                }
            }
            newTemplate.Save();
            return newTemplate;
        }

        public bool IsNewsletterTemplate { get; set; }
        /// <summary>
        /// haal alle pagina's op die gebruik maken van deze template
        /// </summary>
        /// <returns></returns>
        public BaseCollection<CmsPage> GetPages()
        {
            BaseCollection<CmsPage> pages = BaseCollection<CmsPage>.Get("FK_Layout='" + ID.ToString() + "'");
            return pages;
        }

        [NonPersistent]
        public List<CmsPage> UsedInPages
        {
            get
            {
                return this.GetPages().Select(c => new CmsPage() { Name = c.Name, ID = c.ID, RelativeUrl = c.RelativeUrl }).ToList();
            }
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            this.Content = dataRow["Content"].ToString();
            this.LanguageCode = dataRow["LanguageCode"].ToString();
            this.IsNewsletterTemplate = DataConverter.ToBoolean(dataRow["IsNewsletterTemplate"]);
            //this.Screenshot = dataRow["Screenshot"].ToString();
        }

    }
}
