using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using System.Web.Script.Serialization;
using BitPlate.Domain.Utils;

namespace BitPlate.Domain.Newsletters
{
    public class NewsletterMailing: BaseDomainSiteObject
    {
        public override string Name {get; set;}
        public bool NewsletterSent { get; set; }
        public string EmailAddress { get; set; }

        private NewsletterSubscriber _Subscriber;
        [Association("FK_NewsletterSubscriber")]
        public NewsletterSubscriber Subscriber
        {
            get
            {
                if (this._Subscriber == null || !this._Subscriber.IsLoaded)
                {
                    this._Subscriber.Load();
                }
                return this._Subscriber;
            }
            set
            {
                this._Subscriber = value;
            }
        }

        private Newsletter _Newsletter;
        [ScriptIgnore]
        [Association("FK_Newsletter")]
        public Newsletter Newsletter
        {
            get
            {
                if (this._Newsletter == null)
                {
                    this._Newsletter.Load();
                }
                return this._Newsletter;
            }
            set
            {
                this._Newsletter = value;
            }
        }

        public override void Save()
        {
            if (IsNew && WebSessionHelper.CurrentLicense != null && WebSessionHelper.CurrentLicense.HasMaxNumberExceeded("newslettermailing"))
            {
                throw new Exception("Kan niet opslaan: licentie heeft maximale aantal mailings (" + WebSessionHelper.CurrentLicense.MaxNumberOfNewsletterMailings + ") overschreden.");
            }
            base.Save();
        }
    }
}
