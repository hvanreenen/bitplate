using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Logging;

namespace BitPlate.Domain.Autorisation
{
    /// <summary>
    /// Dit is de klant van een site
    /// </summary>
    [Persistent("User")]
    public class CmsSiteUser: BaseUser
    {
        public CmsSiteUser()
        {
            base.UserType = "CmsSiteUser";
        }

        [NonPersistent()]
        public string SecretCode { get { return ID.ToString(); } }
        public string FormData { get; set; }
        public bool ReceiveNewsletter { get; set; }
        public DateTime? DateUnsubscribed { get; set; }
        /// <summary>
        /// Registratie via (bijv. Invoer, Website, Import)
        /// </summary>
        public string Registration { get; set; }
        public string IPAddress { get; set; }
        public bool EmailIsValid { get; set; }
        /// <summary>
        /// Voor gebruikers die alleen nieuwsbrief ontvangen is allowlogin false
        /// </summary>
        public bool AllowLogin { get; set; }

        private CmsSite _site;
        [Association("FK_Site")]
        public CmsSite Site
        {
            get
            {
                if (_site != null && !_site.IsLoaded)
                {
                    _site.Load();
                }

                return _site;
            }
            set { _site = value; }
        }

        BaseCollection<CmsSiteUserGroup> _userGroups;
        [NonPersistent()]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public BaseCollection<CmsSiteUserGroup> UserGroups
        {
            get
            {
                //if (_availableLayouts == null)
                {
                    string where = "EXISTS (SELECT * FROM UserGroupPerUser WHERE FK_User = '" + this.ID + "' AND UserGroup.ID = FK_UserGroup)";
                    _userGroups = BaseCollection<CmsSiteUserGroup>.Get(where, "Name");
                }
                return _userGroups;
            }
            set
            {
                _userGroups = value;
            }
        }


        //komt binnen na opt-in email. 
        //in de email staat een link die deze functi aanroept
        public static bool ConfirmNewsletter(string guid)
        {
            try
            {
                CmsSiteUser user = BaseCollection<CmsSiteUser>.Get("SecretCode = '" + guid + "'")[0];
                user.EmailIsValid = true;
                user.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool UnsubscribeNewsletter(string guidOrEmail)
        {
            try
            {
                CmsSiteUser user;
                if (guidOrEmail.Contains("@") || guidOrEmail.Contains("%40"))
                {
                    user = BaseCollection<CmsSiteUser>.Get("Email = '" + guidOrEmail + "'")[0];
                }
                else
                {
                    user = BaseCollection<CmsSiteUser>.Get("SecretCode = '" + guidOrEmail + "'")[0];
                }
                user.ReceiveNewsletter = false;
                user.DateTill = DateTime.Now;
                user.DateUnsubscribed = DateTime.Now;
                user.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
