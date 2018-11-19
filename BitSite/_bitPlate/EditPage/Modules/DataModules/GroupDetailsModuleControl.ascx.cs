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
    public partial class GroupDetailsModuleControl : BaseDataModuleUserControl
    {
        Repeater formView;
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Page_Load Start", "GroupDetailsModuleControl");
            base.Load(sender, e);
            if (CheckAutorisation())
            {
                Guid dataId;
                bool hasFixedData = base.getSetting<bool>("HasFixedData");
                if (hasFixedData)
                {
                    Guid fixedDataObjectID = base.getSetting<Guid>("FixedDataObjectID");

                    if (fixedDataObjectID != Guid.Empty)
                    {
                        DataGroup group = BaseObject.GetById<DataGroup>(fixedDataObjectID);
                        ShowData(group);
                    }
                }
                else
                {
                    //kijk of dataID van gebruikersselectie komt
                    string dataIdQuery = Request.QueryString["dataid"];
                    Guid.TryParse(dataIdQuery, out dataId);
                    //kijk of id ook een item geeft binnen deze datacollectie komt
                    DataGroup group = BaseObject.GetById<DataGroup>(dataId);
                    if (group != null && group.DataCollection.ID == DataCollectionID)
                    {
                        ShowData(group);
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
                            //eerste group ophalen
                            if (DataCollectionID != Guid.Empty) //Geen datacollectie geselecteerd.
                            {
                                string where = String.Format("FK_DataCollection='{0}' AND FK_Parent_Group is null", DataCollectionID);
                                group = BaseObject.GetFirst<DataGroup>(where);
                                ShowData(group);
                            }
                            ShowData(group);
                        }
                        else
                        {
                            group = BaseObject.GetById<DataGroup>(initialDataObjectID);
                            ShowData(group);
                        }

                    }

                }
                //Guid dataId;
                //Guid defaultDataObjectID = base.getSetting<Guid>("DefaultDataObjectID");

                //if (defaultDataObjectID != Guid.Empty)
                //{
                //    dataId = defaultDataObjectID;
                //}
                //else
                //{
                //    string dataIdQuery = Request.QueryString["dataid"];

                //    Guid.TryParse(dataIdQuery, out dataId);
                //}
                //if (dataId == Guid.Empty)
                //{
                //    //eerste group ophalen
                //    if (DataCollectionID != Guid.Empty) //Geen datacollectie geselecteerd.
                //    {
                //        string where = String.Format("FK_DataCollection='{0}' AND FK_Parent_Group is null", DataCollectionID);
                //        DataGroup group = BaseObject.GetFirst<DataGroup>(where);
                //        ShowData(group);
                //    }
                //} 
                //else if (dataId != Guid.Empty)
                //{
                //    SelectAndShowData(dataId);
                //}
            }
            System.Diagnostics.Trace.WriteLine("Page_Load End", "GroupDetailsModuleControl");
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
            DataGroup group = BaseObject.GetById<DataGroup>(dataId);
            if (group == null)
            {
                //als dataId niet naar een groep verwijst, kan het naar een item verwijzen.
                DataItem item = BaseObject.GetById<DataItem>(dataId);
                //dan group van item laden
                if (item != null)
                {
                    group = item.ParentGroup;
                }
            }
            ShowData(group);
        }

        private void ShowData(DataGroup group)
        {
            formView = (Repeater)FindControl("Repeater" + ModuleID.ToString("N"));
            if (formView == null)
            {
                LabelMsg.Text = "Geen repeater gevonden.<br />";
            }
            if (group != null)
            {
                string pageLanguage = this.GetLanguageCodeFromMasterPage();
                if (pageLanguage != "" && pageLanguage != null)
                {
                    group.Translate(pageLanguage);
                }
                //voor tag: {DirectUrl}
                group.RewriteUrl = group.GetRewriteUrl(Request.Url.LocalPath);
                //formview moet aan list worden gekoppeld, vreemd genoeg
                var list = new List<DataGroup> { group };
                formView.DataSource = list;
                formView.DataBind();
                overridePageTitleAndMetaTags(group);
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


    }
}