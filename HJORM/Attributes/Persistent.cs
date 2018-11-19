using System;
using System.Collections.Generic;

using System.Text;

namespace HJORM.Attributes
{
    public class Persistent: Attribute
    {
        /// <summary>
        /// Gebruik dit attribuut voor alternatieve DB-tabelnaam bij class of alternatief veldnaam bij Property
        /// Dat wil zeggen, de Tablenaam wijkt af van de exacte class naam.
        /// <example>
        /// [Persistent("Persoon")]
        /// public class Person: BaseObject{
        ///     [Persistent("Naam")]
        ///     public string Name {get; set;}
        /// }
        /// </example>
        /// </summary>
        public string DataBaseObject = "";
        
        
        public Persistent(string dataBaseObject)
        {
            DataBaseObject = dataBaseObject;
        }
    }
}
