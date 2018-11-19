using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using HJORM;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Autorisation;

namespace BitSite._bitPlate
{
    public partial class DataCollectionData2 : BasePage
    {
        private DataCollection dataCollection = null;
        private Dictionary<string, string> itemTabs = new Dictionary<string, string>();
        private Dictionary<string, string> groupTabs = new Dictionary<string, string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();

            base.CheckPermissions(FunctionalityEnum.DataCollections);
            base.CheckPermissions(FunctionalityEnum.DataCollectionsEditData);

            if (!SessionObject.HasPermission(FunctionalityEnum.DataCollectionDataEdit))
            {
                tdDataCollectionDataGroupConfig.Disabled = true;
                aDataCollectionDataGroupConfig.HRef = "#";
                tdDataCollectionDataItemConfig.Disabled = true;
                aDataCollectionDataItemConfig.HRef = "#";
            }

            if (!SessionObject.HasPermission(FunctionalityEnum.DataCollectionDataRemove))
            {
                tdDataCollectionDataGroupRemove.Disabled = true;
                aDataCollectionDataGroupRemove.HRef = "#";
                tdDataCollectionDataItemRemove.Disabled = true;
                aDataCollectionDataItemRemove.HRef = "#";
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.DataCollectionDataCreate))
            {
                liAddDataGroup.Disabled = true;
                aAddDataGroup.HRef = "#";
                liAddDataItem.Disabled = true;
                aAddDataItem.HRef = "#";
                tdDataCollectionDataGroupCopy.Disabled = true;
                aDataCollectionDataGroupCopy.HRef = "#";
                tdDataCollectionDataItemCopy.Disabled = true;
                aDataCollectionDataItemCopy.HRef = "#";
            }

            SelectDataCollection();
            CreateGroupTabs();
            CreateItemTabs();
        }

        private void SelectDataCollection()
        {
            string id = Request.QueryString["datacollectionid"];
            dataCollection = BaseObject.GetById<DataCollection>(new Guid(id));
        }
        /// <summary>
        /// bouw scherm op op basis van de velden uit de datacollectie 
        /// </summary>
        private void CreateGroupTabs()
        {
            int tabIndex = 1;
            foreach (DataField field in dataCollection.DataGroupFields)
            {
                string tabPageName = field.TabPage;
                if (tabPageName.Trim() == "") tabPageName = "Andere gegevens";
                if (!groupTabs.ContainsKey(tabPageName))
                {
                    string tabClientID = "tabPageGroup" + tabIndex;

                    groupTabs.Add(tabPageName, tabClientID);
                    if (LiteralGroupFields.Text != "")
                    {
                        LiteralGroupFields.Text += "</div>\r\n";
                    }

                    LiteralGroupTabPages.Text += String.Format(@"<li><a href=""#{0}"">{1}</a></li>", tabClientID, tabPageName);
                    tabIndex++;
                    //waarde tussen [] wordt hierna vervangen
                    LiteralGroupFields.Text += String.Format(@"<div id=""{0}"" class=""bitTabPage"">", tabClientID);
                }
                string fieldHtml = GetElementByFieldType(field);
                string rowHtml = String.Format(@"
                <div class=""bitPageSettingsCollumnA"">{0}</div>
                <div class=""bitPageSettingsCollumnB"">
                    
                </div>
                <div class=""bitPageSettingsCollumnC"">
                    {1}
                </div>
                <br clear=""all"" />
                <!-- NEXT ROW -->", field.Name, fieldHtml);
                if (field.FieldType == FieldTypeEnum.Html)
                {
                    rowHtml = String.Format(@"
                {0}<br/>
                {1}
                <br clear=""all"" />
                <!-- NEXT ROW -->", field.Name, fieldHtml);
                }
                LiteralGroupFields.Text += rowHtml;
            }
            if (LiteralGroupFields.Text != "")
            {
                LiteralGroupFields.Text += "</div>\r\n";
            }
        }
        /// <summary>
        /// bouw scherm op op basis van de velden uit de datacollectie 
        /// </summary>
        private void CreateItemTabs()
        {
            int tabIndex = 1;
            foreach (DataField field in dataCollection.DataItemFields)
            {

                string tabPageName = field.TabPage;
                if (tabPageName.Trim() == "") tabPageName = "Andere gegevens";
                if (!itemTabs.ContainsKey(tabPageName))
                {
                    string tabClientID = "tabPageItem" + tabIndex;

                    itemTabs.Add(tabPageName, tabClientID);
                    if (LiteralItemFields.Text != "")
                    {
                        LiteralItemFields.Text += "</div>\r\n";
                    }

                    LiteralItemTabPages.Text += String.Format(@"<li><a href=""#{0}"">{1}</a></li>", tabClientID, tabPageName);
                    tabIndex++;
                    //waarde tussen [] wordt hierna vervangen
                    LiteralItemFields.Text += String.Format(@"<div id=""{0}"" class=""bitTabPage"">", tabClientID);
                }
                string fieldHtml = GetElementByFieldType(field);
                string rowHtml = String.Format(@"
                <div class=""bitPageSettingsCollumnA"">{0}</div>
                <div class=""bitPageSettingsCollumnB"">
                    
                </div>
                <div class=""bitPageSettingsCollumnC"">
                    {1}
                </div>
                <br clear=""all"" />
                <!-- NEXT ROW -->", field.Name, fieldHtml);
                if (field.FieldType == FieldTypeEnum.Html)
                {
                    rowHtml = String.Format(@"
                {0}<br/>
                {1}
                <br clear=""all"" />
                <!-- NEXT ROW -->", field.Name, fieldHtml);
                }
                LiteralItemFields.Text += rowHtml;
            }
            if (LiteralItemFields.Text != "")
            {
                LiteralItemFields.Text += "</div>\r\n";
            }
        }


        private string GetElementByFieldType(DataField field)
        {
            string returnHtml = "";
            if (field.FieldType == FieldTypeEnum.Text)
            {
                returnHtml = String.Format(@"<input type=""text"" data-field=""{0}"" />", field.MappingColumn);
            }
            else if (field.FieldType == FieldTypeEnum.LongText)
            {
                returnHtml = String.Format(@"<textarea data-field=""{0}"" ></textarea>", field.MappingColumn);
            }
            else if (field.FieldType == FieldTypeEnum.DateTime)
            {
                returnHtml = String.Format(@"<input type=""date"" data-field=""{0}"" />", field.MappingColumn);
            }
            else if (field.FieldType == FieldTypeEnum.Numeric)
            {
                returnHtml = String.Format(@"<input type=""text"" data-field=""{0}"" data-validation=""Numeric"" />", field.MappingColumn);
            }
            else if (field.FieldType == FieldTypeEnum.Currency)
            {
                returnHtml = String.Format(@"<input type=""text"" data-field=""{0}"" data-validation=""Numeric""/>", field.MappingColumn);
            }
            else if (field.FieldType == FieldTypeEnum.YesNo)
            {
                returnHtml = String.Format(@"<input type=""checkbox"" data-field=""{0}"" />", field.MappingColumn);
            }
            else if (field.FieldType == FieldTypeEnum.DropDown)
            {
                string options = "";
                foreach (DataLookupValue lookupValue in field.LookupValues)
                {
                    options += String.Format(@"<option value=""{0}"">{1}</option>", lookupValue.ID, lookupValue.Name);
                }
                returnHtml = String.Format(@"<select data-field=""{0}"">{1}</select>", field.MappingColumn, options);
            }
            else if (field.FieldType == FieldTypeEnum.Html)
            {
                string imgTargetElm = field.Type + field.MappingColumn.Replace(".", "");
                returnHtml = String.Format(@"<div id=""bitToolbar{1}"" class=""bitToolbarSmall"">
                </div>
                <div id=""bitEditor{1}"" style=""width: 700px"" class=""wysiwygEditor"" data-field=""{0}"" data-control-type=""wysiwyg"" contenteditable=""true"">
                </div>", field.MappingColumn, imgTargetElm);
            }
            else if (field.FieldType == FieldTypeEnum.Image)
            {
                string imgTargetElm = field.Type + field.MappingColumn.Replace(".", "");
                returnHtml = String.Format(@"<img id=""img{1}"" src=""../"" alt="""" data-field=""{0}"" data-title-field=""{0}""
                            style=""max-height: 150px; max-width: 150px"" /> <span id=""fileName{1}"" data-field=""{0}""></span>
                        <input type=""button"" value=""..."" onclick=""javascript:BITDATACOLLECTIONDATA.openImagePopup('{1}');"" />
                        <input type=""button"" value=""x"" title=""verwijder afbeelding"" onclick=""javascript:BITDATACOLLECTIONDATA.clearImage('{1}');"" />
", field.MappingColumn, imgTargetElm);
            }
            else if (field.FieldType == FieldTypeEnum.File)
            {
                string imgTargetElm = field.Type + field.MappingColumn.Replace(".", "");
                returnHtml = String.Format(@"<img id=""fileIcon{1}"" src=""../"" alt="""" data-field=""{0}"" data-title-field=""{0}""
                            style=""max-height: 150px; max-width: 150px"" /> <span id=""fileName{1}"" data-field=""{0}""></span>
                        <input type=""button"" value=""..."" onclick=""javascript:BITDATACOLLECTIONDATA.openFilesPopup('{1}');"" />
                        <input type=""button"" value=""x"" title=""verwijder bestand"" onclick=""javascript:BITDATACOLLECTIONDATA.clearFile('{1}');"" />
", field.MappingColumn, imgTargetElm);
            }
            else if (field.FieldType == FieldTypeEnum.ImageList)
            {
                returnHtml = String.Format(@"<input type=""button"" value=""Toevoegen"" onclick=""javascript:BITDATACOLLECTIONDATA.openImagePopup('ExtraImages');"" />
                        <table id=""tableExtraImages"">
                            <tbody>
                                <tr>
                                    <td>
                                        <a class=""bitDeleteButton"" data-child-field=""ID"" title=""verwijder"" href=""javascript:BITDATACOLLECTIONDATA.removeExtraFile('[data-field]', [list-index]);""></a>
                                    </td>
                                    <td>
                                        <img src=""null"" data-child-field=""Url"" style=""max-height: 50px; max-width: 50px"" />
                                    </td>
                                    <td data-child-field=""Url""></td>
                                </tr>
                            </tbody>
                        </table>");
            }
            else if (field.FieldType == FieldTypeEnum.FileList)
            {
                returnHtml = String.Format(@"<input type=""button"" value=""Toevoegen"" onclick=""javascript:BITDATACOLLECTIONDATA.openFilesPopup('ExtraFiles');"" />
                        <table id=""tableExtraFiles"">
                            <tbody>
                                <tr>
                                    <td>
                                        <a class=""bitDeleteButton"" data-child-field=""ID"" title=""verwijder"" href=""javascript:BITDATACOLLECTIONDATA.removeExtraFile('[data-field]', [list-index]);""></a>
                                    </td>
                                    <td>
                                        <img src=""null"" data-child-field=""Url"" style=""max-height: 50px; max-width: 50px"" />
                                    </td>
                                    <td data-child-field=""Url""></td>
                                </tr>
                            </tbody>
                        </table>");
            }
            return returnHtml;
        }

    }
}