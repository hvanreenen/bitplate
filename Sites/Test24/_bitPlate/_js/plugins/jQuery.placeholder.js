/*
-----::: ABCnet.nl (c) 2012 :::-----
DATE: 27/08/2012
EDITED: 27/08/2012
DESCRIPTION: Find all input with placeholder in IE
*/
$(document).ready(

	function()
		{
			if(!Modernizr.input.placeholder)
			{
			    $("input").each(
                    function ()
                    {
                        if ($(this).val() == "" && $(this).attr("placeholder") != "")
                        {
                            $(this).val($(this).attr("placeholder"));
                            $(this).focus(function ()
                            {
                                if ($(this).val() == $(this).attr("placeholder")) $(this).val("");
                            });
                            $(this).blur(function ()
                            {
                                if ($(this).val() == "") $(this).val($(this).attr("placeholder"));
                            });
                        }
                    });
			}
			
		});