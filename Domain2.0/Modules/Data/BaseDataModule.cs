using BitPlate.Domain.DataCollections;
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
using System.Xml.Serialization;

namespace BitPlate.Domain.Modules.Data
{
    public enum ShowDataEnum { UserSelect, MainGroups, SubGroups, AllGroups, AllItems, UserSelectNoDefaultData };
    public enum DataCollectionTagTypeEnum { All, Input, Output };

    [Persistent("Module")]
    public class BaseDataModule : BaseModule
    {
        protected Guid dataId; //drilldown
        public string dataType = ""; //I of G; Item of Groep

        protected override string getModuleStartDiv()
        {
            string moduleStartDiv = base.getModuleStartDiv();
            //moduleStartDiv += @"<asp:Label runat=""server"" CssClass=""bitModuleMessage"" id=""LabelMsg""></asp:Label>";
            return moduleStartDiv;
        }
        //private DataField _sortField;
        //[XmlIgnore()]
        //[Association("FK_SortDataField")]
        //public DataField SortField
        //{
        //    get
        //    {
        //        if (_sortField != null && !_sortField.IsLoaded)
        //        {
        //            _sortField.Load();
        //        }

        //        return _sortField;
        //    }
        //    set
        //    {
        //        _sortField = value;
        //    }
        //}
        //public string SortOrder { get; set; }

        //private DataField _selectField;
        //[XmlIgnore()]
        //[Association("FK_SelectDataField")]
        //public DataField SelectField
        //{
        //    get
        //    {
        //        if (_selectField != null && !_selectField.IsLoaded)
        //        {
        //            _selectField.Load();
        //        }

        //        return _selectField;
        //    }
        //    set
        //    {
        //        _selectField = value;
        //    }
        //}

        //public string SelectOperator { get; set; }
        //public string SelectValue { get; set; }
        //public string SelectGroup { get; set; }
        //[XmlIgnore]
        //[NonPersistent]
        //public string SelectGroupPath { get; set; }


        //public bool ShowInactive { get; set; }
        //public int MaxNumberOfRows { get; set; }
        //public int ShowFromRowNumber { get; set; }



        //[NonPersistent]
        //public DataCollectionTagTypeEnum TagType { get; set; }

        private List<Tag> _dataFieldTags = null;
        /// <summary>
        /// bij modules die aan een datacollectie hangen gelden er extra tags
        /// een datacollectie heeft flex velden
        /// al deze flexvelden dienen een tag te worden
        /// </summary>
        [NonPersistent()]
        public List<Tag> GetDataFieldTags()
        {
            if (_dataFieldTags == null)
            {
                _dataFieldTags = new List<Tag>();

                if (this.DataCollection != null)
                {
                    ShowDataEnum showDataBy = ShowDataEnum.UserSelect;
                    if (Settings != null && Settings.ContainsKey("ShowDataBy"))
                    {
                        showDataBy = (ShowDataEnum)Convert.ToInt32(Settings["ShowDataBy"]);
                    }

                    BaseCollection<DataField> fields;
                    if (this.Type.Contains("Group"))
                    {
                        fields = DataCollection.DataGroupFields;

                    }
                    else
                    {
                        fields = DataCollection.DataItemFields;
                    }

                    foreach (DataField fld in fields)
                    {
                        _dataFieldTags.AddRange(this.GetTagsByField(fld));
                        if (showDataBy != ShowDataEnum.MainGroups)
                        {
                            _dataFieldTags.AddRange(this.GetParentTagsByField(fld));
                            _dataFieldTags.AddRange(this.GetParentParentTagsByField(fld));
                        }
                    }
                }
            }
            return _dataFieldTags;
        }
        
        /// <summary>
        /// sommige velden hebben per veld meerdere tags. Dit zijn tags met Url (images, files)
        /// sommige types kijgen een format. Deze krijgen hier geen extra tag, maar pas tijdens de weergave in de popup
        /// </summary>
        /// <returns></returns>
        public List<Tag> GetTagsByField(DataField fld)
        {
            List<Tag> tagCollection = new List<Tag>();
            Tag tag = new Tag();
            tag.Name = "{" + fld.Name + "}";
            //tag.ReplaceValue = "<%#DataBinder.Eval(Container.DataItem,\"" + fld.MappingColumn + "\")%>";

            switch (fld.FieldType)
            {
                case FieldTypeEnum.Currency:
                    //tags zijn te gebruiken als {bedrag:C}
                    //C = format
                    //formats van numbers en dates worden gezet in publish van module
                    tag.AllowFormats = true;
                    tag.SampleFormats = new string[] { "C" }; //voor weergave in tags-popup
                    tagCollection.Add(tag);
                    break;
                case FieldTypeEnum.DateTime:
                    //tags zijn te gebruiken als {datum:dd-MM-yyyy}
                    //C = format
                    //formats van numbers en dates worden gezet in publish van module
                    tag.AllowFormats = true;
                    tag.SampleFormats = new string[] { "dd-MM-yyyy", "HH:mm:ss" };
                    tag.AvailableArguments = new string[] { "/format=dd-MM-yyyy" };
                    tagCollection.Add(tag);
                    break;
                case FieldTypeEnum.File:
                    tagCollection.Add(tag);

                    //extra tag voor plain Url
                    tag = new Tag();
                    tag.Name = "{" + fld.Name + "Url}";
                    tagCollection.Add(tag);

                    break;
                case FieldTypeEnum.FileList:
                    tag.HasCloseTag = true;
                    
                    tagCollection.Add(tag);

                    //extra tag voor plain Url
                    tag = new Tag();
                    tag.Name = "{" + fld.Name + "Url}";
                    tagCollection.Add(tag);
                    break;

                case FieldTypeEnum.Image:

                    tagCollection.Add(tag);
                    //extra tag voor plain Url
                    tag = new Tag();
                    tag.Name = "{" + fld.Name + "Url}";
                    tagCollection.Add(tag);

                    tag = new Tag();
                    tag.Name = "{" + fld.Name + "ThumbnailUrl}";
                    tagCollection.Add(tag);
                    break;
                case FieldTypeEnum.ImageList:

                    tag.HasCloseTag = true;
                    tagCollection.Add(tag);
                    //extra tag voor plain Url
                    tag = new Tag();
                    tag.Name = "{" + fld.Name + "Url}";
                    tagCollection.Add(tag);

                    tag = new Tag();
                    tag.Name = "{" + fld.Name + "ThumbnailUrl}";
                    tagCollection.Add(tag);
                    break;
                case FieldTypeEnum.Numeric:
                    tag.AllowFormats = true;
                    //tag.SampleFormats = new string[] {"N"}; //uitgezet, want Volgnummer is altijd geheel getal
                    tagCollection.Add(tag);
                    break;
                case FieldTypeEnum.YesNo:
                    tag.AllowFormats = true;
                    tag.SampleFormats = new string[] { "Ja;Nee" };
                    tagCollection.Add(tag);
                    break;

                case FieldTypeEnum.CheckboxList:
                    tag.HasCloseTag = true;
                    tagCollection.Add(tag);

                    tag = new Tag();
                    tag.Name = "{" + fld.Name + ".Value}";
                    tagCollection.Add(tag);
                    break;
                default:
                    tagCollection.Add(tag);
                    break;

            }
            return tagCollection;
        }

        public List<Tag> GetParentTagsByField(DataField fld)
        {
            List<Tag> tagCollection = new List<Tag>();
            Tag tag = new Tag();
            tag.Name = "{Parent." + fld.Name + "}";
            //tag.ReplaceValue = "<%#DataBinder.Eval(Container.DataItem,\"ParentGroup." + fld.MappingColumn + "\")%>";
            //parentgroup zit rechtstreeks in de code van de usercontrol
            tag.ReplaceValue = "<%# ParentGroup." + fld.MappingColumn + "%>";

            switch (fld.FieldType)
            {
                case FieldTypeEnum.Currency:
                    //tags zijn te gebruiken als {bedrag:C}
                    //C = format
                    //formats van numbers en dates worden gezet in publish van module
                    tag.AllowFormats = true;
                    tag.SampleFormats = new string[] { "C" }; //voor weergave in tags-popup
                    tagCollection.Add(tag);
                    break;
                case FieldTypeEnum.DateTime:
                    //tags zijn te gebruiken als {datum:dd-MM-yyyy}
                    //C = format
                    //formats van numbers en dates worden gezet in publish van module
                    tag.AllowFormats = true;
                    tag.SampleFormats = new string[] { "dd-MM-yyyy", "HH:mm:ss" };
                    tagCollection.Add(tag);
                    break;
                case FieldTypeEnum.DropDown:
                    //vervang FK_1 door LookupValue1.Name
                    string lookupMapping = fld.MappingColumn.Replace("FK_", "ParentGroup.LookupValue") + ".Name";
                    tag.ReplaceValue = "<%#DataBinder.Eval(Container.DataItem,\"" + lookupMapping + "\")%>";
                    tagCollection.Add(tag);
                    break;
                case FieldTypeEnum.File:


                    break;
                case FieldTypeEnum.FileList:

                    break;

                case FieldTypeEnum.Image:

                    tag.ReplaceValue = "<img src=\"<%#DataBinder.Eval(Container.DataItem,\"ParentGroup." + fld.MappingColumn + "\")%>\" alt=\"" + fld.Name + "\"Image\" />";
                    tagCollection.Add(tag);
                    //extra tag voor plain Url
                    tag = new Tag();
                    tag.Name = "{Parent." + fld.Name + "Url}";
                    tag.ReplaceValue = "<%#DataBinder.Eval(Container.DataItem,\"ParentGroup." + fld.MappingColumn + "\")%>";
                    tagCollection.Add(tag);

                    break;
                case FieldTypeEnum.ImageList:

                    break;
                case FieldTypeEnum.Numeric:
                    tag.AllowFormats = true;
                    //tag.SampleFormats = new string[] {"N"}; //uitgezet, want Volgnummer is altijd geheel getal
                    tagCollection.Add(tag);
                    break;
                case FieldTypeEnum.YesNo:
                case FieldTypeEnum.LongText:
                case FieldTypeEnum.Html:

                default:
                    tagCollection.Add(tag);
                    break;

            }

            return tagCollection;
        }

        public List<Tag> GetParentParentTagsByField(DataField fld)
        {
            List<Tag> tagCollection = new List<Tag>();
            if (fld.MappingColumn == "Name" || fld.MappingColumn == "Title" || fld.MappingColumn == "ID")
            {
                Tag tag = new Tag();
                tag.Name = "{ParentParent." + fld.Name + "}";
                tag.ReplaceValue = "<%# ParentParentGroup." + fld.MappingColumn + " %>";
                tagCollection.Add(tag);
            }
            return tagCollection;
        }

        private List<Tag> _dataSortTags = null;
        public List<Tag> GetSortLinksTags(int pageNumber = 0, int maxDataRows = 0)
        {
            
            if (_dataSortTags == null)
            {
                _dataSortTags = new List<Tag>();

                if (this.DataCollection != null)
                {
                    BaseCollection<DataField> fields;
                    if (this.Type.Contains("Group"))
                    {
                        fields = DataCollection.DataGroupFields;

                    }
                    else
                    {
                        fields = DataCollection.DataItemFields;
                    }
                    string jsonParameters = JSONSerializer.Serialize(Parameters);
                    
                    foreach (DataField field in fields)
                    {
                        if (field.FieldType != FieldTypeEnum.CheckboxList &&
                            field.FieldType != FieldTypeEnum.DropDown &&
                            field.FieldType != FieldTypeEnum.File &&
                            field.FieldType != FieldTypeEnum.FileList &&
                            field.FieldType != FieldTypeEnum.ImageList)
                        {
                            Tag tag = new Tag();
                            tag.Name = "{" + field.Name + "SortLink}";
                            tag.ReplaceValue = "<a class=\"bitsortlink\" href=\"javascript:void(0);\" onclick=\"BITSITESCRIPT.doSort('" + this.ID.ToString() + "', '" + field.MappingColumn + "', " + pageNumber + ", " + maxDataRows + ", " + jsonParameters + ");\" >";
                            tag.HasCloseTag = true;
                            tag.ReplaceValueCloseTag = "</a>";
                            _dataSortTags.Add(tag);
                        }
                    }
                }
            }
            return _dataSortTags;
        }

        protected string replaceFormatedTag(string html, Tag tag)
        {
            //zoek in de tekst naar een tag met een format
            string searchTag = tag.Name.Replace("}", "") + ":";
            int posSearchStart = html.IndexOf(searchTag);
            if (posSearchStart >= 0)
            {
                int posSearchEnd = html.IndexOf("}", posSearchStart);
                string tagWithFormat = html.Substring(posSearchStart, posSearchEnd - posSearchStart + 1);

                string format = this.GetFormat(tagWithFormat);
                //format  in databinder zetten
                //<%#DataBinder.Eval(Container.DataItem, "Datum")%> wordt bijvoorbeeld
                //<%#String.Format(new BitSite.CustomFormatProvider(), "{0:dd-MM-yyyy}", DataBinder.Eval(Container.DataItem, "Datum"))%>
                //zie _bitplate/_bitClasses/CustomFormatProvider --> kan ook booleans format geven
                string replaceTagValue = tag.ReplaceValue.Replace("<%#DataBinder.Eval(", "<%#String.Format(new BitSite.CustomFormatProvider(), \"{0:" + format + "}\", DataBinder.Eval(");
                replaceTagValue = replaceTagValue.Replace(")%>", "))%>");

                //string replaceValue = tag.ReplaceValue.Replace(")%>", ", \"{0:" + format + "}\")%>");
                html = html.Replace(tagWithFormat, replaceTagValue);

            }
            return html;
        }


        protected void getDrillDownUrlAndLink(out string drillDownUrl, out string drillDownLink, bool refreshItself = false)
        {
            drillDownUrl = "";
            ModuleNavigationAction navigationAction = this.GetNavigationActionByTagName("{DrillDownLink}");
            if (navigationAction.NavigationType == NavigationTypeEnum.NavigateToPage)
            {
                if (navigationAction.NavigationPage != null)
                {
                    drillDownUrl = navigationAction.NavigationPage.RelativeUrl;
                }
                drillDownLink = "<a href=\"{DrillDownUrl}\">";
            }
            else
            {
                drillDownUrl = HttpContext.Current.Request.Url.LocalPath;
                string refreshModules = String.Join(",", navigationAction.RefreshModules);
                //haal eventuele lege waardes weg
                refreshModules = refreshModules.Replace(",,", "");
                if (refreshModules.EndsWith(","))
                {
                    refreshModules = refreshModules.Remove(refreshModules.Length - 1, 1);
                }
                //breadcrumb moet altijd zichzelf refreshen. Dat doen we door eigen id toe te voegen aan refreshModules
                if (refreshItself && !refreshModules.Contains(this.ID.ToString()))
                {
                    refreshModules += "," + this.ID.ToString();
                }
                string extraJs = (navigationAction.JsFunction != null) ? navigationAction.JsFunction.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\"", "'") : "";
                drillDownLink = "<a class=\"showDetailsInModules\" href=\"{DrillDownUrl}\" onclick=\"BITSITESCRIPT.reloadModulesOnSamePage('" + refreshModules + "', {dataid: '{ID}'});" + extraJs + "\">";
            }
        }

        protected System.Data.DataTable getDataTable(string tableName, string tableAlias, BaseCollection<DataField> fields)
        {
            int maxNumberOfRows = getSetting<int>("MaxNumberOfRows");
            int showFromRowNumber = getSetting<int>("ShowFromRowNumber");
            return getDataTable(tableName, tableAlias, fields, 0, maxNumberOfRows, showFromRowNumber, "");
        }
        protected System.Data.DataTable getDataTable(string tableName, string tableAlias, BaseCollection<DataField> fields, int currentPage, int pageSize, int showFromRowNumber, string sort)
        {
            string selectSql = String.Format("SELECT {0}.ID,{0}.createdate,{0}.active,{0}.datefrom,{0}.datetill,{0}.LastPublishedFileName, {0}.LastPublishedDate,{0}.ChangeStatus,{0}.FK_DataCollection,{0}.FK_Parent_Group,{0}.CompletePath,{0}.Name,{0}.FK_User,{0}.FK_BitplateUser,", tableAlias);
            selectSql += getSelectSqlFromMappingFields(fields, tableAlias);
            if (Content.Contains("{Parent"))
            {
                //selectSql += String.Format("SELECT {0}.FK_Parent_Group,{0}.Title AS ParentTitle, {0}.Name AS ParentName,", "P");
                selectSql += "," + getSelectSqlFromParentMappingFields(DataCollection.DataGroupFields, "Parent");
            }
            string fromSql = getFrom(tableName, tableAlias, fields, DataCollection.DataGroupFields);
            string whereSql = getWhere(tableAlias);
            //in getWhereFromSelectFields() kunnen velden ook worden vertaald
            whereSql += getWhereFromSelectFields(tableAlias, fields);
            string sortSql = (sort == null || sort == "") ? getSort(fields) : " ORDER BY " + sort;
            
            //als paginering wordt gebruikt is currentpage > 0, dan vanaf rij overschrijven
            if (currentPage > 0) showFromRowNumber = 0;

            string limitSql = getLimit(currentPage, pageSize, showFromRowNumber);

            string sql = selectSql + fromSql + whereSql + sortSql + limitSql;

            System.Data.DataTable groupsTable = DataBase.Get().GetDataTable(sql);
            return groupsTable;
        }

        
        
        public string CreateRewriteUrl(CmsPage page, string navigationPageRelativeUrl, string type, Guid dataObjId, string completePath, string title)
        {
            //type = I of G, hieraan kun je later zien welke soort dataid het was.
            if (navigationPageRelativeUrl == null || navigationPageRelativeUrl == "")
            {
                navigationPageRelativeUrl = page.RelativeUrl;// HttpContext.Current.Request.Url.LocalPath;
            }

            if (dataObjId == Guid.Empty)
            {
                return navigationPageRelativeUrl;
            }

            string Url = navigationPageRelativeUrl + "/" + type+ dataObjId.ToString("N") + "/";
            if (completePath != null)
            {
                Url += completePath.Replace(" ", "_").Replace(":", "").Replace(".", "").Replace("\"", "").Replace("&", "-").Replace("%", "") + "/";
            }
            if (title != null)
            {
                Url += title.Replace(" ", "_").Replace(":", "").Replace(".", "").Replace("\"", "").Replace("&", "-").Replace("%", "");
            }
            else
            {
                Url += "index";
            }

            return Url;
        }
       
        /// <summary>
        /// Velden komma gescheiden zonder vaste velden en zonder SELECT
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected string getSelectSqlFromMappingFields(BaseCollection<DataField> fields, string tableAlias, string languageCode = "")
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataField field in fields)
            {
                if (field.FieldType == FieldTypeEnum.ImageList || field.FieldType == FieldTypeEnum.FileList || field.FieldType == FieldTypeEnum.CheckboxList)
                {
                    continue;
                }
                if (field.FieldType == FieldTypeEnum.Image)
                {
                    if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage && field.IsMultiLanguageField)
                    {
                        sb.Append("Lang." + field.MappingColumn + " AS " + field.MappingColumn + "Url, ");
                    }
                    else
                    {
                        sb.Append(tableAlias + "." + field.MappingColumn + " AS " + field.MappingColumn + "Url, ");
                    }
                }
                else if (field.FieldType == FieldTypeEnum.DropDown)
                {
                    string lookupTableAlias = field.MappingColumn.Replace("LookupValue", "Lookup");
                    string lookupNameFieldAlias = field.MappingColumn.Replace("LookupValue", "LookupName");
                    string lookupIDFieldAlias = field.MappingColumn.Replace("LookupValue", "LookupID");
                    if (Content.Contains("{" + lookupNameFieldAlias + "}") || Content.Contains("{" + lookupIDFieldAlias + "}"))
                    {
                        if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage && field.IsMultiLanguageField)
                        {
                            //voorlopig heeft elke taal in datalookupvalue een eigen veld, want het volstaat. Later als bitplate echt multinational wordt is er wel tijd om het beter te maken
                            sb.Append(lookupTableAlias + "." + LanguageCode + " AS " + lookupNameFieldAlias + ", ");
                        }
                        else
                        {
                            sb.Append(lookupTableAlias + ".Name AS " + lookupNameFieldAlias + ", ");
                        }
                        sb.Append(lookupTableAlias + ".ID AS " + lookupIDFieldAlias + ", ");
                    }
                }
                else
                {
                    if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage && field.IsMultiLanguageField)
                    {
                        sb.Append("Lang." + field.MappingColumn + " AS " + field.MappingColumn + ", ");
                    }
                    else
                    {
                        sb.Append(tableAlias + "." + field.MappingColumn + " AS " + field.MappingColumn + ", ");
                    }
                }
            }
            sb.Remove(sb.Length - 2, 2);
            string returnValue = sb.ToString();
            return returnValue;
        }

        protected string getSelectSqlFromParentMappingFields(BaseCollection<DataField> fields, string tableAlias)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataField field in fields)
            {
                if (field.FieldType == FieldTypeEnum.ImageList || field.FieldType == FieldTypeEnum.FileList || field.FieldType == FieldTypeEnum.CheckboxList)
                {
                    continue;
                }
                if (field.FieldType == FieldTypeEnum.Image)
                {
                    //IMAGE: plak Url achter veldnaam
                    if (!DataCollection.HasStaticLanguage &&  this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage && field.IsMultiLanguageField)
                    {
                        sb.Append("ParentLang." + field.MappingColumn + " AS Parent" + field.MappingColumn + "Url, ");
                    }
                    else
                    {
                        sb.Append(tableAlias + "." + field.MappingColumn + " AS Parent" + field.MappingColumn + "Url, ");
                    }
                }
               
                else if (field.FieldType == FieldTypeEnum.DropDown)
                {
                    //LOOKUP
                    string lookupTableAlias = field.MappingColumn.Replace("LookupValue", "ParentLookup");
                    string lookupNameFieldAlias = field.MappingColumn.Replace("LookupValue", "ParentLookupName");
                    string lookupIDFieldAlias = field.MappingColumn.Replace("LookupValue", "ParentLookupID");
                    if (Content.Contains("{" + lookupNameFieldAlias + "}") || Content.Contains("{" + lookupIDFieldAlias + "}"))
                    {
                        if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage && field.IsMultiLanguageField)
                        {
                            //voorlopig heeft elke taal in datalookupvalue een eigen veld, want het volstaat. Later als bitplate echt multinational wordt is er wel tijd om het beter te maken
                            sb.Append(lookupTableAlias + "." + LanguageCode + " AS " + lookupNameFieldAlias + ", ");
                        }
                        else
                        {
                            sb.Append(lookupTableAlias + ".Name AS " + lookupNameFieldAlias + ", ");
                        }
                        sb.Append(lookupTableAlias + ".ID AS " + lookupIDFieldAlias + ", ");
                    }
                }    
              
                else
                {
                    //OTHER
                    if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage && field.IsMultiLanguageField)
                    {
                        sb.Append("ParentLang." + field.MappingColumn + " AS Parent" + field.MappingColumn + ", ");
                    }
                    else
                    {
                        sb.Append(tableAlias + "." + field.MappingColumn + " AS Parent" + field.MappingColumn + ", ");
                    }
                }
            }
            sb.Remove(sb.Length - 2, 2);
            string returnValue = sb.ToString();
            return returnValue;
        }

        private string getFrom(string tableName, string tableAlias, BaseCollection<DataField> fields, BaseCollection<DataField> parentFields)
        {
            string fromSql = " FROM " + tableName + " AS " + tableAlias;
            if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage)
            {
                fromSql += " JOIN " + tableName + "Language AS Lang ON " + tableAlias + ".ID = Lang.FK_" + tableName + " AND Lang.LanguageCode = '" + this.LanguageCode + "'";
            }
            if (Content.Contains("{Parent"))
            {
                fromSql += " LEFT OUTER JOIN DataGroup AS Parent ON Parent.ID = " + tableAlias + ".FK_Parent_Group ";
                if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage)
                {
                    fromSql += " LEFT OUTER  JOIN DataGroupLanguage AS ParentLang ON Parent.ID = ParentLang.FK_DataGroup AND ParentLang.LanguageCode = '" + this.LanguageCode + "'";
                }
                foreach (DataField parentField in parentFields)
                {
                    if (parentField.FieldType == FieldTypeEnum.DropDown)
                    {
                        string lookupTableAlias = parentField.MappingColumn.Replace("LookupValue", "ParentLookup");
                        string lookupNameFieldAlias = parentField.MappingColumn.Replace("LookupValue", "ParentLookupName");
                        string lookupIDFieldAlias = parentField.MappingColumn.Replace("LookupValue", "ParentLookupID");
                        string foreignKeyName = parentField.MappingColumn.Replace("LookupValue", "FK_");
                        if (Content.Contains("{" + lookupNameFieldAlias + "}") || Content.Contains("{" + lookupIDFieldAlias + "}"))
                        {
                            fromSql += " LEFT OUTER JOIN DataLookupValue AS " + lookupTableAlias + "  ON " + lookupTableAlias + ".ID = Parent." + foreignKeyName;
                        }

                    }
                }
            }
            foreach (DataField field in fields)
            {
                if (field.FieldType == FieldTypeEnum.DropDown)
                {
                    string lookupTableAlias = field.MappingColumn.Replace("LookupValue", "Lookup");
                    string lookupNameFieldAlias = field.MappingColumn.Replace("LookupValue", "LookupName");
                    string lookupIDFieldAlias = field.MappingColumn.Replace("LookupValue", "LookupID");
                    string foreignKeyName  = field.MappingColumn.Replace("LookupValue", "FK_");
                    if (Content.Contains("{" + lookupNameFieldAlias + "}") || Content.Contains("{" + lookupIDFieldAlias + "}"))
                    {
                        fromSql += " LEFT OUTER JOIN DataLookupValue AS " + lookupTableAlias + "  ON " + lookupTableAlias + ".ID = " + tableAlias + "." + foreignKeyName;
                    }
            
                }
            }
            return fromSql;
        }

        protected virtual string getWhere(string tableAlias)
        {
            bool showInActive = getSetting<bool>("ShowInactive");

            string where = String.Format(" WHERE {1}.FK_DataCollection = '{0}'", DataCollection.ID, tableAlias);
            if (!showInActive)
            {
                where += String.Format(" AND ({1}.Active = 1 OR ({1}.Active = 2 AND IFNULL({1}.DateFrom, '2000-1-1') <= '{0:yyyy-MM-dd} 00:00:00' AND IFNULL({1}.DateTill, '2999-1-1') >= '{0:yyyy-MM-dd}'))", DateTime.Now, tableAlias);
            }
            return where;
        }
        /// <summary>
        /// Bouwt where op aan de hand van getSetting<string>("SelectField");
        /// Deze velden kunnen ook multi-talig zijn en zullen dan naar de LanguageTabel verwijzen
        /// </summary>
        /// <param name="tableAlias"></param>
        /// <param name="dataFields">Collectie met de velden die mogelijk multilanguage aan hebben staan</param>
        /// <returns></returns>
        protected virtual string getWhereFromSelectFields(string tableAlias, BaseCollection<DataField> dataFields)
        {
            string where = "";
            string selectField = getSetting<string>("SelectField");
            string selectOperator = getSetting<string>("SelectOperator");
            string selectValue = getSetting<string>("SelectValue");
            //string selectGroupID = getSetting<string>("SelectGroupID");
            
            if (selectField == "null") selectField = null;
            if (selectField != null && selectField != "")
            {
                if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage)
                {
                    selectField = getLanguageFieldIfExists(dataFields, selectField);
                }
                if (selectOperator == "like" || selectOperator == "not like")
                {
                    where += String.Format(" AND {3}.{0} {1} '%{2}%'", selectField, selectOperator, selectValue, tableAlias);
                }
                else if (selectOperator == "likeStart" || selectOperator == "not likeStart")
                {
                    selectOperator = selectOperator.Replace("Start", "");
                    where += String.Format(" AND {3}.{0} {1} '{2}%'", selectField, selectOperator, selectValue, tableAlias);
                }
                else if (selectOperator == "likeEnd" || selectOperator == "not likeEnd")
                {
                    selectOperator = selectOperator.Replace("End", "");
                    where += String.Format(" AND {3}.{0} {1} '%{2}'", selectField, selectOperator, selectValue, tableAlias);
                }
                else
                {
                    if (selectOperator == "") selectOperator = "=";
                    where += String.Format(" AND {3}.{0} {1} '{2}'", selectField, selectOperator, selectValue, tableAlias);
                }
            }

            return where;
        }

        private static string getLanguageFieldIfExists(BaseCollection<DataField> fields, string fieldName)
        {
            //kijk of het veld uit de taal-gejoinde tabel komt
            foreach (DataField field in fields)
            {
                if (field.IsMultiLanguageField && field.MappingColumn == fieldName)
                {
                    fieldName = "Lang." + fieldName;
                }
            }
            return fieldName;
        }


        protected virtual string getSort(BaseCollection<DataField> fields)
        {
            string sort = "";
            string sortField = getSetting<string>("SortField");
            if (sortField != null && sortField != "")
            {
                if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage)
                {
                    sortField = getLanguageFieldIfExists(fields, sortField);
                }
                sort = sortField + " " + getSetting<string>("SortOrder");
            }
            sort = sort.Trim();
            if (sort != null && sort != "")
            {
                sort = " ORDER BY " + sort;
            }
            return sort;
        }

        protected string getLimit(int pageNumber, int pageSize, int fromRowNumber)
        {
            string sql = "";
            if (pageSize != 0)
            {

                if (pageNumber != 0)
                {
                    //pageNumber--;
                    sql = @" LIMIT " + (pageNumber * pageSize) + @", " + pageSize;
                }
                else
                {
                    if (fromRowNumber > 0) fromRowNumber--; //gebruikers vullen vanaf rij 1 in, in sql is dit rij 0 
                    sql = @" LIMIT " + fromRowNumber + @", " + pageSize;
                }
            }
            return sql;
        }

        protected System.Data.DataTable getExtraFilesDataTable(string itemId, string groupId)
        {
            string sql = "";
            if (itemId != null && itemId != "")
            {
                DataField field = null;
                foreach (DataField fieldItem in DataCollection.DataItemFields)
                {
                    if (fieldItem.FieldType == FieldTypeEnum.FileList)
                    {
                        field = fieldItem;
                    }
                }
                if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && field.IsMultiLanguageField)
                {
                    sql = String.Format("SELECT * FROM DataFile WHERE FK_Item='{0}' AND type = 'ExtraFile'", itemId);
                }
                else
                {
                    sql = String.Format("SELECT * FROM DataFile WHERE FK_Item='{0}' AND type = 'ExtraFile' AND Language = '{1}'", itemId, this.LanguageCode);
                }
            }
            else
            {
                sql = String.Format("SELECT * FROM DataFile WHERE FK_Group='{0}' AND type = 'ExtraFile'", groupId);
            }
            return DataBase.Get().GetDataTable(sql);
        }

        protected System.Data.DataTable getExtraImagesDataTable(string itemId, string groupId)
        {
            string sql = "SELECT * FROM DataFile WHERE 1=0";
            if (itemId != null && itemId != "")
            {
                sql = String.Format("SELECT * FROM DataFile WHERE FK_Item='{0}' AND type = 'ExtraImage'", itemId);
            }
            else if (groupId != null && groupId != "")
            {
                sql = String.Format("SELECT * FROM DataFile WHERE FK_Group='{0}' AND type = 'ExtraImage'", groupId);
            }
            return DataBase.Get().GetDataTable(sql);
        }

        protected string fillExtraFilesSubTemplate(string html, string itemId, string groupId)
        {
            string extraFilesSubTemplate = base.GetSubTemplate("{FileList1}");
            string extraFiles = "";
            System.Data.DataTable dataTableExtraFiles = getExtraFilesDataTable(itemId, groupId);
            foreach (DataRow extraFilesRow in dataTableExtraFiles.Rows)
            {
                string extraFile = extraFilesSubTemplate;
                extraFile = extraFile.Replace("{FileList1Url}", extraFilesRow["Url"].ToString());
                extraFile = extraFile.Replace("{FileList1Name}", extraFilesRow["Name"].ToString());
                extraFiles += extraFile;
            }
            html = base.ReplaceSubTemplate("{FileList1}", html, extraFiles, extraFilesSubTemplate);
            return html;
        }

        protected string fillExtraImagesSubTemplate(string html, string itemId, string groupId)
        {
            string extraImagesSubTemplate = base.GetSubTemplate("{ImageList1}");
            string extraImages = "";
            System.Data.DataTable dataTableExtraImages = getExtraImagesDataTable(itemId, groupId);
            foreach (DataRow extraImagesRow in dataTableExtraImages.Rows)
            {
                string extraImage = extraImagesSubTemplate;
                extraImage = extraImage.Replace("{ImageList1Url}", extraImagesRow["Url"].ToString());
                extraImage = extraImage.Replace("{ImageList1ThumbnailUrl}", extraImagesRow["Url"].ToString().Replace(".jpg", "_thumb.jpg"));
                //extraImage = extraImage.Replace("{ImageList1Name}", extraImagesRow["Name"].ToString());
                //extraImage = extraImage.Replace("{ImageList1Image}", "<img src=\"" + extraImagesRow["Url"].ToString() + "\" alt=''/>");
                extraImages += extraImage;
            }
            html = base.ReplaceSubTemplate("{ImageList1}", html, extraImages, extraImagesSubTemplate);
            return html;
        }

        protected string fillLookupSubTemplate(string html, DataField dataField)
        {
            return html;
            string subTemplate = Regex.Match(html, "{" + dataField.Name + "}(.*?){/" + dataField.Name + "}", RegexOptions.Singleline).ToString();
            if (subTemplate != "")
            {

            }
        }

        public override string Publish2(CmsPage page)
        {
            if (DataCollection == null)
            {
                this.Content = "Kies eerst een datacollectie";
            }
            else
            {
                if (dataId == Guid.Empty)
                {
                    string dataIdQuery = HttpContext.Current.Request.QueryString["dataid"];
                    Guid.TryParse(dataIdQuery, out dataId);
                    if (HttpContext.Current.Request.QueryString["datatype"] != null)
                    {
                        this.dataType = HttpContext.Current.Request.QueryString["datatype"].ToString();
                    }
                    //zet parameters vanuit query string in parameters want wordt gebruikt in pager
                    //todo refactor in parameters object welke in Page.aspx wordt aangemaakt
                    if (dataId != Guid.Empty)
                    {
                        if (Parameters.ContainsKey("dataid"))
                        {
                            Parameters["dataid"] = dataId;
                        }
                        else
                        {
                            Parameters.Add("dataid", dataId);
                        }
                    }
                    if (dataType != null && dataType != string.Empty)
                    {
                        if (Parameters.ContainsKey("datatype"))
                        {
                            Parameters["datatype"] = dataType;
                        }
                        else
                        {
                            Parameters.Add("datatype", dataType);
                        }
                    }
                }
            }
            return base.Publish2(page);
        }



        protected void PrepairTemplate(BaseCollection<DataField> fields, BaseCollection<DataField> parentFields)
        {
            //prepareer template
            if (DataCollection == null)
            {
                this.Content = "Kies eerst een datacollectie";
                return;
            }
       
            foreach (DataField field in fields)
            {
                if (field.FieldType == FieldTypeEnum.Image)
                {
                    //vervang images voor html-img-tags
                    Content = Content.Replace("{" + field.Name + "}", "<img src='{" + field.MappingColumn + "Url}' alt='{Title}'/>");
                    //Content = Content.Replace("{Parent." + field.Name + "}", "<img src='{Parent" + field.MappingColumn + "Url}' alt='{Title}'/>");
                    Content = Content.Replace("{" + field.Name + "Url}", "{" + field.MappingColumn + "Url}");
                    Content = Content.Replace("{" + field.Name + "ThumbnailUrl}", "{" + field.MappingColumn + "ThumbnailUrl}");
                    //Content = Content.Replace("{Parent." + field.Name + "Url}", "{Parent" + field.MappingColumn + "Url}");
                }
                else if (field.FieldType == FieldTypeEnum.DropDown)
                {
                    Content = Content.Replace("{" + field.Name + "}", "{" + field.MappingColumn.Replace("LookupValue", "LookupName") + "}");
                    Content = Content.Replace("{" + field.Name + ".ID}", "{" + field.MappingColumn.Replace("LookupValue", "LookupID") + "}");
                    
                }
                else if (field.FieldType == FieldTypeEnum.CheckboxList)
                {
                    //Deze tag wordt gereplaced in de publish2 functie van de modules.
                }
                else if (field.FieldType == FieldTypeEnum.FileList || field.FieldType == FieldTypeEnum.ImageList)
                {
                    Content = Content.Replace("{" + field.Name + "}", "{" + field.MappingColumn + "}");
                    Content = Content.Replace("{/" + field.Name + "}", "{/" + field.MappingColumn + "}");
                    Content = Content.Replace("{" + field.Name + "Url}", "{" + field.MappingColumn + "Url}");
                    Content = Content.Replace("{" + field.Name + "ThumbnailUrl}", "{" + field.MappingColumn + "ThumbnailUrl}");
                    
                }
                else if (field.FieldType == FieldTypeEnum.DateTime)
                {
                    Content = Content.Replace("{" + field.Name + "}", "{" + field.MappingColumn + "}");
                    Content = Content.Replace("{" + field.Name + ":", "{" + field.MappingColumn + ":");
                }
                else
                {
                    //vervang punten, want kan niet in sql. Veldnamen worden namelijk aliassen in sql --> niet
                    Content = Content.Replace("{" + field.Name + "}", "{" + field.MappingColumn + "}");
                    //Content = Content.Replace("{Parent." + field.Name + "}", "{Parent" + field.MappingColumn + "}");
                }
            }
            foreach (DataField parentField in parentFields)
            {
                if (parentField.FieldType == FieldTypeEnum.Image)
                {
                    //vervang images voor html-img-tags
                    Content = Content.Replace("{Parent." + parentField.Name + "}", "<img src='{Parent" + parentField.MappingColumn + "Url}' alt='{Title}'/>");
                    Content = Content.Replace("{Parent." + parentField.Name + "Url}", "{Parent" + parentField.MappingColumn + "Url}");
                }
                else if (parentField.FieldType == FieldTypeEnum.DropDown)
                {
                    Content = Content.Replace("{Parent." + parentField.Name + "}", "{" + parentField.MappingColumn.Replace("LookupValue", "ParentLookupName") + "}");
                    Content = Content.Replace("{Parent." + parentField.Name + ".ID}", "{" + parentField.MappingColumn.Replace("LookupValue", "ParentLookupID") + "}");
                }
                else
                {
                    //vervang punten, want kan niet in sql. Veldnamen worden namelijk aliassen in sql --> niet
                    Content = Content.Replace("{Parent." + parentField.Name + "}", "{Parent" + parentField.MappingColumn + "}");
                }
            }
        }


        internal DataRow getDataParentGroupRow(Guid dataId)
        {
            string selectSql = " SELECT Parent.ID AS ParentID, FK_Parent_Group, Parent.CompletePath, Parent.Name AS ParentName, " + getSelectSqlFromParentMappingFields(DataCollection.DataGroupFields, "Parent");
            string fromSql = " FROM DataGroup AS Parent ";
            if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage)
            {
                fromSql += " JOIN DataGroupLanguage AS ParentLang ON Parent.ID = ParentLang.FK_DataGroup AND ParentLang.LanguageCode = '" + this.LanguageCode + "'";
            }
            string whereSql = " WHERE Parent.ID = '" + dataId.ToString() + "'";

            string sql = selectSql + fromSql + whereSql;

            System.Data.DataTable groupsTable = DataBase.Get().GetDataTable(sql);

            return (groupsTable.Rows.Count > 0) ? groupsTable.Rows[0] : groupsTable.NewRow();
        }

        //internal DataRow getDataParentItemRow(Guid dataId)
        //{
        //    string selectSql = " SELECT P.ID as ParentID, FK_Parent_Group, CompletePath, Title, " + getSelectSqlFromParentMappingFields(DataCollection.DataItemFields, "P");
        //    string fromSql = " FROM DataItem AS P ";
        //    string whereSql = " WHERE ID = '" + dataId.ToString() + "'";

        //    string sql = selectSql + fromSql + whereSql;

        //    System.Data.DataTable groupsTable = DataBase.Get().GetDataTable(sql);

        //    return (groupsTable.Rows.Count > 0) ? groupsTable.Rows[0] : groupsTable.NewRow();
        //}

        internal DataRow getDataGroupRow(string where)
        {
            string tableAlias = "G";
            BaseCollection<DataField> fields = DataCollection.DataGroupFields;
            string selectSql = String.Format("SELECT {0}.ID,{0}.createdate,{0}.active,{0}.datefrom,{0}.datetill,{0}.LastPublishedFileName, {0}.LastPublishedDate,{0}.ChangeStatus,{0}.FK_DataCollection,{0}.FK_Parent_Group,{0}.CompletePath,{0}.Name,", tableAlias);
            selectSql += getSelectSqlFromMappingFields(fields, tableAlias);
            
            if (Content.Contains("{Parent"))
            {
                //selectSql += String.Format("SELECT {0}.FK_Parent_Group,{0}.Title AS ParentTitle, {0}.Name AS ParentName,", "P");
                selectSql += "," + getSelectSqlFromParentMappingFields(DataCollection.DataGroupFields, "Parent");
            }
            string fromSql = getFrom("DataGroup", "G", fields, DataCollection.DataGroupFields);
            string whereSql = " WHERE " + tableAlias + ".FK_DataCollection = '" + DataCollection.ID.ToString() + "'";
            whereSql += " AND " + where;
            string sortSql = " ORDER BY OrderNumber ASC";
            string sql = selectSql + fromSql + whereSql + sortSql;

            System.Data.DataTable dataTable = DataBase.Get().GetDataTable(sql);

            return (dataTable.Rows.Count > 0) ? dataTable.Rows[0] : dataTable.NewRow();
        }

        internal DataRow getDataItemRow(string where)
        {
            string tableAlias = "I";
            BaseCollection<DataField> fields = DataCollection.DataItemFields;
            string selectSql = String.Format("SELECT {0}.ID,{0}.createdate,{0}.active,{0}.datefrom,{0}.datetill,{0}.LastPublishedFileName, {0}.LastPublishedDate,{0}.ChangeStatus,{0}.FK_DataCollection,{0}.FK_Parent_Group,{0}.CompletePath,{0}.Name,", tableAlias);
            selectSql += getSelectSqlFromMappingFields(fields, tableAlias);
            if (Content.Contains("{Parent"))
            {
                //selectSql += String.Format("SELECT {0}.FK_Parent_Group,{0}.Title AS ParentTitle, {0}.Name AS ParentName,", "P");
                selectSql += "," + getSelectSqlFromParentMappingFields(DataCollection.DataGroupFields, "Parent");
            }
            string fromSql = getFrom("DataItem", tableAlias, DataCollection.DataItemFields, DataCollection.DataGroupFields);
            string whereSql = " WHERE " + tableAlias + ".FK_DataCollection = '" + DataCollection.ID.ToString() + "'";
            whereSql += " AND " + where;
            string sortSql = " ORDER BY OrderNumber ASC";
            string sql = selectSql + fromSql + whereSql + sortSql;

            System.Data.DataTable dataTable = DataBase.Get().GetDataTable(sql);

            return (dataTable.Rows.Count > 0) ? dataTable.Rows[0] : dataTable.NewRow();
        }


        protected string ReplaceTag(string html, System.Data.DataRow dataRow, System.Data.DataColumn column)
        {
            MatchCollection usedTags = Regex.Matches(html, "{" + column.ColumnName + "(.*?)}", RegexOptions.Singleline);
            foreach (Match usedTag in usedTags)
            {
                string completeTag = usedTag.ToString();
                string argument = completeTag.Replace("{" + column.ColumnName, "").Replace("}", "");
                if (argument != "" && argument != null)
                {
                    argument = argument.Substring(1);
                    DateTime dateValue = DateTime.Parse(dataRow[column.ColumnName].ToString());
                    html = Regex.Replace(html, completeTag, dateValue.ToString(argument));
                }
                else
                {
                    if (column.ColumnName.ToLower().Contains("image"))
                    {
                        if (dataRow[column.ColumnName] == null || dataRow[column.ColumnName] == DBNull.Value || dataRow[column.ColumnName].ToString() == "" || dataRow[column.ColumnName].ToString().Trim().ToLower() == "null")
                        {
                            dataRow[column.ColumnName] = "' style='display: none";
                        }
                    }
                    html = Regex.Replace(html, completeTag, dataRow[column.ColumnName].ToString());
                }
            }
            return html;
        }
        
    }
}
