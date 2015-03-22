var page = require('webpage').create();

page.zoomFactor=1;

page.onLoadFinished = function(e) {
     setTimeout(function(){
      page.render('msn.jpg',{quality:50});
      phantom.exit();},30000);
 };
page.viewportSize={width:1420,height:800};
console.log(page.viewportSize);
page.localT1oRemoteUrlAccessEnabled =true;

setTimeout(page.open('http://www.msn.com', function() {}),30000);