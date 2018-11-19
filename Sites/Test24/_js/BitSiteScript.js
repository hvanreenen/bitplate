String.prototype.replaceAll = function (search, replacement) {
    return this.split(search).join(replacement);
};

var navigationTypeEnum = { NavigateToPage: 0, ShowDetailsInModules: 1 };

$(document).ready(function () {
    var action = $('form').attr('action');
    $('form').attr('action', window.location);
    $('img[src=""]').hide();
    $("img[src='']").hide();

   
    $(window).on('hashchange', BITSITESCRIPT.onBrowserHistoryChanged);

    BITSITESCRIPT.replaceLinksWithAjax();

    BITSITESCRIPT.initializePostableForms();
    //Voor Treemodule want die heeft al ander event op hyperlinks, deze pas vervangen aan het eind
    $(document).trigger('bitplateLoaded'); // Event wat getriggerd wordt na dat de linkjes zijn vervangen. 

});
//BITSITEOBJECT
var BITSITESCRIPT = {
    pageId: null,
    historyArray: {},
    lastHash: '',

    //voor datamodules met drilldownlinks
    replaceLinksWithAjax: function (moduleId) {
        var count = 0;
        if (moduleId) {
            $("#bitModule" + moduleId).find('a.showDetailsInModules').each(function (i) {
                //var onClick = $(this).attr('onclick');
                //$(this).attr('href', onClick);
                $(this).attr('href', 'javascript:void(0)');
                //$(this).removeAttr('onclick');
                count++;
            });
        }
        else {
            $('a.showDetailsInModules').each(function (i) {
                //var onClick = $(this).attr('onclick');
                $(this).attr('href', 'javascript:void(0)');
                //$(this).attr('href', onClick);
                //$(this).removeAttr('onclick');
                count++;
            });
        }
    },

    //zorg dat back knop in browser werkt met ajax reload data
    onBrowserHistoryChanged: function () {
        if (location.hash == "") {
            location.href = location.href;
        }
        else if (location.hash != BITSITESCRIPT.lastHash) {
            var dataid = location.hash;
            var parametersObject = BITSITESCRIPT.historyArray[dataid];
            if (parametersObject) {
                BITSITESCRIPT.reloadModulesOnSamePage(parametersObject.refreshModulesString, parametersObject.parameters);
            }
        }
    },

    reloadModulesOnSamePage: function (refreshModulesString, parameters) {
        if (parameters && parameters.dataid) {
            BITSITESCRIPT.historyArray['#' + parameters.dataid] = { refreshModulesString: refreshModulesString, parameters: parameters };
            location.hash = parameters.dataid;
            //voorkom dat event al afgaat door laatste hash te zetten en daarop te checken in onBrowserHistoryChanged()
            BITSITESCRIPT.lastHash = location.hash;
        }
        if (refreshModulesString == undefined) refreshModulesString = "";
        var refreshModules = refreshModulesString.split(',');

        for (var i in refreshModules) {
            var refreshModuleId = refreshModules[i];
            if (refreshModuleId && refreshModuleId != "") {

                var parametersObject = { pageid: BITSITESCRIPT.pageId, id: refreshModuleId, parameters: parameters };
                var jsonstring = JSON.stringify(parametersObject);
                BITAJAX.dataServiceUrl = "/_bitAjaxServices/ModuleService.aspx";
                BITAJAX.callWebService("ReloadModule", jsonstring, function (data) {
                    $('#bitModule' + refreshModuleId).replaceWith(data.d);
                    if (parent && window.location != window.parent.location) {
                        //vanuit iframe geladen
                        parent.BITEDITPAGE.attachModuleCommands(refreshModuleId);
                        //parent.BITEDITPAGE.attachDraggable(refreshModuleId);
                    }
                    BITSITESCRIPT.replaceLinksWithAjax(refreshModuleId);
                });
            }
        }
        //Voor Treemodule want die heeft al ander event op hyperlinks, deze pas vervangen aan het eind
        $(document).trigger('bitplateLoaded'); // Event wat getriggerd wordt na dat de linkjes zijn vervangen. 
    },

    ///////////////////////////
    //FROM MODULES
    ///////////////////////////
    initializePostableForms: function() {
        $('.bitModule form').each(function () {
            var frm = this;
            var moduleId = $(frm).find('[name="hiddenModuleID"]').val();
            var options = {
                iframeID: 'iframe-post-form',       // Iframe ID.
                json: true,                        // Parse server response as a json object.
                post: function () {
                    return BITSITESCRIPT.validatePostableModule(frm);
                },               // Form onsubmit.
                complete: function (data) {
                    BITSITESCRIPT.handleIframePostComplete(data, moduleId);
                }   // After response from the server has been received.
            }
            $(frm).iframePostForm(options);
        });
    },

    setCurrentButtonAction: function(action, moduleId, validationRequired) {
        //bewaar op welke submit button was geklikt
        $("#hiddenCurrentSubmitAction" + moduleId.replace(/-/g, '')).val(action);
        $("#hiddenValidationRequired" + moduleId.replace(/-/g, '')).val(validationRequired);
    },

    //deze aanroep wordt gedaan als je op een link-button hebt geklikt
    submitPostableModule: function(moduleId) {
        var frm = $("#form" + moduleId.replace(/-/g, ''))[0];
        //if (validatePostableModule(frm)) {
        //frm.submit();
        //}
        var options = {
            iframeID: 'iframe-post-form',       // Iframe ID.
            json: true,                        // Parse server response as a json object.
            post: function () {
                return BITSITESCRIPT.validatePostableModule(frm);
            },               
            complete: function (data) {
                BITSITESCRIPT.handleIframePostComplete(data, moduleId);
            }   
        }
        $(frm).iframePostForm(options);
        frm.submit();
    },

    //deze aanroep wordt gedaan als form wordt gesubmit
    validatePostableModule: function(frm) {
        var valid = true;
        var moduleId = $(frm).find('[name="hiddenModuleID"]').val();
        var isValidationRequired = ($("#hiddenValidationRequired" + moduleId.replace(/-/g, '')).val() == "true");
        if (isValidationRequired ) {
            valid = $(frm).validate();
        }
    
        if (valid) {
            //frm.submit doet iframe.post.js zelf
        }
        else {
            return false;
        }
    },

    handleIframePostComplete: function(postResult, moduleId) {
        if (postResult.Success && postResult.NavigationType == navigationTypeEnum.NavigateToPage) {
            window.location.href = postResult.NavigationUrl;
        }

        else if (postResult.Success && postResult.NavigationType == navigationTypeEnum.ShowDetailsInModules) {
            $('#bitModule' + moduleId).replaceWith(postResult.HtmlResult);
            //opnieuw initialiseren
            BITSITESCRIPT.initializePostableForms();
            if (parent && window.location != window.parent.location) {
                //vanuit iframe geladen
                parent.BITEDITPAGE.attachModuleCommands(moduleId);
            }
            BITSITESCRIPT.replaceLinksWithAjax(moduleId);
        }

        else {
            $('#bitErrorMessage' + moduleId.replace(/-/g, '')).html(postResult.ErrorMessage);
            var errorTemplate = $('#bitErrorTemplate' + moduleId.replace(/-/g, ''));
            $(errorTemplate).show();
        }
    },


    ////SEARCHRESULT- MODULE + ITEMLISTMODULE
    doPaging: function(moduleId, pageNumber, totalResults, parameters) {
        //in parameters staat de huidige selectie
        var parametersObject = { pageid: BITSITESCRIPT.pageId, moduleid: moduleId, pageNumber: pageNumber, totalResults: totalResults, parameters: parameters };

        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitAjaxServices/ModuleService.aspx";
        BITAJAX.callWebService("DoPaging", jsonstring, function (data) {
            $('#bitModule' + moduleId).replaceWith(data.d);
            if (parent && window.location != window.parent.location) {
                //vanuit iframe geladen
                parent.BITEDITPAGE.attachModuleCommands(moduleId);
            }
            BITSITESCRIPT.replaceLinksWithAjax(moduleId);
        });
    },

    ////SEARCHMODULE
    searchSite: function(moduleId) {
        //call from searchModule
        moduleId = moduleId.replace(/-/g, "");
        var searchString = $("#bitTextBoxSearch" + moduleId).val();
        var searchDataCollectionId = $("[type=radio][name=bitRadioDataCollection]:checked").val();
        var navigationType = $("#hiddenModuleNavigationType" + moduleId).val();
        if (navigationType == "NavigateToPage") {

            var redirectUrl = $("#hiddenNavigationUrl" + moduleId).val();
            redirectUrl += "?search=" + searchString;
            if (searchDataCollectionId) {
                redirectUrl += "&datacol=" + searchDataCollectionId;
            }
            location.href = redirectUrl;
        }
        else {
            var refreshModulesString = $("#hiddenRefreshModules" + moduleId).val();
            var parameters = {};
            parameters["bitTextBoxSearch"] = searchString;
            parameters["bitRadioDataCollection"] = searchDataCollectionId;
            BITSITESCRIPT.reloadModulesOnSamePage(refreshModulesString, parameters);
        }
    },

    checkEnterKeyPress: function (e, moduleId, successFunction) {
        var characterCode; //literal character code will be stored in this variable

        if (event && event.which) {
            //other browsers
            characterCode = event.which;
        }
        else {
            //IE
            characterCode = event.keyCode;
        }

        if (characterCode == 13) //enter key
        {
            successFunction(moduleId);
        }
    },
    sortDirection: 'ASC',
    sortColumn: null,
    doSort: function (moduleId, column, pageNumber, totalResults, parameters) {
        if (!BITSITESCRIPT.sortColumn || BITSITESCRIPT.sortColumn != column) {
            BITSITESCRIPT.sortDirection = 'ASC';
            BITSITESCRIPT.sortColumn = column;
        }
        else {
            BITSITESCRIPT.sortDirection = (BITSITESCRIPT.sortDirection == 'ASC') ? 'DESC' : 'ASC';
        }
        var parametersObject = { pageid: BITSITESCRIPT.pageId, moduleid: moduleId, column: column, sortDirection: BITSITESCRIPT.sortDirection, pageNumber: pageNumber, totalResults: totalResults, parameters: parameters };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitAjaxServices/ModuleService.aspx";
        BITAJAX.callWebService("DoSort", jsonstring, function (data) {
            $('#bitModule' + moduleId).replaceWith(data.d);
            if (parent && window.location != window.parent.location) {
                //vanuit iframe geladen
                parent.BITEDITPAGE.attachModuleCommands(moduleId);
            }
            BITSITESCRIPT.replaceLinksWithAjax(moduleId);
        });
    }
}