using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Utils;
using BitPlate.Domain.Logging;
using System.Xml.Serialization;

namespace BitPlate.Domain.Autorisation
{
    
    public class BaseUser: BaseDomainObject
    {
        public enum SexeEnum
        {
            Undefined, Male, Female, Company
        }

        [NonPersistent()]
        [XmlIgnore]
        public string CompleteName
        {
            get
            {
                if (this.ForeName == "" && this.NamePrefix == "")
                {
                    return this.Name;
                }
                else
                {
                    return String.Format("{0}, {1} {2}", this.Name, this.ForeName, this.NamePrefix).Trim();
                }
            }
            private set { }
        }



        public BaseUser()
        {


        }
        //[DataMember]
        public string ForeName { get; set; }
        //[DataMember]
        public string NamePrefix { get; set; }
        //public string CompanyName { get; set; }
        //[DataMember]
        public string Email { get; set; }
        //[DataMember]
        public string Telephone { get; set; }
        ////[DataMember]
        public string Address { get; set; }
        ////[DataMember]
        public string Postalcode { get; set; }
        ////[DataMember]
        public string City { get; set; }
        ////[DataMember]
        public string Country { get; set; }
        public SexeEnum Gender { get; set; }
        [NonPersistent()]
        public string GenderString { 
            get {
                //Name() method van enum = extension method in Utils.TypeExtensions.cs
                return Gender.Name(); 
            } 
        }
        /// <summary>
        /// Briefhoofd
        /// </summary>
        public string MessageHeader { get; set; }
        public DateTime? BirthDate { get; set; }
        
        ////[DataMember]
        public string Password { get; set; }
        public DateTime PasswordLastChanged { get; set; }
       

        public static T Login<T>(string email, string password) where T : BaseUser, new()
        {
            try
            {
                string temp = CalculateMD5Hash("welkom");
                T user = null;
                bool loggedin = false;

                user = BaseObject.GetFirst<T>("Email = '" + email + "'");

                if (user.IsActive && user.IsValidPassword(password))
                {
                    loggedin = true;
                }

                if (loggedin)
                {
                    //EventLog.LogLoggedIn(user);
                    return user;
                }
                else
                {
                    EventLog.LogLoginFailure(email);
                    return null;
                }
            }
            catch (Exception ex)
            {
                EventLog.LogLoginFailure(email);
                return null;
            }
        }

        protected bool IsValidPassword(string password)
        {
            string md5 = CalculateMD5Hash(password);
            return (md5 == this.Password);
        }

        public static string CalculateMD5Hash(string input)
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
            return sb.ToString();
        }

        public override void Save()
        {
            bool isNew = IsNew;
            if (Name == null || Name == "")
            {
                throw new Exception("Naam is verplicht");
            }
            else if (Email == null || Email == "")
            {
                throw new Exception("Email is verplicht");
            }
            else if (!(Email.Contains("@") && Email.Contains(".")))
            {
                throw new Exception("Geen geldige email");
            }

            base.Save();

            if (isNew)
            {
                SendNewPasswordEmail();
            }
        }

        public override void Delete()
        {
            //if (IsSystemValue)
            //{
            //    throw new Exception(Resources.Messages.Instance.UserDeleteSystemUser);
            //}
            base.Delete();
        }


        public virtual bool SendNewPasswordEmail()
        {
            return false;
        }

        public virtual bool IsUserMemberOf(string groupName)
        {
            return false;
        }

        public void ChangePassword(string currentPassword, string newPassword)
        {
            if (!IsValidPassword(currentPassword))
            {
                throw new Exception("Huidige wachtwoord niet geldig");
            }
            this.Password = CalculateMD5Hash(newPassword);
            this.Save();
        }

        public void SetPassword(string password)
        {
            this.Password = CalculateMD5Hash(password);
        }

        //public CmsBitplateUser ConvertToType()
        //{
        //    CmsBitplateUser returnvalue = this;
        //    if (this.Role.RoleType == RoleEnum.WebshopCustomers)
        //    {
        //        DataCollections.Webshop.Customer customer = BaseObject.GetById<DataCollections.Webshop.Customer>(this.ID);
        //        returnvalue = customer;
        //    }
        //    return returnvalue;
        //}

        //public void UpdateAllUsersWithSameEmail()
        //{
        //    //naw gegevens van alle gebruikers met hetzelfde emailadres gelijk trekken
        //    //BaseCollection<CmsUser> users = BaseCollection<CmsUser>.Get("Email = '" + this.Email + "'");
        //    string where = " EXISTS (SELECT * FROM role WHERE user.FK_Role = role.ID AND role.RoleType IN(2,3)) AND Email = '" + this.Email + "' ";

        //    BaseCollection<CmsBitplateUser> users = BaseCollection<CmsUser>.Get(where);
        //    foreach (CmsBitplateUser user in users)
        //    {
        //        if (!user.Equals(this) && !user.Role.IsClientRol)
        //        {
        //            user.Name = this.Name;
        //            user.NamePrefix = this.NamePrefix;
        //            user.Password = this.Password;
        //            user.Plaats = this.Plaats;
        //            user.PostCode = this.PostCode;
        //            user.Sexe = this.Sexe;
        //            user.Telefoon = this.Telefoon;
        //            user.Adres = this.Adres;
        //            user.CompanyName = this.CompanyName;
        //            user.ForeName = this.ForeName;
        //            user.GeboorteDatum = this.GeboorteDatum;
        //            user.Land = this.Land;
        //            user.Save();
        //        }
        //    }
        //}

        public static T GetUserOrNewByEmail<T>(string email) where T: BaseUser, new()
        {
            T user = new T();
            user.Email = email;
            BaseCollection<T> users = BaseCollection<T>.Get("Email = '" + email + "'");
            if (users.Count == 1)
            {
                user = users[0];
            }
            return user;

        }

       
        
       
    }

}
