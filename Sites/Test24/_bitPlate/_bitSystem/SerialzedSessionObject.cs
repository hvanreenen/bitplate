using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Utils;
namespace BitSite
{
    //public enum ModeEnum { View, Edit } 
    public class SessionObject
    {
        public static ModeEnum Mode
        {
            get
            {
                ModeEnum returnValue = ModeEnum.View;
                //if (HttpContext.Current.Session != null)
                //{
                //    if (HttpContext.Current.Session["Mode"] != null)
                //    {
                //        returnValue = (ModeEnum)HttpContext.Current.Session["Mode"];
                //    }
                //}

                if (returnValue == ModeEnum.View && HttpContext.Current.Request.QueryString["mode"] != null && HttpContext.Current.Request.QueryString["mode"].ToLower() == "edit")
                {
                    returnValue = ModeEnum.Edit;
                    HttpContext.Current.Session["Mode"] = ModeEnum.Edit.ToJsonString();
                }
                return returnValue;

            }
            set
            {
                HttpContext.Current.Session["Mode"] = value.ToJsonString();
            }
        }

        public static bool LiveMode
        {
            get
            {
                if (ConfigurationManager.AppSettings["TestEnvironment"] == "True")
                {
                    return false;
                }
                else
                {
                    return (CurrentBitplateUser == null);
                }
            }
        }

        public static BitplateUser CurrentBitplateUser
        {
            get
            {
                BitplateUser returnValue = null;
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentBitplateUser"] != null)
                    {
                        returnValue = JSONSerializer.Deserialize<BitplateUser>(HttpContext.Current.Session["CurrentBitplateUser"].ToString());
                    }
                    if (returnValue == null)
                    {
                        returnValue = CreateDummyUser();
                        HttpContext.Current.Session["CurrentBitplateUser"] = returnValue.ToJsonString();
                    }
                }

                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["CurrentBitplateUser"] = value.ToJsonString();
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
                user.CurrentSite = new LicenseSite();
                //user.SuperUserType = SuperUserTypeEnum.Developers;
                string siteID = ConfigurationManager.AppSettings["SiteID"];
                if (SessionObject.CurrentSite != null)
                {
                    user.CurrentSite = new LicenseSite();
                    user.CurrentSite.ID = SessionObject.CurrentSite.ID;
                }
                user.Theme = "bitplate";
                user.Language = "NL";

                BitplateUserGroup group = new BitplateUserGroup();
                //group.Site = new LicenseSite();
                //group.Site.ID = SessionObject.CurrentSite.ID;
                group.SuperUserGroupType = SuperUserTypeEnum.Developers;
                Functionality permission = new Functionality();
                permission.FunctionNumber = FunctionalityEnum.FileManager;
                group.UserGroupPermissions.Add(permission);
                user.UserGroups.Add(group);

                user.AllPermissions4CurrentSite = new List<int>();

                foreach (FunctionalityEnum funcEnumValue in Enum.GetValues(typeof(FunctionalityEnum)))
                {
                    user.AllPermissions4CurrentSite.Add((int)funcEnumValue);
                }
                return user;
            }
            else
            {
                return null;
            }
        }

        public static SiteUser CurrentBitSiteUser
        {
            get
            {
                SiteUser returnValue = null;
                if (HttpContext.Current.Session["CurrentBitSiteUser"] != null)
                {
                    returnValue = JSONSerializer.Deserialize<SiteUser>(HttpContext.Current.Session["CurrentBitSiteUser"].ToString());
                }

                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["CurrentBitSiteUser"] = value.ToJsonString();
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
                    returnValue = JSONSerializer.Deserialize<Exception>(HttpContext.Current.Session["Error"].ToString());
                }
                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["Error"] = value.ToJsonString();
            }
        }


        public static License CurrentLicense
        {
            get
            {
                License returnValue = null;
                if (HttpContext.Current.Session["CurrentLicense"] != null)
                {
                    returnValue = JSONSerializer.Deserialize<License>(HttpContext.Current.Session["CurrentLicense"].ToString());
                }
                else
                {
                    returnValue = LoadLicense();
                    HttpContext.Current.Session["CurrentLicense"] = returnValue.ToJsonString();
                }

                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["CurrentLicense"] = value.ToJsonString();
            }
        }

        public static License LoadLicense()
        {
            string licenseCode = "";
            if (CurrentSite != null)
            {
                licenseCode = CurrentSite.LicenceCode;
            }
            string serverName = Environment.MachineName;
            string ipAddress = HttpContext.Current.Request.ServerVariables["remote_host"];
            string relativeUrl = HttpContext.Current.Request.Url.PathAndQuery;
            string completeUrl = HttpContext.Current.Request.Url.ToString();
            string domainName = completeUrl.Replace(relativeUrl, "");

            BitSite.BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
            License returnValue = client.LoadLicense(licenseCode, serverName, domainName);
            client.Close();
            if (returnValue != null)
            {
                CurrentSite.IsValidLicense = true;
            }
            return returnValue;
        }

        public static CmsSite CurrentSite
        {
            get
            {
                CmsSite returnValue = null;
                if (HttpContext.Current.Session["CurrentSite"] != null)
                {
                    returnValue = JSONSerializer.Deserialize<CmsSite>(HttpContext.Current.Session["CurrentSite"].ToString());
                }
                else
                {
                    string siteID = ConfigurationManager.AppSettings["SiteID"];
                    //string siteEnvironmentID = ConfigurationManager.AppSettings["SiteEnvironmentID"];
                    //string publishMode = "";
                    //if (ConfigurationManager.AppSettings["PublishMode"] != null)
                    //{
                    //    publishMode = ConfigurationManager.AppSettings["PublishMode"];
                    //}

                    if (siteID != null && siteID != "")
                    {
                        returnValue = BaseObject.GetById<CmsSite>(new Guid(siteID));
                        //if (publishMode == "Editable")
                        //{
                        //    string where = String.Format("FK_Site='{0}' AND SiteEnvironmentType={1}", siteID, (int)SiteEnvironmentTypeEnum.Editable);
                        //    CmsSiteEnvironment environment = BaseObject.GetFirst<CmsSiteEnvironment>(where);
                        returnValue.CurrentWorkingEnvironment = SessionObject.GetWorkingEnvironment(new Guid(siteID));
                        //    HttpContext.Current.Session["CurrentWorkingEnvironment"] = environment;
                        //}
                        HttpContext.Current.Session["CurrentSite"] = returnValue.ToJsonString();
                        //HttpContext.Current.Session.
                    }
                }

                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["CurrentSite"] = value.ToJsonString();
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

        public static bool HasPermission(FunctionalityEnum funcEnum)
        {
            return HasPermission(funcEnum, true);
        }

        public static bool HasPermission(FunctionalityEnum funcEnum, bool checkLicenseAlso)
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
        public static List<ModuleDefinition> AvailableModules
        {
            get
            {
                List<ModuleDefinition> returnValue = null;
                if (HttpContext.Current.Session["AvailableModules"] != null)
                {
                    returnValue = JSONSerializer.Deserialize<List<ModuleDefinition>>(HttpContext.Current.Session["AvailableModules"].ToString());
                }
                else
                {
                    string modulesXmlFile = String.Format("{0}\\_bitPlate\\EditPage\\Modules\\AllModules.xml", AppDomain.CurrentDomain.BaseDirectory);

                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<ModuleDefinition>));
                    System.IO.StreamReader reader = System.IO.File.OpenText(modulesXmlFile);
                    returnValue = (List<ModuleDefinition>)serializer.Deserialize(reader);
                    reader.Close();

                    HttpContext.Current.Session["AvailableModules"] = returnValue.ToJsonString();
                }
                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["AvailableModules"] = value.ToJsonString();
            }
        }
    }
}