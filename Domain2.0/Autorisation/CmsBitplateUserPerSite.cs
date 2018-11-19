using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM.Attributes;

namespace BitPlate.Domain.Autorisation
{
    public class CmsBitplateUserPerSite: BaseDomainObject
    {
        private CmsSite _site;
        [Association("FK_Site")]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public CmsSite Site
        {
            get
            {
                if (_site != null && !_site.IsLoaded)
                {
                    _site.Load();
                }

                return _site;
            }
            set { _site = value; }
        }

        private CmsBitplateUser _bitplateUser;
        [Association("FK_Site")]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public CmsBitplateUser BitplateUser
        {
            get
            {
                if (_bitplateUser != null && !_bitplateUser.IsLoaded)
                {
                    _bitplateUser.Load();
                }

                return _bitplateUser;
            }
            set { _bitplateUser = value; }
        }
    }
}
