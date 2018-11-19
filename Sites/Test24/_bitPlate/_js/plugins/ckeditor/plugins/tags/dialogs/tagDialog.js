CKEDITOR.dialog.add('tagDialog', function (editor) {
    return {
        title: 'Insert Tag',
        minWidth: 400,
        minHeight: 200,
        onShow: function (evt) {
            //Remove background cover.
            $('.cke_dialog_background_cover').fadeOut('fast');
        },
        buttons: [],
        contents: [
            {
                id: 'tab1',
                label: 'Basic Settings',
                elements: [
                    // UI elements of the first tab will be defined here 
                    /* {
                        type: 'text',
                        id: 'tag',
                        label: 'Tag',
                        validate: CKEDITOR.dialog.validate.notEmpty("Field cannot be empty")
                    }, */
                    {
                        type: 'html',
                        id: 'ckTagList',
                        html: '<div id="tagList" style="max-height: 250px; overflow: auto;"></div>',
                        onShow: function (evt) {
                            var dialogDocument = evt.sender.getElement().getDocument();
                            var element = dialogDocument.getById('tagList');
                            element.setHtml(tagList);
                            /* $('.tagToInsert').click(function () {
                                evt.sender.getContentElement('tab1', 'tag').setValue($(this).html());
                            }); */

                            $('.tagToInsert').dblclick(function () {
                                if (editor.mode == 'wysiwyg') {
                                    editor.insertText($(this).html());
                                }
                                else {
                                    var TextToInsert = $(this).html();
                                    if (editor.config.extraPlugins.indexOf("codemirror") !== -1) {
                                        var codemirror = window["codemirror_" + editor.id];
                                        codemirror.replaceSelection(TextToInsert, 'start');
                                        var currentCursorPosition = codemirror.getCursor();
                                        codemirror.setCursor(currentCursorPosition);
                                        //console.log(codemirror.getSelection());
                                    }
                                    else {
                                        var input = $('#' + editor.element.$.id).parent().find('.cke_source')[0];
                                        input.focus();

                                        if (typeof input.selectionStart != 'undefined') {
                                            /* Einfügen des Formatierungscodes */
                                            var start = input.selectionStart;
                                            var end = input.selectionEnd;

                                            input.value = input.value.substr(0, start) + TextToInsert + input.value.substr(end);
                                            /* Anpassen der Cursorposition */
                                            var pos;

                                            pos = start + TextToInsert.length;

                                            input.selectionStart = pos;
                                            input.selectionEnd = pos;
                                        }
                                    }
                                    
                                }
                            });
                        }
                    }
                ]
            }
        ]
    };
});