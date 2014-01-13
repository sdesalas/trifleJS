/*
* System.js
*
* By: Steven de Salas
* On: Sep 2013
* 
* Defines a System class representing a
* general program helper.
* 
*/

// Initialise Namespace
this.trifle = this.trifle || {};
trifle.modules = trifle.modules || {};

// Wrap code to avoid global vars
(function(trifle) {


    // Define Module
    var System = trifle.modules.System = trifle.extend({
    
		// Derives functionality from System.cs
		module: trifle.API.System,
		
		// Constructor
		init: function() {
			console.xdebug("new System()");
        }
        
    });


})(this.trifle);

