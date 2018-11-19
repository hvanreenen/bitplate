using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Utils;
using System.IO;
using System.Web;

namespace BitPlate.Domain
{

    /// <summary>
    /// Object in CMS kan inactief/actief zijn
    /// 3 mogelijkheden
    /// Active, InActive en ActifeFrom
    /// Bij ActiveFrom geldt een periode waarin object actief is van begin en einddatum, of alleen begindatum of alleen einddatum
    /// </summary>
    public enum ActiveEnum { InActive, Active, ActiveFrom};

    /// <summary>
    /// BaseDomainObject is basis class voor domain objecten
    /// Erft over van HJORM.BaseObject. Hierdoor kan het worden opgeslagen in DB
    /// BaseDomainObject kent extra gegevens of het object actief is
    /// </summary>
    //[System.Runtime.Serialization.DataContract(IsReference = true)]
    
    public abstract class BaseDomainObject : BaseObject
    {
        public BaseDomainObject()
        {
            Active = ActiveEnum.Active;
        }

        public virtual ActiveEnum Active { get; set; }
        /// <summary>
        /// Active From
        /// </summary>
        public DateTime? DateFrom { get; set; }
        /// <summary>
        /// Active Till
        /// </summary>
        public DateTime? DateTill { get; set; }

        

        protected string _isActiveString = "Actief";
        /// <summary>
        /// Is het Objcet Actief?
        /// Afhankelijk van Setting van Active en van DateFrom en DateTill
        /// </summary>
        [NonPersistent()]
        [System.Xml.Serialization.XmlIgnore()]
        public virtual bool IsActive
        {
            get
            {
                bool returnValue = true;
                if (Active == ActiveEnum.ActiveFrom)
                {
                    returnValue = (this.DateFrom.GetValueOrDefault(DateTime.MinValue) <= DateTime.Today && this.DateTill.GetValueOrDefault(DateTime.MaxValue) >= DateTime.Today);
                }
                else if (Active == ActiveEnum.InActive)
                {
                    returnValue = false;
                }
                if (!returnValue)
                {
                    _isActiveString = "Niet Actief";
                }
                return returnValue;
            }
            private set
            {
                bool dummy = value;
            }
        }
        /// <summary>
        /// Van enum wordt string gemaakt
        /// </summary>
        [NonPersistent()]
        public virtual string IsActiveString
        {
            get
            {
                return _isActiveString;
            }
            set
            {
                _isActiveString = value;
            }
        }
        /// <summary>
        /// returns Object.Name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        public override void Save()
        {
            
            base.Save();
            
           // Logging.EventLog.LogSaveEvent(this);
        }

        public override void Delete()
        {
            base.Delete();
            //Logging.EventLog.LogDeleteEvent(this);
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            this.Active = (ActiveEnum)DataConverter.ToInt32(dataRow["Active"]);
            this.DateFrom = DataConverter.ToNullableDateTime(dataRow["DateFrom"]);
            this.DateTill = DataConverter.ToNullableDateTime(dataRow["DateTill"]);
        }

    }
}
