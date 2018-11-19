//voor inloggen
var BITAUTH = {
    user: null,
    initialize: function () {
        
        $('#bitPopups').formEnrich();
    },

    //////////////////////////////////////////////
    // LOGIN
    //////////////////////////////////////////////
    login: function () {
        $('#submit').addClass('submit-loader');
        $('#submit').val('waiting');
        var email = $('#username').val();
        var password = $('#password').val();
        var theme = $('#selectTheme').val();
        var saveData = $('#save').is(":checked");

        var validation = $('#bitLoginWindowLogin').validate();

        if (validation) {
            var parametersObject = { email: email, password: password, theme: theme, language: '', saveData: saveData };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/Auth/AuthService.aspx";
            BITAJAX.callWebServiceASync("Login", jsonstring, function (data) {
                if (data.d) {
                    var urlParams = BITUTILS.getQueryStringAsObject();
                    if (urlParams && urlParams.referer) {
                        location.href = urlParams.referer;
                    }
                    else {
                        location.href = '/';
                    }
                }
                else {
                    $.messageBox(MSGBOXTYPES.ERROR, 'Kan niet aanmelden', 'Login mislukt!');
                    $('#submit').removeClass('submit-loader');
                    $('#submit').val('Login');
                }
            });
        }
    },

    checkEnter: function (e, moduleID) {
        var characterCode; //literal character code will be stored in this variable

        if (event && event.which) {
            //other browsers
            characterCode = event.which;
        }
        else {
            //IE
            characterCode = event.keyCode;

        }

        if (characterCode == 13) {
            BITAUTH.login();
        }
    },

    logoff: function () {
    },

    userSettingsDialog: function () {
        if ($('#bitUserSettingsDialog').html() == '') {
            var dialogData = BITAJAX.load('/Dialogs/ConfigCurrentUser.aspx');
            $('#bitUserSettingsDialog').html(dialogData);
            $('#bitUserSettingsDialog').formEnrich();
            $('#bitUserSettingsDialog').initDialog(BITAUTH.saveCurrentUserSettings);
        }
       
        BITAJAX.dataServiceUrl = "/Auth/AuthService.aspx";
        BITAJAX.callWebService('GetCurrentUser', null, function (data) {
            BITAUTH.user = data.d;
            $('#bitUserSettingsDialog').dataBind(BITAUTH.user);
        });
        $('.button').button();
        $('#bitUserSettingsDialog').dialog('open');
       
    },

    saveCurrentUserSettings: function () {
        BITAUTH.user = $('#bitUserSettingsDialog').collectData(BITAUTH.user);
        BITAJAX.dataServiceUrl = "/Auth/AuthService.aspx";
        var jsonstring = convertToJsonString(BITAUTH.user);
        BITAJAX.callWebService('SaveCurrentUser', jsonstring);
        $.messageBox(MSGBOXTYPES.INFO, 'Gegevens opgeslagen', 'Uw gegevens zijn succesvol opgeslagen.');
        $('#bitUserSettingsDialog').dialog('close');
    },

    changeUserPassword: function () {
        var validation = true;
        var isEmptyFieldValidationError = false;
        if ($('#bitUserPassword').val().trim() == '') { 
            validation = false;
            isEmptyFieldValidationError = true;
        }
        if ($('#bitUserPasswordConfirm').val().trim() == '') { 
            validation = false;
            isEmptyFieldValidationError = true;
        }
        if ($('#bitUserCurrentPassword').val().trim() == '') { 
            validation = false;
            isEmptyFieldValidationError = true;
        }
        
        if ($('#bitUserPassword').val() != $('#bitUserPasswordConfirm').val()) {
            validation = false;
        }

        if (validation) {
            var parametersObject = { NewPassword: $('#bitUserPassword').val(), NewPasswordConfirm: $('#bitUserPasswordConfirm').val(), Password: $('#bitUserCurrentPassword').val() };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.callWebService('ChangePassword', jsonstring, function (data) {
                if (data.d.Success) {
                    $.messageBox(MSGBOXTYPES.INFO, 'Uw wachtwoord is gewijzigd.', 'Wachtwoord wijzigen.');
                    $('#bitUserPassword').val('');
                    $('#bitUserPasswordConfirm').val('');
                    $('#bitUserCurrentPassword').val('');
                    BITAUTH.user.Password = data.d.DataObject;
                }
                else {
                    $.messageBox(MSGBOXTYPES.WARNING, 'Controlleer uw huidige wachtwoord.');
                    $('#bitUserCurrentPassword').val('');
                }
            });


        }
        else {
            $('#bitUserCurrentPassword').val('');
            if (isEmptyFieldValidationError) {
                $.messageBox(MSGBOXTYPES.WARNING, 'Vul alle velden in.', 'Wachtwoord wijzigen.');
            }
            else {
                $.messageBox(MSGBOXTYPES.WARNING, 'De nieuw ingevoerde wachtwoorden komen niet overeen.', 'Wachtwoord wijzigen.');
            }
        }
    },

    changeTheme: function () {
        var theme = $('#selectThema').val();
        $('link[type="text/css"]').each(function () {
            var cssLink = $(this).attr('href');
            var i = cssLink.indexOf('_themes/');

            if (i != undefined) {
                var currentTheme = cssLink.substr(i);
                currentTheme = currentTheme.substr(0, currentTheme.indexOf('/', 8));
                cssLink = cssLink.replace(currentTheme, '_themes/' + theme);
                $(this).attr('href', cssLink);
            }
        });
    },

    sendNewPassword: function () {
        var validation = $('#E-mailadres').val().trim() != "";
        if (validation) {
            var email = $('#E-mailadres').val();
            var parametersObject = { email: email };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "/Auth/AuthService.aspx";
            BITAJAX.callWebService("SendNewPassword", jsonstring, function (data) {
                if (data.d) {
                    $.messageBox(MSGBOXTYPES.INFO, "Er is een nieuw wachtwoord naar " + email + " gestuurd.");
                }
                else {
                    $.messageBox(MSGBOXTYPES.ERROR, "Er heeft zich een fout voorgedaan tijdens het verzenden van uw wachtwoord.<br />Probeer op nieuw.");
                }
                
            });
        }
        else {
            $.messageBox(MSGBOXTYPES.WARNING, 'Vul uw email adres in.');
        }
    },

    showForgetPassword: function () {
        $('#bitLoginWindowLogin').fadeOut('slow', 'linear');
        $('#bitLoginWindowForgotPassword').fadeIn('slow', 'linear');
    },

    showLogin: function () {
        $('#bitLoginWindowLogin').fadeIn('slow', 'linear');
        $('#bitLoginWindowForgotPassword').fadeOut('slow', 'linear');
    }
}

