using BitMetaServer;
using BitPlate.Domain.Backup;
using BitPlate.Domain.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace BitMetaServer20.BackupRestore
{
    /// <summary>
    /// Summary description for RestoreService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class RestoreService : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BackupStatusReport GetBackupStatus(Guid siteId)
        {
            try
            {
                BitplateBackupServiceReference.BackupServiceClient backupClient = new BitplateBackupServiceReference.BackupServiceClient();
                return JSONSerializer.Deserialize<BackupStatusReport>(backupClient.GetBackupStatus(siteId), true);
            }
            catch (Exception ex)
            {
                BackupStatusReport exceptioReport = new BackupStatusReport();
                exceptioReport.BackupCompleted = true;
                exceptioReport.StatusMessage = "Kan BitBackupService niet vinden.\r\n" + ex.ToString();
                return exceptioReport;
            }
            
        }
    }
}
