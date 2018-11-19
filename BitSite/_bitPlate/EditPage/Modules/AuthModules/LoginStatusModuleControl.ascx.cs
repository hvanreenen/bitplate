using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate.EditPage.Modules.AuthModules
{
    public partial class LoginStatusModuleControl : BaseModuleUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Load(sender, e);
            if (base.CheckAutorisation())
            {
                this.BindUserData();
                Button LogoutButton = (Button)this.FindControlRecursive(this, "LogoutButton" + this.ModuleID.ToString("N"));
                if (LogoutButton != null)
                {
                    LogoutButton.Click += LogoutButton_Click;
                }

                LinkButton LogoutLinkButton = (LinkButton)this.FindControlRecursive(this, "LogoutLinkButton" + this.ModuleID.ToString("N"));
                if (LogoutLinkButton != null)
                {
                    LogoutLinkButton.Click += LogoutButton_Click;
                }
            }
        }

        public override void Reload(BaseModuleUserControl sender, NavigationParameterObject args = null)
        {
            if (base.CheckAutorisation())
            {
                this.BindUserData();
            }
        }

        private void BindUserData()
        {
            if (SessionObject.CurrentBitplateUser != null)
            {
                DataBind(SessionObject.CurrentBitplateUser);
            }
            else
            {
                DataBind(SessionObject.CurrentBitSiteUser);

            }
        }

        protected void DataBind(BaseUser user)
        {
            FormView formView = (FormView)this.FindControl("FormView" + this.ModuleID.ToString("N"));
            if (formView != null)
            {
                formView.DataSource = (user != null) ? new List<BaseUser> { user } : null;

                formView.DataBind();
            }



        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            SessionObject.CurrentBitSiteUser = null;
            DataBind(null);
            ModuleNavigationActionLite navigationAction = GetNavigationActionByTagName("{LogoutButton}");

            if (navigationAction.NavigationType == BitPlate.Domain.Modules.NavigationTypeEnum.NavigateToPage)
            {
                if (navigationAction.NavigationUrl != null)
                {
                    Response.Redirect(navigationAction.NavigationUrl);
                }
            }
            else if (navigationAction.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {
                foreach (string drillDownModuleId in navigationAction.RefreshModules)
                {
                    BaseModuleUserControl moduleControl = (BaseModuleUserControl)FindControlRecursive(this.Page.Master, "Mod" + drillDownModuleId.Replace("-", ""));
                    if (moduleControl != null)
                    {
                        moduleControl.Reload(this);
                    }
                }
            }
            //else
            //{
            //    this.Visible = false;
            //}
        }
    }
}