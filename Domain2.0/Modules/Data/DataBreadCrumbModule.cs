using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using System.Text.RegularExpressions;
using System.Web;
using BitPlate.Domain.DataCollections;

namespace BitPlate.Domain.Modules.Data
{
    [Persistent("Module")] 
    public class DataBreadCrumbModule : BaseDataModule, IRefreshableModule
    {
        public DataBreadCrumbModule()
        {
            ContentSamples.Add(@"U bevindt zich hier:{RootLink}Home{/RootLink} :: {List}{DrillDownLink}{Title}{/DrillDownLink} :: {/List}
            ");
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Data" });
           
        }

        protected override void setNavigationActions()
        {
            base.setNavigationActions();
            if (this.NavigationActions.Count == 0)
            {
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    Name = "{DrillDownLink}",
                    NavigationType = NavigationTypeEnum.NavigateToPage,

                });
            }


        }

        public override void SetAllTags()
        {

            this._tags.Add(new Tag() { Name = "{RootLink}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "<!--{List}-->", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });
            this._tags.Add(new Tag() { Name = "{List}", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });

            this._tags.Add(new Tag() { Name = "{DrillDownLink}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{DrillDownUrl}" });

            this._tags.Add(new Tag() { Name = "{Name}" });
            this._tags.Add(new Tag() { Name = "{Title}" });
            this._tags.Add(new Tag() { Name = "{ID}" });

        }



        public override string Publish2(CmsPage page)
        {
            string html = base.Publish2(page);
            try
            {
                string drillDownUrl = "";
                string drillDownLink = "";
                
                ModuleNavigationAction navigationActionDrillDown = this.GetNavigationActionByTagName("{DrillDownLink}");
                if (navigationActionDrillDown != null)
                {
                    
                    drillDownUrl = navigationActionDrillDown.GetNavigationUrl();
                    drillDownLink = navigationActionDrillDown.CreateNavigationHyperlink("G", refreshItself: true);
                }

                DataGroup group = null;
                if (dataType == "I")
                {
                    //als er op item wordt genavigeerd, parentgroep van item laden
                    //breadcrumb doet namelijk alleen groepen
                    DataItem item = BaseObject.GetById<DataItem>(dataId);
                    if (item != null)
                    {
                        group = item.ParentGroup;
                    }
                }
                else 
                {
                    group = BaseObject.GetById<DataGroup>(dataId);
                    
                }

                string itemTemplate = base.GetSubTemplate("{List}");
                string itemsHtml = "";

                if (group != null)
                {
                    group.TryTranslate(this.LanguageCode);
                    string itemHtml = GetItemHtml(drillDownUrl, drillDownLink, group, itemTemplate);
                    itemsHtml = itemHtml;
                    while (group.ParentGroup != null)
                    {
                        group = group.ParentGroup;
                        group.TryTranslate(this.LanguageCode);
                        itemHtml = GetItemHtml(drillDownUrl, drillDownLink, group, itemTemplate);
                        itemsHtml = itemHtml + itemsHtml;
                    }
                }

                html = html.Replace("<!--{List}-->" + itemTemplate + "<!--{/List}-->", itemsHtml);
                html = html.Replace("{List}" + itemTemplate + "{/List}", itemsHtml);

                html = html.Replace("{RootLink}", drillDownLink);
                html = html.Replace("{/RootLink}", "</a>");
                // html = html.Replace("{RootUrl}", drillDownUrl);
                html = html.Replace("{DrillDownUrl}", drillDownUrl);
                html = html.Replace("{ID}", Guid.Empty.ToString());
            }
            catch (Exception ex)
            {
                html = base.HandlePublishError(ex);
            }

            return html;

        }

        private static string GetItemHtml(string drillDownUrl, string drillDownLink, DataGroup group, string itemTemplate)
        {
            string itemHtml = itemTemplate;
            group.RewriteUrl = group.GetRewriteUrl(drillDownUrl, "G");
            itemHtml = itemHtml.Replace("{DrillDownLink}", drillDownLink);
            itemHtml = itemHtml.Replace("{/DrillDownLink}", "</a>");
            itemHtml = itemHtml.Replace("{DrillDownUrl}", group.RewriteUrl);
            itemHtml = itemHtml.Replace("{ID}", group.ID.ToString());
            itemHtml = itemHtml.Replace("{Name}", group.Name);
            itemHtml = itemHtml.Replace("{Title}", group.Title);
            return itemHtml;
        }


        public string Reload(CmsPage page, Dictionary<string, object> Parameters)
        {
            if (Parameters != null && Parameters.ContainsKey("dataid"))
            {
                this.dataId = new Guid(Parameters["dataid"].ToString());
                this.dataType = Parameters["datatype"].ToString();
            }
            this.Parameters = Parameters;
            return Publish2(page);
        }
    }
}
