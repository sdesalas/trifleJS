
(function(GLOBAL) {

    // Initialise window
    var window = {
        host: GLOBAL.window,
        setTimeout: function(callback, ms) {
            if (callback && ms) {
                this.host.setTimeout((new triflejs.Callback(function() {
                    callback.call(window);
                })).id, ms);
            }
        },
        setTimeout: function(callback, ms) {
            if (callback && ms) {
                this.host.setInterval((new triflejs.Callback(function() {
                    callback.call(window);
                })).id, ms);
            }
        }
    };

    GLOBAL.window = window;

    // Initialise trifleJS
    var triflejs = GLOBAL.triflejs = GLOBAL.triflejs || {};

    // Add UID generation
    triflejs.uid = function() {
        var s4 = function() {
            return Math.floor((1 + Math.random()) * 0x10000)
             .toString(16)
             .substring(1);
        };

        return s4() + s4();
    };

    // Set interop inside trifle
    triflejs._interop = GLOBAL.interop;
    delete GLOBAL.interop;

    // Initialize callback hashmap
    triflejs.callbacks = {};

    // Callback Class
    // Define Constructor
    var Callback = triflejs.Callback = function(func, scope, defaultArgs) {
        this.func = func;
        this.scope = scope;
        this.defaultArgs = defaultArgs;
        this.id = triflejs.uid();
        console.xdebug('new Callback#' + this.id + '(func, scope, defaultArgs)');
        triflejs.callbacks[this.id] = this;
    };

    // Execute callback
    Callback.prototype.execute = function() {
        console.xdebug('Callback#' + this.id + '.prototype.execute()');
        this.func.apply(this.scope || this, arguments || this.defaultArgs)
    }

    // Execute callback and delete reference
    Callback.prototype.executeOnce = function() {
        this.execute.apply(this, arguments);
        // Remove when finished
        delete triflejs.callbacks[this.id];
    }

    // PhantomJS Compatibility
    this.phantom = this.phantom || {};
    phantom.version = triflejs.version;
    phantom.exit = triflejs.exit;

    // Console
    this.console = {
        host: console, // Import host object
        clear: function() {
            this.host.clear();
        },
        log: function() {
            this._do('log', arguments);
        },
        error: function() {
            this._do('error', arguments);
        },
        xdebug: function() {
            this._do('xdebug', arguments);
        },
        debug: function() {
            this._do('debug', arguments);
        },
        warn: function() {
            this._do('warn', arguments);
        },
        _do: function(method, args) {
            if (method) {
                switch (args.length) {
                    case 0:
                        this.host[method]("");
                        break;
                    case 1:
                        this.host[method](args[0]);
                        break;
                    default:
                        var params = [];
                        for (var i = 0; i < args.length; i++) {
                            params[i] = args[i];
                        }
                        this.host[method](params);
                        break;
                }
            }
        }
    };

})(this);


