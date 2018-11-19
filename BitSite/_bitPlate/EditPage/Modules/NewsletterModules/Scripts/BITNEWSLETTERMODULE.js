var BITNEWSLETTERMODULE= {
    
    loadNewsletterGroups: function () {
        BITAJAX.dataServiceUrl = BITALLMODULES.getServiceUrl("NewsletterService.aspx");
        BITAJAX.callWebService('ListNewsletterGroup', null, function (data) {
            $('.newsletter-opt-in-table').dataBindList(data.d);
        });
    },

    register: function () {
        BITAJAX.dataServiceUrl = BITALLMODULES.getServiceUrl("DataService.aspx");
        var parametersObject = { email: ''}
        var jsonstring = JSON.stringify(parametersObject);

        BITAJAX.callWebService(webserviceCall, jsonstring, function (data) {


        });
    }

}