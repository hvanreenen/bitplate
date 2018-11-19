using System;
using System.Collections.Generic;

using System.Text;

namespace HJORM.Attributes
{
    public class Association: Attribute
    {
        public string ForeignObjectName = "";
        public string  ForeignKey = "";
        public string CollectionForeignKey = "";
        //public string ParentPropertyName = "";
        public Association(string foreignKey)
        {
            ForeignKey = foreignKey;
        }
        public Association(string thisForeignKey, string collectionForeignKey)
        {
            ForeignKey = thisForeignKey;
            CollectionForeignKey = collectionForeignKey;
        }
        public Association(string foreignObjectName, string thisForeignKey, string collectionForeignKey)
        {
            ForeignObjectName = foreignObjectName;
            ForeignKey = thisForeignKey;
            CollectionForeignKey = collectionForeignKey;
        }
        public Association()
        {
            
        }
    }
}
