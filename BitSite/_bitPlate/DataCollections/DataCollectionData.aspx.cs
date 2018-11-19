using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Autorisation;
using System.IO;
using Autofac;
using elFinder.Connector.Config;
using elFinder.Connector;
using BitPlate.Domain.Licenses;

namespace BitSite._bitPlate.DataCollections
{
    public partial class DataCollectionData : BasePage
    {
        private DataCollection dataCollection = null;
        private Dictionary<string, string> itemTabs = new Dictionary<string, string>();
        private Dictionary<string, string> groupTabs = new Dictionary<string, string>();
        private BaseCollection<CmsLanguage> siteLanguages = null;

        private static IContainer _container; //Elfinder container;

        protected void Page_Load(object sender, EventArgs e)
        {

            base.CheckLoginAndLicense();

            base.CheckPermissions(FunctionalityEnum.DataCollections);
            base.CheckPermissions(FunctionalityEnum.DataCollectionsEditData);

            SelectDataCollection();

            siteLanguages = SessionObject.CurrentSite.Languages;

            if (SessionObject.CurrentSite.IsMultiLingual && SessionObject.CurrentLicense.AllowMultipleLanguages &&
                (dataCollection.LanguageCode == "" || dataCollection.LanguageCode == null))
            
            {
                //alleen keuzelijst met talen tonen als datacollectie geen vaste taal heeft 
                divLanguage_Groups.Visible = true;
                divLanguage_Items.Visible = true;
                //vullen dropdown met talen
                int selectedIndex = 0;
                int index = 0;
                foreach (CmsLanguage language in SessionObject.CurrentSite.Languages)
                {
                    if (language.LanguageCode == SessionObject.CurrentSite.DefaultLanguage)
                    {
                        selectedIndex = index;
                        
                    }

                    dropdownLanguages_Groups.Items.Add(new ListItem(language.Name, language.LanguageCode));
                    dropdownLanguages_Items.Items.Add(new ListItem(language.Name, language.LanguageCode));

                    index++;
                }
                dropdownLanguages_Groups.SelectedIndex = selectedIndex;
                dropdownLanguages_Items.SelectedIndex = selectedIndex;
            }

            if (!SessionObject.HasPermission(FunctionalityEnum.DataCollectionDataEdit))
            {
                htdDataCollectionDataEdit.Visible = false;
                tdDataCollectionDataGroupEdit.Visible = false;
                tdDataCollectionDataItemEdit.Visible = false;

            }

            if (!SessionObject.HasPermission(FunctionalityEnum.DataCollectionDataRemove))
            {
                htdDataCollectionDataRemove.Visible = false;
                tdDataCollectionDataGroupRemove.Visible = false;
                tdDataCollectionDataItemRemove.Visible = false;

               
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.DataCollectionDataCreate))
            {
                liAddDataGroup.Disabled = true;
                liAddDataGroup.Attributes["class"] = "bitItemDisabled";
                aAddDataGroup.HRef = "#";
                
                liAddDataItem.Disabled = true;
                liAddDataItem.Attributes["class"] = "bitItemDisabled";
                aAddDataItem.HRef = "#";

                htdDataCollectionDataCopy.Visible = false;
                tdDataCollectionDataGroupCopy.Visible = false;
                tdDataCollectionDataItemCopy.Visible = false;

            }
            //            LiteralJsScript.Text = String.Format(@"<script type='text/javascript'>
            //        BITIMAGESPOPUP.siteDomain = '{0}';
            //</script>", SessionObject.CurrentSite.DomainName);

            CreateGroupTabs();
            CreateItemTabs();
            try
            {
                /************************************ELFINDER CONFIG*************************************/
                AppConnectorConfig elConfig = new AppConnectorConfig();
                elConfig.BaseThumbsUrl = SessionObject.CurrentSite.DomainName + "/_temp/_thumb/";
                elConfig.BaseUrl = SessionObject.CurrentSite.DomainName + "/_files/";
                elConfig.DefaultVolumeName = "LocalFileSystem";
                elConfig.DuplicateDirectoryPattern = "Copy of {0}";
                elConfig.DuplicateFilePattern = "Copy of {0}";
                elConfig.ThumbsSize = new System.Drawing.Size(48, 48);
                elConfig.UploadMaxSize = "20M";
                string BitplatePath = SessionObject.CurrentSite.Path;
                elConfig.LocalFSRootDirectoryPath = BitplatePath + "_files";
                elConfig.LocalFSThumbsDirectoryPath = BitplatePath + "_temp\\_thumb";
                elConfig.RootDirectoryName = "Root";
                elConfig.EnableAutoScaleImages = (SessionObject.CurrentSite.MaxWidthImages > 0);
                elConfig.AutoScaleImagesWidth = SessionObject.CurrentSite.MaxWidthImages;
                elFinder.Connector.Config.AppConnectorConfig.Instance = elConfig;

                if (!Directory.Exists(BitplatePath + "_temp"))
                {
                    Directory.CreateDirectory(BitplatePath + "_temp");
                }

                if (!Directory.Exists(BitplatePath + "_temp\\_thumb"))
                {
                    Directory.CreateDirectory(BitplatePath + "_temp\\_thumb");
                }
            }
            catch (Exception ex)
            {
            }

            // register IoC
            var builder = new ContainerBuilder();
            builder.RegisterElFinderConnector();
            _container = builder.Build();
            // need also to set container in elFinder module
            _container.SetAsElFinderResolver();
            /************************************END ELFINDER CONFIG*********************************/
        }

        private void SelectDataCollection()
        {
            string id = Request.QueryString["datacollectionid"];
            dataCollection = BaseObject.GetById<DataCollection>(new Guid(id));
            if (dataCollection.HasAutorisation)
            {

                if (!dataCollection.IsAutorized(SessionObject.CurrentBitplateUser))
                {
                    throw new Exception("U heeft geen rechten voor deze datacollectie.");
                }
            }
        }
        /// <summary>
        /// bouw scherm op op basis van de velden uit de datacollectie 
        /// </summary>
        private void CreateGroupTabs()
        {
            int tabIndex = 1;
            foreach (DataField field in dataCollection.DataGroupFields)
            {
                if (field.MappingColumn == "Name" || field.MappingColumn == "OrderNumber")
                {
                    continue;
                }
                else if (field.MappingColumn == "Title")
                {
                    this.LiteralGroupTitle.Text = GetElementByFieldType(field);
                    continue;
                }
                else if (field.MappingColumn == "MetaDescription")
                {
                    this.LiteralGroupMetaDescription.Text = GetElementByFieldType(field);
                    continue;
                }
                else if (field.MappingColumn == "MetaKeywords")
                {
                    this.LiteralGroupMetaKeywords.Text = GetElementByFieldType(field);
                    continue;
                }
                
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
                if (field.MappingColumn == "Name" || field.MappingColumn == "OrderNumber")
                {
                    //vaste velden
                    continue;
                }
                else if (field.MappingColumn == "Title")
                {
                    //vast veld
                    this.LiteralItemTitle.Text = GetElementByFieldType(field);
                    continue;
                }
                else if (field.MappingColumn == "MetaDescription")
                {
                    //vast veld
                    this.LiteralItemMetaDescription.Text = GetElementByFieldType(field);
                    continue;
                }
                else if (field.MappingColumn == "MetaKeywords")
                {
                    this.LiteralItemMetaKeywords.Text = GetElementByFieldType(field);
                    continue;
                }
                //if (field.IsSystemField) continue;
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
                if (field.IsMultiLanguageField)
                {
                    returnHtml = String.Format("<input type=\"text\" data-field=\"{0}\" data-language=\"{1}\"/>", field.MappingColumn, SessionObject.CurrentSite.DefaultLanguage);
                    foreach (CmsLanguage language in this.siteLanguages)
                    {
                        if (language.LanguageCode != SessionObject.CurrentSite.DefaultLanguage)
                        {
                            returnHtml = returnHtml + String.Format("<input type=\"text\" data-field=\"{1}.{0}\" data-language=\"{1}\" style=\"display:none\"/>", field.MappingColumn, language.LanguageCode);
                        }
                    }
                }
                else
                {
                    returnHtml = String.Format("<input type=\"text\" data-field=\"{0}\" />", field.MappingColumn);
                }
            }
            else if (field.FieldType == FieldTypeEnum.LongText)
            {
                if (field.IsMultiLanguageField)
                {
                    returnHtml = String.Format("<textarea data-field=\"{0}\" data-language=\"{1}\" class=\"bitTextArea\" ></textarea>", field.MappingColumn, SessionObject.CurrentSite.DefaultLanguage);
                    foreach (CmsLanguage language in this.siteLanguages)
                    {
                        if (language.LanguageCode != SessionObject.CurrentSite.DefaultLanguage)
                        {
                            returnHtml = returnHtml + String.Format("<textarea data-field=\"{1}.{0}\" data-language=\"{1}\" style=\"display:none\" class=\"bitTextArea\"></textarea>", field.MappingColumn, language.LanguageCode);
                        }
                    }
                    return returnHtml;
                }
                else
                {
                    returnHtml = String.Format("<textarea data-field=\"{0}\" class=\"bitTextArea\"></textarea>", field.MappingColumn);
                }
            }

            else if (field.FieldType == FieldTypeEnum.DateTime)
            {
                returnHtml = String.Format("<input type=\"datetime-local\" data-field=\"{0}\" step=\"1\" />", field.MappingColumn);
            }

            else if (field.FieldType == FieldTypeEnum.Numeric)
            {
                returnHtml = String.Format("<input type=\"text\" data-field=\"{0}\" data-format=\"N\" data-validation=\"number\" />", field.MappingColumn);
            }

            else if (field.FieldType == FieldTypeEnum.Currency)
            {
                if (field.IsMultiLanguageField)
                {
                    returnHtml = String.Format("<input type=\"text\" data-field=\"{0}\" data-language=\"{1}\" data-format=\"C\" data-validation=\"decimal\"/>", field.MappingColumn, SessionObject.CurrentSite.DefaultLanguage);
                    foreach (CmsLanguage language in this.siteLanguages)
                    {
                        if (language.LanguageCode != SessionObject.CurrentSite.DefaultLanguage)
                        {
                            returnHtml = returnHtml + String.Format("<input type=\"text\" data-field=\"{1}.{0}\" data-language=\"{1}\" style=\"display:none\" data-format=\"C\" data-validation=\"decimal\"/>", field.MappingColumn, language.LanguageCode);
                        }
                    }

                }
                else
                {
                    returnHtml = String.Format("<input type=\"text\" data-field=\"{0}\" data-format=\"C\" data-validation=\"decimal\"/>", field.MappingColumn);
                }
            }

            else if (field.FieldType == FieldTypeEnum.YesNo)
            {
                returnHtml = String.Format("<input type=\"checkbox\" data-field=\"{0}\" />", field.MappingColumn);
            }

            else if (field.FieldType == FieldTypeEnum.CheckboxList)
            {
                returnHtml = "<div id=\"Checkboxlist\" data-field=\"" + field.MappingColumn + "\" data-text-field=\"Title\" data-control-type=\"checkboxlist\">\r\n";
                string checkboxes = "";
                foreach (DataLookupValue lookupValue in field.LookupValues)
                {
                    string optionValue = lookupValue.Name;
                    checkboxes += "<input type=\"checkbox\" id=\"bitCheckBox" + lookupValue.ID.ToString("N") + "\" value=\"" + lookupValue.ID.ToString() + "\"><label for=\"bitCheckBox" + lookupValue.ID.ToString("N") + "\">" + lookupValue.Name + "</label><br/>\r\n";  
                }
                returnHtml += checkboxes + "</div>\r\n"; 
            }

            else if (field.FieldType == FieldTypeEnum.DropDown)
            {
                string options = "";
                foreach (DataLookupValue lookupValue in field.LookupValues)
                {
                    string optionValue = lookupValue.Name;
                    if (field.IsMultiLanguageField)
                    {
                        if ((lookupValue.NL != null) && (lookupValue.NL != ""))
                        {
                            optionValue = optionValue + "/" + lookupValue.NL;
                        }
                        if ((lookupValue.EN != null) && (lookupValue.EN != ""))
                        {
                            optionValue = optionValue + "/" + lookupValue.EN;
                        }
                        if ((lookupValue.DE != null) && (lookupValue.DE != ""))
                        {
                            optionValue = optionValue + "/" + lookupValue.DE;
                        }
                        if ((lookupValue.FR != null) && (lookupValue.FR != ""))
                        {
                            optionValue = optionValue + "/" + lookupValue.FR;
                        }
                        if ((lookupValue.SP != null) && (lookupValue.SP != ""))
                        {
                            optionValue = optionValue + "/" + lookupValue.SP;
                        }
                        if ((lookupValue.PL != null) && (lookupValue.PL != ""))
                        {
                            optionValue = optionValue + "/" + lookupValue.PL;
                        }
                        if ((lookupValue.IT != null) && (lookupValue.IT != ""))
                        {
                            optionValue = optionValue + "/" + lookupValue.IT;
                        }
                    }
                    options = options + String.Format("<option value=\"{0}\">{1}</option>", lookupValue.ID, optionValue);
                }
                returnHtml = String.Format("<select data-field=\"{0}.ID\">{1}</select>", field.MappingColumn, options);
            }

            else if (field.FieldType == FieldTypeEnum.Html)
            {
                string imgTargetElm = field.Type + field.MappingColumn.Replace(".", "");

                if (field.IsMultiLanguageField)
                {
                    returnHtml = String.Format("<div data-language=\"{2}\">\r\n                <div id=\"ckEditor{1}{2}\" style=\"width: 700px\" class=\"wysiwygEditor\" data-field=\"{0}\"  data-control-type=\"wysiwyg\">\r\n                </div></div>", field.MappingColumn, imgTargetElm, SessionObject.CurrentSite.DefaultLanguage);
                    foreach (CmsLanguage language in this.siteLanguages)
                    {
                        if (language.LanguageCode != SessionObject.CurrentSite.DefaultLanguage)
                        {
                            returnHtml = returnHtml + String.Format("<div data-language=\"{2}\" style=\"display:none\">\r\n                <div id=\"ckEditor{1}{2}\" style=\"width: 700px\" class=\"wysiwygEditor\" data-field=\"{2}.{0}\"  data-control-type=\"wysiwyg\">\r\n                </div></div>", field.MappingColumn, imgTargetElm, language.LanguageCode);
                        }
                    }

                }
                else
                {
                    returnHtml = String.Format("<div id=\"ckEditor{1}\" style=\"width: 700px\" class=\"wysiwygEditor\" data-field=\"{0}\" data-control-type=\"wysiwyg\">\r\n                </div>", field.MappingColumn, imgTargetElm);
                }

            }

            else if (field.FieldType == FieldTypeEnum.Image)
            {
                string imgTargetElm = field.Type + field.MappingColumn.Replace(".", "");
                if (field.IsMultiLanguageField)
                {
                    //returnHtml = String.Format("<div data-language=\"{3}\"><img id=\"img{2}{3}\" src=\"{0}/[data-field]\" alt=\"\" data-field=\"{1}\" data-title-field=\"{1}\"\r\n                            style=\"max-height: 150px; max-width: 150px\" /> <span id=\"fileName{2}{3}\" data-field=\"{1}\"></span>\r\n                        <input type=\"button\" value=\"...\" onclick=\"javascript:BITDATACOLLECTIONDATA.openImagePopup('{2}', '{3}');\" />\r\n                        <input type=\"button\" value=\"x\" title=\"verwijder afbeelding\" onclick=\"javascript:BITDATACOLLECTIONDATA.clearImage('{2}', '{3}');\" /></div>\r\n", new object[] { SessionObject.CurrentSite.DomainName, field.MappingColumn, imgTargetElm, SessionObject.CurrentSite.DefaultLanguage });
                    returnHtml = String.Format("<div data-language=\"{3}\"><img id=\"img{2}{3}\" src=\"[data-field]\" alt=\"\" data-field=\"{1}\" data-title-field=\"{1}\"\r\n                            style=\"max-height: 150px; max-width: 150px\" /> <span id=\"fileName{2}{3}\" data-field=\"{1}\"></span>\r\n                        <input type=\"button\" value=\"...\" onclick=\"javascript:BITDATACOLLECTIONDATA.openImagePopup('{2}', '{3}');\" />\r\n                        <input type=\"button\" value=\"x\" title=\"verwijder afbeelding\" onclick=\"javascript:BITDATACOLLECTIONDATA.clearImage('{2}', '{3}');\" /></div>\r\n", new object[] { SessionObject.CurrentSite.DomainName, field.MappingColumn, imgTargetElm, SessionObject.CurrentSite.DefaultLanguage });
                    foreach (CmsLanguage language in this.siteLanguages)
                    {
                        if (language.LanguageCode != SessionObject.CurrentSite.DefaultLanguage)
                        {
                            //returnHtml = returnHtml + String.Format("<div data-language=\"{3}\" style=\"display:none\"><img id=\"img{2}{3}\" src=\"{0}/[data-field]\" alt=\"\" data-field=\"{3}.{1}\" data-title-field=\"{3}.{1}\"\r\n                            style=\"max-height: 150px; max-width: 150px\" /> <span id=\"fileName{2}{3}\" data-field=\"{3}.{1}\" ></span>\r\n                        <input type=\"button\" value=\"...\" onclick=\"javascript:BITDATACOLLECTIONDATA.openImagePopup('{2}', '{3}');\" />\r\n                        <input type=\"button\" value=\"x\" title=\"verwijder afbeelding\" onclick=\"javascript:BITDATACOLLECTIONDATA.clearImage('{2}', '{3}');\" /></div>\r\n", new object[] { SessionObject.CurrentSite.DomainName, field.MappingColumn, imgTargetElm, language5.LanguageCode });
                            returnHtml = returnHtml + String.Format("<div data-language=\"{3}\" style=\"display:none\"><img id=\"img{2}{3}\" src=\"[data-field]\" alt=\"\" data-field=\"{3}.{1}\" data-title-field=\"{3}.{1}\"\r\n                            style=\"max-height: 150px; max-width: 150px\" /> <span id=\"fileName{2}{3}\" data-field=\"{3}.{1}\" ></span>\r\n                        <input type=\"button\" value=\"...\" onclick=\"javascript:BITDATACOLLECTIONDATA.openImagePopup('{2}', '{3}');\" />\r\n                        <input type=\"button\" value=\"x\" title=\"verwijder afbeelding\" onclick=\"javascript:BITDATACOLLECTIONDATA.clearImage('{2}', '{3}');\" /></div>\r\n", new object[] { SessionObject.CurrentSite.DomainName, field.MappingColumn, imgTargetElm, language.LanguageCode });
                        }
                    }
                }
                else
                {
                    //return String.Format("<img id=\"img{2}\" src=\"{0}/[data-field]\" alt=\"\" data-field=\"{1}\" data-title-field=\"{1}\"\r\n  
                    returnHtml = String.Format("<img id=\"img{2}\" src=\"[data-field]\" alt=\"\" data-field=\"{1}\" data-title-field=\"{1}\"\r\n  style=\"max-height: 150px; max-width: 150px\" /> <span id=\"fileName{2}\" data-field=\"{1}\"></span>\r\n                        <input type=\"button\" value=\"...\" onclick=\"javascript:BITDATACOLLECTIONDATA.openImagePopup('{2}');\" />\r\n                        <input type=\"button\" value=\"x\" title=\"verwijder afbeelding\" onclick=\"javascript:BITDATACOLLECTIONDATA.clearImage('{2}');\" />\r\n", SessionObject.CurrentSite.DomainName, field.MappingColumn, imgTargetElm);
                }
            }

            else if (field.FieldType == FieldTypeEnum.File)
            {
                string imgTargetElm = field.Type + field.MappingColumn.Replace(".", "");
                if (field.IsMultiLanguageField)
                {
                    //returnHtml = String.Format("<div data-language=\"{2}\"><img id=\"fileIcon{1}{2}\" src=\"../\" alt=\"\" data-field=\"{0}\" data-title-field=\"{0}\"\r\n                            style=\"max-height: 150px; max-width: 150px\" /> <span id=\"fileName{1}{2}\" data-field=\"{0}\"></span>\r\n                        <input type=\"button\" value=\"...\" onclick=\"javascript:BITDATACOLLECTIONDATA.openFilesPopup('{1}', '{2}');\" />\r\n                        <input type=\"button\" value=\"x\" title=\"verwijder bestand\" onclick=\"javascript:BITDATACOLLECTIONDATA.clearFile('{1}', '{2}');\" />\r\n</div>", field.MappingColumn, str6, SessionObject.CurrentSite.DefaultLanguage);
                    returnHtml = String.Format("<div data-language=\"{2}\"><img id=\"fileIcon{1}{2}\" src=\"../\" alt=\"\" data-field=\"{0}\" data-title-field=\"{0}\"\r\n                            style=\"max-height: 150px; max-width: 150px\" /> <span id=\"fileName{1}{2}\" data-field=\"{0}\"></span>\r\n                        <input type=\"button\" value=\"...\" onclick=\"javascript:BITDATACOLLECTIONDATA.openFilesPopup('{1}', '{2}');\" />\r\n                        <input type=\"button\" value=\"x\" title=\"verwijder bestand\" onclick=\"javascript:BITDATACOLLECTIONDATA.clearFile('{1}', '{2}');\" />\r\n</div>", field.MappingColumn, imgTargetElm, SessionObject.CurrentSite.DefaultLanguage);
                    foreach (CmsLanguage language in this.siteLanguages)
                    {
                        if (language.LanguageCode != SessionObject.CurrentSite.DefaultLanguage)
                        {
                            returnHtml = returnHtml + String.Format("<div data-language=\"{2}\" style=\"display:none\"><img id=\"fileIcon{1}{2}\" src=\"../\" alt=\"\" data-field=\"{2}.{0}\" data-title-field=\"{2}.{0}\"\r\n                            style=\"max-height: 150px; max-width: 150px\" /> <span id=\"fileName{1}{2}\" data-field=\"{2}.{0}\"></span>\r\n                        <input type=\"button\" value=\"...\" onclick=\"javascript:BITDATACOLLECTIONDATA.openFilesPopup('{1}', '{2}');\" />\r\n                        <input type=\"button\" value=\"x\" title=\"verwijder bestand\" onclick=\"javascript:BITDATACOLLECTIONDATA.clearFile('{1}', '{2}');\" />\r\n</div>", field.MappingColumn, imgTargetElm, language.LanguageCode);
                        }
                    }
                }
                else
                {
                    returnHtml = String.Format("<img id=\"fileIcon{1}\" src=\"../\" alt=\"\" data-field=\"{0}\" data-title-field=\"{0}\"\r\n                            style=\"max-height: 150px; max-width: 150px\" /> <span id=\"fileName{1}\" data-field=\"{0}\"></span>\r\n                        <input type=\"button\" value=\"...\" onclick=\"javascript:BITDATACOLLECTIONDATA.openFilesPopup('{1}');\" />\r\n                        <input type=\"button\" value=\"x\" title=\"verwijder bestand\" onclick=\"javascript:BITDATACOLLECTIONDATA.clearFile('{1}');\" />\r\n", field.MappingColumn, imgTargetElm);
                }
            }

            else if (field.FieldType == FieldTypeEnum.ImageList)
            {
                returnHtml = "";
                if (field.IsMultiLanguageField)
                {
                    returnHtml = String.Format("<input type=\"button\" value=\"Toevoegen\" data-language=\"{0}\" onclick=\"javascript:BITDATACOLLECTIONDATA.openImagePopup('ExtraImages', '{0}');\" />", SessionObject.CurrentSite.DefaultLanguage);
                    foreach (CmsLanguage language in this.siteLanguages)
                    {
                        if (language.LanguageCode != SessionObject.CurrentSite.DefaultLanguage)
                        {
                            returnHtml = returnHtml + String.Format("<input type=\"button\" value=\"Toevoegen\" data-language=\"{0}\" style=\"display:none\" onclick=\"javascript:BITDATACOLLECTIONDATA.openImagePopup('ExtraImages', '{0}');\" />", language.LanguageCode);
                        }
                    }
                }
                else
                {
                    returnHtml = "<input type=\"button\" value=\"Toevoegen\" onclick=\"javascript:BITDATACOLLECTIONDATA.openImagePopup('ExtraImages');\" />";
                }

                returnHtml += String.Format("<table id=\"tableExtraImages\">\r\n                            <tbody>\r\n                                <tr>\r\n                                    <td>\r\n                                        <a class=\"bitDeleteButton\" data-child-field=\"ID\" title=\"verwijder\" href=\"javascript:BITDATACOLLECTIONDATA.removeExtraFile('[data-field]', [list-index]);\"></a>\r\n                                    </td>\r\n                                    <td>\r\n                                        <img src=\"null\" data-child-field=\"Url\" style=\"max-height: 50px; max-width: 50px\" />\r\n                                    </td>\r\n                                    <td data-child-field=\"Url\"></td>\r\n                                    <td data-child-field=\"Language\"></td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>", new object[0]);

            }

            else if (field.FieldType == FieldTypeEnum.FileList)
            {
                returnHtml = "";
                if (field.IsMultiLanguageField)
                {
                    returnHtml = String.Format("<input type=\"button\" value=\"Toevoegen\" data-language=\"{0}\" onclick=\"javascript:BITDATACOLLECTIONDATA.openFilesPopup('ExtraFiles', '{0}');\" />", SessionObject.CurrentSite.DefaultLanguage);
                    foreach (CmsLanguage language in this.siteLanguages)
                    {
                        if (language.LanguageCode != SessionObject.CurrentSite.DefaultLanguage)
                        {
                            returnHtml = returnHtml + String.Format("<input type=\"button\" value=\"Toevoegen\" data-language=\"{0}\" style=\"display:none\" onclick=\"javascript:BITDATACOLLECTIONDATA.openFilesPopup('ExtraFiles', '{0}');\" />", language.LanguageCode);
                        }
                    }
                }
                else
                {
                    returnHtml = "<input type=\"button\" value=\"Toevoegen\" onclick=\"javascript:BITDATACOLLECTIONDATA.openFilesPopup('ExtraFiles');\" />";
                }
                returnHtml += String.Format("<table id=\"tableExtraFiles\">\r\n                            <tbody>\r\n                                <tr>\r\n                                    <td>\r\n                                        <a class=\"bitDeleteButton\" data-child-field=\"ID\" title=\"verwijder\" href=\"javascript:BITDATACOLLECTIONDATA.removeExtraFile('[data-field]', [list-index]);\"></a>\r\n                                    </td>\r\n                                    <td>\r\n                                        <img src=\"null\" data-child-field=\"Url\" style=\"max-height: 50px; max-width: 50px\" />\r\n                                    </td>\r\n                                    <td data-child-field=\"Url\"></td>\r\n                                    <td data-child-field=\"Language\"></td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>", new object[0]);
            }
            else if (field.FieldType == FieldTypeEnum.ReadOnly)
            {
                //voor oa Latitude en Longitude
                returnHtml = String.Format("<span data-field=\"{0}\"></span>", field.MappingColumn);

            }
            return returnHtml;
        }

        private string GetElementByFieldType_old(DataField field)
        {
            string returnHtml = "";
            if (field.FieldType == FieldTypeEnum.Text)
            {
                if (field.IsMultiLanguageField)
                {
                    //returnHtml = String.Format(@"<input type=""text"" data-field=""{0}"" />", field.MappingColumn);
                    //foreach (CmsLanguage language in siteLanguages)
                    //{
                    //    if (language.LanguageCode != SessionObject.CurrentSite.DefaultLanguage)
                    //    {
                    //        returnHtml += String.Format(@"<input type=""text"" data-field=""{1}.{0}"" data-language=""{1}"" style=""display:none""/>", field.MappingColumn, language.LanguageCode);
                    //    }

                    //}
                    returnHtml = String.Format(@"<input type=""text"" data-language-field=""{0}"" />", field.MappingColumn);
                }
                else
                {
                    returnHtml = String.Format(@"<input type=""text"" data-field=""{0}"" />", field.MappingColumn);
                }
            }
            else if (field.FieldType == FieldTypeEnum.LongText)
            {
                if (field.IsMultiLanguageField)
                {
                    returnHtml = String.Format(@"<textarea data-language-field=""{0}"" ></textarea>", field.MappingColumn);
                }
                else
                {
                    returnHtml = String.Format(@"<textarea data-field=""{0}"" ></textarea>", field.MappingColumn);
                }
            }
            else if (field.FieldType == FieldTypeEnum.DateTime)
            {
                returnHtml = String.Format(@"<input type=""date"" data-field=""{0}"" />", field.MappingColumn);
            }
            else if (field.FieldType == FieldTypeEnum.Numeric)
            {
                returnHtml = String.Format(@"<input type=""text"" data-field=""{0}"" data-format=""N"" data-validation=""number"" />", field.MappingColumn);
            }
            else if (field.FieldType == FieldTypeEnum.Currency)
            {
                returnHtml = String.Format(@"<input type=""text"" data-field=""{0}"" data-format=""C"" data-validation=""decimal""/>", field.MappingColumn);
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
                returnHtml = String.Format(@"<select data-field=""{0}.ID"">{1}</select>", field.MappingColumn, options);
            }
            else if (field.FieldType == FieldTypeEnum.Html)
            {
                string imgTargetElm = field.Type + field.MappingColumn.Replace(".", "");
                returnHtml = String.Format(@"<div id=""bitToolbar{1}"" class=""bitToolbarSmall"">
                </div>
                <div id=""bitEditor{1}"" style=""width: 700px"" class=""wysiwygEditor"" data-field=""{0}"" data-control-type=""wysiwyg"" contenteditable=""true"">
                </div>", field.MappingColumn, imgTargetElm);
                returnHtml = String.Format(@"<div id=""bitToolbar{1}"" class=""bitToolbarSmall"">
                </div>
                <div id=""bitEditor{1}"" style=""width: 700px"" class=""wysiwygEditor"" data-field=""{0}"" data-control-type=""wysiwyg"">
                </div>", field.MappingColumn, imgTargetElm);
            }
            else if (field.FieldType == FieldTypeEnum.Image)
            {
                string imgTargetElm = field.Type + field.MappingColumn.Replace(".", "");
                /* returnHtml = String.Format(@"<img id=""img{2}"" src=""{0}/[data-field]"" alt="""" data-field=""{1}"" data-title-field=""{1}""
                            style=""max-height: 150px; max-width: 150px"" /> <span id=""fileName{2}"" data-field=""{1}""></span>
                        <input type=""button"" value=""..."" onclick=""javascript:BITDATACOLLECTIONDATA.openImagePopup('{2}');"" />
                        <input type=""button"" value=""x"" title=""verwijder afbeelding"" onclick=""javascript:BITDATACOLLECTIONDATA.clearImage('{2}');"" />
", SessionObject.CurrentSite.DomainName, field.MappingColumn, imgTargetElm); */

                returnHtml = String.Format(@"<img id=""img{2}"" src=""{0}[data-field]"" alt="""" data-field=""{1}"" data-title-field=""{1}""
                            style=""max-height: 150px; max-width: 150px"" /> <span id=""fileName{2}"" data-field=""{1}""></span>
                        <input type=""button"" value=""..."" onclick=""javascript:BITDATACOLLECTIONDATA.openImagePopup('{2}');"" />
                        <input type=""button"" value=""x"" title=""verwijder afbeelding"" onclick=""javascript:BITDATACOLLECTIONDATA.clearImage('{2}');"" />
", SessionObject.CurrentSite.DomainName, field.MappingColumn, imgTargetElm);

                //                string imgTargetElm = field.Type + field.MappingColumn.Replace(".", "");
                //                returnHtml = String.Format(@"<img id=""img{1}"" src=""../"" alt="""" data-field=""{0}"" data-title-field=""{0}""
                //                            style=""max-height: 150px; max-width: 150px"" /> <span id=""fileName{1}"" data-field=""{0}""></span>
                //                        <input type=""button"" value=""..."" onclick=""javascript:BITDATACOLLECTIONDATA.openImagePopup('{1}');"" />
                //                        <input type=""button"" value=""x"" title=""verwijder afbeelding"" onclick=""javascript:BITDATACOLLECTIONDATA.clearImage('{1}');"" />
                //", field.MappingColumn, imgTargetElm);
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