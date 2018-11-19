using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using HJORM;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Utils;
using System.Text.RegularExpressions;
namespace BitPlate.Domain.Search
{
    public static class SearchHelper
    {
        private static Dictionary<string, string> allUrls;
        public static void FillSearchIndex(CmsSite site)
        {
            //eerst oude weggooien
            string sql = "DELETE FROM SearchIndex WHERE FK_Site = '" + site.ID.ToString() + "'";
            DataBase.Get().Execute(sql);

            //nieuwe lege dict aanmaken voor check of url al is toegevoegd
            allUrls = new Dictionary<string, string>();
            //dan per pagina vullen
            foreach (CmsPage page in site.Pages)
            {
                string pageUrl = page.RelativeUrl;
                FillSearchIndex(pageUrl, site);
            }
        }

        public static void FillSearchIndex(string relativeUrl, CmsSite site)
        {
            if (allUrls.ContainsKey(relativeUrl)) return;
            allUrls.Add(relativeUrl, "");
            //voor Server.Execute altijd complete url nodig
            string originalUrl = UrlRewriter.GetOriginalUrl(relativeUrl, "", site.ID.ToString(), false);
            string htmlOutput = ExecuteUrl(originalUrl);
            if (htmlOutput == "") return;

            string fk_datacollection = string.Empty;
            string searchItemType = "Page";
            if (originalUrl.Contains("dataid="))
            {
                fk_datacollection = tryGetFKDataCollection(originalUrl, out searchItemType);
            }
            HtmlDocument doc = new HtmlDocument();
            try
            {
                doc.LoadHtml(htmlOutput);

                //gegevens verzamelen voor in searchindex record
                string bodyContent = doc.DocumentNode.InnerHtml;
                //string textOnlyPageContent = removeHTMLTags(bodyContent);
                string textOnlyPageContent = Utils.HtmlToText.StripHTML(bodyContent);
                string title = doc.DocumentNode.Descendants("title").SingleOrDefault().InnerText;
                string metaDescription = "";
                HtmlNode metaDescriptionNode = doc.DocumentNode.SelectSingleNode("//meta[@name='description']");//meta[@name='description']
                if (metaDescriptionNode != null)
                {
                    metaDescription = metaDescriptionNode.GetAttributeValue("content", "");
                }
                string metaKeywords = "";
                HtmlNode metaKeywordsNode = doc.DocumentNode.SelectSingleNode("//meta[@name='keywords']");//meta[@name='description']
                if (metaKeywordsNode != null)
                {
                    metaKeywords = metaKeywordsNode.GetAttributeValue("content", "");
                }

                //Search item opslaan
                SearchResultItem indexItem = new SearchResultItem();
                indexItem.Name = relativeUrl;
                indexItem.Type = searchItemType;
                indexItem.Title = title;
                indexItem.MetaDescription = metaDescription;
                indexItem.MetaKeywords = metaDescription;
                indexItem.Content = textOnlyPageContent;
                indexItem.Site = site;
                indexItem.Url = relativeUrl;
                
                indexItem.FK_DataCollection = fk_datacollection;
                indexItem.Save();


                //zoek naar doorlink urls
                foreach (HtmlNode elm in doc.DocumentNode.Descendants("a"))
                {
                    string href = elm.GetAttributeValue("href", "href");
                    if (isInternalPageLink(href, site))
                    {
                        try
                        {
                            href = href.Replace(site.DomainName, "");

                            if (!allUrls.ContainsKey(href))
                            {
                                FillSearchIndex(href, site);
                            }
                        }
                        catch (Exception ex) { }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private static string tryGetFKDataCollection(string originalUrl, out string searchItemType)
        {
            searchItemType = "";
            string fk_datacollection = string.Empty;
            if (System.Web.HttpUtility.ParseQueryString(originalUrl)["dataid"] != null)
            {
                string dataid = System.Web.HttpUtility.ParseQueryString(originalUrl)["dataid"];
                string datatype = System.Web.HttpUtility.ParseQueryString(originalUrl)["datatype"];
                //try get dataitem
                if (datatype == "G")
                {
                    searchItemType = "DataGroup";
                }
                else
                {
                    searchItemType = "DataItem";
                }
                string sql = "SELECT fk_datacollection FROM " + searchItemType + " WHERE ID ='" + dataid + "'";
                object result = DataBase.Get().Execute(sql);
                if (result != null)
                {
                    fk_datacollection = result.ToString();
                }
                
            }
            return fk_datacollection;
        }

        private static bool isInternalPageLink(string href, CmsSite site)
        {
            if (href.StartsWith("javascript:"))
            {
                return false;
            }
            if ((href.StartsWith("http:") || href.StartsWith("https:"))
                && !href.StartsWith(site.DomainName))
            {
                return false;
            }
            if(href.EndsWith(".pdf")){
                return false;
            }
            return true;
        }



        private static string ExecuteUrl(string relativeUrl)
        {
            string htmlOutput = "";
            try
            {
                System.IO.StringWriter htmlStringWriter = new System.IO.StringWriter();
                System.Web.HttpContext.Current.Server.Execute("../../" + relativeUrl, htmlStringWriter);
                htmlOutput = htmlStringWriter.GetStringBuilder().ToString();
            }
            catch (Exception ex)
            {

            }
            return htmlOutput;
        }

        //private static string removeHTMLTags(string source)
        //{
        //    string expn = "<.*?>";
        //    return Regex.Replace(source, expn, string.Empty);
        //}

        //public static void FillSearchIndex(CmsPage page)
        //{
        //    SearchResultItem indexItem = BaseObject.GetFirst<SearchResultItem>("FK_Page = '" + page.ID + "'");
        //    if (indexItem == null)
        //    {
        //        indexItem = new SearchResultItem();
        //    }

        //    string url = page.RelativeUrl;
        //    string title = page.Title;
        //    string meta = page.MetaDescription + " " + page.MetaKeywords;
        //    string content = "";


        //    indexItem.Name = page.Name;
        //    indexItem.Type = "Page";
        //    indexItem.DateFrom = page.DateFrom;
        //    indexItem.DateTill = page.DateTill;
        //    indexItem.Content = content;
        //    indexItem.Site = page.Site;
        //    indexItem.FK_Page = page.ID;
        //    indexItem.Url = url;
        //    indexItem.Title = page.Title;
        //    indexItem.Meta = meta;
        //    indexItem.Save();
        //}

        //public static void FillSearchIndex(BaseDataObject dataObj)
        //{
        //    SearchResultItem indexItem = BaseObject.GetFirst<SearchResultItem>("FK_Group = '" + dataObj.ID + "'");
        //    if (indexItem == null)
        //    {
        //        indexItem = new SearchResultItem();
        //    }

        //    string url = "";

        //    foreach (CmsPage page in dataObj.DataCollection.IsInUseAtPages())
        //    {
        //        url = page.RelativeUrl + "?dataid=" + dataObj.ID;
        //    }
        //    string title = dataObj.Title;
        //    string meta = dataObj.Name + " " + dataObj.Title;
        //    string content = "";
        //    foreach (DataField field in dataObj.DataCollection.DataGroupFields)
        //    {
        //        content += field.Name + ": " + getFieldValue(dataObj, field.MappingColumn) + "\r\n";
        //    }
        //    content = Utils.HtmlToText.StripHTML(content);
        //    indexItem.Name = dataObj.Name;
        //    indexItem.Type = "Data";
        //    indexItem.DateFrom = dataObj.DateFrom;
        //    indexItem.DateTill = dataObj.DateTill;
        //    indexItem.Content = content;
        //    indexItem.Site = dataObj.Site;
        //    indexItem.FK_Group = dataObj.ID;
        //    indexItem.Url = url;
        //    indexItem.Title = title;
        //    indexItem.Meta = meta;
        //    indexItem.Save();
        //}

        //private static object getFieldValue(BaseDataObject dataObj, string mappingField)
        //{

        //    System.Reflection.PropertyInfo prop = dataObj.GetType().GetProperty(mappingField);
        //    if (prop == null) return null;
        //    Object returnValue = prop.GetValue(dataObj, null);
        //    return returnValue;
        //}

        //public static Dictionary<string, SearchResultItem> Find(CmsSite site, string searchString)
        //{
        //    //mysql only!!!
        //    BaseCollection<SearchResultItem> founditems = BaseCollection<SearchResultItem>.Get("FK_Site = '" + site.ID.ToString() + "' AND  MATCH(Url,Title,Meta,Content) AGAINST('" + searchString + "*' IN BOOLEAN MODE)", "type, MATCH(Meta,Content) AGAINST('" + searchString + "*' IN BOOLEAN MODE) DESC");

        //    Dictionary<string, SearchResultItem> results = new Dictionary<string, SearchResultItem>();
        //    foreach (SearchResultItem item in founditems)
        //    {
        //        if (item.IsActive)
        //        {
        //            if (!results.ContainsKey(item.Url))
        //            {
        //                results.Add(item.Url, item);
        //            }
        //        }
        //    }
        //    return results;
        //}


    }
}
