/*
-----::: ABCnet.nl (c) 2012 :::-----
DATE: 01/03/2012
EDITED: 01/03/2012
DESCRIPTION: This is the JavaScript required to showdivs Requires jQuery library 
*/
$(document).ready(

	function()
		{
			// HOVER MENU SLIDE IN
			$('#bitSideBar').hover(
				function()
					{
						$('#bitSideBar').css('width','166px');
						$('#bitSideBar ul li a').css('width','130px');
					},
					function()
						{
							$('#bitSideBar').css('width','50px');
							$('#bitSideBar ul li a').css('width','40px');
						});
						
		// just for samples
			$('#editButton').click(
				function()
					{
						
						$('.edit').show('slow');
					});
			
			$('#cancelButton').click(
				function()
					{
						$('.edit').hide('slow');
					});
					
		// check screen and set menu
		
		var sHeight = screen.height
		var sWidth = screen.availWidth
		
		//alert('height' + sHeight);
		
		if ((sHeight<=1024))
		{
			$('#bitSideBar').css('position','absolute');
			$('#bitSideBar').css('height','777px');
		}
		if ((sHeight<=1080) && $.browser.msie)
		{
			$('#bitSideBar').css('position','absolute');
			$('#bitSideBar').css('height','900px');
		}
		});