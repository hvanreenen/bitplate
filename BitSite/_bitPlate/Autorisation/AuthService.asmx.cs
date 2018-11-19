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
using BitPlate.Domain.News;

namespace BitSite._bitPlate.bitAjaxServices
{
    [System.Web.Script.Services.ScriptService]
    public partial class AuthService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void CreateSystemUserGroupsIfNotExists(out BitplateUserGroup UserGroupAdmins, out BitplateUserGroup UserGroupEveryOne)
        {
            UserGroupAdmins = BaseObject.GetFirst<BitplateUserGroup>("FK_Site = '" + SessionObject.CurrentSite.ID + "' AND IsSystemValue = 1 AND Type=9");
            if (UserGroupAdmins == null)
            {
                UserGroupAdmins = new BitplateUserGroup();
                UserGroupAdmins.Type = UserTypeEnum.SiteAdmins;
                UserGroupAdmins.Name = "Admins";
                UserGroupAdmins.IsSystemValue = true;
                UserGroupAdmins.Save();
            }

            UserGroupEveryOne = BaseObject.GetFirst<BitplateUserGroup>("FK_Site = '" + SessionObject.CurrentSite.ID + "' AND IsSystemValue = 1 AND Type=0");
            if (UserGroupEveryOne == null)
            {
                UserGroupEveryOne = new BitplateUserGroup();
                UserGroupEveryOne.Type = UserTypeEnum.Custom;
                UserGroupEveryOne.Name = "Everyone";
                UserGroupEveryOne.IsSystemValue = true;
                UserGroupEveryOne.Save();
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public bool Login(string email, string password, string theme, string language, bool saveData, string md5Hash)
        {

            BitplateUserGroup userGroupAdmins;
            BitplateUserGroup userGroupEveryOne;
            CreateSystemUserGroupsIfNotExists(out userGroupAdmins, out userGroupEveryOne);

            SessionObject.CurrentSiteUser = null;
            bool returnValue = false;
            Guid SiteId;
            Guid.TryParse(ConfigurationManager.AppSettings["SiteID"], out SiteId);
            if (md5Hash == null)
            {
                md5Hash = CalculateMD5Hash(password);
            }
            BitplateUser user = BaseObject.GetFirst<BitplateUser>("Email ='" + email + "' AND Password = '" + md5Hash + "' AND Active = 1");

            if (user != null)
            {
                returnValue = true;
                SessionObject.CurrentBitplateUser = user;
                user.Theme = "bitplate";

                if (saveData)
                {
                    saveUserDataCookie(email, md5Hash); //OPSLAAN GEBRUIKERSGEGEVENS WERKT NOG NIET GOED.
                }
                else
                {
                    RemoveUserDataCookie();
                }
                if (user.IsMultiSiteUser)
                {
                    
                    //gegevens syncen
                    //haal data op van meta server
                    string domainName = getDomainName();
                    UserServiceReference.UserServiceClient client = BitMetaServerServicesHelper.GetUserServiceClient();
                    MultiSiteUser multiSiteUser = null;
                    if (client != null) multiSiteUser = client.Login(domainName, email, md5Hash);
                    //opslaan
                    if (multiSiteUser != null)
                    {
                        BitplateUser localUser = multiSiteUser.ToBitPlateUser(user.Email);
                        if (localUser.ModifiedDate > user.ModifiedDate)
                        {
                            localUser.UserGroups = user.UserGroups;
                            localUser.Permissions = user.Permissions;
                            localUser.Save();
                            user = localUser;
                        }

                        if (multiSiteUser.IsAdmin)
                        {
                            if (!user.IsUserMemberOf(userGroupAdmins.Name))
                            {
                                user.UserGroups.Add(userGroupAdmins);
                                user.Save();
                            }
                        }
                        else
                        {
                            if (user.IsUserMemberOf(userGroupAdmins.Name))
                            {
                                BitplateUserGroup userGroup = BaseObject.GetFirst<BitplateUserGroup>("FK_Site = '" + SessionObject.CurrentSite.ID + "' AND Type=9");
                                user.UserGroups.Remove(userGroupAdmins);
                                user.Save();
                            }
                        }
                        //Gebruiker toevoegen aan systeem usergroup everyone.
                        if (!user.IsUserMemberOf(userGroupEveryOne.Name))
                        {
                            user.UserGroups.Add(userGroupEveryOne);
                            user.Save();
                        }
                        returnValue = true;
                        SessionObject.CurrentBitplateUser = user;
                    }
                    else if (multiSiteUser == null && client != null)
                    {
                        //Delete user als dit een multisiteuser is welke niet meer voorkomt in de licentie server.
                        user.Delete();
                        returnValue = false;
                    }
                    else
                    {
                        returnValue = true;
                        SessionObject.CurrentBitplateUser = user;
                    }
                }

                

                
            }
            else
            {
                //probeer in te loggen bij metaserver
                string domainName = getDomainName();
                UserServiceReference.UserServiceClient client=  BitMetaServerServicesHelper.GetUserServiceClient();
                MultiSiteUser multiSiteUser = client.Login(domainName, email, md5Hash);

                if (multiSiteUser != null)
                {
                    //nieuwe user aanmaken en opslaan
                    BitplateUser localUser = multiSiteUser.ToBitPlateUser(email);
                    //usergroup toevoegen
                    if (multiSiteUser.IsAdmin)
                    {
                        localUser.UserGroups.Add(userGroupAdmins);
                    }
                    //Gebruiker toevoegen aan systeem usergroup everyone.
                    localUser.UserGroups.Add(userGroupEveryOne);
                    localUser.Save();
                    SessionObject.CurrentBitplateUser = localUser;

                    returnValue = true;
                }
            }

            return returnValue;
        }

        private static string getDomainName()
        {
            string relativeUrl = HttpContext.Current.Request.Url.PathAndQuery;
            string completeUrl = HttpContext.Current.Request.Url.ToString();
            string domainName = completeUrl.Replace(relativeUrl, "");
            return domainName;
        }

        private static void saveUserDataCookie(string UserName, string Password)
        {
            HttpCookie AuthCookie;
            bool isNew = false;
            if (HttpContext.Current.Request.Cookies["BITAUTHORISATION"] != null)
            {
                AuthCookie = HttpContext.Current.Request.Cookies["BITAUTHORISATION"];
            }
            else
            {
                isNew = true;
                AuthCookie = new HttpCookie("BITAUTHORISATION");
            }

            AuthCookie.Domain = HttpContext.Current.Request.Url.Host;
            DateTime ExpireDate = DateTime.Now;
            ExpireDate = ExpireDate.AddYears(1);
            AuthCookie.Expires = ExpireDate;
            AuthCookie["UserName"] = UserName;
            //AuthCookie["Password"] = Password;
            //HttpContext.Current.Response.Cookies.Add(AuthCookie);
            if (isNew)
            {
                //HttpContext.Current.Response.Cookies.Add(AuthCookie);
                HttpContext.Current.Response.SetCookie(AuthCookie);
            }
            else
            {
                HttpContext.Current.Response.SetCookie(AuthCookie);
            }

        }

        private static void RemoveUserDataCookie()
        {
            if (HttpContext.Current.Request.Cookies["BITAUTHORISATION"] != null)
            {
                HttpContext.Current.Request.Cookies.Remove("BITAUTHORISATION");
            }

            if (HttpContext.Current.Response.Cookies["BITAUTHORISATION"] != null)
            {
                HttpContext.Current.Response.Cookies.Remove("BITAUTHORISATION");
            }
        }


        private static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            string md5hash = sb.ToString();
            return md5hash.ToUpper();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult ChangePassword(string NewPassword, string NewPasswordConfirm, string Password)
        {
            JsonResult result = new JsonResult();
            if (NewPassword == NewPasswordConfirm)
            {
                string currentPasswordHash = CalculateMD5Hash(Password).ToLower();
                string newPasswordHash = CalculateMD5Hash(NewPassword);

                //ToLowerCase BUG FIX.
                if (SessionObject.CurrentBitplateUser.Password.ToLower() == currentPasswordHash.ToLower())
                {
                    SessionObject.CurrentBitplateUser.Password = newPasswordHash;
                    SessionObject.CurrentBitplateUser.Save();
                    result.Success = true;
                    result.DataObject = newPasswordHash;
                }
            }
            else
            {
                result.Success = false;
            }
            return result;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public bool SendNewPassword(string email)
        {
            bool returnValue = false;
            BitplateUser user = BaseObject.GetFirst<BitplateUser>("Email ='" + email + "'");
            //voor als je aankomt vanuit login.aspx --> nieuw ww aanvragen --> dan is er nog geen lic gezet
            LicenseFile lic = SessionObject.CurrentLicense;
             if (user != null)
            {
                returnValue = user.SendNewPasswordEmail();
                if (user.IsMultiSiteUser)
                {
                    string domainName = getDomainName();
                    UserServiceReference.UserServiceClient client = BitMetaServerServicesHelper.GetUserServiceClient();
                    client.SetUserData(user, domainName);
                }
            }
            return returnValue;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BitplateUser GetCurrentUser()
        {
            return SessionObject.CurrentBitplateUser;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveCurrentUser(BitplateUser obj)
        {
            obj.Save();
            if (obj.IsMultiSiteUser)
            {
                //userGroups & permissions hoeven niet opgeslagen bij multisite users
                obj.UserGroups.Clear();
                obj.Permissions = null;
                string domainName = getDomainName();
                UserServiceReference.UserServiceClient client = BitMetaServerServicesHelper.GetUserServiceClient();
                client.SetUserData(obj, domainName);
                //weer laden om permissions & usergroups weer op te halen, usergroups eerst null maken
                obj.UserGroups = null;
                obj.Load();
            }
            SessionObject.CurrentBitplateUser = obj;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<BitplateUser> GetUsers(string sort, string searchString)
        {
            //users hebben geen fk_site (omdat mogelijk in de toekomst meer sites in 1 database kunnen en dan users in meer sites moeten kunnen)
            //sites zijn gekoppeld via usergroups
            string where = String.Format(@" EXISTS(SELECT * FROM BitplateUserGroup g 
                LEFT JOIN BitplateUserGroupPerUser ug ON ug.FK_UserGroup = g.ID 
                WHERE g.FK_Site='{0}' AND ug.FK_User = BitplateUser.ID)", SessionObject.CurrentSite.ID);
            if (searchString != null && searchString != "")
            {
                where += String.Format(" AND (Name like '%{0}%' OR Email like '%{0}%')", searchString);
            }
            BaseCollection<BitplateUser> users = BaseCollection<BitplateUser>.Get(where, sort);
            return users;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BitplateUser GetUser(string id)
        {
            BitplateUser user = null;
            if (id == null || id == "")
            {
                user = new BitplateUser();
            }
            else
            {
                user = BaseObject.GetById<BitplateUser>(new Guid(id));
            }
            return user;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveUser(BitplateUser obj)
        {
            //Quick password fix, kan vast mooier.
            if (!obj.IsNew)
            {
                BitplateUser currentUserObject = BaseObject.GetById<BitplateUser>(obj.ID);
                obj.Password = currentUserObject.Password;
            }

            obj.Save();
            if (obj.IsMultiSiteUser)
            {
                //userGroups & permissions hoeven niet opgeslagen bij multisite users
                obj.UserGroups.Clear();
                obj.Permissions = null;
                string domainName = getDomainName();
                UserServiceReference.UserServiceClient client= BitMetaServerServicesHelper.GetUserServiceClient();
                client.SetUserData(obj, domainName);
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteUser(Guid id)
        {
            BaseObject.DeleteById<BitplateUser>(id);
        }
        /////////////////////////////////
        //USERGROUPS
        //////////////////////////////////
        #region USERGROUPS
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<BitplateUserGroup> GetUserGroups(string sort, string searchString)
        {
            string where = String.Format("FK_Site = '{0}' ", SessionObject.CurrentSite.ID);
            if (searchString != "")
            {
                where += String.Format(" AND (Name like '%{0}%')", searchString);
            }
            BaseCollection<BitplateUserGroup> usergroups = BaseCollection<BitplateUserGroup>.Get(where, sort);
            return usergroups;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BitplateUserGroup GetUserGroup(string id)
        {
            BitplateUserGroup userGroup = null;
            
            if (id == null || id == "")
            {
                userGroup = new BitplateUserGroup();
            }
            else
            {
                userGroup = BaseObject.GetById<BitplateUserGroup>(new Guid(id));
            }
            return userGroup;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveUserGroup(BitplateUserGroup obj)
        {
            obj.Save();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteUserGroup(Guid id)
        {
            BaseObject.DeleteById<BitplateUserGroup>(id);
        }
        #endregion

        /////////////////////////////////
        //SITES
        //////////////////////////////////
        #region MULTI SITES
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public LicensedEnvironment[] GetSites(string sort, string searchString)
        {
            UserServiceReference.UserServiceClient client = BitMetaServerServicesHelper.GetUserServiceClient();
            BitplateUser copyUser = new BitplateUser();
            copyUser.Email = SessionObject.CurrentBitplateUser.Email;
            copyUser.Password = SessionObject.CurrentBitplateUser.Password;
            LicensedEnvironment[] returnValue = client.GetSiteUrls(copyUser);
            client.Close();
            return returnValue;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ChangeSite(string id, string url)
        {
            //1 melden bij metaserver dat je gaat wisselen van site onder dezelfde gebruiker
            // meta server stuurt een key terug waarmee je ingelogd blijft
            //key meegeven in url van site waarnaar je gaat wisselen
            //2 wisselen sit
            //bij nieuwe site wordt key weer opgestuurd aan metaserver
            //klopt deze sleutel dan zuto inloggen bij de nieuwe site
            UserServiceReference.UserServiceClient client = BitMetaServerServicesHelper.GetUserServiceClient();
            BitplateUser copyUser = new BitplateUser();
            copyUser.Email = SessionObject.CurrentBitplateUser.Email;
            copyUser.Password = SessionObject.CurrentBitplateUser.Password;
            string loginkey = client.GenerateTempLoginKey(url, copyUser);
            client.Close();

            return url + "/_bitplate/Login.aspx?loginkey=" + loginkey;




            //if (SessionObject.CurrentBitplateUser.CurrentSite == null)
            //{
            //    SessionObject.CurrentBitplateUser.CurrentSite = new LicenseSite();
            //}
            //SessionObject.CurrentBitplateUser.CurrentSite.ID = new Guid(id);
            //SessionObject.CurrentBitplateUser.StoreValues4CurrentSite(new Guid(id));
            //CmsSite site = BaseObject.GetById<CmsSite>(new Guid(id));
            //string serverName = Environment.MachineName;
            //BitSite.BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
            //SessionObject.CurrentBitplateUser = client.ChangeSite(SessionObject.CurrentBitplateUser, new Guid(id));



            //if (site == null)
            //{
            //    SessionObject.CurrentSite = new CmsSite();
            //}
            //else
            //{
            //    //site.Port = HttpContext.Current.Request.Url.Port;
            //    //zet working environment in site
            //    string where = String.Format("FK_Site='{0}' AND SiteEnvironmentType={1}", site.ID, (int)SiteEnvironmentTypeEnum.Editable);
            //    CmsSiteEnvironment environment = BaseObject.GetFirst<CmsSiteEnvironment>(where);
            //    site.CurrentWorkingEnvironment = environment;
            //    //zet sessie
            //    SessionObject.CurrentSite = site;
            //    License l = client.LoadLicense(site.LicenceCode, serverName, site.DomainName);
            //    SessionObject.CurrentLicense = client.LoadLicense(site.LicenceCode, serverName, site.DomainName);
            //    site.IsValidLicense = (SessionObject.CurrentLicense != null);

            //}

        }

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public LicenseSite GetSite(string id)
        //{
        //    LicenseSite site = null;
        //    if (id == null || id == "")
        //    {
        //        site = new LicenseSite();
        //        site.License = new License();
        //    }
        //    else
        //    {
        //        BitSite.BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
        //        site = client.GetSite(new Guid(id));
        //        client.Close();
        //    }

        //    return site;
        //}

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public void SaveSite(LicenseSite obj)
        //{
        //    BitSite.BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();

        //    //ook site aanmaken in db
        //    bool useOwnDatabase = false;
        //    if (obj.IsNew && !useOwnDatabase)
        //    {
        //        Guid siteID = client.SaveSite(obj);
        //        CmsSite site = new CmsSite();
        //        site.Name = obj.Name;
        //        site.LicenceCode = obj.License.Code;
        //        site.CurrentWorkingEnvironment = new CmsSiteEnvironment();
        //        site.CurrentWorkingEnvironment.Name = "Live";
        //        site.CurrentWorkingEnvironment.Site = site;
        //        site.CurrentWorkingEnvironment.DomainName = obj.Url;
        //        site.CurrentWorkingEnvironment.Path = obj.Path;

        //        //check uniek
        //        string where = String.Format("Path = '{0}'", site.Path);
        //        if (BaseCollection<CmsSite>.Get(where).Count > 0)
        //        {
        //            throw new Exception("Server bestandslocatie is niet uniek. Er bestaat al een site op: " + site.Path + ". Geef een andere fysieke locatie op. (opmerking: als u de locatie leeg laat wordt de site opgeslagen op de locatie van deze bitplate-installatie. Dat mag maar 1 site zijn.)");
        //        }
        //        site.Save();
        //        // ID van licenseSite en Site moeten gelijk worden
        //        // Dat gebeurt in update query, want bij ingevuld id denkt ORM dat het om bestaand record gaat en doet update query ipv insert
        //        string sql = String.Format("UPDATE Site SET ID = '{0}' WHERE ID = '{1}'", siteID, site.ID);
        //        DataBase.Get().Execute(sql);

        //        site.CurrentWorkingEnvironment.Site.ID = siteID;
        //        site.CurrentWorkingEnvironment.Save();



        //    }
        //    else
        //    {
        //        client.SaveSite(obj);
        //    }
        //    client.Close();
        //}

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public void DeleteSite(string id)
        //{

        //    //BitSite.BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
        //    BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
        //    client.DeleteSite(new Guid(id));
        //    client.Close();
        //}

        

        #endregion

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public NewsItem[] GetNewsItems()
        {
            UserServiceReference.UserServiceClient client = BitMetaServerServicesHelper.GetUserServiceClient();
            return client.GetNewsItems();
        }
    }
}