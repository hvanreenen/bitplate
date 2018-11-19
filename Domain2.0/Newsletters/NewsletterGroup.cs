using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using System.Web.Script.Serialization;
using System.IO;
using System.Web;

namespace BitPlate.Domain.Newsletters
{
    public class NewsletterGroup : BaseDomainPublishableObject
    {
        public bool IsChoosableGroup { get; set; }
        public bool IsMandatoryGroup { get; set; }

        private BaseCollection<Newsletter> _Newsletters;
        [Persistent("newsletterpergroup")]
        [Association("FK_NewsletterGroup", "FK_Newsletter")]
        [ScriptIgnoreAttribute]
        public BaseCollection<Newsletter> Newsletters
        {
            get
            {
                if (this._Newsletters == null)
                {
                    string where = "EXISTS (SELECT * FROM newsletterpergroup WHERE FK_NewsletterGroup = '" + this.ID + "' AND newsletter.ID = newsletterpergroup.FK_Newsletter)"; 
                    this._Newsletters = BaseCollection<Newsletter>.Get(where); //ORDERBY
                    this._Newsletters.IsLoaded = true;
                }
                return this._Newsletters;
            }
            set
            {
                this._Newsletters = value;
                this.IsLoaded = true;
            }
        }

        private BaseCollection<NewsletterSubscriber> _Subscribers;
        [ScriptIgnoreAttribute]
        [Persistent("newslettergroupsubscriber")]
        [Association("FK_NewsletterGroup", "FK_NewsletterSubscriber")]
        public BaseCollection<NewsletterSubscriber> Subscribers
        {
            get
            {
                if (this._Subscribers == null)
                {
                    string where = "EXISTS (SELECT * FROM newslettergroupsubscriber WHERE FK_NewsletterGroup = '" + this.ID + "' AND newslettersubscriber.ID = newslettergroupsubscriber.FK_NewsletterSubscriber)";
                    this._Subscribers = BaseCollection<NewsletterSubscriber>.Get(where);
                }
                return this._Subscribers;
            }
            set
            {
                this._Subscribers = value;
                this.IsLoaded = true;
            }
        }

        public override void Save()
        {
            BaseCollection<NewsletterGroup> selectedGroups = BaseCollection<NewsletterGroup>.Get("FK_Site='" + this.Site.ID.ToString() + "' AND Name='" + this.Name + "'");
            //if (selectedGroups.Count > 0)
            //{
            //    throw new Exception("Kan de nieuwsgroep niet opslaan, omdat er al een groep bestaat onder deze naam.");
            //}

            base.Save();
            //Directory.CreateDirectory(HttpContext.Current.Server.MapPath("") + "\\Newsletters\\" + this.Name);
        }

        public override void Delete()
        {
            base.Delete();
            //Directory.Delete(HttpContext.Current.Server.MapPath("") + "\\Newsletters\\" + this.Name, true);
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            this.IsChoosableGroup = Convert.ToBoolean(dataRow["IsChoosableGroup"]);
            this.IsMandatoryGroup = Convert.ToBoolean(dataRow["IsMandatoryGroup"]);
            this.IsLoaded = true;
        }
    }
}
