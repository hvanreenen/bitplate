using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace BitPlate.Domain.Utils
{
    public static class HtmlHelper
    {
        public static string RemoveHeadAndBody(string html)
        {
            int startbody = html.ToLower().IndexOf("<body");
            int endbody = 0;
            if (startbody > 0)
            {
                endbody = html.IndexOf(">", startbody) + 1;
            }
            int end = html.ToLower().IndexOf("</body>");
            if (endbody > 0 && end > 0)
            {
                html = html.Substring(endbody, end - endbody);
            }
            return RemoveCssLink(html);
        }

        internal static string RemoveCssLink(string html)
        {
            int startlink = html.ToLower().IndexOf("<link");
            int endlink = 0;
            if (startlink >= 0)
            {
                endlink = html.IndexOf(">", startlink) + 1;
            }
            int end = html.Length;
            if (startlink >= 0 && endlink > 0)
            {
                html = html.Substring(0, startlink) + html.Substring(endlink, end - endlink);
                //html = html.Substring(endlink, end - endlink);
            }
            return html;
        }

        public static HtmlNode GetHtmlElementByID(string completeHtml, string elmId)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(completeHtml);
            HtmlNode elm = doc.GetElementbyId(elmId);
            return elm;
        }

        public static string GetInnerHtmlFromElementByID(string completeHtml, string elmId)
        {
            HtmlNode elm = GetHtmlElementByID(completeHtml, elmId);
            return elm.InnerHtml;
        }

        public static string GetOuterHtmlFromElementByID(string completeHtml, string elmId)
        {
            HtmlNode elm = GetHtmlElementByID(completeHtml, elmId);
            return elm.OuterHtml;
        }

        

        /// <summary>
        /// Vervang alle live links voor editlinks als je in pageedit.aspx zit
        /// hierdoor kun je navigeren terwijl je in editmode blijft
        /// </summary>
        /// <param name="content"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// 
        public static string ReplaceLinks(string content, BitPlate.Domain.ModeEnum mode, CmsSite site)
        {

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            
            foreach (HtmlNode elm in doc.DocumentNode.Descendants("a"))
            {
                string href = elm.GetAttributeValue("href", "href");
                try
                {
                    Uri uri = new Uri(href);
                    href = uri.PathAndQuery;
                    href = href.Replace("/_bitPlate/", "");
                }
                catch (Exception ex) { }

                //Guid pageid = CmsPage.GetPageIDByUrl(href, site.ID.ToString());
                //if (pageid != Guid.Empty)
                //{
                //    if (mode == BitPlate.Domain.Modules.ModeEnum.EditPageMode)
                //    {
                //        elm.SetAttributeValue("href", "javascript:BITEDITPAGE.loadPage('" + pageid + "');");
                //    }
                //    else if (mode == BitPlate.Domain.Modules.ModeEnum.PublishMode)
                //    {
                //        elm.SetAttributeValue("href", site.DomainName + "/" + href);
                //    }
                //}
            }
            content = doc.DocumentNode.WriteTo();


            return content;
        }

        /// <summary>
        /// Vervangt alle image src voor urls met domainname erin (relative urls worden absolute)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string ReplaceImageSources(string content, BitPlate.Domain.ModeEnum mode, CmsSite site)
        {

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);

            foreach (HtmlNode elm in doc.DocumentNode.Descendants("img"))
            {
                string src = elm.GetAttributeValue("src", "src");
                if (!src.StartsWith(site.DomainName) && src!="null")
                {
                    elm.SetAttributeValue("src", site.DomainName + "/" + src);
                }
                
            }
            content = doc.DocumentNode.WriteTo();

            return content;
        }

        public static string ValidateHtml(string html)
        {
            //List<string> returnList = new List<string>();
            string returnValue = "";
            
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            IEnumerable<HtmlParseError> errors = doc.ParseErrors;
            foreach (HtmlParseError error in errors)
            {
                string msg = String.Format("{0} in regel {1} op posititie {2}\r\n", error.Reason, error.Line, error.LinePosition);
                msg = msg.Replace("<", "&lt;");
                msg = msg.Replace(">", "&gt;");

                returnValue += msg;
            }
            return returnValue;
        }

    }
}
