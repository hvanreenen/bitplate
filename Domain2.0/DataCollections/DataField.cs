using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Utils;
using BitPlate.Domain.Modules;
using System.Web.Script.Serialization;


namespace BitPlate.Domain.DataCollections
{
    public enum FieldTypeEnum
    {
        Text = 1,
        LongText = 2,
        Html = 3,
        Numeric = 4,
        Currency = 5,
        YesNo = 6,
        DropDown = 7,
        Image = 8,
        File = 9,
        ImageList = 10,
        FileList = 11,
        DateTime = 12,
        ReadOnly = 13,
        CheckboxList = 14
    }

    public enum FieldRuleEnum {
        None = 0,
        Required =1,
        ReadOnly = 2
    }

    public class DataField : BaseDomainSiteObject
    {
        public string Type { get; set; } //group of item
        public FieldTypeEnum FieldType { get; set; }
        [NonPersistent()]
        public string FieldTypeString
        {
            get
            {
                //Name() method van enum = extension method in Utils.TypeExtensions.cs
                return FieldType.Name();
            }
        }
        public string MappingColumn { get; set; }
        public bool IsMultiLanguageField { get; set; }
        [NonPersistent()]
        public string IsMultiLanguageFieldString
        {
            get
            {
                return IsMultiLanguageField.Name(); // ? "Ja" : "Nee";
            }
        }
        public bool IsSystemField { get; set; }
        public int OrderingNumber { get; set; }
        public string TabPage { get; set; }
        public string DefaultValue { get; set; }
        public FieldRuleEnum FieldRule { get; set; }

        private DataCollection _dataCollection;
        [Association("FK_DataCollection")]
        [ScriptIgnore]
        public virtual DataCollection DataCollection
        {
            get
            {
                if (_dataCollection != null && !_dataCollection.IsLoaded)
                {
                    _dataCollection.Load();
                }
                return _dataCollection;
            }
            set { _dataCollection = value; }
        }

        private BaseCollection<DataLookupValue> _lookupValues;
        /// <summary>
        /// Voor DropDowns kun je waarden invullen
        /// </summary>
        [Association("FK_DataField")]
        public BaseCollection<DataLookupValue> LookupValues
        {
            get
            {
                if (_lookupValues == null)
                {
                    _lookupValues = BaseCollection<DataLookupValue>.Get("FK_DataField='" + this.ID + "'", "OrderingNumber, Name");
                }
                return _lookupValues;
            }
            set { _lookupValues = value; }
        }
        ///// <summary>
        ///// Voor in de datamodules
        ///// Tag wordt vervangen door stukje html
        ///// Dit is afhankelijk van het type
        ///// </summary>
        //public string GetHtmlReplaceTag(ModeEnum mode, bool DataCollectionValue = false, bool fromParent = false)
        //{
        //    string html = "";
        //    string dataField = (fromParent) ? "ParentGroup." + this.MappingColumn : this.MappingColumn;
        //    if (!DataCollectionValue)
        //    {
        //        html = "<div data-field='" + dataField + "' ></div>";
        //        if (this.FieldType == FieldTypeEnum.Image)
        //        {
        //            if (mode == ModeEnum.Edit)
        //            {
        //                html = "<img src='" + Site.DomainName + "/[data-field]' data-field='" + dataField + "' />";
        //            }
        //            else
        //            {
        //                html = "<img src='null' data-field='" + dataField + "' />";
        //            }
                    
        //        }
        //        else if (this.FieldType == FieldTypeEnum.Html)
        //        {
        //            html = "<div data-field='" + dataField + "' ></div>";
        //        }
        //        else if (this.FieldType == FieldTypeEnum.DropDown)
        //        {
        //            html = String.Format("<span data-field='{0}.Name'></span>", dataField);
        //        }
        //        else if (this.FieldType == FieldTypeEnum.ImageList)
        //        {
        //            html = "<div class=\"ImageList\" data-control-type=\"list\">{CollectionList}</div>";
        //        }
        //    }
        //    else
        //    {
        //        html = dataField;
        //    }
            
         
        //    //TODO andere types
        //    return html;
        //}

        ///// <summary>
        ///// Voor in de datamodules
        ///// Tag wordt vervangen door stukje html
        ///// Dit is afhankelijk van het type
        ///// </summary>
        //public string GetHtmlInputReplaceTag()
        //{
        //    string InputHtml = "";

        //    switch (this.FieldType)
        //    {
        //        case FieldTypeEnum.Text:
        //            InputHtml = "<input type=\"text\" name=\"" + this.Name + "\" data-field=\"" + this.MappingColumn + "\" />";
        //            break;

        //        case FieldTypeEnum.Numeric:
        //            InputHtml = "<input data-validation=\"number\" type=\"text\" name=\"" + this.Name + "\" data-field=\"" + this.MappingColumn + "\" />";
        //            break;

        //        case FieldTypeEnum.YesNo:
        //            InputHtml = "<input type=\"checkbox\" name=\"" + this.Name + "\" data-field=\"" + this.MappingColumn + "\" />";
        //            break;

        //        case FieldTypeEnum.LongText:
        //            InputHtml = "<textarea  name=\"" + this.Name + "\" data-field=\"" + this.MappingColumn + "\" ></textarea>";
        //            break;

        //        default:
        //            InputHtml = "<input type=\"text\" name=\"" + this.Name + "\" data-field=\"" + this.MappingColumn + "\" />";
        //            break;
        //    }

        //    return InputHtml;
        //}

        public override void Save()
        {
            if (this.Type == "group")
            {
                if (IsNew)
                {
                    if (!IsSystemField)
                    {
                        this.MappingColumn = this.DataCollection.GetNewGroupMappingColumn(this.FieldType);
                    }
                }
                else
                {
                    //kijk of type is gewijzigd
                    DataField fieldFromDB = BaseObject.GetById<DataField>(this.ID);
                    
                    if (fieldFromDB.FieldType != this.FieldType)
                    {
                        //Controleer datafield types. 
                        ValidateNewFieldType(fieldFromDB);

                        //zoja dataupdaten
                        string newMappingColumn = this.DataCollection.GetNewGroupMappingColumn(this.FieldType);
                        string sql = string.Format("UPDATE DataGroup SET {0} = {1}, {1} = NULL WHERE FK_DATACOLLECTION = '{2}'", newMappingColumn, fieldFromDB.MappingColumn, this.DataCollection.ID);
                        DataBase.Get().Execute(sql);
                        this.MappingColumn = newMappingColumn;
                    }

                }
            }
            else
            {
                if (IsNew)
                {
                    if (!IsSystemField)
                    {
                        this.MappingColumn = this.DataCollection.GetNewItemMappingColumn(this.FieldType);
                    }
                }
                else
                {
                    //kijk of type is gewijzigd
                    DataField fieldFromDB = BaseObject.GetById<DataField>(this.ID);
                    
                    if (fieldFromDB.FieldType != this.FieldType)
                    {
                        //if (fieldFromDB.FieldType == FieldTypeEnum.ImageList ||
                        //fieldFromDB.FieldType == FieldTypeEnum.FileList ||
                        //this.FieldType == FieldTypeEnum.ImageList ||
                        //this.FieldType == FieldTypeEnum.FileList)
                        //{
                        //    throw new Exception("Kan veldtypes niet wijzigen van en naar afbeelingenlijsten of bestandenlijsten.");
                        //}
                        //Controleer datafield types. 
                        ValidateNewFieldType(fieldFromDB);
                        //zoja dataupdaten
                        string newMappingColumn = this.DataCollection.GetNewItemMappingColumn(this.FieldType);
                        string sql = string.Format("UPDATE DataItem SET {0} = {1}, {1} = NULL WHERE FK_DATACOLLECTION = '{2}'", newMappingColumn, fieldFromDB.MappingColumn, this.DataCollection.ID);
                        DataBase.Get().Execute(sql);
                        this.MappingColumn = newMappingColumn;
                    }
                }
            }
            base.Save();

            UnpublishedItem.Set(this.DataCollection, "DataCollection");
        }

        private void ValidateNewFieldType(DataField fieldFromDB)
        {
            if (fieldFromDB.FieldType == FieldTypeEnum.ImageList ||
            fieldFromDB.FieldType == FieldTypeEnum.FileList ||
            this.FieldType == FieldTypeEnum.ImageList ||
            this.FieldType == FieldTypeEnum.FileList)
            {
                throw new Exception("Kan veldtypes niet wijzigen van en naar afbeelingenlijsten of bestandenlijsten.");
            }

            if (fieldFromDB.FieldType == FieldTypeEnum.DropDown ||
                fieldFromDB.FieldType == FieldTypeEnum.CheckboxList ||
                this.FieldType == FieldTypeEnum.DropDown ||
                this.FieldType == FieldTypeEnum.CheckboxList)
            {
                throw new Exception("Kan veldtypes niet wijzigen van en naar dropdownlijst of checkboxlijst.");
            }
        }
        public override void Delete()
        {
            //bij verwijderen van een veld ook alle waardes leegmaken
            //bij imagelist en filelist moet de koppel tabel leeg
            if (this.FieldType == FieldTypeEnum.ImageList)
            {
                //alles van items weg
                string sql = string.Format("DELETE FROM DataFile WHERE Type='ExtraImage' AND EXISTS (SELECT * FROM DataItem WHERE DataItem.ID = DataFile.FK_Item AND DataItem.FK_DataCollection = '{0}')", this.DataCollection.ID);
                DataBase.Get().Execute(sql);
                //alles van groepen weg
                sql = string.Format("DELETE FROM DataFile WHERE Type='ExtraImage' AND EXISTS (SELECT * FROM DataGroup WHERE DataGroup.ID = DataFile.FK_Group AND DataGroup.FK_DataCollection = '{0}')", this.DataCollection.ID);
                DataBase.Get().Execute(sql);
            }
            else if (this.FieldType == FieldTypeEnum.FileList)
            {
                //alles van items weg
                string sql = string.Format("DELETE FROM DataFile WHERE Type='ExtraFile' AND EXISTS (SELECT * FROM DataItem WHERE DataItem.ID = DataFile.FK_Item AND DataItem.FK_DataCollection = '{0}')", this.DataCollection.ID);
                DataBase.Get().Execute(sql);
                //alles van groepen weg
                sql = string.Format("DELETE FROM DataFile WHERE Type='ExtraFile' AND EXISTS (SELECT * FROM DataGroup WHERE DataGroup.ID = DataFile.FK_Group AND DataGroup.FK_DataCollection = '{0}')", this.DataCollection.ID);
                DataBase.Get().Execute(sql);
            }
            else if (this.FieldType == FieldTypeEnum.CheckboxList || this.FieldType == FieldTypeEnum.DropDown)
            {

                // doe niks 
                //DB regelt dit via Cascade DELETE

            }
            else
            {
                string sql = "";

                if (this.Type == "group")
                {
                    sql = string.Format("UPDATE DataGroup SET {0} = null WHERE FK_DATACOLLECTION = '{1}'", this.MappingColumn, this.DataCollection.ID);
                    DataBase.Get().Execute(sql);
                }
                else
                {
                    sql = string.Format("UPDATE DataItem SET {0} = null WHERE FK_DATACOLLECTION = '{1}'", this.MappingColumn, this.DataCollection.ID);
                    DataBase.Get().Execute(sql);
                }
            }
            base.Delete();
        }

        public void Copy(Guid newCollectionID, Guid siteID, string newCollectionName)
        {
            DataField newField = this.CreateCopy<DataField>(false);
            newField.DataCollection = new DataCollection();
            newField.DataCollection.ID = newCollectionID;
            newField.DataCollection.Name = newCollectionName;
            newField.DataCollection.Site = new CmsSite();
            newField.DataCollection.Site.ID = siteID;
            newField.Save();

            if (newField.FieldType == FieldTypeEnum.DropDown)
            {
                foreach (DataLookupValue lookup in this.LookupValues)
                {
                    lookup.Copy(newField.ID);
                }
            }
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            this.Type = dataRow["Type"].ToString();
            this.FieldType = (FieldTypeEnum)DataConverter.ToInt32(dataRow["FieldType"]);
            this.MappingColumn = dataRow["MappingColumn"].ToString();
            this.IsMultiLanguageField = DataConverter.ToBoolean(dataRow["IsMultiLanguageField"]);
            this.IsSystemField = DataConverter.ToBoolean(dataRow["IsSystemField"]);
            this.OrderingNumber = DataConverter.ToInt32(dataRow["OrderingNumber"]);
            this.TabPage = dataRow["TabPage"].ToString();
            this.DefaultValue = dataRow["DefaultValue"].ToString();
            this.FieldRule = (FieldRuleEnum)int.Parse(dataRow["FieldRule"].ToString());
            //Kan deze regel anders?
            this.DataCollection = BaseObject.GetById<DataCollection>(Guid.Parse(dataRow["FK_DataCollection"].ToString()));
            this.IsLoaded = true;
        }
    }
}
