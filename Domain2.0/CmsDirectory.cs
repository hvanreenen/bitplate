using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace BitPlate.Domain
{
    
    public class CmsDirectory: BaseDomainSiteObject
    {
        public string FullName { get; set; }

        public CmsDirectory(DirectoryInfo DirectoryInfoObject)
        {
            this.Name = DirectoryInfoObject.Name;
            this.ModifiedDate = DirectoryInfoObject.LastWriteTime;
            this.CreateDate = DirectoryInfoObject.CreationTime;
            this.FullName = this.relativePath(DirectoryInfoObject.FullName);
        }

        private string relativePath(string FullName)
        {
            int index = FullName.ToUpper().IndexOf("\\_FILES");
            string virtualPath = FullName.Substring(index, FullName.Length - index);
            return virtualPath;
        }
    }
}
