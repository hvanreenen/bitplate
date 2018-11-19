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
    /// 
    
    public class BaseDataLanguageObject : BaseDomainPublishableObject
    {
       
        public string Title {get; set;}
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

        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }

        public string LanguageCode { get; set; }

        


        
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }
        public string Text5 { get; set; }
        public string LongText1 { get; set; }
        public string Html1 { get; set; }
       
        public string Image1 { get; set; }
        public string File1 { get; set; }
        public string FK_1 { get; set; }
        public string FK_2 { get; set; }
        public string FK_3 { get; set; }
        public string FK_4 { get; set; }
        public string FK_5 { get; set; }
        public string FK_6 { get; set; }
        public string FK_7 { get; set; }
        public string FK_8 { get; set; }
        public string FK_9 { get; set; }
        public string FK_10 { get; set; }

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

       

        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Image5 { get; set; }

        public string File2 { get; set; }
        public string File3 { get; set; }
        public string File4 { get; set; }
        public string File5 { get; set; }


        public override T CreateCopy<T>(bool newName)
        {
            return base.CreateCopy<T>(newName);
        }



        public void FillObject(System.Data.DataRow dataRow, System.Data.DataColumnCollection columns)
        {


            base.FillObject(dataRow);
            this.Title = dataRow["Title"].ToString();
            this.LanguageCode = dataRow["LanguageCode"].ToString();
            if (dataRow["FK_DataCollection"] != DBNull.Value)
            {
                this.DataCollection = new DataCollection();
                this.DataCollection.ID = new Guid(dataRow["FK_DataCollection"].ToString());
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
            
        }
        public override void FillObject(System.Data.DataRow dataRow)
        {
            FillObject(dataRow, dataRow.Table.Columns);

        }

    }
}
