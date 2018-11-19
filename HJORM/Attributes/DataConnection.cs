using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HJORM.Attributes
{
    public class DataConnection : Attribute
    {
        public string DataBaseConnectionName = "";

        public DataConnection(string dataBaseConnectionName)
        {
            DataBaseConnectionName = dataBaseConnectionName;
        }
    }
}
