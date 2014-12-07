var server = require('webserver').create();
console.log('Starting webserver on port 8080');

var service = server.listen(8080, function(request, response) {
	console.log('starting connection, printing request info');
	console.log({
		method: request.method,
		url: request.url,
		httpVersion: request.httpVersion,
		headers: request.headers,
		post: request.post,
		rawPost: request.rawPost
	});
	
    response.statusCode = 200;
    console.log('adding response.headers', response.headers);
    response.headers = {
		"header1": "header1",
		"header2": "header2"
    };
    console.log('headers added');
    response.setHeader('header3', 'header3');
    response.write('<html><body><p>Hello there!</p>');
    response.write('<p>From port:' + server.port + '</p>');
    response.write('<p><form action="/" method="post">Name:<input type="text" name="name" value=""/><br/><input type="file" name="theFile"/><input type="submit"></form></p></body></html>');
    response.close();
});
console.log('Ending');



/*
var page = require('webpage').create();
var service = server.listen(8080, function(request, response) {
	console.log('server started, opening page');
	console.log({
		method: request.method,
		url: request.url,
		httpVersion: request.httpVersion,
		headers: request.headers,
		post: request.post,
		rawPost: request.rawPost
	});
    response.write('<html><body><p>Hello there!</p>');
    response.write('<p>From port:' + server.port + '</p>');
    response.write('<p><form action="/" method="post"><input type="text" name="name" value=""/><input type="file" name="theFile"/><input type="submit"></form></p></body></html>');
    response.close();
});

trifle.wait(1000);


page.open('http://localhost:8080', function(status) {
	console.log('page opened, status: ' + status);
	console.log('printing contents:');
	console.log(page.plainText);
	phantom.exit();
});

*/