var BITNEWSLETTERMODULES = {

    saveSubscriber: function (moduleId) {
        if ($('#bitModule' + moduleId).validate()) {
            
            $('#Form1').submit();
        }
    },

    unsubcribeSubscriber: function (moduleId) {
        $('input[name=hiddenFormAction]').attr('value', 'Unsubscribe');
        var frm = $('#form' + moduleId.replace(/-/g, ""))[0];
        submitBitModule(frm);
        //frm.submit();
    }
};