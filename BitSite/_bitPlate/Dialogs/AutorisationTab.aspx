<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutorisationTab.aspx.cs"
    Inherits="BitSite._bitPlate.Dialogs.AutorisationTab" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="/_bitplate/Dialogs/BITAUTORISATIONTAB.js"></script>
</head>
<body>
    <asp:Literal ID="LiteralMsg" runat="server"></asp:Literal>
    <asp:Panel ID="PanelMain" runat="server">
        <div id="tabPageAutorisation" class="bitTabPage">
            <div class="bitPageSettingsCollumnA">Toegankelijkheid</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input type="radio" name="radioObjectPermissions" id="radioNoAutorisation" value="false"
                    data-field1="HasAutorisation" onclick="javascript: BITAUTORISATIONTAB.changeHasAutorisation();"
                    checked="checked" />
                <label for="radioNoAutorisation">Toegankelijk voor iedereen</label><br />
                <input type="radio" name="radioObjectPermissions" id="radioHasAutorisation" value="true"
                    data-field1="HasAutorisation" onclick="javascript: BITAUTORISATIONTAB.changeHasAutorisation();" />
                <label for="radioHasAutorisation">Beperkt toegankelijk</label><br />
                <br />
                <div id="divAutorisation" disabled="disabled" style="display: none;">
                    <b>Alleen toekankelijk voor:</b><br />

                    <input type="button" value="Toevoegen gebruikersgroep..." onclick="javascript: BITAUTORISATIONTAB.openUserGroupsPopup();" />
                    <br />
                    <div id="divBitplateUserGroups" style="display: none">
                        <b>Bitplate backend gebruikersgroepen:</b><br />
                        <table id="tableBitplateUserGroups" style="width: 100%">
                            <tbody>
                                <tr>
                                    <td style="width: 150px" data-field="Name"></td>
                                    <td style="width: 50px"><a data-child-field="ID" title="verwijder" href="javascript:BITAUTORISATIONTAB.removeUserGroup('[data-field]', [list-index], 'BitplateUserGroup');"
                                        class="bitDeleteButton">verwijder </a></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div id="divSiteUserGroups" class="siteAuthorisation" style="display: none">
                        <br />
                        <b>Site frontend gebruikersgroepen:</b><br />
                        <table id="tableSiteUserGroups" style="width: 100%">
                            <tbody>
                                <tr>
                                    <td style="width: 150px" data-field="Name"></td>
                                    <td style="width: 50px"><a data-child-field="ID" title="verwijder" href="javascript:BITAUTORISATIONTAB.removeUserGroup('[data-field]', [list-index], 'SiteUserGroup');"
                                        class="bitDeleteButton">verwijder </a></td>
                                </tr>
                            </tbody>
                        </table>
                        <br />
                    </div>
                    <br />
                    <input type="button" value="Toevoegen gebruiker..." onclick="javascript: BITAUTORISATIONTAB.openUsersPopup();" />
                    <br />
                    <div id="divBitplateUsers" style="display: none">
                        <b>Bitplate backend gebruikers:</b><br />
                        <table id="tableBitplateUsers">
                            <tbody>
                                <tr>
                                    <td style="width: 150px" data-field="CompleteName"></td>
                                    <td style="width: 50px"><a data-child-field="ID" title="verwijder" href="javascript:BITAUTORISATIONTAB.removeUser('[data-field]', [list-index], 'BitplateUser');"
                                        class="bitDeleteButton">verwijder </a></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div id="divSiteUsers" class="siteAuthorisation" style="display: none">
                        <br />
                        <b>Site frontend gebruikers:</b><br />
                        <table id="tableSiteUsers">
                            <tbody>
                                <tr>
                                    <td style="width: 150px" data-field="Name"></td>
                                    <td style="width: 50px"><a data-child-field="ID" title="verwijder" href="javascript:BITAUTORISATIONTAB.removeUser('[data-field]', [list-index], 'SiteUser');"
                                        class="bitDeleteButton">verwijder </a></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>

            </div>
            <br clear="all" />

        </div>

        <!-- USERGROUPS DIALOG -->
        <div id="bitUserGroupsDialog" title="Gebruikersgroep kiezen" style="display: none;">
            Bitplate backend gebruikersgroepen:
        <ul id="bitBitplateUserGroupsList" style="list-style-type: none" data-control-type="list">
            <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px; padding: 5px"><a data-field="ID" data-title-field="Name" href="javascript:BITAUTORISATIONTAB.addUserGroup('[data-field]', '[data-title-field]', 'BitplateUserGroup');">
                <input type="hidden" data-field="ID" />
                <span data-field="Name"></span><span class="remove-script btn-remove-script"></span>
            </a></li>
        </ul>
            <div class="siteAuthorisation">
                <br />
                Site frontend gebruikersgroepen:
            <ul id="bitSiteUserGroupsList" style="list-style-type: none" data-control-type="list">
                <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px; padding: 5px"><a data-field="ID" data-title-field="Name" href="javascript:BITAUTORISATIONTAB.addUserGroup('[data-field]', '[data-title-field]', 'SiteUserGroup');">
                    <input type="hidden" data-field="ID" />
                    <span data-field="Name"></span><span class="remove-script btn-remove-script"></span>
                </a></li>
            </ul>
            </div>

        </div>

        <!-- USERS DIALOG -->
        <div id="bitUsersDialog" title="Gebruikers kiezen" style="display: none;">
            Bitplate backend gebruikers:
        <ul id="bitBitplateUsersList" style="list-style-type: none" data-control-type="list">
            <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px; padding: 5px"><a data-field="ID" data-title-field="CompleteName" href="javascript:BITAUTORISATIONTAB.addUser('[data-field]', '[data-title-field]', 'BitplateUser');">
                <input type="hidden" data-field="ID" />
                <span data-field="CompleteName"></span><span class="remove-script btn-remove-script"></span>
            </a></li>
        </ul>
            <div class="siteAuthorisation">
                <br />
                Site frontend gebruikers:
            <ul id="bitSiteUsersList" style="list-style-type: none" data-control-type="list">
                <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px; padding: 5px"><a data-field="ID" data-title-field="CompleteName" href="javascript:BITAUTORISATIONTAB.addUser('[data-field]', '[data-title-field]', 'SiteUser');">
                    <input type="hidden" data-field="ID" />
                    <span data-field="CompleteName"></span><span class="remove-script btn-remove-script"></span></a></li>
            </ul>
            </div>
        </div>
    </asp:Panel>
</body>
</html>
