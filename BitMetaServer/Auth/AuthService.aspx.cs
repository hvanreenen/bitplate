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

namespace BitMetaServer.bitAjaxServices
{

    public partial class AuthService : BaseService
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static bool Login(string email, string password, string theme, string language, bool saveData)
        {
            bool returnValue = false;
            string md5Hash = CalculateMD5Hash(password);

            MetaServerUser user = BaseObject.GetFirst<MetaServerUser>("Email ='" + email + "' AND Password = '" + md5Hash + "'");

            if (user != null && user.IsActive)
            {
                user.Theme = "bitplate";
                if (saveData)
                {
                    saveUserDataCookie(email, md5Hash); //OPSLAAN GEBRUIKERSGEGEVENS WERKT NOG NIET GOED.
                }
                else
                {
                    RemoveUserDataCookie();
                }

                SessionObject.CurrentUser = user;
                string log = String.Format("Login geslaagd, User: {0}", email);
                BitPlate.Domain.Utils.Logger.Log(AppDomain.CurrentDomain.BaseDirectory + "App_data\\Logs\\Logins.log", log);

                returnValue = true;
            }
            else
            {
                string log = String.Format("Login niet geslaagd, User: {0}", email);
                BitPlate.Domain.Utils.Logger.Log(AppDomain.CurrentDomain.BaseDirectory + "App_data\\Logs\\Logins.log", log);
            }

            return returnValue;
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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static JsonResult ChangePassword(string NewPassword, string NewPasswordConfirm, string Password)
        {
            CheckLogin();
            JsonResult result = new JsonResult();
            if (NewPassword == NewPasswordConfirm)
            {
                string currentPasswordHash = CalculateMD5Hash(Password).ToLower();
                string newPasswordHash = CalculateMD5Hash(NewPassword);

                //ToLowerCase BUG FIX.
                if (SessionObject.CurrentUser.Password.ToLower() == currentPasswordHash.ToLower())
                {
                    SessionObject.CurrentUser.Password = newPasswordHash;
                    SessionObject.CurrentUser.Save();
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                }


                result.DataObject = newPasswordHash;
                return result;
            }
            else
            {
                result.Success = false;
                return result;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static bool SendNewMetaServerUserPassword(string email)
        {
            CheckLogin();
            bool returnValue = false;
            MetaServerUser user = BaseObject.GetFirst<MetaServerUser>("Email ='" + email + "'");
            if (user != null)
            {
                returnValue = user.SendNewPasswordEmail();

            }
            return returnValue;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static bool SendNewMultiSiteUserPassword(string email)
        {
            CheckLogin();
            bool returnValue = false;
            MultiSiteUser user = BaseObject.GetFirst<MultiSiteUser>("Email ='" + email + "'");
            if (user != null)
            {
                returnValue = user.SendNewPasswordEmail();

            }
            return returnValue;
        }

         [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static MetaServerUser GetCurrentUser()
        {
            CheckLogin();
            return SessionObject.CurrentUser;
        }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         public static void SaveCurrentUser(MetaServerUser obj)
         {
             CheckLogin();
             obj.Save();
             SessionObject.CurrentUser = obj;
         }

        // ///////////////
        // SERVER USERS
        // /////////////////

        #region SERVERUSERS
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static BaseCollection<MetaServerUser> GetUsers(string sort, string searchString)
        {
            CheckLogin();
            string where = String.Format("(Name like '%{0}%' OR Email like '%{0}%')", searchString);
            BaseCollection<MetaServerUser> users = BaseCollection<MetaServerUser>.Get(where, sort);
            return users;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static MetaServerUser GetUser(string id)
        {
            CheckLogin();
            MetaServerUser user = null;
            if (id == null || id == "")
            {
                user = new MetaServerUser();
            }
            else
            {

                user = BaseObject.GetById<MetaServerUser>(new Guid(id));
            }

            return user;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SaveUser(MetaServerUser obj)
        {
            CheckLogin();
            obj.Save();
            
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void DeleteUser(Guid id)
        {
            CheckLogin();
            BaseObject.DeleteById<MetaServerUser>(id);
        }
        #endregion


        // ///////////////
        // MULTI-SITE USERS
        // /////////////////

        #region MULTI-SITEUSERS
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static BaseCollection<MultiSiteUser> GetMultiSiteUsers(string sort, string searchString)
        {
            CheckLogin();
            string where = String.Format("(Name like '%{0}%' OR Email like '%{0}%')", searchString);
            BaseCollection<MultiSiteUser> users = BaseCollection<MultiSiteUser>.Get(where, sort);
            return users;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static MultiSiteUser GetMultiSiteUser(string id)
        {
            CheckLogin();
            MultiSiteUser user = null;
            if (id == null || id == "")
            {
                user = new MultiSiteUser();
            }
            else
            {

                user = BaseObject.GetById<MultiSiteUser>(new Guid(id));
            }

            return user;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SaveMultiSiteUser(MultiSiteUser obj)
        {
            CheckLogin();
            obj.Save();

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void DeleteMultiSiteUser(Guid id)
        {
            CheckLogin();
            BaseObject.DeleteById<MultiSiteUser>(id);
        }
        #endregion

    }
}