/*
* FileSystem.js
*
* By: Steven de Salas
* On: Sep 2013
* 
* Defines a FileSystem class representing a
* helper for file read/write operations and management.
* 
*/

// Initialise Namespace
this.trifle = this.trifle || {};
trifle.modules = trifle.modules || {};

// Wrap code to avoid global vars
(function(trifle) {


    // Define Constructor
    var FileSystem = trifle.modules.FileSystem = function() {
        console.xdebug("new FileSystem()");
        // Instantiate a V8 FileSystem object and stores it in internal API property
        this.API = trifle.API['FileSystem']();
        // Set the working directory
        this.workingDirectory = this.API.WorkingDirectory;
    }

    // Changes the current workingDirectory to the specified path.
    FileSystem.prototype.changeWorkingDirectory = function(path) {
        console.xdebug("FileSystem.prototype.changeWorkingDirectory(path)");
        return this.API.ChangeWorkingDirectory(path || '');
    }


})(this.trifle);

