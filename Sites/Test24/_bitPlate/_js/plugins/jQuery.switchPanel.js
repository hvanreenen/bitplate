/*
-----::: ABCnet.nl (c) 2012 :::-----
DATE: 17/07/2012
EDITED: 17/07/2012
DESCRIPTION: This is the JavaScript required to showdivs Requires jQuery library 
*/
$(document).ready(
	
		function()
			{
				$('#forget_password').click(
					function()
						{
					    $('#bitLoginWindowLogin').fadeOut('slow', 'linear');
					    $('#bitLoginWindowForgotPassword').fadeIn('slow', 'linear');
						});
					$('#back').click(
						function()
							{
						    $('#bitLoginWindowLogin').fadeIn('slow', 'linear');
						    $('#bitLoginWindowForgotPassword').fadeOut('slow', 'linear');
							});
			});