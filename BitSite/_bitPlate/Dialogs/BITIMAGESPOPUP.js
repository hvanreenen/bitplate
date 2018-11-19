var BITIMAGESPOPUP = {
    target: "EditPage",
    isInitialized: false,
    //siteDomain: "",

    initialize: function(){
        $('#bitImagesDialog').initDialog(null, { width: 300, height: 600 }, false);
        $('#bitImagesDialog').load('/_bitplate/dialogs/Images.aspx', this.onImagesLoaded);
        BITIMAGESPOPUP.isInitialized = true;
    },

    show: function(){
        $('#bitImagesDialog').dialog("open");
    },

    getImagesAndSubFolders: function (folder) {
        var parametersObject = { folder: folder };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/FileManager/FileService.asmx";
        BITAJAX.callWebService("GetImagesAndSubFolders", jsonstring, this.onImagesLoaded);
    },

    onImagesLoaded: function (data) {
        $('#bitImagesDialog').html(data.d);
        if (BITIMAGESPOPUP.target == "EditPage") {
            $('.bitImage').dblclick(function () {
                var html = $(this).html();
                if (BITEDITOR.selection) BITEDITOR.selection.replaceHTML(html);
            });
        }
        else {
            $('.bitImage').click(function () {
                var src = $(this).find('img').attr('src');
                //src = src.replace(BITIMAGESPOPUP.siteDomain, "..");
                BITDATACOLLECTIONDATA.setImageUrl(src);
                $('#bitImagesDialog').dialog("close");
            });
        }
    },

    CkSelect: function (link) {
        // Get CKEditorFuncNum from page url
        var
           urlArg = window.location.search.substr(1).split('&'), // Split search strings to "name=value" pair
           tmp,
           params = {},
           i
        ;
        // P
        for (var i = urlArg.length; i-- > 0;) {
            // Split to param name and value ("name=value")
            tmp = urlArg[i].split('=');
            // Save it to object
            params[tmp[0]] = tmp[1];
        }

        // Check eviroment
        if (params.CKEditorFuncNum) {
            // Call CKEditor function to insert the URL
            window.opener.CKEDITOR.tools.callFunction(params.CKEditorFuncNum, link);
            // Close Window
            window.close();
        } else {
            // Else do nothing
            return false;
        }
    }
};