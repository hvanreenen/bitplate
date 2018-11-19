<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs"
    Inherits="BitSite._bitPlate.Login" %>

<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>login</title>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/BitCore") %>

    <link href="_themes/bitplate/css/login.css" rel="stylesheet" type="text/css" media="screen" />
    <link href="_themes/bitplate/css/languageswitcher.css" rel="stylesheet" type="text/css"
        media="screen" />
    <link href="_themes/bitplate/css/topMenu.css" rel="stylesheet" type="text/css" media="screen" />
    <link href="_themes/bitplate/css/formElements.css" rel="stylesheet" type="text/css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="<%=ResolveClientUrl("_themes/bitplate/jquery-ui-1.8.22/css/custom-theme/jquery-ui-1.8.22.custom.css")%>" />
    <script type="text/javascript" src="_js/plugins/jQuery.style.checkbox.js"></script>
    <script type="text/javascript" src="_js/plugins/languageswitcher.js"></script>
    <script type="text/javascript" src="_js/plugins/jQuery.change.css.js"></script>
    <script type="text/javascript" src="_js/plugins/jQuery.switchPanel.js"></script>
</head>

<body>
    <!-- page -->
    <div id="divWrapperMain">
        <!-- login window -->
        <div id="bitLoginWindow">

            <div id="bitLoginWindowTitle">
                <div id="bitLoginWindowLogo"></div>
            </div>
            <div id="bitLoginWindowMain">
                <div id="bitLoginWindowLogin">
                    <asp:Label ID="LabelMsg" runat="server" Text="" Visible="false"></asp:Label>
                    <form method="post" action="javascript:BITAUTH.login();">
                    <div id="bitLoginWindowUser">
                        Gebruikersnaam:<br />
                        <input type="text" runat="server" name="username" id="username" class="required" title="Vul hier uw Gebruikersnaam in..."
                            autocomplete="on" placeholder="Vul hier uw Gebruikersnaam in..." onkeypress1="javascript:BITAUTH.checkEnter(event, '{0}');" />
                    </div>
                    <div id="bitLoginWindowPassword">
                        Wachtwoord:<br />
                        <input type="password" runat="server" name="password" id="password" class="required" title="Vul hier uw Wachtwoord in..."
                            autocomplete="on" placeholder="Vul hier uw Wachtwoord in..." onkeypress1="javascript:BITAUTH.checkEnter(event, '{0}');" />
                    </div>
                    <div id="bitLoginWindowSave">
                        <span>
                            <input runat="server" type="checkbox" name="save" id="save" /><label for="save" title="Bewaren">Bewaar gebruikersnaam
                                </label></span>

                        
                    </div>


                    <!-- country select -->
                    <select id="country-options" name="country-options">
                        
                        <option title="http://www.abcnet.nl" value="nl">Nederlands</option>
                        <!--<option title="http://www.abcnet.nl" value="en">English</option>
                        <option title="http://www.abcnet.nl" value="fr">Français</option>
                        <option title="http://www.abcnet.nl" value="de">Deutsch</option>-->
                    </select>
                    <!--
                    <div id="country-select">
                        <form action="server-side-script.php">
                        <select id="country-options_old" name="country-options">
                            <option selected="selected" title="#" value="nl">Selecteer uw taal</option>
                            <option title="http://www.abcnet.nl" value="us">English</option>
                            <option title="http://www.abcnet.nl" value="fr">Français</option>
                            <option title="http://www.abcnet.nl" value="de">Deutsch</option>
                            <option title="http://www.abcnet.nl" value="nl">Nederlands</option>
                        </select>
                        <input value="Select" type="submit" />
                        </form>
                    </div>
                        -->
                    <!-- .country select -->
                    <!-- theme select -->
                    <div id="bitLoginWindowSelect">
                        <select onchange="BITAUTH.changeTheme()" name="theme_select" id="selectTheme" runat="server">
                        </select>
                    </div>
                    <!-- .theme select -->
                    <div class="clr_bth"></div>
                    <div class="bitLoginWindowButton">
                        <input type="submit" name="submit" id="submit" value="Login" />
                        <input onclick="javascript:BITAUTH.showForgetPassword();" type="button" name="forget-password"
                            id="forget_password" value="Wachtwoord vergeten" />
                    </div>
                    </form>
                </div>
                <div>
                    <!-- .login window -->
                    <!-- forgot password -->
                    <div id="bitLoginWindowForgotPassword">
                        <span>Voer hieronder uw e-mail adres in, die bij ons bekend is en er wordt een nieuw
                            wachtwoord naar dit adres verstuurd.</span>
                        <div class="bitLoginWindowUser">
                            E-mailadres:<br />
                            <input type="text" name="E-mailadres" id="E-mailadres" class="required" title="Vul hier uw E-mailadres in..."
                                placeholder="Vul hier uw E-mailadres in..." />
                        </div>
                        <div class="clr_bth"></div>
                        <div class="bitLoginWindowButton">
                            <input onclick="BITAUTH.sendNewPassword(); return false;" type="button" name="request-password"
                                id="request-password" value="Wachtwoord opnieuw aanvragen" />
                            <input onclick="BITAUTH.showLogin()" type="button" name="back" id="back" value="Cancel" />
                        </div>
                    </div>
                    <!-- .forgot password -->
                </div>
            </div>
        </div>
        <!-- .login window -->
    </div>
    <!-- .page -->
    <div style="display: none">
        <div id="bitMasterInputDialog">
            <!--<p><span id="bitMasterInputBoxIcon" class="ui-icon ui-icon-alert" style="float: left; margin-right: .3em;"></span></p>-->
            <div id="bitMasterInputMessage"></div>
            <div>
                <input id="bitMasterInput" type="text" />
            </div>
        </div>

        <div id="bitMasterMsgDialog">
            <!--<p><span id="Span1" class="ui-icon ui-icon-alert" style="float: left; margin-right: .3em;"></span>-->
            <p>
                <span id="bitMasterMsgBoxIcon" class="bitMessageBoxQuestion" style="float: left;
                    margin-right: .3em;"></span>

                <span id="bitMasterMessage" style="float: left; margin-top: .3em;"></span>
            </p>
        </div>
    </div>
</body>
</html>
