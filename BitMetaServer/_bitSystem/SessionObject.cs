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
namespace BitMetaServer
{

    public class SessionObject
    {

        public static MetaServerUser CurrentUser
        {
            get
            {
                MetaServerUser returnValue = null;
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        returnValue = (MetaServerUser)HttpContext.Current.Session["CurrentUser"];
                    }
                    if (returnValue == null)
                    {
                        returnValue = CreateDummyUser();
                        HttpContext.Current.Session["CurrentUser"] = returnValue;
                    }
                }

                return returnValue;
            }
            set
            {
                HttpContext.Current.Session["CurrentUser"] = value;
            }
        }

        private static MetaServerUser CreateDummyUser()
        {
            string checkDevelopMode = ConfigurationManager.AppSettings["DevelopMode"];
            if (checkDevelopMode == "qweutyrqwe81238761238917263876123128376123")
            {
                MetaServerUser user = new MetaServerUser();
                user.ID = new Guid("afbc2811-2487-4c01-83b2-b9479f82a99d");
                user.Name = "Dummy";
                user.Email = "dummy@bitplate.com";
                
                
                user.Theme = "bitplate";
                user.Language = "NL";
                
                
                return user;
            }
            else
            {
                return null;
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
            set
            {
                HttpContext.Current.Session["Error"] = value;
            }
        }



        public static bool HasPermission(FunctionalityEnum funcEnum)
        {
            return HasPermission(funcEnum, true);
        }

        public static bool HasPermission(FunctionalityEnum funcEnum, bool checkLicenseAlso)
        {
            
            bool returnValue = false;
            
            

                returnValue = CurrentUser.HasPermission(funcEnum);
            
            return returnValue;

        }
       
        
    }
}