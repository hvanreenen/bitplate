using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BitPlate.Domain.Licenses
{
    public class LicensePermissions
    {
        //[NonPersistent()]
        //public string PropertiesString { get; set; }
        [NonPersistent()]
        public int[] FunctionNumbers
        {
            get;
            private set;
        }

        [NonPersistent()]
        public int? MaxNumberOfSites { get; private set; }
        [NonPersistent()]
        public int? MaxNumberOfPages { get; private set; }
        [NonPersistent()]
        public int? MaxNumberOfPageFolders { get; private set; }
        [NonPersistent()]
        public int? MaxNumberOfTemplates { get; private set; }
        [NonPersistent()]
        public int? MaxNumberOfScripts { get; private set; }
        [NonPersistent()]
        public int? MaxNumberOfStylesheets { get; private set; }
        [NonPersistent()]
        public int? MaxNumberOfDataCollections { get; private set; }
        [NonPersistent()]
        public int? MaxNumberOfDataPerDataCollection { get; private set; }
        [NonPersistent()]
        public int? MaxNumberOfUsers { get; private set; }
        [NonPersistent()]
        public int? MaxNumberOfNewsletterTemplates { get; private set; }
        [NonPersistent()]
        public int? MaxNumberOfNewsletters { get; private set; }
        [NonPersistent()]
        public int? MaxNumberOfNewsletterMailings { get; private set; }
        [NonPersistent()]
        public int? MaxNumberOfNewsletterSubscribers { get; private set; }
        [NonPersistent()]
        public bool AllowMultipleLanguages { get; private set; }
        [NonPersistent()]
        public bool MultipleSites { get; private set; }
        [NonPersistent()]
        public DateTime? ValidUntill { get; internal set; }
        public override string ToString()
        {
            string returnValue = "";

            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                returnValue += propertyInfo.Name + "=" + propertyInfo.GetValue(this, null) + ";";
            }

            return returnValue;
        }

        public void FromString(string propertiesString)
        {

            Dictionary<string, object> properties = new Dictionary<string, object>();

            string[] propertyRows = propertiesString.Split(new char[] { ';' });
            foreach (string row in propertyRows)
            {
                if (row == "" || !row.Contains("=")) continue;
                string propName = row.Split(new char[] { '=' })[0];
                string propValue = row.Split(new char[] { '=' })[1];
                properties.Add(propName, propValue);
            }
            if (properties.Count > 0)
            {
                //this.IPAddress = properties["IPAddress"].ToString();
                //this.DomainName = properties["DomainName"].ToString();
                this.FunctionNumbers = Array.ConvertAll<string, int>(properties["FunctionNumbers"].ToString().Split(new char[] { ',' }), int.Parse);
                this.MaxNumberOfDataCollections = toNullableInt(properties["MaxNumberOfDataCollections"]);
                this.MaxNumberOfDataPerDataCollection = toNullableInt(properties["MaxNumberOfDataPerDataCollection"]);
                this.MaxNumberOfNewsletterSubscribers = toNullableInt(properties["MaxNumberOfNewsletterCustomers"]);
                this.MaxNumberOfNewsletterMailings = toNullableInt(properties["MaxNumberOfNewsletterMailings"]);
                this.MaxNumberOfNewsletters = toNullableInt(properties["MaxNumberOfNewsletters"]);
                this.MaxNumberOfNewsletterTemplates = toNullableInt(properties["MaxNumberOfNewsletterTemplates"]);
                this.MaxNumberOfPageFolders = toNullableInt(properties["MaxNumberOfPageFolders"]);
                this.MaxNumberOfPages = toNullableInt(properties["MaxNumberOfPages"]);
                this.MaxNumberOfScripts = toNullableInt(properties["MaxNumberOfScripts"]);
                this.MaxNumberOfSites = toNullableInt(properties["MaxNumberOfSites"]);
                this.MaxNumberOfStylesheets = toNullableInt(properties["MaxNumberOfStylesheets"]);
                this.MaxNumberOfTemplates = toNullableInt(properties["MaxNumberOfTemplates"]);
                this.MaxNumberOfUsers = toNullableInt(properties["MaxNumberOfUsers"]);
                this.ValidUntill = toNullableDateTime(properties.ContainsKey("ValidUntill") ? properties["ValidUntill"] : null);
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
