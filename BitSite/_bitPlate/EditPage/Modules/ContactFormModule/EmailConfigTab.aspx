<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailConfigTab.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.ContactFormModule.EmailConfigTab" %>

<%@ Register Src="../../ModuleConfig/GeneralConfigTab.ascx" TagName="GeneralConfigTab" TagPrefix="uc1" %>

<%@ Register Src="../../ModuleConfig/NavigationActionConfigTab.ascx" TagName="NavigationActionConfigTab" TagPrefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>


    <div id="standardTabPageEmailSettings" class="bitTabPage">
        <div class="bitPageSettingsCollumnA">Stuur formulier naar emailadres:</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="">info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <div>
                <input id="RecipientTypeOne" type="radio" name="RecipientTypeTo" value="static" data-field="Settings.RecipientTypeTo" /><label for="RecipientTypeOne">vast emailadres:</label>
                <input type="text" data-field="Settings.ContactFormRecipient" />
            </div>
            <div>
                <script type="text/javascript">
                    $('#RecipientTypeMore').change(function () {
                        if (!BITEDITPAGE.currentModule.Settings.Recipients) {
                            BITEDITPAGE.currentModule.Settings.Recipients = new Array();
                        }
                        $('#AllRecipients').dataBindList(BITEDITPAGE.currentModule.Settings.Recipients);
                    });
                    

                    function addRecipientToList() {
                        var isvalid = true; //HIER MOET VALIDATIE
                        if (isvalid) {
                            var Recipient = {
                                Name: $('#recipientName').val(),
                                EmailAddress: $('#recipientEmail').val(),
                            }
                            BITEDITPAGE.currentModule.Settings.Recipients.push(Recipient);
                            $('#AllRecipients').dataBindList(BITEDITPAGE.currentModule.Settings.Recipients);
                            $('#recipientName').val('');
                            $('#recipientEmail').val('');
                        }
                    }

                    function deleteRecipientFromList(index) {
                        BITEDITPAGE.currentModule.Settings.Recipients.splice(index, 1);
                        $('#AllRecipients').dataBindList(BITEDITPAGE.currentModule.Settings.Recipients);
                    }
                </script>

                <input id="RecipientTypeMore" type="radio" name="RecipientTypeTo" value="dynamic" data-field="Settings.RecipientTypeTo" /><label for="RecipientTypeMore">Keuzelijst:</label>
                <table id="AllRecipients">
                    <thead>
                        <tr>
                            <td style="width: 222px;">Naam</td>
                            <td style="width: 222px;">Emailadres</td>
                            <td></td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td data-field="Name"></td>
                            <td data-field="EmailAddress"></td>
                            <td><a href="javascript:void(0);" class="bitDeleteButton" data-field="listIndex" onclick="deleteRecipientFromList([data-field])"></a>  </td>
                        </tr>
                    </tbody>
                </table>
                <table>
                    <tr id="newRecipient">
                        <td><input id="recipientName" type="text" /></td>
                        <td><input id="recipientEmail" type="text" /></td>
                        <td><button onclick="addRecipientToList();">Toevoegen</button></td>
                    </tr>
                </table>
            </div>
        </div>
        <br clear="all">
        <div class="bitPageSettingsCollumnA">Stuur formulier vanaf emailadres</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="">info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <div>
                <input id="emailTypeEnumServer" type="radio" name="EmailAddressFrom" value="0" data-field="Settings.EmailAddressFrom" /><label for="emailTypeEnumServer">algemeen emailadres van site (afhankelijk van omgeving): <%=BitSite.SessionObject.CurrentSite.CurrentWorkingEnvironment.EmailSettingsFrom %></label></div>
            <div>
                <input id="emailTypeEnumFormTag" type="radio" name="EmailAddressFrom" value="1" data-field="Settings.EmailAddressFrom" /><label for="emailTypeEnumFormTag">ingevulde email op van formulier</label></div>
            <div>
                <input id="emailTypeEnumElse" type="radio" name="EmailAddressFrom" value="2" data-field="Settings.EmailAddressFrom" /><label for="emailTypeEnumFormTag">anders:</label>
                <input type="text" data-field="Settings.ContactFormSender" />
            </div>
        </div>
        <br clear="all">
        <div class="bitPageSettingsCollumnA">Onderwerp:</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="">info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <input type="text" data-field="Settings.ContactFormSubject" />
        </div>
        <br clear="all">
        <div class="bitPageSettingsCollumnA">BCC:</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="">info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <input type="text" data-field="Settings.BCCEmailAddress" />
        </div>
        <br clear="all">
        <!--<div class="bitPageSettingsCollumnA">Formulier verzonden melding:</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="Settings.SendCompleteMessage" value="Er is een email verzonden." />
            </div>
            <br clear="all">
            <div class="bitPageSettingsCollumnA">Formulier fout meldingen:</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="checkbox" data-field="Settings.showFieldErrorMessageInLabel" />Fouten in label tonen.<br />
                <input type="checkbox" data-field="Settings.showFieldErrorClass" />Class toekennen aan foutieve velden. class:<input type="text" data-field="Settings.showFieldErrorClassName" /><br />
            </div>
            <br clear="all">-->
    </div>

</body>
</html>
