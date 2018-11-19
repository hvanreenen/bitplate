using BitPlate.Domain.DataCollections;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate.EditPage.ModuleConfig
{
    public partial class DataDetailsConfigTab : BaseModuleConfigUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                fillLookupData();
            }
        }
        private void fillLookupData()
        {
            //datacollecties
            this.SelectDataCollectionsDetails.Items.Add(new ListItem("", Guid.Empty.ToString()));
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);
            string pageLanguage = this.GetLanguageCode();
            if (pageLanguage != "" && pageLanguage != null)
            {
                //als er een taal is dan alleen de collecties tonen met diezelfde vaste taal of zonder taal. (zonder taal is meertalig)
                where += String.Format(" AND (LanguageCode = '{0}' OR LanguageCode IS NULL OR LanguageCode = '')", pageLanguage);
            }
            BaseCollection<DataCollection> datacollections = BaseCollection<DataCollection>.Get(where, "Name");
            foreach (DataCollection col in datacollections)
            {
                this.SelectDataCollectionsDetails.Items.Add(new ListItem(col.Name, col.ID.ToString()));
            }
        }
    }
}