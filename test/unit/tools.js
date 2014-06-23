
var assert = function(condition, message) {
	assert.n++;
	assert.count++;
    if (condition !== true) {
       assert.fail(assert.n + '. FAIL: ' + (message || '(no message)') + '.');
    } else {
       assert.pass(assert.n + '. PASS: ' + (message || '(no message)') + '.');
    }
}

assert.count = 0;

assert.pass = function(message) {
	console.API.color('green', message);
    assert.pass.count++;
}

assert.pass.count = 0;

assert.fail = function(message) {
	console.API.color('red', message);
    assert.fail.count++;
}

assert.fail.count = 0;

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
	callback();
}

assert.section = function(name) {
	console.log();
	console.log(' ' + this.suitename + ' - ' + name);
	console.log();
	this.sectionname = name;
}

assert.waitFor = function(condition) {
	while(condition !== true) {
		trifle.doEvents();
	}
}

