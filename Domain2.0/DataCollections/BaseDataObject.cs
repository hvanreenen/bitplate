using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Linq;

using HJORM;
using HJORM.Attributes;

using BitPlate.Domain.Modules;

namespace BitPlate.Domain.DataCollections
{
    /// <summary>
    /// Base data object. Kan zijn een group of een item.
    /// Group en item hebben veel dezelfde velden
    /// </summary>
    public class BaseDataObject : BaseDomainPublishableObject
    {
        private string _title;
        public string Title
        {
            get
            {
                if (_title == null || _title == "")
                {
                    _title = Name;
                }
                //if (CurrentLanguage != null)
                //{
                //    _title = CurrentLanguage.Title;
                //}
                return _title;
            }
            set { _title = value; }
        }

        public int OrderNumber { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }

        private DataCollection _dataCollection;
        [Association("FK_DataCollection")]
        public DataCollection DataCollection
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

        protected DataGroup _parentGroup;
        [Association("FK_Parent_Group")]
        public virtual DataGroup ParentGroup
        {
            get
            {
                if (_parentGroup != null && !_parentGroup.IsLoaded)
                {
                    _parentGroup.Load();
                }
                return _parentGroup;
            }
            set { _parentGroup = value; }
        }

        public virtual bool IsRoot()
        {
            return (this.ParentGroup == null);
        }

        //private BaseCollection<BaseDataLanguageObject> _languageObjects;
        //[NonPersistent()]
        //public BaseCollection<BaseDataLanguageObject> LanguageObjects
        //{
        //    get
        //    {
        //        _languageObjects = BaseCollection<BaseDataLanguageObject>.Get("FK_DataObject='" + this.ID + "'");
        //        return _languageObjects;
        //    }
        //    set { _languageObjects = value; }
        //}

        [NonPersistent()]
        public override bool IsActive
        {
            get
            {
                bool returnValue = base.IsActive;
                string isActiveStringInherited = "";
                if (ParentGroup != null && !ParentGroup.IsActive)
                {
                    returnValue = false;
                    isActiveStringInherited = "groep";
                }
                if (DataCollection != null && !DataCollection.IsActive)
                {
                    returnValue = false;
                    if (isActiveStringInherited != "")
                    {
                        isActiveStringInherited += " & ";
                    }
                    isActiveStringInherited += "datacollectie";
                }
                if (!returnValue && isActiveStringInherited != "")
                {
                    _isActiveString = "Niet actief (overgenomen van " + isActiveStringInherited + ")";
                }
                return returnValue;
            }
        }


        protected BaseCollection<DataFile> _fileList1;
        [Persistent("DataFile")]
        public virtual BaseCollection<DataFile> FileList1
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
                        _fileList1 = BaseCollection<DataFile>.Get("Type = '" + DataFile.TYPE_EXTRA_FILE + "' AND FK_Item='" + this.ID + "' AND Language = '" + sessionLanguage + "'", "Name");
                    }
                    else
                    {
                        _fileList1 = BaseCollection<DataFile>.Get("Type = '" + DataFile.TYPE_EXTRA_FILE + "' AND FK_Item='" + this.ID + "'", "Name");
                    }
                }
                return _fileList1;
            }
            set { _fileList1 = value; }
        }

        protected BaseCollection<DataFile> _imageList1;
        [Persistent("DataFile")]
        //[Association(ForeignObjectName = "DataItem")]
        public virtual BaseCollection<DataFile> ImageList1
        {
            get
            {
                if (_imageList1 == null || (_imageList1 != null && !_imageList1.IsLoaded))
                {
                    _imageList1 = BaseCollection<DataFile>.Get("Type = '" + DataFile.TYPE_EXTRA_IMAGE + "' AND FK_Item='" + this.ID + "'", "Name");
                    _imageList1.IsLoaded = true;
                }
                return _imageList1;
            }
            set { _imageList1 = value; }
        }

        public bool HasSubgroups()
        {
            BaseCollection<DataGroup> subGroups = BaseCollection<DataGroup>.Get("FK_Parent_Group = '" + this.ID + "'");

            return (subGroups.Count > 0);
        }
        public bool HasItems()
        {
            BaseCollection<DataItem> items = BaseCollection<DataItem>.Get("FK_Parent_Group = '" + this.ID + "'");

            return (items.Count > 0);
        }

        public bool IsLeaf()
        {
            return (!HasItems() && !HasSubgroups());
        }

        public override void Save()
        {
            if (_fileList1 != null)
            {
                _fileList1.IsLoaded = true;
            }
            if (_imageList1 != null)
            {
                _imageList1.IsLoaded = true;
            }
            CompletePath = GetCompletePath();
            if (IsNew)
            {
                if (ParentGroup == null)
                {
                    //OrderingNumber = this.DataCollection.CalculateNewRootOrderingNumber();
                }
                else
                {
                    //OrderingNumber = ParentGroup.CalculateNewOrderingNumber();
                }
            }
            //zorg dat in de db altijd relatieve urls blijven bestaan
            //if (Image1 != null) this.Image1 = Image1.Replace(Site.DomainName + "/", "");
            //if (Image2 != null) this.Image2 = Image2.Replace(Site.DomainName + "/", "");
            //if (Image3 != null) this.Image3 = Image3.Replace(Site.DomainName + "/", "");
            //if (Image4 != null) this.Image4 = Image4.Replace(Site.DomainName + "/", "");
            //if (Image5 != null) this.Image5 = Image5.Replace(Site.DomainName + "/", "");

            if (Image1 != null) this.Image1 = Image1.Replace(Site.DomainName, "");
            if (Image2 != null) this.Image2 = Image2.Replace(Site.DomainName, "");
            if (Image3 != null) this.Image3 = Image3.Replace(Site.DomainName, "");
            if (Image4 != null) this.Image4 = Image4.Replace(Site.DomainName, "");
            if (Image5 != null) this.Image5 = Image5.Replace(Site.DomainName, "");

            //this._extraFieldsJsonString = Utils.JSONSerializer.Serialize(this.ExtraFields);
            base.Save();

            string sql = String.Format("UPDATE DataCollection SET ModifiedDate=NOW() WHERE ID='{0}'", DataCollection.ID);
            DataBase.Get().Execute(sql);

        }
        /// <summary>
        /// kijk of groep niet onderzichzelf valt
        /// </summary>
        protected void ValidateGroupPath()
        {
            DataGroup parentGroup = this.ParentGroup;
            while (parentGroup != null)
            {
                if (this.ID == parentGroup.ID && this.ID != Guid.Empty)
                {
                    throw new Exception("Groep mag niet onder zichzelf vallen.");
                }

                parentGroup = parentGroup.ParentGroup;
            }
        }

        public string GetCompletePath()
        {
            string path = this.Name;
            DataGroup parentGroup = this.ParentGroup;
            while (parentGroup != null)
            {
                //path = this.ParentGroup.Name + "/" + path;
                path = parentGroup.Name + "/" + path;
                parentGroup = parentGroup.ParentGroup;
            }
            return path;
        }

        public string GetRewriteUrl(string navigationPageRelativeUrl, string dataType)
        {
            if (navigationPageRelativeUrl != null)
            {
                if (this.ID == Guid.Empty)
                {
                    return navigationPageRelativeUrl;
                }
                //string Url = "/" + navigationPageRelativeUrl.Replace(".aspx", "/") + this.ID.ToString() + "/";
                string Url = navigationPageRelativeUrl.Replace(".aspx", "") + "/" + dataType  + this.ID.ToString("N") + "/";
                if (this.CompletePath != null)
                {
                    Url += this.CompletePath.Replace(" ", "_").Replace(":", "").Replace(".", "").Replace("\"", "").Replace("&", "-").Replace("%", "") + "/";
                }
                if (this.Title != null)
                {
                    Url += this.Title.Replace(" ", "_").Replace(":", "").Replace(".", "").Replace("\"", "").Replace("&", "-").Replace("%", "");
                }
                else
                {
                    Url += "index";
                }
                //Url += ".aspx";
                return Url;
            }
            else
            {
                return "";
            }

        }
        [NonPersistent()]
        public string RewriteUrl { get; set; }

        //        public void UpdateSubOrderingNumbers(string groupNumberString)
        //        {
        //            string sql = String.Format(@"SELECT ID, Name, 'Item' AS Title, OrderingNumber From DataItem WHERE FK_Parent_Group = '{0}'
        //UNION SELECT ID, Name, 'Group' AS Title, OrderingNumber From DataGroup WHERE FK_Parent_Group = '{0}' ORDER BY OrderingNumber, Name", this.ID);
        //            BaseCollection<BaseDataObject> dataObjects = BaseCollection<BaseDataObject>.LoadFromSql(sql);
        //            int num = 0;
        //            foreach (BaseDataObject dataObject in dataObjects)
        //            {
        //                num++;
        //                //string orderingNumberString = groupNumberString + "." + num.ToString("0000");
        //                string orderingNumberString = this.OrderingNumber + "." + num.ToString("0000");
        //                string updateSql = String.Format("UPDATE Data{0} SET OrderingNumber='{1}' WHERE ID='{2}'", dataObject.Title, orderingNumberString, dataObject.ID);
        //                DataBase.Get().Execute(updateSql);
        //                if (dataObject.Title == "Group")
        //                {
        //                    dataObject.OrderingNumber = orderingNumberString;
        //                    dataObject.UpdateSubOrderingNumbers(orderingNumberString);
        //                }
        //            }
        //        }

        /* [NonPersistent()]
        public int Level
        {
            get
            {
                return CompletePath.Split(new char[] { '\\' }).Length;
            }
        } */

        public string CompletePath { get; set; }

        public override T CreateCopy<T>(bool newName)
        {
            return base.CreateCopy<T>(newName);
        }

        //private string _extraFieldsJsonString;
        //[System.Web.Script.Serialization.ScriptIgnore()]
        //public string ExtraFieldsJsonString
        //{
        //    get { return this._extraFieldsJsonString; }
        //    set
        //    {
        //        this._extraFieldsJsonString = value;
        //        this._extraFields = Utils.JSONSerializer.Deserialize<Dictionary<string, object>>(value);
        //    }
        //}

        //private Dictionary<string, object> _extraFields;
        //[NonPersistent()]
        ////[System.Web.Script.Serialization.ScriptIgnore()]
        //public Dictionary<string, object> ExtraFields
        //{
        //    get { return _extraFields; }
        //    set
        //    {
        //        this._extraFields = value;
        //        this._extraFieldsJsonString = Utils.JSONSerializer.Serialize(value);
        //    }
        //}

        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }
        public string Text5 { get; set; }
        public string LongText1 { get; set; }
        public string Html1 { get; set; }
        public int Numeric1 { get; set; }
        public double Currency1 { get; set; }
        public DateTime DateTime1 { get; set; }
        public bool YesNo1 { get; set; }
        public string Image1 { get; set; }
        public string File1 { get; set; }
        //public string FK_1 { get; set; }
        protected DataLookupValue _lookupValue1;
        [Association("FK_1")]
        public virtual DataLookupValue LookupValue1
        {
            get
            {
                if (_lookupValue1 != null && !_lookupValue1.IsLoaded)
                {
                    _lookupValue1.Load();
                }
                return _lookupValue1;
            }
            set { _lookupValue1 = value; }
        }
        //public string FK_2 { get; set; }
        protected DataLookupValue _lookupValue2;
        [Association("FK_2")]
        public virtual DataLookupValue LookupValue2
        {
            get
            {
                if (_lookupValue2 != null && !_lookupValue2.IsLoaded)
                {
                    _lookupValue2.Load();
                }
                return _lookupValue2;
            }
            set { _lookupValue2 = value; }
        }
        //public string FK_3 { get; set; }
        protected DataLookupValue _lookupValue3;
        [Association("FK_3")]
        public virtual DataLookupValue LookupValue3
        {
            get
            {
                if (_lookupValue3 != null && !_lookupValue3.IsLoaded)
                {
                    _lookupValue3.Load();
                }
                return _lookupValue3;
            }
            set { _lookupValue3 = value; }
        }
        //public string FK_4 { get; set; }
        protected DataLookupValue _lookupValue4;
        [Association("FK_4")]
        public virtual DataLookupValue LookupValue4
        {
            get
            {
                if (_lookupValue4 != null && !_lookupValue4.IsLoaded)
                {
                    _lookupValue4.Load();
                }
                return _lookupValue4;
            }
            set { _lookupValue4 = value; }
        }
        //public string FK_5 { get; set; }
        protected DataLookupValue _lookupValue5;
        [Association("FK_5")]
        public virtual DataLookupValue LookupValue5
        {
            get
            {
                if (_lookupValue5 != null && !_lookupValue5.IsLoaded)
                {
                    _lookupValue5.Load();
                }
                return _lookupValue5;
            }
            set { _lookupValue5 = value; }
        }
        //public string FK_6 { get; set; }
        protected DataLookupValue _lookupValue6;
        [Association("FK_6")]
        public virtual DataLookupValue LookupValue6
        {
            get
            {
                if (_lookupValue6 != null && !_lookupValue6.IsLoaded)
                {
                    _lookupValue6.Load();
                }
                return _lookupValue6;
            }
            set { _lookupValue6 = value; }
        }
        //public string FK_7 { get; set; }
        protected DataLookupValue _lookupValue7;
        [Association("FK_7")]
        public virtual DataLookupValue LookupValue7
        {
            get
            {
                if (_lookupValue7 != null && !_lookupValue7.IsLoaded)
                {
                    _lookupValue7.Load();
                }
                return _lookupValue7;
            }
            set { _lookupValue7 = value; }
        }
        //public string FK_8 { get; set; }
        protected DataLookupValue _lookupValue8;
        [Association("FK_8")]
        public virtual DataLookupValue LookupValue8
        {
            get
            {
                if (_lookupValue8 != null && !_lookupValue8.IsLoaded)
                {
                    _lookupValue8.Load();
                }
                return _lookupValue8;
            }
            set { _lookupValue8 = value; }
        }
        //public string FK_9 { get; set; }
        protected DataLookupValue _lookupValue9;
        [Association("FK_9")]
        public virtual DataLookupValue LookupValue9
        {
            get
            {
                if (_lookupValue9 != null && !_lookupValue9.IsLoaded)
                {
                    _lookupValue9.Load();
                }
                return _lookupValue9;
            }
            set { _lookupValue9 = value; }
        }
        //public string FK_10 { get; set; }
        protected DataLookupValue _lookupValue10;
        [Association("FK_10")]
        public virtual DataLookupValue LookupValue10
        {
            get
            {
                if (_lookupValue10 != null && !_lookupValue10.IsLoaded)
                {
                    _lookupValue10.Load();
                }
                return _lookupValue10;
            }
            set { _lookupValue10 = value; }
        }

        public string Text6 { get; set; }
        public string Text7 { get; set; }
        public string Text8 { get; set; }
        public string Text9 { get; set; }
        public string Text10 { get; set; }
        public string Text11 { get; set; }
        public string Text12 { get; set; }
        public string Text13 { get; set; }
        public string Text14 { get; set; }
        public string Text15 { get; set; }
        public string Text16 { get; set; }
        public string Text17 { get; set; }
        public string Text18 { get; set; }
        public string Text19 { get; set; }
        public string Text20 { get; set; }
        public string Text21 { get; set; }
        public string Text22 { get; set; }
        public string Text23 { get; set; }
        public string Text24 { get; set; }
        public string Text25 { get; set; }
        public string Text26 { get; set; }
        public string Text27 { get; set; }
        public string Text28 { get; set; }
        public string Text29 { get; set; }
        public string Text30 { get; set; }

        public string LongText2 { get; set; }
        public string LongText3 { get; set; }
        public string LongText4 { get; set; }
        public string LongText5 { get; set; }

        public string Html2 { get; set; }
        public string Html3 { get; set; }
        public string Html4 { get; set; }
        public string Html5 { get; set; }

        public int Numeric2 { get; set; }
        public int Numeric3 { get; set; }
        public int Numeric4 { get; set; }
        public int Numeric5 { get; set; }

        public double Currency2 { get; set; }
        public double Currency3 { get; set; }
        public double Currency4 { get; set; }
        public double Currency5 { get; set; }

        public DateTime DateTime2 { get; set; }
        public DateTime DateTime3 { get; set; }
        public DateTime DateTime4 { get; set; }
        public DateTime DateTime5 { get; set; }

        public bool YesNo2 { get; set; }
        public bool YesNo3 { get; set; }
        public bool YesNo4 { get; set; }
        public bool YesNo5 { get; set; }

        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Image5 { get; set; }

        public string File2 { get; set; }
        public string File3 { get; set; }
        public string File4 { get; set; }
        public string File5 { get; set; }

        public void FillObject(System.Data.DataRow dataRow, System.Data.DataColumnCollection columns)
        {
            base.FillObject(dataRow);
            if (columns.Contains("OrderNumber")) this.OrderNumber = DataConverter.ToInt32(dataRow["OrderNumber"]);
            if (columns.Contains("Title")) this.Title = dataRow["Title"].ToString();
            if (columns.Contains("CompletePath")) this.CompletePath = dataRow["CompletePath"].ToString();
            if (columns.Contains("FK_DataCollection") && dataRow["FK_DataCollection"] != DBNull.Value)
            {
                this.DataCollection = new DataCollection();
                this.DataCollection.ID = DataConverter.ToGuid(dataRow["FK_DataCollection"]);
            }
            if (columns.Contains("FK_Parent_Group") && dataRow["FK_Parent_Group"] != DBNull.Value)
            {
                this.ParentGroup = new DataGroup();
                this.ParentGroup.ID = DataConverter.ToGuid(dataRow["FK_Parent_Group"]);
            }
            if (columns.Contains("MetaDescription")) this.MetaDescription = dataRow["MetaDescription"].ToString();
            if (columns.Contains("MetaKeywords")) this.MetaKeywords = dataRow["MetaKeywords"].ToString();
            if (columns.Contains("Text1")) this.Text1 = dataRow["Text1"].ToString();
            if (columns.Contains("Text2")) this.Text2 = dataRow["Text2"].ToString();
            if (columns.Contains("Text3")) this.Text3 = dataRow["Text3"].ToString();
            if (columns.Contains("Text4")) this.Text4 = dataRow["Text4"].ToString();
            if (columns.Contains("Text5")) this.Text5 = dataRow["Text5"].ToString();
            if (columns.Contains("Text6")) this.Text6 = dataRow["Text6"].ToString();
            if (columns.Contains("Text7")) this.Text7 = dataRow["Text7"].ToString();
            if (columns.Contains("Text8")) this.Text8 = dataRow["Text8"].ToString();
            if (columns.Contains("Text9")) this.Text9 = dataRow["Text9"].ToString();
            if (columns.Contains("Text10")) this.Text10 = dataRow["Text10"].ToString();
            if (columns.Contains("Text11")) this.Text11 = dataRow["Text11"].ToString();
            if (columns.Contains("Text12")) this.Text12 = dataRow["Text12"].ToString();
            if (columns.Contains("Text13")) this.Text13 = dataRow["Text13"].ToString();
            if (columns.Contains("Text14")) this.Text14 = dataRow["Text14"].ToString();
            if (columns.Contains("Text15")) this.Text15 = dataRow["Text15"].ToString();
            if (columns.Contains("Text16")) this.Text16 = dataRow["Text16"].ToString();
            if (columns.Contains("Text17")) this.Text17 = dataRow["Text17"].ToString();
            if (columns.Contains("Text18")) this.Text18 = dataRow["Text18"].ToString();
            if (columns.Contains("Text19")) this.Text19 = dataRow["Text19"].ToString();
            if (columns.Contains("Text20")) this.Text20 = dataRow["Text20"].ToString();
            if (columns.Contains("Text21")) this.Text21 = dataRow["Text21"].ToString();
            if (columns.Contains("Text22")) this.Text22 = dataRow["Text22"].ToString();
            if (columns.Contains("Text23")) this.Text23 = dataRow["Text23"].ToString();
            if (columns.Contains("Text24")) this.Text24 = dataRow["Text24"].ToString();
            if (columns.Contains("Text25")) this.Text25 = dataRow["Text25"].ToString();
            if (columns.Contains("Text26")) this.Text26 = dataRow["Text26"].ToString();
            if (columns.Contains("Text27")) this.Text27 = dataRow["Text27"].ToString();
            if (columns.Contains("Text28")) this.Text28 = dataRow["Text28"].ToString();
            if (columns.Contains("Text29")) this.Text29 = dataRow["Text29"].ToString();
            if (columns.Contains("Text30")) this.Text30 = dataRow["Text30"].ToString();
            if (columns.Contains("LongText1")) this.LongText1 = dataRow["LongText1"].ToString();
            if (columns.Contains("LongText2")) this.LongText2 = dataRow["LongText2"].ToString();
            if (columns.Contains("LongText3")) this.LongText3 = dataRow["LongText3"].ToString();
            if (columns.Contains("LongText4")) this.LongText4 = dataRow["LongText4"].ToString();
            if (columns.Contains("LongText5")) this.LongText5 = dataRow["LongText5"].ToString();
            if (columns.Contains("Html1")) this.Html1 = dataRow["Html1"].ToString();
            if (columns.Contains("Html2")) this.Html2 = dataRow["Html2"].ToString();
            if (columns.Contains("Html3")) this.Html3 = dataRow["Html3"].ToString();
            if (columns.Contains("Html4")) this.Html4 = dataRow["Html4"].ToString();
            if (columns.Contains("Html5")) this.Html5 = dataRow["Html5"].ToString();
            if (columns.Contains("Image1")) this.Image1 = dataRow["Image1"].ToString();
            if (columns.Contains("Image2")) this.Image2 = dataRow["Image2"].ToString();
            if (columns.Contains("Image3")) this.Image3 = dataRow["Image3"].ToString();
            if (columns.Contains("Image4")) this.Image4 = dataRow["Image4"].ToString();
            if (columns.Contains("Image5")) this.Image5 = dataRow["Image5"].ToString();
            if (columns.Contains("File1")) this.File1 = dataRow["File1"].ToString();
            if (columns.Contains("File2")) this.File2 = dataRow["File2"].ToString();
            if (columns.Contains("File3")) this.File3 = dataRow["File3"].ToString();
            if (columns.Contains("File4")) this.File4 = dataRow["File4"].ToString();
            if (columns.Contains("File5")) this.File5 = dataRow["File5"].ToString();
            if (columns.Contains("YesNo1")) this.YesNo1 = DataConverter.ToBoolean(dataRow["YesNo1"]);
            if (columns.Contains("YesNo2")) this.YesNo2 = DataConverter.ToBoolean(dataRow["YesNo2"]);
            if (columns.Contains("YesNo3")) this.YesNo3 = DataConverter.ToBoolean(dataRow["YesNo3"]);
            if (columns.Contains("YesNo4")) this.YesNo4 = DataConverter.ToBoolean(dataRow["YesNo4"]);
            if (columns.Contains("YesNo5")) this.YesNo5 = DataConverter.ToBoolean(dataRow["YesNo5"]);
            if (columns.Contains("Numeric1")) this.Numeric1 = DataConverter.ToInt32(dataRow["Numeric1"]);
            if (columns.Contains("Numeric2")) this.Numeric2 = DataConverter.ToInt32(dataRow["Numeric2"]);
            if (columns.Contains("Numeric3")) this.Numeric3 = DataConverter.ToInt32(dataRow["Numeric3"]);
            if (columns.Contains("Numeric4")) this.Numeric4 = DataConverter.ToInt32(dataRow["Numeric4"]);
            if (columns.Contains("Numeric5")) this.Numeric5 = DataConverter.ToInt32(dataRow["Numeric5"]);
            if (columns.Contains("Currency1")) this.Currency1 = DataConverter.ToDouble(dataRow["Currency1"]);
            if (columns.Contains("Currency2")) this.Currency2 = DataConverter.ToDouble(dataRow["Currency2"]);
            if (columns.Contains("Currency3")) this.Currency3 = DataConverter.ToDouble(dataRow["Currency3"]);
            if (columns.Contains("Currency4")) this.Currency4 = DataConverter.ToDouble(dataRow["Currency4"]);
            if (columns.Contains("Currency5")) this.Currency5 = DataConverter.ToDouble(dataRow["Currency5"]);
            if (columns.Contains("DateTime1")) this.DateTime1 = DataConverter.ToDateTime(dataRow["DateTime1"]);
            if (columns.Contains("DateTime2")) this.DateTime2 = DataConverter.ToDateTime(dataRow["DateTime2"]);
            if (columns.Contains("DateTime3")) this.DateTime3 = DataConverter.ToDateTime(dataRow["DateTime3"]);
            if (columns.Contains("DateTime4")) this.DateTime4 = DataConverter.ToDateTime(dataRow["DateTime4"]);
            if (columns.Contains("DateTime5")) this.DateTime5 = DataConverter.ToDateTime(dataRow["DateTime5"]);
            if (columns.Contains("FK_1") && dataRow["FK_1"] != DBNull.Value)
            {
                this.LookupValue1 = new DataLookupValue();
                this.LookupValue1.ID = new Guid(dataRow["FK_1"].ToString());
            }
            if (columns.Contains("FK_2") && dataRow["FK_2"] != DBNull.Value)
            {
                this.LookupValue2 = new DataLookupValue();
                this.LookupValue2.ID = new Guid(dataRow["FK_2"].ToString());
            }
            if (columns.Contains("FK_3") && dataRow["FK_3"] != DBNull.Value)
            {
                this.LookupValue3 = new DataLookupValue();
                this.LookupValue3.ID = new Guid(dataRow["FK_3"].ToString());
            }
            if (columns.Contains("FK_4") && dataRow["FK_4"] != DBNull.Value)
            {
                this.LookupValue4 = new DataLookupValue();
                this.LookupValue4.ID = new Guid(dataRow["FK_4"].ToString());
            }
            if (columns.Contains("FK_5") && dataRow["FK_5"] != DBNull.Value)
            {
                this.LookupValue5 = new DataLookupValue();
                this.LookupValue5.ID = new Guid(dataRow["FK_5"].ToString());
            }
            if (columns.Contains("FK_6") && dataRow["FK_6"] != DBNull.Value)
            {
                this.LookupValue6 = new DataLookupValue();
                this.LookupValue6.ID = new Guid(dataRow["FK_6"].ToString());
            }
            if (columns.Contains("FK_7") && dataRow["FK_7"] != DBNull.Value)
            {
                this.LookupValue7 = new DataLookupValue();
                this.LookupValue7.ID = new Guid(dataRow["FK_7"].ToString());
            }
            if (columns.Contains("FK_8") && dataRow["FK_8"] != DBNull.Value)
            {
                this.LookupValue8 = new DataLookupValue();
                this.LookupValue8.ID = new Guid(dataRow["FK_8"].ToString());
            }
            if (columns.Contains("FK_9") && dataRow["FK_9"] != DBNull.Value)
            {
                this.LookupValue9 = new DataLookupValue();
                this.LookupValue9.ID = new Guid(dataRow["FK_9"].ToString());
            }
            if (columns.Contains("FK_10") && dataRow["FK_10"] != DBNull.Value)
            {
                this.LookupValue10 = new DataLookupValue();
                this.LookupValue10.ID = new Guid(dataRow["FK_10"].ToString());
            }

            this.IsLoaded = true;
        }
        public override void FillObject(System.Data.DataRow dataRow)
        {
            FillObject(dataRow, dataRow.Table.Columns);
            //return;
            //base.FillObject(dataRow);
            //this.Title = dataRow["Title"].ToString();
            //this.CompletePath = dataRow["CompletePath"].ToString();
            //if (dataRow["FK_DataCollection"] != DBNull.Value)
            //{
            //    this.DataCollection = new DataCollection();
            //    this.DataCollection.ID = new Guid(dataRow["FK_DataCollection"].ToString());
            //}
            //if (dataRow["FK_Parent_Group"] != DBNull.Value)
            //{
            //    this.ParentGroup = new DataGroup();
            //    this.ParentGroup.ID = new Guid(dataRow["FK_Parent_Group"].ToString());
            //}
            //this.MetaDescription = dataRow["MetaDescription"].ToString();
            //this.MetaKeywords = dataRow["MetaKeywords"].ToString();
            //this.Text1 = dataRow["Text1"].ToString();
            //this.Text2 = dataRow["Text2"].ToString();
            //this.Text3 = dataRow["Text3"].ToString();
            //this.Text4 = dataRow["Text4"].ToString();
            //this.Text5 = dataRow["Text5"].ToString();
            //this.Text6 = dataRow["Text6"].ToString();
            //this.Text7 = dataRow["Text7"].ToString();
            //this.Text8 = dataRow["Text8"].ToString();
            //this.Text9 = dataRow["Text9"].ToString();
            //this.Text10 = dataRow["Text10"].ToString();
            //this.Text11 = dataRow["Text11"].ToString();
            //this.Text12 = dataRow["Text12"].ToString();
            //this.Text13 = dataRow["Text13"].ToString();
            //this.Text14 = dataRow["Text14"].ToString();
            //this.Text15 = dataRow["Text15"].ToString();
            //this.Text16 = dataRow["Text16"].ToString();
            //this.Text17 = dataRow["Text17"].ToString();
            //this.Text18 = dataRow["Text18"].ToString();
            //this.Text19 = dataRow["Text19"].ToString();
            //this.Text20 = dataRow["Text20"].ToString();
            //this.Text21 = dataRow["Text21"].ToString();
            //this.Text22 = dataRow["Text22"].ToString();
            //this.Text23 = dataRow["Text23"].ToString();
            //this.Text24 = dataRow["Text24"].ToString();
            //this.Text25 = dataRow["Text25"].ToString();
            //this.Text26 = dataRow["Text26"].ToString();
            //this.Text27 = dataRow["Text27"].ToString();
            //this.Text28 = dataRow["Text28"].ToString();
            //this.Text29 = dataRow["Text29"].ToString();
            //this.Text30 = dataRow["Text30"].ToString();
            //this.LongText1 = dataRow["LongText1"].ToString();
            //this.LongText2 = dataRow["LongText2"].ToString();
            //this.LongText3 = dataRow["LongText3"].ToString();
            //this.LongText4 = dataRow["LongText4"].ToString();
            //this.LongText5 = dataRow["LongText5"].ToString();
            //this.Html1 = dataRow["Html1"].ToString();
            //this.Html2 = dataRow["Html2"].ToString();
            //this.Html3 = dataRow["Html3"].ToString();
            //this.Html4 = dataRow["Html4"].ToString();
            //this.Html5 = dataRow["Html5"].ToString();
            //this.Image1 = dataRow["Image1"].ToString();
            //this.Image2 = dataRow["Image2"].ToString();
            //this.Image3 = dataRow["Image3"].ToString();
            //this.Image4 = dataRow["Image4"].ToString();
            //this.Image5 = dataRow["Image5"].ToString();
            //this.File1 = dataRow["File1"].ToString();
            //this.File2 = dataRow["File2"].ToString();
            //this.File3 = dataRow["File3"].ToString();
            //this.File4 = dataRow["File4"].ToString();
            //this.File5 = dataRow["File5"].ToString();
            //this.YesNo1 = DataConverter.ToBoolean(dataRow["YesNo1"]);
            //this.YesNo2 = DataConverter.ToBoolean(dataRow["YesNo2"]);
            //this.YesNo3 = DataConverter.ToBoolean(dataRow["YesNo3"]);
            //this.YesNo4 = DataConverter.ToBoolean(dataRow["YesNo4"]);
            //this.YesNo5 = DataConverter.ToBoolean(dataRow["YesNo5"]);
            //this.Numeric1 = DataConverter.ToInt32(dataRow["Numeric1"]);
            //this.Numeric2 = DataConverter.ToInt32(dataRow["Numeric2"]);
            //this.Numeric3 = DataConverter.ToInt32(dataRow["Numeric3"]);
            //this.Numeric4 = DataConverter.ToInt32(dataRow["Numeric4"]);
            //this.Numeric5 = DataConverter.ToInt32(dataRow["Numeric5"]);
            //this.Currency1 = DataConverter.ToDouble(dataRow["Currency1"]);
            //this.Currency2 = DataConverter.ToDouble(dataRow["Currency2"]);
            //this.Currency3 = DataConverter.ToDouble(dataRow["Currency3"]);
            //this.Currency4 = DataConverter.ToDouble(dataRow["Currency4"]);
            //this.Currency5 = DataConverter.ToDouble(dataRow["Currency5"]);
            //this.DateTime1 = DataConverter.ToDateTime(dataRow["DateTime1"]);
            //this.DateTime2 = DataConverter.ToDateTime(dataRow["DateTime2"]);
            //this.DateTime3 = DataConverter.ToDateTime(dataRow["DateTime3"]);
            //this.DateTime4 = DataConverter.ToDateTime(dataRow["DateTime4"]);
            //this.DateTime5 = DataConverter.ToDateTime(dataRow["DateTime5"]);
            //if (dataRow["FK_1"] != DBNull.Value)
            //{
            //    this.LookupValue1 = new DataLookupValue();
            //    this.LookupValue1.ID = new Guid(dataRow["FK_1"].ToString());
            //}
            //if (dataRow["FK_2"] != DBNull.Value)
            //{
            //    this.LookupValue2 = new DataLookupValue();
            //    this.LookupValue2.ID = new Guid(dataRow["FK_2"].ToString());
            //}
            //if (dataRow["FK_3"] != DBNull.Value)
            //{
            //    this.LookupValue3 = new DataLookupValue();
            //    this.LookupValue3.ID = new Guid(dataRow["FK_3"].ToString());
            //}
            //if (dataRow["FK_4"] != DBNull.Value)
            //{
            //    this.LookupValue4 = new DataLookupValue();
            //    this.LookupValue4.ID = new Guid(dataRow["FK_4"].ToString());
            //}
            //if (dataRow["FK_5"] != DBNull.Value)
            //{
            //    this.LookupValue5 = new DataLookupValue();
            //    this.LookupValue5.ID = new Guid(dataRow["FK_5"].ToString());
            //}
            //if (dataRow["FK_6"] != DBNull.Value)
            //{
            //    this.LookupValue6 = new DataLookupValue();
            //    this.LookupValue6.ID = new Guid(dataRow["FK_6"].ToString());
            //}
            //if (dataRow["FK_7"] != DBNull.Value)
            //{
            //    this.LookupValue7 = new DataLookupValue();
            //    this.LookupValue7.ID = new Guid(dataRow["FK_7"].ToString());
            //}
            //if (dataRow["FK_8"] != DBNull.Value)
            //{
            //    this.LookupValue8 = new DataLookupValue();
            //    this.LookupValue8.ID = new Guid(dataRow["FK_8"].ToString());
            //}
            //if (dataRow["FK_9"] != DBNull.Value)
            //{
            //    this.LookupValue9 = new DataLookupValue();
            //    this.LookupValue9.ID = new Guid(dataRow["FK_9"].ToString());
            //}
            //if (dataRow["FK_10"] != DBNull.Value)
            //{
            //    this.LookupValue10 = new DataLookupValue();
            //    this.LookupValue10.ID = new Guid(dataRow["FK_10"].ToString());
            //}

            //this.IsLoaded = true;
        }

    }
}
