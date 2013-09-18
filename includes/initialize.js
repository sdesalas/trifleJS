/*
* initialize.js
*
* By: Steven de Salas
* On: Sep 2013
* 
*
* Runs a whole set of instructions to initialize 
* the host environment.
*
* This is particularly important in order to pipe
* asyncronous callbacks as these are not currently
* supported in the Javascript.NET project.
* 
*/


console.debug("Initializing require()");

// Loading module framework
// @see http://wiki.commonjs.org/wiki/Modules/1.1.1
this.exports = {
    webpage: function() {
        return new triflejs.modules.WebPage();
    },
    fs: function() {
        return new triflejs.modules.FileSystem();
    }
};

this.require = function(name) {
    return {
        create: function() {
            if (!exports[name]) {
                console.error('require.create() -- Invalid module: ' + name);
                return;
            }
            return exports[name]();
        }
    }
}


console.debug("Finished initializing environment!");
console.clear();