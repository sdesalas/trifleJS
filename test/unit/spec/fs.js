
assert.suite('FS MODULE', function() {

	assert.section('Instantiation');

	var fs = require("fs");

	assert(!!fs, 'fs can be instantiated using require()');
	assert(typeof fs === 'object', 'fs is an object');

	assert.section('Properties & methods');

	assert(typeof fs.absolute === 'function', 'fs.absolute() is defined');
	assert(typeof fs.changeWorkingDirectory === 'function', 'fs.changeWorkingDirectory() is defined');
	assert(typeof fs.exists === 'function', 'fs.exists() is defined');
	assert(typeof fs.isAbsolute === 'function', 'fs.isAbsolute() is defined');
	assert(typeof fs.isDirectory === 'function', 'fs.isDirectory() is defined');
	assert(typeof fs.isExecutable === 'function', 'fs.isExecutable() is defined');
	assert(typeof fs.isFile === 'function', 'fs.isFile() is defined');
	assert(typeof fs.isLink === 'function', 'fs.isLink() is defined');
	assert(typeof fs.isReadable === 'function', 'fs.isReadable() is defined');
	assert(typeof fs.isWritable === 'function', 'fs.isWritable() is defined');
	assert(typeof fs.list === 'function', 'fs.list() is defined');
	assert(typeof fs.makeDirectory === 'function', 'fs.makeDirectory() is defined');
	assert(typeof fs.makeTree === 'function', 'fs.makeTree() is defined');
	assert(typeof fs.removeDirectory === 'function', 'fs.removeDirectory() is defined');
	assert(typeof fs.removeTree === 'function', 'fs.removeTree() is defined');
	assert(typeof fs.copyTree === 'function', 'fs.copyTree() is defined');
	assert(typeof fs.readLink === 'function', 'fs.readLink() is defined');
	assert(typeof fs.separator === 'string' && fs.separator === "\\", 'fs.separator is defined as a windows separator (\\)');
	assert(typeof fs.workingDirectory === 'string' && fs.workingDirectory.indexOf(":\\") == 1, 'fs.workingDirectory is defined and has a windows path (ie C:\\..)');
	assert(typeof fs.open === 'function', 'fs.open() is defined');
	assert(typeof fs.read === 'function', 'fs.read() is defined');
	assert(typeof fs.write === 'function', 'fs.write() is defined');
	assert(typeof fs.size === 'function', 'fs.size() is defined');
	assert(typeof fs.remove === 'function', 'fs.remove() is defined');
	assert(typeof fs.copy === 'function', 'fs.copy() is defined');
	assert(typeof fs.move === 'function', 'fs.move() is defined');
	assert(typeof fs.touch === 'function', 'fs.touch() is defined');

	assert.section('Stream instantiation');

	var path = "../../test/unit/ref/fs.txt";

	assert(fs.exists(path), 'file fs.txt exists')

	var stream = fs.open(path, "rw");

	assert(typeof stream === 'object', 'file stream can be instantiated');
	assert(typeof stream.atEnd === 'function', 'stream.atEnd() is defined');
	assert(typeof stream.close === 'function', 'stream.close() is defined');
	assert(typeof stream.flush === 'function', 'stream.flush() is defined');
	assert(typeof stream.read === 'function', 'stream.read() is defined');
	assert(typeof stream.readLine === 'function', 'stream.readLine() is defined');
	assert(typeof stream.seek === 'function', 'stream.seek() is defined');
	assert(typeof stream.write === 'function', 'stream.write() is defined');
	assert(typeof stream.writeLine === 'function', 'stream.writeLine() is defined');

	assert(stream.read() === 'original text', 'file stream can be read sucessfully');
	stream.close();

	assert.section('File Operations using fs');

	assert(fs.read(path) === 'original text', 'fs.read() can read text sucessfully');

	fs.write(path, 'some new text', 'w');
	
	assert(fs.read(path) === 'some new text', 'fs.write() can write text sucessfully');

	fs.write(path, ', extra text', 'a');

	assert(fs.read(path) === 'some new text, extra text', 'fs.write() can append text sucessfully');

	// reset file to original
	fs.write(path, 'original text');

});