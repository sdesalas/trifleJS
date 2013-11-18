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
        // Populate other properties
        this.workingDirectory = this.API.WorkingDirectory;
    }

    // Changes the current workingDirectory to the specified path.
    FileSystem.prototype.changeWorkingDirectory = function(path) {
        console.xdebug("FileSystem.prototype.changeWorkingDirectory(path)");
        return this.API.ChangeWorkingDirectory(path || '');
    }
    
    // Gets the absolute path of a file or directory
    FileSystem.prototype.absolute = function(path) {
        console.xdebug("FileSystem.prototype.absolute(path)");
        return this.API.Absolute(path || '');
    }
    
    // Returns a list of files in selected path
    FileSystem.prototype.list = function(path) {
        console.xdebug("FileSystem.prototype.list(path)");
        return this.API.List(path || '');
    }
    
    // Determines if file exists
    FileSystem.prototype.exists = function(path) {
        console.xdebug("FileSystem.prototype.exists(path)");
        return this.API.Exists(path || '');
    }
    
    // Determines if path is a directory
    FileSystem.prototype.isDirectory = function(path) {
        console.xdebug("FileSystem.prototype.isDirectory(path)");
        return this.API.IsDirectory(path || '');
    }
    
    // Determines if path is a file
    FileSystem.prototype.isFile = function(path) {
        console.xdebug("FileSystem.prototype.isFile(path)");
        return this.API.IsFile(path || '');
    }
    
    // Gets file size
    FileSystem.prototype.size = function(path) {
        console.xdebug("FileSystem.prototype.size(path)");
        return this.API.Size(path || '');
    }

    // Reads file contents
    FileSystem.prototype.read = function(path) {
        console.xdebug("FileSystem.prototype.read(path)");
        return this.API.Read(path || '');
    }
    
    // Deletes a file
    FileSystem.prototype.remove = function(path) {
        console.xdebug("FileSystem.prototype.remove(path)");
        return this.API.Remove(path || '');
    }


})(this.trifle);

