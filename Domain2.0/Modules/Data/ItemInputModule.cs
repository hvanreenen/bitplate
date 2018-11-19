using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Newsletters;
using BitPlate.Domain.Utils;
using HJORM;
using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BitPlate.Domain.Modules.Data
{
    [Persistent("Module")]
    public class ItemInputModule : BasePostableDataModule, IPostableModule, IRefreshableModule
    {
        [NonPersistent()]
        public string ItemTitle { get; set; }
        [NonPersistent()]
        public string ItemMetaDescription { get; set; }
        [NonPersistent()]
        public string ItemMetaKeywords { get; set; }

        public ItemInputModule()
            : base()
        {
            ContentSamples.Add(@"
<div><label>Naam: </label>{Naam}</div>
<div><label>Title: </label>{Titel}</div>");

            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Data" });
            //ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "DataInputConfig", IsExternal = true, Url = "/_bitplate/EditPage/Modules/DataModules/InputModuleTab.aspx" });
        }

        public override void SetAllTags()
        {
            base.SetAllTags();
            this._tags.Add(new Tag() { Name = "{SubmitButton}", HasCloseTag = true, ReplaceValue = "<input type=\"submit\"" });
            this._tags.Add(new Tag() { Name = "{SubmitLink}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{SuccessTemplate}", HasCloseTag = true });
            this._tags.AddRange(this.GetInputDataFieldTags());
        }

        protected override void setNavigationActions()
        {
            base.setNavigationActions();
            if (this.NavigationActions.Count == 0)
            {
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    Name = "{SubmitButton}",
                    NavigationType = NavigationTypeEnum.NavigateToPage
                });
            }
        }

        public override string Publish2(CmsPage page)
        {
            string html = "";
            try
            {
                if (DataCollection != null) base.PrepairTemplate(DataCollection.DataItemFields, DataCollection.DataGroupFields);
                html = base.Publish2(page);
                if (DataCollection == null) return html;
                string tableAlias = "I";

                //kijk of dataID van querystring komt, als die van Reload() komt is die al gevuld, dan hoeft niks
                if (dataId == Guid.Empty)
                {
                    string dataIdQuery = HttpContext.Current.Request.QueryString["dataid"];
                    Guid.TryParse(dataIdQuery, out dataId);
                }

                string where = getWhere(tableAlias);

                System.Data.DataRow dataRow = getDataItemRow(where); //geeft lege rij bij geen data gevonden

                if (dataRow["ID"] == DBNull.Value)
                {
                    //geen rij gevonden: kijken of er initiele data moet worden getoond
                    Guid initialDataObjectID = base.getSetting<Guid>("DefaultDataObjectID");
                    if (initialDataObjectID != Guid.Empty)
                    {
                        where = "";

                        if (initialDataObjectID.ToString() == "11111111-1111-1111-1111-111111111111")
                        {
                            bool showInactive = getSetting<bool>("ShowInactive");
                            //eerste item tonen 
                            if (dataId != Guid.Empty)
                            {
                                //eerste rij uit subgroep
                                //todo: sortering toevoegen bij eerste item tonen
                                where = tableAlias + ".FK_Parent_Group = '" + dataId.ToString() + "'";
                            }
                            else
                            {
                                //eerste rij van datacollectie (zonder parentgroep)
                                //todo: sortering toevoegen bij eerste item tonen
                                where = tableAlias + ".FK_Parent_Group Is Null";
                            }
                            if (!showInactive)
                            {
                                where += String.Format(" AND ({1}.Active = 1 OR ({1}.Active = 2 AND IFNULL({1}.DateFrom, '2000-1-1') <= '{0:yyyy-MM-dd} 00:00:00' AND IFNULL({1}.DateTill, '2999-1-1') >= '{0:yyyy-MM-dd}'))", DateTime.Now, tableAlias);
                            }
                        }
                        else
                        {
                            where = tableAlias + ".ID = '" + initialDataObjectID.ToString() + "'";
                        }
                        dataRow = getDataItemRow(where);
                    }

                }

                html = html.Replace("{SubmitButton}", String.Format(@"<input name=""dataid"" type=""hidden"" value=""{1:N}"" /><button type=""submit"" ID=""button{0:N}"">", ID, this.dataId))
                     .Replace("{/SubmitButton}", "</button>")
                     .Replace("{SubmitLink}", String.Format(@"<a href=""javascript:void(0)"" onclick=""BITSITESCRIPT.submitPostableModule('form{0}');"">", ID))
                     .Replace("{/SubmitLink}", @"</a>");

                //Vervang hier de invoerveld tags.
                foreach (Tag tag in this.GetAllTags())
                {
                    if (tag.Name.Contains("{DropDown"))
                    {
                        string dropdown = FillLookUpDropDown(tag);
                        html = html.Replace(tag.Name, dropdown);
                    }
                    else if (tag.Name.Contains("{CheckboxList"))
                    {
                        string checkboxlist = FillCheckboxList(tag);
                        html = html.Replace(tag.Name, checkboxlist);
                    }
                    else
                    {
                        html = html.Replace(tag.Name, tag.ReplaceValue);
                        if (tag.HasCloseTag) html = html.Replace(tag.CloseTag, tag.ReplaceValueCloseTag);
                    }
                }

                //vervang de tags dmv velden uit columns
                if ((WebSessionHelper.CurrentSiteUser != null && WebSessionHelper.CurrentSiteUser.ID.ToString() == dataRow["FK_User"].ToString()) ||
                    (dataRow["FK_User"].ToString() == "" && dataRow["FK_BitplateUser"].ToString() == "" && WebSessionHelper.CurrentBitplateUser != null) ||
                    (WebSessionHelper.CurrentBitplateUser != null && WebSessionHelper.CurrentBitplateUser.ID.ToString() == dataRow["FK_BitplateUser"].ToString()) ||
                    (dataId == Guid.Empty && this.getSetting<bool>("AllowNewItemsFromUnkownUsers")))
                {
                    foreach (System.Data.DataColumn column in dataRow.Table.Columns)
                    {
                        html = Regex.Replace(html, "{" + column.ColumnName + "}", dataRow[column.ColumnName].ToString());
                    }
                }
                else
                {
                    //11111111-1111-1111-1111-111111111111
                    html = ""; //"U heeft geen rechten op het geselecteerde dataitem.";
                }
                

                if (html.Contains("{FileList1}"))
                {
                    html = fillExtraFilesSubTemplate(html, dataRow["ID"].ToString(), "");
                }
                if (html.Contains("{ImageList1}"))
                {
                    html = fillExtraImagesSubTemplate(html, dataRow["ID"].ToString(), "");
                }

                //zet titel en meta-tags: worden gebruikt in Page voor SEO
                ItemTitle = dataRow["Title"].ToString();
                ItemMetaDescription = dataRow.Table.Columns.Contains("MetaDescription") ? dataRow["MetaDescription"].ToString() : "";
                ItemMetaKeywords = dataRow.Table.Columns.Contains("MetaKeywords") ? dataRow["MetaKeywords"].ToString() : "";

                //foreach (DataField df in this.DataCollection.DataItemFields.Where(c => c.FieldType == FieldTypeEnum.CheckboxList))
                //{
                //    fillLookupSubTemplate(html, df);
                //}
                //SELECT datalookupvalue.* FROM datalookupvalue JOIN datalookupvalueperitem ON datalookupvalueperitem.FK_LookupValue = datalookupvalue.ID WHERE FK_Item = '7b4aafef-2693-480f-80c9-2e0c6f3cd5ea' 
                
                foreach (DataField datafield in this.DataCollection.DataItemFields)
                {
                    if (datafield.FieldType == FieldTypeEnum.CheckboxList)
                    {
                        html = fillCheckboxListSubTemplate(html, datafield, dataRow["ID"].ToString());
                    }
                }

                bool hideWhenNoData = getSetting<bool>("HideWhenNoData");
                if (hideWhenNoData && dataRow["ID"] == DBNull.Value)
                {
                    html = getModuleStartDiv() + getModuleEndDiv();
                }
            }
            catch (Exception ex)
            {
                html = base.HandlePublishError(ex);
            }
            return html;
        }

        private string FillLookUpDropDown(Tag tag)
        {
            string options = "";
            string fieldName = tag.Name.Replace("{DropDown", "").Replace("}", "");
            string sql = "SELECT datalookupvalue.* FROM `datalookupvalue` JOIN datafield ON datalookupvalue.FK_DataField = datafield.ID WHERE datafield.Name = '" + fieldName + "' AND datafield.FK_DataCollection = '" + this.DataCollection.ID.ToString() + "'";
            BaseCollection<DataLookupValue> lookUpValues = BaseCollection<DataLookupValue>.LoadFromSql(sql);
            foreach (DataLookupValue lookUpValue in lookUpValues)
            {
                options += "<option value=\"" + lookUpValue.ID + "\">" + lookUpValue.Name + "</option>";
            }
            return tag.ReplaceValue.Replace("{options}", options);
        }

        private string FillCheckboxList(Tag tag)
        {
            string checkboxes = "";
            string fieldName = tag.Name.Replace("{CheckboxList", "").Replace("}", "");
            string sql = "SELECT datalookupvalue.* FROM `datalookupvalue` JOIN datafield ON datalookupvalue.FK_DataField = datafield.ID WHERE datafield.Name = '" + fieldName + "' AND datafield.FK_DataCollection = '" + this.DataCollection.ID.ToString() + "'";
            BaseCollection<DataLookupValue> lookUpValues = BaseCollection<DataLookupValue>.LoadFromSql(sql);
            foreach (DataLookupValue lookUpValue in lookUpValues)
            {
                checkboxes += tag.ReplaceValue + "<br />";
                checkboxes = checkboxes.Replace("{ElementID}", lookUpValue.ID.ToString("N")).Replace("{ElementValue}", lookUpValue.Name);
            }
            return checkboxes;
        }

        private string fillCheckboxListSubTemplate(string html, DataField datafield, string dataid)
        {
            string fieldTemplate = "";
            string fieldResult = "";
            DataTable lookupValueTable = getSelectedLookupValuesByDataField(dataid);
            if (lookupValueTable.Rows.Count > 0)
            {
                fieldTemplate = Regex.Match(html, "{" + datafield.Name + "}(.*?){/" + datafield.Name + "}", RegexOptions.Singleline).ToString();
                if (fieldTemplate != "")
                {
                    foreach (DataRow lookupRow in lookupValueTable.Rows)
                    {
                        string rowTemplate = fieldTemplate;
                        rowTemplate = rowTemplate.Replace("{" + datafield.Name + "}", "")
                            .Replace("{/" + datafield.Name + "}", "")
                            .Replace("{" + datafield.Name + ".Value}", lookupRow["Name"].ToString());
                        fieldResult += rowTemplate;
                    }
                    html = html.Replace(fieldTemplate, fieldResult);
                }
            }
            else
            {
                html = html.Replace("{" + datafield.Name + "}", "")
                    .Replace("{/" + datafield.Name + "}", "")
                    .Replace("{" + datafield.Name + ".Value}", "");
            }
            //this._tags.Where(c => c.Name == "{" + lookupValues["DataFieldName"].ToString() + "}");
            return html;
        }


        public DataTable getSelectedLookupValuesByDataField(string dataid)
        {
            string sql = @"SELECT datalookupvalue.Name FROM dataitem JOIN datalookupvalueperitem ON datalookupvalueperitem.FK_Item = dataitem.ID
                        JOIN datalookupvalue ON datalookupvalue.ID = datalookupvalueperitem.FK_LookupValue
                        WHERE dataitem.ID = '" + dataid + "'";
            //string sql = "SELECT DISTINCT datafield.Name AS DataFieldName, datalookupvalue.* FROM datalookupvalue JOIN datalookupvalueperitem ON datalookupvalueperitem.FK_LookupValue = datalookupvalue.ID JOIN datafield ON datafield.ID = datalookupvalue.FK_DataField WHERE datafield.ID = '" + datafield.ID.ToString() + "'";
            return DataBase.Get().GetDataTable(sql);
        }

        protected override string getWhere(string tableAlias)
        {

            string whereSql = "";

            bool hasFixedData = base.getSetting<bool>("HasFixedData");
            if (hasFixedData)
            {
                Guid fixedDataObjectID = base.getSetting<Guid>("FixedDataObjectID");
                whereSql = tableAlias + ".ID = '" + fixedDataObjectID.ToString() + "'";
            }
            else
            {
                whereSql = tableAlias + ".ID = '" + dataId.ToString() + "'";
            }
            return whereSql;
        }

        public string Reload(CmsPage page, Dictionary<string, object> Parameters)
        {
            if (Parameters != null && Parameters.ContainsKey("dataid"))
            {
                this.dataId = new Guid(Parameters["dataid"].ToString());
            }
            return Publish2(page);
        }



        //internal string Publish2(out string title, out string metaDescription, out string metaKeywords)
        //{
        //    string html = Publish2();
        //    title = this.ItemTitle;
        //    metaDescription = this.ItemMetaDescription;
        //    metaKeywords = this.ItemMetaKeywords;
        //    return html;
        //}


        public override PostResult HandlePost(CmsPage page, Dictionary<string, string> FormParameters)
        {
            this.dataId = (FormParameters.ContainsKey("dataid")) ? Guid.Parse(FormParameters["dataid"]) : Guid.NewGuid();
            PostResult result = new PostResult();
            string messageLabel = "<ul>";
            bool itemValid = true;
            DataItem item;
            if (dataId == Guid.Empty)
            {
                item = new DataItem();
            }
            else
            {
                item = BaseObject.GetById<DataItem>(dataId);
            }
            Type dataFieldType = typeof(DataItem);
            foreach (DataField dataField in this.DataCollection.DataItemFields)
            {
                if (FormParameters.ContainsKey(dataField.Name))
                {
                    string value = FormParameters[dataField.Name];
                    if (value == "")
                    {
                        value = dataField.DefaultValue;
                    }

                    if (dataField.FieldRule == FieldRuleEnum.Required)
                    {
                        if (value == "")
                        {
                            messageLabel += "<li>" + dataField.Name + "</li>";
                            itemValid = false;
                        }
                    }

                    if (dataField.FieldRule == FieldRuleEnum.ReadOnly)
                    {
                    }

                    switch (dataField.FieldType)
                    {
                        case FieldTypeEnum.DropDown:
                            DataLookupValue lookUpValue = BaseObject.GetById<DataLookupValue>(Guid.Parse(value));
                            dataFieldType.GetProperty(dataField.MappingColumn).SetValue(item, lookUpValue, null);
                            break;

                        case FieldTypeEnum.CheckboxList:
                            string[] selectedLookUpIDs = value.Split(',');
                            foreach (string selectedLookUpID in selectedLookUpIDs)
                            {
                                DataLookupValue lookUpValue1 = BaseObject.GetById<DataLookupValue>(Guid.Parse(selectedLookUpID));
                                item.LookupValues1.Add(lookUpValue1);
                            }
                            break;

                        default:
                            dataFieldType.GetProperty(dataField.MappingColumn).SetValue(item, value, null);
                            break;
                    }
                }
            }

            if (itemValid)
            {
                if (WebSessionHelper.CurrentBitplateUser != null)
                {
                    item.BitplateUser = WebSessionHelper.CurrentBitplateUser;
                }

                if (WebSessionHelper.CurrentSiteUser != null)
                {
                    item.User = WebSessionHelper.CurrentSiteUser;
                }

                item.DataCollection = this.DataCollection;
                Guid parentGroupId;
                Guid.TryParse(this.getSetting<string>("SelectGroupID"), out parentGroupId);
                string strParentGroupID = "";
                if (parentGroupId != Guid.Empty && item.ParentGroup == null) {
                    item.ParentGroup = BaseObject.GetById<DataGroup>(parentGroupId);
                    strParentGroupID = parentGroupId.ToString();
                }
                DataItem checkItemNameExist = BaseObject.GetFirst<DataItem>("Name = '" + item.Name + "' AND FK_Parent_Group = '" + strParentGroupID + "'");
                if (checkItemNameExist == null || checkItemNameExist != null && checkItemNameExist.ID == this.dataId)
                {
                    bool isNewItemActive = false;

                    if (item.BitplateUser != null || item.User != null)
                    {
                        isNewItemActive = this.getSetting<bool>("IsNewItemActiveKownUsers");
                    }
                    else
                    {
                        isNewItemActive = this.getSetting<bool>("IsNewItemActiveUnkownUsers");
                    }

                        //this.getSetting<bool>("IsNewItemActive");
                    item.Active = (isNewItemActive) ? ActiveEnum.Active : ActiveEnum.InActive;
                    item.Save();
                }
                else
                {
                    itemValid = false;
                    messageLabel = "<ul><li>Er bestaat al een item met deze naam in de groep.<li>";
                }
                
            }
            ModuleNavigationAction navigationActionDrillDown = this.GetNavigationActionByTagName("{SubmitButton}");
            result.ErrorMessage = messageLabel + "</ul>";
            result.Success = itemValid;
            result.NavigationType = navigationActionDrillDown.NavigationType;
            if (navigationActionDrillDown.NavigationType == NavigationTypeEnum.NavigateToPage)
            {
                result.NavigationUrl = navigationActionDrillDown.NavigationPage.GetRewriteUrl();
            }
            else
            {
                result.HtmlResult += Regex.Match(this.Content, "{SuccessTemplate}(.*?){/SuccessTemplate}", RegexOptions.Singleline).ToString().Replace("{SuccessTemplate}", "").Replace("{/SuccessTemplate}", "");
            }

            return result;
        }
    }
}
