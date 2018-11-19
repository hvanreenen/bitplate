var BITAJAX = {
    rootFolderServices: "bitAjaxServices/",
    dataServiceUrl: null,

    callWebServiceASync: function (remoteFunction, parameters, successFunction, errorFunction) {
        BITAJAX.callWebService(remoteFunction, parameters, successFunction, errorFunction, true);
    },

    callWebService: function (remoteFunction, parameters, successFunction, errorFunction, async) {

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

                if (successFunction) {
                    successFunction(data);
                }
            },
            error: function (xhr, msg, thrownError) {
                if (errorFunction) {
                    errorFunction();
                }
            }
        });
    }

 

};





