using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.Newsletters;

namespace BitSite._bitPlate.EditPage.ModuleConfig
{
    public class BaseModuleConfigUserControl : System.Web.UI.UserControl
    {
        internal string GetLanguageCode()
        {
            string returnValue = "";
            if (SessionObject.CurrentSite.IsMultiLingual)
            {
                string pageid = Request.QueryString["pageid"];
                string newsletterid = Request.QueryString["newsletterid"];
                if (pageid != "" && pageid != null)
                {
                    CmsPage page = BaseObject.GetById<CmsPage>(new Guid(pageid));
                    if (page != null)
                    {
                        returnValue = page.LanguageCode;
                    }
                }
                else if (newsletterid != "" && newsletterid != null)
                {
                    Newsletter newsletter = BaseObject.GetById<Newsletter>(new Guid(newsletterid));
                    if (newsletter != null)
                    {
                        returnValue = newsletter.LanguageCode;
                    }
                }
            }
            return returnValue;
            //DefaultMaster masterPage = (DefaultMaster)this.Page.Master;
            //if (masterPage != null)
            //{
            //    return masterPage.LanguageCode;
            //}
            //else return "";
        }
    }
}