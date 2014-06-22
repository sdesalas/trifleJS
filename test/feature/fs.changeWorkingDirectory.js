
var fs = require('fs');

console.log("Current Working Dir: " + fs.workingDirectory);
fs.changeWorkingDirectory("C:/");
console.log("Current Working Dir: " + fs.workingDirectory);

phantom.exit();

