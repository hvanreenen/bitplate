using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using System.Text.RegularExpressions;
using BitPlate.Domain.DataCollections;
using System.Web;

namespace BitPlate.Domain.Modules.Data
{
    [Persistent("Module")]
    public class GroupListModule : BaseDataModule, IRefreshableModule
    {
        private string drillDownUrl = "", drillDownLink = "";
        private string drillUpUrl = "", drillUpLink = "";

        public GroupListModule()
        {
            ContentSamples.Add(@"
            <table>
                <thead>
                  <tr>
                    <td>
                      Naam
                    </td>
                    <td>
                      Title
                    </td>
                  </tr>
                </thead>
                <tbody>
       <!--{List}-->
                <tr>
                    <td>
                      {DrillDownLink}{Naam}{/DrillDownLink}
                    </td>
                    <td>
                      {Titel}
                    </td>
                </tr>
        <!--{/List}-->
                </tbody>
            </table>");

            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Navigation" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Data" });
        }

        protected override ValidationResult validateModule()
        {
            ValidationResult returnValue = base.validateModule();
            if (!returnValue.IsValid) return returnValue;
            ModuleNavigationAction navigationAction = this.GetNavigationActionByTagName("{DrillDownLink}");
            //modules die opzelfde pagina blijven moet in <a> de class worden gezet en wordt er een onclick functie in aangemaakt. Hierom moeten dit complete links zijn en niet alleen url
            if (navigationAction.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {
                if (this.Content.Contains("{DrillDownUrl}"))
                {
                    returnValue.Message += "Modules met navigatie die op dezelfde pagina blijft mogen geen tags bevatten met {DrillDownUrl}. Gebruik {DrillDownLink} hiervoor in de plaats.<br/>";
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
            if (this.NavigationActions.Count == 1)
            {
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    Name = "{DrillUpLink}",
                    NavigationType = NavigationTypeEnum.NavigateToPage,

                });
            }

        }

        public override void SetAllTags()
        {
            //this._tags.Add(new Tag() { Name = "&lt;!--{List}--&gt;", HasCloseTag = true, ReplaceValueCloseTag = "</ItemTemplate></asp:Repeater>" });
            //this._tags.Add(new Tag() { Name = "{List}", HasCloseTag = true, ReplaceValueCloseTag = "</ItemTemplate></asp:Repeater>" });
            //Tags waren niet zichtbaar in tag list.
            this._tags.Add(new Tag() { Name = "<!--{List}-->", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });
            this._tags.Add(new Tag() { Name = "{List}", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });

            this._tags.Add(new Tag() { Name = "{DrillDownLink}", HasCloseTag = true, ReplaceValueCloseTag = "</asp:HyperLink>" });
            this._tags.Add(new Tag() { Name = "{DrillUpLink}", HasCloseTag = true, ReplaceValueCloseTag = "</asp:HyperLink>" });
            this._tags.Add(new Tag() { Name = "{DrillDownUrl}", ReplaceValue = "MANUEL REPLACEMENT" });

            this._tags.AddRange(this.GetDataFieldTags());

        }






        //public string ConvertHeaderFooterTags(string html)
        //{
        //    //{Parent.Naam}
        //    string header = Regex.Match(html, "<HeaderTemplate>(.*?)</HeaderTemplate>", RegexOptions.Singleline).ToString();
        //    html = ConvertHtml(html, header);
        //    string footer = Regex.Match(html, "<FooterTemplate>(.*?)</FooterTemplate>", RegexOptions.Singleline).ToString();
        //    html = ConvertHtml(html, footer);
        //    return html;
        //}

        //public string ConvertHtml(string html, string source)
        //{
        //    if (source != null)
        //    {
        //        string convertedHeader = source;
        //        MatchCollection tags = Regex.Matches(source, "{Parent(.*?)}");
        //        foreach (Match tagMatch in tags)
        //        {
        //            string field = tagMatch.ToString().Replace("{Parent.", "").Replace("}", "");
        //            Tag tag = this._tags.Where(c => c.Name == "{Parent." + field + "}").FirstOrDefault();
        //            if (tag != null)
        //            {
        //                string mappingColumn = tag.ReplaceValue.Replace("<%#DataBinder.Eval(Container.DataItem,\"", "").Replace("\")%>", "");
        //                convertedHeader = convertedHeader.Replace(tagMatch.ToString(), "<%# " + mappingColumn + " %>");
        //            }
        //        }
        //        html = html.Replace(source, convertedHeader);
        //    }
        //    return html;
        //}

        public override string Publish2(CmsPage page)
        {
            string html = "";
            try
            {
                if (DataCollection != null) PrepairTemplate(DataCollection.DataGroupFields, DataCollection.DataGroupFields);
                html = base.Publish2(page);
                if (DataCollection == null) return html;


                ModuleNavigationAction navigationActionDrillDown = this.GetNavigationActionByTagName("{DrillDownLink}");
                if (navigationActionDrillDown != null)
                {
                    drillDownUrl = navigationActionDrillDown.GetNavigationUrl();
                    drillDownLink = navigationActionDrillDown.CreateNavigationHyperlink("G");
                }

                if (html.Contains("{DrillUpLink}"))
                {
                    ModuleNavigationAction navigationActionDrillUp = this.GetNavigationActionByTagName("{DrillUpLink}");
                    if (navigationActionDrillUp != null)
                    {
                        drillUpUrl = navigationActionDrillUp.GetNavigationUrl();
                        drillUpLink = navigationActionDrillUp.CreateNavigationHyperlink("G");
                    }
                }
                //getDrillDownUrlAndLink(out drillDownUrl, out drillDownLink);
                ShowDataEnum showdataBy = getSetting<ShowDataEnum>("ShowDataBy");
                System.Data.DataTable groupsTable = new System.Data.DataTable();
                if (showdataBy == ShowDataEnum.UserSelectNoDefaultData)
                {
                    if (this.dataId != Guid.Empty && this.dataId != null)
                    {
                        groupsTable = getDataTable("DataGroup", "G", DataCollection.DataGroupFields);
                    }
                }
                else
                {
                    groupsTable = getDataTable("DataGroup", "G", DataCollection.DataGroupFields);
                }

                string rowTemplate = base.GetSubTemplate("{List}");
                string rowsHtml = "";
                foreach (System.Data.DataRow dataRow in groupsTable.Rows)
                {
                    string rowHtml = rowTemplate;
                    string rewriteUrlDrillDown = CreateRewriteUrl(page, drillDownUrl, "G", new Guid(dataRow["ID"].ToString()), dataRow["CompletePath"].ToString(), dataRow["Title"].ToString());
                    rowHtml = rowHtml.Replace("{DrillDownLink}", drillDownLink);
                    rowHtml = rowHtml.Replace("{/DrillDownLink}", "</a>");
                    rowHtml = rowHtml.Replace("{DrillDownUrl}", rewriteUrlDrillDown);
                    foreach (System.Data.DataColumn column in dataRow.Table.Columns)
                    {
                        rowHtml = ReplaceTag(rowHtml, dataRow, column);
                        //rowHtml = rowHtml.Replace("{" + column.ColumnName + "}", dataRow[column.ColumnName].ToString());
                        if (rowHtml.Contains("{FileList1}"))
                        {
                            rowHtml = fillExtraFilesSubTemplate(rowHtml, "", dataRow["ID"].ToString());
                        }
                        if (html.Contains("{ImageList1}"))
                        {
                            rowHtml = fillExtraImagesSubTemplate(rowHtml, "", dataRow["ID"].ToString());
                        }

                    }

                    rowsHtml += rowHtml;
                }
                html = html.Replace("<!--{List}-->" + rowTemplate + "<!--{/List}-->", rowsHtml);
                html = html.Replace("{List}" + rowTemplate + "{/List}", rowsHtml);

                //parent-tags kunnen ook buiten de list-template staan. dus buiten {List} en {/List}
                //dan ook deze tags vervangen, ook als er geen rows zouden zijn. (parent kan er nog wel zijn)
                if (html.Contains("{Parent") || html.Contains("{DrillUp"))
                {
                    //dataId = dataId == Guid.Empty ? Guid.Parse(getSetting<string>("SelectGroupID")) : dataId;
                    System.Data.DataRow parentGroupRow = base.getDataParentGroupRow(dataId);
                    html = ReplaceDrillUpTag(page, html, parentGroupRow["FK_Parent_Group"].ToString());
                    foreach (System.Data.DataColumn column in parentGroupRow.Table.Columns)
                    {
                        html = html.Replace("{" + column.ColumnName + "}", parentGroupRow[column.ColumnName].ToString());
                    }
                }

                bool hideWhenNoData = getSetting<bool>("HideWhenNoData");
                if (hideWhenNoData && groupsTable.Rows.Count == 0)
                {
                    html = getModuleStartDiv() + getModuleEndDiv();
                }
            }
            catch (Exception ex)
            {
                html = base.HandlePublishError(ex);
            }
            return html;
        }


        protected string ReplaceDrillUpTag(CmsPage page, string html, string parentID)
        {
            if (parentID != "")
            {
                System.Data.DataRow parentParentGroupRow = base.getDataParentGroupRow(new Guid(parentID));

                string rewriteUrlDrillUp = CreateRewriteUrl(page, drillUpUrl, "G", new Guid(parentID), parentParentGroupRow["CompletePath"].ToString(), parentParentGroupRow["ParentTitle"].ToString());
                drillUpLink = drillUpLink.Replace("{ID}", parentID);
                html = html.Replace("{DrillUpLink}", drillUpLink);
                html = html.Replace("{/DrillUpLink}", "</a>");
                html = html.Replace("{DrillUpUrl}", rewriteUrlDrillUp);
                html = html.Replace("{ParentParent.Naam}", parentParentGroupRow["ParentName"].ToString());
                html = html.Replace("{ParentParent.Titel}", parentParentGroupRow["ParentTitle"].ToString());
            }
            else
            {
                drillUpLink = drillUpLink.Replace("{ID}", Guid.Empty.ToString());
                html = html.Replace("{DrillUpLink}", drillUpLink);
                html = html.Replace("{/DrillUpLink}", "</a>");
                html = html.Replace("{DrillUpUrl}", drillUpUrl);
                html = html.Replace("{ParentParent.Naam}", this.DataCollection.Name);
                html = html.Replace("{ParentParent.Titel}", this.DataCollection.Name);

                //html = Regex.Replace(html, "{DrillUpLink}(.*?){/DrillUpLink}", "");
            }
            return html;
        }
        protected override string getWhere(string tableAlias)
        {
            string where = base.getWhere(tableAlias);
            ShowDataEnum showDataBy = base.getSetting<ShowDataEnum>("ShowDataBy");
            string selectGroupID = getSetting<string>("SelectGroupID");

            if (showDataBy == ShowDataEnum.AllGroups)
            {
                //als vaste groep is gekozen dan deze nemen
                //if (selectGroupID != null && selectGroupID != "")
                //{
                //    where += String.Format(" AND {0}.FK_Parent_Group = '{1}'", tableAlias, selectGroupID);
                //}
                //else {
                ////doe niks
                //}
            }
            else if (showDataBy == ShowDataEnum.MainGroups)
            {
                where += String.Format(" AND {0}.FK_Parent_Group is null", tableAlias);
            }
            else if (showDataBy == ShowDataEnum.UserSelect || showDataBy == ShowDataEnum.UserSelectNoDefaultData)
            {

                if (dataId == Guid.Empty)
                {
                    //initiele waarde
                    //als vaste groep is gekozen dan deze nemen
                    if (selectGroupID != null && selectGroupID != "")
                    {
                        where += String.Format(" AND {0}.FK_Parent_Group = '{1}'", tableAlias, selectGroupID);
                    }
                    else
                    {
                        //alle uit root tonen
                        where += String.Format(" AND {0}.FK_Parent_Group is null", tableAlias);
                    }
                }
                else
                {
                    where += String.Format(" AND {0}.FK_Parent_Group = '{1}'", tableAlias, dataId);
                }
            }
            return where;
        }



        public string Reload(CmsPage page, Dictionary<string, object> Parameters)
        {
            if (Parameters != null && Parameters.ContainsKey("dataid"))
            {
                this.dataId = new Guid(Parameters["dataid"].ToString());
            }
            return Publish2(page);
        }
    }
}
