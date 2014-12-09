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
			listen: function(binding, opts, callback) {
				console.xdebug("Webserver.prototype.listen(binding, opts, callback)");
				var API = this;
				if (typeof callback === 'undefined' && typeof opts === 'function') {
					callback = opts;
					opts = {};
				}
				
				// Instantiate Callback
				var complete = function(connectionId) {
					// Get Request & Response
					var request = {}, _request = API._getRequest(connectionId);
					var response = API._getResponse(connectionId);
					// Decorate request object so it can be printed using console.log() or JSON.stringify()
					request.httpVersion = _request.httpVersion;
					request.method = _request.method;
					request.headers = _request.headers;
					request.post = _request.post;
					request.url = _request.url;
					request.rawPost = _request.rawPost;
					request.write = function() { return _request.write.apply(_request, arguments); };
					request.close = function() { return _request.close.apply(_request, arguments); };
					// Execute callback
					if (callback && callback.call) {
						var result = !!callback ? callback.call(API, request, response) : null;
						if (!opts.keepAlive) {
							response.close();
						}
						return result;
					}
				};
				// Start listening on binding
				return API._listen(binding, (new trifle.Callback(complete)).id);
			}
        }
    });
    

})(this.trifle);

