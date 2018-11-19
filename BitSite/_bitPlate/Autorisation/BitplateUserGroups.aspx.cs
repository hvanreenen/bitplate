using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;

namespace BitSite._bitPlate.Autorisation
{
    public partial class BitplateUserGroups : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();
            base.CheckPermissions(FunctionalityEnum.UserManagement, checkLicenseAlso: false);

            //BitSite.BitAutorisationService.AutorisationClient client = BitAutorisationServiceHelper.GetClient();
            //Functionality[] functionalities = client.GetFunctionalities(SessionObject.CurrentBitplateUser);
            
            LiteralUserGroupPermissions.Text = BuildFunctionsTree("Group");

            //vullen van select sites (dropdown)
            //LicenseSite[] sites = client.GetSites(SessionObject.CurrentBitplateUser, "Name");
            //foreach (LicenseSite site in sites)
            //{
            //    this.SelectSite.Items.Add(new ListItem(site.Name, site.ID.ToString()));
            //}
            //this.SelectSite.SelectedIndex = 0;

            //vullen van standaard permissionsets;
            string js = @"<script type='text/javascript'>";
            js += "BITAUTHUSERGROUPS.defaultPermissionSets['1'] = " + GetJavascriptDefaultPermissionsArray(UserTypeEnum.Moderators) + ";";
            js += "BITAUTHUSERGROUPS.defaultPermissionSets['2'] = " + GetJavascriptDefaultPermissionsArray(UserTypeEnum.Designers) + ";";
            js += "BITAUTHUSERGROUPS.defaultPermissionSets['9'] = " + GetJavascriptDefaultPermissionsArray(UserTypeEnum.SiteAdmins) + ";";
            js += "</script>";
            DefaultPermissionSets.Text = js;
            
        }

        private static string GetJavascriptDefaultPermissionsArray(UserTypeEnum userType)
        {
            //Functionality[] defaultFunctionalities = client.GetDefaultFunctionalitiesForUserGroup(SessionObject.CurrentBitplateUser, setName);
            string jsArray = "[";

            foreach (FunctionalityEnum funcEnumValue in Enum.GetValues(typeof(FunctionalityEnum)))
            {
                int functionNumber = (int)funcEnumValue;
                if (userType == UserTypeEnum.Moderators)
                {
                    if (functionNumber >= 1000 && functionNumber < 2000)
                    {
                        jsArray += functionNumber + ",";
                    }
                }
                else if (userType == UserTypeEnum.Designers)
                {
                    if (functionNumber >= 1000 && functionNumber < 3000)
                    {
                        jsArray += functionNumber + ",";
                    }
                }
                else if (userType == UserTypeEnum.SiteAdmins)
                {
                    jsArray += functionNumber + ",";
                }
            }
            
            jsArray = jsArray.Substring(0, jsArray.Length - 1);
            jsArray += "]";
            
            return jsArray;
        }

        /// <summary>
        /// bouwt boom van rechten
        /// type: users of usergroups
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// 
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
            string numbersPassed = "";
            foreach (int functionNumber in SessionObject.CurrentLicense.FunctionNumbers)
            {
    //bugfix voor dubbelnummers
                if(numbersPassed.Contains(functionNumber.ToString())) continue;
                numbersPassed += functionNumber + ",";
            //foreach (FunctionalityEnum funcEnumValue in Enum.GetValues(typeof(FunctionalityEnum)))
            //{
                //int functionNumber = (int)funcEnumValue;
                string functionName = Enum.GetName(typeof(FunctionalityEnum), functionNumber);

                if (functionNumber % 1000 == 0)
                {
                    //is parent
                    if (htmlLevel3 != "")
                    {
                        htmlLevel2 += String.Format(@"<div class='bitPermissionsPanelLevel2' id='bitPermissionsPanel{0}{1}' disabled='disabled'>", type, level3PerviousFunctionNumber);
                        htmlLevel2 += String.Format(@"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkboxAll{0}{1}' class='checkAll' value='{1}' type='checkbox'>
        <label for='checkboxAll{0}{1}'>Alles aan/uit</label><br />
", type, level3PerviousFunctionNumber);

                        htmlLevel2 += htmlLevel3;
                        htmlLevel2 += "</div>";
                    }
                    if (htmlLevel2 != "")
                    {
                        htmlLevel1 += String.Format(@"<div class='bitPermissionsPanelLevel2' id='bitPermissionsPanel{0}{1}' disabled='disabled'>", type, level2PerviousFunctionNumber);
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
                        htmlLevel2 += String.Format(@"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkboxAll{0}{1}' class='checkAll' value='{1}' type='checkbox'>
        <label for='checkboxAll{0}{1}'>Alles aan/uit</label><br />
", type, level3PerviousFunctionNumber);

                        htmlLevel2 += htmlLevel3;
                        htmlLevel2 += "</div>";
                    }
                    if (htmlLevel2 != "")
                    {
                        htmlLevel1 += String.Format(@"<div class='bitPermissionsPanelLevel2' id='bitPermissionsPanel{0}{1}' disabled='disabled'>", type, level2PerviousFunctionNumber);
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


                    level2PerviousFunctionNumber = functionNumber;
                }
                else if (functionNumber % 10 == 0)
                {
                    if (htmlLevel3 != "")
                    {
                        htmlLevel2 += String.Format(@"<div class='bitPermissionsPanelLevel2' id='bitPermissionsPanel{0}{1}' disabled='disabled'>", type, level3PerviousFunctionNumber);
                        
                        htmlLevel2 += String.Format(@"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkboxAll{0}{1}' class='checkAll' value='{1}' type='checkbox'>
        <label for='checkboxAll{0}{1}'>Alles aan/uit</label><br />
", type, level3PerviousFunctionNumber);

                        htmlLevel2 += htmlLevel3;
                        htmlLevel2 += "</div>";
                    }
                    htmlLevel3 = "";
                    htmlLevel2 += String.Format(@"    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkbox{0}{1}' value='{1}' class='checkParent' type='checkbox'> 
    <label for='checkbox{0}{1}'>{2}</label><br/>", type, functionNumber, functionName);
                    

                    level3PerviousFunctionNumber = functionNumber;
                }
                else
                {
                    htmlLevel3 += String.Format(@"    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkbox{0}{1}' value='{1}' class='checkParent' type='checkbox'> 
    <label for='checkbox{0}{1}'>{2}</label><br/>", type, functionNumber, functionName);
                    
                }

            }

            return html;
        }
//        private string BuildFunctionsTree_old(Functionality[] functionalities, string type)
//        {
//            string html = ""; //all
//            string htmlLevel1 = ""; //parent
//            string htmlLevel2 = ""; //sub
//            string htmlLevel3 = ""; //sub-sub
            
//            string level1PerviousFunctionID = "";
//            string level2PerviousFunctionID = "";
//            string level3PerviousFunctionID = "";
            
//            int level1PerviousFunctionNumber = 0;
//            int level2PerviousFunctionNumber = 0;
//            int level3PerviousFunctionNumber =0;

//            License license = SessionObject.CurrentLicense;
            
            
//            foreach (Functionality func in functionalities)
//            {
//                int functionNumber = (int)func.FunctionNumber;

//                //if (SessionObject.CurrentBitplateUser.GetSuperUserType() != SuperUserTypeEnum.Developers)
//                //{
//                //    //als er in de licentie geen rechten zijn daan dpoorgaan naar de volgende
//                //    if (license == null) { continue; }
//                //    if (!license.HasPermission(func.FunctionNumber)) { continue; }
//                //}
                
//                string functionID = func.ID.ToString();
//                if (functionNumber % 1000 == 0)
//                {
//                    //is parent
//                    if (htmlLevel1 != "")
//                    {
//                        //checkbox all toevoegen van vorige
//                        html += String.Format(@"<div class='dockPanelContent open bitPermissionsPanelLevel1' id='bitPermissionsPanel{0}{1}' disabled='disabled'>
//        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkboxAll{0}{1}' class='checkAll' value='{1}' data-function-number='{2}' type='checkbox'>
//        <label for='checkboxAll{0}{1}'>Alles aan/uit</label><br />
//", type, level1PerviousFunctionID, level1PerviousFunctionNumber);
//                        html += htmlLevel1;
//                        html += "</div>";
//                    }
//                    htmlLevel1 = ""; 
//                    htmlLevel2 = ""; 
//                    htmlLevel3 = "";
                    
//                    //volgende toevoegen
//                    html += String.Format(@"<div class='dockPanelTitle'>
//    <span class='arrow'>&#9660;</span>
//    <input id='checkbox{0}{1}' class='checkParent' value='{1}' data-function-number='{3}' type='checkbox'>
//    <label for='checkbox{0}{1}'>{2}</label>
//</div>", type, functionID, func.Name, functionNumber);
//                    level1PerviousFunctionID = functionID;
//                    level1PerviousFunctionNumber = functionNumber;
//                }
//                else if (functionNumber % 100 == 0)
//                {
//                    if (htmlLevel2 != "")
//                    {
//                        htmlLevel1 += String.Format(@"<div class='bitPermissionsPanelLevel2' id='bitPermissionsPanel{0}{1}' disabled='disabled'>
//        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkboxAll{0}{1}' class='checkAll' value='{1}' type='checkbox'>
//        <label for='checkboxAll{0}{1}'>Alles aan/uit</label><br />
//", type, level2PerviousFunctionID);
//                        htmlLevel1 += htmlLevel2;
//                        htmlLevel1 += "</div>";
//                    }
//                    htmlLevel2 = "";
//                    htmlLevel3 = "";
                    
//                    htmlLevel1 += String.Format(@"    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkbox{0}{1}' value='{1}' class='checkParent' type='checkbox'> 
//    <label for='checkbox{0}{1}'>{2}</label><br/>", type, functionID, func.Name);
//                    level2PerviousFunctionID = functionID;
//                    level2PerviousFunctionNumber = functionNumber;
//                }
//                else if (functionNumber % 10 == 0)
//                {
//                    if (htmlLevel3 != "")
//                    {
//                        htmlLevel2 += String.Format(@"<div class='bitPermissionsPanelLevel2' id='bitPermissionsPanel{0}{1}' disabled='disabled'>
//        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkboxAll{0}{1}' class='checkAll' value='{1}' type='checkbox'>
//        <label for='checkboxAll{0}{1}'>Alles aan/uit</label><br />
//", type, level3PerviousFunctionID);
//                        htmlLevel2 += htmlLevel3;
//                        htmlLevel2 += "</div>";
//                    }
//                    htmlLevel3 = ""; 
//                    htmlLevel2 += String.Format(@"    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkbox{0}{1}' value='{1}' class='checkParent' type='checkbox'> 
//    <label for='checkbox{0}{1}'>{2}</label><br/>", type, functionID, func.Name);
//                    level3PerviousFunctionID = functionID;
//                    level3PerviousFunctionNumber = functionNumber;
//                }
//                else
//                {
//                    htmlLevel3 += String.Format(@"    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id='checkbox{0}{1}' value='{1}' class='checkParent' type='checkbox'> 
//    <label for='checkbox{0}{1}'>{2}</label><br/>", type, functionID, func.Name);
//                }
                
//            }
            
//            return html;
//        }
   }
}