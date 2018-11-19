using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM.Attributes;

using BitPlate.Domain.Utils;
using System.Web.Script.Serialization;

namespace BitPlate.Domain.Modules
{
    public enum NavigationTypeEnum { NavigateToPage, ShowDetailsInModules };


    //public class ModuleNavigationActionLite
    //{
    //    public string Name { get; set; }
    //    public NavigationTypeEnum NavigationType { get; set; }
    //    public string[] RefreshModules { get; set; }
    //    //Om er voor te zorgen dat een navigatie actie altijd een url heeft. (Wanneer een module geplaatst is, maar niet geconfigureerd.)
    //    private string _navigationUrl = System.Web.HttpContext.Current.Request.Url.ToString();
    //    public string NavigationUrl
    //    {
    //        get
    //        {
    //            return _navigationUrl;
    //        }
    //        set
    //        {
    //            _navigationUrl = value;
    //        }
    //    }
    //    public string JsFunction { get; set; }
    //}

    public class ModuleNavigationAction : BaseDomainSiteObject
    {
        public ModuleNavigationAction()
        {
        }

        public ModuleNavigationAction(string name)
        {
            Name = name;
        }

        [NonPersistent]
        public string TagName
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value; 
            }
        }

        public NavigationTypeEnum NavigationType { get; set; }
        [NonPersistent()]
        public string NavigationTypeString
        {
            get
            {
                //Name() method van enum = extension method in Utils.TypeExtensions.cs
                return NavigationType.Name(); 
            }
        }

        private CmsPage _navigationPage;
        [Association("FK_NavigationPage")]
        public CmsPage NavigationPage
        {
            get
            {
                if (_navigationPage != null && !_navigationPage.IsLoaded)
                {
                    _navigationPage.Load();
                }
                return _navigationPage;
            }
            set
            {
                _navigationPage = value;
            }
        }
        //[NonPersistent()]
        //public string ServerEventName { get; set; }
        public string JsFunction { get; set; }

        public string GetJavascriptInLine(){
            if (JsFunction != null)
            {
                string js = JsFunction.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
                js = js.Replace("\"", "'");
                return js;
            }
            return "";
        }
        /// <summary>
        /// ajax load andere modules op zelfde pagina
        /// </summary>
        public string[] RefreshModules { get; set; }

        private BaseModule _module;
        [Association("FK_Module")]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public BaseModule Module
        {
            get
            {
                if (_module != null && !_module.IsLoaded)
                {
                    _module.Load();
                }

                return _module;
            }
            set
            {
                _module = value;
                _module.IsLoaded = true;
            }
        }

        public string CreateNavigationHyperlink(string dataType, bool refreshItself = false, string moduleFunctionBeforeReload = "")
        {
            string navigationUrl = this.Name.Replace("Link", "Url");
            string hyperlink = "<a href=\""+ navigationUrl+ "\">";
            if (this.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {
                string refreshModules = String.Join(",", this.RefreshModules);
                //haal eventuele lege waardes weg
                refreshModules = refreshModules.Replace(",,", "");
                if (refreshModules.EndsWith(","))
                {
                    refreshModules = refreshModules.Remove(refreshModules.Length - 1, 1);
                }
                //breadcrumb moet altijd zichzelf refreshen. Dat doen we door eigen id toe te voegen aan refreshModules
                if (refreshItself && !refreshModules.Contains(this.Module.ID.ToString()))
                {
                    refreshModules += "," + this.Module.ID.ToString();
                }
                if (moduleFunctionBeforeReload != "" && !moduleFunctionBeforeReload.EndsWith(";")) moduleFunctionBeforeReload += ";";
                string extraJs = (this.JsFunction != null) ? this.JsFunction.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\"", "'") : "";
                hyperlink = "<a class=\"showDetailsInModules\" href=\"" + navigationUrl + "\" onclick=\"" + moduleFunctionBeforeReload + "BITSITESCRIPT.reloadModulesOnSamePage('" + refreshModules + "', {dataid: '{ID}', datatype: '" + dataType + "'});" + extraJs + "\">";
            }
            return hyperlink;
        }


        public string GetNavigationUrl()
        {
            string url = "";
            if (this.NavigationType == NavigationTypeEnum.NavigateToPage && this.NavigationPage != null)
            {
                url = this.NavigationPage.RelativeUrl;
            }
            return url;
        }

        public override void FillObject(Type type, System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            this.NavigationType = (NavigationTypeEnum) Convert.ToInt32(dataRow["NavigationType"]);
            this.RefreshModules = dataRow["RefreshModules"].ToString().Split(new char[] { ',' });
            this.JsFunction = dataRow["JsFunction"].ToString();
            if (dataRow["FK_NavigationPage"] != DBNull.Value)
            {
                this.NavigationPage = new CmsPage();
                this.NavigationPage.ID = new Guid(dataRow["FK_NavigationPage"].ToString());
            }
        }
    }
}
