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
    public class UserService : BaseMetaService, IUserService
    {

        public void SetUserData(BitplateUser user, string domainName)
        {
            MultiSiteUser multiSiteUser = MultiSiteUser.LoadFromBitplateUser(user);
            if (multiSiteUser == null)
            {
                multiSiteUser = new MultiSiteUser();
            }
            multiSiteUser.FromBitplateUser(user);
            multiSiteUser.SetEnvironment(domainName);
            multiSiteUser.Save();
        }

        public MultiSiteUser GetUserData(BitplateUser user)
        {
            MultiSiteUser multiSiteUser = MultiSiteUser.LoadFromBitplateUser(user);
            //zorg dat omgevingen niet mee gaan richting bitplate, want dan krijg je ook de llicentie nog een keer mee
            multiSiteUser.Sites.Clear();
            return multiSiteUser;
        }

        public BaseCollection<LicensedEnvironment> GetSiteUrls(BitPlate.Domain.Autorisation.BitplateUser user)
        {
            MultiSiteUser multiSiteUser = MultiSiteUser.LoadFromBitplateUser(user);
            if (multiSiteUser != null)
            {
                return multiSiteUser.Sites;
            }
            else return null;
        }


        public string GenerateTempLoginKey(string url, BitplateUser user)
        {
            MultiSiteUser multiSiteUser = MultiSiteUser.LoadFromBitplateUser(user);
            if (multiSiteUser != null)
            {
                return multiSiteUser.GenenateAndSaveTemporaryKey();
            }
            else return "";
        }

        public MultiSiteUser CheckTempLoginKey(string key)
        {
            MultiSiteUser multiSiteUser = MultiSiteUser.LoadFromTempKey(key);
            if (multiSiteUser != null)
            {
                //zorg dat omgevingen niet mee gaan richting bitplate, want dan krijg je ook de llicentie nog een keer mee
                multiSiteUser.Sites.Clear();
                return multiSiteUser;
            }
            else
            {
                return null;
            }
        }


        public MultiSiteUser Login(string siteDomainName, string email, string passwordMd5)
        {
            MultiSiteUser user = BaseObject.GetFirst<MultiSiteUser>("Email ='" + email + "' AND Password = '" + passwordMd5 + "' AND Active = 1");
            //check site url
            bool valid = false;
            if (user != null && user.IsActive)
            {
                foreach (LicensedEnvironment env in user.Sites)
                {
                    if (siteDomainName == env.DomainName)
                    {
                        valid = true;
                        break;
                    }
                }
            }
            if (valid)
            {
                //zorg dat omgevingen niet mee gaan richting bitplate, want dan krijg je ook de llicentie nog een keer mee
                user.Sites.Clear();
                return user;
            }
            else
            {
                return null;
            }
        }


        public BaseCollection<NewsItem> GetNewsItems()
        {
            return BaseCollection<NewsItem>.Get();
        }
    }
}
