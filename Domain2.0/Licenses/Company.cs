using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using HJORM;
using HJORM.Attributes;
using System.Drawing;
using System.Web;
using System.IO;
using System.Drawing.Imaging;

namespace BitPlate.Domain.Licenses
{
    [DataConnection("licenseDB")]
    
    public class Company : BaseDomainObject
    {
        public Company()
        {
            PasswordEmailTemplate = @"Beste [name],<br/>
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
        public string Address { get; set; }
        public string Postalcode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CompanyName { get; set; }
        public string Website { get; set; }
        public string Telephone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string PasswordEmailTemplate { get; set; }
        public string PasswordEmailFrom { get; set; }
        public string PasswordEmailSubject { get; set; }

        public bool IsReseller { get; set; }
        //[System.Xml.Serialization.XmlIgnore()]
        [NonPersistent()]
        public string Type
        {
            get
            {
                if (Reseller != null)
                {
                    return "Reseller";
                }
                else
                {
                    return "Company";
                }
            }
            private set { }
        }
        private Company _reseller;
        [Association("FK_Company")]
        public Company Reseller
        {
            get
            {
                if (_reseller != null && !_reseller.IsLoaded)
                {
                    _reseller.Load();
                }
                return _reseller;
            }
            set
            {
                _reseller = value;
            }
        }
        [NonPersistent()]
        public string Logo
        {
            get
            {
                //string licenseServerUrl = ConfigurationManager.AppSettings["LicenseHost"];
                if (Name != null)
                {
                    return  "/_img/ResellerLogos/" + this.Name.ToLower() + ".png";
                }
                else return "";
            }
        }
        [NonPersistent]
        [System.Xml.Serialization.XmlIgnore()]
        private byte[] _LogoBitmap;
        [NonPersistent]
        [System.Xml.Serialization.XmlIgnore()]
        public byte[] LogoBitmap
        {
            get
            {
                if (this._LogoBitmap == null)
                {
                    /* MemoryStream stream = new MemoryStream();
                    string imgPath = HttpContext.Current.Server.MapPath("") + "\\..\\_img\\ResellerLogos\\" + this.Name.ToLower() + ".png";
                    if (!File.Exists(imgPath))
                    {
                        imgPath = HttpContext.Current.Server.MapPath("") + "\\..\\_img\\ResellerLogos\\bitnetmedia.png";
                    }
                    Bitmap bitmap = new Bitmap(imgPath);
                    bitmap.Save(stream, bitmap.RawFormat);
                    this._LogoBitmap = stream.GetBuffer(); */
                }
                return this._LogoBitmap;
            }
        }

        public override void Save()
        {
            bool isNew = this.IsNew;
            this.IsReseller = (Reseller == null);
            //if(!(this.Website.StartsWith("http://") || this.Website.StartsWith("https://"))){
            if (this.Website != null && !(this.Website.StartsWith("http://") || this.Website.StartsWith("https://")))
            {
                this.Website = "http://" + this.Website;
            }
            base.Save();
             
            // Logging.EventLog.LogSaveEvent(this);

        }
       
    }
}
