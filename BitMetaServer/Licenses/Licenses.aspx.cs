using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;

namespace BitMetaServer.Licenses
{
    public partial class Licenses : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLogin();
            //base.CheckPermissions(FunctionalityEnum.LicenseManagement);
            
            

            //BitSite.BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
            //Functionality[] functionalities = client.GetFunctionalities(SessionObject.CurrentBitplateUser);

            LiteralPermissions.Text = BuildFunctionsTree("License");

            //vullen van standaard permissionsets;
            string js = @"<script type='text/javascript'>";
            js += GetJavascriptDefaultsArray((int)LicenseTypeEnum.BitplateLite);
            js += GetJavascriptDefaultsArray((int)LicenseTypeEnum.BitplateStandard);
            js += GetJavascriptDefaultsArray((int)LicenseTypeEnum.BitplateCorporate);
            js += GetJavascriptDefaultsArray((int)LicenseTypeEnum.BitplateEnterprise);
            js += GetJavascriptDefaultsArray((int)LicenseTypeEnum.BitplateCustom);
            js += GetJavascriptDefaultsArray((int)NewsletterLicenseTypeEnum.BitnewsletterLite);
            js += GetJavascriptDefaultsArray((int)NewsletterLicenseTypeEnum.BitnewsletterCorporate);
            js += GetJavascriptDefaultsArray((int)NewsletterLicenseTypeEnum.BitnewsletterCustom);
            js += GetJavascriptDefaultsArray((int)WebshopLicenseTypeEnum.BitshopLite);
            js += GetJavascriptDefaultsArray((int)WebshopLicenseTypeEnum.BitshopCorporate);
            js += GetJavascriptDefaultsArray((int)WebshopLicenseTypeEnum.BitshopXtra);
            js += GetJavascriptDefaultsArray((int)WebshopLicenseTypeEnum.BitshopCustom);
            js += "</script>";
            DefaultPermissionSets.Text = js;


            //client.Close();

        }

        private string BuildFunctionsTree(string type)
        {
            string html = ""; //all
            string htmlLevel1 = ""; //parent
            string htmlLevel2 = ""; //sub
            string htmlLevel3 = ""; //sub-sub

            //string level1PerviousFunctionID = "";
            //string level2PerviousFunctionID = "";
            //string level3PerviousFunctionID = "";

            int level1PerviousFunctionNumber = 0;
            int level2PerviousFunctionNumber = 0;
            int level3PerviousFunctionNumber = 0;

            foreach (FunctionalityEnum funcEnumValue in Enum.GetValues(typeof(FunctionalityEnum)))
            {
                int functionNumber = (int)funcEnumValue;
                string functionName = Enum.GetName(typeof(FunctionalityEnum), funcEnumValue);

                if (functionNumber % 1000 == 0)
                {
                    //is parent
                    if (htmlLevel3 != "")
                    {
                        htmlLevel2 += String.Format(@"<div class='bitPermissionsPanelLevel2' id='bitPermissionsPanel{0}{1}' disabled='disabled'>", type, level3PerviousFunctionNumber);
                        htmlLevel2 += tryAddMaxNumberofTextBox(level3PerviousFunctionNumber, 2);
                        htmlLevel2 += String.Format(@"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkboxAll{0}{1}' class='checkAll' value='{1}' type='checkbox'>
        <label for='checkboxAll{0}{1}'>Alles aan/uit</label><br />
", type, level3PerviousFunctionNumber);

                        htmlLevel2 += htmlLevel3;
                        htmlLevel2 += "</div>";
                    }
                    if (htmlLevel2 != "")
                    {
                        htmlLevel1 += String.Format(@"<div class='bitPermissionsPanelLevel2' id='bitPermissionsPanel{0}{1}' disabled='disabled'>", type, level2PerviousFunctionNumber);
                        htmlLevel1 += tryAddMaxNumberofTextBox(level2PerviousFunctionNumber, 1);
                        htmlLevel1 += String.Format(@"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkboxAll{0}{1}' class='checkAll' value='{1}' type='checkbox'>
        <label for='checkboxAll{0}{1}'>Alles aan/uit</label><br />
", type, level2PerviousFunctionNumber);

                        htmlLevel1 += htmlLevel2;
                        htmlLevel1 += "</div>";
                    }
                    if (htmlLevel1 != "")
                    {
                        //checkbox all toevoegen van vorige
                        html += String.Format(@"<div class='dockPanelContent open bitPermissionsPanelLevel1' id='bitPermissionsPanel{0}{1}' disabled='disabled'>", type, level1PerviousFunctionNumber);
                        html += tryAddMaxNumberofTextBox(level1PerviousFunctionNumber, 0);
                        html += String.Format(@"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkboxAll{0}{1}' class='checkAll' value='{1}' data-function-number_old='{2}' type='checkbox'>
        <label for='checkboxAll{0}{1}'>Alles aan/uit</label><br />
", type, level1PerviousFunctionNumber, level1PerviousFunctionNumber);
                        html += htmlLevel1;
                        html += "</div>";
                    }
                    htmlLevel1 = "";
                    htmlLevel2 = "";
                    htmlLevel3 = "";

                    //volgende toevoegen
                    html += String.Format(@"<div class='dockPanelTitle'>
    <span class='arrow'>&#9660;</span>
    <input id='checkbox{0}{1}' class='checkParent' value='{1}' data-function-number='{1}' type='checkbox'>
    <label for='checkbox{0}{1}'>{2}</label>
</div>", type, functionNumber, functionName);

                    level1PerviousFunctionNumber = functionNumber;
                }
                else if (functionNumber % 100 == 0)
                {
                    if (htmlLevel3 != "")
                    {
                        htmlLevel2 += String.Format(@"<div class='bitPermissionsPanelLevel2' id='bitPermissionsPanel{0}{1}' disabled='disabled'>", type, level3PerviousFunctionNumber);
                        htmlLevel2 += tryAddMaxNumberofTextBox(level3PerviousFunctionNumber, 2);
                        htmlLevel2 += String.Format(@"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkboxAll{0}{1}' class='checkAll' value='{1}' type='checkbox'>
        <label for='checkboxAll{0}{1}'>Alles aan/uit</label><br />
", type, level3PerviousFunctionNumber);

                        htmlLevel2 += htmlLevel3;
                        htmlLevel2 += "</div>";
                    }
                    if (htmlLevel2 != "")
                    {
                        htmlLevel1 += String.Format(@"<div class='bitPermissionsPanelLevel2' id='bitPermissionsPanel{0}{1}' disabled='disabled'>", type, level2PerviousFunctionNumber);
                        htmlLevel1 += tryAddMaxNumberofTextBox(level2PerviousFunctionNumber, 1);
                        htmlLevel1 += String.Format(@"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkboxAll{0}{1}' class='checkAll' value='{1}' type='checkbox'>
        <label for='checkboxAll{0}{1}'>Alles aan/uit</label><br />
", type, level2PerviousFunctionNumber);

                        htmlLevel1 += htmlLevel2;
                        htmlLevel1 += "</div>";
                    }
                    htmlLevel2 = "";
                    htmlLevel3 = "";

                    htmlLevel1 += String.Format(@"    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkbox{0}{1}' value='{1}' class='checkParent' type='checkbox'> 
    <label for='checkbox{0}{1}'>{2}</label><br/>", type, functionNumber, functionName);
                    //htmlLevel1 += tryAddMaxNumberofTextBox(functionNumber, 2);
                    
                    level2PerviousFunctionNumber = functionNumber;
                }
                else if (functionNumber % 10 == 0)
                {
                    if (htmlLevel3 != "")
                    {
                        htmlLevel2 += String.Format(@"<div class='bitPermissionsPanelLevel2' id='bitPermissionsPanel{0}{1}' disabled='disabled'>", type, level3PerviousFunctionNumber);
                        htmlLevel2 += tryAddMaxNumberofTextBox(level3PerviousFunctionNumber, 2);
                        htmlLevel2 += String.Format(@"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkboxAll{0}{1}' class='checkAll' value='{1}' type='checkbox'>
        <label for='checkboxAll{0}{1}'>Alles aan/uit</label><br />
", type, level3PerviousFunctionNumber);

                        htmlLevel2 += htmlLevel3;
                        htmlLevel2 += "</div>";
                    }
                    htmlLevel3 = "";
                    htmlLevel2 += String.Format(@"    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkbox{0}{1}' value='{1}' class='checkParent' type='checkbox'> 
    <label for='checkbox{0}{1}'>{2}</label><br/>", type, functionNumber, functionName);
                    //htmlLevel2 += tryAddMaxNumberofTextBox(functionNumber, 3);
                    
                    level3PerviousFunctionNumber = functionNumber;
                }
                else
                {
                    htmlLevel3 += String.Format(@"    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkbox{0}{1}' value='{1}' class='checkParent' type='checkbox'> 
    <label for='checkbox{0}{1}'>{2}</label><br/>", type, functionNumber, functionName);
                    htmlLevel3 += tryAddMaxNumberofTextBox(functionNumber, 3);
                }

            }

            return html;
        }

        private string tryAddMaxNumberofTextBox(int functionNumber, int level)
        {
            string returnHtml = String.Empty;
            if (functionNumber == (int)FunctionalityEnum.Pages)
            {
                returnHtml = "Max. aantal pagina's per cms: <input type='text' data-field='MaxNumberOfPages' data-validation='number' class='textboxInteger'/><br/>";
                for (int i = 0; i <= level; i++)
                {
                    returnHtml += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                }
                returnHtml += " Max. aantal mappen per cms: <input type='text' data-field='MaxNumberOfPageFolders' data-validation='number' class='textboxInteger'/><br/>";
            
            }
            
            else if (functionNumber == (int)FunctionalityEnum.Templates)
            {
                returnHtml = "Max. aantal templates per cms: <input type='text' data-field='MaxNumberOfTemplates' data-validation='number' class='textboxInteger'/><br/>";
            }
            else if (functionNumber == (int)FunctionalityEnum.Stylesheets)
            {
                returnHtml = "Max. aantal stylesheets per cms: <input type='text' data-field='MaxNumberOfStylesheets' data-validation='number' class='textboxInteger'/><br/>";
            }
            else if (functionNumber == (int)FunctionalityEnum.Scripts)
            {
                returnHtml = "Max. aantal scripts per cms: <input type='text' data-field='MaxNumberOfScripts' data-validation='number' class='textboxInteger'/><br/>";
            }
            else if (functionNumber == (int)FunctionalityEnum.DataCollections)
            {
                returnHtml = "Max. aantal datacollections per cms: <input type='text' data-field='MaxNumberOfDataCollections' data-validation='number' class='textboxInteger'/><br/>";
                //returnHtml = "Max. aantal rijen per datacollections <input type='text' data-field='MaxNumberOfDataPerDataCollection' data-validation='number' class='textboxInteger'/><br/>";
            }
            else if (functionNumber == (int)FunctionalityEnum.NewsLetters)
            {
                returnHtml = "Max. aantal nieuwsbrieven per cms: <input type='text' data-field='MaxNumberOfNewsletters' data-validation='number' class='textboxInteger'/><br/>";
                for (int i = 0; i <= level; i++)
                {
                    returnHtml += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                }
                returnHtml += " Max. aantal verzendingen per cms: <input type='text' data-field='MaxNumberOfNewsletterMailings' data-validation='number' class='textboxInteger'/><br/>";
                for (int i = 0; i <= level; i++)
                {
                    returnHtml += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                }
                returnHtml += " Max. aantal abonnees per cms: <input type='text' data-field='MaxNumberOfNewsletterSubscribers' data-validation='number' class='textboxInteger'/><br/>";
            
            }
            

            //inspringen toevoegen
            if (returnHtml != String.Empty)
            {
                for (int i = 0; i <= level; i++)
                {
                    returnHtml = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + returnHtml;
                }
                returnHtml = "&nbsp;" + returnHtml;
            }
            return returnHtml;
        }

        private static string GetJavascriptDefaultsArray(int enumNumber)
        {
            List<int> defaultFunctionalityNumbers = License.LoadDefaultFunctionNumbersByType(enumNumber);

            string jsArray = "[";
            foreach (int funcNumber in defaultFunctionalityNumbers)
            {
                jsArray += funcNumber + ",";
            }

            if (jsArray != "[")
            {
                jsArray = jsArray.Substring(0, jsArray.Length - 1);
            }
            jsArray += "]";
            string returnValue = String.Format("BITLICENSES.defaultPermissionSets['{0}'] = {1};\r\n", enumNumber, jsArray);


            //Maxnumbers
            List<string> defaultValues = License.LoadDefaultValuesByType(enumNumber);
            //new string[] { "MaxNumberOfSites:1", "MaxNumberOfPages:100", "MaxNumberOfTemplates:10" };
            //

            jsArray = "{";
            foreach (string defaultValue in defaultValues)
            {
                jsArray += defaultValue + ",";
            }
            if (jsArray != "{")
            {
                jsArray = jsArray.Substring(0, jsArray.Length - 1);
            }
            jsArray += "};";
            returnValue += String.Format("BITLICENSES.defaultValues['{0}'] = {1};\r\n", enumNumber, jsArray);

            return returnValue;
        }
    }
}