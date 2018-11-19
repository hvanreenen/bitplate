using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Utils
{
    public static class CollectionsHelper
    {
        public static Dictionary<string, string> ConvertFormParametersToDictionary(System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string key in nameValueCollection.AllKeys)
            {
                if (dict.ContainsKey(key))
                {
                    dict[key] += "," + nameValueCollection[key];
                }
                else
                {
                    dict.Add(key, nameValueCollection[key]);
                }
            }
            return dict;
        }
    }
}
