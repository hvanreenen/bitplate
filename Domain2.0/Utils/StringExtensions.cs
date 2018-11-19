using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BitPlate.Domain.Utils
{
    public static class StringExtensions
    {
        public static int[] IndexesOf(this string self, string needle)
        {
            int start = 0;
            int index;
            List<int> indexes = new List<int>();
            while ((index = self.IndexOf(needle, start)) >= 0)
            {
                indexes.Add(index);
                start = index + 1;
            }
            return indexes.ToArray();
        }

        //public static string Format(string input, object obj)
        //{
        //    foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(obj))
        //        input = input.Replace("{" + prop.Name + "}", (prop.GetValue(obj) ?? "(null)").ToString());

        //    return input;
        //}

        //public static string StringFormat(string format, object source)
        //{
        //    var matches = Regex.Matches(format, @"\{(.+?)\}");
        //    List<string> keys = (from Match matche in matches select matche.Groups[1].Value).ToList();

        //    return keys.Aggregate(
        //        format,
        //        (current, key) =>
        //        {
        //            int colonIndex = key.IndexOf(':');
        //            return current.Replace(
        //                "{" + key + "}",
        //                colonIndex > 0
        //                    ? DataBinder.Eval(source, key.Substring(0, colonIndex), "{0:" + key.Substring(colonIndex + 1) + "}")
        //                    : DataBinder.Eval(source, key).ToString());
        //        });
        //}

        /* public static string HightlightSearchResults(this string self, string needle)
        {
            int[] foundIndexes = self.ToLower().IndexesOf(needle.ToLower());
            foreach (int index in foundIndexes.OrderByDescending(c => c))
            {
                string orginalstring = self.Substring(index, needle.Length);
                string hightlightIndex = orginalstring;
                hightlightIndex = "<span class='highlight'>" + hightlightIndex + "</span>";
                self = self.Substring(0, index) + hightlightIndex + self.Substring(index + needle.Length); //str.Replace(orginalstring, hightlightIndex);
            }
            return self;
        } */
    }
}
