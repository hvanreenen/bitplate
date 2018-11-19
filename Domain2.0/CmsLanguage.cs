using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM.Attributes;

namespace BitPlate.Domain
{
    
    [Persistent("Language")]
    public class CmsLanguage: BaseDomainSiteObject
    {
        //LandCode
        [Persistent("Code")]
        public string LanguageCode { get; set; }
    }
}
