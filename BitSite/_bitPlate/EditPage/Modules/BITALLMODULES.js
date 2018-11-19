//script voor initialiseren van en laden van data bij alle modules op een pagina
//registreer de init functie per moduel java script. bijv:
//BITALLMODULES.registerInitFunction("GroupListModule", BITDATAMODULES.loadData4Module);
//na laden van moduledependentscripts worden deze inits uitgevoerd.
var BITALLMODULES = {
    modules: [],

    registerInitFunction: function (moduleType, initFunction) {
        var functionAllReadyInCollection = false;
        for (var i = 0; i > BITALLMODULES.modules.length; i++) {
            if (BITALLMODULES.modules[i] == initFunction) {
                functionAllReadyInCollection = true;
                break;
            }
        }
        if (!functionAllReadyInCollection) {
            BITALLMODULES.modules[moduleType] = initFunction;
        }
    },

    loadAllModules: function () {
        $(".bitModule").each(function (i) {
            var moduleId = $(this).attr('id').replace('bitModule', '');
            var moduleType = $("#hiddenModuleType" + moduleId).val();
            BITALLMODULES.initModule(moduleType, moduleId);
        });
        $(document).trigger('PageReady');
    },

    initModule: function (moduleType, moduleId) {
        if (BITALLMODULES.modules[moduleType]) {
            var initFunction = BITALLMODULES.modules[moduleType];
            initFunction(moduleId);
        }
    },

    showDetailsInModules: function (dataid, modules) {
        $(".bitModule").each(function (i) {
            var moduleId = $(this).attr('id').replace('bitModule', '');
            if (modules.indexOf(moduleId) >= 0) {
                var moduleType = $("#hiddenModuleType" + moduleId).val();
                if (BITALLMODULES.modules[moduleType]) {
                    var initFunction = BITALLMODULES.modules[moduleType];
                    initFunction(moduleId, dataid);
                }
            }
        });
    },

    getServiceUrl: function (serviceName) {
        return location.protocol + "//" + location.host + "/_bitplate/_bitModules/DataModules/" + serviceName;
    }
};