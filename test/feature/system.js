var sys = new trifle.modules.System();
var sys2 = new trifle.modules.System();

console.log('created');


sys.blah('I am sys');
sys2.blah('I am sys2');

console.log({
	sys: sys,
	pid: sys.pid,
	args: sys.args,
	prototype: trifle.modules.System.prototype
});


