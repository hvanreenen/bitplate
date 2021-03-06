﻿using System;
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

namespace BitSite._bitPlate.SiteUsers
{
    //[GenerateScriptType(typeof(CmsSiteUser))]
    public partial class SiteAuthService : BaseService
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static bool Login(string email, string password, string language = "")
        {
            BaseService.CheckLoginAndLicense();
            Guid SiteId = Guid.Parse(ConfigurationManager.AppSettings["SiteID"]);
            SiteUser user = SiteUser.Login<SiteUser>(email, CalculateMD5Hash(password));
            if (user != null)
            {
                if (language != "")
                {

                }

                SessionObject.CurrentSiteUser = user;
                return true;
            }
            else
            {
                return false;
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
            return md5hash.ToLower();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static bool ChangePassword(string NewPassword, string NewPasswordConfirm, string Password)
        {
            BaseService.CheckLoginAndLicense();
            Dictionary<string, object> returnValue = new Dictionary<string, object>();
            if (NewPassword == NewPasswordConfirm)
            {
                string CurrentPassword = CalculateMD5Hash(Password).ToLower();
                if (CurrentPassword == SessionObject.CurrentSiteUser.Password)
                {
                    SiteUser user = SessionObject.CurrentSiteUser;
                    user.Password = CalculateMD5Hash(NewPassword).ToLower();
                    user.Save();
                    SessionObject.CurrentSiteUser = user;
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static bool SendNewPassword(string email)
        {
            BaseService.CheckLoginAndLicense();
            bool returnValue = false;
            SiteUser user = SiteUser.GetFirst<SiteUser>("Email ='" + email + "'");
            if (user != null)
            {
                returnValue = user.SendNewPasswordEmail();

            }
            return returnValue;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<TreeGridItem> GetUsers(string sort, int pageNumber, int pageSize, string searchString)
        {
            BaseService.CheckLoginAndLicense();
            List<TreeGridItem> returnValue = new List<TreeGridItem>();
            

            

            string where = String.Format(@" EXISTS(SELECT * FROM SiteUserGroup g 
                INNER JOIN SiteUserGroupPerUser ug ON ug.FK_UserGroup = g.ID 
                WHERE g.FK_Site='{0}' AND ug.FK_User = SiteUser.ID)", SessionObject.CurrentSite.ID);

            Guid environmentId = Guid.Empty; //todo
            string connectionString = getConnectionString(environmentId);
            BaseCollection<SiteUser> users = BaseCollection<SiteUser>.Get(where, sort, pageNumber, pageSize, connectionString: connectionString);
            foreach (SiteUser user in users)
            {
                TreeGridItem item = TreeGridItem.NewItem<SiteUser>(user);
                item.Name = user.CompleteName;
                foreach (SiteUserGroup group in user.UserGroups)
                {
                    item.Field1 += group.CompleteName + ", ";
                }
                if (item.Field1 != null && item.Field1 != "")
                {
                    item.Field1 = item.Field1.Substring(0, item.Field1.Length - 2);
                }
                returnValue.Add(item);
            }
            return returnValue;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static SiteUser GetUser(string id)
        {
            BaseService.CheckLoginAndLicense();
            SiteUser user = null;
            Guid environmentId = Guid.Empty; //todo
            string connectionString = getConnectionString(environmentId); 
            if (id == null || id == "")
            {
                user = new SiteUser();
                
            }
            else
            {
                user = SiteUser.GetById<SiteUser>(new Guid(id), connectionString);
            }
            return user;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SaveUser(SiteUser obj)
        {
            BaseService.CheckLoginAndLicense();
            obj.Site = SessionObject.CurrentSite;
            Guid environmentId = Guid.Empty; //todo
            string connectionString = getConnectionString(environmentId);
            obj.SetConnectionString(connectionString);
            obj.Save();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void DeleteUser(string id)
        {
            BaseService.CheckLoginAndLicense();

            Guid environmentId = Guid.Empty; //todo
            string connectionString = getConnectionString(environmentId);
            BaseObject.DeleteById<SiteUser>(new Guid(id), connectionString);
        }
        /////////////////////////////////
        //USERGROUPS
        //////////////////////////////////
        #region USERGROUPS
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static SiteUserGroup[] GetUserGroups(string sort, int pageNumber, int pageSize, string searchString)
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_Site = '{0}'", SessionObject.CurrentSite.ID);
            


            //List<TreeGridItem> returnValue = new List<TreeGridItem>();

            //BitAutorisationService.AutorisationClient client = new BitAutorisationService.AutorisationClient();
            //BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
            //UserGroup[] usergroups = client.GetUserGroups(SessionObject.CurrentBitplateUser, onlyCurrentSite, sort, pageNumber, pageSize, searchString);
            //foreach (UserGroup usergroup in usergroups)
            //{
            //    TreeGridItem item = TreeGridItem.NewGroup<UserGroup>(usergroup);
            //    item.Field1 = usergroup.Site.Name;
            //    returnValue.Add(item);
            //}

            Guid environmentId = Guid.Empty; //todo
            string connectionString = getConnectionString(environmentId);

            return BaseCollection<SiteUserGroup>.Get(where, sort, pageNumber, pageSize, connectionString: connectionString).ToArray();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static SiteUserGroup GetUserGroup(string id)
        {
            BaseService.CheckLoginAndLicense();
            
            Guid environmentId = Guid.Empty; //todo
            string connectionString = getConnectionString(environmentId);

            SiteUserGroup userGroup;
            if (id == null)
            {
                userGroup = new SiteUserGroup();
                
                userGroup.Site = SessionObject.CurrentSite;
            }
            else
            {
                userGroup = SiteUserGroup.GetById<SiteUserGroup>(new Guid(id), connectionString);
            }
            

            /* UserGroup userGroup = null;
            if (id == null || id == "")
            {
                userGroup = new UserGroup();
                userGroup.UserGroupPermissions = new BaseCollection<Functionality>();
                userGroup.Site = new LicenseSite();
                userGroup.Site.ID = SessionObject.CurrentSite.ID;
            }
            else
            {
                //BitAutorisationService.AutorisationClient client = new BitAutorisationService.AutorisationClient();
                BitSite.BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
                userGroup = client.GetUserGroup(new Guid(id));
            } */
            return userGroup;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SaveUserGroup(SiteUserGroup obj)
        {
            BaseService.CheckLoginAndLicense();


            Guid environmentId = Guid.Empty; //todo
            string connectionString = getConnectionString(environmentId);

            obj.Site = SessionObject.CurrentSite;
            obj.SetConnectionString(connectionString);
            obj.Save();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void DeleteUserGroup(string id)
        {
            BaseService.CheckLoginAndLicense();

            Guid environmentId = Guid.Empty; //todo
            string connectionString = getConnectionString(environmentId);
            BaseObject.DeleteById<SiteUserGroup>(new Guid(id), connectionString);
        }
        #endregion

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static SiteUser GetCurrentUser()
        {
            return SessionObject.CurrentSiteUser;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SaveCurrentUser(SiteUser obj)
        {
            obj.Save();
            SessionObject.CurrentSiteUser = obj;

        }

        private static string getConnectionString(Guid environmentID)
        {
            //todo
            //CmsSiteEnvironment env = BaseObject.GetById<CmsSiteEnvironment>(environmentID);
            //string connectionString = env.CreateDataBaseConnectionString();
            string connectionString = "Data Source=localhost;Database=bitplate20_asd2;user id=root;password=mysql;";
            connectionString = "";
            return connectionString;
        }
    }
}