using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitplateBackupService;
using System.ServiceModel;
using HJORM;
using BitPlate.Domain;
using Ionic.Zip;
using BitplateBackupService.Components.Service.ContractInterfaces;
using System.Diagnostics;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using BitPlate.Domain.Utils;
using BitPlate.Domain.Backup;

namespace BitplateBackupService.Components.Service
{
    public class BackupService: IBackupService
    {
        public static List<BackupStatusReport> StatusLogs;
        public bool MakeBackup(Guid siteId, string filePath, string databaseConnectionString)
        {
            if (StatusLogs == null)
            {
                StatusLogs = new List<BackupStatusReport>();
            }
            BackupStatusReport backupStatusFlag = new BackupStatusReport();
            backupStatusFlag.SiteId = siteId;
            StatusLogs.Add(backupStatusFlag);
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments.Add("siteId", siteId);
            arguments.Add("filePath", filePath);
            arguments.Add("databaseConnectionString", databaseConnectionString);
            BackgroundWorker backupWorker = new BackgroundWorker();
            backupWorker.DoWork += backupWorker_DoWork;
            backupWorker.RunWorkerAsync(arguments);
            
            return true;
        }

        private void backupWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            BackgroundWorker currentWorker = (BackgroundWorker)sender;
            Dictionary<string, object> arguments = (Dictionary<string, object>)e.Argument;
            string filePath = arguments["filePath"].ToString();
            string databaseConnectionString = arguments["databaseConnectionString"].ToString();
            Guid siteId = (Guid)arguments["siteId"];

            BackupStatusReport currentBackupStatusReportObject = StatusLogs.Where(c => c.SiteId == siteId).FirstOrDefault();
            currentBackupStatusReportObject.BackupCompleted = false;
            Console.WriteLine("Initializeren");
            currentBackupStatusReportObject.StatusMessage = "Backup initializeren.";
            createBackupFolders(filePath);
            //WriteFile(filePath + "\\..\\backups\\Temp\\backupstatus.txt", "Initializeren");
            DirectoryInfo workingDirecotry = new DirectoryInfo(filePath + "\\..\\backups");
            string timeStamp = DateTime.Now.ToString("yyMMddHHmmss");
            ZipFile backupFile = new ZipFile(filePath + "\\..\\backups\\bitplate-" + timeStamp + ".zip");
            backupFile.ParallelDeflateThreshold = -1;
            backupFile.SaveProgress += backupFile_SaveProgress;
            currentBackupStatusReportObject.ArchiveName = backupFile.Name;
            backupFile.AddDirectory(filePath, "");
            backupFile.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

            //WriteFile(filePath + "\\..\\backups\\Temp\\backupstatus.txt", "Bezig met het maken van databasedump.");
            currentBackupStatusReportObject.StatusMessage = "Bezig met het maken van databasedump.";
            Console.WriteLine("Bezig met het maken van databasedump.");
            string databaseDump = this.GetMySqlDump(databaseConnectionString);
            this.WriteFile(filePath + "\\..\\backups\\Temp\\sqldump.sql", databaseDump);
            backupFile.AddFile(filePath + "\\..\\backups\\Temp\\sqldump.sql", "");

            currentBackupStatusReportObject.StatusMessage = "Bezig met het inpakken van de backup. (0%)";
            Console.WriteLine("Bezig met het inpakken van de backup. " + filePath + "\\..\\backups\\bitplate-" + timeStamp + ".zip");
            //WriteFile(filePath + "\\..\\backups\\Temp\\backupstatus.txt", "Bezig met het inpakken van de backup.");
            backupFile.Save();
            backupFile.Dispose();
            currentBackupStatusReportObject.BackupCompleted = true;
            currentBackupStatusReportObject.StatusMessage = "Backup is geslaagd.";
            Console.WriteLine(backupFile.Name + " is opgeslagen");
            //WriteFile(filePath + "\\..\\backups\\Temp\\backupstatus.txt", "Backup is geslaagd.");
            
        }

        void backupFile_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            BackupStatusReport currentBackupStatusReportObject = StatusLogs.Where(c => c.ArchiveName == e.ArchiveName).FirstOrDefault();
            if (e.EventType == ZipProgressEventType.Saving_Completed) {
                
            }
            else if (e.EventType == ZipProgressEventType.Saving_BeforeWriteEntry)
            {
                currentBackupStatusReportObject.StatusMessage = string.Format("Bezig met het inpakken van de backup. ({2:N0}%)", e.EntriesSaved, e.EntriesTotal, e.EntriesSaved / (0.01 * e.EntriesTotal));
            }
        }

        public string GetBackupStatus(Guid siteId)
        {
            string returnValue = "";
            if (StatusLogs != null)
            {
                BackupStatusReport currentBackupStatusReportObject = StatusLogs.Where(c => c.SiteId == siteId).FirstOrDefault();
                if (currentBackupStatusReportObject != null)
                {
                    returnValue = JSONSerializer.Serialize(currentBackupStatusReportObject);
                    if (returnValue != null && currentBackupStatusReportObject.BackupCompleted)
                    {
                        StatusLogs.Remove(currentBackupStatusReportObject);
                    }
                }
            }
            return returnValue;
        }

        public bool RestoreBackup(Guid siteId, string filePath, string backupName, string databaseConnectionString)
        {
            if (StatusLogs == null)
            {
                StatusLogs = new List<BackupStatusReport>();
            }
            BackupStatusReport backupStatusFlag = new BackupStatusReport();
            backupStatusFlag.ArchiveName = backupName;
            backupStatusFlag.SiteId = siteId;
            StatusLogs.Add(backupStatusFlag);

            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments.Add("siteId", siteId);
            arguments.Add("filePath", filePath);
            arguments.Add("backupName", backupName);
            arguments.Add("databaseConnectionString", databaseConnectionString);

            BackgroundWorker restoreWorker = new BackgroundWorker();
            restoreWorker.DoWork += restoreWorker_DoWork;
            restoreWorker.RunWorkerAsync(arguments);
            return true;
        }

        void restoreWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //System.Threading.Thread.Sleep(10000); //Wacht met verwijderen zodat bitplate de tijd heeft om tot rust te komen.
            Dictionary<string, object> arguments = (Dictionary<string, object>)e.Argument;
            string backupName = arguments["backupName"].ToString();
            string filePath = arguments["filePath"].ToString();
            string connectionString = arguments["databaseConnectionString"].ToString();
            Guid siteId = (Guid)arguments["siteId"];
            createBackupFolders(filePath);

            BackupStatusReport currentBackupStatusReportObject = StatusLogs.Where(c => c.SiteId == siteId).FirstOrDefault();
            currentBackupStatusReportObject.StatusMessage = "Verwijderen systeem bestanden.";
            
            string backupFilePath = filePath + "\\..\\backups\\" + backupName;
            if (File.Exists(backupFilePath))
            {
                Console.WriteLine("Verwijderen systeem bestanden.");
                while (Directory.Exists(filePath + "\\bin"))
                {
                    try
                    {
                        Directory.Delete(filePath + "\\bin", true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        currentBackupStatusReportObject.StatusMessage = "De toegang tot de bin map is geweigerd. Restoreservice wacht op toestemming van IIS.";
                    }
                    
                }

                while (File.Exists(filePath + "\\web.config"))
                {
                    try
                    {
                        File.Delete(filePath + "\\web.config");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        currentBackupStatusReportObject.StatusMessage = "De toegang tot bestand web.config is geweigerd. Restoreservice wacht op toestemming van IIS.";
                    }
                    
                }

                currentBackupStatusReportObject.StatusMessage = "Bezig met uitpakken van de backup. (0%)";
                Console.WriteLine("Uitpakken van backup.");
                currentBackupStatusReportObject.ArchiveName = backupFilePath;
                ZipFile backupFile = new ZipFile(backupFilePath);
                backupFile.ParallelDeflateThreshold = -1;
                backupFile.ExtractProgress += backupFile_ExtractProgress;
                DirectoryInfo wwwrootDirectoryInfo = new DirectoryInfo(filePath);
                foreach (FileInfo file in wwwrootDirectoryInfo.GetFiles())
                {
                    file.IsReadOnly = false;
                    file.Delete();
                }
                foreach (DirectoryInfo dir in wwwrootDirectoryInfo.GetDirectories())
                {
                    if (dir.Name != "Bin") //Kennerlijk wordt bin directory nog wel gevonden door .NET. Deze is hier boven al verwijderd.
                    {
                        dir.Delete(true);
                    }
                }
                backupFile.ExtractAll(filePath, ExtractExistingFileAction.OverwriteSilently);
                backupFile.Dispose();

                currentBackupStatusReportObject.StatusMessage = "Bezig met importeren van database.";
                Console.WriteLine("Bezig met importeren van database.");
                string sqlFile = filePath + "\\sqldump.sql";
                string sql = this.ReadFile(sqlFile);
                this.DropAllTables(connectionString);
                this.ImportMySQLDump(connectionString, sql);
                File.Delete(sqlFile);
                currentBackupStatusReportObject.StatusMessage = "Backup is met succes terug gezet.";
                Console.WriteLine("Backup is met succes terug gezet.");
                currentBackupStatusReportObject.BackupCompleted = true;
            }
            else
            {
                currentBackupStatusReportObject.StatusMessage = "De geselecteerde backup kan niet worden gevonden.";
                Console.WriteLine("De geselecteerde backup kan niet worden gevonden.");
                currentBackupStatusReportObject.BackupCompleted = true;
            }
        }

        private void backupFile_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            BackupStatusReport currentBackupStatusReportObject = StatusLogs.Where(c => c.ArchiveName == e.ArchiveName).FirstOrDefault();
            if (e.EventType == ZipProgressEventType.Extracting_AfterExtractAll)
            {
                currentBackupStatusReportObject.StatusMessage = "Uitpakken van backup voltooid.";
                ////Dit moet anders, maar weet nog even niet HOE
                //currentBackupStatusReportObject.Progress += 25;
                //if (currentBackupStatusReportObject.Progress == 100)
                //{
                //    currentBackupStatusReportObject.StatusMessage = "Restore geslaagd.";
                //}
            }

            if (e.EventType == ZipProgressEventType.Extracting_EntryBytesWritten)
            {

                currentBackupStatusReportObject.StatusMessage = string.Format("Bezig met uitpakken van de backup. ({2:N0}%)", e.BytesTransferred, e.TotalBytesToTransfer,
                              e.BytesTransferred / (0.01 * e.TotalBytesToTransfer));
            }
        }

        //public List<BackupFile> GetAllBackups(string filePath)
        //{
        //    List<BackupFile> fileInfoList = new List<BackupFile>();
        //    this.createBackupFolders(filePath);
        //    string[] files = Directory.GetFiles(filePath, "*.zip");
        //    foreach (string file in files)
        //    {
        //        FileInfo fileInfo = new FileInfo(file);
        //        BackupFile backupFile = new BackupFile();
        //        backupFile.CompleteName = fileInfo.FullName;
        //        backupFile.Name = fileInfo.Name;
        //        backupFile.CreationDate = fileInfo.CreationTime;
        //        fileInfoList.Add(backupFile);
        //    }
        //    return fileInfoList;
        //}

        private void createBackupFolders(string filePath)
        {
            if (!Directory.Exists(filePath + "\\..\\backups"))
            {
                Directory.CreateDirectory(filePath + "\\..\\backups");
            }

            if (!Directory.Exists(filePath + "\\..\\backups\\Temp"))
            {
                Directory.CreateDirectory(filePath + "\\..\\backups\\Temp");
            }
        }

        private void WriteFile(string path, string content)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(content);
                sw.Flush();
            }
        }

        private string ReadFile(string path)
        {
            string content = "";
            using (StreamReader sr = new StreamReader(path))
            {
                content = sr.ReadToEnd();
            }
            return content;
        }

        private string GetMySqlDump(string connectionString)
        {
           System.Data.SqlClient.SqlConnectionStringBuilder connectionstringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = AppDomain.CurrentDomain.BaseDirectory + "Tools\\mysqldump.exe";
            proc.RedirectStandardInput = false;
            proc.RedirectStandardOutput = true;
            proc.Arguments = "-h" + connectionstringBuilder.DataSource + " -u" + connectionstringBuilder.UserID + " -p" + connectionstringBuilder.Password + " " + connectionstringBuilder.InitialCatalog;
            proc.UseShellExecute = false;
            Process p = Process.Start(proc);
            string databaseDump = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Dispose();
            return "SET FOREIGN_KEY_CHECKS = 0;\r\n" + databaseDump + "\r\nSET FOREIGN_KEY_CHECKS = 1;";
        }

        private void ImportMySQLDump(string connectionString, string sqldump)
        {
            DataBase dataBase = DataBase.Get("MySql", connectionString);
            dataBase.Execute(sqldump);
        }

        private void DropAllTables(string connectionString)
        {
            DataBase dataBase = DataBase.Get("MySql", connectionString);
            DataTable dt = dataBase.GetDataTable("SHOW TABLES");
            string sql = "SET FOREIGN_KEY_CHECKS = 0;\r\nDROP TABLE IF EXISTS ";
            foreach (DataRow row in dt.Rows)
            {
                sql += row[0].ToString() + ",\r\n";
            }
            sql = sql.Substring(0, sql.Length - 5);
            sql += ";\r\nSET FOREIGN_KEY_CHECKS = 1;";

            if (dt.Rows.Count > 0)
            {
                dataBase.Execute(sql);
            }
        }
    }


    
}
