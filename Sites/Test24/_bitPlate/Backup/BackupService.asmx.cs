using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.IO;

using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Modules;
using BitSite._bitPlate.Scripts;
using BitPlate.Domain.Utils;
using BitSite._bitPlate._bitSystem;
using System.Diagnostics;
using System.Configuration;
using System.Data;
using BitPlate.Domain;
using BitPlate.Domain.Backup;

namespace BitSite._bitPlate.Backup
{
    [GenerateScriptType(typeof(CmsPage))]
    [GenerateScriptType(typeof(CmsPageFolder))]
    [GenerateScriptType(typeof(CmsTemplate))]
    [GenerateScriptType(typeof(CmsScript))]
    [System.Web.Script.Services.ScriptService]
    public partial class BackupService : BaseService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CreateBackup()
        {
            BitplateBackupServiceReference.BackupServiceClient backupClient = new BitplateBackupServiceReference.BackupServiceClient();
            bool test = backupClient.MakeBackup(SessionObject.CurrentSite.ID, SessionObject.CurrentSite.CurrentWorkingEnvironment.Path, System.Configuration.ConfigurationManager.ConnectionStrings["cmsdb"].ConnectionString);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BackupStatusReport GetBackupStatus()
        {
            BitplateBackupServiceReference.BackupServiceClient backupClient = new BitplateBackupServiceReference.BackupServiceClient();
            return JSONSerializer.Deserialize<BackupStatusReport>(backupClient.GetBackupStatus(SessionObject.CurrentSite.ID), true);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> GetAllBackups()
        {
            List<TreeGridItem> gridItems = new List<TreeGridItem>();
            if (!Directory.Exists(SessionObject.CurrentSite.Path + "\\..\\backups"))
            {
                Directory.CreateDirectory(SessionObject.CurrentSite.Path + "\\..\\backups");
            }
            string[] files = Directory.GetFiles(SessionObject.CurrentSite.Path + "\\..\\backups", "*.zip");
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                TreeGridItem item = new TreeGridItem();
                item.Name = fileInfo.Name;
                item.CreateDate = fileInfo.CreationTime;
                item.Url = "/downloadbackup.handler?file=" + fileInfo.Name;
                gridItems.Add(item);
            }
            return gridItems;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteBackup(string backupName)
        {
            if (File.Exists(SessionObject.CurrentSite.Path + "\\..\\backups\\" + backupName))
            {
                File.Delete(SessionObject.CurrentSite.Path + "\\..\\backups\\" + backupName);
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string RestoreBackup(string backupName)
        {
            BitplateBackupServiceReference.BackupServiceClient backupClient = new BitplateBackupServiceReference.BackupServiceClient();
            backupClient.RestoreBackup(SessionObject.CurrentSite.ID, SessionObject.CurrentSite.CurrentWorkingEnvironment.Path, backupName, System.Configuration.ConfigurationManager.ConnectionStrings["cmsdb"].ConnectionString);
            return ConfigurationManager.AppSettings["LicenseHost"] + "BackupRestore/restore.aspx?siteId=" + SessionObject.CurrentSite.ID.ToString() + "&returnUrl=" + SessionObject.CurrentSite.CurrentWorkingEnvironment.DomainName;
        }
        
        //public JsonResult GetBackupStatus()
        //{
        //    JsonResult result = new JsonResult();
        //    return result;
        //}
    }
}