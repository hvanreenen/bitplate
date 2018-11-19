using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPlate.Domain.Backup
{
    public class BackupStatusReport
    {
        public BackupStatusReport()
        {
            this.BackupCompleted = false;
        }

        public Guid SiteId { get; set; }
        public string ArchiveName { get; set; }
        public string StatusMessage { get; set; }
        public bool BackupCompleted { get; set; }
        public int Progress { get; set; }
    }
}
