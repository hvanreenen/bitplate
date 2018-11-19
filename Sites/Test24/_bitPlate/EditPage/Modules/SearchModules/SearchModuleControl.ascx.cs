using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain.Modules;
namespace BitSite._bitPlate.EditPage.Modules.SearchModules
{
    public partial class SearchModuleControl : BaseModuleUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (base.CheckAutorisation())
            {
            }
            string searchString = "";
            TextBoxSearch = (TextBox)FindControl("TextBoxSearch" + ModuleID.ToString("N"));
            if (Request.QueryString["Search"] != null)
            {
                searchString = Request.QueryString["Search"];
                if (!IsPostBack)
                {
                    TextBoxSearch.Text = searchString;
                }
            }

            //base.LoadModule();
        }

        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            ModuleNavigationActionLite navigationAction = GetNavigationActionByTagName("{SearchLink}");
            if (navigationAction == null) return;
            if (navigationAction.NavigationType == NavigationTypeEnum.NavigateToPage && navigationAction.NavigationUrl != null)
            {
                Response.Redirect(navigationAction.NavigationUrl + "?Search=" + TextBoxSearch.Text);
            }
            else if (navigationAction.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {
                foreach (string drillDownModuleId in navigationAction.RefreshModules)
                {
                    SearchResultsModuleControl searchResultsControl = (SearchResultsModuleControl)FindControlRecursive(this.Page.Master, "Mod" + drillDownModuleId.Replace("-", "_"));
                    if (searchResultsControl != null)
                    {
                        searchResultsControl.ShowResults(TextBoxSearch.Text);
                    } 
                }
            }
        }
    }
}