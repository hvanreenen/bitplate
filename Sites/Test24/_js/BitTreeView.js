

$(document).ready(function () {
    //BITTREEVIEW.initialize();
});

var BITTREEVIEW = {
    
    openAllNodesInitialy: false,
    themeSupport: false,
    isReloadRequired: true,

    selectedNode: null,
    initialize: function () {
        $('.bitTree').hide();
        //BITTREEVIEW.makeTree(); //Load tree for current page
    },

    makeTree: function () {
        //if isReloadRequired is false dan betekend dit dat het reload event afkomstig is van een click event op de tree zelf. Hiervoor hoeft de tree niet opnieuw te worden opgebouwd.
        if (!BITTREEVIEW.isReloadRequired) {
            //Reset isReloadRequired naar true. Dit is nodig omdat een event mogelijk de volgende keer vanaf een andere module verstuurd wordt. In dit geval moet de tree opnieuw worden ingeladen.
            BITTREEVIEW.isReloadRequired = true;
            return;
        }
        $('.bitTree').each(function () {
            var jsonsettings = $(this).parent().find('.modulesettings').attr('data-module-settings').replace(/-/g, '"');
            var settings = $.parseJSON(jsonsettings);
            var plugins = (settings.themeSupport != 'false') ? ["themes", "html_data", "ui"] : ["html_data", "ui"]
            var initialSelectNodeId = $(this).attr('data-tree-selected-id');
            initialSelectNodeId = (initialSelectNodeId) ? [initialSelectNodeId] : undefined;
            var thisTree = $(this).jstree({
                "core": {
                    "animation": settings.AnimationSpeed
                },
                'ui': {
                    'selected_parent_open': true,
                    "initially_select": initialSelectNodeId
                },
                "plugins": plugins
            })
            .bind("select_node.jstree", function (NODE, REF_NODE) {
                REF_NODE.inst.toggle_node(REF_NODE.rslt.obj);
                var selectedNode = REF_NODE.rslt.obj;//$.jstree._focused().get_selected();
                if (initialSelectNodeId) {
                    $.jstree._reference(thisTree).open_node(selectedNode);
                    BITTREEVIEW.selectedNode = selectedNode;
                    initialSelectNodeId = undefined;
                }
                else {
                    if (!BITTREEVIEW.selectedNode && !initialSelectNodeId || BITTREEVIEW.selectedNode[0].id != selectedNode[0].id) {
                        //var functionlink = $(selectedNode).find('.jstree-clicked').attr('onclick').replace('javascript:', '');
                        ////"use non strict";
                        ////eval(functionlink);
                        //var f = new Function(functionlink);
                        //f();
                        BITTREEVIEW.selectedNode = selectedNode;
                    }
                }
                //Reset isReloadRequired naar true. Dit is nodig omdat een event mogelijk de volgende keer vanaf een andere module verstuurd wordt. In dit geval moet de tree opnieuw worden ingeladen.
                //BITTREEVIEW.isReloadRequired = true;
            }).bind("loaded.jstree", function (event, data) {
                // you get two params - event & data - check the core docs for a detailed description
                if (settings.openAllNodesInitialy != 'false') {
                    $(this).jstree("open_all");
                }
            });
        });
        $('.bitTree').fadeIn('fast');
    },

    onBeginUpdatePanelRequest: function (sender, args) {
        $('.bitTree').jstree('destory');
    },

    
    /* onEndUpdatePanelRequest: function () {
        BITTREEVIEW.makeTree(); // make tree in the treeview module.
    } */
}
$(document).on('bitplateLoaded', BITTREEVIEW.makeTree);