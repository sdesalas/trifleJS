


assert.suite('REQUIRE', function() {

    assert.section('Globals');
    
    assert(typeof require === 'function', 'require is a function');

    assert.section('Calling Inbuilt Modules');

    var fs = require('fs');
    assert(typeof fs === 'object', 'fs module can be instantiated');
    assert(typeof fs.workingDirectory === 'string', 'fs module contains a workingDirectory')
    var fs2 = require('fs');
    assert(typeof fs2 === 'object', 'fs module can be instantiated a second time');
    assert(fs2 === fs, 'fs module returns the same object when instantiated a second time');

    assert.section('Calling Modules using a path');

    var path = 'examples/universe';
    var sample_module = require(path);
    assert(typeof sample_module === 'object', 'module can be instantiated without an extension');
    assert(!!sample_module.id, 'module has an id property');
    
    assert(sample_module.answer === 42, 'module.answer is set to 42');

    path = 'examples/universe.js';
    sample_module = require(path);
    assert(typeof sample_module === 'object', 'module can be instantiated using a file path');
    assert(sample_module.answer === 42, 'module.answer is set to 42');

    var sample_module2 = require(path);
    
    assert(typeof sample_module2 === 'object', 'module can be instantiated a second time');
    assert(sample_module2.answer === 42, 'module.answer is set to 42 when instantiated a second time');
	assert(sample_module === sample_module2, 'module returns same object when instantiated a second time');

    var sample_module3 = require('examples\\universe.js');

    assert(typeof sample_module3 === 'object', 'module can be instantiated using a windows path');
    assert(sample_module3.answer === 42, 'module.answer is set to 42 when instantiated using windows path');
	assert(sample_module === sample_module3, 'module returns same object when instantiated using windows path');

});