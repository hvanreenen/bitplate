var BITFILESPOPUP = {
    target: "EditPage",
    isInitialized: false,

    initialize: function(){
        $('#bitFilesDialog').initDialog(null, { width: 300, height: 600 }, false);
        $('#bitFilesDialog').load('/_bitplate/dialogs/Files.aspx', this.onFilesLoaded);
        BITFILESPOPUP.isInitialized = true;
    },

    show: function(){
        $('#bitFilesDialog').dialog("open");
    },

    getFilesAndSubFolders: function (folder) {
        var parametersObject = { folder: folder };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "/_bitplate/FileManager/FileService.asmx";
        BITAJAX.callWebService("GetFilesAndSubFolders", jsonstring, this.onFilesLoaded);
    },

    onFilesLoaded: function (data) {
        $('#bitFilesDialog').html(data.d);
        if (BITFILESPOPUP.target == "EditPage") {
            $('.bitFile').dblclick(function () {
                var html = $(this).html();
                if (BITEDITOR.selection) BITEDITOR.selection.replaceHTML(html);
            });
        }
        else {
            $('.bitFile').click(function () {
                var src = $(this).find('img').attr('src');
                BITDATACOLLECTIONDATA.setFileUrl(src);
                $('#bitFilesDialog').dialog("close");
            });
        }
    }


};