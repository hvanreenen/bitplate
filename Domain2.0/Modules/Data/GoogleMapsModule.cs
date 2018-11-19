using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using System.Text.RegularExpressions;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Utils;
using System.Web;

namespace BitPlate.Domain.Modules.Data
{
    [Persistent("Module")]
    public class GoogleMapsModule : BaseDataModule
    {
        public GoogleMapsModule()
        {
            ContentSamples.Add(@"
<table>
<tr><td>{SideBar}
{Name} | {Title} <br/>
{/SideBar}</td><td valign=""top"">{Map}</td></tr>
<tr><td></td><td>{SearchTextBoxAddress} {SearchDropDownDistance} {SearchButton}Zoek{/SearchButton}</td></tr>
</table>
{Marker}
    {Name}<br/>
    {Title}<br/>
    {PostCode} {Adres}<br/>    
    {Plaats}<br/>    
    {DrillDownLink}Details{/DrillDownLink}<br/>
{/Marker}
            ");
            IncludeScripts.Add("/_js/BitGoogleMapsModule.js");

            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Data" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Googlemaps instellingen", IsExternal = true, Url = "/_bitplate/EditPage/Modules/DataModules/GooglemapsModuleTab.aspx" });

        }

        protected override void setNavigationActions()
        {
            base.setNavigationActions();
            if (this.NavigationActions.Count == 0)
            {
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    Name = "{DrillDownLink}",
                    NavigationType = NavigationTypeEnum.NavigateToPage
                });
            }

        }

        protected override ValidationResult validateModule()
        {
            ValidationResult returnValue = base.validateModule();
            if (!returnValue.IsValid) return returnValue;
            ModuleNavigationAction navigationAction = this.GetNavigationActionByTagName("{DrillDownLink}");
            if (navigationAction.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {
                if (this.Content.Contains("{DrillDownUrl}"))
                {
                    returnValue.Message += "Modules met navigatie die op dezelfde pagina blijft mogen geen tags bevatten met {DrillDownUrl}.<br/>";
                }
            }
            if (Utils.StringHelper.CountOccurences(this.Content, "{List}") > 1)
            {
                returnValue.Message += "Module mag maar 1 keer tag bevatten: {List} (controleer ook code)<br/>";
            }
            if (Utils.StringHelper.CountOccurences(this.Content, "{/List}") > 1)
            {
                returnValue.Message += "Module mag maar 1 keer sluit-tag bevatten: {/List} (controleer ook code)<br/>";
            }
            return returnValue;
        }

        public override void SetAllTags()
        {
            //Tags waren niet zichtbaar in tag list.
            this._tags.Add(new Tag() { Name = "{Map}" });
            this._tags.Add(new Tag() { Name = "{SearchTextBoxAddress}" });
            this._tags.Add(new Tag() { Name = "{SearchDropDownDistance}" });
            this._tags.Add(new Tag() { Name = "{SearchDropDownCountries}" });
            this._tags.Add(new Tag() { Name = "{SearchButton}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{Marker}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{SideBar}", HasCloseTag = true });

            //this._tags.Add(new Tag() { Name = "{City}"});
            //this._tags.Add(new Tag() { Name = "{Address}" });
            //this._tags.Add(new Tag() { Name = "&lt;!--{List}--&gt;", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });
            //this._tags.Add(new Tag() { Name = "{List}", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });

            this._tags.Add(new Tag() { Name = "{DrillDownLink}", HasCloseTag = true });
            this._tags.AddRange(this.GetDataFieldTags());

        }


        public override string Publish2(CmsPage page)
        {
            string html = "";
            try
            {
                if (DataCollection != null) PrepairTemplate(DataCollection.DataItemFields, DataCollection.DataGroupFields);
                if (DataCollection == null) return base.Publish2(page);

                html = Content;
                string initAddress = getSetting<string>("InitialAddress");
                string initCountry = getSetting<string>("InitialCountry");
                string initSearch = "";
                if (initAddress == null || initAddress == "")
                {
                    initSearch = "Utrecht, Nederland";
                }
                else
                {
                    if (initCountry == null || initCountry == "")
                    {
                        initCountry = "Nederland";
                    }
                    initSearch = initAddress + ", " + initCountry;
                }

                GPoint initPoint = GoogleGeocoder.GetLatLng(initSearch, page.Site.GoogleMapsKey);

                string infoBalloonFormat = base.GetSubTemplate("{Marker}");
                string sideBarRowFormat = base.GetSubTemplate("{SideBar}");

                //LET WEL: vervanging van de datatags zoals {City} en {Address} gebeurt in javascript: BITGoogleMapModule.js --> formatHtml()
                html = html.Replace("{Marker}" + infoBalloonFormat + "{/Marker}", ""); //waardes worden in js var gezet
                html = html.Replace("{SideBar}" + sideBarRowFormat + "{/SideBar}", @"<div id=""bitGooglemapsSideBar""></div>"); //waardes worden in js var gezet
                html = html.Replace("<!--{SideBar}-->" + sideBarRowFormat + "<!--{/SideBar}-->", @"<div id=""bitGooglemapsSideBar""></div>"); //waardes worden in js var gezet

                infoBalloonFormat = infoBalloonFormat.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                infoBalloonFormat = infoBalloonFormat.Replace("'", "&quot;");

                sideBarRowFormat = sideBarRowFormat.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                sideBarRowFormat = sideBarRowFormat.Replace("'", "&quot;");
                
                string drillDownUrl = HttpContext.Current.Request.Url.LocalPath;
                string drillDownlink = "";
                ModuleNavigationAction navigationAction = this.GetNavigationActionByTagName("{DrillDownLink}");
                if (navigationAction.NavigationType == NavigationTypeEnum.NavigateToPage)
                {
                    if (navigationAction.NavigationPage != null)
                    {
                        drillDownUrl = navigationAction.NavigationPage.RelativeUrl;
                    }
                    drillDownlink = "<a href=\"{DrillDownUrl}\">";
                }

                else
                {
                    drillDownUrl = HttpContext.Current.Request.Url.LocalPath;
                    string refreshModules = String.Join(",", navigationAction.RefreshModules);
                    //haal eventuele lege waardes weg
                    refreshModules = refreshModules.Replace(",,", "");
                    if (refreshModules.EndsWith(","))
                    {
                        refreshModules = refreshModules.Remove(refreshModules.Length - 1, 1);
                    }

                    drillDownlink = "<a class=\"showDetailsInModules\" href=\"{DrillDownUrl}\" onclick=\"BITSITESCRIPT.reloadModulesOnSamePage('" + refreshModules + "', {dataid: '{ID}'})\">";
                }

                string hiddenfields = String.Format(@"
<!-- FIELDS FOR GOOGLEMAPS  -->
    <input type='hidden' id='hiddenGoogleMapsKey' value='{0}'/>
    <input type='hidden' id='hiddenInitLatitude' value='{1}'/>
    <input type='hidden' id='hiddenInitLongitude' value='{2}'/>
    <input type='hidden' id='hiddenInitZoom' value='{3}'/>
    <input type='hidden' id='hiddenDatacollectionID' value='{4}'/>
    <input type='hidden' id='hiddenInfoBalloonFormat' value='{5}'/>
    <input type='hidden' id='hiddenSideBarRowFormat' value='{6}'/>
    <input type='hidden' id='hiddenNavigationUrl' value='{7}'/>

", page.Site.GoogleMapsKey, initPoint.Lat.ToString().Replace(",", "."), initPoint.Long.ToString().Replace(",", "."), base.getSetting<string>("InitialZoom"), this.DataCollection.ID, infoBalloonFormat, sideBarRowFormat, drillDownUrl);


                html = getModuleStartDiv() + hiddenfields + html + getModuleEndDiv();
                int width = getSetting<int>("MapWidth"), height = getSetting<int>("MapHeight");

                if (width == 0) width = 400;
                if (height == 0) height = 300;

                html = html.Replace("{Map}", String.Format(@"<div id=""bitGoogleMap"" style=""width:{0}px;height:{1}px""></div>", width, height));



                html = html.Replace("{SearchTextBoxAddress}", String.Format(@"<input type=""text"" id=""bitGooglemapsSearchAddress"" value=""{0}"" onkeypress=""javascript:BITGOOGLEMAP.checkEnter(event);"">", initAddress));
                //onderstaande kilometers zijn bij benadering. Ze zijn namelijk ook afhankelijk van de grootte van de kaart op het scherm. Dus zoomlevel 12 is niet altijd 5 km.
                html = html.Replace("{SearchDropDownDistance}", @" <select id=""bitGooglemapsSearchDistance"">
                    <option value=""12"">5 km</option>
                    <option value=""11"">10 km</option>
                    <option value=""10"">20 km</option>
                    <option value=""9"">50 km</option>
                    <option value=""8"">100 km</option>
                    <option value=""7"">250 km</option>
                </select>");

                html = html.Replace("{SearchDropDownCountries}", @" <select id=""bitGooglemapsSearchCountry"">
                    <option value=""Nederland"">Nederland</option>
                    <option value=""Belgium"">België</option>
                    <option value=""Germany"">Duitsland</option>
                    <option value=""France"">Frankrijk</option>
                    <option value=""England"">Engeland</option>
                </select>");

                html = html.Replace("{SearchButton}", @"<button type=""button"" onclick=""BITGOOGLEMAP.searchAddress();""> ");
                html = html.Replace("{/SearchButton}", @"</button> ");


                html = html.Replace("{DrillDownLink}", drillDownlink);
                html = html.Replace("{/DrillDownLink}", @"</a> ");
                
                //vervang datafields, die blijven {tag} houden en worden pas in js vervangen door de veldwaarde
                //BITGoogleMapModule.js --> formatHtml()
                //data wordt geladen via service voor deze module
                if (navigationAction.NavigationType == NavigationTypeEnum.NavigateToPage)
                {
                    //{RewriteUrl} wordt vervangen in js
                    html = html.Replace("{DrillDownUrl}", "{RewriteUrl}");
                }
                if (DataCollection != null)
                {
                    foreach (DataField field in DataCollection.DataItemFields)
                    {
                        html = html.Replace("{" + field.Name + "}", "{" + field.MappingColumn + "}");
                    }
                }

            }
            catch (Exception ex)
            {
                html = base.HandlePublishError(ex);
            }


            return html;
        }



    }
}
