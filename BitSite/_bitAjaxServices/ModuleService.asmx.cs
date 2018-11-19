using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Services;

using BitPlate.Domain.Modules;
using BitPlate.Domain;
using HJORM;
using BitSite._bitPlate;

namespace BitSite._bitAjaxServices
{
    [System.Web.Script.Services.ScriptService]
    public partial class ModuleService : BaseService
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ReloadModule(string pageid, string id, Dictionary<string, object> parameters)
        {
            BaseModule module = BaseModule.GetById<BaseModule>(new Guid(id));
            if (module == null) return "";
            IRefreshableModule refreshableModule = module.ConvertToType() as IRefreshableModule;
            if (refreshableModule == null) return "";
            CmsPage page = BaseObject.GetById<CmsPage>(Guid.Parse(pageid));
            return refreshableModule.Reload(page, parameters);

        }

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public static string HandleAjaxPost(string moduleid, Dictionary<string, string> parameters)
        //{
        //    BaseModule module = BaseModule.GetById<BaseModule>(new Guid(moduleid));
        //    if (module == null) return "";
        //    IPostableModule postableModule = module.ConvertToType() as IPostableModule;
        //    if (postableModule == null) return "";

        //    return postableModule.HandlePost(parameters);

        //}

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DoPaging(string pageid, string moduleid, int pageNumber, int totalResults, Dictionary<string, object> parameters)
        {
            BaseModule module = BaseModule.GetById<BaseModule>(new Guid(moduleid));
            if (module == null) return "";
            IPageableModule pageableModule = module.ConvertToType() as IPageableModule;
            if (pageableModule == null) return "";
            CmsPage page = BaseObject.GetById<CmsPage>(Guid.Parse(pageid));
            return pageableModule.DoPaging(page, pageNumber, totalResults, parameters);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DoSort(string pageid, string moduleid, string column, SortDirectionEnum sortDirection, int pageNumber, int totalResults, Dictionary<string, object> parameters)
        {
            BaseModule module = BaseModule.GetById<BaseModule>(new Guid(moduleid));
            if (module == null) return "";
            ISortable pageableModule = module.ConvertToType() as ISortable;
            if (pageableModule == null) return "";
            CmsPage page = BaseObject.GetById<CmsPage>(Guid.Parse(pageid));
            return pageableModule.DoSort(page, column, sortDirection, pageNumber, totalResults, parameters);
        }
    }
}