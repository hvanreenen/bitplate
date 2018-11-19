using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Utils
{
    /// <summary>
    /// in deze class uitbreidingen (extension methods) van bestaande types (enums en bools)
    /// wel maken voor elk type een methode Name()
    /// hiermee kan de bool of ennum een (vertaalde) beschrijving krijgen
    /// </summary>
    public static class TypeExtensions
    {
        public static string Name(this Enum self)
        {
            string enumType = self.GetType().Name;
            string enumName = self.ToString();
            int enumValue = Convert.ToInt32(self);
            string langCode = "NL";
            //get from db
            //string where = String.Format("EnumType = '{0}' AND EnumValue = {1} AND LanguageCode = '{2}'", enumType, enumValue, langCode);
            //vertaal
            string name = Translator.Translate(langCode, enumType, enumName);
            return name;
        }

        public static string Name(this Boolean self)
        {
            if (self)
                return "Ja";
            else
                return "Nee";
        }

        public static bool IsNummeric(this string self)
        {
            try
            {
                double.Parse(self);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
