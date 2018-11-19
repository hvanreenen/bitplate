//constants
var MSGBOXTYPES = { INFO: "INFO", YESNO: "YESNO", DELETECONFIRM: "DELETECONFIRM", DETACHCONFIRM: "DETACHCONFIRM", WARNING: "WARNING", ERROR: "ERROR", INPUT: "INPUT" };


(function ($) {

    $.fn.extend({

        initDialog: function (okfunction, size, modal, buttons) {
            if (!size) {
                size = new Object;
                size.width = 1000;
                size.height = 600;
            }
            if (modal==undefined || modal==null) modal = true;
            if (!buttons) {
                if (okfunction) {
                    buttons = {
                        "Opslaan": okfunction,
                        "Annuleer": function () {
                            $(this).dialog('close');
                        }
                    };
                }
                else {
                    buttons = {
                        "Sluiten": function () {
                            $(this).dialog('close');
                        }
                    };
                }
            }
            $(this).dialog({
                autoOpen: false,
                width: size.width,
                minHeight: size.height,
                maxHeight: 800,
                modal: modal,
                resizable: false,
                open: function (event, ui) {
                    //$(".ui-dialog-titlebar").append("<a href='javascript:;' id='aBitPopupMaximize' role='button'><span class='bitPopupMaximize' id='bitPopupMaximize'></span></a>");
                },
                buttons: buttons,
                dialogClass: 'bitPopups',
                hide: { effect: "fade", duration: 100 },
                show: { effect: "fade", duration: 100 }
            });
            
        },

        initResizableDialog: function (okfunction, size, modal, buttons) {
            if (!size) {
                size = new Object;
                size.width = 1000;
                size.height = 600;
            }
            if (modal == undefined || modal == null) modal = true;
            if (!buttons) {
                if (okfunction) {
                    buttons = {
                        "Opslaan": okfunction,
                        "Annuleer": function () {
                            $(this).dialog('close');
                        }
                    };
                }
                else {
                    buttons = {
                        "Sluiten": function () {
                            $(this).dialog('close');
                        }
                    };
                }
            }
            $(this).dialog({
                autoOpen: false,
                width: size.width,
                minHeight: size.height,
                modal: modal,
                resizable: true,
                open: function (event, ui) {
                    //$(".ui-dialog-titlebar").append("<a href='javascript:;' id='aBitPopupMaximize' role='button'><span class='bitPopupMaximize' id='bitPopupMaximize'></span></a>");
                },
                buttons: buttons,
                dialogClass: 'bitPopups',
                hide: { effect: "fade", duration: 100 },
                show: { effect: "fade", duration: 100 }
            });

        },
        
        msgBox: function (type, msg, title, okfunction, defaultValue, cancelfunction) {
            $('#bitMasterMsgBoxIcon').attr('class', '');
            cancelfunction = (!cancelfunction) ? function () { } : cancelfunction;
            if (type != MSGBOXTYPES.INPUT) {
                $('#bitMasterMessage').html(msg);
            }
            
            if (type == MSGBOXTYPES.INFO) {
                $('#bitMasterMsgBoxIcon').addClass('bitMessageBoxInfo');
                buttons = {
                    "OK": function (e) {
                        $(this).dialog('close');
                        if (okfunction) {
                            okfunction(e);
                        }
                    }
                };
                if (!title) title = "Bitplate info";
                //TODO $('#bitMasterMsgBoxIcon').toggleClass("");
            }
            else if (type == MSGBOXTYPES.ERROR) {
                $('#bitMasterMsgBoxIcon').addClass('bitMessageBoxError');
                buttons = {
                    "OK": function (e) {
                        $(this).dialog('close');
                        if (okfunction) {
                            okfunction(e);
                        }
                    }
                };
                if (!title) title = "Error";
                    //TODO $('#bitMasterMsgBoxIcon').toggleClass("");
            }
            else if (type == MSGBOXTYPES.WARNING) {
                $('#bitMasterMsgBoxIcon').addClass('bitMessageBoxWarning');
                buttons = {
                    "OK": function (e) {
                        $(this).dialog('close');
                        if (okfunction) {
                            okfunction(e);
                        }
                    }
                };
                if (!title) title = "Waarschuwing";
                //TODO $('#bitMasterMsgBoxIcon').toggleClass("");
            }
            else if (type == MSGBOXTYPES.YESNO) {
                $('#bitMasterMsgBoxIcon').addClass('bitMessageBoxQuestion');
                buttons = {
                    "Ja": function (e) {
                        $(this).dialog('close');
                        okfunction(e);
                    },
                    "Nee": function (e) {
                        $(this).dialog('close');
                        cancelfunction(e);
                    }
                };
                if (!title) title = "Verwijderen";
                //TODO $('#bitMasterMsgBoxIcon').toggleClass("");
            }
            else if (type == MSGBOXTYPES.DELETECONFIRM) {
                $('#bitMasterMsgBoxIcon').addClass('bitMessageBoxQuestion');
                buttons = {
                    "Verwijder": function (e) {
                        $(this).dialog('close');
                        okfunction(e);
                    },
                    "Annuleer": function (e) {
                        $(this).dialog('close');
                        cancelfunction(e);
                    }
                };
                if (!title) title = "Verwijderen";
                    //TODO $('#bitMasterMsgBoxIcon').toggleClass("");
            }
            else if (type == MSGBOXTYPES.DETACHCONFIRM) {
                $('#bitMasterMsgBoxIcon').addClass('bitMessageBoxQuestion');
                buttons = {
                    "Loskoppelen": function (e) {
                        $(this).dialog('close');
                        okfunction(e);
                    },
                    "Annuleer": function (e) {
                        $(this).dialog('close');
                        cancelfunction(e);
                    }
                };
                if (!title) title = "Loskoppelen";
                //TODO $('#bitMasterMsgBoxIcon').toggleClass("");
            }
            else if (type == MSGBOXTYPES.INPUT) {
                $('#bitMasterInput').val(defaultValue);
                buttons = {
                    "OK": function (e) {
                        var value = $('#bitMasterInput').val();
                        if ($('#bitMasterInput').attr('data-validation') != '') {
                            if ($.inputBox.validate()) {
                                $(this).dialog('close');
                                okfunction(e, value);
                            }
                        }
                        else {
                            $(this).dialog('close');
                            okfunction(e, value);
                        }
                    },
                    "Annuleer": function (e) {
                        cancelfunction(e);
                        $(this).dialog('close');
                    }
                };
                if (!title) title = "Invoer veld";
            }

            $(this).dialog({
                autoOpen: true,
                minWidth: 420,
                modal: false,
                title: title,
                buttons: buttons,
                zIndex: 10000,
                hide: { effect: "drop", duration: 100 },
                show: { effect: "drop", duration: 100 }
            });
            $(this).dialog("moveToTop");
        }
    });

})(jQuery);

jQuery.inputBox = function (msg, title, defaultValue, okFunction) {
    $('#bitMasterInputDialog').msgBox(MSGBOXTYPES.INPUT, msg, title, okFunction, defaultValue);
}

jQuery.inputBox.setValidation = function (validationOptions) {
    $('#bitMasterInput').attr('data-validation', validationOptions);
}

jQuery.inputBox.validate = function () {
    return $('#bitMasterInputDialog').validate();
}

/* jQuery.messageBox = function (msg, title, okFunction, cancelFunction) {
    if (!okFunction) {
        okFunction = function () { }
    }
    if (cancelFunction) {
        cancelFunction = function () { }
    }
    $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.INFO, msg, title, okFunction, null, cancelFunction);
} */

jQuery.messageBox = function (MSGBOXTYPE, msg, title, okFunction, cancelFunction) {
    if (!okFunction) {
        okFunction = function () { }
    }
    if (cancelFunction) {
        cancelFunction = function () { }
    }
    $('#bitMasterMsgDialog').msgBox(MSGBOXTYPE, msg, title, okFunction, null, cancelFunction);
}

jQuery.deleteConfirmBox = function (msg, title, okFunction) {
    if (okFunction == undefined || okFunction == null) {
        okFunction = function () { }
    }
    $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.DELETECONFIRM, msg, title, okFunction);
}

jQuery.confirmBox = function (msg, title, okFunction, cancelfunction) {
    if (okFunction == undefined || okFunction == null) {
        okFunction = function () { }
    }
    $('#bitMasterMsgDialog').msgBox(MSGBOXTYPES.YESNO, msg, title, okFunction, null, cancelfunction);
}