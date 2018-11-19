<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pages_old.aspx.cs" Inherits="BitSite._bitPlate.Pages_old" %>

<!DOCTYPE html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>Pagina's</title>
<link href="_themes/bitplate/css/page_grid.css" rel="stylesheet" type="text/css" media="screen" />
<link href="_themes/bitplate/css/topMenu.css" rel="stylesheet" type="text/css" media="screen" />
<link href="_themes/bitplate/css/formElements.css" rel="stylesheet" type="text/css" media="screen" />
<link href="_themes/bitplate/css/bitPlateIcons.css" rel="stylesheet" type="text/css" media="screen" />
<script type="text/javascript" src="_js/jquery-1.7.2-vsdoc.js"></script>
<script type="text/javascript" src="_js/jquery-1.7.2.min.js"></script>
<!--<link type="text/css" href="_themes/bitplate/jquery-ui-1.8.22/css/ui-lightness/jquery-ui-1.8.22.custom.css" rel="stylesheet" />-->
<link type="text/css" href="_themes/bitplate/jquery-ui-1.8.22/css/custom-theme/jquery-ui-1.8.22.custom.css" rel="stylesheet" />
<script type="text/javascript" src="_themes/bitplate/js/jQuery.UI.forms.js"></script>
<script type="text/javascript" src="_themes/bitplate/js/jQuery.style.checkbox.js"></script>
<script type="text/javascript" src="_js/jquery-ui-1.8.21.custom.min.js"></script>
<script type="text/javascript" src="_js/jquery.ui.datepicker-nl.js"></script>
<script type="text/javascript" src="_js/JSON.js"></script>
<script type="text/javascript" src="_js/BITUTILS.js"></script>
<script type="text/javascript" src="_js/BITCONSTANTS.js"></script>
<script type="text/javascript" src="_js/BITAJAX.js"></script>
<script type="text/javascript" src="_js/prototypes/date.js"></script>
<script type="text/ecmascript" src="_js/prototypes/validation.js"></script>
<script type="text/ecmascript" src="_js/prototypes/databind.js"></script>
<script type="text/javascript" src="_js/BITPAGES_NEW.js"></script>

    <style>
        .ui-draggable-helper {
         list-style-type: none;
         margin-top: 2px; 
         margin-bottom: 2px; 
         padding: 5px;
         width: 180px;
         z-index: 9999;
        }

        #bitPageScriptList li {
            margin-bottom:  5px;
            padding: 5px;
            list-style-type: none;
        }
        
    </style>

</head>

<body>

<!-- page -->
<!-- topmenu -->
<div id="divTopMenu">
	<!-- wrap main menu -->
	<div id="divWrapperMainMenu">
		<ul class="bitUlMenu">
        	<li><a href="#">Dashboard</a></li>
            <li><a href="#">Help</a></li>
            <li><a href="#">Contact</a></li>
            <li><a href="#">FAQ</a></li>
        </ul>
        <ul class="bitUlMenu bitUlMenuRight">
        	<li><a href="#">Ingelogd als: *******@*********</a></li>
            <li><a href="#">Instellingen</a></li>
            <li><a href="#">Uitloggen</a></li>
        </ul>
    </div>
    <!-- .wrap main menu -->
</div>
<!-- .topmenu -->
<!-- sub topmenu -->
<div id="bitSubTopMenu">
	<div id="divWrapperSubmenu">
	<ul>
    	<li><a href="#" class="bitBack">back</a><div>Terug</div></li>
        <li><a href="#" class="bitAddPage">addpage</a><div>Nieuwe pagina</div></li>
        <li><a href="#" class="bitFolders">folders</a><div>Mappen</div></li>
    </ul>
    <div id="bitplateLogo"><img src="_themes/bitplate/images/BPCMS_logo.png" width="278" height="54" alt="bitplate logo"></div>
  </div>
</div>
<!-- .sub topmenu -->
<!-- main area -->
<div id="divWrapperMain">
	<div id="bitplateTitlePage">Pagina's</div>
    <br clear="all" />
    <table width="100%" border="0" cellspacing="0" cellpadding="0" id="bitTableDataSorter">
        <thead>
            <tr>
                <td class="bitTableIcon">&nbsp;</td>
                <td class="bitTableNameCollumn"><a href="#">Naam</a></td>
                <td class="bitTableNameTitle"><a href="#">Title</a></td>
                <td class="bitTableStatus"><a href="#">Status</a></td>
                <td class="bitTableActionButton">Bewerk</td>
                <td class="bitTableActionButton">Config</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableActionButton">Kopieer</td>
                <td class="bitTableActionButton">Live</td>
                <td class="bitTableActionButton">Aanmaakdatum</td>
            </tr>
          </thead>
          <tbody>
              <tr>
                <td class="bitTableIcon"><a href="#" data-field="TypeIconClass" class="[data-field]"></a></td>
                <td class="bitTableNameCollumn" data-field="Name"><a href="#">test</a></td>
                <td class="bitTableNameTitle" data-field="Title">&nbsp;</td>
                <td class="bitTableStatus">Actief &amp; Online</td>
                <td class="bitTableActionButton"><a href="#" class="bitEdit">bitEdit</a></td>
                <td class="bitTableActionButton"><a data-field="ID" data-title-field="Type" href="[data-field]" class="bitConfig">bitConfig</a></td>
                <td class="bitTableActionButton"><a data-field="ID" data-title-field="Type" href="[data-field]" class="bitDelete">del</a></td>
                <td class="bitTableActionButton"><a data-field="ID" data-title-field="Type" href="[data-field]" class="bitCopy">copy</a></td>
                <td class="bitTableActionButton"><a href="#" class="bitLive">live</a></td>
                <td class="bitTableActionButton">26-07-2012</td>
              </tr>
              
          </tbody>
    </table>
	<div class="bitEndTable"></div>
</div>
<!-- .main area -->
<!-- .page -->
<!-- forms UI -->
<!-- FORM page settings -->
<div id="bitPageSettings" title="Pagina eigenschappen">
   <!-- <div id="bitFormWindowResize"><span class="ui-state-default ui-state-default-arrow-4-diag"></span></div> -->
    <!-- ROW -->
    <div id="bitTabs">
	<ul>
		<li><a href="#tabs-1">Pagina instellingen</a></li>
		<li><a href="#tabs-2">Scripts & Pagina Header</a></li>
		<li><a href="#tabs-3">Zoek machine instellingen</a></li>
	</ul>
    
    <!-- TAB1: Home page -->
	<div id="tabs-1" class="bitTabs">
        <div class="bitPageSettingsCollumnA">Bestandsnaam</div>
        <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Voeg hier de bestandsnaam in van de pagina. De pagina krijgt zelf achtervoegsel .aspx">info</span></div>
        <div class="bitPageSettingsCollumnC"><input type="text" data-field="Name" data-validation="required" name="bitPageName" id="bitPageName" title="Voeg hier de bestandsnaam in van de pagina. De pagina krijgt zelf achtervoegsel .aspx" /></div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Titel</div>
        <div class="bitPageSettingsCollumnB"><span class="bitInfo" title="Voeg hier de titel in van de pagina. De title wordt weergegeven in de head van de pagina">info</span></div>
        <div class="bitPageSettingsCollumnC"><input type="text" data-field="Title" data-validation="required" name="bitPageName" id="bitPageTitle" title="Voeg hier de titel in van de pagina. De title wordt weergegeven in de head van de pagina" /></div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Layout template</div>
        <div class="bitPageSettingsCollumnB"></div>
        <div class="bitPageSettingsCollumnC">
            <select name="bitTemplateSelect" id="bitTemplateSelect" data-field="ID" data-text-field="Name">
                <option selected="selected" style="font-weight:700">Selecteer uw template:</option>
                <option value="template01">template01</option>
                <option value="template02">template02</option>
                <option value="template03">template03</option>
            </select>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Map</div>
        <div class="bitPageSettingsCollumnB"></div>
        <div class="bitPageSettingsCollumnC">
            <select id="selectPageFolders" data-field="Folder.ID" data-text-field="Folder.RelativePath">
                    </select>
            <!--<select name="bitMapSelect">
                <option selected="selected" style="font-weight:700">Selecteer map:</option>
                <option value="map01">map01</option>
                <option value="map02">map02</option>
                <option value="map03">map03</option>
            </select>-->
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Status</div>
        <div class="bitPageSettingsCollumnB"></div>
        <div class="bitPageSettingsCollumnC">
            <select name="bitMapSelect" id="bitMapSelect" data-field="Active">
                <option style="font-weight:700">Maak uw selectie:</option>
                <option value="1">Online</option>
                <option value="3">Offline</option>
                <option value="2">Online vanaf</option>
            </select>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitHide">
            <div class="bitPageSettingsCollumnA">Online vanaf</div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <input type="text" id="bitDatepickerFrom" type="date" data-field="DateFrom" class="bitDatepicker" /> Tot: <input type="date" data-field="DateTill" id="bitDatepickerTill"  class="bitDatepicker" />
            </div>
       </div>
        <br clear="all" />
            <div class="bitPageSettingsCollumnA"></div>
            <div class="bitPageSettingsCollumnB"></div>
            <div class="bitPageSettingsCollumnC">
                <span class="checkbox"><input type="checkbox" data-field="IsHomePage"  name="bitDefaultPage" id="bitDefaultPage" /><label for="bitDefaultPage" type="date" data-field="DateTill" title="Is standaardpagina?">Is standaardpagina?</label></span>
            </div>
            <br clear="all" />
	</div>
    <!-- .TAB1: Home page -->
    
    <!-- TAB2: Scripts & Pagina Header -->
	<div id="tabs-2" class="bitTabs">
    	<!-- ROW -->
    	<div class="bitPageSettingsCollumnA"></div>
        <div class="bitPageSettingsCollumnB"></div>
        <div class="bitPageSettingsCollumnC"><button id="bitButtonAddScript">Toevoegen</button></div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <!-- ROW -->
    	<div class="bitPageSettingsCollumnA">Scripts</div>
        <div class="bitPageSettingsCollumnB"></div>
        <div class="bitPageSettingsCollumnC">
            <ul id="bitPageScriptList" data-control-type="list">
                <li class="ui-state-default">
                    <input type="hidden" data-field="ID" />
                    <span data-field="Name"></span>
                    <span data-field="Type"></span>
                    <span class="ui-icon ui-icon-trash btn-remove-script" style="display: inline-block; float: right; cursor: pointer"></span>
                </li>
            </ul>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Extra header-inhoud</div>
        <div class="bitPageSettingsCollumnB"><a href="javascript:;" class="bitTextareaResize" id="bitEnlargeTextarea01">enlarge</a></div>
        <div class="bitPageSettingsCollumnC">
        	<textarea name="bitExtraHeaderContent" id="bitExtraHeaderContent" class="bitTextAreaPageSettings" data-field="HeadContent"></textarea>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
	</div>
    <!-- .TAB2: Scripts & Pagina Header -->
    
    <!-- TAB3: Zoek machine instellingen -->
	<div id="tabs-3" class="bitTabs">
		<!-- ROW -->
        <div class="bitPageSettingsCollumnA">Pagina beschrijving</div>
        <div class="bitPageSettingsCollumnB"><a href="javascript:;" class="bitTextareaResize"  id="bitEnlargeTextarea02"></a></div>
        <div class="bitPageSettingsCollumnC">
        	<textarea name="bitPageDescription" data-field="MetaDescription" id="bitPageDescription" class="bitTextAreaPageSettings"></textarea>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Sleutelwoorden</div>
        <div class="bitPageSettingsCollumnB"><a href="javascript:;" class="bitTextareaResize" id="bitEnlargeTextarea03">enlarge</a></div>
        <div class="bitPageSettingsCollumnC">
        	<textarea name="bitPageKeyWords" data-field="MetaKeywords" id="bitPageKeyWords" class="bitTextAreaPageSettings"></textarea>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA"></div>
        <div class="bitPageSettingsCollumnB"></div>
        <div class="bitPageSettingsCollumnC">
        	<span class="checkbox"><input type="checkbox" data-field="InSiteMap" onchange="javascript:BITPAGES2.inSideMapChange()" name="bitInSitemap" id="bitInSitemap" /><label for="bitInSitemap" title="In sitemap">In sitemap</label></span>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Wijzigingsfrequentie</div>
        <div class="bitPageSettingsCollumnB"></div>
        <div class="bitPageSettingsCollumnC">
            <select name="SiteMapChangeFreq" id="SiteMapChangeFreq" data-field="SiteMapChangeFreq">
                <option value="Always">Voortdurend</option>
                <option value="Hourly">Per uur</option>
                <option value="Daily">Dagelijks</option>
                <option value="Weekly">Wekelijks</option>
                <option value="Monthly">Maandelijks</option>
                <option value="Yearly">Jaarlijks</option>
                <option value="Never">Nooit</option>
            </select>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA">Prioriteit</div>
        <div class="bitPageSettingsCollumnB"></div>
        <div class="bitPageSettingsCollumnC">
            <select name="SiteMapPriority" id="SiteMapPriority">
                <option selected="selected" value="0.0">0,0</option>
                <option value="0.1">0,1</option>
                <option value="0.2">0,2</option>
                <option value="0.3">0,3</option>
                <option value="0.4">0,4</option>
                <option value="0.5">0,5</option>
                <option value="0.6">0,6</option>
                <option value="0.7">0,7</option>
                <option value="0.8">0,8</option>
                <option value="0.9">0,9</option>
                <option value="1.0">1,0</option>
                
            </select>
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div class="bitPageSettingsCollumnA"></div>
        <div class="bitPageSettingsCollumnB"></div>
        <div class="bitPageSettingsCollumnC">
        	<span class="checkbox"><input type="checkbox" name="bitDenySearchengines" id="bitDenySearchengines" /><label for="bitDenySearchengines" title="Zoekmachines weigeren">Zoekmachines weigeren</label></span>
        </div>
       <br clear="all" />
	</div>
    <!-- .TAB3: Zoek machine instellingen -->

</div>
<!-- .FORM page settings -->
<div id="bitScriptDialog" title="Scripts selecteren" style="display: none;">
    <ul id="bitScriptList" style="list-style-type: none" data-control-type="list">
        <li class="ui-state-default bitScript" style="margin-top: 2px; margin-bottom: 2px; padding: 5px">
            <input type="hidden" data-field="ID" />
            <span data-field="Name"></span>
            <span data-field="Type"></span>
            <span class="remove-script btn-remove-script"></span>
        </li>
    </ul>
</div>

<div id="bitScriptRemoveDialog" title="Script ontkoppelen." style="display: none;">
    Weet u zeker dat u dit script wilt los koppelen van deze pagina?
</div>

<div id="bitPageRemoveDialog" title="Pagina verwijderen." style="display: none;">
Weet u zeker dat u deze pagina wilt verwijderen?
</div>
<!-- .forms UI -->
</body>
</html>
