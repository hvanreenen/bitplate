using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules
{
    public class NavigationParameterObject
    {
        public string Name { get; set; }
        public Guid GuidValue { get; set; }
        public int IntValue { get; set; }
        public object ObjectValue { get; set; }
        public string StringValue { get; set; }
    }
}
