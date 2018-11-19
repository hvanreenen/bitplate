using System;
using System.Collections.Generic;

using System.Text;

namespace HJORM.Attributes
{
    /// <summary>
    /// Gebruik dit attribuut als je een property niet wilt opslaan in de database
    /// <example>
    /// public class Person: BaseObject{
    ///     public string Name {get; set;}
    ///     public DateTime Birthdate {get; set;}
    ///     [NonPersistent()]
    ///     public int Age {
    ///         get{
    ///             //return CurrentDate - BirtDate;
    ///         }
    ///     }
    /// }
    /// </example>
    /// </summary>
    public class NonPersistent: Attribute
    {
    }
}
