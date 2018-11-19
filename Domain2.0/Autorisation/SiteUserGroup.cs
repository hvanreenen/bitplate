using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM.Attributes;
using System.Web.Script.Serialization;

namespace BitPlate.Domain.Autorisation
{
    public enum SiteUserGroupType {
        NewsletterSubscribersGroup = 10,
        WebshopCustomersGroup = 20,
        AutorisedUsersGroup = 30,
        CustomGroup = 100
    }

    
    public class SiteUserGroup : BaseUserGroup
    {
        private CmsSite _site;
        [System.Xml.Serialization.XmlIgnore()]
        [System.Web.Script.Serialization.ScriptIgnore()]
        [Association("FK_Site")]
        public CmsSite Site
        {
            get
            {
                _site = (CmsSite)System.Web.HttpContext.Current.Session["CurrentSite"];
                return _site;
            }
            set { _site = value; }
        }

        public SiteUserGroupType Type { get; set; }
        [NonPersistent()]
        public string TypeString
        {
            get { return Type.ToString();}
        }
        public bool IsPublicVisible { get; set; }

        [NonPersistent()]
        public string CompleteName
        {
            get
            {
                //if (this.Site != null)
                //{
                //    return String.Format("{0} @ {1}", this.Name, this.Site.Name);
                //}
                //else
                //{
                //    return String.Format("{0} @ SERVER", this.Name);
                //}

                return this.Name;
            }
        }
    }
}
