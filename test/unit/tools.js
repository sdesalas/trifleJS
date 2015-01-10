
var assert = function(condition, message) {
	assert.n++;
	assert.count++;
    if (condition !== true) {
       assert.fail(message);
    } else {
       assert.pass(message);
    }
}

assert.count = 0;

assert.pass = function(message) {
	console.API.color('green', assert.n + '. PASS: ' + (message || '(no message)') + '.');
    assert.pass.count++;
}

assert.pass.count = 0;

assert.fail = function(message) {
	console.API.color('red', assert.n + '. FAIL: ' + (message || '(no message)') + '.');
    assert.fail.count++;
}

assert.fail.count = 0;


assert.isError = function(callback, message) {
	assert.n++;
	assert.count++;
	try {
		callback.call(this);
	} catch(e) {
		assert.pass(message);
		return;
	}
	assert.fail(message);
}

assert.checkMembers = function(scope, objName, config) {
	if (scope && objName && config) {
		var obj = scope[objName];
		if (typeof obj === 'undefined') {
			obj = scope;
		}
		if (config instanceof Array) {
			config.forEach(function(prop) {
				assert(typeof obj[prop] !== 'undefined', objName + '.' + prop + ' exists');
			});
			return;
		} else if (config instanceof Object) {
			Object.keys(config).forEach(function(prop) {
				assert(typeof obj[prop] === config[prop], objName + '.' + prop + ' exists and is of type "' + config[prop] + '"'); 
			});
			return;
		}
	}
	throw new Error('assert.checkMembers() received an incorrect config object');
}

assert.reset = function() {
	assert.n = 0;
}

assert.suite = function(name, callback) {
	this.reset();
	console.log();
	console.log('-------------------------------');
	console.log('  ' + name)
	console.log('-------------------------------');
	this.suitename = name;
	try {
		callback();
	} catch (e) {
		assert.n++
		assert.fail(e);
	}
}

assert.section = function(name, callback) {
	console.log();
	console.log(' ' + this.suitename + ' - ' + name);
	console.log();
	this.sectionname = name;
	if (typeof callback === 'function') {
		callback();
	}
}

assert.waitUntilReady = function() {
	while(assert.ready !== true) {
		trifle.wait(10);
	}
	assert.ready = false;
}

