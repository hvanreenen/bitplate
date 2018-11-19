using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM.Attributes;
using BitPlate.Domain.DataCollections;
namespace BitPlate.Domain.Search
{
    [Persistent("SearchIndex")]
    public class SearchResultItem : BaseDomainSiteObject
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        [System.Web.Script.Serialization.ScriptIgnore()]
        public string Content { get; set; }
        
        [NonPersistent()]
        public string ContentFirst80Chars
        {
            get
            {
                if (Content.Length <= 80)
                {
                    return Content;
                }
                else
                {
                    return Content.Substring(0, 80) + "...";
                }
            }
        }
        //public Guid FK_Page { get; set; }
        public string FK_DataCollection { get; set; }
        //[Association("FK_Page")]
        //public CmsPage Page { get; set; }
        //[Association("FK_Module")]
        //public Module Module { get; set; }
        //[Association("FK_Group")]
        //public DataGroup Group { get; set; }
        //[Association("FK_Item")]
        //public DataItem Item { get; set; }
        [NonPersistent()]
        public double Score { get; set; }
        public string Type { get; set; }

        //public string Language { get; set; }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            this.Title = dataRow["Title"].ToString();
            this.Url = dataRow["Url"].ToString();
            this.MetaDescription = dataRow["MetaDescription"].ToString();
            this.MetaKeywords = dataRow["MetaKeywords"].ToString();
            this.Content = dataRow["Content"].ToString();
            this.Type = dataRow["Type"].ToString();
            this.Score = Convert.ToDouble(dataRow["Score"]);
            this.IsLoaded = true;
        }
    }

}
