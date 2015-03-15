/*
* require.js
* 
* Runs a set of unit tests used to verify 
* system functionality (platform, arguments etc)
* 
*/

assert.suite('Object: system', function() {

	// SETUP
	var system = require('system');
	
	// --------------------------------------------
	assert.section('Properties & methods');
	
	assert.checkMembers(system, 'system', {
		pid: 'number',
		platform: 'string',
		os: 'object',
		env: 'object',
		args: 'object'
	});
	
	assert.checkMembers(system.os, 'system.os', {
		architecture: 'string',
		name: 'string',
		version: 'string'
	});
	
	var exists = {};
	
	Object.keys(system.env).forEach(function(key) {
		switch(key.toLowerCase()) {
			case 'path':
				assert(typeof system.env[key] === 'string', 'system.env "' + key + '" variable is of type string');
				exists.path = true;
				break;
			case 'programfiles':
				assert(typeof system.env[key] === 'string', 'system.env "' + key + '" variable is of type string');
				exists.programfiles = true;
				break;
		}
	});

	assert(exists.path === true, 'system.env contains the "PATH" variable');
	assert(exists.programfiles === true, 'system.env contains the "ProgramFiles" variable');

	assert(system.args instanceof Array, 'system.args is an array');
	assert(system.os.name === 'windows', 'system.os.name is "windows"');
	
	if (system.env.ProgramFiles.indexOf('x86') > 0) {
		assert(system.os.architecture === '64bit', 'system.os.architecture is "64bit"');
	} else {
		assert(system.os.architecture === '32bit', 'system.os.architecture is "32bit"');
	}
	
});
