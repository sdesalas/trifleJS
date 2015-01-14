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
			spawn: function(cmd, args, opts) {
				// Execute and return context object
				return this._spawn(cmd, args || [], opts);
			},
			execFile: function(cmd, args, opts, callback) {
				var child_process = this;
				var complete = function(contextId) {
					// Execute callback
					var context = child_process._findContext(contextId);
					if (context && callback && callback.call) {
						return callback.call(child_process, null, context.output, context.errorOutput);
					}
				};
				// Execute and return context object
				return this._execFile(cmd, args || [], opts, (new trifle.Callback(complete)).id);
			},
			execSync: function(cmd, args, opts) {
				if (cmd) {
					return this._execSync(cmd, args || [], opts);
				}
			}
		}
    
    });
    

})(this.trifle);

