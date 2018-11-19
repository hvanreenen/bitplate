<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuModuleTab.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.Menu.MenuModuleTab" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

        <div id="tabPageMenu" class="bitTabPage">
            <div class="bitPageSettingsCollumnA">Menu</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Kies het menu">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <select id="selectMenu" runat="server" data-field="Settings.FK_Menu" onchange=""></select>
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Animation speed.</div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <span id="animationSpeed" data-field="Settings.AnimationSpeed"></span>
                <div id="animationSpeedSlider"></div>

                

            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA"><b>Scripts</b></div>
            <div class="bitPageSettingsCollumnB">
                <span class="bitInfo" title="Gekoppelde scripts. Wijzig deze koppelingen via menu beheer">info</span>
            </div>
            <div class="bitPageSettingsCollumnC">
                <asp:Literal ID="LiteralScripts" runat="server"></asp:Literal>
            </div>
            <br clear="all" />

        </div>

        <script type="text/javascript">
            $(document).ready(function () {
                var firstChange = true;
                $('#animationSpeed').change(function () {
                    var currentAnimationSpeed = $('#animationSpeed').val();
                    if (firstChange) {
                        $('#animationSpeedSlider').slider({
                            min: 0,
                            max: 1000,
                            slide: function (event, ui) {
                                $('#animationSpeed').html(ui.value);
                                //alert(ui.value);
                            },
                            value: currentAnimationSpeed
                        });
                        firstChange = false;
                    }
                });
                $('#selectMenu').change(function () {
                    var id = $('#selectMenu').val();
                    id = id.replace(/-/g, '');
                    //alert(id);
                    $(".menuScripts").hide();
                    $("#div" + id).show();
                });
            });
        </script>

    </form>
</body>
</html>
