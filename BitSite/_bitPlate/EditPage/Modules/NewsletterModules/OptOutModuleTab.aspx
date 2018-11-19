<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OptOutModuleTab.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.ContactFormModule.OptOutModuleTab" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        
        <div id="standardTabPageOptOut" class="bitTabPage">
            <div class="bitPageSettingsCollumnA">Verstuur afscheids email:</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="checkbox" data-field="Settings.SendOptOutEmail" />
            </div>
            <br clear="all">

            <div class="bitPageSettingsCollumnA">Toon nieuwsgroepenlijst:</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="checkbox" data-field="Settings.ShowNewsGroupList" />
            </div>
            <br clear="all">
        </div>
    </form>
</body>
</html>