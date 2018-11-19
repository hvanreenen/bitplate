using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Logging;
using BitPlate.Domain.Licenses;

namespace BitPlate.Domain.Autorisation
{

    public class BitplateUser : BaseUser
    {
        private string _theme;
        public string Theme
        {
            get
            {
                if (_theme == null || _theme == "")
                {
                    _theme = "Bitplate";
                }
                return _theme;
            }
            set
            {
                _theme = value;
            }
        }

        public string Language { get; set; }

        public bool IsSystemValue { get; set; }

        [IgnoreDataMember]
        public int[] Permissions { get; set; }

        /// alle permissions van alle groepen en van de user zelf bij elkaar opgeteld
        /// </summary>
        /// 
        private List<int> _allPermissions;
        [NonPersistent()]
        public List<int> GetAllPermissions()
        {
            if (_allPermissions == null)
            {
                _allPermissions = new List<int>();
                foreach (BitplateUserGroup userGroup in UserGroups)
                {
                    _allPermissions.AddRange(userGroup.Permissions);
                }
                if (this.Permissions != null) _allPermissions.AddRange(this.Permissions);
                _allPermissions = _allPermissions.Distinct().ToList();
            }
            return _allPermissions;
        }

        public bool HasPermission(FunctionalityEnum func)
        {
            bool returnvalue = false;
            if (this.GetUserType() == UserTypeEnum.Developers || this.GetUserType() == UserTypeEnum.SiteAdmins)
            {
                returnvalue = true;
            }
            else
            {
                foreach (int number in GetAllPermissions())
                {
                    if (number == (int)func)
                    {
                        returnvalue = true;
                        break;
                    }
                }
            }


            return returnvalue;
        }


        private BaseCollection<BitplateUserGroup> _userGroups;
        /// <summary>
        /// In een usergroup zit altijd een site.
        /// Als de gebruiker aan meerdere sites is gekoppeld, dan heeft die meerdere usergroups.
        /// Een gebruiker kan ook meerdere usergroups hebben per site.
        /// </summary>
        /// 
        [Persistent("BitplateUserGroupPerUser")]
        [Association("FK_User", "FK_UserGroup")]
        //[DataMember]
        [IgnoreDataMember]
        public BaseCollection<BitplateUserGroup> UserGroups
        {
            get
            {
                if (_userGroups == null)
                {
                    string where = "EXISTS (SELECT * FROM BitplateUserGroupPerUser WHERE FK_User = '" + this.ID + "' AND BitplateUserGroup.ID = BitplateUserGroupPerUser.FK_UserGroup)";
                    _userGroups = BaseCollection<BitplateUserGroup>.Get(where, "Name");

                }
                return _userGroups;
            }
            set
            {
                _userGroups = value;
            }
        }
        private string _userGroupsString = "";
        [NonPersistent()]
        [IgnoreDataMember]
        public string UserGroupsString
        {
            get
            {
                if (_userGroupsString == "")
                {
                    foreach (BitplateUserGroup group in this.UserGroups)
                    {
                        _userGroupsString += group.Name + ", ";
                    }
                    if (_userGroupsString.Length > 2)
                    {
                        _userGroupsString = _userGroupsString.Substring(0, _userGroupsString.Length - 2);
                    }
                }
                return _userGroupsString;
            }
        }

        public bool IsMultiSiteUser { get; set; }

        public string[] SiteUrls { get; set; }

        /// <summary>
        /// gooi hoogste nummer terug uit enum
        /// </summary>
        /// <returns></returns>
        public UserTypeEnum GetUserType()
        {
            int superType = 0;
            foreach (BitplateUserGroup userGroup in UserGroups)
            {
                if ((int)userGroup.Type > superType)
                {
                    superType = (int)userGroup.Type;
                }
            }
            return (UserTypeEnum)superType;
        }
        /// <summary>
        /// Voor controle vanuit DefaultPage.Load() en BaseModuleUserControl.CheckAutorisation()
        /// Er wordt string meegegeven met de geautoriseerde groepen en users (id's met komma ertussen)
        /// </summary>
        /// <param name="autorizedUserGroupsString"></param>
        /// <param name="autorizedUsersString"></param>
        /// <returns></returns>
        public bool IsAutorized(string autorizedUserGroupsString, string autorizedUsersString)
        {
            bool isAutorized = false;
            
            //kijk of string van geautoriseerde gebruikersgroepen een groep van deze gebruiker bevat
            foreach (BitplateUserGroup userGroup in this.UserGroups)
            {
                if (autorizedUserGroupsString != null && autorizedUserGroupsString.Contains(userGroup.ID + ","))
                {
                    isAutorized = true;
                    break;
                }
            }
            //als er geen match is gevonden, kijken of user voorkomt in de string van geautoriseerde gebruikers
            if (!isAutorized)
            {
                if (autorizedUsersString != null && autorizedUsersString.Contains(this.ID + ","))
                {
                    isAutorized = true;
                }
            }
            return isAutorized;
        }

        //Controlleerd of een gebruiker lid is van een groep.
        public override bool IsUserMemberOf(string groupName)
        {
            BitplateUserGroup existInUserGroup = BaseObject.GetFirst<BitplateUserGroup>("EXISTS(SELECT * FROM bitplateusergroupperuser WHERE bitplateusergroupperuser.FK_UserGroup = BitplateUserGroup.ID AND bitplateusergroupperuser.FK_User = '" + this.ID.ToString() + "') AND bitplateusergroup.Name = '" + groupName + "'");
            return (existInUserGroup != null);
        }

        public override bool SendNewPasswordEmail()
        {
            //nog niet ingelogd, en dus nog geen licentie, maar nieuwe gebruiker is aangemaakt vanuit mutisiteuser
            if (IsMultiSiteUser && Utils.WebSessionHelper.CurrentLicense == null)
            {
                return false;
            }

            string password = Utils.RandomPasswordGenerator.Generate(8);
            this.Password = CalculateMD5Hash(password);
            this.PasswordLastChanged = DateTime.Now;
            this.Save();

            string passwordEmailTemplate = Utils.WebSessionHelper.CurrentLicense.PasswordEmailTemplate;
            string emailFrom = Utils.WebSessionHelper.CurrentLicense.PasswordEmailFrom;
            string subject = Utils.WebSessionHelper.CurrentLicense.PasswordEmailSubject;

            //Company owner = Utils.WebSessionHelper.CurrentLicense.Owner;
            //string passwordEmailTemplate = owner.PasswordEmailTemplate;
            //string emailFrom = owner.PasswordEmailFrom;
            //string subject = owner.PasswordEmailSubject;
            //if (passwordEmailTemplate == "" || passwordEmailTemplate == null)
            //{
            //    Company reseller = Utils.WebSessionHelper.CurrentLicense.Owner.Reseller;
            //    passwordEmailTemplate = reseller.PasswordEmailTemplate;
            //    emailFrom = reseller.PasswordEmailFrom;
            //    subject = reseller.PasswordEmailSubject;
            //}

            if (passwordEmailTemplate == "" || passwordEmailTemplate == null)
            {
                passwordEmailTemplate = @"Beste [name],<br/>
<br/>            
Hierbij ontvangt u de login gegevens voor Bitplate CMS.<br/>
Url: <a href='[url]'>[url]</a><br/>
Email: [email]<br/>
Wachtwoord: [password]<br/>
<br/>
Met vriendelijke groet,<br/>
<br/>
Bitplate team";
            }

            if (emailFrom == "" || emailFrom == null)
            {
                emailFrom = "info@bitnetmedia.nl";
            }

            if (subject == "" || subject == null)
            {
                subject = "Bitplate logingegevens";
            }

            string url = Utils.WebSessionHelper.CurrentSite.CurrentWorkingEnvironment.DomainName + "/_bitplate/";

            //foreach (LicenseSite site in this.GetSites())
            //{
            //    if (url != "")
            //    {
            //        url += ", of ";
            //    }
            //    url += String.Format("<a href='{0}'>{0}</a>", site.Url);
            //}
            string passwordEmail = passwordEmailTemplate;
            passwordEmail = passwordEmail.Replace("[naam]", this.Name);
            passwordEmail = passwordEmail.Replace("[gebruikersnaam]", this.Email);
            passwordEmail = passwordEmail.Replace("[wachtwoord]", password);
            passwordEmail = passwordEmail.Replace("[name]", this.Name);
            passwordEmail = passwordEmail.Replace("[username]", this.Email);
            passwordEmail = passwordEmail.Replace("[email]", this.Email);
            passwordEmail = passwordEmail.Replace("[password]", password);

            passwordEmail = passwordEmail.Replace("[url]", url);
            return Utils.EmailManager.SendMail(emailFrom, this.Email, subject, passwordEmail, true);
        }
    }

}
