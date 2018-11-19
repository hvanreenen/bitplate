<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HTMLModuleConfig.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.HTMLModule.HTMLModuleConfig" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <div class="bitTabs">
        <ul>
            <li id="tabPage1"><a href="#tab1">Tab1</a></li>
            <li id="tabPage2"><a href="#tab2">Tab2</a></li>
        </ul>
        <div id="tab1">

            <div class="bitPageSettingsCollumnA">Taal:</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <select data-field="Settings.Language">
                    <option value="NL">Nederlands</option>
                    <option value="EN">Engels</option>
                </select>
                
            </div>
            <br clear="all" />

        </div>
        <div id="tab2">
            Extra tab ter test

        </div>
    </div>
</body>
</html>
