var BITINPUTFORMMODULE = {
    formId: null,
    settings: null,
    registerForm: function (formId) {
        var settingString = $("#hiddenSettings" + formId).val();
        BITINPUTFORMMODULE.settings = $.parseJSON(settingString);
        BITINPUTFORMMODULE.formId = formId
        var form = $('#bitForm' + formId);
        if (!BITEDITPAGE) {
            $(form).iframePostForm({
                iframeID: 'PostFrame',
                json: true,
                post: function () {

                },
                complete: function (data) {
                    var EventData = new Object();
                    EventData.Form = form;
                    EventData.ServerResponse = data;
                    //console.log(data);
                    if (data.success) {
                        $('#bitForm' + BITINPUTFORMMODULE.formId).find('input, select, textarea').each(function () {
                            if (BITINPUTFORMMODULE.settings.EnableTooltip) {
                                if ($(this).is(':data(tooltip)')) {
                                    $(this).tooltip('destroy');
                                }
                            }
                        });
                        if (data.DrillDownUrl) {
                            if (data.DrillDownUrl != '') {
                                window.location = data.DrillDownUrl;
                            }
                        }
                        else {
                            $(document).trigger('onBitFormSuccess' + BITINPUTFORMMODULE.formId, EventData);
                            /* $(form).fadeOut('fast', function () { $('#bitFormAcceptResponse' + formId).fadeIn('fast'); }); */
                        }
                    }
                    else {
                        $('#bitForm' + BITINPUTFORMMODULE.formId).find('input, select').each(function () {
                            var fieldName = $(this).attr('name');
                            if (typeof data[fieldName] == 'string') {
                                $(this).addClass('inputError');
                                $(this).keydown(function () {
                                    $(this).removeClass('inputError');
                                    $(this).attr('title', '');
                                });
                                $(this).attr('title', data[fieldName]);
                                if (BITINPUTFORMMODULE.settings.EnableTooltip) {
                                    $(this).tooltip({
                                        track: true
                                    });
                                }
                            }
                        });

                        $(document).trigger('onBitFormError' + BITINPUTFORMMODULE.formId, EventData);
                        /* $(form).fadeOut('fast', function () { $('#bitFormErrorResponse' + formId).fadeIn('fast'); });
                        $('#bitFormErrorTryAgain' + formId).click(function () {
                            $('#bitFormErrorResponse' + formId).fadeOut('fast', function () { $(form).fadeIn('fast'); });
                        }); */
                    }
                }
            });
        }
    },

    submitForm: function (id) {
        var validation = $('#bitForm' + id).validate();
        if (validation) {
            $('#bitForm' + id).submit();
        }
    },

    configLoadComplete: function () {
        BITINPUTFORMMODULE.dataCollectieStateToggle();
        BITINPUTFORMMODULE.emailStateToggle();
        BITINPUTFORMMODULE.updateFieldList();
        CKEDITOR.replace('emailTemplate', {
            filebrowserBrowseUrl: 'pages.aspx?cmd=select',
            filebrowserImageBrowseUrl: 'bitPopups/ElFinder.aspx',
            filebrowserUploadUrl: 'bitPopups/ElFinder.aspx',
            filebrowserImageUploadUrl: 'bitPopups/ElFinder.aspx'
        });
    },

    emailStateToggle: function () {
        var state = $('#saveToEmail').is(':checked');
        if (state) {
            $('#bitFormEmailSettings').slideDown('fast');
        }
        else {
            $('#bitFormEmailSettings').slideUp('fast');
        }
    },

    dataCollectieStateToggle: function () {
        var state = $('#saveToDataCollectie').is(':checked');
        if (state) {
            //$('#bitFormDataCollectieSettings').slideDown('fast');
        }
        else {
            //$('#bitFormDataCollectieSettings').slideUp('fast');
        }
    },

    updateFieldList: function () {
        //$('#InputFormFieldList')
        var FormId = BITEDITPAGE.currentModule.ID;
        var list = Array();
        $('#bitForm' + FormId).find('input, select, textarea').each(function () {
            var name = $(this).attr('name');
            //console.log(name);
            var namePresents = false;
            if (name != 'bitFormId') {
                $(list).each(function () {
                    if (this.fieldName == name) {
                        namePresents = true;
                    }
                });
                if (!namePresents && (name)) {
                    var listObject = {};
                    listObject.fieldName = $(this).attr('name');
                    var dataValidation = $(this).attr('data-validation');
                    if (BITEDITPAGE.currentModule.Settings) {
                        if (BITEDITPAGE.currentModule.Settings.validation) {
                            if (BITEDITPAGE.currentModule.Settings.validation[name]) {
                                listObject.dataRequired = BITEDITPAGE.currentModule.Settings.validation[name].Required;
                                listObject.dataValidationType = BITEDITPAGE.currentModule.Settings.validation[name].DataType;
                                listObject.errorMessage = BITEDITPAGE.currentModule.Settings.validation[name].ErrorMessage;
                            }
                            else {
                                listObject.dataRequired = false;
                                listObject.dataValidationType = '';
                                listObject.errorMessage = '';
                            }

                            //console.log(listObject);
                        }
                    }
                    list.push(listObject);
                }
            }
        });
        $('#InputFormFieldList').dataBindList(list);
        
    },
    
    configSave: function () {
        BITEDITPAGE.currentModule.Settings.validation = null;
        $('#InputFormFieldList tbody tr').each(function () {
            BITINPUTFORMMODULE.writeDataRule(this);
        });
    },

    writeDataRule: function (row) {
        var FormId = BITEDITPAGE.currentModule.ID;
        if (!BITEDITPAGE.currentModule.Settings.validation) {
            BITEDITPAGE.currentModule.Settings.validation = new Object();
        }
        var name = $(row).find('td:first').html();
        if (name) {
            BITEDITPAGE.currentModule.Settings.validation[name] = new Object();
            BITEDITPAGE.currentModule.Settings.validation[name].Name = name;
            BITEDITPAGE.currentModule.Settings.validation[name].Required = $(row).find('input[type="checkbox"]').is(':checked');
            BITEDITPAGE.currentModule.Settings.validation[name].DataType = $(row).find('select').val();
            BITEDITPAGE.currentModule.Settings.validation[name].ErrorMessage = $(row).find('input[type="text"]').val();
        }
        
        //console.log(BITEDITPAGE.currentModule.Settings.validation[name]);
    }
};

BITALLMODULES.registerInitFunction("InputFormModule", BITINPUTFORMMODULE.registerForm);