<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/bitMasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="DataCollectionData2.aspx.cs" Inherits="BitSite._bitPlate.DataCollectionData2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="_themes/bitplate/css/bitEditor.css" />
    <link rel="stylesheet" type="text/css" href="_js/plugins/jsTree/themes/default/style1.css" />
    <%= BitBundler.ResolveBundleUrl("~/Plugins/jsTree") %>
    <script src="_js/_bitEditor/ToolbarButton.js" type="text/javascript"></script>
    <script src="_js/_bitEditor/ToolbarSeperator.js" type="text/javascript"></script>
    <script src="_js/_bitEditor/ToolbarDropDown.js" type="text/javascript"></script>
    <script src="_js/_bitEditor/Toolbar.js" type="text/javascript"></script>
    <script src="_js/_bitEditor/MenuItem.js" type="text/javascript"></script>
    <script src="_js/_bitEditor/Menu.js" type="text/javascript"></script>
    <script src="_js/_bitEditor/Statusbar.js" type="text/javascript"></script>
    <script src="_js/_bitEditor/Editor.js" type="text/javascript"></script>
    <script src="_js/_bitEditor/Selection.js" type="text/javascript"></script>
    <script src="_js/_bitEditor/Popup.js" type="text/javascript"></script>
    <script src="_js/_bitEditor/CommandManager.js" type="text/javascript"></script>
    <script src="_js/_bitEditor/BITEDITOR.js" type="text/javascript"></script>
        <link rel="stylesheet" type="text/css" href="_themes/bitplate/plugins/jbreadcrump/Styles/BreadCrumb.css" />
    <script src="_js/plugins/jbreadcrump/js/jquery.easing.1.3.js" type="text/javascript"></script>
    <script src="_js/plugins/jbreadcrump/js/jquery.jBreadCrumb.1.1.js" type="text/javascript"></script>
    <script type="text/javascript" src="_js/pages/popups/BITIMAGESPOPUP.js"></script>
    <script type="text/javascript" src="_js/pages/popups/BITFILESPOPUP.js"></script>
    <script type="text/javascript" src="_js/pages/BITDATACOLLECTIONS.js"></script>
    <script type="text/javascript" src="_js/pages/BITDATACOLLECTIONDATA.js"></script>
    <script type="text/javascript">
        //tijdelijke hack voor wysiwig editor
        var BITEDITPAGE = null;

        $(document).ready(function () {
            var datacollectionId = BITUTILS.getQueryString()["datacollectionid"];
            BITDATACOLLECTIONDATA.datacollectionId = datacollectionId;
            BITDATACOLLECTIONDATA.initialize();
            BITDATACOLLECTIONDATA.loadData('');

        });

    </script>
    <style>
        #treeView {
            width: 300px;
            border: 1px solid black;
        }

        #tableDiv {
            border: 1px solid black;
        }

        li {
            list-style: none;
            padding-left: 20px;
        }

            li.a {
                padding: 5px;
            }

        .groupIcon {
            background: url(_themes/bitplate/images/BPCMS_page-icons.png) no-repeat 0px -156px;
            width: 26px;
            height: 26px;
            display: inline-block;
            border-bottom: none !important;
            outline: none;
        }

        .itemIcon {
            background: url(_themes/bitplate/images/BPCMS_page-icons.png) no-repeat 0px -130px;
            width: 26px;
            height: 26px;
            display: inline-block;
            border-bottom: none !important;
            outline: none;
        }

        a.selected {
            background-color: #0094ff;
        }

        .highlight {
            min-height: 30px;
            background-color: #ff6a00;
            border: 1px;
        }

        ul {
            min-height: 10px;
        }
    </style>
    <link rel="stylesheet" type="text/css" href="_themes/bitplate/css/tree.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="DataCollections.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>

        <li id="liAddDataGroup" runat="server"><a id="aAddDataGroup" runat="server" href="javascript:BITDATACOLLECTIONDATA.newDataGroup();" class="bitNavBarButtonAddPage">
        </a>
            <div>Nieuwe groep...</div>
        </li>
        <li id="liAddDataItem" runat="server"><a id="aAddDataItem" runat="server" href="javascript:BITDATACOLLECTIONDATA.newDataItem();" class="bitNavBarButtonAddPage">
        </a>
            <div>Nieuw item...</div>
        </li>
        <li id="toolbarMenuItemShowTree"><a href="javascript:BITDATACOLLECTIONDATA.showTree();"
            class="bitConfigButton"></a>
            <div>Toon tree (nog doen)</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Gegevensverzameling
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <div id="treeView" style="display: none"></div>
    <br />
    Locatie: 
        <div class="breadCrumb module">
            <ul data-control-type="list" id="breadcrump">
                <li><a data-field="Path" data-title-field="Name" data-text-field="Name" href="javascript:BITDATACOLLECTIONDATA.loadData('', '[data-field]')">
                </a></li>
            </ul>
        </div>
    
    <div id="tableView">
        <table id="tableData" class="bitGrid" data-control-type="table" cellspacing="0"
            cellpadding="2" style="
            margin-bottom:0px">
            <thead>
                <tr>
                    <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                    <td class="bitTableColumn" style="width: 70px" data-sort-field="OrderingNumber">VolgNr</td>
                    <td class="bitTableColumn" style="width: 240px" data-sort-field="Name">Naam</td>
                    <td class="bitTitleColumn1" style="width: 180px; display:none" data-sort-field="Title">Titel</td>
                    <td class="bitGroupColumn" style="width: 180px; display:none" data-sort-field="">Groep</td>
                    <td class="bitStatusColumn" style="width: 100px" data-sort-field="DateTill">Status</td>
                    <td class="bitTableActionButton">Config</td>
                    <td class="bitTableActionButton">Verwijder</td>
                    <td class="bitTableActionButton">Kopieer</td>
                    <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum</td>
                </tr>
            </thead>
            <tbody id="tbodyGroups">
                <tr data-id-field="ID" class="jstree-draggable" data-title-field="Type" title="[data-title-field]">
                    <td class="icon"><a href="#" class="bitTableFolderIcon"></a></td>
                    <td data-field="Field1"></td>
                    <td class="nameColumn" ><a data-field="ID" data-title-field="Path" data-text-field="Name" href="javascript:BITDATACOLLECTIONDATA.loadData('[data-field]', '[data-title-field]', undefined, '');"></a></td>
                    <td class="bitGroupTitleColumn" data-field="Title"></td>
                    <td class="bitGroupGroupColumn" data-field="Field2"></td>
                    <td class="bitStatusColumn" data-field="Status"></td>
                    <td class="bitTableActionButton" id="tdDataCollectionDataGroupConfig" runat="server"><a id="aDataCollectionDataGroupConfig" runat="server" data-field="ID" data-title-field="Type" href="javascript:BITDATACOLLECTIONDATA.openDetailsPopup('[data-field]', '[data-title-field]');"
                        class="bitConfigButton"></a></td>
                    <td class="bitTableActionButton" id="tdDataCollectionDataGroupRemove" runat="server"><a id="aDataCollectionDataGroupRemove" runat="server" data-field="ID" href="javascript:BITDATACOLLECTIONDATA.remove('[data-field]', 'Group');"
                        class="bitDeleteButton"></a></td>
                    <td class="bitTableActionButton" id="tdDataCollectionDataGroupCopy" runat="server"><a id="aDataCollectionDataGroupCopy" runat="server" data-field="ID" data-text-field="Name"
                        href="javascript:BITDATACOLLECTIONDATA.copy('[data-field]', 'Group', '[data-text-field]');"
                        class="bitCopyButton"></a></td>
                    <td class="bitTableDateColumn" data-field="CreateDate"></td>
                </tr>
            </tbody>
            <tbody id="tbodyItems">
                <tr data-id-field="ID" class="jstree-draggable" data-title-field="Type" title="[data-title-field]">
                    <td class="icon" style="width: 40px"><a href="#" class="bitTablePageIcon"></a></td>
                    <td data-field="Field1" style="width: 70px"></td>
                    <td class="nameColumn" data-field="Name" style="width: 240px"></td>
                    <td class="bitItemTitleColumn" data-field="Title" style="width: 180px"></td>
                    <td class="bitItemGroupColumn" data-field="Field2"></td>
                    <td class="bitStatusColumn" data-field="Status" style="width: 100px"></td>
                    <td class="bitTableActionButton" id="tdDataCollectionDataItemConfig" runat="server"><a id="aDataCollectionDataItemConfig" runat="server" data-field="ID" data-title-field="Type" href="javascript:BITDATACOLLECTIONDATA.openDetailsPopup('[data-field]', '[data-title-field]');"
                        class="bitConfigButton"></a></td>
                    <td class="bitTableActionButton" id="tdDataCollectionDataItemRemove" runat="server"><a id="aDataCollectionDataItemRemove" runat="server" data-field="ID" href="javascript:BITDATACOLLECTIONDATA.remove('[data-field]', 'Item');"
                        class="bitDeleteButton"></a></td>
                    <td class="bitTableActionButton" id="tdDataCollectionDataItemCopy" runat="server"><a id="aDataCollectionDataItemCopy" runat="server" data-field="ID" data-text-field="Name"
                        href="javascript:BITDATACOLLECTIONDATA.copy('[data-field]', 'Item', '[data-text-field]');"
                        class="bitCopyButton"></a></td>
                    <td class="bitTableDateColumn" data-field="CreateDate"></td>
                </tr>
            </tbody>
            <tfoot>
                <tr>
                    <td colspan="8">
                        <div class="bitEndTable"></div>
                    </td>
                </tr>
                <tr>
                    <td colspan="8" id="paging"></td>
                </tr>
            </tfoot>
        </table>
        <table id="tableData2" class="bitGrid" data-control-type="table" cellspacing="0"
            cellpadding="2" style="
            margin-top:0px">
            
        </table>
    </div>
    <br clear="all" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <!--IMAGES DIALOG -->
    <div id="bitImagesDialog" title="Afbeeldingen"></div>

    <!--FILES DIALOG -->
    <div id="bitFilesDialog" title="Bestanden"></div>

    <!--DATACOLLECTION CONFIG DIALOG -->
    <div id="bitDataCollectionConfigDialog" title="Dataverzameling eigenschappen"></div>

    <!--DATAGROUPS DIALOG -->
    <div id="bitDataGroupDetailsDialog" title="Data groep"
        style="display: none">

        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageGeneralGroups">Algemene gegevens</a></li>
                <asp:Literal ID="LiteralGroupTabPages" runat="server"></asp:Literal>
            </ul>
            <!-- TAB1: Home page -->
            <div id="tabPageGeneralGroups" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Naam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voer hier de naam in. Naam is niet taalgevoelig."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="text2" type="text" data-field="Name" data-validation="required" class="required" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Titel</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voer hier de titel in. De titel is belangrijk voor vindbaarheid in zoekmachines, verwerk hierin dus zoektermen. Indien er meerdere talen zijn is dit veld taalgevoelig."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="text3" type="text" data-field="Title" data-validation="required" class="required" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Valt onder</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Onder welke groep valt deze (sub)groep? Wijzig dit veld om de groep naar een andere (hoofd)groep te verhuizen."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectParentGroup_Groups" data-field="ParentGroup.ID" data-text-field="ParentGroup.CompletePath">
                    </select>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Volgnummer</div>
                <div class="bitPageSettingsCollumnB">
                    
                </div>
                <div class="bitPageSettingsCollumnC">
                    <span data-field="OrderNumber" ></span>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Status</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Nieuwe items en wijzigingen zijn pas online na publiceren. Wijzigingen in actief/inactief worden wel meteen doogevoerd. 2DO: hier duidelijkheid over krijgen.">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <span data-field="ChangeStatusString"></span>
                    <br />
                    <br />
                    <div>
                        <span class="radio1" title="status">
                            <input checked="checked" type="radio" id="radioStatus1" name="status" data-field="Active" value="1" /><label for="radioStatus1">Actief
                            </label>
                        </span>
                    </div>
                    <div>
                        <span class="radio1" title="status">
                            <input type="radio" id="radioStatus2" name="status" data-field="Active" value="2" /><label for="radioStatus2">Actief vanaf:
                            </label>
                            <input type="date" data-field="DateFrom" class="bitDatepicker" />
                            Tot:
                        <input type="date" data-field="DateTill" class="bitDatepicker" />
                        </span>
                    </div>
                    <div>
                        <span class="radio1" title="status">
                            <input type="radio" id="radioStatus3" name="status" data-field="Active" value="0" /><label for="radioStatus3">Niet actief
                            </label>
                        </span>
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA"></div>
                <div class="bitPageSettingsCollumnB">
                    
                </div>
                <div class="bitPageSettingsCollumnC">
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
            </div>
            <asp:Literal ID="LiteralGroupFields" runat="server"></asp:Literal>
        </div>
    </div>

    <!--DATAITEMS DIALOG -->
    <div id="bitDataItemDetailsDialog" title="Dataitem"
        style="display: none">

        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageGeneralItems">Algemene gegevens</a></li>
                <asp:Literal ID="LiteralItemTabPages" runat="server"></asp:Literal>
            </ul>
            <!-- TAB1: Home page -->
            <div id="tabPageGeneralItems" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Naam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voer hier de naam in."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="textNaam" type="text" data-field="Name" data-validation="required" class="required" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Titel</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voer hier de titel in. De titel is belangrijk voor vindbaarheid in zoekmachines, verwerk hierin dus zoektermen. Indien er meerdere talen zijn is dit veld taalgevoelig."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="text1" type="text" data-field="Title" data-validation="required" class="required" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Valt onder</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Onder welke groep valt dit item? Wijzig dit veld om het item naar een andere (hoofd)groep te verhuizen."></span>
                
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectParentGroup_Items" data-field="ParentGroup.ID" data-text-field="ParentGroup.CompletePath">
                    </select>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Volgnummer</div>
                <div class="bitPageSettingsCollumnB">
                    
                </div>
                <div class="bitPageSettingsCollumnC">
                    
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Status</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Nieuwe items en wijzigingen zijn pas online na publiceren. Wijzigingen in actief/inactief worden wel meteen doogevoerd. 2DO: hier duidelijkheid over krijgen.">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <span data-field="ChangeStatusString"></span>
                    <br />
                    <br />
                    <div>
                        <span class="radio1" title="status">
                            <input checked="checked" type="radio" id="radio1" name="status" data-field="Active" value="1" /><label for="radioStatus1">Actief
                            </label>
                        </span>
                    </div>
                    <div>
                        <span class="radio1" title="status">
                            <input type="radio" id="radio2" name="status" data-field="Active" value="2" /><label for="radioStatus2">Actief vanaf:
                            </label>
                            <input type="date" data-field="DateFrom" class="bitDatepicker" />
                            Tot:
                        <input type="date" data-field="DateTill" class="bitDatepicker" />
                        </span>
                    </div>
                    <div>
                        <span class="radio1" title="status">
                            <input type="radio" id="radio3" name="status" data-field="Active" value="0" /><label for="radioStatus3">Niet actief
                            </label>
                        </span>
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA"></div>
                <div class="bitPageSettingsCollumnB">
                    
                </div>
                <div class="bitPageSettingsCollumnC">
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
            </div>
            <asp:Literal ID="LiteralItemFields" runat="server"></asp:Literal>
        </div>
    </div>
</asp:Content>
