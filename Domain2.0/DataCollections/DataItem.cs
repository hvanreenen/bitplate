using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Autorisation;

namespace BitPlate.Domain.DataCollections
{

    public class DataItem : BaseDataObject
    {
        /// <summary>
        /// Voor googlemaps
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Voor googlemaps
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// Voor googlemaps: Na zoeken vanuit een adres, wordt afstand berekend, wordt in nonpersitent veld opgeslagen
        /// </summary>
        [NonPersistent()]
        public double Distance { get; set; }


        private BaseCollection<DataItemLanguage> _dataItemLanguages;
        [Persistent("DataItemLanguage")]
        [Association("FK_Item")]
        public BaseCollection<DataItemLanguage> DataItemLanguages
        {
            get
            {
                //(DataCollection is leeg gemaakt  bij downloaden van items lite voor googlemaps module
                if (DataCollection == null || (DataCollection.LanguageCode != "" && DataCollection.LanguageCode != null))
                {
                    //datacollectie heeft vaste taal: geen talen per item
                    return null;
                }
                else
                {
                    if (_dataItemLanguages == null || (_dataItemLanguages != null && !_dataItemLanguages.IsLoaded))
                    {
                        _dataItemLanguages = BaseCollection<DataItemLanguage>.Get("FK_DataItem='" + this.ID + "'");
                        _dataItemLanguages.IsLoaded = true;
                    }
                    foreach (CmsLanguage language in Site.Languages)
                    {
                        if (Site.DefaultLanguage != language.LanguageCode) //standaard taal niet toevoegen aan talencollectie
                        //standaard taal wordt in velden van het item zelf opgeslagen 
                        {
                            //mochten er later talen worden toegevoegd aan de site, dan moeten ze in item beschikbaar zijn
                            //hieronder wordt gekeken of er nieuwe talen zijn
                            bool found = false;
                            if (_dataItemLanguages != null)
                            {
                                foreach (DataItemLanguage itemLanguage in _dataItemLanguages)
                                {
                                    if (itemLanguage.LanguageCode == language.LanguageCode)
                                    {
                                        found = true;
                                    }
                                }
                            }
                            if (!found)
                            {
                                DataItemLanguage itemLanguage = new DataItemLanguage();
                                itemLanguage.LanguageCode = language.LanguageCode;
                                //itemLanguage.Html1 = "testhtml1" + language.LanguageCode;
                                //itemLanguage.Text1 = "testtext1" + language.LanguageCode;
                                _dataItemLanguages.Add(itemLanguage);
                            }
                        }
                    }
                    return _dataItemLanguages;
                }
            }
            set
            {
                _dataItemLanguages = value;
                //todo: FIX misschien moet dit anders HJ
                if (_dataItemLanguages != null) _dataItemLanguages.IsLoaded = true;
            }
        }

        public static DataItem LoadNew(bool isActive)
        {
            DataItem item = new DataItem();
            if (!isActive)
            {
                item.DateTill = DateTime.Today.AddDays(-1);
            }
            return item;
        }

        public override void Save()
        {
            if (!IsNew)
            {
                //kijk of item naar andere groep is verhuisd
                DataItem itemFromDB = DataItem.GetById<DataItem>(this.ID);
                Guid parentGroupIDFromDB = itemFromDB.ParentGroup != null ? itemFromDB.ParentGroup.ID : Guid.Empty;
                Guid parentGroupIDFromWebForm = this.ParentGroup != null ? this.ParentGroup.ID : Guid.Empty;

                if (parentGroupIDFromDB != parentGroupIDFromWebForm)
                {
                    //verhuisd naar andere groep --> volgnummers updaten. Item komt onderaan:
                    if (this.ParentGroup != null)
                    {
                        this.OrderNumber = this.ParentGroup.GetMaxItemOrderNumber() + 1;
                    }
                    else
                    {
                        //root
                        this.OrderNumber = this.DataCollection.GetMaxItemOrderNumber() + 1;
                    }

                    //er komt gat in lijst waar die vandaan kwam: alles wat hoger is 1 plek naar beneden zetten
                    if (itemFromDB.ParentGroup != null)
                    {
                        string sql = String.Format("UPDATE DataItem SET OrderNumber = OrderNumber - 1 WHERE OrderNumber > {0} AND FK_Parent_Group = '{1}'", itemFromDB.OrderNumber, itemFromDB.ParentGroup.ID);
                        DataBase.Get().Execute(sql);
                    }
                    else
                    {
                        string sql = String.Format("UPDATE DataItem SET OrderNumber = OrderNumber - 1 WHERE OrderNumber > {0} AND FK_Parent_Group IS NULL AND FK_Datacollection='{1}'", itemFromDB.OrderNumber, itemFromDB.DataCollection.ID);
                        DataBase.Get().Execute(sql);
                    }
                }
            }
            if (this.DataItemLanguages != null)
            {
                foreach (DataItemLanguage lang in this.DataItemLanguages)
                {
                    if (lang.Image1 != null) lang.Image1 = lang.Image1.Replace(Site.DomainName + "/", "");
                    if (lang.Image2 != null) lang.Image2 = lang.Image2.Replace(Site.DomainName + "/", "");
                    if (lang.Image3 != null) lang.Image3 = lang.Image3.Replace(Site.DomainName + "/", "");
                    if (lang.Image4 != null) lang.Image4 = lang.Image4.Replace(Site.DomainName + "/", "");
                    if (lang.Image5 != null) lang.Image5 = lang.Image5.Replace(Site.DomainName + "/", "");
                }
            }

            if (this.DataCollection.Type == DataCollectionTypeEnum.GoogleMaps)
            {
                string googleMapsKey = Site.GoogleMapsKey;
                //Utils.GPoint point = Utils.GoogleGeocoder.GetLatLng(this.Address, this.PostalCode, this.Place, this.Country, googleMapsKey);
                Utils.GPoint point = Utils.GoogleGeocoder.GetLatLng(this.Text1, this.Text2, this.Text3, this.Text4, googleMapsKey);
                
                this.Latitude = point.Lat;
                this.Longitude = point.Long;
            }
            base.Save();
            UnpublishedItem.Set(this, "DataItem");
        }

        private SiteUser _user;

        [Association("FK_User")]
        public SiteUser User
        {
            get
            {
                if (_user != null && !_user.IsLoaded)
                {
                    this._user.Load();
                }
                return this._user;
            }
            set
            {
                this._user = value;
            }
        }

        private BitplateUser _bitplateUser;

        [Association("FK_BitplateUser")]
        public BitplateUser BitplateUser
        {
            get
            {
                if (_bitplateUser != null && !_bitplateUser.IsLoaded)
                {
                    this._bitplateUser.Load();
                }
                return this._bitplateUser;
            }
            set
            {
                this._bitplateUser = value;
            }
        }

        public override void Delete()
        {
            if (this.ParentGroup != null)
            {
                //update ordering numbers
                //er komt gat in lijst: alles wat hoger is 1 plek naar beneden zetten
                string sql = String.Format("UPDATE DataItem SET OrderNumber = OrderNumber - 1 WHERE OrderNumber > {0} AND FK_Parent_Group = '{1}'", this.OrderNumber, this.ParentGroup.ID);
                DataBase.Get().Execute(sql);
            }
            else
            {
                //update ordering numbers
                //er komt gat in lijst: alles wat hoger is 1 plek naar beneden zetten
                string sql = String.Format("UPDATE DataItem SET OrderNumber = OrderNumber - 1 WHERE OrderNumber > {0} AND FK_Parent_Group IS NULL AND FK_Datacollection='{1}'", this.OrderNumber, this.DataCollection.ID);
                DataBase.Get().Execute(sql);
            }
            base.Delete();
            UnpublishedItem.Set(this, "DataItem", ChangeStatusEnum.Deleted);
        }

        public void Copy(string newName, Guid? dataCollectionID, Guid? parentGroupID, bool updateOrderNumber = false)
        {

            DataItem newItem = this.CreateCopy<DataItem>(false);
            if (newName != "")
            {
                newItem.Name = newName;
            }
            newItem.LastPublishedDate = null;
           
            if (dataCollectionID.HasValue)
            {
                newItem.DataCollection = new DataCollection();
                newItem.DataCollection.ID = dataCollectionID.Value;
            }
            if (parentGroupID.HasValue)
            {
                newItem.ParentGroup = new DataGroup();
                newItem.ParentGroup.ID = parentGroupID.Value;
            }
            if (updateOrderNumber)
            {
                //gekopieerde item krijgt 1 plek hoger volgnummer
                newItem.OrderNumber++;
            }
            newItem.Save();

            if (updateOrderNumber)
            {
                //de rest van de items moeten 1 plek opschuiven om plaats te maken
                int newOrderNumber = newItem.OrderNumber;
                if (newItem.ParentGroup != null)
                {
                    string sql = String.Format("UPDATE DataItem SET OrderNumber = OrderNumber + 1 WHERE OrderNumber >= {0} AND ID != '{1} ' AND FK_Parent_Group = '{2}'", newOrderNumber, newItem.ID, this.ParentGroup.ID);
                    DataBase.Get().Execute(sql);
                }
                else
                {
                    string sql = String.Format("UPDATE DataItem SET OrderNumber = OrderNumber + 1 WHERE OrderNumber >= {0} AND ID != '{1} ' AND FK_DataCollection = '{2}' AND FK_Parent_Group IS NULL", newOrderNumber, newItem.ID, this.DataCollection.ID);
                    DataBase.Get().Execute(sql);
                }
            }
            if (DataItemLanguages != null)
            {
                foreach (DataItemLanguage languageItem in DataItemLanguages)
                {
                    DataItemLanguage newLanguageItem = languageItem.CreateCopy<DataItemLanguage>(false);
                    newLanguageItem.DataItem = newItem;
                    newLanguageItem.Save();
                }
            }
        }


        internal string GetGroupName()
        {
            if (_parentGroup != null && !_parentGroup.IsLoaded)
            {
                _parentGroup.Load();
            }
            return _parentGroup.Name;
        }

        internal void Publish(bool reGenerateSearchIndex)
        {
            string sql = String.Format(@"DELETE FROM DataItem_Live WHERE ID = '{0}'; 
INSERT DataItem_Live SELECT * FROM DataItem WHERE ID = '{0}';
", this.ID);
            DataBase.Get().Execute(sql);
            base.SavePublishInfo();
        }
        /// <summary>
        /// zet alle waardes van een DataItemLanguage regel in de DataItem
        /// Hierdoor kunnen dezelfde velden worden gebruikt in de dataModule
        /// </summary>
        /// <param name="languageCode"></param>
        public void Translate(string languageCode)
        {
            if (DataCollection.HasStaticLanguage || languageCode == Site.DefaultLanguage)
            {
                return;
            }
            string where = "FK_DataItem='" + this.ID + "' AND LanguageCode ='" + languageCode + "'";
            DataItemLanguage itemLanguage = DataItemLanguage.GetFirst<DataItemLanguage>(where);
            if (itemLanguage == null) return;
            this.Title = itemLanguage.Title;
            foreach (DataField fld in this.DataCollection.DataItemFields)
            {
                if (fld.IsMultiLanguageField)
                {
                    if (fld.FieldType != FieldTypeEnum.ImageList &&
                    fld.FieldType != FieldTypeEnum.FileList &&
                    fld.FieldType != FieldTypeEnum.DropDown)
                    {
                        System.Reflection.PropertyInfo propItem = this.GetType().GetProperty(fld.MappingColumn);

                        System.Reflection.PropertyInfo propLanguage = itemLanguage.GetType().GetProperty(fld.MappingColumn);

                        Object valueLanguage = propLanguage.GetValue(itemLanguage, null);
                        propItem.SetValue(this, valueLanguage, null);

                    }
                    else if (fld.FieldType == FieldTypeEnum.ImageList )
                    {
                        _imageList1 = BaseCollection<DataFile>.Get("Type = '" + DataFile.TYPE_EXTRA_IMAGE + "' AND FK_Item='" + this.ID + "' AND Language='" + languageCode + "'", "Name");
                        _imageList1.IsLoaded = true;
                    }
                    else if (fld.FieldType == FieldTypeEnum.FileList)
                    {
                        _imageList1 = BaseCollection<DataFile>.Get("Type = '" + DataFile.TYPE_EXTRA_FILE + "' AND FK_Item='" + this.ID + "' AND Language='" + languageCode + "'", "Name");
                        _imageList1.IsLoaded = true;
                    }
                    else if (fld.FieldType == FieldTypeEnum.DropDown)
                    {
                        //todo
                    }
                }
            }
        }


        BaseCollection<DataLookupValue> _lookupValues1;
        [Persistent("datalookupvalueperitem")]
        [Association("FK_Item", "FK_LookupValue")]
        public BaseCollection<DataLookupValue> LookupValues1
        {
            get
            {
                if (_lookupValues1 == null || (_lookupValues1 != null && !_lookupValues1.IsLoaded))
                {
                    string where = "EXISTS (SELECT * FROM datalookupvalueperitem WHERE FK_Item = '" + this.ID + "' AND datalookupvalue.ID = datalookupvalueperitem.FK_LookupValue)";
                    _lookupValues1 = BaseCollection<DataLookupValue>.Get(where, "Name");
                    _lookupValues1.IsLoaded = true;
                }

                return _lookupValues1;
            }
            set
            {
                _lookupValues1 = value;
                _lookupValues1.IsLoaded = true;
            }
        }

        /* BaseCollection<DataLookupValue> _lookupValues2;
        [Persistent("datalookupvalueperitem")]
        [Association("FK_Item", "FK_LookupValue")]
        public BaseCollection<DataLookupValue> LookupValues2
        {
            get
            {
                if (_lookupValues2 == null || (_lookupValues2 != null && !_lookupValues2.IsLoaded))
                {
                    string where = "EXISTS (SELECT * FROM datalookupvalueperitem JOIN datalookupvalue ON datalookupvalueperitem.FK_LookupValue = datalookupvalue.ID JOIN datafield ON datafield.ID = datalookupvalue.FK_datafield WHERE FK_Item = '" + this.ID + "' AND datalookupvalue.ID = datalookupvalueperitem.FK_LookupValue  AND datafield.mappingcolumn = 'lookupvalues2')";
                    _lookupValues2 = BaseCollection<DataLookupValue>.Get(where, "Name");
                    _lookupValues2.IsLoaded = true;
                }

                return _lookupValues2;
            }
            set
            {
                _lookupValues2 = value;
                _lookupValues2.IsLoaded = true;
            }
        }

        BaseCollection<DataLookupValue> _lookupValues3;
        [Persistent("datalookupvalueperitem")]
        [Association("FK_Item", "FK_LookupValue")]
        public BaseCollection<DataLookupValue> LookupValues3
        {
            get
            {
                if (_lookupValues3 == null || (_lookupValues3 != null && !_lookupValues3.IsLoaded))
                {
                    string where = "EXISTS (SELECT * FROM datalookupvalueperitem JOIN datalookupvalue ON datalookupvalueperitem.FK_LookupValue = datalookupvalue.ID JOIN datafield ON datafield.ID = datalookupvalue.FK_datafield WHERE FK_Item = '" + this.ID + "' AND datalookupvalue.ID = datalookupvalueperitem.FK_LookupValue  AND datafield.mappingcolumn = 'lookupvalues3')";
                    _lookupValues3 = BaseCollection<DataLookupValue>.Get(where, "Name");
                    _lookupValues3.IsLoaded = true;
                }

                return _lookupValues3;
            }
            set
            {
                _lookupValues3 = value;
                _lookupValues3.IsLoaded = true;
            }
        } */

        
        public override void FillObject(System.Data.DataRow dataRow)
        {

            base.FillObject(dataRow);

            if (dataRow.Table.Columns.Contains("Latitude")) this.Latitude = DataConverter.ToDouble(dataRow["Latitude"]);
            if (dataRow.Table.Columns.Contains("Longitude")) this.Longitude = DataConverter.ToDouble(dataRow["Longitude"]);
            this.IsLoaded = true;
        }
    
       

    }
}
