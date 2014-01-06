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
(function(trifle) {


    // Define Constructor
    var WebServer = trifle.modules.WebServer = function() {
        console.xdebug("new WebServer()");
        // Instantiate a V8 WebServer object and stores it in internal API property
        this.API = trifle.API['WebServer']();
        this.port = '';
        this.objectName = 'WebServer';
    };
    
	// Listen for incoming requests
	WebServer.prototype.listen = function(binding, callback) {
        console.xdebug("Webserver.prototype.listen(binding, callback)");
        var webserver = this;
        // Instantiate Callback
        var complete = function(connectionId) {
			console.log('Processing connection: ' + connectionId);
			// Get Request & Response
			var request = webserver.API.GetRequest(connectionId);
			var response = webserver.API.GetResponse(connectionId);
            // Execute callback
            if (callback && callback.call) {
				return !!callback ? callback.call(webserver, request, response) : null;
            }
        };
        var result = this.API.Listen(binding, (new trifle.Callback(complete)).id);
        this.port = this.API.Port;
        return result;
	};
	
	// Shutdown the server
	WebServer.prototype.close = function() {
		this.API.Close();
	}

})(this.trifle);

