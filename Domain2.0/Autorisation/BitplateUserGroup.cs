using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using System.Runtime.Serialization;
//using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Licenses;


namespace BitPlate.Domain.Autorisation
{
    public enum UserTypeEnum
    {
        Custom = 0,
        Moderators = 1, //heeft alleen rechten die zijn aangegeven
        Designers = 2, //heeft alleen rechten die zijn aangegeven
        SiteAdmins = 9, //mag alles
        Developers = 99 //mag alles
    }

    public class BitplateUserGroup : BaseUserGroup
    {

        public UserTypeEnum Type { get; set; }

        [NonPersistent()]
        public string TypeString
        {
            get
            {
                return Type.ToString();
            }
            set { }
        }

        private int[] _permissions; 
        public int[] Permissions { 
            get {
                if ((Type == UserTypeEnum.SiteAdmins || Type == UserTypeEnum.Developers)
                    && Utils.WebSessionHelper.CurrentLicense != null)
                {
                    _permissions = Utils.WebSessionHelper.CurrentLicense.FunctionNumbers;
                }
                return _permissions;
            }
            set {_permissions = value;}
        }


        public bool IsSystemValue { get; set; }

        public override void Save()
        {
            
            base.Save();
        }

        public override void Delete()
        {
            if (IsSystemValue)
            {
                throw new Exception("Systeemgroepen kunnen niet worden verwijderd. Gooi hiervoor in de plaats het bedrijf of de site weg");
            }
            else
            {
                base.Delete();
            }
        }
    }
}
