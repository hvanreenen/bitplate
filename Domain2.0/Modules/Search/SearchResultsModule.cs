using BitPlate.Domain.Utils;
using HJORM;
using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BitPlate.Domain.Modules.Search
{
    [Persistent("module")]
    public class SearchResultsModule : BaseModule, IRefreshableModule, IPageableModule
    {
        private string searchString = "";
        private string fk_datacollection = "";

        public SearchResultsModule()
            : base()
        {
            ContentSamples.Add(@"{ResultsTemplate}<table id='divSearchResultsModule' >
        <thead>
          <tr>
            <td>
              Naam
            </td>
            <td>
              Titel
            </td>
            
          </tr>
        </thead>
        <tbody>
          <!--{List}-->
          <tr>
            <td>
              <a href=""{Url}"">{Name}</a>
            </td>
            <td>
              {Title}
            </td>
          </tr>
          <!--{/List}-->
        </tbody>
      </table>
{/ResultsTemplate}<br/>
Gevonden: {NumberOfResults}<br/>
{Pager}<br/>
{NoResultsTemplate}De zoekopdracht heeft geen resultaten opgeleverd.{/NoResultsTemplate}
");

            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Resultaten", IsExternal = true, Url = "/_bitplate/EditPage/Modules/SearchModules/SearchResultsModuleTab.aspx" });

        }

        public override void SetAllTags()
        {
            this._tags.Add(new Tag() { Name = "<!--{List}-->", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{List}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{Url}" });
            this._tags.Add(new Tag() { Name = "{Name}" });
            this._tags.Add(new Tag() { Name = "{Title}" });
            this._tags.Add(new Tag() { Name = "{MetaDescription}" });
            this._tags.Add(new Tag() { Name = "{Content}" });
            this._tags.Add(new Tag() { Name = "{Pager}" });
            this._tags.Add(new Tag() { Name = "{NumberOfResults}" });
            this._tags.Add(new Tag() { Name = "{ResultsTemplate}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{NoResultsTemplate}", HasCloseTag = true });

        }


        public override string Publish2(CmsPage page)
        {
            return Publish2(page, 0, 0, "");
        }



        public string Publish2(CmsPage page, int currentPage, int totalResults, string sort)
        {
            string html = base.Publish2(page);
            try
            {
                initSearchParameters();

                string filterOnDataCollection = "";
                if (fk_datacollection != String.Empty)
                {
                    filterOnDataCollection = " AND FK_DataCollection = '" + fk_datacollection + "'";
                }
                int pagesize = getSetting<int>("MaxNumberOfRows");
                //int showFromRowNumber = getSetting<int>("ShowFromRowNumber");


                bool searchOnlyMetaContent = getSetting<bool>("SearchOnlyMetaContent");
                if (sort == "")
                {
                    //url en titel tellen zwaarder in score, daarom optellen en delen door 2 (zie sql)
                    //sorteer op score desc (hoogste eerst)
                    sort = string.Format("((MATCH(Url,Title) AGAINST('{0}*' IN BOOLEAN MODE)) + (MATCH(Url,Title, MetaDescription, MetaKeywords, Content) AGAINST('{0}*' IN BOOLEAN MODE)))/2 DESC", searchString);
                }
                string sql = string.Format(@"SELECT Name, Title, Url, MetaDescription, Type, LEFT(Content, 80) AS Content, ((MATCH(Url,Title) AGAINST('{0}*' IN BOOLEAN MODE)) + (MATCH(Url,Title, MetaDescription, MetaKeywords, Content) AGAINST('{0}*' IN BOOLEAN MODE)))/2 AS Score
                FROM SearchIndex 
                WHERE FK_Site='{0}' AND MATCH(Url,Title,MetaDescription, MetaKeywords,Content) AGAINST('{1}*' IN BOOLEAN MODE)
                {2}
                ORDER BY {3}", this.Site.ID, searchString, filterOnDataCollection, sort);

                if (searchOnlyMetaContent)
                {
                    if (sort == "")
                    {
                        //url en titel tellen zwaarder in score, daarom optellen en delen door 2 (zie sql)
                        //sorteer op score desc (hoogste eerst)
                        sort = string.Format("((MATCH(Url,Title) AGAINST('{0}*' IN BOOLEAN MODE)) + (MATCH(Url,Title, MetaDescription, MetaKeywords) AGAINST('{0}*' IN BOOLEAN MODE)))/2 DESC", searchString);
                    }
                    sql = string.Format(@"SELECT Name, Title, Url, MetaDescription, Type, MetaDescription AS Content, ((MATCH(Url,Title) AGAINST('{0}*' IN BOOLEAN MODE)) + (MATCH(Url,Title, MetaDescription, MetaKeywords) AGAINST('{0}*' IN BOOLEAN MODE)))/2 AS Score
                FROM SearchIndex 
                WHERE FK_Site='{0}' AND MATCH(Url,Title,MetaDescription, MetaKeywords) AGAINST('{1}*' IN BOOLEAN MODE)
                {2}
                ORDER BY {3}", this.Site.ID, searchString, filterOnDataCollection, sort);
                }
                if (pagesize > 0)
                {
                    sql += @" LIMIT " + (currentPage * pagesize) + @", " + pagesize;
                }
                string rowTemplate = base.GetSubTemplate("{List}");
                string rowsHtml = "";
                System.Data.DataTable resultsTable = DataBase.Get().GetDataTable(sql);

                foreach (System.Data.DataRow dataRow in resultsTable.Rows)
                {
                    string rowHtml = rowTemplate;

                    foreach (System.Data.DataColumn column in dataRow.Table.Columns)
                    {
                        rowHtml = rowHtml.Replace("{" + column.ColumnName + "}", dataRow[column.ColumnName].ToString());
                    }
                    //rowHtml = rowHtml.Replace("{/Url}", "</a>");
                    rowsHtml += rowHtml;
                }

                if (totalResults == 0)
                {
                    if (pagesize > 0 && resultsTable.Rows.Count > 0)
                    {
                        string sqlTotalCount = string.Format(@"SELECT COUNT(*) FROM SearchIndex 
            WHERE FK_Site='{0}' AND MATCH(Url,Title,MetaDescription, MetaKeywords,Content) AGAINST('{1}*' IN BOOLEAN MODE) {2}", this.Site.ID, searchString, filterOnDataCollection);
                        if (searchOnlyMetaContent)
                        {
                            sqlTotalCount = string.Format(@"SELECT COUNT(*) FROM SearchIndex 
            WHERE FK_Site='{0}' AND  MATCH(Url,Title,MetaDescription, MetaKeywords) AGAINST('{1}*' IN BOOLEAN MODE) {2}", this.Site.ID, searchString, filterOnDataCollection);
                        }
                        object result = DataBase.Get().Execute(sqlTotalCount);
                        totalResults = Convert.ToInt32(result);
                    }
                    else
                    {
                        totalResults = resultsTable.Rows.Count;
                    }
                }

                if (totalResults == 0)
                {
                    html = base.EmptySubTemplate("{ResultsTemplate}", html);
                    html = html.Replace("{NoResultsTemplate}", "<div>");
                    html = html.Replace("{/NoResultsTemplate}", "</div>");

                }
                else
                {
                    html = base.EmptySubTemplate("{NoResultsTemplate}", html);
                }
                html = html.Replace("{ResultsTemplate}", "<div>");
                html = html.Replace("{/ResultsTemplate}", "</div>");
                html = html.Replace("<!--{List}-->" + rowTemplate + "<!--{/List}-->", rowsHtml);
                html = html.Replace("{List}" + rowTemplate + "{/List}", rowsHtml);
                html = html.Replace("{NumberOfResults}", totalResults.ToString());
                html = html.Replace("{Pager}", CreatePager(currentPage, totalResults, pagesize));
            }
            catch (Exception ex)
            {
                html = base.HandlePublishError(ex);
            }
            return html;
        }

        private void initSearchParameters()
        {

            if (Parameters != null && Parameters.ContainsKey("bitTextBoxSearch"))
            {
                searchString = Parameters["bitTextBoxSearch"].ToString();
            }
            if (Parameters != null && Parameters.ContainsKey("bitRadioDataCollection"))
            {
                fk_datacollection = Parameters["bitRadioDataCollection"].ToString();
            }

            if (searchString == String.Empty)
            {
                if (HttpContext.Current.Request.QueryString["search"] != null)
                {
                    searchString = HttpContext.Current.Request.QueryString["search"];
                    //zet parameter, want wordt gebruikt voor paging
                    Parameters.Add("bitTextBoxSearch", searchString);
                }
            }
            
            if (fk_datacollection == String.Empty)
            {
                if (HttpContext.Current.Request.QueryString["datacol"] != null)
                {
                    fk_datacollection = HttpContext.Current.Request.QueryString["datacol"];
                    Parameters.Add("bitRadioDataCollection", fk_datacollection);
                }
            }
        }

        public string CreatePager(int currentPage, int totalResults, int pageSize)
        {
            return base.createPager(currentPage, totalResults, pageSize);
        }

        public string Reload(CmsPage page, Dictionary<string, object> Parameters)
        {
            this.Parameters = Parameters;
            return Publish2(page);
        }


        public string DoPaging(CmsPage page, int pageNumber, int totalResults, Dictionary<string, object> Parameters)
        {
            this.Parameters = Parameters;
            return Publish2(page, pageNumber, totalResults, "");
        }
    }
}
