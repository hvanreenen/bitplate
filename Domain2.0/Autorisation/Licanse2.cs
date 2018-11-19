using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;

using HJORM;
using HJORM.Attributes;
using System.Reflection;


namespace BitPlate.Domain.Autorisation
{



    [DataConnection("licenseDB")]

    public class License2 : BaseDomainObject
    {
        //private string _ipAddress = "";
        //public string IPAddress
        //{
        //    get
        //    {
        //        return _ipAddress;
        //    }
        //    set
        //    {

        //    }
        //}
        //private string _domainName = "";
        //public string DomainName {
        //    get
        //    {
        //        return _domainName;
        //    }
        //    set
        //    {

        //    }
        //}
        //public string SecretKey { get; set; }

        public string IPAddress { get; set; }
        public string DomainName { get; set; }
        [NonPersistent()]
        public string DynamicKey { get; set; }

        public string Code { get; set; }
        public LicenseTypeEnum LicenseType { get; set; }
        public NewsletterLicenseTypeEnum NewsletterLicenseType { get; set; }
        public WebshopLicenseTypeEnum WebshopLicenseType { get; set; }
        [NonPersistent()]
        public string TempPropertyList { get; set; }
        public string LicenseFile { get; set; }

        public int MaxNumberOfSites { get; private set; }
        public int MaxNumberOfPages { get; private set; }
        public int MaxNumberOfPageFolders { get; private set; }
        public int MaxNumberOfTemplates { get; private set; }
        public int MaxNumberOfScripts { get; private set; }
        public int MaxNumberOfStylesheets { get; private set; }
        public int MaxNumberOfDataCollections { get; private set; }
        public int MaxNumberOfDataPerDataCollection { get; private set; }
        public int MaxNumberOfUsers { get; private set; }

        public int MaxNumberOfNewsletterTemplates { get; private set; }
        public int MaxNumberOfNewsletters { get; private set; }
        public int MaxNumberOfNewsletterMailings { get; private set; }
        public int MaxNumberOfNewsletterCustomers { get; private set; }

        [NonPersistent()]
        private bool _allowMultipleLanguages = true;

        [NonPersistent()]
        public bool AllowMultipleLanguages
        {
            get { return this._allowMultipleLanguages; }
            set { this._allowMultipleLanguages = value; }
        }

        public bool MultipleSites { get; private set; }
        private Company _owner;
        [Association("FK_Company")]
        [System.Xml.Serialization.XmlIgnore()]
        public Company Owner
        {
            get
            {
                if (_owner != null && !_owner.IsLoaded)
                {
                    _owner.Load();
                }
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }

        public SuperUserTypeEnum SuperUserlicenseType { get; set; }

        //private BaseCollection<Functionality> _permissions;
        //[Association("FK_License", "FK_Functionality ")]
        //[Persistent("LicensePermission")]
        //public BaseCollection<Functionality> Permissions
        //{
        //    get
        //    {
        //        if (_permissions == null)
        //        {
        //            string where = "EXISTS (SELECT * FROM LicensePermission WHERE FK_License = '" + this.ID + "' AND LicensePermission.FK_Functionality = Functionality.ID)";
        //            _permissions = BaseCollection<Functionality>.Get(where, "Name");
        //        }
        //        return _permissions;
        //    }
        //    set
        //    {
        //        _permissions = value;
        //    }
        //}

        //public FunctionalityEnum[] Permissions { get; private set; }
        public string Permissions
        {
            get;
            private set;
        }

        public static License New()
        {
            License returnValue = new License();
            //returnValue.LicenseType = LicenceTypeEnum.BitplateLite;
            returnValue.NewsletterLicenseType = NewsletterLicenceTypeEnum.BitnewsletterNone;
            returnValue.WebshopLicenseType = WebshopLicenceTypeEnum.BitshopNone;
            returnValue.Code = Utils.RandomPasswordGenerator.Generate(32);
            return returnValue;

        }


        public override void Save()
        {
            setName();

            this.LicenseFile = encryptLicenceFileContent();
            base.Save();
            SaveFile();

        }


        private void setName()
        {
            if (Owner != null)
            {
                this.Name = Owner.Name + ", ";
            }
            this.Name += LicenseType.ToString();

            if (NewsletterLicenseType != NewsletterLicenceTypeEnum.BitnewsletterNone)
            {
                this.Name += " + " + NewsletterLicenseType.ToString();
                if (this.MaxNumberOfNewsletterMailings != 0)
                {
                    this.Name += "(" + this.MaxNumberOfNewsletterMailings + ")";
                }
                else
                {
                    this.Name += "(onbeperkt)";
                }
            }
            if (WebshopLicenseType != WebshopLicenceTypeEnum.BitshopNone)
            {
                this.Name += " + " + WebshopLicenseType.ToString();
            }
        }

        public void Encrypt()
        {
            LicenseFile = Utils.Encrypter.EncryptString(LicenseFile, DynamicKey);
        }

        public void Decrypt()
        {
            LicenseFile = Utils.Encrypter.DecryptString(LicenseFile, DynamicKey);
            this.Load();
        }

        private string encryptLicenceFileContent()
        {
            string file = TempPropertyList;
            //file = Utils.JSONSerializer.Serialize(this);
            if (file == "" || file == null)
            {
                foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
                {
                    file += propertyInfo.Name + "=" + propertyInfo.GetValue(this, null) + "\r\n";
                }
            }
            file = Utils.Encrypter.EncryptString(file, "12365");
            return file;

            //XmlSerializer serializer = new XmlSerializer(typeof(License));

            //MemoryStream stream = new MemoryStream();
            //serializer.Serialize(stream, this);

            //stream.Position = 0;
            //StreamReader reader = new StreamReader(stream);
            //return reader.ReadToEnd();
        }

        public void SaveFile()
        {
            if (LicenseFile == "" || LicenseFile == null)
            {
                LicenseFile = encryptLicenceFileContent();
            }
            if (Name == "" || Name == null)
            {
                setName();
            }
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(path + "_lic\\"))
            {
                Directory.CreateDirectory(path + "_lic\\");
            }
            TextWriter writer = File.CreateText(path + "_lic\\" + Name + ".lic");
            writer.Write(this.LicenseFile);
            writer.Close();
        }



        public void Load()
        {
            string file = Utils.Encrypter.DecryptString(LicenseFile, "12365");


            Dictionary<string, object> properties = new Dictionary<string, object>();

            string[] fileRules = file.Split(new char[] { ';' });
            foreach (string rule in fileRules)
            {
                if (rule == "" || !rule.Contains("=")) continue;
                string propName = rule.Split(new char[] { '=' })[0];
                string propValue = rule.Split(new char[] { '=' })[1];
                PropertyInfo prop = this.GetType().GetProperty(propName);
                if (propName != "ID")
                {
                    properties.Add(propName, propValue);
                }
            }
            if (properties.Count > 0)
            {
                //this.IPAddress = properties["IPAddress"].ToString();
                //this.DomainName = properties["DomainName"].ToString();
                this.Permissions = properties["Permissions"].ToString();
                this.MaxNumberOfDataCollections = Convert.ToInt32(properties["MaxNumberOfDataCollections"]);
                this.MaxNumberOfDataPerDataCollection = Convert.ToInt32(properties["MaxNumberOfDataPerDataCollection"]);
                this.MaxNumberOfNewsletterCustomers = Convert.ToInt32(properties["MaxNumberOfNewsletterCustomers"]);
                this.MaxNumberOfNewsletterMailings = Convert.ToInt32(properties["MaxNumberOfNewsletterMailings"]);
                this.MaxNumberOfNewsletters = Convert.ToInt32(properties["MaxNumberOfNewsletters"]);
                this.MaxNumberOfNewsletterTemplates = Convert.ToInt32(properties["MaxNumberOfNewsletterTemplates"]);
                this.MaxNumberOfPageFolders = Convert.ToInt32(properties["MaxNumberOfPageFolders"]);
                this.MaxNumberOfPages = Convert.ToInt32(properties["MaxNumberOfPages"]);
                this.MaxNumberOfScripts = Convert.ToInt32(properties["MaxNumberOfScripts"]);
                this.MaxNumberOfSites = Convert.ToInt32(properties["MaxNumberOfSites"]);
                this.MaxNumberOfStylesheets = Convert.ToInt32(properties["MaxNumberOfStylesheets"]);
                this.MaxNumberOfTemplates = Convert.ToInt32(properties["MaxNumberOfTemplates"]);
                this.MaxNumberOfUsers = Convert.ToInt32(properties["MaxNumberOfUsers"]);
            }
        }

        //private List<int> _allPermissions;
        public bool HasPermission(FunctionalityEnum func)
        {
            bool returnvalue = false;
            //if (_allPermissions == null)
            //{
            //    fillAllPermissions();
            //}
            foreach (FunctionalityEnum permission in Permissions)
            {
                if (permission == func)
                {
                    returnvalue = true;
                    break;
                }
            }
            return returnvalue;
        }

        //private void fillAllPermissions()
        //{
        //    _allPermissions = new List<int>();
        //    foreach (Functionality func in this.Permissions)
        //    {
        //        _allPermissions.Add((int)func.FunctionNumber);
        //    }
        //}

        public static List<int> LoadDefaultFunctionNumbersByType(int licenceType)
        {
            List<int> returnValue = new List<int>();
            string name = "";
            if (licenceType < 10)
            {
                name = Enum.GetName(typeof(LicenceTypeEnum), licenceType);
            }
            else if (licenceType < 100)
            {
                name = Enum.GetName(typeof(NewsletterLicenceTypeEnum), licenceType);
            }
            else
            {
                name = Enum.GetName(typeof(WebshopLicenceTypeEnum), licenceType);
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + "_licenseTypeDefaults\\" + name + ".lic";
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (line.Trim() != "")
                    {
                        string[] fields = line.Split(new char[] { ',' });
                        if (Convert.ToInt32(fields[0].Trim()) >= 1000)
                        {
                            //if(fields[2].Trim() == "1"){
                            returnValue.Add(Convert.ToInt32(fields[0]));
                            //}
                        }
                    }
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Haal max. amounts op voor licentie
        /// Staat in template per licentie
        /// bijv. Max. aantal pagina's = 100
        /// </summary>
        /// <param name="licenceType"></param>
        /// <returns></returns>
        public static List<string> LoadDefaultValuesByType(int licenceType)
        {
            List<string> returnValue = new List<string>();
            string name = "";
            if (licenceType < 10)
            {
                name = Enum.GetName(typeof(LicenceTypeEnum), licenceType);
            }
            else if (licenceType < 100)
            {
                name = Enum.GetName(typeof(NewsletterLicenceTypeEnum), licenceType);
            }
            else
            {
                name = Enum.GetName(typeof(WebshopLicenceTypeEnum), licenceType);
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + "_licenseTypeDefaults\\" + name + ".lic";
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                //regel heeft structuur:
                //0,MaxNumberOfSites:1
                //als die begint met 0 dan is het een defaultvalue

                foreach (string line in lines)
                {
                    if (line.Trim() != "")
                    {
                        string[] fields = line.Split(new char[] { ',' });
                        if (Convert.ToInt32(fields[0].Trim()) == 0)
                        {
                            //if(fields[2].Trim() == "1"){
                            returnValue.Add(fields[1]);
                            //}
                        }
                    }
                }
            }
            return returnValue;
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            this.LicenseFile = dataRow["LicenseFile"].ToString();
            this.Code = dataRow["Code"].ToString();
            this.IPAddress = dataRow["IPAddress"].ToString();
            this.DomainName = dataRow["DomainName"].ToString();
            if (dataRow["FK_Company"] != DBNull.Value)
            {
                this.Owner = new Company();
                this.Owner.ID = DataConverter.ToGuid(dataRow["FK_Company"]);
            }
            this.IsLoaded = true;
        }
    }
}
