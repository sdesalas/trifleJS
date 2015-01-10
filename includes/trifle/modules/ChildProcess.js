/*
* ChildProcess.js
*
* By: Steven de Salas
* On: Jan 2015
* 
* Defines a ChildProcess class representing a
* helper to spawn and manage new child processes
* 
*/

// Initialise Namespace
this.trifle = this.trifle || {};
trifle.modules = trifle.modules || {};

// Wrap code to avoid global vars
(function(trifle) {

    // Define Module
    var ChildProcess = trifle.modules.ChildProcess = trifle.extend({
		
		// Derives functionality from ChildProcess.cs
		module: trifle.API.ChildProcess,
		
		// Constructor
		init: function() {
			console.xdebug("new ChildProcess()");
		},
		
		// Additional methods
		methods: {
			spawn: function(cmd, args) {

			},
			execFile: function(cmd, args, opts, callback) {
			
			},
			execSync: function(cmd, args, opts) {
				if (cmd) {
					return this._execSync(cmd, args || [], opts);
				}
			}
		}
    
    });
    

})(this.trifle);

