using System;
using System.Collections.Generic;
using System.Text;

namespace HJORM.Attributes
{
    public enum InheritanceEnum { OwnTable, ParentTable, TwoTables } 
    public class Inheritance : Attribute
    {
        public InheritanceEnum InheritanceType = InheritanceEnum.OwnTable;
        //public string ParentTable;
        //public string ForeignKey;
        public Inheritance(InheritanceEnum inheritanceType)
        {
            InheritanceType = inheritanceType;
            //if (InheritanceType == InheritanceEnum.ParentTable)
            //{
            //    ParentTable = JoinedObject;
            //}
            //else
            //{
            //    ForeignKey = JoinedObject;
            //}
        }
        public Inheritance()
        {
            
        }

    }
}
