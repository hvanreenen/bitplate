using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Licenses;
namespace BitSite
{
    //public enum ModeEnum { View, Edit } 
    public sealed class SessionObject
    {


        public static BitplateUser CurrentBitplateUser
        {
            get
            {
                BitplateUser returnValue = null;
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentBitplateUser"] != null)
                    {
                        returnValue = (BitplateUser)HttpContext.Current.Session["CurrentBitplateUser"];
                    }
                    if (returnValue == null)
                    {
                        returnValue = CreateDummyUser();
                        HttpContext.Current.Session["CurrentBitplateUser"] = returnValue;
                    }
                }

                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["CurrentBitplateUser"] = value;
            }
        }

        private static BitplateUser CreateDummyUser()
        {
            string checkDevelopMode = ConfigurationManager.AppSettings["DevelopMode"];
            if (checkDevelopMode == "qweutyrqwe81238761238917263876123128376123")
            {
                BitplateUser user = new BitplateUser();
                user.ID = new Guid("afbc2811-2487-4c01-83b2-b9479f82a99d");
                user.Name = "Dummy";
                user.Email = "dummy@bitplate.com";
                //user.CurrentSite = new LicenseSite();
                ////user.SuperUserType = SuperUserTypeEnum.Developers;
                //string siteID = ConfigurationManager.AppSettings["SiteID"];
                //if (SessionObject.CurrentSite != null)
                //{
                //    user.CurrentSite = new LicenseSite();
                //    user.CurrentSite.ID = SessionObject.CurrentSite.ID;
                //}
                user.Theme = "bitplate";
                user.Language = "NL";

                BitplateUserGroup group = new BitplateUserGroup();
                
                group.Type = UserTypeEnum.Developers;
               
                user.UserGroups.Add(group);

                return user;
            }
            else
            {
                return null;
            }
        }

        internal static SiteUser CurrentSiteUser
        {
            get
            {
                SiteUser returnValue = null;
                if (HttpContext.Current.Session["CurrentSiteUser"] != null)
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
        internal static Exception Error
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
            set
            {
                HttpContext.Current.Session["Error"] = value;
            }
        }


        internal static LicenseFile CurrentLicense
        {
            get
            {
                LicenseFile returnValue = null;
                if (HttpContext.Current.Session["CurrentLicense"] != null)
                {
                    returnValue = (LicenseFile)HttpContext.Current.Session["CurrentLicense"];
                }
                else
                {
                    returnValue = LoadLicense();
                    HttpContext.Current.Session["CurrentLicense"] = returnValue;
                }

                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["CurrentLicense"] = value;
            }
        }

        internal static LicenseFile LoadLicense()
        {
             LicenseFile license = null;
            string licenseCode = "";
            if (CurrentSite != null)
            {
                licenseCode = CurrentSite.LicenceCode;
            }
            string serverName = Environment.MachineName;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            //string ipAddress = HttpContext.Current.Request.ServerVariables["remote_host"];
            string relativeUrl = HttpContext.Current.Request.Url.PathAndQuery;
            string completeUrl = HttpContext.Current.Request.Url.ToString();
            string domainName = completeUrl.Replace(relativeUrl, "");
            try
            {
                BitSite.LicServiceReference.LicServiceClient client = BitMetaServerServicesHelper.GetLicenseServiceClient();
                //BitSite.BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
                license = client.GetLicense(licenseCode, serverName, path, domainName);
                client.Close();
            }
            catch (Exception ex)
            {
            }

            if (license != null)
            {
                license.Decrypt();
                //opslaan als bestand. Er is vanuit Fred een verzoek dat licentie blijft werken, mocht lic server down zijn 
                //dan lic laden vanuit bestand. Hier laatste versie opslaan.
                //hierdoor is license niet meer hackproof...
                license.SaveAsFile();

                CurrentSite.IsValidLicense = true;
            }
            else
            {
                //lic server is down: laden vanuit bestand
                license = new LicenseFile();

                license.LoadFromFile(licenseCode);

                CurrentSite.IsValidLicense = license.IsValid;
            }
            return license;
        }

        public static CmsSite CurrentSite
        {
            get
            {
                CmsSite returnValue = null;
                if (HttpContext.Current.Session["CurrentSite"] != null)
                {
                    returnValue = (CmsSite)HttpContext.Current.Session["CurrentSite"];
                }
                else
                {
                    SetSiteObject();
                }

                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["CurrentSite"] = value;
            }
        }

        internal static void SetSiteObject()
        {
            string siteID = ConfigurationManager.AppSettings["SiteID"];
            if (siteID != null && siteID != "")
            {
                CmsSite site = BaseObject.GetById<CmsSite>(new Guid(siteID));
                if (site != null)
                {
                    site.CurrentWorkingEnvironment = SessionObject.GetWorkingEnvironment(new Guid(siteID));
                }
                HttpContext.Current.Session["CurrentSite"] = site;
            }
        }

        private static CmsSiteEnvironment GetWorkingEnvironment(Guid siteID)
        {

            CmsSiteEnvironment returnValue = null;

            string envID = ConfigurationManager.AppSettings["SiteEnvironmentID"];
            if (envID != null && envID != "")
            {
                returnValue = BaseObject.GetById<CmsSiteEnvironment>(new Guid(envID));
            }
            else
            {

                //eerste editable omgeving wordt standaard
                string where = String.Format("FK_Site='{0}' AND SiteEnvironmentType={1}", siteID, (int)SiteEnvironmentTypeEnum.Editable);
                returnValue = BaseObject.GetFirst<CmsSiteEnvironment>(where);
            }
            return returnValue;
        }

        internal static bool HasPermission(FunctionalityEnum funcEnum)
        {
            return HasPermission(funcEnum, true);
        }

        internal static bool HasPermission(FunctionalityEnum funcEnum, bool checkLicenseAlso)
        {
            //return true;
            bool returnValue = false;
            //if (CurrentBitplateUser.GetSuperUserType() == SuperUserTypeEnum.Developers)
            //{
            //    return true;
            //}
            //in sommige gevallen is er geen licentie, maar moet dashboard & siteconfig wel beschikbaar zijn,
            //namelijk om de licentiecode te zetten.
            //dan kan er dus geen check zijn op licentie (deze check zit wel in alle subpagina's)
            if (checkLicenseAlso)
            {
                if (CurrentLicense != null && CurrentBitplateUser != null)
                {
                    returnValue = (CurrentLicense.HasPermission(funcEnum) &&
                         CurrentBitplateUser.HasPermission(funcEnum));
                }
            }
            else
            {

                returnValue = CurrentBitplateUser.HasPermission(funcEnum);
            }
            return returnValue;
            //return true;
        }
        //public static CmsSite CurrentLiveSite
        //{
        //    get
        //    {
        //        CmsSite returnValue = null;
        //        if (HttpContext.Current.Session["CurrentLiveSite"] != null)
        //        {
        //            returnValue = (CmsSite)HttpContext.Current.Session["CurrentLiveSite"];
        //        }
        //        else
        //        {
        //            string siteID = ConfigurationManager.AppSettings["LiveSiteID"];
        //            returnValue = BaseDomainObject.GetById<CmsSite>(new Guid(siteID));
        //            HttpContext.Current.Session["CurrentLiveSite"] = returnValue;
        //        }

        //        return returnValue;
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["CurrentLiveSite"] = value;
        //    }
        //}
        internal static List<ModuleDefinition> AvailableModules
        {
            get
            {
                List<ModuleDefinition> returnValue = null;
                if (HttpContext.Current.Session["AvailableModules"] != null)
                {
                    returnValue = (List<ModuleDefinition>)HttpContext.Current.Session["AvailableModules"];
                }
                else
                {
                    string modulesXmlFile = String.Format("{0}\\bin\\AllModules.xml", AppDomain.CurrentDomain.BaseDirectory);

                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<ModuleDefinition>));
                    System.IO.StreamReader reader = System.IO.File.OpenText(modulesXmlFile);
                    returnValue = (List<ModuleDefinition>)serializer.Deserialize(reader);
                    reader.Close();
                    reader.Dispose();
                    HttpContext.Current.Session["AvailableModules"] = returnValue;
                }
                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["AvailableModules"] = value;
            }
        }

        //public static ModeEnum Mode
        //{
        //    get
        //    {
        //        ModeEnum returnValue = ModeEnum.View;
        //        //if (HttpContext.Current.Session != null)
        //        //{
        //        //    if (HttpContext.Current.Session["Mode"] != null)
        //        //    {
        //        //        returnValue = (ModeEnum)HttpContext.Current.Session["Mode"];
        //        //    }
        //        //}

        //        if (returnValue == ModeEnum.View && HttpContext.Current.Request.QueryString["mode"] != null && HttpContext.Current.Request.QueryString["mode"].ToLower() == "edit")
        //        {
        //            returnValue = ModeEnum.Edit;
        //            HttpContext.Current.Session["Mode"] = ModeEnum.Edit;
        //        }
        //        return returnValue;

        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["Mode"] = value;
        //    }
        //}

        //public static bool LiveMode
        //{
        //    get
        //    {
        //        if (ConfigurationManager.AppSettings["TestEnvironment"] == "True")
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return (CurrentBitplateUser == null);
        //        }
        //    }
        //}
    }
}