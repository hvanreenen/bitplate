using BitPlate.Domain.Newsletters;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Utils
{
    public static class UrlRewriter
    {

        public static string GetOriginalUrl(string inputRelativeUrl, string inputQueryString, string siteid, bool isEditMode)
        {
            string inputUrlExtension = System.Web.VirtualPathUtility.GetExtension(inputRelativeUrl);
            
            string outputUrl = "";
            if (inputRelativeUrl.StartsWith("/newsletter"))
            {
                Guid newsletterid = Newsletter.GetNewsletterIDByUrl(inputRelativeUrl, siteid);
                if (newsletterid != Guid.Empty)
                {
                    if (isEditMode)
                    {
                        outputUrl = "/_bitplate/editpage/EditPage.aspx?newsletterid=" + newsletterid.ToString();
                    }
                    else
                    {
                        outputUrl = "/Newsletter.aspx?newsletterid=" + newsletterid.ToString();
                    }
                    if (inputQueryString != "")
                    {
                        outputUrl += "&" + inputQueryString;
                    }
                }
            }
            else if (inputUrlExtension == ".aspx" || inputUrlExtension == "")
            {
                Guid pageid = Guid.Empty;
                //get site homepage;
                if (inputRelativeUrl == "/")
                {
                    pageid = CmsSite.GetHomePageIDBySiteID(siteid);
                }
                //get folder homepage
                if (inputRelativeUrl.EndsWith("/") && pageid == Guid.Empty)
                {
                    pageid = CmsPage.GetPageIDByFolderUrl(inputRelativeUrl, siteid);
                }
                //chrome fix: chrome plaatst zelf / achter de urls als je die een keer erachter hebt gezet. Hierom nog een keer checken of pagina kan worden gevonden (ook met /)
                if (pageid == Guid.Empty)
                {
                    pageid = CmsPage.GetPageIDByUrl(inputRelativeUrl, siteid);
                }

                if (pageid != Guid.Empty)
                {
                    if (isEditMode)
                    {
                        outputUrl = "/_bitplate/editpage/EditPage.aspx?pageid=" + pageid.ToString();
                    }
                    else
                    {
                        outputUrl = "/Page.aspx?pageid=" + pageid.ToString();
                    }
                }
                else
                {
                    outputUrl = getUrlIfUrlSegmentsContainsGuid(inputRelativeUrl, siteid);
                }
                if (outputUrl != string.Empty && inputQueryString != string.Empty)
                {
                    outputUrl += "&" + inputQueryString;
                }
            }
            return outputUrl;
        }


        private static string getUrlIfUrlSegmentsContainsGuid(string relativeUrl, string siteid)
        {
            string outputUrl = string.Empty;
            relativeUrl = relativeUrl.ToLower();
            //rewriteurl bestaat uit [pad]/[page zonder .aspx]/[I of G][Guid in 32 posities]/[groep name]/[itemtitel]

            //bijvoorbeeld: /nl/producten/I123fd45678a9fecb3672fe410/brood/afbakbrood/rogge brood met stukjes
            //hier moeten we kijken of de url geldig is opgebouwd, zo jda dan geven we nieuwe url terug
            //zo nee, dan geven we lege string terug
 
            //dus er moet ergens /g of /i in de url staan
            int indexOfDataIdStart = relativeUrl.IndexOf("/g");
            if (indexOfDataIdStart < 0)
            {
                //probeer met I
                indexOfDataIdStart = relativeUrl.IndexOf("/i");
            }
            if (indexOfDataIdStart > 0)
            {
                Guid dataId = Guid.Empty;
                //is dat het geval, dan kijken of 32 posities verder een guid is
                string possibleGuidSegment = relativeUrl.Substring(indexOfDataIdStart + 2, 32);
                Guid.TryParse(possibleGuidSegment, out dataId);
                if (dataId != Guid.Empty)
                {
                    //is dat het geval, dan is eerste letter van segment een i of een g (item of groep)
                    string dataType = relativeUrl.Substring(indexOfDataIdStart + 1, 1);
                    //en is gedeelte voor de guid het pad en de paginanaam
                    string pageUrl = relativeUrl.Substring(0, indexOfDataIdStart);
                    //hiermee halen we id op
                    Guid pageId = CmsPage.GetPageIDByUrl(pageUrl, siteid);
                    //en bouwen nieuwe url op
                    outputUrl = "/Page.aspx?pageid=" + pageId.ToString() + "&dataid=" + dataId.ToString() + "&datatype=" + dataType;
                }
            }
            return outputUrl;

        }


    }
}
