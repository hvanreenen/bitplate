using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Modules.Data;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate.EditPage.Modules.DataModules
{
    public partial class BreadCrumbModuleControl : BaseDataModuleUserControl
    {

        Repeater repeater;

        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Page_Load Start", "BreadCrumbModuleControl");
            base.Load(sender, e);
            this.Load();
            if (!IsPostBack)
            {
                Guid dataId = Guid.Empty;
                string dataIdQuery = Request.QueryString["dataid"];
                Guid.TryParse(dataIdQuery, out dataId);

                this.SelectAndShowData(dataId);
            }
            System.Diagnostics.Trace.WriteLine("Page_Load End", "BreadCrumbModuleControl");
        }

        private void Load()
        {

            Repeater repeater = (Repeater)FindControl("Repeater" + ModuleID.ToString("N"));
            if (repeater == null)
            {
                LabelMsg.Text = "Geen repeater gevonden.<br />";
                return;
            }

            this.LoadSettings();

            if (this.DataCollectionID == null || this.DataCollectionID == Guid.Empty)
            {
                this.LabelMsg.Text = "Kies eerst een datacollectie";
            }

        }
        /// <summary>
        /// wordt aangeroepen vanuit Page_Load() en ReLoad() in BaseDataModuleUserControl
        /// </summary>
        /// <param name="dataId"></param>
        /// 
        public override void SelectAndShowData(Guid dataId)
        {
            DataGroup group = BaseObject.GetById<DataGroup>(dataId);
            if (group == null)
            {
                DataItem item = BaseObject.GetById<DataItem>(dataId);
                if (item != null)
                {
                    group = item.ParentGroup;
                }
            }
            if (group == null)
            {
                group = new DataGroup();

            }
            if (group.DataCollection != null && group.DataCollection.ID != DataCollectionID)
            {

                //return;
            }
            ModuleNavigationActionLite navigationAction = GetNavigationActionByTagName("{DrillDownLink}");
            if (navigationAction != null)
            {
                string navigationUrl = navigationAction.NavigationUrl;
                if (navigationAction.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
                {
                    //blijven op dezelfde pagina
                    navigationUrl = Request.Url.LocalPath;
                }
                group.RewriteUrl = group.GetRewriteUrl(navigationUrl);

                //maak list aan 
                BaseCollection<BaseDataObject> dataObjects = new BaseCollection<BaseDataObject>();
                dataObjects.Add(group);
                while (group.ParentGroup != null)
                {
                    group = group.ParentGroup;
                    group.RewriteUrl = group.GetRewriteUrl(navigationUrl);
                    dataObjects.Insert(0, group);
                }
                DataBind(dataObjects);
            }

        }

        public void DataBind(BaseCollection<BaseDataObject> dataObjects)
        {
            repeater = (Repeater)FindControl("Repeater" + ModuleID.ToString("N"));
            if (repeater == null)
            {
                LabelMsg.Text = "Geen repeater gevonden.<br />";
                return;
            }
            if (dataObjects.Count > 0)
            {
                string pageLanguage = this.GetLanguageCodeFromMasterPage();
                ModuleNavigationActionLite navigationAction = GetFirstNavigationAction();
                if (navigationAction != null)
                {
                    //set rewriteurl per groep 
                    foreach (BaseDataObject dataObject in dataObjects)
                    {
                        dataObject.RewriteUrl = dataObject.GetRewriteUrl(navigationAction.NavigationUrl);

                        if (pageLanguage != "" && pageLanguage != null)
                        {
                            //dataObject.Translate(pageLanguage);
                        }
                    }
                }

                repeater.DataSource = dataObjects;
                //repeater.ItemDataBound += repeater_ItemDataBound; //Trigger list repeater.
                repeater.DataBind();
                repeater.Visible = true;

            }
            else
            {
                if (base.getSetting<bool>("HideWhenNoData"))
                {
                    repeater.Visible = false;
                }
            }


        }

        //public override void SelectAndShowData_old(Guid dataId)
        //{

        //    DataGroup group = BaseObject.GetById<DataGroup>(dataId);
        //    if (group == null)
        //    {
        //        DataItem item = BaseObject.GetById<DataItem>(dataId);
        //        if (item != null)
        //        {
        //            group = item.ParentGroup;
        //        }
        //    }
        //    if (group == null)
        //    {
        //        //breadCrumbLiteral.Text = GetBreadCrumbRoot();
        //        return;
        //    }
        //    if (group.DataCollection.ID != DataCollectionID)
        //    {
        //        //breadCrumbLiteral.Text = GetBreadCrumbRoot();
        //        return;
        //    }
        //    ModuleNavigationActionLite navigationAction = GetNavigationActionByName("{DrillDownLink}");
        //    if (navigationAction != null)
        //    {
        //        string html = "";
        //        string navigationUrl = navigationAction.NavigationUrl;
        //        if (navigationAction.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
        //        {
        //            //blijven op dezelfde pagina
        //            navigationUrl = Request.Url.LocalPath;
        //            group.RewriteUrl = group.GetRewriteUrl(navigationUrl);

        //            string dummyLinkButtonName = this.Controls[3].ClientID; //Linkbutton is altijd 3de element todo: deze code kan beter
        //            dummyLinkButtonName = dummyLinkButtonName.Replace("_", "$");
        //            string drillDownLink = String.Format(@"<a class=""showDetailsInModules"" href=""{0}"" onclick=""javascript:__doPostBack('{1}','{2}');"">", group.RewriteUrl, dummyLinkButtonName, group.ID);

        //            html = lastItemTemplate.Replace("{DrillDownLink}", drillDownLink)
        //                .Replace("{Name}", group.Name)
        //                .Replace("{Title}", group.Title)
        //                .Replace("{/DrillDownLink}", "</a>");
        //            while (group.ParentGroup != null)
        //            {
        //                group = group.ParentGroup;
        //                group.RewriteUrl = group.GetRewriteUrl(navigationUrl);

        //                //HYJAX: In BitSiteScript.js worden de hrefs vervangen door onClick events. 
        //                //Hrefs wel nodig voor seo met ajax
        //                drillDownLink = String.Format(@"<a class=""showDetailsInModules"" href=""{0}"" onclick=""javascript:__doPostBack('{1}','{2}');"">", group.RewriteUrl, dummyLinkButtonName, group.ID);
        //                //if (!group.IsRoot())
        //                //{
        //                html = itemTemplate.Replace("{DrillDownLink}", drillDownLink)
        //                    .Replace("{Name}", group.Name)
        //                    .Replace("{Title}", group.Title)
        //                    .Replace("{/DrillDownLink}", "</a>") + separatorTemplate + html;
        //                //}
        //                //else
        //                //{
        //                //    html = rootTemplate.Replace("{DrillDownLink}", drillDownLink)
        //                //        .Replace("{Name}", group.Name)
        //                //        .Replace("{Title}", group.Title)
        //                //        .Replace("{/DrillDownLink}", "</a>") + separatorTemplate + html;
        //                //}
        //            }
        //            html = GetBreadCrumbRoot() + html;

        //        }
        //        else
        //        {
        //            group.RewriteUrl = group.GetRewriteUrl(navigationUrl);
        //            html = String.Format("<a href='{0}'>{1}</a>", group.RewriteUrl, group.Name);
        //            while (group.ParentGroup != null)
        //            {
        //                group = group.ParentGroup;
        //                group.RewriteUrl = group.GetRewriteUrl(navigationUrl);
        //                html = String.Format("<a href='{0}'>{1}</a>", group.RewriteUrl, group.Name) + " --> " + html;
        //            }
        //        }
        //        breadCrumbLiteral.Text = html;
        //    }
        //}


        //    private string GetBreadCrumbRoot()
        //    {
        //        string html = "";
        //        ModuleNavigationActionLite navigationAction = GetNavigationActionByName("{DrillDownLink}");
        //        string navigationUrl = navigationAction.NavigationUrl;
        //        DataGroup dummyGroup = new DataGroup();
        //        dummyGroup.Name = "Root";
        //        dummyGroup.Title = "Root";
        //        if (navigationAction.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
        //        {
        //            //blijven op dezelfde pagina
        //            navigationUrl = Request.Url.LocalPath;
        //            dummyGroup.RewriteUrl = dummyGroup.GetRewriteUrl(navigationUrl);

        //            string dummyLinkButtonName = this.Controls[3].ClientID; //Linkbutton is altijd 3de element todo: deze code kan beter
        //            dummyLinkButtonName = dummyLinkButtonName.Replace("_", "$");
        //            string drillDownLink = String.Format(@"<a class=""showDetailsInModules"" href=""{0}"" onclick=""javascript:__doPostBack('{1}','{2}');"">", dummyGroup.RewriteUrl, dummyLinkButtonName, dummyGroup.ID);

        //            html = rootTemplate.Replace("{DrillDownLink}", drillDownLink)
        //                .Replace("{Name}", dummyGroup.Name)
        //                .Replace("{Title}", dummyGroup.Title)
        //                .Replace("{/DrillDownLink}", "</a>");


        //        }
        //        else
        //        {
        //            string drillDownLink = String.Format(@"<a href=""{0}"">", navigationUrl);

        //            html = rootTemplate.Replace("{DrillDownLink}", drillDownLink)
        //                 .Replace("{Name}", dummyGroup.Name)
        //                 .Replace("{Title}", dummyGroup.Title)
        //                 .Replace("{/DrillDownLink}", "</a>");

        //        }
        //        return html;
        //    }
    }





}
