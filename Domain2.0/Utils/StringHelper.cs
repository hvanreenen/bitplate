using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Utils
{
    public static class StringHelper
    {
        public static string ReplaceFirstOccurrance(string original, string oldValue, string newValue)
        {
            if (String.IsNullOrEmpty(original))
                return String.Empty;
            if (String.IsNullOrEmpty(oldValue))
                return original;
            if (String.IsNullOrEmpty(newValue))
                newValue = String.Empty;
            int loc = original.IndexOf(oldValue);
            return original.Remove(loc, oldValue.Length).Insert(loc, newValue);
        }

        public static int CountOccurences(string content, string find)
        {
            return (content.Length - content.Replace(find, "").Length) / find.Length;
        }

        public static string HighlightSearchResults(string str, string searchString)
        {
            int[] foundIndexes = str.ToLower().IndexesOf(searchString.ToLower());
            foreach (int index in foundIndexes.OrderByDescending(c => c))
            {
                string orginalstring = str.Substring(index, searchString.Length);
                string hightlightIndex = orginalstring;
                hightlightIndex = "<span class='highlight'>" + hightlightIndex + "</span>";
                str = str.Substring(0, index) + hightlightIndex + str.Substring(index + searchString.Length); //str.Replace(orginalstring, hightlightIndex);
            }
            return str;
        }
    }
}
