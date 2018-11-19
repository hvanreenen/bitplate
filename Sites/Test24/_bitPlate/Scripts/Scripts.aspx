<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="Scripts.aspx.cs" Inherits="BitSite._bitPlate.Scripts.Scripts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="BITSCRIPTS.js"></script>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/_bitplate/_themes/" + BitSite.SessionObject.CurrentBitplateUser.Theme + "/css/codemirror") %>
    <%--<link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/CodeMirror-3.14/lib/codemirror.css" />
    <link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/CodeMirror-3.14/addon/hint/show-hint.css" />
    <link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/CodeMirror-3.14/addon/lint/lint.css" />
    <link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/CodeMirror-3.14/addon/dialog/dialog.css" />--%>
    <script type="text/javascript" src="/_bitplate/_js/plugins/jquery.iframe-post-form.js"></script>
    <link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/fileUpload/css/jquery.fileupload-ui.css" />
    <!--<link rel="stylesheet" type="text/css" href="_js/plugins/fileUpload/css/jquery.image-gallery.min.css" />-->
    <link rel="stylesheet" type="text/css" href="/_bitplate/_js/plugins/fileUpload/css/style.css" />

    <style type="text/css">
        .CodeMirror {
            border: 1px solid black;
        }
        /* .lint-error {font-family: arial; font-size: 70%; background: #ffa; color: #a00; padding: 2px 5px 3px; }
      .lint-error-icon {color: white; background-color: red; font-weight: bold; border-radius: 50%; padding: 0 3px; margin-right: 7px;} */
    </style>

    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/Plugins/FileUpload") %>
    <%= BitSite._bitPlate.BitBundler.ResolveBundleUrl("~/Plugins/CodeMirror") %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        <li id="liAddScript" runat="server"><a id="aAddScript" runat="server" href="javascript:BITSCRIPTS.newScript();" class="bitNavBarButtonAddPage">addpage</a>
            <div>Nieuw script</div>
        </li>
        <li id="liUploadScript" runat="server"><a id="aUploadScript" runat="server" href="javascript:BITSCRIPTS.openUploadPanel();" class="bitNavBarButtonUpload">Upload</a>
            <div>Upload File</div>
        </li>

    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Scripts
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <table id="tableScripts" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 20px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 25px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 240px" data-sort-field="Name">Naam</td>
                <!--<td class="bitTableColumn" style="width: 100px" data-sort-field="DateTill">Status
                </td>-->
                <td class="bitTableActionButton">Bewerk</td>
                <td class="bitTableActionButton">Verwijder</td>
                <td class="bitTableActionButton">Kopieer</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon"><a class="bitTablePageIcon" data-field="ID" onclick="BITSCRIPTS.openDetailsPopup('[data-field]');" href="javascript:void(0);"></a></td>
                <td class="iconAuth"><a href="#" class="bitTableAuthIcon" style="display: none"></a></td>
                <td class="nameColumn bitTableColumnEllipsis" data-field="Name" data-title-field="Name"></td>
                <!--<td data-field="ChangeStatusString"></td>-->
                <td class="bitTableActionButton" id="tdScriptConfig" runat="server"><a id="aScriptConfig" runat="server" data-field="ID" onclick="BITSCRIPTS.openDetailsPopup('[data-field]');" href="javascript:void(0);"
                    class="bitEditButton"></a></td>
                <td class="bitTableActionButton" id="tdScriptRemove" runat="server"><a id="aScriptRemove" runat="server" data-field="ID" onclick="BITSCRIPTS.remove('[data-field]');" href="javascript:void(0);"
                    class="bitDeleteButton"></a></td>
                <td class="bitTableActionButton" id="tdScriptCopy" runat="server"><a id="aScriptCopy" runat="server" data-field="ID" data-title-field="Name" href="javascript:BITSCRIPTS.copy('[data-field]', '[data-title-field]');"
                    class="bitCopyButton"></a></td>
                <td class="bitTableDateColumn" data-field="CreateDate"></td>
            </tr>
        </tbody>
    </table>
    <div class="bitEndTable"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <div id="bitScriptDialog" title="Script eigenschappen" style="display: none">
        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageDetails1">Algemene instellingen</a></li>
                <li><a href="#tabPageDetails2">Inhoud</a></li>

            </ul>
            <!-- TAB1: -->
            <div id="tabPageDetails1" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Naam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="De naam van het script of stylesheet. Het script krijgt zelf het achtervoegsel .css of .js; deze hoeft u er niet bij te zetten.">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="textNaam" type="text" data-field="Name" data-validation="required" class="required" />
                    <input type="hidden" id="selectScriptType" />
                    <input data-field="ScriptType" type="hidden" id="ScriptType" />
                </div>
                <br clear="all" />
                <div id="divSystemValue" style="display: none">
                    <div class="bitPageSettingsCollumnA">Systeemwaarde</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Sommige script horen bij het systeem. Deze kunnen niet worden verwijderd of kan je de naam wijzigen">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        Ja
                    </div>
                    <br clear="all" />
                </div>
                <div class="bitPageSettingsCollumnA">Actief in editor:</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Zet 'actief in editor' uit wanneer u wilt dat dit script niet wordt geladen in de pagina tijdens de bewerk modus. Hiermee kunt u voorkomen dat het script conficteert met de scripts die de pagina nodig heeft vioor de bewerk-modus. ">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="checkbox" data-field="ActiveInEditor" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Geladen in gehele site:</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Script wordt geladen in alle pagina's van de site. In elke pagina staat een refentie naar dit script.">info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="checkbox" data-field="LoadInWholeSite" />
                </div>
                <br clear="all" />
                <div id="StylesheetProperties" runat="server">
                    <div class="bitPageSettingsCollumnA">Media attribute:</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Script wordt geladen in alle pagina's van de site. In elke pagina staat een refentie naar dit script.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="text" data-field="StylesheetMedia" />
                    </div>
                    <br clear="all" />
                </div>


                <!-- NEXT ROW -->
                <!--<div class="bitPageSettingsCollumnA">Soort</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="">
                        info</span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectType" data-field="ScriptType" onchange="BITSCRIPTS.changeScriptStyle()">
                        <option value="0">Stylesheet (css)</option>
                        <option value="1">Javascript (js)</option>
                    </select>
                </div>
                <br clear="all" />-->
                <!-- NEXT ROW -->
            </div>
            <div id="tabPageDetails2" class="bitTabPage">
                <div>
                    <textarea data-field="Content" id="textareaContent" style="width: 800px"></textarea>
                </div>
            </div>
        </div>
    <!--COPY SCRIPT DIALOG -->
        <div id="bitScriptsCopyDialog" title="Script Kopiëren." style="display: none">
            <div>Kopie scriptnaam:</div>
            <div><input type="text" id="CopyName" /></div>
        </div>

        <!--UPLOAD SCRIPT DIALOG--> 
        <div id="bitScriptUploadDialog" title="Script Uploaden" style="display: none">
            <form id="ScriptUploadForm" action="/_bitAjaxServices/PostHandler.ashx?service=BitSite._bitPlate.Scripts.ScriptService&method=UploadScript" method="post" enctype="multipart/form-data">
                <div id="SelectScript">
                    <div>Selecteer een script:</div>
                    <div><input type="file" name="SelectedScript" /></div>
                </div>
                <div id="UploadScript" style="display:none;">
                    <div>Bezig met uploaden.</div>
                    <div id="uploadProgressbar"></div>
                </div>
            </form>
        </div>

        <div id="bitScriptUploadDialogv2" title="Bestanden Uploaden" style="display: none;">
            <div id="divUploadFiles">
                <div class="container">
                    <!-- The file upload form used as target for the file upload widget -->
                    <form id="fileupload" action="/_bitAjaxServices/PostHandler.ashx?service=BitSite._bitPlate.Scripts.ScriptService&method=UploadScripts" method="POST"
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
                <form method="post" id="form1" name="form1" action="bitAjaxServices/FileService.asmx" enctype="multipart/form-data">
                    <input type="file" name="file1" id="file1" multiple="multiple" onchange="javascript:BITFILEMANAGEMENT.fileUpload(this.form,'bitAjaxServices/FileService.asmx/SaveFiles','upload');" />
                    <input type="button" value="Verstuur" onclick="javascript:BITFILEMANAGEMENT.send(); return false;" />
                    <br />
                    <div id="upload"></div>
                    <iframe id="iframeUpload" name="iframeUpload" style="border: 0px; width: 600px; display: none" frameborder="0"></iframe>
                </form>-->
            </div>
        </div>
</asp:Content>
