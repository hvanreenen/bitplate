<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailTemplateTab.aspx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.ContactFormModule.EmailTemplateTab" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="standardTabPageEmailTemplate" class="bitTabPage">
            <textarea name="ckeditor" data-field="Settings.ContactFormTemplate" ></textarea>
            <script type="text/javascript">
                var editor = CKEDITOR.replace('ckeditor', {
                    fullPage: true
                });
                editor.config.extraPlugins = 'tags';
                setTags('<div class="tagToInsert">[FormResult]</div>');
            </script>
        </div>
    </form>
</body>
</html>