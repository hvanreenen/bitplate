$(document).ready(function ()
	{
		/*$('body').click(
			function()
				{
					$('body').css('cursor','default');
					$('.bitBoxMain ul li a').css('cursor','default');
					$('.ui-bitTooltip.hidden').removeClass('bitHelpHover');
				});
		*/
		$('a#bitHelp').toggle( function()
			{
				$('.ui-bitTooltip.hidden').addClass('bitHelpHover');
				//set all titles
				$('body').css('cursor','help');
				$('.bitBoxMain ul li a').css('cursor','help');
			},
			function()
				{
					$('body').css('cursor','auto');
					$('.bitBoxMain ul li a').css('cursor','auto');
					$('.ui-bitTooltip.hidden').removeClass('bitHelpHover');
					
				});
		$('.bitBoxMain li').hover(function ()
			{
				$('.bitHelpHover', this).fadeIn('slow');
			},
			function()
				{
					$('.bitHelpHover', this).fadeOut('slow');
			});
		$('.bitTableActionButton').hover(
                function()
                {
                    //alert('inside');
                    $('.bitHelpHover', this).fadeIn('slow');
                },
                function ()
                {
                    $('.bitHelpHover', this).fadeOut('slow');
                });
		
	});