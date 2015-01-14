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
		var stderr1 = [], stdout2 = [], stdout3 = [];
		
		assert(!!context, '.spawn() returns a context');
		assert(finished === false, '.spawn() runs asynchronously');

		assert(context.output === '', 'context.output is an empty string');
		assert(context.errorOutput === '', 'context.errorOutput is an empty string');
		assert(context.exited === false, 'context.exited is false');
		assert(context.exitCode === null, 'context.exitCode is null');

		// Try different subscription methods

		// Default
		child_process.on("stdout", function(data) {
			stdout1.push(data);	
		});
		
		// Via child
		child_process.stdout.on("data", function(data) {
			stdout2.push(data);
		});
		
		// Via Setter
		child_process.onStdOut = function(data) {
			stdout3.push(data);
		}
		
		child_process.on("exit", function() {
			assert.ready = true;
		});
	
		assert.waitUntilReady();
		
		assert(finished === true, '.spawn() runs asynchronously');

	
	});


});


