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


    // Define Module
    var FileSystem = trifle.modules.FileSystem = trifle.extend({
		
		// Derives functionality from FileSystem.cs
		module: trifle.API.FileSystem,
		
		// Constructor
		init: function() {
			console.xdebug("new FileSystem()");
		},
		
		// Additional methods
		methods: {
		
		}
    
    });
    

})(this.trifle);

