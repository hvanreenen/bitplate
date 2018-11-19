using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Modules;


namespace BitPlate.Domain.DataCollections
{
    
    
    public class DataExtraField : BaseDomainSiteObject
    {
        public FieldTypeEnum FieldType { get; set; }
        
        public string Language { get; set; }

        private DataCollection _dataCollection;
        [Association("FK_DataCollection")]
        public virtual DataCollection DataCollection
        {
            get
            {
                if (_dataCollection != null && _dataCollection.IsLoaded)
                {
                    _dataCollection.Load();
                }
                return _dataCollection;
            }
            set { _dataCollection = value; }
        }

        BaseCollection<DataExtraFieldOption> _options;
        [NonPersistent()]
        
        public BaseCollection<DataExtraFieldOption> Options
        {
            get
            {
                //if (_options == null)
                {
                    _options = BaseCollection<DataExtraFieldOption>.Get("FK_DataExtraField='" + this.ID + "'");
                }
                return _options;
            }
            set
            {
                _options = value;
            }
        }
    }
}
