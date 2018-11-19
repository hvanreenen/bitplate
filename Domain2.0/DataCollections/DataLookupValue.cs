using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Utils;
using BitPlate.Domain.Modules;


namespace BitPlate.Domain.DataCollections
{
    public class DataLookupValue : BaseDomainSiteObject
    {
        //private DataCollection _dataCollection;
        //[Association("FK_DataCollection")]
        //public virtual DataCollection DataCollection
        //{
        //    get
        //    {
        //        if (_dataCollection != null && _dataCollection.IsLoaded)
        //        {
        //            _dataCollection.Load();
        //        }
        //        return _dataCollection;
        //    }
        //    set { _dataCollection = value; }
        //}

        private DataField _field;
        [Association("FK_DataField")]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public DataField DataField
        {
            get
            {
                if (_field != null && _field.IsLoaded)
                {
                    _field.Load();
                }
                return _field;
            }
            set { _field = value; }
        }
        ////hier json vertalingen opslaan
        ////public string Languages { get; set; }
        //private string _languagesJsonString;
        //[Persistent("Languages")]
        //public string LanguagesJsonString
        //{
        //    get { return this._languagesJsonString; }
        //    set
        //    {
        //        this._languagesJsonString = value;
        //        Languages = JSONSerializer.Deserialize<Dictionary<string, object>>(value);
        //    }
        //}

        //[NonPersistent()]
        //[System.Web.Script.Serialization.ScriptIgnore()]
        //public Dictionary<string, object> Languages { get; set; }

        //voorlopig doen we het even even zo: per taal eigen veld, want het volstaat. Later als bitplate echt multinational wordt is er wel tijd om het beter te maken
        public string NL { get; set; }
        public string EN { get; set; }
        public string DE { get; set; }
        public string FR { get; set; }
        public string SP { get; set; }
        public string PL { get; set; }
        public string IT { get; set; }
        
        public void Copy(Guid newFieldID)
        {
            DataLookupValue newLookup = this.CreateCopy<DataLookupValue>(false);
            newLookup.DataField = new DataField();
            newLookup.DataField.ID = newFieldID;
            newLookup.Save();
        }

        public void FillObject(System.Data.DataRow dataRow, System.Data.DataColumnCollection columns)
        {

            base.FillObject(dataRow);
            //if (dataRow["FK_DataCollection"] != DBNull.Value)
            //{
            //    this.DataCollection = new DataCollection();
            //    this.DataCollection.ID = new Guid(dataRow["FK_DataCollection"].ToString());
            //}
            if (dataRow["FK_DataField"] != DBNull.Value)
            {
                this.DataField = new DataField();
                this.DataField.ID = new Guid(dataRow["FK_DataField"].ToString());
            }
            this.NL = dataRow["NL"].ToString();
            this.EN = dataRow["EN"].ToString();
            this.DE = dataRow["DE"].ToString();
            this.FR = dataRow["FR"].ToString();
            this.SP = dataRow["SP"].ToString();
            this.IT = dataRow["IT"].ToString();
            this.PL = dataRow["PL"].ToString();
            
            this.IsLoaded = true;
        }
    
    }
}
