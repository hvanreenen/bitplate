using BitPlate.Domain.News;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace BitMetaServer20.News
{
    /// <summary>
    /// Summary description for NewsService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class NewsService : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<NewsItem> GetNewsMessages()
        {
            return BaseCollection<NewsItem>.Get();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public NewsItem GetNewsMessage(Guid ID)
        {
            NewsItem returnValue;

            if (ID == null || ID == Guid.Empty)
            {
                returnValue = new NewsItem();
                returnValue.ID = Guid.Empty;
                returnValue.BitplateVersion = "ALL";
                returnValue.CreateDate = DateTime.Now;
            }
            else
            {
                returnValue = BaseObject.GetById<NewsItem>(ID);
            }
            return returnValue;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public bool SaveNewsMessage(NewsItem obj)
        {
            obj.Save();
            return true;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public bool DeleteNewsMessage(Guid ID)
        {
            NewsItem news = BaseObject.GetById<NewsItem>(ID);
            if (news != null)
            {
                news.Delete();
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}
