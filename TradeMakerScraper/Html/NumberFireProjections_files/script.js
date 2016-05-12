




/*
     FILE ARCHIVED ON 13:15:15 Sep 5, 2015 AND RETRIEVED FROM THE
     INTERNET ARCHIVE ON 18:41:31 Apr 13, 2016.
     JAVASCRIPT APPENDED BY WAYBACK MACHINE, COPYRIGHT INTERNET ARCHIVE.

     ALL OTHER CONTENT MAY ALSO BE PROTECTED BY COPYRIGHT (17 U.S.C.
     SECTION 108(a)(3)).
*/
//common functionality for autocomplete
function split( val ) {
	return val.split( /,\s*/ );
}
function extractLast( term ) {
	return split( term ).pop();
}

function pushKMQ(value) {
	if (typeof _kmq != 'undefined') {
		_kmq.push(value);
	}
}

function pushGAQ(value) {
	if (typeof _gaq != 'undefined') {
		_gaq.push(value);
	}
}

function round(num, dec) {
	return Math.round(num*Math.pow(10,dec))/Math.pow(10,dec);
}

function setPreferredDFSite(site) {
	switch (site) {
		case 'fanduel':
		case 'draft_kings':
    	case 'fantasy_feud':
    	case 'draftday':
    	case 'fantasy_aces':
    	case 'draftster':
    	case 'fantasy_score':
    	case 'victiv':
    	case 'yahoo':
    		$.cookie("preferred_df_site", site, { expires: 14, path : '/' });
			break;
	}
}

function getPreferredDFSite() {
	switch ($.cookie("preferred_df_site")) {
		case 'fanduel':
		case 'draft_kings':
    	case 'fantasy_feud':
    	case 'draftday':
    	case 'fantasy_aces':
    	case 'draftster':
    	case 'fantasy_score':
    	case 'victiv':
    	case 'yahoo':
    		return $.cookie("preferred_df_site");
		default:
			return 'fanduel';
	}
}

function handleError(details) {
	var data = {
		d : details
	};
	
    $.ajax({
        type: "POST",
        url: "/error/handler",
        data: data,
        dataType : 'json',
		error : function() {},
        success: function(msg) {
        }
     });
    return false;
}

function sortDataTable(table_body, column, direction) {
	var rows = $('#' + table_body + ' tr').get();
	
	rows.sort(function(a, b) {
		var A = parseFloat($(a).children('td.col-'+column).text());
		var B = parseFloat($(b).children('td.col-'+column).text());
		if(A < B) { return -1 * direction; }
		if(A > B) { return 1 * direction; }
		return 0;
	});
	
	$('#'+table_body).html('');
	$.each(rows, function(index, row) {
		$('#'+table_body).append(row);
	});
	
	$("tbody[id="+table_body+"]>tr").removeClass('odd');
	$("tbody[id="+table_body+"]>tr:odd").addClass('odd');
}


$(document).ready(function () {

    /* fixing google chrome rendering bug of web fonts: /web/20150905131515/https://productforums.google.com/forum/#!topic/chrome/elw8busIfJA */
    $(function() { $('body').hide().show(); });

	$("abbr.timeago").timeago();
	
	/* functions here */
	var G = function () {
		var a = this;
		clearTimeout(a.out_id);
		a.over_id = setTimeout(function () {
			$(a).addClass("hover");
			a = null
		}, 20)
	};
	var H = function () {
		var a = this;
		clearTimeout(a.over_id);
		a.out_id = setTimeout(function () {
			$(a).removeClass("hover");
			a = null
		}, 20)
	};

    (function (a) {
        a.fn.simpletabs = function (b) {
            var c = {};
            var b = a.extend({}, c, b);
            return this.each(function () {
                var b = a(this);
                var c = a("ul.tab-nav", this).find("a").click(function () {
                    a(".tab-pane", b).removeClass("db");
                    a(".tab-pane", b).addClass("dn");
                    var d = a(this).attr("href");
                    var e = d.indexOf("#");
                    d = d.substring(e);
                    a(d, b).removeClass("dn").addClass("db");
                    c.removeClass("active");
                    a(this).addClass("active");
                    return false
                });
                a(".tab-pane", b).removeClass("db").addClass("dn");
                a(".tab-pane:first", b).addClass("db").removeClass("dn")
            })
        }
    })(jQuery);
		



		
	// nav hoverrrrrrrrrrrr
    	$("ul#primary li").hover(G, H);
    	$("ul.section-subnav li").hover(G, H);
    
    $("#search-box").autocomplete({
    	source: function(request, response) {
			data = {
				term : request.term
			};
			$.ajax({
				type: "GET",
				url: "/search",
				data: data,
				dataType : 'json',
				error : function() {},
				success: function(msg) {
					pushGAQ(['_trackEvent', 'SiteSearch', 'AutoComplete', request.term]);
					response(msg);
				}
			});
		},
        minLength: 3,
        select: function (event, ui) {
        	if (ui.item.url.indexOf("?") != -1) {
        		window.location.href = ui.item.url + "&term=" + $(this).val();
        	} else {
        		window.location.href = ui.item.url + "?term=" + $(this).val();
        	}
        }
    });
		
	/* calls there */
	
	$("ul#premium-list a").live('click',function() {
		$("div.premium-slide").hide();
		$("div#" + $(this).attr("href")).fadeIn();
		$("ul#premium-list a").removeClass("active");
		$(this).addClass("active");
		return false;
	});
	
	$("a.premium-faq").live('click',function() {
		$("a.premium-faq").removeClass("strong");
		$("div.faq-answer").hide();
		$("div#" + $(this).attr("href")).fadeIn();
		$(this).addClass("strong");
		return false;
	});
	
	$("a.account-slide").live('click',function() {
		$("div.pref-slide").each(function(index) {
			$(this).hide();
		});
		$("ul.pref-list li").each(function(index) {
			$(this).removeClass("active");
		});
		$(this).parent().addClass("active");
		$("div#" + $(this).attr("id")).show();
		return false;
	});
	
	if (typeof NF_DATA.is_logged_in != 'undefined' && NF_DATA.is_logged_in == true) {
		$.cookie("user-last-seen", Math.round((new Date()).getTime() / 1000), { expires: 7 });
	}
	
	
	$("a.premium-ss").colorbox({rel:'nofollow'});
	$("a.inline").colorbox({inline:true, width:"1010px"});
	$("a.inline-small").colorbox({inline:true, width:"500px"});
	$(".alpha-tab").simpletabs();
    
    $('a.follow-category').live('click',function() {
    	var the_link = this;
	    $.ajax({
	        type: "POST",
	        url: "/newsfeed/subscribe",
	        data: 'category_id='+$(the_link).attr('rel'),
	        dataType : 'json',
			error : function() {},
	        success: function(msg) {
		        if (msg.error == 0) {
		        	$(the_link).removeClass('follow-category').addClass('unfollow-category').addClass('cta-blue').html('Unfollow');
		        	$('#follower-count-'+$(the_link).attr('rel')).html(msg.category.follower_count);
		        } else if (typeof msg.redirect_url != 'undefined') {
		        	window.location.href = msg.redirect_url;
		        }
	        }
	     });
	    return false;
    });
    
    
    $('a.unfollow-category').live('click',function() {
    	var the_link = this;
	    $.ajax({
	        type: "POST",
	        url: "/newsfeed/unsubscribe",
	        data: 'category_id='+$(the_link).attr('rel'),
	        dataType : 'json',
			error : function() {},
	        success: function(msg) {
		        if (msg.error == 0) {
		        	$(the_link).removeClass('unfollow-category').addClass('follow-category').removeClass('cta-blue').html('Follow');
		        	$('#follower-count-'+$(the_link).attr('rel')).html(msg.category.follower_count);
		        } else if (typeof msg.redirect_url != 'undefined') {
		        	window.location.href = msg.redirect_url;
		        }
	        }
	     });
	    return false;
    });
    
    $('a#show-draft-help').live('click', function() {
    	$('div#draftkit-explanation').toggle();
    });
    
    $('a#show-team-rankings-help').live('click', function() {
    	$('div#teamrankings-explanation').toggle();
    });
    
    
    $('input.settings').live('click', function() {
    	if($(this).val() == "custom-settings") {
	    	$("div#custom-settings").show();
    	} else {
	    	$("div#custom-settings").hide();
    	}
    });
    
    $('select#league-select').live('change', function() {
    	if($(this).val() == "Yahoo!") {
	    	$("div#yahoo-update").show();
    	} else {
	    	$("div#yahoo-update").hide();
    	}
    });
    
    
    if (typeof KMQ_PUSH != 'undefined') {
    	$.each(KMQ_PUSH, function(index, value) {
    		pushKMQ(value);
    	});
    }
    
    if (typeof GAQ_PUSH != 'undefined') {
    	$.each(GAQ_PUSH, function(index, value) {
    		pushGAQ(value);
    	});
    }
    
    $("form#who-do-i-draft-form input:text").autocomplete({
        source: "/search/nfl-draft-projection-player",
        minLength: 3,
        select: function (event, ui) {
    		$('#' + $(this).attr('id') + '-slug').val(ui.item.slug);
        }
    });
    
    $('a#compare-players').on('click', function() {
    	var player_a = $('#player-a-slug').val();
    	var player_b = $('#player-b-slug').val();
    	if (player_a == '' || player_b == '') {
    		alert('Need to select two players to compare.');
    		return false;
    	}
    	
    	if (player_a < player_b) {
    		window.location.href = '/nfl/fantasy/who-do-i-draft/' + player_a + '-or-' + player_b;
    	} else {
    		window.location.href = '/nfl/fantasy/who-do-i-draft/' + player_b + '-or-' + player_a;
    	}
    	
    	return false;
    });
    
   $("form#who-do-i-start-form input:text").autocomplete({
        source: "/search/nfl-draft-projection-player",
        minLength: 3,
        select: function (event, ui) {
    		$('#' + $(this).attr('id') + '-slug').val(ui.item.slug);
        }
    });
    
    $('a#compare-start-players').on('click', function() {
    	var player_a = $('#player-a-slug').val();
    	var player_b = $('#player-b-slug').val();
    	if (player_a == '' || player_b == '') {
    		alert('Need to select two players to compare.');
    		return false;
    	}
    	
    	if (player_a < player_b) {
    		window.location.href = '/nfl/fantasy/who-do-i-start/' + player_a + '-or-' + player_b;
    	} else {
    		window.location.href = '/nfl/fantasy/who-do-i-start/' + player_b + '-or-' + player_a;
    	}
    	
    	return false;
    });
    
    $("form#nba-who-do-i-start-form input:text").autocomplete({
    	source: "/search/nba-who-do-i-start",
    	minLength: 3,
    	select: function (event, ui) {
    		$('#' + $(this).attr('id') + '-slug').val(ui.item.slug);
    	}
    });
    
    $('a#nba-compare-start-players').on('click', function() {
    	var player_a = $('#player-a-slug').val();
    	var player_b = $('#player-b-slug').val();
    	if (player_a == '' || player_b == '') {
    		alert('Need to select two players to compare.');
    		return false;
    	}
    	
    	if (player_a < player_b) {
    		window.location.href = '/nba/fantasy/who-do-i-start/' + player_a + '-or-' + player_b;
    	} else {
    		window.location.href = '/nba/fantasy/who-do-i-start/' + player_b + '-or-' + player_a;
    	}
    	
    	return false;
    });
    
    $("form#mlb-who-do-i-start-form input:text").autocomplete({
    	source: "/search/mlb-who-do-i-start",
    	minLength: 3,
    	select: function (event, ui) {
    		$('#' + $(this).attr('id') + '-slug').val(ui.item.slug);
    	}
    });
    
    $('a#mlb-compare-start-players').on('click', function() {
    	var player_a = $('#player-a-slug').val();
    	var player_b = $('#player-b-slug').val();
    	if (player_a == '' || player_b == '') {
    		alert('Need to select two players to compare.');
    		return false;
    	}
    	
    	if (player_a < player_b) {
    		window.location.href = '/mlb/fantasy/who-do-i-start/' + player_a + '-or-' + player_b;
    	} else {
    		window.location.href = '/mlb/fantasy/who-do-i-start/' + player_b + '-or-' + player_a;
    	}
    	
    	return false;
    });
    
    
    //login modal for visitors via hasoffers
	if($("#membersonly-modal").length && $.cookie("user-last-seen") == null) {
		setTimeout(function() {
			$.colorbox({inline:true, width:"900px", href:"#membersonly-modal", overlayClose:false, escKey:false, onLoad: function() {
				$('#cboxClose').remove();
			}});
		}, 5000);
	}
	
    $("abbr.timeago").livequery(function () {
        $(this).timeago();
    });
    
	var monthNames = [ "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" ];
	$('.relative-datetime').each(function(){
		var d = new Date(parseInt($(this).attr('rel')) * 1000);
		$(this).html(monthNames[d.getMonth()] + ' ' + d.getDate() + ', ' + d.getFullYear() + ', ' + hourlyFormat(d));
	});
	
	function hourlyFormat(date) {
		var h = date.getHours();
		var ampm = (h < 12) ? 'am' : 'pm';
		h %= 12;
		h = (h == 0) ? 12 : h;
		var m = date.getMinutes();
		m = (m > 9) ? m : '0' + m;
		return h + ':' + m + ' ' + ampm;
	}
	
	$('.facebook-share').click(function(){
		elem = $(this);
		postToFeed(elem.data('title'), elem.data('desc'), elem.prop('href'), elem.data('image'));
		return false;
	});
	
	$('.twitter-share').click(function(event) {
	    var width  = 500,
	    	height = 250,
    		left   = ($(window).width()  - width)  / 2,
	        top    = ($(window).height() - height) / 2,
	        url    = this.href,
	        opts   = 'status=1' +
	                 ',width='  + width  +
	                 ',height=' + height +
	                 ',top='    + top    +
	                 ',left='   + left;
	    
	    window.open(url, 'twitter', opts);
	    return false;
	});
    
	$('img.qna-answer-option').on('error', function(){
        $(this).attr('src', '/web/20150905131515/https://d1tjohjvimcqgl.cloudfront.net/category/no-image.png');
	});
	
	$('img.qna-answer-option').each(function(){
	     $(this).attr('src', $(this).attr('src'));
	});
	
	$('img.category-image').on('error', function(){
		$(this).attr('src', '/web/20150905131515/https://d1tjohjvimcqgl.cloudfront.net/category/no-image.png');
	});
	
	$('img.category-image').each(function(){ 
		if (typeof this.naturalWidth != "undefined" && this.naturalWidth == 0) {
			$(this).attr('src', '/web/20150905131515/https://d1tjohjvimcqgl.cloudfront.net/category/no-image.png'); 
		}   
	});
	
});