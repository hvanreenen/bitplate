using System;
using System.Collections.Generic;

using System.Text;

namespace HJORM.Attributes
{
    /// <summary>
    /// Wordt niet gebruikt
    /// </summary>
    public class Discriminator: Attribute
    {
        public string Where = "";
        public Discriminator(string where)
        {
            Where = where;
        }
    }
}
