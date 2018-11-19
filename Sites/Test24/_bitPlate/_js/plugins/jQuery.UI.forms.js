/*
-----::: ABCnet.nl (c) 2012 :::-----
DATE: 01/03/2012
EDITED: 30/07/2012
DESCRIPTION: This is the JavaScript required to showdivs Requires jQuery library
*/
$(document).ready(
	function () {

	    // Textarea vergrootknop
	    $('.bitTextareaEnlargeButton').click(function () {
	        $(this).toggleClass('bitTextareaShrinkButton');
	        $(this).parent().next().find("textarea").toggleClass('bitTextareaLarge');
	    });
	    // Textarea verkleinknop
	    $('.bitTextareaShrinkButton').click(function () {
	        $(this).toggleClass('bitTextareaEnlargeButton');
	        $(this).parent().next().find("textarea").toggleClass('bitTextarea');
	    });

	    // create datepickers
	    $('input[type=date]').datepicker();


	    // on select change
	    $('#bitMapSelect').change(
				function () {
				    var bitSelectValue = $(this).val();
				    if (bitSelectValue == 'onlineFrom') {
				        $('.bitHide').show('slow');
				    }
				    else {
				        $('.bitHide').hide('slow');
				    }

				});
	    // tabs
	    $(".bitTabs").tabs();

	});
