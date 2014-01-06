var page = require("webpage").create();
var server = require("webserver").create();

server.listen(8080, function(request, response) {
	console.log({
		method: request.method,
		url: request.url,
		httpVersion: request.httpVersion,
		headers: request.headers,
		post: request.post,
		rawPost: request.rawPost
	});
	response.close();
});

// GET
page.open("http://localhost:8080/get", function(status) {
    if (status !== 'success') {
        console.error('Cannot load url.');
    }
});


page.customHeaders = {
	'X-Test': 'foo',
	'DNT': 1,
	'scooby': 'doo'
}

//POST
page.open("http://localhost:8080/post?custom-headers", "POST", "blah", function(status) {
    if (status !== 'success') {
        console.error('Cannot load url.');
    }
});


// FORM POST
page.open("http://localhost:8080/post?form-data", 'post', "universe=expanding&answer=42", { "Content-Type" : "application/x-www-form-urlencoded" }, function (status) {
    if (status !== 'success') {
        console.error('Cannot load url.');
    }
    phantom.exit();
});
