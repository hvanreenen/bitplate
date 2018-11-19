using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Licenses
{
    public class LicenseStatistics
    {
        public string Name { get; set; }
        public int? Available { get; set; }
        public int Used { get; set; }
        public int? Over
        {
            get
            {
                if (Available.HasValue)
                {
                    return Available.Value - Used;
                }
                else
                {
                    return null;
                }

            }
        }
    }
}
