var BITLOGINSTATUSMODULE = {
    moduleId: null,
    initialize: function () {
        $(document).on('endRequestHandler', function (sender, args) {
            if ($(args._activeElement).attr('id').indexOf('LogoutButton')) {

            }
        });
    }
}

$(document).ready(BITLOGINSTATUSMODULE.initialize());




//BITALLMODULES.registerInitFunction('LoginModule', BITLOGINMODULE.initialize);