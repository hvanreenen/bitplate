using BitPlate.Domain.Autorisation;
using BitSite._bitPlate.bitAjaxServices;
using HJORM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate
{
    public partial class Login : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //als gebruiker met meerdere site heeft gewisseld naar een andere site dan krijgt die een tempkey mee vanuit de metaserver.
            //deze temp key checken in metaserver, bestaat die, dan autom. inloggen met deze gebruiker 
            if (Request.QueryString["loginkey"] != null)
            {
                UserServiceReference.UserServiceClient client = BitMetaServerServicesHelper.GetUserServiceClient();
                MultiSiteUser tempUser = client.CheckTempLoginKey(Request.QueryString["loginkey"]);
                using (AuthService authService = new AuthService())
                {
                    authService.Login(tempUser.Email, null, tempUser.Theme, tempUser.Language, false, tempUser.Password);
                }
               
            }

            if (SessionObject.CurrentBitplateUser != null)
            {
                Session.Clear();
                Session.Abandon();

                
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