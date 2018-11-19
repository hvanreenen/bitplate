<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master" AutoEventWireup="true" CodeBehind="EventLogList.aspx.cs" Inherits="BitSite._bitPlate.EventLog.EventLogList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="/_bitplate/Eventlog/BITEVENTLOG.js"></script>
    <asp:Literal ID="LiteralScript" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <!-- icons -->
    <ul>
        <li id="liBack"><a href="../Default.aspx" class="bitNavBarButtonBack" runat="server" id="BackLink">back</a>
            <div>Terug</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Event Log
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <br />
    <div id="treeView" style="display: none"></div>
    <div id="tableView">
        <table id="tableData" class="bitGrid" data-control-type="table" cellspacing="0"
            cellpadding="2">
            <thead>
                <tr>
                    <td style="width: 50px"></td>
                    <td style="width: 200px" data-sort-field="Date">Datum</td>
                    <td style="width: 500px" data-sort-field="Description">Naam</td>
                    <td style="width: 200px" data-sort-field="Type"></td>
                    <td style="width: 50px"></td>
                </tr>
            </thead>
            <tbody>
                <tr data-id-field="ID" class="jstree-draggable">
                    <td class="icon">
                        <a data-field="Name" href="javascript:BITFILEMANAGEMENT.showFile('[data-field]', this);" class="file-record">
                            <img data-field="Icon" src="null" alt="" style="width: 30px; height: 30px" />
                        </a>
                    </td>
                    <td data-field="Date" data-format="dd-MM-yyyy HH:mm:ss" ></td>
                    <td data-field="Description" class="bitTableColumnName bitTableColumnEllipsis"></td>
                    <td data-field="Type"></td>
                    <td>
                        <div class="ui-state-default ui-corner-all" style="width: 18px; height: 18px; cursor:pointer;"><span data-id-field="listIndex" onclick="BITEVENTLOG.moreDetails([list-index])" class="ui-icon ui-icon-comment"></span></div>
                    </td>
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
<asp:Content ID="Content4" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <div class="dialogs">
        <div id="eventDetails" title="Event record details">
            <div class="bitPageSettingsCollumnA">Datum/Tijd</div>
            <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Binnen welke (sub)map valt deze pagina?">
                    info</span></div>
            <div class="bitPageSettingsCollumnC" data-field="Date" data-format="dd-MM-yyyy HH:mm:ss"></div>
            <br clear="all" />

            <div class="bitPageSettingsCollumnA">Url</div>
            <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Binnen welke (sub)map valt deze pagina?">
                    info</span></div>
            <div class="bitPageSettingsCollumnC" data-field="Name"></div>
            <br clear="all" />

            <div class="bitPageSettingsCollumnA">Type</div>
            <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Binnen welke (sub)map valt deze pagina?">
                    info</span></div>
            <div class="bitPageSettingsCollumnC" data-field="Type"></div>
            <br clear="all" />

            <div class="bitPageSettingsCollumnA">Failure</div>
            <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Binnen welke (sub)map valt deze pagina?">
                    info</span></div>
            <div class="bitPageSettingsCollumnC" data-field="Failure"></div>
            <br clear="all" />

            <div class="bitPageSettingsCollumnA">Description</div>
            <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Binnen welke (sub)map valt deze pagina?">
                    info</span></div>
            <div class="bitPageSettingsCollumnC"><pre data-field="Description"></pre></div>
            <br clear="all" />
        </div>
    </div>
</asp:Content>
