using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;
using BitPlate.Domain.News;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BitAutorisation.Services
{
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        void SetUserData(BitplateUser user, string domainName);

        [OperationContract]
        MultiSiteUser GetUserData(BitplateUser user);

        [OperationContract]
        BaseCollection<LicensedEnvironment> GetSiteUrls(BitplateUser user);

        [OperationContract]
        string GenerateTempLoginKey(string url, BitplateUser user);

        [OperationContract]
        MultiSiteUser CheckTempLoginKey(string key);

        [OperationContract]
        MultiSiteUser Login(string siteDomainName, string email, string passwordMd5);

        [OperationContract]
        string HandShake();

        [OperationContract]
        BaseCollection<NewsItem> GetNewsItems();

    }
}
