using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitPlate.Domain.Autorisation;
using HJORM;
using HJORM.Attributes;
using System.Web.Script.Serialization;
using BitPlate.Domain.Utils;
using System.Xml.Serialization;

namespace BitPlate.Domain.Newsletters
{
    public enum RegistrationTypeEnum { BitplateInput, Website, Import }
    public class NewsletterSubscriber : BaseObject
    {
        public string ForeName { get; set; }
        public string NamePrefix { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        //public override string Name { get; set; }

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

        public BitPlate.Domain.Autorisation.BaseUser.SexeEnum Gender { get; set; }
        [NonPersistent()]
        public string GenderString
        {
            get
            {
                //Name() method van enum = extension method in Utils.TypeExtensions.cs
                return Gender.Name();
            }
        }

        [NonPersistent]
        public bool Unsubscribed
        {
            get
            {
                return this.UnsubscribeDate.HasValue;
            }
        }
        public DateTime? UnsubscribeDate { get; set; }
        private CmsSite _site;
        /// <summary>
        /// Haal Object uit WebSession (System.Web.HttpContext.Current.Session["CurrentSite"])
        /// </summary>
        [System.Xml.Serialization.XmlIgnore()]
        [System.Web.Script.Serialization.ScriptIgnore()]
        [Association("FK_Site")]
        public CmsSite Site
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["CurrentSite"] != null)
                {
                    _site = (CmsSite)System.Web.HttpContext.Current.Session["CurrentSite"];
                }
                else if (_site != null && !_site.IsLoaded)
                {
                    _site.Load();
                }

                return _site;
            }
            set
            {
                _site = value;
                //System.Web.HttpContext.Current.Session["CurrentSite"] = value;
            }
        }

        private BaseCollection<NewsletterGroup> _subscribedGroups;
        [Persistent("newslettergroupsubscriber")]
        [Association("FK_NewsletterSubscriber", "FK_NewsletterGroup")]
        public BaseCollection<NewsletterGroup> SubscribedGroups
        {
            get
            {
                if (this._subscribedGroups == null || (_subscribedGroups != null && !_subscribedGroups.IsLoaded))
                {
                    string where = "EXISTS (SELECT * FROM newslettergroupsubscriber WHERE FK_NewsletterSubscriber = '" + this.ID + "' AND newslettergroup.ID = newslettergroupsubscriber.FK_NewsletterGroup)";
                    this._subscribedGroups = BaseCollection<NewsletterGroup>.Get(where);
                    this._subscribedGroups.IsLoaded = true;
                }
                return this._subscribedGroups;
            }
            set
            {
                this._subscribedGroups = value;
                this._subscribedGroups.IsLoaded = true;
            }
        }

        public bool Confirmed { get; set; }

        public RegistrationTypeEnum RegistrationType { get; set; }

        [NonPersistent()]
        public string RegistrationTypeString
        {
            get
            {
                if (RegistrationType == RegistrationTypeEnum.Import)
                {
                    return "Import";
                }
                else if (RegistrationType == RegistrationTypeEnum.Website)
                {
                    return "Website";
                }
                else
                {
                    return "Invoer";
                }
            }
        }
        /* private SiteUser _user;
        [Association("FK_User")]
        public SiteUser User
        {
            get
            {
                if (_user != null && !_user.IsLoaded)
                {
                    _user.Load();
                }
                return _user;
            }
            set
            {
                _user = value;
            }
        } */

        private BaseCollection<NewsletterMailing> _Mailings;
        [ScriptIgnoreAttribute]
        public BaseCollection<NewsletterMailing> Mailings
        {
            get
            {
                if (this._Mailings == null)
                {
                    this._Mailings = BaseCollection<NewsletterMailing>.Get("FK_NewsletterSubscriber = '" + this.ID + "'");
                }
                return this._Mailings;
            }
            set
            {
                this._Mailings = value;
            }
        }


        public override void Save()
        {
            /* if (this.User != null && this.User.Name.Trim() != "")
            {
                this.User.Save();
            }
            else
            {
                this.User = null;
            } */

            
            if (IsNew && WebSessionHelper.CurrentLicense != null && WebSessionHelper.CurrentLicense.HasMaxNumberExceeded("newslettersubscriber"))
            {
                throw new Exception("Kan niet opslaan: licentie heeft maximale aantal abonnees (" + WebSessionHelper.CurrentLicense.MaxNumberOfNewsletterSubscribers + ") overschreden.");
            }
            base.Save();
        }

        public void Unsubscribe()
        {
            this.Email = this.Email + "_unsubscribed_" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            //this.Unsubscribed = true;
            this.UnsubscribeDate = DateTime.Now;
            this.Save();
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            this.Email = dataRow["Email"].ToString();
            this.ForeName = dataRow["ForeName"].ToString();
            this.NamePrefix = dataRow["NamePrefix"].ToString();
            this.CompanyName = dataRow["CompanyName"].ToString();
            this.Confirmed = Convert.ToBoolean(dataRow["Confirmed"]);
            this.Gender = dataRow["Gender"] != DBNull.Value ? (BitPlate.Domain.Autorisation.BaseUser.SexeEnum)Convert.ToInt32(dataRow["Gender"]) : BaseUser.SexeEnum.Undefined;
            this.UnsubscribeDate = dataRow["UnsubscribeDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dataRow["UnsubscribeDate"]) : null;
            this.RegistrationType = dataRow["RegistrationType"] != DBNull.Value ? (RegistrationTypeEnum)Convert.ToInt32(dataRow["RegistrationType"]) : RegistrationTypeEnum.BitplateInput;
            this.IsLoaded = true;
        }

        internal bool HasGroup(NewsletterGroup checkGroup)
        {
            bool returnValue = false;
            foreach (NewsletterGroup group in this.SubscribedGroups)
            {
                if (group.Equals(checkGroup))
                {
                    returnValue = true;
                    break;
                }
            }
            return returnValue;
        }
    }
}
