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
using System.Web.UI.HtmlControls;

namespace BitSite._bitPlate.EditPage.Modules.DataModules
{
    public partial class ItemListModuleControl : BaseDataModuleUserControl
    {
        protected Repeater repeater;
        protected Repeater listRepeater;
        public DataGroup ParentGroup;

        string where = "";
        bool usePaging = false;
        PlaceHolder placeHolderPaging = null;
        int pageSize = 0;
        int currentPage = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Page_Load Start", "ItemListModuleControl");
            base.Load(sender, e);

            if (base.CheckAutorisation())
            {
                LabelMsg.Text = "";
                if (DataCollectionID == null || DataCollectionID == Guid.Empty)
                {
                    LabelMsg.Text = "Kies eerst een datacollectie";
                }
                else
                {
                    pageSize = base.getSetting<int>("MaxNumberOfRows");
                    placeHolderPaging = (PlaceHolder)FindControl("PlaceHolderPaging" + ModuleID.ToString("N"));
                    usePaging = ((pageSize > 0) && (placeHolderPaging != null));

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
            System.Diagnostics.Trace.WriteLine("Page_Load End", "ItemListModuleControl");
        }

        public void DataBind(BaseCollection<DataItem> items)
        {
            repeater = (Repeater)FindControl("Repeater" + ModuleID.ToString("N"));
            if (repeater == null)
            {
                LabelMsg.Text = "Geen repeater gevonden.<br />";
                return;
            }
            if (items.Count > 0)
            {
                string pageLanguage = this.GetLanguageCodeFromMasterPage();
                string navigationUrl = Request.Url.LocalPath;
                ModuleNavigationActionLite navigationAction = GetNavigationActionByTagName("{DrillDownLink}");
                if (navigationAction != null && navigationAction.NavigationType == NavigationTypeEnum.NavigateToPage)
                {
                    navigationUrl = navigationAction.NavigationUrl;
                }

                //set rewriteurl per groep 
                foreach (DataItem item in items)
                {
                    item.RewriteUrl = item.GetRewriteUrl(navigationUrl);

                    if (pageLanguage != "" && pageLanguage != null)
                    {
                        item.Translate(pageLanguage);
                    }
                }


                //if (items.Count > 0 && this.ParentGroup == null)
                //{
                //    if (this.getSetting<int>("ShowDataBy") == 0)
                //    {
                //        this.ParentGroup = items[0].ParentGroup;
                //    }
                //}
                //Null waarde kan niet (NullRefenceException)
                //if (this.ParentGroup == null) this.ParentGroup = new DataGroup();
                //repeater = (Repeater)FindControl("Repeater" + ModuleID.ToString("N"));


                //repeater.DataSource = items;
                //repeater.ItemDataBound += repeater_ItemDataBound; //Trigger list repeater.
                //repeater.DataBind();
                //repeater.Visible = true;

            }
            else
            {
                if (base.getSetting<bool>("HideWhenNoData"))
                {
                    repeater.Visible = false;
                }
            }


            if (usePaging)
            {
                PagedDataSource pagedResults = new PagedDataSource();
                pagedResults.DataSource = items;
                pagedResults.AllowPaging = true;
                pagedResults.PageSize = pageSize;
                pagedResults.CurrentPageIndex = currentPage;


                repeater.DataSource = pagedResults;
            }
            else
            {
                repeater.DataSource = items;
            }

            
            repeater.ItemDataBound += repeater_ItemDataBound; //Trigger list repeater.
            repeater.DataBind();
            repeater.Visible = true;

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
                    DataItem dg = (DataItem)item.DataItem;
                    listRepeater.DataSource = dg.ImageList1;
                    listRepeater.DataBind();
                }
            }
        }
        //private BaseCollection<DataItem> getItems()
        //{
        //    Guid dataId = Guid.Empty;
        //    string dataIdQuery = Request.QueryString["dataid"];
        //    Guid.TryParse(dataIdQuery, out dataId);

        //    return getItems(dataId);
        //}

        private BaseCollection<DataItem> getItems(Guid dataId, string extraWhere = "")
        {
            if (DataCollectionID == null)
            {
                return new BaseCollection<DataItem>();
            }
            where = base.getWhere(extraWhere);
            ShowDataEnum showDataBy = base.getSetting<ShowDataEnum>("ShowDataBy");
            //als vaste groep is gekozen dan deze nemen
            string selectGroupID = getSetting<string>("SelectGroupID");
            if (selectGroupID != null && selectGroupID != "")
            {
                dataId = new Guid(selectGroupID);
            }

            if (showDataBy == ShowDataEnum.AllItems)
            {
                //doe niks
            }
            else if (showDataBy == ShowDataEnum.UserSelect && extraWhere == "")
            {
                if (dataId == Guid.Empty)
                {
                    //als geen user select: alle items uit root
                    where += " AND FK_Parent_Group is null";
                }
                else
                {
                    //dataid kan naar item verwijzen
                    DataItem item = BaseObject.GetById<DataItem>(dataId);
                    //dan group van item laden
                    if (item != null && item.ParentGroup != null)
                    {
                        dataId = item.ParentGroup.ID;
                    }
                    where += String.Format(" AND FK_Parent_Group = '{0}'", dataId);
                }
            }

            string sort = getSort();
            int maxNumberOfRows = getSetting<int>("MaxNumberOfRows");
            int showFromRowNumber = getSetting<int>("ShowFromRowNumber");

            BaseCollection<DataItem> items;

            if (maxNumberOfRows > 0 || showFromRowNumber > 1)
            {
                items = BaseCollection<DataItem>.Get(where, sort, 0, maxNumberOfRows, showFromRowNumber);
            }
            else
            {
                items = BaseCollection<DataItem>.Get(where, sort, 0, 0, 0);
            }

            return items;
        }

        public override void Reload(BaseModuleUserControl sender, NavigationParameterObject args = null)
        {
            if (base.CheckAutorisation())
            {
                if (args != null && args.Name == "dataid")
                {
                    this.SelectAndShowData(args.GuidValue);
                }

                if (args != null && args.Name == "filter")
                {
                    this.SelectAndShowData(args.GuidValue, args.StringValue);
                }
            }
        }

        public override void SelectAndShowData(Guid dataId)
        {
            SelectAndShowData(dataId, "");
        }

        public int SelectAndShowData(Guid dataId, string extraWhere)
        {
            

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

            BaseCollection<DataItem> items = getItems(dataId, extraWhere);

            DataBind(items);

            return items.Count;
        }

        


        protected void DrillDownLink_Click(Object sender, EventArgs args)
        {
            SelectItemLink_Click(sender, args);
        }

        protected void SelectItemLink_Click(Object sender, EventArgs args)
        {
            //LinkButton link = (LinkButton)sender;
            //Guid dataId;
            //Guid.TryParse(link.CommandArgument, out dataId);
            //this.LoadModuleObject();
            //ModuleNavigationAction action = this.Module.GetNavigationActionByTagName("DrillDownLink");
            ModuleNavigationActionLite action = GetNavigationActionByTagName("{DrillDownLink}");

            if (action.NavigationType == NavigationTypeEnum.NavigateToPage)
            {
                //Response.Redirect(DrillDownPage.GetRewriteUrl());
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

                foreach (string drillDownModuleId in action.RefreshModules)
                {
                    BaseModuleUserControl moduleControl = (BaseModuleUserControl)FindControlRecursive(this.Page.Master, "Mod" + drillDownModuleId.Replace("-", ""));
                    if (moduleControl != null)
                    {
                        moduleControl.Reload(this, new NavigationParameterObject() { Name = "dataItemID", GuidValue = dataId }); //{  //SelectAndShowData(dataId);
                    }
                }


                string js = linkButton.OnClientClick;
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "navigation" + this.ModuleID.ToString("N"), js, true);
            }
        }

        protected void DrillUpLink_Click(Object sender, EventArgs args)
        {
            LinkButton link = (LinkButton)sender;
            Guid dataId;
            Guid.TryParse(link.CommandArgument, out dataId);

            ModuleNavigationActionLite action = GetNavigationActionByTagName("{DrillUpLink}");
            if (action.NavigationType == NavigationTypeEnum.NavigateToPage)
            {
                Response.Redirect(action.NavigationUrl);
            }
            else if (action.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {
                foreach (string drillDownModuleId in action.RefreshModules)
                {
                    BaseModuleUserControl moduleControl = (BaseModuleUserControl)FindControlRecursive(this.Page.Master, "Mod" + drillDownModuleId.Replace("-", ""));
                    if (moduleControl != null)
                    {
                        moduleControl.Reload(this, new NavigationParameterObject() { Name = "dataid", GuidValue = dataId }); //{  //SelectAndShowData(dataId);
                    }
                }
            }

            string js = link.OnClientClick;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "navigation" + this.ModuleID.ToString("N"), js, true);

        }

        public void CreatePager(int currentPage, int totalResults)
        {
            //placeHolderPaging.Controls.Clear(); regel verhuist naar beneden.
            if (placeHolderPaging != null)
            {
                placeHolderPaging.Controls.Clear();
                if (totalResults == 0 || pageSize == 0)
                {
                    placeHolderPaging.Visible = false;
                }
                else
                {
                    for (int i = 0; i <= (totalResults / pageSize); i++)
                    {
                        LinkButton lnk = new LinkButton();
                        lnk.Click += new EventHandler(PagerLinkButton_Click);
                        lnk.ID = "lnkPage" + (i + 1).ToString();
                        lnk.Text = (i + 1).ToString();
                        if (i == currentPage)
                        {
                            lnk.Font.Bold = true;
                        }
                        placeHolderPaging.Controls.Add(lnk);
                        Label spacer = new Label();
                        spacer.Text = "&nbsp;";
                        placeHolderPaging.Controls.Add(spacer);
                    }
                }

            }
        }


        void PagerLinkButton_Click(object sender, EventArgs e)
        {
            LinkButton lnk = sender as LinkButton;
            currentPage = int.Parse(lnk.Text) - 1;

            SelectAndShowData(Guid.Empty, where);
       
            CreatePager(currentPage, Convert.ToInt32(ViewState["TotalResults" + ModuleID.ToString("N")]));

        }

        
    }
}