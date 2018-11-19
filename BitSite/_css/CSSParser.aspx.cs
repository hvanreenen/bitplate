using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain;

/*
 *  Wordt gebruikt in pageeditor.aspx. Alle style bestanden van de geopende pagina worden uitgelezen door deze parser.
 *  Alle style regels worden aangepast zodat deze alleen invloed hebben op de geladen pagina binnen de pageeditor. 
 *  Globale regels voor de elementen 'A' & 'DIV' worden aangepast om alle elementen welke een in de class naam 'bit' uit te zonderen van deze regels.
 * 
 */

namespace BitSite._css
{
    public partial class CSSParser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string url = Request.QueryString["url"];
            CmsSite site = SessionObject.CurrentSite;
            bool isTemplate = (Request.QueryString["type"] == "template");
            string ContainerWrapper = (isTemplate) ? "#bitTemplateEditorWrapper" : "#bitEditPageBody";
            string path = site.Path + url.Replace("/", "\\");
            string newStyleSheet = ""; //"/* Unable to load script: " + path + "*/\r\n";
            try
            {
                StreamReader sr = new StreamReader(path);
                //Uitlezen van CSS bestand.
                string css = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();

                //FIX
                css = Regex.Replace(css, @"/\*(.*?)\*/", "", RegexOptions.Singleline);

                //CSS regels zoeken
                foreach (Match style in Regex.Matches(css, @"(\s|^)(.*?)\s*{(.*?)}", RegexOptions.Singleline)) {
                    string cssRule = style.ToString();
                    //unieke rules (met id) hoef je niet te corrigeren
                    string cssSelectorRule = "";

                    if (cssRule.Trim().StartsWith("#") || cssRule.Trim().StartsWith("@"))
                    {
                        newStyleSheet += cssRule + "\r\n";
                    }
                    else
                    {
                        // CSS url() function zoeken en corrigeren 
                        /* foreach (Match cssUrl in Regex.Matches(cssRule, @"url\((.*?)\)", RegexOptions.Singleline))
                        {
                            string cssUrlString = cssUrl.ToString().Replace("url(", "").Replace(")", "").Replace("'", "");
                            cssUrlString = cssUrlString.Replace("../", "");
                            cssUrlString = site.DomainName + cssUrlString;
                            cssRule = cssRule.Replace(cssUrl.ToString(), "url(" + cssUrlString + ")");
                        } */
                        //IE8 en lager vallen terug naar fallback method (css2 compatible)
                        //CSS3 ondersteunende browser krijgen regel uitzonderingen voor globale regels welke betrekking hebben op de elemenen 'A' & 'DIV'.
                        if (Request.Browser.Browser == "IE" && Request.Browser.MajorVersion <= 8)
                        {
                            //FallBack
                        }
                        else
                        {
                            cssSelectorRule = Regex.Match(cssRule, @"(\s|^)(.*?)\s*{").ToString();
                            string orginalCssSelectorRule = cssSelectorRule;
                            cssSelectorRule = cssSelectorRule.Replace("{", "").TrimStart();
                            string[] cssSelectors = cssSelectorRule.Split(',');
                            foreach (string cssSelector in cssSelectors)
                            {
                                string newCssSelector = cssSelector;
                                /* if (cssSelector.Trim().ToLower().EndsWith("a") || cssSelector.Trim().ToLower().EndsWith("div"))//|| cssSelector.Trim().ToLower().EndsWith("*") || cssSelector.Trim().ToLower().EndsWith("body"))
                                {
                                    newCssSelector += ":not([class*='bit'])";//:not([id*='bit'])";
                                }

                                if (cssSelector.ToLower().Contains("body"))
                                {
                                    newCssSelector = newCssSelector.Replace("body", ContainerWrapper);
                                }
                                else
                                {
                                    newCssSelector = ContainerWrapper + " " + newCssSelector;
                                } */
                                newCssSelector += ":not(#bitEditPageMenusWrapper):not([class*='cmsObject']):not([class*='-ui']):not([class*='ui-'])";
                                cssSelectorRule = cssSelectorRule.Replace(cssSelector, newCssSelector);
                            }
                            cssRule = "\r\n" + cssRule.Replace(orginalCssSelectorRule, cssSelectorRule + " {");
                            
                        }
                        // #bitEditPageBody binnen dit element wordt de pagina geladen in pageedit.aspx
                        /* if (isTemplate)
                        {
                            newStyleSheet += "#bitTemplateEditorWrapper " + cssRule + "\r\n";
                        }
                        else
                        { */
                            newStyleSheet += cssRule + "\r\n";
                            /* if (cssSelectorRule == "body")
                            {
                                //newStyleSheet += cssRule + "\r\n";
                                newStyleSheet += "#bitEditPageBody " + cssRule.Replace("body", "") + "\r\n";
                            }
                            else
                            {
                                newStyleSheet += "# " + cssRule + "\r\n";
                            } */
                       // }
                    }
                }
            }
            catch (Exception ex)
            {
                newStyleSheet += "/* " + ex.ToString() + " */";
            }
           
            //Print de aangepaste content als CSS script
            Response.ContentType = "text/css";
            Response.Write(newStyleSheet);
        }

//        /* 
//         protected void Page_Load(object sender, EventArgs e)
//        {
//            string url = Request.QueryString["url"];
//            CmsSite site = SessionObject.CurrentSite;
//            bool isTemplate = (Request.QueryString["type"] == "template");
//            string ContainerWrapper = (isTemplate) ? "#bitTemplateEditorWrapper" : "#bitEditPageBody";
//            string path = site.Path + url.Replace("/", "\\");
//            string newStyleSheet = ""; //"/* Unable to load script: " + path + "*/ //\r\n";
//            try
//            {
//                StreamReader sr = new StreamReader(path);
//                //Uitlezen van CSS bestand.
//                string css = sr.ReadToEnd();
//                sr.Close();
//                sr.Dispose();

//                //FIX
//                css = Regex.Replace(css, @"/\*(.*?)\*/", "", RegexOptions.Singleline);

//                //CSS regels zoeken
//                foreach (Match style in Regex.Matches(css, @"(\s|^)(.*?)\s*{(.*?)}", RegexOptions.Singleline)) {
//                    string cssRule = style.ToString();
//                    //unieke rules (met id) hoef je niet te corrigeren
//                    string cssSelectorRule = "";

//                    if (cssRule.Trim().StartsWith("#") || cssRule.Trim().StartsWith("@"))
//                    {
//                        newStyleSheet += cssRule + "\r\n";
//                    }
//                    else
//                    {
//                        // CSS url() function zoeken en corrigeren 
//                        foreach (Match cssUrl in Regex.Matches(cssRule, @"url\((.*?)\)", RegexOptions.Singleline))
//                        {
//                            string cssUrlString = cssUrl.ToString().Replace("url(", "").Replace(")", "").Replace("'", "");
//                            cssUrlString = cssUrlString.Replace("../", "");
//                            cssUrlString = site.DomainName + cssUrlString;
//                            cssRule = cssRule.Replace(cssUrl.ToString(), "url(" + cssUrlString + ")");
//                        }
//                        //IE8 en lager vallen terug naar fallback method (css2 compatible)
//                        //CSS3 ondersteunende browser krijgen regel uitzonderingen voor globale regels welke betrekking hebben op de elemenen 'A' & 'DIV'.
//                        if (Request.Browser.Browser == "IE" && Request.Browser.MajorVersion <= 8)
//                        {
//                            //FallBack
//                        }
//                        else
//                        {
//                            cssSelectorRule = Regex.Match(cssRule, @"(\s|^)(.*?)\s*{").ToString();
//                            string orginalCssSelectorRule = cssSelectorRule;
//                            cssSelectorRule = cssSelectorRule.Replace("{", "").TrimStart();
//                            string[] cssSelectors = cssSelectorRule.Split(',');
//                            foreach (string cssSelector in cssSelectors)
//                            {
//                                string newCssSelector = cssSelector;
//                                if (cssSelector.Trim().ToLower().EndsWith("a") || cssSelector.Trim().ToLower().EndsWith("div"))//|| cssSelector.Trim().ToLower().EndsWith("*") || cssSelector.Trim().ToLower().EndsWith("body"))
//                                {
//                                    newCssSelector += ":not([class*='bit'])";//:not([id*='bit'])";
//                                }

//                                if (cssSelector.ToLower().Contains("body"))
//                                {
//                                    newCssSelector = newCssSelector.Replace("body", ContainerWrapper);
//                                }
//                                else
//                                {
//                                    newCssSelector = ContainerWrapper + " " + newCssSelector;
//                                }
//                                cssSelectorRule = cssSelectorRule.Replace(cssSelector, newCssSelector);
//                            }
//                            cssRule = "\r\n" + cssRule.Replace(orginalCssSelectorRule, cssSelectorRule + " {");
                            
//                        }
//                        // #bitEditPageBody binnen dit element wordt de pagina geladen in pageedit.aspx
//                        /* if (isTemplate)
//                        {
//                            newStyleSheet += "#bitTemplateEditorWrapper " + cssRule + "\r\n";
//                        }
//                        else
//                        { */
//                            newStyleSheet += cssRule + "\r\n";
//                            /* if (cssSelectorRule == "body")
//                            {
//                                //newStyleSheet += cssRule + "\r\n";
//                                newStyleSheet += "#bitEditPageBody " + cssRule.Replace("body", "") + "\r\n";
//                            }
//                            else
//                            {
//                                newStyleSheet += "# " + cssRule + "\r\n";
//                            } */
//                       // }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                newStyleSheet += "/* " + ex.ToString() + " */";
//            }
           
//            //Print de aangepaste content als CSS script
//            Response.ContentType = "text/css";
//            Response.Write(newStyleSheet);
//        }
//*/
    }
}