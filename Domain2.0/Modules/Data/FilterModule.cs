using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using System.Text.RegularExpressions;
using BitPlate.Domain.DataCollections;

namespace BitPlate.Domain.Modules.Data
{
    [Persistent("Module")]
    public class FilterModule : BaseDataModule
    {

        //private List<Tag> _dataFieldTags = null;

        public FilterModule()
        {
            IncludeScripts.Add("/_js/BitFilterModule.js");
            ContentSamples.Add(@"
Zoek: {SearchAllTextBox}<br/>
Plaats hier ook de andere filtertags...<br/>
{FilterButton}Zoek{/FilterButton} {ClearFilterButton}Begin waarden{/ClearFilterButton}<br/>
");

            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Data" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Filter", IsExternal = true, Url="/_bitplate/EditPage/Modules/DataModules/FilterModuleTab.aspx" });
        }

        protected override void setNavigationActions()
        {
            base.setNavigationActions();
            if (this.NavigationActions.Count == 0)
            {
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    Name = "{ApplyFilter}",
                    NavigationType = NavigationTypeEnum.ShowDetailsInModules
                });
            }
        }

        public override void SetAllTags()
        {
            //string JsFunction = "";
            //ModuleNavigationAction navigationAction = this.GetNavigationActionByTagName("{ApplyFilter}");
            //if (navigationAction.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            //{
            //    JsFunction = String.Format("BITFILTERMODULE.applyFilter('{0}');", navigationAction.RefreshModules);
            //}
            //else {

            //}

            //autopostback setting. Default = false (Je moet op de FilterButton klikken om de filter te activeren)
            string autoPostBackJsFunction = "";
            bool autoPostBack = this.getSetting<bool>("Autopostback");
            //if(autoPostBack) autoPostBackJsFunction = "onchange=javascript:BITFILTERMODULE.applyFilter('" + ID.ToString() + "');";

            if (DataCollection != null)
            {
                BaseCollection<DataField> fields = DataCollection.DataItemFields;

                //Tags aanmaken op basis van de gevonden Fields.
                foreach (DataField fld in fields)
                {
                    switch (fld.FieldType)
                    {
                        case FieldTypeEnum.YesNo:
                            autoPostBackJsFunction = (autoPostBack) ? "onchange=\"javascript:BITFILTERMODULE.applyFilter('" + ID.ToString() + "');\"" : "";
                            this._tags.Add(new Tag() { Name = "{" + fld.Name + "Checkbox}", ReplaceValue = string.Format("<input type=\"checkbox\" data-filter-mapping-column=\"{0}\" data-filter-field-type=\"yesno\" {1}/>", fld.MappingColumn, autoPostBackJsFunction) });
                            break;

                        case FieldTypeEnum.DropDown:
                            this._tags.Add(new Tag() { Name = "{" + fld.Name + "DropDown}", ReplaceValue = createDropDown(fld, autoPostBack) });
                            this._tags.Add(new Tag() { Name = "{" + fld.Name + "RadioList}", ReplaceValue = createRadioList(fld, autoPostBack) });
                            this._tags.Add(new Tag() { Name = "{" + fld.Name + "CheckboxList}", ReplaceValue = createCheckBoxList(fld, autoPostBack) });
                            break;

                        case FieldTypeEnum.CheckboxList:
                            this._tags.Add(new Tag() { Name = "{" + fld.Name + "CheckboxList}", ReplaceValue = createCheckBoxList(fld, autoPostBack) });
                            break;

                        case FieldTypeEnum.Text:
                        case FieldTypeEnum.LongText:
                        case FieldTypeEnum.Html:
                            autoPostBackJsFunction = (autoPostBack) ? "onkeypress=\"javascript:BITSITESCRIPT.checkEnterKeyPress(event, '" + ID.ToString() + "', BITFILTERMODULE.applyFilter);\"" : "";
                            this._tags.Add(new Tag() { Name = "{" + fld.Name + "SearchTextBox}", ReplaceValue = string.Format("<input type=\"text\" data-filter-mapping-column=\"{0}\" data-filter-field-type=\"text\" {1}/>", fld.MappingColumn, autoPostBackJsFunction) });
                            break;

                        case FieldTypeEnum.Numeric:
                            autoPostBackJsFunction = (autoPostBack) ? "onkeypress=\"javascript:BITSITESCRIPT.checkEnterKeyPress(event, '" + ID.ToString() + "', BITFILTERMODULE.applyFilter);\"" : "";
                            this._tags.Add(new Tag() { Name = "{" + fld.Name + "SearchTextBox}", ReplaceValue = string.Format("<input type=\"text\" data-filter-mapping-column=\"{0}\" data-filter-field-type=\"numeric\" {1}/>", fld.MappingColumn, autoPostBackJsFunction) });
                            this._tags.Add(new Tag() { Name = "{" + fld.Name + "SearchTextBox}", ReplaceValue = string.Format("<input type=\"text\" data-filter-mapping-column=\"{0}\" data-filter-field-type=\"numeric\" {1}/>", fld.MappingColumn, autoPostBackJsFunction) });
                            break;

                        default:
                            break;
                    }
                }
            }
            autoPostBackJsFunction = (autoPostBack) ? "onkeypress=\"BITSITESCRIPT.checkEnterKeyPress(event, '" + ID.ToString() + "', BITFILTERMODULE.applyFilter);\"" : "";
            this._tags.Add(new Tag() { Name = "{SearchAllTextBox}", ReplaceValue = String.Format("<input type=\"text\" id=\"bitTextBox_All{0:N}\" data-filter-mapping-column=\"TextAll\" data-filter-field-type=\"text\" {1}/>", this.ID, autoPostBackJsFunction) });

            //ModuleNavigationAction navigationAction = this.GetNavigationActionByTagName("{ApplyFilter}");
            //string filterButton = "";
            //if (navigationAction.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            //{
            //    filterButton = String.Format(@"<button type=""button"" onclick=""BITFILTERMODULE.applyFilter();"">", navigationAction.RefreshModules);
            //}
            this._tags.Add(new Tag() { Name = "{FilterButton}", HasCloseTag = true, ReplaceValue = "<button type=\"button\" onclick=\"BITFILTERMODULE.applyFilter('" + ID.ToString() + "');\">", ReplaceValueCloseTag = "</button>" });

            this._tags.Add(new Tag() { Name = "{ClearFilterButton}", HasCloseTag = true, ReplaceValue = "<button type=\"button\" onclick=\"BITFILTERMODULE.clearAll('" + ID.ToString() + "');\" ID=\"bitClearFilterButton_" + this.ID.ToString("N") + "\">", ReplaceValueCloseTag = "</button>" });

        }

        private string createDropDown(DataField fld, bool autoPostBack)
        {
            string autoPostBackJsFunction = (autoPostBack) ? "onchange=\"javascript:BITFILTERMODULE.applyFilter('" + ID.ToString() + "');\"" : "";

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<select id=\"bitDropDown_{0}\" data-filter-mapping-column=\"{0}\" data-filter-field-type=\"lookup\" {1}>", fld.MappingColumn, autoPostBackJsFunction);

            sb.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, "***Alle***");
            BaseCollection<DataLookupValue> lookupValues = getLookupValues(fld);
            foreach (DataLookupValue lookupValue in lookupValues)
            {
                if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage)
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", lookupValue.ID, getTranslation(lookupValue));
                }
                else
                {
                    sb.AppendFormat("<option value='{0}'>{1}</option>", lookupValue.ID, lookupValue.Name);
                }
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        private string getTranslation(DataLookupValue lookupValue)
        {
            if (LanguageCode == "NL")
            {
                return lookupValue.NL;
            }
            else if (LanguageCode == "EN")
            {
                return lookupValue.EN;
            }
            else if (LanguageCode == "DE")
            {
                return lookupValue.DE;
            }
            else if (LanguageCode == "FR")
            {
                return lookupValue.FR;
            }
            else if (LanguageCode == "SP")
            {
                return lookupValue.SP;
            }
            else if (LanguageCode == "PL")
            {
                return lookupValue.PL;
            }
            else if (LanguageCode == "IT")
            {
                return lookupValue.IT;
            }
            else
            {
                return lookupValue.Name;
            }
        }


        private string createRadioList(DataField fld, bool autoPostBack)
        {
            string autoPostBackJsFunction = (autoPostBack) ? "onclick=\"javascript:BITFILTERMODULE.applyFilter('" + ID.ToString() + "');\"" : "";

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<ul class=\"bitRadioList\"><li><input type=\"radio\" id=\"bitRadioButton_{0}_{1}\" name=\"bitRadioButton_{0}\" value=\"{1}\" {3}/> <label for=\"bitRadioButton_{0}_{1}\">{2}</label></li>", fld.MappingColumn, Guid.Empty, "***Alle***", autoPostBackJsFunction);

            BaseCollection<DataLookupValue> lookupValues = getLookupValues(fld);
            foreach (DataLookupValue lookupValue in lookupValues)
            {
                if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage)
                {
                    sb.AppendFormat("<li><input type=\"radio\" id=\"bitRadioButton_{0}_{1}\" name=\"bitRadioButton_{0}\" value=\"{1}\" data-filter-mapping-column=\"{0}\" data-filter-field-type=\"lookup\" {3}/> <label for=\"bitRadioButton_{0}_{1}\">{2}</label></li>", fld.MappingColumn, lookupValue.ID, getTranslation(lookupValue), autoPostBackJsFunction);
                }
                else
                {
                    sb.AppendFormat("<li><input type=\"radio\" id=\"bitRadioButton_{0}_{1}\" name=\"bitRadioButton_{0}\" value=\"{1}\" data-filter-mapping-column=\"{0}\" data-filter-field-type=\"lookup\" {3}/> <label for=\"bitRadioButton_{0}_{1}\">{2}</label></li>", fld.MappingColumn, lookupValue.ID, lookupValue.Name, autoPostBackJsFunction);
                }
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        private string createCheckBoxList(DataField fld, bool autoPostBack)
        {
            string autoPostBackJsFunction = (autoPostBack) ? "onclick=\"javascript:BITFILTERMODULE.applyFilter('" + ID.ToString() + "');\"" : "";

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<div id=\"bitCheckboxList_{0:N}_{1:N}\"><ul class=\"bitCheckboxList\">", this.ID, fld.ID);
            BaseCollection<DataLookupValue> lookupValues = getLookupValues(fld);
            foreach (DataLookupValue lookupValue in lookupValues)
            {
                if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage)
                {
                    sb.AppendFormat("<li><input type=\"checkbox\" id=\"bitCheckboxList_{0}_{1:N}\" data-id=\"{1}\" data-filter-mapping-column=\"{0}\" data-filter-field-type=\"lookups\" {3}/> <label for=\"bitCheckboxList_{0}_{1:N}\">{2}</label></li>", fld.MappingColumn, lookupValue.ID, getTranslation(lookupValue), autoPostBackJsFunction);
                }
                else
                {
                    sb.AppendFormat("<li><input type=\"checkbox\" id=\"bitCheckboxList_{0}_{1:N}\" data-id=\"{1}\" data-filter-mapping-column=\"{0}\" data-filter-field-type=\"lookups\" {3}/> <label for=\"bitCheckboxList_{0}_{1:N}\">{2}</label></li>", fld.MappingColumn, lookupValue.ID, lookupValue.Name, autoPostBackJsFunction);
                }
            }
            sb.Append("</ul></div>");
            return sb.ToString();
        }

        private BaseCollection<DataLookupValue> getLookupValues(DataField fld)
        {
            BaseCollection<DataLookupValue> lookupValues = BaseCollection<DataLookupValue>.Get("FK_DataField='" + fld.ID + "'", "OrderingNumber, Name");

            return lookupValues;
        }


        public override string ConvertTags(string html)
        {
            foreach (Tag tag in GetAllTags())
            {
                html = html.Replace(tag.Name, tag.ReplaceValue);
                if (tag.HasCloseTag)
                {
                    html = html.Replace(tag.CloseTag, tag.ReplaceValueCloseTag);

                }
            }
            return html;
        }

        public override string Publish2(CmsPage page)
        {

            string html = base.Publish2(page);
            try
            {
                ModuleNavigationAction navAction = NavigationActions[0];
                string navigationUrl = navAction.NavigationPage != null ? navAction.NavigationPage.RelativeUrl : "";
                string refreshModules = navAction.RefreshModules == null ? "" : String.Join(",", navAction.RefreshModules);
                html += String.Format(@"
<input type=""hidden"" id=""hiddenModuleID{0:N}"" name=""hiddenModuleID"" value=""{0}""/>
<input type=""hidden"" id=""hiddenModuleType{0:N}"" name=""hiddenModuleType"" value=""{1}""/>
<input type=""hidden"" id=""hiddenModuleNavigationType{0:N}"" name=""hiddenModuleNavigationType"" value=""{2}""/>
<input type=""hidden"" id=""hiddenRefreshModules{0:N}"" name=""hiddenRefreshModules"" value=""{3}""/>
<input type=""hidden"" id=""hiddenNavigationUrl{0:N}"" name=""hiddenNavigationUrl"" value=""{4}""/>
", this.ID, this.Type, navAction.NavigationType, refreshModules, navigationUrl);

                html = ConvertTags(html);
            }
            catch (Exception ex)
            {
                html = base.HandlePublishError(ex);
            }
            //html += "</form>";
            return html;
        }


        public string BuildWhere(string tableAlias, Dictionary<string, object> parameters)
        {
            this.Parameters = parameters;
            string where = "";
            //where = " WHERE " + tableAlias + ".FK_DataCollection = '" + DataCollection.ID + "'";
            foreach (string mappingField in Parameters.Keys)
            {
                string value = Parameters[mappingField].ToString().Trim();
                if (value == string.Empty) continue;
                if (mappingField == "TextAll")
                {
                    string concatmappingField = " CONCAT(''";
                    foreach (DataField dataFieldToConcat in DataCollection.DataItemFields)
                    {
                        if (dataFieldToConcat.FieldType == FieldTypeEnum.Text ||
                            dataFieldToConcat.FieldType == FieldTypeEnum.LongText ||
                            dataFieldToConcat.FieldType == FieldTypeEnum.Html)
                        {
                            if (dataFieldToConcat.MappingColumn != "")
                            {
                                concatmappingField += ", " + tableAlias + "." + dataFieldToConcat.MappingColumn;
                            }
                        }
                    }
                    concatmappingField += ")";
                    where += " AND " + concatmappingField + " like '%" + value + "%'";
                }
                else if (mappingField.Contains("Text") || mappingField.Contains("LongText") || mappingField.Contains("Html")
                    || mappingField == "Name" || mappingField=="Title" )
                {
                    where += " AND " + tableAlias + "." + mappingField + " like '%" + value + "%'";
                }
                else if (mappingField.Contains("YesNo"))
                {
                    int yesno = (value.ToString() == "on") ? 1 : 0;
                    where += " AND " + tableAlias + "." + mappingField + " = " + yesno;
                }
                else if (mappingField.Contains("LookupValues"))
                {
                    object values = Parameters[mappingField];
                    foreach (object id in values as Array)
                    {
                        where += " AND EXISTS (SELECT * FROM datalookupvalueperitem WHERE FK_LookupValue = '" + id.ToString() + "' AND " + tableAlias + ".ID = datalookupvalueperitem.FK_Item) ";
                    }

                    //where += " AND " + tableAlias + "." + mappingField + " = '" + Parameters[mappingField].ToString();
                }
                else if (mappingField.Contains("LookupValue"))
                {
                    //kijk of het 1 waarde is of array van waardes
                    object values = Parameters[mappingField];
                    if (values.GetType().IsArray)
                    {
                        string wherein = "";
                        foreach (object id in values as Array)
                        {
                            wherein += "'" + id.ToString() + "',";
                        }
                        wherein = wherein.Substring(0, wherein.Length - 1);
                        where += " AND " + tableAlias + "." + mappingField.Replace("LookupValue", "FK_") + " IN (" + wherein + ")";
                    }
                    else
                    {
                        if (value.ToString() != Guid.Empty.ToString())
                        {
                            where += " AND " + tableAlias + "." + mappingField.Replace("LookupValue", "FK_") + " = '" + value + "'";
                        }
                    }
                }

                else if (mappingField.Contains("Numeric"))
                {
                    where += " AND " + tableAlias + "." + mappingField + " = " + value;
                }
            }
            //om te voorkomen dat die alle items terug geeft bij lege zoek actie:
            //is dat juist niet de bedoeling? ik vind hem zwaar onlogische.
            if (where == string.Empty)
            {
                //where = " AND 1=0";
                where = " AND 1=1";
            }
            return where;
        }
        //public string HandlePost(Dictionary<string, string> FormParameters)
        //{
        //    string where = "";
        //    Dictionary<string, string> checkboxListValuesPerMappingColumn = new Dictionary<string, string>();
        //    foreach (string key in FormParameters.Keys)
        //    {
        //        if (!key.Contains("_"))
        //        {
        //            continue;
        //        }
        //        string[] keys = key.Split(new char[] { '_' });
        //        string controlType = keys[0];
        //        string mappingColumn = keys[1];
        //        if (controlType == "bitTextboxAll")
        //        {
        //        }
        //        if (controlType == "bitTextbox")
        //        {
        //            where += " AND " + mappingColumn + " = '" + FormParameters[key] + "'";
        //        }
        //        else if (controlType == "bitDropDown")
        //        {
        //            mappingColumn = mappingColumn.Replace("LookupValue", "FK_");
        //            where += " AND " + mappingColumn + " = '" + FormParameters[key] + "'";
        //        }
        //        else if (controlType == "bitCheckBox")
        //        {
        //            where += " AND " + mappingColumn + " = 1 ";
        //        }
        //        else if (controlType == "bitRadioButton")
        //        {
        //            mappingColumn = mappingColumn.Replace("LookupValue", "FK_");
        //            where += " AND " + mappingColumn + " = '" + FormParameters[key] + "'";
        //        }
        //        else if (controlType == "bitCheckBoxList")
        //        {
        //            //eerst alle waardes opslaan
        //            //string type = keys[2];
        //            if (checkboxListValuesPerMappingColumn.ContainsKey(mappingColumn))
        //            {
        //                checkboxListValuesPerMappingColumn[mappingColumn] += ",'" + FormParameters[key] + "'";
        //            }
        //            else
        //            {
        //                checkboxListValuesPerMappingColumn.Add(mappingColumn, "'" + FormParameters[key] + "'");
        //            }
        //            //if (type == "ManyToMany")
        //            //{
        //            //}
        //            //else
        //            //{
        //            //    string[] values = FormParameters[key].Split(new char[] {','});
        //            //    string wherein = "";
        //            //    where += " AND " + mappingColumn + " IN ( " + wherein + ")";
        //            //}

        //        }
        //        foreach (string mappingColumnName in checkboxListValuesPerMappingColumn.Keys)
        //        {
        //            string whereInPart = checkboxListValuesPerMappingColumn[mappingColumn];
        //            where += " AND " + mappingColumnName + " IN ( " + whereInPart + ")";
        //        }

        //    }
        //    return Publish2();
        //}

        //private string GetFilterControlValue(string controlType, string mappingColumn, object value)
        //{
        //    string whereStatement = "";
        //    //Zoek juiste Control type bij de control.
        //    switch (controlType)
        //    {
        //        case "CheckBox":
        //            //Is het een valide control voeg dan indien nodig de AND operator toe aan de filterColumn query.
        //            whereStatement += " AND " + mappingColumn + " = " + value.ToString();
        //            break;

        //        case "CheckBoxList":
        //        case "RadioButtonList":
        //        case "DropDownList":
        //            //ListControl listControl = ((ListControl)control);
        //            //string values = "";
        //            //foreach (ListItem li in listControl.Items)
        //            //{
        //            //    if (li.Selected && li.Value != Guid.Empty.ToString())
        //            //    {
        //            //        //Zoek alle geselecteerde Items. Bij checkboxlist kunnen dit meerdere items zijn. Bij radiobuttonlist maar 1.
        //            //        values += li.Value + ",";
        //            //    }
        //            //}
        //            //if (values != "")
        //            //{
        //            //    values = values.Substring(0, values.Length - 1);
        //            //    if (dataField.FieldType == FieldTypeEnum.DropDown)
        //            //    {
        //            //        //LookupValue[X] heet in de database FK_[X]
        //            //        whereStatement += " AND " + dataField.MappingColumn.Replace("LookupValue", "FK_") + " IN (" + values + ")";
        //            //    }
        //            //    else
        //            //    {
        //            //        foreach (string value in Regex.Split(values, ","))
        //            //        {
        //            //            whereStatement += " AND EXISTS (SELECT * FROM datalookupvalueperitem WHERE FK_LookupValue = '" + value + "' AND dataitem.ID = datalookupvalueperitem.FK_Item) ";
        //            //        }
        //            //        //filterColumn += " EXISTS (SELECT * FROM datalookupvalueperitem WHERE FK_LookupValue IN (" + values + ") AND dataitem.ID = datalookupvalueperitem.FK_Item) ";
        //            //    }
        //            //}
        //            break;

        //        case "TextBox":
        //            //Zoeken in specifiek veld.
        //            //TextBox textBox = (TextBox)control;
        //            //if (textBox.Text.Trim() != "")
        //            //{
        //            //    whereStatement += " AND dataitem." + dataField.MappingColumn + " LIKE '%" + textBox.Text + "%'";
        //            //}
        //            break;
        //    }
        //    return whereStatement;
        //}

    }
}
