using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace BitPlate.Domain.Licenses
{
    /// <summary>
    /// dit is een vertalingsobject van de licentie. Dit object wordt via de webservice over het lijntje gestuurd.
    /// Dit is een sealed object zodat er niet van overgeerfd kan worden een waardes van props aangepast zouden kunnen worden
    /// Properties die rechten bevatten zijn readonly
    /// </summary>
    public sealed class LicenseFile
    {
        public LicenseFile()
        {
            //IsValid = true;
            //Owner = new Company();
        }
        public string Name { get; set; }
        /// <summary>
        /// Welke versie van bitplate--> komt uite de LicensedEnvironment
        /// </summary>
        public string BitplateVersion { get; set; }

        public bool IsValid { get; private set; }

        public string EncryptedContent { get; set; }

        private string _decryptedContent;

        public string Code { get; set; }

        public string ResellerInfo { get; set; }
        public string ResellerLogoSrc { get; set; }
        public string PasswordEmailTemplate { get; set; }
        public string PasswordEmailFrom { get; set; }
        public string PasswordEmailSubject { get; set; }
        //public Company Owner { get; set; }
        //internal string DynamicKey { get; set; }

        internal DateTime? ValidUntill { get; set; }

        private int[] _functionNumbers; // { get; set; }

        public int[] FunctionNumbers
        {
            get { return _functionNumbers; }
        }
        //private Dictionary<string, object> _permissions = new Dictionary<string, object>();

        private int? _maxNumberOfSites;
        public int? MaxNumberOfSites
        {
            get { return _maxNumberOfSites; }
        }

        private int? _maxNumberOfPages;
        public int? MaxNumberOfPages
        {
            get { return _maxNumberOfPages; }
        }

        private int? _maxNumberOfPageFolders;
        public int? MaxNumberOfPageFolders
        {
            get { return _maxNumberOfPageFolders; }
        }

        private int? _maxNumberOfTemplates;
        public int? MaxNumberOfTemplates
        {
            get { return _maxNumberOfTemplates; }
        }

        private int? _maxNumberOfScripts;
        public int? MaxNumberOfScripts
        {
            get { return _maxNumberOfScripts; }
        }

        private int? _maxNumberOfStylesheets;
        public int? MaxNumberOfStylesheets
        {
            get { return _maxNumberOfStylesheets; }
        }

        private int? _maxNumberOfDataCollections;
        public int? MaxNumberOfDataCollections
        {
            get { return _maxNumberOfDataCollections; }
        }

        private int? _maxNumberOfDataPerDataCollection;
        public int? MaxNumberOfDataPerDataCollection
        {
            get { return _maxNumberOfDataPerDataCollection; }
        }

        private int? _maxNumberOfUsers;
        public int? MaxNumberOfUsers
        {
            get { return _maxNumberOfUsers; }
        }

        private int? _maxNumberOfNewsletterTemplates;
        public int? MaxNumberOfNewsletterTemplates
        {
            get { return _maxNumberOfNewsletterTemplates; }
        }

        private int? _maxNumberOfNewsletters;
        public int? MaxNumberOfNewsletters
        {
            get { return _maxNumberOfNewsletters; }
        }

        private int? _maxNumberOfNewsletterMailings;
        public int? MaxNumberOfNewsletterMailings
        {
            get { return _maxNumberOfNewsletterMailings; }
        }

        private int? _maxNumberOfNewsletterSubscribers;
        public int? MaxNumberOfNewsletterSubscribers
        {
            get { return _maxNumberOfNewsletterSubscribers; }
        }

        private bool _allowMultipleLanguages;
        public bool AllowMultipleLanguages
        {
            get { return _allowMultipleLanguages; }
        }

        private bool _multipleSites;

        public bool HasPermission(FunctionalityEnum func)
        {
            if (!IsValid)
            {
                return false;
            }
            
            bool returnvalue = false;

            foreach (FunctionalityEnum permission in _functionNumbers)
            {
                if (permission == func)
                {
                    returnvalue = true;
                    break;
                }
            }
            return returnvalue;
        }

        internal bool HasMaxNumberExceeded(string name)
        {
            string table = name;
            string sql = "select count(*) from " + table + " where fk_site='" + Utils.WebSessionHelper.CurrentSite.ID + "'";
            object result = HJORM.DataBase.Get().Execute(sql);
            int count = Convert.ToInt32(result);
            int? max = getMaxNumberByName(name);
            bool returnValue = false;
            if (max.HasValue && max.Value <= count)
            {
                returnValue = true;
            }
            return returnValue;
            
        }

        private int? getMaxNumberByName(string name)
        {
            if (name.ToLower() == "page")
            {
                return _maxNumberOfPages;
            }
            else if (name.ToLower() == "pagefolder")
            {
                return _maxNumberOfPageFolders;
            }
            else if (name.ToLower() == "template")
            {
                return _maxNumberOfTemplates;
            }
            else if (name.ToLower() == "script")
            {
                return _maxNumberOfScripts;
            }
            else if (name.ToLower() == "stylesheet")
            {
                return _maxNumberOfStylesheets;
            }
            else if (name.ToLower() == "datacollection")
            {
                return _maxNumberOfDataCollections;
            }
            else if (name.ToLower() == "newslettermailing")
            {
                return _maxNumberOfNewsletterMailings;
            }
            else if (name.ToLower() == "newslettersubscriber")
            {
                return _maxNumberOfNewsletterSubscribers;
            }
            else if (name.ToLower() == "siteuser")
            {
                return _maxNumberOfUsers;
            }
            else
            {
                return null;
            }
        }

        public void SaveAsFile()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(path + "_lic\\"))
            {
                Directory.CreateDirectory(path + "_lic\\");
            }
            TextWriter writer = File.CreateText(path + "_lic\\" + Code + ".lic");
            writer.Write(this.EncryptedContent);
            writer.Close();
            writer.Dispose();
        }



        public void LoadFromFile(string code)
        {
            string serverName = Environment.MachineName;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string relativeUrl = HttpContext.Current.Request.Url.PathAndQuery;
            string completeUrl = HttpContext.Current.Request.Url.ToString();
            string domainName = completeUrl.Replace(relativeUrl, "");
            if (!File.Exists(path + "_lic\\" + code + ".lic"))
            {
                IsValid = false;
                this.Name = "Licentie ongeldig.";
                return;
            }


            this.EncryptedContent = File.ReadAllText(path + "_lic\\" + code + ".lic");

            this.Code = code;
            this.Name = "Tijdelijke licentie 24 uur geldig (service uit de lucht)";

            string key = serverName + path + domainName;

            this.Decrypt();

            IsValid = true;
            if (!this.ValidUntill.HasValue)
            {
                //bij eerste keer openen Valid Until zetten en file opnieuw opslaan
                _decryptedContent = _decryptedContent.Replace("END==>", "ValidUntill==>" + DateTime.Now.AddHours(24).ToString() + ";;END==>");

                this.encrypt();
                this.SaveAsFile();
            }
            else
            {
                //bij tweede keer openen: checken of 24 uur zijn verstreken
                if (this.ValidUntill < DateTime.Now)
                {
                    this.Name = "Tijdelijke licentie is verlopen.";
                    this.EncryptedContent = "";
                    IsValid = false;
                }
                else
                {
                    this.Name = "Tijdelijke licentie geldig tot " + ValidUntill.Value.ToString("dd-MM-yyyy HH:mm") + " (service uit de lucht).";
                }
            }

        }

        private void encrypt()
        {
            string key = getKey();
            EncryptedContent = Utils.Encrypter.EncryptString(_decryptedContent, key);
        }

        public void Decrypt()
        {
            try
            {
                string key = getKey();
                _decryptedContent = Utils.Encrypter.DecryptString(EncryptedContent, key);
                fromString(_decryptedContent);
                IsValid = true;
            }
            catch (Exception ex)
            {
                IsValid = false;
            }


        }

        private string getKey()
        {

            string serverName = Environment.MachineName;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            //string ipAddress = HttpContext.Current.Request.ServerVariables["remote_host"];
            string relativeUrl = HttpContext.Current.Request.Url.PathAndQuery;
            string completeUrl = HttpContext.Current.Request.Url.ToString();
            string domainName = completeUrl.Replace(relativeUrl, "");

            string key = serverName + path + domainName + Code;
            return key;
        }


        private void fromString(string propertiesString)
        {

            Dictionary<string, object> properties = new Dictionary<string, object>();

            string[] propertyRows = Regex.Split(propertiesString, ";;");
            foreach (string row in propertyRows)
            {
                if (row == "" || !row.Contains("==>")) continue;
                string propName = Regex.Split(row, "==>")[0];
                string propValue = Regex.Split(row, "==>")[1];
                properties.Add(propName, propValue);
            }
            if (properties.Count > 0)
            {
                this._functionNumbers = Array.ConvertAll<string, int>(properties["Permissions"].ToString().Split(new char[] { ',' }), int.Parse);
                this._maxNumberOfDataCollections = toNullableInt(properties["MaxNumberOfDataCollections"]);
                this._maxNumberOfDataPerDataCollection = toNullableInt(properties["MaxNumberOfDataPerDataCollection"]);
                this._maxNumberOfNewsletterSubscribers = toNullableInt(properties["MaxNumberOfNewsletterSubscribers"]);
                this._maxNumberOfNewsletterMailings = toNullableInt(properties["MaxNumberOfNewsletterMailings"]);
                this._maxNumberOfNewsletters = toNullableInt(properties["MaxNumberOfNewsletters"]);
                this._maxNumberOfNewsletterTemplates = toNullableInt(properties["MaxNumberOfNewsletterTemplates"]);
                this._maxNumberOfPageFolders = toNullableInt(properties["MaxNumberOfPageFolders"]);
                this._maxNumberOfPages = toNullableInt(properties["MaxNumberOfPages"]);
                this._maxNumberOfScripts = toNullableInt(properties["MaxNumberOfScripts"]);
                this._maxNumberOfSites = toNullableInt(properties["MaxNumberOfSites"]);
                this._maxNumberOfStylesheets = toNullableInt(properties["MaxNumberOfStylesheets"]);
                this._maxNumberOfTemplates = toNullableInt(properties["MaxNumberOfTemplates"]);
                this._maxNumberOfUsers = toNullableInt(properties["MaxNumberOfUsers"]);
                this._allowMultipleLanguages = Convert.ToBoolean(properties["AllowMultipleLanguages"]);
                this.ValidUntill = toNullableDateTime(properties.ContainsKey("ValidUntill") ? properties["ValidUntill"] : null);
                this.ResellerInfo = properties.ContainsKey("ResellerInfo") ? properties["ResellerInfo"].ToString() : "";
                this.ResellerLogoSrc = properties.ContainsKey("ResellerLogoSrc") ? properties["ResellerLogoSrc"].ToString() : "";
                this.PasswordEmailFrom = properties.ContainsKey("PasswordEmailFrom") ? properties["PasswordEmailFrom"].ToString() : "";
                this.PasswordEmailSubject = properties.ContainsKey("PasswordEmailSubject") ? properties["PasswordEmailSubject"].ToString() : "";
                this.PasswordEmailTemplate = properties.ContainsKey("PasswordEmailTemplate") ? properties["PasswordEmailTemplate"].ToString() : "";
            
            }
        }

        private int? toNullableInt(object value)
        {
            if (value != null && value.ToString() != "" && value.ToString() != "null")
            {
                return Convert.ToInt32(value);
            }
            else
            {
                return null;
            }
        }
        private DateTime? toNullableDateTime(object value)
        {
            if (value != null && value.ToString() != "" && value.ToString() != "null")
            {
                return Convert.ToDateTime(value);
            }
            else
            {
                return null;
            }
        }
    }
}
