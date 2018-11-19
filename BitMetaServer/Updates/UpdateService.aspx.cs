using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

using HJORM;

using BitPlate.Domain;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Utils;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using BitPlate.Domain.Licenses;
using System.IO;
using BitMetaServer;

namespace BitMetaServer.Updates
{
    public partial class UpdateService : BaseService
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<string> GetVersions()
        {
            CheckLogin();
            List<string> versions = new List<string>();
            string path = GetUpdatesRootPath();
            string[] directories = Directory.GetDirectories(path);
            foreach (string dir in directories)
            {
                if (Path.GetFileName(dir) != "FullVersion")
                {
                    versions.Add(Path.GetFileName(dir));
                }
            }
            versions = versions.OrderByDescending(s => s).ToList();
            return versions;
        }

        public static string GetUpdatesRootPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Versions";
        }
        public static string GetLastVersionNumber()
        {
            return GetVersions()[0];
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static BaseCollection<LicensedEnvironment> GetLicensedEnvironmentsWithLesserVersion(string versionNumber, string sort)
        {
            CheckLogin();
            string where = string.Format("version < '{0}'", versionNumber);

            return BaseCollection<LicensedEnvironment>.Get(where, sort);
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string DOUpdates(string[] LicensedEnvironmentsIDs, string versionNumber)
        {
            CheckLogin();
            string logMsg = "";
            foreach (string id in LicensedEnvironmentsIDs)
            {
                if (id != "")
                {
                    //updat uitvoeren
                    LicensedEnvironment env = BaseObject.GetById<LicensedEnvironment>(new Guid(id));
                    logMsg += env.DoUpdate(versionNumber);
                }
            }
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Versions\\UPDATE_LOG_ALL_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";
            File.WriteAllText(fileName, logMsg);
            return logMsg;
        }


    }
}