/*
* trifle.core.js
*
* By: Steven de Salas
* On: Sep 2013
* 
*
* Generates the core running environment for
* javascript code to execute under
* 
*/

(function(GLOBAL) {

    // Save imported params
    var API = {
        trifle: GLOBAL.trifle,
        module: GLOBAL.module,
        console: GLOBAL.console,
        window: GLOBAL.window
    };

    delete GLOBAL.trifle;
    delete GLOBAL.module;
    delete GLOBAL.console;
    delete GLOBAL.window;

    // Initialise window object
    var window = GLOBAL.window = {
        API: API.window,
        window: window,
        setTimeout: function(callback, ms) {
            if (callback && ms) {
                var c = new trifle.Callback(function() {
                    callback.call(window);
                });
                window.API.SetTimeout(c.id, ms);
            }
        },
        clearTimeout: function(id) {
            if (typeof id === 'number') {
                window.API.ClearTimeout(id);
            }
        },
        setInterval: function(callback, ms) {
            if (callback && ms) {
                window.API.SetInterval((new trifle.Callback(function() {
                    callback.call(window);
                })).id, ms);
            }
        },
        clearInterval: function(id) {
            if (typeof id === 'number') {
                window.API.ClearInterval(id);
            }
        }
    };

    // Apply to global
    GLOBAL.window = window;
    GLOBAL.setTimeout = window.setTimeout;
    GLOBAL.setInterval = window.setInterval;
    GLOBAL.clearTimeout = window.clearTimeout;
    GLOBAL.clearInterval = window.clearInterval;

    // Initialise trifle
    var trifle = GLOBAL.trifle = {
        API: API.trifle,
        module: API.module,
        version: API.trifle.Version,
        libraryPath: API.trifle.LibraryPath,
        exit: function(code) {
            return trifle.API.Exit(code || 0);
        },
        wait: function(ms) {
            return trifle.API.Wait(ms || 0);
        },
        injectJs: function(filename) {
            return trifle.API.InjectJs(filename || '');
        },
        callbacks: {}
    };


    // Add PhantomJS Compatibility
    var phantom = GLOBAL.phantom = {
        version: trifle.version,
        exit: trifle.exit,
        injectJs: trifle.injectJs,
        libraryPath: trifle.libraryPath
    };

    // Callback Class
    // Define Constructor
    var Callback = trifle.Callback = function(func, scope, defaultArgs) {
        this.func = func;
        this.scope = scope;
        this.defaultArgs = defaultArgs;
        this.id = this.newUID();
        console.xdebug('new Callback#' + this.id + '(func, scope, defaultArgs)');
        trifle.callbacks[this.id] = this;
    };

    // Unique ID Generator
    Callback.prototype.newUID = function() {
        var s4 = function() {
            return Math.floor((1 + Math.random()) * 0x10000)
             .toString(16)
             .substring(1);
        };
        return s4() + s4();
    };

    // Execute callback
    Callback.prototype.execute = function() {
        console.xdebug('Callback#' + this.id + '.prototype.execute()');
        if (this.expired) {
            console.xdebug('Callback#' + this.id + ' has expired. Skip execution..');
        } else {
            this.func.apply(this.scope || this, arguments || this.defaultArgs)
        }
    }

    // Execute callback and delete reference
    Callback.prototype.executeOnce = function() {
        console.xdebug('Callback#' + this.id + '.prototype.executeOnce()');
        this.execute.apply(this, arguments);
        // Expire callback
        this.expired = true;
    }

    // Console
    var console = GLOBAL.console = {
        API: API.console,
        clear: function() {
            this.API.clear();
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
                        this.API[method]("");
                        break;
                    case 1:
                        if (typeof args[0] === 'function') { args[0] = args[0].toString(); }
                        this.API[method](args[0]);
                        break;
                    default:
                        var params = [];
                        for (var i = 0; i < args.length; i++) {
                            if (typeof args[i] === 'function') { args[i] = args[i].toString(); }
                            params[i] = args[i];
                        }
                        this.API[method](params);
                        break;
                }
            }
        }
    };


    // Loading module framework
    // @see http://wiki.commonjs.org/wiki/Modules/1.1.1
    var exports = GLOBAL.exports = {
        webpage: function() {
            return new trifle.modules.WebPage();
        },
        fs: function() {
            return new trifle.modules.FileSystem();
        },
        system: function() {
            return new trifle.modules.System();
        }
    };

    // Defines require() method
    var require = GLOBAL.require = function(name) {

        if (!exports[name]) {
            console.error('require() -- Invalid module: ' + name);
            return;
        }

        var module = exports[name]();
        module.create = function() {
            return this;
        }

        return module;
    }


})(this);


