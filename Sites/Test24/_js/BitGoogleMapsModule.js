var BITGOOGLEMAP = {
    key: '',
    map: null, //het kaart obejct
    geocoder: null, //hiermee kun je lat en long zoeken op basis van adres
    infoWindow: null, //infoballon op de kaart. Er is telkens maar 1 infoWindow beschikbaar, daarom globaal definieren
    icons: ["http://maps.google.com/mapfiles/ms/icons/red-dot.png", "http://maps.google.com/mapfiles/ms/icons/green-dot.png", "http://maps.google.com/mapfiles/ms/icons/blue-dot.png", "http://maps.google.com/mapfiles/ms/icons/yellow-dot.png", "http://maps.google.com/mapfiles/ms/icons/pink-dot.png", "http://maps.google.com/mapfiles/ms/icons/orange-dot.png", "http://maps.google.com/mapfiles/ms/icons/purple-dot.png", "http://maps.google.com/mapfiles/ms/icons/ltblue-dot.png"],
    categories: [], //elke categorie heeft eigen icoontje
    markers: [],
    //initPoint: null, //new google.maps.LatLng(),
    initLatitude: null,
    initLongitude: null,
    initAddress: 'Utrecht',
    initCountry: 'Nederland',
    initZoom: 9,
    infoBalloonFormat: '',
    sideBarRowFormat: '',
    navigationUrl: '',

    loadScript: function () {
        BITGOOGLEMAP.key = $('#hiddenGoogleMapsKey').val();
        var script = document.createElement("script");
        script.type = "text/javascript";
        script.src = "http://maps.googleapis.com/maps/api/js?key=" + BITGOOGLEMAP.key + " &sensor=false&callback=BITGOOGLEMAP.init";
        document.body.appendChild(script);
    },

    init: function () {
        
        BITGOOGLEMAP.initLatitude = $('#hiddenInitLatitude').val();
        BITGOOGLEMAP.initLongitude = $('#hiddenInitLongitude').val();
        BITGOOGLEMAP.initZoom = parseInt($('#hiddenInitZoom').val());
        BITGOOGLEMAP.datacollectionID = $('#hiddenDatacollectionID').val();
        BITGOOGLEMAP.infoBalloonFormat = $('#hiddenInfoBalloonFormat').val();
        BITGOOGLEMAP.sideBarRowFormat = $('#hiddenSideBarRowFormat').val();
        BITGOOGLEMAP.navigationUrl = $('#hiddenNavigationUrl').val();

        var initPoint = new google.maps.LatLng(BITGOOGLEMAP.initLatitude, BITGOOGLEMAP.initLongitude);
        var mapOptions = {
            center: initPoint,
            zoom: BITGOOGLEMAP.initZoom
        };
        BITGOOGLEMAP.map = new google.maps.Map(document.getElementById("bitGoogleMap"),
            mapOptions);
        //na zoomen en pannen worden bij behorende markers opnieuw gezocht.
        google.maps.event.addListener(BITGOOGLEMAP.map, 'zoom_changed', BITGOOGLEMAP.onMapChanged);
        google.maps.event.addListener(BITGOOGLEMAP.map, 'center_changed', BITGOOGLEMAP.onMapChanged);
        
        //om bugs met halve kaarten te voorkomen 
        google.maps.event.trigger(BITGOOGLEMAP.map, 'resize');
        //standaard objecten aanmaken
        BITGOOGLEMAP.geocoder = new google.maps.Geocoder();
        BITGOOGLEMAP.infoWindow = new google.maps.InfoWindow({ content: '' });
        google.maps.event.addListener(BITGOOGLEMAP.infoWindow, 'domready', function () {
            BITGOOGLEMAP.isLoaded = true;
        });
        $('#bitGooglemapsSearchDistance').val(BITGOOGLEMAP.initZoom);
        BITGOOGLEMAP.loadMarkers(initPoint, BITGOOGLEMAP.initZoom);
        //om grijs begin scherm te vookomen
        BITGOOGLEMAP.map.setCenter(initPoint);
    },

    onMapChanged: function () {
        if (BITGOOGLEMAP.isLoaded) {
            BITGOOGLEMAP.isLoaded = false;
            var zoomLevel = BITGOOGLEMAP.map.getZoom();
            $('#bitGooglemapsSearchDistance').val(zoomLevel);
            var point = BITGOOGLEMAP.map.getCenter();
            //na zoomen en pannen worden bij behorende markers opnieuw gezocht.
            BITGOOGLEMAP.loadMarkers(point, zoomLevel);
        }
    },


    getAddress: function () {
        var address = $('#bitGooglemapsSearchAddress').val();
        if (!address) address = BITGOOGLEMAP.initAddress;
        var country = $('#bitGooglemapsSearchCountry option:selected').val();
        if (!country) country = BITGOOGLEMAP.initCountry;
        address = address + "," + country;
        return address;
    },

    searchAddress: function () {
        BITGOOGLEMAP.isLoaded = false;
        var address = BITGOOGLEMAP.getAddress();

        var zoom = $('#bitGooglemapsSearchDistance').val();
        if (!zoom) zoom = BITGOOGLEMAP.initZoom;

        BITGOOGLEMAP.geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {

                var point = results[0].geometry.location;
                
                //BITGOOGLEMAP.isLoaded = false;
                BITGOOGLEMAP.map.setCenter(point);
                BITGOOGLEMAP.map.setZoom(parseInt(zoom));
                BITGOOGLEMAP.loadMarkers(point, zoom);
                //BITGOOGLEMAP.isLoaded = true;
            } else {
                alert('Adres niet gevonden: ' + status);
            }
        });
    },

    loadMarkers: function (point, zoom) {
        var geoPoint = new Object();
        geoPoint.Lat = point.lat();
        geoPoint.Long = point.lng();

        var parametersObject = {
            googleMapsKey: BITGOOGLEMAP.key,
            datacollectionID: BITGOOGLEMAP.datacollectionID,
            geoPoint: geoPoint,
            zoomLevel: zoom,
            navigationUrl: BITGOOGLEMAP.navigationUrl
        };
        var jsonString = JSON.stringify(parametersObject);
        //var jsonString = '{googleMapsKey: "' + BITGOOGLEMAP.key + '", datacollectionID: "' + BITGOOGLEMAP.datacollectionID + '", searchAddress: "' + address + '", zoomLevel: ' + parseInt(zoom) + '}';
        BITAJAX.dataServiceUrl = '/_bitAjaxServices/GoogleMapsService.aspx';
        BITAJAX.callWebServiceASync('GetPlacesAsJsonString', jsonString, BITGOOGLEMAP.onPlacesLoaded);
    },

    clearAllMarkers: function () {
        for (var i = 0; i < BITGOOGLEMAP.markers.length; i++) {
            BITGOOGLEMAP.markers[i].setMap(null);
        }
        BITGOOGLEMAP.markers = [];
    },

    onPlacesLoaded: function (data) {
        var jsonString = data.d;

        var locations = $.parseJSON(jsonString);
        //map.clearOverlays();
        //Eerst legen anders kom de markers dubbel op de kaart te staan.
        BITGOOGLEMAP.clearAllMarkers();
        points = '';
        $('#bitGooglemapsSideBar').html("");
        for (var row in locations) {
            var location = locations[row];
            var latitude = location.Latitude;
            var longitude = location.Longitude;

            var infoBalloon = BITGOOGLEMAP.formatInfoBalloon(location);
            var sideBarRow = BITGOOGLEMAP.formatSideBarRow(location);

            BITGOOGLEMAP.showAddress(latitude, longitude, infoBalloon, sideBarRow, location.Category, location.Title, location.ID);
        }
        BITSITESCRIPT.replaceLinksWithAjax();
        BITGOOGLEMAP.isLoaded = true;
    },

    formatInfoBalloon: function (location) {
        var format = BITGOOGLEMAP.infoBalloonFormat;
        return BITGOOGLEMAP.formatHtml(format, location);
    },

    formatSideBarRow: function (location) {
        var format = BITGOOGLEMAP.sideBarRowFormat;
        return BITGOOGLEMAP.formatHtml(format, location);
    },

    formatHtml: function (format, location) {
        var html = format;
        for (var prop in location) {
            html = html.replaceAll('{' + prop + '}', location[prop]);
        }
        //vervang links met ajax
        var tempWrapperDiv = document.createElement('div');
        $(tempWrapperDiv).html(html);
        $(tempWrapperDiv).find('a.showDetailsInModules').each(function (i) {
            var onClick = $(this).attr('onclick');
            $(this).attr('href', onClick);
            $(this).removeAttr('onclick');
        });
        html = $(tempWrapperDiv).html();
        return html;
    },

    showAddress: function (latitude, longitude, infoBalloon, sideBarRow, category, title, guid) {
        var point = new google.maps.LatLng(latitude, longitude);

        if (!point) {
            return;
        }
        else {
            var marker = new google.maps.Marker({
                position: point,
                map: BITGOOGLEMAP.map,
                title: title,
                note: infoBalloon
            });

            if (category) {
                BITGOOGLEMAP.categories[category] = BITGOOGLEMAP.icons[Object.keys(BITGOOGLEMAP.categories).length - 1];
                marker.setIcon(BITGOOGLEMAP.categories[category]);
            }

            BITGOOGLEMAP.markers[guid] = marker;

            google.maps.event.addListener(marker, 'click', function () {
                BITGOOGLEMAP.isLoaded = false;
                BITGOOGLEMAP.infoWindow.content = marker.note;
                BITGOOGLEMAP.infoWindow.open(BITGOOGLEMAP.map, marker);
                // wordt gezet in domready-event: BITGOOGLEMAP.isLoaded = true;
            });

            if (BITGOOGLEMAP.sideBarRowFormat.indexOf("<tr>") >= 0) {
                $('#bitGooglemapsSideBar').append(sideBarRow);
                $('#bitGooglemapsSideBar').find("table").delegate("tr.rows", "click", function () {
                    BITGOOGLEMAP.sideBarClick(guid);
                });
            }
            else{

                //// add it to the sidebar
                $('#bitGooglemapsSideBar').append("<a href='javascript:BITGOOGLEMAP.sideBarClick(&quot;" + guid + "&quot;);'>" + sideBarRow + "</a>");
            }
        }
    },

    sideBarClick: function (guid) {
        
        var marker = BITGOOGLEMAP.markers[guid];
        google.maps.event.trigger(marker, 'click');
    },

    checkEnter: function (e) {
        var characterCode; //literal character code will be stored in this variable

        if (e && e.which) {
            //if which property of event object is supported (NN4)
            e = e;
            characterCode = e.which;
            //character code is contained in NN4's which property
        }
        else {
            e = event;
            characterCode = e.
        Code;
            //character code is contained in IE's keyCode property
        }

        if (characterCode == 13) {
            //if generated character code is equal to ascii 13 (if enter key)
            searchOnMap();
        }
    },

    //nadat module config of html is bewaard, opnieuw init uitvoeren
    onModuleSaved: function () {
        BITGOOGLEMAP.init();
    }


};









$(document).ready(function () {
    BITGOOGLEMAP.loadScript(); //In Load script wordt google script geladen, pas hierna kan init runnen. Loadscript zorgt dat init() afgaat
    //$(document).on('endRequestHandler', function (sender, args) {
    //    //BITGOOGLEMAP.init();
    //});

});

//$(document).on('endRequestHandler', function (sender, args) {
//    //BITGOOGLEMAP.init();
//});

//$(document).on('onModuleSaved', new function () {
////    BITGOOGLEMAP.init();
//}); // Een event welke getriggerd wordt na de link replacement. Load script is dan niet meer nodig
//om BITGOOGLEMAP.onModuleSaved() af te laten gaan//wordt aangeroepen vanuit BITEDITPAGE na opslaan module//BITEDITPAGE.registerModuleConfigJsClass(BITGOOGLEMAP, "BITGOOGLEMAP");