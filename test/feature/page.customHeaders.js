

var page = require("webpage").create();

page.customHeaders = {
	'X-Test': 'foo',
	'DNT': 1,
	'scooby': 'doo'
}

page.open("http://www.xhaus.com/headers", function(status) {
    if (status === 'success') {
        page.render("headers.png");
        console.log('Page rendered');
    } else {
        console.error('Cannot load url.');
    }
    phantom.exit();
});