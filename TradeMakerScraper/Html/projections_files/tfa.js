




/*
     FILE ARCHIVED ON 22:23:54 Aug 24, 2015 AND RETRIEVED FROM THE
     INTERNET ARCHIVE ON 18:41:30 Apr 13, 2016.
     JAVASCRIPT APPENDED BY WAYBACK MACHINE, COPYRIGHT INTERNET ARCHIVE.

     ALL OTHER CONTENT MAY ALSO BE PROTECTED BY COPYRIGHT (17 U.S.C.
     SECTION 108(a)(3)).
*/
/*! 5-132-130857-5-Release 2014-10-30 */
!function(a,b){function c(a){return"action"===a.notify?(w.push(a),void 0):("mark"===a.notify&&x.push(a),void 0)}function d(){var a,b,c=l();if(c)for(a=0,b=w.length;b>a;a++)g(f(u,{$publishreId:c?c+"/":"",$logType:"action"})+"tim="+escape(i())+"&item-url="+escape(h())+m(s,w.shift())+j())}function e(){var a,b,c=l();if(c)for(a=0,b=x.length;b>a;a++)g(f(u,{$publishreId:c?c+"/":"",$logType:"mark"})+"tim="+escape(i())+"&item-url="+escape(h())+m(t,x.shift())+j())}function f(a,b){return a.replace(/\{([^{}]*)\}/g,function(a,c){var d=b[c];return"string"==typeof d||"number"==typeof d?d:a})}function g(a){var b=new Image;b.src=a}function h(){return a.location.href}function i(){var a=new Date,b=a.getHours(),c=a.getMinutes(),d=a.getSeconds()+a.getMilliseconds()/1e3;return(10>b?"0":"")+b+":"+(10>c?"0":"")+c+":"+(10>d?"0":"")+d.toFixed(3)}function j(){var c=a.location.search,d=b.referrer.match(/(\?\S+)$/g),e="";return e=k(c.replace(/^\?/,"").split(/&/))+(d?k(d[0].replace(/^\?/,"").split(/&/)):"")}function k(a){var b,c,d="",e="trc_";for(b=0,c=a.length;c>b;b++)0==a[b].indexOf(e)&&(d=d+"&"+a[b]);return d}function l(){var a,b,c,d=document.getElementsByTagName("script"),e="";for(a=0,b=d.length;b>a;a++)if(c=d[a].src,e=c.replace(v,"$3"),d[a].src&&e!==d[a].src)return e;return e}function m(a,b){var c,d="";for(c in a)void 0!==b[c]&&(d+="&"+a[c]+"="+b[c]);return d}function n(a){for(var b=0;b<arguments.length;b++)a=arguments[b],a instanceof Object&&c(a);return o(),arguments.length}function o(){d(),e()}function p(){for(;queue.length;)n(queue.shift())}function q(){queue=a[r]=a[r]||[],queue.registered||(queue.push=n,queue.registered=!0,p())}var r="_tfa",s={orderid:"orderid",currency:"currency",revenue:"revenue",quantity:"quantity",name:"name"},t={type:"marking-type"},u=(a.location.protocol.match(/http/)?a.location.protocol:"http:")+"//trc.taboola.com/{$publishreId}log/3/{$logType}?",v=/(\S+)taboola(\S+|)\.com\/libtrc\/(\S+)\/tfa\.js(\S+|)/,w=[],x=[];a._trcIsUTactive?(a.tfaObject={},tfaObject.dispatchMessage=c,tfaObject.pushMessage=n,tfaObject.getMapQueryString=m,tfaObject.getPublisherId=l,tfaObject.getClientTimestamp=i,tfaObject.doActions=d,tfaObject.dispatchMessage=c):q()}(window,document);