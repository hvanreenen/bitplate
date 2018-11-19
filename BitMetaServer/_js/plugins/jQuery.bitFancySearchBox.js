/*
-----::: ABCnet.nl (c) 2012 :::-----
DATE: 23/08/2012
EDITED: 23/08/2012
DESCRIPTION: This is the JavaScript required to fancy searchbox Requires jQuery library .keydown
*/
$(document).ready(

	function()
		{
			$('#bitTextboxSearch').focusin(
				function()
					{
						tVal = $(this).val();
						if ( tVal != '' )
							{
								$('a.bitTextboxResetX').fadeIn('slow');
							}
					});
			$('#bitTextboxSearch').focusout(
				function()
					{
						$('.bitTextboxResetX').hide('fast');
						
					});
			$('#bitTextboxSearch').keydown(
				function()
					{
						$('a.bitTextboxResetX').fadeIn('slow');
					})
		});