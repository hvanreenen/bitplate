<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InputFormModuleConfig.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.InputFormModule.InputFormModuleConfig" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="_bitModules/InputFormModule/CONFIG.js"></script>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/Plugins/ckeditor") %>
    <script type="text/javascript">
        $(document).ready(function () {
            $(document).on('onConfigLoaded', BITINPUTFORMMODULE.configLoadComplete);
            $(document).on('onConfigSave', BITINPUTFORMMODULE.configSave);
        });
    </script>
</head>
<body>
    <div class="bitTabs">
        <ul>
            <li id="tabPage1"><a href="#tab1">Form Validatie</a></li>
            <li id="tabPage2"><a href="#tab2">Form Resultaten</a></li>
        </ul>
        <div id="tab1">
            <div>
                <input type="checkbox" data-field="Settings.EnableTooltip" id="tooltipState" /><label for="tooltipState">Maak gebruik van tooltip meldingen.</label>
            </div>
            <table id="InputFormFieldList">
                <thead>
                    <tr>
                        <td>Veldnaam</td>
                        <td>Verplicht</td>
                        <td>DataType</td>
                        <td>ErrorMessage</td>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td data-field="fieldName"></td>
                        <td><input type="checkbox" data-field="dataRequired" class="required" data-id-field="fieldName" /></td>
                        <td>
                            <select data-field="dataValidationType">
                                <option>Text</option>
                                <option>Number</option>
                                <option>Email</option>
                            </select>
                        </td>
                        <td><input type="text" data-field="errorMessage" class="fieldError" /></td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div id="tab2">

            <div class="bitPageSettingsCollumnA">Formulier opslaan in:</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input data-field="Settings.isEmailEnabled" type="checkbox" name="saveToEmail" id="saveToEmail" onchange="BITINPUTFORMMODULE.emailStateToggle()" /><label for="saveToEmail">Verstuur Email</label>
                <input data-field="Settings.isDataCollectieEnabled" type="checkbox" name="saveToDataCollectie" id="saveToDataCollectie" onchange="BITINPUTFORMMODULE.dataCollectieStateToggle()"/><label for="saveToEmail">Opslaan in DataCollectie</label>
            </div>
            <br clear="all" />

            <div id="bitFormEmailSettings" style="display: none;">
                <div>Email instellingen</div>
                <div class="bitPageSettingsCollumnA">Email van:</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" name="emailFrom" data-field="Settings.EmailFrom" />
                </div>
                <br clear="all" />

                <div class="bitPageSettingsCollumnA">Email naar:</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Settings.EmailTo" type="text" name="emailTo" />
                    <input data-field="Settings.EmailToOverride" type="checkbox" name="emailToOverride" id="emailToOverride" /><label for="emailToOverride">Override EmailTo</label>
                </div>
                <br clear="all" />

                <div class="bitPageSettingsCollumnA">Onderwerp:</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input data-field="Settings.EmailSubject" type="text" name="emailOnderwerp" />
                </div>
                <br clear="all" />

                <div class="bitPageSettingsCollumnA">Template:</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">

                    <textarea data-field="Settings.EmailTemplate" name="emailTemplate" id="emailTemplate"></textarea>
                </div>
                <br clear="all" />
            </div>
        </div>
    </div>
</body>
</html>
