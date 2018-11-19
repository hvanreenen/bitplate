var BitContactFormModule = {
    initialized: false,
    initializeSubmit: function (moduleId) {
        $('#Form1').iframePostForm({ //look like ajax post
            iframeID: 'asyncPost',
            json: true,
            post: function () {
            },
            complete: function (data) {
                var elementModuleId = moduleId.replace(/-/g, "");
                //console.log(data);
                if (data.Success) {
                    switch (data.DataObject.NavigationType) {
                        case 1:
                            $('#button' + elementModuleId).attr('disabled', 'disabled');
                            $('#bitModule' + moduleId + ' *').fadeOut('fast', function () {
                                $('#PanelSuccesMessage' + elementModuleId).fadeIn('fast');
                            });
                            break;
                        default:
                        case 0:
                            window.location = data.DataObject.NavigationUrl;
                            break;
                    }
                }
                else {
                    $('#PanelErrorMessage' + elementModuleId).fadeIn('fast');
                    ElementTimeOut($('#PanelErrorMessage' + elementModuleId), 1000);
                }
            }
        });
        BitContactFormModule.initialized = true;
    },

    submitContactForm: function (moduleId) {
        if (!BitContactFormModule.initialized) {
            BitContactFormModule.initializeSubmit(moduleId);
        }
        if ($('#bitModule' + moduleId).validate()) {
            $('#bitModule' + moduleId).find('input, textarea, select').each(function () {
                var input = $(this);
                var name = input.attr('name');
                if (name && name.indexOf(moduleId) < 0) {
                    var newName = moduleId + name;
                    input.attr('name', newName);
                }
            });
            //document.forms[0].submit();
            $('#Form1').submit();
        }
    },

    ElementTimeOut: function (element, timeout) {
        setTimeout(function () {
            $(element).fadeOut('fast');
        }, timeout);
    }
};