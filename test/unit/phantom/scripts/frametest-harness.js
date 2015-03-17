
var page = require('webpage').create();
var server = require('webserver').create();
var fs = require('fs');

fs.changeWorkingDirectory('E:\\PROJECTS\\GITHUB\\trifleJS\\bin\\Debug\\');

server.listen(8897, function(request, response) {
	console.log('requested url--> ' + request.url);
	var path = request.url;
	if (!path || path === '/') path = '/index.html';
	response.write(fs.read('test/frames' + path));
	response.close();
});

