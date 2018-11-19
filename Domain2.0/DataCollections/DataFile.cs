using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM.Attributes;

namespace BitPlate.Domain.DataCollections
{
    public class DataFile : BaseDomainSiteObject
    {
        public const string TYPE_EXTRA_FILE = "ExtraFile";
        public const string TYPE_EXTRA_IMAGE = "ExtraImage";

        private DataItem _item;
        private DataGroup _group;
        
        public string Language { get; set; }
        public string Url { get; set; }
        [NonPersistent()]
        public string ThumbnailUrl { 
            get 
            {
                string thumb = Utils.ImageHelper.GetThumbnailFileName(Url);
                return thumb;
            }
        }
        public string Type { get; set; } //extra file or extra image

        private string _name;
        public virtual string Name
        {
            get
            {
                if (_name == Url && Url != "" )
                {
                    int posLastSlash = Url.LastIndexOf("/");
                    _name = Url.Substring(posLastSlash + 1, Url.Length - posLastSlash - 5);
                }
                return _name;
            }
            set { _name = value; }
        }

        [Association("FK_Group")]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public DataGroup DataGroup
        {
            get
            {
                if (_group != null && _group.IsLoaded)
                {
                    _group.Load();
                }
                return _group;
            }
            set { _group = value; }
        }

        [Association("FK_Item")]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public DataItem DataItem
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

        [NonPersistent()]
        public string FileType
        {
            get {
                string returnValue = "";
                if (Url != null && Url != "" && Url.Length > 3)
                {
                    returnValue = Url.Substring(Url.Length - 3, 3).ToLower();
                }
                return returnValue;
            
            }
        }

        public void FillObject(System.Data.DataRow dataRow, System.Data.DataColumnCollection columns)
        {

            base.FillObject(dataRow);
            if (dataRow["FK_Group"] != DBNull.Value)
            {
                this.DataGroup = new DataGroup();
                this.DataGroup.ID = HJORM.DataConverter.ToGuid(dataRow["FK_Group"]);
            }
            if (dataRow["FK_Item"] != DBNull.Value)
            {
                this.DataItem = new DataItem();
                this.DataItem.ID = HJORM.DataConverter.ToGuid(dataRow["FK_Item"]);
            }
            this.Type = dataRow["Type"].ToString();
            this.Language = dataRow["Language"].ToString();
            this.Url = dataRow["Url"].ToString();
            this.IsLoaded = true;
        }
    }
}
