using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using System.Text.RegularExpressions;
using System.Web;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Utils;

namespace BitPlate.Domain.Modules.Data
{
    [Persistent("Module")]
    public class ItemListModule : BaseDataModule, IRefreshableModule, IPageableModule, ISortable
    {
        private string drillDownUrl = "", drillDownLink = "";
        private string drillUpUrl = "", drillUpLink = "";
        private string editUrl = "", editLink = "";

        private bool reloadedFromFilterModule = false;


        public ItemListModule()
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
            if (this.NavigationActions.Count == 1)
            {
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    Name = "{DrillUpLink}",
                    NavigationType = NavigationTypeEnum.NavigateToPage
                });
            }

            if (this.NavigationActions.Count == 2)
            {
                this.NavigationActions.Add(new ModuleNavigationAction()
                {
                    Name = "{EditLink}",
                    NavigationType = NavigationTypeEnum.NavigateToPage
                });
            }

            

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

        public override void SetAllTags()
        {
            //Tags waren niet zichtbaar in tag list.
            this._tags.Add(new Tag() { Name = "<!--{List}-->", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{List}", HasCloseTag = true });

            this._tags.Add(new Tag() { Name = "{DrillDownLink}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{DrillUpLink}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{DrillDownUrl}" });
            this._tags.Add(new Tag() { Name = "{EditLink}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{Pager}" });
            this._tags.Add(new Tag() { Name = "{NumberOfResults}" });
            this._tags.AddRange(this.GetDataFieldTags());
            this._tags.AddRange(this.GetSortLinksTags());
            this._tags = this._tags.OrderBy(c => c.Name).ToList();

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

        //protected override void setDrillDownActions()
        //{
        //    if (_drillDownActions == null)
        //    {
        //        _drillDownActions = new BaseCollection<ModuleDrillDownAction>();
        //    }
        //    ModuleDrillDownAction action1 = new ModuleDrillDownAction();
        //    action1.Name = "DrillDown";
        //    action1.Tag = new Tag("{DrillDownLink}", "", true, "");
        //    _drillDownActions.Add(action1);

        //    ModuleDrillDownAction action2 = new ModuleDrillDownAction();
        //    action2.Name = "Terug";
        //    action2.Tag = new Tag("{BackLink}", "", true, "");
        //    _drillDownActions.Add(action2);

        //}

        public override string Publish2(CmsPage page)
        {
            return Publish2(page, 0, 0, "");
        }

        public string Publish2(CmsPage page, int currentPage, int totalResults, string sort)
        {
            string html = "";
            try
            {
                if (DataCollection != null) base.PrepairTemplate(DataCollection.DataItemFields, DataCollection.DataGroupFields);

                html = base.Publish2(page);
                if (DataCollection == null) return html;

                //string drillDownUrl = "", drillDownLink = "";

                ModuleNavigationAction navigationActionDrillDown = this.GetNavigationActionByTagName("{DrillDownLink}");
                if (navigationActionDrillDown != null)
                {
                    drillDownUrl = navigationActionDrillDown.GetNavigationUrl();
                    drillDownLink = navigationActionDrillDown.CreateNavigationHyperlink("I");
                }

                ModuleNavigationAction navigationActionEdit = this.GetNavigationActionByTagName("{EditLink}");
                if (navigationActionEdit != null)
                {
                    editUrl = navigationActionEdit.GetNavigationUrl();
                    editLink = navigationActionEdit.CreateNavigationHyperlink("I");
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
                int pagesize = getSetting<int>("MaxNumberOfRows");
                int showFromRowNumber = getSetting<int>("ShowFromRowNumber");

                string rowTemplate = base.GetSubTemplate("{List}");
                string rowsHtml = "";
                System.Data.DataTable itemsTable = getDataTable("DataItem", "I", DataCollection.DataItemFields, currentPage, pagesize, showFromRowNumber, sort);

                foreach (System.Data.DataRow dataRow in itemsTable.Rows)
                {
                    string rowHtml = rowTemplate;
                    string rewriteUrl = CreateRewriteUrl(page, drillDownUrl, "I", new Guid(dataRow["ID"].ToString()), dataRow["CompletePath"].ToString(), dataRow["Title"].ToString());
                    rowHtml = rowHtml.Replace("{DrillDownLink}", drillDownLink)
                    .Replace("{/DrillDownLink}", "</a>")
                    .Replace("{DrillDownUrl}", rewriteUrl);
                    if ((WebSessionHelper.CurrentSiteUser != null && WebSessionHelper.CurrentSiteUser.ID.ToString() == dataRow["FK_User"].ToString()) ||
                    (dataRow["FK_User"].ToString() == "" && dataRow["FK_BitplateUser"].ToString() == "" && WebSessionHelper.CurrentBitplateUser != null) ||
                    (WebSessionHelper.CurrentBitplateUser != null && WebSessionHelper.CurrentBitplateUser.ID.ToString() == dataRow["FK_BitplateUser"].ToString())) {
                        rowHtml = rowHtml.Replace("{EditLink}", editLink)
                        .Replace("{/EditLink}", "</a>");
                    }
                    else 
                    {
                        rowHtml = Regex.Replace(rowHtml, "{EditLink}(.*?){/EditLink}", "");
                    }

                    foreach (System.Data.DataColumn column in dataRow.Table.Columns)
                    {

                        rowHtml = ReplaceTag(rowHtml, dataRow, column);

                        //rowHtml = rowHtml.Replace("{" + column.ColumnName + "}", dataRow[column.ColumnName].ToString());
                        if (rowHtml.Contains("{FileList1}"))
                        {
                            rowHtml = fillExtraFilesSubTemplate(rowHtml, dataRow["ID"].ToString(), "");
                        }
                        if (rowHtml.Contains("{ImageList1}"))
                        {
                            rowHtml = fillExtraImagesSubTemplate(rowHtml, dataRow["ID"].ToString(), "");
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
                if (pagesize > 0 && totalResults == 0)
                {
                    totalResults = getTotalCount("I");
                }
                else if (totalResults == 0)
                {
                    totalResults = itemsTable.Rows.Count;
                }
                html = html.Replace("{Pager}", CreatePager(currentPage, totalResults, pagesize));
                html = html.Replace("{NumberOfResults}", totalResults.ToString());

                foreach (Tag tag in this.GetSortLinksTags(currentPage, pagesize))
                {
                    html = html.Replace(tag.Name, tag.ReplaceValue)
                        .Replace(tag.CloseTag, tag.ReplaceValueCloseTag);
                }

                bool hideWhenNoData = getSetting<bool>("HideWhenNoData");
                if (hideWhenNoData && totalResults == 0)
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

        private int getTotalCount(string tableAlias)
        {
            string sqlTotalCount = "SELECT COUNT(*) FROM DataItem AS " + tableAlias + getWhere(tableAlias);
            object result = DataBase.Get().Execute(sqlTotalCount);
            return Convert.ToInt32(result);
        }



        protected override string getWhere(string tableAlias)
        {
            string where = base.getWhere(tableAlias);
            ShowDataEnum showDataBy = base.getSetting<ShowDataEnum>("ShowDataBy");
            string selectGroupID = getSetting<string>("SelectGroupID");
            if (reloadedFromFilterModule)
            {
                where += new FilterModule() { DataCollection = this.DataCollection }.BuildWhere(tableAlias, Parameters);
            }
            else
            {
                if (showDataBy == ShowDataEnum.AllItems)
                {
                    //als vaste groep is gekozen dan deze nemen
                    if (selectGroupID != null && selectGroupID != "")
                    {
                        where += String.Format(" AND {0}.FK_Parent_Group = '{1}'", tableAlias, selectGroupID);
                    }
                    else
                    {
                        //doe niks
                    }
                }
                else if (showDataBy == ShowDataEnum.UserSelect)
                {
                    if (dataType == "I")
                    {
                        //Wordt op item geselecteerd: haal de groep op van dit item
                        string sql = "SELECT FK_Parent_Group FROM DataItem WHERE ID = '" + dataId.ToString() + "'";
                        object result = DataBase.Get().Execute(sql);
                        Guid.TryParse(result.ToString(), out dataId);
                    }
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
            }
            return where;
        }

        protected string ReplaceDrillUpTag(CmsPage page, string html, string parentID)
        {
            if (parentID != "")
            {
                System.Data.DataRow parentParentGroupRow = base.getDataParentGroupRow(new Guid(parentID));

                //string rewriteUrlDrillUp = CreateRewriteUrl(drillUpUrl, "G", new Guid(parentID), parentParentGroupRow["CompletePath"].ToString(), parentParentGroupRow["Title"].ToString());
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

                html = Regex.Replace(html, "{DrillUpLink}(.*?){/DrillUpLink}", "");
            }
            return html;
        }

        public string CreatePager(int currentPage, int totalResults, int pageSize)
        {
            return base.createPager(currentPage, totalResults, pageSize);
        }

        public string Reload(CmsPage page, Dictionary<string, object> Parameters)
        {
            if (Parameters != null && Parameters.ContainsKey("dataid"))
            {
                this.dataId = new Guid(Parameters["dataid"].ToString());
                this.dataType = Parameters["datatype"].ToString();
            }
            else if (Parameters != null && Parameters.Keys.Count > 0)
            {
                //todo: beter afvangen wanneer hij vanaf filtermodule komt
                //parameters object aanmaken, laden in Page.aspx.cs en daar sourcemoduletype in stoppen samen met querystring 
                //filtermodule
                reloadedFromFilterModule = true;
                this.Parameters = Parameters;
            }

            return Publish2(page);
        }

        public string DoPaging(CmsPage page, int pageNumber, int totalResults, Dictionary<string, object> Parameters)
        {
            if (Parameters != null && Parameters.ContainsKey("dataid"))
            {
                this.dataId = new Guid(Parameters["dataid"].ToString());
                this.dataType = Parameters["datatype"].ToString();
            }
            else if (Parameters != null && Parameters.Keys.Count > 0)
            {
                //filtermodule
                reloadedFromFilterModule = true;
                this.Parameters = Parameters;
            }
            return Publish2(page, pageNumber, totalResults, "");
        }

        public string DoSort(CmsPage page, string column, SortDirectionEnum sortDirection, int pageNumber, int totalResults, Dictionary<string, object> Parameters)
        {
            if (Parameters != null && Parameters.ContainsKey("dataid"))
            {
                this.dataId = new Guid(Parameters["dataid"].ToString());
                this.dataType = Parameters["datatype"].ToString();
            }
            else if (Parameters != null && Parameters.Keys.Count > 0)
            {
                //filtermodule
                reloadedFromFilterModule = true;
                this.Parameters = Parameters;
            }
            return Publish2(page, pageNumber, totalResults, column + " " + sortDirection.ToString());
        }
    }
}
