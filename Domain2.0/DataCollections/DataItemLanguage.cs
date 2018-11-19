using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM.Attributes;

namespace BitPlate.Domain.DataCollections
{
    public class DataItemLanguage: BaseDataLanguageObject
    {
        protected DataItem _dataItem;

        [System.Web.Script.Serialization.ScriptIgnore]
        [Association("FK_DataItem")]
        public virtual DataItem DataItem
        {
            get
            {
                if (_dataItem != null && !_dataItem.IsLoaded)
                {
                    _dataItem.Load();
                }
                return _dataItem;
            }
            set { _dataItem = value; }
        }


        public override void FillObject(System.Data.DataRow dataRow)
        {
            FillObject(dataRow, dataRow.Table.Columns);
        }

        public void FillObject(System.Data.DataRow dataRow, System.Data.DataColumnCollection columns)
        {
            base.FillObject(dataRow, columns);
            if (dataRow["FK_DataItem"] != DBNull.Value)
            {
                this.DataItem = new DataItem();
                this.DataItem.ID = HJORM.DataConverter.ToGuid(dataRow["FK_DataItem"]);
            }
            this.IsLoaded = true;
        }
    }
}
