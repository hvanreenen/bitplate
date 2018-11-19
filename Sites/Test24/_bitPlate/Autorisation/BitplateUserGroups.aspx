<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="BitplateUserGroups.aspx.cs" Inherits="BitSite._bitPlate.Autorisation.BitplateUserGroups" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="/_bitplate/_js/prototypes/dockable.js"></script>
    <script type="text/javascript" src="BITAUTHUSERGROUPS.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITAUTHUSERGROUPS.initialize();
            BITAUTHUSERGROUPS.loadUserGroups();
            //BITAUTHUSERGROUPS.loadUsers();
            //BITAUTHUSERGROUPS.showUsers();
        });
    </script>
    <asp:Literal ID="DefaultPermissionSets" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="BitplateUsers.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        
        <li><a href="javascript:BITAUTHUSERGROUPS.newUserGroup();" class="bitNavBarButtonAddPage">
            addpusergroup</a>
            <div>Nieuwe gebruikersgroep...</div>
        </li>

    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Gebruikersgroepen
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    

    <table id="tableUserGroups" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 140px" data-sort-field="Name">Naam</td>
                <td class="bitTableColumn" style="width: 140px" data-sort-field="SuperUserGroupType">Type</td>
                

                <td class="bitTableActionButton">Bewerk</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableActionButton">Kopieer</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum</td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon"><a href="#" class="bitTablePageIcon"></a></td>
                <td class="nameColumn" data-field="Name"></td>
                <td data-field="TypeString"></td>

                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITAUTHUSERGROUPS.openUserGroupDetailsPopup('[data-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" href="javascript:BITAUTHUSERGROUPS.removeUserGroup('[data-field]');"
                    class="bitDeleteButton"></a></td>
                <td class="bitTableActionButton"><a data-field="ID" data-title-field="Name" href="javascript:BITAUTHUSERGROUPS.copyUserGroup('[data-field]', '[data-title-field]');"
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
        </tfoot>
    </table>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">

    <div id="bitUserGroupDetailsDialog" title="Gebruikersgroep">
        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageDetailsUserGroup1">Algemene gegevens</a></li>
                <li><a href="#tabPageDetailsUserGroup2">Rechten</a></li>
            </ul>
            <!-- TAB1 -->
            <div id="tabPageDetailsUserGroup1" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Naam</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Name" type="text" data-validation="required" />
                </div>
                <br clear="all" />
                <div class="bitPageSettingsCollumnA">Type</div>
                <div class="bitPageSettingsCollumnB"></div>
                <div class="bitPageSettingsCollumnC">
                    <select data-field="Type" id="selectDefaultPermissions" onchange="javascript:BITAUTHUSERGROUPS.setDefaultPermissions();">
                    <option value="0">Custom</option>
                    <option value="1">Moderator</option>
                    <option value="2">Designer</option>
                    <option value="9">Admin</option>
                </select>
                </div>
                <br clear="all" />
                
                <div id="divSystemValue">
                    <!--systeemwaarde = hard coded, want is altijd waar bij company admins-->
                    <div class="bitPageSettingsCollumnA">Systeemwaarde</div>
                    <div class="bitPageSettingsCollumnB"></div>
                    <div class="bitPageSettingsCollumnC">
                        <span>Ja</span>
                    </div>
                    <br clear="all" />

                </div>
                
            </div>

            <!-- TAB2 -->
            <!-- TAB2 -->
            <!-- TAB2 -->
            <div id="tabPageDetailsUserGroup2" class="bitTabPage">
                
                <asp:Literal ID="LiteralUserGroupPermissions" runat="server"></asp:Literal>
            </div>
        </div>
    </div>

</asp:Content>
