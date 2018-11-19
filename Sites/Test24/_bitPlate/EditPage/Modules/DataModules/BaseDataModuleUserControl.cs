using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HJORM;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Modules.Data;
using System.Web.UI.HtmlControls;
using BitPlate.Domain.Modules;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace BitSite._bitPlate.EditPage.Modules.DataModules
{
    public class BaseDataModuleUserControl: BaseModuleUserControl
    {

        public Guid DataCollectionID { get; set; }

        public DataGroup ParentGroup;
        public DataGroup ParentParentGroup;

        public virtual void SelectAndShowData(Guid dataId)
        {
            
        }

        /// <summary>
        /// Module wordt vanuit andere module aangeroepen met ajax call (MS-updatepanels)
        /// Bij NavigateAction van type ShowDetailsInModules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void Reload(BaseModuleUserControl sender, NavigationParameterObject args = null)
        {
            if (base.CheckAutorisation())
            {
                if (args != null && args.Name == "dataid")
                {
                    this.SelectAndShowData(args.GuidValue);
                }
            }
        }


        protected virtual string getWhere(string filterWhere = "")
        {
            string where = String.Format("FK_DataCollection = '{0}'", DataCollectionID);
            string selectField = getSetting<string>("SelectField");
            string selectOperator = getSetting<string>("SelectOperator");
            string selectValue = getSetting<string>("SelectValue");
            //string selectGroupID = getSetting<string>("SelectGroupID");
            bool showInActive = getSetting<bool>("ShowInactive");
            if (selectField == "null") selectField = null;
            if (selectField != null && selectField != "")
            {
                if (selectOperator == "like" || selectOperator == "not like")
                {
                    where += String.Format(" AND {0} {1} '%{2}%'", selectField, selectOperator, selectValue);
                }
                else if (selectOperator == "likeStart" || selectOperator == "not likeStart")
                {
                    selectOperator = selectOperator.Replace("Start", "");
                    where += String.Format(" AND {0} {1} '{2}%'", selectField, selectOperator, selectValue);
                }
                else if (selectOperator == "likeEnd" || selectOperator == "not likeEnd")
                {
                    selectOperator = selectOperator.Replace("End", "");
                    where += String.Format(" AND {0} {1} '%{2}'", selectField, selectOperator, selectValue);
                }
                else
                {
                    if (selectOperator == "") selectOperator = "=";
                    where += String.Format(" AND {0} {1} '{2}'", selectField, selectOperator, selectValue);
                }
            }
            if (!showInActive)
            {
                where += String.Format(" AND (Active = 1 OR (Active = 2 AND IFNULL(DateFrom, '2000-1-1') <= '{0:yyyy-MM-dd} 00:00:00' AND IFNULL(DateTill, '2999-1-1') >= '{0:yyyy-MM-dd}'))", DateTime.Now);
            }

            if (filterWhere != "")
            {
                where += filterWhere;
            }
            
            return where;
        }

        protected virtual string getSort()
        {
            string sort = "";
            string sortField = getSetting<string>("SortField");
            if (sortField != null && sortField != "")
            {
                sort = sortField  + " " + getSetting<string>("SortOrder");
            }
            return sort.Trim();
        }

        /// <summary>
        /// Voor data modules met drilldown links
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void DrillDownLink_Click(Object sender, EventArgs args)
        {
            ModuleNavigationActionLite action = GetNavigationActionByTagName("{DrillDownLink}");
            if (action.NavigationType == NavigationTypeEnum.NavigateToPage)
            {
                Response.Redirect(action.NavigationUrl);
            }
            else if (action.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {
                Guid dataId;
                //click van linkbutton wordt via javascript:__doPostback() gedaan
                //de link is dus geen echte linkbutton
                //dataid staat in eventArgument van deze postbackfunctie
                string eventArgument = Request.Params.Get("__EVENTARGUMENT");
                Guid.TryParse(eventArgument, out dataId);

                foreach (string drillDownModuleId in action.RefreshModules)
                {
                    BaseModuleUserControl moduleControl = (BaseModuleUserControl)FindControlRecursive(this.Page.Master, "Mod" + drillDownModuleId.Replace("-", ""));
                    if (moduleControl != null)
                    {
                        moduleControl.Reload(this, new NavigationParameterObject() { Name = "dataid", GuidValue = dataId }); //{  //SelectAndShowData(dataId);
                    }
                }
                //stuur eventueel javascript naar de browser zodat deze wordt uitgevoerd nadat pagina opnieuw is gerenderd
                LinkButton linkButton = (LinkButton)sender;
                string js = linkButton.OnClientClick;
                if (js != null && js != "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "navigation" + this.ModuleID.ToString("N"), js, true);
                }
            }
        }


        protected void overridePageTitleAndMetaTags(BaseDataObject dataObj)
        {
            //methode is alleen geldig voor dataDeatilsmodules
            //als data vanuit user komt, title op pagina overschrijven met item titel
            if (Request.QueryString["dataid"] != null && dataObj != null)
            {
                if (dataObj.Title != "" && dataObj.Title != null)
                {
                    this.Page.Title = dataObj.Title;
                }
                if (dataObj.MetaDescription != "" && dataObj.MetaDescription != null)
                {
                    HtmlGenericControl metaDescriptionCtl = (HtmlGenericControl)FindControlRecursive(this.Page, "MetaDescription");
                    if (metaDescriptionCtl != null)
                    {
                        metaDescriptionCtl.Attributes["Content"] = dataObj.MetaDescription;
                    }
                }
                if (dataObj.MetaKeywords != "" && dataObj.MetaKeywords != null)
                {
                    HtmlGenericControl metaKeywordsCtl = (HtmlGenericControl)FindControlRecursive(this.Page, "MetaKeywords");
                    if (metaKeywordsCtl != null)
                    {
                        metaKeywordsCtl.Attributes["Content"] = dataObj.MetaKeywords;
                    }
                }
            }
        }


    }
}