using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJORM;
using HJORM.Attributes;
using System.IO;
using System.Web;

namespace BitPlate.Domain.Licenses
{


    [DataConnection("licenseDB")]
    public class LicensedEnvironment : BaseEnvironmentObject
    {

        public string SiteID { get; set; }
        public string Version { get; set; }

        /// <summary>
        /// Is redundant met License. Zit erin om te kunnen sorteren op servername 
        /// </summary>
        public string ServerName { get; set; }


        public string DatabaseServer { get; set; }
        //public string DatabaseName { get { return base.DataBaseName; } set { base.DataBaseName = value; } }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }


        [NonPersistent()]
        public Guid LicenseID { get; set; }



        private License _license;
        [System.Web.Script.Serialization.ScriptIgnore()]
        [IgnoreDataMember]
        [Association("FK_License")]
        public License License
        {
            get
            {
                if (_license == null && LicenseID != Guid.Empty)
                {
                    _license = new License();
                    _license.ID = LicenseID;
                }
                if (_license != null && !_license.IsLoaded)
                {
                    _license.Load();
                }
                return _license;
            }
            set
            {
                _license = value;

            }
        }
        [Persistent("LicenseFile")]
        public string EncryptedContent { get; set; }

        [NonPersistent()]
        public string LogMsg { get; set; }
        /// <summary>
        /// Environments worden opgeslagen via de licentie
        /// </summary>
        /// <param name="savedFromParent"></param>
        public override void Save(bool savedFromParent)
        {
            if (License != null)
            {
                LicenseFile file = License.CreateLicenseFile(License.ServerName, this.Path, this.DomainName);
                this.EncryptedContent = file.EncryptedContent;
                this.Name = DomainName;
                this.ServerName = License.ServerName;
            }
            if (Version == null || Version == "")
            {
                string[] directories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Versions\\FullVersion");
                if (directories.Length == 1)
                {
                    Version = System.IO.Path.GetFileName(directories[0]);
                }
            }
            base.Save(savedFromParent);
        }

        public List<LicenseStatistics> GetStatistics()
        {
            List<LicenseStatistics> stats = new List<LicenseStatistics>();
            stats.Add(getStatistic("page", License.MaxNumberOfPages, "Pagina's"));
            stats.Add(getStatistic("pagefolder", License.MaxNumberOfPageFolders, "Folders"));
            stats.Add(getStatistic("template", License.MaxNumberOfTemplates, "Templates"));
            stats.Add(getStatistic("script", License.MaxNumberOfScripts, "Scripts", "ScriptType = " + (int)ScriptTypeEnum.Js));
            stats.Add(getStatistic("script", License.MaxNumberOfStylesheets, "Stylesheets", "ScriptType = " + (int)ScriptTypeEnum.Css));

            stats.Add(getStatistic("datacollection", License.MaxNumberOfDataCollections, "Datacollecties"));
            //stats.Add(getStatistic("datacollection", License.Permissions.MaxNumberOfDataCollections, "Datacollection"));

            if (this.License.NewsletterLicenseType != NewsletterLicenseTypeEnum.BitnewsletterNone)
            {
                stats.Add(getStatistic("newsletter", License.MaxNumberOfNewsletters, "Nieuwsbrieven"));
                //stats.Add(getStatistic("newslettertemplate", License.Permissions.MaxNumberOfNewsletterTemplates, "NieuwsbriefTemplates"));
                stats.Add(getStatistic("newslettersubscriber", License.MaxNumberOfNewsletterSubscribers, "Abonnees"));
                stats.Add(getStatistic("newslettermailing", License.MaxNumberOfNewsletterMailings, "Verzendingen"));
            }
            return stats;
        }

        private LicenseStatistics getStatistic(string dbTable, int? available, string displayName)
        {
            return getStatistic(dbTable, available, displayName, "");
        }
        private LicenseStatistics getStatistic(string dbTable, int? available, string displayName, string extraWhere)
        {
            string sql = "SELECT COUNT(*) FROM " + dbTable;
            if (SiteID != "" && SiteID != null)
            {
                sql += "  WHERE FK_Site = '" + SiteID + "'";
            }
            if (extraWhere != "")
            {
                if (sql.Contains("WHERE"))
                {
                    sql += " AND " + extraWhere;
                }
                else
                {
                    sql += " WHERE " + extraWhere;
                }
            }

            Object result = DataBase.Get("cmsdb", this.CreateDataBaseConnectionString()).Execute(sql);
            int used = Convert.ToInt32(result);
            LicenseStatistics stat = new LicenseStatistics();
            stat.Name = displayName;
            stat.Used = used;
            stat.Available = available;
            return stat;


        }


        internal string GetStatisticsHtmlTable()
        {
            string html = "<table width='100%'>";
            try
            {
                html += "<tr><td style='font-weight:bold'>Onderdeel</td><td style='font-weight:bold'>Beschikbaar</td><td style='font-weight:bold'>Gebruikt</td><td style='font-weight:bold'>Over</td></tr>";
                foreach (LicenseStatistics stats in GetStatistics())
                {
                    string color = "green";
                    if (stats.Over.HasValue && stats.Over.Value < 5)
                    {
                        color = "orange";
                    }
                    if (stats.Over.HasValue && stats.Over.Value < 0)
                    {
                        color = "red";
                    }
                    html += String.Format("<tr><td>{0}</td><td style='font-weight:bold'>{1}</td><td style='font-weight:bold'>{2}</td><td style='font-weight:bold'><span style='color:{4}'>{3}</span></td></tr>",
                        stats.Name, stats.Available, stats.Used, stats.Over, color);


                }
                html += "</table>";
            }
            catch (Exception ex)
            {
                html = "Kan geen statistieken ophalen voor deze omgeving. Controleer de database instellingen. (Bij meer dan 1 site in de database moet ook de site id worden gevuld)<br/>";
                html += "Foutmelding: " + ex.Message;
            }
            return html;
        }

        public string DoUpdate(string toVersionNumber)
        {
            string logMsg = "";
            string trace = "";
            trace = "UPDATE UITGEVOERD OP OMGEVING: " + this.Name + " VAN VERSIE: " + this.Version + " NAAR VERSIE: " + toVersionNumber + "\r\n";
            logMsg = trace;
            string currentVersion = this.Version;
            if (DataBase.CheckConnection("MySql", this.CreateDataBaseConnectionString()))
            {
                throw new Exception("Controleer site omgeving instellingen. Kan niet verbinden met de database");
            }

            if (this.SiteID == null || this.SiteID == "" || this.SiteID == Guid.Empty.ToString())
            {
                throw new Exception(this.SiteID + " is een ongeldig siteID. Controleer de instellingen van de omgeving.");
            }

            try
            {

                string serverName = this.License.ServerName;
                string pathOnServer = this.Path;
                string destPath = "\\\\" + serverName + "\\" + pathOnServer.Replace(":", "$");
                //onderste regel: tijdelijk voor testen op de garfield
                destPath = pathOnServer;

                //kan zijn dat een omgeving meerdere updates achterligt:
                //daarom alle updates > currentversion en < toVersionNumber moeten worden gedaan
                List<string> versions = new List<string>();
                string[] directories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Versions");
                foreach (string dir in directories)
                {
                    versions.Add(System.IO.Path.GetFileName(dir));
                }
                versions = versions.OrderBy(s => s).ToList();
                foreach (string version in versions)
                {

                    if (version.CompareTo(currentVersion) > 0 && version.CompareTo(toVersionNumber) <= 0)
                    {
                        //STAP 1: bestanden kopieren
                        trace += "UPDATE NAAR VERSIE: " + version + "\r\n";
                        trace += "STAP 1: bestanden kopieren\r\n";
                        

                        string sourcePath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Versions\\" + version;

                        Utils.FileHelper.CopyDir(sourcePath, destPath);
                        //db mapje weer weggooien
                        Utils.FileHelper.DeleteDir(destPath + "\\DB");

                        //STAP 2: dbupdates doen

                        //string updatesSql = "";
                        if (Directory.Exists(sourcePath + "\\DB\\"))
                        {
                            trace += "STAP 2: dbupdates doen\r\n";
                            string[] files = Directory.GetFiles(sourcePath + "\\DB\\");
                            string lastFileName = "";
                            foreach (string file in files)
                            {
                                string sql = "USE " + this.DatabaseName + ";\r\n";
                                sql += File.ReadAllText(file);
                                trace += "STAP 2a: sql uitvoeren. File: " + System.IO.Path.GetFileName(file) + "\r\n";
                                //database updates uitvoeren
                                //DataBase.Get( "cmsdb", this.CreateDataBaseConnectionString()).Execute(sql);
                                if (file.Contains("_noerrors"))
                                {
                                    try
                                    {
                                        DataBase.Get("rootConnection").Execute(sql);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                else
                                {
                                    DataBase.Get("rootConnection").Execute(sql);
                                }
                                lastFileName = file;
                            }
                            trace += "STAP 2b: versienummer updaten\r\n";
                            string updatesql = "USE " + this.DatabaseName + ";";
                            updatesql += String.Format("UPDATE _DatabaseVersion SET VersionNumber= '{0}', LastDate = NOW(), LastFileName='{1}';", version, lastFileName);
                            DataBase.Get("rootConnection").Execute(updatesql);
                        }

                        trace += "EINDE VERSIE: " + version + "\r\n\r\n";

                        //if (File.Exists(version + "\\DB\\DBupdates.txt"))
                        //{
                        //    updatesSql += File.ReadAllText(version + "\\DB\\DBupdates.txt");
                        //}

                        //if (updatesSql != "")
                        //{
                        //    trace = "STAP 2a: dbupdates doen, sql uitvoeren";
                        //    //database aanmaken
                        //    DataBase.Get("cmsdb", this.getConnectionString()).Execute(updatesSql);
                        //}
                    }
                }

                trace += "STAP 3: versienummer ophogen en opslaan in licensedEnvironment\r\n";
                
                //STAP 3: versienummer ophogen en opslaan
                this.Version = toVersionNumber;
                this.Save();
                trace += "STAP 3a: versienummer opslaan in bin\\ map van site\r\n";
                trace += "EINDE " + this.Name + "\r\n============\r\n";

                string fileContent = "V" + toVersionNumber + " Laatste update uitgevoerd: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                fileContent += "\r\n" + trace;
                File.WriteAllText(destPath + "\\bin\\_versionNumber.txt", fileContent);
                


            }
            catch (Exception ex)
            {
                logMsg = "ERROR: Er is een fout opgetreden bij het uitvoeren van de update naar site " + this.Name + "\r\nTRACE: " + trace + "\r\n ERRORMSG: " + ex.Message;
                string fileName = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Versions\\UPDATE_ERRORLOG_" + DateTime.Now.ToString("yyyyMMddHHmm") + "_" + this.Name.Replace("http://", "").Replace("https://", "").Replace(":", "") + ".txt";
                File.WriteAllText(fileName, logMsg);
            }
            
            return logMsg;
        }

        public LicensedEnvironment CreateNewSite()
        {
            string logMsg = "NIEUWE SITE AANMAKEN<br/>";
            string sql = "";
            //create database
            try
            {
                sql = "CREATE DATABASE IF NOT EXISTS " + this.DatabaseName;
                DataBase.Get("rootConnection").Execute(sql);
                logMsg += "Database aangemaakt.<br/>";
            }
            catch (Exception ex)
            {
                logMsg += "ERROR. Database niet aangemaakt: " + ex.Message + "<br/>";
            }

            try
            {
                sql = "USE " + this.DatabaseName + ";";
                sql += "CREATE USER IF NOT EXISTS '" + this.DatabaseUser + "' IDENTIFIED BY '" + this.DatabasePassword + "';";
                DataBase.Get("rootConnection").Execute(sql);
                logMsg += "User aangemaakt.<br/>";
            }
            catch (Exception ex)
            {
                logMsg += "ERROR. User niet aangemaakt: " + ex.Message + "<br/>";
            }

            try
            {
                sql = "USE " + this.DatabaseName + ";";
                sql += "GRANT ALL ON " + this.DatabaseName + ".* TO '" + this.DatabaseUser + "';";
                DataBase.Get("rootConnection").Execute(sql);
                logMsg += "User granted.<br/>";
            }
            catch (Exception ex)
            {
                logMsg += "ERROR. User niet granted: " + ex.Message + "<br/>";
            }

            string version = "";
            try
            {
                string[] directories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Versions\\FullVersion");
                if (directories.Length != 1)
                {
                    throw new Exception("Er moet precies 1 versie van bitplate staan in App_Data\\Versions\\FullVersion");
                }
                version = System.IO.Path.GetFileName(directories[0]);
                sql = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Versions\\FullVersion\\" + version + "\\DB\\CREATETABLES.sql");

                sql = "USE " + this.DatabaseName + ";" + sql;
                DataBase.Get("rootConnection").Execute(sql);
                logMsg += "Tabellen aangemaakt.<br/>";
            }
            catch (Exception ex)
            {
                logMsg += "ERROR. Tabellen niet aangemaakt: " + ex.Message + "<br/>";
            }

            Guid newSiteID = Guid.NewGuid();
            try
            {
                string licCode = (License != null) ? License.Code : "";
                sql = "USE " + this.DatabaseName + ";";
                sql += "INSERT INTO Site (ID, Name, CreateDate, LicenceCode) VALUES ('" + newSiteID.ToString() + "', '" + this.DomainName + "', NOW(), '" + licCode + "');";
                sql += @"INSERT INTO SiteEnvironment (ID, Name, CreateDate, FK_Site, SiteEnvironmentType, Path, DomainName, DatabaseName) 
            VALUES (UUID(), 'Ontwikkel', NOW(), '" + newSiteID.ToString() + "', 0, '" + this.Path.Replace("\\", "\\\\") + "', '" + this.DomainName + "', '" + this.DatabaseName + "');";
                DataBase.Get("rootConnection").Execute(sql);
                logMsg += "Site & omgeving toegevoegd.<br/>";
            }
            catch (Exception ex)
            {
                logMsg += "ERROR. Site & omgeving niet toegevoegd.: " + ex.Message + "<br/>";
            }

            try
            {
                sql = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Versions\\FullVersion\\" + version + "\\DB\\INSERTDEFAULTS.sql");
                sql = "USE " + this.DatabaseName + ";" + sql;
                DataBase.Get("rootConnection").Execute(sql);
                logMsg += "Standaardwaardes toegevoegd.<br/>";
            }
            catch (Exception ex)
            {
                logMsg += "ERROR. Standaardwaardes niet toegevoegd.: " + ex.Message + "<br/>";
            }


            //create path
            string destPath = "";
            try
            {
                string serverName = this.License.ServerName.ToUpper();
                //destPath = "\\\\" + serverName + "\\" + this.Path.Replace(":", "$");

                destPath = this.Path;

                Directory.CreateDirectory(destPath);
                logMsg += "Map op server aangemaakt.<br/>";
            }
            catch (Exception ex)
            {
                logMsg += "ERROR. Map op server niet aangemaakt: " + ex.Message + "<br/>";
            }

            //copy bitplate
            try
            {
                string sourcePath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Versions\\FullVersion\\" + version;
                Utils.FileHelper.CopyDir(sourcePath, destPath);
                //versienummer zetten en wegschrijven in bestand
                //Versie nummer zit in licensedomgeving File.WriteAllText(destPath + "\\bin\\VersionNumber.txt", "V" + version);

                //db mapje weer weggooien
                Utils.FileHelper.DeleteDir(destPath + "\\DB");
                logMsg += "Bitplate gekopieerd.<br/>";
            }
            catch (Exception ex)
            {
                logMsg += "ERROR. Bitplate niet gekopieerd: " + ex.Message + "<br/>";
            }

            //web.config
            try
            {
                string webConfigFileContent = CreateWebConfig(newSiteID);
                File.WriteAllText(destPath + "\\web.config", webConfigFileContent);
                logMsg += "Web.config aangemaakt.<br/>";
            }
            catch (Exception ex)
            {
                logMsg += "ERROR. Web.config niet aangemaakt: " + ex.Message + "<br/>";
            }
            try
            {
                this.Version = version;
                this.SiteID = newSiteID.ToString();

                //if (License.IsNew)
                //{
                logMsg += "Gegevens opgeslagen<br/>";
                License.Environments.Add(this);
                License.Save();
                //}
            }
            catch (Exception ex)
            {
                logMsg += "ERROR. Gegevens niet opgeslagen: " + ex.Message + "<br/>";
            }
            logMsg += "EINDE<br/>";
            this.LogMsg = logMsg;
            return this;
        }


        public string CreateWebConfig(Guid SiteId)
        {
            string licenseHost = HttpContext.Current.Request.Url.ToString().Replace(HttpContext.Current.Request.Path, "");
            return base.CreateWebConfig(licenseHost, this.DomainName, SiteId);
        }

        public override string CreateDataBaseConnectionString()
        {

            string newConnectionString = String.Format("Data Source={0};Database={1};user id={2};password={3};", DatabaseServer, DatabaseName, DatabaseUser, DatabasePassword);
            return newConnectionString;
        }


    }
}
