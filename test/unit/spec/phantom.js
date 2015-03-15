/*
* phantom.js
* 
* Runs a set of unit tests used that check
* the functionality of the global 'phantom' object.
* 
*/


assert.suite('Object: phantom', function() {

	// SETUP
	var fs = require('fs');
	var system = require('system');
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
	assert(typeof phantom.clearCookies === 'function', 'phantom.clearCookies is a function');
	//assert(typeof phantom.deleteCookie === 'function', 'phantom.deleteCookie is a function');
	assert(typeof phantom.exit === 'function', 'phantom.exit is a function');
	assert(typeof phantom.injectJs === 'function', 'phantom.injectJs is a function');
	assert(typeof phantom.onError === 'function', 'phantom.onError is a function');

	// --------------------------------------------
	assert.section('Functionality');

	phantom.libraryPath = "test";
	if (!fs.isDirectory('test')) fs.makeDirectory('test');

	assert(phantom.scriptName === system.args[0], 'phantom.scriptName is the currently executing script');
	assert(phantom.libraryPath !== fs.workingDirectory, 'changing phantom.libraryPath does not alter fs.workingDirectory');
	assert(phantom.version.major === 1, 'phantom.version.major is 1');
	
	var injection = phantom.injectJs('../examples/variable.js');
	
	assert(injection === true, 'phantom.injectJs() returned true when pointing to file');
	assert(___test190234 === true, 'phantom.injectJs() executes a script in current V8 context');
	
	// --------------------------------------------
	assert.section('Global Error Handling', function() {
	
		var lastErr, lastTrace;
		var defaultErrHandler = phantom.onError[0];
		
		phantom.onError = function(msg, trace) {
			lastErr = msg;
			lastTrace = trace;
		}
	
		window.setTimeout(function () {assert.ready = true; unknownVar.method();}, 0);
	
		assert.waitUntilReady();
		
		assert(lastErr.indexOf('unknownVar') > -1, 'phantom.onError can capture javascript errors');
		assert(lastTrace instanceof Array && lastTrace.length > 0, 'phantom.onError contains a javascript trace');
	
		assert.checkMembers(lastTrace[0], 'errorTrace', {
			'file': 'string',
			'line': 'number',
			'function': 'string'
		});
		
		phantom.onError = function(msg, trace) {
			throw "error within the error handler!";
		}
		
		assert.isError(function() {anotherException.isTriggered();}, 'phantom.onError generates errors without recursing on itself');
		
		phantom.onError = defaultErrHandler;
	
	});
	
	// --------------------------------------------
	assert.section('Cookies');
	
	// Start a listener to check headers
	server.listen(8086, function(request, response) { 
		response.write(JSON.stringify({
			success: true, 
			url: request.url, 
			headers: request.headers
		})); 
		//console.log(request);
		response.close(); 
	});
	
	var cookieSuccess = phantom.addCookie({
		name: 'PhantomTestCookie',
		value: 'ariya/phantomjs/wiki',
		domain: 'localhost'
	});
	
	assert(cookieSuccess === true, 'phantom.addCookie() returned true when adding a test cookie');
	assert(phantom.cookies.length === 1, 'phantom.cookies has one cookie listed');

	phantom.cookiesEnabled = false;
	
	var cookieSuccess = phantom.addCookie({
		name: 'PhantomTestCookie2',
		value: 'ariya/phantomjs/wiki2',
		domain: 'localhost'
	});

	assert(cookieSuccess === false, 'phantom.cookiesEnabled can be used to disable cookie usage');

	phantom.cookiesEnabled = true;

	var cookies = null;
	var checkCookies = function(status) {
		try {
			var response = JSON.parse(page.plainText);
			if (response && response.headers) cookies = response.headers['Cookie'];
		} catch (e) {}
		assert.ready = true;
	};

	page.open('http://localhost:8086', checkCookies);
	
	assert.waitUntilReady();
	
	assert(!!cookies && cookies.indexOf('PhantomTestCookie=ariya/phantomjs/wiki') > -1, 'phantom.addCookie() succesfully sends a cookie to the server');
	
	// Check HTTPOnly cookies include "HttpOnly"

	// Check Secure cookies include "secure"

	// Check Path (only send cookies if in same path)
	
	// Check Expires (should remain even after deletion - or ignore/ throw an error)
	
	
	// NOTE: phantom.cookies setter should remove 
	// previous cookies and replace them with new ones.
	
	phantom.cookies = [{
		name: 'PhantomTestCookie2',
		value: 'ariya/phantomjs/wiki2',
		domain: 'localhost'
	}, {
		name: 'PhantomTestCookie3',
		value: 'ariya/phantomjs/wiki3',
		domain: 'localhost'
	}];
	
	assert(phantom.cookies.length === 2, 'phantom.cookies has 2x cookies after using setter');

	cookies = null;

	page.open('http://localhost:8086', function(status) {
		var response = JSON.parse(page.plainText);
		if (response && response.headers) cookies = response.headers['Cookie'];
		assert.ready = true;
	});
	
	assert.waitUntilReady();
		
	assert(!!cookies && cookies.indexOf('PhantomTestCookie=ariya/phantomjs/wiki') === -1, 'phantom.cookies removes previous cookies from request');
	assert(!!cookies && cookies.indexOf('PhantomTestCookie2=ariya/phantomjs/wiki2') > -1, 'phantom.cookies succesfully adds a cookie and sends to the server');
	assert(!!cookies && cookies.indexOf('PhantomTestCookie3=ariya/phantomjs/wiki3') > -1, 'phantom.cookies succesfully adds a cookie and sends to the server');

	phantom.clearCookies();
	
	assert(phantom.cookies.length === 0, 'phantom.cookies has 0 cookies after using phantom.clearCookies()');

	// Tear down
	phantom.libraryPath = fs.workingDirectory;
	server.close();
	page.close();

});