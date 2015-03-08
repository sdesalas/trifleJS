/*
* ssl.js
* 
* Runs a set of unit tests used to verify 
* SSL connectivity.
* 
*/


assert.suite('SSL connectivity', function() {

	if (sslSupport !== true) {
		console.warn('No SSL Support, skipping tests!');
		return;
	}
	
	// Instantiate SSL server
	var serverCount = 0;
	var server = require('webserver').create();
	server.listen('https://localhost:8043', function(request, response) { 
		serverCount++; 
		response.write("Hello SSL!"); 
		response.close(); 
	});
    
	// --------------------------------------------
	assert.section('Basic SSL Interaction (WebServer & WebPage)', function() {
    
		var clientCount = 0;
		var page = require('webpage').create();
		
		// Turn errors on
		trifle.API.IgnoreSSLErrors = false;
		
		page.open('https://localhost:8043/1', function(status) {
			clientCount++;
		});

		trifle.wait(1000);

		assert(serverCount === 0, 'Browser does not hit the server if we turn errors on');
		assert(clientCount === 1, 'Browser executes callback if we turn errors on');
		assert(page.plainText.indexOf('security certificate') > -1, 'Browser shows a certificate error page');

		
		// Turn errors off
		trifle.API.IgnoreSSLErrors = true;
		
		page.open('https://localhost:8043/2', function(status) {
			assert.ready = true;
			clientCount++;
		});
		
		assert.waitUntilReady();
		
		assert(serverCount === 1, 'Browser hits the server when ignoring SSL errors');
		assert(clientCount === 2, 'Browser executes callback if we turn SSL errors off');
		assert(page.plainText === 'Hello SSL!', 'Correct text returned from SSL server');
	    
    });
    
    assert.section('CLI Options', function() {
    
		
    
    });
    
});