<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigCurrentUser.aspx.cs" Inherits="BitSite._bitPlate.Dialogs.ConfigCurrentUser" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <div class="bitTabs">
        <ul>
            <li><a href="#tabPageDetails1">Algemene gegevens</a></li>
            <li><a href="#tabPageDetails2">Adresgegevens</a></li>
            <li><a href="#tabPageDetails3">Voorkeuren</a></li>
            <!--<li><a href="#tabPageDetails4">Rechten</a></li>-->
            <li><a href="#tabPageDetails5">Wachtwoord wijzigen</a></li>
        </ul>
        <!-- TAB1 -->
        <div id="tabPageDetails1" class="bitTabPage">
            <div class="bitPageSettingsCollumnA">Email</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input data-field="Email" type="text" data-validation="required" />
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Voornaam</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input data-field="ForeName" type="text" />
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Tussenvoegsels</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input data-field="NamePrefix" type="text" />
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Achternaam</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input data-field="Name" type="text" />
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Geslacht</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <div>
                    <input type="radio" name="Gender" data-field="Gender" value="1" />Man
                </div>
                <div>
                    <input type="radio" name="Gender" data-field="Gender" value="2" />Vrouw
                </div>
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Geboortedatum</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input data-field="BirthDate" type="date" />
            </div>
            <br clear="all" />


        </div>
        <!--TAB2-->
        <!--TAB2-->
        <!--TAB2-->
        <div id="tabPageDetails2" class="bitTabPage">
            <div class="bitPageSettingsCollumnA">Adres</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input data-field="Address" type="text" />
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Postcode</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input data-field="Postalcode" type="text" />
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Plaats</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input data-field="City" type="text" />
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Land</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input data-field="Country" type="text" />
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Telefoon</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input data-field="Telephone" type="text" />
            </div>
            <br clear="all" />
            <!--<div class="bitPageSettingsCollumnA">Mobiel</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input data-field="MobilePhone" type="text" />
            </div>
            <br clear="all" /> -->
        </div>
        <!--TAB3-->
        <!--TAB3-->
        <!--TAB3-->
        <div id="tabPageDetails3" class="bitTabPage">
            <div class="bitPageSettingsCollumnA">Thema:</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <select onchange="BITAUTH.changeTheme()" data-field="Theme" runat="server" id="selectThema"></select>
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Taal:</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <select id="select1">
                    <option value="NL" selected="selected">Nederlands</option>
                    <option value="EN" disabled="disabled">English</option>
                </select>
            </div>
            <br clear="all" />
        </div>
        <!--TAB4-->
        <!--TAB4: WW VERSTUREN-->
        <!--TAB4-->
        <div id="tabPageDetails5" class="bitTabPage">
            <div class="bitPageSettingsCollumnA">Huidige wachtwoord:</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input type="password" id="bitUserCurrentPassword" />
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Nieuwe wachtwoord:</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input type="password" id="bitNewUserPassword" />
            </div>
            <br clear="all" />
            <div class="bitPageSettingsCollumnA">Voer het wachtwoord nogmaals in:</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input type="password" id="bitNewUserPasswordConfirm" />
            </div>
            <br clear="all" />
            <!--<div class="bitPageSettingsCollumnA"></div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <div class="button" onclick="BITAUTH.changeUserPassword()">Wijzig</div>
            </div>
            <br clear="all" />-->
        </div>
    </div>
</body>
</html>
