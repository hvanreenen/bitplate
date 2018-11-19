using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using BitPlate.Domain;
using BitSite._bitPlate.bitAjaxServices;

namespace BitSite._bitPlate
{
    public partial class PreviewPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["CurrentSite"] == null)
            {
               
                string siteID = ConfigurationManager.AppSettings["SiteID"];
                if (siteID != null && siteID != "")
                {
                    CmsSite site = BaseDomainObject.GetById<CmsSite>(new Guid(siteID));

                    HttpContext.Current.Session["CurrentSite"] = site;
                }
            }

            string id = Request.QueryString["id"];
            //CmsPage page = BaseDomainObject.GetById<CmsPage>(new Guid(id));
           CmsPage page = PageService.GetPreviewPage(id, "");
           //Response.ClearContent();
           //Response.Write(page.GetPublishHtml());
           //Response.Flush();
           //LiteralTitle.Text = "Preview: " + page.Title;
           LiteralHead.Text = page.Head;
           LiteralBody.Text = page.Body;

        }
    }
}