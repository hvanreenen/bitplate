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
using System.Data;
using HtmlAgilityPack;

namespace BitPlate.Domain.Modules.Data
{
    [Persistent("Module")]
    public class TreeViewModule : BaseDataModule, IRefreshableModule
    {



        private string treeViewTemplate;
        private string treeNodeTemplate;

        public TreeViewModule()
        {
            this.IncludeScripts.Add("/_js/BitTreeView.js");
            this.IncludeScripts.Add("/_js/jstree/jquery.jstree.js");
            this.IncludeScripts.Add("/_js/jstree/_lib/jquery.cookie.js");
            this.IncludeScripts.Add("/_js/jstree/_lib/jquery.hotkeys.js");
       

            ContentSamples.Add(@"
            <!--{TreeView}-->
            <ul>
                <!--{TreeNode}-->
                <li id=""{TreeNode.ID}"">
                    <span>{TreeNode.DrillDownLink}{TreeNode.Title}{/TreeNode.DrillDownLink}</span>
                    {TreeNode.ChildNodes}
                </li>
                <!--{/TreeNode}-->    
            </ul>
            <!--{/TreeView}-->");

            //<ul>
            //    {rootNodes}
            //        <li>
            //            <span>{rootNode.Name}</span>
            //            <ul>
            //                {subNodes}
            //                    <li>{subNode.DrillDownLink}{subNode.Name}{/subNode.DrillDownLink}</li>
            //                {/subNodes}
            //            </ul>
            //        </li>
            //    {/rootNodes}
            //</ul>

            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Data" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Tree", IsExternal = true, Url = "/_bitplate/EditPage/Modules/DataModules/TreeviewModuleTab.aspx" });

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

        public override void SetAllTags()
        {
            //Tags waren niet zichtbaar in tag list.
            this._tags.Add(new Tag() { Name = "<!--{TreeView}-->", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "<!--{TreeNode}-->", HasCloseTag = true });

            this._tags.Add(new Tag() { Name = "{TreeNode.DrillDownLink}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{TreeNode.Name}" });
            this._tags.Add(new Tag() { Name = "{TreeNode.Title}" });
            this._tags.Add(new Tag() { Name = "{TreeNode.ID}" });
            this._tags.Add(new Tag() { Name = "{TreeNode.ChildNodes}" });

            //this._tags.AddRange(this.GetDataFieldTags());

        }



        public string BuildTree2(CmsPage page)
        {
            int viewMode = 1;//int.Parse(this.ViewModeCheckBoxList.SelectedValue);

            this.treeViewTemplate = base.GetSubTemplate("{TreeView}");
            this.treeNodeTemplate = base.GetSubTemplate("{TreeNode}");

            string tree = treeViewTemplate;
            string rootNodes = "";
            string select = "SELECT DataGroup.ID, DataGroup.Name, DataGroup.FK_Parent_Group, DataGroup.CompletePath, ";
            string from = " FROM DataGroup ";
            if (!DataCollection.HasStaticLanguage && this.LanguageCode != "" && this.LanguageCode != Site.DefaultLanguage)
            {
                DataField titleField = DataField.GetFirst<DataField>("FK_DataCollection = '" + DataCollection.ID + "' AND MappingColumn='Title' AND type='group'");
                if (titleField.IsMultiLanguageField)
                {
                    select += "Lang.Title AS Title ";
                    from += " JOIN DataGroupLanguage AS Lang ON DataGroup.ID = Lang.FK_DataGroup AND Lang.LanguageCode = '" + this.LanguageCode + "'";
                }
                else
                {
                    select += "DataGroup.Title ";
                }
            }
            else
            {
                select += "DataGroup.Title ";
            }

            string where = String.Format(" WHERE DataGroup.FK_DataCollection = '{0}' ", DataCollection.ID);
            string sort = getSort(DataCollection.DataGroupFields); //" ORDER BY DataGroup.OrderNumber";
            string sql = select + from + where + sort;

            System.Data.DataTable allGroupsTable = DataBase.Get().GetDataTable(sql);

            string drillDownUrl = "";
            string drillDownLink = "";
            ModuleNavigationAction navigationActionDrillDown = this.GetNavigationActionByTagName("{DrillDownLink}");
            if (navigationActionDrillDown != null)
            {
                drillDownUrl = navigationActionDrillDown.GetNavigationUrl();
                drillDownLink = navigationActionDrillDown.CreateNavigationHyperlink("G", false, "BITTREEVIEW.isReloadRequired = false;");
            }


            foreach (System.Data.DataRow dataRow in allGroupsTable.Rows)//.Select("FK_Parent_Group Is Null", sort.Replace("ORDER BY ", "")))
            {
                //DataTable.Select() sorteert de items opnieuw. Dit is niet de bedoeling.
                if (dataRow["FK_Parent_Group"] == DBNull.Value)
                {
                    string rewriteUrl = CreateRewriteUrl(page, drillDownUrl, "G", new Guid(dataRow["ID"].ToString()), dataRow["CompletePath"].ToString(), dataRow["Title"].ToString());

                    string rootNode = this.treeNodeTemplate;
                    string name = dataRow["Name"].ToString();
                    rootNode = rootNode
                        .Replace("{TreeNode.DrillDownLink}", drillDownLink)
                        .Replace("{TreeNode.Name}", dataRow["Name"].ToString())
                        .Replace("{TreeNode.Title}", dataRow["Title"].ToString())
                        .Replace("{/TreeNode.DrillDownLink}", "</a>")
                        .Replace("{TreeNode.ID}", "TreeNode" + dataRow["ID"].ToString().Replace("-", ""))
                        .Replace("{DrillDownUrl}", rewriteUrl)
                        .Replace("{ID}", dataRow["ID"].ToString())
                        .Replace("{TreeNode.ChildNodes}", this.BuildTreeNode2(page, allGroupsTable, rootNode, dataRow["ID"].ToString(), viewMode, drillDownUrl, drillDownLink));

                    rootNodes += rootNode;
                }
            }


            /* if (viewMode == 2) //groups and items
            {
                foreach (DataItem item in dataCollection.Items)
                {
                    TreeNode itemNode = new TreeNode();
                    itemNode.Text = itemNode.Text;
                    itemNode.Value = item.ID.ToString();
                    DataCollectionTreeView.Nodes.Add(itemNode);
                }
            } */
            tree = Content;
            tree = tree.Replace("<!--{TreeNode}-->" + treeNodeTemplate + "<!--{/TreeNode}-->", rootNodes);
            tree = tree.Replace("{TreeNode}" + treeNodeTemplate + "{/TreeNode}", rootNodes);
            string treeDiv = "<div class=\"bitTree\" data-tree-selected-id=\"TreeNode" + dataId.ToString("N") + "\">";
            tree = tree.Replace("<!--{TreeView}-->", treeDiv);
            tree = tree.Replace("{TreeView}", treeDiv);
            tree = tree.Replace("<!--{/TreeView}-->", "</div>");
            tree = tree.Replace("{/TreeView}", "</div>");

            return tree;
        }

        private string getModuleSettingsDiv()
        {
            string html = "";
            bool themeSupport = getSetting<bool>("ThemeSupport");
            bool openAllNodes = getSetting<bool>("OpenAllInitialy");
            int AnimationSpeed = getSetting<int>("AnimationSpeed");

            Dictionary<string, object> liveSettings = new Dictionary<string, object>();
            liveSettings.Add("themeSupport", themeSupport.ToString().ToLower());
            liveSettings.Add("openAllNodesInitialy", openAllNodes.ToString().ToLower());
            liveSettings.Add("AnimationSpeed", AnimationSpeed);
            html = "<div class=\"modulesettings\" data-module-settings=\"" + liveSettings.ToJsonString().Replace("\"", "-") + "\"></div>";
            return html;
        }

        private string BuildTreeNode2(CmsPage page, DataTable allGroupsTable, string rootNode, string FK_Parent_Group, int viewMode, string drillDownUrl, string drillDownLink)
        {
            DataRow[] dataRows = allGroupsTable.Select("FK_Parent_Group = '" + FK_Parent_Group + "'");
            string childRoot = (dataRows.Length > 0) ? this.treeViewTemplate : "";
            string childNodes = "";
            foreach (DataRow dataRow in dataRows)
            {
                string rewriteUrl = CreateRewriteUrl(page, drillDownUrl, "G", new Guid(dataRow["ID"].ToString()), dataRow["CompletePath"].ToString(), dataRow["Title"].ToString());

                string childNode = this.treeNodeTemplate;
                childNode = childNode
                    .Replace("{TreeNode.DrillDownLink}", drillDownLink)
                    .Replace("{TreeNode.Name}", dataRow["Name"].ToString())
                    .Replace("{TreeNode.Title}", dataRow["Title"].ToString())
                    .Replace("{/TreeNode.DrillDownLink}", "</a>")
                    .Replace("{TreeNode.ID}", "TreeNode" + dataRow["ID"].ToString().Replace("-", ""))
                    .Replace("{DrillDownUrl}", rewriteUrl)
                    .Replace("{ID}", dataRow["ID"].ToString())
                    .Replace("{TreeNode.ChildNodes}", this.BuildTreeNode2(page, allGroupsTable, rootNode, dataRow["ID"].ToString(), viewMode, drillDownUrl, drillDownLink));

                childNodes += childNode;

            }

            //if (viewMode == 2)
            //{
            //    foreach (DataItem item in dataGroup.Items)
            //    {
            //        TreeNode itemNode = new TreeNode();
            //        itemNode.Text = itemNode.Text;
            //        itemNode.Value = item.ID.ToString();
            //        rootNode.ChildNodes.Add(itemNode);
            //    }
            //}
            //childRoot = childRoot.Replace("{ReplaceNodes}", childNodes);
            childRoot = childRoot.Replace("<!--{TreeNode}-->" + treeNodeTemplate + "<!--{/TreeNode}-->", childNodes);
            childRoot = childRoot.Replace("{TreeNode}" + treeNodeTemplate + "{/TreeNode}", childNodes);

            return childRoot;
        }

        public override string Publish2(CmsPage page)
        {
            string html = "";
            try
            {
                if (DataCollection == null) return base.Publish2(page);
                bool prePublishData = getSetting<bool>("PrePublishData");
                if (prePublishData && PrePublishedContent != "")
                {
                    //PrePublishData needs an uptodate selected TreeNode.
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(PrePublishedContent);
                    List<HtmlNode> htmlNodes = document.DocumentNode.ChildNodes.Where(c => c.Attributes.Any(x => x.Name == "class" && x.Value.Contains("bitTree"))).ToList();
                    if (htmlNodes.Count > 0)
                    {
                        HtmlNode node = htmlNodes[0];
                        node.Attributes["data-tree-selected-id"].Value = "TreeNode" + dataId.ToString("N");
                        return getModuleStartDiv() + getModuleSettingsDiv() + node.OuterHtml + getModuleEndDiv();
                    }
                    return getModuleStartDiv() + getModuleSettingsDiv() + PrePublishedContent + getModuleEndDiv();
                }

                html = getModuleStartDiv();
                html += getModuleSettingsDiv();

                string tree = BuildTree2(page);
                html += tree;
                html += getModuleEndDiv();

                //Prepublish geactiveerd, maar geen data? Sla de content dan op.
                if (prePublishData)
                {
                    PrePublishedContent = BuildTree2(page);
                    this.Save();
                }
            }
            catch (Exception ex)
            {
                html = base.HandlePublishError(ex);
            }
            return html;
        }

        public override void Save()
        {
            base.Save();
        }
        public string Reload(CmsPage page, Dictionary<string, object> Parameters)
        {
            if (Parameters != null && Parameters.ContainsKey("dataid"))
            {
                this.dataId = new Guid(Parameters["dataid"].ToString());
            }
            //hier openklappen
            return Publish2(page);
        }
    }
}
