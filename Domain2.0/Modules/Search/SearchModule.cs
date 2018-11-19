using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM.Attributes;
using System.Web;
using BitPlate.Domain.Utils;

namespace BitPlate.Domain.Modules.Search
{
    [Persistent("Module")]
    public class SearchModule: BaseModule
    {
        public SearchModule()
            : base()
        {
            ContentSamples.Add(@"{SearchTextbox} {SearchButton}Zoek{/SearchButton}");
            this.IncludeScripts.Add("/_js/BitFormValidation.js");

            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
            
        }

        public override void SetAllTags()
        {
            this._tags.Add(new Tag() { Name = "{SearchTextbox}" });
            this._tags.Add(new Tag() { Name = "{SearchButton}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{SearchLink}", HasCloseTag = true });
            //uitbreiden met zoeken in datacollecties
            //moet nog beter worden uitgewerkt: heeft gebruiker keuze mogelijkheid of alleen in config van module?
            //uitgezet vanweg performance en vanwege geen mogelijkheid labels te zetten
            //staat nu hardcoded in zoekmodule bij terrasana
            //ZoekResultsModule reageert wel al op datacollectie
            //todo: hier een checklist van maken (meer keuzes ipv 1)
            //string replacevalue = String.Format(@"<label><input type=""radio"" value="""" name=""bitRadioDataCollection"" /> doorzoek hele site</label>");
            //this._tags.Add(new Tag() { Name = "{RadioWholeSite}", ReplaceValue = replacevalue });
            //foreach (DataCollections.DataCollection col in Site.DataCollections)
            //{
            //    replacevalue = String.Format(@"<label><input type=""radio"" value=""{0}"" name=""bitRadioDataCollection"" /> zoek {1}</label>", col.ID, col.Name);
            //    //wat als naam datacollectie veranderd?
            //    this._tags.Add(new Tag() { Name = "{Radio" + col.Name + "}", ReplaceValue = replacevalue});
            //}
           // todo: uitbreiden met zoeken in taal

            base.SetAllTags();
        }

        protected override void setNavigationActions()
        {
            base.setNavigationActions();
            if (this.NavigationActions.Count == 0)
            {
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    Name = "{SearchLink}",
                    NavigationType = NavigationTypeEnum.NavigateToPage
                });
            }
        }

        
        public override string Publish2(CmsPage page)
        {
            string html =  base.Publish2(page);
            string searchString = "";
            if (HttpContext.Current.Request.QueryString["search"] != null)
            {
                searchString = HttpContext.Current.Request.QueryString["search"];
            }
            html = html.Replace("{SearchTextbox}", String.Format(@"<input type=""text"" ID=""bitTextBoxSearch{0:N}"" name=""bitTextBoxSearch"" value=""{1}"" onkeypress=""BITSITESCRIPT.checkEnterKeyPress(event, '{0}', BITSITESCRIPT.searchSite);""/>", ID, searchString));

            html = html.Replace("{SearchButton}", String.Format(@"<button type=""button"" ID=""bitButtonSearch{0:N}"" onclick=""BITSITESCRIPT.searchSite('{0}');return false;"" >", ID));
            html = html.Replace("{/SearchButton}", "</button>");

            html = html.Replace("{SearchLink}", String.Format(@"<a  ID=""bitLinkButtonSearch{0:N}"" href=""javascript:void(0)"" onclick=""BITSITESCRIPT.searchSite('{0}');"">", ID));
            html = html.Replace("{/SearchLink}", "</a>");

            html = ConvertTags(html);
            return html;
        }

        protected override string getModuleStartDiv()
        {
            string moduleStartDiv = base.getModuleStartDiv();
            ModuleNavigationAction navAction = NavigationActions[0];
                string navigationUrl = navAction.NavigationPage != null ? navAction.NavigationPage.RelativeUrl : "";
                string refreshModules = navAction.RefreshModules == null ? "" : String.Join(",", navAction.RefreshModules);
                moduleStartDiv += String.Format(@"
<input type=""hidden"" id=""hiddenModuleID{0:N}"" value=""{0}""/>
<input type=""hidden"" id=""hiddenModuleType{0:N}"" value=""{1}""/>
<input type=""hidden"" id=""hiddenModuleNavigationType{0:N}"" value=""{2}""/>
<input type=""hidden"" id=""hiddenRefreshModules{0:N}"" value=""{3}""/>
<input type=""hidden"" id=""hiddenNavigationUrl{0:N}"" value=""{4}""/>
", this.ID, this.Type, navAction.NavigationType, refreshModules, navigationUrl);

            return moduleStartDiv;
            }
        }
    }

