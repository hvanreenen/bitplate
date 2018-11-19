<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutorisationTab2.aspx.cs"
    Inherits="BitSite._bitPlate.Dialogs.AutorisationTab2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="/_bitplate/Dialogs/BITAUTORISATIONTAB2.js"></script>
</head>
<body>
    <div id="tabPageAutorisation2" class="bitTabPage">
        <div class="bitPageSettingsCollumnA">Toegankelijkheid</div>
        <div class="bitPageSettingsCollumnB"></div>
        <div class="bitPageSettingsC  ollumnC">
            <input type="radio" name="radioObjectPermissions" id="radioNoAutorisation2" value="false"
                data-field1="HasAutorisation" onclick="javascript: BITAUTORISATIONTAB2.changeHasAutorisation();"
                checked="checked" />
            <label for="radioNoAutorisation">Toegankelijk voor iedereen</label><br />
            <input type="radio" name="radioObjectPermissions" id="radioHasAutorisation2" value="true"
                data-field1="HasAutorisation" onclick="javascript: BITAUTORISATIONTAB2.changeHasAutorisation();" />
            <label for="radioHasAutorisation">Beperkt toegankelijk</label><br />
            <br />
            <div id="divAutorisation2" disabled="disabled">
                <b>Alleen toekankelijk voor:</b><br />

                <input type="button" value="Toevoegen gebruikersgroep..." onclick="javascript: BITAUTORISATIONTAB2.openUserGroupsPopup();" />
                <br />
                <div id="divBitplateUserGroups2" style="display:none">
                    <b>Bitplate backend gebruikersgroepen:</b><br />
                    <table id="tableBitplateUserGroups2" style="width: 100%">
                        <tbody>
                            <tr>
                                <td style="width: 150px" data-field="Name"></td>
                                <td style="width: 50px"><a data-child-field="ID" title="verwijder" href="javascript:BITAUTORISATIONTAB2.removeUserGroup('[data-field]', [list-index], 'BitplateUserGroup');"
                                    class="bitDeleteButton">verwijder </a></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div id="divSiteUserGroups2" class="siteAuthorisation" style="display:none">
                    <br />
                    <b>Site frontend gebruikersgroepen:</b><br />
                    <table id="tableSiteUserGroups2" style="width: 100%">
                        <tbody>
                            <tr>
                                <td style="width: 150px" data-field="Name"></td>
                                <td style="width: 50px"><a data-child-field="ID" title="verwijder" href="javascript:BITAUTORISATIONTAB2.removeUserGroup('[data-field]', [list-index], 'SiteUserGroup');"
                                    class="bitDeleteButton">verwijder </a></td>
                            </tr>
                        </tbody>
                    </table>
                    <br />
                </div>
                <br />
                <input type="button" value="Toevoegen gebruiker..." onclick="javascript: BITAUTORISATIONTAB2.openUsersPopup();" />
                <br />
                <div id="divBitplateUsers2" style="display:none">
                    <b>Bitplate backend gebruikers:</b><br />
                    <table id="tableBitplateUsers2">
                        <tbody>
                            <tr>
                                <td style="width: 150px" data-field="CompleteName"></td>
                                <td style="width: 50px"><a data-child-field="ID" title="verwijder" href="javascript:BITAUTORISATIONTAB2.removeUser('[data-field]', [list-index], 'BitplateUser');"
                                    class="bitDeleteButton">verwijder </a></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div id="divSiteUsers2" class="siteAuthorisation" style="display:none">
                    <br />
                    <b>Site frontend gebruikers:</b><br />
                    <table id="tableSiteUsers2">
                        <tbody>
                            <tr>
                                <td style="width: 150px" data-field="Name"></td>
                                <td style="width: 50px"><a data-child-field="ID" title="verwijder" href="javascript:BITAUTORISATIONTAB2.removeUser('[data-field]', [list-index], 'SiteUser');"
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
    <div id="bitUserGroupsDialog2" title="Gebruikersgroep kiezen" style="display: none;">
        Bitplate backend gebruikersgroepen:
        <ul id="bitBitplateUserGroupsList2" style="list-style-type: none" data-control-type="list">
            <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px;
                padding: 5px"><a data-field="ID" data-title-field="Name" href="javascript:BITAUTORISATIONTAB2.addUserGroup('[data-field]', '[data-title-field]', 'BitplateUserGroup');">
                    <input type="hidden" data-field="ID" />
                    <span data-field="Name"></span><span class="remove-script btn-remove-script"></span>
                </a></li>
        </ul>
        <div class="siteAuthorisation">
            <br />
            Site frontend gebruikersgroepen:
            <ul id="bitSiteUserGroupsList2" style="list-style-type: none" data-control-type="list">
                <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px;
                    padding: 5px"><a data-field="ID" data-title-field="Name" href="javascript:BITAUTORISATIONTAB2.addUserGroup('[data-field]', '[data-title-field]', 'SiteUserGroup');">
                        <input type="hidden" data-field="ID" />
                        <span data-field="Name"></span><span class="remove-script btn-remove-script"></span>
                    </a></li>
            </ul>
        </div>

    </div>

    <!-- USERS DIALOG -->
    <div id="bitUsersDialog2" title="Gebruikers kiezen" style="display: none;">
        Bitplate backend gebruikers:
        <ul id="bitBitplateUsersList2" style="list-style-type: none" data-control-type="list">
            <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px;
                padding: 5px"><a data-field="ID" data-title-field="CompleteName" href="javascript:BITAUTORISATIONTAB2.addUser('[data-field]', '[data-title-field]', 'BitplateUser');">
                    <input type="hidden" data-field="ID" />
                    <span data-field="CompleteName"></span><span class="remove-script btn-remove-script"></span>
                </a></li>
        </ul>
        <div class="siteAuthorisation">
            <br />
            Site frontend gebruikers:
            <ul id="bitSiteUsersList2" style="list-style-type: none" data-control-type="list">
                <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px;
                    padding: 5px"><a data-field="ID" data-title-field="CompleteName" href="javascript:BITAUTORISATIONTAB2.addUser('[data-field]', '[data-title-field]', 'SiteUser');">
                        <input type="hidden" data-field="ID" />
                        <span data-field="CompleteName"></span><span class="remove-script btn-remove-script">
                        </span></a></li>
            </ul>
        </div>
    </div>

</body>
</html>
