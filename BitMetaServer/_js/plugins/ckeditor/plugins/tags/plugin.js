CKEDITOR.plugins.add('tags', {
    icons: 'tags',
    init: function (editor) {
        // Plugin logic goes here...
        var command = editor.addCommand('tagDialog', new CKEDITOR.dialogCommand('tagDialog'));
        command.modes = { wysiwyg:1, source: 1 }
        editor.ui.addButton('Tags', {
            label: 'Insert Tag',
            command: 'tagDialog',
            toolbar: 'insert'
        });
        CKEDITOR.dialog.add('tagDialog', this.path + 'dialogs/tagDialog.js');
    }
});

var tagList;

var setTags = function (tags) {
    tagList = tags
}

var setTagArray = function (tagArray) {
    tagList = '';
    for (var i in tagArray) {
        tagList += '<div class="tagToInsert">' + tagArray[i] + '</div>';
    }
}