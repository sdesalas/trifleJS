
(function(GLOBAL) {

    // Initialise window
    var window = {
        interop: GLOBAL.window,
        setTimeout: function(callback, ms) {
            if (callback && ms) {
                var c = new triflejs.Callback(function() {
                    callback.call(window);
                });
                this.interop.setTimeout(c.id, ms);
            }
        },
        setInterval: function(callback, ms) {
            if (callback && ms) {
                this.interop.setInterval((new triflejs.Callback(function() {
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
    triflejs.module = GLOBAL.module;
    delete GLOBAL.module;

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
        interop: console, // Import host object
        clear: function() {
            this.interop.clear();
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
                        this.interop[method]("");
                        break;
                    case 1:
                        if (typeof args[0] === 'function') { args[0] = args[0].toString(); }
                        this.interop[method](args[0]);
                        break;
                    default:
                        var params = [];
                        for (var i = 0; i < args.length; i++) {
                            if (typeof args[i] === 'function') { args[i] = args[i].toString(); }
                            params[i] = args[i];
                        }
                        this.interop[method](params);
                        break;
                }
            }
        }
    };

})(this);


