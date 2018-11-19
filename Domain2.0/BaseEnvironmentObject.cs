using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Web.Configuration;

using HJORM.Attributes;
using BitPlate.Domain.Utils;
using System.Net.Configuration;
using System.Web;

namespace BitPlate.Domain
{


    /// <summary>
    /// Site kan meerdere omgevingen hebben
    /// Elke omgeving heeft eigen database, eigen url en eigen pad op de server
    /// Ook SMTP settings zijn te overrulen
    /// </summary>

    public class BaseEnvironmentObject : BaseDomainObject
    {

   
        private string _path;
        /// <summary>
        /// Pad eindigt altijd op \
        /// </summary>
        public string Path
        {
            get
            {
                if (!_path.EndsWith("\\"))
                {
                    _path += "\\";
                }
                return _path;
            }
            set { _path = value; }
        }
        private string _domainname;
        /// <summary>
        /// DomainName begint altijd met http:// of https://, eindigt nooit op /
        /// </summary>
        public string DomainName
        {
            get
            {
                if (_domainname != null && !_domainname.StartsWith("http") && !_domainname.StartsWith("https"))
                {
                    _domainname = @"http://" + _domainname;
                }
                if (_domainname != null && _domainname.EndsWith("/"))
                {
                    _domainname = _domainname.Substring(0, _domainname.Length - 1);
                }
                return _domainname;
            }
            set
            {
                _domainname = value;
            }
        }

        private string dataBaseServer
        {
            get
            {
                if (dataBaseConnectionStringParts == null) fillDataBaseConnectionStringParts();
                return dataBaseConnectionStringParts["data source"];
            }
        }

        private string _databaseName;
        public string DatabaseName
        {
            get
            {
                if (_databaseName == null || _databaseName == String.Empty)
                {
                    try
                    {
                        //database name uit config halen
                        string connString = ConfigurationManager.ConnectionStrings["cmsdb"].ConnectionString;
                        string[] parts = connString.Split(new char[] { ';' });
                        foreach (string part in parts)
                        {
                            if (part.ToLower().StartsWith("database"))
                            {
                                _databaseName = part.Split(new char[] { '=' })[1];
                            }
                        }
                    }
                    catch (Exception ex) { }

                }
                return _databaseName;
            }
            set { _databaseName = value; }
        }

        private string dataBaseUser
        {
            get
            {
                if (dataBaseConnectionStringParts == null) fillDataBaseConnectionStringParts();
                return dataBaseConnectionStringParts["user id"];
            }
        }
        private string dataBasePassword
        {
            get
            {
                if (dataBaseConnectionStringParts == null) fillDataBaseConnectionStringParts();
                return dataBaseConnectionStringParts["password"];
            }
        }

        private string _emailSettingsHost;
        public string EmailSettingsHost
        {
            get
            {
                if (_emailSettingsHost == null || _emailSettingsHost == String.Empty)
                {
                    _emailSettingsHost = readFromConfig("host");
                }
                return _emailSettingsHost;
            }
            set { _emailSettingsHost = value; }
        }

        private string _emailSettingsFrom;
        public string EmailSettingsFrom
        {
            get
            {
                if (_emailSettingsFrom == null || _emailSettingsFrom == String.Empty)
                {
                    _emailSettingsFrom = readFromConfig("from");
                }
                return _emailSettingsFrom;
            }
            set { _emailSettingsFrom = value; }
        }

        private string _mailSettingsUser;
        public string EmailSettingsUser
        {
            get
            {
                if (_mailSettingsUser == null || _mailSettingsUser == String.Empty)
                {
                    _mailSettingsUser = readFromConfig("user");
                }
                return _mailSettingsUser;
            }
            set { _mailSettingsUser = value; }
        }

        private string _emailSettingsPassword;
        public string EmailSettingsPassword
        {
            get
            {
                if (_emailSettingsPassword == null || _emailSettingsPassword == String.Empty)
                {
                    _emailSettingsPassword = readFromConfig("password");
                }
                return _emailSettingsPassword;
            }
            set { _emailSettingsPassword = value; }
        }


        private string readFromConfig(string type)
        {
            string returnValue = "";
            try
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
                if (type == "host")
                {
                    returnValue = settings.Smtp.Network.Host;
                }
                else if (type == "from")
                {
                    returnValue = settings.Smtp.From;
                }
                else if (type == "user")
                {
                    returnValue = settings.Smtp.Network.UserName;
                }
                else if (type == "password")
                {
                    returnValue = settings.Smtp.Network.Password;
                }
            }
            catch (Exception ex) { }
            return returnValue;
        }

        private Dictionary<string, string> dataBaseConnectionStringParts
        {
            get;
            set;
        }

        private void fillDataBaseConnectionStringParts()
        {
            //alle connecties moeten gebruik maken van dezelfde databaseuser, want anders kun je niet tussen de databases kopieren 
            dataBaseConnectionStringParts = new Dictionary<string, string>();
            string connectionString = ConfigurationManager.ConnectionStrings["cmsdb"].ConnectionString;

            string[] connectionStringParts = connectionString.Split(new char[] { ';' });
            foreach (string connectionStringPart in connectionStringParts)
            {
                if (connectionStringPart == "") continue;
                string[] nameValuePair = connectionStringPart.Split(new char[] { '=' });
                dataBaseConnectionStringParts.Add(nameValuePair[0].ToLower(), nameValuePair[1]);
            }

        }



        public string CreateWebConfig(string licenseHost, string bitplateHost, Guid SiteId)
        {
            

            string connectionString = CreateDataBaseConnectionString();
            string emailSettings = createEmailSettingsSection();
            string returnValue = String.Format(@"<?xml version=""1.0""?>
<configuration>
  <appSettings>
    <add key=""SQLCommandTimeOut"" value=""-1""/> <!-- 0 = geen timeout, -1 = default timeout-->
    <add key=""SiteID"" value=""{0}""/>
    <add key=""SiteEnvironmentID"" value=""{1}""/>
    <add key=""LicenseHost"" value=""{2}""/>
    <add key=""BitplateHost"" value=""{3}""/>
    <add key=""BundleScripts"" value=""false""/>
    <add key=""UseSQLLogging"" value=""false""/>
    <add key=""jQueryVersion"" value=""jquery-1.8.2.js""/>
  </appSettings>
  <connectionStrings>
    <add name=""cmsdb"" connectionString=""{4}"" providerName=""MySql""/>
  </connectionStrings>
  <system.web>
    <httpRuntime maxRequestLength=""999999"" />
    <sessionState timeout=""600""/>
    <httpHandlers>
      <add verb=""*"" path=""elfinder.connector"" type=""elFinder.Connector.Connector, elFinder.Connector""/>
      <add verb=""*"" path=""captcha.handler"" type=""BitSite._bitPlate.CaptchaImageHandler"" />
      <add verb=""*"" path=""script.handler"" type=""BitSite._bitPlate.ScriptHandler"" />
      <add verb=""*"" path=""downloadbackup.handler"" type=""BitSite._bitPlate.BackupDownloadHandler"" />
    </httpHandlers>
  </system.web>
  <system.web.extensions>
    <scripting>
      <webServices>
        <jsonSerialization maxJsonLength=""2147483644"" />
      </webServices>
    </scripting>
  </system.web.extensions>
  <system.net>
    <mailSettings>
      {5}
    </mailSettings>
  </system.net>

  <system.webServer>
    <directoryBrowse enabled=""false"" />
    <validation validateIntegratedModeConfiguration=""false""/>
    <!-- Voor iis7 moet runAllManagedModulesForAllRequests=""true"" aan staan -->
    <modules runAllManagedModulesForAllRequests=""true"" />
    <handlers>
      <add name=""ElFinderConnector"" verb=""*"" path=""*.connector"" type=""ElFinder.Connector.Connector"" />
      <add name=""ScriptResource"" preCondition=""integratedMode"" verb=""GET,HEAD"" path=""ScriptResource.axd"" type=""System.Web.Handlers.ScriptResourceHandler, System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" />
      <add name=""BitSiteCaptchaImage"" verb=""*"" path=""captcha.handler"" type=""BitSite._bitPlate.CaptchaImageHandler"" />
      <add name=""BitSiteScript"" verb=""*"" path=""script.handler"" type=""BitSite._bitPlate.ScriptHandler"" />
      <add name=""BitBackupDownload"" verb=""*"" path=""downloadbackup.handler"" type=""BitSite._bitPlate.BackupDownloadHandler"" />
    </handlers>
  </system.webServer>

  <system.serviceModel>
    <serviceHostingEnvironment aspNetCompatibilityEnabled=""true"" multipleSiteBindingsEnabled=""true"" />
    <bindings>
        <wsHttpBinding>
        <binding name=""WSHttpBinding_IBackupService"" />
        </wsHttpBinding>
    </bindings>
    <client>
        <endpoint address=""http://localhost:8000/bitnetmedia/bitplate/backup""
        binding=""wsHttpBinding"" bindingConfiguration=""WSHttpBinding_IBackupService""
        contract=""BitplateBackupServiceReference.IBackupService"" name=""WSHttpBinding_IBackupService"">
        <identity>
            <userPrincipalName value=""e.vantoor@twist.local"" />
        </identity>
        </endpoint>
    </client>
  </system.serviceModel>
</configuration>", SiteId, this.ID, licenseHost, bitplateHost, connectionString, emailSettings);
            return returnValue;
        }

        public virtual string CreateDataBaseConnectionString()
        {
            //alle connecties moeten gebruik maken van dezelfde databaseuser, want anders kun je niet tussen de databases kopieren 
            fillDataBaseConnectionStringParts();

            string newConnectionString = String.Format("Data Source={0};Database={1};user id={2};password={3};", dataBaseServer, DatabaseName, dataBaseUser, dataBasePassword);
            return newConnectionString;
        }

        protected string createEmailSettingsSection()
        {


            string emailSettings = String.Format(@"<smtp from=""{1}"">
        <network host=""{0}"" userName=""{2}"" password=""{3}""/>
      </smtp>", EmailSettingsHost, EmailSettingsFrom, EmailSettingsUser, EmailSettingsPassword);
            return emailSettings;
        }


    }
}

