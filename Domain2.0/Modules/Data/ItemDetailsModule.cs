using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM.Attributes;
using BitPlate.Domain.Utils;
using HJORM;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;
using BitPlate.Domain.DataCollections;
using System.Collections.Specialized;

namespace BitPlate.Domain.Modules.Data
{
    [Persistent("Module")]
    public class ItemDetailsModule : BaseDataModule, IRefreshableModule
    {
        [NonPersistent()]
        public string ItemTitle { get; set; }
        [NonPersistent()]
        public string ItemMetaDescription { get; set; }
        [NonPersistent()]
        public string ItemMetaKeywords { get; set; }

        public ItemDetailsModule()
            : base()
        {
            ContentSamples.Add(@"
<div><label>Naam: </label>{Naam}</div>
<div><label>Title: </label>{Titel}</div>");

            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Data" });
        }

        public override void SetAllTags()
        {
            this._tags.AddRange(this.GetDataFieldTags());
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
                            //Begin in groep
                            if (dataId == null || dataId == Guid.Empty)
                            {
                                dataId = getSetting<Guid>("SelectGroupID");
                            }
                            
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

                if (html.Contains("{FileList1}"))
                {
                    html = fillExtraFilesSubTemplate(html, dataRow["ID"].ToString(), "");
                }
                if (html.Contains("{ImageList1}"))
                {
                    html = fillExtraImagesSubTemplate(html, dataRow["ID"].ToString(), "");
                }

                //vervang de tags dmv velden uit columns
                foreach (System.Data.DataColumn column in dataRow.Table.Columns)
                {
                    html = ReplaceTag(html, dataRow, column);
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




    }
}
