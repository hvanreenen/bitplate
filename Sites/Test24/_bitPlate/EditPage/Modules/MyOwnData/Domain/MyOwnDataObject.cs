using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitPlate.Domain;

namespace BitSite._bitPlate._bitModules.MyOwnData.Domain
{
    public class MyOwnDataObject: BaseDomainSiteObject
    {
        public string ArtName { get; set; }
        public string ArtCode { get; set; }
        public double Price { get; set; }
    }
}