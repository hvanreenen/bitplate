using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using HJORM;
using HJORM.Attributes;

using BitPlate.Domain.Modules;

namespace BitPlate.Domain.DataCollections
{

    public class DataGroup : BaseDataObject
    {

        public static DataGroup LoadNew(bool isActive)
        {
            DataGroup group = new DataGroup();
            if (!isActive)
            {
                group.DateTill = DateTime.Today.AddDays(-1);
            }
            return group;
        }

        private BaseCollection<DataGroup> _subGroups;
        //[NonPersistent()]
        //[System.Web.Script.Serialization.ScriptIgnore()]
        public BaseCollection<DataGroup> GetSubGroups()
        {

            _subGroups = BaseCollection<DataGroup>.Get("FK_Parent_Group='" + this.ID + "'", "OrderNumber, CreateDate");
            return _subGroups;

        }

        private BaseCollection<DataItem> _items;
        //[NonPersistent()]
        //[System.Web.Script.Serialization.ScriptIgnore()]
        public BaseCollection<DataItem> GetItems()
        {

            _items = BaseCollection<DataItem>.Get("FK_Parent_Group='" + this.ID + "'", "OrderNumber, CreateDate");
            return _items;
        }

        [Persistent("DataFile")]
        [Association("FK_Group")]
        public override BaseCollection<DataFile> ImageList1
        {
            get
            {
                if (_imageList1 == null || (_imageList1 != null && !_imageList1.IsLoaded))
                {
                    _imageList1 = BaseCollection<DataFile>.Get("Type = '" + DataFile.TYPE_EXTRA_IMAGE + "' AND FK_Group='" + this.ID + "'", "Name");
                    _imageList1.IsLoaded = true;
                }
                return _imageList1;
            }
            set { _imageList1 = value; }
        }

        [Persistent("DataFile")]
        [Association("FK_Group")]
        public override BaseCollection<DataFile> FileList1
        {
            get
            {
                if (_fileList1 == null || (_fileList1 != null && !_fileList1.IsLoaded))
                {
                    string sessionLanguage = "";
                    if (HttpContext.Current.Session["CurrentLanguage"] != null)
                    {
                        sessionLanguage = HttpContext.Current.Session["CurrentLanguage"].ToString();
                    }

                    if (sessionLanguage != "")
                    {
                        _fileList1 = BaseCollection<DataFile>.Get("Type = '" + DataFile.TYPE_EXTRA_FILE + "' AND FK_Group='" + this.ID + "' AND Language = '" + sessionLanguage + "'", "Name");
                    }
                    else
                    {
                        _fileList1 = BaseCollection<DataFile>.Get("Type = '" + DataFile.TYPE_EXTRA_FILE + "' AND FK_Group='" + this.ID + "'", "Name");
                    }
                }
                return _fileList1;
            }
            set { _fileList1 = value; }
        }

        public override void Save()
        {
            base.ValidateGroupPath();
            if (!IsNew)
            {
                //kijk of item naar andere groep is verhuisd
                DataGroup groupFromDB = DataGroup.GetById<DataGroup>(this.ID);
                Guid parentGroupIDFromDB = groupFromDB.ParentGroup != null ? groupFromDB.ParentGroup.ID : Guid.Empty;
                Guid parentGroupIDFromWebForm = this.ParentGroup != null ? this.ParentGroup.ID : Guid.Empty;

                if (parentGroupIDFromDB != parentGroupIDFromWebForm)
                {
                    //verhuisd naar andere groep --> volgnummers updaten. group komt onderaan:
                    if (this.ParentGroup != null)
                    {
                        this.OrderNumber = this.ParentGroup.GetMaxGroupOrderNumber() + 1;
                    }
                    else
                    {
                        //root
                        this.OrderNumber = this.DataCollection.GetMaxGroupOrderNumber() + 1;
                    }

                    //er komt gat in lijst waar die vandaan kwam: alles wat hoger is 1 plek naar beneden zetten
                    if (groupFromDB.ParentGroup != null)
                    {
                        string sql = String.Format("UPDATE DataGroup SET OrderNumber = OrderNumber - 1 WHERE OrderNumber > {0} AND FK_Parent_Group = '{1}'", groupFromDB.OrderNumber, groupFromDB.ParentGroup.ID);
                        DataBase.Get().Execute(sql);
                    }
                    else
                    {
                        string sql = String.Format("UPDATE DataGroup SET OrderNumber = OrderNumber - 1 WHERE OrderNumber > {0} AND FK_Parent_Group IS NULL AND FK_Datacollection='{1}'", groupFromDB.OrderNumber, groupFromDB.DataCollection.ID);
                        DataBase.Get().Execute(sql);
                    }
                }
            }
            base.Save();
            UnpublishedItem.Set(this, "DataGroup");
            //Save(true);
        }

        public override void Delete()
        {
            if (this.ParentGroup != null)
            {
                //update ordering numbers
                //er komt gat in lijst: alles wat hoger is 1 plek naar beneden zetten
                string sql = String.Format("UPDATE DataGroup SET OrderNumber = OrderNumber - 1 WHERE OrderNumber > {0} AND FK_Parent_Group = '{1}'", this.OrderNumber, this.ParentGroup.ID);
                DataBase.Get().Execute(sql);
            }
            else
            {
                //update ordering numbers
                //er komt gat in lijst: alles wat hoger is 1 plek naar beneden zetten
                string sql = String.Format("UPDATE DataGroup SET OrderNumber = OrderNumber - 1 WHERE OrderNumber > {0} AND FK_Parent_Group IS NULL AND FK_Datacollection='{1}'", this.OrderNumber, this.DataCollection.ID);
                DataBase.Get().Execute(sql);
            }
            base.Delete();
            UnpublishedItem.Set(this, "DataGroup", ChangeStatusEnum.Deleted);
        }

        //        public void MoveItemsAndGroups(int fromOrderingNumber, int? tillOrderingNumber, MoveDirectionEnum direction)
        //        {
        //            //zowel groepen als items krijgen volgorde in de tree
        //            //union query waarbij groepen en item in 1 query staan.
        //            //resultaat van query wordt in BaseDataObject gezet.
        //            //veld title misbruiken om hier type (groep of item) in te bewaren 
        //            //alleen de lagere nummers hoef je te updaten  (OrderingNumber <= orderingNumber)
        //            //krijgen allemaal 1 plekje lager, totaan tillOrderingNumber
        //            //ook subgroepen moeten worden geupdate, want hier wordt nieuwe hierarchie in nummering weergegeven
        //            string fromOrderingNumberString = this.OrderingNumber + "." + fromOrderingNumber.ToString("0000");
        //            string tillWhereClause = "";
        //            if (tillOrderingNumber.HasValue)
        //            {
        //                string tillOrderingNumberString = this.OrderingNumber + "." + tillOrderingNumber.Value.ToString("0000");
        //                tillWhereClause = String.Format(" AND OrderingNumber <= '{0}'", tillOrderingNumberString);
        //            }

        //            string sql = String.Format(@"SELECT ID, Name, 'Item' AS Title, OrderingNumber From DataItem WHERE FK_Parent_Group='{0}' AND OrderingNumber >= '{1}' {2} 
        //UNION SELECT ID, Name, 'Group' AS Title, OrderingNumber From DataGroup WHERE FK_Parent_Group='{0}' AND OrderingNumber >= '{1}' {2} 
        //ORDER BY OrderingNumber, Name", this.ID, fromOrderingNumberString, tillWhereClause);
        //            BaseCollection<BaseDataObject> dataObjects = BaseCollection<BaseDataObject>.LoadFromSql(sql);
        //            int currentOrderingNumber = fromOrderingNumber;
        //            foreach (BaseDataObject dataObject in dataObjects)
        //            {
        //                string newOrderingNumberString = "";
        //                if (direction == MoveDirectionEnum.Down)
        //                {
        //                    newOrderingNumberString = this.OrderingNumber + "." + (currentOrderingNumber - 1).ToString("0000");
        //                }
        //                else
        //                {
        //                    newOrderingNumberString = this.OrderingNumber + "." + (currentOrderingNumber + 1).ToString("0000");
        //                }

        //                string updateSql = String.Format("UPDATE Data{0} SET OrderingNumber='{1}' WHERE ID='{2}'", dataObject.Title, newOrderingNumberString, dataObject.ID);
        //                DataBase.Get().Execute(updateSql);
        //                if (dataObject.Title == "Group")
        //                {
        //                    dataObject.UpdateSubOrderingNumbers(newOrderingNumberString);
        //                }
        //                currentOrderingNumber++;
        //            }
        //        }

        //        public void MoveItemsAndGroupsDown(int fromOrderingNumber, int? tillOrderingNumber)
        //        {

        //            MoveItemsAndGroups(fromOrderingNumber, tillOrderingNumber, MoveDirectionEnum.Down);

        //        }

        //        public void MoveItemsAndGroupsUp(int fromOrderingNumber, int? tillOrderingNumber)

        //        {
        //            MoveItemsAndGroups(fromOrderingNumber, tillOrderingNumber, MoveDirectionEnum.Up);

        //        }

        //        internal string CalculateNewOrderingNumber()
        //        {
        //            string sql = String.Format(@"SELECT ID, Name, 'Item' AS Title, OrderingNumber From DataItem WHERE FK_Parent_Group='{0}' 
        //UNION SELECT ID, Name, 'Group' AS Title, OrderingNumber From DataGroup WHERE FK_Parent_Group='{0}'", this.ID);
        //            BaseCollection<BaseDataObject> dataObjects = BaseCollection<BaseDataObject>.LoadFromSql(sql);
        //            int newOrderNumber = dataObjects.Count + 1;
        //            string newOrderingNumberString = this.OrderingNumber + "." + newOrderNumber.ToString("0000");
        //            return newOrderingNumberString;
        //        }

        public void Copy(string newName, Guid? dataCollectionID, Guid? parentGroupID, bool updateOrderNumber = false)
        {
            DataGroup newGroup = this.CreateCopy<DataGroup>(false);
            if (newName != "")
            {
                newGroup.Name = newName;
            }
            newGroup.LastPublishedDate = null;
            
            if (dataCollectionID.HasValue)
            {
                newGroup.DataCollection = new DataCollection();
                newGroup.DataCollection.ID = dataCollectionID.Value;
            }
            if (parentGroupID.HasValue)
            {
                newGroup.ParentGroup = new DataGroup();
                newGroup.ParentGroup.ID = parentGroupID.Value;
            }
            if (updateOrderNumber)
            {
                //gekopieerde item krijgt 1 plek hoger volgnummer
                newGroup.OrderNumber++;
            }
            newGroup.Save();

            if (updateOrderNumber)
            {
                //de rest van de items moeten 1 plek opschuiven om plaats te maken
                int newOrderNumber = newGroup.OrderNumber;
                if (newGroup.ParentGroup != null)
                {
                    string sql = String.Format("UPDATE DataGroup SET OrderNumber = OrderNumber + 1 WHERE OrderNumber >= {0} AND ID != '{1} ' AND FK_Parent_Group = '{2}'", newOrderNumber, newGroup.ID, this.ParentGroup.ID);
                    DataBase.Get().Execute(sql);
                }
                else
                {
                    string sql = String.Format("UPDATE DataGroup SET OrderNumber = OrderNumber + 1 WHERE OrderNumber >= {0} AND ID != '{1} ' AND FK_DataCollection = '{2}' AND FK_Parent_Group IS NULL", newOrderNumber, newGroup.ID, this.DataCollection.ID);
                    DataBase.Get().Execute(sql);
                }
            }


            foreach (DataItem item in this.GetItems())
            {
                item.Copy(item.Name, newGroup.DataCollection.ID, newGroup.ID);
            }

            foreach (DataGroup subGroup in this.GetSubGroups())
            {
                subGroup.Copy(subGroup.Name, newGroup.DataCollection.ID, newGroup.ID);
            }
        }

        //        internal void Publish(bool reGenerateSearchIndex)
        //        {
        //            string sql = String.Format(@"DELETE FROM DataGroup_Live WHERE ID = '{0}'; 
        //INSERT DataGroup_Live SELECT * FROM DataGroup WHERE ID = '{0}';
        //", this.ID);
        //            DataBase.Get().Execute(sql);

        //            if (reGenerateSearchIndex)
        //            {
        //                Search.SearchHelper.FillSearchIndex(this);
        //            }

        //            base.SavePublishInfo();
        //        }

        public void MoveItems(string itemid, int newOrderingnumber)
        {
            DataItem item = BaseObject.GetById<DataItem>(new Guid(itemid));
            int oldOrderingNumber = item.OrderNumber;
            if (oldOrderingNumber < newOrderingnumber)
            {
                //nummer wordt hoger: alles tussen oude nummer en nieuwe nummer 1 plek lager zetten
                string sql = String.Format("UPDATE DataItem SET OrderNumber = OrderNumber - 1 WHERE OrderNumber > {0} And OrderNumber <= {1} AND FK_Parent_Group = '{2}'", oldOrderingNumber, newOrderingnumber, this.ID);
                DataBase.Get().Execute(sql);
            }
            else if (oldOrderingNumber > newOrderingnumber)
            {
                //nummer wordt lager: alles tussen nieuwe nummer en oude nummer 1 plek verder zetten
                string sql = String.Format("UPDATE DataItem SET OrderNumber = OrderNumber + 1 WHERE OrderNumber < {0} And OrderNumber >= {1} AND FK_Parent_Group = '{2}'", oldOrderingNumber, newOrderingnumber, this.ID);
                DataBase.Get().Execute(sql);
            }
            item.OrderNumber = newOrderingnumber;
            item.Save();
        }

        public void MoveGroups(string groupid, int newOrderingnumber)
        {
            DataGroup group = BaseObject.GetById<DataGroup>(new Guid(groupid));
            int oldOrderingNumber = group.OrderNumber;
            if (oldOrderingNumber < newOrderingnumber)
            {
                //nummer wordt hoger: alles tussen oude nummer en nieuwe nummer 1 plek lager zetten
                string sql = String.Format("UPDATE DataGroup SET OrderNumber = OrderNumber - 1 WHERE OrderNumber > {0} And OrderNumber <= {1} AND FK_Parent_Group = '{2}'", oldOrderingNumber, newOrderingnumber, this.ID);
                DataBase.Get().Execute(sql);
            }
            else if (oldOrderingNumber > newOrderingnumber)
            {
                //nummer wordt lager: alles tussen nieuwe nummer en oude nummer 1 plek verder zetten
                string sql = String.Format("UPDATE DataGroup SET OrderNumber = OrderNumber + 1 WHERE OrderNumber < {0} And OrderNumber >= {1} AND FK_Parent_Group = '{2}'", oldOrderingNumber, newOrderingnumber, this.ID);
                DataBase.Get().Execute(sql);
            }
            group.OrderNumber = newOrderingnumber;
            group.Save();
        }

        public int GetMaxGroupOrderNumber()
        {
            string sql = String.Format("SELECT MAX(OrderNumber) FROM DataGroup WHERE FK_Parent_Group = '{0}'", this.ID);
            object returnValue = DataBase.Get().Execute(sql);
            return Convert.ToInt32(returnValue);
        }

        public int GetMaxItemOrderNumber()
        {
            string sql = String.Format("SELECT MAX(OrderNumber) FROM DataItem WHERE FK_Parent_Group = '{0}'", this.ID);
            object returnValue = DataBase.Get().Execute(sql);
            return Convert.ToInt32(returnValue);
        }

        private BaseCollection<DataGroupLanguage> _dataGroupLanguages;
        [Persistent("DataGroupLanguage")]
        [Association("FK_Group")]
        public BaseCollection<DataGroupLanguage> DataGroupLanguages
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
                    if (_dataGroupLanguages == null || (_dataGroupLanguages != null && !_dataGroupLanguages.IsLoaded))
                    {
                        _dataGroupLanguages = BaseCollection<DataGroupLanguage>.Get("FK_DataGroup='" + this.ID + "'");
                        _dataGroupLanguages.IsLoaded = true;
                    }
                    foreach (CmsLanguage language in Site.Languages)
                    {
                        if (Site.DefaultLanguage != language.LanguageCode) //standaard taal niet toevoegen aan talencollectie
                        //standaard taal wordt in velden van het item zelf opgeslagen 
                        {
                            //mochten er later talen worden toegevoegd aan de site, dan moeten ze in item beschikbaar zijn
                            //hieronder wordt gekeken of er nieuwe talen zijn
                            bool found = false;
                            if (_dataGroupLanguages != null)
                            {
                                foreach (DataGroupLanguage groupLanguage in _dataGroupLanguages)
                                {
                                    if (groupLanguage.LanguageCode == language.LanguageCode)
                                    {
                                        found = true;
                                    }
                                }
                            }
                            if (!found)
                            {
                                DataGroupLanguage groupLanguage = new DataGroupLanguage();
                                groupLanguage.LanguageCode = language.LanguageCode;
                                //itemLanguage.Html1 = "testhtml1" + language.LanguageCode;
                                //itemLanguage.Text1 = "testtext1" + language.LanguageCode;
                                _dataGroupLanguages.Add(groupLanguage);
                            }
                        }
                    }
                    return _dataGroupLanguages;
                }
            }
            set
            {
                _dataGroupLanguages = value;
                //todo: FIX misschien moet dit anders HJ
                if (_dataGroupLanguages != null) _dataGroupLanguages.IsLoaded = true;
            }
        }

        /// <summary>
        /// zet alle waardes van een DataGroupLanguage regel in de DataItem
        /// Hierdoor kunnen dezelfde velden worden gebruikt in de dataModule
        /// </summary>
        /// <param name="languageCode"></param>
        public void TryTranslate(string languageCode)
        {
            if (DataCollection.HasStaticLanguage || languageCode == null || languageCode == String.Empty || languageCode == Site.DefaultLanguage)
            {
                return;
            }
            string where = "FK_DataGroup='" + this.ID + "' AND LanguageCode ='" + languageCode + "'";
            DataGroupLanguage groupLanguage = DataGroupLanguage.GetFirst<DataGroupLanguage>(where);
            //alleen Titel voldoende. TryTranslate wordt gebruikt in Breadcrumbmodule, de andere modules werken met dataTables
            this.Title = groupLanguage.Title;

            //foreach (DataField fld in this.DataCollection.DataGroupFields)
            //{
            //    if (fld.IsMultiLanguageField)
            //    {
            //        if (fld.FieldType != FieldTypeEnum.ImageList &&
            //        fld.FieldType != FieldTypeEnum.FileList &&
            //        fld.FieldType != FieldTypeEnum.DropDown)
            //        {
            //            System.Reflection.PropertyInfo propItem = this.GetType().GetProperty(fld.MappingColumn);

            //            System.Reflection.PropertyInfo propLanguage = groupLanguage.GetType().GetProperty(fld.MappingColumn);

            //            Object valueLanguage = propLanguage.GetValue(groupLanguage, null);
            //            propItem.SetValue(this, valueLanguage, null);
            //            //    return returnValue;
            //        }
            //        else if (fld.FieldType == FieldTypeEnum.ImageList)
            //        {
            //            _imageList1 = BaseCollection<DataFile>.Get("Type = '" + DataFile.TYPE_EXTRA_IMAGE + "' AND FK_Group='" + this.ID + "' AND Language='" + languageCode + "'", "Name");
            //            _imageList1.IsLoaded = true;
            //        }
            //        else if (fld.FieldType == FieldTypeEnum.FileList)
            //        {
            //            _imageList1 = BaseCollection<DataFile>.Get("Type = '" + DataFile.TYPE_EXTRA_FILE + "' AND FK_Group='" + this.ID + "' AND Language='" + languageCode + "'", "Name");
            //            _imageList1.IsLoaded = true;
            //        }
            //        else if (fld.FieldType == FieldTypeEnum.DropDown)
            //        {
            //            //todo
            //        }
            //    }
            //}
        }
    }
}
