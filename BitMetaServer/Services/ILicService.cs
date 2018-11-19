using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BitAutorisation.Services
{
    [ServiceContract]
    public interface ILicService
    {
        [OperationContract]
        LicenseFile GetLicense(string licenseCode, string serverName, string path, string domainName);

        [OperationContract]
        string HandShake();
    }
}
