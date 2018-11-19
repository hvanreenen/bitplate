/*
-----::: ABCnet.nl (c) 2012 :::-----
DATE: 22/05/2012
EDITED: 17/07/2012
DESCRIPTION: This is the JavaScript required to showdivs Requires jQuery library 
*/
$(document).ready(
	
		function()
			{
				$("#theme_select").change(
					function()
						{
							var sel = [];
							$('#theme_select :selected').each(
								function(i, selected)
									{
										sel[i] = $(selected).val();
										
										if (sel == ("bitplate"))
											{
												var link = $("<link>");
												link.attr(
													{
														type: 'text/css',
														rel: 'stylesheet',
														href: '_themes/bitplate/css/login.css'
													});
													$("head").append( link ); 

											}
											else if (sel == ("vista"))
												{
													var link = $("<link>");
													link.attr(
														{
															type: 'text/css',
															rel: 'stylesheet',
															href: '_themes/vista/css/login.css'
														});
														$("head").append( link );
												}
												else
													{
														
													}
									});
						});
			});