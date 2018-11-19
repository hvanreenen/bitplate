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


namespace BitPlate.Domain.Licenses
{
    
    /// <summary>
    /// Deze class is bedoeld voor het opslaan en wijzigen van een licentie.
    /// Hier zijn er namelijk public setters nodig, zodat json er mee overweg kan.
    /// In het echte license object zijn er private setters zodat de licentie niet is te manipuleren
    /// </summary>
    [DataConnection("licenseDB")]
    [Persistent("License")]
    public sealed class LicenseTransferObject : BaseDomainObject
    {
        [Persistent("IPAddress")]
        public string ServerName { get; set; }
        [Persistent("DomainName")]
        public string Urls { get; set; }
        public string Code { get; set; }
        [Persistent("LicenseFile")]
        public string PermissionsString { get; set; }
        [NonPersistent()]
        public string TempPermissionsString { get; set; }
        public LicenseTypeEnum LicenseType { get; set; }
        public NewsletterLicenseTypeEnum NewsletterLicenseType { get; set; }
        public WebshopLicenseTypeEnum WebshopLicenseType { get; set; }

        private Company _owner;
        [Association("FK_Company")]
        //[System.Xml.Serialization.XmlIgnore()]
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



        [NonPersistent()]
        public LicensePermissions Permissions { get; set; }

        private BaseCollection<CMSEnvironment> _environments;
        [Association(ForeignObjectName = "License")]
        public BaseCollection<CMSEnvironment> Environments
        {
            get
            {
                //if (_pages == null)
                //{
                if (_environments == null || (_environments != null && !_environments.IsLoaded))
                {
                    _environments = BaseCollection<CMSEnvironment>.Get("FK_License='" + this.ID + "'", "Name");
                    _environments.IsLoaded = true;
                }
                //}
                return _environments;
            }
            set
            {
                _environments = value;
            }
        }


        public static License New()
        {
            License returnValue = new License();
            returnValue.LicenseType = LicenseTypeEnum.BitplateStandard;
            returnValue.NewsletterLicenseType = NewsletterLicenseTypeEnum.BitnewsletterNone;
            returnValue.WebshopLicenseType = WebshopLicenseTypeEnum.BitshopNone;
            returnValue.Code = Utils.RandomPasswordGenerator.Generate(32);
            //returnValue.Permissions = new LicensePermissions();
            return returnValue;

        }


        public override void Save()
        {
            setName();
            setUrls();
            

            base.Save();
        }


        private void setName()
        {
            if (Owner != null)
            {
                this.Name = Owner.Name + ", ";
            }
            this.Name += LicenseType.ToString();

            if (NewsletterLicenseType != NewsletterLicenseTypeEnum.BitnewsletterNone)
            {
                this.Name += " + " + NewsletterLicenseType.ToString();
                if (this.Permissions.MaxNumberOfNewsletterMailings.HasValue && this.Permissions.MaxNumberOfNewsletterMailings.Value != 0)
                {
                    this.Name += "(" + this.Permissions.MaxNumberOfNewsletterMailings + ")";
                }
                else
                {
                    this.Name += "(onbeperkt)";
                }
            }
            if (WebshopLicenseType != WebshopLicenseTypeEnum.BitshopNone)
            {
                this.Name += " + " + WebshopLicenseType.ToString();
            }
        }

        private void setUrls()
        {
            Urls = "";
            BaseCollection<CMSEnvironment> envs = BaseCollection<CMSEnvironment>.Get("FK_License = '" + this.ID + "'");
            foreach (CMSEnvironment env in envs)
            {
                Urls += env.Url + ", ";
            }
            if (Urls.Length > 2)
            {
                Urls = Urls.Substring(0, Urls.Length - 2);
            }
        }



        private string encryptPermissionsString()
        {
            //if (Permissions == null) Permissions = new LicensePermissions();
            //string permissionsString = Permissions.ToString();
            string permissionsString = Utils.Encrypter.EncryptString(TempPermissionsString + ";END=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "12365");
            return permissionsString;
        }

        public void LoadPermissions()
        {
            string propertiesString = Utils.Encrypter.DecryptString(PermissionsString, "12365");
            if (Permissions == null) Permissions = new LicensePermissions();
            Permissions.FromString(propertiesString);
        }

        public void SaveAsFile()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(path + "_lic\\"))
            {
                Directory.CreateDirectory(path + "_lic\\");
            }
            TextWriter writer = File.CreateText(path + "_lic\\" + Code + ".lic");
            writer.Write(this.PermissionsString);
            writer.Close();
        }


        public static List<int> LoadDefaultFunctionNumbersByType(int licenceType)
        {
            List<int> returnValue = new List<int>();
            string name = "";
            if (licenceType < 10)
            {
                name = Enum.GetName(typeof(LicenseTypeEnum), licenceType);
            }
            else if (licenceType < 100)
            {
                name = Enum.GetName(typeof(NewsletterLicenseTypeEnum), licenceType);
            }
            else
            {
                name = Enum.GetName(typeof(WebshopLicenseTypeEnum), licenceType);
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
                name = Enum.GetName(typeof(LicenseTypeEnum), licenceType);
            }
            else if (licenceType < 100)
            {
                name = Enum.GetName(typeof(NewsletterLicenseTypeEnum), licenceType);
            }
            else
            {
                name = Enum.GetName(typeof(WebshopLicenseTypeEnum), licenceType);
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
            this.PermissionsString = dataRow["LicenseFile"].ToString();
            this.Code = dataRow["Code"].ToString();
            this.ServerName = dataRow["IPAddress"].ToString();
            this.Urls = dataRow["DomainName"].ToString();
            this.LicenseType = (LicenseTypeEnum)DataConverter.ToInt32(dataRow["LicenseType"]);
            this.NewsletterLicenseType = (NewsletterLicenseTypeEnum)DataConverter.ToInt32(dataRow["NewsletterLicenseType"]);
            this.WebshopLicenseType = (WebshopLicenseTypeEnum)DataConverter.ToInt32(dataRow["WebshopLicenseType"]);

            if (dataRow["FK_Company"] != DBNull.Value)
            {
                this.Owner = new Company();
                this.Owner.ID = DataConverter.ToGuid(dataRow["FK_Company"]);
            }
            LoadPermissions();
            this.IsLoaded = true;
        }
        [NonPersistent()]
        public bool AllowMultipleLanguages
        {
            get
            {
                if (Permissions != null)
                {
                    return Permissions.AllowMultipleLanguages;
                }
                else
                {
                    return false;
                }
            }
        }

       
       

        public string RetrieveStatistics()
        {
            string html = "";
            BaseCollection<CMSEnvironment> environments = BaseCollection<CMSEnvironment>.Get("FK_License = '" + this.ID + "'");
            foreach (CMSEnvironment env in environments)
            {
                html += "Statistieken voor " + env.Name + "<br/>";
                html += env.GetStatisticsHtmlTable();
                html += "<br/><br/>";
            }
            return html;
        }
    }
}
