
// Initialise Namespace
this.triflejs = this.triflejs || {};

// Wrap code to avoid global vars
(function(triflejs) {

    // Define namespace
    triflejs.modules = triflejs.modules || {};

    // WebPage Class
    // Define Constructor
    var WebPage = triflejs.modules.WebPage = function() {
        console.xdebug("new WebPage()");
        // Instantiate a V8 WebPage object and stores it in internal module property
        this.module = triflejs.module['WebPage']();
        // Fire Initialized event
        if (this.onInitialized) {
            page.onInitialized.call(this);
        }
    };

    // Open URL
    WebPage.prototype.open = function(url, callback) {
        console.xdebug("WebPage.prototype.open(url, callback)");
        var page = this;
        // Fire LoadStarted event
        if (this.onLoadStarted) {
            page.onLoadStarted.call(this);
        }
        return this.module.Open(url, (new triflejs.Callback(function(status) {
            // Fire LoadFinished event
            if (page.onLoadFinished) {
                page.onLoadFinished.call(page, status);
            }
            return callback.call(page, status);
        })).id);
    };

    // Evaluate JS
    WebPage.prototype.evaluateJavaScript = function(code) {
        console.xdebug("WebPage.prototype.evaluateJavaScript(code)");
        if (code && code.length) {
            return this.module.EvaluateJavaScript(code);
        }
    };

    // Evaluate Function
    WebPage.prototype.evaluate = function(func) {
        console.xdebug("WebPage.prototype.evaluate(func)");
        if (typeof func === 'function') {
            var args = [];
            for (var i = 1; i < arguments.length; i++) {
                args.push(arguments[i]);
            }
            return this.module.Evaluate(func.toString(), args);
        }
        return null;
    };

    // Inject JS file
    WebPage.prototype.injectJs = function(filename) {
        console.xdebug("WebPage.prototype.injectJs(filename)");
        if (typeof filename === 'string') {
            return this.module.InjectJs(filename);
        }
    }

    // Include remote JS
    WebPage.prototype.includeJs = function(url, callback) {
        console.xdebug("WebPage.prototype.includeJs(url, callback)");
        var page = this;
        if (typeof url === 'string') {
            return this.module.IncludeJs(url, (new triflejs.Callback(function() {
                if (callback && callback.call) {
                    callback.call(page);
                }
            })).id);
        }
    }


    // Render File
    WebPage.prototype.render = function(filename) {
        console.xdebug("WebPage.prototype.render(filename)");
        if (filename) {
            return this.module.Render(filename)
        };
    }

    // Render to Base64 string
    WebPage.prototype.renderBase64 = function(format) {
        console.xdebug("WebPage.prototype.renderBase64(format)");
        return this.module.RenderBase64(format || "PNG");
    }

    // FileSystem Class
    // Define Constructor
    var FileSystem = triflejs.modules.FileSystem = function() {
        console.xdebug("new FileSystem()");
        // Instantiate a V8 FileSystem object and stores it in internal module property
        this.module = triflejs.module['FileSystem']();
    }

    // System Class
    // Define Constructor
    var System = triflejs.modules.System = function() {
        console.xdebug("new System()");
        // Instantiate a V8 System object and stores it in internal module property
        this.module = triflejs.module['System']();
        // Populate other properties
        this.args = this.module.args;
    }


})(this.triflejs);

