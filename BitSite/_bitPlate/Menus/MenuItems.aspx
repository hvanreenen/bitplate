<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="MenuItems.aspx.cs" Inherits="BitSite._bitPlate.Menus.MenuItems"
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
    <script type="text/javascript" src="BITMENUS.js"></script>

    <script type="text/javascript">


        $(document).ready(function () {
            var menuId = BITUTILS.getQueryString()["menuId"];
            var menuType = BITUTILS.getQueryString()["menuType"];
            BITMENUS.menuId = menuId;
            BITMENUS.menuType = menuType;
            BITMENUS.initialize();
            BITMENUS.loadMenuItems('');
            BITMENUS.fillDropDownParents();
        });
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="Menus.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>

        <li id="liAddDataItem" runat="server"><a id="aAddDataItem" runat="server" href="javascript:BITMENUS.newMenuItem();" class="bitNavBarButtonAddPage"></a>
            <div>Nieuw Menuitem...</div>
        </li>

    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Menuitems
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <div id="treeView" style="display: none"></div>
    <br />
    Locatie: 
        <div class="breadCrumb module">
            <ul data-control-type="list" id="breadcrump">
                <li><a data-field="Path" data-title-field="Name" href="javascript:BITMENUS.loadMenuItems('', '[data-field]')"><span data-field="Name"></span></a></li>
            </ul>
        </div>

    <div id="tableView">
        <table id="tableMenuItems" class="bitGrid" data-control-type="table" cellspacing="0"
            cellpadding="2" style="margin-bottom: 0px">
            <thead>
                <tr>
                    <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                    <td class="bitTableColumn" style="width: 70px">VolgNr</td>
                    <td class="bitTableColumn bitTableColumnEllipsis" style="width: 240px">Naam</td>
                    <!--<td class="bitStatusColumn" style="width: 100px" data-sort-field="DateTill">Status</td>-->
                    <td class="bitTableActionButton" id="htdMenuItemEdit" runat="server">Bewerk</td>
                    <td class="bitTableActionButton" id="htdMenuItemRemove" runat="server">Verwijder</td>
                    <td class="bitTableActionButton" id="htdMenuItemCopy" runat="server">Kopieer</td>
                    <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum</td>
                </tr>
            </thead>

            <tbody id="tbodyItems">
                <tr data-id-field="ID" class="jstree-draggable" data-title-field="Type" title="[data-title-field]">
                    <td class="icon" style="width: 40px"><a href="#" class="bitTablePageIcon"></a></td>
                    <td data-field="OrderingNumber" style="width: 70px"></td>
                    <td class="nameColumn bitTableColumnEllipsis" style="width: 240px"><a class="drillDown" data-field="ID" data-title-field="CompletePath" data-text-field="Name" href="javascript:BITMENUS.loadMenuItems('[data-field]', '[data-title-field]', undefined, '');"></a></td>

                    <td class="bitTableActionButton" id="tdMenuItemItemEdit" runat="server"><a id="aMenuItemItemConfig" runat="server" data-field="ID" data-title-field="Type" href="javascript:BITMENUS.openMenuItemPopup('[data-field]', '[data-title-field]');"
                        class="bitConfigButton"></a></td>
                    <td class="bitTableActionButton" id="tdMenuItemItemRemove" runat="server"><a id="aMenuItemItemRemove" runat="server" data-field="ID" href="javascript:BITMENUS.removeMenuItem('[data-field]', 'Item');"
                        class="bitDeleteButton"></a></td>
                    <!--<td class="bitTableActionButton" id="tdMenuItemItemCopybackup" runat="server"><a id="aMenuItemItemCopybackup" runat="server" data-field="ID" data-text-field="Name"
                        href="javascript:BITDATACOLLECTIONDATA.copy('[data-field]', 'Item', '[data-text-field]');"
                        class="bitCopyButton"></a></td>-->
                    <td class="bitTableActionButton" id="tdMenuItemItemCopy" runat="server"><a id="aMenuItemItemCopy" runat="server" data-field="ID" data-text-field="Name"
                        href="javascript:BITMENUS.copyMenuItem('[data-field]', 'Item');"
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

    </div>
    <br clear="all" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <!--IMAGES DIALOG -->
    <div id="bitImagesDialog" title="Afbeeldingen" style="max-height: 600px; overflow-y: scroll; overflow-x: hidden"></div>


    <!--MENU ITEMS DIALOG -->
    <div id="bitMenuItemDetailsDialog" title="Menuitem"
        style="display: none">

        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageMenuItemsGeneral">Algemene gegevens</a></li>
                <li><a href="#tabPageMenuItemsImages">Afbeeldingen</a></li>
            </ul>
            <!-- TAB1: Algemeen -->
            <div id="tabPageMenuItemsGeneral" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Soort</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Soort menu item"></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <label>
                        <input id="radioUrlType0" type="radio" data-field="UrlType" name="urlType" value="0" onclick="javascript: BITMENUS.changeMenuItemUrlType();" />
                        Verwijst naar pagina binnen deze site</label><br />

                    <label>
                        <input id="radioUrlType1" type="radio" data-field="UrlType" name="urlType" value="1" onclick="javascript: BITMENUS.changeMenuItemUrlType();" />
                        Url buiten deze site</label>
                    <label>
                        <br />
                        <input id="radioUrlType2" type="radio" data-field="UrlType" name="urlType" value="2" onclick="javascript: BITMENUS.changeMenuItemUrlType();" />
                        Alleen kopje (geen url)</label><br />
                </div>
                <br clear="all" />
                <div id="divPageLink">
                    <!-- NEXT ROW -->
                    <div class="bitPageSettingsCollumnA">Pagina</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Voer hier de pagina/folder in."></span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="selectPages" runat="server" data-field="Page.ID" data-text-field="Page.RelativeUrl" onchange="javascript:BITMENUS.changePage();">
                        </select>
                    </div>
                    <br clear="all" />
                </div>
                <div id="divExternalLink" style="display: none">
                    <!-- NEXT ROW -->
                    <div class="bitPageSettingsCollumnA">Url</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Voer hier de externe url in."></span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input id="textExternalUrl" type="text" data-field="ExternalUrl" data-validation="required" class="required" />
                    </div>
                    <br clear="all" />
                </div>
                <div id="divTarget">
                    <!-- NEXT ROW -->
                    <div class="bitPageSettingsCollumnA">Doel van hyperlink (target)</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title=""></span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select data-field="Target">
                            <option value="">blijf binnen hetzelfde browser-venster (target='_self')</option>
                            <option value="_blank">open nieuw browser-venster/tabblad (target='_blank')</option>
                        </select>
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Weergave naam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voer hier de naam van het menuitem in."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="textName" type="text" data-field="Name" data-validation="required" class="required" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Titel</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voer hier de titel in. De titel is belangrijk voor vindbaarheid in zoekmachines, verwerk hierin dus zoektermen. Indien er meerdere talen zijn is dit veld taalgevoelig."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="text3" type="text" data-field="Title" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">CSS Class</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Geef optioneel een css class naam. De naam moet in de html worden gezet als MenuItem.CssClass"></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="text4" type="text" data-field="CssClass" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Valt onder</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Onder welk menuitem valt deze? Wijzig dit veld om naar een andere menuitem te verhuizen."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectParentMenuItems" data-field="ParentMenuItem.ID" data-text-field="ParentMenuItem.Name">
                    </select>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Volgnummer</div>
                <div class="bitPageSettingsCollumnB">
                </div>
                <div class="bitPageSettingsCollumnC">
                    <span data-field="OrderingNumber"></span>
                </div>
                <br clear="all" />


                <!-- NEXT ROW -->
                <!--
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
                    -->
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA"></div>
                <div class="bitPageSettingsCollumnB">
                </div>
                <div class="bitPageSettingsCollumnC">
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->

            </div>
            <!-- TAB1: Afbeeldingen -->
            <div id="tabPageMenuItemsImages" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Afbeelding</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title=""></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <img id="imgImageUrl" src="[data-field]" alt="" data-field="ImageUrl" data-title-field="ImageUrl" style="max-height: 150px; max-width: 150px" />
                    <span id="fileNameImageUrl" data-field="ImageUrl"></span>
                    <input type="button" value="x" title="verwijder afbeelding" onclick="javascript: BITMENUS.clearImage('ImageUrl');" />
                    <input type="button" value="..." onclick="javascript: BITMENUS.openImagePopup('ImageUrl');" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Afbeelding bij mouse over</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title=""></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <img id="imgImageHoverUrl" src="[data-field]" alt="" data-field="ImageHoverUrl" data-title-field="ImageHoverUrl" style="max-height: 150px; max-width: 150px" />
                    <span id="fileNameImageHoverUrl" data-field="ImageHoverUrl"></span>
                    <input type="button" value="x" title="verwijder afbeelding" onclick="javascript: BITMENUS.clearImage('ImageHoverUrl');" />
                    <input type="button" value="..." onclick="javascript: BITMENUS.openImagePopup('ImageHoverUrl');" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Afbeelding voor actieve menuitems</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title=""></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <img id="imgImageActiveUrl" src="[data-field]" alt="" data-field="ImageActiveUrl" data-title-field="ImageHoverUrl" style="max-height: 150px; max-width: 150px" />
                    <span id="fileNameImageActiveUrl" data-field="ImageActiveUrl"></span>
                    <input type="button" value="x" title="verwijder afbeelding" onclick="javascript: BITMENUS.clearImage('ImageActiveUrl');" />
                    <input type="button" value="..." onclick="javascript: BITMENUS.openImagePopup('ImageActiveUrl');" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
            </div>
        </div>
    </div>
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

                        BITMENUS.setImageUrl(url);


                        //window.opener.CKEDITOR.tools.callFunction(funcNum, url);
                        $('#bitFileSelectDialog').dialog('close');
                    },
                    resizable: false
                };

                var elf = $('#elfinder').elfinder(options).elfinder('instance');
            });
        </script>
    </div>

</asp:Content>
