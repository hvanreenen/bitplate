using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM.Attributes;

namespace BitPlate.Domain.DataCollections
{
    public class DataGroupLanguage : BaseDataLanguageObject
    {
        protected DataGroup _dataGroup;

        [System.Web.Script.Serialization.ScriptIgnore]
        [Association("FK_DataGroup")]
        public virtual DataGroup DataGroup
        {
            get
            {
                if (_dataGroup != null && !_dataGroup.IsLoaded)
                {
                    _dataGroup.Load();
                }
                return _dataGroup;
            }
            set { _dataGroup = value; }
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            FillObject(dataRow, dataRow.Table.Columns);
        }
        public void FillObject(System.Data.DataRow dataRow, System.Data.DataColumnCollection columns)
        {

            base.FillObject(dataRow, columns);
            if (dataRow["FK_DataGroup"] != DBNull.Value)
            {
                this.DataGroup = new DataGroup();
                this.DataGroup.ID = new Guid(dataRow["FK_DataGroup"].ToString());
            }
            this.IsLoaded = true;
        }
    
    }
}
