var BITAUTHMODULES = {
    CurrentUser: null,

    initialize: function () {
    },

    getUser: function (callBack) {
        if (BITAUTHMODULES.CurrentUser == null) {
            BITAJAX.dataServiceUrl = "../_bitPlate/_bitModules/Authmodules/AuthService.aspx";
            BITAJAX.callWebService("GetSiteUser", null, function (data) {
                if (data.d != null) {
                    BITUSERPROFILE.CurrentUser = data.d;
                    callBack(BITUSERPROFILE.CurrentUser);
                }
            });
        }
        else {
            callBack(BITUSERPROFILE.CurrentUser);
        }
    },
}