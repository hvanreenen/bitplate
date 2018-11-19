using System;
using System.Collections.Generic;
using System.Text;

using HJORM;
using HJORM.Attributes;
using BitPlate.Domain;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Utils;
using BitPlate.Domain.Modules.Data;

namespace BitPlate.Domain.DataCollections
{
    public enum DataCollectionTypeEnum { Simple, Default, News, Products, FAQ, GoogleMaps };
    public class DataCollection : BaseDomainPublishableObject
    {
        public DataCollection()
        {

        }
        public DataCollectionTypeEnum Type { get; set; }
        [NonPersistent()]
        public string TypeString
        {
            get
            {
                //Name() method van enum = extension method in Utils.TypeExtensions.cs
                return Type.Name();
            }
        }
        public string LanguageCode { get; set; }
        /// <summary>
        /// Kijk of Datacollectie een vaste taal heeft. DataCollecties met vaste taal zijn niet meertalig, maar gelden alleen voor deze taal. Deze hoeven niet te worden vertaald.
        /// </summary>
        [NonPersistent()]
        public bool HasStaticLanguage
        {
            get
            {
                return !(LanguageCode == null || LanguageCode == "" || LanguageCode == "none");
            }
        }
        private BaseCollection<DataField> _dataGroupFields;
        //[Association("FK_DataCollection")]
        [NonPersistent()]
        public BaseCollection<DataField> DataGroupFields
        {
            get
            {
                if (_dataGroupFields == null)
                {
                    _dataGroupFields = BaseCollection<DataField>.Get("Type='group' AND FK_DataCollection = '" + this.ID + "'", "TabPage, OrderingNumber, Name");
                }
                return _dataGroupFields;
            }
            set { _dataGroupFields = value; }
        }

        private BaseCollection<DataField> _dataItemFields;
        //[Association("FK_DataCollection")]
        [NonPersistent()]
        public BaseCollection<DataField> DataItemFields
        {
            get
            {
                if (_dataItemFields == null)
                {
                    _dataItemFields = BaseCollection<DataField>.Get("Type='item' AND FK_DataCollection = '" + this.ID + "'", "TabPage, OrderingNumber, Name");
                }
                return _dataItemFields;
            }
            set { _dataItemFields = value; }
        }

        private BaseCollection<DataGroup> _maingroups;
        [NonPersistent()]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public BaseCollection<DataGroup> MainGroups
        {
            get
            {
                //laad alleen parentgroups
                _maingroups = BaseCollection<DataGroup>.Get("FK_Parent_Group is null AND FK_DataCollection='" + this.ID + "'", "OrderNumber");
                foreach (DataGroup g in _maingroups)
                {
                    g.DataCollection = this;
                }
                return _maingroups;
            }
            set { _maingroups = value; }
        }

        private BaseCollection<DataGroup> _groups;
        [NonPersistent()]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public BaseCollection<DataGroup> Groups
        {
            get
            {
                //laad alleen parentgroups
                _groups = BaseCollection<DataGroup>.Get("FK_DataCollection='" + this.ID + "'", "OrderNumber");
                return _groups;
            }
            set { _groups = value; }
        }

        public BaseCollection<DataGroup> GetParentGroups()
        {
            BaseCollection<DataGroup> groups = BaseCollection<DataGroup>.Get("FK_Parent_Group is null AND FK_DataCollection='" + this.ID + "'", "OrderNumber");
            return groups;
        }
        public BaseCollection<DataItem> GetRootItems()
        {
            BaseCollection<DataItem> items = BaseCollection<DataItem>.Get("FK_Parent_Group is null AND FK_DataCollection='" + this.ID + "'", "OrderNumber");
            return items;
        }
        private BaseCollection<DataItem> _items;
        [NonPersistent()]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public BaseCollection<DataItem> Items
        {
            get
            {
                //laad alleen parentgroups
                _items = BaseCollection<DataItem>.Get("FK_DataCollection='" + this.ID + "'", "OrderNumber");
                return _items;
            }
            set { _items = value; }
        }

        //[NonPersistent()]
        //public bool PublishIntoDB { get; set; }
        //[NonPersistent()]
        //public bool PublishAsHtml { get; set; }

        protected DataGroup getGroupByNameOrNew(string name)
        {
            DataGroup returnValue = null;
            if (name != "")
            {
                //find group
                foreach (DataGroup group in Groups)
                {
                    if (group.Name == name)
                    {
                        returnValue = group;
                        break;
                    }
                }
                if (returnValue == null)
                {
                    //nieuwe groep aanmaken
                    DataGroup newgroup = new DataGroup();
                    newgroup.Name = name;
                    newgroup.Site = this.Site;
                    newgroup.DataCollection = this;
                    newgroup.Save();
                    returnValue = newgroup;
                }
            }
            return returnValue;
        }


        public void DeleteAll()
        {
            List<CmsPage> usedInPages = IsInUseAtPages();

            if (usedInPages.Count > 0)
            {
                string msg = "Wilt u alle items verwijderen?";
                foreach (CmsPage page in usedInPages)
                {
                    msg += page.Name + ", ";
                }
                throw new Exception(msg.Substring(0, msg.Length - 2) + ".");
            }
            Items.DeleteAll();
            Groups.DeleteAll();
        }
        /// <summary>
        /// Activeer alle groepen en items die nog niet actief zijn
        /// </summary>
        //public void ActivateAll()
        //{
        //    foreach (DataGroup group in this.Groups)
        //    {
        //        if (!group.IsActive)
        //        {
        //            group.Activate();
        //            group.Save();
        //        }
        //        foreach (DataItem item in group.Items)
        //        {
        //            if (!item.IsActive)
        //            {
        //                item.Activate();
        //                item.Save();
        //            }
        //        }
        //    }
        //}

        public override void Delete()
        {
            //if (Groups.Count > 0 || Items.Count > 0)
            //{
            //    throw new Exception("Kan niet verwijderen, want de dataverzameling bevat nog gegevens");
            //}
            List<CmsPage> usedInPages = IsInUseAtPages();

            if (usedInPages.Count > 0)
            {
                string msg = "Kan niet verwijderen, want de dataverzameling wordt gebruikt op pagina's: ";
                foreach (CmsPage page in usedInPages)
                {
                    msg += page.Name + ", ";
                }
                throw new Exception(msg.Substring(0, msg.Length - 2) + ".");
            }
            base.Delete();
        }
        public List<CmsPage> IsInUseAtPages()
        {
            return IsInUseAtPages(this.Site);
        }

        public List<CmsPage> IsInUseAtPages(CmsSite site)
        {
            List<CmsPage> returnPages = new List<CmsPage>();
            foreach (CmsPage page in site.Pages)
            {
                foreach (BaseModule mod in page.Modules)
                {
                    BaseDataModule dataModule = BaseObject.GetById<BaseDataModule>(mod.ID); //FIX
                    DataCollection dataCollection = dataModule.DataCollection;
                    if (dataCollection != null && dataCollection.Equals(this))
                    {
                        if (!returnPages.Contains(page))
                        {
                            returnPages.Add(page);
                        }
                    }

                }
            }
            return returnPages;
        }



        //        public void UpdateAllOrderingNumbers()
        //        {
        //            string sql = String.Format(@"SELECT ID, Name, 'Item' AS Title, OrderingNumber From DataItem WHERE FK_DataCollection='{0}' AND FK_Parent_Group Is Null
        //UNION SELECT ID, Name, 'Group' AS Title, OrderingNumber From DataGroup WHERE FK_DataCollection='{0}' AND FK_Parent_Group Is Null ORDER BY OrderingNumber, Name", this.ID);
        //            BaseCollection<BaseDataObject> dataObjects = BaseCollection<BaseDataObject>.LoadFromSql(sql);
        //            int orderingNumber = 0;
        //            foreach (BaseDataObject dataObject in dataObjects)
        //            {
        //                orderingNumber++;
        //                string orderingNumberString = orderingNumber.ToString("0000");
        //                string updateSql = String.Format("UPDATE Data{0} SET OrderingNumber='{1}' WHERE ID='{2}'", dataObject.Title, orderingNumberString, dataObject.ID);
        //                DataBase.Get().Execute(updateSql);
        //                if (dataObject.Title == "Group")
        //                {
        //                    ((DataGroup)dataObject).UpdateSubOrderingNumbers(orderingNumberString);
        //                }
        //            }
        //        }

        //        public void MoveRootItemsAndGroups(int fromOrderingNumber, int? tillOrderingNumber, MoveDirectionEnum direction)
        //        {
        //            //zowel groepen als items krijgen volgorde in de tree
        //            //union query waarbij groepen en item in 1 query staan.
        //            //resultaat van query wordt in BaseDataObject gezet.
        //            //veld title misbruiken om hier type (groep of item) in te bewaren 
        //            //alleen de lagere nummers hoef je te updaten  (OrderingNumber <= orderingNumber)
        //            //krijgen allemaal 1 plekje lager, totaan tillOrderingNumber
        //            //ook subgroepen moeten worden geupdate, want hier wordt nieuwe hierarchie in nummering weergegeven
        //            string fromOrderingNumberString = fromOrderingNumber.ToString("0000");
        //            string tillWhereClause = "";
        //            if (tillOrderingNumber.HasValue)
        //            {
        //                string tillOrderingNumberString = tillOrderingNumber.Value.ToString("0000");
        //                tillWhereClause = String.Format(" AND OrderingNumber <= '{0}'", tillOrderingNumberString);
        //            }

        //            string sql = String.Format(@"SELECT ID, Name, 'Item' AS Title, OrderingNumber From DataItem WHERE FK_DataCollection='{0}' AND FK_Parent_Group IS NULL AND OrderingNumber >= '{1}' {2} 
        //UNION SELECT ID, Name, 'Group' AS Title, OrderingNumber From DataGroup WHERE FK_DataCollection='{0}' AND FK_Parent_Group IS NULL AND OrderingNumber >= '{1}' {2} 
        //ORDER BY OrderingNumber, Name", this.ID, fromOrderingNumberString, tillWhereClause);
        //            BaseCollection<BaseDataObject> dataObjects = BaseCollection<BaseDataObject>.LoadFromSql(sql);
        //            int currentOrderingNumber = fromOrderingNumber;
        //            foreach (BaseDataObject dataObject in dataObjects)
        //            {
        //                string newOrderingNumberString = "";
        //                if (direction == MoveDirectionEnum.Down)
        //                {
        //                    newOrderingNumberString = (currentOrderingNumber - 1).ToString("0000");
        //                }
        //                else
        //                {
        //                    newOrderingNumberString = (currentOrderingNumber + 1).ToString("0000");
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
        //        public void MoveRootItemsAndGroupsDown(int fromOrderingNumber, int? tillOrderingNumber)
        //        {

        //            MoveRootItemsAndGroups(fromOrderingNumber, tillOrderingNumber, MoveDirectionEnum.Down);

        //        }

        //        public void MoveRootItemsAndGroupsUp(int fromOrderingNumber, int? tillOrderingNumber)
        //        {
        //            MoveRootItemsAndGroups(fromOrderingNumber, tillOrderingNumber, MoveDirectionEnum.Up);

        //        }

        //        public void UpdateGroupOrderingNumbers(BaseDataObject group, string groupNumberString)
        //        {
        //            string sql = String.Format(@"SELECT ID, Name, 'Item' AS Title, OrderingNumber From DataItem WHERE FK_DataCollection='{0}' AND FK_Parent_Group = '{1}'
        //UNION SELECT ID, Name, 'Group' AS Title, OrderingNumber From DataGroup WHERE FK_DataCollection='{0}' AND FK_Parent_Group = '{1}' ORDER BY OrderingNumber, Name", this.ID, group.ID);
        //            BaseCollection<BaseDataObject> dataObjects = BaseCollection<BaseDataObject>.LoadFromSql(sql);
        //            int num = 0;
        //            foreach (BaseDataObject dataObject in dataObjects)
        //            {
        //                num++;
        //                string orderingNumberString = groupNumberString + "." + num.ToString("0000");
        //                string updateSql = String.Format("UPDATE Data{0} SET OrderingNumber='{1}' WHERE ID='{2}'", dataObject.Title, orderingNumberString, dataObject.ID);
        //                DataBase.Get().Execute(updateSql);
        //                if (dataObject.Title == "Group")
        //                {
        //                    UpdateGroupOrderingNumbers(dataObject, orderingNumberString);
        //                }
        //            }
        //        }


        internal DataGroup GetFirstGroup()
        {
            DataGroup returnValue = null;
            if (Groups.Count > 0)
            {
                returnValue = Groups[0];
            }
            return returnValue;
        }

        internal string CalculateNewRootOrderingNumber()
        {
            string sql = String.Format(@"SELECT ID, Name, 'Item' AS Title, OrderingNumber From DataItem WHERE FK_DataCollection = '{0}' AND FK_Parent_Group IS NULL 
UNION SELECT ID, Name, 'Group' AS Title, OrderingNumber From DataGroup WHERE FK_DataCollection = '{0}' AND FK_Parent_Group IS NULL", this.ID);
            BaseCollection<BaseDataObject> dataObjects = BaseCollection<BaseDataObject>.LoadFromSql(sql);
            int newOrderNumber = dataObjects.Count + 1;
            string newOrderingNumberString = newOrderNumber.ToString("0000");
            return newOrderingNumberString;
        }

        //public void MoveItemsAndGroups(BaseDataObject dataObj, string oldParentId, string newParentId, int oldOrderingNumber, int newOrderingNumber)
        //{
        //    if (oldParentId != newParentId)
        //    {
        //        //versleping van ene groep naar andere
        //        //in oude groep ontstaat een gat
        //        //alle items met hoger volgnummer dan oude volgnummer, moeten 1 plek naar boven (volgnummer--)om dit gat op te vullen
        //        if (oldParentId == "treeRoot")
        //        {
        //            //this.MoveRootItemsAndGroupsDown(oldOrderingNumber, null);
        //        }
        //        else
        //        {
        //            //dataObj.ParentGroup.MoveItemsAndGroupsDown(oldOrderingNumber, null);
        //        }
        //        //EN in nieuwe groep moet plaats worden gemaakt voor nieuwe item
        //        //dus alle items met hoger volgnummer dan nieuwe volgnummer, moeten 1 plek naar beneden om gat te maken
        //        if (newParentId == "treeRoot")
        //        {
        //            this.MoveRootItemsAndGroupsUp(newOrderingNumber, null);
        //            dataObj.ParentGroup = null;
        //            dataObj.OrderingNumber = newOrderingNumber.ToString("0000");
        //        }
        //        else
        //        {
        //            DataGroup newParentGroup = BaseObject.GetById<DataGroup>(new Guid(newParentId));
        //            dataObj.ParentGroup = newParentGroup;
        //            //newParentGroup.MoveItemsAndGroupsUp(newOrderingNumber, null);
        //            dataObj.OrderingNumber = newParentGroup.OrderingNumber + "." + newOrderingNumber.ToString("0000");
        //        }
        //        dataObj.Save();
        //        if (dataObj.GetType() == typeof(DataGroup))
        //        {
        //            dataObj.UpdateSubOrderingNumbers(dataObj.OrderingNumber);
        //        }
        //    }
        //    else if (oldParentId == newParentId &&
        //        oldOrderingNumber != newOrderingNumber)
        //    {
        //        //verslepen binnen dezelfde groep
        //        if (newOrderingNumber > oldOrderingNumber)
        //        {
        //            //omlaag slepen (nieuwe nummer > oude nummer): er onstaat gat in lijst bij oude nummer (lager in lijst)
        //            //en er moet plek worden gemaakt bij nieuwe nummer (hoger in lijst)
        //            //Dus alle items die hoger zijn tot aan oude nummer moeten 1 plek omlaag om dit gat op te vullen
        //            if (newParentId == "treeRoot")
        //            {
        //                //this.MoveRootItemsAndGroupsDown(oldOrderingNumber, newOrderingNumber);
        //                dataObj.OrderingNumber = newOrderingNumber.ToString("0000");
        //            }
        //            else
        //            {
        //                //dataObj.ParentGroup.MoveItemsAndGroupsDown(oldOrderingNumber, newOrderingNumber);
        //                dataObj.OrderingNumber = dataObj.ParentGroup.OrderingNumber + "." + newOrderingNumber.ToString("0000");
        //            }
        //        }
        //        else
        //        {
        //            //omhoog slepen (nieuwe nummer < oude nummer): er moet plaats worden gemaakt in de lijst bij nieuwe nummer
        //            //en gat = hoger in lijst
        //            //Dus alle items die groter zijn dan nieuwe nummer tot aan oude nummer moeten 1 plek omhoog om plek te maken
        //            if (newParentId == "treeRoot")
        //            {
        //                //this.MoveRootItemsAndGroupsUp(newOrderingNumber, oldOrderingNumber);
        //                dataObj.OrderingNumber = newOrderingNumber.ToString("0000");
        //            }
        //            else
        //            {
        //                //dataObj.ParentGroup.MoveItemsAndGroupsUp(newOrderingNumber, oldOrderingNumber);
        //                dataObj.OrderingNumber = dataObj.ParentGroup.OrderingNumber + "." + newOrderingNumber.ToString("0000");
        //            }
        //        }
        //        dataObj.Save();
        //        if (dataObj.GetType() == typeof(DataGroup))
        //        {
        //            dataObj.UpdateSubOrderingNumbers(dataObj.OrderingNumber);
        //        }
        //    }
        //}

        public override void Save()
        {
            if (IsNew && WebSessionHelper.CurrentLicense != null && WebSessionHelper.CurrentLicense.HasMaxNumberExceeded("DataCollection"))
            {
                throw new Exception("Kan niet opslaan: licentie heeft maximale aantal datacollecties (" + WebSessionHelper.CurrentLicense.MaxNumberOfDataCollections + ") overschreden.");
            }
            bool isNew = this.IsNew;
            if (LanguageCode == "none")
            {
                LanguageCode = "";
            }
            base.Save();

            if (isNew)
            {
                //automatisch mappings maken voor Fields
                foreach (DataField field in DataGroupFields)
                {
                    //if (field.IsNew)
                    //{
                    field.DataCollection = this;
                    field.Save();
                    //field.MappingColumn = GetNewGroupMappingColumn(field);
                    //}
                }
                foreach (DataField field in DataItemFields)
                {
                    //if (field.IsNew)
                    //{
                    field.DataCollection = this;
                    field.Save();
                    //}
                    //if (field.MappingColumn == null || field.MappingColumn == "")
                    //{
                    //    field.MappingColumn = GetNewItemMappingColumn(field);
                    //}

                }
            }

            UnpublishedItem.Set(this, "DataCollection");

        }

        private List<string> usedGroupMappingColumns = null;
        public string GetNewGroupMappingColumn(FieldTypeEnum fieldType)
        {
            //eerst lijst vullen van welke al gebruikt zijn
            if (usedGroupMappingColumns == null)
            {
                usedGroupMappingColumns = new List<string>();
                foreach (DataField usedField in DataGroupFields)
                {
                    if (usedField.MappingColumn != null && usedField.MappingColumn != "")
                    {
                        usedGroupMappingColumns.Add(usedField.MappingColumn);
                    }
                }
            }

            //dan naam maken met volgnummer erachter
            //naam = type + volgnummer
            string initMappingName = fieldType.ToString();
            if (fieldType == FieldTypeEnum.DropDown)
            {
                initMappingName = "LookupValue";
            }
            int number = 1;
            string mappingName = initMappingName + number.ToString();
            //zolang deze naam al gebruikt is, volgnummer ophogen
            while (usedGroupMappingColumns.Contains(mappingName))
            {
                number++;
                mappingName = initMappingName + number.ToString();
            }
            //als maximale beschikbare velden overschreden dan wordt het een extra veld, zo nee dan is de mapping gevonden op veld
            if (hasMaxMappingFieldsExceeded(fieldType, number))
            {
                //
                mappingName = "";
                throw new Exception("Maximale aantal velden bereikt voor " + fieldType);
                //extraFields is jsonstring in database
                //mappingName = "ExtraFields." + field.Name.Replace(" ", "_");
            }
            //bij imagelist en filelist is er maar 1 veld zonder nummer
            //if (fieldType == FieldTypeEnum.ImageList)
            //{
            //    mappingName = "ExtraImages";
            //}
            //else if (fieldType == FieldTypeEnum.FileList)
            //{
            //    mappingName = "Files";
            //}
            //lijst van welke al gebruikt zijn aanvullen
            usedGroupMappingColumns.Add(mappingName);

            return mappingName;
        }

        private List<string> usedItemMappingColumns = null;
        public string GetNewItemMappingColumn(FieldTypeEnum fieldType)
        {
            //eerst lijst vullen van welke al gebruikt zijn
            if (usedItemMappingColumns == null)
            {
                usedItemMappingColumns = new List<string>();
                foreach (DataField usedField in DataItemFields)
                {
                    if (usedField.MappingColumn != null && usedField.MappingColumn != "")
                    {
                        usedItemMappingColumns.Add(usedField.MappingColumn);
                    }
                }
            }
            //sorteren
            usedItemMappingColumns.Sort();
            //dan naam maken met volgnummer erachter
            //mappingnaam = type + volgnummer
            string initMappingName = fieldType.ToString();
            //behalve bij dropdowns dan is mappingnaam FK_ + volgnummer
            if (fieldType == FieldTypeEnum.DropDown)
            {
                initMappingName = "LookupValue";
            }
            //set initMappingColumn Checkboxlist
            if (fieldType == FieldTypeEnum.CheckboxList)
            {
                initMappingName = "LookupValues";
            }

            int number = 1;
            string mappingName = initMappingName + number.ToString();
            while (usedItemMappingColumns.Contains(mappingName))
            {
                number++;
                mappingName = initMappingName + number.ToString();
            }
            //als maximale beschikbare velden overschreden dan wordt het een extra veld, zo nee dan is de mapping gevonden op veld
            if (hasMaxMappingFieldsExceeded(fieldType, number))
            {
                //dan waarschuwing geven
                mappingName = "";
                throw new Exception("Maximale aantal velden bereikt voor " + fieldType);
                //extraFields is jsonstring in database
                //mappingName = "ExtraFields." + field.Name.Replace(" ", "_");
            }
            ////bij imagelist en filelist is er maar 1 veld zonder nummer
            //if (fieldType == FieldTypeEnum.ImageList)
            //{
            //    mappingName = "ExtraImages";
            //}
            //else if (fieldType == FieldTypeEnum.FileList)
            //{
            //    mappingName = "Files";
            //}

            usedItemMappingColumns.Add(mappingName);

            return mappingName;
        }

        private bool hasMaxMappingFieldsExceeded(FieldTypeEnum fieldType, int number)
        {
            if (fieldType == FieldTypeEnum.Text)
            {
                return (number > 30);
            }
            else if (fieldType == FieldTypeEnum.DropDown)
            {
                return (number > 10);
            }
            else if (fieldType == FieldTypeEnum.ImageList || fieldType == FieldTypeEnum.FileList)
            {
                return (number > 1);
            }
            else if (fieldType == FieldTypeEnum.CheckboxList)
            {
                return (number > 1);
            }
            else
            {
                return (number > 5);
            }
        }


        public void Copy(string newName)
        {
            DataCollection newCollection = this.CreateCopy<DataCollection>(true);
            newCollection.Name = newName;
            newCollection.LastPublishedDate = null;
           
            newCollection.Save();
            foreach (DataField field in this.DataGroupFields)
            {
                field.Copy(newCollection.ID, this.Site.ID, newName);
            }
            foreach (DataField field in this.DataItemFields)
            {
                field.Copy(newCollection.ID, this.Site.ID, newName);
            }
            foreach (DataGroup group in this.GetParentGroups())
            {
                group.Copy(group.Name, newCollection.ID, null);
            }
            foreach (DataItem item in this.GetRootItems())
            {
                item.Copy(item.Name, newCollection.ID, null);
            }
        }


        private Dictionary<string, bool> availableGroupMappingFields_old = null;
        private string GetNextGroupMappingNameByFieldType_old(FieldTypeEnum fieldType)
        {
            if (availableGroupMappingFields_old == null)
            {
                availableGroupMappingFields_old = new Dictionary<string, bool>();
                //init vullen
                bool isUsed = false;
                availableGroupMappingFields_old.Add("Text1", isUsed);
                availableGroupMappingFields_old.Add("Text2", isUsed);
                availableGroupMappingFields_old.Add("Text3", isUsed);
                availableGroupMappingFields_old.Add("Text4", isUsed);
                availableGroupMappingFields_old.Add("Text5", isUsed);
                foreach (DataField field in DataGroupFields)
                {
                    if (field.MappingColumn != null && field.MappingColumn != "" && !field.MappingColumn.StartsWith("ExtraFields."))
                    {
                        if (availableGroupMappingFields_old.ContainsKey(field.MappingColumn))
                        {
                            isUsed = true;
                            availableGroupMappingFields_old[field.MappingColumn] = isUsed;
                        }
                    }
                }
            }

            string initMappingName = fieldType.ToString();
            int number = 1;
            string mappingName = initMappingName + number.ToString();
            while (availableGroupMappingFields_old.ContainsKey(mappingName))
            {
                bool isUsed = availableGroupMappingFields_old[mappingName];
                if (isUsed)
                {
                    mappingName = initMappingName + number.ToString();
                    number++;
                }
                else
                {
                    availableGroupMappingFields_old[mappingName] = true;
                }
            }
            //if (!hasMaxMappingFieldsExceeded(fieldType, number))
            //{

            //    usedGroupMappingFields.Add(mappingName);
            //}

            //else
            //{
            //    mappingName = "ExtraFields." + field.Name.Replace(" ", "_");
            //}
            return mappingName;
        }

        internal void Publish()
        {
            Publish(false);
        }

        internal void Publish(bool publishAll)
        {
            //publish all
            string sql = String.Format(@"DELETE FROM DataField_Live WHERE FK_DataCollection = '{0}'; 
INSERT DataField_Live SELECT * FROM DataField WHERE FK_DataCollection = '{0}';
", this.ID);
            DataBase.Get().Execute(sql);

            if (publishAll)
            {
                sql = String.Format(@"DELETE FROM DataGroup_Live WHERE FK_DataCollection = '{0}'; 
INSERT DataGroup_Live SELECT * FROM DataGroup WHERE FK_DataCollection = '{0}';
", this.ID);
                DataBase.Get().Execute(sql);
                sql = String.Format(@"UPDATE DataGroup SET LastPublishedDate=NOW(), ChangeStatus = 0 WHERE FK_DataCollection = '{0}'; 
", this.ID);
                DataBase.Get().Execute(sql);


                sql = String.Format(@"DELETE FROM DataItem_Live WHERE FK_DataCollection = '{0}'; 
INSERT DataItem_Live SELECT * FROM DataItem WHERE FK_DataCollection = '{0}';
", this.ID);
                DataBase.Get().Execute(sql);
                sql = String.Format(@"UPDATE DataItem SET LastPublishedDate=NOW(), ChangeStatus = 0 WHERE FK_DataCollection = '{0}'; 
", this.ID);
            }

            base.SavePublishInfo();
        }


        public void MoveItems(string itemid, int newOrderingnumber)
        {
            DataItem item = BaseObject.GetById<DataItem>(new Guid(itemid));
            int oldOrderingNumber = item.OrderNumber;
            if (oldOrderingNumber < newOrderingnumber)
            {
                //nummer wordt hoger: alles tussen oude nummer en nieuwe nummer 1 plek lager zetten
                string sql = String.Format("UPDATE DataItem SET OrderNumber = OrderNumber - 1 WHERE OrderNumber > {0} And OrderNumber <= {1} AND FK_DataCollection = '{2}' AND FK_Parent_Group IS NULL", oldOrderingNumber, newOrderingnumber, this.ID);
                DataBase.Get().Execute(sql);
            }
            else if (oldOrderingNumber > newOrderingnumber)
            {
                //nummer wordt lager: alles tussen nieuwe nummer en oude nummer 1 plek verder zetten
                string sql = String.Format("UPDATE DataItem SET OrderNumber = OrderNumber + 1 WHERE OrderNumber < {0} And OrderNumber >= {1} AND FK_DataCollection = '{2}' AND FK_Parent_Group IS NULL", oldOrderingNumber, newOrderingnumber, this.ID);
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
                string sql = String.Format("UPDATE DataGroup SET OrderNumber = OrderNumber - 1 WHERE OrderNumber > {0} And OrderNumber <= {1} AND FK_DataCollection = '{2}' AND FK_Parent_Group IS NULL", oldOrderingNumber, newOrderingnumber, this.ID);
                DataBase.Get().Execute(sql);
            }
            else if (oldOrderingNumber > newOrderingnumber)
            {
                //nummer wordt lager: alles tussen nieuwe nummer en oude nummer 1 plek verder zetten
                string sql = String.Format("UPDATE DataGroup SET OrderNumber = OrderNumber + 1 WHERE OrderNumber < {0} And OrderNumber >= {1} AND FK_DataCollection = '{2}' AND FK_Parent_Group IS NULL", oldOrderingNumber, newOrderingnumber, this.ID);
                DataBase.Get().Execute(sql);
            }
            group.OrderNumber = newOrderingnumber;
            group.Save();
        }

        public int GetMaxGroupOrderNumber()
        {
            string sql = String.Format("SELECT MAX(OrderNumber) FROM DataGroup WHERE FK_Parent_Group IS NULL AND FK_DataCollection = '{0}'", this.ID);
            object returnValue = DataBase.Get().Execute(sql);

            return Convert.ToInt32(returnValue);
        }

        public int GetMaxItemOrderNumber()
        {
            string sql = String.Format("SELECT MAX(OrderNumber) FROM DataItem WHERE FK_Parent_Group IS NULL AND FK_DataCollection = '{0}'", this.ID);
            object returnValue = DataBase.Get().Execute(sql);
            return Convert.ToInt32(returnValue);
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            this.Type = (DataCollectionTypeEnum)DataConverter.ToInt32(dataRow["Type"]);
            this.LanguageCode = dataRow["LanguageCode"].ToString();
            this.IsLoaded = true;
        }
    }
}

//public virtual void MakeDownload(System.IO.StreamWriter writer)
//{
//    //string fileContent = "";
//    string format = "\"{0}\";\"{1}\";\"{2:yyyy-MM-dd}\";\"{3:yyyy-MM-dd}\";\"{4}\";\"{5}\";\"{6}\";\"{7}\";\"{8}\";\"{9}\";\"{10}\";\"{11}\";\"{12}\"";
//    //header
//    string titles = "\"soort\";\"valt onder\";\"geldig van\";\"geldig tot\";\"naam\";\"volgnummer\";\"titel\";\"samenvatting\";\"inhoud(html)\";\"thumbnailSrc\";\"imageSrc\";\"bestanden\";\"extra afbeeldingen\";\"adres\";\"postcode\";\"plaats\";\"land\";\"categorie\"";
//    writer.WriteLine(titles);

//    foreach (DataGroup mainGroup in this.MainGroups)
//    {
//        MakeGroupRows(writer, format, mainGroup);
//    }
//}

//protected virtual void MakeGroupRows(System.IO.StreamWriter writer, string format, DataGroup group)
//{

//    string title = group.Title.Replace("#", "");
//    //string intro = group.Introduction.Replace("#", "");
//    //string content = group.Content.Replace("#", "");
//    //foreach (DataLanguageText text in group.Languages)
//    //{
//    //    title += "#";
//    //    intro += "#";
//    //    content += "#";
//    //    if (text.Title != null || text.Title != "")
//    //    {
//    //        title += text.Language + "=" + text.Title.Replace("#", "");
//    //    }
//    //    if (text.Introduction != null || text.Introduction != "")
//    //    {
//    //        intro += text.Language + "=" + text.Introduction.Replace("#", "");
//    //    }
//    //    if (text.Content != null || text.Content != "")
//    //    {
//    //        content += text.Language + "=" + text.Content.Replace("#", "");
//    //    }
//    //}
//    object[] fields = new object[13];
//    fields[0] = "Group";
//    fields[1] = group.ParentGroup == null ? "" : group.ParentGroup.Name;
//    fields[2] = group.DateFrom;
//    fields[3] = group.DateTill;
//    fields[4] = group.Name;
//    fields[5] = group.OrderNumber;
//    fields[6] = title;
//    //fields[7] = intro;
//    //fields[8] = content;

//    fields[11] = MakeExtraFilesDownload(group.ExtraFiles);
//    fields[12] = MakeExtraImagesDownload(group.ExtraImages);

//    writer.WriteLine(String.Format(format, fields));
//    foreach (DataItem item in group.Items)
//    {
//        //# = scheidingsteken
//        title = item.Title.Replace("#", "");
//        //intro = item.Introduction.Replace("#", "");
//        //content = item.Content.Replace("#", "");
//        //foreach (DataLanguageText text in item.Languages)
//        //{
//        //    title += "#";
//        //    //intro += "#";
//        //    //content += "#";
//        //    if (text.Title != null || text.Title != "")
//        //    {
//        //        title += text.Language + "=" + text.Title.Replace("#", "");
//        //    }
//        //    if (text.Introduction != null || text.Introduction != "")
//        //    {
//        //        //intro += text.Language + "=" + text.Introduction.Replace("#", "");
//        //    }
//        //    if (text.Content != null || text.Content != "")
//        //    {
//        //        content += text.Language + "=" + text.Content.Replace("#", "");
//        //    }
//        //}
//        fields[0] = "Item";
//        fields[1] = item.GetGroupName();
//        fields[2] = item.DateFrom;
//        fields[3] = item.DateTill;
//        fields[4] = item.Name;
//        fields[5] = item.OrderingNumber;
//        fields[6] = title;
//        //fields[7] = intro;
//        //fields[8] = content;
//        fields[9] = item.ThumbnailUrl;
//        fields[10] = item.ImageUrl;
//        fields[11] = MakeExtraFilesDownload(item.ExtraFiles);
//        fields[12] = MakeExtraImagesDownload(item.ExtraImages);

//        writer.WriteLine(String.Format(format, fields));
//    }
//    foreach (DataGroup subGroup in group.SubGroups)
//    {
//        subGroup.ParentGroup = group;
//        MakeGroupRows(writer, format, subGroup);
//    }

//}

//protected object MakeExtraImagesDownload(BaseCollection<DataFile> groupItemFiles)
//{
//    string returnValue = "";
//    foreach (DataFile file in groupItemFiles)
//    {
//        returnValue += file.Url.Replace("#", "") + "#";
//    }
//    return returnValue;
//}

//protected object MakeExtraFilesDownload(BaseCollection<DataFile> groupItemFiles)
//{
//    string returnValue = "";
//    foreach (DataFile file in groupItemFiles)
//    {
//        returnValue += file.Language + "=" + file.Url.Replace("#", "") + "#";
//    }
//    return returnValue;
//}

//public virtual void Import(System.IO.StreamReader reader)
//{
//    while (!reader.EndOfStream)
//    {
//        string line = reader.ReadLine();
//        //line = line.Replace("\"", "");
//        line = line.Replace("\\\"", "");

//        //string[] fields = line.Split(new string[] { "\";\"" }, StringSplitOptions.None);
//        //fields[0] = fields[0].Replace("\"", "");
//        //fields[fields.Length - 1] = fields[fields.Length - 1].Replace("\"", "");
//        string[] fields = Utils.CsvReader.ParseLine(line);
//        if (fields[0] == "\"Group" || fields[0] == "Group")
//        {
//            ImportGroup(fields);
//        }
//        else if (fields[0] == "\"Item" || fields[0] == "Item")
//        {
//            ImportItem(fields);
//        }
//    }
//}



//protected virtual void ImportGroup(string[] fields)
//{
//    DataGroup group = getGroupByNameOrNew(fields[4]);
//    group.Site = this.Site;
//    group.DataCollection = this;
//    group.ParentGroup = getGroupByNameOrNew(fields[1]);
//    if (fields[2] != "")
//    {
//        group.DateFrom = DataConverter.ToDateTime(fields[2]);
//    }
//    if (fields[3].Trim() != "")
//    {
//        group.DateTill = Convert.ToDateTime(fields[3]);
//    }
//    group.Name = fields[4];
//    int orderingNr = 0;
//    try
//    {
//        orderingNr = Convert.ToInt32(fields[5]);
//    }
//    catch { }
//    group.OrderingNumber = orderingNr.ToString();
//    group.Title = GetLanguage("Standaard", fields[6]);
//    //group.Introduction = GetLanguage("Standaard", fields[7]);
//    //group.Content = GetLanguage("Standaard", fields[8]);
//    group.ThumbnailUrl = CompleteUrl(fields[9]);
//    group.ImageUrl = CompleteUrl(fields[10]);
//    group.Save();
//    //extra talen
//    foreach (CmsLanguage lang in Site.Languages)
//    {
//        DataLanguageText text = new DataLanguageText();
//        text.Site = Site;
//        text.Language = lang.Name;
//        text.Group = group;
//        text.Title = GetLanguage(lang.Name, fields[6]);
//        text.Introduction = GetLanguage(lang.Name, fields[7]);
//        text.Content = GetLanguage(lang.Name, fields[8]);
//        text.Save();
//    }
//    if (fields.Length >= 12 && fields[11] != "")
//    {
//        ImportExtraFiles(group, fields[11]);
//    }
//    if (fields.Length >= 13 && fields[12] != "")
//    {
//        ImportExtraImages(group, fields[12]);
//    }

//}

//protected string GetLanguage(string lang, string text)
//{
//    string returnValue = text;
//    if (text.Contains("#"))
//    {
//        if (lang == "Standaard")
//        {
//            int posEnd = text.IndexOf("#");
//            if (posEnd < 0)
//            {
//                posEnd = text.Length;
//            }
//            returnValue = text.Substring(0, posEnd);
//        }
//        else
//        {
//            int posStart = text.IndexOf(lang + "=") + lang.Length + 1;
//            int posEnd = text.IndexOf("#", posStart);
//            if (posEnd < 0)
//            {
//                posEnd = text.Length;
//            }
//            returnValue = text.Substring(posStart, posEnd - posStart);
//        }
//    }
//    return returnValue;
//}
//protected void ImportExtraFiles(DataGroup group, string extraFiles)
//{
//    string[] imagesUrls = extraFiles.Split(new char[] { '#' });
//    foreach (string url in imagesUrls)
//    {
//        if (url.Trim() != "")
//        {
//            string[] namevalue = url.Split(new char[] { '=' });
//            string name = namevalue[0];
//            string value = namevalue[1];
//            DataFile fileObject = new DataFile();
//            fileObject.DataGroup = group;
//            fileObject.Language = name.Trim();
//            fileObject.Name = value.Trim();
//            fileObject.Url = value.Trim();
//            fileObject.Site = this.Site;
//            fileObject.Type = DataFile.TYPE_EXTRA_FILE;
//            fileObject.Save();
//        }
//    }
//}
//protected void ImportExtraImages(DataGroup group, string extraImages)
//{
//    string[] imagesUrls = extraImages.Split(new char[] { '#' });
//    foreach (string url in imagesUrls)
//    {
//        if (url.Trim() != "")
//        {
//            DataFile fileObject = new DataFile();
//            fileObject.DataGroup = group;
//            fileObject.Name = url.Trim();
//            fileObject.Url = url.Trim();
//            fileObject.Site = this.Site;
//            fileObject.Type = DataFile.TYPE_EXTRA_IMAGE;
//            fileObject.Save();
//        }
//    }
//}
//protected void ImportExtraFiles(DataItem item, string extraFiles)
//{
//    string[] imagesUrls = extraFiles.Split(new char[] { '#' });
//    foreach (string url in imagesUrls)
//    {
//        if (url.Trim() != "")
//        {
//            string[] namevalue = url.Split(new char[] { '=' });
//            string name = namevalue[0];
//            string value = namevalue[1];
//            DataFile fileObject = new DataFile();
//            fileObject.DataItem = item;
//            fileObject.Language = name.Trim();
//            fileObject.Name = value.Trim();
//            fileObject.Url = value.Trim();
//            fileObject.Site = this.Site;
//            fileObject.Type = DataFile.TYPE_EXTRA_FILE;
//            fileObject.Save();
//        }
//    }
//}
//protected void ImportExtraImages(DataItem item, string extraImages)
//{
//    string[] imagesUrls = extraImages.Split(new char[] { '#' });
//    foreach (string url in imagesUrls)
//    {
//        if (url.Trim() != "")
//        {
//            DataFile fileObject = new DataFile();
//            fileObject.DataItem = item;
//            fileObject.Name = url.Trim();
//            fileObject.Url = url.Trim();
//            fileObject.Site = this.Site;
//            fileObject.Type = DataFile.TYPE_EXTRA_IMAGE;
//            fileObject.Save();
//        }
//    }
//}

//protected string CompleteUrl(string file)
//{
//    if (file != null && file != "" && !file.ToLower().StartsWith("files"))
//    {
//        file = "Files/" + file;
//    }
//    return file;
//}

//protected virtual void ImportItem(string[] fields)
//{
//    DataItem item = new DataItem();
//    item.Site = this.Site;
//    item.DataCollection = this;
//    item.ParentGroup = getGroupByNameOrNew(fields[1]);
//    if (fields[2] != "")
//    {
//        item.DateFrom = Convert.ToDateTime(fields[2]);
//    }
//    if (fields[3] != "")
//    {
//        item.DateTill = Convert.ToDateTime(fields[3]);
//    }

//    item.Name = fields[4];
//    int orderingNr = 0;
//    try
//    {
//        orderingNr = Convert.ToInt32(fields[5]);
//    }
//    catch { }
//    item.OrderingNumber = orderingNr.ToString(); ;
//    item.Title = GetLanguage("Standaard", fields[6]);
//    //item.Introduction = GetLanguage("Standaard", fields[7]);
//    //item.Content = GetLanguage("Standaard", fields[8]);
//    if (fields.Length > 9)
//    {
//        item.ThumbnailUrl = CompleteUrl(fields[9]);
//    }
//    if (fields.Length > 10)
//    {
//        item.ImageUrl = CompleteUrl(fields[10]);
//    }
//    item.Save();
//    //extra talen
//    foreach (CmsLanguage lang in Site.Languages)
//    {
//        DataLanguageText text = new DataLanguageText();
//        text.Site = Site;
//        text.Language = lang.Name;
//        text.Item = item;
//        text.Title = GetLanguage(lang.Name, fields[6]);
//        text.Introduction = GetLanguage(lang.Name, fields[7]);
//        text.Content = GetLanguage(lang.Name, fields[8]);
//        text.Save();
//    }
//    if (fields.Length >= 12 && fields[11] != "")
//    {
//        ImportExtraFiles(item, fields[11]);
//    }
//    if (fields.Length >= 13 && fields[12] != "")
//    {
//        ImportExtraImages(item, fields[12]);
//    }

//}
