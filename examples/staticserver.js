

var server = require('webserver').create();
var fs = require('fs');

server.listen(8897, function(request, response) {
	console.log('requested url--> ' + request.url);
	var path = request.url;
	if (!path || path === '/') path = '/index.html';
	response.write(fs.read('www' + path));
	response.close();
});

