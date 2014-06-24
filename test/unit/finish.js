
console.log();
console.log('-------------------------------');
console.log('  TESTING COMPLETED');
console.log('-------------------------------');
console.log('Total Tests: ' + assert.count);

if (assert.fail.count) { console.API.color('red', 'Total Failed: ' + assert.fail.count); }
else { console.API.color('green', 'Total Passed: ' + assert.pass.count); }

console.log();
