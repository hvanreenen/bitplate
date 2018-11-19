//script voor drag drop functionaliteit wanneer pagina is geladen in iframe
//andere helft van drag drop functionaliteit staat in BITEDITPAGE.js

$(document).ready(function () {
    if(parent && window.location != window.parent.location){
        //vanuit iframe geladen
        parent.BITEDITPAGE.pageId= BITSITESCRIPT.pageId;
        parent.BITEDITPAGE.language= BITSITESCRIPT.pageLanguage;
        $('.bitContainer').sortable({
            placeholder: 'bitContainerPlaceholder',
            connectWith: '.bitContainer',
            handle: '.moduleMoveGrip',
    
            update: function (event, ui) {
                if (this === ui.item.parent()[0]) {
                    var item = ui.item;
                    var isNew = ($(item).is('.moduleToDrag'));
                    var moduleType = $(item).attr('data-module-type');
                    var id = $(item).attr('id');
                    var containerName = $(this).attr('id').replace("bitContainer", "");
                    var index = $(this).children().index(ui.item[0]);


                    BITAJAX.dataServiceUrl = "/_bitplate/EditPage/ModuleService2.aspx";

                    if (isNew) {
                        var parametersObject = { type: moduleType, pageid: parent.BITEDITPAGE.pageId, containername: containerName, sortorder: index, newsletterid: parent.BITEDITPAGE.newsletterId };
                        var jsonstring = JSON.stringify(parametersObject);
                        BITAJAX.callWebService("AddNewModule", jsonstring, function (data) {
                            item.replaceWith(data.d[0]);
                            BITEDITPAGE.attachModuleCommands();
                            BITEDITPAGE.addModuleScriptsToHead(data.d[1]);
                            BITEDITPAGE.fillModulesCheckBoxList(parent.BITEDITPAGE.pageId);
                        });
                    }
                    else {
                        //sleep van ene container naar andere of van ene positie naar andere
                        var parametersObject = { moduleid: id, pageid: parent.BITEDITPAGE.pageId, containername: containerName, sortorder: index, newsletterid: parent.BITEDITPAGE.newsletterId };
                        var jsonstring = JSON.stringify(parametersObject);
                        BITAJAX.callWebService("MoveModule", jsonstring, null);
                    }
                }
            },                
    
            stop: function (event, ui) {
                var moduleMoveGrip = $(ui.item).find('.moduleMoveGrip');
                $(moduleMoveGrip).css({ 'width': 0, 'height': 0, 'display': 'none' });
                parent.BITEDITPAGE.calulateOrderingNumber(ui.item);
                var moduleId = $(ui.item).attr('id').replace('bitModule', '');
                //verberg infodivje
                $('#ModuleInfo' + moduleId).remove();
            }
            //$('.bitContainer').disableSelection();
        });
    }
});