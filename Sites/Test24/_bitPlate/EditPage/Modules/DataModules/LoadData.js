var hash;
var id;
var documentReady = false;
 
//om backbrowser button te kunnen gebruiken
////maakt gebruik van timer
//setInterval(function () {
//    if (location.hash != hash && documentReady) {
//        hash = location.hash;
//        if (hash) {
//            id = hash.replace("#", "");
//        }
//        else {
//            id = null;
//        }
       
//        //if (id) {
//        //    DATA.loadSubGroupsAndDataBind("31cb3827-bea7-4b1f-9381-f164e0b36a02", id, "testGroupsTable");
//        //    DATA.loadGroupDetailsAndDataBind(id, "groupDetails");
//        //    DATA.loadItemsAndDataBind("31cb3827-bea7-4b1f-9381-f164e0b36a02", id, "testItems");
//        //}
//        //else {
//        //    DATA.loadParentGroupsAndDataBind("31cb3827-bea7-4b1f-9381-f164e0b36a02", "testGroupsTable");
//        //    DATA.loadItemsAndDataBind("31cb3827-bea7-4b1f-9381-f164e0b36a02", null, "testItems");
//        //}
//    }
//}, 100);

$(document).ready(function () {
    //alert('hi');
    documentReady = true;
    var queryString = getQueryString();
    if (queryString && queryString["search"]) {
        BITSEARCHMODULES.find();
    }
    
});

function drillDown(id) {
    location.href = "#" + id;
}

function itemDetails(id) {
    location.href = "#" + id;
}


function getQueryString() {
    var query = window.location.search;

    query = query.replace('?', '');
    query = query.replace(')', '');

    if (query == '') return null;

    var queryString = new Array();
    var nameValues = query.split('&');
    for (i = 0; (i < nameValues.length) ; i++) {
        var nameValue = nameValues[i];
        var name = nameValue;
        var value = nameValue;
        if (nameValue.indexOf('=') > 0) {
            name = nameValue.split('=')[0];
            value = nameValue.split('=')[1];
        }
        queryString[name] = value;
    }
    return queryString;
}