




/*
     FILE ARCHIVED ON 7:07:04 Sep 6, 2015 AND RETRIEVED FROM THE
     INTERNET ARCHIVE ON 18:41:31 Apr 13, 2016.
     JAVASCRIPT APPENDED BY WAYBACK MACHINE, COPYRIGHT INTERNET ARCHIVE.

     ALL OTHER CONTENT MAY ALSO BE PROTECTED BY COPYRIGHT (17 U.S.C.
     SECTION 108(a)(3)).
*/
$(document).ready(function() {
	
	function roundNumber(num, dec) {
		var result = Math.round(num*Math.pow(10,dec))/Math.pow(10,dec);
		return result;
	}
	
	if (typeof NF_DATA.projections != 'undefined' && typeof NF_DATA.projections.projections != 'undefined') {
		$.each(NF_DATA.projections.projections, function(index, projection) {
			projection['player'] = NF_DATA.projections.players[projection.player_id];
			projection['team'] = NF_DATA.teams[projection.team_id];
			
			projection['ci_lower'] = parseFloat(projection['fp']) - parseFloat(projection['sd_fp']);
			projection['ci_upper'] = parseFloat(projection['fp']) + parseFloat(projection['sd_fp']);
			if(projection['ci_lower'] < 0) { projection['ci_lower'] = 0; }
			projection['ci'] = roundNumber(projection['ci_lower'], 2) + "-" + roundNumber(projection['ci_upper'], 2);
			
			$('#projection-data').append(TrimPath.processDOMTemplate('projection-row-template', projection));
		});
	}
	$("tbody[id=projection-data]>tr:odd").addClass('odd');
	
	if ($.cookie("user-last-seen") == null) {
		setTimeout(function() {
			$.colorbox({inline:true, width:"900px", href:"#players-projections-modal", overlayClose:false, escKey:false, onLoad: function() {
				$('#cboxClose').remove();
			}});
		}, 2000);
	}
	
	
});