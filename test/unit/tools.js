
var assert = function(condition, message) {
	assert.n++;
	assert.count++;
    if (condition !== true) {
       assert.fail(assert.n + '. FAILS: ' + (message || '(no message)') + '.');
       assert.fail.count++;
    } else {
       assert.pass(assert.n + '. PASS: ' + (message || '(no message)') + '.');
       assert.pass.count++;
    }
}

assert.count = 0;

assert.pass = function(message) {
	console.API.color('green', message);
}

assert.pass.count = 0;

assert.fail = function(message) {
	console.API.color('red', message);
}

assert.fail.count = 0;

assert.reset = function() {
	assert.n = 0;
}

assert.suite = function(name) {
	this.reset();
	console.log('-------------------------------');
	console.log('  ' + name)
	console.log('-------------------------------');
	this.suitename = name;
}

assert.section = function(name) {
	console.log();
	console.log(' ' + this.suitename + ' - ' + name);
	console.log();
	this.sectionname = name;
}

