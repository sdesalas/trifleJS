
// Initialise Namespace
this.triflejs = this.triflejs || {};

// Wrap code to avoid global vars
(function(triflejs) {

    // Define namespace
    modules = triflejs.modules = triflejs.modules || {};

    // WebPage Class
    // Define Constructor
    var WebPage = triflejs.modules.WebPage = function() {
        console.debug("new WebPage()");
        // Instantiate a C# webpage and store it in _interop property
        this._interop = triflejs._interop['WebPage']();
        // Fire Initialized event
        if (this.onInitialized) {
            page.onInitialized.call(this);
        }
    };

    // Open URL
    WebPage.prototype.open = function(url, callback) {
        console.debug("WebPage.prototype.open(url, callback)");
        var page = this;
        // Fire LoadStarted event
        if (this.onLoadStarted) {
            page.onLoadStarted.call(this);
        }
        return this._interop.Open(url, (new triflejs.Callback(function(status) {
            // Fire LoadFinished event
            if (page.onLoadFinished) {
                page.onLoadFinished.call(page, status);
            }
            return callback.call(page, status);
        })).id);
    };

    // Evaluate JS
    WebPage.prototype.evaluateJavaScript = function(code) {
        console.debug("WebPage.prototype.evaluateJavaScript(code)");
        if (code && code.length) {
            return this._interop.EvaluateJavaScript(code);
        }
    };

    // Evaluate Function
    WebPage.prototype.evaluate = function(func) {
        console.debug("WebPage.prototype.evaluate(func)", arguments);
        if (typeof func === 'function') {
            var args = [];
            for (var i = 1; i < arguments.length; i++) {
                args.push(arguments[i]);
            }
            console.log('arguments... ', args);
            return this._interop.Evaluate(func.toString(), args);
        }
        return null;
    };

    // Inject JS file
    WebPage.prototype.injectJs = function(filename) {
        console.debug("WebPage.prototype.injectJs(filename)", arguments);
        if (typeof filename === 'string') {
            return this._interop.InjectJs(filename);
        }
    }

    // Include remote JS
    WebPage.prototype.includeJs = function(url, callback) {
        console.debug("WebPage.prototype.includeJs(url, callback)", arguments, typeof url);
        var page = this;
        if (typeof url === 'string') {
            return this._interop.IncludeJs(url, (new triflejs.Callback(function() {
                if (callback && callback.call) {
                    callback.call(page);
                }
            })).id);
        }
    }


    // Render File
    WebPage.prototype.render = function(filename) {
        console.debug("WebPage.prototype.render(filename)");
        if (filename) {
            return this._interop.Render(filename)
        };
    }

    // Render to Base64 string
    WebPage.prototype.renderBase64 = function(format) {
        console.debug("WebPage.prototype.renderBase64(format)");
        return this._interop.RenderBase64(format || "PNG");
    }

    // FileSystem Class
    // Define Constructor
    triflejs.FileSystem = function() {
        console.debug("new FileSystem()");
        // Instantiate a C# webpage and store it in _interop property
        this._interop = triflejs._interop['FileSystem']();
    }

})(this.triflejs);

