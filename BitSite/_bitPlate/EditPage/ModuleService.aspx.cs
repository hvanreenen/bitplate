using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

using HJORM;

using BitPlate.Domain;
using BitPlate.Domain.Autorisation;

using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Utils;
using System.Text;
using System.IO;

using System.Web.UI.HtmlControls;

using BitPlate.Domain.Modules;
using BitPlate.Domain.Modules.Search;
using BitPlate.Domain.Modules.Data;
using BitPlate.Domain.Modules.Auth;
using BitPlate.Domain.Modules.ContactForm;
using BitPlate.Domain.Newsletters;
using BitSite._bitPlate._bitSystem;

namespace BitSite._bitPlate.EditPage
{
    [GenerateScriptType(typeof(BaseModule))]
    [GenerateScriptType(typeof(BaseDataModule))]
    //search
    [GenerateScriptType(typeof(SearchModule))]
    [GenerateScriptType(typeof(SearchResultsModule))]
    //data
    [GenerateScriptType(typeof(GroupListModule))]
    [GenerateScriptType(typeof(GroupDetailsModule))]
    [GenerateScriptType(typeof(ItemListModule))]
    [GenerateScriptType(typeof(ItemDetailsModule))]
    [GenerateScriptType(typeof(TreeViewModule))]
    [GenerateScriptType(typeof(DataBreadCrumbModule))]
    [GenerateScriptType(typeof(GoogleMapsModule))]
    [GenerateScriptType(typeof(FilterModule))]
    //auth
    [GenerateScriptType(typeof(LoginModule))]
    [GenerateScriptType(typeof(LoginStatusModule))]
    [GenerateScriptType(typeof(MyProfileModule))]
    //form
    [GenerateScriptType(typeof(ContactFormModule))]
    public partial class ModuleService : BaseService
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object[] AddNewModule(string type, string pageid, string containername, int sortorder)
        {
            string Url = HttpContext.Current.Request.UrlReferrer.ToString();
            BaseService.CheckLoginAndLicense();
            //volgende waardes terug geven:
            //1. De inhoud gewrapt met module toolbar, 
            //2: de specifieke javascripts
            //3: de moduleID
            object[] returnValue = new object[3];
            BaseModule module = null;
            type = type.Replace("bitModule", "");
            containername = containername.Replace("bitContainer", "");

            module = ModuleLoader.Load(type);
            if (module != null)
            {
                module.Name = type;
                module.Site = SessionObject.CurrentSite;
                //module.Page = new CmsPage();
                //module.Page.ID = new Guid(pageid);
                if (Url.ToLower().Contains("/newsletters"))
                {
                    module.Newsletter = new Newsletter();
                    module.Newsletter.ID = new Guid(pageid);
                }
                else
                {
                    module.Page = new CmsPage();
                    module.Page.ID = new Guid(pageid);
                }
                module.ContainerName = containername;
                module.OrderingNumber = sortorder;
                module.Content = module.FirstContentSample;
                module.Settings.Add("UseCkeditor", true);
                //WORK ARROUND SETTINGS WILLEN NIET OPSLAAN.
                module.SettingsJsonString = module.Settings.ToJsonString();
                module.Save();
                
                
                //SetUnpublishedItem(module);

                returnValue[1] = module.IncludeScripts;
                returnValue[2] = module.ID.ToString();
                //moet naar container
                //updateOrderingNumber
                //alleen degene ophalen met een groter nummer dan sortorder
                string where = String.Format("FK_PAGE = '{0}' And ContainerName='{1}' AND OrderingNumber > {2}", pageid, containername, sortorder);
                BaseCollection<BaseModule> modules = BaseCollection<BaseModule>.Get(where);
                foreach (BaseModule mod in modules)
                {
                    mod.OrderingNumber++;
                    mod.Save();
                }
                
            }
            module.ForceReloadTags(); //De tags zijn al gegenereerd, maar dit is gebeurd voor het opslaan van de module. Hierdoor hebben de tags geen geldig control ID. Forceer hergeneratie van de tags. Dit is alleen bij nieuwe modules nodig.
            PublishModule(module);
            //Na toevoegen nieuwe ook pagina updaten
            PublishDependentPages(module);
            
            returnValue[0] = new ModuleService().GetUserControlContent(module);
            
            return returnValue;
        }

        

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string MoveModule(string moduleid,string pageid, string containername, int sortorder)
        {
            BaseService.CheckLoginAndLicense();

            string where = String.Format("FK_PAGE = '{0}' And ContainerName='{1}' AND OrderingNumber >= {2}", pageid, containername, sortorder);
            //where = String.Format("FK_PAGE = '{0}' And Container='{1}'", pageid, containername);
            BaseCollection<BaseModule> modules = BaseCollection<BaseModule>.Get(where, "OrderingNumber");
            foreach (BaseModule mod in modules)
            {
                mod.OrderingNumber++;
                mod.Save();
            }

            string returnValue = "";
            //moduleid = moduleid.Replace("bitModule", "");
            moduleid = moduleid.Replace("bitModule", "").Replace("_", "-");
            containername = containername.Replace("bitContainer", "");

            BaseModule module = (BaseModule)BaseDomainObject.GetById<BaseModule>(new Guid(moduleid));
            module.ContainerName = containername;
            module.OrderingNumber = sortorder;
            module.Save();
            returnValue = new ModuleService().GetUserControlContent(module);
            //moet naar container
            //updateOrderingNumber
            //alleen degene ophalen met een groter nummer dan sortorder
            
            PublishDependentPages(module);
            return returnValue;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetModuleContent(string id, string mode)
        {
            BaseService.CheckLoginAndLicense();
            string returnValue = "";
            //id = id.Replace("bitModule", "");
            id = id.Replace("bitModule", "").Replace("_", "-");

            BaseModule module = (BaseModule)BaseDomainObject.GetById<BaseModule>(new Guid(id));
            module = module.ConvertToType();
            if (mode == "edit")
            {
                returnValue = module.Content;
                Uri uri = System.Web.HttpContext.Current.Request.UrlReferrer;
                string currentDomain = uri.GetLeftPart(UriPartial.Authority);
                if (SessionObject.CurrentSite.DomainName != currentDomain)
                {
                    returnValue = HtmlHelper.ReplaceImageSources(returnValue, ModeEnum.Edit, SessionObject.CurrentSite);
                }
                if (module.HasAutorisation)
                {
                    BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
                    BitplateUserGroup[] usergroups = client.GetUserGroupsByObjectPermission(module.ID);
                    module.AutorizedBitplateUserGroups = usergroups;

                    BitplateUser[] users = client.GetUsersByObjectPermission(module.ID);
                    module.AutorizedBitplateUsers = users;
                    if (!module.IsAutorized(SessionObject.CurrentBitplateUser))
                    {
                        throw new Exception("U heeft geen rechten voor deze module");
                    }
                }
            }
            else
            {
                //module.LoadPropsFromXmlFile();
                returnValue = new ModuleService().GetUserControlContent(module);
            }

            
            return returnValue;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveModuleContent(string id, string content)
        {
            BaseService.CheckLoginAndLicense();
            id = id.Replace("bitModule", "");
            id = id.Replace("bitEditor", "");
            BaseModule module = BaseModule.GetById<BaseModule>(new Guid(id));
            module = module.ConvertToType();
            //module.LoadPropsFromXmlFile();
            content = content.Replace("?mode=edit", ""); //BUG WORKAROUND. BUG ZIT MOGELIJK NOG IN DE EDITOR.
            module.Content = content;
            module.Save();

            module.Publish();

            //Experimenteel caching reset
            //if (module.Page != null)
            //{
            //    BitCaching.RemoveItemFromCache(module.Page.ID.ToString());
            //}

            if (module.Type == "HtmlModule") PublishModule(module);
            return new ModuleService().GetUserControlContent(module);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string RenderModule(Guid moduleId)
        {
            BaseModule Module = BaseObject.GetById<BaseModule>(moduleId);
            return new ModuleService().GetUserControlContent(Module);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveModule(BaseModule module, string pageid)
        {
            string Url = HttpContext.Current.Request.UrlReferrer.ToString();
            BaseService.CheckLoginAndLicense();
            module.Site = SessionObject.CurrentSite;
            if (module.IsVisibleOnAllPages)
            {
                module.Template = null;
                module.Page = null;
            }
            else if (module.IsVisibleOnAllPagesInLayout)
            {
                if (Url.ToLower().Contains("/newsletters"))
                {
                    Newsletter newletter = BaseObject.GetById<Newsletter>(new Guid(pageid));
                    module.Template = newletter.Template;
                }
                else
                {
                    CmsPage page = BaseObject.GetById<CmsPage>(new Guid(pageid));
                    module.Template = page.Template;
                }
            }
            else
            {
                
                if (Url.ToLower().Contains("/newsletters"))
                {
                    module.Newsletter = new Newsletter();
                    module.Newsletter.ID = new Guid(pageid);
                }
                else
                {
                    module.Page = new CmsPage();
                    module.Page.ID = new Guid(pageid);
                }
            }
            module.Save();
            //SetUnpublishedItem(module);
            module = module.ConvertToType();
            
            //lijst van ObjectPermissions wordt hier alleen als drager gebruikt.
            //in de licentieserver wordt deze lijst weer gesplitst en in 2 tabellen gezet
            BaseCollection<ObjectPermission> objPermissions = module.GetObjectPermissions4LicenseServer();
            if (objPermissions.Count > 0)
            {
                BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
                client.SaveObjectPermissions(objPermissions);
            }
            //publiceren van pagina('s)
            PublishModule(module);
            PublishDependentPages(module);

            //Experimenteel caching reset
            //if (module.Page != null) BitCaching.RemoveItemFromCache(module.Page.ID.ToString());

            string returnValue = new ModuleService().GetUserControlContent(module);
            return returnValue;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void UpdateModuleOrderingNumbers(Dictionary<string, object> obj)
        {
            string ContainerName = obj["ContainerName"].ToString();
            Array Modules = (Array)obj["Modules"];
            foreach (Dictionary<string, object> moduleData in Modules)
            {
                BaseModule module = BaseObject.GetById<BaseModule>(Guid.Parse(moduleData["ID"].ToString()));
                module.OrderingNumber = int.Parse(moduleData["OrderingNumber"].ToString());
                module.Save();
            }
        }

        private static void PublishModule(BaseModule module)
        {
            //bij html hele pagina publiceren 
            if (module.Type == "HtmlModule" && !module.HasAutorisation)
            {
                PublishDependentPages(module);
            }
            else
            {
                //anders is alleen .ascx voldoende
                module.Publish();
            }
        }

        private static void PublishDependentPages(BaseModule module)
        {
            if (module.IsVisibleOnAllPages)
            {
                foreach (CmsPage page in SessionObject.CurrentSite.Pages)
                {
                    UnpublishedItem.Set(page, "Page");
                    page.Publish(false);
                }
            }
            else if (module.IsVisibleOnAllPagesInLayout)
            {
                foreach (CmsPage page in module.Template.GetPages())
                {
                    UnpublishedItem.Set(page, "Page");
                    page.Publish(false);
                }
            }
            else
            {
                if (module.Page != null)
                {
                    if (!module.Page.IsLoaded) module.Page.Load();
                    module.Page.Publish(false);
                }

                if (module.Newsletter != null)
                {
                    if (!module.Newsletter.IsLoaded) module.Newsletter.Load();
                    module.Newsletter.Publish(false);
                }
            }
        }

        //private static void SetUnpublishedItem(BaseModule module)
        //{
        //    if (module.IsVisibleOnAllPages)
        //    {
        //        foreach (CmsPage page in SessionObject.CurrentSite.Pages)
        //        {
        //            UnpublishedItem.Set(page, "Page", true);
        //        }
        //    }
        //    else if (module.IsVisibleOnAllPagesInLayout)
        //    {
        //        UnpublishedItem.Set(module.Template, "Template", true);
        //    }
        //    else
        //    {
        //        UnpublishedItem.Set(module.Page, "Page", true);
        //    }
        //}

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void DeleteModule(string id)
        {
            BaseService.CheckLoginAndLicense();
            BaseModule module = GetModule(id);
            
            CmsPage page = module.Page;
            if (page != null)
            {
                //verwijderen drilldown modules
                string where = String.Format("FK_Page='{0}'", module.Page.ID);
                BaseCollection<BaseModule> modulesOnThisPage = BaseCollection<BaseModule>.Get(where);
                //foreach (BaseModule mod in modulesOnThisPage)
                //{
                //    if (mod.DrillDownType == NavigationTypeEnum.ShowDetailsInModules)
                //    {
                //        string modArray = mod.DrillDownModules.ToString().Replace(module.ID + ",", "");
                //        mod.DrillDownModules = modArray.Split(new char[] { ',' });
                //    }
                //}
                //Remove page from caching.
                //BitCaching.RemoveItemFromCache(page.ID.ToString());
            }
            //SetUnpublishedItem(module);
            module.Delete();
            
            PublishDependentPages(module);
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static BaseModule GetModule(string id)
        {
            BaseService.CheckLoginAndLicense();
            id = id.Replace("bitModule", "");
            id = id.Replace("bitEditor", "");
            BaseModule mod = BaseModule.GetById<BaseModule>(new Guid(id));
            
            if (mod.HasAutorisation)
            {
                BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
                BitplateUserGroup[] usergroups = client.GetUserGroupsByObjectPermission(mod.ID);
                mod.AutorizedBitplateUserGroups = usergroups;

                BitplateUser[] users = client.GetUsersByObjectPermission(mod.ID);
                mod.AutorizedBitplateUsers = users;
                if (!mod.IsAutorized(SessionObject.CurrentBitplateUser))
                {
                    throw new Exception("U heeft geen rechten voor deze module");
                }
            }
            mod = mod.ConvertToType();
            //if (mod.SelectGroup != null && mod.SelectGroup != "")
            //{
            //    DataGroup dataGroup = BaseObject.GetById<DataGroup>(Guid.Parse(mod.SelectGroup));
            //    if (dataGroup != null)
            //    {
            //        mod.SelectGroupPath = dataGroup.DataCollection.Name + "/" + dataGroup.GetCompletePath(); //FindDataGroupBasePath(Guid.Parse(mod.SelectGroup));
            //    }
            //}
            return mod;
        }

        //private static string FindDataGroupBasePath(Guid DataCollectionId)
        //{
        //    DataGroup dataGroup = BaseObject.GetById<DataGroup>(DataCollectionId);
        //    string result = "";
        //    string dataCollectionName = dataGroup.DataCollection.Name;
        //    while (dataGroup != null)
        //    {
        //        result = dataGroup.Name + @"\" + result;
        //        dataGroup = dataGroup.ParentGroup;
        //    }
        //    return dataCollectionName + @"\" + result;
        //}

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Dictionary<string, object> GetTags(string id)
        {
            BaseService.CheckLoginAndLicense();
            List<string> tagArray = new List<string>();
            string tagString = "Dubbelklik op een tag om het op de plaats van de cursor te zetten<br/>";
            id = id.Replace("bitModule", "");
            id = id.Replace("bitEditor", "");
            //id = id.Replace("Template", "");
            if (id.Contains("Template"))
            {
                tagString += "<br/>";
                tagString += "<strong>Body-tags:</strong> (Deze tags kunnen genummerd worden.<br />Bijvoorbeeld: [CONTENT1], [CONTENT2], [CONTENT...)<br/>";
                tagString += "<div class='tagToInsert'>[CONTENT]</div>";
                tagArray.Add("[CONTENT]");

                tagString += "<div class='tagToInsert'>[TOP]</div>";
                tagArray.Add("[TOP]");
                tagString += "<div class='tagToInsert'>[LEFT]</div>";
                tagArray.Add("[LEFT]");
                tagString += "<div class='tagToInsert'>[CENTER]</div>";
                tagArray.Add("[CENTER]");
                tagString += "<div class='tagToInsert'>[RIGHT]</div>";
                tagArray.Add("[RIGHT]");
                tagString += "<div class='tagToInsert'>[BOTTOM]</div>";
                tagArray.Add("[BOTTOM]");
                tagString += "<br/>";
                tagString += "<strong>Head-tags:</strong><br/>";
                tagString += "<div class='tagToInsert'>[PAGETITLE]</div>";
                tagArray.Add("[PAGETITLE]");
                tagString += "<div class='tagToInsert'>[SCRIPTS]</div>";
                tagArray.Add("[SCRIPTS]");
                tagString += "<div class='tagToInsert'>[HEAD]</div>";
                tagArray.Add("[HEAD]");
                //returnValue += "<div class='tagToInsert'>[METADESCRIPTION]</div>";
                //returnValue += "<div class='tagToInsert'>[METAKEYWORDS]</div>";
            }
            else if (id != "")
            {
                //TemplateModule module = (TemplateModule)BaseDomainObject.GetById<TemplateModule>(new Guid(id));
                BaseModule module = (BaseModule)BaseDomainObject.GetById<BaseModule>(new Guid(id));
                module = module.ConvertToType();
                List<Tag> tags = module.GetAllTags();
               tags = tags.OrderBy(c => c.Name).ToList();

               foreach (Tag tag in tags) //ModeEnum.EditPageMode))
                {
                    if (tag.HasCloseTag)
                    {
                        tagString += String.Format("<div class='tagToInsert'>{0}</div>", tag.Name + tag.CloseTag);
                        tagArray.Add(tag.Name + tag.CloseTag);
                    }
                    else
                    {
                        tagString += String.Format("<div class='tagToInsert'>{0}</div>", tag.Name);
                        tagArray.Add(tag.Name);

                        if (tag.AllowFormats && tag.SampleFormats != null)
                        {
                            foreach (string sampleFormat in tag.SampleFormats)
                            {
                                string tagWithFormat = tag.Name.Replace("}", ":" + sampleFormat + "}");
                                tagString += String.Format("<div class='tagToInsert'>{0}</div>", tagWithFormat);
                                tagArray.Add(tagWithFormat);
                            }
                        }
                    }
                }
               if (tags.Count == 0)
                {
                    tagString = "Deze module kent geen tags";
                }
            }
            Dictionary<string, object> returnvalue = new Dictionary<string, object>();
            returnvalue.Add("CKTags", tagString);
            returnvalue.Add("CMTags", tagArray.ToArray());
            return returnvalue;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<TreeGridItem> GetModulesLiteByPageID(string pageid)
        {
            BaseService.CheckLoginAndLicense();
            List<TreeGridItem> returnValue = new List<TreeGridItem>();
            string where = String.Format("FK_Page = '{0}'", pageid);
            BaseCollection<BaseModule> modules = BaseCollection<BaseModule>.Get(where);
            foreach (BaseModule mod in modules)
            {
                TreeGridItem item = new TreeGridItem();
                item.ID = mod.ID;
                item.Name = mod.Name + " @ " + mod.ContainerName + "-container";
                returnValue.Add(item);
            }
            return returnValue;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<ListItem> GetSelectAndSortFieldsByDataCollectionID(string datacollectionid, string moduleType)
        {
            BaseService.CheckLoginAndLicense();
            string type = "item";
            if (moduleType == "GroupListModule")
            {
                type = "group";
            }
            string whereIn = (int)FieldTypeEnum.Text + ", ";
            whereIn += (int)FieldTypeEnum.DateTime + ", ";
            whereIn += (int)FieldTypeEnum.Currency + ", ";
            whereIn += (int)FieldTypeEnum.Numeric + ", ";
            whereIn += (int)FieldTypeEnum.YesNo + ", ";
            whereIn += (int)FieldTypeEnum.DropDown;
            string where = String.Format("FK_DataCollection = '{0}' AND Type='{1}' AND FieldType IN({2})", datacollectionid, type, whereIn);
            BaseCollection<DataField> fields = BaseCollection<DataField>.Get(where);
            List<ListItem> returnItems = new List<ListItem>();
            foreach (DataField fld in fields)
            {
                returnItems.Add(new ListItem(fld.Name, fld.MappingColumn));
            }

            //return fields;
            return returnItems;
        }
        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public static string AddNewModule(string type, string siteid, string pageid, string containername, int sortorder)
        //{
        //    string returnValue = "";
        //    Module mod = null;
        //    type = type.Replace("bitModule", "");
        //    containername = containername.Replace("bitContainer", "");
        //    if (type == "HTML")
        //    {
        //        mod = new HTMLModule();
        //    }
        //    else if (type == "Links")
        //    {
        //        mod = new LinksModule();
        //    }
        //    else if (type == "Search")
        //    {
        //        mod = new SearchModule();
        //    }
        //    else if (type == "SearchResults")
        //    {
        //        mod = new SearchResultsModule();
        //    }
        //    else if (type == "Maingroups")
        //    {
        //        mod = new BitPlate.Domain.Modules.MainGroupModule();
        //    }
        //    else if (type == "Subgroups")
        //    {
        //        mod = new BitPlate.Domain.Modules.SubGroupModule();
        //    }
        //    else if (type == "Groupdetails")
        //    {
        //        mod = new BitPlate.Domain.Modules.GroupDetailsModule();
        //    }
        //    else if (type == "Items")
        //    {
        //        mod = new ItemsModule();
        //    }
        //    else if (type == "Itemdetails")
        //    {
        //        mod = new ItemDetailsModule();
        //    }
        //    else if (type == "Subscribe")
        //    {
        //        mod = new SubscribeModule();
        //    }
        //    else if (type == "Image")
        //    {
        //        mod = new ImageModule();
        //    }
        //    else if (type == "Media")
        //    {
        //        mod = new MediaModule();
        //    }
        //    else if (type == "BreadCrumb")
        //    {
        //        mod = new BreadCrumbModule();
        //    }
        //    else if (type == "Login")
        //    {
        //        mod = new LoginModule();
        //    }
        //    else if (type == "User")
        //    {
        //        //mod = new UserModule();
        //    }
        //    else if (type == "Form")
        //    {
        //        mod = new FormModule();
        //    }
        //    else if (type == "Products")
        //    {
        //        mod = new ProductsModule();
        //    }
        //    else if (type == "ProductDetails")
        //    {
        //        mod = new ProductDetailsModule();
        //    }
        //    else if (type == "ShoppingCart")
        //    {
        //        mod = new ShoppingCartModule();
        //    }
        //    else if (type == "OrderForm")
        //    {
        //        mod = new OrderFormModule();
        //    }
        //    else if (type == "PaymentTypeForm")
        //    {
        //        mod = new PaymentTypeModule();
        //    }
        //    else if (type == "OrderPrint")
        //    {
        //        mod = new OrderPrintModule();
        //    }
        //    else if (type == "Payment")
        //    {
        //        mod = new PaymentModule();
        //    }
        //    else if (type == "OrderList")
        //    {
        //        //mod = new OrderListModule();
        //    }
        //    else if (type == "DocumentManagement")
        //    {
        //        mod = new DocumentManagementModule();
        //    }
        //    else if (type == "Popup")
        //    {
        //        mod = new PopupModule();
        //    }
        //    else if (type == "Tree")
        //    {
        //        mod = new TreeModule();
        //    }
        //    else if (type == "ItemsForm")
        //    {
        //        //mod = new ItemDetailsEditModule();
        //    }
        //    else if (type == "GoogleMaps")
        //    {
        //        //mod = new GoogleMapsModule();
        //    }

        //    if (mod != null)
        //    {
        //        mod.Page = new CmsPage();
        //        mod.Page.ID = new Guid(pageid);
        //        mod.Container = containername;
        //        mod.SortOrder = sortorder;
        //        //SessionObject.CurrentSite = (CmsSite)BaseDomainObject.GetById<CmsSite>(new Guid(siteid));
        //        // mod.Site = SessionObject.CurrentSite;
        //        mod.Save();
        //        returnValue = mod.ToString();
        //        //moet naar container
        //        //updateSortOrder
        //        //alleen degene ophalen met een groter nummer dan sortorder
        //        string where = String.Format("FK_PAGE = '{0}' And Container='{1}' AND SortOrder > {2}", pageid, containername, sortorder);
        //        BaseCollection<Module> modules = BaseCollection<Module>.Get(where);
        //        foreach (Module module in modules)
        //        {
        //            module.SortOrder++;
        //            module.Save();
        //        }
        //    }
        //    return returnValue;
        //}

        public static string GetModuleRenderErrorPlaceHolder(Exception ex, BaseModule module = null)
        {
            string returnValue;
            returnValue = string.Format("<div class=\"bitModule bitModuleError\" data-module-type=\"{1}\">", module.ID, module.Type);
            returnValue = "<div style=\"background-color: #FFF; color: red; padding: 5px; border: 3px solid red;\" id=\"bitModule{0:N}\"><strong>Er is een fout opgetreden tijdens het renderen van de module.</strong><br />";
            returnValue += "Melding: " + ex.Message.ToString();
            returnValue += "</div>";
            return returnValue;
        }
    }
}
