var server = require('webserver').create();
console.log('Starting webserver, port: ' + server.port);
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
    response.write('<p><form action="/" method="post"><input type="text" name="name" value=""/><input type="file" name="theFile"/><input type="submit"></form></p></body></html>');
    response.close();
});
console.log('Ending');
