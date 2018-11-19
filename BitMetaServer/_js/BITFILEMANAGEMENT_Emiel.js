var BITFILEMANAGEMENT = {
    currentFolder: '',
    clipBoard: null,
    file: null,

    initialize: function () {
        $('#upload-dialog').dialog({
            autoOpen: false,
            minWidth: 350,
            modal: true,
            resize: true,
            buttons: {
                "Upload": BITFILEMANAGEMENT.uploadFiles
            }
        });

        $('#input-file').bind('change', BITFILEMANAGEMENT.getFileInfo);

        $('.button').button();
        BITFILEMANAGEMENT.registerEvents();
        var form = $('#upload-form');
        $('#file-gate').load(BITFILEMANAGEMENT.uploadComplete);
    },

    registerEvents: function() {
        $('#btn-add-file').unbind().click(BITFILEMANAGEMENT.addFileToUploadArray);
    },

    showFileUpload: function () {
        $(":button:contains('Upload')").removeAttr("disabled").removeClass("ui-state-disabled");
        $('#file-select-container').css('display', 'block');
        $('#file-overview').css('display', 'block');
        $('#upload-progress').css('display', 'none');

        $('.file-item').remove();
        BITFILEMANAGEMENT.filesToUpload = [];
        BITFILEMANAGEMENT.file = null;
        $('#input-file').val('');
        $('#upload-dialog').dialog('open');
    },

    getFileInfo: function () {
        //var form = $('#upload-form');
        //form.attr("encoding", "multipart/form-data");
        //$(form).html($(this).val());
        //form.submit();
        BITFILEMANAGEMENT.file = this.files[0];
    },

    addFileToUploadArray: function () {
        var item = '<tr class="file-item"><td>' + escape(BITFILEMANAGEMENT.file.name) + '</td><td>' + BITFILEMANAGEMENT.file.type + '</td><td></td>' + BITFILEMANAGEMENT.file.size + '</td></tr>';
        $('#file-overview > table').html($('#file-overview > table').html() + item);
        var fileName = $('#input-file').val().split(/(\\|\/)/g).pop();
        $('#input-file').attr('name', fileName);
        $('#input-file').appendTo('#upload-form');
        $('#input-file').attr('id', fileName);
        $('#upload-input').html('<input id="input-file" type="file" name="file1" />');
        $('#input-file').unbind().bind('change', BITFILEMANAGEMENT.getFileInfo);
    },

    uploadFiles: function () {
        $('#file-select-container').slideUp('fast');
        $('#file-overview').slideUp('fast');
        $('#upload-progress').slideDown('fast');
        $(":button:contains('Upload')").attr("disabled", "disabled").addClass("ui-state-disabled");
        $('#upload-form').submit();
    },

    uploadComplete: function () {
        $('#upload-form').html('');
        BITFILEMANAGEMENT.filesToUpload = [];
        $('#file-overview > table').html(' <tr>'+
                        '<td>Naam</td>'+ 
                        '<td>Type</td>' +
                        '<td>Size</td>' +
                    '</tr>');
        var parametersObject = { folder: BITFILEMANAGEMENT.currentFolder };
        var jsonstring = JSON.stringify(parametersObject);
        alert(jsonstring);

        BITAJAX.dataServiceUrl = "bitAjaxServices/FileService.aspx";
        BITAJAX.callWebService("SaveFiles", jsonstring, function (data) {
            BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder);
            $('#iframeUpload').html("");
        });
        $('#upload-dialog').dialog('close');
    },

    loadTree: function () {
        var timestamp = new Date().getTime();
        $('#treePanel').load('bitTrees/FoldersTree.aspx?' + timestamp, function () {
            $('#treePanel').treeview({
                selectedIndexChanged: function (id, type) {
                    BITFILEMANAGEMENT.getFiles(id);
                }
            });
        });
    },

    getFiles: function (folder) {
        BITFILEMANAGEMENT.currentFolder = folder;
        var parametersObject = { folder: folder, sortField: DATABINDER.currentsortfield, sortOrder: DATABINDER.currentsortorder };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "bitAjaxServices/FileService.aspx";
        BITAJAX.callWebService("GetFiles", jsonstring, function (data) {
            //BITPAGES.templates = data.d;
            $("#tableFiles").dataBindList(data.d);
        });
    },

    newFolder: function (folderName) {
        var parametersObject = { parentFolder: BITFILEMANAGEMENT.currentFolder, folderName: folderName };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "bitAjaxServices/FileService.aspx";
        BITAJAX.callWebService("AddFolder", jsonstring, function (data) {
            var timestamp = new Date().getTime();
            $('#treePanel').load('bitTrees/FoldersTree.aspx?' + timestamp, null);
        });
    },

    remove: function () {
        var parametersObject = { folderName: BITFILEMANAGEMENT.currentFolder };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "bitAjaxServices/FileService.aspx";
        var remoteFunction = "DeleteFolder";
        var msg = "";
        var selectedFiles = [];
        $('.checkFile').each(function (i) {
            if ($(this).is(":checked")) {
                selectedFiles.push($(this).attr('title'));
            }
        });
        if (selectedFiles.length > 0) {
            remoteFunction = "DeleteFiles";
            msg = "Wilt u deze bestanden verwijderen?";
            var parametersObject = { folderName: BITFILEMANAGEMENT.currentFolder, files: selectedFiles };
            var jsonstring = JSON.stringify(parametersObject);

            if (confirm(msg)) {
                BITAJAX.callWebService(remoteFunction, jsonstring, function (data) {
                    BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder);
                });
            }
        }
        else {
            remoteFunction = "DeleteFolder";
            msg = "Wilt u deze map met alle inhoud verwijderen?";

            if (confirm(msg)) {
                BITAJAX.callWebService(remoteFunction, jsonstring, function (data) {
                    var timestamp = new Date().getTime();
                    $('#treePanel').load('bitTrees/FoldersTree.aspx?' + timestamp, null);
                });
            }
        }

    },

    copy: function () {
        this.putIntoClipboard("copy");
    },

    cut: function () {
        this.putIntoClipboard("cut");
    },

    putIntoClipboard: function (type) {
        //unmake all gray
        $("#tableFiles").find('td, a').each(function (i) {
            $(this).css("color", "");
        });

        var selectedFiles = [];
        $('.checkFile').each(function (i) {
            if ($(this).is(":checked")) {
                selectedFiles.push($(this).attr('title'));
                if (type == "cut") {
                    //make gray
                    $(this).parent().parent().find('td, a').each(function (i) {
                        $(this).css("color", "gray");
                    });
                }
            }
        });
        if (selectedFiles.length > 0) {
            clipBoard = { type: type, folderName: BITFILEMANAGEMENT.currentFolder, files: selectedFiles };
        }
    },

    paste: function () {
        BITAJAX.dataServiceUrl = "bitAjaxServices/FileService.aspx";
        if (clipBoard) {
            var parametersObject = { oldFolderName: clipBoard.folderName, newFolderName: BITFILEMANAGEMENT.currentFolder, files: clipBoard.files };
            var jsonstring = JSON.stringify(parametersObject);
            if (clipBoard.type == "cut") {
                BITAJAX.callWebService("MoveFiles", jsonstring, function (data) {
                    BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder);
                });
            }
            else if (clipBoard.type == "copy") {
                BITAJAX.callWebService("CopyFiles", jsonstring, function (data) {
                    BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder);
                });
            }
            clipBoard = null;
        }

    },

    makeTextBox: function (name) {
        var html = "<input type='text' value='" + name + "' title='" + name + "' onblur=\"javascript:BITFILEMANAGEMENT.renameFile('" + name + "', this);\"/>";
        $("a[title='" + name + "']").replaceWith(html);
        $("input[title='" + name + "']").focus();
    },

    renameFile: function (oldName, textBox) {
        var newName = $(textBox).val();

        if (newName != oldName) {
            var parametersObject = { folder: BITFILEMANAGEMENT.currentFolder, oldFileName: oldName, newFileName: newName };
            var jsonstring = JSON.stringify(parametersObject);

            BITAJAX.callWebService("RenameFile", jsonstring, function (data) {
                BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder);
            });
        }
        else {
            //maak van textbox weer een hyperlink
            var html = "<a title='" + newName + "' href=\"javascript:BITFILEMANAGEMENT.makeTextBox('" + newName + "');\">" + newName + "</a>";
            $(textBox).replaceWith(html);
        }
    },

    showFile: function (name, link) {
        var popup = new Popup(name, "", null, { width: 600, height: 400 });
        if (name.toLowerCase().indexOf(".jpg") > 0 ||
            name.toLowerCase().indexOf(".png") > 0 ||
            name.toLowerCase().indexOf(".gif") > 0) {
                var src = ".." + BITFILEMANAGEMENT.currentFolder.replace(/\\/g, "/") + "/" + name;
                //style='max-width:600px;max-height:600px'
                popup.setContent("<img src = '" + src + "' />");
            }
        popup.show();
    },

    sort: function (sortField) {
        DATABINDER.sort(sortField);
        BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder);
    },

    selectAll: function (checkbox) {
        $(".checkFile").each(function (i) {
            $(this).attr("checked", $(checkbox).attr("checked"));
        });
    },

    downloadFiles: function () {
        var selectedFiles = [];
        $('.checkFile').each(function (i) {
            if ($(this).is(":checked")) {
                selectedFiles.push($(this).attr('title'));
            }
        });
        document.getElementById("iframeUpload").src = "bitAjaxServices/FileService.aspx?download=" + selectedFiles + "&downloadfolder=" + BITFILEMANAGEMENT.currentFolder;
           
    }


};