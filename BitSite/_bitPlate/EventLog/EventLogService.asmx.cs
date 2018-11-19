using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.IO;

using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.Modules;
using Ionic.Zip;
using Ionic;
using BitPlate.Domain.Utils;

namespace BitSite._bitPlate.EventLog
{
    [GenerateScriptType(typeof(BitPlate.Domain.Logging.EventLog))]
    [System.Web.Script.Services.ScriptService]
    public partial class EventLogService : BaseService
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            
           
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<BitPlate.Domain.Logging.EventLog> GetAllErrors(int pager, int recordLimit, string orderby, string searchstring)
        {
            string sql = "FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "'";
            if (searchstring != "") {
                sql += " AND CONCAT(Description, Type, Name, CreateDate, username) LIKE '%" + searchstring + "%'";
            }
            BaseCollection<BitPlate.Domain.Logging.EventLog> logs = BaseCollection<BitPlate.Domain.Logging.EventLog>.Get(sql, orderby, pager, recordLimit);
            return logs;
        }
    }
}