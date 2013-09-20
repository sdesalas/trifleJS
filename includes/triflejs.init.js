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


console.xdebug("Finished initializing environment!");
console.log();
console.log();
console.wait(2000);

