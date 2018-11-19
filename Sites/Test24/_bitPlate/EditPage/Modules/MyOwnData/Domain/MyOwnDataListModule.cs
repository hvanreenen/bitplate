using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HJORM.Attributes;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Utils;

namespace BitSite._bitPlate._bitModules.MyOwnData.Domain
{
    [Persistent("Module")]
    public class MyOwnDataListModule: Module
    {
        public MyOwnDataListModule()
        {
            this.Type = "MyOwnDataModule";
            this.Content = "<p>test</p>";
        }
        
        //public string BTWType { get; set; }
        public override string Publish()
        {
            string html = GetUserControlDeclarative();
            html += this.getModuleStartDiv();
            html += this.getModuleContextMenu();
            html += this.Content;
            //2do replace module-tags
            html += "</div>";

            //save
            string controlName = GetUserControlName();
            string fileName = this.Site.Path + "PageControls\\" + controlName + ".ascx";

            FileHelper.WriteFile(fileName, html);
            return String.Format(@"<mod:{0} runat=""server"" id=""Mod{1}"" />", controlName, this.ID.ToString("N"));

        }

        private string getModuleContextMenu()
        {
            string html = String.Format(@"
            <ul class=""bitModuleContextMenu"" style=""display1: none; width: 150px; z-index: 500"">
        <li><asp:LinkButton ID=""LinkButtonConfigModule{0}"" OnClick=""LinkButtonConfigModule_Click"" runat=""server"" CommandArgument=""{0}"">Eigenschappen</asp:LinkButton>
</li>
       </ul>", this.ID.ToString("N")); ;
            return html;
        }

        public override string GetRegisterDeclarative()
        {
            return String.Format(@"<%@ Register Src=""~/PageControls/{0}.ascx"" TagPrefix=""mod"" TagName=""{0}"" %>", GetUserControlName());
            
        }

        public override string GetUserControlName()
        {
            if (Page != null)
            {
                return this.Page.Name.Replace(".aspx", "") + "_" + this.Type + "_" + this.ID.ToString("N");
            }
            else
            {
                return "";
            }
        }

        public override string GetUserControlDeclarative()
        {
            return @"<%@ Control Language=""C#"" AutoEventWireup=""true"" Inherits=""BitSite._bitPlate._bitModules.MyOwnData.MyOwnListModule"" %>
";
        }
       
    }
}