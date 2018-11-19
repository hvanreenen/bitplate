using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitSite
{
    /// <summary>
    /// Class wordt gebruikt tijdens databind van datamodules.
    /// Wordt aangeroepen als String.Format(new CustomFormatProvider(), String, args)
    /// Doel is om naast standaard .Net formats voor dates en numbers ook een format voor booleans te hebben
    /// 
    /// <example>
    /// <%#String.Format(new BitSite.CustomFormatProvider(), "{0:Aan;Uit}", DataBinder.Eval(Container.DataItem,"YesNo1"))%>
    /// waarde voor puntkomma is true part, waarde na puntkomma is false part
    /// </example>
    /// </summary>
    public class CustomFormatProvider : IFormatProvider, ICustomFormatter
    {
        #region ICustomFormatter Members

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg.GetType() == typeof(bool))
            {
                bool value = (bool)arg;
               
                if (format!= null && format.Contains(";"))
                {
                    string[] true_false = format.Split(new char[] { ';' });
                    return value ? true_false[0] : true_false[1];
                }
                else
                {
                    return arg.ToString();
                }
            }
            else
            {
                if (arg is IFormattable)
                    return ((IFormattable)arg).ToString(format, formatProvider);
                else
                    return arg.ToString();
            }
        }

        #endregion

        #region IFormatProvider Members

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        #endregion
    }
}