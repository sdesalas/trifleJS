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

	// Private
	var Callback = trifle.Callback;

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
			// Decorate a childprocess context with event listeners
			// to support 'stdout' and 'stderr' events
			// as well as their counterpart 'fakestream' objects
			decorate: function(ctx) {
				if (ctx) {
					// Add StdOut and StdErr Events
					Object.addEvent(ctx, "onExit");
					Object.addEvent(ctx, "onStdOut");
					Object.addEvent(ctx, "onStdErr");
					// Pipe output to stdout and stderr 'fakestream' objects
					ctx.stdout = {};
					ctx.stderr = {};
					Object.addEvent(ctx.stdout, "onData");
					Object.addEvent(ctx.stderr, "onData");
					ctx.on('stdout', function(data) {
						ctx.stdout.fireEvent('data', [data]);
					});
					ctx.on('stderr', function(data) {
						ctx.stderr.fireEvent('data', [data]);
					});
				}
				return ctx;
			},
			spawn: function(cmd, args, opts) {
				// Add event listeners, spawn and return context object
				var context; 
				var exit = function(id, code) {context.fireEvent('exit', [code]);};
				var stdout = function(data) {context.fireEvent('stdout', [data]);};
				var stderr = function(data) {context.fireEvent('stderr', [data]);};
				context = this._spawn(cmd, args || [], opts, Callback.id(exit), Callback.id(stdout), Callback.id(stderr));
				return this.decorate(context);
			},
			execFile: function(cmd, args, opts, callback) {
				var cp = this;
				var complete = function(contextId) {
					// Execute callback
					var context = cp._findContext(contextId);
					if (context && callback && callback.call) {
						return callback.call(cp, null, context.output, context.errorOutput);
					}
				};
				// Execute and return context object
				return this._execFile(cmd, args || [], opts, Callback.id(complete));
			},
			execSync: function(cmd, args, opts) {
				if (cmd) {
					return this._execSync(cmd, args || [], opts);
				}
			}
		}
    
    });
    

})(this.trifle);

