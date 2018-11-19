using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitMetaServer
{
    public partial class Login : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           

            if (SessionObject.CurrentUser != null)
            {
                
              Response.Redirect("/Default.aspx");
                
            }
            else
            {
                this.ShowLoginPage();
            }
            
        }

        protected void ShowLoginPage()
        {
            if (Request.QueryString["SessionExpired"] != null)
            {
                LabelMsg.Text = "De sessie is verlopen. U dient zich opnieuw aan te melden.";
                LabelMsg.Visible = true;
            }
            ListItem li = new ListItem();
            li.Value = "";
            li.Text = "Selecteer een Thema.";
            this.selectTheme.Items.Add(li);
            /* foreach (string theme in Directory.GetDirectories(Server.MapPath("") + "..\\_themes\\")) //Uitgeschakeld omdat de overige templates toch niet werken.
            {
                DirectoryInfo di = new DirectoryInfo(theme);
                this.selectTheme.Items.Add(di.Name);
            } */
            this.selectTheme.Items.Add("bitplate");

            if (Request.Cookies["BITAUTHORISATION"] != null)
            {
                HttpCookie AuthCookie = Request.Cookies["BITAUTHORISATION"];
                username.Value = AuthCookie["UserName"];
                //password.Value = AuthCookie["Password"];
                save.Checked = true;
            }
        }
    }
}