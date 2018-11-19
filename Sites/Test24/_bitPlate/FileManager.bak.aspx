<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/bitMasterPages/Grids.master" AutoEventWireup="true" CodeBehind="FileManager.bak.aspx.cs" Inherits="BitSite._bitPlate.FileManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="_themes/bitplate/plugins/jsTree/themes/default/style.css" />
    <link rel="stylesheet" type="text/css" href="_js/plugins/jqContextMenu/jquery.contextMenu.css" />
    <link rel="stylesheet" type="text/css" href="_js/plugins/fileUpload/css/jquery.fileupload-ui.css" />
    <link rel="stylesheet" type="text/css" href="_js/plugins/fileUpload/css/jquery.image-gallery.min.css" />
    <link rel="stylesheet" type="text/css" href="_js/plugins/fileUpload/css/style.css" />
    <link rel="stylesheet" type="text/css" href="_themes/bitplate/css/filemanager.css" />
    <%= BitBundler.ResolveBundleUrl("~/Plugins/FileUpload") %>
    <%= BitBundler.ResolveBundleUrl("~/Plugins/jsTree") %>
    <link rel="stylesheet" type="text/css" href="_js/plugins/CodeMirror-2.32/lib/codemirror.css" />
    <%= BitBundler.ResolveBundleUrl("~/Plugins/CodeMirror") %>
    <script type="text/javascript" src="_js/plugins/jqContextMenu/jquery.contextMenu.js"></script>
    <script type="text/javascript" src="_js/pages/BITFILEMANAGER.js"></script>
    <style type="text/css">
      .CodeMirror {
        border: 1px solid #eee;
      }
      .CodeMirror-scroll {
        height: auto;
        overflow-y: hidden;
        overflow-x: auto;
      }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <!-- icons -->
    <ul>
        <li><a href="Default.aspx" class="bitNavBarButtonBack" runat="server" id="BackLink">back</a>
            <div>Terug</div>
        </li>
        <li id="liFileUpload" runat="server"><a id="aFileUpload" runat="server" href="javascript:BITFILEMANAGEMENT.showUpload();" class="bitNavBarButtonUpload">Upload</a>
            <div>Upload</div>
        </li>
        <li id="liFileDownload" runat="server"><a id="aFileDownload" runat="server" href="javascript:BITFILEMANAGEMENT.downloadFiles();" class="bitNavBarButtonDownload">Download</a>
            <div>Download</div>
            <iframe id="download-frame" style="display: none;"></iframe>
        </li>
        <li id="liFileRename" runat="server"><a id="aFileRename" runat="server" href="javascript:BITFILEMANAGEMENT.showRenameFile();" class="bitNavBarButtonEditName">Naam Wijzigen</a>
            <div>Naam Wijzigen</div>
        </li>
        <li id="liFileCut" runat="server"><a id="aFileCut" runat="server" href="javascript:BITFILEMANAGEMENT.cut();" class="bitNavBarButtonCut">Knippen</a>
            <div>Knippen</div>
        </li>
        <li id="liFileCopy" runat="server"><a id="aFileCopy" runat="server" href="javascript:BITFILEMANAGEMENT.copy();" class="bitNavBarButtonCopy">Kopiëren</a>
            <div>Kopiëren</div>
        </li>
        <li id="liFilePaste" runat="server"><a id="aFilePaste" runat="server" href="javascript:BITFILEMANAGEMENT.paste();" class="bitNavBarButtonPaste">Plakken</a>
            <div>Plakken</div>
        </li>
        <li id="liRemoveFile" runat="server"><a id="aRemoveFile" runat="server" href="javascript:BITFILEMANAGEMENT.remove();" class="bitNavBarButtonDelete">Verwijder</a>
            <div>Verwijder</div>
        </li>
        <li id="liCreateThumb" runat="server"><a id="aCreateThumb" runat="server" href="javascript:BITFILEMANAGEMENT.showThumbnailer();" class="bitNavBarButtonMakeThumb" onclick="javascript:BITFILEMANAGEMENT.showThumbnailer()">Maak Thumb.</a>
            <div>Maak Thumb.</div>
        </li>
        <li id="liAddFolder" runat="server"><a id="aAddFolder" runat="server" href="javascript:BITFILEMANAGEMENT.newFolder();" class="bitNavBarButtonAddFolder">Nieuwe Map</a>
            <div>Nieuwe Map</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Bestandsbeheer
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <!-- bitBox01 -->
    <div style="display: none;" class="bitBoxWrapper" id="bitBox01">
        <div class="bitBoxTitle">Mappen:</div>
        <div  class="bitBoxMain" id="bitBoxMain01" data-field="folders"></div>
    </div>
    <!-- .bitBox01 -->
    <!-- bitBox02 
    <div class="bitBoxWrapper" id="bitBox02">
        <div class="bitBoxTitle">Status</div>
        <div class="bitBoxMain" id="bitBoxMain02" style="text-align: left;">
            <div style="text-align: left;">Huidigemap: <span id="currentPath"></span></div>
            <div style="text-align: left;">Klembord: <span id="clibbordLength">0</span></div>
        </div>
    </div>
     .bitBox02 -->
    <!-- bitBox07 -->
    <div class="bitBoxWrapper" id="bitBox03">
        <div class="bitBoxTitle">Status:</div>
        <div class="bitBoxStatus">Huidigemap: <span id="Span1"></span></div>
        <div class="bitBoxStatus">Klembord: <span id="Span2">0</span></div>
        
        <div class="bitBoxMain" id="bitBoxMain07">
            <!-- icons -->
            <hr />
            <div class="bitBoxSubTitle">Bestanden:</div>
            <table id="tableFiles" class="bitGrid" data-sort-function="BITFILEMANAGEMENT.sort">
                <thead>
                    <tr>
                        <td id="bitGridTDCheckbox">
                            <input type="checkbox" onclick="javascript:BITFILEMANAGEMENT.selectAll(this);" />
                        </td>
                        <td data-sort-field="FileType" id="bitGridTDFileType"></td>
                        <td data-sort-field="Name" id="bitGridTDName">Naam</td>
                        <td data-sort-field="FileType" id="bitGridTDFileType2">Type</td>
                        <td data-sort-field="Volume" id="bitGridTDVolume">Grootte</td>
                        <td data-sort-field="ModifiedDate" id="bitGridTDModifiedDate">Datum wijziging</td>
                        <td data-sort-field="CreateDate" id="bitGridTDCreateDate">Datum Aanmaak</td>
                    </tr>
                </thead>
                <tbody>
                    <tr class="jstree-draggable">
                        <td>
                            <input type="checkbox" data-field="ID" data-title-field="Name" class="checkFile" />
                        </td>
                        <td><a data-field="Name" href="javascript:BITFILEMANAGEMENT.showFile('[data-field]', this);"
                            class="file-record">
                            <img data-field="ImageSrc" src="null" alt="" style="width: 30px; height: 30px" />
                        </a></td>
                        <td data-field="Name" id="bitGridTDBodyName" data-title-field="Name"></td>
                        <td data-field="FileType"></td>
                        <td data-field="Volume"></td>
                        <td data-field="ModifiedDate"></td>
                        <td data-field="CreateDate"></td>
                    </tr>
                </tbody>
            </table>
            <!-- .icons -->
        </div>
    </div>
    <!-- .bitBox07 -->
    <ul id="file-menu" class="contextMenu">
        <li class="edit "><a id="tbtn-download" href="#edit">Download</a></li>
        <li class="edit separator"><a id="tbtn-open" href="#edit">Open</a></li>
        <li class="cut separator"><a id="tbtn-knippen" href="#cut">Knippen</a></li>
        <li class="copy"><a id="tbtn-kopieren" href="#copy">Kopieren</a></li>
        <li class="paste"><a id="tbtn-plakken" href="#paste">Plakken</a></li>
        <li class="delete separator"><a id="tbtn-verwijderen" href="#delete">Verwijderen</a>
        </li>
    </ul>
    <ul id="folder-menu" class="contextMenu">
        <li class="edit separator"><a id="btn-open-folder" href="#edit">Open</a></li>
        <li class="edit separator"><a id="tbtn-naamwijzigen-folder" href="#edit">Naam Wijzigen
        </a></li>
        <li class="delete separator"><a id="tbtn-verwijder-folder" href="#delete">Verwijderen
        </a></li>
    </ul>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <div class="dialogs">
        <div id="upload-dialog" title="Bestanden Uploaden">
            <div id="divUploadFiles">
                <div class="container">
                    <!-- The file upload form used as target for the file upload widget -->
                    <form id="fileupload" action="bitAjaxServices/FileService.aspx?upload" method="POST"
                    enctype="multipart/form-data">
                    <!-- The fileupload-buttonbar contains buttons to add/delete files and start/cancel the upload -->
                    <div class="row fileupload-buttonbar">
                        <div class="span7">
                            <!-- The fileinput-button span is used to style the file input field as button -->
                            <span class="btn btn-success fileinput-button"><i class="icon-plus icon-white"></i><span>
                                Add files...</span>
                                <input type="file" name="files[]" multiple>
                            </span>
                            <button type="submit" class="btn btn-primary start">
                                <i class="icon-upload icon-white"></i><span>Start upload</span>
                            </button>
                            <button type="reset" class="btn btn-warning cancel">
                                <i class="icon-ban-circle icon-white"></i><span>Cancel upload</span>
                            </button>
                            <button type="button" class="btn btn-danger delete">
                                <i class="icon-trash icon-white"></i><span>Delete</span>
                            </button>
                            <input type="checkbox" class="toggle">
                        </div>
                        <!-- The global progress information -->
                        <div class="span5 fileupload-progress fade">
                            <!-- The global progress bar -->
                            <div class="progress progress-success progress-striped active" role="progressbar"
                                aria-valuemin="0" aria-valuemax="100">
                                <div class="bar" style="width: 0%;"></div>
                            </div>
                            <!-- The extended global progress information -->
                            <div class="progress-extended">&nbsp;</div>
                        </div>
                    </div>
                    <!-- The loading indicator is shown during file processing -->
                    <div class="fileupload-loading"></div>
                    <br>
                    <!-- The table listing the files available for upload/download -->
                    <table role="presentation" class="table table-striped">
                        <tbody class="files" data-toggle="modal-gallery" data-target="#modal-gallery"></tbody>
                    </table>
                    </form>
                </div>
                <!-- The template to display files available for upload -->
                <script id="template-upload" type="text/x-tmpl">
                {% for (var i=0, file; file=o.files[i]; i++) { %}
                    <tr class="template-upload fade">
                        <td class="preview"><span class="fade"></span></td>
                        <td class="name"><span>{%=file.name%}</span></td>
                        <td class="size"><span>{%=o.formatFileSize(file.size)%}</span></td>
                        {% if (file.error) { %}
                            <td class="error" colspan="2"><span class="label label-important">{%=locale.fileupload.error%}</span> {%=locale.fileupload.errors[file.error] || file.error%}</td>
                        {% } else if (o.files.valid && !i) { %}
                            <td>
                                <div class="progress progress-success progress-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100" aria-valuenow="0"><div class="bar" style="width:0%;"></div></div>
                            </td>
                            <td class="start">{% if (!o.options.autoUpload) { %}
                                <button class="btn btn-primary">
                                    <i class="icon-upload icon-white"></i>
                                    <span>{%=locale.fileupload.start%}</span>
                                </button>
                            {% } %}</td>
                        {% } else { %}
                            <td colspan="2"></td>
                        {% } %}
                        <td class="cancel">{% if (!i) { %}
                            <button class="btn btn-warning">
                                <i class="icon-ban-circle icon-white"></i>
                                <span>{%=locale.fileupload.cancel%}</span>
                            </button>
                        {% } %}</td>
                    </tr>
                {% } %}
                </script>
                <!-- The template to display files available for download -->
                <script id="template-download" type="text/x-tmpl">
                </script>
                <!--<h2>Upload bestanden</h2>
                <form method="post" id="form1" name="form1" action="bitAjaxServices/FileService.aspx" enctype="multipart/form-data">
                    <input type="file" name="file1" id="file1" multiple="multiple" onchange="javascript:BITFILEMANAGEMENT.fileUpload(this.form,'bitAjaxServices/FileService.aspx/SaveFiles','upload');" />
                    <input type="button" value="Verstuur" onclick="javascript:BITFILEMANAGEMENT.send(); return false;" />
                    <br />
                    <div id="upload"></div>
                    <iframe id="iframeUpload" name="iframeUpload" style="border: 0px; width: 600px; display: none" frameborder="0"></iframe>
                </form>-->
            </div>
        </div>

            <div id="new-folder-dialog" title="Nieuwe Map" style="display:none;">
                <input data-field="folderName" data-validation="required" type="text" name="foldername"
                    id="new-foldername" />
            </div>
            <!--<div id="image-dialog" title="Image Viewer" >
                <div style="height: 40px;">
                    <div class="button">Thumbnail</div>
                    <div class="button">Crop</div>
                </div>
                <div style="text-align: center;">
                    <img id="image" alt="foto" />
                </div>
                     
            </div>-->

            <div id="image-dialog" title="Image Viewer" style="text-align: center; display:none;" >
                <img id="image" alt="foto" />
            </div>

            <div id="imageThumbnailDialog" title="Create thumbnail" style="display:none;">
                <div>
                    <div>Aantal afbeeldingen</div>
                    <div data-field="imageCount"></div>
                </div>
                <!--<div>
                    <div>Height</div>
                    <div><input data-field="imageHeight" type="text" style="width: 100px" /></div>
                </div>-->
                <div>
                    <div>Width</div>
                    <div><input id="tumbnailWith" class="tumbnailWith" data-field="imageWidth" runat="server" type="text" style="width: 100px" /></div>
                </div>
            </div>

            <div id="edit-dialog" title="File Editor" style=" display:none;">
                <textarea id="file-editor"></textarea>
            </div>
            <div id="pdf-viewer-dialog" title="PDF Reader" style="display:none;">
                <iframe style="width: 98%; height: 500px" id="pdf-viewer"></iframe>
            </div>
</asp:Content>
