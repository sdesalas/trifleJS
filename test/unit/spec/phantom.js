

assert.suite('Object: phantom', function() {

	// SETUP
	var fs = require('fs');
	var server = require('webserver').create();
	var page = require('webpage').create();
	
	// --------------------------------------------
	assert.section('Object availability');

	assert(this.hasOwnProperty('phantom'), 'this.phantom exists');
	assert(typeof this.phantom === 'object', 'this.phantom is an object');
	assert(window.hasOwnProperty('phantom'), 'window.phantom exists');
	assert(typeof window.phantom === 'object', 'window.phantom is an object');

	// --------------------------------------------
	assert.section('Properties & methods');

	assert(phantom.args instanceof Array, 'phantom.args is an array');
	assert(phantom.cookies instanceof Array, 'phantom.cookies is an array');
	assert(phantom.cookies.length === 0, 'phantom.cookies has no cookies to begin with');
	assert(typeof phantom.cookiesEnabled === 'boolean', 'phantom.cookiesEnabled is a boolean');
	assert(phantom.cookiesEnabled === true, 'phantom.cookiesEnabled is true');
	assert(typeof phantom.outputEncoding === 'string', 'phantom.outputEncoding is a string');
	assert(phantom.outputEncoding === 'UTF-8', 'phantom.outputEncoding is "UTF-8"');
	assert(typeof phantom.libraryPath === 'string', 'phantom.libraryPath is a string');
	assert(phantom.libraryPath === fs.workingDirectory, 'phantom.libraryPath is equal to fs.workingDirectory');
	assert(typeof phantom.scriptName === 'string', 'phantom.scriptName is a string');
	assert(typeof phantom.version === 'object', 'phantom.version is an object');
	assert(typeof phantom.version.major === 'number', 'phantom.version.major is a number');
	assert(typeof phantom.version.minor === 'number', 'phantom.version.minor is a number');
	assert(typeof phantom.addCookie === 'function', 'phantom.addCookie is a function');
	//assert(typeof phantom.clearCookies === 'function', 'phantom.clearCookies is a function');
	//assert(typeof phantom.deleteCookie === 'function', 'phantom.deleteCookie is a function');
	assert(typeof phantom.exit === 'function', 'phantom.exit is a function');
	assert(typeof phantom.injectJs === 'function', 'phantom.injectJs is a function');

	// --------------------------------------------
	assert.section('Functionality');

	phantom.libraryPath = "..\\..\\test\\unit\\ref\\";

	assert(phantom.scriptName === 'test/unit/spec/phantom.js', 'phantom.scriptName is the currently executing script');
	assert(phantom.libraryPath !== fs.workingDirectory, 'changing phantom.libraryPath does not alter fs.workingDirectory');
	assert(phantom.version.major === 1, 'phantom.version.major is 1');
	
	var injection = phantom.injectJs('script.js');
	
	assert(injection === true, 'phantom.injectJs() returned true when pointing to file');
	assert(___test190234 === true, 'phantom.injectJs() executes a script in current V8 context');
	
	// --------------------------------------------
	assert.section('Cookies');

	var cookieSuccess = phantom.addCookie({
		name: 'PhantomTestCookie',
		value: 'ariya/phantomjs/wiki',
		domain: 'localhost'
	});
	
	server.listen(8086, function(request, response) { 
		response.write(JSON.stringify({
			success: true, 
			url: request.url, 
			headers: request.headers
		})); 
		response.close(); 
	});
	
	assert(cookieSuccess === true, 'phantom.addCookie() returned true when adding a test cookie');
	assert(phantom.cookies.length === 1, 'phantom.cookies has one cookie listed');

	var ready = false;
	var cookies = null;

	page.open('http://localhost:8086', function(status) {
		var response = JSON.parse(page.plainText);
		if (response && response.headers) cookies = response.headers['Cookie'];
		ready = true;
	});
	
	assert.waitFor(ready);
	
	assert(!!cookies && cookies.indexOf('PhantomTestCookie=ariya/phantomjs/wiki') > -1, 'phantom.addCookie() succesfully sends a cookie to the server');
	
	// Tear down
	phantom.libraryPath = fs.workingDirectory;
	server.close();
	page.close();

});