
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
		trifle.wait(100);
	}
	assert.ready = false;
}

