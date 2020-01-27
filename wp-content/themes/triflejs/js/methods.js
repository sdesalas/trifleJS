/* Author:
M.Satrya - http://twitter.com/msattt
*/

var $ = jQuery.noConflict();
$(document).ready(function(){

    $( "#content article" ).fitVids();

    $(document).imagesLoaded(function(){

    	$( ".rslides" ).responsiveSlides({
    		pager: false,
			nav: true,
			pause: true,
			prevText: "&larr;",
			nextText: "&rarr;", 
    	});

    });

});