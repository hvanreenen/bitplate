using System;
using System.Collections.Generic;

using System.Text;
using System.Web;

using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Modules;

namespace BitPlate.Domain.Logging
{
    public class EventLog : BaseObject
    {
        private CmsSite _site;

        public DateTime Date {get; set;}
        public String Description{get; set;}
        public String UserName{get; set;}
        public String Type { get; set; }
        public bool Failure { get; set; }
        
        [Association("FK_Site")]
        public CmsSite Site
        {
            get
            {
                if (_site == null)
                {
                    //_site = BaseObject.LoadBaseObjectLazy<CmsSite>(this, "FK_Site");
                }
                return _site;
            }
            set { _site = value; }
        }

        public EventLog()
        {
            Date = DateTime.Now;
        }

        public static void LogLoggedIn(BitplateUser user)
        {
            EventLog log = new EventLog();
            log.Description = "Ingelogd: "  + user.Email;
            log.Type = "Login";
            log.UserName = user.Email;
            //log.Site = user.Site;
            log.Save();
        }

        public static void LogLoginFailure(string userName)
        {
            EventLog log = new EventLog();
            log.Description = "Login fout: " + userName;
            log.Type = "Login";
            log.Failure = true;
            log.Save();
        }


        //internal static void LogLicenceCheck(Licencing.Licence licence)
        //{
        //    EventLog log = new EventLog();
        //    //deze niet vertalen --> komen vanuit LicenceServer!
        //    log.Description = "Licentie controle. Klant: " + licence.CustomerName + ", Domain:  " + licence.DomainName + ", Code: " + licence.Code;
        //    log.Type = "Licence check";

        //    log.Save();
        //}

        internal static void LogLicenceCheckFailure(string domainName, string code, string machineName, Exception ex)
        {
            EventLog log = new EventLog();

            //deze niet vertalen --> komen vanuit LicenceServer!
            log.Description = "Licentie controle FOUT. Domain:  " + domainName + ", Code: " + code + ", MachineName: " + machineName;
            log.Description += "<br/>" + ex.Message;
            log.Type = "Licence check failure";
            log.Failure = true;


            log.Save();
        }

        internal static void LogSaveEvent(BaseDomainObject baseDomainObject)
        {
            if (//baseDomainObject.GetType() == typeof(SearchResultItem) ||
                baseDomainObject.GetType() == typeof(BaseModule))
            {
                return;
            }
            EventLog log = new EventLog();
            log.Description = "Bewaar: "  + GetObjectNameNL(baseDomainObject) + " (" + baseDomainObject.Name + ", ID=" + baseDomainObject.ID.ToString() + ")";
            if (baseDomainObject.IsNew)
            {
                log.Description += " (Nieuw)";
            }
            log.Type = "Save action";
            if (baseDomainObject.GetType() == typeof(CmsSite) && baseDomainObject.IsNew)
            {
                log.Site = null;
            }
            else
            {
                log.Site = GetSite();
            }
            log.UserName = GetUserName();
            log.Failure = false;
            log.Save();
        }

        private static string GetUserName()
        {
            string returnValue = "";
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                BaseUser user = (BaseUser)HttpContext.Current.Session["CurrentUser"];
                returnValue = user.Email;
            }
            return returnValue;
        }

        private static CmsSite GetSite()
        {
            CmsSite returnValue = null;
            if (HttpContext.Current.Session["CurrentSite"] != null)
            {
                returnValue = (CmsSite)HttpContext.Current.Session["CurrentSite"];
            }
            return returnValue;
        }

        private static string GetObjectNameNL(BaseDomainObject baseDomainObject)
        {
            string typeName = baseDomainObject.GetType().Name;
            string returnValue = typeName;
            if(typeName=="CmsSite"){
                returnValue = "Site";
            }
            else if (typeName == "CmsPage")
            {
                returnValue = "Pagina";
            }
            else if (typeName == "CmsTemplate")
            {
                returnValue = "Template";
            }
            return returnValue;
        }

        internal static void LogDeleteEvent(BaseDomainObject baseDomainObject)
        {
            EventLog log = new EventLog();
            log.Description = "Delete "  +GetObjectNameNL(baseDomainObject) + " (" + baseDomainObject.Name + ")";
            log.Site = GetSite();
            log.UserName = GetUserName();
            log.Type = "Delete action";
            log.Failure = false;
            log.Save();
        }

        public static void LogException(Exception ex, string url = ""){
            try
            {
                EventLog log = new EventLog();
                string errMsg = GetErrorMessage(ex, "");
                log.Name = url;
                log.Description = "ERROR @ " + GetSite().Name + ": " + errMsg + " STACK: " + ex.StackTrace;
                //log.Site = null;// alleen zicht baar voor server admin
                log.Site = GetSite(); //Andere mening.
                log.UserName = GetUserName();
                log.Type = "Error";
                log.Failure = true;
                log.Save();
            }
            catch (Exception err) { }
        }

        private static string GetErrorMessage(Exception ex, string msg)
        {
            if (ex.GetType() != typeof(HttpUnhandledException))
            {    
                msg += ex.Message + "; ";
            }
            if (ex.InnerException != null)
            {
                msg = GetErrorMessage(ex.InnerException, msg);
            }
            return msg;
        }
    }
}
