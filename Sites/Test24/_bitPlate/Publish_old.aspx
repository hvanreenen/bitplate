<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/bitMasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Publish_old.aspx.cs" Inherits="BitSite._bitPlate.Publish_old" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="_js/pages/BITPUBLISH.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITPUBLISH.loadUnpublishedItems();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="Default.aspx" class="bitNavBarButtonBack" runat="server" id="BackLink">
            back</a>
            <div>Terug</div>
        </li>
        <li><a href="javascript:BITPUBLISH.publish();" class="bitBoardPublish">Publiceer
        </a>
            <div>Publiceer</div>
        </li>
        <!--<li id="liSaveConfig" runat="server"><a id="aSaveConfig" runat="server" href="javascript:BITPUBLISH.saveConfig();" class="bitNavBarButtonSavePage">Opslaan</a>
            <div>Opslaan</div>
        </li>-->
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Publiceer
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <br />
    <input type ="radio" id="radioLiveEnvironment" name="publishMode" checked="checked" onchange="javascript:BITPUBLISH.changeLiveTestEnvironment();"/><label for="radioLiveEnvironment">Publiceer naar LIVE omgeving</label><br />
    <input type ="radio" id="radioTestEnvironment" name="publishMode" onchange="javascript:BITPUBLISH.changeLiveTestEnvironment();"/><label for="radioTestEnvironment">Publiceer naar TEST omgeving</label>
    <div id="publishTestModeImmediate" style="display:none"><input type="checkbox" id="checkboxTestModeImmediate" disabled="disabled" runat="server"/><label for="checkboxTestModeImmediate">Wijzigingen onmiddelijk naar testomgeving publiceren</label></div> 
    <div id="publishLiveFolder">Publicatie map op server: <asp:Label ID="LabelPublishFolder" runat="server" Text=""></asp:Label> </div>
    <div id="publishTestFolder" style="display:none">Publicatie map op server (testomgeving): <asp:Label ID="LabelPublishFolderTestEnvironment" runat="server" Text=""></asp:Label></div> <br />
    (wijzig bij algemene instellingen)<br />
    <fieldset>
        <legend>Filter</legend>
        <input type="checkbox" id="checkPublishComplete" onclick="javascript:BITPUBLISH.selectAll(this, 'CompleteSite');" />
        <label for="checkPublishComplete">
            Hele site publiceren (ongeacht of er een wijziging
            heeft plaatsgevonden)</label>
        <br />
        <span id="bitSpanHidden"><input type="checkbox" id="checkCleanUp" disabled="disabled" />
        <label id="labelCleanUp" for="checkCleanUp">
            Opschonen: alle bestanden eerst verwijderen
        </label><br />
            </span>
        
        <input type="checkbox" id="checkPublishFilesAndImages" checked="checked" />
        <label for="checkPublishFilesAndImages">
            Afbeeldingen en andere bestanden kopieren
        </label>
        <br />
        <input type="checkbox" id="checkReGenerateSearchIndex" />
        <label for="checkReGenerateSearchIndex">Zoekindex opnieuw aanmaken (langzaam)</label>
        <br />
        <br />
        <input type="checkbox" id="checkAll" onclick="javascript:BITPUBLISH.selectAll(this, 'All');"
            checked="checked" />
        <label for="checkAll">Alles aan/uit</label>
    </fieldset>
    <br />
    <table id="tableUnpublishedItems" class="bitGrid">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 15px"></td>
                <td class="bitTableColumn" style="width: 70px" data-sort-field="Type">Type</td>
                <td class="bitTableColumn" style="width: 150px" data-sort-field="Name">Naam</td>
                <td class="bitTableColumn" style="width: 100px" data-sort-field="LastPublishedDate">
                    Laatst gepubliceerd</td>
                <td class="bitTableColumn" style="width: 50px" data-sort-field="ChangeStatus">Status
                </td>
                <td class="bitTableColumn" style="width: 100px" data-sort-field="ModifiedDate">Laatst
                    gewijzigd</td>
                <td class="bitTableColumn" style="width: 150px" data-sort-field="UserName">Door wie
                </td>
                <td class="bitTableColumn" style="width: 50px">Wijzigingen</td>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>
                    <input type="checkbox" checked="checked" data-field="Checked" data-title-field="FK_Object"
                        class="checkItem" />
                </td>
                <td data-field="Type"></td>
                <td class="bitTableColumnEllipsis" data-field="Name" data-title-field="Name"></td>
                <td data-field="LastPublishedDate" data-format="dd-MM-yyyy HH:mm:ss"></td>
                <td data-field="ChangeStatusString"></td>
                <td data-field="ModifiedDate" data-format="dd-MM-yyyy HH:mm:ss"></td>
                <td data-field="UserName"></td>
                <td class="bitTableActionButton"><a href="#" onclick="javascript:BITPUBLISH.openUnpublishedChangesPopup(this);"
                    class="bitInfo">wijzigingen
                </a>
                    <div style="display: none;" data-field="Actions"></div>
                </td>
            </tr>
        </tbody>
        <tfoot>
            <tr>
                <td colspan="3" data-control-type="paging"></td>
            </tr>
        </tfoot>
    </table>
    <div class="bitEndTable"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <!--WIJZIGINGEN INFO DIALOG -->
    <div id="bitChangesInfoDialog" title="Wijzigingen" style="display: none;">
    </div>
    <!--PUBLICEREN GEREED DIALOG -->
    <div id="bitPublishReadyDialog" title="Publiceren" style="display: none;">
        Het publiceren is gereed!
    </div>
</asp:Content>
