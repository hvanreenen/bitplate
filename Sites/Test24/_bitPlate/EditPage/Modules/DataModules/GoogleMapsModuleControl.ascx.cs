using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain.Modules;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Modules.Data;
using BitPlate.Domain.Utils;
using System.Data;

namespace BitSite._bitPlate.EditPage.Modules.DataModules
{
    public partial class GoogleMapsModuleControl : BaseDataModuleUserControl
    {
        protected Repeater repeater;
        
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
            System.Diagnostics.Trace.WriteLine("Page_Load Start", "GoogleMapsModuleControl");
            base.Load(sender, e);
            if (base.CheckAutorisation())
            {
                
                LabelMsg.Text = "";
                this.ParentGroup = null;
                if (DataCollectionID == null || DataCollectionID == Guid.Empty)
                {
                    LabelMsg.Text = "Kies eerst een datacollectie";
                }
                else
                {
                    if (!IsPostBack)
                    {
                        Guid dataId = Guid.Empty;
                        ShowDataEnum showDataBy = base.getSetting<ShowDataEnum>("ShowDataBy");
                        if (showDataBy == ShowDataEnum.UserSelect)
                        {
                            string dataIdQuery = Request.QueryString["dataid"];
                            Guid.TryParse(dataIdQuery, out dataId);
                        }
                        SelectAndShowData(dataId);


                        GPoint initPoint = GoogleGeocoder.GetLatLng(base.getSetting<string>("InitialAddress") + "," + base.getSetting<string>("InitialAddress"), SessionObject.CurrentSite.GoogleMapsKey);
                        
                        string infoBalloonFormat = "";
                        string sideBarRowFormat = "";
                        Literal literalInfoBalloon = (Literal)FindControl("LiteralInfoBalloon" + ModuleID.ToString("N"));
                        if (literalInfoBalloon != null)
                        {
                            infoBalloonFormat = literalInfoBalloon.Text;
                            infoBalloonFormat = infoBalloonFormat.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                            infoBalloonFormat = infoBalloonFormat.Replace("'", "&quot;");
                        }

                        Literal literalSideBar = (Literal)FindControl("LiteralSideBar" + ModuleID.ToString("N"));
                        if (literalSideBar != null)
                        {
                            sideBarRowFormat = literalSideBar.Text;
                            sideBarRowFormat = sideBarRowFormat.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                            sideBarRowFormat = sideBarRowFormat.Replace("'", "&quot;");
                        
                        }

                        string navigationUrl = Request.Url.LocalPath;

                        ModuleNavigationActionLite navigationAction = GetNavigationActionByTagName("{DrillDownLink}");
                        if (navigationAction != null)
                        {
                            navigationUrl = navigationAction.NavigationUrl;
                            if (navigationAction.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
                            {
                                //blijven op dezelfde pagina
                                navigationUrl = Request.Url.LocalPath;
                            }
                        }
                        LabelMsg.Text = String.Format(@"
<script type=""text/javascript"">
    BITGOOGLEMAP.key = '{0}';
    
    BITGOOGLEMAP.initLatitude = {1};
    BITGOOGLEMAP.initLongitude = {2};
    BITGOOGLEMAP.initZoom = {3};
    BITGOOGLEMAP.datacollectionID = '{4}';
    BITGOOGLEMAP.infoBalloonFormat = '{5}';
    BITGOOGLEMAP.sideBarRowFormat = '{6}';
    BITGOOGLEMAP.navigationUrl = '{7}'
    //BITGOOGLEMAP.mapType = '{0}';
</script>", SessionObject.CurrentSite.GoogleMapsKey, initPoint.Lat.ToString().Replace(",", "."), initPoint.Long.ToString().Replace(",", "."), base.getSetting<string>("InitialZoom"), this.DataCollectionID, infoBalloonFormat, sideBarRowFormat, navigationUrl);

                    }
                }
            }
            System.Diagnostics.Trace.WriteLine("Page_Load End", "GoogleMapsModuleControl");
        }

        
        
    }
}