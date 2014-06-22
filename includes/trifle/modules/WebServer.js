/*
* WebServer.js
*
* By: Steven de Salas
* On: Jan 2014
* 
* Defines a WebServer class representing a
* HTTP Daemon.
* 
*/

// Initialise Namespace
this.trifle = this.trifle || {};
trifle.modules = trifle.modules || {};

// Wrap code to avoid global vars
(function (trifle) {

    // Define Module
    var WebServer = trifle.modules.WebServer = trifle.extend({
		
		// Derives functionality from WebServer.cs
		module: trifle.API.WebServer,
		
		// Constructor
		init: function() {
			console.xdebug("new WebServer()");
			// Properties
			this.objectName = "WebServer";
        },
        
        // Additional methods
        methods: {

			// Listen for incoming requests
			listen: function(binding, callback) {
				console.xdebug("Webserver.prototype.listen(binding, callback)");
				var API = this;
				// Instantiate Callback
				var complete = function(connectionId) {
					// Get Request & Response
					var request = API._getRequest(connectionId);
					var response = API._getResponse(connectionId);
					// Execute callback
					if (callback && callback.call) {
						return !!callback ? callback.call(API, request, response) : null;
					}
				};
				// Start listening on binding
				return API._listen(binding, (new trifle.Callback(complete)).id);
			}
        }
    });
    

})(this.trifle);

