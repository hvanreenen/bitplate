using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;

using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Licenses;

namespace BitPlate.Domain.Utils
{
    public static class WebSessionHelper
    {
        public static BitplateUser CurrentBitplateUser
        {
            get
            {
                BitplateUser returnValue = null;
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentBitplateUser"] != null)
                {
                    returnValue = (BitplateUser)HttpContext.Current.Session["CurrentBitplateUser"];
                }
                return returnValue;
            }
            //set
            //{
            //    HttpContext.Current.Session["CurrentBitplateUser"] = value;
            //}
        }

        public static SiteUser CurrentSiteUser
        {
            get
            {
                SiteUser returnValue = null;
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentSiteUser"] != null)
                {
                    returnValue = (SiteUser)HttpContext.Current.Session["CurrentSiteUser"];
                }
                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["CurrentSiteUser"] = value;
            }
        }
        
        /// <summary>
        /// laatste fout
        /// </summary>
        public static Exception Error
        {
            get
            {
                Exception returnValue = null;
                if (HttpContext.Current.Session["Error"] != null)
                {
                    returnValue = (Exception)HttpContext.Current.Session["Error"];
                }
                return returnValue;
            }
            //set
            //{
            //    HttpContext.Current.Session["Error"] = value;
            //}

        }

        public static CmsSite CurrentSite
        {
            get
            {
                CmsSite returnValue = null;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentSite"] != null)
                {
                    returnValue = (CmsSite)HttpContext.Current.Session["CurrentSite"];
                }
                //else
                //{
                //    string siteID = ConfigurationManager.AppSettings["SiteID"];
                //    returnValue = BaseDomainObject.GetById<CmsSite>(new Guid(siteID));
                //    HttpContext.Current.Session["CurrentSite"] = returnValue;
                //}

                return returnValue;
            }
            //set
            //{
            //    HttpContext.Current.Session["CurrentWorkingSite"] = value;
            //}
        }

        public static CmsSiteEnvironment CurrentWorkingEnvironment
        {
            get
            {
                CmsSiteEnvironment returnValue = null;
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentWorkingEnvironment"] != null)
                {
                    returnValue = (CmsSiteEnvironment)HttpContext.Current.Session["CurrentWorkingEnvironment"];
                }
                //else
                //{
                //    string siteID = ConfigurationManager.AppSettings["SiteID"];
                //    if (siteID != null && siteID != "")
                //    {
                //        string where = String.Format("FK_Site='{0}' AND SiteEnvironmentType={1}", siteID, (int)SiteEnvironmentTypeEnum.Editable);
                //        CmsSiteEnvironment environment = BaseObject.GetFirst<CmsSiteEnvironment>(where);
                //        returnValue = environment;
                //        HttpContext.Current.Session["CurrentWorkingEnvironment"] = returnValue;
                //    }
                //}

                return returnValue;
            }
            //set
            //{
            //    HttpContext.Current.Session["CurrentWorkingEnvironment"] = value;
            //}
        }


        public static LicenseFile CurrentLicense
        {
            get
            {
                LicenseFile returnValue = null;
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentLicense"] != null)
                {
                    returnValue = (LicenseFile)HttpContext.Current.Session["CurrentLicense"];
                }

                return returnValue;
            }
            
        }

        public static List<ModuleDefinition> AvailableModules
        {
            get
            {
                List<ModuleDefinition> returnValue = null;
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["AvailableModules"] != null)
                {
                    returnValue = (List<ModuleDefinition>)HttpContext.Current.Session["AvailableModules"];
                }
                else
                {
                    //string modulesXmlFile = String.Format("{0}\\_bitPlate\\Editpage\\Modules\\AllModules.xml", AppDomain.CurrentDomain.BaseDirectory);
                    string modulesXmlFile = String.Format("{0}\\bin\\AllModules.xml", AppDomain.CurrentDomain.BaseDirectory);

                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<ModuleDefinition>));
                    System.IO.StreamReader reader = System.IO.File.OpenText(modulesXmlFile);
                    returnValue = (List<ModuleDefinition>)serializer.Deserialize(reader);
                    reader.Close();

                    HttpContext.Current.Session["AvailableModules"] = returnValue;
                }
                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["AvailableModules"] = value;
            }
        }
       
    }
}
