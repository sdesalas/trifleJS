/*
* triflejs.init.js
*
* By: Steven de Salas
* On: Sep 2013
* 
*
* Runs instructions to initialize the host environment.
*
* This is particularly important in order to pipe
* asyncronous callbacks as these are not currently
* supported in the Javascript.NET project.
* 
*/


console.xdebug("Initializing require()");

// Loading module framework
// @see http://wiki.commonjs.org/wiki/Modules/1.1.1
this.exports = {
    webpage: function() {
        return new triflejs.modules.WebPage();
    },
    fs: function() {
        return new triflejs.modules.FileSystem();
    },
    system: function() {
        return new triflejs.modules.System();
    }
};

// Defines require() method
this.require = function(name) {

    if (!exports[name]) {
        console.error('require() -- Invalid module: ' + name);
        return;
    }

    var module = exports[name]();
    module.create = function() {
        return this;
    }

    return module;
}


console.xdebug("Finished initializing environment!");
console.xdebug("----------------------------------");
console.log();
console.log();
triflejs.wait(2000);

