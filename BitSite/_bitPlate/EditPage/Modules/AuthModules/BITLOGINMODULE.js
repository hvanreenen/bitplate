var BITLOGINMODULE = {
    moduleId: null,
    initialize: function () {
        $(document).on('endRequestHandler', function (sender, args) {
            //console.log('test');
            if ($(args._activeElement).attr('id').indexOf('AuthSubmitButton')) {
                BITLOGINMODULE.moduleId = $(args._activeElement).attr('id').replace('AuthSubmitButton', '');
                //console.log(BITLOGINMODULE.moduleId);
                BITLOGINMODULE.setMessageTimeOut();
            }
        });
    },

    setMessageTimeOut: function () {
        setTimeout(function () {
            $('#AuthMessageLabel' + BITLOGINMODULE.moduleId).fadeOut();
        }, 10000);
    }
}

$(document).ready(BITLOGINMODULE.initialize());




//BITALLMODULES.registerInitFunction('LoginModule', BITLOGINMODULE.initialize);