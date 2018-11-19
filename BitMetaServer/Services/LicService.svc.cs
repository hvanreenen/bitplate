using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;
using HJORM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BitAutorisation.Services
{
    public class LicService : BaseMetaService, ILicService
    {
        public LicenseFile GetLicense(string licenseCode, string serverName, string path, string domainName)
        {
            string logMsg = "Licentie aanvraag voor code: " + licenseCode + ", server: " + serverName + ", locatie: " + path + ", domain: " + domainName + "\r\n";
            LicenseFile licFile = null;
            try
            {
                License license = null;
                string where = String.Format("Code = '{0}'", licenseCode);
                license = BaseObject.GetFirst<License>(where);
                if (license != null && license.IsActive)
                {
                    //versleutel license met path, url, datum
                    licFile = license.CreateLicenseFile(serverName, path, domainName);
                    if (license == null) logMsg += "Geen licFile aangemaakt";
                    else logMsg += "licFile aangemaakt ";
                }
                else
                {
                    if (license == null) logMsg += "Geen licentie gevonden voor code: " + licenseCode;
                    else if (license.IsActive) logMsg += "Licentie is niet actief "; 
                }
            }
            catch (Exception ex)
            {
                //log error
                logMsg += "ERROR: " + ex.Message + "\r\n " + ex.StackTrace;
            }
            TryWriteLogMsg(logMsg);
            return licFile;
            
        }

        private void TryWriteLogMsg(string logMsg)
        {
            string use = ConfigurationManager.AppSettings["UseLicenseRequestLogging"];
            if (use.ToLower() == "true")
            {
                string path = ConfigurationManager.AppSettings["LogPath"];

                string filename = path + "\\LicenseRequest_log_" + DateTime.Now.ToString("yyyyMM") + ".txt";
                BitPlate.Domain.Utils.Logger.Log(filename, logMsg + "\r\n===============\r\n\r\n");
            }
        }
    }
}
