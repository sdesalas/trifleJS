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
this.trifle.modules = this.trifle.modules || {};

// Wrap code to avoid global vars
(function (trifle) {

	// Private
	var Callback = trifle.Callback;
	
    // Define Module
    var WebPage = this.WebPage = window.WebPage = trifle.modules.WebPage = trifle.extend({
		
		// Derives functionality from WebPage.cs
		module: trifle.API.WebPage,

        // Constructor
		init: function() {
			console.xdebug("new WebPage()");
			// Properties
			this.objectName = "WebPage";
			// Store reference
			WebPage.all[this.uuid] = this;
			// Add Events
			Object.addEvent(this, 'onCallback');
			Object.addEvent(this, 'onInitialized');
			Object.addEvent(this, 'onAlert');
			Object.addEvent(this, 'onConfirm', true); // unique (uses return value)
			Object.addEvent(this, 'onPrompt', true); // unique (uses return value)
			Object.addEvent(this, 'onError');
			// Run pending COM events
			trifle.wait(1);
		},
		
		// Additional methods
		methods: {

            // Opens a URL
			open: function() {
				console.xdebug("WebPage.prototype.open()");
				var page = this, a = arguments;
				// Determine the arguments to use
				var url = a[0], method = "GET", data, headers, callback;
				// Using: page.open(url, method, data, callback)
				if (typeof a[4] === "function") {
					method = a[1];
					data = a[2];
					headers = a[3];
					callback = a[4];
				}
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
					// Execute callback
					if (callback && callback.call) {
						return !!callback ? callback.call(page, status) : null;
					}
				};
				// Open URL in .NET API
				return this._open(url, method, data, Callback.id(complete));
			},

            // Executes a JavaScript code string in the browser
			evaluateJavaScript: function(code) {
				console.xdebug("WebPage.prototype.evaluateJavaScript(code)");
				if (code && typeof code === "string") {
					// Set current page (for WebPage events)
					WebPage.current = this;
					// Execute JS on IE host
					return this._evaluateJavaScript(code);
				}
			},


            // Executes a javascript function inside the browser
			evaluate: function(func) {
				console.xdebug("WebPage.prototype.evaluate(func)");
				if (!(typeof func === 'function' || typeof func === 'string' || func instanceof String)) {
					throw "Wrong use of WebPage.evaluate()";
				}
				var args = [];
				for (var i = 1; i < arguments.length; i++) {
					// Fix undefined (coming up as null)
					if (arguments[i] === undefined) {
						arguments[i] = "{{undefined}}";
					}
					args.push(arguments[i]);
				}
				// Set current page (for WebPage events)
				WebPage.current = this;
				// Execute JS on IE host
				return JSON.parse(this._evaluate(func.toString(), args));
			},
			
			// Evaluations a function in the context of the page without 
			// blocking execution. Can be delayed by a specific timeout.
			evaluateAsync: function(func, timeMs) {
				console.xdebug("WebPage.prototype.evaluateAsync(func)");
				if (!(typeof func === 'function' || typeof func === 'string' || func instanceof String)) {
					throw "Wrong use of WebPage.evaluateAsync()";
				}
				var page = this, args = Array.prototype.slice.call(arguments, 2);
				args.unshift(func);
				window.setTimeout(function() {
					page.evaluate.apply(page, args);
				}, timeMs || 0);
			},

            // Injects a local JavaScript file into the browser
			injectJs: function(filename) {
				console.xdebug("WebPage.prototype.injectJs(filename)");
				if (typeof filename === 'string') {
					// Set current page (for WebPage events)
					WebPage.current = this;
					// Execute JS on IE host
					return this._injectJs(filename);
				}
			},

            // Includes a JS file from remote URL and executes it on the browser
			includeJs: function(url, callback) {
				console.xdebug("WebPage.prototype.includeJs(url, callback)");
				var page = this;
				if (typeof url === 'string') {
					var complete = function() {
						if (callback && callback.call) {
							callback.call(page);
						}
					};
					// Set current page (for WebPage events)
					WebPage.current = this;
					// Execute JS on IE host
					return this._includeJs(url, Callback.id(complete));
				}
			},

            /**
             * Renders screen to a file
             * @param filename
             * @return {*}
             */
			render: function(filename) {
				console.xdebug("WebPage.prototype.render(filename)");
				if (filename) {
					return this._render(filename)
				};
			},

            /**
             * Renders screen to base64 string
             * @param format
             * @return {*}
             */
			renderBase64: function(format) {
				console.xdebug("WebPage.prototype.renderBase64(format)");
				return this._renderBase64(format || "PNG");
			}
				
		}
    
    });

    // STATIC PROPERTIES

    // Currently running IE instance
    WebPage.current = null;
    
    // HashMap of instantiated pages
    WebPage.all = {};
    
    // Add static fireEvent() method for event handling
    WebPage.fireEvent = function(nickname, uuid, args) {
        console.xdebug("WebPage.fireEvent('" + nickname + "', uuid, args)");
        var page = WebPage.all[uuid]; 
        if (page && page.fireEvent) {
            return page.fireEvent(nickname, args);
        }
    };


})(this.trifle);

