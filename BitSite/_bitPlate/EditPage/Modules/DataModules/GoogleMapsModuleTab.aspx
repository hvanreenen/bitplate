<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoogleMapsModuleTab.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.DataModules.GoogleMapsModuleTab" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>

    <div id="tabPageGoogleMaps" class="bitTabPage">
        <div class="bitPageSettingsCollumnA">Start adres</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="De kaart wordt geladen met als middelpunt het start adres. Vul hier het begin adres in; komma gescheiden, volgens formaat (straat, (optioneel: postcode), plaats, land. Controleer eventueel in http://maps.google.com/ of het adres wordt gevonden.">info</span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <input type="text" data-field="Settings.InitialAddress" />
            <select data-field="Settings.InitialCountry">
                <option value="Nederland">Nederland</option>
                <option value="Belgium">België</option>
                <option value="Germany">Duitsland</option>
                <option value="France">Frankrijk</option>
                <option value="England">Engeland</option>
            </select>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Initiële zoom-niveau</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="De kaart wordt geladen met zoomniveau. Zoomniveau is later door gebruiker aan te passen. Kilometers zijn bij benadering en hangen mede af van kaart grootte op het scherm.">info</span>

        </div>
        <div class="bitPageSettingsCollumnC">
            <select data-field="Settings.InitialZoom">
                <option value="12">5 km</option>
                <option value="11">10 km</option>
                <option value="10">20 km</option>
                <option selected="selected" value="9">50 km</option>
                <option value="8">100 km</option>
                <option value="7">250 km</option>
            </select>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Afmeting kaart</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title="Kaart is een div met vaste afmetingen. De div is verder nog te op te maken in CSS met id #bitGoogleMap.">info</span>

        </div>
        <div class="bitPageSettingsCollumnC">
            Breedte:
            <input type="text" data-field="Settings.MapWidth" data-validation="Numeric" style="width: 100px" />px 
                Hoogte:
            <input type="text" data-field="Settings.MapHeight" data-validation="Numeric" style="width: 100px" />px 

        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
</body>
</html>
