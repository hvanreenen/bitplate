<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubscribeModuleTab.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.ContactFormModule.SubscribeModuleTab" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        
        <div id="standardTabPageOptIn" class="bitTabPage">
            <div class="bitPageSettingsCollumnA">Emailadres verificatie:</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="checkbox" data-field="Settings.SendVerifiationEmail" />
            </div>
            <br clear="all">
            <div class="bitPageSettingsCollumnA">Nieuwsgroep:</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <select runat="server" id="SelectNewsGroup" data-field="Settings.Newsgroup"></select>
            </div>
            <br clear="all">
        </div>
    </form>
</body>
</html>