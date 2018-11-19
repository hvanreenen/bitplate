using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

using HJORM;

using BitPlate.Domain;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Utils;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using BitPlate.Domain.Licenses;

namespace BitMetaServer.Licenses
{

    public partial class LicenseService : BaseService
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        /////////////////////////////////
        //LICENSES
        //////////////////////////////////
        #region LICENSES
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static BaseCollection<License> GetLicenses(string companyId, string sort, string searchString)
        {
            CheckLogin();
            string where = "";
            if (companyId != null && companyId != "")
            {
                where = string.Format("FK_Company = '{0}'", companyId);
            }
            if (searchString != null && searchString != "")
            {
                if (where != "") where += " AND ";
                where += String.Format(" (Code like '%{0}%' OR Name like '%{0}%' OR ServerName like '%{0}%' OR DomainNames like '%{0}%' )", searchString);
            }
            return BaseCollection<License>.Get(where, sort);
            
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static License GetLicense(string id)
        {
            CheckLogin();
            License license = null;
            if (id == null || id == "")
            {
                license = License.New();
            }
            else
            {
                license = BaseObject.GetById<License>(new Guid(id));
            }
            return license;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SaveLicense(License obj)
        {
            CheckLogin();
            
            obj.Save();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void DeleteLicense(Guid id)
        {
            CheckLogin();
            BaseObject.DeleteById<License>(id);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetStatisticsByLicense(Guid id)
        {
            CheckLogin();
            License license = BaseObject.GetById<License>(id);
            return license.RetrieveStatistics();
        }
        
        #endregion
        

        /////////////////////////////////
        //CMS ENVIRONMENTS
        //////////////////////////////////
        //#region CMS ENVIRONMENTS
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static BaseCollection<LicensedEnvironment> GetLicensedEnvironments(string licenseId)
        {
            CheckLogin();
            string where = "";
            where = string.Format("FK_License = '{0}'", licenseId);
            return BaseCollection<LicensedEnvironment>.Get(where);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static LicensedEnvironment GetLicensedEnvironment(string id)
        {
            CheckLogin();
            return BaseObject.GetById<LicensedEnvironment>(new Guid(id));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SaveLicensedEnvironment(LicensedEnvironment obj)
        {
            CheckLogin();
            obj.Save();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static LicensedEnvironment CreateNewSite(LicensedEnvironment obj)
        {


            CheckLogin();
            obj = obj.CreateNewSite();

            return obj;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void DeleteLicensedEnvironment(Guid id)
        {
            CheckLogin();
            BaseObject.DeleteById<LicensedEnvironment>(id);
        }
        //#endregion

        /////////////////////////////////
        //COMPANIES & RESELLERS
        //////////////////////////////////
        #region COMPANIES
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static BaseCollection<Company> GetResellers(string sort, string searchString)
        {
            CheckLogin();
            string where = "FK_Company Is Null";
            return BaseCollection<Company>.Get(where, sort);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static BaseCollection<Company> GetCompanies(string resellerId, string sort, string searchString)
        {
            CheckLogin();
            string where = "FK_Company Is Not Null";
            if (resellerId != null && resellerId != "")
            {
                where = string.Format("FK_Company = '{0}'", resellerId);
            }
            return BaseCollection<Company>.Get(where, sort);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Company GetCompany(string id)
        {
            CheckLogin();
            Company company = null;
            if (id == null || id == "")
            {
                company = new Company();
            }
            else
            {
                company = BaseObject.GetById<Company>(new Guid(id));
            }
            return company;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SaveCompany(Company obj)
        {
            CheckLogin();
            obj.Save();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void DeleteCompany(Guid id)
        {
            CheckLogin();
            BaseObject.DeleteById<Company>(id);
        }
        #endregion



    }
}