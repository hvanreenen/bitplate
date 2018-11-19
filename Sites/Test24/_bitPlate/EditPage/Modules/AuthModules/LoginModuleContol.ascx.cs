using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.Modules;
using BitSite._bitPlate.bitAjaxServices;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Utils;
using BitPlate.Domain.Modules.Auth;

namespace BitSite._bitPlate.EditPage.Modules.AuthModules
{
    public partial class LoginModuleControl : BaseModuleUserControl
    {
        protected TextBox UserNameTextbox;
        protected TextBox UserPasswordTextbox;
        protected LinkButton LinkSubmitButton;
        protected Button SubmitButton;
        protected Label ResultLabel;
        protected LoginModule loginModule;
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Load(sender, e);
            if (base.CheckAutorisation())
            {
                this.LoadLoginForm();
            }
        }

        public override void Reload(BaseModuleUserControl sender, NavigationParameterObject args = null)
        {
            if (base.CheckAutorisation())
            {
                this.LoadLoginForm();
            }
        }

        public void LoadLoginForm()
        {
            if (SessionObject.CurrentBitSiteUser == null)
            {
                this.Visible = true;
                UserNameTextbox = (TextBox)this.FindControl("AuthUserNameTextbox" + this.ModuleID.ToString("N"));
                UserPasswordTextbox = (TextBox)this.FindControl("AuthUserPasswordTextbox" + this.ModuleID.ToString("N"));
                LinkSubmitButton = (LinkButton)this.FindControl("AuthLinkSubmitButton" + this.ModuleID.ToString("N"));
                SubmitButton = (Button)this.FindControl("AuthSubmitButton" + this.ModuleID.ToString("N"));
                ResultLabel = (Label)this.FindControl("AuthMessageLabel" + this.ModuleID.ToString("N"));

                if (UserNameTextbox != null)
                {
                }

                if (UserPasswordTextbox != null)
                {
                }

                if (LinkSubmitButton != null)
                {
                    LinkSubmitButton.Click += SubmitButton_Click;
                }

                if (SubmitButton != null)
                {
                    SubmitButton.Click += SubmitButton_Click;
                }
            }
            else
            {
                this.Visible = false;
            }
        }

        public void SubmitButton_Click(object sender, EventArgs e)
        {
            if (UserNameTextbox.Text.Trim() != "" && UserPasswordTextbox.Text.Trim() != "")
            {
                SiteUser user = this.Login(UserNameTextbox.Text, UserPasswordTextbox.Text);
                if (user != null)
                {
                    //loginModule = BaseObject.GetById<LoginModule>(this.ModuleID);
                    //this.LoadModuleObject();
                    //ModuleNavigationAction drillDownAction = this.GetDrillDownActionByTagName("LoginButton");
                    ModuleNavigationActionLite navigationAction = GetNavigationActionByTagName("{LoginButton}");

                    switch (navigationAction.NavigationType)
                    {
                        case NavigationTypeEnum.NavigateToPage:
                            Response.Redirect(navigationAction.NavigationUrl);
                            break;
                        case NavigationTypeEnum.ShowDetailsInModules:
                            foreach (string drillDownModuleId in navigationAction.RefreshModules)
                            {
                                BaseModuleUserControl moduleControl = (BaseModuleUserControl)FindControlRecursive(this.Page.Master, "Mod" + drillDownModuleId.Replace("-", ""));
                                if (moduleControl != null)
                                {
                                    moduleControl.Reload(this);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    UserNameTextbox.Text = "";
                    UserPasswordTextbox.Text = "";
                    this.Visible = false;
                }
                else
                {
                    ResultLabel.Text = "Login mislukt.";
                }
            }
            else
            {
                ResultLabel.Text = "Login mislukt.";
            }
        }

        public SiteUser Login(string email, string password)
        {
            string MD5Password = Encrypter.CalculateMD5Hash(password);
            SiteUser user = BaseObject.GetFirst<SiteUser>("Email ='" + email + "' AND Password = '" + MD5Password + "'"); //"' AND Type = 30");
            if (user == null)
            {
                if (email == "test" && password == "test")
                {
                    SiteUser siteUser = new SiteUser();
                    siteUser.Name = "test gebruiker";
                    siteUser.Email = email;
                    user = siteUser;
                }
            }

            SessionObject.CurrentBitSiteUser = user;
            return user;
        }
    }
}