using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BitPlate.Domain.Modules
{
    /// <summary>
    /// Module kent tags-collectie
    /// Tags zijn altijd tussen { en }
    /// Tags hebben vervangwaarde. Tijdens publiceren van de module worden de tags uit de content van de module vervangen door de vervangwaarde van de tag
    /// </summary>
    public class Tag
    {
        public Tag()
        {
        }

        public Tag(string tagName)
        {
            this.Name = tagName;
        }

        public Tag(string tagName, string replaceValue): this(tagName)
        {
            this.ReplaceValue = replaceValue;
        }

        public Tag(string tagName, string replaceValue, bool hasCloseTag, string replaceValueCloseTag): this(tagName, replaceValue)
        {
            this.HasCloseTag = hasCloseTag;
            this.ReplaceValueCloseTag = replaceValueCloseTag;
        }
        /// <summary>
        /// Tagname inclusief { en }
        /// Bijvoorbeeld {Titel}
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Vervangwaarde voor tag tijdens publiceren van de module
        /// bijvoorbeeld {textbox} wordt <asp:TextBox runat="server" />
        /// </summary>
        public string ReplaceValue { get; set; }
        /// <summary>
        /// Tag heeft sluit tag
        /// Bijvoorbeeld {submitButton} en {/submitButton}
        /// Gebruiker kan dit weergeven als: {submitButton}OK{/submitButton}
        /// </summary>
        public bool HasCloseTag { get; set; }
        /// <summary>
        /// Vervangwaarde voor sluittag tijdens publiceren van de module
        /// {/submitButton} wordt </asp:Button>
        /// </summary>
        public string ReplaceValueCloseTag { get; set; }
        /// <summary>
        /// Tag staat formats toe
        /// Tag kan worden neergezet als {Datum:dd-MM-yyyy} of {Bedrag:C} 
        /// Formats zijn door gebruiker aan te passen, moet wel .Net compatible zijn
        /// </summary>
        public bool AllowFormats { get; set; }
        /// <summary>
        /// Voorbeeld formats voor weergave in tags-popup
        /// Formats zijn door gebruiker aan te passen, moet wel .Net compatible zijn
        /// </summary>
        public string[] SampleFormats { get; set; }
        /// <summary>
        /// Automatische naam van sluitTag
        /// in de Naam wordt { vervangen door {/
        /// </summary>
        public string[] SampleParameters { get; set; }

        public string CloseTag
        {
            get
            {
                if (HasCloseTag)
                {
                    return Name.Replace("{", "{/");
                }
                else
                {
                    return "";
                }
            }
        }

        public string[] AvailableArguments { get; set; }

        public bool HasArguments()
        {
            return Name.Contains(":");
        }

        public List<ModuleTagArgument> GetTagArgumentsFromHtml()
        {
            string sparametes = Regex.Match(this.Name, "{" + this.Name + ":(.*?)}", RegexOptions.Singleline).ToString().Replace("{" + this.Name + ":", "").Replace("}", "");
            List<string> lparameters = Regex.Split(sparametes, "/").ToList();
            List<ModuleTagArgument> parameters = new List<ModuleTagArgument>();
            foreach (string parameter in lparameters)
            {
                ModuleTagArgument argument = new ModuleTagArgument();
                argument.Argument = parameter;
                parameters.Add(argument);
            }
            return parameters;
        }

        public string ReplaceTagInHtml(string html, string value)
        {
            string tag = Regex.Match(html, this.Name.Replace("}", "(.*?)}")).ToString();
            string argument = this.GetSingleArgumentFromHtml(tag);
            return ""; //TIJDELIJKE FIX.
        }

        public string GetSingleArgumentFromHtml(string currentTag)
        {
            string returnValue = "";
            int pos = currentTag.IndexOf(":");
            if (pos > 0)
            {
                returnValue = currentTag.Substring(pos + 1, currentTag.Length - pos - 2);
                //voor de zekerheid } erafhalen, voor het geval er spaties in tag zitten
                returnValue = returnValue.Replace("}", "");
            }

            return returnValue.Trim();

        }
        //public string GetNameWithoutFormat()
        //{
        //    string returnValue = Name;
        //    returnValue = returnValue.Replace("{", "");
        //    returnValue = returnValue.Replace("}", "");

        //    int pos = returnValue.IndexOf(":");
        //    if (pos > 0)
        //    {
        //        returnValue = returnValue.Substring(0, pos);
        //    }

        //    return returnValue.Trim();

        //}
    }
}
