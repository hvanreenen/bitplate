using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Web.Configuration;

using HJORM.Attributes;
using BitPlate.Domain.Utils;
using System.Net.Configuration;
using System.Web;

namespace BitPlate.Domain
{
    /// <summary>
    /// Enum 2 types:
    /// 1: alleen site (zonder bitplate)
    /// 0: een editable site met bitplate
    /// </summary>
    public enum SiteEnvironmentTypeEnum { Editable, PublishOnly }
    /// <summary>
    /// Site kan meerdere omgevingen hebben
    /// Elke omgeving heeft eigen database, eigen url en eigen pad op de server
    /// Ook SMTP settings zijn te overrulen
    /// </summary>
    [Persistent("SiteEnvironment")]
    public class CmsSiteEnvironment : BaseEnvironmentObject
    {

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
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["CurrentSite"] != null)
                {
                    _site = (CmsSite)System.Web.HttpContext.Current.Session["CurrentSite"];
                }
                else if (_site != null && !_site.IsLoaded)
                {
                    _site.Load();
                    //System.Web.HttpContext.Current.Session["CurrentSite"] = _site;
                }

                return _site;
            }
            set
            {

                _site = value;
                //System.Web.HttpContext.Current.Session["CurrentSite"] = value;
            }
        }


        public override void Save()
        {
            base.Save();
            if (!System.IO.Directory.Exists(Path))
            {
                System.IO.Directory.CreateDirectory(Path);
                System.IO.Directory.CreateDirectory(Path + "_files");
                System.IO.Directory.CreateDirectory(Path + "_files\\_img");
                System.IO.Directory.CreateDirectory(Path + "_css");
                System.IO.Directory.CreateDirectory(Path + "_js");

                string jQueryVersion = ConfigurationManager.AppSettings["jQueryVersion"];
                if (jQueryVersion == null || jQueryVersion == "")
                {
                    jQueryVersion = "jquery-1.8.2.js";
                }
                Utils.FileHelper.CopyFile(AppDomain.CurrentDomain.BaseDirectory + "_bitplate\\js\\" + jQueryVersion, Path + "\\js\\" + jQueryVersion);

            }
        }
        public SiteEnvironmentTypeEnum SiteEnvironmentType { get; set; }
        [NonPersistent]
        public string SiteEnvironmentTypeString
        {
            get
            {
                return this.SiteEnvironmentType.Name();
            }
        }

        public DateTime? LastPublishedDate { get; set; }
        



        public string CreateWebConfig(string bitplateHost)
        {
            string licenseHost = ConfigurationManager.AppSettings["LicenseHost"];
            return CreateWebConfig(licenseHost, bitplateHost, this.Site.ID);
        }


    }
}

