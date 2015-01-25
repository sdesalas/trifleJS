/*
* child_process.js
* 
* Runs a set of unit tests used that check
* the functionality of the child process object
* 
*/


assert.suite('Module: ChildProcess', function() {

	var child_process = require("child_process");
	
	// --------------------------------------------
	assert.section('Instantiation');
	// --------------------------------------------

	assert(!!child_process, 'child_process can be instantiated using require()');
	assert(typeof child_process === 'object', 'child_process is an object');

	// --------------------------------------------
	assert.section('Properties & methods');
	// --------------------------------------------

	assert.checkMembers(child_process, 'child_process', {
		'spawn': 'function',
		'execFile': 'function',
		'execSync': 'function'
	});
	
	// --------------------------------------------
	assert.section('Context Object', function() {

		var context = child_process.execSync('dir');
		
		assert(!!context, '.execSync() returns a context with a single argument');

		assert.checkMembers(context, 'context', {
			'output': 'string',
			'errorOutput': 'string',
			'kill': 'function',
			'start': 'function',
			'execute': 'function',
			'exited': 'boolean',
			'exitCode': 'number'
		});
		
		assert(context.output.substr(0, 10) === ' Volume in', 'context.output returns executable output');
		assert(context.errorOutput === '', 'context.errorOutput is empty');
		assert(context.exited === true, 'context.exited returns true when finished');
		assert(context.exitCode === 0, 'context.exitCode returns 0 when there is no error');
		
		assert(context.start('dir', ['/p']) === false, 'context.start() returns false once exited.');
		assert(context.execute('dir', ['/p']) === false, 'context.execute() returns false once exited.');
		assert(context.kill() === false, 'context.kill() returns false once exited.');
		
		// Check error
		
		var context = child_process.execSync('unknown234');

		assert(context.errorOutput.indexOf("'unknown234' is not recognized") === 0, 'context.errorOutput returns executable error output');
		assert(context.output === '', 'context.output is empty');
		assert(context.exited === true, 'context.exited returns true when finished');
		assert(context.exitCode === 1, 'context.exitCode returns 1 when there is an error');
		
	});
	
	// --------------------------------------------
	assert.section('Synchronous execution', function() {
		
		// Test executable (local, with arguments)
		var context = child_process.execSync('triflejs.exe', ['--help']);
		
		assert(context.errorOutput === '', 'context.errorOutput is empty when executing with arguments');
		assert(context.output.indexOf('Headless automation for Internet Explorer') > 0, 'context.output as expected when executing with arguments.');
		assert(context.exitCode === 0, 'context.exitCode returns 0 when executing with arguments');
		
		// Test executable (use of PATH variable)
		var context = child_process.execSync('ping.exe', ['-?']);
		
		assert(context.errorOutput === '', 'context.errorOutput is empty when depending on PATH variable');
		assert(context.output.indexOf('Ping the specified host until stopped') > 0, 'context.output as expected when depending on PATH variable.');
		assert(context.exitCode === 0, 'context.exitCode returns 1 when depending on PATH variable');

	});
	
	// --------------------------------------------
	assert.section('Asynchronous execution', function() {
	
		var finished = false;
		var version = [];
		Object.keys(trifle.version).forEach(function(k) {version.push(trifle.version[k]);});
		var context = child_process.execFile('triflejs.exe', ['--version'], null, function(err, stdout, stderr) {
			finished = true;
			assert.ready = true;
		});
		
		assert(!!context, '.execFile() returns a context');
		assert(finished === false, '.execFile() runs asynchronously');
		
		assert(context.output === '', 'context.output is an empty string');
		assert(context.errorOutput === '', 'context.errorOutput is an empty string');
		assert(context.exited === false, 'context.exited is false');
		assert(context.exitCode === null, 'context.exitCode is null');
	
		assert.waitUntilReady();
		
		// Finished
		assert(finished === true, '.execFile() runs asynchronously');
		assert(context.output.indexOf(version.join('.')) > 0, 'context.output contains ' + version.join('.'));
		assert(context.errorOutput === '', 'context.errorOutput is an empty string');
		assert(context.exited === true, 'context.exited is true');
		assert(context.exitCode === 0, 'context.exitCode is 0');
	
	});

	
	// --------------------------------------------
	assert.section('Event handling', function() {
	
		var finished = false;
		var context = child_process.spawn('triflejs.exe', ['--help']);
		var stdout1 = [], stdout2 = [], stdout3 = [];
		var stderr1 = [], stderr2 = [], stderr3 = [];
		var returnCode = null;
		
		assert(!!context, '.spawn() returns a context');
		assert(finished === false, '.spawn() runs asynchronously');

		assert(context.output === '', 'context.output is an empty string');
		assert(context.errorOutput === '', 'context.errorOutput is an empty string');
		assert(context.exited === false, 'context.exited is false');
		assert(context.exitCode === null, 'context.exitCode is null');

		// Try different subscription methods

		// Default
		context.on("stdout", function(data) { stdout1.push(data); });
		context.on("stderr", function(data) { stderr1.push(data); });
		
		// Via child 'fakestream'
		context.stdout.on("data", function(data) { stdout2.push(data); });
		context.stderr.on("data", function(data) { stderr2.push(data); });
		
		// Via Setter
		context.onStdOut = function(data) { stdout3.push(data); }
		context.onStdErr = function(data) { stderr3.push(data); }
		
		context.on("exit", function(code) {
			finished = true;
			returnCode = code;
			assert.ready = true;
		});
	
		assert.waitUntilReady();
		
		assert(finished === true, '.spawn() runs asynchronously');
		assert(returnCode === 0, 'context#exit event has a return code of 0 as argument');

		assert(stdout1.length > 10, 'context#stdout returns some data.');
		assert(stdout1.length === stdout2.length && stdout2.length === stdout3.length, 'There are 3 valid ways to subscribe to stdout event data');
		assert(stdout1.join('') === stdout2.join('') && stdout2.join('') === stdout3.join(''), 'All 3 stdout event subscription methods return same data');
		assert(stdout1.join('') === context.output, 'context#stdout returns actual output');
		assert(context.output.indexOf('Headless automation for Internet Explorer') > -1, 'context#stdout returns actual output');

		assert(stderr1.length === 0, 'context#stderr returns no data.');
		assert(stderr1.length === stderr2.length && stderr2.length === stderr3.length, 'There are 3 valid ways to subscribe to stderr event data');
		assert(stderr1.join('') === stderr2.join('') && stderr2.join('') === stderr3.join(''), 'All 3 stderr event subscription methods return same data');
		assert(stderr1.join('') === context.errorOutput, 'context#stderr returns actual error output');
	
	});
	
	
	// --------------------------------------------
	assert.section('Error handling', function() {
	
		var finished = false;
		var context = child_process.spawn('triflejs.exe', ['--debug=false', 'unknown.js']);
		var stdout1 = [], stdout2 = [], stdout3 = [];
		var stderr1 = [], stderr2 = [], stderr3 = [];
		var returnCode = null;
		
		assert(!!context, '.spawn() returns a context');
		assert(finished === false, '.spawn() runs asynchronously');

		// Try different subscription methods

		// Default
		context.on("stdout", function(data) { stdout1.push(data); });
		context.on("stderr", function(data) { stderr1.push(data); });
		
		// Via child 'fakestream'
		context.stdout.on("data", function(data) { stdout2.push(data); });
		context.stderr.on("data", function(data) { stderr2.push(data); });
		
		// Via Setter
		context.onStdOut = function(data) { stdout3.push(data); }
		context.onStdErr = function(data) { stderr3.push(data); }
		
		context.on("exit", function(code) {
			finished = true;
			returnCode = code;
			assert.ready = true;
		});
	
		assert.waitUntilReady();
		
		assert(finished === true, '.spawn() runs asynchronously');
		assert(returnCode === 1, 'context#exit event has a return code of 1 as argument');

		assert(stdout1.length === 1, 'context#stdout returns one line of data.');
		assert(stdout1.length === stdout2.length && stdout2.length === stdout3.length, 'There are 3 valid ways to subscribe to stdout event data');
		assert(stdout1.join('') === stdout2.join('') && stdout2.join('') === stdout3.join(''), 'All 3 stdout event subscription methods return same data');
		assert(stdout1.join('') === context.output, 'context#stdout returns actual output');

		assert(stderr1.length > 0, 'context#stderr returns some data.');
		assert(stderr1.length === stderr2.length && stderr2.length === stderr3.length, 'There are 3 valid ways to subscribe to stderr event data');
		assert(stderr1.join('') === stderr2.join('') && stderr2.join('') === stderr3.join(''), 'All 3 stderr event subscription methods return same data');
		assert(stderr1.join('') === context.errorOutput, 'context#stderr returns actual error output');
		assert(context.errorOutput.indexOf('File does not exist') > -1, 'context#stderr returns actual error output');
	
	
	});


});


