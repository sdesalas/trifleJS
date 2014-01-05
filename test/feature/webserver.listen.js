var server = require('webserver').create();
console.log('Starting webserver, port: ' + server.port);
var service = server.listen(8080, function(request, response) {
    response.statusCode = 200;
    console.log('response.headers', response.headers);
    response.headers = {
		header1: 'header1',
		header2: 'header2'
    };
    response.setHeader('header3', 'header3');
    response.write('<html><body><p>Hello there!</p>');
    response.write('<p>From port:' + server.port + '</p>');
    response.write('<p>YEA2</p></body></html>');
    response.close();
});
console.log('Ending');
