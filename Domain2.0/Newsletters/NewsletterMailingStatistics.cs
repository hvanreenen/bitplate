using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace BitPlate.Domain.Newsletters
{
    public class NewsletterMailingStatistics : BaseDomainSiteObject
    {

        private Newsletter _newsletter;
        [Association("FK_Newsletter")]
        [ScriptIgnore]
        public Newsletter Newsletter
        {
            get
            {
                if (_newsletter != null && !_newsletter.IsLoaded)
                {
                    _newsletter.Load();
                }

                return _newsletter;
            }
            set { _newsletter = value; }
        }

        private NewsletterMailing _mailing;
        [Association("FK_Mailing")]
        public NewsletterMailing Mailing
        {
            get
            {
                if (_mailing != null && !_mailing.IsLoaded)
                {
                    _mailing.Load();
                }

                return _mailing;
            }
            set { _mailing = value; }
        }

        public string Url { get; set; }
        public string UserEmail { get; set; }
        public string IPAddress { get; set; }
        public string Log { get; set; }

        //niet gebruikelijk om FK's rechtstreeks in een domeinobject te stoppen, maar voor hier wel handig, want het gaat om statistieken
        public Guid FK_Page { get; set; }
        public Guid FK_User { get; set; }
        public Guid FK_Group { get; set; }
        public Guid FK_Item { get; set; }
    }
}
