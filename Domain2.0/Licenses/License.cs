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
    [Flags]
    public enum LicenseTypeEnum
    {
        [EnumMember]
        BitplateLite = 1,
        [EnumMember]
        BitplateStandard = 2,
        [EnumMember]
        BitplateCorporate = 3,
        [EnumMember]
        BitplateEnterprise = 4,
        [EnumMember]
        BitplateCustom = 9
    }
    [Flags]
    public enum NewsletterLicenseTypeEnum
    {
        [EnumMember]
        BitnewsletterNone = 0,
        [EnumMember]
        BitnewsletterLite = 10,
        [EnumMember]
        BitnewsletterCorporate = 11,
        [EnumMember]
        BitnewsletterCustom = 12
    }
    [Flags]
    public enum WebshopLicenseTypeEnum
    {
        [EnumMember]
        BitshopNone = 0,
        [EnumMember]
        BitshopLite = 101,
        [EnumMember]
        BitshopCorporate = 102,
        [EnumMember]
        BitshopXtra = 103,
        [EnumMember]
        BitshopCustom = 109
    }

    /// <summary>
    /// Object wordt opgeslagen in MetaServer
    /// Aan een Licentie hangen 1 of meerdere omgevingen (LicensedEnvironment)
    /// Van een Licentie wordt een LicenseFile gemaakt. Dit is een ander object dat zelf geen dataopslag heeft. 
    /// In de licentiefile staan alleen de noodzakelijke (readonly) eigenschappen.
    /// LicentieFile wordt versleuteld verstuurd. Om het te onsleutelen heb je de lic-code nodig, de servernaam en ook de domeinname
    /// Een licentie heeft rechten.
    /// Gebruikers mogen nooit meer rechten hebben als de licentie
    /// 
    /// </summary>
    [DataConnection("licenseDB")]
    public sealed class License : BaseDomainObject
    {

        public string ServerName { get; set; }
        public string DomainNames { get; set; }
        public string Code { get; set; }

        public LicenseTypeEnum LicenseType { get; set; }
        public NewsletterLicenseTypeEnum NewsletterLicenseType { get; set; }
        public WebshopLicenseTypeEnum WebshopLicenseType { get; set; }

        private Company _owner;
        /// <summary>
        /// Klant
        /// </summary>
        [Association("FK_Company")]
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

       

        /// <summary>
        /// int-array van Permissions. zie FunctionalityEnum
        /// </summary>
        public int[] Permissions { get; set; }

        /// <summary>
        /// wordt niet gebruikt
        /// </summary>
        public int? MaxNumberOfSites { get; set; }

        public int? MaxNumberOfPages { get; set; }

        public int? MaxNumberOfPageFolders { get; set; }

        public int? MaxNumberOfTemplates { get; set; }

        public int? MaxNumberOfScripts { get; set; }

        public int? MaxNumberOfStylesheets { get; set; }

        public int? MaxNumberOfDataCollections { get; set; }

        public int? MaxNumberOfDataPerDataCollection { get; set; }

        public int? MaxNumberOfUsers { get; set; }

        public int? MaxNumberOfNewsletterTemplates { get; set; }

        public int? MaxNumberOfNewsletters { get; set; }

        public int? MaxNumberOfNewsletterMailings { get; set; }

        public int? MaxNumberOfNewsletterSubscribers { get; set; }

        public bool AllowMultipleLanguages
        {
            get
            {
                bool returnvalue = false;
                if (Permissions != null)
                {
                    returnvalue =(Array.IndexOf(Permissions, (int)FunctionalityEnum.MultipleLanguages) >= 0);
                }
                return returnvalue;
            }
        }

        public bool MultipleSites { get; set; }

        private BaseCollection<LicensedEnvironment> _environments;
        [Association(ForeignObjectName = "License")]
        public BaseCollection<LicensedEnvironment> Environments
        {
            get
            {

                if (_environments == null || (_environments != null && !_environments.IsLoaded))
                {
                    _environments = BaseCollection<LicensedEnvironment>.Get("FK_License='" + this.ID + "'", "Name");
                    _environments.IsLoaded = true;
                }
                return _environments;
            }
            set
            {
                _environments = value;
                _environments.IsLoaded = true;
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
            this.ServerName = ServerName.ToUpper();
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
                if (this.MaxNumberOfNewsletterMailings.HasValue && this.MaxNumberOfNewsletterMailings.Value != 0)
                {
                    this.Name += "(" + this.MaxNumberOfNewsletterMailings + ")";
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
        /// <summary>
        /// Bij opslaan worden urls in database gezet. Dit maakt zoeken en sorteren makkelijker
        /// </summary>
        private void setUrls()
        {
            DomainNames = "";
            foreach (LicensedEnvironment env in this.Environments)
            {
                DomainNames += env.DomainName + ", ";
            }
            if (DomainNames.Length > 2)
            {
                DomainNames = DomainNames.Substring(0, DomainNames.Length - 2);
            }
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
        /// Staat in template per licentieType (map _licenseTypeDefaults op server)
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

        /// <summary>
        /// Aanmaken van LicenseFile-object.
        /// Dit is een sealed object zonder dataopslag en met alleen readonly properties
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="path"></param>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public LicenseFile CreateLicenseFile(string serverName, string path, string domainName)
        {
            LicenseFile licenseFile = null;
            
            //eerst controleren of path en domeinname geldig is voor deze licentie
            string versionNumber = "";
            if (validateLicenseRequest(serverName, path, domainName, out versionNumber))
            {
                licenseFile = new LicenseFile();
                string key = serverName + path + domainName + Code;

                licenseFile.Name = this.Name;
                licenseFile.Code = this.Code;
                licenseFile.BitplateVersion = versionNumber;

                string propertiesString = createPropertiesString();
                licenseFile.EncryptedContent = Utils.Encrypter.EncryptString(propertiesString, key);
                
            }
            return licenseFile;
        }
        /// <summary>
        /// controleer of aanvraag voor licentieFile goed is.
        /// Licentie moet bestaan met juiste code en ServerName
        /// Licentie moet een omgeving hebben met de juiste pad en domeinname
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="path"></param>
        /// <param name="domainName"></param>
        /// <param name="versionNumber"></param>
        /// <returns></returns>
        private bool validateLicenseRequest(string serverName, string path, string domainName, out string versionNumber)
        {
            bool isValid = false;
            string trace = "";
            versionNumber = "invalid";
            if (this.ServerName.ToUpper() != serverName.ToUpper())
            {
                throw new Exception("Ongeldige licentie aanvraag. Servername klopt niet.");
            }
            if (this.DateFrom.GetValueOrDefault(new DateTime(1900, 1, 1)) > DateTime.Now ||
                this.DateTill.GetValueOrDefault(new DateTime(2900, 1, 1)) < DateTime.Now)
            {
                throw new Exception("Ongeldige licentie aanvraag. Licentie niet geldig");
            }
            domainName = domainName.ToLower();
            if (domainName.EndsWith("/")) domainName = domainName.Substring(0, domainName.Length - 1);
            
            trace += "Domain: " + domainName;
            string domainNameAlias = "";
            if (!domainName.Contains("www."))
            {
                //maar van bijv nu.nl --> www.nu.nl
                domainNameAlias = domainName.Replace("http://", "http://www.");
            }
            trace += "DomainAlias: " + domainNameAlias;
            path = path.ToLower();
            if (!path.EndsWith(@"\")) path += @"\";
            trace += ", Path: " + path;
            trace += ", EnvironmentsCount: " + Environments.Count;
            foreach (LicensedEnvironment env in Environments)
            {
                trace += ", EnvironmentPath: " + env.Path.ToLower();
                trace += ", EnvironmentUrl: " + env.DomainName.ToLower();
                if (env.Path.ToLower() == path && (env.DomainName.ToLower() == domainName || env.DomainName.ToLower() == domainNameAlias))
                {
                    isValid = true;
                    versionNumber = env.Version;
                    break;
                }
            }
            if (!isValid)
            {
                throw new Exception("Ongeldige licentie aanvraag. Path en/of Url kloppen niet. TRACE: " + trace);
            }
            return isValid;
        }
        /// <summary>
        /// vertalen van properties naar string voor in licenseFile object 
        /// </summary>
        /// <returns></returns>
        private string createPropertiesString()
        {
            StringBuilder sb = new StringBuilder();
            string permissionsString = convertArrayToString(Permissions);
            sb.AppendFormat("Permissions==>{0};;", permissionsString);
            sb.AppendFormat("MaxNumberOfDataCollections==>{0};;", MaxNumberOfDataCollections);
            sb.AppendFormat("MaxNumberOfDataPerDataCollection==>{0};;", MaxNumberOfDataPerDataCollection);
            sb.AppendFormat("MaxNumberOfNewsletterMailings==>{0};;", MaxNumberOfNewsletterMailings);
            sb.AppendFormat("MaxNumberOfNewsletters==>{0};;", MaxNumberOfNewsletters);
            sb.AppendFormat("MaxNumberOfNewsletterSubscribers==>{0};;", MaxNumberOfNewsletterSubscribers);
            sb.AppendFormat("MaxNumberOfNewsletterTemplates==>{0};;", MaxNumberOfNewsletterTemplates);
            sb.AppendFormat("MaxNumberOfPageFolders==>{0};;", MaxNumberOfPageFolders);
            sb.AppendFormat("MaxNumberOfPages==>{0};;", MaxNumberOfPages);
            sb.AppendFormat("MaxNumberOfScripts==>{0};;", MaxNumberOfScripts);
            sb.AppendFormat("MaxNumberOfSites==>{0};;", MaxNumberOfSites);
            sb.AppendFormat("MaxNumberOfStylesheets==>{0};;", MaxNumberOfStylesheets);
            sb.AppendFormat("MaxNumberOfTemplates==>{0};;", MaxNumberOfTemplates);
            sb.AppendFormat("MaxNumberOfUsers==>{0};;", MaxNumberOfUsers);
            sb.AppendFormat("AllowMultipleLanguages==>{0};;", AllowMultipleLanguages);

            string passwordEmailTemplate = Owner.PasswordEmailTemplate;
            string emailFrom = Owner.PasswordEmailFrom;
            string subject = Owner.PasswordEmailSubject;
            if (passwordEmailTemplate == "" || passwordEmailTemplate == null)
            {
                passwordEmailTemplate = Owner.Reseller.PasswordEmailTemplate;
            }
            if (emailFrom == "" || emailFrom == null)
            {
                emailFrom = Owner.Reseller.PasswordEmailFrom;
            }
            if (subject == "" || subject == null)
            {
                subject = Owner.Reseller.PasswordEmailSubject;
            }
            //voor zekerheid tekens nodig voor uitrafelen van props eruithalen
            passwordEmailTemplate = passwordEmailTemplate.Replace("==>", "=>").Replace(";;", ";");
            sb.AppendFormat("PasswordEmailFrom==>{0};;", emailFrom);
            sb.AppendFormat("PasswordEmailSubject==>{0};;", subject);
            sb.AppendFormat("PasswordEmailTemplate==>{0};;", passwordEmailTemplate);

            string resellerInfo = String.Format(@"T.: {0}<br />" +
                    "<a href=\"{1}\" target=\"\\_blank\">{1}</a><br />" +
                    "<a href=\"mailto:{2}\">{2}</a>", Owner.Reseller.Telephone, Owner.Reseller.Website, Owner.Reseller.Email);
            //voor zekerheid tekens nodig voor uitrafelen van props eruithalen
            resellerInfo = resellerInfo.Replace("==>", "=>").Replace(";;", ";");
            sb.AppendFormat("ResellerInfo==>{0};;", resellerInfo);
            sb.AppendFormat("ResellerLogoSrc==>{0};;", Owner.Reseller.Logo);

            sb.AppendFormat("END==>XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX;;");
            return sb.ToString();
            
            
        }

        private string convertArrayToString(int[] array)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int i in array)
            {
                sb.Append(i + ",");
            }
            if(sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        /// <summary>
        /// Uitlezen van database(s) per omgeving en hieruit de licentie stats halen
        /// </summary>
        /// <returns></returns>
        public string RetrieveStatistics()
        {
            string html = "";
            BaseCollection<LicensedEnvironment> environments = BaseCollection<LicensedEnvironment>.Get("FK_License = '" + this.ID + "'");
            foreach (LicensedEnvironment env in environments)
            {
                html += "Statistieken voor " + env.Name + "<br/>";
                html += env.GetStatisticsHtmlTable();
                html += "<br/><br/>";
            }
            return html;
        }



        
    }
}
