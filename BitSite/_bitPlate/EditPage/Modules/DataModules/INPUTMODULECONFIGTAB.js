var INPUTMODULECONFIGTAB = {
    showSelectDataGroupDialog: function () {
        var dataCollectionSelected = true;
        if (!BITEDITPAGE.currentModule.DataCollection) dataCollectionSelected = false;
        if (dataCollectionSelected && (!BITEDITPAGE.currentModule.DataCollection.ID || BITEDITPAGE.currentModule.DataCollection.ID == '00000000-0000-0000-0000-000000000000')) dataCollectionSelected = false;

        if (!dataCollectionSelected) {
            $.messageBox(MSGBOXTYPES.ERROR, 'Selecteer een datacollectie.', 'Selecteer een datacollectie.');
        }
        else {
            //console.log(BITEDITPAGE.currentModule.DataCollection.ID);
            var dataCollectionGroup = new Object()
            $('#selectDataGroupDialog').initDialog(null, { width: 400, height: 600 }, true, {
                'Selecteer': function () {
                    if (BITEDITPAGE.currentModule.DataCollection.ID != dataCollectionGroup.Id) { //if selectedId == Datacollection id)
                        BITEDITPAGE.currentModule.Settings = (!BITEDITPAGE.currentModule.Settings) ? new Object() : BITEDITPAGE.currentModule.Settings;
                        BITEDITPAGE.currentModule.Settings.SelectGroupID = dataCollectionGroup.Id;
                        //$('#dataCollectionPathName').html(dataCollectionGroup.Name);
                        $('#inputDataCollectionPathName').val(dataCollectionGroup.Name);
                    }
                    else {
                        BITEDITPAGE.currentModule.Settings.SelectGroupID = '';


                    }
                    $('#selectDataGroupDialog').dialog('close');
                },
                "Annuleer": function () {
                    $('#selectDataGroupDialog').dialog('close');
                }
            });
            var parametersObject = { datacollectionID: BITEDITPAGE.currentModule.DataCollection.ID, searchString: '' };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/_bitplate/DataCollections/DataCollectionService.asmx";
            BITAJAX.callWebService("GetTreeHtml", jsonstring, function (data) {
                $('#dataCollectionGroupTree').html(data.d);
                //console.log(data.d);
                $('#dataCollectionGroupTree').jstree({ "plugins": ["themes", "html_data", "ui"] }).bind("select_node.jstree", function (event, data) {
                    //`data.rslt.obj` is the jquery extended node that was clicked          
                    // To get path [ID or Name] from root node to selected node 
                    var ids = data.inst.get_path('#' + data.rslt.obj.attr('id'), true);
                    // Returns IDs from root to selected node
                    var names = data.inst.get_path('#' + data.rslt.obj.attr('id'), false);
                    // Returns Name's from root to selected node 

                    dataCollectionGroup.Name = names.toString().replace(/,/g, '/');
                    dataCollectionGroup.Id = ids[ids.length - 1];
                });
                $('#selectDataGroupDialog').dialog('open');
            });
        }
    },

    clearSelectGroup: function () {
        $('#inputDataCollectionPathName').val('');
        BITEDITPAGE.currentModule.Settings.SelectGroupID = '';
    },
}