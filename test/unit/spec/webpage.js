
assert.suite('WEBPAGE MODULE', function() {

	assert.section('Instantiation');

	assert(this.hasOwnProperty('WebPage'), 'this.WebPage exists');
	assert(typeof this.WebPage === 'function', 'this.WebPage is a function');
	assert(window.hasOwnProperty('WebPage'), 'window.WebPage exists');
	assert(typeof window.WebPage === 'function', 'window.WebPage is a function');
	var page = require("webpage").create();
	assert(typeof page === 'object', 'require("webpage").create() returns an object');

	assert.section('Method availability');

	assert(typeof page.goBack === 'function', 'page.goBack() method available');
	assert(typeof page.goForward === 'function', 'page.goForward() method available');

	assert.section('Properties before loading');

	assert(page.canGoBack === false, 'page.canGoBack is false');
	assert(page.canGoForward === false, 'page.canGoForward is false');
	assert(page.clipRect != null && page.clipRect.width === 0, 'page.clipRect.width is 0');
	assert(page.clipRect != null && page.clipRect.height === 0, 'page.clipRect.height is 0');
	assert(page.clipRect != null && page.clipRect.top === 0, 'page.clipRect.top is 0');
	assert(page.clipRect != null && page.clipRect.left === 0, 'page.clipRect.left is 0');
	assert(page.content != null && page.content.indexOf("<HTML>") === 0 , 'page.content has some HTML in it');
	//assert(page.frameContent != null && page.frameContent.indexOf("<HTML>") === 0 , 'page.frameContent has some HTML in it');
	//assert(page.frameName === '', 'page.frameName is an empty string');
	//assert(page.framePlainText === '', 'page.framePlainText is an empty string');
	//assert(page.frameTitle === '', 'page.frameTitle is an empty string');
	//assert(page.frameUrl === 'about:blank', 'page.frameUrl is "about:blank"');
	//assert(page.framesCount === 0, 'page.framesCount is 0');
	//assert(page.libraryPath != null, 'page.libraryPath is not null');
	assert(page.loading === false, 'page.loading is false');
	//assert(page.plainText === '', 'page.plainText is an empty string');
	//assert(page.scrollPosition != null && page.scrollPosition.left === 0, 'page.scrollPosition.left is 0');
	//assert(page.scrollPosition != null && page.scrollPosition.top === 0, 'page.scrollPosition.top is 0');
	assert(page.title === '', 'page.title is an empty string');
	assert(page.url === 'about:blank', 'page.url is "about:blank"');
	//assert(page.windowName === '', 'page.windowName is an empty string');
	assert(page.zoomFactor === 1, 'page.zoomFactor is 1');
	//assert(page.viewportSize != null && page.viewportSize.width === 400, 'page.viewportSize.width is 400');
	//assert(page.viewportSize != null && page.viewportSize.height === 300, 'page.viewportSize.height is 300');


	// Instantiate a web server to check that pages are loading
	var server = require('webserver').create();
	server.listen(8898, function(request, response) {
		response.write(JSON.stringify({success: true, message: "OK", url: request.url}));
		response.close();
	});
	
	assert.section('Simple page loading.', function() {
	
		var loaded = false, responseData = null;
		
		trifle.wait(500);
		page.open('http://localhost:8898', function() { 
			loaded = true; 
		});
		
		assert.waitFor(loaded);
		try {responseData = JSON.parse(page.plainText); } catch (e) {}
		assert(responseData !== null && responseData.success, 'page.open(url) can load a simple request');
	
	});
	
	assert.section('Sequential page loading.', function() {
	
		var loaded1 = false, loaded3 = false, responseData = null;
		var callbacks = 0;
		
		page.open('http://localhost:8898/1', function() {
			loaded1 = true; 
			callbacks++;
			try {responseData = JSON.parse(page.plainText)} catch (e) {}
			assert(responseData !== null && responseData.success && responseData.url === '/1', 'page.open(url) loads 1/3 sequential requests');
			page.open('http://localhost:8898/2');
			try {responseData = JSON.parse(page.plainText)} catch (e) {}
			assert(responseData !== null && responseData.success && responseData.url === '/1', 'page.open(url) continues executing when callback is missing');
			page.open('http://localhost:8898/3', function() {
				loaded3 = true;
				callbacks++;
				try {responseData = JSON.parse(page.plainText)} catch (e) {}
				assert(responseData !== null && responseData.success && responseData.url === '/3', 'page.open(url) loads 3/3 sequential requests');
			});
		});

		assert.waitFor(loaded1);
		assert.waitFor(loaded3);
		
		assert(callbacks === 2, 'page.open(url) executes each callback once.');
	
	});
	
	assert.section('Properties after loading');
	
	assert(page.canGoBack === true, 'page.canGoBack is true');	
	assert(page.canGoForward === false, 'page.canGoForward is false');
	
	/*
	assert.section('Navigating through history');

	page.goBack();
	
	assert.waitFor(page.loading === false);
	
	page.render('back.png');
	
	assert(page.canGoBack === false, 'page.canGoBack is false after navigating back');	
	assert(page.canGoForward === true, 'page.canGoForward is true after navigating back');

	page.goForward();
	
	assert.waitFor(page.loading === false);

	page.render('forward.png');
	
	assert(page.canGoBack === true, 'page.canGoBack is true');	
	assert(page.canGoForward === false, 'page.canGoForward is false');
*/

	// Finish serving web pages
	server.close();
	

});

