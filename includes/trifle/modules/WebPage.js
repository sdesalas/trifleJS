/*
* WebPage.js
*
* By: Steven de Salas
* On: Sep 2013
* 
* Defines an object representing a
* browser page opened inside IE environment.
* 
*/

// Initialise Namespace
this.trifle = this.trifle || {};
trifle.modules = trifle.modules || {};

// Wrap code to avoid global vars
(function(trifle) {

    // Define Constructor
    var WebPage = this.WebPage = window.WebPage = trifle.modules.WebPage = function() {
        console.xdebug("new WebPage()");
        // Instantiate a V8 WebPage object and stores it in internal API property
        this.API = trifle.API['WebPage']();
        // Assign properties
        this.customHeaders = {};
        this.viewportSize = this.API.GetViewportSize();
        this.zoomFactor = 1.0;
		// Fire Initialized event
        if (this.onInitialized) {
            page.onInitialized.call(this);
        }
    };

    // Open URL
    WebPage.prototype.open = function() {
        console.xdebug("WebPage.prototype.open()");
        var page = this, a = arguments;
        // Determine the arguments to use
        var url = a[0], method = "GET", data, callback;
        // Using: page.open(url, method, data, callback)
        if (typeof a[3] === "function") {
			method = a[1];
			data = a[2];
			callback = a[3];
        }
        // Using: page.open(url, method, callback)
        else if (typeof a[2] === "function") {
			method = a[1];
			callback = a[2];
        }
        // Using: page.open(url, callback)
        else if (typeof a[1] === "function") {
			callback = a[1];
        }
        // Fire LoadStarted event
        if (this.onLoadStarted) {
            page.onLoadStarted.call(this);
        }
        // Instantiate Callback
        var complete = function(status) {
            // Fire LoadFinished event
            if (page.onLoadFinished) {
                page.onLoadFinished.call(page, status);
            }
            // Load additional properties for current page
            page.content = page.API.Content;
            page.plainText = page.API.PlainText;
            page.url = page.API.Url;
            page.title = page.API.Title;
            // Execute callback
            if (callback && callback.call) {
				return !!callback ? callback.call(page, status) : null;
            }
        };
        // Load custom headers
        if (typeof this.customHeaders === "object") {
			var headers = [];
			for (var prop in this.customHeaders) {
				headers.push(prop + ': ' + this.customHeaders[prop] + '\r\n');
			}
			this.API.CustomHeaders = headers.join('');
        }
        // Sync internal properties
        this.syncProperties();
        // Open URL in .NET API
		return this.API.Open(url, method, data, (new trifle.Callback(complete)).id);
    };
    
    // Closes the webpage and releases memory
    WebPage.prototype.close = function() {
        console.xdebug("WebPage.prototype.close()");
		this.API.Close();
    }

    // Evaluate JS
    WebPage.prototype.evaluateJavaScript = function(code) {
        console.xdebug("WebPage.prototype.evaluateJavaScript(code)");
        if (code && typeof code === "string") {
            // Set current page (for WebPage#onCallback)
            WebPage.current = this;
			// Sync internal properties
			this.syncProperties();
            // Execute JS on IE host
            return this.API.EvaluateJavaScript(code);
        }
    };

    // Evaluate Function
    WebPage.prototype.evaluate = function(func) {
        console.xdebug("WebPage.prototype.evaluate(func)");
        if (typeof func === 'function') {
            var args = [];
            for (var i = 1; i < arguments.length; i++) {
                // Fix undefined (coming up as null)
                if (arguments[i] === undefined) {
                    arguments[i] = "{{undefined}}";
                }
                args.push(arguments[i]);
            }
            // Set current page (for WebPage#onCallback)
            WebPage.current = this;
			// Sync internal properties
			this.syncProperties();
            // Execute JS on IE host
            return this.API.Evaluate(func.toString(), args);
        }
        return null;
    };

    // Inject JS file
    WebPage.prototype.injectJs = function(filename) {
        console.xdebug("WebPage.prototype.injectJs(filename)");
        if (typeof filename === 'string') {
            // Set current page (for WebPage#onCallback)
            WebPage.current = this;
			// Sync internal properties
			this.syncProperties();
            // Execute JS on IE host
            return this.API.InjectJs(filename);
        }
    }

    // Include remote JS
    WebPage.prototype.includeJs = function(url, callback) {
        console.xdebug("WebPage.prototype.includeJs(url, callback)");
        var page = this;
        if (typeof url === 'string') {
            var complete = function() {
                if (callback && callback.call) {
                    callback.call(page);
                }
            };
            // Set current page (for WebPage#onCallback)
            WebPage.current = this;
			// Sync internal properties
			this.syncProperties();
            // Execute JS on IE host
            return this.API.IncludeJs(url, (new trifle.Callback(complete)).id);
        }
    }


    // Render File
    WebPage.prototype.render = function(filename) {
        console.xdebug("WebPage.prototype.render(filename)");
        if (filename) {
			// Sync internal properties
			this.syncProperties();
            return this.API.Render(filename)
        };
    }

    // Render to Base64 string
    WebPage.prototype.renderBase64 = function(format) {
        console.xdebug("WebPage.prototype.renderBase64(format)");
		// Sync internal properties
		this.syncProperties();
        return this.API.RenderBase64(format || "PNG");
    }
    
    // Helper function to sync internal properties
    WebPage.prototype.syncProperties = function() {
        // Viewport Size
        if (typeof this.viewportSize === "object") {
			this.API.SetViewportSize(this.viewportSize.width || 0, this.viewportSize.height || 0);
        }
        if (typeof this.zoomFactor === "number" && this.zoomFactor > 0) {
			this.API.ZoomFactor = this.zoomFactor;
        }
    }

    // STATIC PROPERTIES

    // Currently running IE instance
    WebPage.current = null;

    // Add static onCallback() method for event handling
    WebPage.onCallback = function(args) {
        console.xdebug("WebPage.onCallback(args)");
        var page = WebPage.current;
        if (page && page.onCallback && page.onCallback.apply) {
            return page.onCallback.apply(page, args);
        }
    }

    // Add static onError() method for event handling
    WebPage.onError = function(msg, line, url) {
        console.xdebug("WebPage.onCallback(args)");
        var page = WebPage.current;
        if (page && page.onError && page.onError.call) {
            page.onError.call(page, msg, [{ line: line, file: url}]);
            return true;
        }
        return false;
    }


})(this.trifle);

