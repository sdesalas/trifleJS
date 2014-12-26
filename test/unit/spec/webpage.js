

assert.suite('WEBPAGE MODULE', function() {

	// SETUP
	var fs = require('fs');
	var server = require('webserver').create();
	var loadCount = 0;

	// --------------------------------------------
	assert.section('Instantiation');

	assert(this.hasOwnProperty('WebPage'), 'this.WebPage exists');
	assert(typeof this.WebPage === 'function', 'this.WebPage is a function');
	assert(window.hasOwnProperty('WebPage'), 'window.WebPage exists');
	assert(typeof window.WebPage === 'function', 'window.WebPage is a function');
	var page = require("webpage").create();
	assert(typeof page === 'object', 'require("webpage").create() returns an object');

	// --------------------------------------------
	assert.section('Method availability');

	assert(typeof page.addCookie === 'function', 'page.addCookie() method available');
	//assert(typeof page.clearCookies === 'function', 'page.clearCookies() method available');
	assert(typeof page.close === 'function', 'page.close() method available');
	assert(typeof page.evaluate === 'function', 'page.evaluate() method available');
	//assert(typeof page.evaluateAsync === 'function', 'page.evaluateAsync() method available');
	assert(typeof page.evaluateJavaScript === 'function', 'page.evaluateJavaScript() method available');
	//assert(typeof page.getPage === 'function', 'page.getPage() method available');
	assert(typeof page.goBack === 'function', 'page.goBack() method available');
	assert(typeof page.goForward === 'function', 'page.goForward() method available');
	assert(typeof page.go === 'function', 'page.go() method available');
	assert(typeof page.includeJs === 'function', 'page.includeJs() method available');
	assert(typeof page.injectJs === 'function', 'page.injectJs() method available');
	assert(typeof page.open === 'function', 'page.open() method available');
	//assert(typeof page.openUrl === 'function', 'page.openUrl() method available');
	assert(typeof page.reload === 'function', 'page.reload() method available');
	assert(typeof page.render === 'function', 'page.render() method available');
	assert(typeof page.renderBase64 === 'function', 'page.renderBase64() method available');

	// --------------------------------------------
	assert.section('Properties before loading');

	assert(page.canGoBack === false, 'page.canGoBack is false');
	assert(page.canGoForward === false, 'page.canGoForward is false');
	assert(page.clipRect != null && page.clipRect.width === 0, 'page.clipRect.width is 0');
	assert(page.clipRect != null && page.clipRect.height === 0, 'page.clipRect.height is 0');
	assert(page.clipRect != null && page.clipRect.top === 0, 'page.clipRect.top is 0');
	assert(page.clipRect != null && page.clipRect.left === 0, 'page.clipRect.left is 0');
	assert(page.content != null && page.content.indexOf("<HTML>") === 0 , 'page.content has some HTML in it');
	assert(page.cookies != null && page.cookies instanceof Array, 'page.cookies is an array');
	//assert(page.frameContent != null && page.frameContent.indexOf("<HTML>") === 0 , 'page.frameContent has some HTML in it');
	//assert(page.frameName === '', 'page.frameName is an empty string');
	//assert(page.framePlainText === '', 'page.framePlainText is an empty string');
	//assert(page.frameTitle === '', 'page.frameTitle is an empty string');
	//assert(page.frameUrl === 'about:blank', 'page.frameUrl is "about:blank"');
	assert(page.framesCount === 0, 'page.framesCount is 0');
	assert(page.framesName != null && typeof(page.framesName.length) === 'number', 'page.framesName is an array');
	//assert(page.libraryPath != null, 'page.libraryPath is not null');
	assert(page.loading === false, 'page.loading is false');
	assert(page.objectName === 'WebPage', 'page.objectName is WebPage');
	assert(page.plainText === '', 'page.plainText is an empty string');
	//assert(page.scrollPosition != null && page.scrollPosition.left === 0, 'page.scrollPosition.left is 0');
	//assert(page.scrollPosition != null && page.scrollPosition.top === 0, 'page.scrollPosition.top is 0');
	assert(page.title === '', 'page.title is an empty string');
	assert(page.url === 'about:blank', 'page.url is "about:blank"');
	assert(page.windowName === '', 'page.windowName is an empty string');
	assert(page.zoomFactor === 1, 'page.zoomFactor is 1');
	assert(page.viewportSize != null && page.viewportSize.width === 400, 'page.viewportSize.width is 400');
	assert(page.viewportSize != null && page.viewportSize.height === 300, 'page.viewportSize.height is 300');

	// Start a web server listener to check that pages are loading
	var pageContent, pagePlainText;
	server.listen(8898, function(request, response) {
		loadCount++;
		pagePlainText = JSON.stringify({success: true, message: "OK", url: request.url});
		pageContent = '<html><head><title>Test Page Title</title></head><body>' + pagePlainText + '</body></html>';
		response.write(pageContent);
		response.close();
	});
	
	// --------------------------------------------
	assert.section('Simple page loading.', function() {
	
		var responseData = null;
		
		trifle.wait(500);
		page.open('http://localhost:8898', function() { 
			assert.ready = true; 
		});
		
		assert(assert.ready !== true, 'page.open(url) loads pages asynchronously');
		
		assert.waitUntilReady();
		
		try {responseData = JSON.parse(page.plainText); } catch (e) {}
		assert(responseData !== null && responseData.success, 'page.open(url) can load a simple request');
		assert(loadCount === 1, 'page.open(url) loads the page once');
	
	});
	
	// --------------------------------------------
	assert.section('Properties after loading');	

	assert(page.url === 'http://localhost:8898/', 'page.url is "http://localhost:8898/"');
	assert(page.title === 'Test Page Title', 'page.url is "Test Page Title"');
	assert(page.windowName === '', 'page.windowName is an empty string');
	assert(page.framesCount === 0, 'page.framesCount is 0');
	assert(page.plainText == pagePlainText, 'page.pagePlainText has BODY TEXT in it sent by the server');
	assert(page.content == pageContent, 'page.content has HTML in it sent by the server');

	// --------------------------------------------
	assert.section('Scripting', function() {
	
		var evaluateResult = page.evaluate(function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) {
            var result = document.title + arg1 + arg2 + arg3[0] + arg3[1] + arg4.a + ' ' + arg5 + arg6;
            if (arg7 === null) {
                result += arg7;
            };
            if (arg8 === undefined) {
                result += arg8;
            }
            return result;
        }, 'message 1', 234, [5, '6'], { a: "78" }, !true, NaN, null, undefined);
        
        assert(evaluateResult === 'Test Page Titlemessage 12345678 falseNaNnullundefined', 'page.evaluate can run scripts and return values from the page');
        
        // page.evaluateJavaScript()
		page.evaluateJavaScript('var message = "hello from ie";');
		
		evaluateResult = page.evaluate(function() { return message; });
	
		assert(evaluateResult === 'hello from ie', 'page.evaluateJavaScript executes a javascript string on page context');
		
		server.listen(8891, function(request, response) {
			response.write('window.injectedData = "test content1234";');
			response.close();
		});
		
		page.includeJs("http://localhost:8891/test.js", function() {
			assert.ready = true;
			evaluateResult = page.evaluate(function() {
				return window.injectedData || '';
			});
		});
		
		assert(assert.ready !== true, 'page.includeJs loads files asynchronously');

		assert.waitUntilReady();

		assert(evaluateResult === 'test content1234', 'page.includeJs can be used to load remote javascript files using a url');

		fs.write('injectJs.script.js', 'window.___test8206134 = "hello world 229132";');

		page.injectJs('injectJs.script.js');
		
		evaluateResult = page.evaluate(function() {
			return window.___test8206134 || '';
		});

		assert(evaluateResult === 'hello world 229132', 'page.injectJs can be used to inject local scripts into page context');

		fs.remove('injectJs.script.js');

	});	


	// --------------------------------------------
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

		while(loaded1 !== true && loaded3 !== true) {
			trifle.wait(100);
		}
		
		assert(callbacks === 2, 'page.open(url) executes each callback once.');
		assert(loadCount === 3, 'page.open(url) loads each page once.');
	
	});

	// --------------------------------------------
	assert.section('Navigating through history');
	
	assert(page.canGoBack === true, 'page.canGoBack is true');	
	assert(page.canGoForward === false, 'page.canGoForward is false');

	page.reload();

	assert(loadCount === 4, 'page.reload(url) loads the page again from the server.');

	page.goBack();
	
	assert(loadCount === 4, 'page.goBack(url) uses cache instead of loading from the server.');
	assert(page.canGoBack === false, 'page.canGoBack is false after navigating back');	
	assert(page.canGoForward === true, 'page.canGoForward is true after navigating back');

	page.goForward();
	
	assert(loadCount === 4, 'page.goForward(url) uses cache instead of loading from the server.');	
	assert(page.canGoBack === true, 'page.canGoBack is true after navigating forward');	
	assert(page.canGoForward === false, 'page.canGoForward is false after navigating forward');

	page.go(-1);
	
	assert(loadCount === 4, 'page.go(-n) uses cache instead of loading from the server.');	
	assert(page.canGoBack === false, 'page.canGoBack is false after navigating back using go(-1)');	
	assert(page.canGoForward === true, 'page.canGoForward is true after navigating back using go(-1)');

	page.go(1);
	
	assert(loadCount === 4, 'page.go(+n) uses cache instead of loading from the server.');	
	assert(page.canGoBack === true, 'page.canGoBack is true after navigating forward using go(1)');	
	assert(page.canGoForward === false, 'page.canGoForward is false after navigating forward using go(1)');


	// Clear all listeners and connections.
	server.close();

	// --------------------------------------------
	assert.section('Coookies', function() {

		var cookieSuccess = page.addCookie({
			name: 'PageTestCookie',
			value: 'ariya/phantomjs/wiki/WebPage',
			domain: 'localhost'
		});
		
		// Start a listener to check headers
		server.listen(8086, function(request, response) { 
			response.write(JSON.stringify({
				success: true, 
				url: request.url, 
				headers: request.headers
			})); 
			//console.log(request);
			response.close(); 
		});
		
		assert(cookieSuccess === true, 'page.addCookie() returned true when adding a test cookie');
		assert(page.cookies.length === 1, 'page.cookies has one cookie listed');

		var cookies = null;
		var checkCookies = function(status) {
			try {
				var response = JSON.parse(page.plainText);
				if (response && response.headers) cookies = response.headers['Cookie'];
			} catch (e) {}
			assert.ready = true;
		};

		page.open('http://localhost:8086', checkCookies);
		
		assert.waitUntilReady();
		
		assert(!!cookies && cookies.indexOf('PageTestCookie=ariya/phantomjs/wiki/WebPage') > -1, 'page.addCookie() succesfully sends a cookie to the server');
		
		// When we remove cookies for a page, 
		// they do not get removed from phantom object 
		// (except for same domain)


		// When we remove cookies for a page, 
		// it should not affect cookies added to another page.
		

	});


	// Tear down
	server.close();
	page.close();

});

