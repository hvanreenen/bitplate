using BitPlate.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace BitPlate.Domain.Modules
{
    public class BasePostableModule : BaseModule, IPostableModule
    {

        public BasePostableModule()
        {
            IncludeScripts.Add("/_js/BitFormValidation.js");
            IncludeScripts.Add("/_js/jquery.iframe-post-form.js");
        }

        public virtual PostResult HandlePost(CmsPage page, Dictionary<string, string> FormParameters)
        {
            throw new NotImplementedException("Methode HandlePost() must be overridden");
            
        }
        protected PostResult createPostResult()
        {
            ModuleNavigationAction navAction = NavigationActions[0];
            PostResult result = getNewPostResult(navAction);
            return result;
        }
        /// <summary>
        /// valideer captchacode als die bestaat.
        /// </summary>
        /// <param name="FormParameters"></param>
        /// <returns></returns>
        protected PostResult validateCaptchaIfExists(Dictionary<string, string> FormParameters)
        {
            PostResult result = createPostResult();
            bool captchaValidation = true;
            if (FormParameters["hiddenValidationRequired"] == "true")
            {
                //alleen als catcha in sessie is gezet in Publish2() controle doen 
                if (HttpContext.Current.Session["captcha_code_" + this.ID.ToString("N")] != null)
                {
                    captchaValidation = (FormParameters.ContainsKey("CaptchaValidationField") && HttpContext.Current.Session["captcha_code_" + this.ID.ToString("N")].ToString().ToLower() == FormParameters["CaptchaValidationField"].ToString().ToLower());
                }
            }
            if (!captchaValidation)
            {
                result = new PostResult() {ErrorMessage = "<li>De ingevoerde code klopt niet.</li>"};
            }
            return result;
        }

        

        public override void SetAllTags()
        {
            //postable module heeft minstens error template
            this._tags.Add(new Tag() { Name = "{ErrorTemplate}", HasCloseTag = true, ReplaceValue = "<div id=\"bitErrorTemplate" + this.ID.ToString("N") + "\" style=\"display:none\">", ReplaceValueCloseTag = "</div>" });
            this._tags.Add(new Tag() { Name = "{ErrorMessage}", ReplaceValue = "<span id=\"bitErrorMessage" + this.ID.ToString("N") + "\"></span>" });
            this._tags.Add(new Tag() { Name = "{CaptchaImage}", ReplaceValue = "<img src=\"/captcha.handler?moduleid=" + this.ID.ToString("N") + "\" />" });
            this._tags.Add(new Tag() { Name = "{CaptchaText}", ReplaceValue = "<input type=\"text\" name=\"CaptchaValidationField\" data-validation=\"required\" />" });
            //this._tags.Add(new Tag() { Name = "{CaptchaText}", ReplaceValue = "<input type=\"text\" name=\"CaptchaValidationField\"  />" });

        }

        protected override ValidationResult validateModule()
        {
            ValidationResult result = base.validateModule();
            if (!this.Content.Contains("{ErrorMessage}"))
            {
                result.Message += "Module die iets opsturen moeten de tag {ErrorMessage} bevatten. Tip: stop deze tag tussen {ErrorTemplate} en {/ErrorTemplate} om de melding eigen lay-out te kunnen geven.<br/>";
            }
            return result;
        }

        public override string Publish2(CmsPage page)
        {
            string html = base.Publish2(page);
            if (html.Contains("{CaptchaImage}"))
            {
                //init captcha code
                HttpContext.Current.Session["captcha_code_" + this.ID.ToString("N")] = true;
            }
            return html;
        }
        protected override string getModuleStartDiv()
        {
            string moduleStartDiv = base.getModuleStartDiv();
            ModuleNavigationAction navAction = NavigationActions[0];
            string navigationUrl = navAction.NavigationPage != null ? navAction.NavigationPage.RelativeUrl : "";
            string refreshModules = navAction.RefreshModules == null ? "" : String.Join(",", navAction.RefreshModules);
            //Navigation zit nu in PostResult
//            moduleStartDiv += String.Format(@"<form method=""post"" name=""form{1}{0:N}"" id=""form{0:N}"" onsubmit1=""javascript:return submitPostableModule(this);"" enctype = ""multipart/form-data"">
//<input type=""hidden"" name=""hiddenModuleID"" value=""{0}""/>
//<input type=""hidden"" name=""hiddenModuleType"" value=""{1}""/>
//<input type=""hidden"" name=""hiddenModuleNavigationType"" value=""{2}""/>
//<input type=""hidden"" name=""hiddenRefreshModules"" value=""{3}""/>
//<input type=""hidden"" name=""hiddenNavigationUrl"" value=""{4}""/>
//<input type=""hidden"" id=""hiddenCurrentSubmitAction{0:N}"" name=""hiddenCurrentSubmitAction"" value=""{5}""/>
//<input type=""hidden"" id=""hiddenValidationRequired{0:N}"" value=""true""/>
//", this.ID, this.Type, navAction.NavigationType, refreshModules, navigationUrl, "Submit");

            moduleStartDiv += String.Format(@"<form method=""post"" name=""form{1}{0:N}"" id=""form{0:N}"" onsubmit1=""javascript:return BITSITESCRIPT.submitPostableModule(this);"" enctype = ""multipart/form-data"">
<input type=""hidden"" name=""hiddenIFramePost"" value=""true""/>
<input type=""hidden"" name=""hiddenModuleID"" value=""{0}""/>
<input type=""hidden"" name=""hiddenModuleType"" value=""{1}""/>
<!-- onderste twee fields worden overschreven door setAction van de knoppen in de module -->
<input type=""hidden"" id=""hiddenCurrentSubmitAction{0:N}"" name=""hiddenCurrentSubmitAction"" value=""Submit""/>
<input type=""hidden"" id=""hiddenValidationRequired{0:N}"" name=""hiddenValidationRequired"" value=""true""/>
", this.ID, this.Type);
            return moduleStartDiv;
        }

       

        protected PostResult getNewPostResult(ModuleNavigationAction navigationAction)
        {
            PostResult result = new PostResult();

            result.NavigationType = navigationAction.NavigationType;
            if (navigationAction.NavigationType == NavigationTypeEnum.NavigateToPage)
            {
                string navigationUrl = navigationAction.NavigationPage != null ? navigationAction.NavigationPage.RelativeUrl : "";

                result.NavigationUrl = navigationUrl;
            }
            else
            {
                string refreshModules = navigationAction.RefreshModules == null ? "" : String.Join(",", navigationAction.RefreshModules);
                result.RefreshModules = refreshModules;
            }
            return result;
        }

        protected string createSubmitButton(string action)
        {
            return createSubmitButton(action, true);
        }
        protected string createSubmitButton(string action, bool validationRequired)
        {
            return String.Format("<button type=\"submit\" onclick=\"BITSITESCRIPT.setCurrentButtonAction('{0}', '{1}', {2});\">", action, this.ID, validationRequired.ToString().ToLower());
        }

        internal PostResult HandlePostError(Exception ex)
        {
            try
            {
                string logMessage = "\r\nMESSAGE: " + ex.Message +
                 "\r\nSOURCE: " + ex.Source +
                 "\r\nTARGETSITE: " + ex.TargetSite +
                 "\r\nSTACKTRACE: " + ex.StackTrace;
                string path = ConfigurationManager.AppSettings["ErrorLogPath"];
                if (path == null || path == "") path = AppDomain.CurrentDomain.BaseDirectory;
                Logger.Log(path + @"\error_log_.txt", logMessage);
            }
            catch (Exception logEx) { }
            string errorMsg = "Er is een systeemfout op getreden: " + ex.Message;
            return new PostResult() { ErrorMessage = errorMsg };
        }
    }
}
