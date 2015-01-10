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
	assert.section('Context Object');
	// --------------------------------------------

	var context = child_process.execSync('dir');
	
	assert(!!context, '.execSync() returns a context with a single argument');

	assert.checkMembers(context, 'context', {
		'stdOut': 'string',
		'stdErr': 'string',
		'kill': 'function',
		'start': 'function',
		'execute': 'function',
		'exited': 'boolean'
	});
	
	assert(context.stdOut.substr(0, 10) === ' Volume in', 'context.stdOut returns executable output');
	assert(context.stdErr === '', 'context.stdErr is empty');
	assert(context.exited === true, 'context.exited returns true when finished');
	
	// --------------------------------------------
	assert.section('Synchronous execution');
	// --------------------------------------------

	// Test filesystem commands
	// Test arguments 
	// Test executables
	// Test path usage
	
	// --------------------------------------------
	assert.section('Asynchronous execution');
	// --------------------------------------------


	// --------------------------------------------
	assert.section('Event handling');
	// --------------------------------------------


});


