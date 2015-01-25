var page = require('webpage').create();
var url = 'http://wikipedia.org';
var width = 1366;
var height = 1768;
var file = 'test.noscrollbar.png';

page.viewportSize = { width: width, height: height };

page.open(url, function() {
  page.render(file);
  phantom.exit();
});

