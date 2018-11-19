var BITUSERPROFILE = {

    initialize: function () {
        BITAUTHMODULES.getUser(BITUSERPROFILE.showUser);
    },

    showUser: function (user) {
        var test = $('form[id^="bitUserForm"]');//.dataBind(BITUSERPROFILE.CurrentUser);
        $(test).each(function () {
            $(this).dataBind(user);
        });
    },

    setUser: function () {
    },
}

BITALLMODULES.registerInitFunction('EigenGegevensModule', BITUSERPROFILE.initialize);