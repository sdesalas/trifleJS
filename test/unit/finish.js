
console.log();
console.log('-------------------------------');
console.log('  TESTING COMPLETED');
console.log('-------------------------------');
console.log('Total Tests: ' + assert.count);
console.API.color('green', 'Total Passed: ' + assert.pass.count);
console.API.color('red', 'Total Failed: ' + assert.fail.count);
console.log();
