var page = require("webpage").create();

// GET
page.open("http://www.xhaus.com/headers", function(status) {
    if (status === 'success') {
        page.render("get.png");
        console.log('Page rendered');
    } else {
        console.error('Cannot load url.');
    }
});


page.customHeaders = {
	'X-Test': 'foo',
	'DNT': 1,
	'scooby': 'doo'
}

//POST
page.open("http://www.xhaus.com/headers", "POST", "blah", function(status) {
    if (status === 'success') {
        page.render("post.png");
        console.log('Page rendered');
    } else {
        console.error('Cannot load url.');
    }
    phantom.exit();
});
