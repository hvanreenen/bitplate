using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Logging;
using BitPlate.Domain.Licenses;
using System.Web;
namespace BitPlate.Domain.Autorisation
{

    //public enum LicenseUserTypeEnum
    //{
    //    //CompanyAdmins = 0, //mag sites aanmaken voor sites van zijn bedrijf
    //    ResellerAdmins = 1, //mag sites aanmaken voor sites van zijn bedrijf
    //    ServerAdmins = 2, //mag alles op server, maar geen licenties
    //    LicenseAdmins = 3, //mag licenties aanmaken
    //    Developers = 9 //mag alles
    //}

    [DataConnection("licenseDB")]
    [Persistent("User")]
    public class MetaServerUser: BaseUser
    {

        //public LicenseUserTypeEnum Type {get; set;}

        public string Language { get; set; }

        public string Theme { get; set; }

        private Company _company;
        [Association("FK_Company")]
        public Company Reseller
        {
            get
            {
                if (_company != null && !_company.IsLoaded)
                {
                    _company.Load();
                }
                return _company;
            }
            set
            {
                _company = value;
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
Hierbij ontvangt u de login gegevens voor Bitplate Meta Server.<br/>
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
                subject = "Bitplate Metaserver logingegevens";
            }

            string url = HttpContext.Current.Request.Url.ToString().Replace(HttpContext.Current.Request.Url.PathAndQuery, "");
            
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

        public bool HasPermission(FunctionalityEnum func)
        {
            return true;
        }
    }
}
