// JavaScript source code
$(document).ready(
    function () {
        $('.bitNavBarButtonBack').attr('id', 'uploadClick');
        $('#uploadClick').click(function () {
            alert('click');
            $('.elfinder .elfinder-button form input').trigger('click');
        });
    });
//ui-state-default elfinder-button ui-state-hover class="elfinder-button-icon "