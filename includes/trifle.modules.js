
// Initialise Namespace
this.trifle = this.trifle || {};

// Wrap code to avoid global vars
(function(trifle) {

    // Define namespace
    trifle.modules = trifle.modules || {};

    // WebPage Class
    // Define Constructor
    var WebPage = trifle.modules.WebPage = function() {
        console.xdebug("new WebPage()");
        // Instantiate a V8 WebPage object and stores it in internal API property
        this.API = trifle.module['WebPage']();
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
        // Instantiate Callback
        var complete = function(status) {
            // Fire LoadFinished event
            if (page.onLoadFinished) {
                page.onLoadFinished.call(page, status);
            }
            return callback.call(page, status);
        };
        return this.API.Open(url, (new trifle.Callback(complete)).id);
    };

    // Evaluate JS
    WebPage.prototype.evaluateJavaScript = function(code) {
        console.xdebug("WebPage.prototype.evaluateJavaScript(code)");
        if (code && code.length) {
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
            return this.API.Evaluate(func.toString(), args);
        }
        return null;
    };

    // Inject JS file
    WebPage.prototype.injectJs = function(filename) {
        console.xdebug("WebPage.prototype.injectJs(filename)");
        if (typeof filename === 'string') {
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
            return this.API.IncludeJs(url, (new trifle.Callback(complete)).id);
        }
    }


    // Render File
    WebPage.prototype.render = function(filename) {
        console.xdebug("WebPage.prototype.render(filename)");
        if (filename) {
            return this.API.Render(filename)
        };
    }

    // Render to Base64 string
    WebPage.prototype.renderBase64 = function(format) {
        console.xdebug("WebPage.prototype.renderBase64(format)");
        return this.API.RenderBase64(format || "PNG");
    }

    // FileSystem Class
    // Define Constructor
    var FileSystem = trifle.modules.FileSystem = function() {
        console.xdebug("new FileSystem()");
        // Instantiate a V8 FileSystem object and stores it in internal API property
        this.API = trifle.module['FileSystem']();
        // Set the working directory
        this.workingDirectory = this.API.WorkingDirectory;
    }

    // Changes the current workingDirectory to the specified path.
    FileSystem.prototype.changeWorkingDirectory = function(path) {
        console.xdebug("FileSystem.prototype.changeWorkingDirectory(path)");
        return this.API.ChangeWorkingDirectory(path || '');
    }

    // System Class
    // Define Constructor
    var System = trifle.modules.System = function() {
        console.xdebug("new System()");
        // Instantiate a V8 System object and stores it in internal API property
        this.API = trifle.module['System']();
        // Populate other properties
        this.args = this.API.args;
    }


})(this.trifle);

