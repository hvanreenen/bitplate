<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="DataCollectionData.aspx.cs" Inherits="BitSite._bitPlate.DataCollections.DataCollectionData"
    EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">

    <link rel="stylesheet" type="text/css" href="/_bitplate/_themes/bitplate/plugins/jbreadcrump/Styles/BreadCrumb.css" />
    <link href="../_themes/bitplate/plugins/elFinder-2.0/css/elfinder.min.css" rel="stylesheet" type="text/css" />
    <link href="../_themes/bitplate/plugins/elFinder-2.0/css/theme.css" rel="stylesheet" type="text/css" />
    <!--link rel="stylesheet" type="text/css" href="/_bitplate/_themes/bitplate/css/tree.css" /-->
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/Plugins/jBreadCrumb") %>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/Plugins/ckeditor") %>
    <script src="../_js/plugins/elFinder-2.0/elfinder.min.js" type="text/javascript"></script>
    <script src="../_js/plugins/elFinder-2.0/i18n/elfinder.nl.js" type="text/javascript"></script>

    <script type="text/javascript" src="/_bitplate/Dialogs/BITIMAGESPOPUP.js"></script>
    <script type="text/javascript" src="/_bitplate/Dialogs/BITFILESPOPUP.js"></script>
    <script type="text/javascript" src="BITDATACOLLECTIONS.js"></script>
    <script type="text/javascript" src="BITDATACOLLECTIONDATA.js"></script>
    <script type="text/javascript">
        //tijdelijke hack voor wysiwig editor
        var BITEDITPAGE = null;

        $(document).ready(function () {
            CKEDITOR.basePath = '/_bitplate/_js/plugins/ckeditor/'; // SET ckeditor basepath.
            var datacollectionId = BITUTILS.getQueryString()["datacollectionid"];
            BITDATACOLLECTIONDATA.datacollectionId = datacollectionId;
            BITDATACOLLECTIONDATA.initialize();
            BITDATACOLLECTIONDATA.loadData('');
            
        });
    </script>
    
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="DataCollections.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>

        <li id="liAddDataGroup" runat="server"><a id="aAddDataGroup" runat="server" href="javascript:BITDATACOLLECTIONDATA.newDataGroup();" class="bitNavBarButtonAddPage"></a>
            <div>Nieuwe groep...</div>
        </li>
        <li id="liAddDataItem" runat="server"><a id="aAddDataItem" runat="server" href="javascript:BITDATACOLLECTIONDATA.newDataItem();" class="bitNavBarButtonAddPage"></a>
            <div>Nieuw item...</div>
        </li>
        <!--<li id="toolbarMenuItemShowTree"><a href="javascript:BITDATACOLLECTIONDATA.showTree();"
            class="bitNavBarButtonShowTree"></a>
            <div>Toon tree</div>
        </li>-->
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Datacollecties
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <div id="treeView" style="display: none"></div>
    <br />
    Locatie: 
        <div class="breadCrumb module">
            <ul data-control-type="list" id="breadcrump">
                <li><a data-field="Path" data-title-field="Name" href="javascript:BITDATACOLLECTIONDATA.loadData('', '[data-field]')"><span data-field="Name"></span></a></li>
            </ul>
        </div>

    <div id="tableView">
        <table id="tableData" class="bitGrid" data-control-type="table" cellspacing="0"
            cellpadding="2" style="margin-bottom: 0px">
            <thead>
                <tr>
                    <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                    <td class="bitTableColumn" style="width: 70px" data-sort-field="OrderingNumber">VolgNr</td>
                    <td class="bitTableColumn bitTableColumnEllipsis" style="width: 240px" data-sort-field="Name">Naam</td>
                    <td class="bitTitleColumn bitTableColumnEllipsis" style="width: 180px; display: none" data-sort-field="Title">Titel</td>
                    <td class="bitGroupColumn bitTableColumnEllipsis" style="width: 180px; display: none" data-sort-field="">Groep</td>
                    <!--<td class="bitStatusColumn" style="width: 100px" data-sort-field="DateTill">Status</td>-->
                    <td class="bitTableActionButton" id="htdDataCollectionDataEdit" runat="server">Bewerk</td>
                    <td class="bitTableActionButton" id="htdDataCollectionDataRemove" runat="server">Verwijder</td>
                    <td class="bitTableActionButton" id="htdDataCollectionDataCopy" runat="server">Kopieer</td>
                    <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum</td>
                </tr>
            </thead>
            <tbody id="tbodyGroups">
                <tr data-id-field="ID" class="jstree-draggable" data-title-field="Type" title="[data-title-field]">
                    <td class="icon"><a href="#" class="bitTableFolderIcon"></a></td>
                    <td data-field="Field1"></td>
                    <td class="nameColumn bitTableColumnEllipsis" style="width: 240px"><a data-field="ID" data-title-field="Path" data-text-field="Name" href="javascript:BITDATACOLLECTIONDATA.loadData('[data-field]', '[data-title-field]', undefined, '');"></a></td>
                    <td class="bitItemTitleColumn bitTableColumnEllipsis" data-field="Title" style="width: 180px" data-title-field="Title"></td>
                    <td class="bitGroupGroupColumn" data-field="Field2"></td>
                    <!--<td class="bitStatusColumn" data-field="Status"></td>-->
                    <td class="bitTableActionButton" id="tdDataCollectionDataGroupEdit" runat="server"><a id="aDataCollectionDataGroupConfig" runat="server" data-field="ID" data-title-field="Type" href="javascript:BITDATACOLLECTIONDATA.openDetailsPopup('[data-field]', '[data-title-field]');"
                        class="bitConfigButton"></a></td>
                    <td class="bitTableActionButton" id="tdDataCollectionDataGroupRemove" runat="server"><a id="aDataCollectionDataGroupRemove" runat="server" data-field="ID" href="javascript:BITDATACOLLECTIONDATA.remove('[data-field]', 'Group');"
                        class="bitDeleteButton"></a></td>
                    <td class="bitTableActionButton" id="tdDataCollectionDataGroupCopy" runat="server"><a id="aDataCollectionDataGroupCopy" runat="server" data-field="ID" data-text-field="Name"
                        href="javascript:BITDATACOLLECTIONDATA.copy('[data-field]', 'Group');"
                        class="bitCopyButton"></a></td>
                    <td class="bitTableDateColumn" data-field="CreateDate"></td>
                </tr>
            </tbody>
            <tbody id="tbodyItems">
                <tr data-id-field="ID" class="jstree-draggable" data-title-field="Type" title="[data-title-field]">
                    <td class="icon" style="width: 40px"><a href="#" class="bitTablePageIcon"></a></td>
                    <td data-field="Field1" style="width: 70px"></td>
                    <td class="nameColumn bitTableColumnEllipsis" data-title-field="Name" data-field="Name" style="width: 240px"></td>
                    <td class="bitItemTitleColumn bitTableColumnEllipsis" data-field="Title" style="width: 180px" data-title-field="Title"></td>
                    <td class="bitItemGroupColumn" data-field="Field2"></td>
                    <!--<td class="bitStatusColumn" data-field="Status" style="width: 100px"></td>-->
                    <td class="bitTableActionButton" id="tdDataCollectionDataItemEdit" runat="server"><a id="aDataCollectionDataItemConfig" runat="server" data-field="ID" data-title-field="Type" href="javascript:BITDATACOLLECTIONDATA.openDetailsPopup('[data-field]', '[data-title-field]');"
                        class="bitConfigButton"></a></td>
                    <td class="bitTableActionButton" id="tdDataCollectionDataItemRemove" runat="server"><a id="aDataCollectionDataItemRemove" runat="server" data-field="ID" href="javascript:BITDATACOLLECTIONDATA.remove('[data-field]', 'Item');"
                        class="bitDeleteButton"></a></td>
                    <!--<td class="bitTableActionButton" id="tdDataCollectionDataItemCopybackup" runat="server"><a id="aDataCollectionDataItemCopybackup" runat="server" data-field="ID" data-text-field="Name"
                        href="javascript:BITDATACOLLECTIONDATA.copy('[data-field]', 'Item', '[data-text-field]');"
                        class="bitCopyButton"></a></td>-->
                    <td class="bitTableActionButton" id="tdDataCollectionDataItemCopy" runat="server"><a id="aDataCollectionDataItemCopy" runat="server" data-field="ID" data-text-field="Name"
                        href="javascript:BITDATACOLLECTIONDATA.copy('[data-field]', 'Item');"
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
            cellpadding="2" style="margin-top: 0px">
        </table>
    </div>
    <br clear="all" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <!--IMAGES DIALOG -->
    <div id="bitImagesDialog" title="Afbeeldingen" style="max-height:600px; overflow-y:scroll; overflow-x:hidden"></div>

    <!--FILES DIALOG -->
    <div id="bitFilesDialog" title="Bestanden" style="max-height:600px; overflow-y:scroll; overflow-x:hidden"></div>
    <div id="bitFileSelectDialog" title="Selecteer een bestand" style="display: none;">
        <!-- Element where elFinder will be created (REQUIRED) -->
        <div id="elfinder"></div>
        <script type="text/javascript" charset="utf-8">
            // Helper function to get parameters from the query string.
            function getUrlParam(paramName) {
                var reParam = new RegExp('(?:[\?&]|&amp;)' + paramName + '=([^&]+)', 'i');
                var match = window.location.search.match(reParam);

                return (match && match.length > 1) ? match[1] : '';
            }

            $(document).ready(function () {
                var funcNum = getUrlParam('CKEditorFuncNum');
                var myCommands = elFinder.prototype._options.commands;
                var disabled = ['extract', 'archive', 'help', 'select'];
                $.each(disabled, function (i, cmd) {
                    (idx = $.inArray(cmd, myCommands)) !== -1 && myCommands.splice(idx, 1);
                });
                var selectedFile = null;
                var options = {
                    url: '/elfinder.connector',
                    commands: myCommands,
                    height: 500,
                    lang: 'nl',
                    uiOptions: {
                        toolbar: [
                            ['back', 'forward'],
                            ['reload'],
                            ['home', 'up'],
                            ['mkdir', 'mkfile', 'upload'],
                            ['open', 'download'],
                            ['info'],
                            ['quicklook'],
                            ['copy', 'cut', 'paste'],
                            ['rm'],
                            ['duplicate', 'rename', 'edit', 'resize'],
                            ['view', 'sort']
                        ]
                    },/* ,
                handlers: {
                    select: function (event, elfinderInstance) {

                        if (event.data.selected.length == 1) {
                            var item = $('#' + event.data.selected[0]);
                            if (!item.hasClass('directory')) {
                                selectedFile = event.data.selected[0];
                                $('#elfinder-selectFile').show();
                                return;
                            }
                        }
                        $('#elfinder-selectFile').hide();
                        selectedFile = null;
                    }
                } */
                    getFileCallback: function (file) {
                        
                    var startPoint = file.url.toLowerCase().indexOf('/_files');
                    var url = file.url.substr(startPoint);
                    if (BITDATACOLLECTIONDATA.selectFileType == 'image') {
                        BITDATACOLLECTIONDATA.setImageUrl(url);
                    }
                    if (BITDATACOLLECTIONDATA.selectFileType == 'file') {
                        BITDATACOLLECTIONDATA.setFileUrl(url);
                    }
                    
                        //window.opener.CKEDITOR.tools.callFunction(funcNum, url);
                    $('#bitFileSelectDialog').dialog('close');
                    },
                    resizable: false
                };

                var elf = $('#elfinder').elfinder(options).elfinder('instance');
            });
        </script>
    </div>

    <!--DATACOLLECTION CONFIG DIALOG -->
    <div id="bitDataCollectionConfigDialog" title="Dataverzameling eigenschappen"></div>

    <!--DATAGROUPS DIALOG -->
    <div id="bitDataGroupDetailsDialog" title="Data groep"
        style="display: none">
        <div id="divLanguage_Groups" runat="server" visible="false">
            Kies taal:   <select id="dropdownLanguages_Groups" runat="server" onchange="javascript:BITDATACOLLECTIONDATA.changeLanguage('Groups');"></select>
            <br clear="all" />

        </div>
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
                    <asp:Literal ID="LiteralGroupTitle" runat="server"></asp:Literal>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Valt onder</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Onder welke groep valt deze (sub)groep? Wijzig dit veld om de groep naar een andere (hoofd)groep te verhuizen."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectParentGroup_Groups" data-field="ParentGroup.ID" data-text-field="ParentGroup.Name">
                    </select>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Volgnummer</div>
                <div class="bitPageSettingsCollumnB">
                </div>
                <div class="bitPageSettingsCollumnC">
                    <span data-field="OrderNumber"></span>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Meta-Description</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Geef een korte beschrijving voor vindbaarheid in Google. Bij gebruik van de groupdetails-module op de pagina, wordt de meta-omschrijving van de pagina overschreven door deze waarde. "></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <asp:Literal ID="LiteralGroupMetaDescription" runat="server"></asp:Literal>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Meta-Keywords</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Geef een aantal komma gescheiden zoektermen op voor vindbaarheid in Google. Bij gebruik van de groupdetails-module op de pagina, wordt de meta-keywords van de pagina overschreven door deze waarde. "></span>
                
                </div>
                <div class="bitPageSettingsCollumnC">
                    <asp:Literal ID="LiteralGroupMetaKeywords" runat="server"></asp:Literal>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Status</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Nieuwe items en wijzigingen zijn pas online na publiceren. Wijzigingen in actief/inactief worden wel meteen doogevoerd. 2DO: hier duidelijkheid over krijgen.">info</span>
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
        <div id="divLanguage_Items" runat="server" visible="false">
            Kies taal: <select id="dropdownLanguages_Items" runat="server" onchange="javascript:BITDATACOLLECTIONDATA.changeLanguage('Items');"></select>

            <br clear="all" />

        </div>
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
                    <asp:Literal ID="LiteralItemTitle" runat="server"></asp:Literal>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Valt onder</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Onder welke groep valt dit item? Wijzig dit veld om het item naar een andere (hoofd)groep te verhuizen."></span>

                </div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectParentGroup_Items" data-field="ParentGroup.ID" data-text-field="ParentGroup.Name">
                    </select>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Volgnummer</div>
                <div class="bitPageSettingsCollumnB">
                </div>
                <div class="bitPageSettingsCollumnC">
                    <span data-field="OrderNumber"></span>
                </div>
                <br clear="all" />
                 <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Meta-Description</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Geef een korte beschrijving voor vindbaarheid in Google. Bij gebruik van de itemdetails-module op de pagina, wordt de meta-omschrijving van de pagina overschreven door deze waarde. "></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <asp:Literal ID="LiteralItemMetaDescription" runat="server"></asp:Literal>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Meta-Keywords</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Geef een aantal komma gescheiden zoektermen op voor vindbaarheid in Google. Bij gebruik van de itemdetails-module op de pagina, wordt de meta-keywords van de pagina overschreven door deze waarde. "></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <asp:Literal ID="LiteralItemMetaKeywords" runat="server"></asp:Literal>
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Status</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Nieuwe items en wijzigingen zijn pas online na publiceren. Wijzigingen in actief/inactief worden wel meteen doogevoerd. 2DO: hier duidelijkheid over krijgen.">info</span>
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
