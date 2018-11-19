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
    public partial class FilterModuleControl : BaseDataModuleUserControl
    {
        private DataCollection dataCollection;
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
            if (this.DataCollectionID != null && this.DataCollectionID != Guid.Empty)
            {
                dataCollection = BaseObject.GetById<DataCollection>(this.DataCollectionID);
            }
            
            if (!IsPostBack) this.Load();
            System.Diagnostics.Trace.WriteLine("Page_Load End", "TreeViewModuleControl");
        }

        protected void Load()
        {
            foreach (Control control in this.Controls)
            {
                if (control.ID != null)
                {
                    string filterColumnID = Regex.Match(control.ID, "_(.*?)_").ToString().Replace("_", "");
                    if (filterColumnID != "")
                    {
                        DataField filterColumn = GetDataFieldByID(filterColumnID);
                        if (filterColumn != null)
                        {
                            switch (control.GetType().Name)
                            {
                                case "CheckBoxList":
                                case "RadioButtonList":
                                case "DropDownList":
                                    this.fillSelectList((ListControl)control, filterColumn);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private DataField GetDataFieldByID(string ID)
        {
            if (this.dataCollection != null)
            {
                Guid DataFieldID = Guid.Empty;
                Guid.TryParse(ID, out DataFieldID);
                if (DataFieldID == Guid.Empty)
                {
                    return null;
                }
                return this.dataCollection.DataItemFields.Where(c => c.ID == DataFieldID).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        private void fillSelectList(ListControl control, DataField dataField)
        {
            if (this.DataCollectionID != null && this.DataCollectionID != Guid.Empty)
            {
                if (dataField != null)
                {
                    if (control.GetType().Name == "DropDownList")
                    {
                        ListItem li = new ListItem("***Alle***", Guid.Empty.ToString());
                        control.Items.Add(li);
                    }

                    foreach (DataLookupValue dlv in dataField.LookupValues)
                    {
                        ListItem li = new ListItem(dlv.Name, dlv.ID.ToString());
                        if (control.GetType().Name != "DropDownList") li.Text = " " + li.Text; //Is het geen dropdown? dan een spatie;)
                        control.Items.Add(li);
                    }
                }
            }
        }

        protected void ClearFilter_Event(Object sender, EventArgs args)
        {
            ModuleNavigationActionLite action = GetNavigationActionByTagName("{ApplyFilter}");
            if (action.NavigationType == NavigationTypeEnum.NavigateToPage)
            {
                Response.Redirect(action.NavigationUrl);
            }
            else if (action.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {
                foreach (string drillDownModuleId in action.RefreshModules)
                {
                    BaseModuleUserControl moduleControl = (BaseModuleUserControl)FindControlRecursive(this.Page.Master, "Mod" + drillDownModuleId.Replace("-", ""));
                    if (moduleControl != null)
                    {
                        moduleControl.Reload(this, new NavigationParameterObject() { Name = "dataid", GuidValue = Guid.Empty }); //{  //SelectAndShowData(dataId);
                    }
                }

                foreach (Control control in this.Controls)
                {
                    if (control.ID != null)
                    {
                        string filterColumnID = Regex.Match(control.ID, "_(.*?)_").ToString().Replace("_", "");
                        DataField dataField = null;
                        if (filterColumnID == "02f78e8cfc9c40ca9dd432eadc01f92a") //02f78e8c-fc9c-40ca-9dd4-32eadc01f92a = searchall ID
                        {
                            //SearchAllVelden. In deze velden wordt gezocht naar de invoer van de SearchAllTextbox
                            filterColumnID = "ID, Name, Title";
                        }
                        else
                        {
                            dataField = GetDataFieldByID(filterColumnID);
                        }

                        if (dataField != null)
                        {
                            ClearControlValue(control);
                        }

                        //Zoek in alle dataitem velden.
                        if (dataField == null && filterColumnID != "" && control != null && control.GetType().Name == "TextBox")
                        {
                            ClearControlValue(control);
                        }
                    }
                }
                //stuur eventueel javascript naar de browser zodat deze wordt uitgevoerd nadat pagina opnieuw is gerenderd
                /* string js = linkButton.OnClientClick;
                if (js != null && js != "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "navigation" + this.ModuleID.ToString("N"), js, true);
                } */
            }       
        }

        protected void AppyFilter_Event(Object sender, EventArgs args)
        {
            string where = "";
            string whereStatement;
            //string dropdownList = "", checkboxList = "";
            //string 
            foreach (Control control in this.GetAllControls(this.Page.Master))
            {
                whereStatement = "";
                if (control.ID != null)
                {
                    string filterColumnID = Regex.Match(control.ID, "_(.*?)_").ToString().Replace("_", "");
                    DataField dataField = null;
                    if (filterColumnID == "02f78e8cfc9c40ca9dd432eadc01f92a") //02f78e8c-fc9c-40ca-9dd4-32eadc01f92a = searchall ID
                    {
                        //SearchAllVelden. In deze velden wordt gezocht naar de invoer van de SearchAllTextbox
                        filterColumnID = "ID, Name, Title";
                    }
                    else
                    {
                        dataField = GetDataFieldByID(filterColumnID);
                    }

                    if (dataField != null)
                    {
                        whereStatement = GetFilterControlValue(control, dataField);
                        if (whereStatement != "") where += whereStatement;
                    }

                    //Zoek in alle dataitem velden.
                    if (dataField == null && filterColumnID != "" && control != null && control.GetType().Name == "TextBox")
                    {
                        TextBox textBox = (TextBox)control;
                        string concatmappingField = " AND CONCAT(Name, Title";
                        foreach (DataField dataFieldToConcat in dataCollection.DataItemFields)
                        {
                            if (dataFieldToConcat.FieldType == FieldTypeEnum.Text ||
                                dataFieldToConcat.FieldType == FieldTypeEnum.LongText ||
                                dataFieldToConcat.FieldType == FieldTypeEnum.Html)
                            {
                                if (dataFieldToConcat.MappingColumn != "")
                                {
                                    concatmappingField += ", " + dataFieldToConcat.MappingColumn;
                                }
                            }
                        }
                        concatmappingField += ")";
                        whereStatement += concatmappingField + " LIKE '%" + textBox.Text + "%'";
                        if (whereStatement != "" && textBox.Text.Trim() != "") where += whereStatement;
                    }
                }
            }

            //if (where != "") where = where.Remove(where.Length - 4);

            //Antieke code
            //if (dropdownList != "")
            //{
            //    dropdownList = dropdownList.Remove(dropdownList.Length - 2);
            //    dropdownList = " (FK_1 IN (" + dropdownList + ") OR " +
            //        " FK_2 IN (" + dropdownList + ") OR " +
            //        " FK_3 IN (" + dropdownList + ") OR " +
            //        " FK_4 IN (" + dropdownList + ") OR " +
            //        " FK_5 IN (" + dropdownList + ") OR " +
            //        " FK_6 IN (" + dropdownList + ") OR " +
            //        " FK_7 IN (" + dropdownList + ") OR " +
            //        " FK_8 IN (" + dropdownList + ") OR " +
            //        " FK_9 IN (" + dropdownList + ") OR " +
            //        " FK_10 IN (" + dropdownList + ")) ";
            //    where += " AND " + dropdownList;
            //}

            //if (checkboxList != "")
            //{
            //    checkboxList = checkboxList.Remove(checkboxList.Length - 2);
            //    checkboxList = " EXISTS (SELECT * FROM datalookupvalueperitem WHERE FK_LookupValue IN (" + checkboxList + ") AND dataitem.ID = datalookupvalueperitem.FK_Item) ";
            //    where += " AND " + checkboxList;
            //}
            ModuleNavigationActionLite action = GetNavigationActionByTagName("{ApplyFilter}");
            if (action.NavigationType == NavigationTypeEnum.NavigateToPage)
            {
                Response.Redirect(action.NavigationUrl);
            }
            else if (action.NavigationType == NavigationTypeEnum.ShowDetailsInModules)
            {

                foreach (string drillDownModuleId in action.RefreshModules)
                {
                    BaseModuleUserControl moduleControl = (BaseModuleUserControl)FindControlRecursive(this.Page.Master, "Mod" + drillDownModuleId.Replace("-", ""));
                    if (moduleControl != null)
                    {
                        moduleControl.Reload(this, new NavigationParameterObject() { Name = "filter", StringValue = where, GuidValue = Guid.Empty }); //{  //SelectAndShowData(dataId);
                    }
                }
                //stuur eventueel javascript naar de browser zodat deze wordt uitgevoerd nadat pagina opnieuw is gerenderd
                /* string js = linkButton.OnClientClick;
                if (js != null && js != "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "navigation" + this.ModuleID.ToString("N"), js, true);
                } */
            }
        }

        private void ClearControlValue(Control control)
        {
            //Zoek juiste Control type bij de control.
            switch (control.GetType().Name)
            {
                case "CheckBox":
                    //Is het een valide control voeg dan indien nodig de AND operator toe aan de filterColumn query.
                    ((CheckBox)control).Checked = false;
                    break;

                case "CheckBoxList":
                case "RadioButtonList":
                case "DropDownList":
                    ListControl listControl = ((ListControl)control);
                    foreach (ListItem li in listControl.Items)
                    {
                        li.Selected = false;
                    }
                    break;

                case "TextBox":
                    //Zoeken in specifiek veld.
                    TextBox textBox = (TextBox)control;
                        textBox.Text = "";
                    break;
            }
        }

        private string GetFilterControlValue(Control control, DataField dataField)
        {
            string whereStatement = "";
            //Zoek juiste Control type bij de control.
            switch (control.GetType().Name)
            {
                case "CheckBox":
                    //Is het een valide control voeg dan indien nodig de AND operator toe aan de filterColumn query.
                    whereStatement += " AND dataitem." + dataField.MappingColumn + " = " + ((CheckBox)control).Checked.ToString();
                    break;

                case "CheckBoxList":
                case "RadioButtonList":
                case "DropDownList":
                    ListControl listControl = ((ListControl)control);
                    string values = "";
                    foreach (ListItem li in listControl.Items)
                    {
                        if (li.Selected && li.Value != Guid.Empty.ToString())
                        {
                            //Zoek alle geselecteerde Items. Bij checkboxlist kunnen dit meerdere items zijn. Bij radiobuttonlist maar 1.
                            values +=  li.Value + ",";
                        }
                    }
                    if (values != "")
                    {
                        values = values.Substring(0, values.Length - 1);
                        if (dataField.FieldType == FieldTypeEnum.DropDown)
                        {
                            //LookupValue[X] heet in de database FK_[X]
                            whereStatement += " AND " + dataField.MappingColumn.Replace("LookupValue", "FK_") + " IN (" + values + ")";
                        }
                        else
                        {
                            foreach (string value in Regex.Split(values, ",")) {
                                whereStatement += " AND EXISTS (SELECT * FROM datalookupvalueperitem WHERE FK_LookupValue = '" + value + "' AND dataitem.ID = datalookupvalueperitem.FK_Item) ";
                            }
                            //filterColumn += " EXISTS (SELECT * FROM datalookupvalueperitem WHERE FK_LookupValue IN (" + values + ") AND dataitem.ID = datalookupvalueperitem.FK_Item) ";
                        }
                    }
                    break;

                case "TextBox":
                    //Zoeken in specifiek veld.
                    TextBox textBox = (TextBox)control;
                    if (textBox.Text.Trim() != "")
                    {
                        whereStatement += " AND dataitem." + dataField.MappingColumn + " LIKE '%" + textBox.Text + "%'";
                    }
                    break;
            }
            return whereStatement;
        }

        public override void Reload(BaseModuleUserControl sender, NavigationParameterObject args = null)
        {
            if (base.CheckAutorisation())
            {
                if (args != null && args.Name == "dataid")
                {
                    
                }
            }
        }

        private void UpdateSelectedNode(Guid dataid)
        {
        }
    }
}