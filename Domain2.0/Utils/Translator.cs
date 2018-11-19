using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;

namespace BitPlate.Domain.Utils
{
    public class Translator
    {
        private bool debug = false;
        public string Language;// { get; set; }
        public string Area;// { get; set; }
        public Dictionary<string, string> TranslationsTable;// { get; set; }
        public string FileName { get; set; }
        public static Dictionary<string, Dictionary<string, string>> EnumValuesByLanguage { get; set; }

        #region StaticValues
        //region voor vertalen van enums
        /// <summary>
        /// Init wordt aangeroepen tijdens global.ajax
        /// In deze init wordt per taal een tabel gevuld met daarin weer een tabel.
        /// In de tweede tabel staan de enumwaarden per regel als 
        /// [typenaam]_[enumnaam] : "beschrijving"
        /// Bijvoorbeeld: FieldTypeEnum_Text: "Tekst"
        /// </summary>
        public static void InitStaticValues()
        {
            if (EnumValuesByLanguage == null)
            {
                EnumValuesByLanguage = new Dictionary<string, Dictionary<string, string>>();
            }
            string[] languages = {"NL", "EN", "FR", "DU"}; 
            //voor alle talen
            foreach (string language in languages)
            {
                Dictionary<string, string> enumValues = readStaticValuesByLanguage(language);
                if (enumValues != null)
                {
                    EnumValuesByLanguage.Add(language, enumValues);
                }
            }
        }

        private static Dictionary<string, string> readStaticValuesByLanguage(string language)
        {
            Dictionary<string, string> enumValues = null;
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\_bitplate\\_bitSystem\\Translations\\" + language + "\\StaticValues.translation.txt";
            if (File.Exists(fileName))
            {
                enumValues = new Dictionary<string, string>();
                string[] lines = File.ReadAllLines(fileName);
                foreach (string line in lines)
                {
                    string[] nameValuePair = line.Split(new char[] { ':' });
                    string name = nameValuePair[0].Replace("\"", "").Trim();
                    string value = nameValuePair[1].Replace("\"", "").Trim();
                    enumValues.Add(name, value);
                }
            }
            return enumValues;
        }

        internal static string Translate(string langCode, string enumType, string enumName)
        {
            string returnValue = enumName;
            if (EnumValuesByLanguage != null)
            {

                if (EnumValuesByLanguage.ContainsKey(langCode))
                {
                    Dictionary<string, string> enumvalues = EnumValuesByLanguage[langCode];
                    string key = enumType + "_" + enumName;
                    if (enumvalues.ContainsKey(key))
                    {
                        returnValue = enumvalues[key];
                    }
                }
                else
                {
#if DEBUG
                    string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\_bitplate\\_bitSystem\\Translations\\" + langCode + "\\StaticValues.translation.txt";

#endif
                }
            }

            return returnValue;
        }
        #endregion
        public Translator(string PageName, string Language)
        {
            this.Language = Language;
            this.Area = PageName;
            this.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\_bitplate\\_bitSystem\\Translations\\" + Language + "\\" + Area + ".translation.txt";

            TranslationsTable = LoadTranslationsFile();
        }

        public string Translate(string OriginalString)
        {
            string returnValue = "";
            if (TranslationsTable != null && TranslationsTable.ContainsKey(OriginalString))
            {
                returnValue = TranslationsTable[OriginalString];
            }
            else
            {
                returnValue = OriginalString;
                if (Language != "NL")
                {
#if DEBUG
                    if (!TranslationsTable.ContainsKey(OriginalString))
                    {
                        this.appendToFile(OriginalString);
                    }
#endif
                }
            }
            return returnValue;
        }


        public Dictionary<string, string> LoadTranslationsFile()
        {
            Dictionary<string, string> returnValue = new Dictionary<string, string>();


            if (File.Exists(FileName))
            {
                string jsonFormat = CreateJsonFormatFromFile(FileName);
                returnValue = JSONSerializer.Deserialize<Dictionary<string, string>>(jsonFormat);
                //string[] lines = File.ReadAllLines(path);
                //foreach (string line in lines)
                //{
                //    string[] values = line.Split(new char[] { ':' });
                //    string original = values[0];
                //    string translation = values[1];
                //    returnValue.Add(original, translation);
                //}
            }

            return returnValue;
        }
        public string CreateJsonFormat(string area)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\_bitplate\\_bitSystem\\Translations\\" + Language + "\\" + area + ".translation.txt";
            return CreateJsonFormatFromFile(fileName);
        }
        //maakt van een tekst file met vertalingen een json object
        //in de tekst file staan regels als "te vertalen" : "vertaling". Per regel 1 vertaling
        //hier wordt komma's achter gezet en {-des
        private string CreateJsonFormatFromFile(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            string[] lines = File.ReadAllLines(fileName);
            if (lines.Length > 0)
            {
                sb.Append("{");

                foreach (string line in lines)
                {
                    if (line.Trim() != String.Empty)
                    {
                        sb.Append(line);
                        sb.Append(",");
                    }
                }
                //haal laatste komma weg
                sb.Remove(sb.Length - 1, 1);
                sb.Append("}");
            }
            string json = sb.ToString();
            return json;
        }

        private void appendToFile(string OrginalString)
        {
            TranslationsTable.Add(OrginalString, "");
            if (!File.Exists(FileName))
            {
                TextWriter wr = File.CreateText(FileName);
                wr.Close();
            }
            //string path = HttpContext.Current.Server.MapPath("") + "\\_bitSystem\\Translations\\" + Language + "\\" + Area + ".translation.txt";
            TextWriter writer = File.AppendText(FileName);
            String line = String.Format("\"{0}\" : \"{1}\"", OrginalString, "");
            writer.WriteLine(line);
            writer.Close();
        }

        
    }

}