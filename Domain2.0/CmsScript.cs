using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HJORM.Attributes;
using BitPlate.Domain.Utils;
using System.Text.RegularExpressions;
using HJORM;
using System.Web;

namespace BitPlate.Domain
{
    /// <summary>
    /// Css of javascript
    /// </summary>
    public enum ScriptTypeEnum { Css, Js }
    /// <summary>
    /// Class voor javascripts en css
    /// </summary>
    [Persistent("Script")]
    
    public class CmsScript : BaseDomainPublishableObject
    {
        /// <summary>
        /// Script type: css of javascript
        /// </summary>
        public ScriptTypeEnum ScriptType { get; set; }
        [NonPersistent()]
        public string Type
        {
            get
            {
                if (ScriptType == ScriptTypeEnum.Css)
                {
                    return "css";
                }
                else
                {
                    return "js";
                }
            }
        }
        [NonPersistent()]
        public string CompleteName
        {
            get
            {
                if (ScriptType == ScriptTypeEnum.Css)
                {
                    return Name + " (css)";
                }
                else
                {
                    return Name + " (js)";
                }
            }
        }

        [NonPersistent]
        public string ContentType
        {
            get
            {
                if (this.ScriptType == ScriptTypeEnum.Css)
                {
                    return "text/css";
                }
                else
                {
                    return "text/javascript";
                }
            }
        }

        public CmsScript()
        {
            this.ActiveInEditor = true;
        }
        /// <summary>
        /// Inhoud van het script (Tekst)
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// isScript actief in editor mode (bool)
        /// </summary>
        public bool ActiveInEditor { get; set; }
        /// <summary>
        /// Script wordt gezet in eleke pagina van deze site
        /// </summary>
        public bool LoadInWholeSite { get; set; }

        /// <summary>
        /// CSS kan een extra media tag bevatten.
        /// </summary>
        public string StylesheetMedia { get; set; }

        /// <summary>
        /// Sommige scripts horen bij het systeem, zoals standaard scripts voor menu's
        /// Deze scripts mogen niet worden verwijderd
        /// </summary>
        public bool IsSystemValue { get; set; }

        private string _src;
        /// <summary>
        /// Relative filename (url)
        /// </summary>
        [NonPersistent()]
        public string Url
        {
            get
            {
                //if ((_src == null || _src == "") && this.Name != null)
                if (this.Name != null)
                {
                    if (ScriptType == ScriptTypeEnum.Css)
                    {
                        _src = this.Name.EndsWith(".css") ? this.Name : "/_css/" + this.Name + ".css";
                    }
                    else
                    {
                        _src = this.Name.EndsWith(".js") ? this.Name : "/_js/" + this.Name + ".js";
                    }
                }
                return _src;
            }
            set
            {
                _src = value;
            }
        }

        public string GetTag()
        {
            string tag = "";
            if (ScriptType == ScriptTypeEnum.Css)
            {
                string stylesheetTagFormat = @"<link rel=""stylesheet"" type=""text/css"" href=""{0}"" id=""BitStyleScript{1}""/>";
                if (this.StylesheetMedia != null && this.StylesheetMedia != "")
                {
                    stylesheetTagFormat = @"<link rel=""stylesheet"" type=""text/css"" href=""{0}"" id=""BitStyleScript{1}"" media=""{2}""/>";
                }
                //tag = String.Format(@"<link rel=""stylesheet"" type=""text/css"" href=""{0}"" runat=""server"" id=""BitStyleScript{1}"" media=""{2}""/>
                tag = String.Format(stylesheetTagFormat, Url, ID.ToString().Replace("-", ""), this.StylesheetMedia);
            }
            else
            {
                tag = String.Format(@"<script type=""text/javascript"" src=""{0}"" id=""BitScript{1}""></script>
", Url, ID.ToString().Replace("-", ""));
            }
            return tag;
        }

        //private string PlaceBitSiteContainerInContent() // Voeg BitSiteBody toe aan css
        //{
        //    string css = this.Content;
        //    //css = Regex.Replace(css, @"/\*(.*?)\*/", "", RegexOptions.Singleline);

        //    //CSS regels zoeken
        //    foreach (Match style in Regex.Matches(css, @"(\s|^)(.*?)\s*{(.*?)}", RegexOptions.Singleline))
        //    {
        //        string cssRule = style.ToString();
        //        //unieke rules (met id) hoef je niet te corrigeren
        //        string cssSelectorRule = "";
        //        cssSelectorRule = Regex.Match(cssRule, @"(\s|^)(.*?)\s*{").ToString();
        //        string orginalCssSelectorRule = cssSelectorRule;
        //        cssSelectorRule = cssSelectorRule.Replace("{", "").TrimStart();
        //        string[] cssSelectors = cssSelectorRule.Split(',');
        //        foreach (string cssSelector in cssSelectors)
        //        {
        //            bool changes = false;
        //            string newCssSelector = cssSelector;
        //            if (newCssSelector.ToLower().StartsWith("body"))
        //            {
        //                newCssSelector = newCssSelector.Replace("body", "#BitSiteContainer").Replace("BODY", "#BitSiteContainer");
        //                changes = true;
        //            }

        //            if (!newCssSelector.StartsWith("#") && !newCssSelector.StartsWith("."))
        //            {
        //                newCssSelector = "#BitSiteContainer " + newCssSelector;
        //                changes = true;
        //            }

        //            if (changes)
        //            {
        //                try
        //                {
        //                    Regex CurrentRuleSearch = new Regex(cssSelector);
        //                    cssSelectorRule = CurrentRuleSearch.Replace(cssSelectorRule, newCssSelector, 1);
        //                }
        //                catch
        //                {
        //                }
                        
        //            }
        //            //cssSelectorRule = cssSelectorRule.Replace(cssSelector, newCssSelector);
        //        }
        //        css = css.Replace(orginalCssSelectorRule, cssSelectorRule + " {");
        //    }
        //    return css;
        //}

        private string EscapeBitplate() // Voeg BitSiteBody toe aan css
        {
            string css = this.Content;
            //css = Regex.Replace(css, @"/\*(.*?)\*/", "", RegexOptions.Singleline);

            //CSS regels zoeken
            foreach (Match style in Regex.Matches(css, @"(\s|^)(.*?)\s*{(.*?)}", RegexOptions.Singleline))
            {
                string cssRule = style.ToString();
                //unieke rules (met id) hoef je niet te corrigeren
                string cssSelectorRule = "";
                cssSelectorRule = Regex.Match(cssRule, @"(\s|^)(.*?)\s*{").ToString();
                string orginalCssSelectorRule = cssSelectorRule;
                cssSelectorRule = cssSelectorRule.Replace("{", "").TrimStart();
                string[] cssSelectors = cssSelectorRule.Split(',');
                foreach (string cssSelector in cssSelectors)
                {
                    bool changes = false;
                    string newCssSelector = cssSelector;
                    if (newCssSelector.ToLower().StartsWith("body"))
                    {
                        newCssSelector = newCssSelector.Replace("body", "form").Replace("BODY", "form");
                        changes = true;
                    }

                    if (!newCssSelector.StartsWith("#") && !newCssSelector.StartsWith("."))
                    {
                        newCssSelector = newCssSelector + ":not([id=^bit]):not([class=^bit])";
                        changes = true;
                    }

                    if (changes)
                    {
                        try
                        {
                            Regex CurrentRuleSearch = new Regex(cssSelector);
                            cssSelectorRule = CurrentRuleSearch.Replace(cssSelectorRule, newCssSelector, 1);
                        }
                        catch
                        {
                        }

                    }
                    //cssSelectorRule = cssSelectorRule.Replace(cssSelector, newCssSelector);
                }
                css = css.Replace(orginalCssSelectorRule, cssSelectorRule + " {");
            }
            return css;
        }

        public override void Save()
        {
            //zie publish
            //if (!this.IsNew)
            //{
            //    //kijk of naam is gewijzigd
            //    CmsScript scriptFromDB = BaseDomainObject.GetById<CmsScript>(this.ID);
            //    if (scriptFromDB != null && this.Name != scriptFromDB.Name)
            //    {
            //        //naam is gewijzigd: oude file weggooien
            //        string oldFilename = this.Site.Path + scriptFromDB.Url.Replace("/", "\\");
            //        FileHelper.DeleteFile(oldFilename);
            //    }
            //}
            //string escapedContent = EscapeBitplate();
            base.Save();
            //CheckDirs();
            //string filename = this.Site.Path + Url.Replace("/", "\\");
            //FileHelper.WriteFile(filename, escapedContent);
            //FileHelper.WriteFile(filename, this.Content);

            //UnpublishedItem.Set(this, "Script");

            this.Publish();

            //eventuele dubbele koppelingen verwijderen
            if (LoadInWholeSite)
            {
                //mocht dit script al zijn gekoppeld aan template of page dan hier koppeling verwijderen
                string sql = String.Format("DELETE FROM ScriptPerTemplate WHERE FK_Script =  '{0}'", ID);
                DataBase.Get().Execute(sql);
                sql = String.Format("DELETE FROM ScriptPerPage WHERE FK_Script =  '{0}'", ID);
                DataBase.Get().Execute(sql);
            }

            //Reset scriptbundles
            foreach (string key in HttpContext.Current.Application.AllKeys)
            {
                if (key.Contains("bit_"))
                {
                    HttpContext.Current.Application[key] = null;
                }
            }

        }

        public override void Delete()
        {
            if (this.IsSystemValue)
            {
                throw new Exception("Systeem scripts mogen niet worden verwijderd.");
            }
            base.Delete();
            string filename = this.Site.Path + Url.Replace("/", "\\");
            //FileHelper.DeleteFile(filename);


            UnpublishedItem.Set(this, "Script", ChangeStatusEnum.Deleted);
        }

        /// <summary>
        /// controleer of dirs bestaan
        /// </summary>
        private void CheckDirs()
        {
            if (ScriptType == ScriptTypeEnum.Css)
            {
                if (!Directory.Exists(this.Site.Path + "_css\\"))
                {
                    Directory.CreateDirectory(this.Site.Path + "_css\\");
                }

            }
            else if (ScriptType == ScriptTypeEnum.Js)
            {
                if (!Directory.Exists(this.Site.Path + "_js\\"))
                {
                    Directory.CreateDirectory(this.Site.Path + "_js\\");
                }

            }
        }


        public void Publish()
        {
            string fileName = this.Site.Path + Url.Replace("/", "\\");
            //string escapedContent = EscapeBitplate();

            //FileHelper.WriteFile(fileName, this.Content);

            //if (LastPublishedFileName != null && LastPublishedFileName != fileName)
            //{
            //    //naam is gewijzigd
            //    //dan oude script weggooien
            //    FileHelper.DeleteFile(LastPublishedFileName);
                
            //}
            base.SaveLastPublishInfo("Script", fileName);

        }

        public CmsScript Copy(string newScriptName)
        {
            CmsScript newScript = this.CreateCopy<CmsScript>(false);
            newScript.Name = newScriptName;
            newScript.LastPublishedDate = null;
            
            //bij usergroups & users is het niet het script zelf dat onder een template hangt, maar alleen de koppeling (veel op veel).

            if (this.HasAutorisation)
            {
                foreach (Autorisation.SiteUserGroup userGroup in this.AutorizedSiteUserGroups)
                {
                    newScript.AutorizedSiteUserGroups.Add(userGroup);
                }
                foreach (Autorisation.SiteUser user in this.AutorizedSiteUsers)
                {
                    newScript.AutorizedSiteUsers.Add(user);
                }
            }
            newScript.Save();
            return newScript;

        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            //this.IsSystemValue = DataConverter.ToBoolean(dataRow["IsSystemValue"]);
            this.Content = dataRow["Content"].ToString();
            this.ScriptType = (ScriptTypeEnum)DataConverter.ToInt32(dataRow["ScriptType"]);
            this.LoadInWholeSite =DataConverter.ToBoolean(dataRow["LoadInWholeSite"]);
            this.ActiveInEditor = DataConverter.ToBoolean(dataRow["ActiveInEditor"]);
            this.StylesheetMedia = dataRow["StylesheetMedia"].ToString();
            this.IsLoaded = true;
        }

        public static string GetScriptIDByUrl(string url, string siteid)
        {
            string returnValue = "";
            string scriptType = url.EndsWith(".js") ? "1" : "0";
            url = url.Replace("/_css/", "").Replace(".css", "");
            url = url.Replace("/_js/", "").Replace(".js", "");
            string where = String.Format("FK_Site='{0}' AND Name = '{1}' AND ScriptType='{2}'", siteid, url, scriptType);
            CmsScript script = BaseObject.GetFirst<CmsScript>(where);

            if (script != null)
            {
                returnValue = script.ID.ToString();
            }
            return returnValue;
        }
    }
}

