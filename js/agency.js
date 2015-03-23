/*!
 * Start Bootstrap - Agnecy Bootstrap Theme (http://startbootstrap.com)
 * Code licensed under the Apache License v2.0.
 * For details, see http://www.apache.org/licenses/LICENSE-2.0.
 */

dynNav = function () {
    var item = $("nav.navbar > .container > .navbar-collapse > ul.nav");

    item.find ("li > a").each (function () {
	$(this).text ($(this).attr ("long"));
    });

    var barh = $("nav.navbar > .container").height ();
    var itemh = item.height ();

    if (itemh != 0 && barh > itemh)
    {
	item.find ("li > a").each (function () {
	    var short = $(this).attr ("short");
	    if (short != undefined)
	    {
		$(this).text (short);
	    }
	});
    }
};

// jQuery for page scrolling feature - requires jQuery Easing plugin
$(function() {
    dynNav ();
    $('a.page-scroll').bind('click', function(event) {
        var $anchor = $(this);
        $('html, body').stop().animate({
            scrollTop: $($anchor.attr('href')).offset().top
        }, 1500, 'easeInOutExpo');
        event.preventDefault();
    });
    $("section#wiki a").each (function () {
        if (/^[a-zA-Z\-]+$/.test ($(this).attr ("href")))
            $(this).attr ("href", "../" + $(this).attr ("href"));
    });

    $('#toc').toc ({ noBackToTopLinks: true, title: '', showEffect: 'none', minimumHeaders: 0 });
    $('#tocbar #bartoggle').click (function () {
        $('#tocbar').toggleClass ("col");
    });
    $('#tocbar #toc *').click (function () {
        $('#tocbar').addClass ('col');
    });

    // smooth scrolling when clicking on anchor
    var $root = $('html, body');
    function jq (myid) {
        return myid.replace( /(:|\.|\[|\]|,|\?|\/)/g, "\\$1" );
    }
    $('#tocbar #toc a').click(function(){
        $root.animate({
            scrollTop: $(jq (decodeURIComponent ($.attr (this, 'href')))).offset ().top - 50
        }, 500);
        return false;
    });
});

// Highlight the top nav as scrolling occurs
$('body').scrollspy({
    target: '.navbar-fixed-top'
})

// Closes the Responsive Menu on Menu Item Click
$('.navbar-collapse ul li a').click(function() {
    $('.navbar-toggle:visible').click();
});

$('div.modal').on('show.bs.modal', function() {
	var modal = this;
	var hash = modal.id;
	window.location.hash = hash;
	window.onhashchange = function() {
		if (!location.hash){
			$(modal).modal('hide');
		}
	}
});

$(window).resize (dynNav);
