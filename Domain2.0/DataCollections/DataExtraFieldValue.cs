using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Modules;
//using BitPlate.Domain.DataCollections.Webshop;
namespace BitPlate.Domain.DataCollections
{
    public class DataExtraFieldValue : BaseDomainSiteObject
    {
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

        private DataItem _item;
        [Association("FK_Item")]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public DataItem Item
        {
            get
            {
                if (_item != null && _item.IsLoaded)
                {
                    _item.Load();
                }
                return _item;
            }
            set { _item = value; }
        }

        private DataExtraField _extraField;
        [Association("FK_DataExtraField")]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public DataExtraField ExtraField
        {
            get
            {
                if (_extraField != null && _extraField.IsLoaded)
                {
                    _extraField.Load();
                }
                return _extraField;
            }
            set { _extraField = value; }
        }

        private DataExtraFieldOption _extraFieldOption;
        [Association("FK_DataExtraFieldOption")]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public DataExtraFieldOption ExtraFieldOption
        {
            get
            {
                if (_extraFieldOption != null && _extraFieldOption.IsLoaded)
                {
                    _extraFieldOption.Load();
                }
                return _extraFieldOption;
            }
            set { _extraFieldOption = value; }
        }
    }
}
