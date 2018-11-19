using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.IO;

using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Modules;
using BitSite._bitPlate.Scripts;
using BitPlate.Domain.Utils;
using BitSite._bitPlate._bitSystem;

namespace BitSite._bitPlate.Pages
{
    [GenerateScriptType(typeof(CmsPage))]
    [GenerateScriptType(typeof(CmsPageFolder))]
    [GenerateScriptType(typeof(CmsTemplate))]
    [GenerateScriptType(typeof(CmsScript))]
    [System.Web.Script.Services.ScriptService]
    public partial class PageService : BaseService
    {
        static string basePath = "";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<ListItem> GetPages()
        {
            BaseService.CheckLoginAndLicense();
            string siteid = SessionObject.CurrentSite.ID.ToString();
            List<ListItem> returnList = new List<ListItem>();
            BaseCollection<CmsPage> list = BaseCollection<CmsPage>.Get("FK_Site = '" + siteid + "'", "Path, Name");
            foreach (CmsPage page in list)
            {
                ListItem item = new ListItem();

                item.Text = page.RelativeUrl;
                item.Value = page.ID.ToString();
                returnList.Add(item);

            }
            return returnList;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> GetFoldersAndPages(string folderId, string folderPath, string sort, string searchString)
        {
            BaseService.CheckLoginAndLicense();
            if ((folderId == null || folderId == "") &&
                (folderPath != null && folderPath != "" && (!folderPath.Contains(" DESC") && !folderPath.Contains(" ASC"))))
            {
                //haal folder id op vanuit path
                //path wordt gebruikt als er vanuit de breadcrumb wordt genavigeerd
                string where = String.Format("RelativePath = '{0}'", folderPath);
                CmsPageFolder folder = BaseObject.GetFirst<CmsPageFolder>(where);
                folderId = folder.ID.ToString();
            }
            if (sort == "" || sort == null)
            {
                sort = "Name";
            }
            string sortPages = sort;
            string sortFolders = sort;
            //folders kennen geen title veld en geen path veld
            sortFolders = sortFolders.Replace("Title", "Name");
            sortFolders = sortFolders.Replace("Path", "RelativePath");

            string wherePages = String.Format("FK_Site = '{0}' AND FK_Folder='{1}'", SessionObject.CurrentSite.ID, folderId);
            string whereFolders = String.Format("FK_Site = '{0}' AND FK_Parent_Folder='{1}'", SessionObject.CurrentSite.ID, folderId);
            if (folderId == null || folderId == "" || folderId == Guid.Empty.ToString())
            {
                wherePages = String.Format("FK_Site = '{0}' AND FK_Folder Is Null", SessionObject.CurrentSite.ID);
                whereFolders = String.Format("FK_Site = '{0}' AND FK_Parent_Folder Is Null", SessionObject.CurrentSite.ID);
            }
            if (searchString != null && searchString != "")
            {
                wherePages = String.Format("FK_Site = '{0}' AND (Name like '%{1}%' OR Title like '%{1}%')", SessionObject.CurrentSite.ID, searchString);
                whereFolders = String.Format("FK_Site = '{0}' AND (Name like '%{1}%')", SessionObject.CurrentSite.ID, searchString);

            }
            BaseCollection<CmsPageFolder> folderlist = BaseCollection<CmsPageFolder>.Get(whereFolders, sortFolders);
            BaseCollection<CmsPage> pagelist = BaseCollection<CmsPage>.Get(wherePages, sortPages);
            List<TreeGridItem> returnList = new List<TreeGridItem>();
            if (!(folderId == null || folderId == "" || folderId == Guid.Empty.ToString()))
            {
                //voeg de move up folder toe
                CmsPageFolder folder = BaseObject.GetById<CmsPageFolder>(new Guid(folderId));
                TreeGridItem item = new TreeGridItem();
                if (folder.ParentFolder != null)
                {
                    item = new TreeGridItem(folder.ParentFolder);
                }
                else
                {
                    item.ID = Guid.Empty;
                    item.IsLeaf = false;
                    item.Type = "Folder";
                    item.Path = "";
                }
                item.Name = "...";

                returnList.Add(item);
            }
            foreach (CmsPageFolder folder in folderlist)
            {
                TreeGridItem item = new TreeGridItem(folder);
                //if (searchString != null && searchString != "")
                //{
                //    item.Name = item.Name.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                //}
                returnList.Add(item);
            }
            foreach (CmsPage page in pagelist)
            {
                TreeGridItem item = new TreeGridItem(page);
                //if (searchString != null && searchString != "")
                //{
                //    item.Name = item.Name.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                //    item.Title = item.Title.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                //}
                returnList.Add(item);
            }
            return returnList;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<CmsPageFolder> GetFolders()
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);

            BaseCollection<CmsPageFolder> list = BaseCollection<CmsPageFolder>.Get(where, "RelativePath");
            return list;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsPage GetPage(string id, string folderid)
        {
            BaseService.CheckLoginAndLicense();
            CmsPage page = null;
            if (id == null)
            {
                page = new CmsPage();
                page.Site = SessionObject.CurrentSite;
                page.ChangeStatus = ChangeStatusEnum.New;
                page.InSiteMap = true;
                page.SiteMapChangeFreq = "Monthly";
                page.SiteMapPriority = 0.5;

                if (folderid != null && folderid != "")
                {
                    page.Folder = BaseObject.GetById<CmsPageFolder>(new Guid(folderid));
                }
            }
            else
            {
                page = BaseObject.GetById<CmsPage>(new Guid(id));
                if (page.HasBitplateAutorisation())
                {
                    
                    if (!page.IsAutorized(SessionObject.CurrentBitplateUser))
                    {
                        throw new Exception("U heeft geen rechten voor deze pagina");
                    }
                }
            }
            //page.Body = GetEditPageBody(page);
            //page.Head = GetEditPageHeader(page);
            return page;
        }

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public CmsPage GetPreviewPage(string id, string folderid)
        //{
        //    //BaseService.CheckLoginAndLicense();
        //    CmsPage page = null;


        //    page = BaseDomainObject.GetById<CmsPage>(new Guid(id));
        //    //if (page.HasAutorisation)
        //    //{
        //    //    BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
        //    //    BitplateUserGroup[] usergroups = client.GetUserGroupsByObjectPermission(page.ID);
        //    //    page.AutorizedBitplateUserGroups = usergroups;

        //    //    BitplateUser[] users = client.GetUsersByObjectPermission(page.ID);
        //    //    page.AutorizedBitplateUsers = users;
        //    //    if (!page.IsAutorized(SessionObject.CurrentBitplateUser))
        //    //    {
        //    //        throw new Exception("U heeft geen rechten voor deze pagina");
        //    //    }
        //    //}

        //    page.Head = GetPreviewPageHeader(page);
        //    page.Body = GetPreviewPageBody(page);
            
        //    return page;
        //}

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsPage SavePage(CmsPage obj)
        {
            BaseService.CheckLoginAndLicense();
            obj.Site = SessionObject.CurrentSite;
            //hack om parentfoder leeg te maken vanuit dropdown met folders
            if (obj.Folder != null && obj.Folder.ID == Guid.Empty)
            {
                obj.Folder = null;
            }
            obj.Save();

            ////lijst van ObjectPermissions wordt hier alleen als drager gebruikt.
            ////in de licentieserver wordt deze lijst weer gesplitst en in 2 tabellen gezet
            //BaseCollection<ObjectPermission> objPermissions = obj.GetObjectPermissions4LicenseServer();
            //if (objPermissions.Count > 0)
            //{
            //    BitAutorisationService.AutorisationClient client = BitMetaServerServicesHelper.GetClient();
            //    client.SaveObjectPermissions(objPermissions);
            //}
           
            //BitCaching.RemoveItemFromCache(obj.ID.ToString());
            return obj;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeletePage(string id)
        {
            BaseService.CheckLoginAndLicense();
            CmsPage page = GetPage(id, null);
            page.Delete();
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsPageFolder GetFolder(string id, string folderid)
        {
            BaseService.CheckLoginAndLicense();
            CmsPageFolder folder = null;
            if (id == null)
            {
                folder = new CmsPageFolder();
                folder.Site = SessionObject.CurrentSite;
                if (folderid != null && folderid != "")
                {
                    folder.ParentFolder = BaseObject.GetById<CmsPageFolder>(new Guid(folderid));
                }
            }
            else
            {
                folder = BaseObject.GetById<CmsPageFolder>(new Guid(id));
                if (folder.HasBitplateAutorisation())
                {
                    //BitAutorisationService.AutorisationClient client = BitMetaServerServicesHelper.GetClient();
                    //BitplateUserGroup[] usergroups = client.GetUserGroupsByObjectPermission(folder.ID);
                    //folder.AutorizedBitplateUserGroups = usergroups;

                    //BitplateUser[] users = client.GetUsersByObjectPermission(folder.ID);
                    //folder.AutorizedBitplateUsers = users;
                    if (!folder.IsAutorized(SessionObject.CurrentBitplateUser))
                    {
                        throw new Exception("U heeft geen rechten voor deze map");
                    }
                }
            }
            return folder;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsPageFolder SaveFolder(CmsPageFolder obj)
        {

            BaseService.CheckLoginAndLicense();
            obj.Site = SessionObject.CurrentSite;
            //hack om parentfoder leeg te maken vanuit dropdown met folders
            if (obj.ParentFolder != null && obj.ParentFolder.ID == Guid.Empty)
            {
                obj.ParentFolder = null;
            }
            obj.Save();

            ////lijst van ObjectPermissions wordt hier alleen als drager gebruikt.
            ////in de licentieserver wordt deze lijst weer gesplitst en in 2 tabellen gezet
            //BaseCollection<ObjectPermission> objPermissions = obj.GetObjectPermissions4LicenseServer();
            //if (objPermissions.Count > 0)
            //{
            //    BitAutorisationService.AutorisationClient client = BitMetaServerServicesHelper.GetClient();
            //    //BaseCollection<ObjectPermission> permissions = objPermissions;
            //    string test = objPermissions.ToJsonString();
            //    client.SaveObjectPermissions(objPermissions);
            //}
            return obj;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult DeleteFolder(string id, bool deleteAllPages)
        {
            BaseService.CheckLoginAndLicense();
            CmsPageFolder folder = GetFolder(id, null);
            CmsPageFolder ParentFolder;
            if (folder.GetPages().Count > 0 || folder.GetSubFolders().Count > 0)
            {
                if (!deleteAllPages)
                {
                    return JsonResult.CreateResult(false, "Wilt u deze map inclusief alle subitems van deze map verwijderen?<br />");
                }
                else
                {
                    ParentFolder = folder.ParentFolder;
                    DeleteFolderRecursief(folder);
                }
            }
            else
            {
                ParentFolder = folder.ParentFolder;
                folder.Delete();
            }
            string ParentFolderId = (ParentFolder == null) ? null : ParentFolder.ID.ToString();
            return JsonResult.CreateResult(true, GetFoldersAndPages(ParentFolderId, "", "", ""));
        }

        private static void DeleteFolderRecursief(CmsPageFolder parentFolder)
        {
            foreach (CmsPageFolder folder in parentFolder.GetSubFolders())
            {
                DeleteFolderRecursief(folder);
            }

            foreach (CmsPage page in parentFolder.GetPages())
            {
                page.Delete();
            }
            parentFolder.Delete();
        }


        //private static string GetEditPageHeader(CmsPage page)
        //{
        //    string html = "";

        //    string siteStyleUrl = "../Stylesheet.css";
        //    string layoutStyleUrl = "";
        //    if (page.Template != null)
        //    {
        //        layoutStyleUrl = "../" + page.Template.Name + ".css";
        //    }

        //    html += "<link href='" + siteStyleUrl + "' id='siteStyleSheet' rel='stylesheet' type='text/css'/>";
        //    html += "<link href='" + layoutStyleUrl + "' id='siteStyleSheet' rel='stylesheet' type='text/css'/>";
        //    html += page.Site.HeadContent + "\r\n";
        //    //html += page.Site.HeadContent + "\r\n" +
        //    //    page.Layout.HeadContent + "\r\n    "
        //    //    + page.HeadContent;
        //    //            html +=  @"   
        //    //    <script type=""text/javascript"">
        //    //        BITEDITPAGE.siteid = '" + siteid + @"';
        //    //        BITEDITPAGE.pageid = '" + pageid + @"';
        //    //    </script>";

        //    //html += page.GetHeaderModuleDependentScripts(true);
        //    return html;
        //}

//        private static string GetPreviewPageHeader(CmsPage page)
//        {
//            string html = page.Template.GetHeadContent();
//            string head = page.Site.HeadContent;
//            head += "\r\n";
//            head += page.GetHeaderMetaTags();
//            head += page.HeadContent;
//            html = html.Replace("[HEAD]", head);
//            html = html.Replace("[PAGETITLE]", "Preview: " + page.Title);
//            html = html.Replace("[SCRIPTS]", "");
//            foreach (CmsScript script in page.GetAllScripts())
//            {
                
//                if (script.ScriptType == ScriptTypeEnum.Css)
//                {
//                    html += String.Format(@"<link rel=""stylesheet"" type=""text/css"" href=""{0}/{1}""/>
//", SessionObject.CurrentSite.DomainName, script.Url);
//                }
//                else
//                {
//                    html += String.Format(@"<script type=""text/javascript"" src=""{0}/{1}"" id=""{2}""></script>
//", SessionObject.CurrentSite.DomainName, script.Url, script.ID);
//                }
//            }

//            html += @"<script type=""text/javascript"" src=""/_bitPlate/_bitModules/BITALLMODULES.js""></script>
//";
//            //module afhgankelijke scripts toevoegen
//            foreach (string script in page.GetModuleDependentScripts())
//            {
//                html += String.Format(@"<script type=""text/javascript"" src=""/_bitPlate/{0}""></script>
//", script);
//            }
            
//            return html;
//        }

        //private static string GetEditPageBody(CmsPage page)
        //{
        //    if (page.Template == null) return "";
        //    string bodyContent = page.Template.GetBodyContent();
        //    foreach (CmsTemplateContainer container in page.Template.Containers)
        //    {
        //        string moduleContent = "";
        //        //string where = String.Format("FK_Site='{0}' AND FK_page is null AND FK_Template is null", page.Site.ID);
        //        //BaseCollection<Module> siteModules = BaseCollection<Module>.Get(where, "OrderingNumber");
        //        //foreach (Module mod in siteModules)
        //        //{
        //        //    if (mod.ContainerName == container.Name)
        //        //    {
        //        //        moduleContent += ModuleService.WrapModule(mod.ConvertToType());
        //        //    }
        //        //}
        //        ////dan modules zichtbaar in de template
        //        //where = String.Format("FK_Site='{0}' AND FK_page is null AND FK_Template = '{1}'", page.Site.ID, page.Template.ID);
        //        //BaseCollection<Module> templateModules = BaseCollection<Module>.Get(where, "OrderingNumber");
        //        //foreach (Module mod in templateModules)
        //        //{
        //        //    if (mod.ContainerName == container.Name)
        //        //    {
        //        //        moduleContent += ModuleService.WrapModule(mod.ConvertToType());
        //        //    }
        //        //}
        //        foreach (BaseModule mod in page.GetAllModules())
        //        {
        //            if (mod.ContainerName == container.Name)
        //            {
        //                //mod.CurrentPage = page;
        //                moduleContent += ModuleService.WrapModule(mod);
        //            }
        //        }

        //        bodyContent = bodyContent.Replace("[" + container.Name.ToUpper() + "]", "<div class='bitContainer' id='bitContainer" + container.Name + "'>" + moduleContent + "</div>");

        //    }
        //    bodyContent = bodyContent.Replace("[PAGETITLE]", page.Title);
        //    return bodyContent;
        //}

        //private static string GetPreviewPageBody(CmsPage page)
        //{
        //    if (page.Template == null) return "";
        //    string bodyContent = page.Template.GetBodyContent();
        //    foreach (CmsTemplateContainer container in page.Template.Containers)
        //    {
        //        string moduleContent = "";
                
        //        foreach (Module mod in page.GetAllModules())
        //        {
        //            if (mod.ContainerName == container.Name)
        //            {
        //                //mod.CurrentPage = page;
        //                moduleContent += mod.ToString(ModeEnum.View);
        //            }
        //        }

        //        bodyContent = bodyContent.Replace("[" + container.Name.ToUpper() + "]", moduleContent);
        //    }
        //    bodyContent = bodyContent.Replace("[PAGETITLE]", page.Title);
        //    return bodyContent;
        //}
        /// <summary>
        /// Functie wordt gebruikt voor hyperlinkspopup
        /// </summary>
        /// <param name="folderid"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetHyperlinksAndSubFolders(string folderid)
        {
            BaseService.CheckLoginAndLicense();
            string html = "";
            List<TreeGridItem> items = GetFoldersAndPages(folderid, "", "Name", "");
            foreach (TreeGridItem item in items)
            {
                if (item.Type == "Folder")
                {
                    html += String.Format(@"<div class=""bitFolder"" title=""{0}"" onclick=""BITHYPERLINKSPOPUP.getHyperlinksAndSubFolders('{0}');"">{1}</div>", item.ID, item.Name);
                }
                else
                {
                    html += String.Format(@"<div class=""linkToInsert"" title=""{0}""><a href='#' />{1}</a></div>", item.Url, item.Name);
                }
            }
            return html;
        }
        /// <summary>
        /// onderstaande wordt niet gebruikt
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetHyperlinksAndSubFolders_FileSystem(string folder)
        {
            BaseService.CheckLoginAndLicense();
            folder = folder.Replace("/", "\\");
            basePath = AppDomain.CurrentDomain.BaseDirectory;

            folder = basePath + folder;
            string html = "";
            if (folder != basePath)
            {
                //navigate upwards
                string parentFolder = Directory.GetParent(folder).FullName + "\\";

                parentFolder = parentFolder.Replace(basePath, "");
                if (parentFolder.EndsWith("\\"))
                {
                    parentFolder = parentFolder.Substring(0, parentFolder.Length - 1);
                }
                html += String.Format(@"<div class=""bitFolder"" title=""{0}"" ondblclick=""BITEDITOR.commandManager.getHyperlinksAndSubFolders('{0}');"">...</div>", parentFolder);
            }
            string[] subFolders = Directory.GetDirectories(folder);
            foreach (string subFolder in subFolders)
            {
                string folderName = Path.GetFileName(subFolder);
                //de folders die beginnen met _ overslaan
                if (!folderName.StartsWith("_") && folderName != "bin" && folderName != "Properties" && folderName != "Files" && folderName != "obj")
                {
                    string relativeFolder = subFolder.Replace(basePath, "");
                    relativeFolder = relativeFolder.Replace("\\", "/");
                    html += String.Format(@"<div class=""bitFolder"" title=""{0}"" ondblclick=""BITEDITOR.commandManager.getHyperlinksAndSubFolders('{0}');"">{1}</div>", relativeFolder, folderName);
                }
            }

            string[] files = Directory.GetFiles(folder, "*.aspx");
            List<string> list = new List<string>(files);
            list.AddRange(Directory.GetFiles(folder, "*.htm"));
            list.AddRange(Directory.GetFiles(folder, "*.html"));
            list.AddRange(Directory.GetFiles(folder, "*.pdf"));
            list.Sort();
            foreach (string file in list)
            {
                string url = file.Replace(basePath, "");
                string fileName = Path.GetFileName(file);

                url = url.Replace("\\", "/");
                html += String.Format(@"<div class=""linkToInsert"" title=""{0}""><a href='#' />{1}</a></div>", url, fileName);
            }

            return html;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRadioListTemplates()
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);

            BaseCollection<CmsTemplate> list = BaseCollection<CmsTemplate>.Get(where, "Name");
            return GetRadioListTemplateHtml(list, "Template.ID");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetDropDownListPageFolders()
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);

            BaseCollection<CmsPageFolder> list = BaseCollection<CmsPageFolder>.Get(where, "RelativePath");
            return GetDropDownFolderHtml(list, "Folder");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetDropDownListFolderFolders()
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);

            BaseCollection<CmsPageFolder> list = BaseCollection<CmsPageFolder>.Get(where, "RelativePath");
            return GetDropDownFolderHtml(list, "ParentFolder");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCheckBoxListScripts()
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);

            BaseCollection<CmsScript> list = BaseCollection<CmsScript>.Get(where, "ScriptType, Name");
            return GetCheckBoxListHtml<CmsScript>(list, "");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CopyPage(string pageId, string newPageName)
        {
            BaseService.CheckLoginAndLicense();
            CmsPage page = BaseObject.GetById<CmsPage>(new Guid(pageId));
            if (page.HasBitplateAutorisation())
            {
                if (!page.IsAutorized(SessionObject.CurrentBitplateUser))
                {
                    throw new Exception("U heeft geen rechten voor deze pagina");
                }
            }
            CmsPage copy = page.Copy(newPageName, null);

            ////lijst van ObjectPermissions wordt hier alleen als drager gebruikt.
            ////in de licentieserver wordt deze lijst weer gesplitst en in 2 tabellen gezet
            //BaseCollection<ObjectPermission> objPermissions = copy.GetObjectPermissions4LicenseServer();
            //if (objPermissions.Count > 0)
            //{
            //    BitAutorisationService.AutorisationClient client = BitMetaServerServicesHelper.GetClient();
            //    client.SaveObjectPermissions(objPermissions);
            //}

            //BaseCollection<BaseModule> NewModules = new BaseCollection<BaseModule>();

        }

        /* private static BaseModule CopyModule(BaseModule module, CmsPage page)
        {
            BaseModule NewModule = module.CreateCopy<BaseModule>(true);
            NewModule.Page = page;
            NewModule.Save();
            NewModule.Publish();
            return NewModule;
        } */

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CopyFolder(string folderId, string newFolderName)
        {
            BaseService.CheckLoginAndLicense();
            CmsPageFolder folder = BaseObject.GetById<CmsPageFolder>(new Guid(folderId));
            CmsPageFolder copy = folder.Copy(newFolderName, null);

            //BaseCollection<ObjectPermission> objPermissions = copy.GetObjectPermissions4LicenseServer();
            //if (objPermissions.Count > 0)
            //{
            //    BitAutorisationService.AutorisationClient client = BitMetaServerServicesHelper.GetClient();
            //    client.SaveObjectPermissions(objPermissions);
            //}
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<BaseModule> GetModulesByPageId(Guid pageID)
        {
            BaseCollection<BaseModule> modules = BaseCollection<BaseModule>.Get("FK_Page = '" + pageID + "'");
            return modules;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public bool RecoverModuleById(Guid moduleID)
        {
            BaseModule module = BaseObject.GetById<BaseModule>(moduleID);
            if (module != null)
            {
                string content = module.Content;
                module.Content = "Deze module is hersteld. Om de content terug in de module te plaatsen kiest u bewerken en klik op opslaan.<br /><strong>LETOP!! Corrigeer de content voordat u op opslaan klikt.</strong>";
                module.Save();
                
                module.Content = content;
                module.Save();
                //if (module.IsVisibleOnAllPages || module.IsVisibleOnAllPagesInLayout)
                //{
                //    BitCaching.ClearCache();
                //}
                //else
                //{
                    
                //    BitCaching.RemoveItemFromCache(module.Page.ID.ToString());
                //}
                
                return true;
            }
            else
            {
                return false;
            }
        }

        

        //private static void CopyFolder(string parentFolderId, CmsPageFolder folder, bool newName, string folderName)
        //{
        //    CmsPageFolder newFolder = folder.Copy<CmsPageFolder>(newName);
        //    if (folderName != "")
        //    {
        //        newFolder.Name = folderName;
        //    }

        //    if (parentFolderId != "" && parentFolderId != null)
        //    {
        //        newFolder.ParentFolder = new CmsPageFolder();
        //        newFolder.ParentFolder.ID = new Guid(parentFolderId);
        //    }
        //    newFolder.Save();
        //    foreach (CmsPageFolder subFolder in folder.GetSubFolders())
        //    {
        //        CopyFolder(newFolder.ID.ToString(), subFolder, false, "");
        //    }
        //    foreach (CmsPage page in folder.GetPages())
        //    {
        //        CopyPage(newFolder.ID.ToString(), page, false, "");
        //    }
        //}

        //private static void CopyPage(string folderId, CmsPage page, bool newName, string pageName)
        //{
        //    CmsPage newPage = page.Copy<CmsPage>(newName);

        //    if (pageName != "")
        //    {
        //        newPage.Name = pageName;
        //    }

        //    if (folderId != "" && folderId != null)
        //    {
        //        newPage.Folder = new CmsPageFolder();
        //        newPage.Folder.ID = new Guid(folderId);
        //    }
        //    newPage.Save();
        //}


        private static string GetRadioListTemplateHtml(BaseCollection<CmsTemplate> dataList, string propertyName)
        {
            string tr1 = "<tr>";
            string tr2 = "<tr>";
            foreach (CmsTemplate template in dataList)
            {
                tr1 += String.Format("<td><img src='{0}' alt='{1}' title='{1}'/></td>", template.Screenshot, template.Name);
                tr2 += String.Format("<td><input type='radio'  value='{0}' id='{0}' name='{2}'/><label for='{0}'> {1}</label></td>", template.ID, template.Name, propertyName);

            }
            tr1 += "</tr>";
            tr2 += "</tr>";
            string html = "<table>" + tr1 + tr2 + "</table>";
            return html;
        }

        private static string GetDropDownFolderHtml(BaseCollection<CmsPageFolder> dataList, string propertyName)
        {
            string html = String.Format("<select name='{0}'>", propertyName);
            html += String.Format("<option value='{0}'>{1}</option>", "", "");
            foreach (CmsPageFolder folder in dataList)
            {
                html += String.Format("<option value='{0}'>{1}</option>", folder.ID, folder.RelativePath);
            }
            html += "</select>";
            return html;
        }

        private static string GetDropDownListHtml<T>(BaseCollection<T> dataList, string propertyName) where T : BaseDomainObject, new()
        {
            string html = String.Format("<select name='{0}'>", propertyName);
            html += String.Format("<option value='{0}'>{1}</option>", "", "");
            foreach (T t in dataList)
            {
                html += String.Format("<option value='{0}'>{1}</option>", t.ID, t.Name);
            }
            html += "</select>";
            return html;
        }

        private static string GetCheckBoxListHtml<T>(BaseCollection<T> dataList, string propertyName) where T : BaseDomainObject, new()
        {
            string html = "";
            foreach (T t in dataList)
            {
                html += String.Format("<input type='checkbox' value='{0}'> {1}<br/>", t.ID, t.Name);
            }

            return html;
        }

    }
}