using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Modules.Data;
using HJORM;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitSite._bitPlate.EditPage.Modules.DataModules
{
    public partial class TreeViewModuleControl : BaseDataModuleUserControl
    {
        private Literal treeViewLiteral;
        private LinkButton linkButtonDrillDown;
        private BaseDataModule baseDataModule;

        //Templates
        private string orginalTreeViewTemplate;
        private string orginalTreeNodeTemplate;

        private string treeViewTemplate;
        private string treeNodeTemplate;

        private string selectedTreeNode = "";
        /*  Tags 
            {TreeView}
            {TreeNode}

            {TreeNode.DrilDownLink}
            {TreeNode.Name}
            {TreeNode.ChildNodes}
         * 
         * 
         * "
            {TreeView}
            <ul>
                {TreeNode}
                <li>
                    <span>{TreeNode.DrilDownLink}{TreeNode.Name}{/TreeNode.DrilDownLink}</span>
                    {TreeNode.ChildNodes}
                </li>
                {/TreeNode}     
            </ul>
            {/TreeView}"
         */


        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Page_Load Start", "TreeViewModuleControl");
            base.Load(sender, e);
            this.LoadSettings();
            bool prePublishData = getSetting<bool>("PrePublishData");
            if (!IsPostBack && !prePublishData)
            {
                this.MakeTree();
            }
            else
            {
                Guid dataid = Guid.Empty;
                Guid.TryParse(Request["__EVENTARGUMENT"], out dataid);
                this.UpdateSelectedNode(dataid);
            }
            System.Diagnostics.Trace.WriteLine("Page_Load End", "TreeViewModuleControl");
        }

        

        private void MakeTree()
        {
            this.treeViewLiteral = (Literal)this.FindControl("TreeViewLiteral" + this.ModuleID.ToString("N"));
            
            if (this.treeViewLiteral != null)
            {
                this.LoadModuleObject();
                this.orginalTreeViewTemplate = this.module.Content; //HACK module content veranderd in drilldown. Content kan niet uit literal worden gehaald op dat moment.
                this.orginalTreeNodeTemplate = Regex.Match(this.orginalTreeViewTemplate, "{TreeNode}(.*?){/TreeNode}", RegexOptions.Singleline).ToString();
                this.treeViewTemplate = this.orginalTreeViewTemplate.Replace("{TreeView}", "").Replace("{/TreeView}", "");
                this.treeViewTemplate = Regex.Replace(this.treeViewTemplate, "{TreeNode}(.*?){/TreeNode}", "{ReplaceNodes}", RegexOptions.Singleline);
                this.treeNodeTemplate = this.orginalTreeNodeTemplate.Replace("{TreeNode}", "").Replace("{/TreeNode}", "");
                if (this.DataCollectionID != null && this.DataCollectionID != Guid.Empty)
                {
                    this.LoadData(this.DataCollectionID);
                    this.LabelMsg.Text = "";
                }
                else
                {
                    this.treeViewLiteral.Text = "";
                    this.LabelMsg.Text = "Kies eerst een datacollectie";
                }
            }
            else
            {
                this.treeViewLiteral.Text = "";
                this.LabelMsg.Text = "Kan de TreeViewLiteral niet vinden.";
            }

        }


        private void LoadData(Guid dataCollectionID)
        {
            int viewMode = 1;//int.Parse(this.ViewModeCheckBoxList.SelectedValue);

            //DataCollection dataCollection = BaseObject.GetById<DataCollection>(dataCollectionID);
            string treeRoot = this.treeViewTemplate;
            string rootNodes = "";
            string where = String.Format("FK_DataCollection = '{0}' AND FK_Parent_group Is Null", dataCollectionID);
            BaseCollection<DataGroup> rootGroups = BaseCollection<DataGroup>.Get(where);
            //foreach (DataGroup group in dataCollection.Groups.Where(c => c.ParentGroup == null))
            foreach (DataGroup group in rootGroups)
            
            {
                //todo:  vanonderstaande code en BuildTreeNode() meer 1 functie van maken (met eventueel extra parameter

                //string drillDownLink = String.Format(@"<div style=""display: none;""><asp:LinkButton runat=""server"" ID=""dummyLinkButton{0:N}"" ClientIDMode=""AutoID"" OnClick=""DrillDownLink_Click"" Text='<%# DataBinder.Eval(Container.DataItem,""ID"") %>' CommandArgument='<%# DataBinder.Eval(Container.DataItem,""ID"") %>'>dummy<%# DataBinder.Eval(Container.DataItem,""ID"") %></asp:LinkButton></div>", this.ModuleID);
                //        drillDownLink += String.Format(@"<asp:HyperLink ClientIDMode=""AutoID""  ID=""link{0:N}"" runat=""server"" NavigateUrl='<%#DataBinder.Eval(Container.DataItem,""RewriteUrl"")%>' CssClass=""showDetailsInModules"">", this.ModuleID);
                string drillDownLink = buildDrillDownLink(group);
                string rootNode = this.treeNodeTemplate;
                rootNode = rootNode
                    .Replace("{TreeNode.DrillDownLink}", drillDownLink)
                    .Replace("{TreeNode.Name}", group.Name)
                    .Replace("{/TreeNode.DrillDownLink}", "</a>")
                    .Replace("{TreeNode.ID}", "TreeNode" + group.ID.ToString("N"))
                    .Replace("{TreeNode.ChildNodes}", this.BuildTreeNode(rootNode, group, viewMode));

                rootNodes += rootNode;

                /* TreeNode node = new TreeNode();
                node.Text = group.Name;
                node.Value = group.ID.ToString();
                node.NavigateUrl = group.RewriteUrl;
                node = BuildTreeNode(node, group, viewMode);
                DataCollectionTreeView.Nodes.Add(node); */
            }


            /* if (viewMode == 2)
            {
                foreach (DataItem item in dataCollection.Items)
                {
                    TreeNode itemNode = new TreeNode();
                    itemNode.Text = itemNode.Text;
                    itemNode.Value = item.ID.ToString();
                    DataCollectionTreeView.Nodes.Add(itemNode);
                }
            } */
            treeRoot = treeRoot.Replace("{ReplaceNodes}", rootNodes);
            this.treeViewLiteral.Text = "<div class=\"bitTree\" id=\"BitTree" + this.ModuleID.ToString("N") + "\" data-tree-selected-id=\"" + this.selectedTreeNode + "\">" + treeRoot + "</div>"; 
        }

        private string buildDrillDownLink(DataGroup group)
        {
            string drillDownLink = "";

            ModuleNavigationActionLite navigationAction = this.GetNavigationActionByTagName("{DrillDownLink}");
            string navigationUrl = navigationAction.NavigationUrl;
            if (navigationAction.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {
                //blijven op dezelfde pagina
                navigationUrl = Request.Url.LocalPath;
                group.RewriteUrl = group.GetRewriteUrl(navigationUrl);

                //per module is er bij NavigationTypeEnum.ShowDetailsInModules een dummylinkbutton aangemaakt.
                //Deze staat bovein de usercontrol
                //Het  server event van deze dummylinkbutton is aan te roepen met  javascript:__doPostBack()
                //je doet net alsof de click van de dummylinkbutton, maar gebruiker klik op een andere link 
                //je hebt dan de clientname nodig van de button
                //tweede parameter in __doPostBack() = eventargs = dataid
                string dummyLinkButtonName = this.Controls[3].ClientID; //Linkbutton is altijd 3de element todo: deze code kan beter
                dummyLinkButtonName = dummyLinkButtonName.Replace("_", "$");
                //HYJAX: In BitSiteScript.js worden de hrefs vervangen door onClick events. 
                //Hrefs wel nodig voor seo met ajax
                drillDownLink = String.Format(@"<a class=""showDetailsInModules"" href=""{0}"" onclick=""javascript:__doPostBack('{1}','{2}');"">", group.RewriteUrl, dummyLinkButtonName, group.ID);
            }
            else
            {
                group.RewriteUrl = group.GetRewriteUrl(navigationUrl);
                drillDownLink = String.Format(@"<a href=""{0}"">", group.RewriteUrl);
            }
            return drillDownLink;
        }

        private string BuildTreeNode(string rootNode, DataGroup dataGroup, int viewMode)
        {
            BaseCollection<DataGroup> subGroups = dataGroup.GetSubGroups();
            string childRoot = (subGroups.Count > 0) ? this.treeViewTemplate : "";
            string childNodes = "";
            foreach (DataGroup group in subGroups)
            {
                //string drillDownLink = String.Format(@"<div style=""display: none;""><asp:LinkButton runat=""server"" ID=""dummyLinkButton{0:N}"" ClientIDMode=""AutoID"" OnClick=""DrillDownLink_Click"" Text='<%# DataBinder.Eval(Container.DataItem,""ID"") %>' CommandArgument='<%# DataBinder.Eval(Container.DataItem,""ID"") %>'>dummy<%# DataBinder.Eval(Container.DataItem,""ID"") %></asp:LinkButton></div>", this.ModuleID);
                //drillDownLink += String.Format(@"<asp:HyperLink ClientIDMode=""AutoID""  ID=""link{0:N}"" runat=""server"" NavigateUrl='<%#DataBinder.Eval(Container.DataItem,""RewriteUrl"")%>' CssClass=""showDetailsInModules"">", this.ModuleID);

                string drillDownLink = buildDrillDownLink(group);
                string childNode = this.treeNodeTemplate;
                childNode = childNode
                    .Replace("{TreeNode.DrillDownLink}", drillDownLink)
                    .Replace("{TreeNode.Name}", group.Name)
                    .Replace("{/TreeNode.DrillDownLink}", "</a>")
                    .Replace("{TreeNode.ID}", "TreeNode" + group.ID.ToString("N"))
                    .Replace("{TreeNode.ChildNodes}", this.BuildTreeNode(childNode, group, viewMode));
                childNodes += childNode;
                //TreeNode childNode = new TreeNode();
                //childNode.Text = group.Name;
                //childNode.Value = group.ID.ToString();

                //if (viewMode == 2)
                //{

                //}

                //childNode = BuildTreeNode(childNode, group, viewMode);
                //rootNode.ChildNodes.Add(childNode);
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
            childRoot = childRoot.Replace("{ReplaceNodes}", childNodes);
            return childRoot;
        }

        protected void DrillDownLink_Click(Object sender, EventArgs args)
        {
            ModuleNavigationActionLite action = GetNavigationActionByTagName("{DrillDownLink}");
            if (action.NavigationType == NavigationTypeEnum.NavigateToPage)
            {
                Response.Redirect(action.NavigationUrl);
            }
            else if (action.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {
                Guid dataId;
                //click van linkbutton wordt via javascript:__doPostback() gedaan
                //de link is dus geen echte linkbutton
                //dataid staat in eventArgument van deze postbackfunctie
                string eventArgument = Request.Params.Get("__EVENTARGUMENT");
                Guid.TryParse(eventArgument, out dataId);

                foreach (string drillDownModuleId in action.RefreshModules)
                {
                    BaseModuleUserControl moduleControl = (BaseModuleUserControl)FindControlRecursive(this.Page.Master, "Mod" + drillDownModuleId.Replace("-", ""));
                    if (moduleControl != null)
                    {
                        moduleControl.Reload(this, new NavigationParameterObject() { Name = "dataid", GuidValue = dataId }); //{  //SelectAndShowData(dataId);
                    }
                }
                //stuur eventueel javascript naar de browser zodat deze wordt uitgevoerd nadat pagina opnieuw is gerenderd
                LinkButton linkButton = (LinkButton)sender;
                string js = linkButton.OnClientClick;
                if (js != null && js != "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "navigation" + this.ModuleID.ToString("N"), js, true);
                }
            }
        }

        public override void Reload(BaseModuleUserControl sender, NavigationParameterObject args = null)
        {
            if (base.CheckAutorisation())
            {
                if (args != null && args.Name == "dataid")
                {
                    //Niet nodig in !ispostback wordt het dataid argument altijd uit gelezen bij een postback.
                    //Het vinkje bij reload doet dus ook geen vinkert meer.
                    //this.UpdateSelectedNode(args.GuidValue);
                }
            }
        }

        private void UpdateSelectedNode(Guid dataid)
        {
            //MakeTree alternative. Het geselecteerde Tree ID vervangen in het data-tree-selected-id attribute. Dit is nodig om er voor te zorgen dat jstree weet welke node het moet openen na de refresh.
            this.treeViewLiteral = (Literal)this.FindControl("TreeViewLiteral" + this.ModuleID.ToString("N"));
            if (this.treeViewLiteral != null)
            {
                DataItem testItem = BaseObject.GetById<DataItem>(dataid);
                if (testItem != null && testItem.ParentGroup != null)
                {
                    dataid = testItem.ParentGroup.ID;
                }
                this.selectedTreeNode = "TreeNode" + dataid.ToString("N");
                
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(this.treeViewLiteral.Text);
                HtmlNode node = document.GetElementbyId("BitTree" + this.ModuleID.ToString("N"));
                if (node.Attributes.Contains("data-tree-selected-id"))
                {
                    node.Attributes["data-tree-selected-id"].Value = this.selectedTreeNode;
                }
                //this.MakeTree();
                this.treeViewLiteral.Text = node.OuterHtml;
            }
            
        }
    }
}