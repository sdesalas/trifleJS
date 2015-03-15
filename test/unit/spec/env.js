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
		
	});
	
	// --------------------------------------------
	assert.section('Console object', function() {
	
		assert.checkMembers(console, 'console', {
			'log': 'function',
			'warn': 'function',
			'error': 'function',
			'debug': 'function'
		});
	
	});

	
	// --------------------------------------------
	assert.section('Window object', function() {
	
		assert.checkMembers(window, 'window', {
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
		assert(difference > 198 && difference < 300, 'window.setTimeout(ms) honors the waiting period');
		
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
	
		assert.checkMembers(navigator, 'navigator', {
			appCodeName: 'string',
			appName: 'string',
			appVersion: 'string',
			cookieEnabled: 'boolean',
			platform: 'string',
			userAgent: 'string'
		});
		
	});
	

	// --------------------------------------------
	assert.section('Location object', function() {
	
		assert.checkMembers(location, 'location', {
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
	assert.section('trifle object', function() {
	
		if (!trifle || !trifle.version) {
			console.warn('No trifle object, skipping tests!');
			return;
		}
	
		assert.checkMembers(trifle, 'trifle', {
			version: 'object',
			emulation: 'string',
			wait: 'function',
			extend: 'function'
		});
		
	});
	
	
	// --------------------------------------------
	assert.section('trifle.Callback class', function() {
	
		if (!trifle.Callback) {
			console.warn('No Callback Support, skipping tests!');
			return;
		}
		
		assert(typeof trifle.Callback === 'function', 'trifle.Callback() constructor is available');

		assert.checkMembers(trifle.Callback, 'trifle.Callback', {
			newUid: 'function',
			execute: 'function',
			executeOnce: 'function',
			id: 'function'
		});
		
		var scope =  {};
		var args = [];
		var callback = new trifle.Callback(function() {}, scope, args);
		
		assert.checkMembers(callback, 'callback', {
			id: 'string',
			func: 'function',
			scope: 'object',
			defaultArgs: 'object'
		});
		
	});
	
	
	// --------------------------------------------
	assert.section('Event Handling', function() {
	
		if (!Object.addEvent) {
			console.warn('No Event Handling Support, skipping tests!');
			return;
		}
	
		var observable = new Object();
		var loading1, loading2, loading1_1, loading2_1, loading_scope1, loading_scope2;
		var ready1, ready2, ready1_1, ready2_1, ready_scope1, ready_scope2, ready_scope2_2;
		var loadingCount = 0, readyCount = 0;
		var callbacks = {
			loading1: function(value) { loading1 = value + loadingCount; loading_scope1 = this; loadingCount++; },
			loading2: function(value) { loading2 = value + loadingCount; loading_scope2 = this; loadingCount++; },
			loading3: function(value) { loading1_1 = value + loadingCount; loadingCount++; },
			loading4: function(value) { loading2_2 = value + loadingCount; loadingCount++; },
			ready1: function(value) { ready1 = value + readyCount; ready_scope1 = this; readyCount++; },
			ready2: function(value) { ready2 = value + readyCount; ready_scope2 = this; readyCount++; },
			ready3: function(value) { ready1_1 = value + readyCount; readyCount++; },
			ready4: function(value) { ready2_2 = value + readyCount;  ready_scope2_2 = this; readyCount++; }
		};
		
		Object.addEvent(observable, "onLoading");
		Object.addEvent(observable, "onReady", true);
		
		observable.on("loading", callbacks.loading1);	
		observable.onLoading = callbacks.loading2;
		observable.on("loading", callbacks.loading3);
		observable.onLoading = callbacks.loading4;
		
		observable.on("ready", callbacks.ready1);	
		observable.onReady = callbacks.ready2;
		observable.on("ready", callbacks.ready3);
		observable.onReady = callbacks.ready4;

		assert.checkMembers(observable, 'observable', {
			listeners: 'object',
			fireEvent: 'function',
			on: 'function',
			onLoading: 'function',
			onReady: 'function'
		});

		assert(loading1 === undefined, 'Variables are undefined before firing event');
		assert(loading2 === undefined, 'Variables are undefined before firing event');
		assert(loadingCount === 0, 'Count has not increased before firing event');
		assert(ready1 === undefined, 'Variables are undefined before firing event');
		assert(ready2 === undefined, 'Variables are undefined before firing event');
		assert(readyCount === 0, 'Count has not increased before firing event');
	
		assert(typeof observable.listeners === 'object', 'obj.listeners contains the object listeners');
		assert(typeof observable.listeners.loading === 'object', 'object.listeners[event] contains the event information');
		assert(observable.listeners.loading.callbacks instanceof Array, 'object.listeners[event].callbacks contains the current callbacks');
		assert(observable.listeners.loading.callbacks.length === 4, 'object.listeners[event].callbacks contains 4 callbacks');
		assert(observable.listeners.loading.callbacks[3] === observable.onLoading, 'observable.onEvent contains the latest callback');
		assert(observable.onLoading === callbacks.loading4, 'observable.onEvent contains the latest callback');
		assert(typeof observable.listeners.ready === 'object', 'object.listeners[event] contains the event information');
		assert(observable.listeners.ready.callbacks instanceof Array, 'object.listeners[event].callbacks contains the current callbacks');
		assert(observable.listeners.ready.callbacks.length === 1, 'object.listeners[event].callbacks contains 1 callback (unique event)');
		assert(observable.listeners.ready.callbacks[0] === observable.onReady, 'observable.onEvent contains the latest callback');
		assert(observable.onReady === callbacks.ready4, 'observable.onEvent contains the latest callback');
		
		observable.fireEvent("loading", ["loading argument"]);
		
		assert(loading1 === "loading argument0", 'The 1st #loading event fired succesfully');
		assert(loading2 === "loading argument1", 'The 1st #onLoading event fired successfully');
		assert(loading1_1 === "loading argument2", 'The 2nd #loading event fired succesfully');
		assert(loading2_2 === "loading argument3", 'The 2nd #onLoading event fired successfully');
		assert(loading_scope1 === observable, 'The scope during #loading event is the actual object');
		assert(loading_scope2 === observable, 'The scope during #onLoading event is the actual object');
		assert(loadingCount === 4, 'The #loading event fired once per listener');

		assert(ready1 === undefined, '1st #ready event variable undefined before firing');
		assert(ready2 === undefined, '1st #onReady event variable undefined before firing');
		assert(ready_scope1 === undefined, '2nd #ready event variable undefined before firing');
		assert(ready_scope2 === undefined, '2nd #onReady event variable undefined before firing');
		assert(readyCount === 0, '#ready event did not trigger when loading was fired');

		observable.fireEvent("ready", ["ready argument"]);

		assert(loading1 === "loading argument0", 'The 1st #loading event data did not fire or change');
		assert(loading2 === "loading argument1", 'The 1st #onLoading event did not fire or change');
		assert(loading1_1 === "loading argument2", 'The 2nd #loading event did not fire or change');
		assert(loading2_2 === "loading argument3", 'The 2nd #onLoading event did not fire or change');
		assert(loadingCount === 4, 'The #loading event did not fire a second time');

		assert(ready1 === undefined, 'The 1st #ready event did not fire (unique event)');
		assert(ready2 === undefined, 'The 1st #onReady event defined did not fire (unique event)');
		assert(ready1_1 === undefined, 'The 2nd #ready event did not fire (unique event).');
		assert(ready2_2 === "ready argument0", 'Only the last #onReady event handler fired (unique event)');
		assert(ready_scope1 === undefined, 'The scope during 1st #ready event not defined (unique event)');
		assert(ready_scope2 === undefined, 'The scope during 1st #onReady event not defined (unique event)');
		assert(ready_scope2_2 === observable, 'The scope during last #onReady event is the actual object');
		assert(readyCount === 1, 'The #ready event fired once as it is unique.');


	});



});
