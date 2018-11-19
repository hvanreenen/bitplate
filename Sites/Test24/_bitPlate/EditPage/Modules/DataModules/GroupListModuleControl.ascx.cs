using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain.Modules;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Modules.Data;
using System.Data;

namespace BitSite._bitPlate.EditPage.Modules.DataModules
{
    public partial class GroupListModuleControl : BaseDataModuleUserControl
    {
        protected Repeater repeater;
        protected Repeater listRepeater;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Page_Load Start", "GroupListModuleControl");
            base.Load(sender, e);
            if (base.CheckAutorisation())
            {
                LabelMsg.Text = "";
                this.ParentGroup = null;
                if (DataCollectionID == null || DataCollectionID == Guid.Empty)
                {
                    LabelMsg.Text = "Kies eerst een datacollectie";
                }
                else
                {
                    if (!IsPostBack)
                    {
                        Guid dataId = Guid.Empty;
                        ShowDataEnum showDataBy = base.getSetting<ShowDataEnum>("ShowDataBy");
                        if (showDataBy == ShowDataEnum.UserSelect)
                        {
                            string dataIdQuery = Request.QueryString["dataid"];
                            Guid.TryParse(dataIdQuery, out dataId);
                        }
                        SelectAndShowData(dataId);
                    }
                }
            }
            System.Diagnostics.Trace.WriteLine("Page_Load End", "GroupListModuleControl");
        }

        public void DataBind(BaseCollection<DataGroup> groups)
        {
            SetRewriteUrls(groups);

            //if (groups.Count > 0 && this.ParentGroup == null)
            //{
            //    this.ParentGroup = groups[0].ParentGroup;
            //}
            //if (groups.Count > 0 && this.ParentParentGroup == null)
            //{
            //    if (groups[0].ParentGroup != null)
            //    {
            //        this.ParentParentGroup = groups[0].ParentGroup.ParentGroup;
            //    }
            //}
            ////Null waarde kan niet (NullRefenceException)
            //if (this.ParentGroup == null) this.ParentGroup = new DataGroup();
            //if (this.ParentParentGroup == null) this.ParentParentGroup = new DataGroup();

            repeater = (Repeater)FindControl("Repeater" + ModuleID.ToString("N"));
            if (repeater != null)
            {
                repeater.DataSource = groups;
                repeater.ItemDataBound += repeater_ItemDataBound; //Trigger list repeater.
                repeater.DataBind();
            }
            else
            {
                LabelMsg.Text = "Geen repeater gevonden.<br />";
            }
        }

        private void SetRewriteUrls(BaseCollection<DataGroup> groups)
        {
            ModuleNavigationActionLite navigationAction = GetNavigationActionByTagName("{DrillDownLink}");
            if (navigationAction != null)
            {
                string navigationUrl = navigationAction.NavigationUrl;
                if (navigationAction.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
                {
                    //blijven op dezelfde pagina
                    navigationUrl = Request.Url.LocalPath;
                }
                //set rewriteurl per groep
                foreach (DataGroup group in groups)
                {
                    group.RewriteUrl = group.GetRewriteUrl(navigationUrl);
                }
                this.ParentGroup.RewriteUrl = ParentGroup.GetRewriteUrl(navigationUrl);
                this.ParentParentGroup.RewriteUrl = ParentParentGroup.GetRewriteUrl(navigationUrl);
            }
        }



        private void repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;
            if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
            {
                string controlId = "RepeaterImageList1" + ModuleID.ToString("N");
                listRepeater = (Repeater)item.FindControl(controlId);
                if (listRepeater != null)
                {
                    DataGroup dg = (DataGroup)item.DataItem;
                    listRepeater.DataSource = dg.ImageList1;
                    listRepeater.DataBind();
                }
            }
        }
        //private BaseCollection<DataGroup> getGroups()
        //{
        //    return getGroups(Guid.Empty);
        //}

        private BaseCollection<DataGroup> getGroups(Guid dataId)
        {
            if (DataCollectionID == null)
            {
                return new BaseCollection<DataGroup>();
            }
            string where = base.getWhere();
            ShowDataEnum showDataBy = base.getSetting<ShowDataEnum>("ShowDataBy");
            string selectGroupID = getSetting<string>("SelectGroupID");


            if (showDataBy == ShowDataEnum.MainGroups)
            {
                where += " AND FK_Parent_Group is null";
            }
            else if (showDataBy == ShowDataEnum.UserSelect)
            {
                //als vaste groep is gekozen dan deze nemen
                if (selectGroupID != null && selectGroupID != "")
                {
                    where += " AND FK_Parent_Group = '" + selectGroupID + "'";
                }
                else if (dataId == Guid.Empty)
                {
                    where += " AND FK_Parent_Group is null";
                }
                else
                {
                    where += String.Format(" AND FK_Parent_Group = '{0}'", dataId);
                }
            }

            string sort = getSort();
            int maxNumberOfRows = getSetting<int>("MaxNumberOfRows");
            int showFromRowNumber = getSetting<int>("ShowFromRowNumber");

            BaseCollection<DataGroup> groups;
            if (maxNumberOfRows > 0 || showFromRowNumber > 1)
            {
                groups = BaseCollection<DataGroup>.Get(where, sort, 0, maxNumberOfRows, showFromRowNumber);
            }
            else
            {
                groups = BaseCollection<DataGroup>.Get(where, sort, 0, 0, 0);
            }

            return groups;
        }

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

        public override void SelectAndShowData(Guid dataId)
        {
            BaseCollection<DataGroup> groups = getGroups(dataId);
            this.ParentGroup = BaseObject.GetById<DataGroup>(dataId);
            //controlleer of datacollectie klopt, zo nee parentgroup weer leegmaken
            if (this.ParentGroup != null && ParentGroup.DataCollection.ID != DataCollectionID)
            {
                ParentGroup = null;
            }
            if (ParentGroup != null)
            {
                this.ParentParentGroup = ParentGroup.ParentGroup;
            }

            //Null waarde kan niet (NullRefenceException)
            if (this.ParentGroup == null) this.ParentGroup = new DataGroup();
            if (this.ParentParentGroup == null)
            {
                this.ParentParentGroup = new DataGroup();
                this.ParentParentGroup.Name = "Root";
            }
            DataBind(groups);
        }

        protected void DrillDownLink_Click(Object sender, EventArgs args)
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
                LinkButton linkButton = (LinkButton)sender;
                //Guid.TryParse(link.CommandArgument, out dataId);

                foreach (string drillDownModuleId in action.RefreshModules)
                {
                    BaseModuleUserControl moduleControl = (BaseModuleUserControl)FindControlRecursive(this.Page.Master, "Mod" + drillDownModuleId.Replace("-", ""));
                    if (moduleControl != null)
                    {
                        moduleControl.Reload(this, new NavigationParameterObject() { Name = "dataid", GuidValue = dataId }); //{  //SelectAndShowData(dataId);
                    }
                }
                //stuur eventueel javascript naar de browser zodat deze wordt uitgevoerd nadat pagina opnieuw is gerenderd
                string js = linkButton.OnClientClick;
                if (js != null && js != "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "navigation" + this.ModuleID.ToString("N"), js, true);
                }
            }             
        }

        protected void DrillUpLink_Click(Object sender, EventArgs args)
        {
            ModuleNavigationActionLite action = GetNavigationActionByTagName("{DrillUpLink}");
            if (action.NavigationType == NavigationTypeEnum.NavigateToPage)
            {
                Response.Redirect(action.NavigationUrl);
            }
            else if (action.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {
                //click van linkbutton wordt via javascript:__doPostback() gedaan
                //de link is dus geen echte linkbutton
                //dataid staat in eventArgument van deze postbackfunctie
                Guid dataId;
                string eventArgument = Request.Params.Get("__EVENTARGUMENT");
                Guid.TryParse(eventArgument, out dataId);
                LinkButton linkButton = (LinkButton)sender;

                foreach (string drillDownModuleId in action.RefreshModules)
                {
                    BaseModuleUserControl moduleControl = (BaseModuleUserControl)FindControlRecursive(this.Page.Master, "Mod" + drillDownModuleId.Replace("-", ""));
                    if (moduleControl != null)
                    {
                        moduleControl.Reload(this, new NavigationParameterObject() { Name = "dataid", GuidValue = dataId }); //{  //SelectAndShowData(dataId);
                    }
                }

                //stuur eventueel javascript naar de browser zodat deze wordt uitgevoerd nadat pagina opnieuw is gerenderd
                string js = linkButton.OnClientClick;
                if (js != null && js != "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "navigation" + this.ModuleID.ToString("N"), js, true);
                }
            }
        }
    }
}