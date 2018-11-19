<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContactFormModuleConfig.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.ContactFormModule.ContactFormModuleConfig" %>
<%@ Register Src="../../ModuleConfig/GeneralConfigTab.ascx" TagName="GeneralConfigTab" TagPrefix="uc1" %>

<%@ Register src="../../ModuleConfig/NavigationActionConfigTab.ascx" tagname="NavigationActionConfigTab" tagprefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <ul>
            <li id="tabPage1"><a href="#standardTabPageGeneral">Algemene instellingen</a></li>
            <li id="tabPage2"><a href="#standardTabPageNavigation">Navigatie</a></li>
            <li id="tabPage3"><a href="#standardTabPageEmailSettings">Email instellingen</a></li>
            <li id="tabPage4"><a href="#standardTabPageEmailTemplate">Emailtemplate</a></li>
        </ul>
        <div id="standardTabPageGeneral" style="display: none">
            <uc1:GeneralConfigTab ID="GeneralConfigTab1" runat="server" />
        </div>
        <div id="standardTabPageNavigation" class="bitTabPage">
            <uc2:NavigationActionConfigTab ID="NavigationActionConfigTab1" runat="server" />
        </div>
        <div id="standardTabPageEmailSettings" class="bitTabPage">
            <div class="bitPageSettingsCollumnA">Stuur formulier naar emailadres:</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" data-field="Settings.ContactFormRecipient" />
            </div>
            <br clear="all">
            <div class="bitPageSettingsCollumnA">Stuur formulier vanaf emailadres</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <div><input id="emailTypeEnumServer" type="radio" name="EmailAddressFrom" value="0" data-field="Settings.EmailAddressFrom" /><label for="emailTypeEnumServer">algemeen emailadres van site: <%=BitSite.SessionObject.CurrentSite.EmailSettingsFrom %></label></div>
                <div><input id="emailTypeEnumFormTag" type="radio" name="EmailAddressFrom" value="1" data-field="Settings.EmailAddressFrom" /><label for="emailTypeEnumFormTag">ingevulde email op van formulier</label></div>
                <div><input id="emailTypeEnumElse" type="radio" name="EmailAddressFrom" value="2" data-field="Settings.EmailAddressFrom" /><label for="emailTypeEnumFormTag">anders:</label>
                <input type="text" data-field="Settings.ContactFormSender" /></div>
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
        <div id="standardTabPageEmailTemplate" class="bitTabPage">
            <textarea name="ckeditor" data-field="Settings.ContactFormTemplate" ></textarea>
            <script type="text/javascript">
                var editor = CKEDITOR.replace('ckeditor', {
                    fullPage: true
                });
                editor.config.extraPlugins = 'tags';
                setTags('<div class="tagToInsert">[FormResult]</div>');
            </script>
        </div>
    </form>
</body>
</html>