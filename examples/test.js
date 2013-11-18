
/*



var page = new WebPage();

var msg = "message",
    value = "value",
    result,
    expected = "extra-value";
page.onPrompt = function(msg, value) {
    return "extra-"+value;
};
result = page.evaluate(function(m, v) {
    return window.prompt(m, v);
}, msg, value);
console.log("checkPagePrompt:result(should be : " + expected + "):" + result);



var sys = require("system");

console.log("sys.pid: " + sys.pid);
console.log("sys.os: ", sys.os);
console.log("sys.env: ", sys.env);

*/

/*
var page = new WebPage();
page.open("http://www.google.com", function() {
	console.log("page.url", page.url);
	console.log("page.title", page.title);
	console.log("page.plainText", page.plainText);
})
*/

var fs = require("fs");
var path = "examples/test.js";

var pathInfo = {
	absolute: fs.absolute(path),
	list: fs.list(path),
	size: Math.round(fs.size(path) / 1024),
	exists: fs.exists(path),
	isDirectory: fs.isDirectory(path),
	isFile: fs.isFile(path)
} 

console.log("pathInfo:", pathInfo);

if (pathInfo.isFile) {
	console.log("Reading file " + path);
	console.log(fs.read(path));
}

