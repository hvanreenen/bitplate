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
using BitPlate.Domain.Search;
namespace BitSite._services
{
    public partial class SearchService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<SearchResultItem> Find(string searchString, string sort, int page, int pagesize, bool editMode)
        {
            //mysql only!!!
            
            if (sort == "")
            {
                //url en titel tellen zwaarder in score, sorteer op score desc (hoogste eerst)
                sort = string.Format("((MATCH(Url,Title) AGAINST('{0}*' IN BOOLEAN MODE)) + (MATCH(Url,Title, MetaDescription, MetaKeywords, Content) AGAINST('{0}*' IN BOOLEAN MODE)))/2 DESC", searchString); 
            }
            string sql = string.Format(@"SELECT *, ((MATCH(Url,Title) AGAINST('{0}*' IN BOOLEAN MODE)) + (MATCH(Url,Title, MetaDescription, MetaKeywords, Content) AGAINST('{0}*' IN BOOLEAN MODE)))/2 AS Score
                FROM SearchIndex 
                WHERE  MATCH(Url,Title,MetaDescription, MetaKeywords,Content) AGAINST('{0}*' IN BOOLEAN MODE)
                ORDER BY {2}", searchString, SessionObject.CurrentSite.ID, sort);
            if (pagesize > 0)
            {
                if (page <= 0) page = 1;
                page--;
                sql += @" LIMIT " + page * pagesize + @", " + pagesize;
            }
            System.Data.DataTable table = DataBase.Get().GetDataTable(sql);
            List<SearchResultItem> foundItems = new List<SearchResultItem>();
           foreach(System.Data.DataRow DataControlRowState in table.Rows){
               SearchResultItem item = new SearchResultItem();
               item.Name = DataControlRowState["Name"].ToString();
               item.Title = DataControlRowState["Title"].ToString();
               item.Content = DataControlRowState["Content"].ToString();
               item.Score = Convert.ToDouble(DataControlRowState["Score"]);
               item.MetaDescription = DataControlRowState["MetaDescription"].ToString();
               item.MetaKeywords = DataControlRowState["MetaKeywords"].ToString();
               item.Url = DataControlRowState["Url"].ToString();
               //item.FK_Page = new Guid(DataControlRowState["FK_Page"].ToString());
               //if (editMode)
               //{
               //    item.Url =  "CKEditPage.aspx#" + item.FK_Page.ToString();
               //}
               foundItems.Add(item);
           }

            return foundItems;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static int GetTotalCount(string searchString)
        {
            string where = "FK_Site = '" + SessionObject.CurrentSite.ID.ToString() + "' AND  MATCH(Url,Title,Meta,Content) AGAINST('" + searchString + "*' IN BOOLEAN MODE)";
            string sql = string.Format(@"SELECT COUNT(*) as Count
                FROM SearchIndex 
                WHERE  FK_Site = '{1}' AND  MATCH(Url,Title,Meta,Content) AGAINST('{0}*' IN BOOLEAN MODE)
                ", searchString, SessionObject.CurrentSite.ID);

            System.Data.DataTable table = DataBase.Get().GetDataTable(sql);
            return Convert.ToInt32(table.Rows[0]["Count"]);
        }
    }
}