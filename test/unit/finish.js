
console.log();
console.log('-------------------------------');
console.log('  TESTING COMPLETED');
console.log('-------------------------------');
console.log('Total Tests: ' + assert.count);

// ANY ERRORS?
if (assert.fail.count) { 
	console.API.color('red', 'Total Failed: ' + assert.fail.count); 
	console.log();
	// RETURN ERROR
	phantom.exit(1);
}
else { 
	// ALL GOOD!
	console.API.color('green', 'Total Passed: ' + assert.pass.count); 
	console.log();
	phantom.exit(0);
}

