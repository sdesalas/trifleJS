


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

    var path = '../../test/unit/ref/sample_module';
    var sample_module = require(path);
    assert(typeof sample_module === 'object', 'a sample_module can be instantiated without an extension');
    assert(!!sample_module.module, 'a sample_module has a module argument passed in');
    assert(!!sample_module.module.id, 'the module argument in a sample_module has an id');
    assert(sample_module.ok === true, 'sample_module.ok is set to true');

    path = '../../test/unit/ref/sample_module.js';
    sample_module = require(path);
    assert(typeof sample_module === 'object', 'sample_module can be instantiated using a file path');
    assert(sample_module.ok === true, 'sample_module.ok is set to true');

    var sample_module2 = require(path);
    assert(typeof sample_module2 === 'object', 'a sample_module can be instantiated a second time');
    assert(sample_module2.ok === true, 'sample_module.ok is set to true when instantiated a second time');
	assert(sample_module === sample_module2, 'module returns same object when instantiated a second time');

    var sample_module3 = require('..\\..\\test\\unit\\ref\\sample_module.js');
    assert(typeof sample_module3 === 'object', 'a sample_module can be instantiated using a windows path');
    assert(sample_module3.ok === true, 'sample_module.ok is set to true when instantiated using a windows path');

});