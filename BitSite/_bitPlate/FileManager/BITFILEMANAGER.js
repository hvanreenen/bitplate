$(document).ready(function () {
    BITFILEMANAGEMENT.initialize();
    BITFILEMANAGEMENT.registerEvents();
    //BITFILEMANAGEMENT.makeDraggable();
    BITFILEMANAGEMENT.loadTree();
});

var BITFILEMANAGEMENT = {
    siteDomain: '',
    currentFolder: '/_FILES',
    currentSort: "Name ASC",
    renameObject: null,
    clipBoard: null,
    CodeMirrorEditor: null,
    isTreeVisible: null,
    thumbnailParameterObject: Array(),
    parameters: null,
    FileRefresh: true,

    initialize: function () {
        BITAJAX.dataServiceUrl = "FileService.asmx";
        $('#upload-dialog').initDialog(null, { width: 1200 }, true, {});
        $('#image-dialog').initDialog(null, { width: 800, height: 600 }, true);
        $('#image-dialog').dialog({ autoOpen: false });
        $('#pdf-viewer-dialog').initDialog(null, { width: 800, height: 600 }, true);
        $('#pdf-viewer-dialog').dialog({ minWidth: 800, minHeight: 600 });
        $('.button').button();
        $('button').button();
        $('#bitDivSearch').hide();
        BITFILEMANAGEMENT.parameters = BITUTILS.getQueryString();
        if (BITFILEMANAGEMENT.parameters) {
            $('#liBack').hide();
            $('#liSelect').show();
            $('#divBitplateTopMenu').html('');
        }
        BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder, 'ASC', true);
        $('#fileupload').unbind().bind('fileuploadstopped', function (e, data) { BITFILEMANAGEMENT.UploadComplete(e, data); });
        
    },

    registerEvents: function () {
        $('#tbtn-naamwijzigen-folder').unbind().click(function () {
            $("#bitBoxMain01").jstree("rename");
        });
    },

    loadTree: function () {
        
        var parametersObject = { searchString: '' };
        var jsonstring = JSON.stringify(parametersObject);
        //console.log(BITFILEMANAGEMENT.currentFolder);
        var currentFolder = (BITFILEMANAGEMENT.currentFolder && BITFILEMANAGEMENT.currentFolder.toUpperCase() != '/_FILES') ? $('#treeView').find('li[data-path="' + BITFILEMANAGEMENT.currentFolder + '"]').parent().attr('id') : 'node0';
        //console.log(currentFolder);
        BITAJAX.callWebService("BuildTree", jsonstring, function (data) {
            $('#treeView').html(data.d);
            $('#treeView').jstree({
                "plugins": ["themes", "html_data", "ui", "sort", "dnd", "core"],
                "themes": {
                    "theme": "default",
                    "dots": true,
                    "icons": true
                },
                checkbox: {
                    two_state: true,
                    real_checkboxes: true
                },
                "dnd": {
                    "drop_finish": function (data) {
                    },
                    "drag_check": function (data) {
                        if (data.r.attr("id") == "phtml_1") {
                            return false;
                        }
                        return {
                            after: false,
                            before: false,
                            inside: true
                        };
                    },
                    "drag_finish": function (data) {
                        BITFILEMANAGEMENT.saveJStreeDragFile(data);
                    }
                },

                "core": {
                    "initially_open": [currentFolder]
                }
            }).bind("select_node.jstree", function (event, data) {
                if (BITFILEMANAGEMENT.FileRefresh) {
                    var selectedObj = data.rslt.obj;
                    //console.log(event);
                    var path = $(selectedObj).find('a').attr('data-path');
                    BITFILEMANAGEMENT.currentFolder = path;
                    $('#currentPath').html(BITFILEMANAGEMENT.currentFolder);
                    $("#tableFiles").fadeOut('fast', BITFILEMANAGEMENT.getFiles(path, 'ASC', true));
                }
                BITFILEMANAGEMENT.FileRefresh = true;
            }).bind("move_node.jstree", function (event, data) {
                ////console.log(event);
                BITFILEMANAGEMENT.saveJStreeDropFolder(data);
            }).bind("loaded.jstree", function (event, data) {
                $(this).find('.jstree-checkbox:first').hide();
                $('#treeView ul li').contextMenu({
                    menu: 'folder-menu'
                });
            });
            $('#treeView').jstree('select_node', 'ul > li:first');
            //BITFILEMANAGEMENT.registerEvents();
        }); 
    },

    selectCurrentTreeNode: function () {
        //console.log(BITFILEMANAGEMENT.currentFolder);
        var escapedSelector = BITFILEMANAGEMENT.currentFolder.toLowerCase().replace(/\\/g, '\\\\');
        //console.log(escapedSelector);
        var currentFolderNode = (BITFILEMANAGEMENT.currentFolder && BITFILEMANAGEMENT.currentFolder.toUpperCase() != '/_FILES') ? $('#treeView').find('a[data-path="/' + escapedSelector + '"]').parent().attr('id') : 'node0';
        $('#treeView').jstree("deselect_all");
        //console.log(currentFolderNode);
        BITFILEMANAGEMENT.FileRefresh = false;
        $('#treeView').jstree("select_node", "#" + currentFolderNode);
        $('#treeView').jstree("open_node", "#" + currentFolderNode);
        
    },

    updateBreadCrump: function () {
        var folderList = [];
        var rootFolderItem = new Object();
        rootFolderItem.Path = "/_FILES";
        rootFolderItem.Name = "root";
        folderList.push(rootFolderItem);

        if (BITFILEMANAGEMENT.currentFolder && BITFILEMANAGEMENT.currentFolder != '/') {
            var folderNames = BITFILEMANAGEMENT.currentFolder.split("/");
            var path = "/_FILES";
            for (var i in folderNames) {
                var folderName = folderNames[i];
                if (folderName.toUpperCase() != "_FILES") {
                    if (folderName != "") {
                        if (path != "") {
                            path += '/';
                        }
                        path += folderName;
                        var folderItem = new Object();
                        if (folderList.length > 0) {
                        }
                        folderItem.Path = path + "/";
                        folderItem.Name = folderName;
                        folderList.push(folderItem);
                    }
                }
            }
        }
        

        $('#breadcrump').dataBindList(folderList);
        //<ul>list stylen naar breadcrump */
        $('#breadcrump').jBreadCrumb();

    },

    getFiles: function (folder, sort, isFullPath) {
        if (isFullPath) {
            BITFILEMANAGEMENT.currentFolder = folder;
            
        }
        else {
            if (folder && folder != '/_FILES') {
                BITFILEMANAGEMENT.currentFolder += '/' + folder + '/';
            }
            else {
                if (folder) {
                    BITFILEMANAGEMENT.currentFolder = folder;
                }
            }
            
        }
        BITFILEMANAGEMENT.selectCurrentTreeNode();
        if (sort) { BITFILEMANAGEMENT.currentSort = sort; }
        var parametersObject = { folder: BITFILEMANAGEMENT.currentFolder, sort: BITFILEMANAGEMENT.currentSort };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "FileService.asmx";
        BITAJAX.callWebService("GetFiles", jsonstring, function (data) {
            $("#tableData").dataBindList(data.d, {
                onSort: function (sort) {
                    BITFILEMANAGEMENT.getFiles(null, sort);
                },
                onRowBound: function (obj, index, html) {
                    //maak wrapper div, zet hierin de html van de tr
                    //doe bewerkingen in deze html
                    //zet weer terug in de tr (onderaan)
                    var tempWrapperDiv = document.createElement('div');
                    $(tempWrapperDiv).html(html);
                    if (obj.Type == "Folder") {
                        $(tempWrapperDiv).find('.icon').find('a').attr('class', 'bitTableFolderIcon');

                        $(tempWrapperDiv).find('tr').addClass('jstree-drop');
                        $(tempWrapperDiv).find('.icon').find('img').remove();
                        var htmlLink = "<a href=\"javascript:BITFILEMANAGEMENT.getFiles('" + BITFILEMANAGEMENT.currentFolder + '//' + obj.Name + "', 'ASC', 'true');\">" + obj.Name + "</a>";
                        $(tempWrapperDiv).find('.bitTableColumnName').html(htmlLink);
                    }
                    else {
                        var onlyImages = (BITFILEMANAGEMENT.parameters) ? BITFILEMANAGEMENT.parameters['type'].trim() : '';
                        var htmlLink;
                        
                        if (onlyImages == 'image' || onlyImages == 'file') {
                            if (onlyImages == 'image') {
                                if (obj.Name.indexOf('.png') != -1 || obj.Name.indexOf('.jpg') != -1 || obj.Name.indexOf('.jpeg') != -1 || obj.Name.indexOf('.gif') != -1) {
                                    htmlLink = "<a href=\"javascript:BITFILEMANAGEMENT.showFile('" + obj.Name.jEscape() + "', this);\">" + obj.Name + "</a>";
                                }
                                else {
                                    $(tempWrapperDiv).find('tr').hide();
                                }
                            }

                            
                        }
                        else {
                            htmlLink = "<a href=\"javascript:BITFILEMANAGEMENT.showFile('" + obj.Name.jEscape() + "', this);\">" + obj.Name + "</a>";
                        }
                        
                        $(tempWrapperDiv).find('.bitTableColumnName').html(htmlLink);
                    }

                    html = $(tempWrapperDiv).html();
                    return html;
                }
            });
            $('.file-record').contextMenu({
                menu: 'file-menu'
            });
            BITFILEMANAGEMENT.registerEvents();
            $('#tableData input[type=checkbox]').removeAttr('checked');
            //BITFILEMANAGEMENT.makeSelectableTableRow();
            $("#tableData").fadeIn('fast');
            BITFILEMANAGEMENT.updateBreadCrump();
        });
    },

    showUpload: function () {
        $('#upload-dialog').dialog('open');
    },

    UploadComplete: function (e, data) {
        $('#upload-dialog').dialog('close');
        BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder, 'ASC', true);
    },

    downloadFiles: function () {
        var selectedFiles = [];
        $('.checkFile').each(function (i) {
            if ($(this).is(":checked")) {
                selectedFiles.push($(this).attr('title'));
            }
        });
        if (selectedFiles.length > 0) {
            document.getElementById("download-frame").src = "FileService.asmx?download=" + encodeURIComponent(selectedFiles) + "&downloadfolder=" + encodeURIComponent(BITFILEMANAGEMENT.currentFolder);
        }
        else {
            $.messageBox(MSGBOXTYPES.INFO, 'Selecteer de bestanden die uw wilt downloaden.', 'Hoe download ik een bestand?');
        }
        
    },

    showRenameFile: function () {
        var selectedFiles = [];
        
        $('.checkFile').each(function (i) {
            if ($(this).is(":checked")) {
                selectedFiles.push({
                    name: $(this).attr('title'),
                    newName: ''
                });
            }
        });
        if (selectedFiles.length == 1) {
            var oldFileName = selectedFiles[0].name;
            if (oldFileName.toLowerCase() != '_img') { //BUG #104 FIX: if (oldFileName.toLowerCase() != '_img') { ... ELSE ...
                $.inputBox.setValidation('required');
                $.inputBox('', 'Naam Wijzigen', oldFileName, function (e, value) {
                    if (value != oldFileName) {
                        var parametersObject = { folder: BITFILEMANAGEMENT.currentFolder, oldFileName: oldFileName, newFileName: value };
                        var jsonstring = JSON.stringify(parametersObject);

                        BITAJAX.callWebService("RenameFile", jsonstring, function (data) {
                            BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder, 'ASC', true);
                        });
                    }
                });
            }
            else {
                $.messageBox(MSGBOXTYPES.WARNING, 'De map _img is een systeem map en kan niet van naam gewijzigd worden.');
            }
            
        }
        else {
            //if (selectedFiles.length > 1) {
                $.messageBox(MSGBOXTYPES.WARNING, 'U kunt 1 bestand/map tegelijkertijd hernoemen.', 'Waarschuwing');
            //}
            /* $.inputBox.setValidation('required');
            var folder = BITFILEMANAGEMENT.currentFolder.split('\\');
            folder = folder[folder.length - 1].toLowerCase().replace('\\', '').replace('//', '');
            if (folder != '_files') {
                $.inputBox('', 'Naam Wijzigen', folder, function (value) {
                    if (value != folder) {
                        var i = BITFILEMANAGEMENT.currentFolder.lastIndexOf('\\');
                        var newFolderLokation = BITFILEMANAGEMENT.currentFolder.substr(0, i + 1) + value;
                        var parametersObject = { CurrenFolderLocation: BITFILEMANAGEMENT.currentFolder, NewFolderLokation: newFolderLokation };
                        var jsonstring = JSON.stringify(parametersObject);

                        BITAJAX.callWebService("RenameFolder", jsonstring, function (data) {
                            BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder, 'ASC', true);
                        });
                    }
                });
            } */
            //else {
                
                //else {
                 //   $.messageBox(MSGBOXTYPES.WARNING, 'U kunt de root directory geen andere naam geven.', 'Waarschuwing');
                //}
                
            ///}
            
        }
    },

    CurrentSelectedImageIndex: null,

    showThumbnailer: function () {
        var selectedFiles = Array();
        $('.checkFile').each(function (i) {
            if ($(this).is(":checked")) {
                var fileName = $(this).attr('title').toLowerCase();
                if (fileName.match(/gif$/) || fileName.match(/png$/) || fileName.match(/jpg$/) || fileName.match(/jpeg$/)) {
                    selectedFiles.push(fileName);
                }
            }
        });

        if (selectedFiles.length > 0) {
            BITFILEMANAGEMENT.thumbnailParameterObject = {
                'imageCount': selectedFiles.length,
                'imageHeight': '100',
                'imageWidth': '100',
                'images': Array(),
                'currentFolder': BITFILEMANAGEMENT.currentFolder,
                'cropEnabled': false
            }
            //$('#imageThumbnailDialog').dataBind(BITFILEMANAGEMENT.thumbnailParameterObject);
            $('#imageThumbnailDialog').initDialog(null, { width: 1100, height: 700 }, false, {
                'Genereer thumbnails': function () {
                    var valid = true;
                    parametersObject = $('#imageThumbnailDialog').collectData(BITFILEMANAGEMENT.thumbnailParameterObject);
                    if (!$('#justThumb').is(':checked')) {
                        for (var i in parametersObject.images) {
                            var image = parametersObject.images[i];
                            if (image.x == null || image.previewZoomWidth == 0) {
                                valid = false;
                                ////console.log($('#thumbnailImageList').find('a:contains(' + image.name + ')'));
                                $('#thumbnailImageList').find('a:contains(' + image.name + ')').css('border', 'red solid 1px');
                            }
                            else {
                                $('#thumbnailImageList').find('a:contains(' + image.name + ')').css('border', 'none');
                            }
                        }
                    }
                    if (valid) {
                        var jsonstring = JSON.stringify(BITFILEMANAGEMENT.thumbnailParameterObject);
                        BITAJAX.dataServiceUrl = "FileService.asmx";
                        BITAJAX.callWebService("CreateThumbnails", jsonstring, function (data) {
                            $('#imageThumbnailDialog').dialog('close');
                            BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder, 'ASC', true);
                        });
                    }
                    else {
                        $.messageBox(MSGBOXTYPES.INFO, 'Bij 1 of meerdere afbeeldingen is geen selectie opgegeven.');
                    }
                }
            });
            var ID = 0;
            $(selectedFiles).each(function () {
                BITFILEMANAGEMENT.thumbnailParameterObject['images'].push({
                    'id': ID,
                    'name': this.toString(),
                    'height': 100,
                    'width': 100,
                    'x': null,
                    'y': null,
                    'x2': null,
                    'y2': null,
                    'previewZoomWidth': 0,
                    'previewZoomHeight': 0,
                    'aspectRatio': true
                });
                ID++;
            });
            $('#thumbnailImageList').find('ul').dataBindList(BITFILEMANAGEMENT.thumbnailParameterObject['images']);
            BITFILEMANAGEMENT.selectImageToThumbnail(0);
            $('#imageThumbnailDialog').dialog({
                'close': function () {
                    var JcropAPI = $('#jcroppper').data('Jcrop');
                    if (JcropAPI) {
                        JcropAPI.destroy();
                    }
                }
            });
            $('#imageThumbnailDialog').dialog('open');
            $('#imgThumbnailPreview').parent().resizable({
                maxHeight: 500,
                maxWidth: 350,
                minHeight: 1,
                minWidth: 1,
                resize: function (e, ui) {
                    BITFILEMANAGEMENT.thumbnailParameterObject.images[BITFILEMANAGEMENT.CurrentSelectedImageIndex].height = ui.size.height;
                    BITFILEMANAGEMENT.thumbnailParameterObject.images[BITFILEMANAGEMENT.CurrentSelectedImageIndex].width = ui.size.width;
                    $('#thumbHeight').val(ui.size.height);
                    $('#thumbWidth').val(ui.size.width);
                },
                stop: function () {
                    BITFILEMANAGEMENT.selectImageToThumbnail(BITFILEMANAGEMENT.CurrentSelectedImageIndex);
                }
            });
        }
        else {
            $.messageBox(MSGBOXTYPES.INFO, 'U heeft geen afbeeldingen in uw bestandsselectie.');
        }
    },

    selectImageToThumbnail: function (index) {
        index = (index == undefined) ? 0 : index;
        if (index != BITFILEMANAGEMENT.CurrentSelectedImageIndex) {
            BITFILEMANAGEMENT.CurrentSelectedImageIndex = index;
            $('#keepRatio').attr('checked', BITFILEMANAGEMENT.thumbnailParameterObject['images'][index].aspectRatio);
        }
        var JcropAPI = $('#jcroppper').data('Jcrop');
        if (JcropAPI != null) {
            JcropAPI.destroy();
        }
        var image = BITFILEMANAGEMENT.thumbnailParameterObject['images'][index];
        $('#jcroppper').css({ 'height': '', 'width': '' });
        $('#jcroppper').attr('src', BITFILEMANAGEMENT.siteDomain + BITFILEMANAGEMENT.currentFolder.replace('//', '/') + '/' + image.name);
        $('#imgThumbnailPreview').parent().css({'height': image.height, 'width': image.width});
        $('#imgThumbnailPreview').attr('src', BITFILEMANAGEMENT.siteDomain + BITFILEMANAGEMENT.currentFolder.replace('//', '/') + '/' + image.name);
        
        var ratio = 0;
        if ($('#keepRatio').is(':checked')) {
            ratio = image.width / image.height
        }

        $('#thumbHeight').val(image.height);
        $('#thumbWidth').val(image.width);
        if ($('#justThumb').is(':checked'))
        {
            $('#imgThumbnailPreview').show();
            $('#jcroppper').show();
            $('#jcroppper').Jcrop({
                onChange: BITFILEMANAGEMENT.showThumnailPreview,
                onSelect: BITFILEMANAGEMENT.showThumnailPreview,
                aspectRatio: ratio,
                bgColor: 'transparant',
                setSelect: [image.x, image.y, image.x2, image.y2]
            });
            $('#thumbHeightContainer').show();
        }
        else {
            $('#imgThumbnailPreview').hide();
            $('#jcroppper').hide();
            if ($('#keepRatio').is(':checked')) {
                $('#thumbHeightContainer').hide();
            }
            else {
                $('#thumbHeightContainer').show();
            }
        }
    },

    showThumnailPreview: function (coords)
    {
        var image = BITFILEMANAGEMENT.thumbnailParameterObject.images[BITFILEMANAGEMENT.CurrentSelectedImageIndex];
        var rx = image.width / coords.w;
        var ry = image.height / coords.h;

        image.previewZoomWidth = Math.round(rx * $('#jcroppper').width());
        image.previewZoomHeight = Math.round(ry * $('#jcroppper').height());

        image.previewZoomWidth = (image.previewZoomWidth == Infinity) ? 0 : image.previewZoomWidth;
        image.previewZoomHeight = (image.previewZoomHeight == Infinity) ? 0 : image.previewZoomHeight;

        $('#imgThumbnailPreview').css({
            width: image.previewZoomWidth + 'px',
            height: image.previewZoomHeight + 'px',
            marginLeft: '-' + Math.round(rx * coords.x) + 'px',
            marginTop: '-' + Math.round(ry * coords.y) + 'px'
        });

        image.x = Math.round(coords.x);
        image.y = Math.round(coords.y);
        image.x2 = Math.round(coords.x2);
        image.y2 = Math.round(coords.y2);
    },

    setThumbnailSize: function () {
        var height = $('#thumbHeight').val();
        var width = $('#thumbWidth').val();
        var succes = true;
        if ($.isNumeric(height) && $.isNumeric(width)) {
            if (height > 0 && width > 0 && width <= 350 && height <= 500) {
                BITFILEMANAGEMENT.thumbnailParameterObject.images[BITFILEMANAGEMENT.CurrentSelectedImageIndex].height = $('#thumbHeight').val();
                BITFILEMANAGEMENT.thumbnailParameterObject.images[BITFILEMANAGEMENT.CurrentSelectedImageIndex].width = $('#thumbWidth').val();
                BITFILEMANAGEMENT.selectImageToThumbnail(BITFILEMANAGEMENT.CurrentSelectedImageIndex);
            }
            else {
                succes = false;
            }
        }
        else {
            succes = false;
        }

        if (!succes) {
            $.messageBox(MSGBOXTYPES.INFO, 'De nieuwe afmetingen konden niet worden toegepast. <br />De maten moeten nummeriek en groter zijn dan 1. <br />De maten mogen voor de breete niet groter dan 350 en voor de hoogte niet hoger dan 500');
        }
    },

    toggleThumbMethod: function () {
        BITFILEMANAGEMENT.selectImageToThumbnail(BITFILEMANAGEMENT.CurrentSelectedImageIndex);
        BITFILEMANAGEMENT.thumbnailParameterObject.cropEnabled = $('#justThumb').is(':checked');
    },

    showFile: function (name, link) {
        var src = BITFILEMANAGEMENT.currentFolder + "/" + name;
        src = src.replace(/\\/g, "/") .replace('//', '/');
        var parametersObject = { filePath: src };
        var jsonstring = JSON.stringify(parametersObject);
        if (BITFILEMANAGEMENT.CodeMirrorEditor != null) {
            BITFILEMANAGEMENT.CodeMirrorEditor.toTextArea();
            BITFILEMANAGEMENT.CodeMirrorEditor = null;
        }
        BITAJAX.callWebService("OpenFile", jsonstring, function (data) {
            switch (data.d.Extension.toLowerCase()) {
                case '.html':
                case '.htm':
                case '.txt':
                    $('#file-editor').val(data.d.Value);
                    BITFILEMANAGEMENT.CodeMirrorEditor = CodeMirror.fromTextArea(document.getElementById('file-editor'), { mode: "text/html", tabMode: "indent" });
                    $('#edit-dialog').initDialog(function () { }, null, false, {
                        'Sluiten': function () {
                            $('#edit-dialog').dialog('close');
                        }});
                    $('#edit-dialog').dialog('open');
                    BITFILEMANAGEMENT.CodeMirrorEditor.refresh();
                    break;

                case '.js':
                    $('#file-editor').val(data.d.Value);
                    BITFILEMANAGEMENT.CodeMirrorEditor = CodeMirror.fromTextArea(document.getElementById('file-editor'), { mode: "javascript", tabMode: "indent" });
                    $('#edit-dialog').initDialog(function () { }, null, false, {
                        'Sluiten': function () {
                            $('#edit-dialog').dialog('close');
                        }
                    });
                    $('#edit-dialog').dialog('open');
                    BITFILEMANAGEMENT.CodeMirrorEditor.refresh();
                    break;

                case '.css':
                    $('#file-editor').val(data.d.Value);
                    BITFILEMANAGEMENT.CodeMirrorEditor = CodeMirror.fromTextArea(document.getElementById('file-editor'), { mode: "css", tabMode: "indent" });
                    $('#edit-dialog').initDialog(function () { }, null, false, {
                        'Sluiten': function () {
                            $('#edit-dialog').dialog('close');
                        }});
                    $('#edit-dialog').dialog('open');
                    BITFILEMANAGEMENT.CodeMirrorEditor.refresh();
                    break;

                case '.jpeg':
                case '.jpg':
                case '.png':
                case '.gif':
                    $('#image-dialog').initDialog(function () { }, false, null, {
                        'Sluiten': function () {
                            $('#image-dialog').dialog('close');
                        }
                    });
                    $('#image-dialog').formEnrich();
                    $('#image').attr('src', data.d.Value);
                    $('#image-dialog').dialog('open');
                    break;

                case '.pdf':
                    $('#pdf-viewer-dialog').initDialog();
                    $('#pdf-viewer').attr('src', data.d.Value);
                    $('#pdf-viewer-dialog').dialog('open');
                    break;

                default:
                    $.messageBox(MSGBOXTYPES.INFO, 'Dit bestands type kan niet worden geopend.', 'Onbekend bestand formaat');
                    break;
             
            }
        });
    },

    selectAll: function (checkbox) {
        $(".checkFile").each(function (i) {
            if ($(checkbox).is(':checked')) {
                $(this).attr("checked", true);
            }
            else {
                $(this).attr("checked", false);
            }
        });
    },

    sort: function (sortField) {
        DATABINDER.sort(sortField);
        BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder);
    },

    putIntoClipboard: function (type) {
        //unmake all gray
        var rows = $("#tableFiles").find('td, a');
        rows.each(function (i) {
            $(rows[i]).css("color", "");
        });

        var selectedFiles = [];
        var files = $('.checkFile');
        files.each(function (i) {
            if ($(files[i]).is(":checked")) {
                selectedFiles.push($(files[i]).attr('title'));
                if (type == "cut") {
                    //make gray
                    var x = $(files[i]).parent().parent().find('td, a');
                    x.each(function (n) {
                        $(x[n]).css("color", "gray");
                    });
                }
            }
        });
        if (selectedFiles.length > 0) {
            clipBoard = { type: type, folderName: BITFILEMANAGEMENT.currentFolder, files: selectedFiles };
            $('#clibbordLength').html(selectedFiles.length);
        }
    },

    copy: function () {
        BITFILEMANAGEMENT.putIntoClipboard("copy");
    },

    cut: function () {
        BITFILEMANAGEMENT.putIntoClipboard("cut");
    },

    paste: function () {
        BITAJAX.dataServiceUrl = "FileService.asmx";
        if (clipBoard) {
            var parametersObject = { oldFolderName: clipBoard.folderName, newFolderName: BITFILEMANAGEMENT.currentFolder, files: clipBoard.files };
            var jsonstring = JSON.stringify(parametersObject);
            if (clipBoard.type == "cut") {
                BITAJAX.callWebService("MoveFiles", jsonstring, function (data) {
                    BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder, 'ASC', true);
                },
                function () {
                    BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder, 'ASC', true);
                });
            }
            else if (clipBoard.type == "copy") {
                BITAJAX.callWebService("CopyFiles", jsonstring, function (data) {
                    BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder, 'ASC', true);
                },
                function () {
                    BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder, 'ASC', true);
                });
            }
            clipBoard = null;
            $('#clibbordLength').html('0');
        }
    },

    remove: function () {
        var parametersObject = { folderName: BITFILEMANAGEMENT.currentFolder };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.dataServiceUrl = "FileService.asmx";
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
            msg = "Wilt u deze bestanden en/of mappen verwijderen?";
            $.deleteConfirmBox(msg, 'Bestand(en) Verwijderen', function () {
                var parametersObject = { folderName: BITFILEMANAGEMENT.currentFolder, files: selectedFiles };
                var jsonstring = JSON.stringify(parametersObject);
                BITAJAX.callWebService(remoteFunction, jsonstring, function (data) {
                    //console.log(BITFILEMANAGEMENT.currentFolder);
                    BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder, 'ASC', true);
                    BITFILEMANAGEMENT.loadTree();
                });
            });
        }
    },

    newFolder: function () {
        $.inputBox.setValidation('required');
        $.inputBox('Naam:', 'Nieuwe Map', '', function (e, value) {
            var parametersObject = { parentFolder: BITFILEMANAGEMENT.currentFolder, folderName: value };
            var jsonstring = JSON.stringify(parametersObject);
            BITAJAX.dataServiceUrl = "FileService.asmx";
            BITAJAX.callWebService("AddFolder", jsonstring, function (data) {
                BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder, 'ASC', true);
                BITFILEMANAGEMENT.loadTree();
            });
        });
    },

    saveJStreeDropFolder: function (result) {
        var targetFolder = $(result.rslt.r.context).attr('data-path');
        
        var sourceFolders = [];
        $(result.rslt.o).each(function (i) {
            var sourceFolder = $(result.rslt.o[i]).find('a').attr('data-path');
            sourceFolders[sourceFolders.length] = sourceFolder;
            var destination = targetFolder + sourceFolder.substr(sourceFolder.lastIndexOf('\\'));
            $(result.rslt.o[i]).find('a').attr('data-path', destination);
        });
        var parametersObject = { targetFolder: targetFolder, sourceFolders: sourceFolders };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("MoveFolders", jsonstring, function (data) {
            
        });
    },

    saveJStreeDragFile: function (result) {
        var targetFolder = $(result.r.context).attr('data-path');
        var files = [];

        $(".checkFile").each(function (i) {
            if ($(this).is(':checked')) {
                var fileRecord = $(this).parent().parent().find('input').attr('title');
                files[files.length] = BITFILEMANAGEMENT.currentFolder + '\\' + fileRecord;
            }
        });

        if (files.length == 0) {
            var fileRecord = $(result.o).parent().find('input').attr('title');
            files[files.length] = BITFILEMANAGEMENT.currentFolder + '\\' + fileRecord;
        }

        var parametersObject = { targetFolder: targetFolder, sourceFiles: files };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("DragMoveFiles", jsonstring, function (data) {
            BITFILEMANAGEMENT.getFiles(BITFILEMANAGEMENT.currentFolder, 'ASC', true);
        });

    },

    /* loadTree: function () {
        BITAJAX.dataServiceUrl = "FileService.asmx";
        var parametersObject = { searchString: '' };
        var jsonstring = JSON.stringify(parametersObject);
        BITAJAX.callWebService("BuildTree", jsonstring, function (data) {
            $('#treeView').html(data.d); */
            /* $('#treeView').treeview({
                sortable: true,
                sortstop: function (data) {
                    var jsonstring = JSON.stringify(data);
                    BITAJAX.callWebService("SaveOrderingNummerItem", jsonstring, null);
                    //als zelfde item is geselecteerd dan veld volgnummer updaten
                    if (BITDATACOLLECTIONDATA.loadedDomainObject && BITDATACOLLECTIONDATA.loadedDomainObject.ID == data.id) {
                        $("input[data-field=OrderingNumber]").val(data.orderingnumber);
                    }
                },
                selectedIndexChanged: function (id, type) {
                    //javascript: BITFILEMANAGEMENT.getFiles('/', 'ASC', true)
                    if (id == 'treeRoot' || id == '') return;
                    if (type == "item") {
                        //BITDATACOLLECTIONDATA.loadItem(id);
                    }
                    else if (type == "group") {
                            //BITDATACOLLECTIONDATA.loadGroup(id);
                    }
                    else if (type == "root") {
                            // BITDATACOLLECTIONDATA.loadCollectionDetails(id);
                    }
                }
            }); */
        //});
    //},

    showTree: function () {
        BITFILEMANAGEMENT.isTreeVisible = true;
        $("#treeView").show();
        // orange border set 
        $('.bitEndTable').css('width', '768px');
        $('.bitGridFillTD').css('width', '0px');
        $('.bitGrid').css('width', '768px');
        if ($("#treeView").html() == "") { BITFILEMANAGEMENT.loadTree(); }
        var html = "<a href='javascript:BITFILEMANAGEMENT.hideTree();' class='bitNavBarButtonHideTree'></a><div>Verb. tree</div>";
        $("#toolbarMenuItemShowTree").html(html);
        tableData
        $("#tableData .bitTitleColumn").hide();
        $("#tableData .bitStatusColumn").hide();
        $("#tableData .bitTableDateColumn").hide();

        $("#tableView").width(670);
    },

    hideTree: function () {
        BITFILEMANAGEMENT.isTreeVisible = false;
        $("#treeView").hide();
        $('.bitEndTable').css('width', '1000px');
        $('.bitGridFillTD').css('width', '232px');
        $('.bitGrid').css('width', '1000px');
        var html = "<a href='javascript:BITFILEMANAGEMENT.showTree();' class='bitNavBarButtonShowTree'></a><div>Toon tree</div>";
        $("#toolbarMenuItemShowTree").html(html);
        $("#tableData .bitTitleColumn").show();
        $("#tableData .bitStatusColumn").show();
        $("#tableData .bitTableDateColumn").show();
        $("#tableView").width(1000);
    },

    ckSelect: function () {
        var files = [];
        $(".checkFile").each(function (i) {
            if ($(this).is(':checked')) {
                var fileRecord = $(this).parent().parent().find('input').attr('title');
                if (fileRecord.match(/gif$/) || fileRecord.match(/png$/) || fileRecord.match(/jpg$/) || fileRecord.match(/jpeg$/)) {
                    files[files.length] = BITFILEMANAGEMENT.siteDomain + BITFILEMANAGEMENT.currentFolder.replace('/', '') + '/' + fileRecord;
                }
            }
        });
        // Get CKEditorFuncNum from page url
        if (files.length == 1) {
            var link = files[0];
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
        else {
            if (files.length > 1) {
                $.messageBox(MSGBOXTYPES.WARNING, 'U kunt maximaal 1 bestand selecteren.');
            }
            else {
                $.messageBox(MSGBOXTYPES.WARNING, 'U kunt dient een bestand te selecteren.');
            }
        }
    }

}