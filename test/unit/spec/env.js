/*
* env.js
* 
* Runs a set of unit tests used that check
* global objects and special functions used
* in the V8 execution context.
*
* Most of the functionality here is defined
* in bootstrap.js
* 
*/

assert.suite('Global Environment', function() {

	// --------------------------------------------
	assert.section('Global objects', function() {
	
		this['GLOBAL'] = this;
	
		assert.checkMembers(this, 'GLOBAL', {
			'window': 'object',
			'navigator': 'object',
			'location': 'object',
			'setTimeout': 'function',
			'setInterval': 'function',
			'clearTimeout': 'function',
			'clearInterval': 'function',
			'addEventListener': 'function',
			'console': 'object',
			'trifle': 'object',
			'phantom': 'object'
		});
		
		delete this['GLOBAL'];
		
	});
	
	// --------------------------------------------
	assert.section('Console object', function() {
	
		assert.checkMembers(this, 'console', {
			'clear': 'function',
			'log': 'function',
			'warn': 'function',
			'error': 'function',
			'debug': 'function'
		});
	
	});

	
	// --------------------------------------------
	assert.section('Window object', function() {
	
		assert.checkMembers(this, 'window', {
			setTimeout: 'function',
			setInterval: 'function',
			clearTimeout: 'function',
			clearInterval: 'function',
			addEventListener: 'function',
			navigator: 'object'
		});
		
		// setTimeout() tests
		
		var value = 0;
		var timestamp = (new Date()).getTime();
		
		window.setTimeout(function() {
			assert.ready = true;
			value = 1;
		}, 200);
		
		assert(value === 0, 'window.setTimeout() executes asynchronously');
		
		trifle.wait(10);
		
		assert(value === 0, 'window.setTimeout() does not execute before time');
		
		assert.waitUntilReady();
		
		var difference = (new Date()).getTime() - timestamp;
				
		assert(value === 1, 'window.setTimeout() executes in the background');
		assert(difference > 198 && difference < 300, 'window.setTimeout(ms) honors the waiting perioda');
		
		var timeoutId = window.setTimeout(function() {
			value = 2;
		}, 100);
		
		assert(typeof timeoutId === 'number', 'window.setTimeout() returns a number');

		window.clearTimeout(timeoutId);
		
		trifle.wait(200);
		
		assert(value === 1, 'window.clearTimeout(id) cancels the timeout');

		// setInterval() tests

		value = 0;
		timestamp = (new Date()).getTime();
		
		var intervalId = window.setInterval(function() {
			value++;
		}, 20);
		
		assert(value === 0, 'window.setInterval() executes asynchronously');
		assert(typeof intervalId === 'number', 'window.setInterval() returns a number');
		
		while(value < 10) {
			trifle.wait(1);
		}
		
		difference = (new Date()).getTime() - timestamp;

		assert(value === 10, 'window.setInterval(ms) fires consecutively');
		assert(difference > 198 && difference < 350, 'window.setInterval(ms) honors the waiting period');

		window.clearInterval(intervalId);

		trifle.wait(100);
		
		assert(value === 10, 'window.clearInterval(id) cancels the interval callbacks');
		
	});
	

	// --------------------------------------------
	assert.section('Navigator object', function() {
	
		assert.checkMembers(window, 'navigator', {
			appCodeName: 'string',
			appName: 'string',
			appVersion: 'string',
			browserLanguage: 'string',
			cookieEnabled: 'boolean',
			language: 'string',
			platform: 'string',
			product: 'string',
			systemLanguage: 'string',
			userAgent: 'string',
			userLanguage: 'string'
		});
		
	});
	

	// --------------------------------------------
	assert.section('Location object', function() {
	
		assert.checkMembers(window, 'location', {
			hash: 'string',
			host: 'string',
			hostname: 'string',
			href: 'string',
			origin: 'string',
			pathname: 'string',
			port: 'string',
			protocol: 'string',
			search: 'string'
		});
		
	});
	
	
	// --------------------------------------------
	assert.section('Event Handling');



});
