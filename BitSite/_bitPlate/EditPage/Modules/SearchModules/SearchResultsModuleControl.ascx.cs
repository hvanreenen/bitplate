using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Search;

namespace BitSite._bitPlate.EditPage.Modules.SearchModules
{
    public partial class SearchResultsModuleControl : BaseModuleUserControl
    {
        string searchString = "";
        bool usePaging = false;
        PlaceHolder placeHolderPaging = null;
        int pageSize = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!base.CheckAutorisation())
            {
                return;
            }

            if (Request.QueryString["Search"] != null)
            {
                searchString = Request.QueryString["Search"];
                pageSize = base.getSetting<int>("MaxNumberOfRows");
                placeHolderPaging = (PlaceHolder)FindControl("PlaceHolderPaging" + ModuleID.ToString("N"));
                usePaging = ((pageSize > 0) && (placeHolderPaging != null));
                if (!IsPostBack)
                {
                    ViewState["TotalResults"] = ShowResults(searchString);
                    //ViewState["CurrentPage"] = 0;
                }
                //CreatePager(Convert.ToInt32(ViewState["CurrentPage"]), Convert.ToInt32(ViewState["TotalResults"]));
                CreatePager(0, Convert.ToInt32(ViewState["TotalResults"]));
            
            }
        }
        public int ShowResults(string searchString)
        {
            return ShowResults(searchString, pageSize, 0);
        }

        public int ShowResults(string searchString, int pageSize, int currentPage)
        {
            List<SearchResultItem> totalResults = BitSite._services.SearchService.Find(searchString, "", 0, 0, false);
            Repeater = (Repeater)FindControl("Repeater" + ModuleID.ToString("N"));
            if (usePaging)
            {
                PagedDataSource pagedResults = new PagedDataSource();
                pagedResults.DataSource = totalResults;
                pagedResults.AllowPaging = true;
                pagedResults.PageSize = pageSize;
                pagedResults.CurrentPageIndex = currentPage;


                Repeater.DataSource = pagedResults;
            }
            else
            {
                Repeater.DataSource = totalResults;
            }
            Repeater.DataBind();

            Label labelNumberOfResults = (Label)FindControl("LabelNumberOfResults" + ModuleID.ToString("N"));
            if (labelNumberOfResults != null)
            {
                labelNumberOfResults.Text = totalResults.Count.ToString();
            }

            if (totalResults.Count == 0)
            {
                Panel panelNoresults = (Panel)FindControl("PanelNoResults" + ModuleID.ToString("N"));
                if (panelNoresults != null)
                {
                    panelNoresults.Visible = true;
                }
            }
            return totalResults.Count;
        }

        public void CreatePager(int currentPage, int totalResults)
        {
            //placeHolderPaging.Controls.Clear(); regel verhuist naar beneden.
            if (placeHolderPaging != null)
            {
                placeHolderPaging.Controls.Clear();
                if (totalResults == 0 || pageSize == 0)
                {
                    placeHolderPaging.Visible = false;
                }
                else
                {
                    for (int i = 0; i <= (totalResults / pageSize); i++)
                    {
                        LinkButton lnk = new LinkButton();
                        lnk.Click += new EventHandler(PagerLinkButton_Click);
                        lnk.ID = "lnkPage" + (i + 1).ToString();
                        lnk.Text = (i + 1).ToString();
                        if (i == currentPage)
                        {
                            lnk.Font.Bold = true;
                        }
                        placeHolderPaging.Controls.Add(lnk);
                        Label spacer = new Label();
                        spacer.Text = "&nbsp;";
                        placeHolderPaging.Controls.Add(spacer);
                    }
                }

            }
        }

        
        void PagerLinkButton_Click(object sender, EventArgs e)
        {
            LinkButton lnk = sender as LinkButton;
            int currentPage = int.Parse(lnk.Text) - 1;
           
            ShowResults(searchString, pageSize, currentPage);
            CreatePager(currentPage, Convert.ToInt32(ViewState["TotalResults"]));

        }
    }
}