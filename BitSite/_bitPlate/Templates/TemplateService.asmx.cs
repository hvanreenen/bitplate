using BitPlate.Domain;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace BitSite._bitPlate.Templates
{
    [GenerateScriptType(typeof(CmsTemplate))]
    [GenerateScriptType(typeof(CmsScript))]
    [System.Web.Script.Services.ScriptService]
    public partial class TemplateService : BaseService
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<CmsTemplate> GetTemplates()
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);

            BaseCollection<CmsTemplate> list = BaseCollection<CmsTemplate>.Get(where, "Name");
            return list;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<CmsTemplate> GetNewsletterTemplates()
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_Site='{0}' AND IsNewsletterTemplate=TRUE", SessionObject.CurrentSite.ID);

            BaseCollection<CmsTemplate> list = BaseCollection<CmsTemplate>.Get(where, "Name");
            return list;
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> GetTemplatesLite(string sort, string searchString, int templateType)
        {
            BaseService.CheckLoginAndLicense();
            //IsNewsletterTemplate
            string where = String.Format("FK_Site='{0}' AND IsNewsletterTemplate={1}", SessionObject.CurrentSite.ID, templateType.ToString());
            if (searchString != null && searchString != "")
            {
                where += String.Format(" AND Name like '%{0}%'", searchString);
            }
            List<TreeGridItem> returnList = new List<TreeGridItem>();
            BaseCollection<CmsTemplate> templates = BaseCollection<CmsTemplate>.Get(where, sort);

            foreach (CmsTemplate template in templates)
            {
                TreeGridItem item = TreeGridItem.NewPublishableItem<CmsTemplate>(template);
                item.Icon = template.Screenshot + "?" + DateTime.Now.Ticks;
                item.LanguageCode = template.LanguageCode;
                if (searchString != null && searchString != "")
                {
                    item.Name = item.Name.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                    //item.Title = item.Title.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                }
                returnList.Add(item);
            }
            return returnList;
        }

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public static BaseCollection<CmsTemplate> GetTemplatesSorted(string sort, int page, int pagesize)
        //{
        //    string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);

        //    BaseCollection<CmsTemplate> list = BaseCollection<CmsTemplate>.Get(where, sort, page, pagesize);
        //    return list;
        //}

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public static int GetTemplatesTotalCount(string sort, int page, int pagesize)
        //{
        //    string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);

        //    BaseCollection<CmsTemplate> list = BaseCollection<CmsTemplate>.Get(where);
        //    return list.Count;
        //}


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetTemplateContent(string templateid)
        {
            BaseService.CheckLoginAndLicense();
            CmsTemplate template = BaseObject.GetById<CmsTemplate>(new Guid(templateid));
            if (template == null) return "";
            else
            {
                string html = string.Format(@"<div id='bitEditorTemplate' style='width:100%' class='bitEditor' title='{1}'>
    {0}
</div>
", template.GetBodyContent(), template.Name);
                return html;
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsTemplate GetTemplateIncludeHeader(string id)
        {
            BaseService.CheckLoginAndLicense();
            CmsTemplate template = CmsTemplate.New();
            if (id != null)
            {
                template = BaseObject.GetById<CmsTemplate>(new Guid(id));
                if (template.HasAutorisation)
                {
                    //BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
                    //BitplateUserGroup[] usergroups = client.GetUserGroupsByObjectPermission(template.ID);
                    //template.AutorizedBitplateUserGroups = usergroups;

                    //BitplateUser[] users = client.GetUsersByObjectPermission(template.ID);
                    //template.AutorizedBitplateUsers = users;
                    if (!template.IsAutorized(SessionObject.CurrentBitplateUser))
                    {
                        throw new Exception("U heeft geen rechten voor deze template");
                    }
                }
            }
            else
            {
                template.Site = SessionObject.CurrentSite;
            }
            string tmpHead = "";
            foreach (CmsScript script in template.Scripts)
            {
                tmpHead += script.GetTag() + "\n\r";
            }
            template.Content = template.Content.Replace("[HEAD]", tmpHead).Replace("[SCRIPTS]", "");
            return template;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsTemplate GetTemplate(string id)
        {
            return GetTemplate(id, 0);
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsTemplate GetTemplate(string id, int type)
        {
            BaseService.CheckLoginAndLicense();
            CmsTemplate template = CmsTemplate.New();
            //TODO: NewsletterTemplate service. 0 = PageTemplate & 1 = NewsletterTemplate
            if (type == 1)
            {
                //template.Content = template.Content.Replace("[HEAD]", "");
            }
            if (id != null)
            {
                template = BaseObject.GetById<CmsTemplate>(new Guid(id));
                if (template.HasAutorisation)
                {
                    //BitAutorisationService.AutorisationClient client = BitMetaServerServicesHelper.GetClient();
                    //BitplateUserGroup[] usergroups = client.GetUserGroupsByObjectPermission(template.ID);
                    //template.AutorizedBitplateUserGroups = usergroups;

                    //BitplateUser[] users = client.GetUsersByObjectPermission(template.ID);
                    //template.AutorizedBitplateUsers = users;
                    if (!template.IsAutorized(SessionObject.CurrentBitplateUser))
                    {
                        throw new Exception("U heeft geen rechten voor deze template");
                    }
                }
            }
            else
            {
                template.Site = SessionObject.CurrentSite;
            }
            return template;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsTemplate SaveTemplate(CmsTemplate obj)
        {
            if (obj.Name.Trim() == "")
            {
                throw new Exception("Geen template naam ingevoerd.");
            }
            BaseService.CheckLoginAndLicense();
            obj.Site = SessionObject.CurrentSite;
            obj.Save();

            //lijst van ObjectPermissions wordt hier alleen als drager gebruikt.
            //in de licentieserver wordt deze lijst weer gesplitst en in 2 tabellen gezet
            //BaseCollection<ObjectPermission> objPermissions = obj.GetObjectPermissions4LicenseServer();
            //if (objPermissions.Count > 0)
            //{
            //    BitAutorisationService.AutorisationClient client = BitMetaServerServicesHelper.GetClient();
            //    client.SaveObjectPermissions(objPermissions);
            //}
            //update pages
            //Waarom dit ook alweer???? @HJ
            //foreach (CmsPage page in obj.GetPages())
            //{
            //    page.Save();
            //}
            return obj;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteTemplate(string id)
        {
            BaseService.CheckLoginAndLicense();
            CmsTemplate template = GetTemplate(id);
            if (template.HasAutorisation)
            {
                if (!template.IsAutorized(SessionObject.CurrentBitplateUser))
                {
                    throw new Exception("U heeft geen rechten voor deze template");
                }
            }
            template.Delete();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CopyTemplate(Guid TemplateId, string newTemplateName)
        {
            BaseService.CheckLoginAndLicense();
            CmsTemplate template = GetTemplate(TemplateId.ToString()); // BaseObject.GetById<CmsTemplate>(TemplateId);
            CmsTemplate copy = template.Copy(newTemplateName);

            //BaseCollection<ObjectPermission> objPermissions = copy.GetObjectPermissions4LicenseServer();
            //if (objPermissions.Count > 0)
            //{
            //    BitAutorisationService.AutorisationClient client = BitMetaServerServicesHelper.GetClient();
            //    client.SaveObjectPermissions(objPermissions);
            //}

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<string> GetTags(bool NewsletterMode)
        {
            List<string> tagCollection = new List<string>();
            BaseService.CheckLoginAndLicense();
            string returnValue = "Dubbelklik op een tag om het op de plaats van de cursor te zetten<br/>";

            //returnValue += "<br/>";
            //returnValue += "<strong>Body-tags:</strong> (Deze tags kunnen genummerd worden.<br />Bijvoorbeeld: [CONTENT1], [CONTENT2], [CONTENT...)<br/>";
            tagCollection.Add("[BLOCK]");
            //returnValue += "<div class='tagToInsert'>[CONTENT]</div>";
            tagCollection.Add("[CONTENT]");
            //returnValue += "<div class='tagToInsert'>[TOP]</div>";
            tagCollection.Add("[TOP]");
            //returnValue += "<div class='tagToInsert'>[LEFT]</div>";
            tagCollection.Add("[LEFT]");
            //returnValue += "<div class='tagToInsert'>[CENTER]</div>";
            tagCollection.Add("[CENTER]");
            //returnValue += "<div class='tagToInsert'>[RIGHT]</div>";
            tagCollection.Add("[RIGHT]");
            //returnValue += "<div class='tagToInsert'>[BOTTOM]</div>";
            tagCollection.Add("[BOTTOM]");
            //returnValue += "<br/>";

            if (NewsletterMode)
            {
                //returnValue += "<strong>Nieuwsbrief-tags:</strong><br/>";
                //returnValue += "<div class='tagToInsert'>[NAME]</div>";
                tagCollection.Add("[NAME]");
                //returnValue += "<div class='tagToInsert'>[FORENAME]</div>";
                tagCollection.Add("[FORENAME]");
                //returnValue += "<div class='tagToInsert'>[NAMEPREFIX]</div>";
                tagCollection.Add("[NAMEPREFIX]");
                //returnValue += "<div class='tagToInsert'>[COMPANYNAME]</div>";
                tagCollection.Add("[COMPANYNAME]");
                //returnValue += "<div class='tagToInsert'>[DATE]</div>";
                tagCollection.Add("[DATE]");
                //returnValue += "<div class='tagToInsert'>[SEXE]</div>";
                tagCollection.Add("[SEXE]");
                //returnValue += "<div class='tagToInsert'>[BEGINNING]</div>";
                tagCollection.Add("[BEGINNING]");
                //returnValue += "<div class='tagToInsert'>[USERCODE]</div>";
                tagCollection.Add("[USERCODE]");
                //returnValue += "<div class='tagToInsert'>[LIVEURL]</div>";
                tagCollection.Add("[LIVEURL]");
                //returnValue += "<div class='tagToInsert'>[UNSUBSCRIBEURL]</div>";
                tagCollection.Add("[UNSUBSCRIBEURL]");
            }
            else
            {
                //returnValue += "<strong>Head-tags:</strong><br/>";
                //returnValue += "<div class='tagToInsert'>[PAGETITLE]</div>";
                tagCollection.Add("[PAGETITLE]");
                //returnValue += "<div class='tagToInsert'>[HEAD]</div>";
                tagCollection.Add("[HEAD]");
            }
            return tagCollection;
            //return returnValue;
        }
    }
}
