using System;
using System.Collections.Generic;
using System.Text;
using HJORM;
using HJORM.Attributes;
using System.Web.Script.Serialization;
using BitPlate.Domain.Utils;

namespace BitPlate.Domain
{
    /// <summary>
    /// Objecten die altijd in een site kunnen voorkomen. 
    /// Elke pagina bijvoorbeeld is onderdeel van een site.
    /// Standaard hebben alle objecten in domein een Site en in de database een FK_Site
    /// Hierdoor zijn deze objecten eenvoudig te expoteren en importeren in de database
    /// </summary>
    
    public abstract class BaseDomainSiteObject : BaseDomainObject
    {
        private CmsSite _site;
        /// <summary>
        /// Haal Object uit WebSession (System.Web.HttpContext.Current.Session["CurrentSite"])
        /// </summary>
        [System.Xml.Serialization.XmlIgnore()]
        [System.Web.Script.Serialization.ScriptIgnore()]
        [Association("FK_Site")]
        public CmsSite Site
        {
            get
            {
                if (_site == null)
                {
                    _site = WebSessionHelper.CurrentSite;
                }
                return _site;
            }
            set
            {
                
                _site = value;
                //System.Web.HttpContext.Current.Session["CurrentSite"] = value;
            }
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            //DataRow FK_Site not exist fix.
            if (dataRow.Table.Columns.Contains("FK_Site") && dataRow["FK_Site"] != DBNull.Value)
            {
                if (Object.ReferenceEquals(null, this.Site))
                {
                    this.Site = new CmsSite();
                    this.Site.ID = DataConverter.ToGuid(dataRow["FK_Site"]);
                    this.Site.Load();
                }
            }
        }
    }
}
