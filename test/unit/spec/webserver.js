
// Setup variables

var fs = require("fs");
var server = require('webserver').create();
var refdir = "../../test/unit/ref/";
var textfile = refdir + "fs.txt";
var workingDirectory = fs.workingDirectory;
var helloWorldListener = function(request, response) { response.write("Hello World"); response.close(); }


assert.suite('WEBSERVER MODULE', function() {

	// --------------------------------------------
	assert.section('Instantiation');
	// --------------------------------------------

	assert(!!server, 'server can be instantiated using require()');
	assert(typeof server === 'object', 'server is an object');

	// --------------------------------------------
	assert.section('Properties & methods');
	// --------------------------------------------

	assert(typeof server.listen === 'function', 'server.listen() is a function');
	assert(typeof server.close === 'function', 'server.close() is a function');
	assert(typeof server.port === 'string', 'server.port is a string');
	assert(server.port === '', 'server.port is an empty string to beging with')

	// --------------------------------------------
	assert.section('Listening for connections');
	// --------------------------------------------

	var isListening = server.listen(8080, helloWorldListener);

	assert(isListening === true, 'server.listen() returns true when listening');
	assert(server.port === '8080', 'server.port returns the correct port')

	isListening = server.listen(8080, helloWorldListener);

	assert(isListening === true, 'server.listen() return true when listening on same port');
	assert(server.port === '8080', 'server.port returns the correct port')

	isListening = server.listen(8081, helloWorldListener);

	assert(isListening === true, 'server.listen() return true when listening on same port');
	assert(server.port === '8081', 'server.port returns the correct port')


	// var service = server.listen(8080, function(request, response) {
	// 	console.log('starting connection, printing request info');
	// 	console.log({
	// 		method: request.method,
	// 		url: request.url,
	// 		httpVersion: request.httpVersion,
	// 		headers: request.headers,
	// 		post: request.post,
	// 		rawPost: request.rawPost
	// 	});
		
	//     response.statusCode = 200;
	//     console.log('adding response.headers', response.headers);
	//     response.headers = {
	// 		"header1": "header1",
	// 		"header2": "header2"
	//     };
	//     console.log('headers added');
	//     response.setHeader('header3', 'header3');
	//     response.write('<html><body><p>Hello there!</p>');
	//     response.write('<p>From port:' + server.port + '</p>');
	//     response.write('<p><form action="/" method="post"><input type="text" name="name" value=""/><input type="file" name="theFile"/><input type="submit"></form></p></body></html>');
	//     response.close();
	// });


});

// TEARDOWN