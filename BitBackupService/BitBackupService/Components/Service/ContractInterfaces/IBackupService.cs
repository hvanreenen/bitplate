using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BitplateBackupService.Components.Service.ContractInterfaces
{
    [ServiceContract(Namespace = "http://bitnetmedia.bitplate.backup")]
    public interface IBackupService
    {
        [OperationContract]
        bool MakeBackup(Guid siteId, string filePath, string databaseConnectionString);

        [OperationContract]
        string GetBackupStatus(Guid siteId);

        [OperationContract]
        bool RestoreBackup(Guid siteId, string filePath, string backupName, string databaseConnectionString);
    }
}
