using BitPlate.Domain;
using BitPlate.Domain.Newsletters;
using BitSite._bitPlate._bitSystem;
using HJORM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace BitSite._bitPlate.Scripts
{
    [System.Web.Script.Services.ScriptService]
    public class ScriptService : BaseService
    {
        public void UploadScripts(HttpContext context)
        {


            if (context.Request.Files.Count > 0)
            {
                if (!Directory.Exists(context.Server.MapPath("") + "\\_temp"))
                {
                    Directory.CreateDirectory(context.Server.MapPath("") + "\\_temp");
                }
                foreach (string sScript in context.Request.Files)
                {
                    HttpPostedFile script = context.Request.Files[sScript];
                    string filename = context.Server.MapPath("") + "\\_temp\\" + script.FileName;
                    script.SaveAs(filename);
                    CmsScript cmsScript = new CmsScript();
                    FileInfo fiScript = new FileInfo(filename);
                    if (fiScript.Extension == ".css" || fiScript.Extension == ".js")
                    {
                        cmsScript.Name = fiScript.Name.Replace(fiScript.Extension, "");
                        StreamReader sr = new StreamReader(filename);
                        cmsScript.Content = sr.ReadToEnd();
                        cmsScript.ScriptType = (fiScript.Extension == ".css") ? ScriptTypeEnum.Css : ScriptTypeEnum.Js;
                        cmsScript.Site = SessionObject.CurrentSite;
                        sr.Close();
                        sr.Dispose();
                        cmsScript.Save();
                    }
                    fiScript.Delete();
                }
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<CmsScript> GetScripts(string sort, string searchString, int? scriptMode)
        {
            BaseService.CheckLoginAndLicense();
            string where;

            if (scriptMode == null)
            {
                where = String.Format("FK_Site='{0}'", new object[] { SessionObject.CurrentSite.ID });
            }
            else
            {
                where = String.Format("FK_Site='{0}' AND ScriptType={1}", new object[] { SessionObject.CurrentSite.ID, scriptMode });
            }

            if (searchString != null && searchString != "")
            {
                where += String.Format(" AND Name like '%{0}%'", searchString);
            }

            if (sort == "")
            {
                sort = "Name ASC";
            }
            BaseCollection<CmsScript> list = BaseCollection<CmsScript>.Get(where, sort);
            return list;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveScriptFile(string scriptName, string content)
        {
            BaseService.CheckLoginAndLicense();
            string path = SessionObject.CurrentSite.Path;

            scriptName = scriptName.Replace("/", "\\");

            File.WriteAllText(path + "\\" + scriptName, content);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsScript GetScript(string id)
        {
            BaseService.CheckLoginAndLicense();
            CmsScript script = new CmsScript();
            if (id != null)
            {
                script = BaseObject.GetById<CmsScript>(new Guid(id));
                if (script.HasAutorisation)
                {
                    if (!script.IsAutorized(SessionObject.CurrentBitplateUser))
                    {
                        throw new Exception("U heeft geen rechten voor dit script");
                    }
                }
            }
            else
            {
                script.Site = SessionObject.CurrentSite;
            }
            return script;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsScript SaveScript(CmsScript obj)
        {
            BaseService.CheckLoginAndLicense();
            obj.Site = SessionObject.CurrentSite;
            obj.Save();

            ////lijst van ObjectPermissions wordt hier alleen als drager gebruikt.
            ////in de licentieserver wordt deze lijst weer gesplitst en in 2 tabellen gezet
            //BaseCollection<ObjectPermission> objPermissions = obj.GetObjectPermissions4LicenseServer();
            //if (objPermissions.Count > 0)
            //{
            //    //BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
            //    //client.SaveObjectPermissions(objPermissions);
            //}
            BitCaching.RemoveItemFromCache(obj.ID.ToString());
            return obj;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteScript(string id)
        {
            BaseService.CheckLoginAndLicense();
            CmsScript script = GetScript(id);
            script = BaseObject.GetById<CmsScript>(new Guid(id));
            if (script.HasAutorisation)
            {
                if (!script.IsAutorized(SessionObject.CurrentBitplateUser))
                {
                    throw new Exception("U heeft geen rechten voor dit script");
                }
            }
            BitCaching.RemoveItemFromCache(script.ID.ToString());
            script.Delete();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CopyScript(Guid scriptId, string newScriptName)
        {
            BaseService.CheckLoginAndLicense();
            CmsScript script = GetScript(scriptId.ToString());
            CmsScript copy = script.Copy(newScriptName);

            //BaseCollection<ObjectPermission> objPermissions = copy.GetObjectPermissions4LicenseServer();
            //if (objPermissions.Count > 0)
            //{
            //    BitAutorisationService.AutorisationClient client = BitMetaServerServicesHelper.GetClient();
            //    client.SaveObjectPermissions(objPermissions);
            //}
        }
        /// <summary>
        /// Functie is om popup met scripts te vullen, nodig voor koppelen van script aan site, template of page
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<CmsScript> LoadScripts()
        {
            BaseService.CheckLoginAndLicense();
            string target = "page";
            string id = "";
            string targetscript = "BITTEMPLATES";
            List<CmsScript> Scripts = new List<CmsScript>();
            if (HttpContext.Current.Request.QueryString["target"] != null)
            {
                target = HttpContext.Current.Request.QueryString["target"];
            }
            if (target == "page")
            {
                targetscript = "BITPAGES";
            }
            if (HttpContext.Current.Request.QueryString["id"] != null)
            {
                id = HttpContext.Current.Request.QueryString["id"];
            }
            BaseCollection<CmsScript> scripts = GetScripts("ScriptType, Name", "", null);

            string html = "";
            foreach (CmsScript script in scripts)
            {
                html += String.Format(@"<a href=""javascript:{0}.addScript('{1}', '{2}');"">{2}</a><br/>", targetscript, script.ID, script.CompleteName);
            }
            Scripts.AddRange(scripts);
            return Scripts;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<CmsScript> GetScriptsByPageId(Guid pageid)
        {
            BaseService.CheckLoginAndLicense();
            List<CmsScript> scripts = new List<CmsScript>();

            foreach (CmsScript siteScript in SessionObject.CurrentSite.Scripts)
            {
                CmsScript script = new CmsScript();
                script.ID = siteScript.ID;
                script.Name = siteScript.Url.Replace("_css/", "").Replace("_js/", "") + " (site)";
                //script.Url = siteScript.Url;
                script.ScriptType = siteScript.ScriptType;
                scripts.Add(script);
            }

            CmsPage page = BaseObject.GetById<CmsPage>(pageid);

            //dan alle van template eraan toevoegen
            BaseCollection<CmsScript> templateScripts = page.Template.Scripts;
            foreach (CmsScript templateScript in templateScripts)
            {
                CmsScript script = new CmsScript();
                script.ID = templateScript.ID;
                script.Name = templateScript.Url.Replace("_css/", "").Replace("_js/", "") + " (template)";
                script.ScriptType = templateScript.ScriptType;
                scripts.Add(script);
            }
            //dan alle van pagina eraan toevoegen
            BaseCollection<CmsScript> pageScripts = page.Scripts;
            foreach (CmsScript pageScript in pageScripts)
            {
                CmsScript script = new CmsScript();
                script.ID = pageScript.ID;
                script.Name = pageScript.Url.Replace("_css/", "").Replace("_js/", "");
                script.Url = pageScript.Url;
                script.ScriptType = pageScript.ScriptType;
                scripts.Add(script);
            }
            return scripts;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<CmsScript> GetScriptsByNewsletterId(Guid newsletterid)
        {
            BaseService.CheckLoginAndLicense();
            List<CmsScript> scripts = new List<CmsScript>();

            foreach (CmsScript siteScript in SessionObject.CurrentSite.Scripts)
            {
                CmsScript script = new CmsScript();
                script.ID = siteScript.ID;
                script.Name = siteScript.Url.Replace("_css/", "").Replace("_js/", "") + " (site)";
                //script.Url = siteScript.Url;
                script.ScriptType = siteScript.ScriptType;
                scripts.Add(script);
            }

            Newsletter page = BaseObject.GetById<Newsletter>(newsletterid);

            //dan alle van template eraan toevoegen
            BaseCollection<CmsScript> templateScripts = page.Template.Scripts;
            foreach (CmsScript templateScript in templateScripts)
            {
                CmsScript script = new CmsScript();
                script.ID = templateScript.ID;
                script.Name = templateScript.Url.Replace("_css/", "").Replace("_js/", "") + " (template)";
                script.ScriptType = templateScript.ScriptType;
                scripts.Add(script);
            }
            //dan alle van pagina eraan toevoegen
            BaseCollection<CmsScript> pageScripts = page.Scripts;
            foreach (CmsScript pageScript in pageScripts)
            {
                CmsScript script = new CmsScript();
                script.ID = pageScript.ID;
                script.Name = pageScript.Url.Replace("_css/", "").Replace("_js/", "");
                script.Url = pageScript.Url;
                script.ScriptType = pageScript.ScriptType;
                scripts.Add(script);
            }
            return scripts;
        }
    }
}
