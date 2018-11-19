$(document).ready(function () {
    if ($('#bitAjaxLoader').initDialog) {
        $('#bitAjaxLoader').initDialog(function () {
        }, { height: 75, width: 200 }, true, {});
        //BITAJAX.isLoaderActive = true;
    }
});

function HistoryObject() {
    //public declarations
    this.hash = hash;
    this.url = url
    this.parameters = parameters;
    this.successFunction = successFunction;
    this.errorFunction = errorFunction;

    //private properties
    var hash = null;
    var url = null;
    var parameters = null;
    var successFunction = null;
    var errorFunction = null;

    $(window).on('hashchange', BITAJAX.historyHashChange);
}

var BITAJAX = {
    rootFolderServices: "bitAjaxServices/",
    dataServiceUrl: null,
    isLoaderActive: true,      //Is de loader actief? True = ja de loader toont; false = nee de loader toont niet.
    loaderTimeOut: 1000,        //De loader heeft een timeout om te voorkomen dat de loader bij bijvoorbeeld 3 request toont en verbergt. Dit levert een flikkerend effect op. 
    //Nu kan de server binnen deze timeout een nieuw request starten. De currentTimeOut wordt gereset naar de loaderTimeOut waarde. (Standaard na elk request wacht de loader nog een extra seconden).
    currentTimeOut: null,
    isBuzzy: false,

    isHistorySupported: false,
    orginalUrl: null,
    historyArray: new Array(),
    lastHash: '#-1',

    historyHashChange: function () {
        //laatste call niet nog een keer aanroepen
        if (window.location.hash != BITAJAX.lastHash) {
            if (BITAJAX.historyArray[window.location.hash]) {
                var historyObj = BITAJAX.historyArray[window.location.hash];
                //BITAJAX.historyArray = BITAJAX.historyArray.slice($.inArray(window.location.hash, BITAJAX.historyArray), 1);
                BITAJAX.callWebServiceASync(historyObj.url, historyObj.parameters, historyObj.successFunction, historyObj.errorFunction);
            }
        }
    },

    callWebService2: function (dataServiceUrl, remoteFunction, parameters, successFunction) {
        if (BITAJAX.isLoaderActive) {
            $('#bitAjaxLoaderContainer').fadeIn();
        }
        BITAJAX.isBuzzy = true;
        BITAJAX.dataServiceUrl = BITAJAX.rootFolderServices + dataServiceUrl;
        BITAJAX.callWebService(remoteFunction, parameters, function (data) {
            if (BITAJAX.isLoaderActive) {
                BITAJAX.isBuzzy = false;
                BITAJAX.setLoaderTimeOut();
            }
            if (successFunction) {
                successFunction(data);
            }
        });
    },
    callWebServiceASync: function (remoteFunction, parameters, successFunction, errorFunction, button) {
        if (button) {
            var orginalButtonText = $(button).html();
            $(button).addClass('submit-loader');
            $(button).html('waiting');

            var orginalSuccessFunction = successFunction;
            successFunction = function (data) {
                if (orginalSuccessFunction) orginalSuccessFunction(data);
                $(button).removeClass('submit-loader');
                $(button).html(orginalButtonText);
            }

            var orginalErrorFunction = errorFunction;
            errorFunction = function () {
                if (orginalErrorFunction) orginalErrorFunction();
                $(button).removeClass('submit-loader');
                $(button).html(orginalButtonText);
            }
        }

        BITAJAX.callWebService(remoteFunction, parameters, successFunction, errorFunction, true);
    },

    callWebService: function (remoteFunction, parameters, successFunction, errorFunction, async) {
        BITAJAX.isBuzzy = true;
        if (BITAJAX.isHistorySupported && remoteFunction.indexOf('#historyFunction') !== -1) {
            if (!BITAJAX.orginalUrl) {
                BITAJAX.orginalUrl = window.location.href.replace(location.hash, "")
            }
            var objLenght = getObjectLength(BITAJAX.historyArray);
            if (!BITAJAX.historyArray['#' + objLenght]) {
                var historyObj = new HistoryObject();
                historyObj.hash = objLenght;
                historyObj.url = remoteFunction.replace('#historyFunction', '');
                historyObj.parameters = parameters;
                historyObj.successFunction = successFunction;
                historyObj.errorFunction = errorFunction;

                BITAJAX.historyArray['#' + historyObj.hash] = historyObj;
                window.location.hash = historyObj.hash;
                //voorkom dat cal twee keer afgaat daar last hash te zetten en in event te checken of dit de laatste was.
                //laatste niet nog een keer aanroepen
                BITAJAX.lastHash = historyObj.hash;
            }
        }


        if (BITAJAX.isLoaderActive) {
            $('#bitAjaxLoaderContainer').fadeIn();
        }

        errorFunction = (errorFunction == undefined) ? function () { } : errorFunction;
        async = (async == undefined) ? false : async;
        $.ajax({
            type: "POST",
            url: BITAJAX.dataServiceUrl + "/" + remoteFunction,
            contentType: "application/json; charset=utf-8",
            data: parameters,
            dataType: "json",
            async: async,
            success: function (data) {
                if (BITAJAX.isLoaderActive) {
                    isBuzzy = false;
                    BITAJAX.setLoaderTimeOut();
                }
                if (successFunction) {
                    successFunction(data);
                }
            },
            error: function (xhr, msg, thrownError) {
                if (BITAJAX.isLoaderActive) {
                    BITAJAX.setLoaderTimeOut();
                }
                //var err = eval("(" + xhr.responseText + ")");
                var posStart = xhr.responseText.indexOf("<title>");
                var posEnd = xhr.responseText.indexOf("</title>");
                if (posStart > 0) {
                    var title = xhr.responseText.substring(posStart + 7, posEnd);
                    //alert("ERROR: " + title);
                    $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.ERROR, "<b>Er is een fout opgetreden: </b>" + title, '', errorFunction);
                }
                else {
                    //console.log(xhr);
                    //console.log(msg);
                    //console.log(thrownError);
                    var errObj = (new Function("return " + xhr.responseText))()

                    if (errObj && errObj.ExceptionType == "BitSite._bitPlate.SessionLostException") {
                        errorFunction = function () {
                            window.location.href = "/_bitplate/Login.aspx?SessionExpired=true";
                        };
                    }

                    if (errObj) $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.ERROR, "<b>Er is een fout opgetreden: </b>" + errObj.Message, '', errorFunction);

                    /*
                                $('#bitErrorDialog').dialog({
                                    width: 420,
                                    Height: 160,
                                    modal: false,
                                    buttons: {
                                        "Sluiten": function () {
                                            $(this).dialog('close');
                                        },
                                    }
                                });
                                //alert("ERROR: " + errObj.Message);
                                */
                }
            }
        });
    },

    load: function (url, parameters) {
        //Deze functie heeft geen ondersteuning voor de ajaxloader momenteel.
        var result;
        BITAJAX.isBuzzy = true;
        parameters = parameters == undefined ? '' : parameters;
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            data: parameters,
            async: false,
            success: function (data) {
                result = data;
                BITAJAX.isBuzzy = false;
            }
        });
        return result;
    },

    setLoaderTimeOut: function () {
        if (BITAJAX.currentTimeOut != null) {
            window.clearTimeout(BITAJAX.currentTimeOut);    //Clear activeLoaderTimeOut
        }
        BITAJAX.currentTimeOut = setTimeout(function () {
            $('#bitAjaxLoaderContainer').fadeOut();
            $(document).trigger('AjaxReady', ['Custom', 'Event']); //Javascript event wordt getriggert als ajaxloader completed is. 
            BITAJAX.isBuzzy = false;
        }, BITAJAX.loaderTimeOut);
    }
};

function getObjectLength(obj) {
    var length = 0;
    for (var p in obj) {
        if (obj.hasOwnProperty(p)) {
            length++;
        }
    }
    return length;
}




/**********************************************************Vorige versie******************************************************/


//$(document).ready(function () {
//    if ($('#bitAjaxLoader').initDialog) {
//        $('#bitAjaxLoader').initDialog(function () {
//        }, { height: 75, width: 200 }, true, {});
//        //BITAJAX.isLoaderActive = true;
//    }
//});


//var BITAJAX = {
//    rootFolderServices: "bitAjaxServices/",
//    dataServiceUrl: null,
//    isLoaderActive: true,      //Is de loader actief? True = ja de loader toont; false = nee de loader toont niet.
//    loaderTimeOut: 1000,        //De loader heeft een timeout om te voorkomen dat de loader bij bijvoorbeeld 3 request toont en verbergt. Dit levert een flikkerend effect op. 
//    //Nu kan de server binnen deze timeout een nieuw request starten. De currentTimeOut wordt gereset naar de loaderTimeOut waarde. (Standaard na elk request wacht de loader nog een extra seconden).
//    currentTimeOut: null,    
//    isBuzzy: false,


//    callWebService2: function (dataServiceUrl, remoteFunction, parameters, successFunction) {
//        if (BITAJAX.isLoaderActive) {
//            $('#bitAjaxLoaderContainer').fadeIn();
//        }
//        BITAJAX.isBuzzy = true;
//        BITAJAX.dataServiceUrl = BITAJAX.rootFolderServices + dataServiceUrl;
//        BITAJAX.callWebService(remoteFunction, parameters, function (data) {
//            if (BITAJAX.isLoaderActive) {
//                BITAJAX.setLoaderTimeOut();
//            }
//            if (successFunction) {
//                successFunction(data);
//            }
//        });
//    },
//    callWebServiceASync: function (remoteFunction, parameters, successFunction, errorFunction, button) {
//        if (button) {
//            var orginalButtonText = $(button).html();
//            $(button).addClass('submit-loader');
//            $(button).html('waiting');

//            var orginalSuccessFunction = successFunction;
//            successFunction = function (data) {
//                if (orginalSuccessFunction) orginalSuccessFunction(data);
//                $(button).removeClass('submit-loader');
//                $(button).html(orginalButtonText);
//            }

//            var orginalErrorFunction = errorFunction;
//            errorFunction = function () {
//                if (orginalErrorFunction) orginalErrorFunction();
//                $(button).removeClass('submit-loader');
//                $(button).html(orginalButtonText);
//            }
//        }

//        BITAJAX.callWebService(remoteFunction, parameters, successFunction, errorFunction, true);
//    },

//    callWebService: function (remoteFunction, parameters, successFunction, errorFunction, async) {
//        if (BITAJAX.isLoaderActive) {
//            $('#bitAjaxLoaderContainer').fadeIn();
//        }
//        BITAJAX.isBuzzy = true;
//        errorFunction = (errorFunction == undefined) ? function () { } : errorFunction;
//        async = (async == undefined) ? false : async;
//        $.ajax({
//            type: "POST",
//            url: BITAJAX.dataServiceUrl + "/" + remoteFunction,
//            contentType: "application/json; charset=utf-8",
//            data: parameters,
//            dataType: "json",
//            async: async,
//            success: function (data) {
//                if (BITAJAX.isLoaderActive) {
//                    isBuzzy = true;
//                    BITAJAX.setLoaderTimeOut();
//                }
//                if (successFunction) {
//                    successFunction(data);
//                }
//            },
//            error: function (xhr, msg, thrownError) {
//                if (BITAJAX.isLoaderActive) {
//                    BITAJAX.setLoaderTimeOut();
//                }
//                //var err = eval("(" + xhr.responseText + ")");
//                var posStart = xhr.responseText.indexOf("<title>");
//                var posEnd = xhr.responseText.indexOf("</title>");
//                if (posStart > 0) {
//                    var title = xhr.responseText.substring(posStart + 7, posEnd);
//                    //alert("ERROR: " + title);
//                    $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.ERROR, "<b>Er is een fout opgetreden: </b>" + title, '', errorFunction);
//                }
//                else {
//                    //console.log(xhr);
//                    //console.log(msg);
//                    //console.log(thrownError);
//                    var errObj = (new Function("return " + xhr.responseText))()
                    
//                    if (errObj && errObj.ExceptionType == "BitSite._bitPlate.SessionLostException") {
//                        errorFunction = function () {
//                            window.location.href = "/_bitplate/Login.aspx?SessionExpired=true";
//                        };
//                    }

//                    if (errObj) $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.ERROR, "<b>Er is een fout opgetreden: </b>" + errObj.Message, '', errorFunction);

//                    /*
//                                $('#bitErrorDialog').dialog({
//                                    width: 420,
//                                    Height: 160,
//                                    modal: false,
//                                    buttons: {
//                                        "Sluiten": function () {
//                                            $(this).dialog('close');
//                                        },
//                                    }
//                                });
//                                //alert("ERROR: " + errObj.Message);
//                                */
//                }
//            }
//        });
//    },

//    load: function (url, parameters) {
//        //Deze functie heeft geen ondersteuning voor de ajaxloader momenteel.
//        var result;
//        BITAJAX.isBuzzy = true;
//        parameters = parameters == undefined ? '' : parameters;
//        $.ajax({
//            type: "GET",
//            url: url,
//            contentType: "application/json; charset=utf-8",
//            data: parameters,
//            async: false,
//            success: function (data) {
//                result = data;
//                BITAJAX.isBuzzy = false; 
//            }
//        });
//        return result;
//    },

//    setLoaderTimeOut: function () {
//        if (BITAJAX.currentTimeOut != null) {
//            window.clearTimeout(BITAJAX.currentTimeOut);    //Clear activeLoaderTimeOut
//        }
//        BITAJAX.currentTimeOut = setTimeout(function () {
//            $('#bitAjaxLoaderContainer').fadeOut();
//            $(document).trigger('AjaxReady', ['Custom', 'Event']); //Javascript event wordt getriggert als ajaxloader completed is. 
//            BITAJAX.isBuzzy = false;
//        }, BITAJAX.loaderTimeOut);
//    }
//};





