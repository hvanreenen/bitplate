<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TreeViewModuleConfig.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.DataModules.TreeViewModuleConfig" %>

<%@ Register Src="../../ModuleConfig/GeneralConfigTab.ascx" TagName="GeneralConfigTab" TagPrefix="uc1" %>

<%@ Register Src="../../ModuleConfig/NavigationActionConfigTab.ascx" TagName="NavigationActionConfigTab" TagPrefix="uc2" %>

<%@ Register Src="../../ModuleConfig/DataListConfigTab.ascx" TagName="DataListConfigTab" TagPrefix="uc3" %>

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
            <li id="tabPage3"><a href="#standardTabPageData">Gegevens</a></li>
            <li id="tabPage4"><a href="#tabPagePublish">Tree instellingen</a></li>
        </ul>
        <div id="standardTabPageGeneral" style="display: none">
            <uc1:GeneralConfigTab ID="GeneralConfigTab1" runat="server" />
        </div>
        <div id="standardTabPageNavigation" class="bitTabPage">
            <uc2:NavigationActionConfigTab ID="NavigationActionConfigTab1" runat="server" />
        </div>
        <div id="standardTabPageData" class="bitTabPage">
            <uc3:DataListConfigTab ID="DataListConfigTab1" runat="server" />
        </div>
        <div id="tabPagePublish" class="bitTabPage">
            <div class="bitPageSettingsCollumnA">Gegevens publiceren</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Deze optie verhoogt de performance maar toon geen live data! Als deze optie aan staat, wordt de data uit de datacollectie alvast gepubliceerd nadat u deze module opslaat. Wijzigingen in de data worden dan niet onmiddelijk getoond, maar pas bij opnieuw opslaan.">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input id="checkPrePublishData" type="checkbox" data-field="Settings.PrePublishData" />
                <label for="checkPrePublishData">Vooraf gegevens uit de datacollectie publiceren. (geen live data)</label>
            </div>
            <br clear="all" />
            <!-- NEXT ROW -->

            <div class="bitPageSettingsCollumnA">Theme ondersteuning</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Deze optie verhoogt de performance maar toon geen live data! Als deze optie aan staat, wordt de data uit de datacollectie alvast gepubliceerd nadat u deze module opslaat. Wijzigingen in de data worden dan niet onmiddelijk getoond, maar pas bij opnieuw opslaan.">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="checkbox" data-field="Settings.ThemeSupport" />
            </div>
            <br clear="all" />
            <!-- NEXT ROW -->

             <div class="bitPageSettingsCollumnA">Alle nodes openen.</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Deze optie verhoogt de performance maar toon geen live data! Als deze optie aan staat, wordt de data uit de datacollectie alvast gepubliceerd nadat u deze module opslaat. Wijzigingen in de data worden dan niet onmiddelijk getoond, maar pas bij opnieuw opslaan.">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <input type="checkbox" data-field="Settings.OpenAllInitialy" />
            </div>
            <br clear="all" />
            <!-- NEXT ROW -->

            <div class="bitPageSettingsCollumnA">Animation speed.</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Deze optie verhoogt de performance maar toon geen live data! Als deze optie aan staat, wordt de data uit de datacollectie alvast gepubliceerd nadat u deze module opslaat. Wijzigingen in de data worden dan niet onmiddelijk getoond, maar pas bij opnieuw opslaan.">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <div id="jstreeAnimationSpeedSlider"></div>
                <script type="text/javascript">
                    var firstChange = true;
                    $('#jstreeAnimationSpeed').change(function () {
                        var currentAnimationSpeed = $('#jstreeAnimationSpeed').val();
                        if (firstChange) {
                            $('#jstreeAnimationSpeedSlider').slider({
                                min: 0,
                                max: 500,
                                slide: function (event, ui) {
                                    $('#jstreeAnimationSpeed').val(ui.value);
                                },
                                value: currentAnimationSpeed
                            });
                            firstChange = false;
                        }
                    });
                </script>
                <input id="jstreeAnimationSpeed" type="hidden" data-field="Settings.AnimationSpeed" />
                <label for="jstreeAnimationSpeed"></label>
            </div>
            <br clear="all" />
            <!-- NEXT ROW -->
        </div>
    </form>
</body>
</html>
