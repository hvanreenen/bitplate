using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using HJORM;
using BitPlate.Domain.Modules;
using BitPlate.Domain.DataCollections;
using System.Reflection;
namespace BitSite._bitPlate.EditPage.Modules.DataModules
{
    public partial class ItemDetailsModuleControl : BaseDataModuleUserControl
    {
        Repeater formView;

        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Page_Load Start", "ItemDetailsModuleControl");
            try
            {
                base.Load(sender, e);
                if (base.CheckAutorisation())
                {
                    Guid dataId;
                    bool hasFixedData = base.getSetting<bool>("HasFixedData");
                    if (hasFixedData)
                    {
                        Guid fixedDataObjectID = base.getSetting<Guid>("FixedDataObjectID");

                        if (fixedDataObjectID != Guid.Empty)
                        {
                            DataItem item = BaseObject.GetById<DataItem>(fixedDataObjectID);
                            ShowData(item);
                        }
                    }
                    else
                    {
                        //kijk of dataID van gebruikersselectie komt
                        string dataIdQuery = Request.QueryString["dataid"];
                        Guid.TryParse(dataIdQuery, out dataId);
                        //kijk of id ook een item geeft binnen deze datacollectie komt
                        DataItem item = BaseObject.GetById<DataItem>(dataId);
                        if (item != null && item.DataCollection.ID == DataCollectionID)
                        {
                            ShowData(item);
                        }
                        else
                        {
                            //initiele item tonen
                            Guid initialDataObjectID = base.getSetting<Guid>("DefaultDataObjectID");
                            if (initialDataObjectID == Guid.Empty)
                            {
                                //bewust leeg
                                ShowData(null);
                            }
                            else if (initialDataObjectID.ToString() == "11111111-1111-1111-1111-111111111111")
                            {
                                //eerste item tonen
                                //eerste hoofd item selecteren
                                string where = String.Format("FK_DataCollection='{0}' AND FK_Parent_Group is null", DataCollectionID);
                                where += String.Format(" AND IFNULL(Active, 1) IN (1, 2) AND IFNULL(DateFrom, '2000-1-1') <= '{0:yyyy-MM-dd} 00:00:00' AND IFNULL(DateTill, '2999-1-1') >= '{0:yyyy-MM-dd}'", DateTime.Now);

                                item = BaseObject.GetFirst<DataItem>(where, "OrderNumber");
                                if (item == null)
                                {
                                    //nog steeds geen item dan eerste item van eerste groep selecteren
                                    where = String.Format("FK_DataCollection='{0}'", DataCollectionID);
                                    where += String.Format(" AND IFNULL(Active, 1) IN (1, 2) AND IFNULL(DateFrom, '2000-1-1') <= '{0:yyyy-MM-dd} 00:00:00' AND IFNULL(DateTill, '2999-1-1') >= '{0:yyyy-MM-dd}'", DateTime.Now);

                                    item = BaseObject.GetFirst<DataItem>(where, "OrderNumber");
                                }
                                ShowData(item);
                            }
                            else
                            {
                                item = BaseObject.GetById<DataItem>(initialDataObjectID);
                                ShowData(item);
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LabelMsg.Text = "Er is een fout in deze module.<br />" + ex.Message;
            }
            System.Diagnostics.Trace.WriteLine("Page_Load End", "ItemDetailsModuleControl");
        }

        public override void Reload(BaseModuleUserControl sender, NavigationParameterObject args = null)
        {
            if (base.CheckAutorisation())
            {
                if (args != null && args.Name == "dataItemID")
                {
                    this.SelectAndShowData(args.GuidValue);
                }
            }
        }

        public override void SelectAndShowData(Guid dataId)
        {
            DataItem item = BaseObject.GetById<DataItem>(dataId);
            if (item == null)
            {
                //als dataid naar groep verwijst dan eerste item actieve item uit groep halen
                DataGroup group = BaseObject.GetById<DataGroup>(dataId);
                if (group != null)
                {
                    string where = String.Format("FK_DataCollection='{0}' AND FK_Parent_Group='{1}'", DataCollectionID, dataId);
                    where += String.Format(" AND IFNULL(Active, 1) IN (1, 2) AND IFNULL(DateFrom, '2000-1-1') <= '{0:yyyy-MM-dd} 00:00:00' AND IFNULL(DateTill, '2999-1-1') >= '{0:yyyy-MM-dd}'", DateTime.Now);
                    item = BaseObject.GetFirst<DataItem>(where, "OrderNumber");
                }
            }
            ShowData(item);
        }

        private void ShowData(DataItem item)
        {
            formView = (Repeater)FindControl("Repeater" + ModuleID.ToString("N"));
            if (formView == null)
            {
                LabelMsg.Text = "Geen repeater gevonden.<br />";
            }
            if (item != null)
            {
                string pageLanguage = this.GetLanguageCodeFromMasterPage();
                if (pageLanguage != "" && pageLanguage != null)
                {
                    item.Translate(pageLanguage);
                }
                //voor tag: {DirectUrl}
                item.RewriteUrl = item.GetRewriteUrl(Request.Url.LocalPath);
                //formview moet aan list worden gekoppeld, vreemd genoeg
                var list = new List<DataItem> { item };
                formView.DataSource = list;
                formView.DataBind();
                overridePageTitleAndMetaTags(item);
                formView.Visible = true;
            }
            else
            {
                if (base.getSetting<bool>("HideWhenNoData"))
                {
                    formView.Visible = false;
                }
            }

        }

        //private void OverridePageTitleAndMetaTags(DataItem item)
        //{
        //    //als data vanuit user komt, title op pagina overschrijven met item titel
        //    if (Request.QueryString["dataid"] != null && item != null)
        //    {
        //        if (item.Title != "" && item.Title != null)
        //        {
        //            this.Page.Title = item.Title;
        //        }
        //        if (item.MetaDescription != "" && item.MetaDescription != null)
        //        {
        //            HtmlGenericControl metaDescriptionCtl = (HtmlGenericControl)FindControlRecursive(this.Page, "MetaDescription");
        //            if (metaDescriptionCtl != null)
        //            {
        //                metaDescriptionCtl.Attributes["Content"] = item.MetaDescription;
        //            }
        //        }
        //        if (item.MetaKeywords != "" && item.MetaKeywords != null)
        //        {
        //            HtmlGenericControl metaKeywordsCtl = (HtmlGenericControl)FindControlRecursive(this.Page, "MetaKeywords");
        //            if (metaKeywordsCtl != null)
        //            {
        //                metaKeywordsCtl.Attributes["Content"] = item.MetaKeywords;
        //            }
        //        }
        //    }
        //}
    }
}