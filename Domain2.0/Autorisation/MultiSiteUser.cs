using HJORM;
using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BitPlate.Domain.Autorisation
{
    [DataConnection("licenseDB")]
    public class MultiSiteUser: BaseUser
    {

        public string Theme { get; set; }

        public string Language { get; set; }

        public bool IsSystemValue { get; set; }

        public bool IsMultiSiteUser { get; set; }
        //mag alles
        public bool IsAdmin { get; set; }

        public string TempLoginKey { get; set; }

        public DateTime? TempLoginKeyCreateDate { get; set; }

        private const int TEMPKEYMINUTESVALID = 1;


        //public List<string> GetSiteUrls() {
        //    List<string> returnValue = new List<string>();
        //    foreach (Licenses.LicensedEnvironment site in Sites)
        //    {
        //        returnValue.Add(site.Url);
        //    }
        //    return returnValue;
        
        //}

        BaseCollection<Licenses.LicensedEnvironment> _sites; 
        [Persistent("SitePerUser")]
        [Association("FK_User", "FK_Environment")]
        //[DataMember]
        [IgnoreDataMember]
        public BaseCollection<Licenses.LicensedEnvironment> Sites
        {
            get
            {
                if (_sites == null)
                {
                    string where = "EXISTS (SELECT * FROM SitePerUser WHERE FK_User = '" + this.ID + "' AND LicensedEnvironment.ID = SitePerUser.FK_Environment)";
                    _sites = BaseCollection<Licenses.LicensedEnvironment>.Get(where, "Name");

                }
                return _sites;
            }
            set
            {
                _sites = value;
            }
        }

        public static MultiSiteUser CreateFromBitplateUser(BitplateUser bitplateUser)
        {
            MultiSiteUser newUser = new MultiSiteUser();
            newUser.Email = bitplateUser.Email;
            newUser.Password = bitplateUser.Password;
            newUser.PasswordLastChanged = bitplateUser.PasswordLastChanged;
            newUser.ModifiedDate = bitplateUser.ModifiedDate;
            newUser.Name = bitplateUser.Name;
            newUser.ForeName = bitplateUser.ForeName;
            newUser.NamePrefix = bitplateUser.NamePrefix;
            newUser.Gender = bitplateUser.Gender;
            newUser.BirthDate = bitplateUser.BirthDate;
            newUser.Active = bitplateUser.Active;
            newUser.DateFrom = bitplateUser.DateFrom;
            newUser.DateTill = bitplateUser.DateTill;
            newUser.Address = bitplateUser.Address;
            newUser.Postalcode = bitplateUser.Postalcode;
            newUser.City = bitplateUser.City;
            newUser.Country = bitplateUser.Country;
            newUser.Telephone = bitplateUser.Telephone;
            newUser.MessageHeader = bitplateUser.MessageHeader;
            newUser.Theme = bitplateUser.Theme;
            newUser.Language = bitplateUser.Language;

            return newUser;
        }

        public static MultiSiteUser LoadFromBitplateUser(BitplateUser bitplateUser)
        {
            string email = bitplateUser.Email;
            string password = bitplateUser.Password;
            MultiSiteUser user = BaseObject.GetFirst<MultiSiteUser>("Email ='" + email + "'");
            
            return user;
        }

        public string GenenateAndSaveTemporaryKey()
        {
            this.TempLoginKey = Utils.RandomPasswordGenerator.Generate(25);
            this.TempLoginKeyCreateDate = DateTime.Now;
            this.Save();

            return this.TempLoginKey;
        }

        public static MultiSiteUser LoadFromTempKey(string key)
        {
           
            MultiSiteUser user = BaseObject.GetFirst<MultiSiteUser>("TempLoginKey ='" + key + "'");
            if (user != null)
            {
                //voor de zekerheid: tempkey weer leeg maken
                //user.clearTempKey();

                if (DateTime.Now > user.TempLoginKeyCreateDate.GetValueOrDefault().AddMinutes(TEMPKEYMINUTESVALID))
                {
                    user = null; 
                }
            }
            

            return user;
            
        }

        private void clearTempKey()
        {
            this.TempLoginKey = "";
            this.TempLoginKeyCreateDate = null;
            this.Save();
        }

        public void FromBitplateUser(BitplateUser bitplateUser)
        {
            
            this.Email = bitplateUser.Email;
            this.Password = bitplateUser.Password;
            this.PasswordLastChanged = bitplateUser.PasswordLastChanged;
            this.ModifiedDate = bitplateUser.ModifiedDate;
            this.Name = bitplateUser.Name;
            this.ForeName = bitplateUser.ForeName;
            this.NamePrefix = bitplateUser.NamePrefix;
            this.Gender = bitplateUser.Gender;
            this.BirthDate = bitplateUser.BirthDate;
            this.Active = bitplateUser.Active;
            this.DateFrom = bitplateUser.DateFrom;
            this.DateTill = bitplateUser.DateTill;
            this.Address = bitplateUser.Address;
            this.Postalcode = bitplateUser.Postalcode;
            this.City = bitplateUser.City;
            this.Country = bitplateUser.Country;
            this.Telephone = bitplateUser.Telephone;
            this.MessageHeader = bitplateUser.MessageHeader;
            this.Theme = bitplateUser.Theme;
            this.Language = bitplateUser.Language;

          
        }

        public BitplateUser ToBitPlateUser(string email)
        {
            BitplateUser localUser = BaseObject.GetFirst<BitplateUser>("Email='" + email + "'");
            if (localUser == null)
            {
                localUser = new BitplateUser();
            }
            localUser.IsMultiSiteUser = true;
           
            localUser.Name = this.Name;
            localUser.Email = this.Email;
            localUser.Password = this.Password;
            localUser.PasswordLastChanged = this.PasswordLastChanged;
            localUser.ModifiedDate = this.ModifiedDate;
            localUser.Name = this.Name;
            localUser.ForeName = this.ForeName;
            localUser.NamePrefix = this.NamePrefix;
            localUser.Gender = this.Gender;
            localUser.BirthDate = this.BirthDate;
            localUser.Active = this.Active;
            localUser.DateFrom = this.DateFrom;
            localUser.DateTill = this.DateTill;
            localUser.Address = this.Address;
            localUser.Postalcode = this.Postalcode;
            localUser.City = this.City;
            localUser.Country = this.Country;
            localUser.Telephone = this.Telephone;
            localUser.MessageHeader = this.MessageHeader;
            localUser.Theme = this.Theme;
            localUser.Language = this.Language;
            return localUser;
        }

        public void SetEnvironment(string domainName)
        {
            //kijken of al bestaat
            bool exists = false;
            foreach (Licenses.LicensedEnvironment env in this.Sites)
            {
                if (env.DomainName == domainName)
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                Licenses.LicensedEnvironment envToAdd = BaseObject.GetFirst<Licenses.LicensedEnvironment> ("DomainName = '" + domainName + "'");
                if (envToAdd != null)
                {
                    this.Sites.Add(envToAdd);
                }
            }
        }

        public override bool SendNewPasswordEmail()
        {
            string password = Utils.RandomPasswordGenerator.Generate(8);
            this.Password = CalculateMD5Hash(password);
            this.PasswordLastChanged = DateTime.Now;
            this.Save();

            string passwordEmailTemplate = "";
            string emailFrom = "";
            string subject = "";

            if (passwordEmailTemplate == "" || passwordEmailTemplate == null)
            {
                passwordEmailTemplate = @"Beste [name],<br/>
<br/>            
Hierbij ontvangt u de login gegevens voor Bitplate.<br/>
Urls: [urls]<br/>
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
                subject = "Bitplate multisite logingegevens";
            }



            string urls = "";
            foreach (Licenses.LicensedEnvironment env in Sites)
            {
                urls += String.Format("<a href='{0}/_bitplate/'>{0}/_bitplate/</a>, ", env.DomainName);
            }


            string passwordEmail = passwordEmailTemplate;
            passwordEmail = passwordEmail.Replace("[naam]", this.Name);
            passwordEmail = passwordEmail.Replace("[gebruikersnaam]", this.Email);
            passwordEmail = passwordEmail.Replace("[wachtwoord]", password);
            passwordEmail = passwordEmail.Replace("[name]", this.Name);
            passwordEmail = passwordEmail.Replace("[username]", this.Email);
            passwordEmail = passwordEmail.Replace("[email]", this.Email);
            passwordEmail = passwordEmail.Replace("[password]", password);

            passwordEmail = passwordEmail.Replace("[urls]", urls);
            return Utils.EmailManager.SendMail(emailFrom, this.Email, subject, passwordEmail, true);
        }
    }
}
