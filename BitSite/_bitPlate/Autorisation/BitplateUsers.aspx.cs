using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Licenses;
using HJORM;

namespace BitSite._bitPlate.Autorisation
{
    public partial class BitplateUsers : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();
            base.CheckPermissions(FunctionalityEnum.UserManagement, checkLicenseAlso: false);

            LiteralUserPermissions.Text = BuildFunctionsTree("User");
            literalUserGroupList.Text = BuildUserGroupsList();

        }

        private string BuildUserGroupsList()
        {
            string format = @"          <li class=""ui-state-default bitScript"" style=""margin-top: 2px; margin-bottom: 2px;padding: 5px"">
                <a href=""javascript:BITAUTHUSERS.addUserGroupPerUser('{0}', '{1}');"">
                    <input type=""hidden"" value=""{0}"" />
                    <span>{1}</span> <span class=""remove-script btn-remove-script""></span>
                </a>
            </li>
";
            string returnValue = "";
            string where = String.Format("FK_Site = '{0}'", SessionObject.CurrentSite.ID);
            BaseCollection<BitplateUserGroup> userGroups = BaseCollection<BitplateUserGroup>.Get(where, "Name");
            foreach (BitplateUserGroup userGroup in userGroups)
            {
                returnValue += String.Format(format, userGroup.ID, userGroup.Name);
            }
            return returnValue;

        }


        /// <summary>
        /// bouwt boom van rechten
        /// type: users of usergroups
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string BuildFunctionsTree(string type)
        {
            string html = ""; //all
            string htmlLevel1 = ""; //parent
            string htmlLevel2 = ""; //sub
            string htmlLevel3 = ""; //sub-sub

            int level1PerviousFunctionNumber = 0;
            int level2PerviousFunctionNumber = 0;
            int level3PerviousFunctionNumber = 0;
            string numbersPassed = "";
            foreach (int functionNumber in SessionObject.CurrentLicense.FunctionNumbers)
            {
                //bugfix voor dubbelnummers
                if (numbersPassed.Contains(functionNumber.ToString())) continue;
                numbersPassed += functionNumber + ",";

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
    }
}